using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [SerializeField] private CellScript[] allCellScripts;
    [SerializeField] private GameObject itemsSlots;
    [SerializeField] private GameObject itemsParent;
    [SerializeField] private GameObject itemPrefab;
    
    public Dictionary<Vector2Int, CellScript> cellDictionary = new();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private bool CheckSpace(Vector2Int startPosition, Vector2Int itemSize, out List<CellScript> cellScripts)
    {
        List<CellScript> cellsInArea = new();

        for (int x = 0; x < itemSize.x; x++)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                Vector2Int currentPosition = new Vector2Int(startPosition.y + y, startPosition.x + x);

                if (cellDictionary.TryGetValue(currentPosition, out CellScript cellScript))
                {
                    cellsInArea.Add(cellScript);
                    
                    if (!cellScript || cellScript.isOccupied || cellScript.cellType != ObjectType.Any)
                    {
                        cellScripts = null;
                        return false;
                    }
                }
            }
        }

        if (cellsInArea.Count == SpaceNeed(itemSize))
        {
            cellScripts = cellsInArea;
            return true;
        }

        cellScripts = null;
        return false;
    }

    private int SpaceNeed(Vector2Int itemSize)
    {
        int need = itemSize.x * itemSize.y;
        return need;
    }

    public void TryAddItem(GroundItem groundItem)
    {
        bool spaceFound = false;
        
        for (int i = 0; i < allCellScripts.Length; i++)
        {
            if (CheckSpace(allCellScripts[i].gridPosition, groundItem.itemData.itemSize, out List<CellScript> cellScripts))
            {
                GameObject template = Instantiate(itemPrefab, itemsSlots.transform);
                ItemScript item = template.GetComponent<ItemScript>();
                item.itemSo = groundItem.itemData;
                item.enabled = true;
                DragAndDrop itemDrag = template.GetComponent<DragAndDrop>();
                itemDrag.enabled = true;
                
                itemDrag.passedCell = cellScripts[0];
                
                //Debug.Log(allCellScripts[i].gridPosition);
                foreach (CellScript cell in cellScripts)
                {
                    cell.isOccupied = true;
                }
                
                spaceFound = true;
                break;
            }
        }

        if (!spaceFound)
        {
            Debug.Log("there is no space to add item");
        }
    }
    
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
