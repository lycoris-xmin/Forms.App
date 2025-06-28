using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Forms.App.EntityFrameworkCore.Builder.ColumnConverters
{
    /// <summary>
    /// 
    /// </summary>
    internal class JsonMapConverter<T> : ValueConverter<T?, string> where T : class, new()
    {
        public JsonMapConverter() : base(x => x.ToJson(), x => x.ToTryObject<T>())
        {

        }
    }
}
