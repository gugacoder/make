#!/bin/sh
#
# Script para reinicialização do serviço do PackDm.
#

sudo a2ensite pack-dm || true
sudo /etc/init.d/apache2 reload || true

sudo /etc/init.d/pack-dm stop || true
sudo /etc/init.d/pack-dm start || true
