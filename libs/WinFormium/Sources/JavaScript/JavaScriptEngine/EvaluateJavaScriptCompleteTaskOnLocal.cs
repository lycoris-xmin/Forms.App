// THIS FILE IS PART OF WinFormium PROJECT
// THE WinFormium PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHTS (C) Xuanchen Lin. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/XuanchenLin/NanUI

namespace WinFormium.Sources.JavaScript.JavaScriptEngine;

internal class EvaluateJavaScriptCompleteTaskOnLocal : CefTask
{
    public JavaScriptEngineBridge Bridge { get; }
    public required CefFrame Frame { get; init; }

    public required EvaluateJavaScriptCompleteMessage TaskData { get; init; }


    public EvaluateJavaScriptCompleteTaskOnLocal(JavaScriptEngineBridge javaScriptBridge)
    {
        Bridge = javaScriptBridge;
    }


    protected override void Execute()
    {
        var bag = JavaScriptEngineBridge.JavaScriptExecutionResults;

        if (bag.TryRemove((TaskData.TaskId, Frame.Identifier), out var tcs))
        {
            tcs.SetResult(new JavaScriptResult(Frame, TaskData.Success, TaskData.Data ?? string.Empty, TaskData.Message ?? string.Empty));
        }
        //else
        //{
        //    tcs.SetResult(new JavaScriptExecutionResult(Frame, false, null, TaskData.Message ?? string.Empty));
        //}
    }
}
