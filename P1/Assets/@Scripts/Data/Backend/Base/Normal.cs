using System.Reflection;

namespace BackendData.Base
{
    public class Normal
    {
        public delegate void AfterBackendLoadFunc(bool isSuccess, string className, string functionName,
            string errorInfo);

        public virtual void BackendLoad(AfterBackendLoadFunc afterBackendLoadFuc)
        {
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            afterBackendLoadFuc(true, className, funcName, string.Empty);
        }
    }
}