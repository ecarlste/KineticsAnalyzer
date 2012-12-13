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
using KinectSkeletonAnalyzer;
using Microsoft.Kinect;
using KinectWpfViewers;

namespace KineticsAnalyzer.Controls
{
    /// <summary>
    /// Interaction logic for AnalyzedSkeletonDisplay.xaml
    /// </summary>
    public partial class AnalyzedSkeletonDisplay : UserControl
    {
        private InjuryRiskAnalyzer riskAnalyzer;
        private Dictionary<JointType, JointMapping> jointMapping;

        public AnalyzedSkeletonDisplay()
        {
            InitializeComponent();
        }

        public InjuryRiskAnalyzer RiskAnalyzer
        {
            get { return riskAnalyzer; }
            set { riskAnalyzer = value; }
        }

        public Dictionary<JointType, JointMapping> JointMapping
        {
            get { return jointMapping; }
            set { jointMapping = value; }
        }

        internal void AddAnalyzedKinectSkeleton()
        {
            AnalyzedKinectSkeleton analyzedSkeleton = new AnalyzedKinectSkeleton();
            analyzedSkeleton.JointMapping = this.JointMapping;
            analyzedSkeleton.InjuryRisks = this.riskAnalyzer.InjuryRisks;
            this.AnalyzedSkeletonCanvasPanel.Children.Add(analyzedSkeleton);

            Canvas canvas = new Canvas();

            double canvasTop = 50.0;
            double canvasRight = 20.0;

            if (riskAnalyzer.InjuryRisks.ContainsKey(JointType.KneeLeft))
            {
                InjuryInfoBox infoBox = new InjuryInfoBox();
                infoBox.DataContext = infoBox;
                infoBox.JointType = "Left Knee";

                switch (riskAnalyzer.InjuryRisks[JointType.KneeLeft])
                {
                    case InjuryRiskType.Low:
                        infoBox.RiskFactor = "Low";
                        break;
                    case InjuryRiskType.Moderate:
                        infoBox.RiskFactor = "Moderate";
                        break;
                    case InjuryRiskType.High:
                        infoBox.RiskFactor = "High";
                        break;
                }

                infoBox.SetValue(Canvas.TopProperty, canvasTop);
                infoBox.SetValue(Canvas.RightProperty, canvasRight);

                canvas.Children.Add(infoBox);

                canvasTop += 100.0;
            }

            if (riskAnalyzer.InjuryRisks.ContainsKey(JointType.KneeRight))
            {
                InjuryInfoBox infoBox = new InjuryInfoBox();
                infoBox.DataContext = infoBox;
                infoBox.JointType = "Right Knee";

                switch (riskAnalyzer.InjuryRisks[JointType.KneeRight])
                {
                    case InjuryRiskType.Low:
                        infoBox.RiskFactor = "Low";
                        break;
                    case InjuryRiskType.Moderate:
                        infoBox.RiskFactor = "Moderate";
                        break;
                    case InjuryRiskType.High:
                        infoBox.RiskFactor = "High";
                        break;
                }

                infoBox.SetValue(Canvas.TopProperty, canvasTop);
                infoBox.SetValue(Canvas.RightProperty, canvasRight);

                canvas.Children.Add(infoBox);
            }

            this.AnalyzedSkeletonCanvasPanel.Children.Add(canvas);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            AnalyzedKinectSkeleton analyzedSkeleton = AnalyzedSkeletonCanvasPanel.Children.OfType<AnalyzedKinectSkeleton>().FirstOrDefault();

            if (analyzedSkeleton != null)
            {
                this.AnalyzedSkeletonCanvasPanel.Children.Remove(analyzedSkeleton);
            }
    
            e.Handled = true;
        }
    }
}
