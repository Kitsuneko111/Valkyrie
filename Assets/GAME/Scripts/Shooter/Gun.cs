using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities;
using UnityEngine.InputSystem;
using System;
using Project.Camera;
using Project.Shooter.Targets;
using Project.UI;
using UnityEngine.UI;

namespace Project.Shooter
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BarFiller))]
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        bool leftGun = true;
        bool lastLeftGun = true;
        [SerializeField]
        bool rightGun = false;
        bool lastRightGun = false;
        public bool firing;
        [SerializeField]
        int maxAmmo = 30;
        int ammo;
        [SerializeField]
        float maxMags = Mathf.Infinity;
        float mags;
        public float variationAngle = 1f;
        new CameraController camera;
        BarFiller barFiller;
        [SerializeField]
        Image reloadIcon;

        public LayerMask layerMask;

        float fireRate = 20f;
        float reloadTime = 2f;
        float animationTime = 1 / 15f;
        Timer fireTimer;
        Timer reloadTimer;
        Timer pauseTimer;

        [SerializeField]
        float damage = 5f;

        LineRenderer lineRenderer;

        private void Awake()
        {
            // Define timers and callbacks
            fireTimer = new Timer("fireTimer", animationTime, false);
            pauseTimer = new Timer("pauseTimer", 1 / fireRate - animationTime, false);
            reloadTimer = new Timer("reloadTimer", reloadTime, false);
            ammo = maxAmmo;
            fireTimer.callbacks += () => StartCoroutine(pauseTimer.Start());
            pauseTimer.callbacks += () => { };
            reloadTimer.callbacks += Reload;
            
            mags = maxMags;
        }

        private void Start()
        {
            // Dynamically check for various required components
            camera = FindObjectOfType<CameraController>();
            lineRenderer = GetComponent<LineRenderer>();
            fireTimer.callbacks += () => lineRenderer.enabled = false;
            barFiller = GetComponent<BarFiller>();
            reloadTimer.callbacks += () =>
            {
                reloadIcon.enabled = false;
            };
        }

        private void OnValidate()
        {
            if (leftGun == rightGun)
            {
                leftGun = rightGun == lastRightGun ? leftGun : !rightGun;
                rightGun = leftGun == lastLeftGun ? rightGun : !leftGun;
                lastLeftGun = leftGun;
                lastRightGun = rightGun;
            }
            if (maxMags < mags) mags = maxMags;
            if (mags == 0) mags = maxMags;
        }

        public void OnLeftFire(InputValue input)
        {
            Debug.Log("Fire");
            if (leftGun)
            {
                firing = input.isPressed;
            }
        }

        public void OnRightFire(InputValue input)
        {
            if (rightGun)
            {
                firing = input.isPressed;
            }
        }

        public void OnReload(InputValue input)
        {
            StartReload();
        }

        private void Update()
        {
            // check the ammo count and whether a reload is required
            if (firing && ammo > 0)
            {
                if (!fireTimer.running && !pauseTimer.running && !reloadTimer.running)
                {
                    Fire();
                    StartCoroutine(fireTimer.Start());
                }
            }
            if (ammo <= 0 && mags > 0)
            {
                StartReload();
                
            }
            lineRenderer.SetPosition(0, transform.position);
            if (reloadTimer.running)
            {
                if (reloadIcon)
                {
                    reloadIcon.fillAmount = reloadTimer.remaining / reloadTimer.length;
                }
            }
        }

        private void StartReload()
        {
            if (!reloadTimer.running)
            {
                ammo = 0;
                Debug.Log("Reloading");
                pauseTimer.Cancel();
                StartCoroutine(reloadTimer.Start());
                reloadIcon.enabled = true;
            }
        }

        public void Fire()
        {
            // Get initial firing vector
            RaycastHit hit;
            Debug.Log($"camera xangle {camera.xAngle}");
            Vector3 firingVector = camera.transform.position + (camera.transform.forward * 50f) - transform.position;
            Debug.DrawRay(camera.transform.position, camera.transform.forward * 50f, Color.white);
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 50f, layerMask))
                // check to see if this vector is aimed at anything
            {
                // check the angle of the vector and character to see if this collision should be used
                if(Vector3.Angle(firingVector, hit.point-camera.transform.position) > variationAngle * 2)
                {
                    firingVector = transform.forward;
                }
                else firingVector = hit.point - transform.position;
                
            }
            // apply a random angle to the vector
            float angle = UnityEngine.Random.Range(0f, 360f);
            Debug.Log($"360 angle: {angle}");
            float outwardAngle = UnityEngine.Random.Range(0f, variationAngle);
            Debug.Log($"outward angle: {outwardAngle}");
            Quaternion rotationalVariation = Quaternion.AngleAxis(angle, firingVector);
            firingVector = Quaternion.AngleAxis(outwardAngle, Vector3.up) * firingVector;
            firingVector =  rotationalVariation * firingVector;

            //re-check the new vector to get the finalised hit location
            RaycastHit laserHit;
            if (Physics.Raycast(transform.position, firingVector, out laserHit, 50f, layerMask))
            {
                lineRenderer.SetPosition(1, laserHit.point);
                if (laserHit.collider.gameObject.GetComponent<Target>() != null) laserHit.collider.gameObject.GetComponent<Target>().Hit(damage);
                else if (laserHit.collider.gameObject.GetComponentInChildren<Target>() != null) laserHit.collider.gameObject.GetComponentInChildren<Target>().Hit(damage);
            }
            else lineRenderer.SetPosition(1, transform.position+firingVector.normalized * 75f);
            lineRenderer.enabled = true;
            Debug.Log($"Firing {ammo}/{maxAmmo}");
            ammo--;
            barFiller.progress = (float)ammo / maxAmmo;
        }

        void Reload()
        {
            if (mags <= 0) return;
            Debug.Log("Reloaded");
            ammo = maxAmmo;
            mags--;
            barFiller.progress = (float)ammo / maxAmmo;
        }
    }
}
