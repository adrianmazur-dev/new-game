using Godot;

public partial class ExitMarker : Node3D
{
	public Vector3 GetDirection()
	{
		return GlobalTransform.Basis.Z.Normalized();
	}

	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			Visible = false;
		}
	}
}
