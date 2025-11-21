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
            int _x = n.getTilePosX(), _y = n.getTilePosY();

            checkForEmptyNeighbour(_x - 1, _y);
            checkForEmptyNeighbour(_x, _y - 1);
            checkForEmptyNeighbour(_x + 1, _y);
            checkForEmptyNeighbour(_x, _y + 1);
            checkForEmptyNeighbour(_x - 1, _y + 1);
            checkForEmptyNeighbour(_x + 1, _y - 1);
        }
    }

    private void checkForEmptyNeighbour(int x, int y)
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

        if (this.selectedNode != null && !this.selectedNode.Equals(newSelectedNode)) this.selectedNode.toggleHighlight(false);

        newSelectedNode.toggleHighlight(!newSelectedNode.highlighted);
        this.selectedNode = newSelectedNode;
    }
}
