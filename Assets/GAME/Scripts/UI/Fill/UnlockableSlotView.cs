using System;
using GAME.Scripts;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct UnlockableSlotData
{
    public Sprite Sprite;
    public float UnlockDuration;
    public int CostAmount;
}

public class UnlockableSlotView : MonoBehaviour
{
    
    [SerializeField] private Image _targetImage;
    [SerializeField] private UnityEvent _onFilled;
    [SerializeField] private Image _costType;
    [SerializeField] private Text _costTypeText;
    [SerializeField] private UnlockableSlotData _data;

    [Header("Spawn & Upgrade")] 
    public ResourceDataSO _resourceData;
    public Transform spawnPoint;
    public GameObject structurePrefab;

    private int _filledItems;
    private int _costAmount;
    private float _duration;
    private float _timer;
    private Inventory _cachedInventory;

    private void Start()
    {
        Setup();
    }

    public void Reset()
    {
        if (!_targetImage)
            return;
        
        Setup();
    }

    public void Setup()
    {
        _duration = _data.UnlockDuration;
        _costAmount = _data.CostAmount;
        _costType.sprite = _data.Sprite;
        _costTypeText.text = _costAmount.ToString();
        _targetImage.fillAmount = 0;
        _filledItems = 0;
        _timer = 0;
    }

    public void Tick()
    {
        if (_cachedInventory == null)
        {
            _cachedInventory = Inventory.FindInventory(Gameconstants.MAIN_INVENTORY_NAME, "Player1");
        }


        if (_filledItems < _costAmount)
        {
            if (_cachedInventory.RemoveItemByID(_resourceData.ResourceItem.ItemID, 1))
            {
                _filledItems++;
            }
            else
            {
                return;
            }
        }


        _timer += Time.deltaTime;
        var pct = Mathf.Clamp01(_timer / _duration);
        _targetImage.fillAmount = pct;

        var inverse = 1 - pct;

        int amountProgress = (int)(_costAmount * inverse);
        _costTypeText.text = amountProgress.ToString();
        if (_timer >= _duration)
        {
            _onFilled.Invoke();
            Destroy(gameObject);
        }
    }

    public void SpawnStructure()
    {
        Instantiate(structurePrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    
}
