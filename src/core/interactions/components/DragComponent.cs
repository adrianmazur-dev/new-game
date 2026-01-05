using Godot;

namespace NewGame.Core.Interactions;

public partial class DragComponent : InteractionComponent
{
    [Signal] public delegate void DragStartedEventHandler();
    [Signal] public delegate void DraggedEventHandler(Vector2 relativeMovement);
    [Signal] public delegate void DragEndedEventHandler();
    
    public override void StartInteraction(Node3D interactor)
    {
        EmitSignal(SignalName.InteractionStarted);
        EmitSignal(SignalName.DragStarted);
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            EmitSignal(SignalName.Dragged, mouseMotion.Relative);
        }
    }

    public override void EndInteraction(Node3D interactor)
    {
        EmitSignal(SignalName.DragEnded);
        EmitSignal(SignalName.InteractionCancelled);
    }
}
