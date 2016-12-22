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

using NMSReflector.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace NMSReflector.Utils
{
    internal sealed class AnalysisHelper
    {
        public AnalysisHelper()
        {

        }

        public static void CreateCache(Type temp_Type)
        {

            ColumnAttribute temp_ColumnAttribute = null;
            Dictionary<string, Func<object, object>> GetDict = new Dictionary<string, Func<object, object>>();
            Dictionary<string, Action<object, object>> SetDict = new Dictionary<string, Action<object, object>>();
            Dictionary<string, string> MapDict = new Dictionary<string, string>();
            Dictionary<string, string> ReversionMapDict = new Dictionary<string, string>();
            Dictionary<string, Type> TypeDict = new Dictionary<string, Type>();
            Dictionary<string, MethodInfo> GetMethodInfoDict = new Dictionary<string, MethodInfo>();
            Dictionary<string, MethodInfo> SetMethodInfoDict = new Dictionary<string, MethodInfo>();
            Dictionary<string, FieldInfo> FieldInfoDict = new Dictionary<string, FieldInfo>();
            //拿到Emit反射方法
            WeakReference<EmitHelper> emitHandler = new WeakReference<EmitHelper>(new EmitHelper(temp_Type), false);
            EmitHelper tempNode = null;
            while (!emitHandler.TryGetTarget(out tempNode))
            {
                emitHandler.SetTarget(new EmitHelper(temp_Type));
                emitHandler.TryGetTarget(out tempNode);
            }

            #region 属性Emit
            PropertyInfo[] tPropertyList = temp_Type.GetProperties();

            string FlagName = string.Empty;

            int i_length = tPropertyList.Length;
            int i = 0;
            PropertyInfo temp_Property = null;
            while (i<i_length)
            {
                temp_Property = tPropertyList[i];

                FlagName = temp_Property.Name;
                GetDict[FlagName] = tempNode.EmitGetMethod(temp_Property);
                SetDict[FlagName] = tempNode.EmitSetMethod(temp_Property);
                TypeDict[FlagName] = temp_Property.PropertyType;
                ReversionMapDict[FlagName] = FlagName;
                GetMethodInfoDict[FlagName] = temp_Property.GetGetMethod(true);
                SetMethodInfoDict[FlagName] = temp_Property.GetSetMethod(true);
                //添加映射
                temp_ColumnAttribute = temp_Property.GetCustomAttribute<ColumnAttribute>();
                if (temp_ColumnAttribute != null)
                {
                    FlagName = temp_ColumnAttribute.Name;
                    //添加到映射字典
                    ReversionMapDict[temp_Property.Name] = FlagName;

                    //获取Get方法
                    GetDict[FlagName] = GetDict[temp_Property.Name];

                    //获取Set方法
                    SetDict[FlagName] = SetDict[temp_Property.Name];

                    //获取Type
                    TypeDict[FlagName] = TypeDict[temp_Property.Name];

                    GetMethodInfoDict[FlagName] = temp_Property.GetGetMethod(true);
                    SetMethodInfoDict[FlagName] = temp_Property.GetSetMethod(true);
                }
                MapDict[FlagName] = temp_Property.Name;
                i += 1;
            }
            #endregion

            #region 字段Emit
            FieldInfo[] tFieldList = temp_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            i = 0;
            i_length = tFieldList.Length;
            FieldInfo item_Field = null;
            while (i<i_length)
            {
                item_Field = tFieldList[i];
                FlagName = item_Field.Name;
                GetDict[FlagName] = tempNode.EmitGetMethod(item_Field);
                SetDict[FlagName] = tempNode.EmitSetMethod(item_Field);
                TypeDict[FlagName] = item_Field.FieldType;
                ReversionMapDict[FlagName] = FlagName;
                FieldInfoDict[FlagName] = item_Field;
                temp_ColumnAttribute = item_Field.GetCustomAttribute<ColumnAttribute>();
                if (temp_ColumnAttribute != null)
                {
                    FlagName = temp_ColumnAttribute.Name;
                    //添加到映射字典
                   
                    ReversionMapDict[item_Field.Name] = FlagName;

                    //获取Get方法
                    GetDict[temp_ColumnAttribute.Name] = GetDict[item_Field.Name];

                    //获取Set方法
                    SetDict[temp_ColumnAttribute.Name] = SetDict[item_Field.Name];

                    //获取Type
                    TypeDict[temp_ColumnAttribute.Name] = TypeDict[item_Field.Name];

                    FieldInfoDict[FlagName] = item_Field;
                }
                MapDict[FlagName] = item_Field.Name;
                
                i += 1;
            }
            #endregion

            TempCache.GetMethodCache[temp_Type] = GetDict;
            TempCache.SetMethodCache[temp_Type] = SetDict;
            TempCache.AttibuteNameCache[temp_Type] = MapDict;
            TempCache.ModelTypeCache[temp_Type] = TypeDict;
            TempCache.RealNameCache[temp_Type] = ReversionMapDict;
            TempCache.SetMethodInfoCache[temp_Type] = SetMethodInfoDict;
            TempCache.GetMethodInfoCache[temp_Type] = GetMethodInfoDict;
            TempCache.FieldInfoCache[temp_Type] = FieldInfoDict;

        }


    }
}
