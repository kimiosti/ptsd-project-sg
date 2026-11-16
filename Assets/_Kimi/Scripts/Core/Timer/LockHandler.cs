using System;
using System.IO;
using UnityEngine;

namespace _Kimi.Scripts.Core.Timer
{
    public class LockHandler: MonoBehaviour
    {
        private const string FileReferencePath = "Assets/Resources/Timer/lock_time"; // TODO - modify to point at right path
        private static readonly TimeSpan LockDuration = new TimeSpan(0, 3, 0); // TODO - modify to fix right time suspension amount

        public void SetLockTime()
        {
            var lockTime = DateTime.Now;
            File.WriteAllText(FileReferencePath, lockTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public bool CheckGameLocked()
        {
            if (!File.Exists(FileReferencePath)) return false;
            var now = DateTime.Now;
            var timeSinceLock = now.Subtract(DateTime.Parse(File.ReadAllText(FileReferencePath)));
            return timeSinceLock < LockDuration;

        }
    }
}