using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace Parallaxfix
{
    [PluginName("Parallax Mitigation")]
    public class Parallaxfix : IPositionedPipelineElement<IDeviceReport>
    {
        private float off;
        private float ang;


        [Property("Mitigation amount"), DefaultPropertyValue(0f), ToolTip
        (
            "Default: 0\n\n" +
            "changes amount of parallax mitigation, dont go too high or it will just reverse the effect\n"
        )]
        public float TiltOffset
        {
            set => off = Math.Clamp(value, -10000f, 10000f);
            get => off;
        }

        [Property("angle clamping"), DefaultPropertyValue(60f), ToolTip
        (
            "Set effective angle, put lower value if output is jittery\n" +
            "reccomended value is half your max tilt\n"
        )]
        public float Maxang
        {
            set => ang = Math.Clamp(value, 0f, 90f);
            get => ang;
        }


        public event Action<IDeviceReport> Emit;

        public PipelinePosition Position => PipelinePosition.PostTransform;


        public void Consume(IDeviceReport value)
        {

            if (value is ITabletReport report && value is ITiltReport tilt)
            {
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