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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace TRoseHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread backgroundThread = new Thread(new ThreadStart(BackgroundThread));
            backgroundThread.Start();
            Thread targetThread = new Thread(new ThreadStart(TargetThread));
            targetThread.Start();
        }

        private void BackgroundThread()
        {
            #region Wait for process
            Process tRose = null;
            do
            {
                tRose = Process.GetProcessesByName("TRose").FirstOrDefault();
                Thread.Sleep(1000);
            }
            while (tRose == null);
            MemoryHandler.Process = tRose;
            Application.Current.Dispatcher.Invoke(new Action(() => busyIndicator.IsBusy = false));
            #endregion

            int tabIndex = -1;
            do
            {
                Thread.Sleep(100);
                Application.Current.Dispatcher.Invoke(new Action(delegate() { tabIndex = tbCntrl.SelectedIndex; }));
                if (tabIndex != 1) continue;

                ObjectHandler.UpdatePlayer();
                ObjectHandler.UpdateCreeps();
                Application.Current.Dispatcher.Invoke(new Action(delegate()
                {
                    canvas.Children.Clear();

                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 10;
                    ellipse.Height = 10;
                    ellipse.Fill = new SolidColorBrush(Colors.Blue);
                    double centerX = canvas.ActualWidth / 2;
                    double centerY = canvas.ActualHeight / 2;
                    Canvas.SetLeft(ellipse, centerX);
                    Canvas.SetTop(ellipse, centerY);
                    canvas.Children.Add(ellipse);

                    foreach (Creep creep in ObjectHandler.Creeps)
                    {
                        Ellipse creepEllipse = new Ellipse();
                        creepEllipse.Width = 5;
                        creepEllipse.Height = 5;
                        creepEllipse.Fill = new SolidColorBrush(Colors.Red);
                        Canvas.SetLeft(creepEllipse, centerX + creep.PositionX - ObjectHandler.Player.PositionX);
                        Canvas.SetTop(creepEllipse, centerY + ObjectHandler.Player.PositionY - creep.PositionY);
                        canvas.Children.Add(creepEllipse);

                        if (ObjectHandler.Player.TargetId == creep.Id)
                        {
                            creepEllipse.Fill = new SolidColorBrush(Colors.DarkViolet);
                        }
                    }
                }));
            }
            while (true);
        }
        private void TargetThread()
        {
            do
            {
                Thread.Sleep(100);
                if (ObjectHandler.Player == null || ObjectHandler.Creeps == null) continue;
                Creep target = ObjectHandler.GetCreepById(ObjectHandler.Player.TargetId);
                if (target == null) continue;
                Application.Current.Dispatcher.Invoke(new Action(() =>  tbTargetInfo.Text = target.ToString() + "\r\n\r\nDistance:\r\n" + target.GetDistance(ObjectHandler.Player)));               
            }
            while (true);
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Windows.Settings window = new Windows.Settings();
            window.ShowDialog();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            Windows.About window = new Windows.About();
            window.ShowDialog();
        }
    }
}
