using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Colossal.Annotations;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Reflection;
using Colossal.UI.Binding;
using Game.Economy;
using Game.Reflection;
using Game.Simulation;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Debug;

public class DebugWatchSystem : GameSystemBase
{
	private class ManagedSystemState
	{
		public ComponentSystemBase m_System;

		public Foldout m_Foldout;
	}

	public abstract class Watch : IJsonWritable
	{
		public ComponentSystemBase m_System;

		public string m_DisplayName;

		public string m_Color;

		public abstract void Enable();

		public abstract void Disable();

		public abstract bool Advance(uint frameIndex);

		[CanBeNull]
		public static Watch TryCreate(IValueAccessor accessor, int historyLength, int updateInterval)
		{
			if (accessor is ITypedValueAccessor<int> accessor2)
			{
				return new HistoryWatch<int>(accessor2, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<int2> accessor3)
			{
				return new HistoryWatch<int2>(accessor3, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<int3> accessor4)
			{
				return new HistoryWatch<int3>(accessor4, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<float> accessor5)
			{
				return new HistoryWatch<float>(accessor5, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<float2> accessor6)
			{
				return new HistoryWatch<float2>(accessor6, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<float3> accessor7)
			{
				return new HistoryWatch<float3>(accessor7, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<uint> accessor8)
			{
				return new HistoryWatch<uint>(accessor8, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<uint2> accessor9)
			{
				return new HistoryWatch<uint2>(accessor9, historyLength, updateInterval);
			}
			if (accessor is ITypedValueAccessor<uint3> accessor10)
			{
				return new HistoryWatch<uint3>(accessor10, historyLength, updateInterval);
			}
			if (accessor.valueType == typeof(DebugWatchDistribution))
			{
				return new DistributionWatch(new CastAccessor<DebugWatchDistribution>(accessor), updateInterval);
			}
			return null;
		}

		public abstract void Write(IJsonWriter writer);
	}

	private class HistoryWatch<T> : Watch, IEquatable<HistoryWatch<T>>
	{
		public readonly ITypedValueAccessor<T> m_Accessor;

		private readonly uint[] m_HistoryFrames;

		private readonly T[] m_HistoryValues;

		private readonly int m_UpdateInterval;

		private readonly IWriter<T> m_Writer;

		private int m_LastHistoryIndex;

		public HistoryWatch(ITypedValueAccessor<T> accessor, int historyLength, int updateInterval, [CanBeNull] IWriter<T> writer = null)
		{
			m_Accessor = accessor;
			m_HistoryFrames = new uint[historyLength];
			m_HistoryValues = new T[historyLength];
			m_UpdateInterval = updateInterval;
			m_Writer = writer ?? ValueWriters.Create<T>();
		}

		public override void Enable()
		{
			ClearHistory();
		}

		public override void Disable()
		{
			ClearHistory();
		}

		public override bool Advance(uint frameIndex)
		{
			if (m_UpdateInterval > 0 && (frameIndex & (uint)(m_UpdateInterval - 1)) != 0)
			{
				return false;
			}
			m_HistoryFrames[m_LastHistoryIndex] = frameIndex;
			m_HistoryValues[m_LastHistoryIndex] = m_Accessor.GetTypedValue();
			m_LastHistoryIndex = (m_LastHistoryIndex + 1) % m_HistoryValues.Length;
			return true;
		}

		private void ClearHistory()
		{
			m_LastHistoryIndex = 0;
			for (int i = 0; i < m_HistoryFrames.Length; i++)
			{
				m_HistoryFrames[i] = 0u;
			}
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin("debug.HistoryWatch");
			writer.PropertyName("name");
			writer.Write(m_DisplayName);
			writer.PropertyName("color");
			writer.Write(m_Color);
			writer.PropertyName("history");
			JsonWriterExtensions.ArrayBegin(writer, m_HistoryValues.Length);
			for (int i = 0; i < m_HistoryValues.Length; i++)
			{
				int num = (i + m_LastHistoryIndex) % m_HistoryValues.Length;
				writer.TypeBegin("debug.WatchHistoryValue");
				writer.PropertyName("x");
				writer.Write(m_HistoryFrames[num]);
				writer.PropertyName("y");
				m_Writer.Write(writer, m_HistoryValues[num]);
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}

		public bool Equals(HistoryWatch<T> other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return object.Equals(m_Accessor, other.m_Accessor);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((HistoryWatch<T>)obj);
		}

		public override int GetHashCode()
		{
			if (m_Accessor == null)
			{
				return 0;
			}
			return m_Accessor.GetHashCode();
		}
	}

	private class DistributionWatch : Watch, IEquatable<DistributionWatch>
	{
		private struct DistributionBucket : IJsonWritable
		{
			public int m_Min;

			public int m_Max;

			public int m_Count;

			public void Write(IJsonWriter writer)
			{
				writer.TypeBegin("debug.DistributionBucket");
				writer.PropertyName("min");
				writer.Write(m_Min);
				writer.PropertyName("max");
				writer.Write(m_Max);
				writer.PropertyName("count");
				writer.Write(m_Count);
				writer.TypeEnd();
			}
		}

		private const int kBucketCount = 20;

		public readonly ITypedValueAccessor<DebugWatchDistribution> m_Accessor;

		private readonly DistributionBucket[] m_Buckets;

		private readonly int m_UpdateInterval;

		public DistributionWatch(ITypedValueAccessor<DebugWatchDistribution> accessor, int updateInterval)
		{
			m_Accessor = accessor;
			m_UpdateInterval = updateInterval;
			m_Buckets = new DistributionBucket[20];
		}

		public override void Enable()
		{
			m_Accessor.GetTypedValue()?.Enable();
		}

		public override void Disable()
		{
			m_Accessor.GetTypedValue()?.Dispose();
		}

		public override bool Advance(uint frameIndex)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdateInterval > 0 && (frameIndex & (uint)(m_UpdateInterval - 1)) != 0)
			{
				return false;
			}
			ClearHistory();
			DebugWatchDistribution typedValue = m_Accessor.GetTypedValue();
			if (typedValue == null)
			{
				return false;
			}
			JobHandle deps;
			NativeQueue<int> queue = typedValue.GetQueue(clear: false, out deps);
			((JobHandle)(ref deps)).Complete();
			int count = queue.Count;
			if (count != 0)
			{
				int num = int.MaxValue;
				int num2 = int.MinValue;
				for (int i = 0; i < count; i++)
				{
					int num3 = queue.Dequeue();
					num = Math.Min(num, num3);
					num2 = Math.Max(num2, num3);
					queue.Enqueue(num3);
				}
				int bucketMin;
				int bucketMax;
				int bucketSizeAndRange = GetBucketSizeAndRange(num, num2, out bucketMin, out bucketMax);
				for (int j = 0; j < m_Buckets.Length; j++)
				{
					m_Buckets[j].m_Count = 0;
					m_Buckets[j].m_Min = bucketMin + j * bucketSizeAndRange;
					m_Buckets[j].m_Max = bucketMin + (j + 1) * bucketSizeAndRange - 1;
				}
				for (int k = 0; k < count; k++)
				{
					int num4 = queue.Dequeue();
					int num5 = (num4 - bucketMin) / bucketSizeAndRange;
					m_Buckets[num5].m_Count++;
					if (typedValue.Persistent)
					{
						queue.Enqueue(num4);
					}
				}
				if (typedValue.Relative)
				{
					for (int l = 0; l < m_Buckets.Length; l++)
					{
						m_Buckets[l].m_Count = Mathf.RoundToInt(1000f * (float)m_Buckets[l].m_Count / (float)count);
					}
				}
			}
			else
			{
				ClearHistory();
			}
			return true;
		}

		public int GetBucketSizeAndRange(int min, int max, out int bucketMin, out int bucketMax)
		{
			int num = max - min + 1;
			int num2 = num / m_Buckets.Length;
			int num3 = num2 * m_Buckets.Length;
			if (num3 < num)
			{
				num2++;
				num3 = num2 * m_Buckets.Length;
			}
			bucketMin = min - (num3 - num) / 2;
			bucketMax = bucketMin + num3;
			return num2;
		}

		private void ClearHistory()
		{
			for (int i = 0; i < m_Buckets.Length; i++)
			{
				m_Buckets[i].m_Count = 0;
			}
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin("debug.DistributionWatch");
			writer.PropertyName("name");
			writer.Write(m_DisplayName);
			writer.PropertyName("color");
			writer.Write(m_Color);
			writer.PropertyName("buckets");
			JsonWriterExtensions.Write<DistributionBucket>(writer, (IList<DistributionBucket>)m_Buckets);
			writer.TypeEnd();
		}

		public bool Equals(DistributionWatch other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return object.Equals(m_Accessor, other.m_Accessor);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((DistributionWatch)obj);
		}

		public override int GetHashCode()
		{
			if (m_Accessor == null)
			{
				return 0;
			}
			return m_Accessor.GetHashCode();
		}
	}

	private static readonly string[] colors = new string[36]
	{
		"#000099", "#330099", "#660099", "#990000", "#CC0000", "#FF0000", "#003399", "#333399", "#663399", "#993300",
		"#CC3300", "#FF3300", "#006600", "#336600", "#666600", "#996600", "#CC6600", "#FF6600", "#009900", "#339900",
		"#669900", "#999900", "#CC9900", "#FF9900", "#00CC00", "#33CC00", "#66CC00", "#99CC00", "#CCCC00", "#FFCC00",
		"#00FF00", "#33FF00", "#66FF00", "#99FF00", "#CCFF00", "#FFFF00"
	};

	private SimulationSystem m_SimulationSystem;

	private List<ManagedSystemState> m_ManagedSystemStates;

	private List<Watch> m_Watches;

	private uint m_LastFrameIndex;

	private bool m_WatchesChanged;

	public List<Watch> watches => m_Watches;

	public bool watchesChanged => m_WatchesChanged;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ManagedSystemStates = new List<ManagedSystemState>();
		m_Watches = new List<Watch>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		foreach (ManagedSystemState entry in m_ManagedSystemStates)
		{
			entry.m_System.Enabled = entry.m_Foldout.opened || m_Watches.Any((Watch w) => w.m_System == entry.m_System);
		}
		if (m_SimulationSystem.frameIndex == m_LastFrameIndex)
		{
			return;
		}
		m_LastFrameIndex = m_SimulationSystem.frameIndex;
		foreach (Watch watch in m_Watches)
		{
			m_WatchesChanged |= watch.Advance(m_SimulationSystem.frameIndex);
		}
	}

	public void ClearWatchesChanged()
	{
		m_WatchesChanged = false;
	}

	public void ClearWatches()
	{
		m_Watches.Clear();
		m_WatchesChanged = true;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		foreach (ManagedSystemState managedSystemState in m_ManagedSystemStates)
		{
			managedSystemState.m_System.Enabled = false;
		}
	}

	public List<Widget> BuildSystemFoldouts()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Expected O, but got Unknown
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Expected O, but got Unknown
		//IL_040c: Expected O, but got Unknown
		m_ManagedSystemStates.Clear();
		int colorIndex = 0;
		List<Widget> list = new List<Widget>();
		foreach (ComponentSystemBase system in ((ComponentSystemBase)this).World.Systems)
		{
			Type type = ((object)system).GetType();
			if (!GetWatchValueMembers(type).Any())
			{
				continue;
			}
			Foldout val = new Foldout
			{
				displayName = type.Name
			};
			list.Add((Widget)(object)val);
			UpdateSystem.GetInterval(system, SystemUpdatePhase.GameSimulation, out var interval, out var offset);
			ObjectWithDepsAccessor<object> parent = new ObjectWithDepsAccessor<object>(system, GetWatchDepsFields(type));
			foreach (MemberInfo watchValueMember in GetWatchValueMembers(type))
			{
				DebugWatchValueAttribute customAttribute = watchValueMember.GetCustomAttribute<DebugWatchValueAttribute>();
				string text = WidgetReflectionUtils.NicifyVariableName(watchValueMember.Name);
				int updateInterval = ((customAttribute.updateInterval <= 0) ? interval : customAttribute.updateInterval);
				IValueAccessor valueAccessor = ValueAccessorUtils.CreateMemberAccessor(parent, watchValueMember);
				if (valueAccessor != null)
				{
					IValueAccessor valueAccessor2 = CreateTypedAccessor(valueAccessor);
					if (valueAccessor2 != null)
					{
						valueAccessor = valueAccessor2;
					}
				}
				Watch watch = Watch.TryCreate(valueAccessor, customAttribute.historyLength, updateInterval);
				if (watch != null)
				{
					val.opened |= m_Watches.Contains(watch);
					watch.m_System = system;
					watch.m_DisplayName = text;
					watch.m_Color = NextColor(customAttribute);
					((Container)val).children.Add(WatchToggle(text, watch));
				}
				else
				{
					((Container)val).children.Add((Widget)new Value
					{
						displayName = text,
						getter = () => ""
					});
				}
				Type type2 = valueAccessor?.valueType;
				if (type2 != null && type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(NativeArray<>))
				{
					object value = valueAccessor.GetValue();
					int num = math.min((int)type2.GetProperty("Length").GetGetMethod().Invoke(value, null), 100);
					Foldout val2 = new Foldout
					{
						displayName = "Values"
					};
					Foldout val3 = new Foldout
					{
						displayName = "Watches"
					};
					List<Watch> itemWatches = new List<Watch>();
					for (int num2 = 0; num2 < num; num2++)
					{
						string arrayItemName = GetArrayItemName(watchValueMember, num2);
						IValueAccessor accessor = ValueAccessorUtils.CreateNativeArrayItemAccessor(valueAccessor, num2);
						Watch watch2 = Watch.TryCreate(accessor, customAttribute.historyLength, updateInterval);
						((Container)val2).children.Add(Value(arrayItemName, accessor));
						if (watch2 != null)
						{
							itemWatches.Add(watch2);
							val2.opened |= m_Watches.Contains(watch2);
							watch2.m_System = system;
							watch2.m_DisplayName = text + " [" + arrayItemName + "]";
							watch2.m_Color = colors[colorIndex * 23 % colors.Length];
							offset = colorIndex;
							colorIndex = offset + 1;
							((Container)val3).children.Add(WatchToggle(arrayItemName, watch2));
						}
					}
					Container val4 = new Container();
					if (((Container)val2).children.Count > 0)
					{
						val4.children.Add((Widget)(object)val2);
					}
					if (((Container)val3).children.Count > 0)
					{
						val4.children.Add((Widget)(object)val3);
					}
					if (itemWatches.Count > 0)
					{
						ObservableList<Widget> children = val4.children;
						BoolField val5 = new BoolField
						{
							displayName = $"All Values ({itemWatches.Count})"
						};
						((Field<bool>)val5).getter = () => itemWatches.All((Watch w) => m_Watches.Contains(w));
						((Field<bool>)val5).setter = delegate(bool v)
						{
							m_Watches.RemoveAll((Watch w) => itemWatches.Contains(w));
							if (v)
							{
								m_Watches.AddRange(itemWatches);
							}
							m_WatchesChanged = true;
						};
						children.Add((Widget)val5);
					}
					if (val4.children.Count > 0)
					{
						((Container)val).children.Add((Widget)(object)val4);
					}
				}
				else
				{
					((Container)val).children.Add(ValueContainer("Value", valueAccessor));
				}
			}
			if (ReflectionUtils.HasAttribute<DebugWatchOnlyAttribute>(type, false))
			{
				system.Enabled = val.opened;
				m_ManagedSystemStates.Add(new ManagedSystemState
				{
					m_System = system,
					m_Foldout = val
				});
			}
		}
		list.Sort((Widget a, Widget b) => string.Compare(a.displayName, b.displayName, StringComparison.Ordinal));
		return list;
		string NextColor(DebugWatchValueAttribute attr = null)
		{
			if (attr.color != null)
			{
				return attr.color;
			}
			colorIndex++;
			return colors[colorIndex * 23 % colors.Length];
		}
		static Widget Value(string name, IValueAccessor valueAccessor3)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			return (Widget)new Value
			{
				displayName = name,
				getter = () => valueAccessor3?.GetValue(),
				refreshRate = 0.2f
			};
		}
		static Widget ValueContainer(string name, IValueAccessor accessor2)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			Container val6 = new Container();
			val6.children.Add(Value(name, accessor2));
			return (Widget)val6;
		}
		Widget WatchToggle(string name, Watch watch3)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0045: Expected O, but got Unknown
			BoolField val6 = new BoolField
			{
				displayName = name
			};
			((Field<bool>)val6).getter = () => m_Watches.Contains(watch3);
			((Field<bool>)val6).setter = delegate(bool v)
			{
				if (v)
				{
					m_Watches.Add(watch3);
					watch3.Enable();
				}
				else
				{
					watch3.Disable();
					m_Watches.Remove(watch3);
				}
				m_WatchesChanged = true;
			};
			return (Widget)val6;
		}
	}

