using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    public float speed;
    
    void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        Vector2 translation = direction * speed * Time.deltaTime;

        transform.Translate(translation);
    }
}
