using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace Tests.Unit
{
    public class IODocGenerator
    {
        public string Generate(IEnumerable<ApiDescription> apiDescriptions)
        {
            if (!apiDescriptions.Any()) return JsonConvert.SerializeObject(new { });

            var resources = NewExpandoObject();
            var descriptions =
                apiDescriptions.GroupBy(d => d.ActionDescriptor.ControllerDescriptor.ControllerName)
                    .ToDictionary(g => g.Key, g => g.Select(d => d));

            descriptions.ToList().ForEach(c => resources.Add(c.Key, new { methods = Methods(c.Value) }));

            return JsonConvert.SerializeObject(new { resources });
        }

        private static IDictionary<string, object> Methods(IEnumerable<ApiDescription> actionDescriptions)
        {
            var methods = NewExpandoObject();

            actionDescriptions.Distinct().ToList().ForEach(
                d => methods.Add(d.ActionDescriptor.ActionName,
                    new { path = d.RelativePath, httpMethod = d.HttpMethod.ToString(), description = BuildDocumentation(d) }));

            return methods;
        }

        private static string BuildDocumentation(ApiDescription d)
        {
            return (String.IsNullOrEmpty(d.Documentation))
                ? String.Format("No description provided for {0}.{1}",
                    d.ActionDescriptor.ControllerDescriptor.ControllerName, d.ActionDescriptor.ActionName)
                : d.Documentation;
        }

        private static IDictionary<string, object> NewExpandoObject()
        {
            return new ExpandoObject();
        }
    }
}