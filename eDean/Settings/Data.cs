using DeanDb;
using eDean.Settings.Commands;
using eDean.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using static eDean.Tabs.DeanInfoWindow;
using System.Data.Entity;

namespace eDean.Settings
{
    static class Data
    {
        static readonly string not_student = "Ваша студентская учетная запись не найдена!";
        public readonly static InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
        {
          new []
          {
              InlineKeyboardButton.WithCallbackData("Средний балл", "/average_mark"),
              InlineKeyboardButton.WithCallbackData("Прогулы", "/skips"),
          },
          new []
          {
              InlineKeyboardButton.WithCallbackData("Предметы и Преподаватели", "/subjects_n_teachers"),
          },
          new []
          {
              InlineKeyboardButton.WithCallbackData("Что нужно пересдать?", "/retakes"),
          }
        });
        const string token = "1618636814:AAGnwo1s7YmiGSEdRc3ybjyOUPTAgNpg3sE";
        private static List<Command> commandsList;

        public static DateTime Start { get; set; } = DateTime.Now;
        public static TelegramBotClient Bot { get; set; }
        public static IReadOnlyCollection<Command> Commands { get => commandsList; }
        public static DeanContext Context { get; set; }

        static Data()
        {
            commandsList = new List<Command>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new ForgetCommand());
            commandsList.Add(new MenuCommand());
            commandsList.Add(new HelpCommand());

