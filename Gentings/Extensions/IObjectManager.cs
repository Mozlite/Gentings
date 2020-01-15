using System.Threading;
using System.Threading.Tasks;
using Gentings.Data;
using Gentings.Extensions.Internal;

namespace Gentings.Extensions
{
    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">唯一键类型。</typeparam>
    public interface IObjectManager<TModel, TKey> : IObjectManagerBase<TModel, TKey>
        where TModel : IIdObject<TKey>
    {
        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TModel>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <typeparam name="TObject">返回的对象模型类型。</typeparam>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery Load<TQuery, TObject>(TQuery query) where TQuery : QueryBase<TModel, TObject>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <typeparam name="TObject">返回的对象模型类型。</typeparam>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> LoadAsync<TQuery, TObject>(TQuery query, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel, TObject>;
    }

    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IObjectManager<TModel> : IObjectManager<TModel, int>
        where TModel : IIdObject
    {
    }
}
