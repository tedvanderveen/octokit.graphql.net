﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Octokit.GraphQL.Core.Utilities
{
    public static class ExpressionMethods
    {
        public static readonly MethodInfo ChildrenOfTypeMethod = typeof(ExpressionMethods).GetTypeInfo().GetDeclaredMethod(nameof(ChildrenOfType));
        public static readonly MethodInfo FirstOrDefaultMethod = typeof(ExpressionMethods).GetTypeInfo().GetDeclaredMethod(nameof(FirstOrDefault));
        public static readonly MethodInfo JTokenIndexer = typeof(JToken).GetTypeInfo().GetDeclaredMethod("get_Item");
        public static readonly MethodInfo JTokenToObject = GetMethod(typeof(JToken), nameof(JToken.ToObject), new Type[0]);
        public static readonly MethodInfo SelectMethod = typeof(ExpressionMethods).GetTypeInfo().GetDeclaredMethod(nameof(Select));
        public static readonly MethodInfo SelectEntityMethod = typeof(ExpressionMethods).GetTypeInfo().GetDeclaredMethod(nameof(SelectEntity));

        public static IQueryable<JToken> ChildrenOfType(IEnumerable<JToken> source, string typeName)
        {
            if (source is JToken token && token.Type != JTokenType.Array)
            {
                return Enumerable.Repeat(
                    token,
                    (string)token["__typename"] == typeName ? 1 : 0).AsQueryable();
            }
            else
            {
                return source.Where(x => (string)x["__typename"] == typeName).AsQueryable();
            }
        }

        public static T FirstOrDefault<T>(IQueryable<JToken> tokens, Func<JToken, T> selector)
        {
            return tokens.Select(selector).FirstOrDefault();
        }

        public static IQueryable<T> Select<T>(IQueryable<JToken> tokens, Func<JToken, T> selector)
        {
            return tokens.Select(selector).AsQueryable();
        }

        public static IQueryable<T> SelectEntity<T>(JToken token, Func<JToken, T> selector)
        {
            if (token.Type == JTokenType.Array)
            {
                return token.Select(selector).AsQueryable();
            }
            else if (token.Type == JTokenType.Null)
            {
                return Enumerable.Empty<T>().AsQueryable();
            }
            else
            {
                return Enumerable.Repeat(selector(token), 1).AsQueryable();
            }
        }

        public static IQueryable<T> SelectMany<T>(JToken token, Func<JToken, IQueryable<T>> selector)
        {
            return selector(token);
        }

        static MethodInfo GetMethod(Type type, string name, params Type[] parameters)
        {
            return type.GetTypeInfo().GetDeclaredMethods(name)
                .First(x => Enumerable.Select(x.GetParameters(), y => y.ParameterType).SequenceEqual(parameters));
        }
    }
}