using UnityEngine;

/// <summary>
/// Clase base para todos los jokers.
/// Hereda de ScriptableObject — configurable en el Inspector.
/// Implementa solo los hooks que necesites, el resto no hace nada por defecto.
/// </summary>
public abstract class JokerBase : ScriptableObject
{
    [Header("Identidad")]
    public string jokerName;
    [TextArea]
    public string description;
    public Sprite icon;
    public JokerRarity rarity;

    // Se llama por cada dardo, ANTES de registrar el score
    // Modifica context.bonusPoints y/o context.multiplier
    public virtual void OnDartHit(JokerContext context) { }

    // Se llama al terminar una mano (sin dardos restantes)
    // Puede dar monedas, recuperar dardos, añadir manos, etc.
    public virtual void OnHandComplete(JokerContext context) { }

    // Se llama al empezar un combate
    public virtual void OnCombatStart(JokerContext context) { }

    // Se llama al superar un combate
    public virtual void OnCombatCleared(JokerContext context) { }

    // Se llama al game over (sin manos restantes)
    public virtual void OnGameOver(JokerContext context) { }
}

public enum JokerRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}