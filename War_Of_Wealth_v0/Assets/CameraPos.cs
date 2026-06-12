using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public Transform cameraPosition;

    private void Update()
    {
        if (cameraPosition == null)
            return;

        float targetY = transform.parent != null ? transform.parent.position.y : cameraPosition.position.y;
        if (transform.parent == null && cameraPosition.parent != null)
            targetY = cameraPosition.parent.position.y;

        Vector3 targetPos = cameraPosition.position;
        targetPos.y = targetY;
        transform.position = targetPos;
    }
}
