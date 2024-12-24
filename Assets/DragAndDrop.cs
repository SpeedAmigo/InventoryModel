using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rectTransform;
    
    public bool isHeld;
    public bool flipped;
    
    private Vector2 _originalPosition;
    private CellScript[] _currentCells;

    [SerializeField] private Vector2 boxSize;

    private void OverlapCalculations()
    {
        float rotationAngle = _rectTransform.rotation.eulerAngles.z;
        
        Collider2D[] hits = Physics2D.OverlapBoxAll
        (
            _rectTransform.position,
            new Vector2(_rectTransform.rect.size.x * boxSize.x, _rectTransform.rect.size.y * boxSize.y),
            rotationAngle
        );
            
        // Reset previously highlighted cells
        if (_currentCells != null)
        {
            foreach (var cell in _currentCells)
            {
                cell.isHit = false;
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
                }
            }
        }
    }
    
    private void SetCellToFalse()
    {
        foreach (var cell in _currentCells)
        {
            cell.isHit = false;
        }
        _currentCells = null;
    }
    
    private void Update()
    {
        
        if (isHeld)
        {
            _rectTransform.position = Input.mousePosition;
            
            OverlapCalculations();
        }
        
        if (isHeld && Input.GetMouseButtonDown(1))
        {
            isHeld = false;
            _rectTransform.position = _originalPosition;
            
            SetCellToFalse();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isHeld)
            {
                _originalPosition = _rectTransform.position;
                _rectTransform.position = Input.mousePosition;
            }
            
            if (isHeld && _currentCells != null)
            {
                SetCellToFalse();
            }

            isHeld = !isHeld;
        }
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