            Bot = new TelegramBotClient(token);
            Bot.OnUpdate += Bot_OnUpdate;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery; ;
            Bot.StartReceiving();
        }

        private static async void Bot_OnUpdate(object sender, UpdateEventArgs e)
        {
            if (e.Update.Type != Telegram.Bot.Types.Enums.UpdateType.MessageUpdate)
                return;

            if (e.Update.Message.Date.ToUniversalTime() < Start.ToUniversalTime())
                return;

            var message = e.Update.Message;

            if (message?.Type == Telegram.Bot.Types.Enums.MessageType.ContactMessage)
            {
                RegisterUserAsync(message);
                return;
            }

            bool is_executed = false;
            await Task.Run(new Action(() =>
            {
                foreach (var command in Commands)
                {
                    if (command.Contains(message.Text))
                    {
                        command.Execute(message, Bot);
                        is_executed = true;
                        break;
                    }
                }
            }));

            if (!is_executed)
            {
                Commands.Last().Execute(message, Bot);
            }
        }
        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            string text = "Ошибка.";
            int chatId = e.CallbackQuery.From.Id;

            switch (e.CallbackQuery.Data)
            {
                case "/average_mark":
                    text = await GetAverageMark(chatId);
                    break;
                case "/skips":
                    text = await GetSkips(chatId);
                    break;
                case "/subjects_n_teachers":
                    text = await GetCourses(chatId);
                    break;
                case "/retakes":
                    text = await GetRetakes(chatId);
                    break;
                default:
                    break;
            }
            await Bot.SendTextMessageAsync(chatId, text);
        }

        public static async void RegisterUserAsync(Message message)
        {
            if (message.Contact.UserId != message.Chat.Id)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Неправильный номер телефона!\nПопробуйте еще раз.");
            }
            else
            {
                DeanDb.User user;
                string phone = ValidatePhoneNumber(message.Contact.PhoneNumber);
                if ((user = Context.Deans.Where(d => d.Phone == phone).FirstOrDefault()) != null ||
                    (user = Context.Secretaries.Where(s => s.Phone == phone).FirstOrDefault()) != null ||
                    (user = Context.Teachers.Where(t => t.Phone == phone).FirstOrDefault()) != null ||
                    (user = Context.Students.Where(s => s.Phone == phone).FirstOrDefault()) != null)
                {
                    if (user.ChatId == 0)
                    {
                        if (!(user is Student))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, $"Вы были успешно зарегистрированы!\nВаш логин: {user.Login}\nВаш пароль: {Data.CreatePassword(user.Login)}");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, $"Вы были успешно зарегистрированы!", replyMarkup: InlineKeyboard);
                        }
                    }
                    else
                    {
                        if (user is Student)
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Вы уже зарегистрированы!", replyMarkup: InlineKeyboard);
                        else
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Вы уже зарегистрированы!");
                    }

                    user.ChatId = message.Chat.Id;
                }
                else
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, "На данный момент Вы не зарегистрированы в системе.\n\nОбратитесь в службу поддержки.");
                    if (Data.Context.Unregistered.Where(u => u.Phone == phone).FirstOrDefault() == null)
                    {
                        Data.Context.Unregistered.Add(new Unregistered
                        {
                            ChatId = message.Contact.UserId,
                            Name = message.Contact.FirstName,
                            LastName = message.Contact.LastName,
                            Phone = phone
                        });
                        if (MainWindow.User != null && (MainWindow.User is Dean || MainWindow.User is Secretary))
                            await Application.Current.Dispatcher.BeginInvoke(new Action(() => new NotificationWindow(message).Show()));
                    }
                }
                Context.SaveChanges();
            }
        }
        public static async void RegisterUserAsync(DeanDb.User user)
        {
            string phone = ValidatePhoneNumber(user.Phone);
            if (user.ChatId != 0)
            {
                if (!(user is Student))
                {
                    await Bot.SendTextMessageAsync(user.ChatId, $"Вы были успешно зарегистрированы!\nВаш логин: {user.Login}\nВаш пароль: {Data.CreatePassword(user.Login)}");
                }
                else
                {
                    await Bot.SendTextMessageAsync(user.ChatId, $"Вы были успешно зарегистрированы!", replyMarkup: InlineKeyboard);
                }
            }
            Context.SaveChanges();
        }

        private static async Task<string> GetAverageMark(long chatId)
        {
            var student = Context.Students.Where(s => s.ChatId == chatId).FirstOrDefault();
            string results = not_student;
            if (student != null)
            {
                var week = GetDateRange(DateTime.Today, DateRangeType.Week);
                var month = GetDateRange(DateTime.Today, DateRangeType.Month);
                var semester = GetDateRange(DateTime.Today, DateRangeType.Semester);

                var marks = Data.Context.Marks.Where(m => m.StudentId == student.Id && m.Date > semester.From && m.Date <= semester.To && m.Value != null).ToList();

                results = "Средние баллы:\n\n";
                await Task.Run(new Action(() =>
                {
                    results += $"За текущий семестр {semester.From.ToShortDateString()} - {semester.To.ToShortDateString()}:" +
                                       $" {Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2)}\n\n";

                    marks = marks.Where(m => m.Date > month.From && m.Date <= month.To).ToList();
                    results += $"За текущий месяц {month.From.ToShortDateString()} - {month.To.ToShortDateString()}:" +
                        $" {Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2)}\n\n";

                    marks = marks.Where(m => m.Date > week.From && m.Date <= week.To).ToList();
                    results += $"За текущую неделю {week.From.ToShortDateString()} - {week.To.ToShortDateString()}:" +
                        $" {Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2)}";
                }));
            }
            return results;
        }
        private static async Task<string> GetSkips(long chatId)
        {
            var student = Context.Students.Where(s => s.ChatId == chatId).FirstOrDefault();
            string results = not_student;
            if (student != null)
            {
                var week = GetDateRange(DateTime.Today, DateRangeType.Week);
                var month = GetDateRange(DateTime.Today, DateRangeType.Month);
                var semester = GetDateRange(DateTime.Today, DateRangeType.Semester);

                var skips = Data.Context.Skips.Where(s => s.StudentId == student.Id && s.Date >= semester.From && s.Date <= semester.To).ToList();

                int count = 0;
                foreach (var skip in skips)
                {
                    count += SkipsInfo.GetDaySkips(skip);
                }

                results = "Прогулы:\n\n";
                await Task.Run(new Action(() =>
                {
                    results += $"За текущий семестр {semester.From.ToShortDateString()} - {semester.To.ToShortDateString()}:" +
                        $" {count}\n\n";
                    count = 0;

                    skips = skips.Where(m => m.Date > month.From && m.Date <= month.To).ToList();
                    foreach (var skip in skips)
                    {
                        count += SkipsInfo.GetDaySkips(skip);
                    }
                    results += $"За текущий месяц {month.From.ToShortDateString()} - {month.To.ToShortDateString()}:" +
                        $" {count}\n\n";
                    count = 0;

                    skips = skips.Where(m => m.Date > week.From && m.Date <= week.To).ToList();
                    foreach (var skip in skips)
                    {
                        count += SkipsInfo.GetDaySkips(skip);
                    }
                    results += $"За текущую неделю {week.From.ToShortDateString()} - {week.To.ToShortDateString()}:" +
                        $" {count}";
                }));
            }
            return results;
        }
        private static async Task<string> GetCourses(long chatId)
        {
            var student = Context.Students.Where(s => s.ChatId == chatId).FirstOrDefault();
            string results = not_student;
            if (student != null)
            {
                var week = GetDateRange(DateTime.Today, DateRangeType.Week);
                var month = GetDateRange(DateTime.Today, DateRangeType.Month);
                var semester = GetDateRange(DateTime.Today, DateRangeType.Semester);

                var courses = Data.Context.Courses.Include(c => c.Subject).Include(c => c.Teacher).Where(c => c.GroupId == student.GroupId).ToList();

                results = "Предметы - Преподаватели - Средний балл (За неделю / месяц / семестр):\n\n";
                await Task.Run(new Action(() =>
                {
                    foreach (var course in courses)
                    {
                        var marks = Data.Context.Marks.Where(m => m.CourseId == course.Id && m.Date > semester.From
                        && m.Date <= semester.To && m.Value != null).ToList();
                        double semester_value = Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2);
                        marks = marks.Where(m => m.Date > month.From && m.Date <= month.To).ToList();
                        double month_value = Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2);
                        marks = marks.Where(m => m.Date > week.From && m.Date <= week.To).ToList();
                        double week_value = Math.Round((double)marks.Sum(m => m.Value) / marks.Count, 2);
                        results += $"{course.Subject.Name} - {course.Teacher.ToString()} - {week_value} / {month_value} / {semester_value}\n";
                    }
                }));
            }
            return results;
        }
        private static async Task<string> GetRetakes(long chatId)
        {
            var student = Context.Students.Where(s => s.ChatId == chatId).FirstOrDefault();
            string results = not_student;
            if (student != null)
            {
                var week = GetDateRange(DateTime.Today, DateRangeType.Week);
                var month = GetDateRange(DateTime.Today, DateRangeType.Month);
                var semester = GetDateRange(DateTime.Today, DateRangeType.Semester);

                var marks = Data.Context.Marks.Include(m => m.Course.Subject).Where(m => m.StudentId == student.Id && m.Date > semester.From && m.Date <= semester.To && m.Value != null).ToList();
                if (marks.Count > 0)
                {
                    results = "Список предметов которые нужно пересдать:\n";
                    await Task.Run(new Action(() =>
                    {
                        foreach (var mark in marks)
                        {
                            if (mark.Value <= 2)
                                results += $"- {mark.Course.Subject.Name}\n";
                        }
                    }));
                }
                else
                {
                    results = "У вас нет никаких задолженостей ;)";
                }
            }
            return results;
        }

        public static (DateTime From, DateTime To) GetDateRange(DateTime current, DateRangeType type)
        {
            switch (type)
            {
                case DateRangeType.Week:
                    return (current.AddDays(-7), current);
                case DateRangeType.Month:
                    return (current.AddMonths(-1), current);
                case DateRangeType.Semester:
                    if (current.Month >= 9 && current.Month <= 1)
                        return (new DateTime(current.Year, 9, 1), new DateTime(current.Year, 12, 31));
                    else
                        return (new DateTime(current.Year, 1, 1), new DateTime(current.Year, 8, 31));
                default:
                    break;
            }
            return (DateTime.MinValue, DateTime.MaxValue);
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        public static string CreatePassword(string login)
        {
            return $"{login}";
        }
        private static string ValidatePhoneNumber(string custom)
        {
            if (custom[0] == '+')
            {
                return custom;
            }
            else if (custom[0] == '3')
            {
                return "+" + custom;
            }
            else if (custom[0] == '8')
            {
                return "+3" + custom;
            }
            else
            {
                return "+38" + custom;
            }
        }
    }

    public enum DateRangeType
    {
        Week,
        Month,
        Semester
    }
}
