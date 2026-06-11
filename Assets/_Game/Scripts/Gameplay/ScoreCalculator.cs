using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [Header("Config")]
    public int targetScore = 400;

    private int _totalScore = 0;

    void OnEnable()
    {
        EventBus.OnDartHit      += OnDartHit;
        EventBus.OnHandStarted  += OnHandStarted;
    }

    void OnDisable()
    {
        EventBus.OnDartHit      -= OnDartHit;
        EventBus.OnHandStarted  -= OnHandStarted;
    }

    void OnDartHit(DartHitData data)
    {
        _totalScore += data.basePoints;
        Debug.Log($"Score total: {_totalScore} | Objetivo: {targetScore}");

        if (_totalScore >= targetScore)
        {
            Debug.Log("¡Objetivo alcanzado!");
            EventBus.Publish_CombatCleared();
        }
    }

    void OnHandStarted()
    {
        // Score acumulado — no se resetea por mano
    }

    public void ResetScore()
    {
        _totalScore = 0;
    }

    public int GetTotalScore()  => _totalScore;
    public int GetTargetScore() => targetScore;
}