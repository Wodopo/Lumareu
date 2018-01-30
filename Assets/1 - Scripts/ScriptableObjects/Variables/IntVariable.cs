using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableVariable<int>
{
    public void Add(int value)
    {
        value = Mathf.Abs(value);
        Value += value;
    }

    public void Remove(int value)
    {
        value = Mathf.Abs(value);
        Value -= value;
    }
}