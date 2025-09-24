#!/bin/bash

mongod --replSet rs0 --fork --syslog --bind_ip_all --journal

mongosh --eval "rs.initiate()"

sleep 2

mongo <<EOF
   use admin;
   admin = db.getSiblingDB("admin");
   admin.createUser(
     {
	user: "${MONGO_INITDB_ROOT_USERNAME}",
        pwd: "${MONGO_INITDB_ROOT_PASSWORD}",
        roles: [ { role: "root", db: "admin" } ]
     });
     db.getSiblingDB("admin").auth("${MONGO_INITDB_ROOT_USERNAME}", "${MONGO_INITDB_ROOT_PASSWORD}");
     rs.status();
EOF

mongosh --eval "while(true) {if (rs.status().ok) break;sleep(1000)};"
sleep inf