using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acamti.JsonPatchDocument.Mapper
{
    public class JsonPatchDocumentMapper : IJsonPatchDocumentMapper
    {
        private readonly List<IJsonPatchDocumentMappingSchema> _mappers;

        public JsonPatchDocumentMapper()
        {
            _mappers = new List<IJsonPatchDocumentMappingSchema>();
        }

        public JsonPatchDocument<TDest> Map<TSource, TDest>(JsonPatchDocument<TSource> sourceDoc)
            where TSource : class
            where TDest : class
        {
            var mapper = _mappers.SingleOrDefault(x => x.MapperSourceType == typeof(TSource) && x.MapperDestinationType == typeof(TDest));

            if (mapper is null)
            {
                throw new Exception($"Attempt in mapping a JsonPatchDocument of '{typeof(TSource).FullName}' to '{typeof(TDest).FullName}' failed because mapper schema does not exists");
            }

            return mapper.Map(sourceDoc) as JsonPatchDocument<TDest>;
        }

        public void AddSchema(IJsonPatchDocumentMappingSchema schema)
        {
            _mappers.Add(schema);
        }
    }
}
