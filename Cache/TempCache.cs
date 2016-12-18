/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at
       http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NMSReflector.Cache
{
    public class TempCache
    {
        public static Dictionary<Type, Dictionary<string, Action<object, object>>> SetMethodCache;
        public static Dictionary<Type, Dictionary<string, Func<object, object>>> GetMethodCache;
        public static Dictionary<Type, Dictionary<string, string>> ModelPropertyMapCache;
        public static Dictionary<Type, Dictionary<string, string>> ModelPropertyReversionMapCache;
        public static Dictionary<Type, Dictionary<string, Type>> ModelTypeCache;
        public static Dictionary<Type, Dictionary<string, MethodInfo>> GetMethodInfoCache;
        public static Dictionary<Type, Dictionary<string, MethodInfo>> SetMethodInfoCache;
        public static Dictionary<Type, Dictionary<string, FieldInfo>> FieldInfoCache;
        
        static TempCache()
        {
            SetMethodCache = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
            GetMethodCache = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
            ModelPropertyMapCache = new Dictionary<Type, Dictionary<string, string>>();
            ModelPropertyReversionMapCache = new Dictionary<Type, Dictionary<string, string>>();
            ModelTypeCache = new Dictionary<Type, Dictionary<string, Type>>();
            FieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            GetMethodInfoCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();
            SetMethodInfoCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        }

        ~TempCache() {
            SetMethodCache.Clear();
            SetMethodCache = null;

            GetMethodCache.Clear();
            GetMethodCache = null;

            ModelPropertyMapCache.Clear();
            ModelPropertyMapCache = null;

            ModelPropertyReversionMapCache.Clear();
            ModelPropertyReversionMapCache = null;

            ModelTypeCache.Clear();
            ModelTypeCache = null;

            GetMethodInfoCache.Clear();
            GetMethodInfoCache = null;

            SetMethodInfoCache.Clear();
            SetMethodInfoCache = null;

            FieldInfoCache.Clear();
            FieldInfoCache = null;
        }

    }
}
