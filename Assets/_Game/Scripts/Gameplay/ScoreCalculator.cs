using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [Header("Config")]
    public int targetScore = 400;

    private int _totalScore = 0;

    void OnEnable()
    {
        EventBus.OnDartHit += OnDartHit;
        EventBus.OnHandStarted += OnHandStarted;
    }

    void OnDisable()
    {
        EventBus.OnDartHit -= OnDartHit;
        EventBus.OnHandStarted -= OnHandStarted;
    }

    void OnDartHit(DartHitData data)
    {
        _totalScore += data.basePoints;
        Debug.Log($"Score total: {_totalScore} | Objetivo: {targetScore}");

        if (_totalScore >= targetScore)
        {
            Debug.Log("¡Objetivo alcanzado!");
            EventBus.Publish_LevelCleared();
        }
    }

    void OnHandStarted()
    {
        // Ya no reseteamos — el score es acumulado
    }

    public bool EvaluateHand()
    {
        bool success = _totalScore >= targetScore;
        Debug.Log($"Mano evaluada | Score: {_totalScore} | Objetivo: {targetScore} | Superada: {success}");
        return success;
    }

    public void ResetScore()
    {
        _totalScore = 0;
    }

    public int GetTotalScore() => _totalScore;
    public int GetTargetScore() => targetScore;
}