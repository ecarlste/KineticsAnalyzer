using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace KinectWpfViewers
{
    class SkeletonAnalyzer
    {
        Dictionary<TestMeasurementType,double> testMeasurements;

        public enum TestMeasurementType
        {
            KneeFlexionLeft,
            KneeFlexionRight,
            HipFlexionLeft,
            HipFlexionRight
        };


        public void analyze(Skeleton skeleton)
        {
            #region Joint Initialization
            Joint spine = skeleton.Joints[JointType.Spine];

            Joint hipCenter = skeleton.Joints[JointType.HipCenter];
            Joint hipLeft = skeleton.Joints[JointType.HipLeft];
            Joint hipRight = skeleton.Joints[JointType.HipRight];

            Joint ankleLeft = skeleton.Joints[JointType.AnkleLeft];
            Joint ankleRight = skeleton.Joints[JointType.AnkleRight];

            Joint kneeLeft = skeleton.Joints[JointType.KneeLeft];
            Joint kneeRight = skeleton.Joints[JointType.KneeRight];
            #endregion

            #region Measure Left Knee Flexion
            Vector3D leftLegLower = new Vector3D(
                    ankleLeft.Position.X - kneeLeft.Position.X,
                    ankleLeft.Position.Y - kneeLeft.Position.Y,
                    ankleLeft.Position.Z - kneeLeft.Position.Z
                );
            Vector3D upperLegLeft = new Vector3D(
                    hipLeft.Position.X - kneeLeft.Position.X,
                    hipLeft.Position.Y - kneeLeft.Position.Y,
                    hipLeft.Position.Z - kneeLeft.Position.Z
                );

            leftLegLower.Normalize();
            upperLegLeft.Normalize();

            testMeasurements[TestMeasurementType.KneeFlexionLeft] = 180.0 - Vector3D.AngleBetween(leftLegLower, upperLegLeft);
            #endregion

            #region Measure Right Knee Flexion
            Vector3D lowerLegRight = new Vector3D(
                    ankleRight.Position.X - kneeRight.Position.X,
                    ankleRight.Position.Y - kneeRight.Position.Y,
                    ankleRight.Position.Z - kneeRight.Position.Z
                );
            Vector3D upperLegRight = new Vector3D(
                    hipRight.Position.X - kneeRight.Position.X,
                    hipRight.Position.Y - kneeRight.Position.Y,
                    hipRight.Position.Z - kneeRight.Position.Z
                );

            lowerLegRight.Normalize();
            upperLegRight.Normalize();

            testMeasurements[TestMeasurementType.KneeFlexionRight] = 180.0 - Vector3D.AngleBetween(lowerLegRight, upperLegRight);
            #endregion

            #region Measure Hip Flexion
            Vector3D backLower = new Vector3D(
                    spine.Position.X - hipCenter.Position.X,
                    spine.Position.Y - hipCenter.Position.Y,
                    spine.Position.Z - hipCenter.Position.Z
                );

            backLower.Normalize();

            testMeasurements[TestMeasurementType.HipFlexionLeft] = 180.0 - Vector3D.AngleBetween(backLower, upperLegLeft);
            testMeasurements[TestMeasurementType.HipFlexionRight] = 180.0 - Vector3D.AngleBetween(backLower, upperLegRight);
            #endregion
        }
    }
}
