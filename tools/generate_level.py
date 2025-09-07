import random

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
		result.append((current[0]+1, current[0]+2))

	# TODO: Das ist die vermutlich blödeste Art das zu lösen, aber hey
	for pos in result:
		if pos in visited:
			result.remove(pos)

	return result

def walk(size: list, visited: list, edges: list, current: tuple): 
	visited.append(current)
	n = get_unvisited_neighbors(size, visited, current)
	print("Wir sind bei Node " + str(current) + ", mit " + str(len(n)) + "Möglichkeiten.")
	if len(n) == 0:
		return visited, edges
	else:
		chosen = random.choice(n)
		edges.append((current, chosen))
		return walk(size, visited, edges, chosen)

size = [2, 2]
_, edges = walk(size, [], [], (0, 0))

print(edges)
