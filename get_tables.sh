#!/bin/sh

sqlite3 missions.db <<'EOF'
.table
SELECT * FROM Missions;
SELECT * FROM OrbitalMissions;
SELECT * FROM PlanetaryMissions;
EOF


