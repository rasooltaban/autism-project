using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;

namespace UTKinectSkeletonMovementDetector
{
    public class TextTag
    {
        public String id { set; get; }
        public String text { set; get; }
        public double start { set; get; }
        public double end { set; get; }
        public int duration { set; get; }
        public int totalDuration { set; get; }
        public int color { set; get; }
        public Boolean visible { set; get; }
        public String path { get; set; }

        public TextTag(double start, int totalDuration, String casePath)
        {
            id = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
            if (!Directory.Exists(casePath + "/tags"))
                Directory.CreateDirectory(casePath + "/tags");
            this.path = casePath + "\\tags\\" + id + ".txt";
            this.text = "";
            this.start = start;
            this.duration = 60;
            this.end = start + (double)duration / (double)totalDuration;
            this.totalDuration = totalDuration;
            this.color = 0;
            this.visible = true;
        }

        public TextTag(String path)
        {
            StreamReader sr = new StreamReader(path);
            this.id = sr.ReadLine();
            this.path = sr.ReadLine();
            this.text = sr.ReadLine();
            this.start = double.Parse(sr.ReadLine());
            this.end = double.Parse(sr.ReadLine());
            this.duration = int.Parse(sr.ReadLine());
            this.totalDuration = int.Parse(sr.ReadLine());
            this.color = int.Parse(sr.ReadLine());
            this.visible = false;
        }
        public void saveOrUpdateOnDisk()
        {
            if (File.Exists(path))
                File.Delete(path);
            StreamWriter tw = new StreamWriter(path, false);
            tw.WriteLine(id);
            tw.WriteLine(path);
            tw.WriteLine(text);
            tw.WriteLine(start);
            tw.WriteLine(end);
            tw.WriteLine(duration);
            tw.WriteLine(totalDuration);
            tw.WriteLine(color);
            tw.Close();
        }
    }
}
