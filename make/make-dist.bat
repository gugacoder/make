@rem
@rem Script de compilacao do projeto.
@rem
@echo off

:main
  set error=0
  
  rem win-x64
  set cmd=dotnet publish -c Release -f net47 -r win-x64 /p:PublishSingleFile=true /p:PublishReadyToRun=true /p:PublishTrimmed=true
  echo.
  echo.[CMD]%cmd%
  %cmd%
  if errorlevel 1 set error=1
  
  rem linux-x64
  set cmd=dotnet publish -c Release -f netcoreapp3 -r linux-x64 /p:PublishSingleFile=true
  echo.
  echo.[CMD]%cmd%
  %cmd%
  if errorlevel 1 set error=1
  
  if %error% == 1 goto :error
  goto :ok

:ok
  echo.
  echo.[OK]Sucesso.
  echo.[INFO]Faca o commit das alteracoes.
  echo.
  pause
  exit /b 0

:error
  echo.
  echo.[ERR] A compilacao falhou.
  echo.[ERR] Veja erros anteriores para entender a causa.
  echo.
  pause
  exit /b 1
  
main
