using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microphone.Resiliency
{
    public abstract class Command<T>
    {
        public T Execute()
        {
            try
            {
                T res = Run();
                return res;
            }
            catch
            {
                throw;
            }
        }

        protected abstract T GetFallback();
        protected abstract T Run();

        public bool isResponseFromCache { get; protected set; }

        protected abstract T GetCacheKey();
    }
}
