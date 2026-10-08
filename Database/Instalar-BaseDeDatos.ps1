<#
.SYNOPSIS
    Crea y/o actualiza la base de datos SQL Server del Sistema de Practicas.

.DESCRIPTION
    Lo corre CADA desarrollador contra su propio SQL Server (o contra el servidor
    compartido, si el equipo decide usar uno). Es seguro correrlo todas las veces
    que haga falta:
      1. Crea la base si no existe.
      2. Aplica las migraciones nuevas de .\migraciones (en orden, una sola vez cada
         una; queda registro en la tabla dbo.SchemaVersion).
      3. Carga los datos iniciales de .\datos\datos_iniciales.sql (idempotente).
      4. Con -Demo, carga tambien .\datos\datos_demo.sql.
      5. Crea conexion.local.txt en la raiz del repo (ignorado por git) con la
         cadena de conexion que usa la aplicacion.

    No requiere sqlcmd ni modulos extra: usa System.Data.SqlClient de Windows.

.PARAMETER Servidor
    Instancia de SQL Server. Por defecto .\SQLEXPRESS (SQL Server Express local).

.PARAMETER BaseDeDatos
    Nombre de la base. Por defecto SistemaPracticas.

.PARAMETER Usuario / Clave
    Para autenticacion SQL. Si se omiten se usa la autenticacion de Windows.

.PARAMETER Demo
    Carga ademas los datos de ejemplo (proveedores, productos, stock, carta).

.PARAMETER Recrear
    BORRA la base y la vuelve a crear desde cero (pide confirmacion).

.EXAMPLE
    .\Instalar-BaseDeDatos.ps1 -Demo
.EXAMPLE
    .\Instalar-BaseDeDatos.ps1 -Servidor "localhost\SQLEXPRESS" -BaseDeDatos SistemaPracticas_Joel
