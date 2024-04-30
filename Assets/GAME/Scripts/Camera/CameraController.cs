using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Camera{
    [RequireComponent(typeof(Rigidbody))]
    public class CameraController : MonoBehaviour
    {
        Rigidbody rb;
        [SerializeField, Range(0f, 10f)]
        float targetDistance = -2.5f;
        [SerializeField, Range(0f, 1f)]
        float tolerance = 0.1f;
        [SerializeField]
        float multiplier;
        Transform verticalOrbit;
        public Transform orbit;
        [SerializeField]
        public float xSensitivity = 1f;
        [SerializeField]
        public float ySensitivity = 1f;
        [SerializeField]
        float ymin;
        [SerializeField]
        float ymax;
        [SerializeField]
        LayerMask layerMask;
        float rotationY;
        Vector2 lookVal;
        float targetShoulder = 0.85f;
        [SerializeField]
        Mesh cubemesh;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            verticalOrbit = transform.parent;
            orbit = verticalOrbit.parent;
            rotationY = 0;
        }

        private void LateUpdate()
        {
            
            if(transform.localPosition.z < -targetDistance - tolerance || transform.localPosition.z > -targetDistance + tolerance)
            {
                
                rb.velocity = (transform.forward * (-targetDistance + Mathf.Abs(transform.localPosition.z)) * multiplier);
            }
            //transform.LookAt(orbit);
            while(Physics.Raycast(transform.position, transform.forward, -transform.localPosition.z, layerMask))
            {
                transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z*0.75f);
            }
            transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
            Debug.DrawRay(transform.position, transform.forward*-transform.localPosition.z, Color.green);

            rotationY += lookVal.y * -ySensitivity;
            rotationY = Mathf.Clamp(rotationY, ymin, ymax);
            verticalOrbit.localEulerAngles = new Vector3(rotationY, 0f, 0f);
            orbit.Rotate(new Vector3(0f, lookVal.x * xSensitivity, 0f));
            transform.localRotation = Quaternion.identity;
            //Debug.DrawRay(transform.position - transform.forward * transform.localPosition.z / 2, transform.right*2f, Color.red);
            if (Physics.BoxCast(transform.position-transform.forward*transform.localPosition.z/2, new Vector3(0.05f, 0.25f, -transform.localPosition.z/2), transform.right, out RaycastHit hitInfo, transform.rotation, 2f, layerMask))
            {
                verticalOrbit.transform.localPosition = Vector3.Slerp(verticalOrbit.transform.localPosition, Vector3.left * targetShoulder, Time.deltaTime * 2f);
            }
            else verticalOrbit.transform.localPosition = Vector3.Slerp(verticalOrbit.transform.localPosition, Vector3.right * targetShoulder, Time.deltaTime * 2f);
            //lookVal = new();
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
