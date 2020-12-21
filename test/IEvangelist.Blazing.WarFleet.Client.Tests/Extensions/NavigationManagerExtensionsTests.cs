using Microsoft.AspNetCore.Components;
using Xunit;

namespace IEvangelist.Blazing.WarFleet.Client.Extensions.Tests
{
    public class NavigationManagerExtensionsTests
    {
        [Fact]
        public void TryGetQueryStringTest()
        {
            var navManager = new TestNavigationManager("link?str=val&num=19&dec=7.7&foo=2");

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
        public TestNavigationManager(string uri)
        {
            BaseUri = "https://example.com";
            Uri = uri;

            EnsureInitialized();
        }

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            throw new System.NotImplementedException();
    }
}