#!/bin/sh
#
# Script para desinstalação do PackDm.
#

user=pack-dm
home=/home/$user

#
# Interrompendo serviços
#
sudo a2dissite pack-dm || true
sudo /etc/init.d/apache2 reload || true
sudo /etc/init.d/pack-dm stop || true

#
# Links do Apache2
#
rm -f /etc/apache2/sites-available/pack-dm
rm -f /etc/apache2/sites-available/pack-dm.conf

#
# Links do serviço
#
rm -f /etc/init.d/pack-dm

#
# Links locais
#
rm -f $home/www/browser
