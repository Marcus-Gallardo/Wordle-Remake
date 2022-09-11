using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

public class HoverOverBirb : MonoBehaviour
{
    [SerializeField]
    GameObject label;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(MouseIsHovering())
        {
            label.SetActive(true);
        }
        else
        {
            label.SetActive(false);
        }
    }

    // Complicated EventSystem stuff that uses its own raycast and cycles through all the hits to check if this object is the one hit
    private bool MouseIsHovering()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        for(int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject == this.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
