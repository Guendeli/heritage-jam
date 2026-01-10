using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;



public class ResourceInteractable : MonoBehaviour
{
    [Header("Settings")]
    public ResourceDataSO ResourceSettingData;
    [Header("Dependencies")]
    [SerializeField] private MMFloatingTextSpawner _FloatingTextSpawner;
    
    private float _gatheringTimer;

    public void Reset()
    {
        _gatheringTimer = 0;
    }
    
    private void Tick()
    {
        _gatheringTimer += Time.deltaTime;
        if (_gatheringTimer >= ResourceSettingData.GatheringDuration)
        {
            // Feedback
            _FloatingTextSpawner.Spawn(ResourceSettingData.ResourceAmountPerGathering, transform.position);
            Reset();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.MMGetComponentNoAlloc<Character>() == null)
        {
            return; 
        }
        
        Tick();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.MMGetComponentNoAlloc<Character>() == null)
        {
            return; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.MMGetComponentNoAlloc<Character>() == null)
        {
            return; 
        }
        
        Reset();
        
    }

    public void DebugTest()
    {
        Debug.Log(string.Format("Gathered {0} resources!", ResourceSettingData.ResourceAmountPerGathering));
    }
    
}
