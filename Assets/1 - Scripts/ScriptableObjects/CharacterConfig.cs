using UnityEngine;

[CreateAssetMenu(fileName = "Wodopo/CharacterConfig")]
public class CharacterConfig : ScriptableObject
{
    [Header("Ground")]
    public float horizontalGroundSpeed = 5.0f;
    public float horizontalGroundAcceleration = 50.0f;
    public float jumpForce = 15.0f;

    [Header("Air")]
    public float horizontalAirSpeed = 5.0f;
    public float horizontalAirAcceleration = 50.0f;
}
