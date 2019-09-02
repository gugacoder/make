@rem
@rem Script de compilacao do projeto.
@rem
@echo off

echo.================================================================================
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

cd %current%\pack
make-dist.bat
if %errorlevel% = 1 goto :error

cd %current%\make
make-dist.bat

pause
