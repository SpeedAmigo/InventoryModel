using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float maxValue;
    
    private void Start()
    {
        slider.maxValue = maxValue;
    }

    private void Update()
    {
        slider.value = ItemManager.Instance.inventoryWeight;
    }
}
