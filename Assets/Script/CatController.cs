using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    Animator anim;
    float transitionThrowTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isIdle", true);
        anim.SetBool("isCrouch", false);
        anim.SetBool("isThrow", false);

    }

    // Update is called once per frame
    void Update()
    {
        //投げアニメーションの状態で3秒経過した場合、待機アニメーションに遷移する
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Throw")
            && Time.time * 1000 > transitionThrowTime + 3000)
        {
            transitionIdle();
        }
    }

    public void transitionIdle()
    {
        Debug.Log("Call transitionIdle");

        anim.SetBool("isIdle", true);
        anim.SetBool("isCrouch", false);
        anim.SetBool("isThrow", false);
    }

    public void transitionCrouch()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isCrouch", true);
        anim.SetBool("isThrow", false);
    }

    public void transitionThrow()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isCrouch", false);
        anim.SetBool("isThrow", true);

        //Throwに遷移した時間を記録する
        transitionThrowTime = Time.time * 1000;
    }
}
