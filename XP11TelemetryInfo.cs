using SimFeedback.telemetry;
using System;
using System.Reflection;

namespace XP11
{
    class XP11TelemetryInfo : TelemetryInfo
    {
        private XP11Data telemetryData;
        private Session session;

        public XP11TelemetryInfo(XP11Data data, Session session)
        {
            this.telemetryData = data;
            this.session = session;
        }

        private XP11TelemetryValue SurgeLowPass()
        {
            LowpassFilter lp = (LowpassFilter)session.Get("SurgeLowPass", new LowpassFilter());
            float data = (float)lp.firstOrder_lowpassFilter(telemetryData.AccelerationZ, 0.1);
            XP11TelemetryValue tv = new XP11TelemetryValue("Surge", data);
            return tv;
        }

        public TelemetryValue TelemetryValueByName(string name)
        {
            XP11TelemetryValue tv;
            switch (name)
            {
                case "Surge":
                    tv = SurgeLowPass();
                    break;
                default:
                    object data;
                    Type eleDataType = typeof(XP11Data);
                    PropertyInfo propertyInfo;
                    FieldInfo fieldInfo = eleDataType.GetField(name);
                    if (fieldInfo != null)
                    {
                        data = fieldInfo.GetValue(telemetryData);
                    }
                    else if ((propertyInfo = eleDataType.GetProperty(name)) != null)
                    {
                        data = propertyInfo.GetValue(telemetryData, null);
                    }
                    else
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    tv = new XP11TelemetryValue(name, data);
                    object value = tv.Value;
                    if (value == null)
                    {
                        throw new UnknownTelemetryValueException(name);
                    }

                    break;
            }

            return tv;
        }
    }
}