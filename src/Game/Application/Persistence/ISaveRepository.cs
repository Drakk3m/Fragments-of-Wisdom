namespace Game.Application.Persistence
{
    public interface ISaveRepository<T>
    {
        void Save(
            string slotId,
            T data);

        T Load(string slotId);

        bool Exists(string slotId);
    }
}
