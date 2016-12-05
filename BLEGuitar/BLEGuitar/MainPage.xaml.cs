using System;
using System.Collections.Generic;
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

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            await guitar.Connect();
        }
    }
}
