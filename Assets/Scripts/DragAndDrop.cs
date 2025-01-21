using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public CellScript passedCell;
    [HideInInspector] public RectTransform _rectTransform;
    [HideInInspector] public GameObject _box;
    [HideInInspector] public ItemSO item;
    [HideInInspector] public bool isHeld;
    [HideInInspector] public bool flipped;
    [HideInInspector] public int spaceNeed;
    
    private Canvas _canvas;
    private ItemScript _itemScript;
    private bool _coroutineRunning = false;
    private CellScript[] _currentCells;
    private Vector2 _originalMultiplier;
    private Vector2 _boxMultiplier;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _itemScript = GetComponent<ItemScript>();
    }
    
    private void Start()
    {
        item = _itemScript.itemSo;
        
        SpaceNeedInt();
        SetMultiplier(item.itemSize);
        CreateBox();
        
        if (passedCell != null)
        {
            PositionAfterAdding(passedCell);
        }
    }
    
    private void Update()
    {
        HandleRotationInput();
        HandleDragging();
        HandleRelease();
    }
    #region UpdateMethods

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && isHeld && !_coroutineRunning)
        {
            StartCoroutine(RotateItem());
        }
    }

    private void HandleDragging()
    {
        if (!isHeld) return;
        
        _rectTransform.position = Input.mousePosition;
        MultiplierChanger();
        OverlapCalculations();
    }
    
    private void HandleRelease()
    {
        if (isHeld && Input.GetMouseButtonDown(0))
        {
            if (IsOutsideOfBox())
            {
                ItemManager.Instance.RemoveItemFromList(this);
                Destroy(gameObject);
            }
        }
    }
    #endregion
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        BeginDrag();
        TrySnapToCell();
        TryPlaceInSocket();
    }
    #region PointerClickMethods
    private void BeginDrag()
    {
        if (!isHeld)
        {
            _rectTransform.position = Input.mousePosition;
            isHeld = !isHeld;
            _itemScript.SetObjectSize();
            EventManager.InvokeItemIsHeld(transform);
        }
    }
    
    private void TrySnapToCell()
    {
        if (isHeld && _currentCells != null && _currentCells.Length == spaceNeed && AllCellsNotOccupied() && AllCellsAnyType())
        {
            SnapToTheNearestCell();
            SetCellToFalse();
            isHeld = !isHeld;
            EventManager.InvokeItemSnapped(transform);
        }
    }

    private void TryPlaceInSocket()
    {
        if (isHeld && _currentCells != null && _currentCells.Length > 0 && _currentCells[0].cellType == item.objectType)
        {
            RectTransform parentRectTransform = _currentCells[0].GetComponent<RectTransform>();
            if (parentRectTransform != null)
            {
                _rectTransform.position = _currentCells[0].transform.position;
                _rectTransform.rotation = _currentCells[0].transform.rotation;
                    
                _rectTransform.sizeDelta = parentRectTransform.rect.size;
                    
                transform.SetParent(_currentCells[0].transform);
            }
                
            ItemManager.Instance.RemoveItemFromList(this);
                
            SetCellToFalse();
            isHeld = !isHeld;
        }
    }
    #endregion
    
    private void SetMultiplier(Vector2Int value)
    {
        Dictionary<int, float> multiplier = new Dictionary<int, float>
        {
            { 1, 0.0001f },
            { 2 , 0.5f},
            { 3 , 0.666f}
        };
        
        float xMultiplier = multiplier.ContainsKey(value.x) ? multiplier[value.x] : 1.0f;
        float yMultiplier = multiplier.ContainsKey(value.y) ? multiplier[value.y] : 1.0f;
        
        _boxMultiplier = new Vector2(xMultiplier, yMultiplier);
        _originalMultiplier = _boxMultiplier;
    }
    
    private Vector2 BoxScaledSize()
    {
        return new Vector2
        (
            _rectTransform.rect.size.x * _boxMultiplier.x * _canvas.scaleFactor,
            _rectTransform.rect.size.y * _boxMultiplier.y * _canvas.scaleFactor
        );
    }
    
    private void OverlapCalculations()
    {
        float rotationAngle = _rectTransform.rotation.eulerAngles.z;
        
        Collider2D[] hits = Physics2D.OverlapBoxAll
        (
            _rectTransform.position,
            BoxScaledSize(),
            rotationAngle
        );
        
        // Reset previously highlighted cells
        if (_currentCells != null)
        {
            foreach (var cell in _currentCells)
            {
                if (cell != null)
                {
                    cell.isHit = false;
                    if (cell.item == gameObject)
                    {
                        cell.isOccupied = false;
                    }
                }
            }
        }

        // Highlight new cells
        _currentCells = null;
        if (hits.Length > 0)
        {
            _currentCells = new CellScript[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out CellScript cell))
                {
                    cell.isHit = true;
                    _currentCells[i] = cell;
                    if (cell.item == gameObject) // reset item occupancy
                    {
                        cell.isOccupied = false;
                        cell.isOccupied = false;
                    }
                }
            }
        }
    }
    
    private void SnapToTheNearestCell()
    {
        Vector3[] corners = new Vector3[4];
        _box.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 bottomLeftCorner = corners[0]; // bottom left is at index 0
            

        CellScript closestCell = null;
        float closestDistance = float.MaxValue;
            
        foreach (var cell in _currentCells)
        {
            float distance = Vector3.Distance(bottomLeftCorner, cell.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = cell;
            }
            
            cell.isOccupied = true;
            cell.isHit = false;
            cell.item = gameObject;
        }
        
        ItemManager.Instance.AddItemToList(this);

        if (closestCell != null)
        {
            Vector3 offset = _rectTransform.position - bottomLeftCorner;
                
            _rectTransform.position = closestCell.transform.position + offset;
        }
    }

    private void PositionAfterAdding(CellScript cell)
    {
        Vector3[] corners = new Vector3[4];
        _box.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 bottomLeftCorner = corners[0];
        
        Vector3 offset = _rectTransform.position - bottomLeftCorner;
        _rectTransform.position = cell.transform.position + offset;
    }
    
    private void SetCellToFalse()
    {
        if (_currentCells != null)
        {
            foreach (var cell in _currentCells)
            {
                cell.isHit = false;
            }
            _currentCells = null;
        }
    }
    
    private void MultiplierChanger()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(ItemManager.Instance.multiplierBox, Input.mousePosition))
        {
            _boxMultiplier = new Vector2(0.0001f, 0.0001f);
        }
        else
        {
            _boxMultiplier = _originalMultiplier;
        }
    }

    private bool IsOutsideOfBox()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(ItemManager.Instance.backgroundBox, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    private bool AllCellsNotOccupied()
    {
        if (_currentCells == null) return false;
        foreach (var cell in _currentCells)
        {
            if (cell.isOccupied)
            {
                return false;
            }
        }
        return true; 
    }

    private bool AllCellsAnyType()
    {
        if (_currentCells == null) return false;
        foreach (var cell in _currentCells)
        {
            if (cell.cellType != ObjectType.Any)
            {
                return false;
            }
        }
        return true;
    }
    
    private IEnumerator RotateItem()
    {
        _coroutineRunning = true;
        
        float elapsedTime = 0;
        float rotationTime = 0.3f;

        Quaternion targetRotation;
        
        Quaternion firstRotation = Quaternion.Euler(0, 0, 0);
        Quaternion secondRotation = Quaternion.Euler(0, 0, -90);

        if (_rectTransform.rotation != firstRotation)
        {
            targetRotation = firstRotation;
            flipped = false;
        }
        else
        {
            targetRotation = secondRotation;
            flipped = true;
        }
        
        while (elapsedTime < rotationTime)
        {
            _rectTransform.rotation = Quaternion.Lerp(_rectTransform.rotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        _rectTransform.rotation = targetRotation;
        _coroutineRunning = false;
    }
    
    private void SpaceNeedInt()
    {
        int space = item.itemSize.x * item.itemSize.y;
        spaceNeed = space;
    }

    private void CreateBox()
    {
        GameObject newBox = new GameObject("box", typeof(RectTransform));
        
        newBox.transform.SetParent(transform);
        newBox.transform.localPosition = Vector3.zero;
        
        RectTransform rectTransform = newBox.GetComponent<RectTransform>();
        rectTransform.sizeDelta = BoxScaledSize();
        
        _box = newBox;
    }
    
    private IEnumerator DelayOnFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OverlapCalculations();
        SnapToTheNearestCell();
    }

    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        StartCoroutine(DelayOnFrame());
    }
}
