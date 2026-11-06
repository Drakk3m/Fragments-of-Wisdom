using System;
using NUnitLite;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("P9.6B fixture logic on .NET host only. Unity imports/APIs/Mono tests remain UNVERIFIED.");
        return new AutoRun().Execute(args);
    }
}
