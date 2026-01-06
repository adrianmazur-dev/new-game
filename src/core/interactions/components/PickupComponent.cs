using Godot;
using NewGame.Core.Inventory;

namespace NewGame.Core.Interactions;

public partial class PickupComponent : InteractionComponent
{
    [Export] public ItemData Item;
    [Export] public int Count = 1;
    [Export] public bool DestroyOnPickup = true;

    public override void _Ready()
    {
        GD.Print($"[PickupComponent._Ready] Item = {Item?.DisplayName ?? "NULL"}, Count = {Count}");
    }

    public override void StartInteraction(Node3D interactor)
    {
        GD.Print($"[PickupComponent._Ready] Item = {Item?.DisplayName ?? "NULL"}, Count = {Count}");

        EmitSignal(SignalName.InteractionStarted);
        
        GD.Print($"[PickupComponent] StartInteraction called by {interactor.Name}");
        
        var inventory = interactor.GetNodeOrNull<Inventory.Inventory>("Inventory");
        if (inventory == null)
        {
            GD.PrintErr($"[PickupComponent] Interactor {interactor.Name} has no Inventory node");
            return;
        }

        GD.Print($"[PickupComponent] Found inventory, adding item: {Item?.DisplayName ?? "NULL"}");
        
        if (Item == null)
        {
            GD.PrintErr("[PickupComponent] Item is null");
            return;
        }

        if (inventory.AddItem(Item, Count))
        {
            GD.Print($"[PickupComponent] Item added successfully");
            EmitSignal(SignalName.InteractionCompleted);
            
            if (DestroyOnPickup)
            {
                var target = GetParent()?.GetParent();
                GD.Print($"[PickupComponent] Destroying: {target?.Name}");
                target?.QueueFree();
            }
        }
        else
        {
            GD.Print("[PickupComponent] Failed to add item");
        }
    }
}

