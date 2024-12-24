using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rectTransform;
    
    public bool isHeld;
    
    private Vector2 _originalPosition;
    private CellScript[] _currentCells;

    [SerializeField] private Vector2 boxSize;
    
    private void Update()
    {
        if (isHeld)
        {
            _rectTransform.position = Input.mousePosition;
            
            Collider2D[] hits = Physics2D.OverlapBoxAll
            (
                _rectTransform.position,
                new Vector2(_rectTransform.sizeDelta.x * boxSize.x, _rectTransform.sizeDelta.y * boxSize.y),
                0
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
        
        if (isHeld && Input.GetMouseButtonDown(1))
        {
            isHeld = false;
            _rectTransform.position = _originalPosition;
            
            //_currentCell.isHit = false;
            //_currentCell = null;
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

            isHeld = !isHeld;
        }
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
