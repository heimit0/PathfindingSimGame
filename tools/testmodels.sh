#!/bin/bash

if [[ $# -eq 1 ]]; then
	numvars="$1"
	numcityvars="$1"
elif [[ $# -eq 2 ]]; then
	numvars="$1"
	numcityvars="$2"
else
	echo "Benutzung:"
	echo "    $0 <Anzahl Variationen> [<Anzahl Stadt-Variationen>]"
	exit 1
fi

if [[ ! -d "neudinkel/assets/" ]]; then
	echo "Der Ordner für die Tiles wurde nicht gefunden. Befindest du dich im Hauptverzeichnis des Projektes?"
	exit 1
fi

for h in $(seq 1 3); do
	for i in $(seq 1 $numvars); do
		for j in $(seq 1 3); do
			if [[ ! -f "neudinkel/assets/tiles/street${h}_0${j}_${i}.tscn" ]]; then
				echo "Tile street${h}_0${j}_${i}.tscn fehlt!"
			fi
		done
	done
done

for i in $(seq 3 6); do
	for j in $(seq 1 $numcityvars); do
		if [[ ! -f "neudinkel/assets/tiles/city0${i}_${j}.tscn" ]]; then
			echo "Tile city0${i}_${j}.tscn fehlt"
		fi
	done
done
