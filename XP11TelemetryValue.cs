using SimFeedback.telemetry;

namespace XP11
{
    public class XP11TelemetryValue : AbstractTelemetryValue
    {
        public XP11TelemetryValue(string name, object value) : base()
        {
            Name = name;
            Value = value;
        }
        public override object Value { get; set; }
    }
}