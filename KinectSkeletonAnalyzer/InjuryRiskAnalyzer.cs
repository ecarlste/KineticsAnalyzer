
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;

namespace KinectSkeletonAnalyzer
{
    public class InjuryRiskAnalyzer : INotifyPropertyChanged
    {
        private readonly Dictionary<TestMeasurementType, JointType> riskJoints = new Dictionary<TestMeasurementType, JointType>()
        {
            { TestMeasurementType.HipFlexionLeft, JointType.KneeLeft },
            { TestMeasurementType.HipFlexionRight, JointType.KneeRight },
            { TestMeasurementType.KneeFlexionLeft, JointType.KneeLeft },
            { TestMeasurementType.KneeFlexionRight, JointType.KneeRight },
            { TestMeasurementType.KneeValgusLeft, JointType.KneeLeft },
            { TestMeasurementType.KneeValgusRight, JointType.KneeRight }
        };

        private double progressValue;
        private Dictionary<TestMeasurementType, List<double>> testMeasurementBuffer;
        private Dictionary<JointType, InjuryRiskType> injuryRisks;

        public event PropertyChangedEventHandler PropertyChanged;

        public InjuryRiskAnalyzer()
        {
            ProgressValue = 0.0;
        }

        public InjuryRiskAnalyzer(Dictionary<TestMeasurementType, List<double>> testMeasurementBuffer)
            : this()
        {
            this.testMeasurementBuffer = testMeasurementBuffer;
        }

        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        public Dictionary<TestMeasurementType, List<double>> TestMeasurementBuffer
        {
            get { return testMeasurementBuffer; }
            set { testMeasurementBuffer = value; }
        }
        
        public Dictionary<JointType, InjuryRiskType> InjuryRisks
        {
            get { return injuryRisks; }
        }

        private void AddInjuryRisk(TestMeasurementType type, InjuryRiskType injuryRisk)
        {
            JointType jointAtRisk = riskJoints[type];

            // first check to see if the key "type" exists, if it does then we need to check to see if the new
            // injury risk level is higher than the current one. If it's not, then we don't need to do anything at all.
            if (injuryRisks.ContainsKey(jointAtRisk) && injuryRisks[jointAtRisk] >= injuryRisk)
            {
                return;
            }

            // otherwise we need to update or create the injuryRisk entry in the dictionary
            injuryRisks[jointAtRisk] = injuryRisk;
        }

        public void Analyze()
        {
            ProgressValue = 0.0;

            injuryRisks = new Dictionary<JointType, InjuryRiskType>();

            foreach (TestMeasurementType type in testMeasurementBuffer.Keys)
            {
                DetermineInjuryRisk(type);
            }

            ProgressValue = 1.0;
        }

        private void DetermineInjuryRisk(TestMeasurementType type)
        {
            switch (type)
            {
                case TestMeasurementType.HipFlexionLeft:
                case TestMeasurementType.HipFlexionRight:
                    DetermineRiskFromHipFlexion(type);
                    break;
                case TestMeasurementType.KneeFlexionLeft:
                case TestMeasurementType.KneeFlexionRight:
                    DetermineRiskFromKneeFlexion(type);
                    break;
                case TestMeasurementType.KneeValgusLeft:
                case TestMeasurementType.KneeValgusRight:
                    DetermineRiskFromKneeValgus(type);
                    break;
                default:
                    break;
            }
        }

        private void DetermineRiskFromKneeValgus(TestMeasurementType type)
        {
            double maxHipFlexion = testMeasurementBuffer[type].Max();
            InjuryRiskType injuryRisk;

            injuryRisk = InjuryRiskType.None;

            AddInjuryRisk(type, injuryRisk);
        }

        private void DetermineRiskFromKneeFlexion(TestMeasurementType type)
        {
            double maxKneeFlexion = testMeasurementBuffer[type].Max();
            InjuryRiskType injuryRisk;

            if (maxKneeFlexion < 80.0 || maxKneeFlexion > 99.0)
            {
                injuryRisk = InjuryRiskType.High;
            }
            else if (maxKneeFlexion <= 95.0)
            {
                injuryRisk = InjuryRiskType.Low;
            }
            else
            {
                injuryRisk = InjuryRiskType.Moderate;
            }

            AddInjuryRisk(type, injuryRisk);
        }

        private void DetermineRiskFromHipFlexion(TestMeasurementType type)
        {
            double maxHipFlexion = testMeasurementBuffer[type].Max();
            InjuryRiskType injuryRisk;

            if (maxHipFlexion < 55.0 || maxHipFlexion > 120.0)
            {
                injuryRisk = InjuryRiskType.High;
            }
            else if (maxHipFlexion >= 90.0)
            {
                injuryRisk = InjuryRiskType.Low;
            }
            else
            {
                injuryRisk = InjuryRiskType.Moderate;
            }

            AddInjuryRisk(type, injuryRisk);
        }

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
