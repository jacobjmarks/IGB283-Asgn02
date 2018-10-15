using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class QUTJr : MonoBehaviour {

    private enum Facing { LEFT = -1, RIGHT = 1 };
    private enum Jumping { INPLACE, FORWARD };

    [Header("Movement")]
    public float moveSpeed = 5;
    public bool continuousMovement;

    [Header("Movement Bounds")]
    public float xFrom = -5;
    public float xTo = 5;
    private bool outOfBoundsLeft = false;
    private bool outOfBoundsRight = false;

    [Header("Jumping")]
    public float jumpHeight = 1;
    public float jumpDistance = 1.5f;
    public float ascendSpeed = 5;
    public float descendSpeed = 3;
    public float avgAirspeedVelocity = 3;
    private bool jumping = false;

    [Header("Controls")]
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jumpInPlace;
    public KeyCode jumpForward;
    public KeyCode collapse;

    private Limb baseLimb;

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
        BoundsCheck();
        UserInput();
        if (continuousMovement && !jumping) MoveForward();
    }

    private void OnValidate() {
        // Ensure jump parameters are at least 1
        while (ascendSpeed < 1) ascendSpeed++;
        while (descendSpeed < 1) descendSpeed++;
        while (avgAirspeedVelocity < 1) avgAirspeedVelocity++;

        // Ensure movement bounds are within reasonable values
        while (xFrom > -1) xFrom--;
        while (xTo < 1) xTo++;
    }

    private void UserInput() {
        if (Input.GetKey(moveLeft) && !jumping && !outOfBoundsLeft) {
            direction = Facing.LEFT;
            if (!continuousMovement) MoveForward();
        }

        if (Input.GetKey(moveRight) && !jumping && !outOfBoundsRight) {
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
        if ((direction == Facing.LEFT && outOfBoundsLeft) || (direction == Facing.RIGHT && outOfBoundsRight)) yield break;
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

            // Alter jump conditions based on bounds check
            heightReached = heightReached || (direction == Facing.LEFT) ? outOfBoundsLeft : outOfBoundsRight;
            distanceReached = distanceReached || (direction == Facing.LEFT) ? outOfBoundsLeft : outOfBoundsRight;

            // Alter jump conditions based on target height/distance
            heightReached = heightReached || baseLimb.transform.position.y >= targetHeight;
            distanceReached = distanceReached || Vector2.Distance(
                new Vector2(baseLimb.transform.position.x, 0),
                new Vector2(targetDistance, 0)
            ) <= 0.05; // Not-so-graceful distance check
        }

        // Descend
        while (baseLimb.transform.position.y > 0) {
            baseLimb.Translate(Vector3.down * descendSpeed * Time.deltaTime);
            yield return null;
        }

        // Clamp to 0
        baseLimb.transform.position.Set(transform.position.x, 0, transform.position.z);
        jumping = false;
    }

    private void BoundsCheck() {
        if (baseLimb.transform.position.x <= xFrom) {
            outOfBoundsLeft = true;
            if (continuousMovement && !jumping) direction = Facing.RIGHT;
        } else {
            outOfBoundsLeft = false;
        }

        if (baseLimb.transform.position.x >= xTo) {
            outOfBoundsRight = true;
            if (continuousMovement && !jumping) direction = Facing.LEFT;
        } else {
            outOfBoundsRight = false;
        }
    }

}
