using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BLEGuitar
{
    public partial class MainPage : ContentPage
    {
        BLEGuitarDevice guitar = new BLEGuitarDevice();

        public MainPage()
        {
            InitializeComponent();
            guitar.BluetoothError += Guitar_BluetoothError;
            guitar.MessageReceived += Guitar_MessageReceived;

			Debug.WriteLine(btnScan.Id);
			Debug.WriteLine(btnConnect.Id);
        }

        private void Guitar_MessageReceived(object sender, BLEGuitarDevice.MessageReceivedArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                lblStatus.Text = e.Message;
            });
        }

        private void Guitar_BluetoothError(object sender, EventArgs e)
        {
            DisplayAlert("UhOh!", "Your bluetooth adapter has an issue", "Ok I will correct this!");
        }

        private async void btnScan_OnButtonClicked(object sender, EventArgs e)
        {
			guitar.DeviceMac = string.Empty;
            await guitar.Connect();
        }

        private async void btnConnect_OnButtonClicked(object sender, EventArgs e)
		{
			guitar.DeviceMac = txtMacAddress.Text;
			await guitar.Connect();
		}
    }
}
