using System;


namespace PngifyMe.Helpers
{
    public class SaveDispose<T> : IDisposable where T : IDisposable
    {
        public T Value { get; private set; }
        public bool Rendering { get; set; }
        public bool Disposed { get; private set; }

        public SaveDispose(T resource)
        {
            Value = resource;
        }

        public bool Dispose(bool dispose = true)
        {
            if (Rendering) return false;
            Value?.Dispose();
            Disposed = true;
            return Disposed;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
