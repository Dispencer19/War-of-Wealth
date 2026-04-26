using UnityEngine;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    [Header("Dice Settings")]
    public float rollDuration = 0.6f;

    // Rotations for each face (adjust if your model is oriented differently)
    
    private static readonly Quaternion[] faceRotations = new Quaternion[]
    {
        Quaternion.Euler(-180, 0, 0),       // 1
        Quaternion.Euler(0, 0, 0),      // 2
        Quaternion.Euler(0, 90, -90),     // 3
        Quaternion.Euler(90, 0, 0),     // 4
        Quaternion.Euler(0, 0, 90),      // 5
        Quaternion.Euler(-90, 0, 0)      // 6
    };

    public Tween Roll(int result)
    {
        // Random spin
        return transform
            .DORotate(Random.insideUnitSphere * 720f, rollDuration, RotateMode.FastBeyond360)
            .OnComplete(() =>
            {
                transform.rotation = faceRotations[result - 1];
            });
    }
}
