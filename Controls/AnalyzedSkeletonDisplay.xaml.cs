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
            this.AnalyzedSkeletonCanvasPanel.Children.Add(analyzedSkeleton);
        }
    }
}
