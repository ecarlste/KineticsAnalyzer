
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace KinectSkeletonAnalyzer
{
    public class SkeletonAnalyzer
    {
        Dictionary<TestMeasurementType,double> testMeasurements;

        public SkeletonAnalyzer()
        {
            testMeasurements = new Dictionary<TestMeasurementType, double>();
        }

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
            Vector3D lowerLegLeft = new Vector3D(
                    ankleLeft.Position.X - kneeLeft.Position.X,
                    ankleLeft.Position.Y - kneeLeft.Position.Y,
                    ankleLeft.Position.Z - kneeLeft.Position.Z
                );
            Vector3D upperLegLeft = new Vector3D(
                    hipLeft.Position.X - kneeLeft.Position.X,
                    hipLeft.Position.Y - kneeLeft.Position.Y,
                    hipLeft.Position.Z - kneeLeft.Position.Z
                );

            lowerLegLeft.Normalize();
            upperLegLeft.Normalize();

            testMeasurements[TestMeasurementType.KneeFlexionLeft] = 180.0 - Vector3D.AngleBetween(lowerLegLeft, upperLegLeft);
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

            #region Measure Knee Valgus
            Vector3D hipAsis = new Vector3D(
                    hipLeft.Position.X - hipRight.Position.X,
                    hipLeft.Position.Y - hipRight.Position.Y,
                    hipLeft.Position.Z - hipRight.Position.Z
                );

            hipAsis.Normalize();

            Vector3D up = new Vector3D(0.0, 1.0, 0.0);

            // get the normal vector that represents the plane in which up and
            // hipAsis are contained within, then normalize it
            Vector3D planeNormal = Vector3D.CrossProduct(up, hipAsis);
            planeNormal.Normalize();

            // determine both upper and 
            Vector3D upperLegLeftProjected = upperLegLeft - Vector3D.DotProduct(upperLegLeft, planeNormal) * planeNormal;
            Vector3D lowerLegLeftProjected = lowerLegLeft - Vector3D.DotProduct(lowerLegLeft, planeNormal) * planeNormal;
            testMeasurements[TestMeasurementType.KneeValgusLeft] =
                Vector3D.AngleBetween(upperLegLeftProjected, -lowerLegLeftProjected);

            Vector3D upperLegRightProjected = upperLegRight - Vector3D.DotProduct(upperLegRight, planeNormal) * planeNormal;
            Vector3D lowerLegRightProjected = lowerLegRight - Vector3D.DotProduct(lowerLegRight, planeNormal) * planeNormal;
            testMeasurements[TestMeasurementType.KneeValgusRight] =
                Vector3D.AngleBetween(upperLegRightProjected, -lowerLegRightProjected);
            #endregion
        }
    }
}
