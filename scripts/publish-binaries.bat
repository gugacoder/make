@rem
@rem Script de compilacao do projeto.
@rem
@echo off

:main
  set error=0
  
  rem Versao para: win-x64
  set cmd=dotnet publish -c Release -f net47 -r win-x64 /p:PublishSingleFile=true /p:PublishReadyToRun=true /p:PublishTrimmed=true
  echo.
  echo.[CMD]%cmd%
  %cmd%
  if errorlevel 1 set error=1
  
  rem Comentado por ora. O DotNet Core 3 ainda não produz um arquivo executavel para linux enxuto.
  rem // rem Versao para: linux-x64
  rem // set cmd=dotnet publish -c Release -f netcoreapp3 -r linux-x64 /p:PublishSingleFile=true
  rem // echo.
  rem // echo.[CMD]%cmd%
  rem // %cmd%
  rem // if errorlevel 1 set error=1
  rem // 
  rem // if %error% == 1 goto :error
  rem // goto :ok

:ok
  echo.
  echo.[OK]Sucesso.
  echo.
  exit /b 0

:error
  echo.
  echo.[ERR] A compilacao falhou.
  echo.[ERR] Veja erros anteriores para entender a causa.
  echo.
  exit /b 1
