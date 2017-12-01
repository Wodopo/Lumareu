using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum OnLightBehaviour { Appear, Dissapear }

public class BlockController : MonoBehaviour
{
    [SerializeField] private bool _log;

    public OnLightBehaviour lightBehaviour;
    
    private LayerMask activeLayer;
    private LayerMask hiddenLayer;

    public UnityEvent OnAppear;
    public UnityEvent OnDisapear;

    public int activeLights;

    private SpriteRenderer spRenderer;

    private void Awake()
    {
        activeLayer = LayerMask.NameToLayer("Active");
        hiddenLayer = LayerMask.NameToLayer("-");

        spRenderer = GetComponentInChildren<SpriteRenderer>();

        if (lightBehaviour == OnLightBehaviour.Appear)
            Disapear();
        else
            Appear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BlockControllerLog("Trigger Enter");

        if (!other.CompareTag("Light") && 
            !other.CompareTag("InnerLight"))
            return;

        activeLights++;
        
        if (lightBehaviour == OnLightBehaviour.Appear && other.CompareTag("Light"))
        {
            Appear();
        }
        else if(activeLights == 1)
        {
            Disapear();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BlockControllerLog("Trigger Exit");

        if (!other.CompareTag("Light") &&
            !other.CompareTag("InnerLight"))
            return;

        activeLights--;

        if (activeLights > 0)
            return;

        if (lightBehaviour == OnLightBehaviour.Appear)
            Disapear();
        else
            Appear();
    }

    private void Appear()
    {
        this.gameObject.layer = activeLayer;
        OnAppear.Invoke();

        if (spRenderer != null)
            spRenderer.transform.DOScale(1.0f, 0.25f);
    }

    private void Disapear()
    {
        this.gameObject.layer = hiddenLayer;
        OnDisapear.Invoke();

        if (spRenderer != null)
            spRenderer.transform.DOScale(0.0f, 0.25f);
    }


    void BlockControllerLog(string msg)
    {
        if (_log)
            Debug.Log("Block Controller\n"+msg);
    }
}
