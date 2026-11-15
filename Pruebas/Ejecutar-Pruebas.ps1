<#
.SYNOPSIS
    Ejecuta las pruebas unitarias del sistema (Pruebas\Pruebas.csproj) de forma automatica y repetible.

.DESCRIPTION
    1. Compila el proyecto de pruebas (y las tres capas) con MSBuild.
    2. Crea una base DESCARTABLE (por defecto SistemaPracticas_Prueba) con el instalador y los datos demo.
    3. Ejecuta Pruebas.exe apuntando a esa base mediante SISTEMA_DB_CONNECTION (nunca toca SistemaPracticas).
    4. Borra la base descartable y deja conexion.local.txt como estaba.
    Codigo de salida: 0 = todas las pruebas OK; distinto de 0 = fallas o no se pudo ejecutar.

.PARAMETER Servidor
    Instancia de SQL Server (por defecto localhost).

.PARAMETER SinBd
    Ejecuta solo las pruebas de logica pura (no crea ninguna base).

.EXAMPLE
    .\Pruebas\Ejecutar-Pruebas.ps1 -Servidor localhost
#>
param(
    [string]$Servidor = 'localhost',
    [string]$BaseDeDatos = 'SistemaPracticas_Prueba',
    [string]$MSBuild = '',
    [switch]$SinBd
)

$ErrorActionPreference = 'Stop'
if ($BaseDeDatos -notmatch 'Prueba|Test') { throw "Por seguridad la base descartable debe llevar 'Prueba' o 'Test' en el nombre." }
$raiz = Split-Path -Parent $PSScriptRoot
$conexionLocal = Join-Path $raiz 'conexion.local.txt'

if (-not $MSBuild) {
    $candidatos = @(
        "$env:ProgramFiles\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    )
    $MSBuild = $candidatos | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $MSBuild) { throw 'No se encontro MSBuild.exe. Pasalo con -MSBuild "<ruta>".' }
}

Write-Host '== Compilando =='
& $MSBuild (Join-Path $PSScriptRoot 'Pruebas.csproj') /nologo /v:q /p:Configuration=Debug
if ($LASTEXITCODE -ne 0) { throw 'La compilacion fallo.' }

$exe = Join-Path $PSScriptRoot 'bin\Debug\Pruebas.exe'
$cadena = "Data Source=$Servidor;Initial Catalog=$BaseDeDatos;Integrated Security=True;Connect Timeout=15"
$respaldo = $null
if (Test-Path $conexionLocal) { $respaldo = Get-Content $conexionLocal -Raw }

try {
    if ($SinBd) {
        Write-Host '== Ejecutando pruebas (sin base de datos) =='
        & $exe --sin-bd
    }
    else {
        Write-Host "== Creando la base descartable $BaseDeDatos =="
        $instalador = Join-Path $raiz 'Database\Instalar-BaseDeDatos.ps1'
        # El instalador pide escribir SI antes de recrear la base: se le responde por la entrada estandar.
        cmd /c "echo SI| powershell -NoProfile -ExecutionPolicy Bypass -File `"$instalador`" -Servidor $Servidor -BaseDeDatos $BaseDeDatos -Demo -Recrear" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'No se pudo crear la base descartable.' }
        $env:SISTEMA_DB_CONNECTION = $cadena
        Write-Host '== Ejecutando pruebas =='
        & $exe
    }
    $codigo = $LASTEXITCODE
}
finally {
    Remove-Item Env:\SISTEMA_DB_CONNECTION -ErrorAction SilentlyContinue
    if (-not $SinBd) {
        try {
            $cn = New-Object System.Data.SqlClient.SqlConnection "Data Source=$Servidor;Initial Catalog=master;Integrated Security=True"
            $cn.Open()
            $cmd = $cn.CreateCommand()
            $cmd.CommandText = "IF DB_ID(N'$BaseDeDatos') IS NOT NULL BEGIN ALTER DATABASE [$BaseDeDatos] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$BaseDeDatos]; END"
            [void]$cmd.ExecuteNonQuery(); $cn.Close()
            Write-Host "Base descartable $BaseDeDatos eliminada."
        } catch { Write-Warning "No se pudo borrar $BaseDeDatos : $($_.Exception.Message)" }
    }
    if ($respaldo -ne $null) { Set-Content -Path $conexionLocal -Value $respaldo -NoNewline }
}
exit $codigo
