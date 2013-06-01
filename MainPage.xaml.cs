using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;
using Coding4Fun.Phone.Controls;
using Glossary.Items;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Glossary
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        //shortcut to get the instance of APP;
        private App app = ((App)Application.Current);
        private IEnumerable<TermItem> data;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //used for databinding
            DataContext = this;
        }

        /// <summary>
        /// Used to databinding to listbox
        /// </summary>
        public IEnumerable<TermItem> Data
        {
            get { return data; }
            set 
            { 
                data = value;
                NotifyPropertyChanged("Data");
            }
        }

        private void ApplicationBarIconButton_Add_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TermDetail.xaml?action=add", UriKind.Relative));
        }

        private void ApplicationBarIconButton_About_Click(object sender, EventArgs e)
        {
            //The AboutPrompt has an issue that cannot display at the first time.
            //AboutPrompt about = new AboutPrompt();
            //about.Title = "About glossary";
            //about.Body = new TextBlock { Text = "Please press the glossary list to edit or remove particular term.", TextWrapping = TextWrapping.Wrap };
            //about.Footer = "This app is developed by Jake Lin";
            //about.VersionNumber = "Version " + GetVersionNumber();
            //about.Show();

            MessageBox.Show("ABOUT GLOSSARY\n\nPlease press the glossary list to pop up ContextMenu to edit or remove particular term.\n\nThis app is developed by Jake Lin.\n\nVersion " + GetVersionNumber());
        }

        private string GetVersionNumber()
        {
            var asm = Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            return parts[1].Split('=')[1];
        }

        private void ApplicationBarIconButton_Sort_Description_Click(object sender, EventArgs e)
        {
            app.Glossary.SortData(SortType.Description);
            Data = app.Glossary.Data;
        }

        private void ApplicationBarIconButton_Sort_Term_Click(object sender, EventArgs e)
        {
            app.Glossary.SortData(SortType.Term);
            Data = app.Glossary.Data;
        }


        private void MenuItem_Remove_Term_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var termItem = menuItem.DataContext as TermItem;
            app.Glossary.Remove(termItem.ID);
            Data = app.Glossary.Data;
        }

        private void MenuItem_Edit_Term_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var termItem = menuItem.DataContext as TermItem;

            PhoneApplicationService.Current.State["termItem"] = termItem;
            NavigationService.Navigate(new Uri("/TermDetail.xaml?action=edit", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (app.Activated)
            {
                app.Activated = false;
            }
            Data = app.Glossary.Data;
            base.OnNavigatedTo(e);
        }

        #region Property Changed

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}