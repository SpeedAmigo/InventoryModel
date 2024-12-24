using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellScript : MonoBehaviour
{
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _avalibleColor = Color.green;
    [SerializeField] private Color _occupiedColor = Color.red;

    [SerializeField] private bool _isOccupied;
    public bool isHit;
    
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        
        
        _baseColor = _image.color;
    }
/*
    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = _isOccupied ? _occupiedColor : _avalibleColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = _baseColor;
    }
*/
    private void Update()
    {
        if (isHit)
        {
            _image.color = _isOccupied ? _occupiedColor : _avalibleColor;
        }
        else
        {
            _image.color = _baseColor;
        }
    }

    public void ChangeColor()
    {
        _image.color = _isOccupied ? _occupiedColor : _avalibleColor;
    }
}
