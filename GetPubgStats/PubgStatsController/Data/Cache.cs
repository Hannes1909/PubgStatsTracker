using System;

namespace PubgStatsController.Data
{
    public class Cache<T>
    {
        private readonly DateTime end;
        private readonly T value;

        public Cache(T value, TimeSpan duration)
        {
            this.end = DateTime.Now + duration;

            this.value = value;
        }

        public T Value
        {
            get { return this.IsValid ? this.value : default(T); }
        }

        public bool IsValid
        {
            get { return this.end > DateTime.Now; }
        }
    }
}
