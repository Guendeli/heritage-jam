using MoreMountains.TopDownEngine;
using UnityEngine;

public enum GamePhase
{
    Cozy,
    Pubg
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GamePhase CurrentGamePhase = GamePhase.Cozy;

    public float CozyTime;
    public float PubgTime;
    
    
    private SafeAreaView _safeAreaView;
    private CharacterController _characterController;
    private float _phaseTimer;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _phaseTimer = 0;
        CurrentGamePhase = GamePhase.Cozy;
        _safeAreaView = FindFirstObjectByType<SafeAreaView>();
        _characterController = FindFirstObjectByType<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentGamePhase)
        {
            case GamePhase.Cozy:
                break;
            case GamePhase.Pubg:
                _phaseTimer += Time.deltaTime;
                EvaluateSafeArea();
                if (_phaseTimer >= PubgTime)
                {
                    _phaseTimer = 0;
                    CurrentGamePhase = GamePhase.Cozy;
                }
                break;
        }
    }

    private void EvaluateSafeArea()
    {
        if (LevelManager.Instance.Players.Count <= 1)
            return;

        if (_safeAreaView == null)
        {
            _safeAreaView = FindFirstObjectByType<SafeAreaView>();
        }
        
        Character player = LevelManager.Instance.Players[0];
        
        float distance = Vector3.Distance(player.transform.position, _safeAreaView.transform.position);

        
    }
}
