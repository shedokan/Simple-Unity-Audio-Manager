using System;
using System.Reflection;
using UnityEngine;

namespace JSAM
{
    [Serializable]
    public class AssemblySelector
    {
        // The name we use at runtime to find the assembly. 
        // Can be a Simple Name ("MyAssembly") or a Full Name ("MyAssembly, Version=0.0.0.0, ...")
        [SerializeField] private string assemblyName = "Assembly-CSharp";
        public string AssemblyName
        {
            get => string.IsNullOrEmpty(assemblyName) ? "Assembly-CSharp" : assemblyName;
            set => assemblyName = value;
        }

#if UNITY_EDITOR
        // This is only for the Inspector selector; it won't exist in a build
        [SerializeField] public UnityEditorInternal.AssemblyDefinitionAsset assemblyAsset;
#endif

        public Assembly GetAssembly()
        {
            string name = AssemblyName;
            
            // On Android (IL2CPP), Assembly.Load often fails with just a simple name
            // Searching all loaded assemblies is more robust
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                if (a.GetName().Name == name)
                    return a;
            }

            try {
                return Assembly.Load(name);
            } catch {
                return null;
            }
        }
    }
}
