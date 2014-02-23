using System.Web.Http.Description;
using IODocsNet;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Unit
{
    [TestFixture]
    public class Generating_IODoc_BasicInfo
    {
        private dynamic _doc;
        private IODocGenerator _ioDocGenerator;

        [TestFixtureSetUp]
        public void When_generating_an_iodoc()
        {
            var configSettings = IODocsConfiguration.Settings;

            _ioDocGenerator = new IODocGenerator();
            var jsonDoc = _ioDocGenerator.Generate(new ApiDescription[] { });
            _doc = JsonConvert.DeserializeObject(jsonDoc);
        }

        [Test]
        public void It_should_include_basic_Api_information()
        {
            Assert.AreEqual("SampleApi", _doc.name.ToString());
            Assert.AreEqual("1.0.0", _doc.version.ToString());
            Assert.AreEqual("My Sample API (V1)", _doc.title.ToString());
            Assert.AreEqual("Sample Api Description", _doc.description.ToString());
            Assert.AreEqual("rest", _doc.protocol.ToString());
            Assert.AreEqual("https://api.sampleapp.mashery.com:443", _doc.basePath.ToString());
        }
    }
}