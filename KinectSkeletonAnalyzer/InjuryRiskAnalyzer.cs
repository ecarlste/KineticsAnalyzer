using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectSkeletonAnalyzer
{
    public static class InjuryRiskAnalyzer
    {
        public static Dictionary<JointType, InjuryRiskType> Analyze(Dictionary<TestMeasurementType, List<double>> testMeasurementBuffer)
        {
            Dictionary<JointType, InjuryRiskType> injuryRisks = new Dictionary<JointType, InjuryRiskType>();

            return injuryRisks;
        }
    }
}
