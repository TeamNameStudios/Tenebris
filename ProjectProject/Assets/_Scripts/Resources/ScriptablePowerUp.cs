using UnityEngine;

[CreateAssetMenu(fileName = "New Power Up level", menuName = "Create Power Up level")]
public class ScriptablePowerUp : ScriptableObject
{
    public PowerUpEnum ID;
    public int Level;
    public int PageCost;
    public int PowerUpPercentage;
}

public enum PowerUpEnum
{
    DASH_SPEED,
    DASH_COOLDOWN,
    DASH_CORRUPTION_USAGE,
    GRAPPLE_SPEED,
    GRAPPLE_LAUNCH_SPEED,
    GRAPPLE_CORRUPTION_USAGE,

}