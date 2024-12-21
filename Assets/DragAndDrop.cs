using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rectTransform;
    public bool isHeld;
    private Vector2 _originalPosition;
    
    /// <summary>
    ///  saved for later maybe come in handy
    /// </summary>
    /*
    private Vector2 ScreenPoint()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvas.transform as RectTransform, 
            Input.mousePosition, 
            canvas.worldCamera, out var localPoint
        );
        
        return localPoint;
    }
    */
    
    private void Update()
    {
        if (isHeld)
        {
            //_rectTransform.anchoredPosition = ScreenPoint();
            _rectTransform.position = Input.mousePosition;
        }
        
        if (isHeld && Input.GetMouseButtonDown(1))
        {
            isHeld = false;
            //Debug.Log("back to previous position");
            _rectTransform.position = _originalPosition;
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
                //Debug.Log(_originalPosition);
            }

            isHeld = !isHeld;
            //Debug.Log(_isHeld? "StartedDrag" : "StoppedDrag");
        }
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
