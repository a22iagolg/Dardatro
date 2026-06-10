using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Config")]
    public int maxHands     = 2;
    public int dartsPerHand = 4;

    private int _handsRemaining;
    private int _dartsRemaining;
    private int _currentHandIndex;
    private int _handsModifier = 0;
    private int _dartsModifier = 0;
    private bool _levelCleared = false;

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
        _levelCleared = true;
    }

    void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        _levelCleared     = false;
        _handsRemaining   = maxHands + _handsModifier;
        _currentHandIndex = 0;
        StartHand();
    }

    void StartHand()
    {
        _dartsRemaining = dartsPerHand + _dartsModifier;
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

    public void ApplyModifiers(int handsModifier, int dartsModifier)
    {
        _handsModifier = handsModifier;
        _dartsModifier = dartsModifier;
    }

    public void UseDart()
    {
        _dartsRemaining--;
        Debug.Log($"Dardo usado | Quedan: {_dartsRemaining}");
    }

    public void CheckHandEnd()
    {
        if (_levelCleared) return;
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

    public int GetHandsRemaining()   => _handsRemaining;
    public int GetDartsRemaining()   => _dartsRemaining;
    public int GetCurrentHandIndex() => _currentHandIndex;
}