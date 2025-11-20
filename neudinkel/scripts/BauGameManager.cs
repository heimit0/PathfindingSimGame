using Godot;
using System;

public partial class BauGameManager : GameManager
{
    [Signal]
    public delegate void TileSelectedEventHandler(int[] tilePos);

    public override void _Ready()
    {
        prepScene();
        addNewTileTiles();

        TileSelected += onTileSelected;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void addNewTileTiles()
    {
        foreach (GraphNode n in this.graph.getNodes())
        {
            foreach (GraphNode neighbour in n.getNeighbours())
            {
                if (neighbour == null)
                {
                    Node3D addTileTile = (Node3D) ResourceLoader.Load<PackedScene>("assets/testing/add_tile_tilex.tscn").Instantiate();
                }
            }
        }
    }

    public void onTileSelected(int[] tilePos)
    {
        GraphNode newSelectedNode = this.graph.getNode(tilePos[0], tilePos[1]);

        if (this.selectedNode != null && !this.selectedNode.Equals(newSelectedNode)) this.selectedNode.toggleHighlight(false);

        GD.Print(!newSelectedNode.highlighted);
        newSelectedNode.toggleHighlight(!newSelectedNode.highlighted);
        this.selectedNode = newSelectedNode;
    }
}
