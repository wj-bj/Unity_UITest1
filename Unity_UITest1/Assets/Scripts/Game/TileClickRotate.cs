
using UnityEngine;
using DG.Tweening;

public class TileClickRotate : MonoBehaviour
{
    [Tooltip("Rotation duration in seconds. Set to -1 for infinite rotation.")]
    public float rotateDuration = -1f;

    [Tooltip("Rotation speed when rotateDuration = -1 (degrees per second).")]
    public float infiniteRotateSpeed = 180f;

    private Tween currentTween;
    private bool isRotating = false;

    void OnEnable()
    {
        // Ensure the tile is not rotating when enabled
        isRotating = false;
        // currentTween?.Kill(); // Clean up any existing tween
        RotateTile();
    }

    void OnDisable()
    {
        // Stop rotation when the tile is disabled
        isRotating = false;
        currentTween?.Kill(); // Clean up any existing tween
    }

    void RotateTile()
    {
        
        if (rotateDuration == -1)
        {
            
            // Start infinite rotation if not already rotating
            if (isRotating) return;
            isRotating = true;

            currentTween?.Kill(); // Kill any existing tween
            currentTween = transform.DORotate(
                    
                    new Vector3(0, 360, 0),
                    360f / infiniteRotateSpeed, // One full rotation time
                    RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
        else
        {
            if (isRotating) return;
            isRotating = true;

            currentTween?.Kill();
            currentTween = transform.DORotate(
                    new Vector3(0, 0, 360),
                    rotateDuration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetRelative(true)
                .OnComplete(() => isRotating = false);
        }
    }

    private void OnDestroy()
    {
        currentTween?.Kill(); // Clean up DOTween on destroy
    }
}