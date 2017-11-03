using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buttons
{
    Right,
    Left,
    Up,
    Down,
    A,
    B
}

public enum Condition
{
    GreaterThan,
    LessThan
}

[System.Serializable]
public class InputAxisState
{
    [SerializeField]
    private string descriptiveName;
    public string axisName;
    public float offValue;
    public Buttons button;
    public Condition condition;

    public bool value
    {
        get
        {
            var val = Input.GetAxisRaw(axisName);
            switch (condition)
            {
                case Condition.GreaterThan:
                    return val > offValue;
                case Condition.LessThan:
                    return val < offValue;
                default:
                    return false;
            }
        }
    }
}

public class InputManager : MonoBehaviour
{
    public InputState inputState;
    public InputAxisState[] inputs;

    void Update()
    {
        foreach (InputAxisState input in inputs)
            inputState.SetButtonValue(input.button, input.value);
    }
}
