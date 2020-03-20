using CefSharp.Wpf;
using System;
using System.IO;

namespace CefSharp.MinimalExample.Wpf
{
    public static class Program
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            //For Windows 7 and above, app.manifest entries will take precedences of this call
            Cef.EnableHighDPISupport();

            //We are using our current exe as the BrowserSubProcess
            //Multiple instances will be spawned to handle all the 
            //Chromium proceses, render, gpu, network, plugin, etc.
            var subProcessExe = new CefSharp.BrowserSubprocess.BrowserSubprocessExecutable();
            var result = subProcessExe.Main(args);
            if (result > 0)
            {
                return result;
            }

            //We use our current exe as the BrowserSubProcess
            //var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cache"),
                PersistSessionCookies = true,//VERY IMPORTANT
            };
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            settings.BrowserSubprocessPath = exePath;
            //settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("no-proxy-server");
			settings.CefCommandLineArgs.Add("proxy-auto-detect","0");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            Cef.EnableHighDPISupport();
            var app = new App();
            app.InitializeComponent();
            return app.Run();
        }
    }
}