#!/usr/bin/env bash

# Set this to your values so you don't have to supply `docker` with -e parameters every time

# You can find the token on https://discord.com/developers/applications after selecting Mr. Clean under Good Gartic Discord developer team
DISCORD_TOKEN=

# Click "Copy ID" after right clicking the GG guild and paste the copied value here
DISCORD_GUILD=

# ----------------------------------------------------------------------------------------------------------
# The rest is just a bit of Docker magic, read it if you want, however no further modifications are required

# TODO: Set up docker volume for persisting H2 database, pull / build the docker image and run the application