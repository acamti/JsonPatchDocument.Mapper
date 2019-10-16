using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Acamti.JsonPatchDocument.Mapper
{
    public static class ServiceCollectionExtension
    {
        private static bool IsMapperType(Type type) => type == typeof(IJsonPatchDocumentMappingSchema);

        public static IServiceCollection AddJsonPatchDocumentMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services.Any(sd => sd.ServiceType == typeof(IJsonPatchDocumentMapper)))
                return services;

            var types = assemblies
                .SelectMany(a => a.GetTypes()).Where(x => x.BaseType != null)
                .Where(t => !t.IsAbstract && t.GetInterfaces().Any(IsMapperType));

            var mapper = new JsonPatchDocumentMapper();

            foreach (Type type in types)
            {
                var obj = (IJsonPatchDocumentMappingSchema)Activator.CreateInstance(type);
                mapper.AddSchema(obj);
            }

            services.AddSingleton<IJsonPatchDocumentMapper>(mapper);

            return services;
        }
    }
}
