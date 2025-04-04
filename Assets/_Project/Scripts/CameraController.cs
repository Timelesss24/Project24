using System;
using System.Collections;
using Cinemachine;
using KBCore.Refs;
using Managers;
using UnityEngine;

namespace Timelesss
{
    public class CameraController : ValidatedMonoBehaviour
    {
        [Header("References")] [SerializeField, Anywhere]
        InputReader input;

        [SerializeField, Self] CinemachineFreeLook freeLookVCam;

        [Header("Settings")] [SerializeField, Range(0.5f, 3)]
        float speedMultiplier = 1f;

        bool isRMBPressed;
        bool cameraMovementLock;

        void Start()
        {
            OnEnableMouseControlCamera();    
        }

        void OnEnable()
        {
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        void OnDisable()
        {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }
        
        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (cameraMovementLock || Cursor.lockState != CursorLockMode.Locked) return;
            
            if(isDeviceMouse && !isRMBPressed) return;
            
            // If the device is mouse use fixedDeltaTime, otherwise use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;
            
            // Set the camera axis values
            freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
        }

        void OnEnableMouseControlCamera()
        {
            
            isRMBPressed = true;

           

            StartCoroutine(DisableMouseForFrame());
        }

        void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;

            // UnLock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Reset the camera axis to prevent jumping when re-enabling mouse control
            freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
            freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
        }
        
        IEnumerator DisableMouseForFrame()
        {
            
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => UIManager.Instance.CurrentPopupCount > 0);
            
            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            cameraMovementLock = false;
        }
        
    }
}