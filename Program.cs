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
            _client.UserJoined += JoinHandler;
            _client.Log += Log;

            var token = File.ReadAllLines("token.txt")[0];


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
            else if (command.Equals("info") || command.Equals("user") || command.Equals("userinfo"))
            {
                if (message.MentionedUsers.Count > 0)
                {
                    IEnumerator<SocketUser> enummentions = message.MentionedUsers.GetEnumerator();
                    for (int i = 0; i < message.MentionedUsers.Count; i++)
                    {
                        enummentions.MoveNext();

                        EmbedBuilder emb = new EmbedBuilder
                        {
                            Title = $@"User Info for {enummentions.Current.Mention}"
                        };

                        emb
                            .AddField("Username & Tag", $@"{enummentions.Current.Username}#{enummentions.Current.Discriminator}", true)
                            .AddField("User ID", $@"{enummentions.Current.Id}", true)
                            .AddField("Avatar URL", $@"[Click Me!]({enummentions.Current.GetAvatarUrl()})", true)
                            .AddField("Date & Time Created", $@"<t:{enummentions.Current.CreatedAt.ToUnixTimeSeconds()}:F>")
                            .WithThumbnailUrl(enummentions.Current.GetAvatarUrl())
                            .WithAuthor(message.Author as IUser)
                            .WithCurrentTimestamp();

                        UserProperties userProperties = enummentions.Current.PublicFlags.Value;
                        string flagList = "";
                        if (!userProperties.HasFlag(UserProperties.None))
                        {
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.BugHunterLevel1) ? "Bug Hunter Level 1,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.BugHunterLevel2) ? "Bug Hunter Level 2,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.DiscordCertifiedModerator) ? "Certified Moderator,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.EarlySupporter) ? "Early Supporter,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.EarlyVerifiedBotDeveloper) ? "Early Verified Bot Dev,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.HypeSquadBalance) ? "HypeSquad Balance,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.HypeSquadBravery) ? "HypeSquad Bravery,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.HypeSquadBrilliance) ? "Hypesquad Brilliance,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.HypeSquadEvents) ? "HypeSquad Event Member,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.Partner) ? "Discord Partner Server Owner,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.Staff) ? "Discord Employee,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.System) ? "Discord System,\n" : "");
                            String.Concat(flagList, userProperties.HasFlag(UserProperties.VerifiedBot) ? "Verified Bot,\n" : "");
                            emb.AddField("Public Flags", flagList);
                        }

                        message.Channel.SendMessageAsync(null, false, emb.Build(), null, null, message.Reference);
                    }
                }

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
Size:     {(enumattach.Current.Size / (1024.0 * 1024)).ToString("0.##")}MB
```", false, null, null, null, new MessageReference(message.Id));
                }
                else
                {
                    message.Channel.SendMessageAsync("The message you referenced does not have any attachments!", false, null, null, null, new MessageReference(message.Id));
                }
            }

            return Task.CompletedTask;
        }

        private Task JoinHandler(SocketGuildUser user)
        {
            user.SendMessageAsync($@"Welcome to **{user.Guild.Name}**!");

            if (File.Exists($@"./res/guildjoin/{user.Guild.Id}.txt"))
            {
                user.SendMessageAsync(File.ReadAllText($@"./res/guildjoin/{user.Guild.Id}.txt"));
            }

            return Task.CompletedTask;
        }
    }
}
