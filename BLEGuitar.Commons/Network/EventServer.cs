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
        public async void Listen()
        {
            var listenPort = 11000;
            var listener = new TcpSocketListener();

            // when we get connections, read byte-by-byte from the socket's read stream
            listener.ConnectionReceived += async (sender, args) =>
            {
                var client = args.SocketClient;

                var bytesRead = -1;
                var buf = new byte[1];

                while (bytesRead != 0)
                {
                    bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                    if (bytesRead > 0)
                        Debug.WriteLine(buf[0]);
                }
            };

            // bind to the listen port across all interfaces
            await listener.StartListeningAsync(listenPort);
        }
    }
}
