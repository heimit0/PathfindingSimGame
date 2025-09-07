import random, json

def get_unvisited_neighbors(size, visited, current):
	result = []
	if not current[0] == 0:
		# Position 4
		result.append((current[0]-1, current[1]))
	if not current[1] == 0:
		# Position 5
		result.append((current[0], current[1]-1))
	if current[0] < size[0]-1:
		# Position 1
		result.append((current[0]+1, current[1]))
	if current[1] < size[1]-1:
		# Position 2
		result.append((current[0], current[1]+1))
	if (not current[0] == 0) and current[1] < size[1]-1:
		# Position 3
		result.append((current[0]-1, current[1]+1))
	if (not current[1] == 0) and current[0] < size[0]-1:
		# Position 0
		result.append((current[0]+1, current[0]+1))

	# BuT ItS ThE IdIoMaTiC WaY
	result = list(set(result) - set(visited))

	return result

def walk(size: list, visited: list, edges: list, current: tuple): 
	visited.append(current)
	n = get_unvisited_neighbors(size, visited, current)
	while len(n) > 0:
		chosen = random.choice(n)
		print(chosen)
		edges.append((current, chosen))
		visited, edges = walk(size, visited, edges, chosen)
		n = get_unvisited_neighbors(size, visited, current)
	return visited, edges

size = [10, 10]
_, edges = walk(size, [], [], (0, 0))

j = {"name": "Level", "tiles": [], "edges": []}
for y in range(size[1]):
	for x in range(size[0]):
		j["tiles"].append({"posX": x, "posY": y})

for edge in edges:
	node_from = edge[0][0] + size[0] * edge[0][1]
	node_to = edge[1][0] + size[0] * edge[1][1]
	j["edges"].append({"from": node_from, "to": node_to, "bi": True})
	
print(json.dumps(j))
