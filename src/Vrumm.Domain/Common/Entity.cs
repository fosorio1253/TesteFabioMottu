namespace Vrumm.Domain.Common;
public abstract class Entity<TId>
{
    public TId Id { get; protected set; }
    public DateTime CreationDate { get; protected set; }
    public DateTime UpdateDate { get; protected set; }

    protected Entity()
    {
        CreationDate = DateTime.UtcNow;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateModificationDate()
    {
        UpdateDate = DateTime.UtcNow;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id.Equals(default(TId)) || other.Id.Equals(default(TId)))
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id.ToString()).GetHashCode();
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
}