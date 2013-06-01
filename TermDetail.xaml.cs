using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Glossary.Framework;
using Glossary.Items;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Glossary
{
    public partial class TermDetail : PhoneApplicationPage
    {
        //shortcut to get the instance of APP;
        private App app = ((App)Application.Current);

        private TermItem editingTermItem;
        private DetailAction detailAction;
        public TermDetail()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if(!NavigationContext.QueryString.ContainsKey("action"))
            {//Should never happen
                ToastPromptHelper.ShowToastPromptOnUIThreadAtEndOfQueue("Sorry, We have a technical issue, please try later.", 3000);
                NavigationService.GoBack();
            }

            string action = NavigationContext.QueryString["action"];
            if (action == "add")
            {
                PageTitle.Text = "add term";
                detailAction = DetailAction.Add;
            }
            else
            {
                PageTitle.Text = "edit term";
                detailAction = DetailAction.Edit;

                if (!PhoneApplicationService.Current.State.ContainsKey("termItem"))
                {
                    //Should never happen
                    ToastPromptHelper.ShowToastPromptOnUIThreadAtEndOfQueue(
                        "Sorry, We have a technical issue, please try later.", 3000);
                    NavigationService.GoBack();
                }
                editingTermItem = PhoneApplicationService.Current.State["termItem"] as TermItem;
                TermTextBox.Text = editingTermItem.Term;
                DescriptionTextBox.Text = editingTermItem.Description;
            }

            if(app.Activated)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("editingTermItem"))
                {
                    editingTermItem = PhoneApplicationService.Current.State["editingTermItem"] as TermItem;
                    PhoneApplicationService.Current.State.Remove("editingTermItem");
                    TermTextBox.Text = editingTermItem.Term;
                    DescriptionTextBox.Text = editingTermItem.Description;
                }

                if (PhoneApplicationService.Current.State.ContainsKey("detailAction"))
                {
                    detailAction = (DetailAction)PhoneApplicationService.Current.State["detailAction"];
                    PhoneApplicationService.Current.State.Remove("detailAction");
                }
                app.Activated = false;
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (detailAction == DetailAction.Add)
            {
                editingTermItem = new TermItem();
            }
            editingTermItem.Term = TermTextBox.Text;
            editingTermItem.Description = DescriptionTextBox.Text;

            PhoneApplicationService.Current.State["editingTermItem"] = editingTermItem;
            PhoneApplicationService.Current.State["detailAction"] = detailAction;

            base.OnNavigatedFrom(e);
        }

        private void ApplicationBarIconButton_Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Save_Click(object sender, EventArgs e)
        {
            string term = TermTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            if(term.Length == 0)
            {
                TermTextBox.Focus();
                //ToastPromptHelper.ShowToastPrompt("Term shouldn't be empty.", 3000);
                MessageBox.Show("Term shouldn't be empty.");
                return;
            }
            
            if (description.Length == 0)
            {
                DescriptionTextBox.Focus();
                //ToastPromptHelper.ShowToastPrompt("Description shouldn't be empty.", 3000);
                MessageBox.Show("Description shouldn't be empty.");
                return;
            }

            if (detailAction == DetailAction.Add)
            {
                if (app.Glossary.CheckNewTermName(term))
                {
                    TermTextBox.SelectAll();
                    TermTextBox.Focus();
                    MessageBox.Show("Term name exists. Please input another term. Please notice Term name is case insensitive.");
                    return;
                }
                app.Glossary.Add(term, description);
                NavigationService.GoBack();
            }
            else if (detailAction == DetailAction.Edit)
            {
                editingTermItem.Term = term;
                editingTermItem.Description = description;
                if (app.Glossary.CheckEditTermName(editingTermItem))
                {
                    TermTextBox.SelectAll();
                    TermTextBox.Focus();
                    MessageBox.Show("Term name exists. Please input another term. Please notice Term name is case insensitive.");
                    return;
                }
                app.Glossary.Edit(editingTermItem);
                NavigationService.GoBack();
            }
        }
    }
}