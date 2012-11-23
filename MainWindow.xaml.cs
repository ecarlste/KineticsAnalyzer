
using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;

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

        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


        /// <summary>
        /// Image for drawing depth information from the kinect
        /// </summary>
        private WriteableBitmap depthImageBitmap;

        /// <summary>
        /// Temporary storage space for frames fetched from the Kinect DepthStream
        /// </summary>
        private DepthImagePixel[] depthImagePixels;

        /// <summary>
        /// Temporary storage space for our depth data after it's converted to color
        /// </summary>
        private byte[] colorPixels;

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
            this.SensorChooserUI.KinectSensorChooser = sensorChooser;
            sensorChooser.Start();

        }

        /// <summary>
        /// Actions to take when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            depthImagePixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            colorPixels = new byte[sensor.DepthStream.FramePixelDataLength * sizeof(int)];
            depthImageBitmap = new WriteableBitmap(sensor.DepthStream.FrameWidth,
                sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            DepthImage.Source = depthImageBitmap;
            sensor.DepthFrameReady += KinectSensorDepthFrameReady;

            // Set statusbar to ready
            statusBarText.Text = Properties.Resources.KinectReady;
        }

        /// <summary>
        /// Uninitialize kinect services
        /// </summary>
        /// <param name="sensor"></param>
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            DepthImage.Source = null;
            sensor.DepthFrameReady -= KinectSensorDepthFrameReady;
            statusBarText.Text = Properties.Resources.NoKinectReady;
        }
        
        private void KinectSensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            int imageWidth = 0;
            int imageHeight = 0;
            int minDepth = 0;
            int maxDepth = 0;

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    imageWidth = depthFrame.Width;
                    imageHeight = depthFrame.Height;

                    depthFrame.CopyDepthImagePixelDataTo(depthImagePixels);

                    minDepth = depthFrame.MinDepth;
                    maxDepth = depthFrame.MaxDepth;
                }

                if (imageWidth != 0)
                {
                    DepthColorizer.ConvertDepthFrame(depthImagePixels, minDepth, maxDepth, colorPixels);

                    depthImageBitmap.WritePixels(
                        new Int32Rect(0, 0, imageWidth, imageHeight),
                        colorPixels, imageWidth * Bgr32BytesPerPixel, 0);
                }
            }
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
    }
}
