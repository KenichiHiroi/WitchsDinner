using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTestManager : MonoBehaviour
{
    GameObject ThrowObject;
    GameObject StartPoint;
    GameObject EndPoint;

    Vector3 StartPos;
    Vector3 EndPos;

    bool isGo;

    // Start is called before the first frame update
    void Start()
    {
        ThrowObject = GameObject.Find("ThrowObject");
        StartPoint = GameObject.Find("StartPosition");
        EndPoint = GameObject.Find("EndPosition");

        StartPos = new Vector3(StartPoint.transform.position.x, StartPoint.transform.position.y, 0);
        EndPos = new Vector3(EndPoint.transform.position.x, EndPoint.transform.position.y, 0);

        isGo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > 2
            & isGo == false)
        {
            StartThrow(ThrowObject, 3, StartPos, EndPos, 30);
            isGo = true;
        }
    }

    public void StartThrow(GameObject target, float height, Vector3 start, Vector3 end, float duration)
    {
        // 中点を求める
        Vector3 half = end - start * 0.50f + start;
        half.y += Vector3.up.y + height;

        StartCoroutine(LerpThrow(target, start, half, end, duration));
    }

    IEnumerator LerpThrow(GameObject target, Vector3 start, Vector3 half, Vector3 end, float duration)
    {
        float startTime = Time.timeSinceLevelLoad;  //シーンをロードしてからの経過時間　関数の開始時間
        float rate = 0f;

        Debug.Log("startTime:" + startTime);

        while (true)
        {
            if (rate >= 1.0f)
            {
                yield break;
            }
                
            
            float diff = Time.timeSinceLevelLoad - startTime;   //シーンをロードしてからの経過時間　関数の開始時間
            rate = diff / (duration / 60f);

            Debug.Log("diff:" + diff
                + "  rate:" + rate);

            target.transform.position = CalcLerpPoint(start, half, end, rate);
            //Debug.Log("Pos:" + target.transform.position);
            yield return null;
        }
    }

    Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t);
        var b = Vector3.Lerp(p1, p2, t);

        //Debug.Log("a:" + a);
        //Debug.Log("b:" + b);
        
        return Vector3.Lerp(a, b, t);
    }
}
