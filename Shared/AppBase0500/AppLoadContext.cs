using System.Reflection;
using System.Runtime.Loader;

namespace AppBase0500
{
    public class AppLoadContext : AssemblyLoadContext
    {
        public AppLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }
    }
}
