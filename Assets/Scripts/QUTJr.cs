using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class QUTJr : MonoBehaviour {

    private enum Moving { LEFT = -1, RIGHT = 1 };
    private enum Jumping { INPLACE, FORWARD };

    public float moveSpeed;
    public float jumpHeight;
    public float jumpDistance;
    private bool jumping = false;

    [Header("Controls")]
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jumpInPlace;
    public KeyCode jumpForward;
    public KeyCode collapse;

    private Limb baseLimb;

    private void Start() {
        baseLimb = transform.Find("Base").GetComponent<Limb>();
    }

    private void Update() {
        UserInput();
    }

    private void UserInput() {
        if (Input.GetKey(moveLeft)) Move(Moving.LEFT);

        if (Input.GetKey(moveRight)) Move(Moving.RIGHT);

        if (Input.GetKey(jumpInPlace) && !jumping) StartCoroutine(Jump(Jumping.INPLACE));

        if (Input.GetKey(jumpForward) && !jumping) StartCoroutine(Jump(Jumping.FORWARD));

        if (Input.GetKey(collapse)) ;
    }

    private void Move(Moving direction) {
        //baseLimb.Scale(new Vector3((int)direction, 1, 1));
        baseLimb.Translate(new Vector3(moveSpeed * (int)direction * Time.deltaTime, 0, 0));
    }

    private IEnumerator Jump(Jumping type) {
        jumping = true;

        float targetHeight = baseLimb.transform.position.y + jumpHeight;
        float targetDistance = baseLimb.transform.position.x + jumpDistance;

        bool heightReached = false;
        bool distanceReached = (type == Jumping.INPLACE) ? true : false;
        Debug.Log(distanceReached);

        // Ascend
        while (!heightReached || !distanceReached) {
            baseLimb.Translate(Vector3.up * Time.deltaTime);
            Debug.Log(baseLimb.transform.position.y / targetHeight);
            if (!distanceReached) baseLimb.Translate(Vector3.right * Time.deltaTime);
            yield return null;

            if (baseLimb.transform.position.y >= targetHeight) heightReached = true;
            if (baseLimb.transform.position.x >= targetDistance) distanceReached = true;

            if (heightReached && distanceReached) {
                // Descend
                while (baseLimb.transform.position.y > 0) {
                    baseLimb.Translate(Vector3.down * 1.81f * Time.deltaTime);
                    yield return null;
                }
                // Clamp to 0
                baseLimb.transform.position.Set(transform.position.x, 0, transform.position.z);
                jumping = false;
            }
        }
    }

}
