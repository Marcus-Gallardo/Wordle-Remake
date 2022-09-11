using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class InputSquareManager : MonoBehaviour
{
    [SerializeField]
    Canvas mainCanvas;

    // Scale factor is used to make sure that the layout is the same accross all screen sizes
    float scaleFactor;

    private void Awake()
    {
        scaleFactor = mainCanvas.transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public GameObject inputSquarePrefab;

    public WordleGame gameManager;

    public GameObject[,] InstantiateInputSquares(int numRows, int numCols)
    {
        GameObject[,] inputSquares = new GameObject[numRows, numCols];

        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCols; j++)
            {
                inputSquares[i, j] = Instantiate(inputSquarePrefab, transform);
            }
        }

        return inputSquares;
    }

    public GameObject[] GetInputSquares()
    {
        GameObject[] inputSquares = new GameObject[gameManager.numRows * gameManager.numCols];

        for(int i = 0; i < gameManager.numRows * gameManager.numCols; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            // Check if the child of this object is an input square
            if(child.GetComponent<InputSquare>())
            {
                inputSquares[i] = transform.GetChild(i).gameObject;
            }
        }

        return inputSquares;
    }

    public void LayoutInputSquares(GameObject[] arrObjects, int rowSize, int colSize, float spacing, int cellSizeX, int cellSizeY)
    {
        float lengthOfRow = cellSizeX * rowSize + (rowSize - 1) * spacing / scaleFactor;
        float distanceBetweenObjects = lengthOfRow / rowSize / scaleFactor;

        // Counter for the current object in arrObjects
        int objCounter = 0;

        for (int i = 0; i < colSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                Vector3 objPos = arrObjects[objCounter].transform.position;

                float newPosX = transform.position.x - lengthOfRow / 2 + distanceBetweenObjects * (j + 1) - 60;
                float newPosY = transform.position.y - (cellSizeY + spacing) - distanceBetweenObjects * (i + 1);

                Vector3 newPos = new Vector3(newPosX, newPosY, objPos.z);

                arrObjects[objCounter].transform.position = newPos;

                objCounter++;
            }
        }
    }

    /// <summary>
    /// Starts and then stops the shake animation on inputSquares
    /// </summary>
    /// <param name="inputSquaresToShake"></param>
    public async void ShakeInputSquaresAsync(GameObject[] inputSquaresToShake)
    {
        // Start animations for all of the input squares
        foreach (GameObject obj in inputSquaresToShake)
        {
            Animator animator = obj.GetComponent<Animator>();

            animator.SetTrigger("triggerShake");
        }

        // Wait 1000 ms or 1 second
        await Task.Delay(1000);

        // Stop the animations
        foreach (GameObject obj in inputSquaresToShake)
        {
            Animator animator = obj.GetComponent<Animator>();

            animator.ResetTrigger("triggerShake");
        }
    }

    public async Task ShrinkInputSquareAsync(GameObject inputSquare)
    {
        Animator animator = inputSquare.GetComponent<Animator>();

        animator.SetTrigger("triggerShrink");

        // Wait for the animation to finish playing
        await Task.Delay((int)(GetAnimationClipLength(animator, "Flip") * 1000));

        animator.ResetTrigger("triggerExpand");
    }

    public async Task ExpandInputSquareAsync(GameObject inputSquare)
    {
        Animator animator = inputSquare.GetComponent<Animator>();

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        animator.SetTrigger("triggerExpand");

        // Wait for the animation to finish playing
        await Task.Delay((int)(GetAnimationClipLength(animator, "Flip") * 1000));

        animator.ResetTrigger("triggerExpand"); 
    }

    public IEnumerator AnimateInputSquarePop(GameObject inputSquare)
    {
        Animator animator = inputSquare.GetComponent<Animator>();

        animator.SetTrigger("triggerPop");

        yield return new WaitForSeconds(GetAnimationClipLength(animator, "Pop"));

        animator.ResetTrigger("triggerPop");
    }

    /// <summary>
    /// Gets the length of an animation clip based on the given animator and clip name
    /// <para> Returns 0 if the clip is not found </para>
    /// </summary>
    /// <param name="animator"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public float GetAnimationClipLength(Animator animator, string clipName)
    {
        foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0;
    }

    public void SetInputSquareBorderColor(GameObject inputSquare, Color color)
    {
        inputSquare.transform.GetChild(0).GetComponent<Image>().color = color;
    }
}
