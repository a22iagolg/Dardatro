using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Referencias visuales")]
    public SpriteRenderer bullseyeVisual;
    public SpriteRenderer innerVisual;
    public SpriteRenderer middleVisual;

    [Header("Puntuación")]
    public int bullseyePoints = 100;
    public int innerPoints    = 50;
    public int middlePoints   = 20;
    public int outerPoints    = 0;

    private float _bullseyeRadius;
    private float _innerRadius;
    private float _middleRadius;

    void Start()
    {
        _bullseyeRadius = bullseyeVisual.bounds.extents.x;
        _innerRadius    = innerVisual.bounds.extents.x;
        _middleRadius   = middleVisual.bounds.extents.x;

        Debug.Log($"Radios — Bullseye: {_bullseyeRadius} | Inner: {_innerRadius} | Middle: {_middleRadius}");
    }

    public ScoreResult Evaluate(Vector2 hitPoint)
    {
        float dist = Vector2.Distance(hitPoint, transform.position);

        ScoreResult result = new ScoreResult();
        result.hitPosition = hitPoint;

        if (dist <= _bullseyeRadius)
        {
            result.points     = bullseyePoints;
            result.isBullseye = true;
        }
        else if (dist <= _innerRadius)
        {
            result.points = innerPoints;
        }
        else if (dist <= _middleRadius)
        {
            result.points = middlePoints;
        }
        else
        {
            result.points = outerPoints;
            result.isWood  = true;
        }

        return result;
    }
}

public struct ScoreResult
{
    public int points;
    public bool isBullseye;
    public bool isWood;
    public Vector2 hitPosition;
}