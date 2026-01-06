using Godot;
using NewGame.Core.Inventory;

public partial class Player : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float MouseSensitivity = 0.002f;

	private Marker3D _neck;
	private Camera3D _camera;
	private Inventory _inventory;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		_neck = GetNodeOrNull<Marker3D>("Neck");
		_camera = GetNodeOrNull<Camera3D>("Neck/Camera3D");
		_inventory = GetNodeOrNull<Inventory>("Inventory");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(-mouseMotion.Relative.X * MouseSensitivity);

			if (_neck != null)
			{
				_neck.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
				
				Vector3 rotation = _neck.Rotation;
				rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
				_neck.Rotation = rotation;
			}
		}

		if (@event.IsActionPressed("ui_cancel"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
				Input.MouseMode = Input.MouseModeEnum.Visible;
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
		}

		// Inventory slot switching
		if (@event.IsActionPressed("ui_inventory_prev"))
			_inventory?.SelectPrevious();
		if (@event.IsActionPressed("ui_inventory_next"))
			_inventory?.SelectNext();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionPressed("ui_jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
