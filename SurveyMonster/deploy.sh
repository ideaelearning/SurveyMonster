#!/bin/bash
set -e

cd /home/idearoot/SurveyMonster

echo ">>> Fetching latest code from GitHub..."
git fetch --all
git reset --hard origin/master

echo ">>> Rebuilding Docker containers..."
docker compose down
docker compose build --no-cache
docker compose up -d

echo ">>> Deploy completed! 2"
