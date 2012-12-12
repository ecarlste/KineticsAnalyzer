
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using KinectWpfViewers;
using System.Windows.Controls;

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

            // TODO: Remove commented binding after you understand it.
            //Binding statusBarBinding = new Binding("ProgressValue");
            //statusBarBinding.Source = this.SkeletonViewer.RiskAnalyzer;
            //statusBarText.SetBinding(TextBlock.TextProperty, statusBarBinding);

            this.statusBar.DataContext = this.SkeletonViewer.RiskAnalyzer;
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
        }

        /// <summary>
        /// Uninitialize kinect services
        /// </summary>
        /// <param name="sensor"></param>
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            //sensor.DepthFrameReady -= KinectSensorDepthFrameReady;
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

        /// <summary>
        /// Event fired when the storyboard animation is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAnalyzer(object sender, EventArgs e)
        {
            SkeletonViewer.StartMeasuring();
        }

        /// <summary>
        /// Deal with Button Clicked Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickedEvent(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSource = e.OriginalSource as FrameworkElement;
            switch (feSource.Name)
            {
                case "BeginButton":
                    Button beginButton = feSource as Button;
                    Storyboard countdown = this.FindResource("Countdown") as Storyboard;

                    // Start the animation and change the button content
                    if ( beginButton.Content.Equals("Begin Test") )
                    {
                        countdown.Begin();
                        beginButton.Content = "End Test";
                    }
                    else
                    {
                        // If the animation is running stop the storyboard
                        // otherwise the animation is done so stop measuring needs to be called.
                        if (countdown.GetCurrentState().Equals(ClockState.Active))
                        {
                            countdown.Stop();
                        }
                        else
                        {
                            SkeletonViewer.StopMeasuring();

                            // once the SkeletonViewer's RiskAnalyzer has finished running, we can give the results
                            // to the AnalyzedSkeletonDisplay object and determine the skeleton frame to use for
                            // display
                            this.AnalysisResultsDisplay.DetermineSkeletonFrameUsed(SkeletonViewer.SkeletonBuffer);
                            this.AnalysisResultsDisplay.RiskAnalyzer = SkeletonViewer.RiskAnalyzer;

                            // TODO: when we close the results display, the visibility should change for the
                            // depthviewer/skeletonviewer

                            // hide the depthviewer/skeletonviewer and show the analysis results display
                            this.AnalysisResultsDisplay.Visibility = Visibility.Visible;
                            this.DepthViewer.Visibility = Visibility.Hidden;
                        }
                        
                        // Change button content
                        beginButton.Content = "Begin Test";
                    }
                    // set event to handled    
                    e.Handled = true;
                    break;
                
            }
        }
    }
}
