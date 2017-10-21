using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.FFMPEG;
using System.Windows.Forms;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using Coding4Fun.Kinect.WinForm;
using System.IO;
using SimulatorLib;
using System.Timers;
using System.Threading;
using Kinect.Toolbox.Record;
using System.Windows.Threading;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;


namespace UTKinectSkeletonMovementDetector
{

    /// <summary>   
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(String firstName, String lastName)
        {
            //System.Windows.MessageBox.Show(firstName);
            //System.Windows.MessageBox.Show(lastName);
            InitializeComponent();
            int counter = 1;
            live_button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            newProject(@System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString() + "\\autism\\", firstName, lastName, counter);

        }

        /// <summary>
        /// current color frame as bitmap
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// recording status of the moment
        /// </summary>
        private bool is_recording = false;

        /// <summary>
        /// Count of cases made in this project
        /// </summary>
        private int case_count = 0;

        /// <summary>
        /// collects all recorded skeleton frames
        /// </summary>
        private List<MySkeletonFrame> recorded_skeleton_frames;

        /// <summary>
        /// collects all recorded color frames
        /// </summary>
        private List<MyColorFrame> recorded_color_frames;

        /// <summary>
        /// Path selected through path selection dialog
        /// </summary>
        private String selectedPath = "";

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Progress dialog which pops up when post processing recorded data
        /// </summary>
        private progressDialog progressWindow;

        /// <summary>
        /// Width of our output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// status of kinect sensors activity
        /// </summary>
        private bool is_observing_live = false;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing group for overlay frames rendering output
        /// </summary>
        private DrawingGroup overlaydrawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Drawing overlay image that we will display
        /// </summary>
        private DrawingImage overlayimageSource;

        /// <summary>
        /// timer to control slider
        /// </summary>
        private DispatcherTimer _timer;

        /// <summary>
        /// slider is currently being dragged or not
        /// </summary>
        private bool indrag = false;

        /// <summary>
        /// tags of the current selected case 
        /// </summary>
        private ObservableCollection<TextTag> currentTags;

        /// <summary>
        /// indicate the current state of player
        /// </summary>
        private String player_state = "paused";

        /// <summary>
        /// Current Selected Tag
        /// </summary>
        private TextBlock currentSelectedTag = null;

        /// <summary>
        /// Selected Tag is unlocked or not
        /// </summary>
        private Boolean selectedTagUnlocked = false;

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Enable/Disable buttons based on the Program State and update the status
        /// </summary>
        public void update_main_window_state()
        {
            if (this.selectedPath == "")
            {
                // this means no project is yet loaded
                live_button.IsEnabled = false;
                caseMaker_button.IsEnabled = false;
                play_pause_button.IsEnabled = false;
                caseSelector.IsEnabled = false;
                record_button.IsEnabled = false;
                tagLockButton.IsEnabled = false;
                newTagButton.IsEnabled = false;
                slider.IsEnabled = false;
                playerClear();
                status_text.Text = "No project is loaded.";
            }
            else
            {
                // a project is loaded
                live_button.IsEnabled = true;
                record_button.IsEnabled = true;
                case_count = Directory.GetDirectories(selectedPath + "\\cases").Count();
                if (case_count > 0)
                {
                    caseSelector.IsEnabled = true;
                    play_pause_button.IsEnabled = true;
                    newTagButton.IsEnabled = true;
                    slider.IsEnabled = true;
                    caseSelector.Items.Clear();
                    for (int i = 1; i <= case_count; i++)
                    {
                        caseSelector.Items.Add(i);
                    }
                    caseSelector.SelectedIndex = 0;
                }
                else
                {
                    caseSelector.IsEnabled = false;
                    play_pause_button.IsEnabled = false;
                    newTagButton.IsEnabled = false;
                    tagLockButton.IsEnabled = false;
                    playerClear();
                    slider.IsEnabled = false;
                }
                if (recorded_skeleton_frames != null && recorded_skeleton_frames.Count > 0)
                    caseMaker_button.IsEnabled = true;
                else
                    caseMaker_button.IsEnabled = false;
                status_text.Text = "Current project: " + this.selectedPath;
                clabel2.Content = recorded_color_frames.Count;
                clabel3.Content = recorded_skeleton_frames.Count;
            }
            clabel1.Content = is_observing_live;
            change_main_window_mode(is_observing_live);
        }

