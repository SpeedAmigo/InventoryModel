using UnityEngine;
using UnityEngine.EventSystems;

public class GroundItem : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemData;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        ItemManager.Instance.TryAddItem(this);
    }
}
