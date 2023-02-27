using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Toggle toggle1;
    public Toggle toggle2;
    public void ToggleSwitchOne()
    {
        
        if (toggle1.isOn != toggle2.isOn)
        {
            toggle2.isOn = !toggle2.isOn;
        }
        toggle1.isOn = !toggle1.isOn;
    }
    public void ToggleSwitchTwo()
    {
        
        if (toggle1.isOn != toggle2.isOn)
        {
            toggle1.isOn = !toggle1.isOn;
        }
        toggle2.isOn = !toggle2.isOn;
    }
}
