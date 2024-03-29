﻿
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Kinect;

namespace KinectSkeletonAnalyzer
{
    public class JointColorizer
    {
        private static readonly Dictionary<InjuryRiskType, Color> injuryRiskColors = new Dictionary<InjuryRiskType, Color>()
        {
            { InjuryRiskType.None, Color.FromRgb(0, 255, 0) },
            { InjuryRiskType.Low, Color.FromRgb(0, 255, 0) },
            { InjuryRiskType.Moderate, Color.FromRgb(0, 255, 0) },
            { InjuryRiskType.High, Color.FromRgb(0, 255, 0) }
        };

        public static Dictionary<JointType, Color> getJointColors(Dictionary<JointType, InjuryRiskType> jointRisks)
        {
            Dictionary<JointType, Color> jointColors = new Dictionary<JointType, Color>();

            foreach (JointType jointType in jointRisks.Keys)
            {
                jointColors[jointType] = injuryRiskColors[jointRisks[jointType]];
            }

            return jointColors;
        }
    }
}
