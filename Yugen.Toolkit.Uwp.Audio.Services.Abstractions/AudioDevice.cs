namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public class AudioDevice
    {
        public string Driver { get; set; }

        public int Id { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }
    }
}