using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {

    [Header("Mesh")]
    public float width;
    public float height;
    public Material material;
    public Color color;

    [Header("Joints")]
    public Vector2 selfJoint;
    public Vector2 childJoint;
    public float startRotation;

    [Header("Collapse")]
    public float collapseAngle = 90;
    [HideInInspector]
    public bool collapsed = false;

    [Header("Child Limb")]
    public Limb child;
    
    private void Awake() {
        DrawMesh();

        if (child) child.Translate(childJoint);
        Rotate(startRotation);
    }

    public void Translate(Vector3 offset) {
        transform.position += offset;

        if (child) child.Translate(offset);
    }

    public void Rotate(float angle) {
        transform.Rotate(Vector3.forward, angle);

        if (child) {
            child.Translate(-child.transform.position + transform.position + transform.up * height);
            child.Rotate(angle);
        }
    }

    public void Flip() {
        transform.RotateAround(new Vector2(GameObject.Find("Base").transform.position.x, transform.position.y), Vector3.up, 180);
        if (child) child.Flip();
    }

    public IEnumerator Collapse(float speed) {
        if (child) StartCoroutine(child.Collapse(speed/2));

        while (Mathf.DeltaAngle(transform.eulerAngles.z, collapseAngle) > 0.1) {
            Rotate(speed * Time.deltaTime);
            yield return null;
        }

        if (child) yield return new WaitUntil(() => child.collapsed);
        collapsed = true;
    }


    private void DrawMesh() {
        gameObject.AddComponent<MeshRenderer>().material = material;
        Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;
        mesh.Clear();

        Vector3[] vertices = new Vector3[] {
            new Vector3(0, 0),
            new Vector3(0, height),
            new Vector3(width, height),
            new Vector3(width, 0)
        };

        // Offset by joint
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] -= (Vector3)selfJoint;
        }

        childJoint -= selfJoint;
        selfJoint -= selfJoint;

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mesh.colors = new Color[] { color, color, color, color };

        mesh.RecalculateBounds();
    }

}
