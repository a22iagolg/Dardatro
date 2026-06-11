using UnityEngine;

public class DartLauncher : MonoBehaviour
{
    [Header("Referencias")]
    public AimSystem      aimSystem;
    public Target         target;
    public GameObject     dartPrefab;
    public HandManager    handManager;
    public JokerInventory jokerInventory;

    [Header("Desviación máxima en unidades")]
    public float maxDeviation = 1.5f;

    void OnEnable()
    {
        EventBus.OnGameOver      += OnGameOver;
        EventBus.OnCombatCleared += OnCombatCleared;
    }

    void OnDisable()
    {
        EventBus.OnGameOver      -= OnGameOver;
        EventBus.OnCombatCleared -= OnCombatCleared;
    }

    void OnGameOver()      { enabled = false; }
    void OnCombatCleared() { }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        switch (aimSystem.currentPhase)
        {
            case AimSystem.AimPhase.Phase1_Move: aimSystem.LockPosition(); break;
            case AimSystem.AimPhase.Phase2_Bar:  Shoot();                  break;
        }
    }

    void Shoot()
    {
        float   accuracy  = aimSystem.GetBarAccuracy();
        Vector2 locked    = aimSystem.GetLockedPosition();
        Vector2 deviation = Random.insideUnitCircle * (maxDeviation * accuracy);
        Vector2 hitPos    = locked + deviation;

        Instantiate(dartPrefab, hitPos, Quaternion.identity);

        ScoreResult result          = target.Evaluate(hitPos);
        float       baseMultiplier  = aimSystem.isPerfectAim ? 1.5f : 1f;

        handManager.UseDart();

        DartHitData hitData = new DartHitData
        {
            basePoints   = Mathf.RoundToInt(result.points * baseMultiplier),
            isBullseye   = result.isBullseye,
            isWood       = result.isWood,
            hitPosition  = hitPos,
            handIndex    = handManager.GetCurrentHandIndex(),
            isPerfectAim = aimSystem.isPerfectAim
        };

        // Chain de jokers — devuelve puntos finales procesados
        int finalPoints = jokerInventory != null
            ? jokerInventory.ProcessDartHit(hitData)
            : hitData.basePoints;

        // Publicar con puntos ya procesados
        DartHitData processedData  = hitData;
        processedData.basePoints   = finalPoints;
        EventBus.Publish_DartHit(processedData);

        handManager.CheckHandEnd();

        Debug.Log($"Puntos base: {hitData.basePoints} | Finales: {finalPoints} | PerfectAim: {aimSystem.isPerfectAim}");

        aimSystem.ResetAim();
    }
}