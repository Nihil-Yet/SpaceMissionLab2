CREATE TABLE "Missions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Missions" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Budget" INTEGER NOT NULL,
    "Duration" INTEGER NOT NULL,
    "MissionType" INTEGER NOT NULL
);
CREATE TABLE sqlite_sequence(name,seq);
CREATE TABLE "OrbitalMissions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrbitalMissions" PRIMARY KEY,
    "CurrHeight" REAL NOT NULL,
    "TargetHeight" REAL NOT NULL,
    "Inclination" REAL NOT NULL,
    "EnergySource" INTEGER NOT NULL,
    CONSTRAINT "FK_OrbitalMissions_Missions_Id" FOREIGN KEY ("Id") REFERENCES "Missions" ("Id") ON DELETE CASCADE
);
CREATE TABLE "PlanetaryMissions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PlanetaryMissions" PRIMARY KEY,
    "Planet" TEXT NOT NULL,
    "AtmoDensity" INTEGER NOT NULL,
    "LandingPointName" TEXT NOT NULL,
    "LandingPointX" INTEGER NOT NULL,
    "LandingPointY" INTEGER NOT NULL,
    "LandingPointR" INTEGER NOT NULL,
    CONSTRAINT "FK_PlanetaryMissions_Missions_Id" FOREIGN KEY ("Id") REFERENCES "Missions" ("Id") ON DELETE CASCADE
);
