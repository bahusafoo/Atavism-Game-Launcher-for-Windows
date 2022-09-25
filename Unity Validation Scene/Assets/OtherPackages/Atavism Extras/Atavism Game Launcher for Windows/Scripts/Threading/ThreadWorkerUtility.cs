using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLauncher.Threading
{
    internal static class ThreadWorkerUtility
    {
        public static CustomYieldInstruction WaitForCompletion(this ThreadWorker worker)
        {
            return new WaitForThreadWorker(worker);
        }
    }
}
