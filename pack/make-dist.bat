@echo off

set solution=PackDm.sln
set devenv.com=
set suffix=Common7\IDE\devenv.com

if exist "%ProgramFiles%\Microsoft Visual Studio\2019\Community\%suffix%" set devenv.com=%ProgramFiles%\Microsoft Visual Studio\2019\Community\%suffix%
if not "%devenv.com%" == "" goto :found

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\%suffix%" set devenv.com=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\%suffix%
if not "%devenv.com%" == "" goto :found

if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Community\%suffix%" set devenv.com=%ProgramFiles%\Microsoft Visual Studio\2017\Community\%suffix%
if not "%devenv.com%" == "" goto :found

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\%suffix%" set devenv.com=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\%suffix%
if not "%devenv.com%" == "" goto :found

if exist "%ProgramFiles%\Microsoft Visual Studio 13.0\%suffix%" set devenv.com=%ProgramFiles%\Microsoft Visual Studio 13.0\%suffix%
if not "%devenv.com%" == "" goto :found

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 13.0\%suffix%" set devenv.com=%ProgramFiles(x86)%\Microsoft Visual Studio 13.0\%suffix%
if not "%devenv.com%" == "" goto :found

goto :not_found

:found
  set cmd="%devenv.com%" %solution% /Build "Release|Any CPU"
  echo [CMD]%cmd%

  rem A compilação dupla é necessária:
  rem - A primeira tem o objetivo de gerar a DLL.
  rem - A segunda, tem o objetivo de embarcar a DLL recém gerado no binário.

  echo.
  echo.[INFO]Gerando a ferramenta...
  %cmd%
  if errorlevel 1 goto :error
  
  echo.
  echo.[OK]Ferramenta compilada.
  echo.[OK]Faca o commit das alteracoes.
  echo.
  pause
  exit /b 0

:not_found
  echo.
  echo.[WARN] Falhou a execucao do comando.
  echo.
  echo.[ERR] O comando devenv.com do Visual Studio 2017 nao foi detectado.
  echo.
  pause
  exit /b 1

:error
  echo.
  echo.[WARN] Falhou a execucao do comando.
  echo.
  echo.[ERR] Veja erros anteriores.
  echo.
  pause
  exit /b 1
