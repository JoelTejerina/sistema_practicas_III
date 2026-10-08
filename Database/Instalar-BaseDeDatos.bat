@echo off
REM Crea/actualiza la base SQL Server del sistema. Doble clic o con parametros:
REM   Instalar-BaseDeDatos.bat -Demo
REM   Instalar-BaseDeDatos.bat -Servidor "localhost\SQLEXPRESS" -BaseDeDatos SistemaPracticas
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Instalar-BaseDeDatos.ps1" %*
echo.
pause
