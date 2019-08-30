@rem
@rem Baixa a versão mais recente do nuget.exe
@rem
@rem Faça o commit do binário no repositório depois de baixá-lo.
@rem

set PATH=%PATH%;../Wget
wget -O nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
pause
