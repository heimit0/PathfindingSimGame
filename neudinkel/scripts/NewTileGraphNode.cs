using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class NewTileGraphNode : GraphNode
{

    public NewTileGraphNode()
    {
        this.neighbours = new GraphNode[6];
    }

	public override void _Ready()
	{
		base._Ready();
		this.tileBase = (Node3D) ResourceLoader.Load<PackedScene>("assets/testing/add_tile_tile.tscn").Instantiate();
		this.AddChild(this.tileBase);
		updateModel();
        this.highlighted = false;
    }

    // macht das tile zu einem echten Tile
    public new void toggleHighlight(bool highlight)
    {
        GraphNode node = new GraphNode();
        node.setTilePos(node.tilePosX, node.tilePosY);
        node.setStreetType(0);

        QueueFree(); // kill self
    }
}
