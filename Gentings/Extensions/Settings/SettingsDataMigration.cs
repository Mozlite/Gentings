using Gentings.Data.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gentings.Extensions.Settings
{
    /// <summary>
    /// 数据库迁移。
    /// </summary>
    public class SettingsDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SettingsAdapter>(table => table
                .Column(s => s.SettingKey)
                .Column(s => s.SettingValue)
            );
        }
    }


    /// <summary>
    /// 数据库迁移。
    /// </summary>
    internal class SettingDictionaryDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<SettingDictionary>(table =>
                table.Column(x => x.Id).Column(x => x.ParentId).Column(x => x.Name).Column(x => x.Value));
        }
    }

    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加字典组件。
        /// </summary>
        /// <param name="builder">服务构建实例。</param>
        /// <returns>服务构建实例。</returns>
        public static IServiceBuilder AddSettingDictionary(this IServiceBuilder builder)
        {
            return builder.AddServices(services =>
            {
                services.TryAddEnumerable(ServiceDescriptor
                    .Transient<IDataMigration, SettingDictionaryDataMigration>());
                services.TryAddSingleton<ISettingDictionaryManager, SettingDictionaryManager>();
            });
        }

        /// <summary>
        /// 添加字典组件。
        /// </summary>
        /// <typeparam name="TSettingDictionaryManager">字典实现类。</typeparam>
        /// <param name="builder">服务构建实例。</param>
        /// <returns>服务构建实例。</returns>
        public static IServiceBuilder AddSettingDictionary<TSettingDictionaryManager>(this IServiceBuilder builder)
            where TSettingDictionaryManager : class, ISettingDictionaryManager, new()
        {
            return builder.AddServices(services =>
             {
                 services.TryAddEnumerable(ServiceDescriptor
                     .Transient<IDataMigration, SettingDictionaryDataMigration>());
                 services.TryAddSingleton<ISettingDictionaryManager, TSettingDictionaryManager>();
             });
        }
    }
}