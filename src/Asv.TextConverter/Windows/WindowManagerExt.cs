using System.Collections.Generic;
using Caliburn.Micro;

namespace Asv.TextConverter
{
    public static class WindowManagerExt
    {
        public static void ShowWindow<TViewModel>(this IWindowManager src, object context = null, IDictionary<string, object> settings = null)
        {
            src.ShowWindow(IoC.Get<TViewModel>(),context,settings);
        }
    }
}