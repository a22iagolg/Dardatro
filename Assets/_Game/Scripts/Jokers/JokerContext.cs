using UnityEngine;

/// <summary>
/// Estado acumulado que se pasa por todos los jokers en el chain.
/// Se crea por cada DartHit, pero los contadores de mano persisten en JokerInventory.
/// </summary>
public class JokerContext
{
    // --- Puntuación del dardo actual (modificable por jokers) ---
    public int   bonusPoints = 0;    // Puntos extra sumados por jokers
    public float multiplier  = 1f;   // Multiplicador acumulado por jokers

    // --- Info del dardo actual (read-only para jokers) ---
    public DartHitData dartData;

    // --- Acumulados de la mano actual ---
    public int bullseyesThisHand   = 0;
    public int perfectAimsThisHand = 0;
    public int dartsHitThisHand    = 0;
    public int bullseyeStreak      = 0;  // Racha consecutiva, se rompe si no bullseye

    // --- Estado del run ---
    public int handsRemaining = 0;
    public int dartsRemaining = 0;
    public int currentCoins   = 0;
    public int currentScore   = 0;
    public int targetScore    = 0;

    // --- Flags para comunicación entre jokers en el mismo chain ---
    private System.Collections.Generic.Dictionary<string, bool> _flags
        = new System.Collections.Generic.Dictionary<string, bool>();

    public void SetFlag(string key, bool value) => _flags[key] = value;
    public bool GetFlag(string key) => _flags.TryGetValue(key, out bool v) && v;

    // --- Puntos finales tras el chain ---
    public int FinalPoints => Mathf.RoundToInt((dartData.basePoints + bonusPoints) * multiplier);
}