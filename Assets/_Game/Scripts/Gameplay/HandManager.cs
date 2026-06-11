using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Config")]
    public int maxHands = 2;
    public int dartsPerHand = 4;

    [Header("Referencias")]
    public JokerInventory jokerInventory;

    private int _handsRemaining;
    private int _dartsRemaining;
    private int _currentHandIndex;
    private int _handsModifier = 0;
    private int _dartsModifier = 0;
    private bool _combatCleared = false;


    void Start() { StartCombat(); }

    public void StartCombat()
    {
        _combatCleared = false;
        _handsRemaining = maxHands + _handsModifier;
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
        CheckHandEnd();
    }

    public void CheckHandEnd()
    {
        if (_combatCleared) return;
        if (_dartsRemaining <= 0)
            EvaluateHand();
    }
    void EvaluateHand()
    {
        // Notificar a jokers antes de pasar a la siguiente mano
        jokerInventory?.ProcessHandComplete();

        _currentHandIndex++;
        _handsRemaining--;

        if (_handsRemaining <= 0)
        {
            EventBus.Publish_GameOver();
        }
        else
        {
            Debug.Log($"Manos restantes: {_handsRemaining}");
            StartHand();
        }
    }

    // --- Helpers para jokers ---
    public void AddDartsToCurrentHand(int amount)
    {
        _dartsRemaining += amount;
        Debug.Log($"[HandManager] +{amount} dardos | Quedan: {_dartsRemaining}");
    }

    public void AddHands(int amount)
    {
        _handsRemaining += amount;
        Debug.Log($"[HandManager] +{amount} manos | Quedan: {_handsRemaining}");
    }

    public int GetHandsRemaining() => _handsRemaining;
    public int GetDartsRemaining() => _dartsRemaining;
    public int GetCurrentHandIndex() => _currentHandIndex;
}