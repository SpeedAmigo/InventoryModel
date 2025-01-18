using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemSO itemData;

    public void AddItem()
    {
        ItemManager.Instance.TryAddItem(this);
    }
}
