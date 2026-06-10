using UnityEngine;

public class DartLauncher : MonoBehaviour
{
    [Header("Referencias")]
    public AimSystem aimSystem;
    public Target target;
    public GameObject dartPrefab;
    public HandManager handManager;


    [Header("Desviación máxima en unidades")]
    public float maxDeviation = 1.5f;

    void OnEnable()
    {
        EventBus.OnRunEnded += OnRunEnded;
        EventBus.OnLevelCleared += OnLevelCleared;

    }

    void OnDisable()
    {
        EventBus.OnRunEnded -= OnRunEnded;
        EventBus.OnLevelCleared -= OnLevelCleared;
    }

    void OnRunEnded()
    {
        enabled = false;
    }
    void OnLevelCleared()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        switch (aimSystem.currentPhase)
        {
            case AimSystem.AimPhase.Phase1_Move:
                aimSystem.LockPosition();
                break;

            case AimSystem.AimPhase.Phase2_Bar:
                Shoot();
                break;
        }
    }

    void Shoot()
    {
        float accuracy = aimSystem.GetBarAccuracy(); // 0=perfecto, 1=máximo error
        Vector2 locked = aimSystem.GetLockedPosition();

        // Desviación aleatoria escalada por accuracy
        Vector2 deviation = Random.insideUnitCircle * (maxDeviation * accuracy);
        Vector2 hitPos = locked + deviation;

        Instantiate(dartPrefab, hitPos, Quaternion.identity);

        ScoreResult result = target.Evaluate(hitPos);

        float multiplier = 1f;
        if (aimSystem.isPerfectAim) multiplier = 1.5f;

        handManager.UseDart();

        DartHitData hitData = new DartHitData
        {
            basePoints = Mathf.RoundToInt(result.points * multiplier),
            isBullseye = result.isBullseye,
            isWood = result.isWood,
            hitPosition = hitPos,
            handIndex = handManager.GetCurrentHandIndex(),
            isPerfectAim = aimSystem.isPerfectAim
        };

        EventBus.Publish_DartHit(hitData);
        handManager.CheckHandEnd();


        Debug.Log($"Puntos: {hitData.basePoints} | Locked: {locked} | Accuracy: {accuracy:F2} | PerfectAim: {aimSystem.isPerfectAim} | GoodAim: {aimSystem.isGoodAim}");

        aimSystem.ResetAim();
    }
}