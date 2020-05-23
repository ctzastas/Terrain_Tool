using UnityEngine;

public class Controller : MonoBehaviour {

    private Rigidbody rb;
    public float speed = 5f;
    private float x;
    private float z;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z).normalized * speed;
        rb.AddForce(move, ForceMode.Force);
    }
}
