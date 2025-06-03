using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Camera;
using Project.Grapple;
using Project.Utilities;
using Project.Shooter;

namespace Project.Character
{
    public class CharacterController : MonoBehaviour
    {
        // auto-fetched components
        public Rigidbody rb;
        List<Gun> guns = new();
        List<GrappleController> grapples = new List<GrappleController>();
        [SerializeField]
        CameraController mainCamera;

        // dev/config controlled values
        [SerializeField]
        GameObject respawning;
        [SerializeField]
        GameObject HUD;
        [SerializeField]
        float movementMultiplier;
        [SerializeField]
        float jumpStrength;
        [SerializeField]
        float turnSpeed = 1f;
        [SerializeField]
        float maxGroundSpeed = 20f;
        [SerializeField]
        float maxGroundSprint = 25f;
        [SerializeField]
        int maxJumps = 2;
        [SerializeField]
        int jumps = 2;
        [SerializeField]
        float maxHealth = 100;
        public float health;
        public Animator animator;

        public bool _canMove;
        public bool canMove
        {
            get { return _canMove; }
            set { _canMove = value; }
        }
        public bool _canJump;
        public bool canJump
        {
            get { return _canJump; }
            set { _canJump = value; }
        }
        public bool _canSprint;
        public bool canSprint
        {
            get { return _canSprint; }
            set { _canSprint = value; }
        }
        [SerializeField]
        private bool _canShoot;
        public bool canShoot
        {
            set
            {
                _canShoot = value;
                if (!_canShoot)
                {
                    foreach (Gun gun in guns)
                    {
                        if (gun.isActiveAndEnabled) gun.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (Gun gun in guns)
                    {
                        if (!gun.isActiveAndEnabled) gun.gameObject.SetActive(true);
                    }
                }
            }
            get
            {
                return _canShoot;
            }
        }
        [SerializeField]
        private bool _canGrapple;
        public bool canGrapple
        {
            set
            {
                _canGrapple = value;
                if (!_canGrapple)
                {
                    foreach (GrappleController grapple in grapples)
                    {
                        if (grapple.isActiveAndEnabled) grapple.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (GrappleController grapple in grapples)
                    {
                        if (!grapple.isActiveAndEnabled) grapple.gameObject.SetActive(true);
                    }
                }
            }
            get
            {
                return _canGrapple;
            }
        }

        // internally tracked values
        Vector2 moveAmount = new Vector2();
        public bool isGrounded = false;
        [SerializeField]
        bool sprinting = false;
        bool jumping = false;
        Vector3 jumpVector = new Vector3(0f, 1f, 0f);
        [SerializeField]
        public Vector3 lastWallJump;
        public Vector3 lastWallNormal;
        public bool walled;
        Vector3 groundedOffset = new Vector3(0f, 0.065f, 0f);
        public Transform checkpoint;

        // what is ground/walls
        [SerializeField]
        LayerMask collidable;
        [SerializeField]
        LayerMask jumpable;

        [SerializeField]
        Mesh capsuleMesh;

        // dynamic values to more easily figure out the player's actions
        public bool grappling
        {
            get { return grapples.FindAll(x => x.grappling).Count > 0; }
        }
        public bool firing
        {
            get { return guns.FindAll(x => x.firing).Count > 0; }
        }


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            grapples = new(GetComponentsInChildren<GrappleController>());
            guns = new(GetComponentsInChildren<Gun>());
            mainCamera = UnityEngine.Camera.main.GetComponent<CameraController>();
            checkpoint = transform;
            health = maxHealth;

            canMove = _canMove;
            canSprint = _canSprint;
            canJump = _canJump;

            if (!_canShoot)
            {
                foreach (Gun gun in guns)
                {
                    if (gun.isActiveAndEnabled) gun.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (Gun gun in guns)
                {
                    if (!gun.isActiveAndEnabled) gun.gameObject.SetActive(true);
                }
            }

            if (!_canGrapple)
            {
                foreach (GrappleController grapple in grapples)
                {
                    grapple.marker.enabled = false;
                    if (grapple.isActiveAndEnabled) grapple.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (GrappleController grapple in grapples)
                {
                    grapple.marker.enabled = true;
                    if (!grapple.isActiveAndEnabled) grapple.gameObject.SetActive(true);
                }
            }
        }

        public void OnMove(InputValue input)
        {
            Vector2 movement = input.Get<Vector2>();
            moveAmount = movement.normalized * Mathf.Clamp01(movement.magnitude);
        }

        public void OnJump(InputValue input)
        {
            jumping = input.isPressed;
        }

        public void OnSprint(InputValue input)
        {
            sprinting = !sprinting;
        }

        private void OnDrawGizmos()
        {

            Vector3 groundedOffset = new Vector3(0f, 0.065f, 0f);
            Gizmos.DrawWireSphere(transform.position + groundedOffset, 0.1f);
            Gizmos.color = Color.green;
            Vector3 vector = new Vector3(moveAmount.x, 0f, moveAmount.y);
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + Vector3.left * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + Vector3.right * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + Vector3.forward * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + Vector3.back * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + (Vector3.right +Vector3.forward).normalized * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + (Vector3.left + Vector3.forward).normalized * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + (Vector3.right +Vector3.back).normalized * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));
            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + (Vector3.left + Vector3.back).normalized * 0.25f, Quaternion.identity, new Vector3((0.2f / 0.5f), .8f, (0.2f / 0.5f)));

            //Gizmos.DrawWireMesh(capsuleMesh, new Vector3(0f, 0.8f, 0f) + transform.position + Quaternion.FromToRotation(Vector3.forward, transform.forward) * vector * 0.135f, Quaternion.identity, new Vector3(0.175f / 0.5f, 0.8f, 0.175f / 0.5f));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, 0.18f);
        }

