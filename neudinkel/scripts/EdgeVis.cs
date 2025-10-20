using Godot;
using System;

public partial class EdgeVis : MeshInstance3D
{

	private GraphNode parent;

	public EdgeVis() : base()
	{
		this.Mesh = new ImmediateMesh();
	}
	
	public override void _Ready() { }
	
	public override void _Process(double delta) { }
	
	/// <summary>
	/// Aktualisiert das angezeigte Mesh je nach Gegebenheit.
	/// </summary>
	public void updateMesh()
	{
		ImmediateMesh mesh = (ImmediateMesh) this.Mesh;
		mesh.ClearSurfaces();
		updateEdges(mesh);
	}
	
	public void updateEdges(ImmediateMesh mesh)
	{
		GraphNode[] neighbors = parent.getNeighbours();

		// TODO: Material?
		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, null);
		for (int i = 0; i < 6; i++)
		{
			if (neighbors[i] is not null)
			{
				// Wir zeichnen eine Linie. UVs und Normalen egal
				mesh.SurfaceAddVertex(new Vector3(0, 0, 0));
				mesh.SurfaceAddVertex(new Vector3(-Mathf.Sin(Mathf.DegToRad(i*10)), 0, Mathf.Cos(Mathf.DegToRad(10*i))));
			}
		}
		mesh.SurfaceEnd();
	}

}
