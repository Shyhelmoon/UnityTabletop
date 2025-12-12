using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;
    [SerializeField] private bool useEdgePanning = true;
    [SerializeField] private bool useMiddleMouseDrag = true;
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    
    [Header("Boundaries (Optional)")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -50f;
    [SerializeField] private float maxY = 50f;
    
    private Camera cam;
    private Vector3 lastMousePosition;
    private bool isDragging = false;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandlePanning();
        HandleZoom();
    }

    void HandlePanning()
    {
        Vector3 pos = transform.position;

        // WASD movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            pos.y += panSpeed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            pos.y -= panSpeed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            pos.x += panSpeed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            pos.x -= panSpeed * Time.deltaTime;

        // Edge panning with mouse
        if (useEdgePanning)
        {
            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
                pos.x += panSpeed * Time.deltaTime;
            
            if (Input.mousePosition.x <= panBorderThickness)
                pos.x -= panSpeed * Time.deltaTime;
            
            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
                pos.y += panSpeed * Time.deltaTime;
            
            if (Input.mousePosition.y <= panBorderThickness)
                pos.y -= panSpeed * Time.deltaTime;
        }

        // Middle mouse drag
        if (useMiddleMouseDrag)
        {
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }
            
            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                pos.x -= delta.x * panSpeed * Time.deltaTime * 0.1f;
                pos.y -= delta.y * panSpeed * Time.deltaTime * 0.1f;
                lastMousePosition = Input.mousePosition;
            }
        }

        // Apply boundaries
        if (useBoundaries)
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = pos;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (cam.orthographic)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            // For perspective camera (if you switch later)
            Vector3 pos = transform.position;
            pos.z += scroll * zoomSpeed;
            pos.z = Mathf.Clamp(pos.z, -maxZoom, -minZoom);
            transform.position = pos;
        }
    }
}