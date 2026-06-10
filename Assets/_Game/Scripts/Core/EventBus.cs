using System;
using UnityEngine;

public static class EventBus
{
    // --- Dardo ---
    public static event Action<DartHitData> OnDartHit;

    // --- Mano ---
    public static event Action OnHandStarted;

    // --- Run ---
    public static event Action OnLevelStarted;
    public static event Action OnRunEnded;
    public static event Action OnLevelCleared;

    // --- Publishers ---
    public static void Publish_DartHit(DartHitData data) => OnDartHit?.Invoke(data);
    public static void Publish_HandStarted() => OnHandStarted?.Invoke();
    public static void Publish_LevelStarted() => OnLevelStarted?.Invoke();
    public static void Publish_RunEnded() => OnRunEnded?.Invoke();
    public static void Publish_LevelCleared() => OnLevelCleared?.Invoke();

}

public struct DartHitData
{
    public int basePoints;
    public bool isBullseye;
    public bool isWood;
    public Vector2 hitPosition;
    public int handIndex;
    public bool isPerfectAim;
}