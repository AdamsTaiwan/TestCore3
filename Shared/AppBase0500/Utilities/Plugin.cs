using System;
using System.IO;
using System.Reflection;

namespace AppBase0500
{
    public static class PluginAny
    {
        /// <summary>
        /// Plugin any dll or exe and return instance without calling WireUp
        /// </summary>
        /// <param name="path"></param>
        /// <param name="plugin"></param>
        /// <returns>instance</returns>
        public static dynamic Load(string path, string plugin)
        {
            Assembly? assembly;
            Type type;

            var sa = plugin.Split('\\');  //added to support .dll in subfolders
            string typeName = sa[^1];// plugin; //.Substring(0, i);
            int i = plugin.LastIndexOf(".");// typeName.LastIndexOf("."); // plugin.Substring(i + 1);
            string assemblyName = plugin.Substring(0, i);// typeName.Substring(0, i);
            string assemblyFullname = $"{assemblyName}.dll";

            dynamic? instance;
            string f, f2;
            try
            {
                f = Path.Combine(path, assemblyFullname);
                if (File.Exists(f))
                {
                    assembly = Assembly.LoadFrom(f);
                }
                else
                {
                    f2 = f.Replace(".dll", ".exe");
                    if (File.Exists(f2))
                    {
                        assembly = Assembly.LoadFrom(f2);
                    }
                    else
                    {
                        throw new Exception($"Plugin not found: {Path.Combine(path, assemblyFullname)}");
                    }
                }
                type = assembly.GetType(typeName, true);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception($"Plugin not found: {Path.Combine(path, assemblyFullname)}{Environment.NewLine}{ex.Message}");
            }
            catch (TypeLoadException)
            {
                throw new Exception($"Plugin type not found: {plugin}");
            }
            try
            {
                instance = Activator.CreateInstance(type, null);
            }
            catch (Exception)
            {
                throw;
            }
            return instance;
        }

    }
}
