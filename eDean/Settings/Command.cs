using Telegram.Bot;
using Telegram.Bot.Types;

namespace eDean.Settings
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract void Execute(Message message, TelegramBotClient bot);

        public bool Contains(string command)
        {
            if (command == null) return false;

            return command.Contains(this.Name);
        }
    }
}
