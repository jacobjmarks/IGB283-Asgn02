using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdjustCamera : MonoBehaviour {

    public float minZoom = 5f;
    public float padding = 3f;

    private float dampTime = 0.2f;
    private Vector3 desiredPosition;
    private Vector3 moveVelocity;
    private float zoomSpeed;

    private Camera cam;
    private List<QUTJr> players;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Update () {
        players = FindObjectsOfType(typeof(QUTJr)).Cast<QUTJr>().ToList();
        
        desiredPosition = new Vector3(GetAveragePlayerX(), transform.position.y, transform.position.z);

        // Move
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, 0.2f);
        // Zoom
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, GetDesiredZoom(), ref zoomSpeed, 0.2f);
    }

    /// <summary>
    /// Get the average X position of all players.
    /// </summary>
    /// <returns>The average X position of all players.</returns>
    private float GetAveragePlayerX() {
        return players.Select(p => p.transform.Find("Base").position.x).Aggregate((a, b) => a + b) / players.Count;
    }

    /// <summary>
    /// Calculate the desired orthographic size of the camera, such that all players will be visible on the screen.
    /// </summary>
    /// <returns>The optimal orthographic camera size.</returns>
    private float GetDesiredZoom() {
        float size = 0;
        foreach (QUTJr player in players) {
            Vector3 distanceFromDesired = player.transform.Find("Base").position - desiredPosition;
            size = Mathf.Max(size, Mathf.Abs(distanceFromDesired.x) / cam.aspect);
        }
        size += padding;
        
        size = Mathf.Max(size, minZoom);

        return size;
    }

}
