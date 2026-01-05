using Godot;

namespace NewGame.Core.Interactions;

public abstract partial class InteractionComponent : Node
{
    [Signal] public delegate void InteractionStartedEventHandler();
    [Signal] public delegate void InteractionCompletedEventHandler();
    [Signal] public delegate void InteractionCancelledEventHandler();
    
    [Export] public string InputAction = "ui_interact";

    public virtual bool CanInteract(Node3D interactor) => true;
    
    public virtual void StartInteraction(Node3D interactor) => EmitSignal(SignalName.InteractionStarted);
    public virtual void UpdateInteraction(Node3D interactor, double delta) { }
    public virtual void EndInteraction(Node3D interactor) => EmitSignal(SignalName.InteractionCancelled);
    public virtual void HandleInput(InputEvent @event) { }
}
