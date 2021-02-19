using BimTech.Core.CPlatform.Ids.Implementation;
using BimTech.Core.CPlatform.Runtime.Server.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BimTech.Core.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static List<Assembly> _referenceAssembly = new List<Assembly>();
        public static IServiceCollection AddRuntime(this IServiceCollection services)
        {
            var assemblys = GetReferenceAssembly();
            var types = assemblys.SelectMany(i => i.ExportedTypes).ToArray();
            new AttributeServiceEntryProvider(types, new ClrServiceEntryFactory(new DefaultServiceIdGenerator()));
            //需要注入DefaultServiceEntryManager 待优化
            return services;
        }

        private static List<Assembly> GetAssemblies(params string[] virtualPaths)
        {
            var referenceAssemblies = new List<Assembly>();
            if (virtualPaths.Any())
            {
                referenceAssemblies = GetReferenceAssembly(virtualPaths);
            }
            else
            {
                string[] assemblyNames = DependencyContext
                    .Default.GetDefaultAssemblyNames().Select(p => p.Name).ToArray();
                assemblyNames = GetFilterAssemblies(assemblyNames);
                foreach (var name in assemblyNames)
                    referenceAssemblies.Add(Assembly.Load(name));
                _referenceAssembly.AddRange(referenceAssemblies.Except(_referenceAssembly));
            }
            return referenceAssemblies;
        }

        private static string[] GetFilterAssemblies(string[] assemblyNames)
        {
            var notRelatedFile = "";
            var relatedFile = "";
            var pattern = string.Format("^Microsoft.\\w*|^System.\\w*|^DotNetty.\\w*|^runtime.\\w*|^ZooKeeperNetEx\\w*|^StackExchange.Redis\\w*|^Consul\\w*|^Newtonsoft.Json.\\w*|^Autofac.\\w*{0}",
               string.IsNullOrEmpty(notRelatedFile) ? "" : $"|{notRelatedFile}");
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex relatedRegex = new Regex(relatedFile, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(relatedFile))
            {
                return
                    assemblyNames.Where(
                        name => !notRelatedRegex.IsMatch(name) && relatedRegex.IsMatch(name)).ToArray();
            }
            else
            {
                return
                    assemblyNames.Where(
                        name => !notRelatedRegex.IsMatch(name)).ToArray();
            }
        }

        private static List<Assembly> GetReferenceAssembly(params string[] virtualPaths)
        {
            var refAssemblies = new List<Assembly>();
            var rootPath = AppContext.BaseDirectory;
            var existsPath = virtualPaths.Any();
            var result = _referenceAssembly;
            if (!result.Any() || existsPath)
            {
                var paths = virtualPaths.Select(m => Path.Combine(rootPath, m)).ToList();
                if (!existsPath) paths.Add(rootPath);
                paths.ForEach(path =>
                {
                    var assemblyFiles = GetAllAssemblyFiles(path);

                    foreach (var referencedAssemblyFile in assemblyFiles)
                    {
                        var referencedAssembly = Assembly.LoadFrom(referencedAssemblyFile);
                        if (!_referenceAssembly.Contains(referencedAssembly))
                            _referenceAssembly.Add(referencedAssembly);
                        refAssemblies.Add(referencedAssembly);
                    }
                    result = existsPath ? refAssemblies : _referenceAssembly;
                });
            }
            return result;
        }

        private static List<string> GetAllAssemblyFiles(string parentDir)
        {
            var notRelatedFile = "";
            var relatedFile = "";
            var pattern = string.Format("^Microsoft.\\w*|^System.\\w*|^Netty.\\w*|^Autofac.\\w*{0}",
               string.IsNullOrEmpty(notRelatedFile) ? "" : $"|{notRelatedFile}");
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex relatedRegex = new Regex(relatedFile, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(relatedFile))
            {
                return
                    Directory.GetFiles(parentDir, "*.dll").Select(Path.GetFullPath).Where(
                        a => !notRelatedRegex.IsMatch(a) && relatedRegex.IsMatch(a)).ToList();
            }
            else
            {
                return
                    Directory.GetFiles(parentDir, "*.dll").Select(Path.GetFullPath).Where(
                        a => !notRelatedRegex.IsMatch(Path.GetFileName(a))).ToList();
            }
        }
    }
}
