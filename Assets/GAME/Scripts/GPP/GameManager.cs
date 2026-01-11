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
                if (_phaseTimer >= PubgTime)
                {
                    _phaseTimer = 0;
                    CurrentGamePhase = GamePhase.Cozy;
                }
                break;
        }
    }
}
