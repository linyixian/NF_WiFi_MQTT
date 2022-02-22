using System;
using System.Diagnostics;
using System.Threading;

//�ǉ�
using nanoFramework.Networking;
using System.Text;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;

namespace NF_WiFi_MQTT
{
    public class Program
    {

        const string ssid = "YOUR SSID";
        const string password = "PASSWORD";

        public static void Main()
        {
            //Wifi�ڑ�
            ConnectWifi();

            //mqtt�N���C�A���g
            var client = new MqttClient("BROKER ADDRESS");
            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            //Subscribe
            client.Subscribe(new[] { "NF-mqtt/demo" }, new[] { MqttQoSLevel.AtLeastOnce });
            //�C�x���g�o�^
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //Publish
            for (int i = 0; i < 5; i++)
            {
                client.Publish("NF-mqtt/demo", Encoding.UTF8.GetBytes("=== Hello MQTT! ==="), MqttQoSLevel.AtLeastOnce, false);
                Thread.Sleep(5000);
            }

            //�ڑ��̉���
            client.Disconnect();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.WriteLine($"Message received: {Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)}");
        }

        private static void ConnectWifi()
        {
            CancellationTokenSource cs = new(60000);

            //Wifi��DHCP�ڑ�
            var success = WiFiNetworkHelper.ConnectDhcp(ssid, password, requiresDateTime: true, token: cs.Token);
            if (!success)
            {
                Debug.WriteLine($"Can't connect to the network, error: {WiFiNetworkHelper.Status}");
                if (WiFiNetworkHelper.HelperException != null)
                {
                    Debug.WriteLine($"ex: {WiFiNetworkHelper.HelperException}");
                }
            }
            else
            {
                Debug.WriteLine("Wifi Connected!");
            }
        }
    }
}
