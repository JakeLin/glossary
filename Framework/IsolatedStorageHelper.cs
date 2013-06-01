using System;
using System.IO.IsolatedStorage;
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
    public class IsolatedStorageHelper
    {
        public static void SetSetting(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static object GetSetting(string key)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return null;
            }
            else
            {
                return IsolatedStorageSettings.ApplicationSettings[key];
            }
        }
    }
}
