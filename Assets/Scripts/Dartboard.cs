using UnityEngine;

public class Dartboard : MonoBehaviour
{
    public SpriteRenderer innerinnerVisual;
    public SpriteRenderer innerVisual;
    public SpriteRenderer middleVisual;

    private float outerRadius;
    private float innerinnerRadius;
    private float innerRadius;
    private float middleRadius;

    void Start()
    {
        // Radio real del círculo grande
        outerRadius = GetComponent<SpriteRenderer>().bounds.extents.x;

        // Proporciones
        innerinnerRadius = outerRadius * 0.1f;
        innerRadius = outerRadius * 0.4f;
        middleRadius = outerRadius * 0.9f;

        // Ajustar tamaño usando bounds
        SetCircleSize(innerinnerVisual, innerinnerRadius);
        SetCircleSize(innerVisual, innerRadius);
        SetCircleSize(middleVisual, middleRadius);
    }
    // Test Commit
    void SetCircleSize(SpriteRenderer sr, float radius)
    {
        float currentSize = sr.bounds.size.x;
        float targetSize = radius * 2f;

        float scaleFactor = targetSize / currentSize;
        sr.transform.localScale *= scaleFactor;
    }

    public int GetScore(Vector2 hitPoint)
    {
        float dist = Vector2.Distance(hitPoint, transform.position);

        if (dist <= innerinnerRadius)
            return 100;
        else if (dist <= innerRadius)
            return 50;
        else if (dist <= middleRadius)
            return 20;
        else
            return 0;
    }
}