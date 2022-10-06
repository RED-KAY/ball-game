using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform focus = default; // Transform to focus on.

    [SerializeField, Range(1f, 20f)]
    float distance = 5f; // Distance from the focus 

    Vector3 focusPoint, previousFocusPoint;

    [SerializeField, Min(0f)]
    float focusRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;

    Vector2 orbitAngles = new Vector2(45f, 0f);
    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;

    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    [SerializeField, Min(0f)]
    float alignDelay = 5f; // Delay between player controller rotation and automatic.

    float lastManualRotationTime;

    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    void Awake()
    {
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }

    void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles); //Angle required to rotate in order to bring focus(ball) into the center.
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        
        
        Vector3 lookDirection = lookRotation * Vector3.forward; // Always looks at the focus(ball).
        Vector3 lookPosition = focusPoint - lookDirection * distance; // Follows the ball around.
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }


    // Smoothly and responsively brings ball into center. Creates an effect - speed of the smoothness decreses as the camera approached the focus.
    void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);

            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
        //focusPoint = targetPoint;
    }

    bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera")
            );

        const float e = 0.001f; // ignores minute changes in mouse position.
        if(input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    // You want the camera to adjust to ball movements automatically to keep the ball in focus and for easy controls.
    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay) // Gives a delay between last player controlled rotation of camera and auto rotation. Otherwise the automatic rotation call may overlap with manual rotation call.
        {
            return false;
        }
        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z
        );
        float movementDeltaSqr = movement.sqrMagnitude; // suared magnitude used to prevent unnecessary magnitude calls which uses square root functions which are cpu intensive.
        if (movementDeltaSqr < 0.0001f) // ignore small minute movements.
        {
            return false;
        }
        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        //float rotationChange = rotationSpeed * Time.unscaledDeltaTime;
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr); // We create dependency on the degree of change in movement to create a responsive & lively camera rotation feedback.
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        } else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    // To prevent camera from being upside down.
    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

}
