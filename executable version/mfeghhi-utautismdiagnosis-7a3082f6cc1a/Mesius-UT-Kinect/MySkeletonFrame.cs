using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;
using System.Drawing;

namespace UTKinectSkeletonMovementDetector
{

    /// <summary>   
    /// skeleton frame holder
    /// </summary>
    public class MySkeletonFrame
    {
        private long timeTag;
        private MySkeleton skeleton;


        public MySkeletonFrame(Skeleton skeleton, long timeTag)
        {
            this.skeleton = new MySkeleton(skeleton);
            this.timeTag = timeTag;
        }

        public MySkeletonFrame(MySkeleton skeleton, long timeTag)
        {
            this.skeleton = skeleton;
            this.timeTag = timeTag;
        }
        public MySkeletonFrame(long timeTag)
        {
            this.skeleton = null;
            this.timeTag = timeTag;
        }
        public void saveToFile(String path) {
            StreamWriter tw = new StreamWriter(path, true);
            if (skeleton == null)
            {
                tw.WriteLine(timeTag + " Injected Empty Skeleton");
                tw.WriteLine();
            }
            else
            {
                tw.WriteLine(timeTag + " " + skeleton.getTrackingId());
                tw.Write(skeleton.toString());
                tw.WriteLine();
            }
            
            tw.Close();
        }

        public long getTime()
        {
            return timeTag;
        }

        public MemoryStream toImageMemoryStream()
        {
            Image img = SkeletonPainter.paintonblank(skeleton);
            MemoryStream m = new MemoryStream();
            img.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
            return m;
        }

        public MySkeleton getSkeletons() { return skeleton; }

        public List<MySkeletonFrame> interpolate(MySkeletonFrame next)
        {
            List<MySkeletonFrame> results = new List<MySkeletonFrame>();
            long gap = next.getTime() - getTime();
            long numOfSpans = gap / (long)33;
            if (numOfSpans == 0)
                return results;
            long interval = gap / numOfSpans;
            for (int i = 1; i < numOfSpans; i++)
            {
                MySkeleton interSkeleton = new MySkeleton(this.skeleton, next.skeleton, next.getTime() - this.getTime(), (long)i * interval);
                MySkeletonFrame interpolation = new MySkeletonFrame(interSkeleton, this.timeTag + (long)i * interval);
                results.Add(interpolation);
            }
            return results;
        }
    }
    /// <summary>   
    /// skeleton data holder
    /// </summary>
    public class MySkeleton
    {
        private int trackingId;
        private Hashtable joints = new Hashtable();
        public MySkeleton(int trackingId)
        {
            this.trackingId = trackingId;
        }
        public MySkeleton(Skeleton skeleton)
        {
            this.trackingId = skeleton.TrackingId;
            for (int i = 0; i < skeleton.Joints.Count; i++)
            {
                Joint j = skeleton.Joints.ElementAt(i);
                joints.Add(j.JointType.ToString(), new JointPosition(j.Position));
            }
        }

        public MySkeleton(MySkeleton a, MySkeleton b, long ABtimeDif, long AThisTimeDif) {
            foreach (DictionaryEntry pair in a.getJoints())
            {
                String jointName = (String)pair.Key;
                JointPosition apos = a.getJoint(jointName);
                JointPosition bpos = b.getJoint(jointName);
                float cx = ((float)AThisTimeDif / (float)ABtimeDif) * (bpos.x - apos.x) + apos.x;
                float cy = ((float)AThisTimeDif / (float)ABtimeDif) * (bpos.y - apos.y) + apos.y;
                float cz = ((float)AThisTimeDif / (float)ABtimeDif) * (bpos.z - apos.z) + apos.z;
                joints.Add(jointName, new JointPosition(cx, cy, cz));
            }
        }

        public int getTrackingId() { return this.trackingId; }

        public String toString()
        {
            String result = "";
            foreach (DictionaryEntry pair in joints)
            {
                JointPosition pos = (JointPosition)pair.Value;
                result += pair.Key + " " + pos.x + " " + pos.y + " " + pos.z + "\n";
            }
            return result;
        }

        public JointPosition getJoint(String name)
        {
            return (JointPosition)joints[name];
        }

        public void addJoint(String name, float x, float y, float z)
        {
            joints.Add(name, new JointPosition(x, y, z));
        }

        
        public Hashtable getJoints()
        {
            return joints;
        }
    }

    /// <summary>   
    /// skeleton data holder
    /// </summary>
    public class JointPosition
    {
        public float x;
        public float y;
        public float z;

        public JointPosition(SkeletonPoint pos)
        {
            this.x = pos.X;
            this.y = pos.Y;
            this.z = pos.Z;
        }

        public JointPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
