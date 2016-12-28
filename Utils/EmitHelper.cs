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
using System.Reflection;
using System.Reflection.Emit;

namespace NMSReflector.Utils
{
    internal sealed class EmitHelper
    {
        public Type EmitType;
        public EmitHelper(Type temp_Type)
        {
            EmitType = temp_Type;
        }
        /// <summary>
        /// 根据类的属性建立Set方法的委托。
        /// </summary>
        /// <param name="property">类的属性</param>
        /// <returns></returns>
        public Action<object, object> EmitSetMethod(PropertyInfo property)
        {
            MethodInfo method = property.GetSetMethod(true);

            if (method==null)
            {
                return null;
            }

            DynamicMethod newMethod = new DynamicMethod(property.Name + "_Setter",
                null,
                new Type[] { typeof(object), typeof(object) }
                );
            ILGenerator il = newMethod.GetILGenerator();

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, EmitType);
            }

            il.Emit(OpCodes.Ldarg_1);

            if (property.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, property.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, property.PropertyType);
            }
            if (!method.IsStatic)
            {
                il.EmitCall(OpCodes.Callvirt, method, null);
            }
            else
            {
                il.EmitCall(OpCodes.Call, method, null);
            }
            il.Emit(OpCodes.Ret);
            return (Action<object, object>)newMethod.CreateDelegate(typeof(Action<object, object>));
        }
        /// <summary>
        /// 根据类的属性建立Get方法的委托。
        /// </summary>
        /// <param name="property">类的属性</param>
        /// <returns></returns>
        public Func<object, object> EmitGetMethod(PropertyInfo property)
        {
            MethodInfo method = property.GetGetMethod(true);

            if (method == null)
            {
                return null;
            }
            DynamicMethod newMethod = new DynamicMethod(property.Name + "_Getter",
               typeof(object),
               new Type[] { typeof(object) }
               );

            ILGenerator il = newMethod.GetILGenerator();
            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, EmitType);
                il.EmitCall(OpCodes.Callvirt, method, null);
            }
            else
            {
                il.EmitCall(OpCodes.Call, method, null);
            }
            if (property.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, property.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)newMethod.CreateDelegate(typeof(Func<object, object>));
        }
        /// <summary>
        /// 根据类的字段建立Set方法的委托。
        /// </summary>
        /// <param name="field">类的字段</param>
        /// <returns></returns>
        public Action<object, object> EmitSetMethod(FieldInfo field)
        {
            DynamicMethod newMethod = new DynamicMethod(field.Name + "_Setter",
                null,
                new Type[] { typeof(object), typeof(object) }
                );
            ILGenerator il = newMethod.GetILGenerator();

            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, EmitType);
            }
            il.Emit(OpCodes.Ldarg_1);

            if (field.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, field.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, field.FieldType);
            }
            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Stfld, field);
            }
            else
            {
                il.Emit(OpCodes.Stsfld, field);
            }
            
            il.Emit(OpCodes.Ret);
            return (Action<object, object>)newMethod.CreateDelegate(typeof(Action<object, object>));
        }
        /// <summary>
        /// 根据类的字段建立Get方法的委托。
        /// </summary>
        /// <param name="field">类的字段</param>
        /// <returns></returns>
        public Func<object, object> EmitGetMethod(FieldInfo field)
        {

            DynamicMethod newMethod = new DynamicMethod(field.Name + "_Getter",
               typeof(object),
               new Type[] { typeof(object) }
               );

            ILGenerator il = newMethod.GetILGenerator();
            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, EmitType);
                il.Emit(OpCodes.Ldfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldsfld, field);
            }
            
            if (field.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, field.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)newMethod.CreateDelegate(typeof(Func<object, object>));
        }
    }
}
