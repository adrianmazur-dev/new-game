using System.Collections.Generic;
using Godot;

public partial class Room : Node3D
{
	[Export]
	public ExitMarker[] Exits = [];

	public Room DuplicateAndAddToParent(Node parent)
	{
		Room original = this;
		Room clone = (Room)Duplicate();
		parent.AddChild(clone, true);

		// Remap exit references to the cloned nodes
		for (int i = 0; i < clone.Exits.Length; i++)
		{
			if (original.Exits[i] != null)
			{
				NodePath path = original.GetPathTo(original.Exits[i]);
				clone.Exits[i] = clone.GetNode<ExitMarker>(path);
			}
		}

		return clone;
	}
}
