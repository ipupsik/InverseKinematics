using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Base : MonoBehaviour
{
    public int segmentCount;

    List<IK_Segment> segments = new List<IK_Segment>();
    List<GameObject> objects = new List<GameObject>();

    GameObject head;
    IK_Segment headIkComp;

    public GameObject raycastDir;
    public GameObject target;
    public GameObject pole;
    // Start is called before the first frame update

    public void SetTargetPointPosition(Vector3 newPosition)
    {
        if (target == null)
            target = new GameObject();
        target.transform.position = newPosition;
    }
    private void Start()
    {
        if (target == null)
            target = new GameObject();

        Vector3 a = transform.position;
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject gm = new GameObject();

            IK_Segment newSegmentIKComp = gm.AddComponent<IK_Segment>();
            if (head == null)
                newSegmentIKComp.Init(a, target, 1);
            else
                newSegmentIKComp.Init(a, head, 1);

            a += new Vector3(1.0f, 0.0f, 0.0f);

            head = gm;
            headIkComp = newSegmentIKComp;
            head.transform.parent = gm.transform;

            gm.transform.parent = transform;

            segments.Add(newSegmentIKComp);
            objects.Add(gm);
        }

        objects.Reverse();
        segments.Reverse();
    }

    // Update is called once per frame
    void Update()
    {
        if (headIkComp)
        {
            headIkComp.SegmentUpdate();

            segments[0].SetA(transform.position);

            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].SetA(segments[i - 1].b);
            }

            for (int i = 1; i < objects.Count; i++)
            {
                var plane = new Plane(segments[i].next.transform.position - objects[i - 1].transform.position, objects[i - 1].transform.position);
                var projectedPole = plane.ClosestPointOnPlane(pole.transform.position);
                var projectedBone = plane.ClosestPointOnPlane(objects[i].transform.position);

                var angle = Vector3.SignedAngle(projectedBone - objects[i - 1].transform.position, projectedPole - objects[i - 1].transform.position, plane.normal);

                objects[i].transform.position = Quaternion.AngleAxis(angle, plane.normal) * (objects[i].transform.position - objects[i - 1].transform.position) + objects[i - 1].transform.position;
            }
        }
    }
}