namespace NugetVisualizer.Core.Exceptions
{
    using System;

    /// <summary>
    /// indicates that packages cannot be parsed. If this exception is thrown, it means that also any subsequent package cannot be parsed, therefore it doesn't make sense to continue trying to parse packages.
    /// </summary>
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
