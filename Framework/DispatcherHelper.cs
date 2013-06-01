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

namespace Glossary.Framework
{
    public class DispatcherHelper
    {
        public static void BeginInvokeOnUIThread(Action action)
        {
            var Dispatcher = Deployment.Current.Dispatcher;
            if (Dispatcher.CheckAccess() == false)
            {
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        public static void BeginInvokeOnUIThreadAtEndOfQueue(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
