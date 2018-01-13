using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Server;
using Server.Logic;

namespace Robot
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        private static MainWindow instance;
        public static MainWindow GetInstance() { return instance; }

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            Title = "Robot";
            btnPause.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        private RobotClientMgr mClientMgr = new RobotClientMgr();

        private void OnStart(object sender, RoutedEventArgs e)
        {
            mClientMgr.Init(txtIP.Text);
            mClientMgr.GenerateRobot(int.Parse(txtStartID.Text), int.Parse(txtEndID.Text), int.Parse(txtMapCode.Text), int.Parse(txtMapSize.Text));
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
            btnStop.IsEnabled = true;

        }

        private void OnPause(object sender, RoutedEventArgs e)
        {
            if (btnPause.Content == "Pause")
            {
                btnPause.Content = "Continue";
                mClientMgr.IsPaused = true;
            }
            else
            {
                btnPause.Content = "Pause";
                mClientMgr.IsPaused = false;
            }

        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            mClientMgr.IsStoped = true;
            btnStart.IsEnabled = true;
            btnPause.IsEnabled = false;
            btnStop.IsEnabled = false;

        }

        private void _showText(string s)
        {
            if (lstShow.Items.Count > 1000)
            {
                lstShow.Items.Clear();
            }
            lstShow.Items.Add(s);
        }

        public void ShowText(string s)
        {
            lstShow.Dispatcher.BeginInvoke( new Action<string>(_showText),s );
        }

    }
}
