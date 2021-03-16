using DeanDb;
using eDean.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Data.Entity;

namespace eDean.Tabs
{
    public partial class RetakesWindow : Window
    {
        Teacher teacher;
        IReadOnlyCollection<Subject> subjects;

        public RetakesWindow(Teacher teacher)
        {
            this.teacher = teacher;
            InitializeComponent();
            subjects = Data.Context.Courses.Include(c => c.Subject).Where(c => c.TeacherId == teacher.Id).Select(c => c.Subject).ToList();
            subjectCb.ItemsSource = subjects.Select(s => s.Name).ToList();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            if (String.IsNullOrWhiteSpace(audTb.Text))
            {
                error = true;
                audTb.BorderBrush = Brushes.Red;
            }
            if (String.IsNullOrWhiteSpace(subjectCb.Text) && allSubjectsCb.IsChecked != true)
            {
                error = true;
                subjectTb.Foreground = Brushes.Red;
            }
            if (dateTimePicker.SelectedDate == null)
            {
                error = true;
                dateTimePicker.BorderBrush = Brushes.Red;
            }

            if (error)
            {
                return;
            }

            List<Student> nonachievers = new List<Student>();
            string text = "Пересдача по ";
            var date = dateTimePicker.SelectedDate ?? DateTime.MinValue;
            text += allSubjectsCb.IsChecked == true ? "всем предметам" : $"предмету {subjectCb.Text}";
            text += "\n\n";
            text += $"Преподаватель: {teacher.ToString().Trim()}.\n";
            text += $"Корпус: {housingCb.Text}.\n";
            text += $"Аудитория: {audTb.Text}.\n";
            text += $"Дата: {date.ToShortDateString()}.";

            if (DateTime.TryParse(timePicker.Text, out DateTime time))
                text += $"\nВремя: {time.ToShortTimeString()}.";

            bool checker = allSubjectsCb.IsChecked == true;
            string name = subjectCb.Text;
            List<Mark> list;
            if (checker)
                list = Data.Context.Marks.Include(s => s.Student).Where(s => s.Student.ChatId != 0).ToList();
            else
                list = Data.Context.Marks.Include(s => s.Student).Include(s => s.Course.Subject)
                .Where(s => s.Student.ChatId != 0 && s.Course.Subject.Name == name).ToList();

            foreach (var item in list)
            {
                if (item.Value <= 2)
                {
                    nonachievers.Add(item.Student);
                }
            }
            nonachievers = nonachievers.Distinct().ToList();

            var result = MessageBox.Show($"Отправить уведомление:\n\n\"{text}\"\n\nвсем неуспевающим студентам?", "Отправка", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                var tasks = nonachievers.Select(x => Data.Bot.SendTextMessageAsync(x.ChatId, text));
                await Task.WhenAll(tasks);
                Close();
            }
        }

        private void AllSubjectsCb_Checked(object sender, RoutedEventArgs e)
        {
            subjectCb.SelectedItem = null;
            subjectCb.IsEnabled = false;
        }
        private void AllSubjectsCb_Unchecked(object sender, RoutedEventArgs e)
        {
            subjectCb.IsEnabled = true;
        }
    }
}