#>
[CmdletBinding()]
param(
    [string]$Servidor = '.\SQLEXPRESS',
    [string]$BaseDeDatos = 'SistemaPracticas',
    [string]$Usuario,
    [string]$Clave,
    [switch]$Demo,
    [switch]$Recrear
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

if ($BaseDeDatos -notmatch '^[A-Za-z0-9_]+$') {
    throw "Nombre de base invalido '$BaseDeDatos'. Use solo letras, numeros y guion bajo."
}

$raizDatabase = $PSScriptRoot
$raizRepo     = Split-Path -Parent $raizDatabase

function New-CadenaConexion([string]$catalogo) {
    $b = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
    $b['Data Source']     = $Servidor
    $b['Initial Catalog'] = $catalogo
    $b['Connect Timeout'] = 15
    if ($Usuario) {
        $b['User ID']  = $Usuario
        $b['Password'] = $Clave
    } else {
        $b['Integrated Security'] = $true
    }
    return $b.ConnectionString
}

function Open-Conexion([string]$catalogo) {
    $c = New-Object System.Data.SqlClient.SqlConnection (New-CadenaConexion $catalogo)
    $c.Open()
    return $c
}

# Ejecuta un script T-SQL partiendolo en lotes por las lineas "GO".
function Invoke-Script($conexion, [string]$texto, $transaccion) {
    $lotes = [regex]::Split($texto, '(?im)^[ \t]*GO[ \t]*\r?$')
    foreach ($lote in $lotes) {
        if ([string]::IsNullOrWhiteSpace($lote)) { continue }
        $cmd = $conexion.CreateCommand()
        $cmd.CommandText    = $lote
        $cmd.CommandTimeout = 120
        if ($transaccion) { $cmd.Transaction = $transaccion }
        [void]$cmd.ExecuteNonQuery()
    }
}

function Read-Sql([string]$ruta) {
    return [System.IO.File]::ReadAllText($ruta, [System.Text.Encoding]::UTF8)
}

Write-Host "Servidor: $Servidor   Base: $BaseDeDatos" -ForegroundColor Cyan

# ---------------------------------------------------------------- 1. crear base
$master = Open-Conexion 'master'
try {
    if ($Recrear) {
        $r = Read-Host "Esto BORRA la base '$BaseDeDatos' y todos sus datos. Escriba SI para continuar"
        if ($r -ne 'SI') { Write-Host 'Cancelado.'; return }
        $cmd = $master.CreateCommand()
        $cmd.CommandText = "IF DB_ID(@n) IS NOT NULL BEGIN " +
            "DECLARE @sql NVARCHAR(MAX) = N'ALTER DATABASE ' + QUOTENAME(@n) + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ' + QUOTENAME(@n) + N';'; " +
            "EXEC (@sql); END"
        [void]$cmd.Parameters.AddWithValue('@n', $BaseDeDatos)
        [void]$cmd.ExecuteNonQuery()
        Write-Host 'Base eliminada.'
    }

    $cmd = $master.CreateCommand()
    $cmd.CommandText = "IF DB_ID(@n) IS NULL BEGIN " +
        "DECLARE @sql NVARCHAR(MAX) = N'CREATE DATABASE ' + QUOTENAME(@n) + N' COLLATE Latin1_General_100_CI_AI'; " +
        "EXEC (@sql); SELECT 1; END ELSE SELECT 0;"
    [void]$cmd.Parameters.AddWithValue('@n', $BaseDeDatos)
    $creada = [int]$cmd.ExecuteScalar()
    if ($creada -eq 1) { Write-Host "Base '$BaseDeDatos' creada." -ForegroundColor Green }
    else               { Write-Host "La base '$BaseDeDatos' ya existia." }
}
finally { $master.Dispose() }

# ------------------------------------------------------- 2. migraciones nuevas
$db = Open-Conexion $BaseDeDatos
try {
    $sqlVersion = @"
IF OBJECT_ID(N'dbo.SchemaVersion') IS NULL
CREATE TABLE dbo.SchemaVersion (
    Script      NVARCHAR(200) NOT NULL CONSTRAINT PK_SchemaVersion PRIMARY KEY,
    AplicadoEl  DATETIME2(0)  NOT NULL CONSTRAINT DF_SchemaVersion_Fecha DEFAULT (SYSDATETIME()),
    AplicadoPor NVARCHAR(128) NOT NULL CONSTRAINT DF_SchemaVersion_Quien DEFAULT (SUSER_SNAME())
);
"@
    Invoke-Script $db $sqlVersion $null

    $aplicadas = New-Object 'System.Collections.Generic.HashSet[string]'
    $cmd = $db.CreateCommand()
    $cmd.CommandText = 'SELECT Script FROM dbo.SchemaVersion'
    $rd = $cmd.ExecuteReader()
    while ($rd.Read()) { [void]$aplicadas.Add($rd.GetString(0)) }
    $rd.Close()

    $migraciones = Get-ChildItem -Path (Join-Path $raizDatabase 'migraciones') -Filter '*.sql' | Sort-Object Name
    $nuevas = 0
    foreach ($m in $migraciones) {
        if ($aplicadas.Contains($m.Name)) { continue }
        Write-Host "  Aplicando migracion $($m.Name) ..." -NoNewline
        $tx = $db.BeginTransaction()
        try {
            Invoke-Script $db (Read-Sql $m.FullName) $tx
            $cmd = $db.CreateCommand()
            $cmd.Transaction = $tx
            $cmd.CommandText = 'INSERT INTO dbo.SchemaVersion (Script) VALUES (@s)'
            [void]$cmd.Parameters.AddWithValue('@s', $m.Name)
            [void]$cmd.ExecuteNonQuery()
            $tx.Commit()
            Write-Host ' OK' -ForegroundColor Green
            $nuevas++
        } catch {
            $tx.Rollback()
            Write-Host ' ERROR' -ForegroundColor Red
            throw "Fallo la migracion $($m.Name): $($_.Exception.Message)"
        }
    }
    if ($nuevas -eq 0) { Write-Host '  Esquema al dia (no hay migraciones nuevas).' }

    # ------------------------------------------------------------ 3. datos
    Write-Host '  Cargando datos iniciales ...' -NoNewline
    Invoke-Script $db (Read-Sql (Join-Path $raizDatabase 'datos\datos_iniciales.sql')) $null
    Write-Host ' OK' -ForegroundColor Green

    if ($Demo) {
        Write-Host '  Cargando datos de demostracion ...' -NoNewline
        Invoke-Script $db (Read-Sql (Join-Path $raizDatabase 'datos\datos_demo.sql')) $null
        Write-Host ' OK' -ForegroundColor Green
    }
}
finally { $db.Dispose() }

# --------------------------------------- 4. conexion.local.txt para la aplicacion
$archivoLocal = Join-Path $raizRepo 'conexion.local.txt'
$cadena = New-CadenaConexion $BaseDeDatos
if (-not (Test-Path $archivoLocal)) {
    [System.IO.File]::WriteAllText($archivoLocal, $cadena + [Environment]::NewLine, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "Se creo $archivoLocal (no se sube a git)." -ForegroundColor Green
} else {
    Write-Host "Ya existe $archivoLocal; no se modifico. Cadena para esta base:" -ForegroundColor Yellow
    Write-Host "  $cadena"
}

Write-Host ''
Write-Host 'Listo. Usuario de prueba: Edu / 123' -ForegroundColor Cyan
