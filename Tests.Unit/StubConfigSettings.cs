using IODocsNet;

namespace Tests.Unit
{
    internal class StubConfigSettings : IConfigurationSettings
    {
        public string Name { get { return "SampleApi"; } }
        public string ApiVersion { get { return "1.0.0"; } }
        public string Title { get { return "My Sample API (V1)"; } }
        public string Description { get { return "Sample Api Description"; } }
        public string Protocol { get { return "rest"; } }
        public string BasePath { get { return "https://api.sampleapp.mashery.com:443"; } }
    }
}