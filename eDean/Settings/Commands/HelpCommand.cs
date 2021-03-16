using Telegram.Bot;
using Telegram.Bot.Types;

namespace eDean.Settings.Commands
{
    class HelpCommand :Command
    {
        const string text = "Доступные команды:\n/start - регистрация\n/menu - главное меню\n/forget - восстановление учетной записи (для администрации)\n/help - помощь";

        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            await bot.SendTextMessageAsync(chatId, text, replyToMessageId: messageId);
        }
    }
}
