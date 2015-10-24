using System;
using System.Threading;

namespace Microphone.Core.Util
{
    public static class ThreadLocalRandom
    {
        private static int _seed = Environment.TickCount;

        private static ThreadLocal<Random> _rng = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        /// <summary>
        /// The current random number seed available to this thread
        /// </summary>
        public static Random Current
        {
            get
            {
                return _rng.Value;
            }
        }
    }
}
