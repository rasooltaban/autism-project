using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.IO;


namespace UTKinectSkeletonMovementDetector
{
    /// <summary>
    /// Interaction logic for CaseMaker.xaml
    /// </summary>
    public partial class CaseMaker : Window
    {
        private List<MyColorFrame> recorded_color_frames = new List<MyColorFrame>();
        private List<MySkeletonFrame> recorded_skeleton_frames = new List<MySkeletonFrame>();
        private int startIndex;
        private int endIndex;
        private int currentIndex;
        private String casePath;
        private MainWindow parent;

        /// <summary>
        /// Progress dialog which pops up when post processing recorded data
        /// </summary>
        private progressDialog progressWindow;

        public CaseMaker(MainWindow parent, List<MyColorFrame> recorded_color_frames, List<MySkeletonFrame> recorded_skeleton_frames, String selectedPath)
        {
            InitializeComponent();
            this.parent = parent;
            this.recorded_color_frames.AddRange(recorded_color_frames);
            this.recorded_skeleton_frames.AddRange(recorded_skeleton_frames);
            completeSkeletons();

            if (recorded_skeleton_frames.Count == 0)
                load_rgb(recorded_color_frames[0]);
            else
                load_overlay(new MyOverlayFrame(recorded_skeleton_frames[0], recorded_color_frames[0]));

            currentIndex = 0;
            startIndex = 0;
            endIndex = recorded_color_frames.Count - 1;
            caseStartLabel.Content = recorded_color_frames[startIndex].getTime().ToString();
            caseEndLabel.Content = recorded_color_frames[endIndex].getTime().ToString();

            int caseNum = Directory.GetDirectories(selectedPath + "\\cases").Count() + 1;
            this.casePath = selectedPath + "\\cases\\" + caseNum;
        }
        private void makeSliderOneValueBiggerClick(object sender, RoutedEventArgs e)
        {
            //this.timeTagLabel.Content = Int32.Parse(this.timeTagLabel.Content.ToString()) + 1;
            //float val = (int)this.timeTagLabel.Content/recorded_color_frames.Count;
            //this.slider.Value = val;
            this.slider.Value++;

        }

        private void makeSliderOneValueLittlerClick(object sender, RoutedEventArgs e)
        {
            this.slider.Value--;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value / 100;
            currentIndex = (int)((double)recorded_color_frames.Count * value);
            if (currentIndex < recorded_color_frames.Count && currentIndex < recorded_skeleton_frames.Count)
            {
                load_overlay(new MyOverlayFrame(recorded_skeleton_frames[currentIndex], recorded_color_frames[currentIndex]));
            }
        }
        
        private void load_overlay(MyOverlayFrame frame) 
        {
            timeTagLabel.Content = frame.getTime().ToString();
            System.IO.MemoryStream ms = frame.getOverlayMemoryStream();
            ms.Position = 0;
            ImageSourceConverter c = new ImageSourceConverter();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            currentFrame.Source = bi;
        }

        private void load_rgb(MyColorFrame frame)
        {
            timeTagLabel.Content = frame.getTime().ToString();
            System.IO.MemoryStream ms = frame.getImageMemoryStream();
            ms.Position = 0;
            ImageSourceConverter c = new ImageSourceConverter();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            currentFrame.Source = bi;
        }
        private void load_skeleton(MySkeletonFrame frame)
        {
            System.IO.MemoryStream ms = frame.toImageMemoryStream();
            ms.Position = 0;
            ImageSourceConverter c = new ImageSourceConverter();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            currentFrame.Source = bi;

        }

        private void markStartButton_Click(object sender, RoutedEventArgs e)
        {
            startIndex = currentIndex;
            if (startIndex > endIndex)
                endIndex = recorded_color_frames.Count - 1;
            caseStartLabel.Content = recorded_color_frames[startIndex].getTime().ToString();
            caseEndLabel.Content = recorded_color_frames[endIndex].getTime().ToString();
        }

        private void markEndButton_Click(object sender, RoutedEventArgs e)
        {
            endIndex = currentIndex;
            if (endIndex < startIndex)
                startIndex = 0;
            caseStartLabel.Content = recorded_color_frames[startIndex].getTime().ToString();
            caseEndLabel.Content = recorded_color_frames[endIndex].getTime().ToString();
        }

        private void completeSkeletons()
        {
            if (recorded_skeleton_frames.Count == 0)
            {
                for (int i = 0; i < recorded_color_frames.Count; i++)
                {
                    MySkeletonFrame injection = new MySkeletonFrame(recorded_color_frames[i].getTime());
                    recorded_skeleton_frames.Add(injection);
                }
                return;
            }

            // inject empty skeleton frames at the begining
            for (long firstGap = recorded_skeleton_frames[0].getTime() - recorded_color_frames[0].getTime(); firstGap > 40; firstGap = recorded_skeleton_frames[0].getTime() - recorded_color_frames[0].getTime())
            {
                MySkeletonFrame injection = new MySkeletonFrame(recorded_skeleton_frames[0].getTime() - 33);
                recorded_skeleton_frames.Insert(0, injection);
            }

            // inject empty skeleton frames at the end
            for (long lastGap = recorded_color_frames.Last().getTime() - recorded_skeleton_frames.Last().getTime(); lastGap > 40; lastGap = recorded_color_frames.Last().getTime() - recorded_skeleton_frames.Last().getTime())
            {
                MySkeletonFrame injection = new MySkeletonFrame(recorded_skeleton_frames.Last().getTime() + 33);
                recorded_skeleton_frames.Insert(recorded_skeleton_frames.Count, injection);
            }

            // making overlay and interpolating skeleton when neccessary
            for (int i = 0; i < recorded_color_frames.Count; i++)
            {
                MyColorFrame thisColorFrame = recorded_color_frames.ElementAt(i);
                if (i + 1 < recorded_skeleton_frames.Count)
                {
                    MySkeletonFrame thisSkeletonFrame = recorded_skeleton_frames[i];
                    MySkeletonFrame nextSkeletonFrame = recorded_skeleton_frames[i + 1];
                    recorded_skeleton_frames.InsertRange(i + 1, thisSkeletonFrame.interpolate(nextSkeletonFrame));
                }
            }
        }

