using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    public Action onTakeDamage;

    [Header("Debug")]
    [SerializeField] private bool _log = false;

    public int maxHealth = 2;
    public int currentHealth = 2;

    public UnityEvent onDeath;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int value)
    {
        value = Mathf.Clamp(Mathf.Abs(value), 0, currentHealth);
        currentHealth -= value;

        // Revert to last Checkpoint
        
        if (onTakeDamage != null)
            onTakeDamage();

        if (value > 0)
            HealthLog("Damage Taken: " + value);

        if (currentHealth <= 0)
        {
            SendMessage("OnDeath");
            HealthLog("Died");
        }
    }

    private void OnDeath()
    {
        currentHealth = maxHealth;
    }

    private void HealthLog(string msg)
    {
        if (_log)
            Debug.Log(gameObject.name + " Health\n" + msg);
    }
}
