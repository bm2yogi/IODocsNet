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
        private readonly IConfigurationSettings _configSettings;
        
        public IODocGenerator(IConfigurationSettings configSettings)
        {
            _configSettings = configSettings;
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
                    .GroupBy(ControllerName)
                    .ToDictionary(g => g.Key, g => g.Select(d => d))
                    .ToList()
                    .ForEach(c => resources.Add(c.Key, new { methods = Methods(c.Value) }));
            }

            return resources;
        }

        private static string ControllerName(ApiDescription d)
        {
            return d.ActionDescriptor.ControllerDescriptor.ControllerName;
        }

        private static IDictionary<string, object> Methods(IEnumerable<ApiDescription> actionDescriptions)
        {
            var methods = NewExpandoObject();

            actionDescriptions.Distinct().ToList().ForEach(
                d => methods.Add(ActionName(d),
                    BuildMethod(d)));

            return methods;
        }

        private static string ActionName(ApiDescription d)
        {
            var actionName = d.ActionDescriptor.ActionName;
            var args = string.Join(", ", d.ActionDescriptor.GetParameters().Select(p => p.ParameterName));
            return string.Format("{0}({1})", actionName, args);
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
            var httpActionDescriptor = apiDescription.ActionDescriptor;

            return string.IsNullOrEmpty(apiDescription.Documentation)
                ? string.Format("No description provided for {0}.{1}",
                    httpActionDescriptor.ControllerDescriptor.ControllerName,
                    httpActionDescriptor.ActionName)
                : apiDescription.Documentation;
        }

        private static string ParameterDescription(ApiParameterDescription parameterDescription)
        {
            var httpActionDescriptor = parameterDescription.ParameterDescriptor.ActionDescriptor;

            return string.IsNullOrEmpty(parameterDescription.Documentation)
                ? String.Format("No description provided for \"{0}\" parameter of {1}.{2}",
                    parameterDescription.Name,
                    httpActionDescriptor.ControllerDescriptor.ControllerName,
                    httpActionDescriptor.ActionName)
                : parameterDescription.Documentation;
        }

        private static object BuildParameter(ApiParameterDescription p)
        {
            var parameter = NewExpandoObject();

            parameter.Add("description", ParameterDescription(p));
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
                .Select(v=>v.ToString(CultureInfo.InvariantCulture));
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