using UnityEngine;


[CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Scriptable Objects/ResourceDataSO")]
public class ResourceDataSO : ScriptableObject
{
    [Header("Base Settings")]
    public float GatheringDuration;
    public int ResourceAmountPerGathering;
    [Header("Cosmetic")]
    public Sprite ResourceIcon;
}
