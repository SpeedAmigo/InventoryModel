using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Image _image;
    private DragAndDrop _dragAndDrop;
    
    [SerializeField] private ItemSO _itemSO;
    
    private int _oneUnitSize = 85;
    private bool _coroutineRunning = false;

    private void SetObjectSize()
    {
        _rectTransform.sizeDelta = new Vector2(_oneUnitSize * _itemSO.itemSize.x, _oneUnitSize * _itemSO.itemSize.y);
    }

    private void SetItemImage()
    {
        _image.sprite = _itemSO.sprite;
    }
    
    private void SpaceNeedInt()
    {
        int space = _itemSO.itemSize.x * _itemSO.itemSize.y;
        _dragAndDrop.spaceNeed = space;
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
            _dragAndDrop.flipped = false;
        }
        else
        {
            targetRotation = secondRotation;
            _dragAndDrop.flipped = true;
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
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _dragAndDrop = GetComponent<DragAndDrop>();
        _image = GetComponent<Image>();
        
        SetObjectSize();
        SpaceNeedInt();
        SetItemImage();
        
        _dragAndDrop.item = _itemSO;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _dragAndDrop.isHeld && !_coroutineRunning)
        {
            StartCoroutine(RotateItem());
        }
    }
}
