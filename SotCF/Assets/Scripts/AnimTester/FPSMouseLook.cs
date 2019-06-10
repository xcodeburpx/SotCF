using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseX, MouseY };
    public RotationAxes axes = RotationAxes.MouseY;

    public float currentSensitivity_X = 4.5f;
    public float currentSensitivity_Y = 4.5f;

    public float sensitivity_X = 4.5f;
    public float sensitivity_Y = 4.5f;

    public float rotation_X, rotation_Y;

    public float minimum_X = -360f;
    public float maximum_X = 360f;

    public float minimum_Y = -60f;
    public float maximum_Y = 60f;

    private Quaternion originalRotation;
    private float mouseSensitivity = 1.7f;

    public Animator anim;
    private Transform chest;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;
        chest = anim.GetBoneTransform(HumanBodyBones.Chest);
    }


    void LateUpdate()
    {
        HandleRotation();
    }

    float ClampAngle(float angle, float min, float max)
    {
        if(angle < -360f)
        {
            angle += 360f;
        }

        if(angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    void HandleRotation()
    {
        if(currentSensitivity_X != mouseSensitivity || currentSensitivity_Y != mouseSensitivity)
        {
            currentSensitivity_X = currentSensitivity_Y = mouseSensitivity;
        }

        sensitivity_X = currentSensitivity_X;
        sensitivity_Y = currentSensitivity_Y;

        if(axes == RotationAxes.MouseX)
        {
            rotation_X += Input.GetAxis("Mouse X") * sensitivity_X;

            rotation_X = ClampAngle(rotation_X, minimum_X, maximum_X);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotation_X,Vector3.up);

            transform.localRotation = originalRotation * xQuaternion;
        }

        if(axes == RotationAxes.MouseY)
        {
            rotation_Y += Input.GetAxis("Mouse Y") * sensitivity_Y;

            rotation_Y = ClampAngle(rotation_Y, minimum_Y, maximum_Y);

            Quaternion yQuaternion = Quaternion.AngleAxis(-rotation_Y, Vector3.right);
            Quaternion chestQuaternion = Quaternion.AngleAxis(-rotation_Y, Vector3.up);
            transform.localRotation = originalRotation * yQuaternion;
            chest.rotation = chest.rotation * chestQuaternion;
        }
    }
}
