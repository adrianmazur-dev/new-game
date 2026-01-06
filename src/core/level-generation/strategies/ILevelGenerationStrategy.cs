
using System.Collections.Generic;
using Godot;

public interface ILevelGenerationStrategy
{
    public void GenerateLevel(ICollection<Room> possibleRooms, IRandomNumberGenerator rng, Node parentNode);
}