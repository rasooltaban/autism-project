using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AForge.Video;
using AForge.Video.FFMPEG;
using System.Diagnostics;

namespace UTKinectSkeletonMovementDetector
{
    class MovieCreator
    {

        private static void createMovie(List<System.Drawing.Bitmap> images, String path)
        {

            int width = 640;
            int height = 480;
            var framRate = 30;

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4, 1000000);
                var imageEntities = images;

                //loop throught all images in the collection
                foreach (var imageEntity in imageEntities)
                {
                    //what's the current image data?
                    //var bmpReduced = ReduceBitmap(imageEntity, width, height);
                    vFWriter.WriteVideoFrame(imageEntity);
                }
                vFWriter.Close();
            }
        }


        public static void createSkeletonMovie(List<MySkeletonFrame> recorded_skeleton_frames, String path)
        {

            int width = 640;
            int height = 480;
            var framRate = 30;

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4, 1000000);

                //loop throught all images in the collection
                foreach (MySkeletonFrame sf in recorded_skeleton_frames)
                {
                    MemoryStream frame_memstream = sf.toImageMemoryStream();
                    System.Drawing.Bitmap imageEntity = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(frame_memstream);
                    vFWriter.WriteVideoFrame(imageEntity);
                }
                vFWriter.Close();
            }


      }

        public static void createHeadHidedMovie(List<MyColorFrame> colorframes, List<MySkeletonFrame> skeletonframes, string path)
        {
            int width = 640;
            int height = 480;
            var framRate = 30;

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4, 2000000);

                for (int i = 0; i < colorframes.Count; i++)
                {
                    if (skeletonframes.Count > i)
                    {
                        MyOverlayFrame ovframe = new MyOverlayFrame(skeletonframes.ElementAt(i), colorframes.ElementAt(i));
                        MemoryStream frame_memstream = ovframe.getHeadHidedMemoryStream();
                        System.Drawing.Bitmap imageEntity = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(frame_memstream);
                        vFWriter.WriteVideoFrame(imageEntity);
                    }
                }
                vFWriter.Close();
            }
        }
        public static void createColorMovie(List<MyColorFrame> recorded_color_frames, String path)
        {
            int width = 640;
            int height = 480;
            var framRate = 30;

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4, 1000000);

                //loop throught all images in the collection
                foreach (MyColorFrame cf in recorded_color_frames)
                {
                    MemoryStream frame_memstream = cf.getImageMemoryStream();
                    System.Drawing.Bitmap imageEntity = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(frame_memstream);
                    vFWriter.WriteVideoFrame(imageEntity);
                }
                vFWriter.Close();
            }
        }

        public static void createOverlayMovie(ref List<MyColorFrame> colorframes, ref List<MySkeletonFrame> skeletonframes, string path)
        {
            int width = 640;
            int height = 480;
            var framRate = 30;

            // create instance of video writer
            using (var vFWriter = new VideoFileWriter())
            {
                // create new video file
                vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4, 2000000);
                for (int i = 0; i < colorframes.Count; i++)
                {
                    MyOverlayFrame ovframe = new MyOverlayFrame(skeletonframes[i], colorframes[i]);
                    MemoryStream frame_memstream = ovframe.getOverlayMemoryStream();
                    System.Drawing.Bitmap imageEntity = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(frame_memstream);
                    vFWriter.WriteVideoFrame(imageEntity);   
                }
                vFWriter.Close();
            }
        }


    }
}
