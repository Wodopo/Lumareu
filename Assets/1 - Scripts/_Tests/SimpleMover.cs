using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    public float speed;
    
    void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2")).normalized;

        Vector2 translation = direction * speed * Time.deltaTime;

        transform.Translate(translation);
    }
}
