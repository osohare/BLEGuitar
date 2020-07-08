using BLEGuitar.Commons;
using System;
using System.Threading.Tasks;

namespace BLEGuitar.Server
{
    public interface IBLEGuitarDevice
    {
        event EventHandler<DataReceivedArgs> DataReceived;
        Task Disconnect();
        void FindAndConnect();
    }

    public class DataReceivedArgs : EventArgs
    {
        public GuitarButtonsState Snapshot { get; set; }
    }
}