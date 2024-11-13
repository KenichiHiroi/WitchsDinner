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
        //�����A�j���[�V�����̏�Ԃ�3�b�o�߂����ꍇ�A�ҋ@�A�j���[�V�����ɑJ�ڂ���
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

        //Throw�ɑJ�ڂ������Ԃ��L�^����
        transitionThrowTime = Time.time * 1000;
    }
}
