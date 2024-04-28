using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Camera;
using Project.Grapple;
using Project.Utilities;

namespace Project.Character
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        Mesh capsuleMesh;

        public Rigidbody rb;
        Vector2 moveAmount = new Vector2();
        [SerializeField]
        float movementMultiplier;
        bool isGrounded = false;
        [SerializeField]
        LayerMask layerMask;
        [SerializeField]
        float jumpStrength;
        [SerializeField]
        CameraController mainCamera;
        [SerializeField]
        float turnSpeed = 1f;
        [SerializeField]
        float maxGroundSpeed = 20f;
        [SerializeField]
        int maxJumps = 2;
        [SerializeField]
        int jumps = 2;
        bool jumping = false;
        List<GrappleController> grapples = new List<GrappleController>();
        [SerializeField]
        Collider lastWallJump;
        Vector3 jumpVector = new Vector3(0f, 1f, 0f);
        public bool grappling
        {
            get { return grapples.FindAll(x => x.grappling).Count > 0; }
        }

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            grapples = new(GetComponentsInChildren<GrappleController>());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnMove(InputValue input)
        {
            Vector2 movement = input.Get<Vector2>();
            moveAmount = movement.normalized;
        }

        public void OnJump(InputValue input)
        {
            jumping = input.isPressed;
        }

        private void OnDrawGizmos()
        {

            Vector3 groundedOffset = new Vector3(0f, -0.8f, 0f);
            Gizmos.DrawWireSphere(transform.position + groundedOffset, 0.3f);
            Gizmos.color = Color.green;
            Vector3 vector = new Vector3(moveAmount.x, 0f, moveAmount.y);
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + Vector3.left * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + Vector3.right * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + Vector3.forward * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + Vector3.back * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + (Vector3.right +Vector3.forward).normalized * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + (Vector3.left + Vector3.forward).normalized * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + (Vector3.right +Vector3.back).normalized * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
            Gizmos.DrawWireMesh(capsuleMesh, transform.position + (Vector3.left + Vector3.back).normalized * 0.3f, Quaternion.identity, new Vector3((0.4f / 0.5f), 1f, (0.4f / 0.5f)));
        }

        private void FixedUpdate()
        {
            Vector3 groundedOffset = new Vector3(0f, -0.9f, 0f);
            if (Physics.CheckSphere(transform.position + groundedOffset, 0.3f, layerMask))
            {
                if (!isGrounded)
                {
                    jumps = maxJumps;
                }
                isGrounded = true;
                lastWallJump = null;
                //rb.AddForce(Vector3.up * jumpStrength);
            }
            else
            {
                isGrounded = false;
                if(jumps > maxJumps - 1 && !grappling)
                {
                    jumps -= 1;
                }
            }
            Vector3 vector = new Vector3(moveAmount.x, 0f, moveAmount.y);
            Quaternion movementRotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);
            if(vector != Vector3.zero)
            {
                Quaternion rotationChange = Quaternion.Lerp(transform.rotation, mainCamera.orbit.transform.rotation, turnSpeed*Time.deltaTime);
                Quaternion rotationDifference = transform.rotation * Quaternion.Inverse(rotationChange);
                transform.rotation = rotationChange;
                mainCamera.orbit.transform.rotation = mainCamera.orbit.transform.rotation * rotationDifference;
            }
            bool wallJumped = false;
            if (Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.left, out RaycastHit hitInfo, 0.3f, layerMask) || 
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.right, out hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.forward, out hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.back, out hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.left + Vector3.forward, out  hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.right + Vector3.forward, out hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.left + Vector3.back, out hitInfo, 0.3f, layerMask) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Vector3.right + Vector3.back, out hitInfo, 0.3f, layerMask))
            {
                if(jumps == 0 && !isGrounded && rb.velocity.x != 0 && rb.velocity.z != 0)
                {
                    if (lastWallJump == null || lastWallJump != hitInfo.collider)
                    {
                        jumps++;
                        lastWallJump = hitInfo.collider;
                    }
                }
                Debug.DrawRay(transform.position, new Vector3(0f, 1f, 0f) + hitInfo.normal.normalized * jumpStrength/2f* movementMultiplier - Vector3.Dot(hitInfo.normal, movementRotation * vector) * hitInfo.normal);
                if (jumping && rb.velocity.x != 0 && rb.velocity.z != 0 && jumps > 0)
                {
                    jumpVector = new Vector3(0f, 1f, 0f) + hitInfo.normal.normalized * jumpStrength/3f * movementMultiplier - Vector3.Dot(hitInfo.normal, movementRotation * vector) * hitInfo.normal;
                    wallJumped = true;
                }
                else jumpVector = new Vector3(0f, 1f, 0f);
            }
            if (jumping && jumps > 0)
            {
                Debug.Log("jumping");
                jumps -= 1;
                jumping = false;
                rb.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);
                if(jumps < maxJumps - 1) rb.AddRelativeForce(Vector3.up * jumpStrength/2, ForceMode.Impulse);
                if(grappling && jumps < maxJumps - 1)
                {
                    List<Vector3> positions = new List<Vector3>();
                    foreach (GrappleController grapple in grapples.FindAll(x => x.grappling))
                    {
                        rb.AddForce((grapple.connections.Peek().transform.position-transform.position).normalized * jumpStrength / 2, ForceMode.Impulse);

                    }
                }
                if(grappling && jumps == 0)
                {
                   StartCoroutine(ResetJumps());
                }
            }
            jumping = false;
            Debug.DrawRay(transform.position, movementRotation * vector);
            if (Physics.CapsuleCast(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, 0.4f, Quaternion.FromToRotation(Vector3.forward, transform.forward) * vector, out hitInfo, 0.21f, layerMask))
            {
                Debug.DrawRay(transform.position, movementRotation * vector, Color.blue);
                Debug.DrawRay(transform.position, (movementRotation * vector - Vector3.Dot(hitInfo.normal, movementRotation * vector) * hitInfo.normal * 1.1f) * movementMultiplier, Color.red);
                rb.AddForce((movementRotation * vector - Vector3.Dot(hitInfo.normal, movementRotation * vector) * hitInfo.normal * 1.1f) * movementMultiplier, ForceMode.VelocityChange);
            }
            else if (Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z) < maxGroundSpeed || (!isGrounded && !wallJumped))
            {
                rb.AddRelativeForce(vector * movementMultiplier, ForceMode.VelocityChange);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxGroundSpeed+5);
            }
        }

        IEnumerator ResetJumps()
        {
            while(grappling && !isGrounded && jumps < maxJumps)
            {
                yield return new WaitForSeconds(5f);
                if (grappling && !isGrounded && jumps < maxJumps) jumps++;
            }
        }
    }
}
