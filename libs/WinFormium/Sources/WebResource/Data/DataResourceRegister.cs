// THIS FILE IS PART OF WinFormium PROJECT
// THE WinFormium PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHTS (C) Xuanchen Lin. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/XuanchenLin/NanUI


// THIS FILE IS PART OF WinFormium PROJECT
// THE WinFormium PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHTS (C) Xuanchen Lin. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/XuanchenLin/NanUI

using WinFormium.Sources.Bootstrapper;
using WinFormium.Sources.WebResource.@base;
using WinFormium.Sources.WebResource.Data.@base;

namespace WinFormium.Sources.WebResource.Data;
public static class DataResourceRegister
{
    public static AppBuilder UseDataResource(this AppBuilder appBuilder, string scheme, string domainName, Action<DataResourceProvider> configureDataResourceProvider)
    {
        if (appBuilder.ProcessType == ProcessType.Renderer) return appBuilder;

        appBuilder.Services.AddScoped<ResourceSchemeHandlerFactory, DataResourceSchemeHandlerFactory>(provider =>
        {
            return new DataResourceSchemeHandlerFactory(scheme, domainName, configureDataResourceProvider);
        });

        return appBuilder;
    }

    public static IServiceCollection AddDataResource(this IServiceCollection services, string scheme, string domainName, Action<DataResourceProvider> configureDataResourceProvider)
    {
        services.AddScoped<ResourceSchemeHandlerFactory, DataResourceSchemeHandlerFactory>(provider =>
        {
            return new DataResourceSchemeHandlerFactory(scheme, domainName, configureDataResourceProvider);
        });

        return services;
    }
}
