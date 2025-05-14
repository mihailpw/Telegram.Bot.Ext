using System.Diagnostics.CodeAnalysis;

namespace Telegram.Bot.Ext.Features.Users.Models;

public interface IUser
{
    long Id { get; }
    Group Group { get; }
}

public readonly struct Group : IEquatable<Group>
{
    public static readonly Group User = new("user");
    public static readonly Group Administrator = new("administrator");

    public Group(string name)
    {
        Name = name.Trim();
    }

    public string Name { get; }

    public override string ToString() => Name;

    #region Equality

    public static bool operator ==(Group obj1, Group obj2)
        => obj1.Equals(obj2);

    public static bool operator !=(Group obj1, Group obj2)
        => !obj1.Equals(obj2);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Group other && Equals(other);

    public bool Equals(Group other)
        => Name == other.Name;

    public override int GetHashCode()
        => Name.GetHashCode();

    #endregion
}
