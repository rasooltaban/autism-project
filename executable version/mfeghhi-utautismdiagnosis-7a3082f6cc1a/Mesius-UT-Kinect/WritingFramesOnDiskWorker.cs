using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// writing frames on disk worker class
    /// </summary>
    class WritingFramesOnDiskWorker
    {
        public delegate void OnWorkerMethodCompleteDelegate(string message);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;
        private List<MySkeletonFrame> skeletonframes;
        private List<MyColorFrame> colorframes;
        private string path;

        public WritingFramesOnDiskWorker(List<MyColorFrame> cframes, List<MySkeletonFrame> sframes, string path)
        {
            skeletonframes = sframes;
            colorframes = cframes;
            this.path = path;
        }


        /// <summary>   
        /// main worker method
        /// </summary>
        public void WorkerMethod()
        {

            if (!Directory.Exists(path + "/raw"))
                Directory.CreateDirectory(path + "/raw");
            else
            {
                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path + "/raw");
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
                directory.Delete();
                Directory.CreateDirectory(path + "/raw");
            }

            // write colorframes on disk
            for (int i = 0; i < colorframes.Count; i++)
            {
                colorframes[i].writeOnDisk(path + "/raw/rgb");
            }

            // write skeletonframes on disk
            for (int i = 0; i < skeletonframes.Count; i++)
            {
                skeletonframes[i].saveToFile(path + "/raw/skeleton.txt");
            }
            //OnWorkerComplete("");
        }
    }
}
