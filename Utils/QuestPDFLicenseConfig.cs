using QuestPDF.Infrastructure;

namespace travel_agency_back.Utils
{
    public static class QuestPDFLicenseConfig
    {
        public static void ConfigureLicense()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}
