using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLEGuitar
{
    public class BLEGuitarDevice
    {
		private const string GuidMask = "00000000-0000-0000-0000-{0}"; //fdf7bc0342700
        private Guid serviceGUID = new Guid("533e1523-3abe-f33f-cd00-594e8b0a8ea3");
        private Guid characteristicsGUID = new Guid("533e1524-3abe-f33f-cd00-594e8b0a8ea3");
        private IDevice guitar;
		private List<IDevice> deviceList = new List<IDevice>();

		private string _deviceMac = string.Empty;
		public string DeviceMac
		{
			get 
			{ 
				return _deviceMac; 
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && value.Length == 12)
					_deviceMac = string.Format(GuidMask, value);
				else
					_deviceMac = string.Empty;
			}
		}

        public event EventHandler BluetoothError;
        protected void OnBluetoothError(EventArgs e)
        {
            BluetoothError?.Invoke(this, e);
        }

        public class MessageReceivedArgs : EventArgs
        {
            public string Message { get; set; }
        }
        public event EventHandler<MessageReceivedArgs> MessageReceived;
        protected void OnMessageReceived(MessageReceivedArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public async Task Connect()
        {
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            var state = ble.State;

			//ble.StateChanged += (s, e) =>
			//{
			//	Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
			//};

            switch (ble.State)
            {
                case BluetoothState.Unknown:
                case BluetoothState.Unavailable:
                case BluetoothState.Unauthorized:
                case BluetoothState.TurningOn:
                case BluetoothState.TurningOff:
                case BluetoothState.Off:
                    OnBluetoothError(EventArgs.Empty);
                    return;
                case BluetoothState.On:
                    break;
                default:
                    break;
            }

			CancellationToken cancellationToken = new CancellationToken();
			try
			{
				if (!string.IsNullOrWhiteSpace(DeviceMac))
				{
					guitar = await adapter.ConnectToKnownDeviceAsync(new Guid(DeviceMac), cancellationToken);
				}
				else
				{
					adapter.DeviceDiscovered += (s, a) => deviceList.Add(a.Device);
					await adapter.StartScanningForDevicesAsync();
					if (deviceList.Count < 1)
						throw new ArgumentException("No devices were found while scanning");
					guitar = deviceList[0];
					await adapter.ConnectToDeviceAsync(guitar, false, cancellationToken);
				}
			}
			catch (DeviceConnectionException e)
			{
				OnBluetoothError(EventArgs.Empty);
				Debug.WriteLine($"Could not connect to device " + e.Message);
				return;
			}
			catch (ArgumentException argEx)
			{
				OnBluetoothError(EventArgs.Empty);
				Debug.WriteLine($"Could not connect to device " + argEx.Message);
				return;
			}
			catch (Exception)
			{ 
				OnBluetoothError(EventArgs.Empty);
				return;
			}

            var service = await guitar.GetServiceAsync(serviceGUID);
            var characteristic = await service.GetCharacteristicAsync(characteristicsGUID);
            characteristic.ValueUpdated += (o, args) =>
            {
                OnMessageReceived(new MessageReceivedArgs() { Message = BitConverter.ToString(args.Characteristic.Value) });
            };
            await characteristic.StartUpdatesAsync();
        }
    }
}
