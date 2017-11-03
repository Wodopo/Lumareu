using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonState
{
    public bool value;
    public float holdTime = 0.0f;
}

public class InputState : MonoBehaviour {

    public Dictionary<Buttons, ButtonState> buttonStates = new Dictionary<Buttons, ButtonState>();

    public void SetButtonValue(Buttons key, bool value)
    {
        if (!buttonStates.ContainsKey(key))
            buttonStates.Add(key, new ButtonState());

        ButtonState state = buttonStates[key];

        // Button Released
        //if (state.value && !value)
        //    state.holdTime = 0.0f;

        // Button Pressed
        //if (!state.value && value)
        //    Debug.Log(string.Format("Button {0} was pressed!", key));

        // Button Held
        if (state.value && value)
            state.holdTime += Time.deltaTime;
        // Button not Held
        else if (!state.value && !value)
            state.holdTime = 0.0f;

        state.value = value;
    }

    public float GetButtonHoldTime(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].holdTime;
        else
            return 0.0f;
    }

    public bool GetButton(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].value;
        else
            return false;
    }

    public bool GetButtonDown(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].value && buttonStates[key].holdTime == 0.0f;
        else
            return false;
    }

    public bool GetButtonUp(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return !buttonStates[key].value && buttonStates[key].holdTime != 0.0f;
        else
            return false;
    }
}
