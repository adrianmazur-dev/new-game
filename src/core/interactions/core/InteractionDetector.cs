using Godot;
using System.Collections.Generic;

namespace NewGame.Core.Interactions;

public partial class InteractionDetector : Node
{
    [Export] public float InteractionDistance = 3.0f;
    
    private RayCast3D _ray;
    private Interactable _currentTarget;
    private InteractionComponent _activeComponent;
    private bool _isInteracting;

    public override void _Ready()
    {
        SetupRaycast();
    }
    
    private void SetupRaycast()
    {
        var camera = GetParent().GetNodeOrNull<Camera3D>("Neck/Camera3D");
        if (camera == null) return;
        
        _ray = new RayCast3D
        {
            TargetPosition = new Vector3(0, 0, -InteractionDistance),
            Enabled = true,
            CollideWithAreas = true,
            CollideWithBodies = true
        };
        camera.AddChild(_ray);
    }

    public override void _Input(InputEvent @event)
    {
        if (_isInteracting)
        {
            _activeComponent?.HandleInput(@event);
        }
    }
    
    public override void _Process(double delta)
    {
        if (!_isInteracting) DetectTarget();
        
        ProcessInput(delta);
    }
    
    private void DetectTarget()
    {
        if (_ray == null) return;
        _ray.ForceRaycastUpdate();
        
        if (!_ray.IsColliding())
        {
            ClearTarget();
            return;
        }

        if (_ray.GetCollider() is not Node3D collider)
        {
            ClearTarget();
            return;
        }

        var interactable = FindInteractable(collider);
        if (interactable != _currentTarget)
        {
            _currentTarget = interactable;
            _activeComponent = interactable?.GetDefaultInteraction(GetInteractor());
        }
    }
    
    private Interactable FindInteractable(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is Interactable i) return i;
            foreach (var grandchild in child.GetChildren())
            {
                if (grandchild is Interactable gi) return gi;
            }
        }
        return null;
    }
    
    private void ProcessInput(double delta)
    {
        if (_activeComponent == null) return;
        
        var interactor = GetInteractor();
        if (!_activeComponent.CanInteract(interactor)) return;

        var action = _activeComponent.InputAction;

        if (Input.IsActionJustPressed(action))
        {
            _isInteracting = true;
            _activeComponent.StartInteraction(interactor);
        }

        if (_isInteracting)
        {
            if (Input.IsActionPressed(action))
            {
                _activeComponent.UpdateInteraction(interactor, delta);
            }
            else
            {
                _isInteracting = false;
                _activeComponent.EndInteraction(interactor);
            }
        }
    }
    
    private void ClearTarget()
    {
        if (_isInteracting && _activeComponent != null)
            _activeComponent.EndInteraction(GetInteractor());
        
        _currentTarget = null;
        _activeComponent = null;
        _isInteracting = false;
    }
    
    private Node3D GetInteractor() => GetParent<Node3D>();
    public Interactable CurrentTarget => _currentTarget;
    public InteractionComponent ActiveComponent => _activeComponent;
    
    public IReadOnlyList<InteractionComponent> GetAvailableComponents() 
        => _currentTarget?.GetAvailableInteractions(GetInteractor()) as IReadOnlyList<InteractionComponent> 
           ?? System.Array.Empty<InteractionComponent>();
    
    public void SelectComponent(InteractionComponent component)
    {
        if (_currentTarget == null || _isInteracting) return;
        _activeComponent = component;
    }
}
