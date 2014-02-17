using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using NUnit.Framework;
using SampleApi;
using SampleApi.Areas.HelpPage;

namespace Tests.Unit
{
    [TestFixture]
    public class GeneratingIoDocs
    {
        private dynamic _doc;
        private readonly IODocGenerator _ioDocGenerator = new IODocGenerator();

        [TestFixtureSetUp]
        public void When_generating_an_iodoc()
        {
            IApiExplorer apiExplorer = ApiExplorer(@".\SampleApi.xml");
            var jsonDoc = _ioDocGenerator.Generate(apiExplorer.ApiDescriptions);
            Console.WriteLine(jsonDoc);

            _doc = JsonConvert.DeserializeObject(jsonDoc);
        }

        [Test]
        public void It_should_generate_an_iodoc()
        {
            Assert.IsNotNull(_doc);
        }

        [Test]
        public void It_should_describe_all_defined_resources()
        {
            Assert.IsNotNull(_doc.resources.Address);
            Assert.IsNotNull(_doc.resources.Customer);
        }

        [Test]
        public void It_should_describe_all_actions_on_a_resource()
        {
            Assert.IsNotNull(_doc.resources.Address.methods.Get);
        }

        [Test]
        public void It_should_include_the_default_message_for_undocumented_methods()
        {
            const string expectedDescription = "No description provided for Customer.Delete";
            Assert.AreEqual(expectedDescription, _doc.resources.Customer.methods.Delete.description.ToString());
        }

        [Test]
        public void It_should_include_the_xml_documentation()
        {
            const string expectedDescription = "Retrieve a customer by its id";
            Assert.AreEqual(expectedDescription, _doc.resources.Customer.methods.Get.description.ToString());
        }

        private IApiExplorer ApiExplorer(string documentationPath)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.SetDocumentationProvider(
                new XmlDocumentationProvider(documentationPath));

            WebApiConfig.Register(httpConfiguration);

            return new ApiExplorer(httpConfiguration);
        }
    }
}