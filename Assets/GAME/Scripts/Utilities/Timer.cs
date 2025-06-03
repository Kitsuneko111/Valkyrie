using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Utilities
{

    public class Timer
    {
        static List<Timer> Instances;
        public string id;
        public float length;
        double startTime;
        public bool running;
        bool completable;
        bool deleteAfter;

        public float remaining
        {
            get
            {
                return length - (float)(Time.realtimeSinceStartup - startTime);
            }
        }

        public Timer(string id, float length)
        {
            this.id = id;
            this.length = length;
            deleteAfter = true;
        }

        public Timer(string id, float length, bool destroy)
        {
            this.id = id;
            this.length = length;
            this.deleteAfter = destroy;
        }

        public delegate void Callback();
        public Callback callbacks = () => { };

        public IEnumerator Start()
        {
            startTime = Time.realtimeSinceStartup;
            running = true;
            completable = true;
            while (running && Time.realtimeSinceStartup - startTime < length)
            {
                yield return 0;
            }
            running = false;
            if(completable && callbacks.GetInvocationList().Length > 0) callbacks();
            if (deleteAfter && Instances.Contains(this))
            {
                Instances.Remove(this);
            }
        }

        public void Cancel()
        {
            running = false;
            completable=false;
        }

        public void EndEarly()
        {
            running = false;
        }

        public static List<Timer> GetTimer(string id)
        {
            return Instances.FindAll(x => x.id == id);
        }

        public static void CancelAll()
        {
            foreach(Timer timer in Instances)
            {
                timer.Cancel();
            }
        }
    }

}
