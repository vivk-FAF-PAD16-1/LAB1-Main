namespace Weather.SlotStorage
{
    public class RequestSlotStorage : ISlotStorage
    {
        private readonly int _maxCount;
        private int _counter;

        public RequestSlotStorage(int maxCount)
        {
            _counter = 0;
            _maxCount = maxCount;
        }
        
        public void Load()
        {
            _counter += 1;
        }

        public void Unload()
        {
            _counter -= 1;
        }

        public bool IsFull()
        {
            return _counter >= _maxCount;
        }
    }
}