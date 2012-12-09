
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using System;

namespace KinectSkeletonAnalyzer
{
    public class SkeletonMeasurer
    {
        private List<TestMeasurement> testMeasurements;
        public List<TestMeasurement> TestMeasurements
        {
            get { return testMeasurements; }
        }

        private Dictionary<SkeletonBoneType, Vector3D> bones;

        private Skeleton skeleton;
        public Skeleton Skeleton
        {
            get { return skeleton; }
            set { skeleton = value; }
        }
        
        public SkeletonMeasurer(Skeleton skeleton)
        {
            testMeasurements = new List<TestMeasurement>();
            bones = new Dictionary<SkeletonBoneType, Vector3D>();

            this.skeleton = skeleton;
        }

        public void determineMeasurements()
        {
            measureKneeFlexion();

            measureHipFlexion();

            measureKneeValgus();
        }

        private void measureKneeFlexion()
        {
            Vector3D lowerLegLeft = getBone(SkeletonBoneType.LowerLegLeft);
            Vector3D upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            Vector3D lowerLegRight = getBone(SkeletonBoneType.LowerLegRight);
            Vector3D upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionLeft,
                180.0 - Vector3D.AngleBetween(lowerLegLeft, -upperLegLeft)));
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionRight,
                180.0 - Vector3D.AngleBetween(lowerLegRight, -upperLegRight)));
        }

        private void measureHipFlexion()
        {
            Vector3D backLower = getBone(SkeletonBoneType.BackLower);
            Vector3D upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            Vector3D upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.HipFlexionLeft,
                180.0 - Vector3D.AngleBetween(backLower, upperLegLeft)));
            testMeasurements.Add( new TestMeasurement(TestMeasurementType.HipFlexionRight,
                180.0 - Vector3D.AngleBetween(backLower, upperLegRight)));
        }

        private void measureKneeValgus()
        {
            Vector3D lowerLegLeft = getBone(SkeletonBoneType.LowerLegLeft);
            Vector3D upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            Vector3D lowerLegRight = getBone(SkeletonBoneType.LowerLegRight);
            Vector3D upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            SkeletonPoint hipLeft = skeleton.Joints[JointType.HipLeft].Position;
            SkeletonPoint hipRight = skeleton.Joints[JointType.HipRight].Position;

            Vector3D hipAsis = new Vector3D(
                   hipLeft.X - hipRight.X,
                   hipLeft.Y - hipRight.Y,
                   hipLeft.Z - hipRight.Z
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
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusLeft,
                Vector3D.AngleBetween(upperLegLeftProjected, lowerLegLeftProjected)));

            Vector3D upperLegRightProjected = upperLegRight - Vector3D.DotProduct(upperLegRight, planeNormal) * planeNormal;
            Vector3D lowerLegRightProjected = lowerLegRight - Vector3D.DotProduct(lowerLegRight, planeNormal) * planeNormal;
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusRight,
                Vector3D.AngleBetween(upperLegRightProjected, lowerLegRightProjected)));
        }

        private Vector3D getBone(SkeletonBoneType boneType)
        {
            if (!bones.ContainsKey(boneType))
            {
                SkeletonPoint p1;
                SkeletonPoint p2;

                switch (boneType)
                {
                    case SkeletonBoneType.LowerLegLeft:
                        p1 = skeleton.Joints[JointType.KneeLeft].Position;
                        p2 = skeleton.Joints[JointType.AnkleLeft].Position;
                        break;
                    case SkeletonBoneType.LowerLegRight:
                        p1 = skeleton.Joints[JointType.KneeRight].Position;
                        p2 = skeleton.Joints[JointType.AnkleRight].Position;
                        break;
                    case SkeletonBoneType.UpperLegLeft:
                        p1 = skeleton.Joints[JointType.HipLeft].Position;
                        p2 = skeleton.Joints[JointType.KneeLeft].Position;
                        break;
                    case SkeletonBoneType.UpperLegRight:
                        p1 = skeleton.Joints[JointType.HipRight].Position;
                        p2 = skeleton.Joints[JointType.KneeRight].Position;
                        break;
                    case SkeletonBoneType.BackLower:
                        p1 = skeleton.Joints[JointType.HipCenter].Position;
                        p2 = skeleton.Joints[JointType.Spine].Position;
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

                Vector3D bone = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Y);
                bone.Normalize();
                bones[boneType] = bone;
                
                return bone;
            }

            return bones[boneType];
        }
    }
}
