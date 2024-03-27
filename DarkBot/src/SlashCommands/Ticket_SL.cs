﻿using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using DarkBot.src.Handler;
using DarkBot.src.Common;

namespace DarkBot.src.SlashCommands
{
    [SlashCommandGroup("ticket", "Slash Commands for the Ticketsystem.")]
    public class Ticket_SL : ApplicationCommandModule
    {
        [SlashCommand("system", "Erschaffe das Ticketsystem mit Buttons oder Dropdown Menu :)")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public static async Task Ticketsystem(InteractionContext ctx,
                                [Choice("Button", 0)]
                                [Choice("Dropdown Menu", 1)]
                                [Option("system", "Buttons oder Dropdown")] long systemChoice = 1)
        {
            if (!CmdShortener.CheckPermissions(ctx, Permissions.Administrator))
            {
                await CmdShortener.SendNotification(ctx, "Keinen Zugriff", "Du hast nicht die nötigen Rechte, um diesen Befehl auszuführen.", DiscordColor.Red, 0);
                return;
            }

            if (systemChoice == 0)
            {
                var embedTicketButtons = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.White)
                .WithTitle("**Ticketsystem**")
                .WithDescription("Klicke auf einen Button, um ein Ticket der jeweiligen Kategorie zu erstellen")
                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Success, "ticketSupportButton", "Support"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "ticketUnbanButton", "Entbannung"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "ticketDonationButton", "Spenden"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "ticketOwnerButton", "Inhaber"),
                    new DiscordButtonComponent(ButtonStyle.Success, "ticketApplyButton", "Bewerben")
                });

                var response = new DiscordInteractionResponseBuilder().AddEmbed(embedTicketButtons.Embeds[0]).AddComponents(embedTicketButtons.Components);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            }

            else if (systemChoice == 1)
            {
                var options = new List<DiscordSelectComponentOption>()
                {
                    new(
                        "Support",
                        "dd_TicketSupport",
                        "Allgemeine Probleme, Fragen, Wünsche und sonstiges!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":envelope:"))),

                    new(
                        "Entbannung",
                        "dd_TicketUnban",
                        "Duskutiere über einen Bann!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":tickets:"))),

                    new(
                        "Spenden",
                        "dd_TicketDonation",
                        "Ticket für Donations!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":moneybag:"))),

                    new(
                        "Inhaber",
                        "dd_TicketOwner",
                        "Dieses Ticket geht speziell an den Inhaber des Servers!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":factory_worker:"))),

                    new(
                        "Bewerben",
                        "dd_TicketApplication",
                        "Bewerbung für das Team!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":writing_hand:")))
                };

                var ticketDropdown = new DiscordSelectComponent("ticketDropdown", "Wähle eine passende Kategorie aus", options, false, 0, 1);

                var embedTicketDropdown = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()

                    .WithColor(DiscordColor.Goldenrod)
                    .WithTitle("**Ticketsystem**")
                    .WithDescription("Öffne das Dropdown Menü und wähle eine passende Kategorie aus, um ein Ticket deiner Wahl zu erstellen")
                    )
                    .AddComponents(ticketDropdown);

                await CmdShortener.SendAsEphemeral(ctx, "Ticketsystem erfolgreich geladen.");

                await ctx.Channel.SendMessageAsync(embedTicketDropdown);
            }
        }

        [SlashCommand("pogosystem", "Erschaffe das Ticketsystem für Pokemon Go")]
        public static async Task TicketsystemPOGO(InteractionContext ctx)
        {
            if (!CmdShortener.CheckPermissions(ctx, Permissions.Administrator))
            {
                await CmdShortener.SendNotification(ctx, "Keinen Zugriff", "Du hast nicht die nötigen Rechte, um diesen Befehl auszuführen.", DiscordColor.Red, 0);
                return;
            }

            var options = new List<DiscordSelectComponentOption>()
                {
                    new(
                        "Pokecoins",
                        "dd_TicketPokecoins",
                        "Order a Pokecoin Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Pokecoin1:"))),

                    new(
                        "Stardust",
                        "dd_TicketStardust",
                        "Order a Stardust Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Stardust:"))),

                    new(
                        "XP",
                        "dd_TicketXp",
                        "Order a XP Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Level40:")))
                };

            var ticketDropdown = new DiscordSelectComponent("ticketDropdown", "Choose a Ticket", options, false, 0, 1);

            var embedTicketDropdown = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.IndianRed)
                .WithTitle("Open Ticket To Buy Service")
                .WithDescription("Feel free to open a ticket if you want to know more about the services or if you want to order any Service.\n\n" +
                                 "All my Services are completely safe for your account. 3 Years no Ban/Strike")
                )
                .AddComponents(ticketDropdown);

            await CmdShortener.SendAsEphemeral(ctx, "Ticketsystem erfolgreich geladen.");

            await ctx.Channel.SendMessageAsync(embedTicketDropdown);
        }

        [SlashCommand("add", "Add a User to the Ticket")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public async Task Add(InteractionContext ctx,
                             [Option("User", "The user which will be added to the ticket")] DiscordUser user)
        {
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "User added!",
                Description = $"{user.Mention} has been added to the Ticket by {ctx.User.Mention}!\n",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.AddOverwriteAsync((DiscordMember)user, Permissions.AccessChannels);
        }

        [SlashCommand("remove", "Remove a User from the Ticket")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public async Task Remove(InteractionContext ctx,
                             [Option("User", "The user, which will be removed from the ticket")] DiscordUser user)
        {
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "User removed!",
                Description = $"{user.Mention} has been removed from the Ticket by {ctx.User.Mention}!\n",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.AddOverwriteAsync((DiscordMember)user, Permissions.None);
        }

        [SlashCommand("rename", "Change the Name of the Ticket")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public async Task Rename(InteractionContext ctx,
                             [Option("Name", "New Name of the Ticket")] string newChannelName)
        {
            await CheckIfChannelIsTicket(ctx);

            var oldChannelName = ctx.Channel.Mention;

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Ticket renamed!",
                Description = $"The Ticket {ctx.Channel.Mention} has been renamed by {ctx.User.Mention}!\n\n" +
                              $"New Ticket Name: ```{newChannelName}```",
                Timestamp = DateTime.UtcNow
            };

            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.ModifyAsync(properties => properties.Name = newChannelName);
        }

        [SlashCommand("close", "Close a Ticket")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public async Task Close(InteractionContext ctx)
        {
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "🔒 Ticket closed!",
                Description = $"The Ticket has been closed by {ctx.User.Mention}!\n" +
                              $"The Channel will be deleted in <t:{DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds()}:R>.",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            var messages = await ctx.Channel.GetMessagesAsync(999);

            var content = new StringBuilder();
            content.AppendLine($"Transcript Ticket {ctx.Channel.Name}:");
            foreach (var message in messages)
            {
                content.AppendLine($"{message.Author.Username} ({message.Author.Id}) - {message.Content}");
            }

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                var msg = await new DiscordMessageBuilder()
                    .AddFile("transript.txt", memoryStream)
                    .SendAsync(ctx.Guild.GetChannel(978669571483500574));
            }

            await Task.Delay(TimeSpan.FromSeconds(60));

            await ctx.Channel.DeleteAsync("Ticket closed");
        }

        private async Task<bool> CheckIfChannelIsTicket(InteractionContext ctx)
        {
            const ulong categoryId = 1207086767623381092;

            if (ctx.Channel.Parent.Id != categoryId || ctx.Channel.Parent == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("´´:warning:´´ **This Command is for Tickets only!**").AsEphemeral(true));

                return true;
            }

            return false;
        }
    }
}
