using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UTKinectSkeletonMovementDetector
{
    class LogService
    {
        public static void record_logs(List<MySkeletonFrame> skeletonframes, String path)
        {
            Trace.WriteLine(skeletonframes.Count);
            foreach (MySkeletonFrame skelframe in skeletonframes) {
                skelframe.saveToFile(path);
            }
        }
    }
}
