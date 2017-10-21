using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// overlay movie creation worker class
    /// </summary>
    class OverlayMovieCreatorWorker
    {
        public delegate void OnWorkerMethodCompleteDelegate(string message);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;
        private List<MyColorFrame> colorframes;
        private List<MySkeletonFrame> skeletonframes;
        private string path;

        public OverlayMovieCreatorWorker(ref List<MyColorFrame> cframes, ref List<MySkeletonFrame> sframes, string path)
        {
            colorframes = cframes;
            skeletonframes = sframes;
            this.path = path;
        }

        /// <summary>   
        /// main worker method
        /// </summary>
        public void WorkerMethod()
        {
            MovieCreator.createOverlayMovie(ref colorframes, ref skeletonframes, path);
            OnWorkerComplete("");
        }
    }
}
