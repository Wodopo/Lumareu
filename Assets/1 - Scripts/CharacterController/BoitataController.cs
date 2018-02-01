using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoitataController : MonoBehaviour
{
    public Transform target;
    public Transform visual;
    public Animator anim;

    public float maxDistance;

    public float dampen;

    public float minSpeed;
    public float maxSpeed;

    private float _currentSpeed;

    public float Distance
    {
        get { return Vector2.Distance(target.position, transform.position); }
    }

    public Vector2 Direction
    {
        get { return (transform.position - target.position).normalized; }
    }

    public void Update()
    {
        float v = Mathf.Clamp01(Distance / maxDistance);
        transform.position = target.position + ((Direction * maxDistance) * v).ToVector3();

        _currentSpeed = (1 - (v > 0.5f?v*v:v)).Remap(0, 1, minSpeed, maxSpeed);

        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2")).normalized;

        anim.SetBool("Moving", moveDir != Vector2.zero);

        Vector2 temp = moveDir;
        temp.x = Mathf.Abs(temp.x);
        visual.localRotation = Quaternion.Euler(0, 0, -Angle(Vector2.right, temp));
        transform.localScale = new Vector3(moveDir.x >= 0 ? 1 : -1, 1, 1);

        transform.Translate(moveDir * _currentSpeed * Time.deltaTime);

    }

    float Angle(Vector2 a, Vector2 b)
    {
        var an = a.normalized;
        var bn = b.normalized;
        var x = an.x * bn.x + an.y * bn.y;
        var y = an.y * bn.x - an.x * bn.y;
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
}

public static class Vector2Ex
{
    public static Vector3 ToVector3(this Vector2 vec2 , float z = 0.0f)
    {
        return new Vector3(vec2.x, vec2.y, z);
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

