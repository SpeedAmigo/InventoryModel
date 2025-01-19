using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item Scriptable Object")]
public class ItemSO : ScriptableObject
{
    public Sprite sprite;
    public Vector2Int itemSize;
    public ObjectType objectType;
    public float itemWeight;
}
