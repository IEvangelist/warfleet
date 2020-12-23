using Microsoft.AspNetCore.Components;
using System;
using Xunit;

namespace IEvangelist.Blazing.WarFleet.Client.Extensions.Tests
{
    public class NavigationManagerExtensionsTests
    {
        [Fact]
        public void TryGetQueryStringTest()
        {
            TestNavigationManager navManager = new();

            static void CorrectlyParsesQueryString<T>(
                NavigationManager nav,
                string key,
                bool expectedToGet,
                T expectedValue)
            {
                var result = nav.TryGetQueryString<T>(key, out var value);
                Assert.Equal(expectedToGet, result);
                Assert.Equal(expectedValue, value);
            }

            CorrectlyParsesQueryString(navManager, "str", true, "val");
            CorrectlyParsesQueryString(navManager, "num", true, 19);
            CorrectlyParsesQueryString(navManager, "dec", true, 7.7m);
            CorrectlyParsesQueryString(navManager, "foo", true, FooBarFizzBuzz.Fizz);
        }
    }

    public enum FooBarFizzBuzz { Foo, Bar, Fizz, Buzz };

    public class TestNavigationManager : NavigationManager
    {
        readonly string _baseUri = null!;
        readonly string _uri = null!;

        public TestNavigationManager(
            string baseUri = "http://example.com/",
            string uri = "http://example.com/link?str=val&num=19&dec=7.7&foo=2") =>
            (_baseUri, _uri) = (new Uri(baseUri).ToString(), uri);

        protected override void EnsureInitialized()
        {
            if (_baseUri is { Length: > 0 } && _uri is { Length: > 0 })
            {
                Initialize(_baseUri, _uri);
            }
        }

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            throw new System.NotImplementedException();
    }
}