using System;
using System.Collections.Concurrent;

namespace Head2ScreenMagnifier.Lib
{
    public static class Progressor
    {
        public static ConcurrentQueue<string> ConQueue = new ConcurrentQueue<string>();
    }
}