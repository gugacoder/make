#!/bin/bash
#
# Script de geração da documentação do PackDm baseado no README.md
#

pandoc ../README.md -s -H style.css --toc > index.html


