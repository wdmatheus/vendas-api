#!/usr/bin/env bash

docker compose -f compose.yaml -f compose.arm64.yaml up

trap 'echo "Script encerrado"; docker compose -f compose.yaml -f compose.arm64.yaml down' EXIT