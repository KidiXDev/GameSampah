using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    private bool isGrabbed = false;
    private Vector3 offset;
    private Rigidbody2D rb;
    private bool isOverCorrectBin = false;
    private Collider2D currentBinCollider = null;
    private Vector3 lastMousePosition;
    private Vector3 mouseVelocity;
    private float throwForceMultiplier = 0.3f;
    private float customGravityScale = 5f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }
        col.isTrigger = true;

        if (col is BoxCollider2D boxCol)
        {
            Vector2 size = boxCol.size;
            size.x *= 1.5f;
            boxCol.size = size;
        }
    }

    void OnMouseDown()
    {
        isGrabbed = true;
        offset = transform.position - GetMouseWorldPos();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        lastMousePosition = GetMouseWorldPos();

        var moveComponent = GetComponent<ObjectMover>();
        if (moveComponent != null)
        {
            gameObject.GetComponent<ObjectMover>().enabled = false;
        }
    }


    void OnMouseDrag()
    {
        if (isGrabbed)
        {
            Vector3 currentMousePos = GetMouseWorldPos();
            transform.position = currentMousePos + offset;

            // Calculate velocity
            mouseVelocity = (currentMousePos - lastMousePosition) / Time.deltaTime;
            lastMousePosition = currentMousePos;
        }
    }


    void OnMouseUp()
    {
        isGrabbed = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Atur gravity biar gak lambat banget jatuhnya
        rb.gravityScale = customGravityScale;

        // Apply lemparan tapi dikali multiplier biar gak terlalu OP
        rb.linearVelocity = (Vector2)mouseVelocity * throwForceMultiplier;

        if (isOverCorrectBin && currentBinCollider != null)
        {
            OnTriggerEnter2D(currentBinCollider);
        }
    }



    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tempatSampahTag = other.tag;
        string tagSampah = gameObject.tag;

        if (other.CompareTag("FallPoint"))
        {
            Debug.Log("Waduh, sampah jatoh ke FallPoint!");
            Destroy(gameObject);
            return;
        }

        if (tagSampah == other.gameObject.tag || tagSampah == other.gameObject.tag)
        {
            return;
        }

        if ((tempatSampahTag == "Organik" && tagSampah == "SampahOrganik") ||
            (tempatSampahTag == "Non-Organik" && tagSampah == "SampahNonOrganik"))
        {
            isOverCorrectBin = true;
            currentBinCollider = other;
            if (!isGrabbed)
            {
                Destroy(gameObject);
                // Debug.Log("Sampah sesuai tempatnya dibuang!");
            }
        }
        else
        {
            if (!isGrabbed)
            {
                isOverCorrectBin = false;
                currentBinCollider = null;
                // Debug.Log("Sampah salah tempat, gak dibuang!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentBinCollider)
        {
            isOverCorrectBin = false;
            currentBinCollider = null;
        }
    }
}
