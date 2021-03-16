using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eDean.Settings.Commands
{
    class StartCommand : Command
    {
        const string text = "   Доброго времени суток!\n\nДля дальнейшего использования ботом, отправьте свой телефон для регистрации.\nДля этого достаточно кликнуть по кнопке ниже \"Отправить номер\".";

        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                     {new KeyboardButton("Отправить номер") { RequestContact = true}}, true)
            { OneTimeKeyboard = true };

            await bot.SendTextMessageAsync(chatId, text, replyToMessageId: messageId, replyMarkup: RequestReplyKeyboard);
        }
    }
}
