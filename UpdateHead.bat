@echo off
git fetch origin
git checkout main
git pull origin main
git log --oneline -5