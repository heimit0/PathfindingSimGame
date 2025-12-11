using Godot;

public partial class PruefungsGameManager : SimulationGameManager
{
	private GraphNode checkPointNode;

	[Signal]
	public delegate void TileSelectedEventHandler(Vector2I tilePos);
	private GraphSearchAlgorithm algo;

	private int correctStepCounterSinceLastSave;
	private int wrongStepCounter;
    public override void  _Ready()
    {
		TileSelected += playerInteraction;
		SetStartNode += onSetStartNode;
		SetGoalNode += onSetGoalNode;
		DoSearchForward += onDoSearchForward;
		InitOrChangeAlgorithm += onInitOrChangeAlgorithm;

		
		

		string levelToLoad = string.IsNullOrEmpty(Globals.nextLevel) ? "empty" : Globals.nextLevel;
		algo = Globals.pruefungsAlgorithmus;



		GD.Print("Lade Level");
		prepScene(levelToLoad);
		GD.Print("Lade Algorithmus");
		onInitOrChangeAlgorithm((int)algo);
		GD.Print("Algo geladen");

		initialSteps();

		currentAlgorithm.startNode.updateModel();
		currentAlgorithm.goalNode.updateModel();

		Minimap minimap = GetNode<Minimap>("Pruefungsmodus/UI/Minimap");
		minimap.initMiniMap(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	//Macht bei allen Algorithmen die ersten initilaisierungs steps
	public void initialSteps()
	{
		GD.Print("Startile "+ this.currentAlgorithm.startNode.getTilePosString());
		GD.Print("Goaltile: "+ this.currentAlgorithm.goalNode.getTilePosString());
		
		skipTillNextCurrentNode();
		saveCheckpoint();
		correctStepCounterSinceLastSave = 0;

		GD.Print("Initial steps sind durch");
	}

	//Code um Spielerclick zu behandeln
	public void playerInteraction(Vector2I tilePos)
	{
		GD.Print("Player clicked");
		GraphNode playerNode = this.graph.getNode(tilePos.X, tilePos.Y);
		if (playerNode is null)
		{
			GD.PrintErr("Player clicked the void at: " + tilePos);	
			return;
		}

		
		skipTillNextCurrentNode();

	
		if(currentAlgorithm.currentNode is null)
		{
			GD.Print("current Node is null");
			return;
		}

		GD.Print("Richtige Node: "+ currentAlgorithm.currentNode.getTilePosString()+" Node des Spielers: "+ playerNode.getTilePosString());
		if(playerNode == currentAlgorithm.currentNode)
		{
			GD.Print("Correct Tile Clicked");
			playerDidSmthRight();
			if(currentAlgorithm.currentNode.Equals(currentAlgorithm.goalNode))
			{
				lvlGeschafft();
			}
			return;
		}
		else
		{
			GD.Print("Wrong Tile Clicked, Returning to Checkpoint");
			playerDidSmthWrong();
			return;
		}

	}

    private void lvlGeschafft()
    {
        var popup = GetNode<PopupPanel>("Pruefungsmodus/UI/LevelCompletePopup");

		if (popup is not null)
		{
				popup.Show();
				popup.Exclusive = false;
				popup.PopupWindow = false;
		}
		// Save level completion in progress bit flags
		int bitOffset = 0;
		if (algo == GraphSearchAlgorithm.DIJKSTRA) bitOffset = 0;
		if (algo == GraphSearchAlgorithm.A_STAR) bitOffset = 3;
		if (algo == GraphSearchAlgorithm.DFS) bitOffset = 6;

		// Set the bit for the completed level (stufe is 1-3, so subtract 1 for bit index 0-2)
		int levelBit = 1 << (bitOffset + Globals.stufe - 1);
		Globals.config.progress |= levelBit;

		Globals.saveGame();
    }


	public void onNextLvlButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/mainmenu.tscn");
	}

    public void playerDidSmthWrong()
	{
		wrongStepCounter++;
		if(wrongStepCounter>3)
			checkPointNode = startNode;

		Events.Instance.EmitSignal(Events.SignalName.ClearOpenQueue);

		// Open popup to notify player
		var popup = GetNode<PopupPanel>("Pruefungsmodus/UI/SpielerHatWasFalschGemachtPopup");
		var popuptext = GetNode<Label>("Pruefungsmodus/UI/SpielerHatWasFalschGemachtPopup/VBoxContainer/Label");

		if (popup is not null && popuptext is not null && wrongStepCounter <=4)
		{
			popuptext.Text = "Leider das Falsche Tile ausgewählt \n Das richtige Tile war X: "
			 + currentAlgorithm.currentNode.tilePosX + " Y: " + currentAlgorithm.currentNode.tilePosY + "\n" +
			 "Falsche Schritte: " + wrongStepCounter +"/4";
			popup.Show();
		}

		if(wrongStepCounter >= 4)
		{
			var lvlFailed = GetNode<PopupPanel>("Pruefungsmodus/UI/LevelFailedPopup");
			lvlFailed.Show();
			lvlFailed.Exclusive = false;
			lvlFailed.PopupWindow = false;
		}

		returnToCheckpoint();
	}

	//Legacy
	public void hidePopupOnButtonPressed()
	{
		var popup = GetNode<PopupPanel>("Pruefungsmodus/UI/SpielerHatWasFalschGemachtPopup");
		if(popup is not null) popup.Hide();
	}


	public void playerDidSmthRight()
	{
		correctStepCounterSinceLastSave++;
		if(correctStepCounterSinceLastSave>=3)
		{
			correctStepCounterSinceLastSave = 0;
			saveCheckpoint();
		}
		currentAlgorithm.currentNode.setHighlighted(true);
	}

	public void skipTillNextCurrentNode()
	{
        switch ((currentAlgorithm))
        {
            case AlgorithmAStar astar:
				currentAlgorithm.breakpoint = 5;
                break;

			case AlgorithmDFS dfs:
				currentAlgorithm.breakpoint = 4;
				break;

			case AlgorithmDijkstra dijkstra:
				currentAlgorithm.breakpoint = 6;
				break;
        }

		GD.Print("Selected Breakpoint at:" + currentAlgorithm.breakpoint);
		GD.Print("Current step before doSearchForward: " + currentAlgorithm.currentStep);

		if(currentAlgorithm.currentNode is not null) GD.Print("Old current Node: " + currentAlgorithm.currentNode);

		GD.Print("DEBUG: About to call doSearchForward()");
		currentAlgorithm.doSearchForward();
		GD.Print("DEBUG: doSearchForward() completed");

		if(currentAlgorithm.currentNode is not null) GD.Print("New current Node: " + currentAlgorithm.currentNode);
		GD.Print("Successfull newCurrentNode");
    }




    //Speichert aktuelle Node in CheckpointNode
    public void saveCheckpoint()
	{
		checkPointNode = currentAlgorithm.currentNode;
	}

	// Laedt den Algorithus neu und schreitet bis zum Checkpoint nach vorne
	public int returnToCheckpoint()
	{
		GD.Print("Start resetting Algorithm");
		onInitOrChangeAlgorithm(-1);
		if(wrongStepCounter>3)
			wrongStepCounter = 0;
		int allowedNumberOfSteps = 1000000;
		int failsafe = 0;
		while(currentAlgorithm.currentNode != checkPointNode && failsafe < allowedNumberOfSteps)
		{
			this.doSearchForwardHelper(true);
			failsafe++;

		}
		if(checkPointNode == currentAlgorithm.currentNode)
			return 1;

		return -1;
	}
}
