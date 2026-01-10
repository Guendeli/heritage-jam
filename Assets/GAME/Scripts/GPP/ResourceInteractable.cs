using System;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
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

    [Header("Init Settings")] 
    private int _initialQuantity;
    private Inventory _targetInventory;

    public void Setup()
    {
        
        _initialQuantity = ResourceSettingData.Quantity;
    }

    private void Start()
    {
        Setup();
    }

    private void Tick()
    {
        _gatheringTimer += Time.deltaTime;
        if (_gatheringTimer >= ResourceSettingData.GatheringDuration)
        {
            // Feedback
            _FloatingTextSpawner.Spawn(ResourceSettingData.ResourceAmountPerGathering, transform.position);
            // Logic
            Pick("MainInventory");
            _gatheringTimer = 0;
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
        
        _gatheringTimer = 0;
        
    }
    
    // REgion - Inventory Stuff
    public void Pick(string targetInventoryName, string playerID = "Player1")
    {
        FindTargetInventory(targetInventoryName, playerID);
        if (_targetInventory == null)
        {
            return;
        }
        

        if (!Application.isPlaying)
        {
            if (!ResourceSettingData.ResourceItem.ForceSlotIndex)
            {
                _targetInventory.AddItem(ResourceSettingData.ResourceItem, 1);	
            }
            else
            {
                _targetInventory.AddItemAt(ResourceSettingData.ResourceItem, 1, ResourceSettingData.ResourceItem.TargetIndex);
            }
        }				
        else
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, ResourceSettingData.ResourceItem.TargetInventoryName, ResourceSettingData.ResourceItem, ResourceSettingData.ResourceAmountPerGathering, 0, playerID);
            _initialQuantity -= ResourceSettingData.ResourceAmountPerGathering;
            if (_initialQuantity <= 0)
            {
                Destroy(gameObject);
            }
        }				
        if (ResourceSettingData.ResourceItem.Pick(playerID))
        {
            
        }			
    }
    
    /// <summary>
    /// Finds the target inventory based on its name
    /// </summary>
    /// <param name="targetInventoryName">Target inventory name.</param>
    public virtual void FindTargetInventory(string targetInventoryName, string playerID = "Player1")
    {
        _targetInventory = null;
        _targetInventory = Inventory.FindInventory(targetInventoryName, playerID);
    }
}
