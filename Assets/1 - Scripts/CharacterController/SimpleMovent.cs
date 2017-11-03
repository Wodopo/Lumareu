using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wodopo.Engine2D;

public class SimpleMovent : MonoBehaviour
{
    [Header("Inputs")]
    public Buttons moveRight;
    public Buttons moveLeft;
    public Buttons jump;

    [Header("Parameters")]
    public float jumpForce;
    public float xSpeed;
    public float xAcceleration;
    public float xDeAcceleration;

    private InputState inputState;
    private PhysicsObject body;

    private void Awake()
    {
        inputState = GetComponent<InputState>();
        body = GetComponent<PhysicsObject>();
    }
    
    void Update ()
    {
        if (inputState.GetButtonDown(jump) && body.collisionInfo.down)
            body.Velocity.y += jumpForce;

        float desiredX = 0.0f;
        desiredX += inputState.GetButton(moveRight) ? 1.0f : 0.0f;
        desiredX += inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
        desiredX *= xSpeed;

        body.Velocity.x = Mathf.MoveTowards(body.Velocity.x, desiredX, Time.deltaTime * (desiredX != 0.0f ? xAcceleration : xDeAcceleration));

    }
}
