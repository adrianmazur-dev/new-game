using Godot;

namespace NewGame.Core.Inventory;

public partial class Inventory : Node
{
    [Signal] public delegate void SlotChangedEventHandler(int slotIndex, ItemData item, int count);
    [Signal] public delegate void ActiveSlotChangedEventHandler(int newIndex, ItemData item);
    [Signal] public delegate void ItemAddedEventHandler(ItemData item, int slotIndex);
    [Signal] public delegate void ItemRemovedEventHandler(ItemData item, int slotIndex);

    [Export] public int SlotCount = 4;
    [Export] public bool DebugLog = true;

    private InventorySlot[] _slots;
    private int _activeSlotIndex;

    public int ActiveSlotIndex => _activeSlotIndex;
    public ItemData ActiveItem => GetItemAt(_activeSlotIndex);

    public override void _Ready()
    {
        _slots = new InventorySlot[SlotCount];
        for (int i = 0; i < SlotCount; i++)
            _slots[i] = new InventorySlot();
        
        Log($"Inventory initialized with {SlotCount} slots");
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return false;

        // Try to stack with existing items first
        if (item.IsStackable)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];
                if (slot.Item?.Id == item.Id && slot.Count < item.MaxStackSize)
                {
                    int spaceLeft = item.MaxStackSize - slot.Count;
                    int toAdd = Mathf.Min(count, spaceLeft);
                    slot.Count += toAdd;
                    count -= toAdd;
                    
                    EmitSignal(SignalName.SlotChanged, i, slot.Item, slot.Count);
                    EmitSignal(SignalName.ItemAdded, item, i);
                    Log($"Stacked {toAdd}x {item.DisplayName} in slot {i}");
                    
                    if (count <= 0) return true;
                }
            }
        }

        // Find empty slot for remaining items
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsEmpty)
            {
                int toAdd = item.IsStackable ? Mathf.Min(count, item.MaxStackSize) : 1;
                _slots[i].Item = item;
                _slots[i].Count = toAdd;
                count -= toAdd;
                
                EmitSignal(SignalName.SlotChanged, i, item, toAdd);
                EmitSignal(SignalName.ItemAdded, item, i);
                Log($"Added {toAdd}x {item.DisplayName} to slot {i}");
                
                if (count <= 0 || !item.IsStackable) return true;
            }
        }

        return count <= 0;
    }

    public bool RemoveItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return false;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Item?.Id == item.Id)
            {
                return RemoveItemAt(i, count);
            }
        }
        return false;
    }

    public bool RemoveItemAt(int slotIndex, int count = 1)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return false;
        
        var slot = _slots[slotIndex];
        if (slot.IsEmpty) return false;

        var item = slot.Item;
        slot.Count -= count;
        
        if (slot.Count <= 0)
        {
            slot.Clear();
            EmitSignal(SignalName.SlotChanged, slotIndex, (ItemData)null, 0);
        }
        else
        {
            EmitSignal(SignalName.SlotChanged, slotIndex, slot.Item, slot.Count);
        }
        
        EmitSignal(SignalName.ItemRemoved, item, slotIndex);
        Log($"Removed {count}x {item.DisplayName} from slot {slotIndex}");
        return true;
    }

    public ItemData GetItemAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return null;
        return _slots[slotIndex].Item;
    }

    public int GetCountAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return 0;
        return _slots[slotIndex].Count;
    }

    public bool HasItem(string itemId)
    {
        foreach (var slot in _slots)
        {
            if (slot.Item?.Id == itemId) return true;
        }
        return false;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= _slots.Length) return;
        if (index == _activeSlotIndex) return;
        
        _activeSlotIndex = index;
        EmitSignal(SignalName.ActiveSlotChanged, _activeSlotIndex, ActiveItem);
        Log($"Selected slot {_activeSlotIndex}: {ActiveItem?.DisplayName ?? "empty"}");
    }

    public void SelectNext()
    {
        int next = (_activeSlotIndex + 1) % _slots.Length;
        SelectSlot(next);
    }

    public void SelectPrevious()
    {
        int prev = (_activeSlotIndex - 1 + _slots.Length) % _slots.Length;
        SelectSlot(prev);
    }

    private void Log(string message)
    {
        if (DebugLog) GD.Print($"[Inventory] {message}");
    }
}
