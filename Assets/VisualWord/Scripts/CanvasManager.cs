using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public InputField textField;

    public string GetInputFromInputField()
    {
        return textField.text;
    }
}
