using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbWobble : MonoBehaviour {

    public float angle;
    public float speed;

    private Limb limb;
    private float startAngle;

    private void Start() {
        limb = GetComponent<Limb>();
        startAngle = transform.rotation.eulerAngles.z;
    }

    private void Update() {
        float newAngle = startAngle + Mathf.Sin(speed * Time.realtimeSinceStartup) * angle/2;
        limb.Rotate(newAngle - transform.eulerAngles.z);
    }

}
