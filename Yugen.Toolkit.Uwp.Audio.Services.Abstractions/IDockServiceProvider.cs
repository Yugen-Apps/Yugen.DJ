namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IDockServiceProvider
    {
        void Initialize();

        IDockService Get(Side side);
    }
}