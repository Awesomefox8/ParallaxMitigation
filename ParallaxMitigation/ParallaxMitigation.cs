using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin;

namespace Parallaxfix
{
    [PluginName("Parallax Mitigation")]
    public class Parallaxfix : IPositionedPipelineElement<IDeviceReport>
    {
        private float ang;
        private float des;
        private float ac;
        private float mam;
        public float prevposx;



        [Property("Mitigation amount"), DefaultPropertyValue(1f), ToolTip
        (
            "sets the mitigation amount, for manual calibration\n"
        )]
        public float TiltOffset
        {
            set => mam = Math.Clamp(value, 0f, 1000f);
            get => mam;
        }

        [Property("angle clamping"), DefaultPropertyValue(60f), ToolTip
        (
            "Set effective angle, put lower value if output is jittery\n" +
            "recommended value is half your max tilt\n"
        )]
        public float Maxang
        {
            set => ang = Math.Clamp(value, 0f, 90f);
            get => ang;
        }

        [Property("Accuracy"), DefaultPropertyValue(0.3), ToolTip
        (
            "Acceptable margin of error"
        )]
        public float Accu
        {
            set => ac = Math.Clamp(value, 0f, 1f);
            get => ac;
        }


        [Property("Tightening amount"), DefaultPropertyValue(0.05), ToolTip
        (
            "amount of change per iteration"
        )]
        public float Tightam
        {
            set => des = Math.Clamp(value, 0f, 1f);
            get => des;
        }

        [BooleanProperty("calibration","turn on callibration"), DefaultPropertyValue(false)]
        public bool Calibration { set; get; }


        public event Action<IDeviceReport> Emit;

        public PipelinePosition Position => PipelinePosition.PostTransform;


        public void Consume(IDeviceReport value)
        {

            if (value is ITabletReport report && value is ITiltReport tilt)
            {

                if (Calibration && report.Pressure >= 1)
                {
                    if (prevposx - report.Position.X > Accu || prevposx - report.Position.X < -Accu)
                    {
                        mam += Tightam;
                        OpenTabletDriver.Plugin.Log.Write("Plugin", $"Value: {mam}", LogLevel.Debug);
                    }
                }
                prevposx = report.Position.X;

                var ClampedX = Math.Clamp(tilt.Tilt.X, -Maxang, Maxang);
                var ClampedY = Math.Clamp(tilt.Tilt.Y, -Maxang, Maxang);

                float Xoffset = (float)(TiltOffset * Math.Cos((ClampedX + 270) * (Math.PI / 180)));
                float Yoffset = (float)(TiltOffset * Math.Cos((ClampedY - 270) * (Math.PI / 180)));


                var newposX = report.Position.X + Xoffset;
                var newposY = report.Position.Y + Yoffset;
                report.Position = new Vector2(newposX, newposY);
                value = report;
            }

            Emit?.Invoke(value);
        }
    }
}
