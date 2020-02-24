# Json.Extensions
Provides mapping capability to map a JsonPatchDocument from one type to another

[![Build Status](https://dev.azure.com/acamti/OpenSource/_apis/build/status/Nugets/Nuget-JsonPatchDocment.Mapper?branchName=master)](https://dev.azure.com/acamti/OpenSource/_build/latest?definitionId=2&branchName=master)

`JsonPatchDocument.Mapper` [![NuGet](https://img.shields.io/nuget/v/Acamti.JsonPatchDocument.Mapper.svg)](https://nuget.org/packages/Acamti.JsonPatchDocument.Mapper) [![Nuget](https://img.shields.io/nuget/dt/Acamti.JsonPatchDocument.Mapper.svg)](https://nuget.org/packages/Acamti.JsonPatchDocument.Mapper)


## Usage

### Schema Mapper Definition 
```csharp
    internal class MapperSchema : JsonPatchDocumentMappingSchema<ObjectSource, ObjectTarget>
    {
        public MapperSchema()
        {
            AddRule(s => s.SourcePropString, t => t.TargetPropString);
            AddRule(s => s.SourcePropInt, t => t.ComplexProp.TargetPropInt);
            AddRule(s => s.SourcePropDt, t => t.ComplexProp.TargetPropDt);
        }
    }

    internal class ObjectSource
    {
        public string SourcePropString { get; set; }
        public int SourcePropInt { get; set; }
        public DateTime SourcePropDt { get; set; }
    }

    internal class ObjectTarget
    {
        public string TargetPropString { get; set; }
        public ObjectChild ComplexProp { get; set; }
    }

    internal class ObjectChild
    {
        public int TargetPropInt { get; set; }
        public DateTime TargetPropDt { get; set; }
    }
```

### Mapper Invoke
```csharp
    var mapper = new JsonPatchDocumentMapper();
    var mapperSchema = new MapperSchema();

    mapper.AddSchema(mapperSchema);


    var docSource = new JsonPatchDocument<ObjectSource>();
    docSource.Add(x => x.SourcePropString, "some value");
    docSource.Replace(x => x.SourcePropInt, 123);
    docSource.Replace(x => x.SourcePropDt, DateTime.Now);

    JsonPatchDocument<ObjectTarget> targetDoc = mapper.Map<ObjectSource, ObjectTarget>(docSource);
```

### Dependency Injection
```csharp
    Services.AddJsonPatchDocumentMapper(
        new[] {
                typeof(MapperSchema).Assembly
            }
    );
```

```csharp
    void SomeConstructor(IJsonPatchDocumentMapper mapper)
    {
        JsonPatchDocument<ObjectTarget> targetDoc = mapper.Map<ObjectSource, ObjectTarget>(docSource);
    }
```
