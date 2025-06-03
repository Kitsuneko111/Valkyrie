using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Utilities;
using UnityEngine.UI;
using Project.UI;

namespace Project.Grapple
{

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BarFiller))]
    public class GrappleController : MonoBehaviour
    {
        // auto-detected components
        LineRenderer lineRenderer;
        [SerializeField]
        Character.CharacterController player;
        BarFiller barFiller;

        // dev/config controlled values
        public float maxGrapple = 35f;
        float minGrapple = 2.5f;
        float lenience = 2.5f;
        float acceleration = 0.075f;
        [SerializeField]
        float pullSpeed = 0.25f;
        [SerializeField]
        Transform offset;
        [SerializeField]
        public Image marker;
        [SerializeField]
        float markerSize = 0.5f;

        [SerializeField]
        bool leftGrapple = true;
        bool lastLeftGrapple = true;
        [SerializeField]
        bool rightGrapple = false;
        bool lastRightGrapple = false;

        // internally tracked values
        [SerializeField]
        public Stack<GrapplePoint> connections = new();
        [SerializeField]
        float currentMaxGrapple;
        [SerializeField]
        float currentGrappleLength;
        bool pulling = false;
        float pullStart;
        Vector3 lastHitPoint;
        Collider lastHitCollider;
        bool grappleLock;

        // what can be grappled
        [SerializeField]
        LayerMask layerMask;

        // visualisation only
        [SerializeField]
        Mesh conemesh;

        // dynamic to allow for easier readability
        public bool grappling
        {
            get { return connections.Count > 0; }
        }

        public void AddLayer(int layer){
            layerMask += 1 << layer;
        }

        private void Start()
        {
            currentMaxGrapple = maxGrapple;
            lineRenderer = GetComponent<LineRenderer>();
            player = GetComponentInParent<Character.CharacterController>();
            marker = leftGrapple ? GameObject.FindWithTag("LeftMarker").GetComponent<Image>() : GameObject.FindWithTag("RightMarker").GetComponent<Image>();
            barFiller = GetComponent<BarFiller>();
        }

        void FireGrapple()
        {
            if (!grappling)
            {
                if (grappleLock) return;
                grappleLock = true;
                // if not grappling try and grapple
                currentMaxGrapple = maxGrapple;
                // use the raycast from the camera to make it slightly more up to date to what the player sees
                if (lastHitPoint != Vector3.positiveInfinity)
                {
                    if (Vector3.Distance(lastHitPoint, transform.position) < maxGrapple)
                    {
                        AddConnection(lastHitPoint, 0f, lastHitCollider);
                    }
                }
            }
            else
            {
                UnGrapple();
            }
        }

        public void UnGrapple()
        {
            // remove the grapple points
            while (connections.Count > 0)
            {
                Destroy(connections.Pop().gameObject);
                currentGrappleLength = 0;
            }
        }

        // add a connection to the stack and set its values and behaviour
        private void AddConnection(Vector3 hitPoint, float distance, Collider collider)
        {
            GameObject connectionObject = new GameObject("GrappleConnection");
            GrapplePoint connection = connectionObject.AddComponent<GrapplePoint>();
            connection.transform.position = hitPoint;
            connectionObject.layer = 2;
            connection.currentDistance = distance;
            connection.collider = collider;
            connections.Push(connection);
        }

        public void OnLeftGrappleHold(InputValue input)
        {
            if (leftGrapple)
            {
                pulling = input.isPressed;
                PullGrapple();
            }
        }

        public void OnRightGrappleHold(InputValue input)
        {
            if (rightGrapple)
            {
                pulling = input.isPressed;
                PullGrapple();
            }
        }

        void PullGrapple()
        {
            // logic for which action to perform, not that this does not mean that 3/4 times it fires a new grapple, but that it may remove the grapple status
            if (pulling) pullStart = Time.realtimeSinceStartup;
            if (!grappling && !pulling)
            {
                FireGrapple();
            }
            if (grappling && pulling) { }
            if (!grappling && pulling)
            {
                FireGrapple();
            }
            if (grappling && !pulling)
            {
                if (Time.realtimeSinceStartup < pullStart + 0.2f && !grappleLock)
                {
                    // if the player released the button under the hold time and hasn't already fired a grapple then fire/release it
                    FireGrapple();
                }
            }
            if (!pulling) grappleLock = false;
        }

        private void FixedUpdate()
        {
            if (grappling)
            {
                RaycastHit hitInfo;
                currentGrappleLength = Vector3.Distance(connections.Peek().transform.position, transform.position) + connections.Peek().currentDistance;
                barFiller.progress = (currentMaxGrapple / maxGrapple);

                // if the player significantly exceeds the length then stop any momentum in any other direction
                if (currentGrappleLength > currentMaxGrapple + lenience) player.rb.AddForce((connections.Peek().transform.position - transform.position).normalized * (currentGrappleLength - (currentMaxGrapple + lenience)), ForceMode.VelocityChange);
                // otherwise if they're close to the limit just bounce them in the right direction
                else if (currentGrappleLength > currentMaxGrapple) player.rb.AddForce((connections.Peek().transform.position - transform.position).normalized * acceleration, ForceMode.Force);

                // if there is something in the way of the grapple and it's last connection then wrap around the wall
                if (Physics.Raycast(transform.position, (connections.Peek().transform.position - transform.position).normalized, out hitInfo, Vector3.Distance(transform.position, connections.Peek().transform.position) - 0.01f, layerMask))
                {
                    AddConnection(hitInfo.point, connections.Peek().currentDistance + Vector3.Distance(connections.Peek().transform.position, hitInfo.point), hitInfo.collider);
                }

                GrapplePoint temp = connections.Pop();
                if (connections.Count > 0)
                {
                    // setup for a vector between the player and the second-last point from the location of the last point
                    Vector3 tangentVector = (Quaternion.AngleAxis(90, Vector3.up) * (connections.Peek().transform.position - transform.position)).normalized * 3f;
                    Vector3 finalVector = tangentVector * Mathf.Sign(Vector3.Dot(tangentVector.normalized, Vector3.Normalize(transform.position + ((connections.Peek().transform.position - transform.position) / 2) - temp.transform.position)));
                    
                    // if there is nothing between the player and second-last point and also nothing between the last point and half way through the player and second-last point then we can say that the player has either unwrapped the grapple or done something weird
                    if (!Physics.Raycast(transform.position, (connections.Peek().transform.position - transform.position).normalized, out hitInfo, Vector3.Distance(transform.position, connections.Peek().transform.position) - 0.01f, layerMask) && 
                       (!Physics.Raycast(temp.transform.position-finalVector.normalized*0.01f, finalVector, 1f, layerMask)))
                    {
                        Destroy(temp.gameObject);
                        
                    }
                    // otherwise the temporary one is still valid
                    else connections.Push(temp);
                }
                else connections.Push(temp);
                // if a connection somehow got bugged then just remove it (can happen at high speed)
                if (currentMaxGrapple < connections.Peek().currentDistance && currentGrappleLength < 0.1f) Destroy(connections.Pop().gameObject);
            }
            else
            {
                barFiller.progress = 0;
            }

            // reduce the current maximum grapple length
            if (pulling)
            {
                currentMaxGrapple = Mathf.Clamp(currentMaxGrapple - pullSpeed, minGrapple, maxGrapple);
            }


        }

        private void OnValidate()
        {
            // toggle which is which, editor thing
            if (leftGrapple == rightGrapple)
            {
                leftGrapple = rightGrapple == lastRightGrapple ? leftGrapple : !rightGrapple;
                rightGrapple = leftGrapple == lastLeftGrapple ? rightGrapple : !leftGrapple;
                lastLeftGrapple = leftGrapple;
                lastRightGrapple = rightGrapple;
            }
        }


        private void Update()
        {
            // set up the visual for where the grapple is going and if it should be visible
            if (grappling)
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = connections.Count + 1;
                List<GrapplePoint> connectionList = new List<GrapplePoint>(connections);
                lineRenderer.SetPosition(0, transform.position);
                for (int i = 0; i < connectionList.Count; i++)
                {
                    lineRenderer.SetPosition(i + 1, connectionList[i].transform.position);
                }
            }
            else lineRenderer.enabled = false;
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.green;
            //Gizmos.DrawWireMesh(conemesh, offset.position, Quaternion.LookRotation(offset.forward), new Vector3(5f, 5f, maxGrapple + 20f));
            Gizmos.color = Color.cyan;
            //Gizmos.DrawWireSphere(offset.position - Quaternion.FromToRotation(new Vector3(0, 0, 2.5f), offset.forward) * new Vector3(0, 0, 2.5f)*0.5f, 2.5f);
        }

        private void LateUpdate()
        {
            // find all the spots that could be within the cone shape from the offset point
            // TODO reduce offset but introduce a minimum distance?
            List<RaycastHit> raycastHits = new Physics().ConeCastAll(offset.position, 2.5f, offset.forward, maxGrapple + 20f, 5, layerMask);
            // sort them in order of distance
            raycastHits.Sort((a, b) => ((int)Vector3.Distance(transform.position, a.point) * 100) - ((int)Vector3.Distance(transform.position, b.point) * 100));

            // if there were hits then work out where is valid, set up the marks and etc.
            if (raycastHits.Count > 0)
            {

                if (Physics.Raycast(offset.position, raycastHits[0].point - offset.position, out RaycastHit hitInfo, maxGrapple + 20f, layerMask) && Vector3.Distance(hitInfo.point, transform.position) < maxGrapple)
                {
                    lastHitPoint = hitInfo.point;
                    lastHitCollider = hitInfo.collider;
                    if (Vector3.Distance(lastHitPoint, transform.position) < maxGrapple)
                    {
                        // logic to show the marker for where the player is about to try and grapple to
                        marker.enabled = true;
                        marker.transform.position = Vector3.MoveTowards(lastHitPoint, UnityEngine.Camera.main.transform.position, 0.5f);
                        marker.transform.rotation = Quaternion.LookRotation(lastHitPoint - UnityEngine.Camera.main.transform.position);
                        marker.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, markerSize * Vector3.Distance(marker.transform.position, UnityEngine.Camera.main.transform.position));
                        marker.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, markerSize * Vector3.Distance(marker.transform.position, UnityEngine.Camera.main.transform.position));
                    }
                    // otherwise reset the last hit data and markers
                    else
                    {
                        lastHitPoint = Vector3.positiveInfinity;
                        lastHitCollider = null;
                        marker.enabled = false;
                    }
                }
                else
                {
                    lastHitPoint = Vector3.positiveInfinity;
                    lastHitCollider = null;
                    marker.enabled = false;
                }

            }
            else
            {
                // sometimes the conecast ignores the ground so in this case try firing straight ahead as the backup
                if (Physics.Raycast(offset.position, offset.forward, out RaycastHit hitInfo, maxGrapple + 20f, layerMask))
                {
                    if (Vector3.Distance(hitInfo.point, transform.position) < maxGrapple)
                    {
                        lastHitPoint = hitInfo.point;
                        if (Vector3.Distance(lastHitPoint, transform.position) < maxGrapple)
                        {
                            marker.enabled = true;
                            marker.transform.position = Vector3.MoveTowards(lastHitPoint, UnityEngine.Camera.main.transform.position, 0.5f);
                            marker.transform.rotation = Quaternion.LookRotation(lastHitPoint - UnityEngine.Camera.main.transform.position);
                            marker.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, markerSize*Vector3.Distance(marker.transform.position, UnityEngine.Camera.main.transform.position));
                            marker.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, markerSize * Vector3.Distance(marker.transform.position, UnityEngine.Camera.main.transform.position));

                        }

                        else
                        {
                            lastHitPoint = Vector3.positiveInfinity;
                            lastHitCollider = null;
                            marker.enabled = false;
                        }
                    }
                }
                else
                {
                    lastHitPoint = Vector3.positiveInfinity;
                    lastHitCollider = null;
                    marker.enabled = false;
                }
            }
        }
        private void OnEnable()
        {
            marker = leftGrapple ? GameObject.FindWithTag("LeftMarker").GetComponent<Image>() : GameObject.FindWithTag("RightMarker").GetComponent<Image>();

            marker.enabled = true;
        }
        private void OnDisable()
        {
            marker = leftGrapple ? GameObject.FindWithTag("LeftMarker").GetComponent<Image>() : GameObject.FindWithTag("RightMarker").GetComponent<Image>();

            marker.enabled = false;
        }
    }
}
