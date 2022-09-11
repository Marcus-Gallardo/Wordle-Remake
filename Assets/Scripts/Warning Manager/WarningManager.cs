using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    [SerializeField]
    GameObject warningPrefab;

    [SerializeField]
    GameObject inputSquaresManager;

    /// <summary>
    /// Instantiates a warning and triggers its fade animation
    /// </summary>
    /// <param name="warning"></param>
    public async void InstantiateWarning(string warning)
    {
        // Animate warning
        GameObject warningInstance = Instantiate(warningPrefab, this.transform);
        Animator warningAnimator = warningInstance.GetComponent<Animator>();

        warningInstance.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = warning;

        // Wait 1000 ms or 1 second
        await Task.Delay(1000);

        warningAnimator.SetTrigger("triggerFade");  

        // Wait another second
        await Task.Delay(1000);

        if(Application.isEditor)
        {
            DestroyImmediate(warningInstance);
        }
        else
        {
            Destroy(warningInstance);
        }
    }
}