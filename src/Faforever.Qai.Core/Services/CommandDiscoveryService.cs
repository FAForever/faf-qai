using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Faforever.Qai.Core.Commands;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Qmmands;

namespace Faforever.Qai.Core.Services;

public class CommandDiscoveryService(IMemoryCache cache) : ICommandDiscoveryService
{
    private readonly IMemoryCache _cache = cache;
    private const string COMMANDS_CACHE_KEY = "all_commands";
    private const int CACHE_DURATION_MINUTES = 60;

    public List<CommandCategory> GetAllCommands()
    {
        if (_cache.TryGetValue(COMMANDS_CACHE_KEY, out List<CommandCategory>? cachedCommands))
            return cachedCommands ?? [];

        var commands = DiscoverCommands();
        _cache.Set(COMMANDS_CACHE_KEY, commands, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        return commands;
    }

    public List<CommandInfo> GetCommandsByCategory(string category)
    {
        var allCommands = GetAllCommands();
        var categoryCommands = allCommands.FirstOrDefault(c => 
            string.Equals(c.Name, category, StringComparison.OrdinalIgnoreCase));
        
        return categoryCommands?.Commands ?? [];
    }

    public CommandInfo? GetCommand(string commandName)
    {
        var allCommands = GetAllCommands();
        
        foreach (var category in allCommands)
        {
            var command = category.Commands.FirstOrDefault(c => 
                string.Equals(c.Name, commandName, StringComparison.OrdinalIgnoreCase) ||
                c.Aliases.Any(a => string.Equals(a, commandName, StringComparison.OrdinalIgnoreCase)));
            
            if (command != null)
                return command;
        }
        
        return null;
    }

    public List<CommandCategory> GetAvailableCommands(CustomCommandContext context)
    {
        var allCommands = GetAllCommands();
        return allCommands;
    }

    private static List<CommandCategory> DiscoverCommands()
    {
        var categories = new Dictionary<string, CommandCategory>();
        var commandModuleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(DualCommandModule)) && !t.IsAbstract)
            .ToList();

        foreach (var moduleType in commandModuleTypes)
        {
            var categoryName = GetCategoryFromNamespace(moduleType.Namespace);
            
            if (!categories.ContainsKey(categoryName))
                categories[categoryName] = new CommandCategory { Name = categoryName };

            var methods = moduleType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null);

            foreach (var method in methods)
            {
                var commandAttr = method.GetCustomAttribute<CommandAttribute>();
                var descriptionAttr = method.GetCustomAttribute<DescriptionAttribute>();
                
                if (commandAttr != null)
                {
                    var name = commandAttr.Aliases.FirstOrDefault() ?? method.Name;
                    var aliases = commandAttr.Aliases?.Where(a => !string.Equals(a, name, StringComparison.OrdinalIgnoreCase)).ToList() ?? [];

                    var commandInfo = new CommandInfo
                    {
                        Name = name,
                        Description = descriptionAttr?.Value ?? "No description available",
                        Usage = GenerateUsage(method, name),
                        Category = categoryName,
                        Aliases = aliases
                    };

                    categories[categoryName].Commands.Add(commandInfo);
                }
            }
        }

        return categories.Values.OrderBy(c => c.Name).ToList();
    }

    private static string GetCategoryFromNamespace(string? namespaceName)
    {
        if (string.IsNullOrEmpty(namespaceName))
            return "General";

        var parts = namespaceName.Split('.');
        if (parts.Length >= 2)
        {
            var lastPart = parts[^1]; // Get last part (e.g., "Fun", "Player", etc.)
            return lastPart;
        }

        return "General";
    }

    private static string GenerateUsage(MethodInfo method, string commandName)
    {
        var parameters = method.GetParameters()
            .Where(p => p.ParameterType != typeof(System.Threading.CancellationToken))
            .ToList();

        if (!parameters.Any())
            return $"!{commandName}";

        var usage = $"!{commandName}";
        
        foreach (var param in parameters)
        {
            var paramName = param.Name ?? "param";
            var hasRemainder = param.GetCustomAttribute<RemainderAttribute>() != null;
            var isOptional = param.HasDefaultValue || param.ParameterType.IsGenericType && 
                            param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (hasRemainder)
            {
                usage += isOptional ? $" [text...]" : $" <text...>";
            }
            else
            {
                usage += isOptional ? $" [{paramName}]" : $" <{paramName}>";
            }
        }

        return usage;
    }
}
