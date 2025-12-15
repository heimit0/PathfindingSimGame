using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class BauGameManager : GameManager
{
	[Signal]
	public delegate void TileSelectedEventHandler(Vector2I tilePos);

	[Signal]
	public delegate void ChangeTileStreetTypeEventHandler(int newStreetType);

	public List<GraphNode> selectedNodes { get; set; }

	public LadenPopup ladenPopup { get; set; }

	private NodePath minimapPath = "Baumodus/UI/Minimap";

	public override void _Ready()
	{
		if (Globals.baumodusInitLevel == null)
		{
			prepScene("baumodus_save", true);
			
		}
		else
		{
			prepScene(Globals.baumodusInitLevel, true);
			Globals.baumodusInitLevel = null;
			Globals.openSimpleInfoModal(this, "Level geladen", 2);
		}

		addNewTileTiles();
		this.selectedNodes = new List<GraphNode>();

		TileSelected += onTileSelected;
		ChangeTileStreetType += onChangeStreetType;

		this.ladenPopup = GetNode<LadenPopup>("Baumodus/UI/LadenPopup");
		this.ladenPopup.Submitted += filename => loadNewLevel(filename);
		this.ladenPopup.PopupHide += toggleCameraIsUserInputEnabled;
		this.ladenPopup.Hide();

		Minimap minimap = GetNodeOrNull<Minimap>(minimapPath);
		minimap.initMiniMap(this);

		startNode.updateModel();
		goalNode.updateModel();

		Globals.playBackgroundMusic("res:///assets/sounddesign/baumodusMusik.wav");
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
		int _x = n.tilePosX, _y = n.tilePosY;

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

	public void onTileSelected(Vector2I tilePos)
	{
		NewTileGraphNode selectedGhostNode = this.graph.getGhostNode(tilePos.X, tilePos.Y);

		if (selectedGhostNode != null)
		{
			GD.Print("Selected ghost node: " + selectedGhostNode.getTilePosString());
			selectedGhostNode.makeInstance();
			return;
		}

		GraphNode newSelectedNode = this.graph.getNode(tilePos.X, tilePos.Y);

		// wenn man in die luft klickt, sollen sich alle tiles aus der selection entfernen
		if (newSelectedNode == null)
		{
			foreach (GraphNode n in this.selectedNodes)
			{
				this.graph.getNode(n.tilePosX, n.tilePosY).setHighlighted(false);
			}

			this.selectedNodes.Clear();
			return;
		}
		else
		{
			GD.Print("Selected graph node: " + newSelectedNode.getTilePosString() + " -> streetType: " + newSelectedNode.streetType.ToString());

			// gucken, ob die neue node schon in der liste ist
			// da wir die liste aendern muessen wir sie vorher kopieren
			GraphNode[] _selectedNodes = this.selectedNodes.ToArray();
			foreach (GraphNode n in _selectedNodes)
			{
				if (newSelectedNode.Equals(n))
				{
					// falls ja: wieder entfernen
					newSelectedNode.setHighlighted(false);
					this.selectedNodes.Remove(n);
				}
			}

			newSelectedNode.setHighlighted(true);
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
			if (newStreetType == (int) StreetType.EMPTY) continue;
			if (this.graph.getNode(n) == null) this.graph.addNode(n);
		}

		// add relations
		foreach (GraphNode n1 in this.selectedNodes)
		{
			if (newStreetType == (int) StreetType.EMPTY) continue;

			foreach (GraphNode n2 in this.selectedNodes)
			{
				// if its a neighbour
				if (Math.Abs(n1.tilePosX - n2.tilePosX) == 1 && n1.tilePosY == n2.tilePosY ||
					(Math.Abs(n1.tilePosY - n2.tilePosY) == 1 && n1.tilePosX == n2.tilePosX) ||
					(n1.tilePosX - 1 == n2.tilePosX && n1.tilePosY + 1 == n2.tilePosY) ||
					(n1.tilePosX + 1 == n2.tilePosX && n1.tilePosY - 1 == n2.tilePosY))
				{
					this.graph.addRelation(n1, n2);
					// Special state resetting, probably a bit too much, but time is running out at ths point
					// foreach (GraphNode n3 in n1.getNeighbours())
					// {
					// 	if (this.startNode == n3) this.startNode = null;
					// 	if (this.goalNode == n3) this.goalNode = null;
					// }

					// foreach (GraphNode n3 in n2.getNeighbours())
					// {
					// 	if (this.startNode == n3) this.startNode = null;
					// 	if (this.goalNode == n3) this.goalNode = null;
					// }
				}
			}
		}

		// erst alle streetTypes setzen, dann alle models updaten, um verwirrung zu vermeiden
		foreach (GraphNode n in this.selectedNodes)
		{
			n.streetType = (StreetType)newStreetType;

			if (newStreetType == (int) StreetType.EMPTY)
			{
				n.clearNeighbours();

				if (n.Equals(this.startNode))
				{
					this.startNode = null;
					this.startNode.isStartNode = false;
					GD.Print("Baumodus: reset start node!");
				}

				if (n.Equals(this.goalNode))
				{
					this.goalNode.isGoalNode = false;
					GD.Print("Baumodus: reset goal node!");
				}
			}
		}

		// erst im nachhinein updaten, damit fehler vermieden werden
		foreach (GraphNode n in this.selectedNodes) n.updateModel();

		// deselect all
		onTileSelected(new Vector2I(-10000, -10000));

		// update minimap
		Minimap minimap = GetNode<Minimap>(minimapPath);
		minimap.initMiniMap(this);
	}

	public void onChangeToSimulationMode()
	{
		if (this.startNode == null || this.goalNode == null || this.startNode.Equals(this.goalNode))
		{
			GD.Print("Start- or goalNode not set or both equal (shouldn't happen)! Aborting...");
			Globals.openSimpleInfoModal(this, "Start- oder Endknoten wurden nicht gewählt!", 2);
			return;
		}

		// save state
		actuallySpeichern("baumodus_save");
		GetTree().ChangeSceneToFile("res://scenes/Simulationsmodus.tscn");
	}

	public void baumodusSetStartNode()
	{
		if (this.selectedNodes.Count != 1)
		{
			GD.Print("Warning: tried to set startnode, but more than one tile was selected!");
			Globals.openSimpleInfoModal(this, "Bitte nur eine Node auswählen um Startnode zu setzen!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		if (this.goalNode != null && this.selectedNodes[0].Equals(this.goalNode))
		{
			GD.Print("Tried to set the startNode the same as goalNode! Aborting...");
			Globals.openSimpleInfoModal(this, "Startnode kann nicht Zielnode sein!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		if (this.selectedNodes[0].getNeighbours().Length == 2)
		{
			GD.Print("Tried to select Streetnode as startNode! Aborting...");
			Globals.openSimpleInfoModal(this, "Ausgewählte Node ist keine Stadt!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		this.startNode.isStartNode = false;
		this.startNode.updateModel();
		this.startNode = this.selectedNodes[0];
		this.startNode.isStartNode = true;
		this.startNode.updateModel();
		GD.Print("Setting start node at: " + this.startNode.getTilePosString());
		this.onTileSelected(new Vector2I(-10000, -10000)); // deselect all
	}

	public void baumodusSetGoalNode()
	{
		if (this.selectedNodes.Count != 1)
		{
			GD.Print("Warning: tried to set goalnode, but more than one tile was selected!");
			Globals.openSimpleInfoModal(this, "Bitte nur eine Node auswählen um Zielnode zu setzen!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		if (this.startNode != null && this.selectedNodes[0].Equals(this.startNode))
		{
			GD.Print("Tried to set the startNode the same as goalNode! Aborting...");
			Globals.openSimpleInfoModal(this, "Zielnode kann nicht Startnode sein!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		if (this.selectedNodes[0].getNeighbours().Length == 2)
		{
			GD.Print("Tried to select Streetnode as startNode! Aborting...");
			Globals.openSimpleInfoModal(this, "Ausgewählte Node ist keine Stadt!", 2);
			this.onTileSelected(new Vector2I(-10000, -10000));
			return;
		}

		this.goalNode.isGoalNode = false;
		this.goalNode.updateModel();
		this.goalNode = this.selectedNodes[0];
		this.goalNode.isGoalNode = true;
		this.goalNode.updateModel();
		GD.Print("Setting goal node at: " + this.goalNode.getTilePosString());
		this.onTileSelected(new Vector2I(-10000, -10000)); // deselect all
	}

	// speichern dialog öffnen und level speichern
	public void initLevelSpeichern()
	{
		if (this.startNode == null || this.goalNode == null || this.startNode.Equals(this.goalNode))
		{
			GD.Print("Start- or goalNode not set or both equal (shouldn't happen)! Aborting...");
			Globals.openSimpleInfoModal(this, "Start- oder Endknoten wurden nicht gewählt!", 2);
			return;
		}

		SpeichernPopup popup = GetNode<SpeichernPopup>("Baumodus/UI/SpeichernPopup");
		popup.Submitted += actuallySpeichern;
		popup.Open();

		// reset baumodus
	}

	public void actuallySpeichern(string name)
	{
		// hack, aber egal
		if (name.ToLower().StartsWith("pruef"))
		{
			Globals.openSimpleInfoModal(this, "Level kann nicht mit 'pruef' beginnen!", 2);
			return;
		}

		// save state
		this.graph.toJsonFile(name);
		Globals.openSimpleInfoModal(this, "Level gespeichert", 2);
	}

	public void onLevelLaden()
	{
		this.ladenPopup = GetNode<LadenPopup>("Baumodus/UI/LadenPopup");
		this.ladenPopup.Open();

		toggleCameraIsUserInputEnabled();
		Globals.openSimpleInfoModal(this, "Level geladen", 2);
	}

	private void toggleCameraIsUserInputEnabled()
	{
		Camera3D camera = GetViewport().GetCamera3D();
		(camera as Camera).EmitSignal(Camera.SignalName.setIsUserInputEnabled, (camera as Camera).isUserInputEnabled);
	}

	public void loadNewLevel(string filename)
	{
		Globals.baumodusInitLevel = filename;
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}
}
