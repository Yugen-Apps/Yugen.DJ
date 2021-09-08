using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.System;
using Yugen.DJ.Uwp.Interfaces;
using Yugen.DJ.Uwp.Views.Dialogs;

namespace Yugen.DJ.Uwp.StateTriggers
{
    public class WhatsNewDisplayService : IWhatsNewDisplayService
    {
        private bool isShown = false;

        public async Task ShowIfAppropriateAsync()
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (SystemInformation.Instance.IsAppUpdated && !isShown)
            {
                isShown = true;
                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    var dialog = new WhatsNewDialog();
                    await dialog.ShowAsync();
                });
            }
        }
    }
}