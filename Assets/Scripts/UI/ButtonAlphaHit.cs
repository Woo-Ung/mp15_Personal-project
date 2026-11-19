using UnityEngine;
using UnityEngine.UI;

public class ButtonAlphaHit : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float _alphaThreshold = 0.1f;

    [SerializeField] private Image[] _image;

    private void Awake()
    {
        foreach (Image image in _image)
        {
            image.alphaHitTestMinimumThreshold = _alphaThreshold;
            Debug.Log(image.alphaHitTestMinimumThreshold);
        }
    }
}
