using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace IODocsNet
{
    public class IODocGenerator
    {
        private readonly IODocsConfiguration _configSettings;

        public IODocGenerator()
        {
            _configSettings = IODocsConfiguration.Settings;
        }

        public string Generate(IEnumerable<ApiDescription> apiDescriptions)
        {
            var resources = BuildResources(apiDescriptions);

            var doc = new
            {
                name = _configSettings.Name,
                version = _configSettings.ApiVersion,
                title = _configSettings.Title,
                description = _configSettings.Description,
                protocol = _configSettings.Protocol,
                basePath = _configSettings.BasePath,
                resources
            };

            return JsonConvert.SerializeObject(doc);
        }

        private static IDictionary<string, object> BuildResources(IEnumerable<ApiDescription> apiDescriptions)
        {
            var resources = NewExpandoObject();

            if (apiDescriptions.Any())
            {
                apiDescriptions
                    .GroupBy(d => d.ActionDescriptor.ControllerDescriptor.ControllerName)
                    .ToDictionary(g => g.Key, g => g.Select(d => d))
                    .ToList()
                    .ForEach(c => resources.Add(c.Key, new { methods = Methods(c.Value) }));
            }

            return resources;
        }

        private static IDictionary<string, object> Methods(IEnumerable<ApiDescription> actionDescriptions)
        {
            var methods = NewExpandoObject();

            actionDescriptions.Distinct().ToList().ForEach(
                d => methods.Add(d.ActionDescriptor.ActionName,
                    BuildMethod(d)));

            return methods;
        }

        private static object BuildMethod(ApiDescription apiDescription)
        {
            var parameters = NewExpandoObject();
            apiDescription.ParameterDescriptions.ToList().ForEach(
                p => parameters.Add(p.Name,
                    BuildParameter(p)));

            return new
            {
                path = apiDescription.RelativePath,
                httpMethod = apiDescription.HttpMethod.ToString(),
                description = MethodDescription(apiDescription),
                parameters
            };
        }

        private static string MethodDescription(ApiDescription apiDescription)
        {
            return (String.IsNullOrEmpty(apiDescription.Documentation))
                ? String.Format("No description provided for {0}.{1}",
                    apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName,
                    apiDescription.ActionDescriptor.ActionName)
                : apiDescription.Documentation;
        }

        private static object BuildParameter(ApiParameterDescription p)
        {
            var parameter = NewExpandoObject();

            parameter.Add("description", p.Documentation);
            parameter.Add("default", DefaultValue(p));
            parameter.Add("location", ParameterLocation(p));
            parameter.Add("required", IsRequired(p));

            AddEnumValues(parameter, p);

            return parameter;
        }

        private static void AddEnumValues(IDictionary<string, object> parameter, ApiParameterDescription parameterDescription)
        {
            if (!parameterDescription.ParameterDescriptor.ParameterType.IsEnum) return;

            parameter.Add("enum", EnumValues(parameterDescription));
            parameter.Add("enumDescriptions", EnumDescriptions(parameterDescription));
        }

        private static object EnumDescriptions(ApiParameterDescription parameterDescription)
        {
            return Enum.GetNames(parameterDescription.ParameterDescriptor.ParameterType)
                .Select(v => string.Format(CultureInfo.InvariantCulture, "Description for {0}", v));
        }

        private static object EnumValues(ApiParameterDescription parameterDescription)
        {
            return Enum.GetNames(parameterDescription.ParameterDescriptor.ParameterType);
        }

        private static string DefaultValue(ApiParameterDescription parameterDescription)
        {
            return 0.ToString(CultureInfo.InvariantCulture); // parameterDescription.ParameterDescriptor.DefaultValue
        }

        private static string IsRequired(ApiParameterDescription parameterDescription)
        {
            return parameterDescription.ParameterDescriptor.IsOptional ? "false" : "true";
        }

        private static string ParameterLocation(ApiParameterDescription parameterDescription)
        {
            return new Dictionary<ApiParameterSource, string>
            {
                {ApiParameterSource.Unknown, "query"},
                {ApiParameterSource.FromUri, "pathReplace"},
                {ApiParameterSource.FromBody, "body"},
                //{"", "header"},
            
            }[parameterDescription.Source];
        }

        private static IDictionary<string, object> NewExpandoObject()
        {
            return new ExpandoObject();
        }
    }
}