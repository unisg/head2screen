using Head2ScreenMagnifier.Lib;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Head2ScreenMagnifier.Forms
{
    internal static class Program
    {
        private static Magnifier magnifier = null;

        private static SocketServer socketServer = null;

        private static bool isRunning = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // get settings
            float screenZoomFactor = float.Parse(ConfigurationManager.AppSettings["ScreenZoomFactor"]);
            float magnifierZoomFactor = float.Parse(ConfigurationManager.AppSettings["MagnifierZoomFactor"]);
            float magnifierWindowSizeFactor = float.Parse(ConfigurationManager.AppSettings["MagnifierWindowSizeFactor"]);
            string myIPAddress = ConfigurationManager.AppSettings["MyIPAddress"];
            int myPort = int.Parse(ConfigurationManager.AppSettings["MyPort"]);

            // init magnifier
            magnifier = new Magnifier(magnifierZoomFactor, screenZoomFactor, magnifierWindowSizeFactor);

            // init socket server
            Task.Factory.StartNew(() =>
            {
                socketServer = new SocketServer();
                socketServer.StartServer(myIPAddress, myPort, (x, y) => { ReportProgress(x, y); });
            });

            // init report progress
            Task.Factory.StartNew(async () =>
            {
                while (isRunning)
                {
                    string data = string.Empty;
                    bool isSuccessful = Progressor.ConQueue.TryDequeue(out data);

                    if (isSuccessful)
                    {
                        if (Progressor.ConQueue.Count < 10)
                        {
                            int x = -1;
                            int y = -1;

                            int.TryParse(data.Substring(0, data.IndexOf('_')), out x);
                            int.TryParse(data.Substring(data.IndexOf('_') + 1), out y);

                            if (x != -1 && y != -1)
                            {
                                ReportProgress(x, y);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(1000 / 33); // 30fps
                    }
                }
            });

            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            isRunning = false;

            Task.Delay(1000 / 33).GetAwaiter().GetResult();

            if (magnifier != null)
            {
                magnifier.Dispose();
            }

            if (socketServer != null)
            {
                socketServer.StopServer();
            }
        }

        private static void ReportProgress(int x, int y)
        {
            if (magnifier != null)
            {
                magnifier.UpdateMagnifier(x, y);
            }
        }
    }
}