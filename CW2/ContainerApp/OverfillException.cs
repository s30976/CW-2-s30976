using System;

namespace ContainerApp
{
    public class OverfillException : Exception
    {
        public OverfillException(string message) : base(message) { }
    }
}