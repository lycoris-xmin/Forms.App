using Forms.App.EntityFrameworkCore.Data.Attributes;
using Forms.App.Model.Contexts;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Lycoris.Snowflakes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Forms.App.EntityFrameworkCore.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyAutoProvider
    {
        /// <summary>
        /// 
        /// </summary>
        ClientContext ClientContext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void InsertIntercept(EntityEntry entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void UpdateIntercept(EntityEntry entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void DeleteIntercept(EntityEntry entities);
    }

    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class PropertyAutoProvider : IPropertyAutoProvider
    {
        /// <summary>
        /// 请求上下文
        /// </summary>
        public ClientContext ClientContext { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="requestContext"></param>
        public PropertyAutoProvider(ClientContext ClientContext)
        {
            this.ClientContext = ClientContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void InsertIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if ((item.Metadata.ClrType == typeof(long) || item.Metadata.ClrType == typeof(long?)) && item.Metadata.PropertyInfo != null && item.Metadata.PropertyInfo!.GetCustomAttribute<SnowflakeAttribute>(false) != null)
                {
                    if (item.CurrentValue != null && (long)item.CurrentValue > 0)
                        continue;

                    item.CurrentValue = SnowflakeHelper.GetNextId();
                }
                else if (item.Metadata.Name.Equals("CreateTime", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                        item.CurrentValue = DateTime.Now;
                }
                else if (this.ClientContext.User?.Id > 0)
                {
                    if (item.Metadata.Name.Equals("CreatorId", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(long))
                    {
                        if (item.CurrentValue == null || (long)item.CurrentValue == 0)
                            item.CurrentValue = this.ClientContext.User?.Id ?? 0;
                    }

                    if (item.Metadata.Name.Equals("CreatorName", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.NickName ?? "";
                    }

                    if (item.Metadata.Name.Equals("CreatorAvatar", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.Avatar ?? "";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("UpdateTime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
                else if (this.ClientContext.User?.Id > 0)
                {
                    if (item.Metadata.Name.Equals("UpdaterId", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(long))
                    {
                        if (item.CurrentValue == null || (long)item.CurrentValue == 0)
                            item.CurrentValue = this.ClientContext.User?.Id ?? 0;
                    }

                    if (item.Metadata.Name.Equals("UpdaterName", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.NickName ?? "";
                    }

                    if (item.Metadata.Name.Equals("UpdaterAvatar", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.Avatar ?? "";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void DeleteIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("DeleteTime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
                else if (this.ClientContext.User?.Id > 0)
                {
                    if (item.Metadata.Name.Equals("DeleterId", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(long))
                    {
                        if (item.CurrentValue == null || (long)item.CurrentValue == 0)
                            item.CurrentValue = this.ClientContext.User?.Id ?? 0;
                    }

                    if (item.Metadata.Name.Equals("DeleterName", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.NickName ?? "";
                    }

                    if (item.Metadata.Name.Equals("DeleterAvatar", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.ClientContext.User?.Avatar ?? "";
                    }
                }
            }
        }
    }
}
