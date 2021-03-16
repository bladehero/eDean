using DeanDb;
using eDean.Settings;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace eDean.Tabs
{
    public partial class SettingsWindow : Window
    {
        User user;
        public SettingsWindow(User user)
        {
            this.user = user;
            InitializeComponent();
            loginTb.Text = user.Login;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }
        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }
        private bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'z')
                return true;
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string hash = Data.GetMd5Hash(new MD5Cng(), passwordTb.Password);
            if (user is Dean)
            {
                Dean dean = null;
                if (String.IsNullOrEmpty(loginTb.Text) || (Data.Context.Deans.Where(d => d.Login == user.Login).FirstOrDefault() != null && user.Login != loginTb.Text))
                {
                    loginTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if ((dean = Data.Context.Deans.Where(d => d.Login == user.Login && d.Password == hash).FirstOrDefault()) == null || String.IsNullOrEmpty(passwordTb.Password))
                {
                    passwordTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(newPassword.Password))
                {
                    newPassword.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(repeatNewPassword.Password) || newPassword.Password != newPassword.Password)
                {
                    repeatNewPassword.BorderBrush = Brushes.Red;
                    error = true;
                }

                if (error)
                {
                    return;
                }

                dean.Login = loginTb.Text;
                dean.Password = Data.GetMd5Hash(new MD5Cng(), newPassword.Password);
                user.Login = dean.Login;
                user.Password = dean.Password;
            }
            else if (user is Secretary)
            {
                Secretary secretary = null;
                if (String.IsNullOrEmpty(loginTb.Text) || (Data.Context.Deans.Where(d => d.Login == user.Login).FirstOrDefault() != null && user.Login != loginTb.Text))
                {
                    loginTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if ((secretary = Data.Context.Secretaries.Where(d => d.Login == user.Login && d.Password == hash).FirstOrDefault()) == null || String.IsNullOrEmpty(passwordTb.Password))
                {
                    passwordTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(newPassword.Password))
                {
                    newPassword.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(repeatNewPassword.Password) || newPassword.Password != newPassword.Password)
                {
                    repeatNewPassword.BorderBrush = Brushes.Red;
                    error = true;
                }

                if (error)
                {
                    return;
                }

                secretary.Login = loginTb.Text;
                secretary.Password = Data.GetMd5Hash(new MD5Cng(), newPassword.Password);
                user.Login = secretary.Login;
                user.Password = secretary.Password;
            }
            else if (user is Teacher)
            {
                Teacher teacher = null;
                if (String.IsNullOrEmpty(loginTb.Text) || (Data.Context.Deans.Where(d => d.Login == user.Login).FirstOrDefault() != null && user.Login != loginTb.Text))
                {
                    loginTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if ((teacher = Data.Context.Teachers.Where(d => d.Login == user.Login && d.Password == hash).FirstOrDefault()) == null || String.IsNullOrEmpty(passwordTb.Password))
                {
                    passwordTb.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(newPassword.Password))
                {
                    newPassword.BorderBrush = Brushes.Red;
                    error = true;
                }
                else if (String.IsNullOrEmpty(repeatNewPassword.Password) || newPassword.Password != newPassword.Password)
                {
                    repeatNewPassword.BorderBrush = Brushes.Red;
                    error = true;
                }

                if (error)
                {
                    return;
                }

                teacher.Login = loginTb.Text;
                teacher.Password = Data.GetMd5Hash(new MD5Cng(), newPassword.Password);
                user.Login = teacher.Login;
                user.Password = teacher.Password;
                Data.Context.SaveChanges();
            }
            Close();
        }
    }
}
