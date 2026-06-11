using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI handsText;
    public TextMeshProUGUI dartsText;
    public HandManager     handManager;
    public ScoreCalculator scoreCalculator;

    void OnEnable()
    {
        EventBus.OnDartHit       += OnDartHit;
        EventBus.OnHandStarted   += OnHandStarted;
        EventBus.OnGameOver      += OnGameOver;
        EventBus.OnCombatCleared += OnCombatCleared;
    }

    void OnDisable()
    {
        EventBus.OnDartHit       -= OnDartHit;
        EventBus.OnHandStarted   -= OnHandStarted;
        EventBus.OnGameOver      -= OnGameOver;
        EventBus.OnCombatCleared -= OnCombatCleared;
    }

    void OnDartHit(DartHitData data) { UpdateUI(); }
    void OnHandStarted()             { UpdateUI(); }

    void OnGameOver()
    {
        if (scoreText != null)
            scoreText.text = $"GAME OVER | Score: {scoreCalculator.GetTotalScore()} / {scoreCalculator.GetTargetScore()}";
    }

    void OnCombatCleared()
    {
        if (scoreText != null)
            scoreText.text = $"✓ COMBATE SUPERADO | Score: {scoreCalculator.GetTotalScore()} / {scoreCalculator.GetTargetScore()}";
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {scoreCalculator.GetTotalScore()} / {scoreCalculator.GetTargetScore()}";

        if (handsText != null)
            handsText.text = "Manos: " + handManager.GetHandsRemaining();

        if (dartsText != null)
            dartsText.text = "Dardos: " + handManager.GetDartsRemaining();
    }
}