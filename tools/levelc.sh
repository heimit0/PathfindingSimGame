#!/bin/bash

pos_to_index() {
	x=$1
	y=$2
	echo $((sizearr[0] * y + x))
}

edges=()
declare -A types

# Das kann man sicher schöner machen, möchte ich aber nicht.
read -r size
read -r starttile
read -r endtile
sizearr=($size)
starttilearr=($starttile)
endtilearr=($endtile)

while read -r line; do
	if [[ ! "$line" == \#* ]]; then
		linearr=($line)
		curx="${linearr[0]}"
		cury="${linearr[1]}"
		curtype="${linearr[2]}"
		dirs="${linearr[3]}"
		for (( i=0; i <${#dirs}; i++ )); do
			case "${dirs:$i:1}" in
				"0")
					newx=$((curx+1))
					newy=$((cury-1));;
				"1")
					newx=$((curx+1))
					newy="$cury";;
				"2")
					newx="$curx"
					newy=$((cury+1));;
				"3")
					newx=$((curx-1))
					newy=$((cury+1));;
				"4")
					newx=$((curx-1))
					newy="$cury";;
				"5")
					newx="$curx"
					newy=$((cury-1));;
				*)
					echo "Falsche Richtungsangabe in Zeile $line?"
					exit 1;;
			esac
		edges+=("$(pos_to_index curx cury) $(pos_to_index newx newy)")
		curx=$newx
		cury=$newy
		done
	fi
done

echo -n '{"name":"Level","startPos":[0,0],'

echo -n '"tiles":['
for tiley in $(seq 1 ${sizearr[1]}); do
	for tilex in $(seq 1 ${sizearr[0]}); do
		echo -n '{"posX":' "$tilex" ',"posY":' "$tiley" ',"type":1}'
		if ! ([ $tilex -eq ${sizearr[0]} ] && [ $tiley -eq ${sizearr[1]} ]); then echo -n ','; fi
	done
done
echo -n '],'

echo -n '"edges":['
i=1
for edge in "${edges[@]}"; do
	edgearr=($edge)
	echo -n '{"from":' "${edgearr[0]}" ',"to":' "${edgearr[1]}" ',"bi":true}'
	if [[ i -lt ${#edges[@]} ]]; then
		echo -n ','
	fi
	i=$((i+1))
done

echo ']}'
