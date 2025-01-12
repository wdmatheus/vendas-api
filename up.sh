#!/usr/bin/env bash

docker compose up

trap 'echo "Script encerrado"; docker compose down' EXIT