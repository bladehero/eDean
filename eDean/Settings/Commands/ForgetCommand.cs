using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace eDean.Settings.Commands
{
    class ForgetCommand : Command
    {
        string text = "Вы не являетесь администрацией!";

        public override string Name => "/forget";

        public override async void Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            DeanDb.User user;
            if ((user = Data.Context.Teachers.Where(t => t.ChatId == chatId).FirstOrDefault()) != null ||
                (user = Data.Context.Secretaries.Where(t => t.ChatId == chatId).FirstOrDefault()) != null ||
                (user = Data.Context.Deans.Where(t => t.ChatId == chatId).FirstOrDefault()) != null)
            {
                string password = CreateRandomPassword();
                user.Password = Data.GetMd5Hash(new MD5Cng(), password);
                text = $"Ваша новая учетная запись:\n\nЛогин: {user.Login}\nНовый пароль: {password}";
                await bot.SendTextMessageAsync(chatId, text, replyToMessageId: messageId);
                Data.Context.SaveChanges();
            }
            else
            {
                await bot.SendTextMessageAsync(chatId, text, replyToMessageId: messageId);
            }
        }

        private string CreateRandomPassword()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString();
        }
    }
}
