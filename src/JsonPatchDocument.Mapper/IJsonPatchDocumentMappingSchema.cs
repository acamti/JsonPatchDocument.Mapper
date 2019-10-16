using Microsoft.AspNetCore.JsonPatch;
using System;

namespace Acamti.JsonPatchDocument.Mapper
{
    public interface IJsonPatchDocumentMappingSchema
    {
        Type MapperSourceType { get; }
        Type MapperDestinationType { get; }
        IJsonPatchDocument Map(IJsonPatchDocument sourceDoc);
    }
}
