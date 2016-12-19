using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLEGuitar.Commons.Network
{
    public class EventServer
    {
        public class DataReceivedArgs : EventArgs
        {
            public GuitarButtonsState Snapshot { get; set; }
        }

        public event EventHandler<DataReceivedArgs> DataReceived;
        protected void OnDataReceived(DataReceivedArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        private EventDecoder decoder = new EventDecoder();

        public async void Listen()
        {
            var listenPort = 11000;
            var listener = new TcpSocketListener();

            // when we get connections, read byte-by-byte from the socket's read stream
            listener.ConnectionReceived += async (sender, args) =>
            {
                var client = args.SocketClient;

                var bytesRead = -1;
                var buffer = new byte[20];

                while (bytesRead != 0)
                {
                    bytesRead = await args.SocketClient.ReadStream.ReadAsync(buffer, 0, 20);
                    if (bytesRead > 0)
                    {
                        Debug.WriteLine(BitConverter.ToString(buffer));
                        OnDataReceived(new DataReceivedArgs() { Snapshot = decoder.SetValues(buffer) });
                    }
                }
            };

            // bind to the listen port across all interfaces
            await listener.StartListeningAsync(listenPort);
        }
    }
}
