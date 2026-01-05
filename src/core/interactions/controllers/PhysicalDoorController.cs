using Godot;

namespace NewGame.Core.Interactions;

public partial class PhysicalDoorController : Node
{
    public enum Axis { X, Y, Z }

    [ExportGroup("Constraints")]
    [Export] public float MinAngle = 0f;
    [Export] public float MaxAngle = 90f;
    [Export] public Axis RotationAxis = Axis.Y;

    [ExportGroup("Settings")]
    [Export] public float Sensitivity = 0.5f;
    [Export] public bool InvertDirection = false;
    [Export] public bool UseMouseX = true;

    private float _currentAngle;
    private Node3D _target;

    public override void _Ready()
    {
        _target = GetParentOrNull<Node3D>();
        if (_target == null)
        {
            GD.PrintErr($"PhysicalDoorController parent on {Name} must be node3D");
            SetProcess(false);
            return;
        }
        _currentAngle = GetCurrentRotationValue();
    }

    public void OnDragged(Vector2 relative)
    {
        float input = UseMouseX ? relative.X : relative.Y;
        if (InvertDirection) input *= -1;

        _currentAngle += input * Sensitivity;
        _currentAngle = Mathf.Clamp(_currentAngle, MinAngle, MaxAngle);
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Vector3 rot = _target.RotationDegrees;
        switch (RotationAxis)
        {
            case Axis.X: rot.X = _currentAngle; break;
            case Axis.Y: rot.Y = _currentAngle; break;
            case Axis.Z: rot.Z = _currentAngle; break;
        }
        _target.RotationDegrees = rot;
    }

    private float GetCurrentRotationValue()
    {
        return RotationAxis switch
        {
            Axis.X => _target.RotationDegrees.X,
            Axis.Y => _target.RotationDegrees.Y,
            Axis.Z => _target.RotationDegrees.Z,
            _ => 0f
        };
    }
}
