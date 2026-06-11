using System;
using UnityEngine;

public static class EventBus
{
    // --- Dardo ---
    public static event Action<DartHitData> OnDartHit;

    // --- Mano ---
    public static event Action OnHandStarted;

    // --- Combate ---
    public static event Action OnCombatStarted;
    public static event Action OnCombatCleared;

    // --- Run (partida completa) ---
    public static event Action OnGameOver;

    // --- Publishers ---
    public static void Publish_DartHit(DartHitData data)  => OnDartHit?.Invoke(data);
    public static void Publish_HandStarted()               => OnHandStarted?.Invoke();
    public static void Publish_CombatStarted()             => OnCombatStarted?.Invoke();
    public static void Publish_CombatCleared()             => OnCombatCleared?.Invoke();
    public static void Publish_GameOver()                  => OnGameOver?.Invoke();
}

public struct DartHitData
{
    public int     basePoints;
    public bool    isBullseye;
    public bool    isWood;
    public Vector2 hitPosition;
    public int     handIndex;
    public bool    isPerfectAim;
}