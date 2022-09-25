using UnityEngine;

namespace GameLauncher.Threading
{
    internal class WaitForThreadWorker : CustomYieldInstruction
    {
        public override bool keepWaiting => !worker.IsCompleted;

        private ThreadWorker worker;

        public WaitForThreadWorker(ThreadWorker worker)
        {                
            this.worker = worker;
        }
    }
}
