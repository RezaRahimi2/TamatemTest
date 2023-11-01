namespace GamePlayView
{
    using UnityEngine;

    public class Billboard : MonoBehaviour
    {
        public Camera mainCamera;

        void Start()
        {
            // If no camera referenced, grab the main camera
            mainCamera = Camera.main;
        }

        void Update()
        {
            // Make the billboard face the camera
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }

}