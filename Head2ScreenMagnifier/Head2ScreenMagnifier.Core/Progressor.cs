using System;
using System.Collections.Concurrent;

namespace Head2ScreenMagnifier.Core
{
    public static class Progressor
    {
        public static ConcurrentQueue<string> ConQueue = new ConcurrentQueue<string>();
    }
}