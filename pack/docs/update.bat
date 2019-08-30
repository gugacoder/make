@echo off
rem Script de geração da documentação do PackDm baseado no README.md

pandoc ..\README.md -s -H style.css --toc --metadata pagetitle="PackDm" > index.html

pause
