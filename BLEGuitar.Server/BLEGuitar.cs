using BLEGuitar.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;

namespace BLEGuitar.Server
{
    public class BLEGuitar : IBLEGuitarDevice
    {
        private BluetoothLEAdvertisementWatcher watcher;
        private const string GuitarDeviceName = "Ble Guitar";

        private Guid serviceGUID = new Guid("533e1523-3abe-f33f-cd00-594e8b0a8ea3");
        private Guid characteristicsGUID = new Guid("533e1524-3abe-f33f-cd00-594e8b0a8ea3");
        private GattCharacteristic guitarCharacteristic;

        private readonly EventDecoder decoder = new EventDecoder();

        public event EventHandler<DataReceivedArgs> DataReceived;
        protected void OnDataReceived(DataReceivedArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public void FindAndConnect()
        {
            watcher = new BluetoothLEAdvertisementWatcher()
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            watcher.Received += async (_, args) =>
            {
                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                if (device != null && device.Name == GuitarDeviceName)
                {
                    watcher.Stop();
                    var services = await device.GetGattServicesAsync();
                    if (services.Status == GattCommunicationStatus.Success)
                    {
                        var service = services.Services.First(s => s.Uuid == serviceGUID);
                        var characteristics = await service.GetCharacteristicsAsync();
                        if (characteristics.Status == GattCommunicationStatus.Success)
                        {
                            guitarCharacteristic = characteristics.Characteristics.First(c => c.Uuid == characteristicsGUID);

                            var status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.Notify);

                            if (status == GattCommunicationStatus.Success)
                            {
                                guitarCharacteristic.ValueChanged += Characteristic_ValueChanged;
                            }
                        }
                    }
                }
            };

            watcher.Stopped += (_, args) =>
            {
                Debug.WriteLine("Watcher stopped");
            };

            watcher.Start();
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);

            OnDataReceived(new DataReceivedArgs() { Snapshot = decoder.SetValues(data) });

            Debug.WriteLine(BitConverter.ToString(data));
        }


        public async Task Disconnect()
        {
            if (watcher != null && watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                watcher.Stop();

            if (guitarCharacteristic != null)
            {
                guitarCharacteristic.ValueChanged -= Characteristic_ValueChanged;

                var status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.None);
            }
        }

    }
}
