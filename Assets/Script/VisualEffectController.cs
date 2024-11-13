using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectController : MonoBehaviour
{
    [SerializeField] GameObject SmokeEffect;
    [SerializeField] GameObject CarrotEffect;
    [SerializeField] GameObject PotateEffect;
    [SerializeField] GameObject MeatEffect;

    Animator SmokeAnim;
    Animator CarrotAnim;
    Animator PotateAnim;
    Animator MeatAnim;

    // Start is called before the first frame update
    void Start()
    {
        SmokeAnim = SmokeEffect.GetComponent<Animator>();
        CarrotAnim = CarrotEffect.GetComponent<Animator>();
        PotateAnim = PotateEffect.GetComponent<Animator>();
        MeatAnim = MeatEffect.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SmokeAnim.GetBool("isBeatGood")
            && SmokeAnim.GetCurrentAnimatorStateInfo(0).IsName("FinishedSmoke"))
        {
            SmokeAnim.SetBool("isBeatGood", false);
        }

        if(CarrotAnim.GetBool("isBeatGood")
            && CarrotAnim.GetCurrentAnimatorStateInfo(0).IsName("FinishedCarrot"))
        {
            CarrotAnim.SetBool("isBeatGood", false);
        }

        if (PotateAnim.GetBool("isBeatGood")
            && PotateAnim.GetCurrentAnimatorStateInfo(0).IsName("FinishedPotate"))
        {
            PotateAnim.SetBool("isBeatGood", false);
        }

        if (MeatAnim.GetBool("isBeatGood")
            && MeatAnim.GetCurrentAnimatorStateInfo(0).IsName("FinishedMeat"))
        {
            MeatAnim.SetBool("isBeatGood", false);
        }
    }

    public void PlayVE_Smoke(float PosX, float PosY)
    {
        SmokeEffect.gameObject.transform.position = new Vector3(PosX, PosY, 0);
        SmokeAnim.SetBool("isBeatGood", true);
    }

    public void PlayVE_Carrot(float PosX,float PosY)
    {
        CarrotEffect.gameObject.transform.position = new Vector3(PosX, PosY, 0);
        CarrotAnim.SetBool("isBeatGood", true);
    }

    public void PlayVE_Potate(float PosX, float PosY)
    {
        PotateEffect.gameObject.transform.position = new Vector3(PosX, PosY, 0);
        PotateAnim.SetBool("isBeatGood", true);
    }

    public void PlayVE_Meat(float PosX, float PosY)
    {
        MeatEffect.gameObject.transform.position = new Vector3(PosX, PosY, 0);
        MeatAnim.SetBool("isBeatGood", true);
    }
}
