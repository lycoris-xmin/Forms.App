using Forms.App.Core.Contexts;
using Forms.App.Main.JsObject.Builder;
using Forms.App.Model;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using WinFormium.CefGlue;

namespace Forms.App.Main.JsObject.Objects
{
    [JavaScriptRegister("Root")]
    internal class RootJavaScriptObject : JavaScriptObjectBuilder
    {
        public RootJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            // 
            AddMethod(GotoMainPage);

            // 
            AddMethod(GotoLaunchPage);

            //
            AddMethod(PageLoadedAsync);

            //
            AddMethod(SelectFileAsync);

            //
            AddMethod(SelectFolderAsync);

            // 
            AddMethod(OpenFolder);

            //
            AddMethod(PageReload);

            //
            AddMethod(OpenFolderAndSelectFile);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GotoMainPage() => Browser?.GetMainFrame().LoadUrl(AppSettings.Route.MainUrl);

        /// <summary>
        /// 
        /// </summary>
        private void GotoLaunchPage() => Browser?.GetMainFrame().LoadUrl(AppSettings.Route.LaunchUrl);

        /// <summary>
        /// 
        /// </summary>
        private async Task PageLoadedAsync()
        {
            static async Task SchedulerStart()
            {
                if (!FormAppContext.SchedulerIsRun)
                {
                    var quartz = FormAppContext.ServiceProvider.GetRequiredService<IQuartzSchedulerCenter>();
                    await quartz.StartScheduleAsync();
                    await quartz.ManualRunNonStandbyJobsAsync();

                    FormAppContext.SetSchedulerStatus();
                }
            }

            await SchedulerStart();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PageReload() => Browser?.Reload();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="multiSelect"></param>
        /// <returns></returns>
        private Task<string[]> SelectFileAsync(string filter = "所有文件 (*.*)|*.*", bool multiSelect = false)
        {
            var resultPaths = Array.Empty<string>();

            Invoke(() =>
            {
                using var dialog = new OpenFileDialog
                {
                    Title = "请选择文件",
                    Filter = filter,
                    Multiselect = multiSelect,
                    RestoreDirectory = true
                };

                var result = dialog.ShowDialog(FormAppContext.GetFormium<MainWindow>());
                if (result == DialogResult.OK)
                    resultPaths = dialog.FileNames;
            });

            return Task.FromResult(resultPaths);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task<string> SelectFolderAsync()
        {
            string? path = null;

            Invoke(() =>
            {
                using var dialog = new FolderBrowserDialog();
                dialog.Description = "请选择下载路径";
                dialog.UseDescriptionForTitle = true;
                dialog.ShowNewFolderButton = true;

                var result = dialog.ShowDialog(FormAppContext.GetFormium<MainWindow>());
                path = result == DialogResult.OK ? dialog.SelectedPath : null;
            });

            return Task.FromResult(path ?? "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void OpenFolder(string? path)
        {
            if (path.IsNullOrEmpty() || !Directory.Exists(path))
            {
                FormAppContext.Bridge.Toast.Error("目录不存在");
                return;
            }

            Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenFolderAndSelectFile(string? filePath)
        {
            if (filePath.IsNullOrEmpty() || !File.Exists(filePath))
            {
                FormAppContext.Bridge.Toast.Error("文件不存在");
                return;
            }

            var argument = $"/select,\"{filePath}\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = argument,
                UseShellExecute = true
            });
        }

    }
}

