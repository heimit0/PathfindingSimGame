using Godot;
using System;

public partial class ApplySaturationShader : Node3D
{
	// Path to your .gdshader file
	private const string ShaderPath = "res://assets/shaders/saturation_shader.gdshader";

	// The duplicated material for this specific tile instance
	private ShaderMaterial _materialInstance;

	// Exposed saturation value in Inspector
	[Export(PropertyHint.Range, "0,2,0.01")]
	private float _saturation = 1.0f;

	public float Saturation
	{
		get => _saturation;
		set
		{
			_saturation = value;
			if (_materialInstance != null)
				_materialInstance.SetShaderParameter("saturation", _saturation);
		}
	}

	public override void _Ready()
	{
		// Load shader
		var shader = GD.Load<Shader>(ShaderPath);

		// Create unique material instance for this tile
		_materialInstance = new ShaderMaterial();
		_materialInstance.Shader = shader;

		// Apply initial saturation value
		_materialInstance.SetShaderParameter("saturation", _saturation);

		// Apply material to all meshes inside this node
		ApplyMaterialRecursively(this);
	}

	private void ApplyMaterialRecursively(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is MeshInstance3D mesh)
			{
				// Assign to all surfaces
				int surfaceCount = mesh.Mesh?.GetSurfaceCount() ?? 0;

				for (int i = 0; i < surfaceCount; i++)
				{
					mesh.SetSurfaceOverrideMaterial(i, _materialInstance);
				}
			}

			// Recurse into children
			ApplyMaterialRecursively(child);
		}
	}
}
