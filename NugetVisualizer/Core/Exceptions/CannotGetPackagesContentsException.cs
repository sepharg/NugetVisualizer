namespace NugetVisualizer.Core.Exceptions
{
    using System;

    public class CannotGetPackagesContentsException : Exception
    {
        public CannotGetPackagesContentsException()
        {
            
        }

        public CannotGetPackagesContentsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
