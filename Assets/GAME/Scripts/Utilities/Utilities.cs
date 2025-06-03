using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities
{
    public static class ExtensionMethods
    {
        public static Vector3 Sum(this List<Vector3> vectors)
        {
            Vector3 returnVal = new Vector3();
            foreach (Vector3 vector in vectors)
            {
                returnVal += vector;
            }
            return returnVal;
        }

        public static Vector3 Average(this List<Vector3> vectors)
        {
            return Sum(vectors) / vectors.Count;
        }

        public static Vector3 AveragePosition<T>(this List<T> behaviours) where T : MonoBehaviour
        {
            Vector3 returnVal = new Vector3();
            foreach (T behaviour in behaviours)
            {
                returnVal += behaviour.transform.position;
            }
            return returnVal / behaviours.Count;
        }

        public static float FlatDistance(this Vector3 vector, Vector3 vectorA, Vector3 vectorB)
        {
            return Mathf.Sqrt(Mathf.Pow(vectorA.x - vectorB.x, 2) + Mathf.Pow(vectorA.z- vectorB.z, 2));
        }

        public static float FlatDistance(this Vector3 vector, Vector3 vectorA)
        {
            return Mathf.Sqrt(Mathf.Pow(vectorA.x - vector.x, 2) + Mathf.Pow(vectorA.z - vector.z, 2));
        }

        public static Vector3 Flat(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }


        //https://github.com/walterellisfun/ConeCast/blob/master/ConeCastExtension.cs
        public static List<RaycastHit> ConeCastAll(this Physics physics, Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle, LayerMask layermask)
        {
            RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - Quaternion.FromToRotation(new Vector3(0, 0, maxRadius), direction) * new Vector3(0, 0, maxRadius)*0.5f, maxRadius, direction, maxDistance, layermask);
            List<RaycastHit> coneCastHitList = new List<RaycastHit>();

            if (sphereCastHits.Length > 0)
            {
                for (int i = 0; i < sphereCastHits.Length; i++)
                {
                    Vector3 hitPoint = sphereCastHits[i].point;
                    Vector3 directionToHit = hitPoint - origin;
                    float angleToHit = Vector3.Angle(direction, directionToHit);

                    if (angleToHit < coneAngle)
                    {
                        coneCastHitList.Add(sphereCastHits[i]);
                    }
                }
            }

            return coneCastHitList;
        }
    }
}