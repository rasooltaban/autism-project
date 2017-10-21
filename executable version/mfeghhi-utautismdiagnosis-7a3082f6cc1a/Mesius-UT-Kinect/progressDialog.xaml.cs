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
using System.ComponentModel;
using System.Threading;
namespace UTKinectSkeletonMovementDetector
{
    /// <summary>
    /// Interaction logic for progressDialog.xaml
    /// </summary>
    public partial class progressDialog : Window
    {
        public progressDialog(string msg)
        {
            InitializeComponent();
            this.label.Content = msg;
        }

    }
}
