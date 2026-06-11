using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona los jokers activos del jugador.
/// Mantiene el estado acumulado entre dardos y manos.
/// DartLauncher y HandManager llaman a este componente para procesar eventos.
/// </summary>
public class JokerInventory : MonoBehaviour
{
    [Header("Config")]
    public int maxJokers = 5;

    [Header("Jokers activos")]
    public List<JokerBase> activeJokers = new List<JokerBase>();

    [Header("Referencias")]
    public HandManager     handManager;
    public ScoreCalculator scoreCalculator;

    // --- Estado acumulado de la mano actual ---
    private int _bullseyesThisHand   = 0;
    private int _perfectAimsThisHand = 0;
    private int _dartsHitThisHand    = 0;
    private int _bullseyeStreak      = 0;

    // --- Economía ---
    private int _currentCoins = 0;

    void OnEnable()
    {
        EventBus.OnCombatStarted += OnCombatStarted;
        EventBus.OnCombatCleared += OnCombatCleared;
        EventBus.OnGameOver      += OnGameOver;
    }

    void OnDisable()
    {
        EventBus.OnCombatStarted -= OnCombatStarted;
        EventBus.OnCombatCleared -= OnCombatCleared;
        EventBus.OnGameOver      -= OnGameOver;
    }

    // --- Llamado por DartLauncher ANTES de publicar OnDartHit ---
    public int ProcessDartHit(DartHitData rawData)
    {
        // Actualizar contadores
        if (!rawData.isWood)    _dartsHitThisHand++;
        if (rawData.isBullseye) { _bullseyesThisHand++; _bullseyeStreak++; }
        else                    { _bullseyeStreak = 0; }
        if (rawData.isPerfectAim) _perfectAimsThisHand++;

        JokerContext ctx = BuildContext(rawData);

        foreach (var joker in activeJokers)
            joker.OnDartHit(ctx);

        return ctx.FinalPoints;
    }

    // --- Llamado por HandManager al terminar una mano ---
    public void ProcessHandComplete()
    {
        JokerContext ctx = BuildContext(default);

        foreach (var joker in activeJokers)
            joker.OnHandComplete(ctx);

        // Resetear contadores de mano
        _bullseyesThisHand   = 0;
        _perfectAimsThisHand = 0;
        _dartsHitThisHand    = 0;
        // La racha NO se resetea entre manos — solo entre combates
    }

    // --- Helpers para jokers ---
    public void AddCoins(int amount)
    {
        _currentCoins += amount;
        Debug.Log($"[JokerInventory] +{amount} monedas | Total: {_currentCoins}");
    }

    public int GetCoins() => _currentCoins;

    public bool AddJoker(JokerBase joker)
    {
        if (activeJokers.Count >= maxJokers)
        {
            Debug.Log("[JokerInventory] No hay slots libres");
            return false;
        }
        activeJokers.Add(joker);
        return true;
    }

    public void RemoveJoker(JokerBase joker) => activeJokers.Remove(joker);

    // --- Construcción del contexto ---
    private JokerContext BuildContext(DartHitData dartData)
    {
        return new JokerContext
        {
            dartData             = dartData,
            bullseyesThisHand   = _bullseyesThisHand,
            perfectAimsThisHand = _perfectAimsThisHand,
            dartsHitThisHand    = _dartsHitThisHand,
            bullseyeStreak      = _bullseyeStreak,
            handsRemaining      = handManager     != null ? handManager.GetHandsRemaining()   : 0,
            dartsRemaining      = handManager     != null ? handManager.GetDartsRemaining()   : 0,
            currentCoins        = _currentCoins,
            currentScore        = scoreCalculator != null ? scoreCalculator.GetTotalScore()   : 0,
            targetScore         = scoreCalculator != null ? scoreCalculator.GetTargetScore()  : 0,
        };
    }

    // --- Eventos ---
    void OnCombatStarted()
    {
        JokerContext ctx = BuildContext(default);
        foreach (var joker in activeJokers)
            joker.OnCombatStart(ctx);

        // Resetear racha al empezar nuevo combate
        _bullseyeStreak = 0;
    }

    void OnCombatCleared()
    {
        JokerContext ctx = BuildContext(default);
        foreach (var joker in activeJokers)
            joker.OnCombatCleared(ctx);
    }

    void OnGameOver()
    {
        JokerContext ctx = BuildContext(default);
        foreach (var joker in activeJokers)
            joker.OnGameOver(ctx);
    }
}