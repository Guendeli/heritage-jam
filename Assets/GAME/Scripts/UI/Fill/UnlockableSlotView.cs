using System;
using System.Globalization;
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

    private int _costAmount;
    private float _duration;
    private float _timer;
    
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
        _timer = 0;
    }

    public void Tick()
    {
        _timer += Time.deltaTime;
        var pct = Mathf.Clamp01(_timer / _duration);
        _targetImage.fillAmount = pct;

        var inverse = 1 - pct;

        int amountProgress = (int)(_costAmount * inverse);
        _costTypeText.text = amountProgress.ToString();
        if (_timer >= _duration)
        {
            _onFilled.Invoke();
            this.enabled = false;
        }
    }
    
    
}
