using System;
using System.Collections.Generic;
using Godot;

public class DebugGenerationStrategy : ILevelGenerationStrategy
{
	private const int ROOM_COUNT = 5;

	private struct DFSNode(Room roomTemplate, ExitMarker exitToConnectTo)
	{
		public ExitMarker ExitToConnectTo = exitToConnectTo;
		public Room RoomTemplate = roomTemplate;
	}

	public void GenerateLevel(
		ICollection<Room> possibleRooms,
		IRandomNumberGenerator rng,
		Node parent
	)
	{
		List<Room> roomTemplates = [.. possibleRooms];
		Stack<DFSNode> stack = new();
		Room startingRoom = rng.RandomListElement(roomTemplates).DuplicateAndAddToParent(parent);
		ExitMarker startingExit = rng.RandomListElement(startingRoom.Exits);
		int roomsLeft = ROOM_COUNT - 1;

		stack.Push(new DFSNode(rng.RandomListElement(roomTemplates), startingExit));

		while (stack.Count > 0 && roomsLeft > 0)
		{
			DFSNode node = stack.Pop();
			roomsLeft--;

			Room room = node.RoomTemplate.DuplicateAndAddToParent(parent);
			ExitMarker entrance = rng.RandomListElement(room.Exits);

			room.Position -= new Vector3(
				node.ExitToConnectTo.Position.X,
				0,
				node.ExitToConnectTo.Position.Z
			);

			const int maxRotationAttempts = 3;
			bool aligned = false;

			for (int attempt = 0; attempt < maxRotationAttempts; attempt++)
			{
				Vector3 exitForward = node.ExitToConnectTo.GetDirection();
				Vector3 entranceForward = entrance.GetDirection();
				float dotProduct = exitForward.Dot(entranceForward);

				GD.Print($"Entrance: {entrance.GlobalTransform.Basis}");
				GD.Print($"Inside tree: {entrance.IsInsideTree()}");
				GD.Print($"Idenitity: {entrance.GlobalTransform.Equals(Transform3D.Identity)}");

				if (dotProduct == -1)
				{
					aligned = true;
					break;
				}

				room.RotateY(Mathf.DegToRad(45));
			}

			GD.Print();

			if (!aligned)
			{
				GD.PrintErr("Couldn't align room exits. Does room contain valid exit markers?");
				return;
			}

			List<ExitMarker> exits = [.. room.Exits];
			rng.ShuffleList(exits);

			foreach (ExitMarker exit in exits)
			{
				if (roomsLeft <= 0)
				{
					break;
				}

				Room nextRoomTemplate = rng.RandomListElement(roomTemplates);
				stack.Push(new DFSNode(nextRoomTemplate, exit));
			}
		}
	}
}
