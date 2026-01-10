using MoreMountains.InventoryEngine;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Scriptable Objects/ResourceDataSO")]
public class ResourceDataSO : ScriptableObject
{
    [Header("Base Settings")]
    public InventoryItem ResourceItem;
    public float GatheringDuration;
    public int ResourceAmountPerGathering;
    public int Quantity;
    [Header("Cosmetic")]
    public Sprite ResourceIcon;
}

public enum ResourceType
{
    Wood,
    Stone,
    Food,
    Metal
}
