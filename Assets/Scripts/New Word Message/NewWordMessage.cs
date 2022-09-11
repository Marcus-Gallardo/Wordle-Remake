using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWordMessage : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if(!animator)
        {
            print("Animator is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShrinkAndExpandLoop()
    {
        while(true)
        { 
            animator.SetTrigger("triggerShrink");

            yield return new WaitForSeconds(1f);

            animator.SetTrigger("triggerExpand");

            yield return new WaitForSeconds(1f);

            yield return null;
        }
    }
}
