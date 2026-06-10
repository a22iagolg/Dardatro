using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/ScenarioData")]
public class ScenarioData : ScriptableObject
{
    [Header("Identificación")]
    public string scenarioName;
    public Sprite background;

    [Header("Combates")]
    public LevelConfig enemy1;
    public LevelConfig enemy2;
    public LevelConfig boss;

    [Header("Entre combates")]
    public bool hasEventAfterEnemy1;
    public bool hasEventAfterEnemy2;
}