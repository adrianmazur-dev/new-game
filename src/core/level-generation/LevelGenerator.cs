using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LevelGenerator : Node3D
{
    [Export]
    public PackedScene[] PossibleRoomScenes = [];

    [Export]
    public Node3D RoomsContainer;

    private readonly HashSet<Room> _possibleRooms = [];

    public override void _Ready()
    {
        if (RoomsContainer == null)
        {
            GD.PrintErr("No room container node provided.");
        }

        if (RoomsContainer == this)
        {
            GD.PrintErr("Provide room container node different than LevelGenerator node.");
        }

        foreach (PackedScene roomScene in PossibleRoomScenes)
        {
            try
            {
                Room roomInstance = roomScene.Instantiate<Room>();
                _possibleRooms.Add(roomInstance);
            }
            catch
            {
                GD.PrintErr(
                    $"Scene \"{roomScene.ResourcePath}\" has no room script attached to the root node."
                );
            }
        }

        GD.Print($"Loaded {_possibleRooms.Count} rooms.");
    }

    public void OnGenerateButtonClick()
    {
        DebugGenerationStrategy strategy = new();
        EngineRandomNumberGenerator rng = new();

        foreach (Node child in RoomsContainer.GetChildren())
        {
            child.QueueFree();
        }

        strategy.GenerateLevel(_possibleRooms, rng, RoomsContainer);
    }
}
