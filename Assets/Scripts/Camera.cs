using UnityEngine;

public class Camera : MonoBehaviour {

    public GameObject player;
    private Vector3 offset;
    private Vector3 rotate;
    private float x;
    private float y;

    // Use this for initialization
    void Start() {
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate() {
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void Update() {
        // Rotate camera with mouse
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        rotate = new Vector3(x, y * -1, 0);
        transform.eulerAngles = transform.eulerAngles - rotate;
    }
}
