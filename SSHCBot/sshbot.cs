using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Commands;
using System.Web;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Renci.SshNet;

namespace SSHCBot
{
    class LacunaBot
    {
        DiscordClient discord;

        public LacunaBot()
        {
            discord = new DiscordClient(x => {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            discord.UsingCommands(x => {
                x.PrefixChar = '$';
                x.AllowMentionPrefix = true;
            });
            var commands = discord.GetService<CommandService>();
            commands.CreateCommand("ip")
                .Parameter("ip", ParameterType.Optional)
                .Do(async (e) =>
                {
                    if (e.GetArg("ip") == "")
                    {
                        await e.Channel.SendMessage("The current IP is:" + Globals.ip);
                    }
                    else
                    {
                        Globals.ip = e.GetArg("ip");
                        await e.Channel.SendMessage(Globals.ip + " is the current IP!");
                    }
                });
            commands.CreateCommand("user")
                .Parameter("user", ParameterType.Optional)
                .Do(async (e) =>
                {
                    if (e.GetArg("user") == "")
                    {
                        await e.Channel.SendMessage("The current User is:" + Globals.ip);
                    }
                    else
                    {
                        Globals.user2 = e.GetArg("user");
                        await e.Channel.SendMessage(Globals.user2 + " is the current user!");
                    }
                });
            commands.CreateCommand("password")
                .Parameter("password", ParameterType.Optional)
                .Do(async (e) =>
                {
                        Globals.password = e.GetArg("password");
                        await e.Channel.SendMessage("Set password.");
                });
            commands.CreateCommand("help")
            .Do(async (e) => {
                await e.Channel.SendMessage("__**Hello I am SSHC!** I can connect to a server! :desktop:__ !\nHere are my commands:```css\n$ip 🔗 Set the IP for server connection(ex. $ip \"localhost\")\n$user 🔗 Set the User for server connection(ex. $user \"root\")\n$password 🔗 Set the Password for server connection(ex. $password \"tux\")\n$command 🔗 Send a command to the IP, User, and password provided via SSH!\n$current 🔗 Check current server Info.```\n\n Created by Static#3512");
            });
            commands.CreateCommand("command")
            .Parameter("commandargs")
            .Do(async (e) => {
                if (Globals.ip == "")
                {
                    await e.Channel.SendMessage("The Server's IP is not set!");
                }
                if(Globals.user2 == "")
                {
                    await e.Channel.SendMessage("The Server's User is not set!");
                }
                if(Globals.password == "")
                {
                    await e.Channel.SendMessage("The Server's Password is not set!");
                }
                using (var client = new SshClient(Globals.ip, Globals.user2, Globals.password))
                {
                    // Dont even connect if user is not administrator!
                    if(e.User.ServerPermissions.Administrator == true)
                    {
                        client.Connect();
                        if (client.IsConnected == true)
                        {
                            var cmd = client.CreateCommand(e.GetArg("commandargs"));
                            await e.Channel.SendMessage("Executing ```css\n" + e.GetArg("commandargs") + "```");
                            cmd.Execute();
                            var result = cmd.Result;
                            await e.Channel.SendMessage(result);
                        }
                        else {
                            await e.Channel.SendMessage("Failed to connect! Make sure your info. is correct.");
                        }
                    } else
                    {
                        await e.Channel.SendMessage(e.User.ToString() + ", you do not have access to command!");
                    }
                }
            });
            commands.CreateCommand("current")
                        .Do(async (e) => {
                            await e.Channel.SendMessage("**⇩ __Current Server Info__ ⇩**\n\n```" + Globals.ip.ToString() + "\n" + Globals.user2 + "\n" + Globals.password + "```");
                        });
            #region Token
            discord.ExecuteAndWait(async () => {
                // ENTER YOUR TOKEN!!! <============
                await discord.Connect("token", TokenType.Bot);
                discord.SetGame("ver. 2.0");
            });
            #endregion
        }
        public static class Globals
        {
            public static string ip = "";
            public static string user2 = "";
            public static string password = "";
        }
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}