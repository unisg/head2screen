using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Head2ScreenMagnifier.Lib
{
    public class SocketServer
    {
        #region Private Variables

        private bool stopListening = false;

        #endregion

        #region Constructor

        public SocketServer()
        {
        }

        #endregion

        #region Public Methods

        public void StartServer(string ipAddressAsDottedString, int port, Action<int, int> reportProgress)
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressAsDottedString);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Socket handler = listener.Accept();

                while (!this.stopListening)
                {
                    // listen and receive
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    // respond
                    byte[] msgBytes = Encoding.ASCII.GetBytes(data);
                    handler.Send(msgBytes);

                    // report progress
                    if (reportProgress != null)
                    {
                        Progressor.ConQueue.Enqueue(data);
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch { }
        }

        public void StopServer()
        {
            this.stopListening = true;
        }

        #endregion
    }
}