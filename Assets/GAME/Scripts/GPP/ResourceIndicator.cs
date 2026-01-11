using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class ResourceIndicator : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public ResourceDataSO ResourceData;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMMEvent(MMInventoryEvent eventType)
    {
        Debug.Log(eventType);
        if (eventType.EventItem == ResourceData.ResourceItem)
        {
            Debug.Log("Added Item");
        }
    }
}
