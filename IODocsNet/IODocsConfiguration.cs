using System.Configuration;

namespace IODocsNet
{
    public class IODocsConfiguration : ConfigurationSection
    {
        private const string SectionName = "ioDocs";
        private const string ApiNameKey = "name";
        private const string ApiVersionKey = "version";
        private const string ApiTitleKey = "title";
        private const string ApiDescriptionKey = "description";
        private const string ApiBasePathKey = "basePath";
        private const string ApiProtocolKey = "protocol";

        public static IODocsConfiguration Settings
        {
            get { return ConfigurationManager.GetSection(SectionName) as IODocsConfiguration; }
        }

        [ConfigurationProperty(ApiNameKey, IsRequired = true)]
        public string Name
        {
            get { return (string)this[ApiNameKey]; }
        }

        [ConfigurationProperty(ApiVersionKey, IsRequired = true)]
        public string ApiVersion
        {
            get { return (string)this[ApiVersionKey]; }
        }

        [ConfigurationProperty(ApiTitleKey, IsRequired = true)]
        public string Title
        {
            get { return (string)this[ApiTitleKey]; }
        }

        [ConfigurationProperty(ApiDescriptionKey, IsRequired = false, DefaultValue = "")]
        public string Description
        {
            get { return (string)this[ApiDescriptionKey]; }
        }

        [ConfigurationProperty(ApiBasePathKey, IsRequired = true)]
        public string BasePath
        {
            get { return (string)this[ApiBasePathKey]; }
        }

        [ConfigurationProperty(ApiProtocolKey, IsRequired = false, DefaultValue = "rest")]
        public string Protocol
        {
            get { return (string)this[ApiProtocolKey]; }
        }
    }
}