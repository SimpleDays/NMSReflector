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

using NMSReflector.Utils;
using NMSReflector.Cache;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NMSReflector
{
    public static class ModelOperator
    {
        /// <summary>
        /// 自动获取入口程序集，并生成Emit委托缓存
        /// </summary>
        public static void Create()
        {
            Create(Assembly.GetEntryAssembly().FullName);
        }
        /// <summary>
        /// 获取对应的入口程序集，并根据自己定义的接口名来创建emit缓存
        /// </summary>
        /// <param name="assembly">程序集全名</param>
        /// <param name="interfaceName">接口名</param>
        public static void Create(string assembly, string interfaceName = "INMSReflector")
        {
            Assembly assmbly = Assembly.Load(assembly);
            IEnumerator<Type> typeCollection = assmbly.ExportedTypes.GetEnumerator();
            Type temp_Type = null;
            while (typeCollection.MoveNext())
            {
                temp_Type = typeCollection.Current;
                if (temp_Type.IsClass && !temp_Type.IsAbstract)
                {
                    if (temp_Type.GetInterface(interfaceName) != null)
                    {
                        CreateModelCache(temp_Type);
                    }

                }
            }
        }

        /// <summary>
        /// 创建该类型的Emit缓存
        /// </summary>
        /// <param name="type">类型</param>
        public static void CreateModelCache(Type type)
        {
            if (!TempCache.ModelTypeCache.ContainsKey(type))
            {
                AnalysisHelper.CreateCache(type);
            }
        }
        /// <summary>
        /// 直接调用Set缓存委托
        /// </summary>
        /// <typeparam name="T">要操作的类型</typeparam>
        /// <param name="t">要操作的实例</param>
        /// <param name="propertyName">属性字段名</param>
        /// <param name="value">要赋的值</param>
        public static void Set<T>(T t, string propertyName, object value)
        {
            TempCache.SetMethodCache[typeof(T)][propertyName](t, value);
        }
        /// <summary>
        /// 直接调用Get缓存委托
        /// </summary>
        /// <typeparam name="T">要操作的类型</typeparam>
        /// <param name="t">要操作的实例</param>
        /// <param name="propertyName">属性字段名</param>
        /// <returns>对应的值</returns>
        public static object Get<T>(T t, string propertyName)
        {
            return TempCache.GetMethodCache[typeof(T)][propertyName](t);
        }


        /// <summary>
        /// 根据属性/字段名获取Type
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Type GetType<T>(string propertyName)
        {
            return GetType(typeof(T), propertyName);

        }
        public static Type GetType(Type type, string propertyName)
        {
            if (TempCache.ModelTypeCache[type].ContainsKey(propertyName))
            {
                return TempCache.ModelTypeCache[type][propertyName];
            }
            return null;
        }
        /// <summary>
        /// 返回真实的属性和字段名，因为属性字段可能会被打上标签，而且外部只识标签名，不知道标签下属性和字段的真实名
        /// 所以这个方法就提供了对应的映射
        /// </summary>
        /// <typeparam name="T">要操作的类型</typeparam>
        /// <param name="key">标签或者属性字段名</param>
        /// <returns>真实的属性字段名</returns>
        public static string GetRealName<T>(string key)
        {
            return GetRealName(typeof(T), key);
        }
        public static string GetRealName(Type type, string key)
        {
            if (TempCache.RealNameCache[type].ContainsKey(key))
            {
                return TempCache.RealNameCache[type][key];
            }
            else
            {
                return key;
            }
        }
        /// <summary>
        /// 如果这个类或者字段被打上了标签，那么通过这个方法获取到的就是标签的名。
        /// 如果没有打标签，那么返回的就是属性或者字段的名。
        /// </summary>
        /// <typeparam name="T">要操作类的类型</typeparam>
        /// <param name="key">属性或者字段名</param>
        /// <returns></returns>
        public static string GetAttributeName<T>(string key)
        {
            return GetAttributeName(typeof(T), key);
        }
        public static string GetAttributeName(Type type, string key)
        {
            if (TempCache.AttibuteNameCache[type].ContainsKey(key))
            {
                return TempCache.AttibuteNameCache[type][key];
            }
            else
            {
                return key;
            }
        }

        /// <summary>
        /// 获取对应类型的Get方法的反射输出委托缓存
        /// </summary>
        /// <typeparam name="T">要操作类的类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, Action<object, object>> GetSetCache<T>()
        {
            Type type = typeof(T);
            return TempCache.SetMethodCache[type];
        }
        /// <summary>
        /// 获取对应类型的Set方法的反射输出委托缓存
        /// </summary>
        /// <typeparam name="T">要操作类的类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, Func<object, object>> GetGetCache<T>()
        {
            Type type = typeof(T);
            return TempCache.GetMethodCache[type];
        }
        /// <summary>
        /// 获取对应类型的属性/字段 以及其类型的字典
        /// </summary>
        /// <typeparam name="T">要操作类的类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, Type> GetTypeCache<T>()
        {
            Type type = typeof(T);
            return TempCache.ModelTypeCache[type];
        }

        /// <summary>
        /// 返回的字典有标签名与属性字段名的对应关系，
        /// 如果该属性字段被打上了标签，那么标签会被作为key,而属性作为value.
        /// 如果没有打标签，那么属性字段作为key和value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>标签名/属性字段名 字典</returns>
        public static Dictionary<string, string> GetMapCache<T>()
        {
            Type type = typeof(T);
            return TempCache.AttibuteNameCache[type];
        }

    }


    //object类的扩展方法
    public static class ExtendObject
    {
        public static object EmitGet(this object instance, string propertyName)
        {
            return TempCache.GetMethodCache[instance.GetType()][propertyName](instance);
        }
        public static void EmitSet(this object instance, string propertyName, object value)
        {
            TempCache.SetMethodCache[instance.GetType()][propertyName](instance, value);
        }
    }

}
