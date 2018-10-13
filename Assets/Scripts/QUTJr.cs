using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUTJr : MonoBehaviour {

    private enum Moving { LEFT = -1, RIGHT = 1 };
    private enum Jumping { INPLACE, FORWARD };

    public float moveSpeed;
    public float jumpHeight;

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

        if (Input.GetKey(jumpInPlace)) Jump(Jumping.INPLACE);

        if (Input.GetKey(jumpForward)) Jump(Jumping.FORWARD);

        if (Input.GetKey(collapse)) ;
    }

    private void Move(Moving direction) {
        //baseLimb.Scale(new Vector3((int)direction, 1, 1));
        baseLimb.Translate(new Vector3(moveSpeed * (int)direction * Time.deltaTime, 0, 0));
    }

    private void Jump(Jumping type) {

    }

}
