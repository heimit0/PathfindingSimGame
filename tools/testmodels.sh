#!/bin/bash

if [[ $# -ne 1 ]]; then
	echo "Benutzung:"
	echo "    $0 <Anzahl Variationen>"
	exit 1
fi

if [[ ! -d "neudinkel/assets/" ]]; then
	echo "Der Ordner für die Tiles wurde nicht gefunden. Befindest du dich im Hauptverzeichnis des Projektes?"
	exit 1
fi

for h in $(seq 1 3); do
	for i in $(seq 1 $1); do
		for j in $(seq 1 3); do
			if [[ ! -f "neudinkel/assets/tiles/street${h}_0${j}_${i}.tscn" ]]; then
				echo "Tile street${h}_0${j}_${i}.tscn fehlt!"
			fi
		done
	done
done
