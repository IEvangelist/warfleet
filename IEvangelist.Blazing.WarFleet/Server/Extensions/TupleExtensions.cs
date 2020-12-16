using System;

namespace IEvangelist.Blazing.WarFleet.Server.Extensions
{
    public static class TupleExtensions
    {
        static readonly Random _random = new((int)DateTime.Now.Ticks);

        public static T Random<T>(this (T First, T Second) tuple) =>
            _random.NextDouble() > .5 ? tuple.First : tuple.Second;
    }
}
