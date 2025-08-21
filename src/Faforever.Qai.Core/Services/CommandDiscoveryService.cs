using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Faforever.Qai.Core.Commands;
using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Qmmands;
using DSharpPlus;

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
        var filteredCategories = new List<CommandCategory>();

        foreach (var category in allCommands)
        {
            var availableCommands = new List<CommandInfo>();
            
            foreach (var command in category.Commands)
            {
                if (IsCommandAvailableInContext(command, context))
                {
                    availableCommands.Add(command);
                }
            }

            if (availableCommands.Count > 0)
            {
                filteredCategories.Add(new CommandCategory
                {
                    Name = category.Name,
                    Commands = availableCommands
                });
            }
        }

        return filteredCategories;
    }

    private static bool IsCommandAvailableInContext(CommandInfo command, CustomCommandContext context)
    {
        // Check if command is available in current context (Discord vs IRC)
        if (!IsCommandAvailableForPlatform(command, context))
            return false;

        // Check permissions
        if (!HasRequiredPermissions(command, context))
            return false;

        return true;
    }

    private static bool IsCommandAvailableForPlatform(CommandInfo command, CustomCommandContext context)
    {
        // Discord context - show dual and discord commands
        if (context is DiscordCommandContext)
        {
            return command.ModuleType.IsSubclassOf(typeof(DualCommandModule)) ||
                   command.ModuleType.IsSubclassOf(typeof(DiscordCommandModule));
        }
        
        // IRC context - show dual and IRC commands
        if (context is IrcCommandContext)
        {
            return command.ModuleType.IsSubclassOf(typeof(DualCommandModule)) ||
                   command.ModuleType.IsSubclassOf(typeof(IrcCommandModule));
        }

        // Unknown context - show dual commands only
        return command.ModuleType.IsSubclassOf(typeof(DualCommandModule));
    }

    private static bool HasRequiredPermissions(CommandInfo command, CustomCommandContext context)
    {
        var method = command.Method;
        
        // Check RequireFafStaff attribute - hide these commands for now
        var fafStaffAttr = method.GetCustomAttribute<RequireFafStaffAttribute>();
        if (fafStaffAttr != null)
        {
            return false; // Hide commands that require FAF staff permissions
        }

        // Check permission attributes - hide commands with specific permissions
        var permissionAttrs = method.GetCustomAttributes(typeof(IPermissionsAttribute), true)
            .Cast<IPermissionsAttribute>();
        
        foreach (var permAttr in permissionAttrs)
        {
            // Hide any command that has Discord or IRC permission requirements
            if (permAttr.DiscordPermissions.HasValue || permAttr.IRCPermissions.HasValue)
            {
                return false;
            }
        }

        return true; // Show command only if it has no permission requirements
    }

    private static List<CommandCategory> DiscoverCommands()
    {
        var categories = new Dictionary<string, CommandCategory>();
        var commandModuleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => (t.IsSubclassOf(typeof(DualCommandModule)) || 
                        t.IsSubclassOf(typeof(DiscordCommandModule)) || 
                        t.IsSubclassOf(typeof(IrcCommandModule))) && 
                        !t.IsAbstract)
            .ToList();

        foreach (var moduleType in commandModuleTypes)
        {
            var categoryName = GetCategoryFromModuleType(moduleType);
            
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
                        Aliases = aliases,
                        ModuleType = moduleType,
                        Method = method
                    };

                    categories[categoryName].Commands.Add(commandInfo);
                }
            }
        }

        return categories.Values.OrderBy(c => c.Name).ToList();
    }

    private static string GetCategoryFromModuleType(Type moduleType)
    {
        // Determine platform-specific categorization
        if (moduleType.IsSubclassOf(typeof(DiscordCommandModule)))
        {
            return GetCategoryFromNamespace(moduleType.Namespace, "Discord");
        }
        else if (moduleType.IsSubclassOf(typeof(IrcCommandModule)))
        {
            return GetCategoryFromNamespace(moduleType.Namespace, "IRC");
        }
        else if (moduleType.IsSubclassOf(typeof(DualCommandModule)))
        {
            return GetCategoryFromNamespace(moduleType.Namespace, null);
        }

        return "General";
    }

    private static string GetCategoryFromNamespace(string? namespaceName, string? platformPrefix)
    {
        if (string.IsNullOrEmpty(namespaceName))
            return platformPrefix ?? "General";

        var parts = namespaceName.Split('.');
        if (parts.Length >= 2)
        {
            var lastPart = parts[^1]; // Get last part (e.g., "Fun", "Player", etc.)
            
            // Skip "Commands" if it exists in the path
            if (lastPart.Equals("Commands", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3)
            {
                lastPart = parts[^2];
            }
            
            return platformPrefix != null ? $"{platformPrefix} {lastPart}" : lastPart;
        }

        return platformPrefix ?? "General";
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
