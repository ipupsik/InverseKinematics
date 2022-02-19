using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Segment : MonoBehaviour
{
    GameObject mesh;

    public float width;
    float length = 4.0f;

    Vector3 Dir;
    public Vector3 a;
    public Vector3 b;

    public GameObject next;
    IK_Segment nextIKComponent;

    // Start is called before the first frame update
    public void Init(Vector3 a_, GameObject next_, float width_)
    {
        next = next_;

        next.TryGetComponent<IK_Segment>(out nextIKComponent);

        width = width_;
        transform.position = a_;

        Calculate();
    }

    public void SetA(Vector3 a_)
    {
        a = a_;
        transform.position = a;

        b = a + Dir.normalized * length;
    }

    public void SegmentUpdate()
    {
        if (nextIKComponent)
        {
            nextIKComponent.SegmentUpdate();
        }
        CalculatePositions();
    }

    public void Calculate()
    {
        InitializeMesh();
        CalculatePositions();
    }

    void CalculatePositions()
    {
        a = transform.position;
        b = next.transform.position;

        Dir = b - a;

        transform.rotation = Quaternion.LookRotation(Dir);

        a = b - Dir.normalized * length;
        transform.position = a;
    }

    void InitializeMesh()
    {
        if (mesh)
            Destroy(mesh);

        mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);

        mesh.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Body");

        Destroy(mesh.GetComponent<BoxCollider>());

        mesh.transform.localScale = new Vector3(1, 1, length);
        mesh.transform.parent = transform;

        Vector3 localPos = mesh.transform.localScale * 0.5f;
        localPos.y = 0.0f;
        localPos.x = 0.0f;
        mesh.transform.localPosition = mesh.transform.localScale * 0.5f;
        mesh.transform.localPosition = localPos;
    }
}
