using System;
using System.Collections.Generic;
using System.Reflection;

namespace Faforever.Qai.Core.Models
{
    public class CommandInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Aliases { get; set; } = new List<string>();
        public Type ModuleType { get; set; } = null!;
        public MethodInfo Method { get; set; } = null!;
    }

    public class CommandCategory
    {
        public string Name { get; set; } = string.Empty;
        public List<CommandInfo> Commands { get; set; } = new List<CommandInfo>();
    }
}
