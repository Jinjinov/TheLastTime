namespace TheLastTime.Shared.Models
{
    public interface IEntity<T>
    {
        long Id { get; set; }

        void CopyTo(T destination);
    }
}
