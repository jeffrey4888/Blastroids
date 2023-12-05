using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDesc : MonoBehaviour
{
    public Text toggleDescText;

    public void ClearDescription()
    {
        toggleDescText.text = "---";
    }
    public void DisplayOptionOne()
    {
        toggleDescText.text = "Increases enemy\nhealth.";
    }

    public void DisplayOptionTwo()
    {
        toggleDescText.text = "Some enemies spawn\nwith new forms.";
    }

    public void DisplayOptionThree()
    {
        toggleDescText.text = "Levels contain\nmore enemies.";
    }

    public void DisplayOptionFour()
    {
        toggleDescText.text = "Exit gates require\nsome enemy defeats.";
    }

    public void DisplayOptionFive()
    {
        toggleDescText.text = "Your attacks\nare weaker.";
    }

    public void DisplayOptionSix()
    {
        toggleDescText.text = "You cannot\nrespawn.";
    }

    public void DisplayOptionSeven()
    {
        toggleDescText.text = "You always have\none HP.";
    }

    public void DisplayOptionEight()
    {
        toggleDescText.text = "You cannot use\nyour shield or bomb.";
    }
}
