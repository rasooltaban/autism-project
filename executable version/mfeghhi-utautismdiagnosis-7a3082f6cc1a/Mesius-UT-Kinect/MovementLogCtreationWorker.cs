using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// movement log creation worker class
    /// </summary>
    class MovementLogCtreationWorker
    {
        public delegate void OnWorkerMethodCompleteDelegate(string message);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;
        private List<MySkeletonFrame> skeletonframes;
        private string path;

        public MovementLogCtreationWorker(List<MySkeletonFrame> sframes, string path)
        {
            skeletonframes = sframes;
            this.path = path;
        }

        /// <summary>   
        /// main worker method
        /// </summary>
        public void WorkerMethod()
        {
            LogService.record_logs(skeletonframes, path);
            OnWorkerComplete("");
        }
    }
}
