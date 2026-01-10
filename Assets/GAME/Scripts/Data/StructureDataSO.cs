using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace GAME.Scripts.Data
{
    [CreateAssetMenu(fileName = "StructureData", menuName = "Scriptable Objects/StructureSO")]
    public class StructureDataSO : ScriptableObject
    {
        public List<StructureLevelData> StructureLevels;
    }
    
    [Serializable]
    public class StructureLevelData
    {
        public Sprite StructureSprite;
        public InventoryItem ResourceTypeToUnlock;
        public int ResourceAmountToUnlock;
    }
}