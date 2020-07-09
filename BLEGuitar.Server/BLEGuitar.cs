using BLEGuitar.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;

namespace BLEGuitar.Server
{
    public class BLEGuitar : IBLEGuitarDevice
    {
        private readonly bool isPolling = true;
        private Task pollingTask;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken token;

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

        public BLEGuitar() 
        {
            token = tokenSource.Token;
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

                            //Found characteristic, either subscribe for event based or poll every x ms
                            if (isPolling)
                            {
                                pollingTask = Task.Factory.StartNew(async () => {
                                    while (!token.IsCancellationRequested)
                                    {
                                        var result = await guitarCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                                        CryptographicBuffer.CopyToByteArray(result.Value, out byte[] data);
                                        OnDataReceived(new DataReceivedArgs() { Snapshot = decoder.SetValues(data) });
                                        Debug.WriteLine(BitConverter.ToString(data));

                                        await Task.Delay(10);
                                    }
                                    //token.ThrowIfCancellationRequested();
                                }, token);
                            }
                            else
                            {
                                var status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                        GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                if (status == GattCommunicationStatus.Success)
                                {
                                    guitarCharacteristic.ValueChanged += Characteristic_ValueChanged;
                                }
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
                if (isPolling)
                {
                    tokenSource.Cancel();
                    pollingTask = null;
                }
                else
                {
                    guitarCharacteristic.ValueChanged -= Characteristic_ValueChanged;

                    var status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                            GattClientCharacteristicConfigurationDescriptorValue.None);
                }
            }
        }

    }
}
