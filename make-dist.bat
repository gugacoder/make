@rem
@rem Script de compilacao do projeto.
@rem
@echo off

echo.================================================================================
echo.
echo.  Nota:
echo.
echo.  Eh comum o script falhar na primeira execucao.
echo.  Este eh um problema conhecido da versao DotNet Core 3 Preview.
echo.  
echo.  Execute o script uma vez ate o fim.
echo.  Caso ocorram falhas execute o script novamente.
echo.  A segunda execucao deve produzir os binarios corretamente.
echo.  
echo.  A versao final lancada em setembro de 2019 promete resolver este problema
echo.
echo.================================================================================

set current=%cd%
set scripts=%cd%\scripts

echo.
echo.// Empacotando `pack'...
echo.
cd %current%\pack
call "%scripts%\publish-binaries.bat"

echo.
echo.// Empacotando `make'...
echo.
cd %current%\make
call "%scripts%\publish-binaries.bat"

echo.
echo.// Destacando os binarios por sistema operacional...
echo.
if not exist "%current%\dist" mkdir "%current%\dist"
cd "%current%\dist"

if not exist pack\win-x64 mkdir pack\win-x64
copy /y "%current%\build\net47\win-x64\publish\pack.exe"               pack\win-x64
copy /y "%current%\build\net47\win-x64\publish\pack-bootstrap.exe"     pack\win-x64

if not exist make\win-x64 mkdir make\win-x64
copy /y "%current%\build\net47\win-x64\publish\make.exe"               make\win-x64
copy /y "%current%\build\net47\win-x64\publish\make-bootstrap.exe"     make\win-x64

rem // TODO: Binarios para linux ainda nao suportados

echo.
echo.Distribuicao gerada em:
echo.  %current%\dist

cd "%current%"
pause
