using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using System;
using System.Text;

namespace Acamti.JsonPatchDocument.Mapper.Tests
{
    [TestClass]
    public class Describe_JsonPatchDocumentMapper
    {
        private readonly JsonPatchDocumentMapper _mapper;
        private readonly AutoMocker _mocker;

        public Describe_JsonPatchDocumentMapper()
        {
            _mocker = new AutoMocker();
            _mapper = new JsonPatchDocumentMapper();
        }

        [TestMethod]
        public void It_CallMap()
        {
            var schema = _mocker.GetMock<IJsonPatchDocumentMappingSchema>();

            schema.Setup(s => s.MapperSourceType).Returns(typeof(string));
            schema.Setup(s => s.MapperDestinationType).Returns(typeof(StringBuilder));

            var sourceDoc = new JsonPatchDocument<string>();

            _mapper.AddSchema(schema.Object);

            var result = _mapper.Map<string, StringBuilder>(sourceDoc);

            schema.Verify(v => v.Map(sourceDoc), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void It_ThrowsException_WhenNoSchema()
        {
            var sourceDoc = new JsonPatchDocument<string>();

            _mapper.Map<string, StringBuilder>(sourceDoc);
        }
    }
}
