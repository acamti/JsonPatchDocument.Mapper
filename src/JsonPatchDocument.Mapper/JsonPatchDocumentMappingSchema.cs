using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Acamti.JsonPatchDocument.Mapper
{
    public abstract class JsonPatchDocumentMappingSchema<TSource, TDest> : IJsonPatchDocumentMappingSchema
        where TSource : class
        where TDest : class
    {
        private readonly Dictionary<string, string> _mappingRules;

        protected JsonPatchDocumentMappingSchema()
        {
            _mappingRules = new Dictionary<string, string>();
        }

        public Type MapperSourceType { get; } = typeof(TSource);
        public Type MapperDestinationType { get; } = typeof(TDest);

        public IJsonPatchDocument Map(IJsonPatchDocument sourceDoc)
        {
            var result = new JsonPatchDocument<TDest>();

            sourceDoc.GetOperations().ToList().ForEach(
                x =>
                {
                    if (NeedsToBeMap(x.path))
                    {
                        result.Operations.Add(
                            new Operation<TDest>(
                                x.op,
                                GetTargetMappingPath(x.path),
                                x.from,
                                x.value
                            )
                        );
                    }
                }
            );

            return result;
        }

        public void AddRule<TSourceProp, TTargetProp>(Expression<Func<TSource, TSourceProp>> sourceProp, Expression<Func<TDest, TTargetProp>> targetProp)
        {
            _mappingRules.Add(GetPath(sourceProp).ToLowerInvariant(), GetPath(targetProp));
        }

        private string GetTargetMappingPath(string sourcePath)
        {
            if (_mappingRules.TryGetValue(sourcePath.ToLowerInvariant(), out var result))
            {
                return result;
            }

            throw new Exception($"Key {sourcePath} does not exists");
        }

        private bool NeedsToBeMap(string path)
        {
            return _mappingRules.ContainsKey(path.ToLowerInvariant());
        }

        private string GetPath<TModel, TProp>(Expression<Func<TModel, TProp>> expr)
        {
            var segments = GetPathSegments(expr.Body);
            var path = string.Join("/", segments);

            return "/" + path;
        }

        private static IEnumerable<string> GetPathSegments(Expression expr)
        {
            var listOfSegments = new List<string>();

            switch (expr.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    var binaryExpression = (BinaryExpression)expr;
                    listOfSegments.AddRange(GetPathSegments(binaryExpression.Left));
                    listOfSegments.Add(binaryExpression.Right.ToString());
                    return listOfSegments;

                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression)expr;
                    listOfSegments.AddRange(GetPathSegments(methodCallExpression.Object));
                    listOfSegments.Add(EvaluateExpression(methodCallExpression.Arguments[0]));
                    return listOfSegments;

                case ExpressionType.Convert:
                    listOfSegments.AddRange(GetPathSegments(((UnaryExpression)expr).Operand));
                    return listOfSegments;

                case ExpressionType.MemberAccess:
                    var memberExpression = expr as MemberExpression;
                    listOfSegments.AddRange(GetPathSegments(memberExpression.Expression));

                    // Get property name, respecting JsonProperty attribute
                    listOfSegments.Add(GetPropertyNameFromMemberExpression(memberExpression));
                    return listOfSegments;

                case ExpressionType.Parameter:
                    return listOfSegments;

                default:
                    throw new InvalidOperationException("Not supported format expression");
            }
        }

        private static string GetPropertyNameFromMemberExpression(MemberExpression memberExpression)
        {
            var contractResolver = new DefaultContractResolver();

            var jsonObjectContract = contractResolver.ResolveContract(memberExpression.Expression.Type) as JsonObjectContract;

            return jsonObjectContract?.Properties
                .First(jsonProperty => string.Equals(jsonProperty.UnderlyingName, memberExpression.Member.Name, StringComparison.InvariantCultureIgnoreCase)).PropertyName;
        }

        private static string EvaluateExpression(Expression expression)
        {
            var converted = Expression.Convert(expression, typeof(object));
            var fakeParameter = Expression.Parameter(typeof(object), null);
            var lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
            var func = lambda.Compile();

            return Convert.ToString(func(null), CultureInfo.InvariantCulture);
        }
    }
}
