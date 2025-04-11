using System;
using System.Linq;
using UnityEditor;


namespace Honeylab.Utils.Editor
{
    public class TypeExistenceSymbolDefiner
    {
        private static readonly BuildTargetGroup[] TargetGroups =
        {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.Standalone
        };


        private readonly string _typeNameToLookFor;
        private readonly string _symbolWhenImported;


        public TypeExistenceSymbolDefiner(string typeNameToLookFor, string symbolWhenImported)
        {
            _typeNameToLookFor = typeNameToLookFor;
            _symbolWhenImported = symbolWhenImported;
        }


        public void Run()
        {
            bool isTypeImported = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Any(t => t.FullName.Equals(_typeNameToLookFor));

            foreach (BuildTargetGroup targetGroup in TargetGroups)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] oldSymbols);

                string[] newSymbols = oldSymbols;
                bool importedSymbolsWasDefined = Array.IndexOf(oldSymbols, _symbolWhenImported) >= 0;
                if (isTypeImported)
                {
                    if (!importedSymbolsWasDefined)
                    {
                        newSymbols = oldSymbols.Append(_symbolWhenImported).ToArray();
                    }
                }
                else
                {
                    if (importedSymbolsWasDefined)
                    {
                        newSymbols = oldSymbols.Where(s => !s.Equals(_symbolWhenImported)).ToArray();
                    }
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbols);
            }
        }
    }
}
