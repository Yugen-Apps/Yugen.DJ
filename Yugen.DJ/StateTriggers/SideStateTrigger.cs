using Windows.UI.Xaml;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.StateTriggers
{
    public class SideStateTrigger : StateTriggerBase
    {
        private Side _side;

        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                SetActive(_side == Side.Right);
            }
        }
    }
}