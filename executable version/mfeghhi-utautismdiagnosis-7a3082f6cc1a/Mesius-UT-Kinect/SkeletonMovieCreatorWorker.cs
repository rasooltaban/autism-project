using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// skeleton movie creation worker class
    /// </summary>
    class SkeletonMovieCreatorWorker
    {
        public delegate void OnWorkerMethodCompleteDelegate(string message);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;
        private List<MySkeletonFrame> skeletonframes;
        private string path;

        public SkeletonMovieCreatorWorker(List<MySkeletonFrame> sframes, string path)
        {
            skeletonframes = sframes;
            this.path = path;
        }

        /// <summary>   
        /// main worker method
        /// </summary>
        public void WorkerMethod()
        {
            MovieCreator.createSkeletonMovie(skeletonframes, path);
            OnWorkerComplete("");
        }
    }
}
