using BLEGuitar.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BLEGuitar.Server
{

    public partial class Form1 : Form
    {
        private readonly IBLEGuitarDevice guitar = new BLEGuitar();
        private readonly GuitarController controller;

        public Form1()
        {
            InitializeComponent();

            controller = new GuitarController();
            controller.LoadMaps();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            guitar.FindAndConnect();
            guitar.DataReceived += controller.Guitar_DataReceived;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            guitar.DataReceived -= controller.Guitar_DataReceived;
            var t = guitar.Disconnect();
            t.Wait();
            button2.Enabled = true;
        }
    }
}
