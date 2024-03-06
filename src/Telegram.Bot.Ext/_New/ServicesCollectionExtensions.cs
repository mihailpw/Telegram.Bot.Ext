using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Ext.Features.Forms.Factories;
using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Ext.Features.Users.Providers;
using Telegram.Bot.Ext.Features.Users.Repositories;

namespace Telegram.Bot.Ext.Building;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services, string token,
        Func<IServiceProvider, HttpClient>? httpClientFactory = default)
        => services
            .AddSingleton<ITelegramBotClient>(
                sp => new TelegramBotClient(token, httpClientFactory?.Invoke(sp)));

    #region Users

    public static IServiceCollection AddTelegramUsers<TUser>(this IServiceCollection services,
        Func<IServiceProvider, IUsersProvider> usersProviderFactory,
        Func<IServiceProvider, IUserRepository<TUser>> userRepositoryFactory)
        where TUser : IUser
        => services
            .AddSingleton(usersProviderFactory)
            .AddSingleton(userRepositoryFactory);

    public static IServiceCollection AddTelegramUsers<TUser>(this IServiceCollection services,
        Func<IServiceProvider, IUserRepository<TUser>> userRepositoryFactory)
        where TUser : IUser
        => services
            .AddSingleton<IUsersProvider, UsersProviderToRepositoryAdapter2<TUser>>()
            .AddSingleton(userRepositoryFactory);

    public static IServiceCollection AddTelegramUsersInMemory<TUser>(this IServiceCollection services)
        where TUser : IUser
        => services
            .AddSingleton<IUsersProvider, UsersProviderToRepositoryAdapter2<TUser>>()
            .AddSingleton<IUserRepository<TUser>, InMemoryUserRepository<TUser>>();

    #endregion

    #region Messages

    public static IServiceCollection AddTelegramMessages<TMessage>(this IServiceCollection services,
        Func<IServiceProvider, IMessagesRepository<TMessage>> messagesRepositoryFactory)
        where TMessage : IMessage
        => services
            .AddSingleton<IMessageRemover, MessageRemover>()
            .AddSingleton(messagesRepositoryFactory);

    public static IServiceCollection AddTelegramMessagesInMemory<TMessage>(this IServiceCollection services)
        where TMessage : IMessage
        => services
            .AddSingleton<IMessageRemover, MessageRemover>()
            .AddSingleton<IMessagesRepository<TMessage>, InMemoryMessagesRepository<TMessage>>();

    #endregion

    #region Forms

    public static IServiceCollection AddTelegramForms<TFormsFactory>(this IServiceCollection services)
        where TFormsFactory : class, IFormsFactory
        => services
            .AddSingleton<IFormsFactory, TFormsFactory>();

    #endregion
}