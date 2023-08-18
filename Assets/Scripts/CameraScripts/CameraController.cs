using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public Transform boundary;

    private float pitch = 0f;

    void Update()
    {
        Vector3 pos = transform.position;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        pos += moveDirection * moveSpeed * Time.deltaTime;

        if (Input.GetKey("e")) pos.y += moveSpeed * Time.deltaTime;
        if (Input.GetKey("q")) pos.y -= moveSpeed * Time.deltaTime;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        pitch -= mouseY * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cam.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        pos.x = Mathf.Clamp(pos.x, boundary.position.x - boundary.localScale.x / 2, boundary.position.x + boundary.localScale.x / 2);
        pos.z = Mathf.Clamp(pos.z, boundary.position.z - boundary.localScale.z / 2, boundary.position.z + boundary.localScale.z / 2);
        pos.y = Mathf.Clamp(pos.y, boundary.position.y - boundary.localScale.y / 2, boundary.position.y + boundary.localScale.y / 2); // Restricción vertical

        transform.position = pos;
    }
}
