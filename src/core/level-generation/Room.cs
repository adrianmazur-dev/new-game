using System.Collections.Generic;
using Godot;

public partial class Room : Node3D
{
	[Export]
	public ExitMarker[] Exits = [];

	public Room DuplicateAndAddToParent(Node parent)
	{
		Room clone = (Room)Duplicate();
		parent.AddChild(clone, true);
		return clone;
	}
}
