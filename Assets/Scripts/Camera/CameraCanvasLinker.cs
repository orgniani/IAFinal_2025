using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    public class CameraCanvasLinker : MonoBehaviour
    {
        [SerializeField] private List<Canvas> canvas;

        private void Start()
        {
            Camera mainCamera = Camera.main;

            foreach (var c in canvas)
            {
                if (c != null)
                {
                    c.worldCamera = mainCamera;
                    c.planeDistance = 5f;
                }

                else
                    Debug.LogWarning("One of the canvases is null. Please check your Canvas references.");
            }
        }
    }
}