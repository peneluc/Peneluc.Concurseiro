CREATE TABLE IF NOT EXISTS Concurseiro (
  id uuid PRIMARY KEY,
  device_id text NOT NULL,
  latitude double precision NOT NULL,
  longitude double precision NOT NULL,
  speed double precision NULL,
  accuracy double precision NULL,
  battery_level integer NULL,
  timestamp timestamptz NOT NULL
);