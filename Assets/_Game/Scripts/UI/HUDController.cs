using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI handsText;

    private int _totalScore = 0;

    void OnEnable()
    {
        EventBus.OnDartHit += OnDartHit;
    }

    void OnDisable()
    {
        EventBus.OnDartHit -= OnDartHit;
    }

    void OnDartHit(DartHitData data)
    {
        _totalScore += data.basePoints;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + _totalScore;
    }
}