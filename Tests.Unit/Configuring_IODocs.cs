using System.Configuration;
using IODocsNet;
using NUnit.Framework;
using Sahara;

namespace Tests.Unit
{
    [TestFixture]
    public class Configuring_IODocs
    {
        [Test]
        public void The_configuration_section_element_should_be_named_ioDocs()
        {
            var configSection = ConfigurationManager.GetSection("ioDocs") as IODocsConfiguration;

            configSection.ShouldNotBeNull();
            configSection.ShouldEqual(IODocsConfiguration.Settings);
        }

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