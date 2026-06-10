using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Config")]
    public int maxHands = 4;
    public int dartsPerHand = 3;

    private int _handsRemaining;
    private int _dartsRemaining;
    private int _currentHandIndex;

    void OnEnable()
    {
        EventBus.OnLevelCleared += OnLevelCleared;
    }

    void OnDisable()
    {
        EventBus.OnLevelCleared -= OnLevelCleared;
    }

    void OnLevelCleared()
    {
        enabled = false;
    }

    void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        _handsRemaining = maxHands;
        _currentHandIndex = 0;
        StartHand();
    }

    void StartHand()
    {
        _dartsRemaining = dartsPerHand;
        ClearDarts();
        EventBus.Publish_HandStarted();
        Debug.Log($"Mano {_currentHandIndex + 1} | Dardos: {_dartsRemaining} | Manos restantes: {_handsRemaining}");
    }

    void ClearDarts()
    {
        GameObject[] darts = GameObject.FindGameObjectsWithTag("Dart");
        foreach (GameObject dart in darts)
            Destroy(dart);
    }

    public void UseDart()
    {
        _dartsRemaining--;
        Debug.Log($"Dardo usado | Quedan: {_dartsRemaining}");
    }

    public void CheckHandEnd()
    {
        if (_dartsRemaining <= 0)
            EvaluateHand();
    }
    void EvaluateHand()
    {
        _currentHandIndex++;
        _handsRemaining--;

        if (_handsRemaining <= 0)
        {
            Debug.Log("Run terminado");
            EventBus.Publish_RunEnded();
        }
        else
        {
            Debug.Log($"Manos restantes: {_handsRemaining}");
            StartHand();
        }
    }

    public int GetHandsRemaining() => _handsRemaining;
    public int GetDartsRemaining() => _dartsRemaining;
    public int GetCurrentHandIndex() => _currentHandIndex;
}