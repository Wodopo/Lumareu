using UnityEngine;
using UnityEngine.Events;

public class CollisionDetection : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private string interactionTag;

    [Header("Collision")]
    [SerializeField] private UnityEvent onCollisionEnter2D;
    [SerializeField] private UnityEvent onCollisionStay2D;
    [SerializeField] private UnityEvent onCollisionExit2D;

    [Header("Trigger")]
    [SerializeField] private UnityEvent onTriggerEnter2D;
    [SerializeField] private UnityEvent onTriggerStay2D;
    [SerializeField] private UnityEvent onTriggerExit2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.gameObject.CompareTag(interactionTag)))
            onCollisionEnter2D.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.gameObject.CompareTag(interactionTag)))
            onCollisionStay2D.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.gameObject.CompareTag(interactionTag)))
            onCollisionExit2D.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.CompareTag(interactionTag)))
            onTriggerEnter2D.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.CompareTag(interactionTag)))
            onTriggerStay2D.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(interactionTag) || (!string.IsNullOrEmpty(interactionTag) && collision.CompareTag(interactionTag)))
            onTriggerExit2D.Invoke();
    }
}
