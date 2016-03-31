using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace Codename_RGB
{
    /// <summary>
    /// Control your RGB LED strip through WiFi with an Arduino / Genuino with internet capabilities.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static class Globals
        {
            public static class Connection
            {
                public static bool arduinoInit = false;
                public static string arduinoIP;
            }
            public static class pinSetup
            {
                public static byte rPin = 4;
                public static byte gPin = 3;
                public static byte bPin = 2;
            }
            public static class colorValues
            {
                public static int rValue;
                public static int gValue;
                public static int bValue;
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            loadingBar.Visibility = Visibility.Collapsed;
            

            initializeSliders();
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionIP = ipTextBox.Text;
            IPAddress address;
            if (IPAddress.TryParse(connectionIP, out address))
            {
                //Valid IP, with address containing the IP
                this.InitArduino(connectionIP); //Init Arduino connection
                statusTextBlock.Text = "";
            }
            else {
                //Invalid IP
                statusTextBlock.Text = "Please write a valid IP";
            }
            
        }

        Microsoft.Maker.RemoteWiring.RemoteDevice arduino;
        Microsoft.Maker.Serial.NetworkSerial netWorkSerial;

        public void InitArduino(string IP)
        {
            loadingBar.Visibility = Visibility.Visible;
            //Establish a network serial connection. change it to the right IP address and port
            netWorkSerial = new Microsoft.Maker.Serial.NetworkSerial(new Windows.Networking.HostName(IP), 3030);

            //Create Arduino Device
            arduino = new Microsoft.Maker.RemoteWiring.RemoteDevice(netWorkSerial);

            //Attach event handlers
            netWorkSerial.ConnectionEstablished += NetWorkSerial_ConnectionEstablished;
            netWorkSerial.ConnectionFailed += NetWorkSerial_ConnectionFailed;

            //Begin connection
            netWorkSerial.begin(115200, Microsoft.Maker.Serial.SerialConfig.SERIAL_8N1);
        }

        private void NetWorkSerial_ConnectionEstablished()
        {
            arduino.pinMode(Globals.pinSetup.rPin, Microsoft.Maker.RemoteWiring.PinMode.OUTPUT); //Set the pin to output
            arduino.pinMode(Globals.pinSetup.gPin, Microsoft.Maker.RemoteWiring.PinMode.OUTPUT); //Set the pin to output
            arduino.pinMode(Globals.pinSetup.bPin, Microsoft.Maker.RemoteWiring.PinMode.OUTPUT); //Set the pin to output

            //turn it to High. The RED LED on Arduino should light up
            arduino.digitalWrite(6, Microsoft.Maker.RemoteWiring.PinState.HIGH);

            Globals.Connection.arduinoInit = true;

            loadingBar.Visibility = Visibility.Collapsed;

            statusTextBlock.Text = "Connected";
        }

        private void NetWorkSerial_ConnectionFailed(string message)
        {
            loadingBar.Visibility = Visibility.Collapsed;
            statusTextBlock.Text = "Arduino Connection Failed";
            System.Diagnostics.Debug.WriteLine("Arduino Connection Failed: " + message);
            Globals.Connection.arduinoInit = false;
        }

        private void initializeSliders()
        {
            rLabel.Text = System.Convert.ToString(rSlider.Value);
            gLabel.Text = System.Convert.ToString(gSlider.Value);
            bLabel.Text = System.Convert.ToString(bSlider.Value);
        }

        private void rSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Globals.colorValues.rValue = System.Convert.ToInt32(rSlider.Value);
            rLabel.Text = System.Convert.ToString(rSlider.Value);

            if (Globals.Connection.arduinoInit) {
                arduino.analogWrite(Globals.pinSetup.rPin, (ushort)rSlider.Value);
            }
        }

        private void gSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Globals.colorValues.gValue = System.Convert.ToInt32(gSlider.Value);
            gLabel.Text = System.Convert.ToString(gSlider.Value);

            if (Globals.Connection.arduinoInit) {
                arduino.analogWrite(Globals.pinSetup.gPin, (ushort)gSlider.Value);
            }
        }

        private void bSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Globals.colorValues.bValue = System.Convert.ToInt32(bSlider.Value);
            bLabel.Text = System.Convert.ToString(bSlider.Value);

            if (Globals.Connection.arduinoInit) {
                arduino.analogWrite(Globals.pinSetup.bPin, (ushort)bSlider.Value);
            }
        }


    }

}