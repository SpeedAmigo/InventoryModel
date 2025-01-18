using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public ItemSO itemSo;
    
    private RectTransform _rectTransform;
    private Image _image;
    
    private int _oneUnitSize = 85;
    
    public void SetObjectSize()
    {
        _rectTransform.sizeDelta = new Vector2(_oneUnitSize * itemSo.itemSize.x, _oneUnitSize * itemSo.itemSize.y);
    }

    private void SetItemImage()
    {
        _image.sprite = itemSo.sprite;
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        SetObjectSize();
        SetItemImage();
    }
}
