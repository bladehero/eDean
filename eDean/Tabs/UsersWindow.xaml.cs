using DeanDb;
using eDean.Grids;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Collections;
using System;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Text;
using eDean.Settings;
using Telegram.Bot.Types.ReplyMarkups;

namespace eDean.Tabs
{
    public partial class UsersWindow : Window
    {
        private IList source;
        private DataGridBuilder builder;

        public object Selected { get; private set; }

        public UsersWindow(UserType user)
        {
            InitializeComponent();
            switch (user)
            {
                case UserType.Student:
                    source = Data.Context.Students.Include(s => s.Group.Faculty).ToList();
                    builder = new StudentDataGridBuilder(dataGrid);
                    Title = "Студенты";
                    labelInfoTb.Text = "Группа:";
                    infoPanel.Visibility = Visibility.Visible;
                    break;
                case UserType.Teacher:
                    source = Data.Context.Teachers.Include(s => s.Groups).ToList();
                    builder = new TeacherDataGridBuilder(dataGrid);
                    Title = "Преподаватели";
                    labelInfoTb.Text = "Кураторство:";
                    infoPanel.Visibility = Visibility.Visible;
                    break;
                case UserType.Secretary:
                    source = Data.Context.Secretaries.ToList();
                    builder = new SecretaryDataGridBuilder(dataGrid);
                    Title = "Секретари";
                    break;
                case UserType.Dean:
                    source = Data.Context.Deans.Include(s => s.Faculties).ToList();
                    builder = new DeanDataGridBuilder(dataGrid);
                    Title = "Зав. отделениями";
                    labelInfoTb.Text = "Факультеты:";
                    infoPanel.Visibility = Visibility.Visible;
                    break;
            }
            new DataGridDirector().Create(builder, source);
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected = (sender as DataGrid).SelectedItem;
            if (Selected == null || Selected is Secretary) return;

            if (Selected is Student)
            {
                var group = (Selected as Student).Group;
                if (group == null) return;

                infoTb.Text = $"{group.Number}{group.Faculty?.Name}{group.AdditionalNumber?.ToString()}";
            }
            else if (Selected is Teacher)
            {
                var groups = (Selected as Teacher).Groups;
                if (groups == null) return;

                string str = String.Empty;
                foreach (var group in groups)
                {
                    str += $"{group.Number}{group.Faculty?.Name}{group.AdditionalNumber?.ToString()}; ";
                }
                infoTb.Text = str;
            }
            else if (Selected is Dean)
            {
                var faculties = (Selected as Dean).Faculties;
                if (faculties == null) return;

                string str = String.Empty;
                foreach (var faculty in faculties)
                {
                    str += $"{faculty.Name}; ";
                }
                infoTb.Text = str;
            }
        }
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            builder.Remove(dataGrid.SelectedItem);
            if (dataGrid.SelectedItem is Dean)
            {
                Data.Context.Deans.Remove(dataGrid.SelectedItem as Dean);
            }
            else if (dataGrid.SelectedItem is Secretary)
            {
                Data.Context.Secretaries.Remove(dataGrid.SelectedItem as Secretary);
            }
            else if (dataGrid.SelectedItem is Teacher)
            {
                Data.Context.Teachers.Remove(dataGrid.SelectedItem as Teacher);
            }
            Data.Context.SaveChanges();
            dataGrid.Items.Refresh();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNewItems();
            Data.Context.SaveChanges();
            dataGrid.Items.Refresh();
        }
        private void SaveNewItems()
        {
            foreach (var item in source)
            {
                var user = item as User;
                string login = (item as User).Phone;
                string password = Data.CreatePassword(login);
                string hash = Data.GetMd5Hash(new MD5Cng(), password);

                if (!(item is null))
                {
                    SendToUnregisteredUserMessage(user, login, password);
                    var unreg = Data.Context.Unregistered.Where(u => u.Phone == user.Phone).FirstOrDefault();
                    if (unreg != null)
                    {
                        Data.Context.Unregistered.Remove(unreg);
                    }
                }

                if (item is Student && (item as Student).Id == 0)
                {
                    Data.Context.Students.Add(item as Student);
                }
                else if (item is Teacher && (item as Teacher).Id == 0)
                {
                    var teacher = item as Teacher;
                    teacher.Login = login;
                    teacher.Password = hash;
                    Data.Context.Teachers.Add(teacher);
                }
                else if (item is Secretary && (item as Secretary).Id == 0)
                {
                    var secretary = item as Secretary;
                    secretary.Login = login;
                    secretary.Password = hash;
                    Data.Context.Secretaries.Add(secretary);
                }
                else if (item is Dean && (item as Dean).Id == 0)
                {
                    var dean = item as Dean;
                    dean.Login = login;
                    dean.Password = hash;
                    Data.Context.Deans.Add(dean);
                }
            }
            Data.Context.SaveChanges();
        }
        private void SendToUnregisteredUserMessage(User user, string login, string password)
        {
            Unregistered unregistered;
            if ((unregistered = Data.Context.Unregistered.Where(d => d.Phone == user.Phone).FirstOrDefault()) != null)
            {user.ChatId = unregistered.ChatId;
                user.Login = unregistered.Phone;
                Data.RegisterUserAsync(user);
                Data.Context.SaveChanges();
            }
        }
    }
    public enum UserType : byte
    {
        Student,
        Teacher,
        Secretary,
        Dean
    }
}
