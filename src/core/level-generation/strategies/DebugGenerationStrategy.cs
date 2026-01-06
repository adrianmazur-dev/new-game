using System.Collections.Generic;
using Godot;

public class DebugGenerationStrategy : ILevelGenerationStrategy
{
	private readonly int ROOM_COUNT = 2;

	private void GenerateLevelRecursive(
		List<Room> possibleRoomsList,
		IRandomNumberGenerator rng,
		Node parentNode,
		Room room,
		int roomsRemaining
	)
	{
		foreach (Marker3D exit in room.Exits)
		{
			if (roomsRemaining <= 0)
			{
				return;
			}

			int randomIndex = rng.RandomInt(0, possibleRoomsList.Count);
			Room nextRoomTemplate = possibleRoomsList[randomIndex];

			if (nextRoomTemplate.Exits.Length == 0)
			{
				continue;
			}

			Room nextRoom = (Room)nextRoomTemplate.Duplicate();
			Marker3D entrance = nextRoom.Exits[0];
			Transform3D exitTransform = exit.GlobalTransform;
			Transform3D entranceTransform = entrance.Transform;
			Vector3 offset = exitTransform.Origin - entranceTransform.Origin;
			nextRoom.Position += offset;

			parentNode.AddChild(nextRoom);

			GenerateLevelRecursive(possibleRoomsList, rng, parentNode, nextRoom, roomsRemaining - 1);
		}
	}

	public void GenerateLevel(ICollection<Room> possibleRooms, IRandomNumberGenerator rng, Node parentNode)
	{
		List<Room> possibleRoomsList = [..possibleRooms];
		int randomIndex = rng.RandomInt(0, possibleRoomsList.Count);
		Room firstRoom = possibleRoomsList[randomIndex];

		GenerateLevelRecursive(possibleRoomsList, rng, parentNode, firstRoom, ROOM_COUNT);
	}
}
