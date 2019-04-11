using System;
using System.Text;

namespace XP11
{
    class XP11Data
    {
        #region Telemetry Property
        //private static float Rad2Deg(float v) { return (float)(v * 180 / Math.PI); }

        private static readonly float G = 9.81f;
        public int IsRaceOn = 1;
        private float _accY, _pitch, _roll;

        public float Pitch; //degrees
        public float Roll //degrees
        {
            get
            {
                if (_roll < -90) return (-90 - (_roll + 90));
                if (_roll > 90) return (90 - (_roll - 90));
                return _roll;
            }
            set
            {
                _roll = value;
            }
        }
        public float AccelerationX; //m/s^2
        public float AccelerationY { get { return _accY - G; } set { _accY = value; } } //m/s^2
        public float AccelerationZ; //m/s^2

        public float RollSpeed; //rad/s
        public float PitchSpeed; //rad/s
        public float YawSpeed; //rad/s
        #endregion

        public XP11Data(byte[] rawTelemetryData)
        {
            //The format of the data packet where:
            //* the first five bytes describes the type of packet, ignore last byte
            //* the 'i' in the i000 describes the type of data, the three bytes after 'i' we can ignore
            //* the following data consist of 32 bytes, which describes 8 floating type numbers (each consist of 4 bytes).
            //D A T A X
            //i 0 0 0 ffff ffff ffff ffff ffff ffff ffff ffff
            //i 0 0 0 ffff ffff ffff ffff ffff ffff ffff ffff
            //and so on

            int requiredDataRead = 2;   //We exit the for loop when this variable is 0 (decreased when index 17 and 135 is found).
            String cString = "44-41-54-41"; // "DATA";
            String iString = BitConverter.ToString(rawTelemetryData, 0, 4);
            if (iString.CompareTo(cString) == 0)
            {
                for(int i=5; i<rawTelemetryData.Length && requiredDataRead!=0; i+=36)
                {
                    int index = BitConverter.ToInt16(rawTelemetryData, i);
                    
                    switch (index)
                    {
                        case 17: //Pitch, Roll, True heading, Magnetic heading
                            Pitch = BitConverter.ToSingle(rawTelemetryData, i + 4);
                            Roll = BitConverter.ToSingle(rawTelemetryData, i + 8);
                            requiredDataRead--;
                            break;
                        case 135: //AccX, AccY, AccZ, Roll-, Pitch-, Yaw-Speed
                            AccelerationX = BitConverter.ToSingle(rawTelemetryData, i + 4);
                            AccelerationY = BitConverter.ToSingle(rawTelemetryData, i + 8);
                            AccelerationZ = BitConverter.ToSingle(rawTelemetryData, i + 12);
                            RollSpeed = BitConverter.ToSingle(rawTelemetryData, i + 14);
                            PitchSpeed = BitConverter.ToSingle(rawTelemetryData, i + 18);
                            YawSpeed = BitConverter.ToSingle(rawTelemetryData, i + 22);
                            requiredDataRead--;
                            break;
                    }//switch
                }
            }
        }
    }
}
