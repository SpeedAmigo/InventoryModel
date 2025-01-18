using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CellScript : MonoBehaviour
{
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _avalibleColor = Color.green;
    [SerializeField] private Color _occupiedColor = Color.red;

    public Vector2Int gridPosition;
    public ObjectType cellType;
    public bool isOccupied;
    public bool isHit;
    public GameObject item;
    
    private Image _image;

    public void SetGridPosition(Vector2Int position)
    {
        gridPosition = position;
        //Debug.Log(gridPosition);
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        
        _baseColor = _image.color;
    }
    private void Update()
    {
        if (isHit)
        {
            _image.color = isOccupied ? _occupiedColor : _avalibleColor;
        }
        else
        {
            _image.color = _baseColor;
        }
    }
}
