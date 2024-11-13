using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizzardController : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isIdle", true);
        anim.SetBool("isActive", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isActive", true);
            anim.SetBool("isIdle", false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isActive", false);
            anim.SetBool("isIdle", true);
        }
    }
}