        private void makeButton_Click(object sender, RoutedEventArgs e)
        {
            if (endIndex + 1 < recorded_color_frames.Count)
                recorded_color_frames.RemoveRange(endIndex + 1, recorded_color_frames.Count - endIndex - 1);
            recorded_color_frames.RemoveRange(0, startIndex);

            if (endIndex + 1 < recorded_skeleton_frames.Count)
                recorded_skeleton_frames.RemoveRange(endIndex + 1, recorded_skeleton_frames.Count - endIndex - 1);
            recorded_skeleton_frames.RemoveRange(0, startIndex);

            if (!Directory.Exists(casePath))
                Directory.CreateDirectory(casePath);

            startOverlayMovieCreation();
            this.Hide();
        }

        /// <summary>
        /// shows a waiting dialog and start creation of overlay movie in another thread
        /// </summary>
        private void startOverlayMovieCreation()
        {
            OverlayMovieCreatorWorker worker = new OverlayMovieCreatorWorker(ref recorded_color_frames, ref recorded_skeleton_frames, casePath + "/Overlay-Film.avi");
            worker.OnWorkerComplete += new OverlayMovieCreatorWorker.OnWorkerMethodCompleteDelegate(OnOverlayWorkerMethodComplete);
            ThreadStart tStart = new ThreadStart(worker.WorkerMethod);
            Thread t = new Thread(tStart);
            progressWindow = new progressDialog("Making overlay film. Please wait...");
            progressWindow.Owner = this;
            progressWindow.Show();
            t.Start();
        }


        /// <summary>
        /// runs on overlay movie creation complete, continues with color movie creation process
        /// </summary>
        private void OnOverlayWorkerMethodComplete(string message)
        {
            progressWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                progressWindow.Close();
                startColorMovieCreation();
            }
            ));

        }


        /// <summary>
        /// shows a waiting dialog and start creation of color movie in another thread
        /// </summary>
        private void startColorMovieCreation()
        {
            ColorMovieCreatorWorker worker = new ColorMovieCreatorWorker(recorded_color_frames, recorded_skeleton_frames, casePath + "/RGB-Film.avi");
            worker.OnWorkerComplete += new ColorMovieCreatorWorker.OnWorkerMethodCompleteDelegate(OnColorWorkerMethodComplete);
            ThreadStart tStart = new ThreadStart(worker.WorkerMethod);
            Thread t = new Thread(tStart);
            progressWindow = new progressDialog("Making RGB film. Please wait...");
            progressWindow.Owner = this;
            progressWindow.Show();
            t.Start();
        }


        /// <summary>
        /// runs on color movie creation complete, continues with skeleton movie creation process
        /// </summary>
        private void OnColorWorkerMethodComplete(string message)
        {
            progressWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                progressWindow.Close();
                startSkeletonMovieCreation();
            }
            ));

        }


        /// <summary>
        /// shows a waiting dialog and start creation of skeleton movie in another thread
        /// </summary>
        private void startSkeletonMovieCreation()
        {
            SkeletonMovieCreatorWorker worker = new SkeletonMovieCreatorWorker(recorded_skeleton_frames, casePath + "/Skeleton-Film.avi");
            worker.OnWorkerComplete += new SkeletonMovieCreatorWorker.OnWorkerMethodCompleteDelegate(OnSkeletonWorkerMethodComplete);
            ThreadStart tStart = new ThreadStart(worker.WorkerMethod);
            Thread t = new Thread(tStart);
            progressWindow = new progressDialog("Making Skeleton film. Please wait...");
            progressWindow.Owner = this;
            progressWindow.Show();
            t.Start();
        }

        /// <summary>
        /// runs on skeleton creation complete, continues with log creation process
        /// </summary>
        private void OnSkeletonWorkerMethodComplete(string message)
        {
            progressWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                progressWindow.Close();
                startMovementLogCreation();
            }
            ));
        }


        /// <summary>
        /// shows a waiting dialog and start creation of movement log file in another thread
        /// </summary>
        private void startMovementLogCreation()
        {
            MovementLogCtreationWorker worker = new MovementLogCtreationWorker(recorded_skeleton_frames, casePath + "/log.txt");
            worker.OnWorkerComplete += new MovementLogCtreationWorker.OnWorkerMethodCompleteDelegate(OnLogWorkerMethodComplete);
            ThreadStart tStart = new ThreadStart(worker.WorkerMethod);
            Thread t = new Thread(tStart);
            progressWindow = new progressDialog("Making movement log file. Please wait...");
            progressWindow.Owner = this;
            progressWindow.Show();
            t.Start();
        }

        /// <summary>
        /// runs on movement log creation complete
        /// </summary>
        private void OnLogWorkerMethodComplete(string message)
        {
            progressWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                parent.update_main_window_state();
                progressWindow.Close();
                this.Close();
            }
            ));
        }
    }
}
