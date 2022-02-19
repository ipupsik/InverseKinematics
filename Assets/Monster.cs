using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public List<IK_Base> legs;
    public GameObject Body;
    List<Vector3> prevStep;
    List<Vector3> nextStep;
    List<float> animating;

    Vector3 baseOffset;

    float duration = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        nextStep = new List<Vector3>();
        animating = new List<float>();
        prevStep = new List<Vector3>();

        for (int i = 0; i < legs.Count; i++)
        {
            RaycastHit hit;
            Physics.Raycast(legs[i].transform.position,
                (legs[i].raycastDir.transform.position - legs[i].transform.position) * 3.0f, out hit);

            //Debug.DrawRay(legs[i].transform.position, legs[i].raycastDir * 3.0f, Color.green);

            legs[i].SetTargetPointPosition(hit.point);

            animating.Add(-1);
            nextStep.Add(new Vector3(0, 0, 0));
            prevStep.Add(hit.point);
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 avgLegs = new Vector3(0, 0, 0);
        Vector3 pos = Body.transform.position;
        int count = 0;

        for (int i = 0; i < legs.Count; i++)
        {
            avgLegs += prevStep[i];
        }

        for (int i = 0; i < legs.Count; i++)
        {
            if (animating[i] >= 0)
            {
                animating[i] -= Time.deltaTime;

                float t = animating[i] / duration;

                legs[i].SetTargetPointPosition(
                    (1.0f - t) * nextStep[i] + t * prevStep[i] + new Vector3(0, 3, 0) * Mathf.Sin(t * Mathf.PI));

                if (animating[i] < 0)
                {
                    legs[i].SetTargetPointPosition(nextStep[i]);
                    prevStep[i] = legs[i].target.transform.position;
                }

            }

            avgLegs += prevStep[i];
            count++;
        }

        pos.y = 0.0f;
        avgLegs.x = 0.0f;
        avgLegs.z = 0.0f;

        baseOffset = new Vector3(0, 7, 0) * transform.localScale.x;

        Body.transform.position = pos + baseOffset + avgLegs / legs.Count;

        for (int i = 0; i < legs.Count; i++)
        {
            if (animating[i] < 0)
            {
                RaycastHit hit;
                Physics.Raycast(legs[i].transform.position,
                    (legs[i].raycastDir.transform.position - legs[i].transform.position) * 3.0f, out hit);

                nextStep[i] = hit.point;

                if ((nextStep[i] - legs[i].target.transform.position).magnitude >= 5.0f)
                {
                    animating[i] = duration;
                }
            }
        }

        transform.position += -transform.forward * 3 * Time.deltaTime;

        Vector3 avgLeftLegs = new Vector3(0, 0, 0);
        avgLeftLegs += legs[0].target.transform.position;
        avgLeftLegs += legs[1].target.transform.position;
        avgLeftLegs += legs[4].target.transform.position;
        avgLeftLegs /= 3;

        Vector3 avgRightLegs = new Vector3(0, 0, 0);
        avgRightLegs += legs[2].target.transform.position;
        avgRightLegs += legs[3].target.transform.position;
        avgRightLegs += legs[5].target.transform.position;
        avgRightLegs /= 3;

        float angle = Vector3.SignedAngle(avgRightLegs - avgLeftLegs, -transform.right, Vector3.up);
        //Body.transform.rotation = Quaternion.Euler(new Vector3(Body.transform.eulerAngles.x, Body.transform.eulerAngles.y, angle));

        //Debug.DrawRay(transform.position, -transform.right * 100, Color.blue);
        //Debug.DrawRay(transform.position, (avgRightLegs - avgLeftLegs) * 100, Color.blue);

        Vector3 avgFrontLegs = new Vector3(0, 0, 0);
        avgFrontLegs += prevStep[0];
        avgFrontLegs += prevStep[2];
        avgFrontLegs /= 2;

        Vector3 avgBackLegs = new Vector3(0, 0, 0);
        avgBackLegs += prevStep[1];
        avgBackLegs += prevStep[3];
        avgBackLegs /= 2;


    }
}
