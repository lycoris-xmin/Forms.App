using Forms.App.Core.Contexts;
using Forms.App.Core.Logging;
using Forms.App.Model;
using Lycoris.Common.Helper;

namespace Forms.App.Main.Application
{
    public class ApplicationHostedService
    {
        private readonly IServerLogger _logger = FormAppContext.UseLogger<ApplicationHostedService>();

        public static async Task StartAsync()
        {
            try
            {
                FolderInitialize();
            }
            catch (Exception)
            {

                throw;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task StopAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void FolderInitialize()
        {
            FormAppContext.LaunchStep = LaunchStepEnum.FOLDER_INIT;

            if (!Directory.Exists(AppPath.LogFolder))
                FileHelper.EnsurePathExists(AppPath.LogFolder);

            if (!Directory.Exists(AppPath.TempFolder))
                FileHelper.EnsurePathExists(AppPath.TempFolder);
        }
    }
}
