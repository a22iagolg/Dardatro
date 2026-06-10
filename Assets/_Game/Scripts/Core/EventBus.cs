using System;
using UnityEngine;

public static class EventBus
{
    // --- Dardo ---
    public static event Action<DartHitData> OnDartHit;

    // --- Mano ---
    public static event Action<int> OnHandCompleted;
    public static event Action OnHandFailed;
    public static event Action OnHandStarted;

    // --- Run ---
    public static event Action OnLevelStarted;
    public static event Action OnRunEnded;

    // --- Publishers ---
    public static void Publish_DartHit(DartHitData data) => OnDartHit?.Invoke(data);
    public static void Publish_HandCompleted(int score)  => OnHandCompleted?.Invoke(score);
    public static void Publish_HandFailed()              => OnHandFailed?.Invoke();
    public static void Publish_HandStarted()             => OnHandStarted?.Invoke();
    public static void Publish_LevelStarted()            => OnLevelStarted?.Invoke();
    public static void Publish_RunEnded()                => OnRunEnded?.Invoke();
}

public struct DartHitData
{
    public int basePoints;
    public bool isBullseye;
    public bool isWood;
    public Vector2 hitPosition;
    public int handIndex;
}