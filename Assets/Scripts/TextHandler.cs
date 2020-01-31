using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{

    public Text instructionText;
    public Text titleText;

    // Start is called before the first frame update
    void Start()
    {
        string title = "KEEP THE" + "\n" + "LIGHTS ON...";
        
        string instructions = "Drag the hands away from the windows to keep the light thiefs from stealing all your light!" + "\n \n" +
            "If a window has gone dark, double tap on it to re-light it again! Tap the green light to slow the hands down." + "\n \n" +
            "Make it to the end of the level without all the windows going dark to win!";

        titleText.text = title;
        instructionText.text = instructions;
    }
}
