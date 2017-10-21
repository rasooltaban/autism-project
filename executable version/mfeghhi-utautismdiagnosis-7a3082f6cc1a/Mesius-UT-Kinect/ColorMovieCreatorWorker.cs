using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// color movie creation worker class
    /// </summary>
    class ColorMovieCreatorWorker
    {
        public delegate void OnWorkerMethodCompleteDelegate(string message);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;
        private List<MyColorFrame> colorframes;
        private List<MySkeletonFrame> skeletonframes;
        private string path;

        /// <summary>   
        /// cunstructor
        /// </summary>
        public ColorMovieCreatorWorker(List<MyColorFrame> cframes,List<MySkeletonFrame> sframes, string path)
        {
            colorframes = cframes;
            skeletonframes = sframes;
            this.path = path;
        }

        /// <summary>   
        /// main method of the worker
        /// </summary>
        public void WorkerMethod()
        {
            MovieCreator.createHeadHidedMovie(colorframes, skeletonframes, path);
            OnWorkerComplete("");
        }
    }
}
