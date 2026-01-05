using Godot;
using System.Collections.Generic;
using System.Linq;

namespace NewGame.Core.Interactions;

public partial class Interactable : Node
{
    [Export] public InteractionComponent[] Interactions = [];
    [Export] public InteractionComponent DefaultInteraction;
    [Export] public bool IsEnabled = true;
    
    public override void _Ready()
    {
        if (Interactions == null || Interactions.Length == 0)
        {
            var components = new List<InteractionComponent>();
            foreach (var child in GetChildren())
            {
                if (child is InteractionComponent comp)
                    components.Add(comp);
            }
            Interactions = components.ToArray();
        }
    }

    public IReadOnlyList<InteractionComponent> GetAvailableInteractions(Node3D interactor)
    {
        if (!IsEnabled || Interactions == null || Interactions.Length == 0)
            return [];
        
        return Interactions
            .Where(i => i != null && i.CanInteract(interactor))
            .ToArray();
    }
    
    public InteractionComponent GetDefaultInteraction(Node3D interactor)
    {
        if (!IsEnabled) return null;
        if (DefaultInteraction != null && DefaultInteraction.CanInteract(interactor))
            return DefaultInteraction;
        
        var available = GetAvailableInteractions(interactor);
        return available.Count > 0 ? available[0] : null;
    }
    
    public bool HasInteractions(Node3D interactor) 
        => GetAvailableInteractions(interactor).Count > 0;
    
    public int InteractionCount(Node3D interactor) 
        => GetAvailableInteractions(interactor).Count;
}
