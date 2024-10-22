﻿using UnityEngine;

[System.Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;


    private Quaternion _CharacterTargetRot;
    private Quaternion _CameraTargetRot;

    public MouseLook()
    {

    }

    public void Init(Transform character, Transform camera)
    {
        _CharacterTargetRot = character.localRotation;
        _CameraTargetRot = camera.localRotation;
    }


    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

        _CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        _CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            _CameraTargetRot = ClampRotationAroundXAxis(_CameraTargetRot);

        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, _CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, _CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = _CharacterTargetRot;
            camera.localRotation = _CameraTargetRot;
        }
    }


    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}
