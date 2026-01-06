namespace NewGame.Core.Inventory;

public class InventorySlot
{
    public ItemData Item { get; set; }
    public int Count { get; set; }
    
    public bool IsEmpty => Item == null || Count <= 0;
    
    public void Clear()
    {
        Item = null;
        Count = 0;
    }
}
