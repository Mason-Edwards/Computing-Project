using Dashboard.Client.Enums;
using Dashboard.Shared.GrpcProto;

namespace Dashboard.Client.Adapters
{
    public static class RecordingStatusAdapter
    {
        public static RecordingStatus ToRecordingStatus(this GrpcRecordingStatus status)
        {
            switch(status)
            {
                case GrpcRecordingStatus.NotRecoding: return RecordingStatus.NotRecoding;
                case GrpcRecordingStatus.Recording: return RecordingStatus.Recording;
                default: throw new ArgumentException("Invalid argument");
            }
        }

        public static GrpcRecordingStatus ToGrpcRecordingStatus(this RecordingStatus status)
        {
            switch (status)
            {
                case RecordingStatus.NotRecoding: return GrpcRecordingStatus.NotRecoding;
                case RecordingStatus.Recording: return GrpcRecordingStatus.Recording;
                default: throw new ArgumentException("Invalid argument");
            }
        }
    }
}
