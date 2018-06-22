using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform camTarget = null;

    public float maxSpeed = 5;
    public float currentSpeed = 0;

    public float slowDownDst = 0.5f;
    public float turnSpeed = 3f;

    Vector3 wantedLocation = Vector3.zero;

    public Vector3 offset = Vector3.zero;

	void Update ()
    {

        if (camTarget == null)
            return;

        wantedLocation = camTarget.position + offset;

        Vector3 difference = wantedLocation - transform.position;
        float dst = difference.magnitude;

        currentSpeed = maxSpeed * (dst / slowDownDst);
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        transform.position = Vector3.LerpUnclamped(transform.position, wantedLocation, currentSpeed * Time.deltaTime);
        transform.LookAt(camTarget);
        
	}

    public void SetCamTarget(Transform target)
    {
        camTarget = target;
    }
}
