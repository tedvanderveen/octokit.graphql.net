﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Octokit.GraphQL.Core
{
    public class PagedSubquery<TResult> : PagedQuery<TResult>, ISubquery
    {
        public PagedSubquery(
            SimpleQuery<TResult> masterQuery,
            IEnumerable<ISubquery> subqueries,
            Expression<Func<JObject, JToken>> pageInfo,
            Expression<Func<JObject, JToken>> parentPageInfo)
            : base(masterQuery, subqueries)
        {
            PageInfo = pageInfo.Compile();
            ParentPageInfo = parentPageInfo.Compile();
        }

        public Func<JObject, JToken> ParentId { get; }

        public Func<JObject, JToken> PageInfo { get; }

        public Func<JObject, JToken> ParentPageInfo { get; }

        public IQueryRunner Start(
            IConnection connection,
            string id,
            string after,
            IDictionary<string, object> variables)
        {
            throw new NotImplementedException();
        }

        public static ISubquery Create(
            Type resultType,
            ICompiledQuery masterQuery,
            IEnumerable<ISubquery> subqueries,
            Expression<Func<JObject, JToken>> pageInfo,
            Expression<Func<JObject, JToken>> parentPageInfo)
        {
            var ctor = typeof(PagedSubquery<>)
                .MakeGenericType(resultType)
                .GetTypeInfo()
                .DeclaredConstructors
                .Single();

            return (ISubquery)ctor.Invoke(new object[]
            {
                masterQuery,
                subqueries,
                pageInfo,
                parentPageInfo
            });
        }
    }
}