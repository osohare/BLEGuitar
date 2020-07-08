using BLEGuitar.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;


namespace BLEGuitar.Server
{
    public class BLEGuitarFinder
    {
        private readonly List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();
        private readonly List<DeviceInformation> KnownDevices = new List<DeviceInformation>();
        private DeviceWatcher deviceWatcher;

        private const string GuitarDeviceName = "Ble Guitar";
        private DeviceInformation GuitarDevice;

        private Guid serviceGUID = new Guid("533e1523-3abe-f33f-cd00-594e8b0a8ea3");
        private GattDeviceService guitarService;

        private Guid characteristicsGUID = new Guid("533e1524-3abe-f33f-cd00-594e8b0a8ea3");
        private GattCharacteristic guitarCharacteristic;

        private readonly EventDecoder decoder = new EventDecoder();

        public class DataReceivedArgs : EventArgs
        {
            public GuitarButtonsState Snapshot { get; set; }
        }

        public event EventHandler<DataReceivedArgs> DataReceived;
        protected void OnDataReceived(DataReceivedArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public void FindAndConnect()
        {
            // Additional properties we would like about the device.
            // Property strings are documented here https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            // BT_Code: Example showing paired and non-paired in a single query.
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                        aqsAllBluetoothLEDevices,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            KnownDevices.Clear();

            // Start the watcher. Active enumeration is limited to approximately 30 seconds.
            // This limits power usage and reduces interference with other Bluetooth activities.
            // To monitor for the presence of Bluetooth LE devices for an extended period,
            // use the BluetoothLEAdvertisementWatcher runtime class. See the BluetoothAdvertisement
            // sample for an example.
            deviceWatcher.Start();
        }

        private DeviceInformation FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (DeviceInformation bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            if (sender == deviceWatcher)
            {
                Debug.WriteLine($"{KnownDevices.Count} devices found. Enumeration completed.");
            }
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == deviceWatcher)
            {
                // Make sure device isn't already present in the list.
                if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                {
                    if (deviceInfo.Name != string.Empty)
                    {
                        // If device has a friendly name display it immediately.
                        KnownDevices.Add(deviceInfo);

                        if (deviceInfo.Name == GuitarDeviceName)
                        {
                            GuitarDevice = deviceInfo;
                            var t = Pair();
                            t.Wait();
                        }
                    }
                    else
                    {
                        // Add it to a list in case the name gets updated later. 
                        UnknownDevices.Add(deviceInfo);
                    }
                }
            }
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            if (sender == deviceWatcher)
            {
                var bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                if (bleDeviceDisplay != null)
                {
                    // Device is already being displayed - update UX.
                    bleDeviceDisplay.Update(deviceInfoUpdate);
                    return;
                }

                DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                if (deviceInfo != null)
                {
                    deviceInfo.Update(deviceInfoUpdate);
                    // If device has been updated with a friendly name it's no longer unknown.
                    if (deviceInfo.Name != String.Empty)
                    {
                        KnownDevices.Add(deviceInfo);
                        UnknownDevices.Remove(deviceInfo);
                    }
                }
            }
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == deviceWatcher)
            {
                // Find the corresponding DeviceInformation in the collection and remove it.
                var bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                if (bleDeviceDisplay != null)
                {
                    KnownDevices.Remove(bleDeviceDisplay);
                }

                DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                if (deviceInfo != null)
                {
                    UnknownDevices.Remove(deviceInfo);
                }
            }
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            if (sender == deviceWatcher)
            {
                Debug.WriteLine($"No longer watching for devices.");
            }
        }

        private void StopWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        private async Task Pair()
        {
            StopWatcher();

            var bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(GuitarDevice.Id);
            GattDeviceServicesResult deviceServices = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

            if (deviceServices.Status == GattCommunicationStatus.Success)
            {
                foreach (GattDeviceService service in deviceServices.Services)
                {
                    var accessStatus = await service.RequestAccessAsync();
                    if (accessStatus != DeviceAccessStatus.Allowed)
                        continue;

                    var serviceCharacteristics = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (serviceCharacteristics.Status == GattCommunicationStatus.Success)
                    {
                        foreach (GattCharacteristic characteristic in serviceCharacteristics.Characteristics)
                        {
                            if (service.Uuid == serviceGUID
                                && characteristic.Uuid == characteristicsGUID)
                            {
                                Debug.WriteLine($"Found service:{service.Uuid} characteristic:{characteristic.Uuid}");
                                guitarService = service;
                                guitarCharacteristic = characteristic;

                                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                                if (guitarCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                                }

                                else if (guitarCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                                }

                                status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                                if (status == GattCommunicationStatus.Success)
                                {
                                    guitarCharacteristic.ValueChanged += Characteristic_ValueChanged;
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task Disconnect()
        {
            var status = await guitarCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.None);

            guitarCharacteristic.ValueChanged -= Characteristic_ValueChanged;
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);

            OnDataReceived(new DataReceivedArgs() { Snapshot = decoder.SetValues(data) });
            
            Debug.WriteLine(BitConverter.ToString(data));
        }
    }
}