        /// <summary>
        /// load the recorded raw frames of the project located in selectedPath
        /// </summary>
        private void open_selectedPath_project()
        {
            recorded_color_frames = new List<MyColorFrame>();
            recorded_skeleton_frames = new List<MySkeletonFrame>();

            String[] rgbFilePaths = Directory.GetFiles(selectedPath + "\\raw\\rgb", "*.jpg");
            foreach (String filePath in rgbFilePaths) {
                long timeTag = long.Parse(filePath.Split('\\').Last().Substring(0, filePath.Split('\\').Last().Length - 4));
                System.Drawing.Bitmap img = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(filePath);
                recorded_color_frames.Add(new MyColorFrame(timeTag, img));
            }

            if (File.Exists(selectedPath + "\\raw\\skeleton.txt"))
            {
                using (StreamReader r = new StreamReader(selectedPath + "\\raw\\skeleton.txt"))
                {
                    String headline = r.ReadLine();
                    while (headline != null && headline != "")
                    {
                        long timeTag = long.Parse(headline.Split(' ')[0]);
                        MySkeleton skeleton = new MySkeleton(int.Parse(headline.Split(' ')[1]));
                        for (int i = 0; i < 20; i++)
                        {
                            String line = r.ReadLine();
                            String jointName = line.Split(' ')[0];
                            float x = float.Parse(line.Split(' ')[1]);
                            float y = float.Parse(line.Split(' ')[2]);
                            float z = float.Parse(line.Split(' ')[3]);
                            skeleton.addJoint(jointName, x, y, z);
                        }
                        recorded_skeleton_frames.Add(new MySkeletonFrame(skeleton, timeTag));
                        r.ReadLine();
                        headline = r.ReadLine();
                    }
                }
            }
            case_count = Directory.GetDirectories(selectedPath + "\\cases").Count();

        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();
            this.overlaydrawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Skeleton.Source = this.imageSource;
            Skeleton2.Source = this.imageSource;

            update_main_window_state();

        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                this.sensor.Dispose();
            }

        }

        /// <summary>
        /// runs when new skeleton frame is received
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
            long now = -1;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {  
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    now = skeletonFrame.Timestamp;
                }
            }

            if (is_recording)
            {
                Skeleton trackedSkeleton = null;
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons.ElementAt(i).TrackingState == SkeletonTrackingState.Tracked)
                    {
                        trackedSkeleton = skeletons[i];
                        break;
                    }
                }
                if (trackedSkeleton != null)
                {
                    MySkeletonFrame skelFrame = new MySkeletonFrame(trackedSkeleton, now);
                    recorded_skeleton_frames.Add(skelFrame);
                    clabel3.Content = recorded_skeleton_frames.Count;
                }
            }

            if (is_observing_live)
            {
                using (DrawingContext dc = this.drawingGroup.Open(), overlaydc = this.overlaydrawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    overlaydc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                    if (skeletons.Length != 0)
                    {
                        List<Skeleton> skeletonsinframe = new List<Skeleton>();
                        foreach (Skeleton skel in skeletons)
                        {
                            RenderClippedEdges(skel, dc);
                            if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                this.DrawBonesAndJoints(skel, dc);
                                this.DrawBonesAndJoints(skel, overlaydc);
                            }
                            else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                dc.DrawEllipse(
                                this.centerPointBrush,
                                null,
                                this.SkeletonPointToScreen(skel.Position),
                                BodyCenterThickness,
                                BodyCenterThickness);

                                overlaydc.DrawEllipse(
                                this.centerPointBrush,
                                null,
                                this.SkeletonPointToScreen(skel.Position),
                                BodyCenterThickness,
                                BodyCenterThickness);
                            }

                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                    this.overlaydrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                }
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints

            DateTime now = DateTime.Now;
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {

                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            //DepthImagePoint depthPoint;
            if(this.sensor!=null){
                //depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
                System.Drawing.Point depthPoint = SkeletonPointConverter.convert(new JointPosition(skelpoint));
                return new Point(depthPoint.X, depthPoint.Y);

            }
            else
            {
                System.Drawing.Point p = SkeletonPointConverter.convert(new JointPosition(skelpoint));
                return new Point(p.X, p.Y);
            }
            
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }
            
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }
        /// <summary>
        /// runs when new color frame is received
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (is_observing_live)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        if (is_recording)
                        {
                            long now = colorFrame.Timestamp;
                            MyColorFrame myColorFrame = new MyColorFrame(now, colorFrame.ToBitmap());
                            recorded_color_frames.Add(myColorFrame);
                            clabel2.Content = recorded_color_frames.Count;
                        }
                        // Copy the pixel data from the image to a temporary array
                        colorFrame.CopyPixelDataTo(this.colorPixels);

                        // Write the pixel data into our bitmap
                        this.colorBitmap.WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                            this.colorPixels,
                            this.colorBitmap.PixelWidth * sizeof(int),
                            0);
                    }
                }
            }
        }

        /// <summary>
        /// runs when live button is clicked
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void live_button_Click(object sender, RoutedEventArgs e)
        {
            toggle_live();
        }
        private void go_live()
        {
            toggle_live();
        }

        /// <summary>
        /// changes the mode of main window to either live mode or offline mode
        /// </summary>
        /// <param name="mode">true = live, false = offline</param>
        private void change_main_window_mode(Boolean mode)
        {
            if (mode)
            {
                record_button.Visibility = Visibility.Visible;
                caseMaker_button.Visibility = Visibility.Hidden;
                caseSelector.Visibility = Visibility.Hidden;
                play_pause_button.Visibility = Visibility.Hidden;
                liveConsoleGrid.Visibility = Visibility.Visible;
                offlineConsoleGrid.Visibility = Visibility.Hidden;
                tagLockButton.Visibility = Visibility.Hidden;
                newTagButton.Visibility = Visibility.Hidden;
                slider.Visibility = Visibility.Hidden;
                tagsPlace.Visibility = Visibility.Hidden;
                
                overlayplayer.Visibility = Visibility.Hidden;
                skeletonplayer.Visibility = Visibility.Hidden;
                rgbplayer.Visibility = Visibility.Hidden;

                Skeleton.Visibility = Visibility.Visible;
                Skeleton2.Visibility = Visibility.Visible;
                ColorImage.Visibility = Visibility.Visible;
                ColorImage2.Visibility = Visibility.Visible;

            }
            else
            {
                record_button.Visibility = Visibility.Hidden;
                caseMaker_button.Visibility = Visibility.Visible;
                caseSelector.Visibility = Visibility.Visible;
                play_pause_button.Visibility = Visibility.Visible;
                liveConsoleGrid.Visibility = Visibility.Hidden;
                offlineConsoleGrid.Visibility = Visibility.Visible;
                tagLockButton.Visibility = Visibility.Visible;
                newTagButton.Visibility = Visibility.Visible;
                slider.Visibility = Visibility.Visible;
                tagsPlace.Visibility = Visibility.Visible;

                overlayplayer.Visibility = Visibility.Visible;
                skeletonplayer.Visibility = Visibility.Visible;
                rgbplayer.Visibility = Visibility.Visible;

                Skeleton.Visibility = Visibility.Hidden;
                Skeleton2.Visibility = Visibility.Hidden;
                ColorImage.Visibility = Visibility.Hidden;
                ColorImage2.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// if sensors are already off this makes them go on and viseversa
        /// </summary>
        private bool toggle_live()
        {
            bool result;
            if (!is_observing_live)
            {

                // turn on kinect
                if (!turn_on_live_observation())
                {
                    // fail
                    System.Windows.MessageBox.Show("No kinect found. Please check the connections and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    is_observing_live = false;
                    live_button.Content = "Go Live";
                    result = false;
                }
                else
                {
                    // success
                    playerPause();
                    
                    currentSelectedTag = null;
                    selectedTagUnlocked = false;
                    currentTags = new ObservableCollection<TextTag>();
                    tagsPlace.Children.Clear();
                    is_observing_live = true;
                    live_button.Content = "Go Offline";
                    result = true;
                }
            }
            else
            {
                // turn off live observation
                live_button.Content = "Go Live";
                turn_off_live_observation();
                is_observing_live = false;
                result = true;
            }
            update_main_window_state();
            return result;
        }

        /// <summary>
        /// turns on kinect sensors
        /// </summary>
        private bool turn_on_live_observation()
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();
            this.overlaydrawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);
            this.overlayimageSource = new DrawingImage(this.overlaydrawingGroup);

            // Display the drawing using our image control
            Skeleton.Source = this.imageSource;
            Skeleton2.Source = this.overlayimageSource;


            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {


                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;


                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.ColorImage.Source = this.colorBitmap;
                this.ColorImage2.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;


                // Start the sensor!
                try
                {

                    this.sensor.Start();
                    //set Elevation angle to zero
                    this.sensor.ElevationAngle = 0;
                }
                catch (Exception)
                {
                    this.sensor = null;
                }
                return true;
            } else 
            {
                return false;
            }
        }

        /// <summary>
        /// turns off kinect sensors
        /// </summary>
        private void turn_off_live_observation()
        {
            this.sensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
            this.sensor.ColorFrameReady -= this.SensorColorFrameReady;
            this.drawingGroup = null;
            this.overlaydrawingGroup = null;
            this.overlayimageSource = null;
            this.imageSource = null;
            this.Skeleton.Source = null;
            this.Skeleton2.Source = null;
            this.sensor = null;
            this.colorPixels = null;
            this.colorBitmap = null;
            this.ColorImage.Source = null;
            this.ColorImage2.Source = null;
        }

        /// <summary>
        /// with the execution of this function every frame received from now on is recorded
        /// </summary>
        private void start_recording()
        {            
            if (this.selectedPath != "" && this.selectedPath != null)
            {
                recorded_skeleton_frames = new List<MySkeletonFrame>();
                recorded_color_frames = new List<MyColorFrame>();
                is_recording = true;
                //record_button.Content = "[ Stop ]";
            }
        }

        /// <summary>
        /// runs when new record/stop button is clicked
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void record_button_Click(object sender, RoutedEventArgs e)
        {
            if (!is_recording)
            {
                //start recording...
                if (!is_observing_live)
                {
                    if (toggle_live())
                    {
                        start_recording();
                    }
                }
                else
                {
                    start_recording();
                }

            }
            else
            {
                // stop recording...
                record_button.Content = "Record";
                is_recording = false;
                update_main_window_state();
                startWritingFramesOnDisk();
            }
        }
        private void record()
        {
            if (!is_recording)
            {
                //start recording...
                if (!is_observing_live)
                {
                    if (toggle_live())
                    {
                        start_recording();
                    }
                }
                else
                {
                    start_recording();
                }

            }
        }
        private void stop()
        {
            if (is_recording)
            {
                is_recording = false;
                update_main_window_state();
                startWritingFramesOnDisk();
            }
        }

        /// <summary>
        /// shows a waiting dialog and start writing frames on disk
        /// </summary>
        public void startWritingFramesOnDisk()
        {
            WritingFramesOnDiskWorker worker = new WritingFramesOnDiskWorker(recorded_color_frames, recorded_skeleton_frames, selectedPath);
            //worker.OnWorkerComplete += new WritingFramesOnDiskWorker.OnWorkerMethodCompleteDelegate(OnWritingFramesOnDiskComplete);
            ThreadStart tStart = new ThreadStart(worker.WorkerMethod);
            Thread t = new Thread(tStart);
            //progressWindow = new progressDialog("Writing recorded frames on disk. Please wait...");
            //progressWindow.Owner = this;
            //progressWindow.Show();
            t.Start();
        }

        /// <summary>
        /// runs on writing frames on disk complete
        /// </summary>
        private void OnWritingFramesOnDiskComplete(string message)
        {
            progressWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                progressWindow.Close();
            }
            ));
        }


        private void ColorImage2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.Skeleton2.Height = e.NewSize.Height * 271.0 / 300.0;
                this.Skeleton2.Width = e.NewSize.Width * 361.0 / 400.0;
            }
        }

        private void caseMaker_button_Click(object sender, RoutedEventArgs e)
        {
            CaseMaker cm = new CaseMaker(this, recorded_color_frames, recorded_skeleton_frames, selectedPath);
            cm.ShowDialog();
        }

        private void newProjectItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m = new FolderBrowserDialog();
            DialogResult res = m.ShowDialog();
            this.selectedPath = m.SelectedPath;
            if (this.selectedPath != "" && this.selectedPath != null)
            {
                Directory.CreateDirectory(selectedPath + "\\cases");
                Directory.CreateDirectory(selectedPath + "\\raw\\rgb");
                this.recorded_color_frames = new List<MyColorFrame>();
                this.recorded_skeleton_frames = new List<MySkeletonFrame>();
                update_main_window_state();
            }
        }
        private void newProject(String Address, String firstName, String lastName, int Counter)
        {
            String path = Address + "\\" + firstName + "_" + lastName + "_" + Counter.ToString();
            this.selectedPath = path;
            if (this.selectedPath != "" && this.selectedPath != null)
            {
                Directory.CreateDirectory(selectedPath + "\\cases");
                Directory.CreateDirectory(selectedPath + "\\raw\\rgb");
                this.recorded_color_frames = new List<MyColorFrame>();
                this.recorded_skeleton_frames = new List<MySkeletonFrame>();
                update_main_window_state();
            }
        }

        private void openProjectItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m = new FolderBrowserDialog();
            DialogResult res = m.ShowDialog();
            
            this.selectedPath = m.SelectedPath;
            if (this.selectedPath != "" && this.selectedPath != null)
            {
                open_selectedPath_project();
                update_main_window_state();
            }
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void play_pause_button_Click(object sender, RoutedEventArgs e)
        {
            if (player_state == "paused")
            {
                playerPlay();
            }
            else
            {
                playerPause();
            }
        }
        private void playerPlay()
        {
            overlayplayer.Play();
            rgbplayer.Play();
            skeletonplayer.Play();
            player_state = "playing";
            play_pause_button.Content = "[ Pause || ]";
        }

        private void playerPause()
        {
            overlayplayer.Pause();
            rgbplayer.Pause();
            skeletonplayer.Pause();
            player_state = "paused";
            play_pause_button.Content = "Play >";
        }

        private void playerClear()
        {
            if (overlayplayer != null)
            {
                overlayplayer.Close();
                rgbplayer.Close();
                skeletonplayer.Close();
            }
            tagsPlace.Children.Clear();
            if (_timer != null)
                _timer.Stop();
            slider.Value = 0;
            player_state = "paused";
            play_pause_button.Content = "play";
        }

        private void load_case_for_watch(String casePath)
        {
            tagsPlace.Children.Clear();
            overlayplayer.Source = new Uri(casePath + "/Overlay-Film.avi");
            overlayplayer.LoadedBehavior = MediaState.Manual;

            rgbplayer.Source = new Uri(casePath + "/RGB-Film.avi");
            rgbplayer.LoadedBehavior = MediaState.Manual;

            skeletonplayer.Source = new Uri(casePath + "/Skeleton-Film.avi");
            skeletonplayer.LoadedBehavior = MediaState.Manual;

            currentTags = new ObservableCollection<TextTag>();
            tagsDataGrid.ItemsSource = currentTags;
            if (Directory.Exists(casePath + "\\tags"))
            {
                foreach (string path in Directory.GetFiles(casePath + "\\tags"))
                {
                    TextTag tt = new TextTag(path);
                    currentTags.Add(tt);
                    visualizeTag(tt);
                }
            }
            playerPlay();
        }

        /// <summary>
        /// runs when a new media is opened and loaded into overlayplayer
        /// </summary>
        private void overlayplayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = overlayplayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            // Create a timer that will update the counters and the time slider
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (overlayplayer.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                // Updating time slider
                if (indrag)
                {
                    overlayplayer.Position = TimeSpan.FromMilliseconds(slider.Value);
                    rgbplayer.Position = TimeSpan.FromMilliseconds(slider.Value);
                    skeletonplayer.Position = TimeSpan.FromMilliseconds(slider.Value);
                }
                else
                    slider.Value = overlayplayer.Position.TotalMilliseconds;
            }
            if (currentSelectedTag != null && selectedTagUnlocked)
            {
                TextTag tt = currentSelectedTag.DataContext as TextTag;
                double length = tt.end - tt.start;
                tt.start = slider.Value / slider.Maximum;
                tt.end = tt.start + length;
                tagLocationRefresh(currentSelectedTag);

            }
            foreach (TextTag tag in currentTags)
            {
                double currentPos = slider.Value / slider.Maximum;
                if (tag.start <= currentPos && currentPos <= tag.end)
                {
                    tag.visible = true;
                }
                else
                {
                    tag.visible = false;
                }
                tagsDataGrid.Items.Refresh();
            }

        }

        /// <summary>
        /// runs when the cursor enters the slider area
        /// </summary>
        private void slider_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            indrag = true;
            overlayplayer.Pause();
            rgbplayer.Pause();
            skeletonplayer.Pause();
        }

        /// <summary>
        /// runs when a new media is opened and loaded into overlayplayer
        /// </summary>
        private void slider_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            indrag = false;
            if (player_state == "playing")
            {
                overlayplayer.Play();
                rgbplayer.Play();
                skeletonplayer.Play();
            }
        }

        private void caseSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_timer != null)
                _timer.Stop();
            load_case_for_watch(selectedPath + "\\cases\\" + (caseSelector.SelectedIndex + 1).ToString());
        }

        private void newTagButton_Click(object sender, RoutedEventArgs e)
        {
            TextTagControl newttc = new TextTagControl(new TextTag(slider.Value / slider.Maximum, overlayplayer.NaturalDuration.TimeSpan.Seconds, selectedPath + "\\cases\\" + (caseSelector.SelectedIndex + 1)));
            newttc.ShowDialog();
            TextTag newTag = newttc.data;
            if (newTag != null)
            {
                visualizeTag(newTag);
                currentTags.Add(newTag);
            }
        }

        private void visualizeTag(TextTag tt)
        {
            TextBlock tb = new TextBlock();
            tb.DataContext = tt;
            if (tt.color == 0)
                tb.Background = Brushes.Tomato;
            else if (tt.color == 1)
                tb.Background = Brushes.SkyBlue;
            else if (tt.color == 2)
                tb.Background = Brushes.LimeGreen;
            else if (tt.color == 3)
                tb.Background = Brushes.Gold;
            else if (tt.color == 4)
                tb.Background = Brushes.Orange;
            tb.Opacity = 0.4;
            tb.Height = 17;
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            int left = (int)(tt.start * tagsPlace.ActualWidth);
            tb.Margin = new Thickness(left, 0, 0, 0);
            tb.Width = tagsPlace.ActualWidth * (tt.end - tt.start);
            tb.MouseLeftButtonDown += new MouseButtonEventHandler(tag_mouseLeftDown);
            tb.MouseRightButtonDown += new MouseButtonEventHandler(tag_mouseRightDown);
            tagsPlace.Children.Add(tb);
        }
        private void tagLocationRefresh(TextBlock tb)
        {
            TextTag tt = tb.DataContext as TextTag;
            if (tt.color == 0)
                tb.Background = Brushes.Tomato;
            else if (tt.color == 1)
                tb.Background = Brushes.SkyBlue;
            else if (tt.color == 2)
                tb.Background = Brushes.LimeGreen;
            else if (tt.color == 3)
                tb.Background = Brushes.Gold;
            else if (tt.color == 4)
                tb.Background = Brushes.Orange;
            int left = (int)(tt.start * tagsPlace.ActualWidth);
            tb.Margin = new Thickness(left, 0, 0, 0);
            tb.Width = tagsPlace.ActualWidth * (tt.end - tt.start);
        }

        private void tag_mouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock clickedTag = sender as TextBlock;
            if (currentSelectedTag != clickedTag)
            {
                currentSelectedTag = clickedTag;
                lockSelectedTag();
                tagLockButton.IsEnabled = true;
            }
            else
            {
                lockSelectedTag();
                currentSelectedTag = null;
                tagLockButton.IsEnabled = false;
            }
        }
        
        private void tag_mouseRightDown(object sender, MouseButtonEventArgs e)
        {
            TextTag clickedTag = (sender as TextBlock).DataContext as TextTag;
            TextTagControl ttc = new TextTagControl(clickedTag);
            ttc.ShowDialog();
            TextTag newTag = ttc.data;
            if (newTag != null)
            {
                tagLocationRefresh(sender as TextBlock);
            }
        }

        private void unlockSelectedTag()
        {
            tagLockButton.Content = "Lock Tag";
            selectedTagUnlocked = true;
        }

        private void lockSelectedTag()
        {
            (currentSelectedTag.DataContext as TextTag).saveOrUpdateOnDisk();
            tagLockButton.Content = "Unlock Tag";
            selectedTagUnlocked = false;
        }

        private void tagLockButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTagUnlocked)
                lockSelectedTag();
            else
                unlockSelectedTag();
        }
    }

    


}
