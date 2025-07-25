// THIS FILE IS PART OF WinFormium PROJECT
// THE WinFormium PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHTS (C) Xuanchen Lin. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/XuanchenLin/NanUI

using WinFormium.Sources.WebResource.Data.@base;

namespace WinFormium.Sources.WebResource.Data.Result;
public class StatusCodeResult : ResourceResult
{
    public StatusCodeResult(int statusCode)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }

    public override void ExecuteResult(DataResourceContext context)
    {
        context.Response.HttpStatus = StatusCode;
    }
}
