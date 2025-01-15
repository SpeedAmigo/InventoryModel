using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject itemsSlots;
    [SerializeField] private GameObject itemsParent;

    private void OnEnable()
    {
        EventManager.ItemIsHeld += ChangeToParent;
        EventManager.ItemSnapped += ChangeToSlot;
    }

    private void OnDisable()
    {
        EventManager.ItemIsHeld -= ChangeToParent;
        EventManager.ItemSnapped -= ChangeToSlot;
    }


    private void ChangeToParent(Transform item)
    {
        item.SetParent(itemsParent.transform);
    }

    private void ChangeToSlot(Transform item)
    {
        item.SetParent(itemsSlots.transform);
    }
}
