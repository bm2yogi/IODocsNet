using IODocsNet;
using NUnit.Framework;
using Sahara;

namespace Tests.Integration
{
    [TestFixture]
    public class Configuring_IODocs
    {
        [Test]
        public void It_should_read_configuration_data_from_the_configsection()
        {
            var settings = IODocsConfiguration.Settings;

            settings.ShouldNotBeNull();
            settings.Name.ShouldEqual("SampleApi");
            settings.ApiVersion.ShouldEqual("1.0.0");
            settings.BasePath.ShouldEqual("https://api.sampleapp.mashery.com:443");
            settings.Description.ShouldEqual("Sample Api Description");
            settings.Protocol.ShouldEqual("rest");
            settings.Title.ShouldEqual("My Sample API (V1)");
        }
    }
}