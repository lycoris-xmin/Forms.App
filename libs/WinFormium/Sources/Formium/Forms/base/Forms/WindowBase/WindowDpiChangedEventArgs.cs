// THIS FILE IS PART OF WinFormium PROJECT
// THE WinFormium PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHTS (C) Xuanchen Lin. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/XuanchenLin/NanUI

namespace WinFormium.Sources.Formium.Forms.@base.Forms.WindowBase;

public class WindowDpiChangedEventArgs : System.EventArgs
{
    public int OldDPI { get; }
    public int NewDPI { get; }

    internal WindowDpiChangedEventArgs(int oldDpi, int newDpi)
    {
        OldDPI = oldDpi;
        NewDPI = newDpi;
    }
}
