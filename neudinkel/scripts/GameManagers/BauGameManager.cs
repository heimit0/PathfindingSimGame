using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class BauGameManager : GameManager
{
	[Signal]
	public delegate void TileSelectedEventHandler(int[] tilePos);

	[Signal]
	public delegate void ChangeTileStreetTypeEventHandler(int newStreetType);


	public List<GraphNode> selectedNodes { get; set; }

	public override void _Ready()
	{
		prepScene("baumodus_save");
		addNewTileTiles();
		this.selectedNodes = new List<GraphNode>();

		TileSelected += onTileSelected;
		ChangeTileStreetType += onChangeStreetType;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	private void addNewTileTiles()
	{
		foreach (GraphNode n in this.graph.getNodes())
		{
			checkForAllEmptyNeighbours(n);
		}
	}

	public void checkForAllEmptyNeighbours(GraphNode n)
	{
		int _x = n.getTilePosX(), _y = n.getTilePosY();

		checkForEmptyNeighbour(_x - 1, _y);
		checkForEmptyNeighbour(_x, _y - 1);
		checkForEmptyNeighbour(_x + 1, _y);
		checkForEmptyNeighbour(_x, _y + 1);
		checkForEmptyNeighbour(_x - 1, _y + 1);
		checkForEmptyNeighbour(_x + 1, _y - 1);
	}

	public void checkForEmptyNeighbour(int x, int y)
	{
		if (this.graph.getNode(x, y) == null && this.graph.getGhostNode(x, y) == null)
		{
			NewTileGraphNode node = new NewTileGraphNode();
			node.setTilePos(x, y);
			this.graph.AddChild(node);
		}
	}

	public void onTileSelected(int[] tilePos)
	{
		NewTileGraphNode selectedGhostNode = this.graph.getGhostNode(tilePos[0], tilePos[1]);

		if (selectedGhostNode != null)
		{
			selectedGhostNode.makeInstance();
			return;
		}

		GraphNode newSelectedNode = this.graph.getNode(tilePos[0], tilePos[1]);

		// wenn man in die luft klickt, sollen sich alle tiles aus der selection entfernen
		if (newSelectedNode == null)
		{
			foreach (GraphNode n in this.selectedNodes)
			{
				this.graph.getNode(n.getTilePosX(), n.getTilePosY()).toggleHighlight(false);
			}

			this.selectedNodes.Clear();
			return;
		}
		else
		{
			// gucken, ob die neue node schon in der liste ist
			// da wir die liste aendern muessen wir sie vorher kopieren
			GraphNode[] _selectedNodes = this.selectedNodes.ToArray();
			foreach (GraphNode n in _selectedNodes)
			{
				if (newSelectedNode.Equals(n))
				{
					// falls ja: wieder entfernen
					newSelectedNode.toggleHighlight(false);
					this.selectedNodes.Remove(n);
				}
			}

			newSelectedNode.toggleHighlight(true);
			this.selectedNodes.Add(newSelectedNode);
		}
	}

	public void onChangeStreetType(int newStreetType)
	{
		// if no tiles selected, => bug fix
		if (this.selectedNodes.Count == 0) return;

		// add nodes
		foreach (GraphNode n in this.selectedNodes)
		{
			if (this.graph.getNode(n) == null) this.graph.addNode(n);
		}

		// add relations
		foreach (GraphNode n1 in this.selectedNodes)
		{
			// if "make empty"
			if (newStreetType == (int)StreetType.EMPTY)
			{
				// skip all, remove all neighbours
				n1.clearNeighbours();
				continue;
			}

			foreach (GraphNode n2 in this.selectedNodes)
			{
				// if its a neighbour
				if (Math.Abs(n1.getTilePosX() - n2.getTilePosX()) == 1 && n1.getTilePosY() == n2.getTilePosY() ||
					(Math.Abs(n1.getTilePosY() - n2.getTilePosY()) == 1 && n1.getTilePosX() == n2.getTilePosX()) ||
					(n1.getTilePosX() - 1 == n2.getTilePosX() && n1.getTilePosY() + 1 == n2.getTilePosY()) ||
					(n1.getTilePosX() + 1 == n2.getTilePosX() && n1.getTilePosY() - 1 == n2.getTilePosY()))
				{
					this.graph.addRelation(n1, n2);
				}
			}
		}

		// erst alle streetTypes setzen, dann alle models updaten, um verwirrung zu vermeiden
		foreach (GraphNode n in this.selectedNodes) n.setStreetType((StreetType)newStreetType);
		foreach (GraphNode n in this.selectedNodes) n.updateModel();

		// deselect all
		onTileSelected(new int[] { -10000, -10000 });
	}

	public void onChangeToSimulationMode()
	{
		// save state
		this.graph.toJsonFile();

		GetTree().ChangeSceneToFile("res://scenes/Simulationsmodus.tscn");
	}
}
