using UnityEngine;

public class DartLauncher : MonoBehaviour
{
    [Header("Referencias")]
    public AimSystem aimSystem;
    public Target target;
    public GameObject dartPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        switch (aimSystem.currentPhase)
        {
            case AimSystem.AimPhase.Phase1_Select:
                aimSystem.StartPhase2();
                break;

            case AimSystem.AimPhase.Phase2_Precision:
                Shoot();
                break;
        }
    }

    void Shoot()
    {
        if (Time.time - aimSystem.phase2StartTime < aimSystem.minTimeBeforeShoot)
            return;

        Vector2 hitPos = aimSystem.aimPosition;

        // Instanciar dardo visual
        Instantiate(dartPrefab, hitPos, Quaternion.identity);

        // Evaluar puntuación
        ScoreResult result = target.Evaluate(hitPos);

        // Publicar evento — aquí es donde los jokers y el ScoreCalculator escucharán
        DartHitData hitData = new DartHitData
        {
            basePoints   = result.points,
            isBullseye   = result.isBullseye,
            isWood       = result.isWood,
            hitPosition  = result.hitPosition,
            handIndex    = 0  // HandManager lo rellenará cuando lo tengamos
        };

        EventBus.Publish_DartHit(hitData);
        Debug.Log($"Impacto en {hitPos} | Puntos: {result.points} | Bullseye: {result.isBullseye} | Madera: {result.isWood}");

        aimSystem.ResetAim();
    }
}