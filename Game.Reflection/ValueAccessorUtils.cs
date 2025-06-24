using System;
using System.Reflection;
using Colossal;
using Colossal.Annotations;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Reflection;

public static class ValueAccessorUtils
{
	[CanBeNull]
	public static IValueAccessor CreateMemberAccessor(IValueAccessor parent, MemberInfo member)
	{
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (member is FieldInfo fieldInfo)
		{
			if (fieldInfo.FieldType == typeof(NativePerThreadSumInt))
			{
				PropertyInfo property = typeof(NativePerThreadSumInt).GetProperty("Count");
				MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
				MethodInfo setMethod = property.GetSetMethod(nonPublic: true);
				return new PropertyAccessor(new ObjectAccessor<object>(fieldInfo.GetValue(parent.GetValue())), getMethod, setMethod);
			}
			return new FieldAccessor(parent, fieldInfo);
		}
		if (member is PropertyInfo propertyInfo)
		{
			MethodInfo getMethod2 = propertyInfo.GetGetMethod(nonPublic: true);
			MethodInfo setMethod2 = propertyInfo.GetSetMethod(nonPublic: true);
			return new PropertyAccessor(parent, getMethod2, setMethod2);
		}
		if (member is MethodInfo methodInfo)
		{
			ParameterInfo[] parameters = methodInfo.GetParameters();
			object[] array = new object[parameters.Length];
			int num = -1;
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.Name == "readOnly" && parameterInfo.ParameterType == typeof(bool))
				{
					array[i] = true;
					continue;
				}
				if (parameterInfo.ParameterType == typeof(JobHandle).MakeByRefType() && parameterInfo.IsOut)
				{
					if (num != -1)
					{
						Debug.LogWarning((object)$"Found multiple JobHandle out parameters in {methodInfo}");
						return null;
					}
					num = i;
					array[i] = (object)default(JobHandle);
					continue;
				}
				Debug.LogWarning((object)$"Unknown parameter: {parameterInfo}");
				return null;
			}
			return new GetterWithDepsAccessor(parent, methodInfo, array, num);
		}
		return null;
	}

	[CanBeNull]
	public static IValueAccessor CreateNativeArrayItemAccessor(IValueAccessor accessor, int index)
	{
		Type valueType = accessor.valueType;
		if (valueType == typeof(NativeArray<int>))
		{
			return new NativeArrayElementAccessor<int>(new CastAccessor<NativeArray<int>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<int2>))
		{
			return new NativeArrayElementAccessor<int2>(new CastAccessor<NativeArray<int2>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<int3>))
		{
			return new NativeArrayElementAccessor<int3>(new CastAccessor<NativeArray<int3>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<uint>))
		{
			return new NativeArrayElementAccessor<uint>(new CastAccessor<NativeArray<uint>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<uint2>))
		{
			return new NativeArrayElementAccessor<uint2>(new CastAccessor<NativeArray<uint2>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<uint3>))
		{
			return new NativeArrayElementAccessor<uint3>(new CastAccessor<NativeArray<uint3>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<float>))
		{
			return new NativeArrayElementAccessor<float>(new CastAccessor<NativeArray<float>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<float2>))
		{
			return new NativeArrayElementAccessor<float2>(new CastAccessor<NativeArray<float2>>(accessor), index);
		}
		if (valueType == typeof(NativeArray<float3>))
		{
			return new NativeArrayElementAccessor<float3>(new CastAccessor<NativeArray<float3>>(accessor), index);
		}
		return null;
	}
}
