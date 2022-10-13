namespace Weather.SlotStorage
{
    public interface ISlotStorage
    {
        void Load();
        void Unload();
        bool IsFull();
    }
}