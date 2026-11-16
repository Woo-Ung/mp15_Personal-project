using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private const float EDGE_THRESHOLD = 0.45f;

    [SerializeField] private CameraInput _cameraInput;
    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    private void Update()
    {
        Vector2 mousePos = _cameraInput.GetMousePosition();
        Vector2 screenUV = new Vector2(mousePos.x / Screen.width - 0.5f, mousePos.y / Screen.height - 0.5f);


        Vector3 move = Vector3.zero;
        if(screenUV.x < -EDGE_THRESHOLD)
        {
            move.x = -1f;
        }
        if (screenUV.x > EDGE_THRESHOLD)
        {
            move.x = 1f;
        }
        if (screenUV.y < -EDGE_THRESHOLD)
        {
            move.z = -1f;
        }
        if (screenUV.y > EDGE_THRESHOLD)
        {
            move.z = 1f;
        }

        transform.Translate(Time.deltaTime * moveSpeed * move);

        Vector3 disiredPosition = transform.position;

        disiredPosition.x = Mathf.Clamp(disiredPosition.x, minX, maxX);
        disiredPosition.z = Mathf.Clamp(disiredPosition.z, minZ, maxZ);

        transform.position = disiredPosition;
    }
}