        private void FixedUpdate()
        {

            // SETUP

            // moar gravity to counteract the drag slowing the player down
            rb.AddForce(Physics.gravity * rb.drag, ForceMode.Acceleration);

            // check whether the player is grounded
            CheckGrounded();

            // set up the basis of movement
            Vector3 vector = new Vector3(moveAmount.x, 0f, moveAmount.y);
            Quaternion movementRotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);


            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            // rotate towards camera
            LookInDirection(vector);

            // check if the player is against a wall
            walled = false;
            CheckWalls(vector, movementRotation);


            //JUMPS

            if (canJump)
            {
                // if trying to jump and allowed to jump, lets jump
                if (jumping && jumps > 0)
                {
                    // default jump
                    jumps -= 1;
                    jumping = false;
                    rb.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);
                    animator.Play("Movement.Jump");
                    // extra bit of jump for the jump pack
                    if (jumps < maxJumps - 1 && maxJumps > 1)
                    {
                        rb.AddRelativeForce(Vector3.up * jumpStrength / 2, ForceMode.Impulse);
                        animator.SetTrigger("Double Jump");
                    }

                    // boost in the direction of the grapples a bit if you're not on the ground
                    if (grappling && jumps < maxJumps - 1 && maxJumps > 1)
                    {
                        foreach (GrappleController grapple in grapples.FindAll(x => x.grappling))
                        {
                            if (!walled)
                            {
                                // wall jump takes priority
                                rb.AddForce((grapple.connections.Peek().transform.position - transform.position).normalized * jumpStrength / 2, ForceMode.Impulse);
                            }
                            // even more bonus up direction (user feedback)
                            rb.AddForce(Vector3.up / 2, ForceMode.Impulse);
                        }
                    }

                    // allow the player to reset their jump
                    // TODO use timer system?
                    if (grappling && jumps == 0)
                    {
                        StartCoroutine(ResetJumps());
                    }
                }
            }

            // only allow jumping on the frame the player was trying to (no holding it or recalculating it unless they spam)
            jumping = false;


