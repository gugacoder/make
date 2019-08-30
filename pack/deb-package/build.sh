#!/bin/sh
#
version=$(cat src/DEBIAN/control | grep "Version: " | cut -d" " -f2)
deb=pack-dm-${version}-all.deb

execute() {
  cmd=$*
  echo [exec]$cmd
  $cmd
}

echo "Gerando pacote deb ${version}:"

execute rm -rf src/home/pack-dm/www/updater/*

execute cp -r ../dist/pack-bootstrap.exe src/home/pack-dm/www/updater
execute cp -r ../dist/pack.exe src/home/pack-dm/www/updater
execute cp -r ../dist/pack.pdb src/home/pack-dm/www/updater

execute cp ../dist/pack-bootstrap.exe src/home/pack-dm/bin/pack
execute cp ../dist/pack.exe src/home/pack-dm/bin/pack
execute cp ../dist/pack.pdb src/home/pack-dm/bin/pack

execute chmod +x src/home/pack-dm/bin/pack

execute cp ../docs/index.html src/home/pack-dm/www/README.html

execute dpkg-deb --build src target/${deb}

execute svn add --force target/${deb}

echo ""
echo "Alterações:"
svn status target
echo ""
echo "Commite estas alterações com o comando:"
echo "  svn commit . -m'Pacote deb ${version} gerado para o aplicativo PackDm'"
echo ""

