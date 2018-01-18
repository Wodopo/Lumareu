using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public Action onTakeDamage;

    public void TakeDamage(int value)
    {
        // Revert to last Checkpoint
        Debug.Log("Damage Taken: " + value);
        if (onTakeDamage != null)
            onTakeDamage();
    }
}
