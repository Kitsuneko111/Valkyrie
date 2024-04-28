using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Utilities;
using UnityEngine.UI;

namespace Project.Grapple
{

    [RequireComponent(typeof(LineRenderer))]
    public class GrappleController : MonoBehaviour
    {
        [SerializeField]
        Mesh conemesh;

        [SerializeField]
        Character.CharacterController player;
        [SerializeField]
        public Stack<GrapplePoint> connections = new();
        float maxGrapple = 30f;
        [SerializeField]
        float currentMaxGrapple;
        float minGrapple = 2.5f;
        float lenience = 2.5f;
        float acceleration = 1f;
        [SerializeField]
        LayerMask layerMask;
        [SerializeField]
        bool leftGrapple = true;
        bool lastLeftGrapple = true;
        [SerializeField]
        bool rightGrapple = false;
        bool lastRightGrapple = false;
        [SerializeField]
        float currentGrappleLength;
        bool pulling = false;
        object padlock = new();
        [SerializeField]
        float pullSpeed = 0.25f;
        float pullStart;
        LineRenderer lineRenderer;
        Vector3 lastHitPoint;
        [SerializeField]
        Image marker;
        [SerializeField]
        Transform offset;

        public bool grappling
        {
            get { return connections.Count > 0; }
        }
        bool grappleLock = false;

        private void Start()
        {
            currentMaxGrapple = maxGrapple;
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void OnLeftGrappleTap(InputValue input)
        {
            //if (leftGrapple) FireGrapple();
        }

        public void OnRightGrappleTap(InputValue input)
        {
            //if (rightGrapple) FireGrapple();
        }

        void FireGrapple()
        {
            Debug.Log("FIRE");
            lock (padlock)
            {
                if (grappleLock) return;
                Debug.Log("Locking");
                grappleLock = true;
                if (!grappling)
                {
                    currentMaxGrapple = maxGrapple;
                    Debug.Log("Grappling");
                    if (lastHitPoint != Vector3.positiveInfinity)
                    {
                        if (Vector3.Distance(lastHitPoint, transform.position) < maxGrapple)
                        {
                            AddConnection(lastHitPoint, 0f);
                            Debug.Log("Grappled");
                        }
                    }
                }
                else
                {
                    Debug.Log("Removing Grapple");
                    while (connections.Count > 0)
                    {
                        Destroy(connections.Pop().gameObject);
                        currentGrappleLength = 0;
                    }
                }
            }
        }

        private void AddConnection(Vector3 hitPoint, float distance)
        {
            Debug.Log("ADDING");
            GameObject connectionObject = new GameObject("GrappleConnection");
            GrapplePoint connection = connectionObject.AddComponent<GrapplePoint>();
            connection.transform.position = hitPoint;
            connectionObject.layer = 2;
            connection.currentDistance = distance;
            connections.Push(connection);
            Debug.Log("Added");
        }

        public void OnLeftGrappleHold(InputValue input)
        {
            Debug.Log("Hold");
            pulling = input.isPressed;
            if (leftGrapple) PullGrapple();
        }

        public void OnRightGrappleHold(InputValue input)
        {
            pulling = input.isPressed;
            if (rightGrapple) PullGrapple();
        }

        void PullGrapple()
        {
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
                if (currentGrappleLength > currentMaxGrapple) player.rb.AddForce((connections.Peek().transform.position - transform.position).normalized * acceleration, ForceMode.Force);
                if (currentGrappleLength > currentMaxGrapple + lenience) player.rb.AddForce((connections.Peek().transform.position - transform.position).normalized * (currentGrappleLength - (currentMaxGrapple + lenience)), ForceMode.VelocityChange);
                Debug.DrawLine(transform.position, connections.Peek().transform.position, Color.red);
                if (Physics.Raycast(transform.position, (connections.Peek().transform.position - transform.position).normalized, out hitInfo, Vector3.Distance(transform.position, connections.Peek().transform.position) - 0.01f, layerMask))
                {
                    Debug.Log($"Found wall in between, {hitInfo.collider.gameObject.name} at {hitInfo.point}");
                    AddConnection(hitInfo.point, connections.Peek().currentDistance + Vector3.Distance(connections.Peek().transform.position, hitInfo.point));
                }
                GrapplePoint temp = connections.Pop();
                //Debug.Log("Popped");
                if (connections.Count > 0)
                {
                    //Debug.Log("Pop check");
                    if (!Physics.Raycast(transform.position, (connections.Peek().transform.position - transform.position).normalized, out hitInfo, Vector3.Distance(transform.position, connections.Peek().transform.position) - 0.01f, layerMask) && Vector3.Angle((connections.Peek().transform.position-player.transform.position),(hitInfo.point-player.transform.position)) < 2f)
                    {
                        Destroy(temp.gameObject);
                        
                    }
                    else connections.Push(temp);
                }
                else connections.Push(temp);
                if (currentMaxGrapple < connections.Peek().currentDistance && currentGrappleLength < 0.1f) Destroy(connections.Pop().gameObject);
            }
            if (pulling)
            {
                currentMaxGrapple = Mathf.Clamp(currentMaxGrapple - pullSpeed, minGrapple, maxGrapple);
            }


        }

        private void OnValidate()
        {
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
            Gizmos.DrawWireMesh(conemesh, offset.position, Quaternion.LookRotation(offset.forward), new Vector3(5f, 5f, maxGrapple + 20f));
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(offset.position - Quaternion.FromToRotation(new Vector3(0, 0, 2.5f), offset.forward) * new Vector3(0, 0, 2.5f)*0.5f, 2.5f);
        }

        private void LateUpdate()
        {
            List<RaycastHit> raycastHits = new Physics().ConeCastAll(offset.position, 2.5f, offset.forward, maxGrapple + 20f, 5, layerMask);
            raycastHits.Sort((a, b) => ((int)Vector3.Distance(transform.position, a.point) * 100) - ((int)Vector3.Distance(transform.position, b.point) * 100));
            if (raycastHits.Count > 0)
            {
                Debug.DrawRay(offset.position, raycastHits[0].point-offset.position, Color.yellow);
                

                if (Physics.Raycast(offset.position, raycastHits[0].point - offset.position, out RaycastHit hitInfo, maxGrapple + 20f, layerMask) && Vector3.Distance(hitInfo.point, transform.position) < maxGrapple)
                {
                    lastHitPoint = hitInfo.point;
                    if (Vector3.Distance(lastHitPoint, transform.position) < maxGrapple)
                    {
                        marker.enabled = true;
                        marker.transform.position = Vector3.MoveTowards(lastHitPoint, UnityEngine.Camera.main.transform.position, 0.5f);
                        marker.transform.rotation = Quaternion.LookRotation(lastHitPoint - UnityEngine.Camera.main.transform.position);
                    }
                    else
                    {
                        lastHitPoint = Vector3.positiveInfinity;
                        marker.enabled = false;
                    }
                }
                else
                {
                    lastHitPoint = Vector3.positiveInfinity;
                    marker.enabled = false;
                }

            }
            else
            {
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
                        }
                        else
                        {
                            lastHitPoint = Vector3.positiveInfinity;
                            marker.enabled = false;
                        }
                    }
                }
                else
                {
                    lastHitPoint = Vector3.positiveInfinity;
                    marker.enabled = false;
                }
            }
        }
    }
}
