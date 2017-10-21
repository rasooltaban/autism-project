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
using System.IO;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>
    /// Interaction logic for TextTagControl.xaml
    /// </summary>
    public partial class TextTagControl : Window
    {
        public TextTagControl(TextTag data)
        {
            InitializeComponent();
            this.data = data;
            this.duration.Text = data.duration.ToString();
            this.msg.Text = data.text;
            this.color.SelectedIndex = data.color;
        }

        public TextTag data;

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                data.duration = int.Parse(duration.Text);
            }
            catch (FormatException f)
            {
                data.duration = 60;
            }
            data.color = color.SelectedIndex;
            data.text = msg.Text;
            data.end = data.start + (double)data.duration / (double)data.totalDuration;
            data.saveOrUpdateOnDisk();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            data = null;
            this.Close();
        }
    }
}
