using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using IODocsNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var apiExplorer = ApiExplorer(@".\SampleApi.xml");
            var jsonDoc = _ioDocGenerator.Generate(apiExplorer.ApiDescriptions);
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
        public void It_should_describe_all_methods_on_a_resource()
        {
            Assert.IsNotNull(_doc.resources.Address.methods["Get(id)"]);
            Assert.IsNotNull(_doc.resources.Address.methods["Put(id, value)"]);
            Assert.IsNotNull(_doc.resources.Address.methods["Post(value)"]);
            Assert.IsNotNull(_doc.resources.Address.methods["Delete(id)"]);
        }

        [Test]
        public void It_should_describe_overloaded_methods()
        {
            Assert.IsNotNull(_doc.resources.Address.methods["Get()"]);
            Assert.IsNotNull(_doc.resources.Address.methods["Get(id)"]);

            Assert.IsNotNull(_doc.resources.Customer.methods["Get()"]);
            Assert.IsNotNull(_doc.resources.Customer.methods["Get(id, magicNumber)"]);
        }

        [Test]
        public void It_should_describe_all_properties_of_a_method()
        {
            var method = _doc.resources.Customer.methods["Get(id, magicNumber)"];

            Assert.IsNotNull(method);
            Assert.AreEqual("api/Customer/{id}?magicNumber={magicNumber}", method.path.ToString());
            Assert.AreEqual("GET", method.httpMethod.ToString());
            Assert.AreEqual("Retrieve a customer by its id", method.description.ToString());
        }

        [Test]
        public void It_should_include_the_default_message_for_undocumented_methods()
        {
            const string expectedDescription = "No description provided for Customer.Delete";
            Assert.AreEqual(expectedDescription, _doc.resources.Customer.methods["Delete(id)"].description.ToString());
        }

        [Test]
        public void It_should_describe_all_properties_of_a_parameter()
        {
            var parameters = _doc.resources.Customer.methods["Get(id, magicNumber)"].parameters;

            Assert.IsNotNull(parameters.id);
            Assert.AreEqual("The customer's id", parameters.id.description.ToString());
            Assert.AreEqual("0", parameters.id["default"].ToString());
            Assert.AreEqual("true", parameters.id.required.ToString());
            Assert.AreEqual("pathReplace", parameters.id.location.ToString());
        }

        [Test]
        public void It_should_not_describe_enum_properties_if_they_are_not_provided()
        {
            Assert.IsNull(_doc.resources.Customer.methods["Get(id, magicNumber)"].parameters.id.@enum);
            Assert.IsNull(_doc.resources.Customer.methods["Get(id, magicNumber)"].parameters.id.enumDescriptions);
        }

        [Test]
        public void It_should_describe_optional_parameters_as_not_required()
        {
            Assert.IsFalse(bool.Parse(_doc.resources.Customer.methods["Get(id, magicNumber)"].parameters.magicNumber.required.ToString()));
        }

        [Test]
        public void It_should_describe_all_values_of_an_enum_parameter()
        {
            var parameters = _doc.resources.Customer.methods["Put(id, value, status)"].parameters.status;
            var values = ((JArray)parameters.@enum).Select(s => s.ToString());

            Assert.IsTrue(new[] { "Active", "Inactive", "Suspended", "Cancelled" }
                .SequenceEqual(values));
        }

        [Test]
        public void 
            It_should_describe_all_descriptions_of_an_enum_parameter()
        {
            // Going to have to do for now. Not sure what ideal behavior should be.
            var parameters = _doc.resources.Customer.methods["Put(id, value, status)"].parameters.status;
            var descriptions = ((JArray)parameters.enumDescriptions).Select(s => s.ToString());

            Assert.IsTrue(new[] { "Description for Active", "Description for Inactive", "Description for Suspended", "Description for Cancelled" }
                .SequenceEqual(descriptions));
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