namespace Portalia.Core.Enum
{
    public class AmarisEnum
    {
        public enum Holding
        {
            Amaris = 1,
            Portalia = 13
        }

        public enum DocumentType : byte
        {
            Resume = 1,
            CoverLetter = 2,
            Degree = 3,
            Certification = 4
        }

        public enum SourcingStatus
        {
            Created = 1,
            Pending = 2,
            WebsiteError = 3,
            ToastError = 4,
            InProgress = 5
        }

        public enum ApplicationSource
        {
            AmarisCareerWebsite = 1,
            NextjobsWebsite = 2,
            AceAndPerryWebsite = 3,
            PortaliaWebsite = 4
        }
    }
}
