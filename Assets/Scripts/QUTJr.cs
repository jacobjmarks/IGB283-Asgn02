using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class QUTJr : MonoBehaviour {

    private enum Facing { LEFT = -1, RIGHT = 1 };
    private enum Jumping { INPLACE, FORWARD };

    [Header("Configuration")]
    public float moveSpeed = 5;
    public float jumpHeight = 1;
    public float jumpDistance = 1.5f;
    public float ascendSpeed = 5;
    public float descendSpeed = 3;
    public float avgAirspeedVelocity = 3;
    public bool continuousMovement;

    [Header("Controls")]
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jumpInPlace;
    public KeyCode jumpForward;
    public KeyCode collapse;

    private Limb baseLimb;

    private bool jumping = false;

    private Facing _direction = Facing.LEFT;
    private Facing direction {
        get {
            return _direction;
        }
        set {
            if (value != direction) {
                baseLimb.Flip();
                _direction = value;
            }
        }
    }

    private void Start() {
        baseLimb = transform.Find("Base").GetComponent<Limb>();
    }

    private void Update() {
        UserInput();
        if (continuousMovement && !jumping) MoveForward();
    }

    private void OnValidate() {
        while (ascendSpeed < 1) ascendSpeed++;
        while (descendSpeed < 1) descendSpeed++;
        while (avgAirspeedVelocity < 1) avgAirspeedVelocity++;
    }

    private void UserInput() {
        if (Input.GetKey(moveLeft) && !jumping) {
            direction = Facing.LEFT;
            if (!continuousMovement) MoveForward();
        }

        if (Input.GetKey(moveRight) && !jumping) {
            direction = Facing.RIGHT;
            if (!continuousMovement) MoveForward();
        }

        if (Input.GetKey(jumpInPlace) && !jumping) StartCoroutine(Jump(Jumping.INPLACE));

        if (Input.GetKey(jumpForward) && !jumping) StartCoroutine(Jump(Jumping.FORWARD));

        if (Input.GetKey(collapse)) ;
    }

    private void MoveForward() {
        baseLimb.Translate(new Vector3(moveSpeed * (int)direction * Time.deltaTime, 0, 0));
    }

    private IEnumerator Jump(Jumping type) {
        jumping = true;

        float targetHeight = baseLimb.transform.position.y + jumpHeight;
        float targetDistance = baseLimb.transform.position.x + jumpDistance * (int)direction;

        bool heightReached = false;
        bool distanceReached = (type == Jumping.INPLACE) ? true : false;

        // Ascend
        while (!heightReached || !distanceReached) {
            if (!heightReached) baseLimb.Translate(Vector3.up * ascendSpeed * Time.deltaTime);
            if (!distanceReached) baseLimb.Translate(Vector3.right * avgAirspeedVelocity * (int)direction * Time.deltaTime);
            yield return null;

            heightReached = heightReached || baseLimb.transform.position.y >= targetHeight;
            // Not-so-graceful distance check
            distanceReached = distanceReached || Vector2.Distance(
                new Vector2(baseLimb.transform.position.x, 0),
                new Vector2(targetDistance, 0)
            ) <= 0.05;

            if (heightReached && distanceReached) {
                // Descend
                while (baseLimb.transform.position.y > 0) {
                    baseLimb.Translate(Vector3.down * descendSpeed * Time.deltaTime);
                    yield return null;
                }
                // Clamp to 0
                baseLimb.transform.position.Set(transform.position.x, 0, transform.position.z);
                jumping = false;
            }
        }
    }

}
