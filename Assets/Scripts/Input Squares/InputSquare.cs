using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InputSquare : MonoBehaviour
{
    private string letter;

    Vector2 pos;

    public void SetColor(Color color)
    {
        GameObject image = transform.GetChild(0).gameObject;
        GameObject imageBorder = transform.GetChild(1).gameObject;

        image.GetComponent<Image>().color = color;
        imageBorder.GetComponent<Image>().color = color;
    }
}