            // MOVEMENT
            if (canMove)
            {
                if (Physics.CapsuleCast(transform.position + Vector3.up * 1.35f, transform.position + Vector3.up * 0.25f, 0.175f, Quaternion.FromToRotation(Vector3.forward, transform.forward) * vector, out RaycastHit hitInfo, 0.1325f, collidable))
                {
                    // if trying to move into a wall, nudge them away a bit
                    rb.AddForce((movementRotation * vector - Vector3.Dot(hitInfo.normal, movementRotation * vector) * hitInfo.normal * 1.025f) * movementMultiplier, ForceMode.VelocityChange);
                }
                else if (Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z) < maxGroundSprint && isGrounded && sprinting && canSprint)
                {
                    // if you're sprinting add a lot of value
                    rb.AddRelativeForce(vector * movementMultiplier * 1.5f, ForceMode.VelocityChange);
                }
                else if (Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z) < maxGroundSpeed || (!isGrounded && !walled))
                {
                    // if youre not sprinting, or in the air but not against a wall then add a little bit
                    rb.AddRelativeForce(vector * movementMultiplier, ForceMode.VelocityChange);
                }
            }

            // ground speed caps
            if (isGrounded && (!sprinting || !canSprint))
            {
                Vector3 clampedVelocity = Vector3.ClampMagnitude(rb.velocity.Flat(), maxGroundSpeed);
                rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
            }
            else if (isGrounded && sprinting && canSprint)
            {
                Vector3 clampedVelocity = Vector3.ClampMagnitude(rb.velocity.Flat(), maxGroundSprint);
                rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
            }
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            animator.SetFloat("Velocity X", localVelocity.x);
            animator.SetFloat("Velocity Y", localVelocity.y);
            animator.SetFloat("Velocity Z", localVelocity.z);
            animator.SetBool("Grounded", isGrounded);
            
        }

        private void CheckWalls(Vector3 vector, Quaternion movementRotation)
        {
            // big list of checks to see where the nearby walls are, 8 directions
            if (Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.left,                    out RaycastHit hitInfo, 0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.right,                   out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.forward,                 out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.back,                    out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.left + Vector3.forward,  out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.right + Vector3.forward, out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.left + Vector3.back,     out hitInfo,            0.165f, jumpable) ||
                Physics.CapsuleCast(transform.position + Vector3.up * 1.2f, transform.position + Vector3.up * 0.4f, 0.175f, Vector3.right + Vector3.back,    out hitInfo,            0.165f, jumpable))
            {
                if (rb.velocity.y < 0)
                {
                    // if the player is moving down at all make them slide down the wall
                    walled = true;
                    rb.AddForce(-Physics.gravity * (0.8f + rb.drag) * (Vector3.Magnitude(new Vector3(rb.velocity.x, rb.velocity.z)) > 1f ? 1 : 0.5f), ForceMode.Acceleration);
                }
                if (canJump)
                {
                    // if the player is not on the ground, has no jumps and is moving/trying to move then do another check
                    // CHECK IF THIS SHOULD HAVE A || SOMEWHERE
                    if (jumps == 0 && !isGrounded && rb.velocity.x != 0 && rb.velocity.z != 0)
                    {
                        Debug.Log("Here");
                        // if the player hasn't wall jumped since touching the ground, has moved a lot, or has changed the angle they're jumping from then allow them to gain a jump
                        if (lastWallJump == Vector3.positiveInfinity || Vector3.Distance(transform.position, lastWallJump) > 10f || (Vector3.Angle(lastWallNormal, hitInfo.normal) > 5f && lastWallNormal != Vector3.positiveInfinity))
                        {
                            jumps++;
                            lastWallJump = hitInfo.point;
                            lastWallNormal = hitInfo.normal;
                        }
                    }

                    // if the player is trying to jump and is moving then apply some wall jump logic
                    if (jumping && rb.velocity.x != 0 && rb.velocity.z != 0 && jumps > 0)
                    {

                        Debug.Log("THIS");
                        jumpVector = new Vector3(0f, 1f, 0f) + hitInfo.normal.normalized * jumpStrength / (Physics.gravity.magnitude * rb.mass) - (Vector3.Dot(hitInfo.normal, movementRotation * vector) - 1) / 2f * hitInfo.normal;
                    }
                    else jumpVector = new Vector3(0f, 1f, 0f);
                }
                else jumpVector = new Vector3(0f, 1f, 0f);
            }
            else jumpVector = new Vector3(0f, 1f, 0f);
        }

        private void LookInDirection(Vector3 vector)
        {
            // if the player is moving or firing the character should look in the same direction as the player, but over time
            if (vector != Vector3.zero || firing)
            {
                Quaternion rotationChange = Quaternion.Lerp(transform.rotation, mainCamera.orbit.transform.rotation, turnSpeed * Time.deltaTime);
                Quaternion rotationDifference = transform.rotation * Quaternion.Inverse(rotationChange);
                transform.rotation = rotationChange;
                mainCamera.orbit.rotation = mainCamera.orbit.rotation * rotationDifference;
            }
        }

        private void CheckGrounded()
        {
            if (Physics.CheckSphere(transform.position + groundedOffset, 0.1f, jumpable))
            {
                if (!isGrounded)
                {
                    // this means that last frame the player wasn't grounded as the value isn't updated yet
                    jumps = maxJumps;
                }
                isGrounded = true;
                lastWallJump = Vector3.positiveInfinity;
                lastWallNormal = Vector3.positiveInfinity;
            }
            else
            {
                // reserve one jump for being on the ground. Not efficient but works.
                isGrounded = false;
                if (jumps > maxJumps - 1 && !grappling && maxJumps > 1)
                {
                    jumps -= 1;
                }
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

        public void Death()
        {
            HUD.SetActive(false);
            respawning.SetActive(true);
            canGrapple = false;
            Time.timeScale = 0.1f;
            Timer deathTimer = new Timer("deathTimer", 1f);
            deathTimer.callbacks += () => { 
                transform.position = checkpoint.position;
                transform.rotation = checkpoint.rotation;
                HUD.SetActive(true);
                respawning.SetActive(false);
                foreach(GrappleController grapple in grapples)
                {
                    grapple.UnGrapple();
                }
                canGrapple = true;
                Time.timeScale = 1f;
            };
            StartCoroutine(deathTimer.Start());
        }

        private void OnValidate()
        {
            canGrapple = _canGrapple;
            canShoot = _canShoot;
            canJump = _canJump;
            canMove = _canMove;
            canSprint = _canSprint;
        }

        public void SetMaxJumps(int maxJumps)
        {
            this.maxJumps = maxJumps;
            if (maxJumps < jumps) jumps = maxJumps;
        }

        public void OnSkipDialogue(InputValue input)
        {
            DialogueManager.Instance.Skip();
        }
    }
}
