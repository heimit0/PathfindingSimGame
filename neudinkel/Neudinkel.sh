#!/bin/sh
printf '\033c\033]0;%s\a' Neudinkel
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Neudinkel.x86_64" "$@"
