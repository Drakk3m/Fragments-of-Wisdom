using System;
using Game.Slice;

namespace Game.SmokeTests
{
    public static class Program
    {
        public static int Main()
        {
            var log = FoundationVerticalSlice.Run();

            if (log.Events.Count == 0)
            {
                Console.Error.WriteLine("Foundation vertical slice produced no events.");
                return 1;
            }

            Console.WriteLine("Foundation vertical slice smoke test passed.");
            Console.WriteLine("Events: " + log.Events.Count);
            return 0;
        }
    }
}
