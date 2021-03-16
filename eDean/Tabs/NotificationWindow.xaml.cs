using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telegram.Bot.Types;

namespace eDean.Tabs
{
    public partial class NotificationWindow : Window
    {
        static List<NotificationWindow> list = new List<NotificationWindow>();

        bool isFocusedPhone = false;

        private NotificationWindow()
        {
            InitializeComponent();

            foreach (var window in list)
            {
                window.Top -= 80;
            }

            list.Add(this);

            if (list.Count > 7)
            {
                list[0].Close();
                list.RemoveAt(0);
            }

            Top = (SystemParameters.FullPrimaryScreenHeight - this.Height);
            Left = (SystemParameters.FullPrimaryScreenWidth - this.Width);


            Closing += NotificationWindow_Closing;
        }
        public NotificationWindow(Message message) : this()
        {
            phoneTb.Text = message.Contact.PhoneNumber;
            nameTb.Text = message.Contact.FirstName + " " + message.Contact.LastName;
        }
        public NotificationWindow(DeanDb.User user)
        {
            phoneTb.Text = user.Phone;
            nameTb.Text = user.Name + " " + user.LastName;
        }

        private void NotificationWindow_Closing(object sender, EventArgs e)
        {
            int index = list.IndexOf(sender as NotificationWindow);
            for (int i = 0; i < index; i++)
            {
                list[i].Top += 80;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Phone_GotFocus(object sender, RoutedEventArgs e)
        {
            isFocusedPhone = true;
        }
        private void Phone_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (isFocusedPhone)
            {
                isFocusedPhone = false;
                (sender as TextBox).SelectAll();
            }
        }
    }
}
