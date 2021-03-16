using eDean.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using DeanDb;

namespace eDean.Tabs
{
    public partial class DeanInfoWindow : Window
    {
        DateTime from;
        DateTime to;
        Group group;
        InfoType? info;

        public DeanInfoWindow()
        {
            InitializeComponent();
            groupCb.ItemsSource = GetGroups();
            from = dateFrom.SelectedDate ?? DateTime.MinValue;
            to = dateTo.SelectedDate ?? DateTime.MinValue;
        }

        private IEnumerable GetGroups()
        {
            var groups = Data.Context.Groups.Include(g => g.Faculty).ToList();

            List<string> strings = new List<string>();
            foreach (var group in groups)
            {
                var str = group.ToString();
                if (!strings.Contains(str))
                    strings.Add(str);
            }
            return strings;
        }
        private void InfoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            info = (InfoType)(sender as ComboBox).SelectedIndex;

            if (info == InfoType.Skips)
            {
                skipsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                skipsPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void GroupCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ComboBox).SelectedItem;
            if (item == null) return;

            var value = item.ToString();
            var groups = Data.Context.Groups.Include(g => g.Faculty).ToList();
            foreach (var g in groups)
            {
                if (g.ToString() == value)
                {
                    group = g;
                    break;
                }
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            groupCb.SelectedItem = null;
            groupCb.IsEnabled = false;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            groupCb.IsEnabled = true;
        }
        private void SkipsTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
                e.Handled = true;
        }
        private void DateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dateFrom == null || dateTo == null)
                return;

            if (dateFrom.SelectedDate > dateTo.SelectedDate)
            {
                dateFrom.SelectedDate = from;
                MessageBox.Show("Дата начала периода не может быть позднее его окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            from = dateFrom.SelectedDate ?? DateTime.MinValue;
        }
        private void DateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dateFrom == null || dateTo == null)
                return;

            if (dateTo.SelectedDate < dateFrom.SelectedDate)
            {
                dateTo.SelectedDate = to;
                MessageBox.Show("Дата начала периода не может быть позднее его окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            to = dateTo.SelectedDate ?? DateTime.MinValue;
        }
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if ((group == null && allGroupsCb.IsChecked == false) || info == null)
                return;

            IList list;

            switch (info)
            {
                case InfoType.Skips:
                    int countSkips;
                    Int32.TryParse(skipsTb.Text, out countSkips);
                    List<SkipsInfo> skips = new List<SkipsInfo>();

                    if (allGroupsCb.IsChecked == true)
                        list = Data.Context.Skips.Include(s => s.Student).Where(s => s.Date >= from.Date && s.Date <= to.Date).ToList();
                    else
                        list = Data.Context.Skips.Include(s => s.Student.Group.Faculty).Where(s => s.Student.Group.Id == group.Id && s.Date >= from.Date && s.Date <= to.Date).ToList();

                    foreach (var item in list as IList<Skip>)
                    {
                        int index = -1;
                        for (int i = 0; i < skips.Count; i++)
                        {
                            if (skips[i].Student.Id == item.Student.Id)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            skips.Add(new SkipsInfo { Student = item.Student, CountSkips = SkipsInfo.GetDaySkips(item) });
                        }
                        else
                        {
                            skips[index].CountSkips += SkipsInfo.GetDaySkips(item);
                        }
                    }
                    skips = skips.Where(s => s.CountSkips >= countSkips).ToList();
                    dataGrid.ItemsSource = from s in skips
                                           select new
                                           {
                                               Студент = s.Student.ToString(),
                                               Прогулы = s.CountSkips
                                           };
                    break;
                case InfoType.NonAchievers:
                case InfoType.Progress:
                    List<MarksInfo> marks = new List<MarksInfo>();

                    if (allGroupsCb.IsChecked == true)
                        list = Data.Context.Marks.Include(s => s.Student).Where(s => s.Date >= from.Date && s.Date <= to.Date).ToList();
                    else
                        list = Data.Context.Marks.Include(s => s.Student.Group.Faculty).Where(s => s.Student.Group.Id == group.Id && s.Date >= from.Date && s.Date <= to.Date).ToList();

                    foreach (var item in list as List<Mark>)
                    {
                        int index = -1;
                        int value = (int)item.Value;
                        for (int i = 0; i < marks.Count; i++)
                        {
                            if (marks[i].Student.Id == item.Student.Id)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            var mark = new MarksInfo { Student = item.Student };
                            mark.Marks.Add(value);
                            marks.Add(mark);
                        }
                        else
                        {
                            marks[index].Marks.Add(value);
                        }
                    }

                    if (info == InfoType.Progress)
                    {
                        dataGrid.ItemsSource = from s in marks
                                               select new
                                               {
                                                   Студент = s.Student.ToString(),
                                                   Балл = Math.Round((double)s.Marks.Sum() / s.Marks.Count, 2)
                                               };
                    }
                    else
                    {
                        dataGrid.ItemsSource = (from s in marks
                                                select new
                                                {
                                                    Студент = s.Student.ToString(),
                                                    Балл = Math.Round((double)s.Marks.Sum() / s.Marks.Count, 2)
                                                }).Where(n => n.Балл < 3).ToList();
                    }

                    break;
            }
        }
        public class SkipsInfo
        {
            public Student Student { get; set; }
            public int CountSkips { get; set; }

            public static int GetDaySkips(Skip skip)
            {
                int counter = 0;
                if (skip.ZeroPair) counter++;
                if (skip.FirstPair) counter++;
                if (skip.SecondPair) counter++;
                if (skip.ThirdPair) counter++;
                if (skip.FourthPair) counter++;
                if (skip.FifthPair) counter++;

                return counter;
            }
        }
        class MarksInfo
        {
            public Student Student { get; set; }
            public List<int> Marks { get; set; } = new List<int>();
        }
    }

    public enum InfoType
    {
        Skips,
        NonAchievers,
        Progress
    }
}
