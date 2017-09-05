using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core
{
    using NugetVisualizer.Core.Domain;

    public interface IProjectParser
    {
        Project ParseProject(IProjectIdentifier projectIdentifier);
    }
}
