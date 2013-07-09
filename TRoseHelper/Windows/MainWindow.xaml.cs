using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using TRoseHelper.Interaction;
using TRoseHelper.Interaction.MemoryEditing;
using TRoseHelper.TRose;
using TRoseHelper.TRose.Objects;

namespace TRoseHelper.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Thread _backgroundThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _backgroundThread = new Thread(BackgroundThread);
            _backgroundThread.Start();
        }

        private void BackgroundThread()
        {
            do
            {
                Thread.Sleep(Properties.Settings.Default.ThreadSleepInterval);

                bool isReady = MemoryHandler.IsReady(Properties.Settings.Default.ProcessName);
                Application.Current.Dispatcher.Invoke(new Action(() => BusyIndicator.IsBusy = !isReady));
                if (!isReady) continue;
                
                ObjectHandler.UpdatePlayer();
                ObjectHandler.UpdateCreeps();

                Creep target = ObjectHandler.GetCreepById(ObjectHandler.Player.TargetId);
                if (target != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => TbTargetInfo.Text = target + "\r\n\r\nDistance:\r\n" + target.GetDistance()));
                }

                #region Render radar
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    Canvas.Children.Clear();

                    double centerX = Canvas.ActualWidth / 2;
                    double centerY = Canvas.ActualHeight / 2;

                    Ellipse ellipse = new Ellipse { Width = 10, Height = 10, Fill = new SolidColorBrush(Colors.Blue) };
                    Canvas.SetLeft(ellipse, centerX);
                    Canvas.SetTop(ellipse, centerY);
                    Canvas.Children.Add(ellipse);

                    foreach (Creep creep in ObjectHandler.Creeps)
                    {
                        Ellipse creepEllipse = new Ellipse { Width = 5, Height = 5, Fill = new SolidColorBrush(Colors.Red) };
                        Canvas.SetLeft(creepEllipse, centerX + creep.PositionX - ObjectHandler.Player.PositionX);
                        Canvas.SetTop(creepEllipse, centerY + ObjectHandler.Player.PositionY - creep.PositionY);
                        Canvas.Children.Add(creepEllipse);

                        if (ObjectHandler.Player.TargetId == creep.Id)
                        {
                            creepEllipse.Fill = new SolidColorBrush(Colors.DarkViolet);
                        }
                    }
                #endregion
                }));
            }
            while (true);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings window = new Settings();
            window.ShowDialog();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About window = new About();
            window.ShowDialog();
        }
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _backgroundThread.Abort();
        }
    }
}
