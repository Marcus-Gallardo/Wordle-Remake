using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutTesting : MonoBehaviour
{
    [SerializeField]
    GameObject placeholder;

    // Start is called before the first frame update
    void Start()
    {
        int numPlaceholders = 30;

        GameObject[] objects = new GameObject[numPlaceholders];

        for(int i = 0; i < numPlaceholders; i++)
        {
            GameObject instance = Instantiate(placeholder, gameObject.transform);

            objects[i] = instance;
        }

        LayoutObjectsInGrid(objects, 5, 6, 20, 100, 100);
    }

    void LayoutObjectsInGrid(GameObject[] arrObjects, int rowSize, int colSize, int spacing, int cellSizeX, int cellSizeY)
    {
        int lengthOfRow = cellSizeX * rowSize + (rowSize - 1) * spacing;
        int lengthOfCol = cellSizeY * colSize + (colSize - 1) * spacing;

        int distanceBetweenObjects = lengthOfRow / rowSize;

        // Counter for the current object in arrObjects
        int objCounter = 0;

        for(int i = 0; i < colSize; i++)
        {
            for(int j = 0; j < rowSize; j++)
            {
                Vector3 objPos = arrObjects[objCounter].transform.position;

                float newPosX = transform.position.x - lengthOfRow / 2 + distanceBetweenObjects * (j + 1);
                float newPosY = transform.position.y - (cellSizeY + spacing) - distanceBetweenObjects * (i + 1);

                Vector3 newPos = new Vector3(newPosX, newPosY, objPos.z);

                arrObjects[objCounter].transform.position = newPos;
                arrObjects[objCounter].name = (j * i).ToString();

                objCounter++;
            }
        }
    }
}
