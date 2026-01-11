using System;
using System.Collections.Generic;
using GAME.Scripts;
using UnityEngine;

[RequireComponent(typeof(StructureView))]
public class SafeAreaView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public List<float> Radius = new List<float>();
    [SerializeField] private SpriteRenderer Visual;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Visual == null)
        {
            Visual = GetComponent<SpriteRenderer>();
        }
        
        
        var StructureView = GetComponent<StructureView>();
        int levelIndex = StructureView._currentLevelIndex;
        Visual.transform.localScale = Radius[levelIndex] * Vector3.one;
        
        Gameconstants.OnLevelUp.AddListener(OnLevelUp);
    }

    private void OnDestroy()
    {
        Gameconstants.OnLevelUp.RemoveListener(OnLevelUp);
    }

    private void OnLevelUp(int arg)
    {
        var StructureView = GetComponent<StructureView>();
        int levelIndex = StructureView._currentLevelIndex;
        Visual.transform.localScale = Radius[levelIndex] * Vector3.one;
    }
}
