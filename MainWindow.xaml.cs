
using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using KinectWpfViewers;

namespace KineticsAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(MainWindow),
                new PropertyMetadata(null));

        /// <summary>
        /// Kinect sensor chooser from the KinectToolkit
        /// </summary>
        
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.KinectSensorManager = new KinectSensorManager();
            //Subscribe to the KinectSensorChanged Event
            this.KinectSensorManager.KinectSensorChanged += this.KinectSensorChanged;
            this.DataContext = this.KinectSensorManager;

            this.SensorChooserUI.KinectSensorChooser = sensorChooser;
            sensorChooser.Start();
            
            // Bind the KinectSensor from the sensorChooser to the KinectSensor on the KinectSensorManager
            var kinectSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.KinectSensorManager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);

        }

        /// <summary>
        /// Actions to take when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        
        /// <summary>
        /// Event that is triggered when the sensor is changed, this will be used to stop the old sensor and
        /// start the new sensor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null != args.OldValue)
            {
                this.UninitializeKinectServices(args.OldValue);
            }


            if (null != args.NewValue)
            {
                this.InitializeKinectServices(this.KinectSensorManager, args.NewValue);
            }
        }

        /// <summary>
        /// Setup Kinect Services
        /// </summary>
        /// <param name="kinectSensorManager"></param>
        /// <param name="sensor"></param>
        private void InitializeKinectServices(KinectSensorManager kinectSensorManager, KinectSensor sensor)
        {
            // Enable Streams that are being used
            kinectSensorManager.SkeletonStreamEnabled = true;
            kinectSensorManager.DepthStreamEnabled = true;
            
            
            kinectSensorManager.DepthFormat = DepthImageFormat.Resolution640x480Fps30;

            // Set statusbar to ready
            statusBarText.Text = Properties.Resources.KinectReady;
        }

        /// <summary>
        /// Uninitialize kinect services
        /// </summary>
        /// <param name="sensor"></param>
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            //sensor.DepthFrameReady -= KinectSensorDepthFrameReady;
            statusBarText.Text = Properties.Resources.NoKinectReady;
        }

        /// <summary>
        /// Get and set KinectSensorManager
        /// </summary>
        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        /// <summary>
        /// Window Closing Actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            sensorChooser.Stop();
        }

        /// <summary>
        /// Window Closed Actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            this.KinectSensorManager.KinectSensor = null;
        }

        private void beginButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
