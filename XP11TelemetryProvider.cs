using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.IO;

namespace XP11
{
    public class XP11TelemetryProvider : AbstractTelemetryProvider
    {
        // Port and IP of the game service
        private int PORTNUM = 49001;      // Server Port
        private string IP = "127.0.0.1";  // Server IP
        IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        private bool isStopped = true;          // flag to control the polling thread
        private Thread t;                       // the polling thread, reads telemetry data and sends TelemetryUpdated events

        private void readSettings()
        {
            try
            {
                string _serverip = null;
                string _port = null;
                string _pps = null;
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, "XP11.config");
                string[] lines = System.IO.File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    int index;

                    index = line.IndexOf("SERVERIP=");
                    if (index > -1)
                    {
                        _serverip = line.Substring(index + 9);
                        _serverip.Trim(' ');
                        continue;
                    }

                    index = line.IndexOf("PORT=");
                    if (index > -1)
                    {
                        _port = line.Substring(index + 5);
                        _port.Trim(' ');
                        continue;
                    }

                    index = line.IndexOf("PPS=");
                    if (index > -1)
                    {
                        _pps = line.Substring(index + 4);
                        _pps.Trim(' ');
                        continue;
                    }
                }//for

                if (_serverip != null) IP = _serverip;
                if (_port != null) if (Int32.TryParse(_port, out int result)) PORTNUM = result;
                if (_pps != null) if (Int32.TryParse(_pps, out int result)) TelemetryUpdateFrequency = result;
                else TelemetryUpdateFrequency = 20;
            }//try
            catch
            {
                TelemetryUpdateFrequency = 20;
            }
        }

        public XP11TelemetryProvider() : base()
        {
            Author = "Vantskruv";
            Version = "v0.1";
            BannerImage = @"img\banner_simfeedback.png"; // Image shown on top of the profiles tab
            IconImage = @"img\simfeedback.png";          // Icon used in the tree view for the profile

            readSettings();
        }

        public override string Name {  get { return "xp11"; } }
        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing XP11TelemetryProvider");
        }

        public override string[] GetValueList()
        {
            return null;
        }

        public override void Start()
        {
            if (isStopped)
            {
                isStopped = false;
                t = new Thread(Run);
                t.Start();
            }
        }

        public override void Stop()
        {
            isStopped = true;
            if (t != null) t.Join();
        }

        private void Run()
        {
            XP11Data data;
            Session session = new Session();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, PORTNUM));

            Log("Listener started (port: " + PORTNUM.ToString() + ") XP11TelemetryProvider.Thread");

            while (!isStopped)
            {
                try
                {
                    // get data from game, 
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            IsRunning = false;
                            IsConnected = false;
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                    else
                    {
                        IsConnected = true;
                    }

                    Byte[] received = socket.Receive(ref _senderIP);
                    data = new XP11Data(received);

                    if (data.IsRaceOn == 1)
                    {
                        IsRunning = true;

                        TelemetryEventArgs args = new TelemetryEventArgs(new XP11TelemetryInfo(data, session));
                        RaiseEvent(OnTelemetryUpdate, args);
                    }
                    else
                    {
                        IsRunning = false;
                    }

                    sw.Restart();
                    //Thread.Sleep(SamplePeriod);


                }
                catch (Exception)
                {
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }

            socket.Close();
            IsConnected = false;
            IsRunning = false;
        }
    }
 }
