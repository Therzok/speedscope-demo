#!/bin/zsh
set -x

# $1 will contain the file name to start.

SPEEDSCOPE=$(which speedscope)
TRACE_FILE="traces/$1.nettrace"
OUTPUT_SPEEDSCOPE_FILE="traces/$1.json"
INPUT_SPEEDSCOPE_FILE="traces/$1.speedscope.json"
BIN_FILE="./bin/net7.0/$1"

if [ ! -f "$SPEEDSCOPE" ]; then
  echo "Install speedscope via npm install -g speedscope"
fi

if [ -f "$BIN_FILE" ]; then
  # Launch this in the background with the runtime suspended.
  DOTNET_DefaultDiagnosticPortSuspend=1 "$BIN_FILE" > output &
  PID="$!"

  # Start program execution and profiling as soon it launches
  dotnet trace collect --process-id "$PID" --output "$TRACE_FILE" --resume-runtime

  # Convert the format to speedscope
  dotnet trace convert --format speedscope --output "$OUTPUT_SPEEDSCOPE_FILE" "$TRACE_FILE"

  # Kill the program, we don't care at this point.
  kill "$PID"

  speedscope "$INPUT_SPEEDSCOPE_FILE"
  cat output
else
  echo "$BIN_FILE does not exist. ./profile.sh '$1', expected something like 'alloc', 'cpu-background', 'cpu-wait', 'cpu-spin'"
fi
