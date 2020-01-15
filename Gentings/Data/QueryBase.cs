﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gentings.Extensions;

namespace Gentings.Data
{
    /// <summary>
    /// 查询基类。
    /// </summary>
    /// <typeparam name="TModel">数据库实体类型。</typeparam>
    /// <typeparam name="TObject">返回的实体模型类型。</typeparam>
    public abstract class QueryBase<TModel, TObject> : IPageEnumerable<TObject>
    {
        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected internal abstract void Init(IQueryContext<TModel> context);

        private int _page;
        /// <summary>
        /// 页码。
        /// </summary>
        public int Page
        {
            get
            {
                if (_page < 1)
                    _page = 1;
                return _page;
            }
            set => _page = Math.Max(1, value);
        }

        /// <summary>
        /// 每页显示记录数。
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 总记录数。
        /// </summary>
        public int Size => Models?.Size ?? 0;

        /// <summary>
        /// 总页数。
        /// </summary>
        public int Pages => Models?.Pages ?? 0;

        internal IPageEnumerable<TObject> Models { private get; set; }

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<TObject> GetEnumerator()
        {
            return (Models ?? Enumerable.Empty<TObject>()).GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 查询基类。
    /// </summary>
    /// <typeparam name="TModel">返回的实体模型类型。</typeparam>
    public abstract class QueryBase<TModel> : QueryBase<TModel, TModel>
    {
    }
}