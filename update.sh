#!/bin/sh

cd /home/helloworld/api/HackSheffield9
git pull
curl localhost:2019/load -H "Content-Type: application/json" -d @caddy_config.json
chmod a+rwx ~/api/HackSheffield9/data.db
# dotnet /home/helloworld/api/HackSheffield9/bin/Release/net7.0/HackSheffield.dll &
# disown