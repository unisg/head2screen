using Head2ScreenMagnifier.Core;
using Microsoft.CognitiveServices.Speech;
using System.Configuration;
using System.Numerics;

namespace Head2ScreenMagnifier.Host
{
    internal static class Program
    {
        private static Magnifier magnifier = null;

        private static SocketServer socketServer = null;
        private static MouseServer mouseServer = null;

        private static NLog.Logger logger = null;

        private static bool isRunning = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // init logger
            Program.logger = NLog.LogManager.GetCurrentClassLogger();

            // get settings
            float screenZoomFactor = float.Parse(ConfigurationManager.AppSettings["ScreenZoomFactor"]);
            float magnifierZoomFactor = float.Parse(ConfigurationManager.AppSettings["MagnifierZoomFactor"]);
            float magnifierWindowSizeFactor = float.Parse(ConfigurationManager.AppSettings["MagnifierWindowSizeFactor"]);
            int monitorWidth_mm = int.Parse(ConfigurationManager.AppSettings["MonitorWidth_mm"]);
            int monitorHeight_mm = int.Parse(ConfigurationManager.AppSettings["MonitorHeight_mm"]);
            string myIPAddress = ConfigurationManager.AppSettings["MyIPAddress"];
            int myPort = int.Parse(ConfigurationManager.AppSettings["MyPort"]);
            int fps = int.Parse(ConfigurationManager.AppSettings["FramesPerSecond"]);
            bool trapMouse = bool.Parse(ConfigurationManager.AppSettings["TrapMouse"]);
            float mouseOffsetHorizontal = float.Parse(ConfigurationManager.AppSettings["MouseOffsetHorizontal"]);
            float mouseOffsetVertical = float.Parse(ConfigurationManager.AppSettings["MouseOffsetVertical"]);
            Vector2 extendFactor;
            extendFactor.X = float.Parse(ConfigurationManager.AppSettings["ExtendFactorX"]);
            extendFactor.Y = float.Parse(ConfigurationManager.AppSettings["ExtendFactorY"]);
            if (extendFactor.X == 0) extendFactor.X = 1;
            if (extendFactor.Y == 0) extendFactor.Y = 1;

            // init Smoother
            MoveSmoother smoother = new MoveSmoother(
                     new Vector2(float.Parse(ConfigurationManager.AppSettings["ThresholdMoveStartX"]), float.Parse(ConfigurationManager.AppSettings["ThresholdMoveStartY"])),
                     new Vector2(float.Parse(ConfigurationManager.AppSettings["ThresholdMoveStopX"]), float.Parse(ConfigurationManager.AppSettings["ThresholdMoveStopY"])),
                     float.Parse(ConfigurationManager.AppSettings["LerpAmount"]),
                     float.Parse(ConfigurationManager.AppSettings["LerpAccelerator"]),
                     float.Parse(ConfigurationManager.AppSettings["LerpDecelerator"]),
                     bool.Parse(ConfigurationManager.AppSettings["LerpIsActive"]),
                     float.Parse(ConfigurationManager.AppSettings["DeceleratorRadius"])
                );

            mouseServer = new MouseServer();

            // init magnifier
            magnifier = new Magnifier(magnifierZoomFactor, screenZoomFactor, magnifierWindowSizeFactor, monitorWidth_mm, monitorHeight_mm, trapMouse, mouseOffsetHorizontal, mouseOffsetVertical, extendFactor);
            Program.logger.Info("Magnifier initialized");

            // init socket server
            Task.Factory.StartNew(() =>
            {
                socketServer = new SocketServer();
                socketServer.StartServer(myIPAddress, myPort, (x, y) => { ReportProgress(new Vector2(x, y)); });
                Program.logger.Info("Socketserver started");
            });

            // init report progress
            Task.Factory.StartNew(async () =>
            {
                Program.logger.Info("Reporting initialized");

                while (isRunning)
                {
                    string data = string.Empty;
                    bool isSuccessful = Progressor.ConQueue.TryDequeue(out data);

                    if (AppState.IsInMouseMode)
                    {
                        ReportProgress(mouseServer.GetPosition());
                        await Task.Delay(1000 / fps);
                    }
                    else
                    {
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
                                    Vector2 smoothPosition = smoother.Smooth(new System.Numerics.Vector2(x, y));
                                }
                            }
                        }
                        else
                        {
                            Vector2 smoothPosition = smoother.Smooth();
                            ReportProgress(smoothPosition);
                            await Task.Delay(1000 / fps);
                        }
                    }
                }
            });

            #region Keyword recognition

            Task.Factory.StartNew(() =>
            {
                StartKeywordRecognizer("Mouse-Mode.table");
                Program.logger.Info("Mouse mode recognizer initialized");
            });
            Task.Factory.StartNew(() =>
            {
                StartKeywordRecognizer("Head-Mode.table");
                Program.logger.Info("Head mode recognizer initialized");
            });

            #endregion

            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }

        private static void StartKeywordRecognizer(string modelName)
        {
            try
            {
                var audioConfig = Microsoft.CognitiveServices.Speech.Audio.AudioConfig.FromDefaultMicrophoneInput();
                var recognizer = new KeywordRecognizer(audioConfig);
                recognizer.Recognized += (s, e) =>
                {
                    string mode = e.Result.Text;
                    if (mode.StartsWith("Mouse "))
                    {
                        Program.logger.Info("Recognized 'Mouse' mode");
                        AppState.IsInMouseMode = true;
                    }
                    else
                    {
                        Program.logger.Info("Recognized 'Head' mode");
                        AppState.IsInMouseMode = false;
                    }
                };
                var keywordModel = KeywordRecognitionModel.FromFile(modelName);
                while (true)
                {
                    var result = recognizer.RecognizeOnceAsync(keywordModel);
                    result.Wait();
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error(ex);
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            isRunning = false;

            Task.Delay(1000).GetAwaiter().GetResult();

            if (magnifier != null)
            {
                magnifier.Dispose();
                magnifier = null;
            }

            if (socketServer != null)
            {
                socketServer.StopServer();
                socketServer = null;
            }
        }

        private static void ReportProgress(Vector2 pos)
        {
            try
            {
                if (magnifier != null)
                {
                    magnifier.UpdateMagnifier(pos);
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error(ex);
            }
        }
    }
}