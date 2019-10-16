using Microsoft.AspNetCore.JsonPatch;

namespace Acamti.JsonPatchDocument.Mapper
{
    public interface IJsonPatchDocumentMapper
    {
        public interface IJsonPatchDocumentMapper
        {
            JsonPatchDocument<TDest> Map<TSource, TDest>(JsonPatchDocument<TSource> sourceDoc)
                where TSource : class
                where TDest : class;
        }
    }
}
