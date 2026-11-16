using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private const float EDGE_THRESHOLD = 0.45f;

    [SerializeField] private CameraInput _cameraInput;
    [SerializeField] private float moveSpeed = 10f;

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
    }
}
