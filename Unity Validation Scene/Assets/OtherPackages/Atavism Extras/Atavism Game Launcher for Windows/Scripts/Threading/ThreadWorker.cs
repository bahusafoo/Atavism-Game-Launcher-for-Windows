using System.Threading;

namespace GameLauncher.Threading
{
    internal class ThreadWorker
    {
        public bool IsRunning { get; private set; }
        public bool IsCompleted { get; private set; }

        public delegate void OnThreadWorkerRun();

        private Thread handlerThread;
        private OnThreadWorkerRun handler;

        public ThreadWorker(OnThreadWorkerRun handler)
        {
            this.handler = handler;
        }

        public void Run()
        {
            if (IsRunning) return;

            IsRunning = true;
            
            handler?.Invoke();
            IsCompleted = true;
            IsRunning = false;
        }
    }
}
