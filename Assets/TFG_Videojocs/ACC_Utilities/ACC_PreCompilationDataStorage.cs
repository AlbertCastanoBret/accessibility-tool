using System.Collections.Generic;

namespace TFG_Videojocs.ACC_Utilities
{
    [System.Serializable]
    public class ACC_PreCompilationDataStorage
    {
        public List<ACC_KeyValuePairData<string, string>> keyValuePairs = new List<ACC_KeyValuePairData<string, string>>();
    }
}