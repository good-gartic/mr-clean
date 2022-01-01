#!/usr/bin/env bash

# Set this to your values so you don't have to supply `docker` with -e parameters every time

# You can find the token on https://discord.com/developers/applications after selecting Mr. Clean under Good Gartic Discord developer team
DISCORD_TOKEN=""

# Click "Copy ID" after right clicking the GG guild and paste the copied value here
DISCORD_GUILD=""

# ----------------------------------------------------------------------------------------------------------
# The rest is just a bit of Docker magic, read it if you want, however no further modifications are required

# Setup docker volume for persisting the H2 database (https://docs.docker.com/storage/volumes/)
# Only create the volume if it doesn't exist already, as that would erase the configured filters
if [ ! "$(docker volume ls -q -f name=mr-clean-data)" ]; then
  docker volume create mr-clean-data
fi

echo $DISCORD_TOKEN

# Mount the data volume and run the docker container with configured parameters
docker build -t mr-clean .
docker run --mount source=mr-clean-data,target=/app/data \
  -e DATABASE_URL=jdbc:h2:file:/app/data \
  -e DISCORD_TOKEN="$DISCORD_TOKEN" \
  -e DISCORD_GUILD="$DISCORD_GUILD" \
  -t mr-clean