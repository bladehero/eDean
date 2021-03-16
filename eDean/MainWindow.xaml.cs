using DeanDb;
using eDean.Settings;
using eDean.Tabs;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace eDean
{
    public partial class MainWindow : Window
    {
        static User user;
        public static User User { get => user; }

        public MainWindow()
        {
            InitializeComponent();
            string connection = @"Data Source=localhost;Initial Catalog=dean;Integrated Security=True;UID=sa;Password=sa;";

            Data.Context = new DeanDb.DeanContext(connection);
            try
            {
                Data.Context.Database.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Data.Context.SaveChanges();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = phoneTb.Text;
            string password = Data.GetMd5Hash(new MD5Cng(), passwordBox.Password);
            User user = null;
            switch (userCb.SelectedIndex)
            {
                case 0:
                    user = Data.Context.Deans.FirstOrDefault(d => d.Login == login && d.Password == password);
                    break;
                case 1:
                    user = Data.Context.Secretaries.FirstOrDefault(d => d.Login == login && d.Password == password);
                    break;
                case 2:
                    user = Data.Context.Teachers.FirstOrDefault(d => d.Login == login && d.Password == password);
                    break;
            }
            if (login == "admin")
            {
                user = new Dean();
                deansBtn.Visibility = Visibility.Visible;
                secretariesBtn.Visibility = Visibility.Visible;
                teachersBtn.Visibility = Visibility.Visible;
                moreInfoBtn.Visibility = Visibility.Visible;
                facultiesBtn.Visibility = Visibility.Visible;
                coursesBtn.Visibility = Visibility.Visible;
                skipsBtn.Visibility = Visibility.Visible;
                studentsBtn.Visibility = Visibility.Visible;
                subjectsBtn.Visibility = Visibility.Visible;
                groupsBtn.Visibility = Visibility.Visible;
            }

            if (user != null)
            {
                errorTb.Visibility = Visibility.Collapsed;
                loginPanel.Visibility = Visibility.Collapsed;
                mainPanel.Visibility = Visibility.Visible;
                MainWindow.user = user;
                SelectUser(user);
            }
            else
                errorTb.Visibility = Visibility.Visible;
        }
        private void SelectUser(User user)
        {
            if (user is Dean)
            {
                deansBtn.Visibility = Visibility.Visible;
                secretariesBtn.Visibility = Visibility.Visible;
                teachersBtn.Visibility = Visibility.Visible;
                moreInfoBtn.Visibility = Visibility.Visible;

                facultiesBtn.Visibility = Visibility.Visible;
                coursesBtn.Visibility = Visibility.Visible;
                studentsBtn.Visibility = Visibility.Visible;
            }
            else if (user is Secretary)
            {
                skipsBtn.Visibility = Visibility.Visible;
                studentsBtn.Visibility = Visibility.Visible;
                subjectsBtn.Visibility = Visibility.Visible;
                groupsBtn.Visibility = Visibility.Visible;

                coursesBtn.Visibility = Visibility.Visible;
                facultiesBtn.Visibility = Visibility.Visible;
            }
            else if (user is Teacher)
            {
                marksBtn.Visibility = Visibility.Visible;
                retakesBtn.Visibility = Visibility.Visible;
                skipsBtn.Visibility = Visibility.Visible;
            }

            subGrid.Visibility = Visibility.Visible;
        }
        private void DeansBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new UsersWindow(UserType.Dean);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void SecretariesBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new UsersWindow(UserType.Secretary);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void TeachersBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new UsersWindow(UserType.Teacher);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void StudentsBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new UsersWindow(UserType.Student);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void SubjectsBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new SubjectsWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void FacultiesBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new FacultiesWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void CoursesBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new CoursesWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void MarksBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new MarksWindow(user as Teacher);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void SkipsBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new SkipsWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void RetakesBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new RetakesWindow(user as Teacher);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void MoreInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new DeanInfoWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void GroupsBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new GroupsWindow();
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new SettingsWindow(user);
            window.Closed += (object o, EventArgs args) => (sender as Button).IsEnabled = true;
            window.Show();
        }
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            user = null;
            Close();
        }
    }
}
