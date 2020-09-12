using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools
{
    public static class ModelsVersionTools
    {
        public static List<string> GetVersionsNames()
        {
            return new List<string>() { typeof(Models.Hypermedia).AssemblyQualifiedName };
        }

        public static dynamic CreateVersion(string versionName)
        {
            var objectType = Type.GetType(versionName);
            dynamic toReturn = Convert.ChangeType(Activator.CreateInstance(objectType), objectType);
            return toReturn;
        }
    }
}
