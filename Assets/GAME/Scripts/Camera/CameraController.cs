using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Camera{
    [RequireComponent(typeof(Rigidbody))]
    public class CameraController : MonoBehaviour
    {
        // auto-fetched parts
        Rigidbody rb;
        Transform verticalOrbit;
        public Transform orbit;

        // dev/config controlled values
        [SerializeField]
        public float xSensitivity = 1f;
        [SerializeField]
        public float ySensitivity = 1f;
        [SerializeField, Range(0f, 10f)]
        float targetDistance = 3.75f;
        [SerializeField]
        float ymin;
        [SerializeField]
        float ymax;
        [SerializeField, Range(0f, 1f)]
        float tolerance = 0.05f;
        [SerializeField]
        float multiplier;
        
        float targetShoulder = 0.5f;

        // layer mask for what the camera views as a wall
        [SerializeField]
        LayerMask layerMask;

        float rotationY;
        float rotationX;
        Vector2 lookVal;

        [SerializeField]
        Mesh cubemesh;

        // convenient in other scripts
        public float xAngle
        {
            get { return verticalOrbit.rotation.eulerAngles.x; }
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            verticalOrbit = transform.parent;
            orbit = verticalOrbit.parent;
            rotationY = 0;
            rotationX = 0;
        }

        private void LateUpdate()
        {
            // Check if the camera is too far away and move it if needed
            if (transform.localPosition.z > -targetDistance + tolerance)
            {
                rb.velocity = transform.forward * (-targetDistance - (transform.localPosition.z)) * multiplier;

            }
            // if there is nothing behind the camera and it needs to move back then move the camera to where it should be
            else if (transform.localPosition.z < -targetDistance - tolerance && !Physics.SphereCast(transform.position, 0.45f, -transform.forward, out RaycastHit info, 0.2f, layerMask))
            {
                rb.velocity = transform.forward * (-targetDistance - (transform.localPosition.z)) * multiplier;
            }

            // while there is a wall in the way, move it forward until there isn't
            while(Physics.Raycast(transform.position, transform.forward, -transform.localPosition.z, layerMask))
            {
                transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z*0.8f);
            }
            // fix random issues where sometimes the camera would try and not be locked to the z axis. Constraint may be better.
            transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);

            // track rotation internally so we can use nicer values
            rotationY += lookVal.y * -ySensitivity;
            rotationY = Mathf.Clamp(rotationY, ymin, ymax);

            // vertical orbit just changes to the tracked y value
            verticalOrbit.localEulerAngles = new Vector3(rotationY, 0f, 0f);
            // horizontal orbit can be rotate more easily on its own as there's no constraints
            orbit.Rotate(new Vector3(0f, lookVal.x * xSensitivity, 0f));
            // rotate the camera specific rotation as sometimes it looks away on collissions
            transform.localRotation = Quaternion.identity;
            

            // check if there's a wall in the way and if so move to the other shoulder
            if (Physics.BoxCast(transform.position-transform.forward*transform.localPosition.z/2, new Vector3(0.05f, 0.1f, -transform.localPosition.z/2), transform.right, out RaycastHit hitInfo, transform.rotation, 2f, layerMask))
            {
                // TODO this is jittery
                verticalOrbit.transform.localPosition = Vector3.Slerp(verticalOrbit.transform.localPosition, Vector3.left * targetShoulder, Time.deltaTime * 2f);
            }
            else verticalOrbit.transform.localPosition = Vector3.Slerp(verticalOrbit.transform.localPosition, Vector3.right * targetShoulder, Time.deltaTime * 2f);

            // there's edge cases where the camera can fly away, if the camera has attempted to fly away then stop it
            if (Mathf.Abs(transform.localPosition.z) > 20f) transform.localPosition = new Vector3(0f, 0f, -targetDistance);
        }

        public void OnLook(InputValue input)
        {
            lookVal = input.Get<Vector2>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(cubemesh, transform.position - transform.forward * transform.localPosition.z / 2+transform.right*1f, transform.rotation, new Vector3(2f, 0.5f, transform.localPosition.z));
        }
    }
    
}
