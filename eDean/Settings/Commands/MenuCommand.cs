using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eDean.Settings.Commands
{
    class MenuCommand : Command
    {
        const string text = "Главное меню:";

        public override string Name => "/menu";

        public override async void Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            var student = Data.Context.Students.Where(s => s.ChatId == chatId).FirstOrDefault();
            if (student is null)
                await bot.SendTextMessageAsync(chatId, "У вас нет студенческой учетной записи!");
            else
                await bot.SendTextMessageAsync(chatId, text, replyToMessageId: messageId, replyMarkup: Data.InlineKeyboard);
        }
    }
}
