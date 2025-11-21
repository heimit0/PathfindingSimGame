using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class NewTileGraphNode : Node3D
{
    GraphNode[] neighbours;
    public int tilePosX;
    public int tilePosY;

    public NewTileGraphNode()
    {
        this.neighbours = new GraphNode[6];
    }

	public override void _Ready()
	{
		base._Ready();
        AddChild((Node3D) ResourceLoader.Load<PackedScene>("assets/testing/add_tile_tile.tscn").Instantiate());
    }

	public void setTilePos(int tilePosX, int tilePosY)
    {
        this.tilePosX = tilePosX;
        this.tilePosY = tilePosY;
        this.Position = GraphNode.convertTilePosToPos(tilePosX, tilePosY);
    }

    // macht das tile zu einem echten Tile
    public void makeInstance()
    {
        GraphNode node = new GraphNode();
        node.setTilePos(this.tilePosX, this.tilePosY);
        node.setStreetType(0);
        this.GetParent().AddChild(node);

        QueueFree(); // kill self
    }

	public int getTilePosX()
	{
		return this.tilePosX;
	}

	public int getTilePosY()
	{
		return this.tilePosY;
	}
}