	private static IEnumerable<MemberInfo> GetWatchValueMembers(Type systemType)
	{
		return from m in systemType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where (m is PropertyInfo || m is FieldInfo || m is MethodInfo) && ReflectionUtils.HasAttribute<DebugWatchValueAttribute>(m, false)
			select m;
	}

	[CanBeNull]
	public static IValueAccessor CreateTypedAccessor(IValueAccessor accessor)
	{
		Type valueType = accessor.valueType;
		if (valueType == typeof(int))
		{
			return new CastAccessor<int>(accessor);
		}
		if (valueType == typeof(uint))
		{
			return new CastAccessor<uint>(accessor);
		}
		if (valueType == typeof(float))
		{
			return new CastAccessor<float>(accessor);
		}
		if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(NativeValue<>))
		{
			Type type = valueType.GenericTypeArguments[0];
			if (type == typeof(int))
			{
				return new NativeValueAccessor<int>(accessor);
			}
			if (type == typeof(uint))
			{
				return new NativeValueAccessor<uint>(accessor);
			}
			if (type == typeof(float))
			{
				return new NativeValueAccessor<float>(accessor);
			}
			if (type == typeof(int2))
			{
				return new NativeValueAccessor<int2>(accessor);
			}
			if (type == typeof(uint2))
			{
				return new NativeValueAccessor<uint2>(accessor);
			}
			if (type == typeof(float2))
			{
				return new NativeValueAccessor<float2>(accessor);
			}
			if (type == typeof(int3))
			{
				return new NativeValueAccessor<int3>(accessor);
			}
			if (type == typeof(uint3))
			{
				return new NativeValueAccessor<uint3>(accessor);
			}
			if (type == typeof(float3))
			{
				return new NativeValueAccessor<float3>(accessor);
			}
		}
		return accessor;
	}

	private static FieldInfo[] GetWatchDepsFields(Type systemType)
	{
		return (from f in systemType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where f.FieldType == typeof(JobHandle) && ReflectionUtils.HasAttribute<DebugWatchDepsAttribute>((MemberInfo)f, false)
			select f).ToArray();
	}

	private static string GetArrayItemName(MemberInfo member, int index)
	{
		if (ReflectionUtils.HasAttribute<ResourceArrayAttribute>(member, false))
		{
			return EconomyUtils.GetResource(index).ToString();
		}
		if (ReflectionUtils.HasAttribute<EnumArrayAttribute>(member, false))
		{
			return Enum.GetName(member.GetCustomAttribute<EnumArrayAttribute>().type, index);
		}
		return index.ToString();
	}

	[Preserve]
	public DebugWatchSystem()
	{
	}
}
