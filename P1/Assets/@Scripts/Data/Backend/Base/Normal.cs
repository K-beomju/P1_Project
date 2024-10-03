using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Reflection;

namespace BackendData.Base
{
    public class Normal
    {
        public delegate void AfterBackendLoadFuc(bool isSuccess, string className, string functionName, string errorInfo);

        public virtual void BackendLoad(AfterBackendLoadFuc afterBackendLoadFuc)
        {
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            afterBackendLoadFuc(true, className, funcName, string.Empty);
        }
    }
}