using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShadowAlchemy.CameraControl
{
    public class BoundsCameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private BoundsCamera[] childCameras;
        private BoundsCamera currentCamera;

        private void Awake()
        {
            if (target == null) Debug.LogError($"{nameof(target)} not assigned");

            childCameras = GetComponentsInChildren<BoundsCamera>();
            if (childCameras.Length == 0) Debug.LogWarning($"{nameof(BoundsCameraHandler)} has no child {nameof(BoundsCamera)}s");

            foreach (var childCam in childCameras) childCam.enabled = false;
        }

        private void OnEnable()
        {
            if (currentCamera) currentCamera.enabled = true;
        }

        private void OnDisable()
        {
            if (currentCamera) currentCamera.enabled = false;
        }

        private void Update()
        {
            if (currentCamera && currentCamera.Contains(target.position)) return;

            EvalutateCameras();
        }

        private void EvalutateCameras()
        {
            var camsContainingTarget = childCameras.Where(cam => cam.Contains(target.position)).ToArray();
            if (camsContainingTarget.Length == 0) camsContainingTarget = childCameras;

            if (currentCamera) currentCamera.enabled = false;
            var closestCam = camsContainingTarget.Aggregate((minCam, nextCam) => nextCam.CloserThan(minCam, target.position) ? nextCam : minCam);
            currentCamera = closestCam;
            currentCamera.enabled = true;
        }

        public void ToggleEnabled(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            enabled = !enabled;
        }

        private void OnValidate()
        {
            childCameras = GetComponentsInChildren<BoundsCamera>();
        }
    }
}