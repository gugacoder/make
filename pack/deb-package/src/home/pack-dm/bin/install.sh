#!/bin/sh
#
# Script para instalação do PackDm.
#
# Este script pode ser executado depois da instalação do PackDm para inicializar
# as pastas e os serviços ou a qualquer momento para corrigir uma instalação
# quebrada.
#
# Quando instalando via pacote debian este script é executado automaticamente.
#

user=pack-dm
home=/home/$user

#
# Links locais
#
rm -f $home/www/browser
ln -s $home/.pack/repository/ $home/www/browser

#
# Links do Apache2
#
rm -f /etc/apache2/sites-available/pack-dm
rm -f /etc/apache2/sites-available/pack-dm.conf
ln -s $home/conf/apache2.conf /etc/apache2/sites-available/pack-dm
ln -s $home/conf/apache2.conf /etc/apache2/sites-available/pack-dm.conf

#
# Links do serviço
#
rm -f /etc/init.d/pack-dm
ln -s $home/conf/init-d.sh /etc/init.d/pack-dm

#
# Corrigindo permissões
#
sudo chown $user:www-data -R $home || true

#
# Inicializando serviços
#
sudo a2ensite pack-dm

sudo /etc/init.d/apache2 reload
sudo /etc/init.d/pack-dm start
