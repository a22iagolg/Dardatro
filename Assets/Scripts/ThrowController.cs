using UnityEngine;

public class ThrowController : MonoBehaviour
{
    public AimController aim;
    public GameObject dartPrefab;
    public Dartboard dartboard;

    public int score;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        switch (aim.currentPhase)
        {
            case AimController.AimPhase.Phase1_Select:
                StartPhase2();
                break;

            case AimController.AimPhase.Phase2_Precision:
                Shoot();
                break;
        }
    }

    void StartPhase2()
    {
        aim.StartPhase2();
    }

    void Shoot()
    {
        // evitar disparo instantáneo
        if (Time.time - aim.phase2StartTime < aim.minTimeBeforeShoot)
            return;

        Vector2 hitPos = aim.aimPosition;

        Instantiate(dartPrefab, hitPos, Quaternion.identity);

        int points = dartboard.GetScore(hitPos);
        score += points;

        Debug.Log("Puntos: " + points + " | Total: " + score);

        aim.ResetAim();
    }
}