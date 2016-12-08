using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLEGuitar.Commons.Network
{
    public class EventClient
    {
        private byte EoM = 0x0;

        public async void Send(byte[] data)
        {
            var address = "10.173.53.149";
            var port = 11000;

            var client = new TcpSocketClient();
            await client.ConnectAsync(address, port);

            await client.WriteStream.WriteAsync(data, 0, data.Length);
            client.WriteStream.WriteByte(EoM);
            await client.WriteStream.FlushAsync();

            await client.DisconnectAsync();
        }
    }
}
