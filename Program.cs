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
            else if (command.Equals("user") || command.Equals("userinfo"))
            {
                if (message.MentionedUsers.Count > 0)
                {
                    IEnumerator<SocketUser> enumMentions = message.MentionedUsers.GetEnumerator();
                    for (int i = 0; i < message.MentionedUsers.Count; i++)
                    {
                        enumMentions.MoveNext();

                        UserInfo(enumMentions.Current);
                    }
                }
                else { UserInfo(message.Author); }

                void UserInfo(SocketUser su)
                {
                    EmbedBuilder emb = new EmbedBuilder
                    {
                        //Title = $@"User Info for `{(message.Channel.GetUserAsync(message.Author.Id).Result as SocketGuildUser).Nickname}`"
                    };

                    SocketGuildUser guildUser = message.Channel.GetUserAsync(su.Id).Result as SocketGuildUser;

                    emb
                        //.AddField("Username & Tag", $@"{su.Username}#{su.Discriminator}", true)
                        .AddField("User ID", $@"`{su.Id}`", true)
                        //.AddField("Avatar URL", $@"[Click Me!]({su.GetAvatarUrl()})", true)
                        .AddField("Created At", $@"<t:{su.CreatedAt.ToUnixTimeSeconds()}:F>", true)
                        .AddField("Joined At", $@"<t:{(guildUser.JoinedAt ?? su.CreatedAt ).ToUnixTimeSeconds()}:F>", true)
                        .AddField("Bot?", $@"{guildUser.IsBot.ToString()}", true)
                        .WithThumbnailUrl(su.GetAvatarUrl())
                        .WithAuthor(su)
                        .WithCurrentTimestamp();

                    if (guildUser.Nickname != "" && guildUser.Nickname != null)
                        emb.AddField("Current Nickname", $@"{guildUser.Nickname}", true);

                    // WIP
                        /*string flagList = "";
                        if (!su.PublicFlags.Value.Equals(0))
                        {
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.BugHunterLevel1) ? "Bug Hunter Level 1,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.BugHunterLevel2) ? "Bug Hunter Level 2,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.DiscordCertifiedModerator) ? "Certified Moderator,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.EarlySupporter) ? "Early Supporter,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.EarlyVerifiedBotDeveloper) ? "Early Verified Bot Dev,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.HypeSquadBalance) ? "HypeSquad Balance,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.HypeSquadBravery) ? "HypeSquad Bravery,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.HypeSquadBrilliance) ? "Hypesquad Brilliance,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.HypeSquadEvents) ? "HypeSquad Event Member,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.Partner) ? "Discord Partner Server Owner,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.Staff) ? "Discord Employee,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.System) ? "Discord System,\n" : "");
                            String.Concat(flagList, su.PublicFlags.Value.HasFlag(UserProperties.VerifiedBot) ? "Verified Bot,\n" : "");
                            emb.AddField("Public Flags", flagList);
                        }*/

                    message.Channel.SendMessageAsync(null, false, emb.Build(), null, null, new MessageReference(message.Id));
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
