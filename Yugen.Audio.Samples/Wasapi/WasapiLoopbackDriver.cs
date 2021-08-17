using WASAPI.NET.Com;

namespace WASAPI.NET
{
    public class WasapiLoopbackDriver : WasapiInDriver
    {
        protected override int DataFlow { get { return (int)DataFlowEnum.Render; } }

        protected override int StreamFlags { get { return (int)AudioClientStreamFlagsEnum.StreamFlagsLoopback; } }
    }
}