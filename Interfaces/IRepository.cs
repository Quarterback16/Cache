using System;

namespace Cache.Interfaces
{
   public interface IRepository : IDisposable
   {
      bool IsDisposed { get; }
   }
}
