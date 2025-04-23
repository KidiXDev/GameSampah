using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    private readonly float lifetime = 9f;
    private static float currentSpeed;
    public static float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    private float timer = 0f;
    private bool isGrabbed = false;
    private Vector3 grabOffset;

    void Update()
    {
        if (!isGrabbed)
        {
            // Move object to the left as usual
            transform.position += currentSpeed * Time.deltaTime * Vector3.left;
        }
        else
        {
            // Object is grabbed, follow mouse with offset
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z; // biar ga berubah z-nya
            transform.position = mouseWorldPos + grabOffset;
        }

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        isGrabbed = true;

        // Hitung offset dari posisi mouse ke object
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z; // jaga-jaga biar ga ngegeser ke z yang salah
        grabOffset = transform.position - mouseWorldPos;
    }

    void OnMouseUp()
    {
        isGrabbed = false;
    }
}
