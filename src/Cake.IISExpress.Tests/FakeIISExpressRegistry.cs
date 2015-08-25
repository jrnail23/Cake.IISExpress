using System;
using Cake.Core.IO;

namespace Cake.IISExpress.Tests
{
    public class FakeIISExpressRegistry : IRegistry
    {
        public IRegistryKey LocalMachine
        {
            get { return new FakeRegistryKey(); }
        }

        private class FakeRegistryKey : IRegistryKey
        {
            public void Dispose()
            {
            }

            public string[] GetSubKeyNames()
            {
                return new[] { "8.0", "10.0" };
            }

            public IRegistryKey OpenKey(string name)
            {
                if (name == @"SOFTWARE\Microsoft\IISExpress")
                {
                    return new FakeRegistryKey();
                }
                if (name == "10.0")
                {
                    return new FakeRegistryKey();
                }
                throw new NotSupportedException();
            }

            public object GetValue(string name)
            {
                if (name == "InstallPath")
                {
                    return @"c:\Program Files\IIS Express";
                }
                throw new NotSupportedException();
            }
        }
    }
}