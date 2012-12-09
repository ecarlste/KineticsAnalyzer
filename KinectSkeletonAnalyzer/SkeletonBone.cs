
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace KinectSkeletonAnalyzer
{
    class SkeletonBone
    {
        private SkeletonBoneType type;
        public SkeletonBoneType Type
        {
            get { return type; }
        }

        private Vector3D vector;
        public Vector3D Vector
        {
            get { return vector; }
        }

        private JointTrackingState trackingState;
        public JointTrackingState TrackingState
        {
            get { return trackingState; }
        }

        public SkeletonBone(SkeletonBoneType type, JointTrackingState trackingState, double x, double y, double z)
        {
            this.type = type;
            this.trackingState = trackingState;

            vector = new Vector3D(x, y, z);
            vector.Normalize();
        }
    }
}
