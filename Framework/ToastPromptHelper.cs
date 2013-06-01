using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Coding4Fun.Phone.Controls;

namespace Glossary.Framework
{
    public class ToastPromptHelper
    {
        public static void ShowToastPromptOnUIThreadAtEndOfQueue(string msg, int duration)
        {
            DispatcherHelper.BeginInvokeOnUIThreadAtEndOfQueue(() =>
            {
                ShowToastPrompt(msg, duration);
            });
        }

        public static void ShowToastPrompt(string msg, int duration)
        {
            ToastPrompt toastPrompt = new ToastPrompt();
            toastPrompt.IsTimerEnabled = true;
            toastPrompt.MillisecondsUntilHidden = duration;
            //toastPrompt.Title = "";
            toastPrompt.Message = msg;
            toastPrompt.Show();
        }
    }
}
