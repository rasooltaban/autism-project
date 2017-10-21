using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public enum ApplicationExitCode
    {
        Success = 0,
        Failure = 1,
        CantWriteToApplicationLog = 2,
        CantPersistApplicationState = 3
    }

    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            String firstName = null;
            String lastName = null;
            firstName = e.Args[0].Replace("/", string.Empty) ;
            lastName = e.Args[1].Replace("/", string.Empty) ;
           
            MainWindow mainWindow = new MainWindow(firstName, lastName);
            mainWindow.live_button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            Thread StartR = new Thread(delegate() { Thread1(mainWindow); });
            Thread StopR = new Thread(delegate() { Thread2(mainWindow); });
            StartR.Start();
            StopR.Start();
        }
        private void Thread1(MainWindow w)
        {
            Dispatcher.Invoke(new Action(() =>
            {
             
             w.record_button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
             w.Show();
            }), DispatcherPriority.ContextIdle);
            
        }
        private void Thread2(MainWindow w)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int t =1;
            while (true)//stopwatch.ElapsedMilliseconds < 18000) //true
            {
                //String path = @"..\Mesius-UT-Kinect\CheckStopStartStatus.txt";
                String path =@System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()+"\\CheckStopStartStatus.txt";
                //MessageBox.Show(path);
                if (stopwatch.ElapsedMilliseconds > t * 5000)
                {
                    String text = File.ReadAllText(path);
                    t++;
                    if (text.Equals("Stop"))
                    {
                        //MessageBox.Show("found stop in text file");
                        File.WriteAllText(path, String.Empty);
                        break;
                    }
                }
                
            }
            stopwatch.Stop();
            Dispatcher.Invoke(new Action(() =>
            {
                w.record_button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            }), DispatcherPriority.ContextIdle);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 450000)
            {

            }
           // MessageBox.Show("after 3 second");
            //try
            //{
            //    //foreach(Process proc in Process.GetProcessesByName("vshost32")){
            //    //    MessageBox.Show("find application");
            //    //    proc.Kill();
            //   // }
            //    Application.Current.Shutdown();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
          Dispatcher.Invoke(new Action(() =>
            {
                //MessageBox.Show("find application");
            Application.Current.Shutdown();
            System.Environment.Exit(0);
            }), DispatcherPriority.ContextIdle);
            return;
        }
        void App_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                // Write entry to application log
                if (e.ApplicationExitCode == (int)ApplicationExitCode.Success)
                {
                    WriteApplicationLogEntry("Failure", e.ApplicationExitCode);
                }
                else
                {
                    WriteApplicationLogEntry("Success", e.ApplicationExitCode);
                }
            }
            catch
            {
                // Update exit code to reflect failure to write to application log
                e.ApplicationExitCode = (int)ApplicationExitCode.CantWriteToApplicationLog;
            }

            // Persist application state
            try
            {
                PersistApplicationState();
            }
            catch
            {
                // Update exit code to reflect failure to persist application state
                e.ApplicationExitCode = (int)ApplicationExitCode.CantPersistApplicationState;
            }
        }

        void WriteApplicationLogEntry(string message, int exitCode)
        {
            // Write log entry to file in isolated storage for the user
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly();
            using (Stream stream = new IsolatedStorageFileStream("log.txt", FileMode.Append, FileAccess.Write, store))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string entry = string.Format("{0}: {1} - {2}", message, exitCode, DateTime.Now);
                writer.WriteLine(entry);
            }
        }

        void PersistApplicationState()
        {
            // Persist application state to file in isolated storage for the user
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly();
            using (Stream stream = new IsolatedStorageFileStream("state.txt", FileMode.Create, store))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (DictionaryEntry entry in this.Properties)
                {
                    writer.WriteLine(entry.Value);
                }
            }
        }
    }
}
