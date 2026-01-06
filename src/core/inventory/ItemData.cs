using Godot;

namespace NewGame.Core.Inventory;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string Id = "";
    [Export] public string DisplayName = "";
    [Export] public bool IsStackable = false;
    [Export] public int MaxStackSize = 1;
}
