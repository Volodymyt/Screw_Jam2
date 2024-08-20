using UnityEngine;

public class CubeRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Vector3 previousPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = touch.position - (Vector2)previousPosition;

                float rotationAroundYAxis = -direction.x * rotationSpeed * Time.deltaTime;
                float rotationAroundXAxis = direction.y * rotationSpeed * Time.deltaTime;

                transform.Rotate(Vector3.up, rotationAroundYAxis, Space.World);
                transform.Rotate(Vector3.right, rotationAroundXAxis, Space.World);

                previousPosition = touch.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            previousPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = Input.mousePosition - previousPosition;

            float rotationAroundYAxis = -direction.x * rotationSpeed * Time.deltaTime;
            float rotationAroundXAxis = direction.y * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotationAroundYAxis, Space.World);
            transform.Rotate(Vector3.right, rotationAroundXAxis, Space.World);

            previousPosition = Input.mousePosition;
        }
    }
}
