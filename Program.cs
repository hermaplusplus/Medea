using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace Medea
{
    class Program
    {
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;
            _client.Log += Log;

            var token = File.ReadAllText("token.txt");


            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandHandler(SocketMessage message)
        {
            string command = "";
            int lengthOfCommand = -1;

            if (!message.Content.StartsWith('?'))
                return Task.CompletedTask;

            if (message.Author.IsBot)
                return Task.CompletedTask;

            if (message.Content.Contains(' '))
                lengthOfCommand = message.Content.IndexOf(' ');
            else
                lengthOfCommand = message.Content.Length;

            command = message.Content.Substring(1, lengthOfCommand - 1).ToLower();

            if (command.Equals("hello"))
            {
                message.Channel.SendMessageAsync($@"Hello {message.Author.Mention}");
            }
            else if (command.Equals("age"))
            {
                message.Channel.SendMessageAsync($@"Your account was created at {message.Author.CreatedAt.DateTime.Date}");
            }
            else if ((command.Equals("meta") || command.Equals("metadata")) && message.Reference != null)
            {
                IMessage refmsg = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;
                if (refmsg.Attachments.Count > 0)
                {
                    IEnumerator<Discord.IAttachment> enumattach = refmsg.Attachments.GetEnumerator();
                    enumattach.MoveNext();
                    message.Channel.SendMessageAsync(
                        $@"Metadata for first attachment in the referenced message:
```
Filename: {enumattach.Current.Filename}
Width:    {enumattach.Current.Width}px
Height:   {enumattach.Current.Height}px
Size:     {(enumattach.Current.Size / (1024.0 * 1024)).ToString("0.#")}MB
```", false, null, null, null, new MessageReference(refmsg.Id));
                }
                else
                {
                    message.Channel.SendMessageAsync("This message does not have any attachments!", false, null, null, null, new MessageReference(message.Id));
                }
            }

            return Task.CompletedTask;
        }
    }
}
