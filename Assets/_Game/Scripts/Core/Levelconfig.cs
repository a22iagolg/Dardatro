using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Identificación")]
    public string enemyName;
    public Sprite enemySprite;

    [Header("Objetivo")]
    public int targetScore;

    [Header("Diana")]
    [Tooltip("Si es null usa la diana por defecto asignada en el componente Target de la escena")]
    public TargetData targetData;

    [Header("Modificadores")]
    public int handsModifier = 0;
    public int dartsModifier = 0;

    [Header("Restricciones")]
    public List<string> restrictions;

    [Header("Tipo")]
    public bool isBoss;
}