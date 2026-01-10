using GAME.Scripts.Data;
using UnityEngine;

public class StructureView : MonoBehaviour
{
    [Header("Settings")]
    public StructureDataSO structureDataSO;
    
    [Header("Initialization")]
    public int _currentLevelIndex = 0;

    [SerializeField]private SpriteRenderer _spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Inject();
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Inject()
    {
        if(_spriteRenderer == null) 
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    private void Setup()
    {
        if (structureDataSO.StructureLevels.Count <= _currentLevelIndex)
            return;
        
        _spriteRenderer.sprite = structureDataSO.StructureLevels[_currentLevelIndex].StructureSprite;
    }

    private void Upgrade()
    {
        _currentLevelIndex++;
        StructureLevelData levelData = structureDataSO.StructureLevels[_currentLevelIndex];
        if (levelData == null)
            return;
        _spriteRenderer.sprite = levelData.StructureSprite;
    }
}
