using Example.Forms;
using Example.Handlers;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Ext.Building;
using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Ext.Features.Users.Models;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddTelegramBot("")
    .AddTelegramUsersInMemory<SimpleUser>()
    .AddTelegramMessagesInMemory<Message>()
    .AddTelegramForms<FormsFactory>();

var app = builder.Build();

app.Services
    .UseTelegramHandler(h => h
        .Use<UserConnectTelegramHandler>()
        .UseForms());

app.Run();