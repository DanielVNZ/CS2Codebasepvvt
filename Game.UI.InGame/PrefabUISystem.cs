using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Tutorials;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PrefabUISystem : UISystemBase
{
	public interface IPrefabEffectBinder
	{
		bool Matches(EntityManager entityManager, Entity entity);

		void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity);
	}

	public class CityModifierBinder : IPrefabEffectBinder
	{
		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return ((EntityManager)(ref entityManager)).HasComponent<CityModifierData>(entity);
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<CityModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CityModifierData>(entity, true);
			CityModifierBinder.Bind<DynamicBuffer<CityModifierData>>(binder, buffer);
		}

		public static void Bind<T>(IJsonWriter binder, T cityModifiers) where T : INativeList<CityModifierData>
		{
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < ((IIndexable<CityModifierData>)cityModifiers).Length; i++)
			{
				if (IsValidModifierType(((INativeList<CityModifierData>)cityModifiers)[i].m_Type))
				{
					num++;
				}
			}
			binder.TypeBegin("prefabs.CityModifierEffect");
			binder.PropertyName("modifiers");
			JsonWriterExtensions.ArrayBegin(binder, num);
			for (int j = 0; j < ((IIndexable<CityModifierData>)cityModifiers).Length; j++)
			{
				CityModifierData data = ((INativeList<CityModifierData>)cityModifiers)[j];
				if (IsValidModifierType(data.m_Type))
				{
					binder.TypeBegin("prefabs.CityModifier");
					binder.PropertyName("type");
					binder.Write(Enum.GetName(typeof(CityModifierType), data.m_Type));
					binder.PropertyName("delta");
					binder.Write(ModifierUIUtils.GetModifierDelta(data.m_Mode, data.m_Range.max));
					binder.PropertyName("unit");
					binder.Write(GetModifierUnit(data));
					binder.TypeEnd();
				}
			}
			binder.ArrayEnd();
			binder.TypeEnd();
		}

		public static string GetModifierUnit(CityModifierData data)
		{
			if (data.m_Mode == ModifierValueMode.Absolute)
			{
				switch (data.m_Type)
				{
				case CityModifierType.DiseaseProbability:
				case CityModifierType.OfficeSoftwareEfficiency:
				case CityModifierType.IndustrialElectronicsEfficiency:
				case CityModifierType.CollegeGraduation:
				case CityModifierType.UniversityGraduation:
				case CityModifierType.IndustrialEfficiency:
				case CityModifierType.OfficeEfficiency:
				case CityModifierType.HospitalEfficiency:
				case CityModifierType.IndustrialFishInputEfficiency:
				case CityModifierType.IndustrialFishHubEfficiency:
					return "percentage";
				default:
					return ModifierUIUtils.GetModifierUnit(data.m_Mode);
				}
			}
			return ModifierUIUtils.GetModifierUnit(data.m_Mode);
		}

		private static bool IsValidModifierType(CityModifierType type)
		{
			return type != CityModifierType.CriminalMonitorProbability;
		}
	}

	public class LocalModifierBinder : IPrefabEffectBinder
	{
		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return ((EntityManager)(ref entityManager)).HasComponent<LocalModifierData>(entity);
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LocalModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<LocalModifierData>(entity, true);
			LocalModifierBinder.Bind<DynamicBuffer<LocalModifierData>>(binder, buffer);
		}

		public static void Bind<T>(IJsonWriter binder, T localModifiers) where T : INativeList<LocalModifierData>
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			binder.TypeBegin("prefabs.LocalModifierEffect");
			binder.PropertyName("modifiers");
			JsonWriterExtensions.ArrayBegin(binder, ((IIndexable<LocalModifierData>)localModifiers).Length);
			for (int i = 0; i < ((IIndexable<LocalModifierData>)localModifiers).Length; i++)
			{
				LocalModifierData localModifierData = ((INativeList<LocalModifierData>)localModifiers)[i];
				binder.TypeBegin("prefabs.LocalModifier");
				binder.PropertyName("type");
				binder.Write(Enum.GetName(typeof(LocalModifierType), localModifierData.m_Type));
				binder.PropertyName("delta");
				binder.Write(ModifierUIUtils.GetModifierDelta(localModifierData.m_Mode, localModifierData.m_Delta.max));
				binder.PropertyName("unit");
				binder.Write(ModifierUIUtils.GetModifierUnit(localModifierData.m_Mode));
				binder.PropertyName("radius");
				binder.Write(localModifierData.m_Radius.max);
				binder.TypeEnd();
			}
			binder.ArrayEnd();
			binder.TypeEnd();
		}
	}

	public class LeisureProviderBinder : IPrefabEffectBinder
	{
		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			LeisureProviderData leisureProviderData = default(LeisureProviderData);
			if (EntitiesExtensions.TryGetComponent<LeisureProviderData>(entityManager, entity, ref leisureProviderData))
			{
				return leisureProviderData.m_Efficiency > 0;
			}
			return false;
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			LeisureProviderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<LeisureProviderData>(entity);
			NativeList<LeisureProviderData> providers = default(NativeList<LeisureProviderData>);
			providers._002Ector(1, AllocatorHandle.op_Implicit((Allocator)2));
			providers.Add(ref componentData);
			LeisureProviderBinder.Bind<NativeList<LeisureProviderData>>(binder, providers);
		}

		public static void Bind<T>(IJsonWriter binder, T providers) where T : INativeList<LeisureProviderData>
		{
			binder.TypeBegin("prefabs.LeisureProviderEffect");
			binder.PropertyName("providers");
			JsonWriterExtensions.ArrayBegin(binder, ((IIndexable<LeisureProviderData>)providers).Length);
			for (int i = 0; i < ((IIndexable<LeisureProviderData>)providers).Length; i++)
			{
				LeisureProviderData leisureProviderData = ((INativeList<LeisureProviderData>)providers)[i];
				binder.TypeBegin("prefabs.LeisureProvider");
				binder.PropertyName("type");
				binder.Write(Enum.GetName(typeof(LeisureType), leisureProviderData.m_LeisureType));
				binder.PropertyName("efficiency");
				binder.Write(leisureProviderData.m_Efficiency);
				binder.TypeEnd();
			}
			binder.ArrayEnd();
			binder.TypeEnd();
		}
	}

	public interface IPrefabPropertyBinder
	{
		bool Matches(EntityManager entityManager, Entity entity);

		void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity);
	}

	public abstract class IntPropertyBinder : IPrefabPropertyBinder
	{
		public readonly string m_LabelId;

		public readonly string m_Unit;

		public readonly bool m_Signed;

		public readonly string m_Icon;

		public readonly string m_ValueIcon;

		protected IntPropertyBinder(string labelId, string unit, bool signed = false, string icon = null, string valueIcon = null)
		{
			m_LabelId = labelId;
			m_Unit = unit;
			m_Signed = signed;
			m_Icon = icon;
			m_ValueIcon = valueIcon;
		}

		public abstract bool Matches(EntityManager entityManager, Entity entity);

		public abstract int GetValue(EntityManager entityManager, Entity entity);

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			JsonWriterExtensions.Write<IntProperty>(binder, new IntProperty
			{
				labelId = m_LabelId,
				unit = m_Unit,
				value = GetValue(entityManager, entity),
				signed = m_Signed,
				icon = m_Icon,
				valueIcon = m_ValueIcon
			});
		}
	}

	public abstract class IntRangePropertyBinder : IPrefabPropertyBinder
	{
		public readonly string m_LabelId;

		public readonly string m_Unit;

		public readonly bool m_Signed;

		public readonly string m_Icon;

		public readonly string m_ValueIcon;

		protected IntRangePropertyBinder(string labelId, string unit, bool signed = false, string icon = null, string valueIcon = null)
		{
			m_LabelId = labelId;
			m_Unit = unit;
			m_Signed = signed;
			m_Icon = icon;
			m_ValueIcon = valueIcon;
		}

		public abstract bool Matches(EntityManager entityManager, Entity entity);

		public abstract int GetMinValue(EntityManager entityManager, Entity entity);

		public abstract int GetMaxValue(EntityManager entityManager, Entity entity);

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			JsonWriterExtensions.Write<IntRangeProperty>(binder, new IntRangeProperty
			{
				labelId = m_LabelId,
				unit = m_Unit,
				minValue = GetMinValue(entityManager, entity),
				maxValue = GetMaxValue(entityManager, entity),
				signed = m_Signed,
				icon = m_Icon,
				valueIcon = m_ValueIcon
			});
		}
	}

	public abstract class Int2PropertyBinder : IPrefabPropertyBinder
	{
		public readonly string m_LabelId;

		public readonly string m_Unit;

		public readonly bool m_Signed;

		public readonly string m_Icon;

		public readonly string m_ValueIcon;

		protected Int2PropertyBinder(string labelId, string unit, bool signed = false, string icon = null, string valueIcon = null)
		{
			m_LabelId = labelId;
			m_Unit = unit;
			m_Signed = signed;
			m_Icon = icon;
			m_ValueIcon = valueIcon;
		}

		public abstract bool Matches(EntityManager entityManager, Entity entity);

		public abstract int2 GetValue(EntityManager entityManager, Entity entity);

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			JsonWriterExtensions.Write<Int2Property>(binder, new Int2Property
			{
				labelId = m_LabelId,
				unit = m_Unit,
				value = GetValue(entityManager, entity),
				signed = m_Signed,
				icon = m_Icon,
				valueIcon = m_ValueIcon
			});
		}
	}

	public class ComponentIntPropertyBinder<T> : IntPropertyBinder where T : unmanaged, IComponentData
	{
		private readonly Func<T, int> m_Getter;

		private readonly bool m_OmitZero;

		public ComponentIntPropertyBinder(string labelId, string unit, Func<T, int> getter, bool omitZero = true, bool signed = false, string icon = null, string valueIcon = null)
			: base(labelId, unit, signed, icon, valueIcon)
		{
			m_Getter = getter;
			m_OmitZero = omitZero;
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			T arg = default(T);
			if (EntitiesExtensions.TryGetComponent<T>(entityManager, entity, ref arg))
			{
				if (m_OmitZero)
				{
					return m_Getter(arg) != 0;
				}
				return true;
			}
			return false;
		}

		public override int GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return m_Getter(((EntityManager)(ref entityManager)).GetComponentData<T>(entity));
		}
	}

	public class ComponentIntRangePropertyBinder<T> : IntRangePropertyBinder where T : unmanaged, IComponentData
	{
		private readonly Func<T, int> m_MinGetter;

		private readonly Func<T, int> m_MaxGetter;

		public ComponentIntRangePropertyBinder(string labelId, string unit, Func<T, int> minGetter, Func<T, int> maxGetter, bool signed = false, string icon = null, string valueIcon = null)
			: base(labelId, unit, signed, icon, valueIcon)
		{
			m_MinGetter = minGetter;
			m_MaxGetter = maxGetter;
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			T val = default(T);
			return EntitiesExtensions.TryGetComponent<T>(entityManager, entity, ref val);
		}

		public override int GetMinValue(EntityManager entityManager, Entity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return m_MinGetter(((EntityManager)(ref entityManager)).GetComponentData<T>(entity));
		}

		public override int GetMaxValue(EntityManager entityManager, Entity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return m_MaxGetter(((EntityManager)(ref entityManager)).GetComponentData<T>(entity));
		}
	}

	public class ComponentInt2PropertyBinder<T> : Int2PropertyBinder where T : unmanaged, IComponentData
	{
		private readonly Func<T, int2> m_Getter;

		private readonly bool m_OmitZero;

		public ComponentInt2PropertyBinder(string labelId, string unit, Func<T, int2> getter, bool omitZero = true, bool signed = false, string icon = null, string valueIcon = null)
			: base(labelId, unit, signed, icon, valueIcon)
		{
			m_Getter = getter;
			m_OmitZero = omitZero;
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			T arg = default(T);
			if (EntitiesExtensions.TryGetComponent<T>(entityManager, entity, ref arg))
			{
				if (m_OmitZero)
				{
					return math.all(m_Getter(arg) != int2.zero);
				}
				return true;
			}
			return false;
		}

		public override int2 GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return m_Getter(((EntityManager)(ref entityManager)).GetComponentData<T>(entity));
		}
	}

	public abstract class StringPropertyBinder : IPrefabPropertyBinder
	{
		public readonly string m_LabelId;

		public readonly string m_Icon;

		public readonly string m_ValueIcon;

		protected StringPropertyBinder(string labelId, string icon = null, string valueIcon = null)
		{
			m_LabelId = labelId;
			m_Icon = icon;
			m_ValueIcon = icon;
		}

		public abstract bool Matches(EntityManager entityManager, Entity entity);

		public abstract string GetValueId(EntityManager entityManager, Entity entity);

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			JsonWriterExtensions.Write<StringProperty>(binder, new StringProperty
			{
				labelId = m_LabelId,
				valueId = GetValueId(entityManager, entity),
				icon = m_Icon,
				valueIcon = m_ValueIcon
			});
		}
	}

	public class ConstructionCostBinder : ComponentIntRangePropertyBinder<PlaceableObjectData>
	{
		public ConstructionCostBinder()
			: base("Properties.CONSTRUCTION_COST", "money", (Func<PlaceableObjectData, int>)((PlaceableObjectData data) => (int)data.m_ConstructionCost), (Func<PlaceableObjectData, int>)((PlaceableObjectData data) => (int)data.m_ConstructionCost), signed: false, (string)null, (string)null)
		{
		}

		public override int GetMinValue(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			TreeData treeData = default(TreeData);
			if (EntitiesExtensions.TryGetComponent<TreeData>(entityManager, entity, ref treeData))
			{
				PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
				ObjectToolSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<ObjectToolSystem>();
				EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
				EconomyParameterData singleton = ((EntityQuery)(ref val)).GetSingleton<EconomyParameterData>();
				Game.Tools.AgeMask actualAgeMask = orCreateSystemManaged.actualAgeMask;
				int num = int.MaxValue;
				if ((actualAgeMask & Game.Tools.AgeMask.Sapling) != 0)
				{
					num = Math.Min(num, (int)componentData.m_ConstructionCost);
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Young) != 0)
				{
					num = Math.Min(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.x));
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Mature) != 0)
				{
					num = Math.Min(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.y));
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Elderly) != 0)
				{
					num = Math.Min(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.z));
				}
				((EntityQuery)(ref val)).Dispose();
				return num;
			}
			return base.GetMinValue(entityManager, entity);
		}

		public override int GetMaxValue(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			TreeData treeData = default(TreeData);
			if (EntitiesExtensions.TryGetComponent<TreeData>(entityManager, entity, ref treeData))
			{
				PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
				ObjectToolSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<ObjectToolSystem>();
				EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
				EconomyParameterData singleton = ((EntityQuery)(ref val)).GetSingleton<EconomyParameterData>();
				Game.Tools.AgeMask actualAgeMask = orCreateSystemManaged.actualAgeMask;
				int num = 0;
				if ((actualAgeMask & Game.Tools.AgeMask.Sapling) != 0)
				{
					num = Math.Max(num, (int)componentData.m_ConstructionCost);
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Young) != 0)
				{
					num = Math.Max(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.x));
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Mature) != 0)
				{
					num = Math.Max(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.y));
				}
				if ((actualAgeMask & Game.Tools.AgeMask.Elderly) != 0)
				{
					num = Math.Max(num, (int)(componentData.m_ConstructionCost * singleton.m_TreeCostMultipliers.z));
				}
				((EntityQuery)(ref val)).Dispose();
				return num;
			}
			return base.GetMaxValue(entityManager, entity);
		}
	}

	public class ConsumptionBinder : IPrefabPropertyBinder
	{
		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ConsumptionData consumptionData = default(ConsumptionData);
			if (EntitiesExtensions.TryGetComponent<ConsumptionData>(entityManager, entity, ref consumptionData))
			{
				return consumptionData.m_WaterConsumption + consumptionData.m_ElectricityConsumption + consumptionData.m_GarbageAccumulation > 0f;
			}
			return false;
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			ConsumptionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(entity);
			binder.TypeBegin("prefabs.ConsumptionProperty");
			binder.PropertyName("electricityConsumption");
			binder.Write(Mathf.CeilToInt(componentData.m_ElectricityConsumption));
			binder.PropertyName("waterConsumption");
			binder.Write(Mathf.CeilToInt(componentData.m_WaterConsumption));
			binder.PropertyName("garbageAccumulation");
			binder.Write(Mathf.CeilToInt(componentData.m_GarbageAccumulation));
			binder.TypeEnd();
		}
	}

	public class PollutionBinder : IPrefabPropertyBinder
	{
		private UIPollutionConfigurationPrefab m_ConfigData;

		public PollutionBinder(UIPollutionConfigurationPrefab data)
		{
			m_ConfigData = data;
		}

		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			PollutionData pollutionData = default(PollutionData);
			if (EntitiesExtensions.TryGetComponent<PollutionData>(entityManager, entity, ref pollutionData))
			{
				return pollutionData.m_AirPollution + pollutionData.m_GroundPollution + pollutionData.m_NoisePollution != 0f;
			}
			return false;
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			PollutionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(entity);
			binder.TypeBegin("prefabs.PollutionProperty");
			binder.PropertyName("groundPollution");
			binder.Write((int)PollutionUIUtils.GetPollutionKey(m_ConfigData.m_GroundPollution, componentData.m_GroundPollution));
			binder.PropertyName("airPollution");
			binder.Write((int)PollutionUIUtils.GetPollutionKey(m_ConfigData.m_AirPollution, componentData.m_AirPollution));
			binder.PropertyName("noisePollution");
			binder.Write((int)PollutionUIUtils.GetPollutionKey(m_ConfigData.m_NoisePollution, componentData.m_NoisePollution));
			binder.TypeEnd();
		}
	}

	public abstract class ElectricityPropertyBinder : IPrefabPropertyBinder
	{
		public readonly string m_LabelId;

		protected ElectricityPropertyBinder(string labelId)
		{
			m_LabelId = labelId;
		}

		public abstract bool Matches(EntityManager entityManager, Entity entity);

		public abstract void GetValue(EntityManager entityManager, Entity entity, out int minCapacity, out int maxCapacity, out Layer voltageLayers);

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			GetValue(entityManager, entity, out var minCapacity, out var maxCapacity, out var voltageLayers);
			binder.TypeBegin("prefabs.ElectricityProperty");
			binder.PropertyName("labelId");
			binder.Write(m_LabelId);
			binder.PropertyName("minCapacity");
			binder.Write(minCapacity);
			binder.PropertyName("maxCapacity");
			binder.Write(maxCapacity);
			binder.PropertyName("voltage");
			binder.Write((int)ElectricityUIUtils.GetVoltage(voltageLayers));
			binder.TypeEnd();
		}
	}

	[CompilerGenerated]
	public class UpkeepPropertyBinderSystem : GameSystemBase, IPrefabPropertyBinder
	{
		private ResourceSystem m_ResourceSystem;

		private EntityQuery m_BudgetDataQuery;

		[Preserve]
		protected override void OnCreate()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
			m_BudgetDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceBudgetData>() });
		}

		[Preserve]
		protected override void OnUpdate()
		{
		}

		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager2)).HasComponent<ServiceUpkeepData>(entity);
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			int2 value = GetValue(entity);
			if (value.x == value.y)
			{
				JsonWriterExtensions.Write<UpkeepIntProperty>(binder, new UpkeepIntProperty
				{
					labelId = "Properties.UPKEEP",
					unit = "moneyPerMonth",
					value = value.x
				});
			}
			else
			{
				JsonWriterExtensions.Write<UpkeepInt2Property>(binder, new UpkeepInt2Property
				{
					labelId = "Properties.UPKEEP",
					unit = "moneyPerMonth",
					value = value
				});
			}
		}

		private int2 GetValue(Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			int2 val = int2.op_Implicit(0);
			DynamicBuffer<ServiceUpkeepData> val2 = default(DynamicBuffer<ServiceUpkeepData>);
			if (EntitiesExtensions.TryGetBuffer<ServiceUpkeepData>(((ComponentSystemBase)this).EntityManager, entity, true, ref val2))
			{
				for (int i = 0; i < val2.Length; i++)
				{
					ServiceUpkeepData serviceUpkeepData = val2[i];
					Resource resource = serviceUpkeepData.m_Upkeep.m_Resource;
					int amount = serviceUpkeepData.m_Upkeep.m_Amount;
					if (!((EntityQuery)(ref m_BudgetDataQuery)).IsEmptyIgnoreFilter && resource == Resource.Money)
					{
						val.x += CityServiceUpkeepSystem.CalculateUpkeep(amount, entity, ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(), ((ComponentSystemBase)this).EntityManager);
						continue;
					}
					float num = (float)amount * EconomyUtils.GetMarketPrice(resource, m_ResourceSystem.GetPrefabs(), ((ComponentSystemBase)this).EntityManager);
					val.y += Mathf.RoundToInt(num);
				}
			}
			val.y += val.x;
			return val;
		}

		[Preserve]
		public UpkeepPropertyBinderSystem()
		{
		}
	}

	public struct UpkeepIntProperty : IJsonWritable
	{
		public string labelId;

		public int value;

		public string unit;

		public bool signed;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin("prefabs.UpkeepIntProperty");
			writer.PropertyName("labelId");
			writer.Write(labelId);
			writer.PropertyName("value");
			writer.Write(value);
			writer.PropertyName("unit");
			writer.Write(unit);
			writer.PropertyName("signed");
			writer.Write(signed);
			writer.TypeEnd();
		}
	}

	public struct UpkeepInt2Property : IJsonWritable
	{
		public string labelId;

		public int2 value;

		public string unit;

		public bool signed;

		public void Write(IJsonWriter writer)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin("prefabs.UpkeepInt2Property");
			writer.PropertyName("labelId");
			writer.Write(labelId);
			writer.PropertyName("value");
			MathematicsWriters.Write(writer, value);
			writer.PropertyName("unit");
			writer.Write(unit);
			writer.PropertyName("signed");
			writer.Write(signed);
			writer.TypeEnd();
		}
	}

	public class StorageLimitBinder : IntPropertyBinder
	{
		public StorageLimitBinder()
			: base("Properties.CARGO_CAPACITY", "weight")
		{
		}

		public override int GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			StorageLimitData storageLimitData = default(StorageLimitData);
			if (EntitiesExtensions.TryGetComponent<StorageLimitData>(entityManager, entity, ref storageLimitData))
			{
				PropertyRenter propertyRenter = default(PropertyRenter);
				PrefabRef prefabRef = default(PrefabRef);
				if (EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, entity, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef))
				{
					Entity prefab = prefabRef.m_Prefab;
					BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
					SpawnableBuildingData spawnable = default(SpawnableBuildingData);
					BuildingData building = default(BuildingData);
					if (EntitiesExtensions.TryGetComponent<BuildingPropertyData>(entityManager, prefab, ref buildingPropertyData) && buildingPropertyData.m_AllowedStored != Resource.NoResource && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(entityManager, prefab, ref spawnable) && EntitiesExtensions.TryGetComponent<BuildingData>(entityManager, prefab, ref building))
					{
						return storageLimitData.GetAdjustedLimitForWarehouse(spawnable, building);
					}
				}
				return storageLimitData.m_Limit;
			}
			return 0;
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			StorageLimitData storageLimitData = default(StorageLimitData);
			return EntitiesExtensions.TryGetComponent<StorageLimitData>(entityManager, entity, ref storageLimitData);
		}
	}

	public class PowerProductionBinder : ElectricityPropertyBinder
	{
		public PowerProductionBinder()
			: base("Properties.POWER_PLANT_OUTPUT")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(entity))
			{
				return true;
			}
			if (((EntityManager)(ref entityManager)).HasComponent<EmergencyGeneratorData>(entity))
			{
				return true;
			}
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (((EntityManager)(ref entityManager)).HasComponent<NetData>(entity) && EntitiesExtensions.TryGetBuffer<SubObject>(entityManager, entity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(val[i].m_Prefab))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void GetValue(EntityManager entityManager, Entity entity, out int minCapacity, out int maxCapacity, out Layer voltageLayers)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			minCapacity = 0;
			maxCapacity = 0;
			voltageLayers = Layer.None;
			AddValues(entityManager, entity, ref minCapacity, ref maxCapacity, ref voltageLayers);
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (((EntityManager)(ref entityManager)).HasComponent<NetData>(entity) && EntitiesExtensions.TryGetBuffer<SubObject>(entityManager, entity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					AddValues(entityManager, val[i].m_Prefab, ref minCapacity, ref maxCapacity, ref voltageLayers);
				}
			}
		}

		public static void AddValues(EntityManager entityManager, Entity entity, ref int minCapacity, ref int maxCapacity, ref Layer voltageLayers)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			PowerPlantData powerPlantData = default(PowerPlantData);
			if (EntitiesExtensions.TryGetComponent<PowerPlantData>(entityManager, entity, ref powerPlantData))
			{
				minCapacity += powerPlantData.m_ElectricityProduction;
				maxCapacity += powerPlantData.m_ElectricityProduction;
			}
			WindPoweredData windPoweredData = default(WindPoweredData);
			if (EntitiesExtensions.TryGetComponent<WindPoweredData>(entityManager, entity, ref windPoweredData))
			{
				maxCapacity += windPoweredData.m_Production;
			}
			SolarPoweredData solarPoweredData = default(SolarPoweredData);
			if (EntitiesExtensions.TryGetComponent<SolarPoweredData>(entityManager, entity, ref solarPoweredData))
			{
				maxCapacity += solarPoweredData.m_Production;
			}
			GarbagePoweredData garbagePoweredData = default(GarbagePoweredData);
			if (EntitiesExtensions.TryGetComponent<GarbagePoweredData>(entityManager, entity, ref garbagePoweredData))
			{
				maxCapacity += garbagePoweredData.m_Capacity;
			}
			WaterPoweredData waterPoweredData = default(WaterPoweredData);
			if (EntitiesExtensions.TryGetComponent<WaterPoweredData>(entityManager, entity, ref waterPoweredData))
			{
				maxCapacity += (int)(1000000f * waterPoweredData.m_CapacityFactor);
			}
			GroundWaterPoweredData groundWaterPoweredData = default(GroundWaterPoweredData);
			if (EntitiesExtensions.TryGetComponent<GroundWaterPoweredData>(entityManager, entity, ref groundWaterPoweredData))
			{
				maxCapacity += groundWaterPoweredData.m_Production;
			}
			EmergencyGeneratorData emergencyGeneratorData = default(EmergencyGeneratorData);
			if (EntitiesExtensions.TryGetComponent<EmergencyGeneratorData>(entityManager, entity, ref emergencyGeneratorData))
			{
				maxCapacity += emergencyGeneratorData.m_ElectricityProduction;
			}
			voltageLayers |= ElectricityUIUtils.GetPowerLineLayers(entityManager, entity);
		}
	}

	public class TransformerCapacityBinder : IntPropertyBinder
	{
		public TransformerCapacityBinder()
			: base("Properties.TRANSFORMER_CAPACITY", "power")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Prefabs.TransformerData>(entity))
			{
				return !((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(entity);
			}
			return false;
		}

		public override int GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			DynamicBuffer<Game.Prefabs.SubNet> val = default(DynamicBuffer<Game.Prefabs.SubNet>);
			if (EntitiesExtensions.TryGetBuffer<Game.Prefabs.SubNet>(entityManager, entity, true, ref val))
			{
				Enumerator<Game.Prefabs.SubNet> enumerator = val.GetEnumerator();
				try
				{
					ElectricityConnectionData electricityConnectionData = default(ElectricityConnectionData);
					while (enumerator.MoveNext())
					{
						Game.Prefabs.SubNet current = enumerator.Current;
						if (current.m_NodeIndex.x == current.m_NodeIndex.y && EntitiesExtensions.TryGetComponent<ElectricityConnectionData>(entityManager, current.m_Prefab, ref electricityConnectionData))
						{
							if (electricityConnectionData.m_Voltage == Game.Prefabs.ElectricityConnection.Voltage.Low)
							{
								num += electricityConnectionData.m_Capacity;
							}
							else
							{
								num2 += electricityConnectionData.m_Capacity;
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return math.min(num, num2);
		}
	}

	public class TransformerInputBinder : StringPropertyBinder
	{
		public TransformerInputBinder()
			: base("Properties.TRANSFORMER_INPUT")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Prefabs.TransformerData>(entity))
			{
				return !((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(entity);
			}
			return false;
		}

		public override string GetValueId(EntityManager entityManager, Entity entity)
		{
			return "Properties.VOLTAGE:1";
		}
	}

	public class TransformerOutputBinder : StringPropertyBinder
	{
		public TransformerOutputBinder()
			: base("Properties.TRANSFORMER_OUTPUT")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return ((EntityManager)(ref entityManager)).HasComponent<Game.Prefabs.TransformerData>(entity);
		}

		public override string GetValueId(EntityManager entityManager, Entity entity)
		{
			return "Properties.VOLTAGE:0";
		}
	}

	public class ElectricityConnectionBinder : ElectricityPropertyBinder
	{
		public ElectricityConnectionBinder()
			: base("Properties.POWER_LINE_CAPACITY")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			ElectricityConnectionData electricityConnectionData = default(ElectricityConnectionData);
			NetData netData = default(NetData);
			if (EntitiesExtensions.TryGetComponent<ElectricityConnectionData>(entityManager, entity, ref electricityConnectionData) && electricityConnectionData.m_Capacity > 0 && (electricityConnectionData.m_CompositionAll.m_General & CompositionFlags.General.Lighting) == 0 && EntitiesExtensions.TryGetComponent<NetData>(entityManager, entity, ref netData))
			{
				return ElectricityUIUtils.HasVoltageLayers(netData.m_LocalConnectLayers);
			}
			return false;
		}

		public override void GetValue(EntityManager entityManager, Entity entity, out int minCapacity, out int maxCapacity, out Layer voltageLayers)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			minCapacity = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityConnectionData>(entity).m_Capacity;
			maxCapacity = minCapacity;
			voltageLayers = ((EntityManager)(ref entityManager)).GetComponentData<NetData>(entity).m_LocalConnectLayers;
		}
	}

	public class WaterConnectionBinder : StringPropertyBinder
	{
		public WaterConnectionBinder()
			: base("Properties.WATER_PIPES")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			WaterPipeConnectionData waterPipeConnectionData = default(WaterPipeConnectionData);
			NetData netData = default(NetData);
			if (EntitiesExtensions.TryGetComponent<WaterPipeConnectionData>(entityManager, entity, ref waterPipeConnectionData) && (waterPipeConnectionData.m_FreshCapacity > 0 || waterPipeConnectionData.m_SewageCapacity > 0) && EntitiesExtensions.TryGetComponent<NetData>(entityManager, entity, ref netData) && !((EntityManager)(ref entityManager)).HasComponent<PipelineData>(entity))
			{
				return (netData.m_LocalConnectLayers & (Layer.WaterPipe | Layer.SewagePipe)) != 0;
			}
			return false;
		}

		public override string GetValueId(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			WaterPipeConnectionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterPipeConnectionData>(entity);
			if (componentData.m_FreshCapacity > 0 && componentData.m_SewageCapacity > 0)
			{
				return "Properties.WATER_PIPE_TYPE[Combined]";
			}
			if (componentData.m_FreshCapacity > 0)
			{
				return "Properties.WATER_PIPE_TYPE[Fresh]";
			}
			return "Properties.WATER_PIPE_TYPE[Sewage]";
		}
	}

	public class JailCapacityBinder : IntPropertyBinder
	{
		public JailCapacityBinder()
			: base("Properties.JAIL_CAPACITY", "integer")
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			PoliceStationData policeStationData = default(PoliceStationData);
			if (!EntitiesExtensions.TryGetComponent<PoliceStationData>(entityManager, entity, ref policeStationData) || policeStationData.m_JailCapacity <= 0)
			{
				PrisonData prisonData = default(PrisonData);
				if (EntitiesExtensions.TryGetComponent<PrisonData>(entityManager, entity, ref prisonData))
				{
					return prisonData.m_PrisonerCapacity > 0;
				}
				return false;
			}
			return true;
		}

		public override int GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			PoliceStationData policeStationData = default(PoliceStationData);
			if (EntitiesExtensions.TryGetComponent<PoliceStationData>(entityManager, entity, ref policeStationData))
			{
				num += policeStationData.m_JailCapacity;
			}
			PrisonData prisonData = default(PrisonData);
			if (EntitiesExtensions.TryGetComponent<PrisonData>(entityManager, entity, ref prisonData))
			{
				num += prisonData.m_PrisonerCapacity;
			}
			return num;
		}
	}

	public class TransportStopBinder : IPrefabPropertyBinder
	{
		public bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityManager)(ref entityManager)).HasComponent<NetData>(entity))
			{
				return false;
			}
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (EntitiesExtensions.TryGetBuffer<SubObject>(entityManager, entity, true, ref val))
			{
				TransportStopData transportStopData = default(TransportStopData);
				for (int i = 0; i < val.Length; i++)
				{
					if (EntitiesExtensions.TryGetComponent<TransportStopData>(entityManager, val[i].m_Prefab, ref transportStopData))
					{
						if (transportStopData.m_PassengerTransport)
						{
							return IsValidTransportType(transportStopData.m_TransportType);
						}
						return false;
					}
				}
			}
			return false;
		}

		public void Bind(IJsonWriter binder, EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubObject> buffer = ((EntityManager)(ref entityManager)).GetBuffer<SubObject>(entity, true);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			TransportStopData transportStopData = default(TransportStopData);
			for (int i = 0; i < buffer.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<TransportStopData>(entityManager, buffer[i].m_Prefab, ref transportStopData) && transportStopData.m_PassengerTransport)
				{
					if (transportStopData.m_TransportType == TransportType.Bus)
					{
						num++;
					}
					else if (transportStopData.m_TransportType == TransportType.Train)
					{
						num2++;
					}
					else if (transportStopData.m_TransportType == TransportType.Tram)
					{
						num3++;
					}
					else if (transportStopData.m_TransportType == TransportType.Ship)
					{
						num4++;
					}
					else if (transportStopData.m_TransportType == TransportType.Helicopter)
					{
						num5++;
					}
					else if (transportStopData.m_TransportType == TransportType.Airplane)
					{
						num6++;
					}
					else if (transportStopData.m_TransportType == TransportType.Subway)
					{
						num7++;
					}
				}
			}
			binder.TypeBegin("prefabs.TransportStopProperty");
			binder.PropertyName("stops");
			binder.MapBegin(7u);
			binder.Write("Airplane");
			binder.Write(num6);
			binder.Write("Helicopter");
			binder.Write(num5);
			binder.Write("Ship");
			binder.Write(num4);
			binder.Write("Subway");
			binder.Write(num7);
			binder.Write("Tram");
			binder.Write(num3);
			binder.Write("Train");
			binder.Write(num2);
			binder.Write("Bus");
			binder.Write(num);
			binder.MapEnd();
			binder.TypeEnd();
		}

		private static bool IsValidTransportType(TransportType type)
		{
			switch (type)
			{
			case TransportType.Bus:
			case TransportType.Train:
			case TransportType.Tram:
			case TransportType.Ship:
			case TransportType.Helicopter:
			case TransportType.Airplane:
			case TransportType.Subway:
				return true;
			default:
				return false;
			}
		}
	}

	public class RequiredResourceBinder : StringPropertyBinder
	{
		private ResourceSystem m_ResourceSystem;

		public RequiredResourceBinder(ResourceSystem resourceSystem)
			: base("Properties.REQUIRED_RESOURCE")
		{
			m_ResourceSystem = resourceSystem;
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (!RequiresWater(entityManager, entity, out var _))
			{
				return GetExtractorType(entityManager, entity) != MapFeature.None;
			}
			return true;
		}

		public override string GetValueId(EntityManager entityManager, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (RequiresWater(entityManager, entity, out var types))
			{
				if ((types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None)
				{
					return "Properties.MAP_RESOURCE[GroundWater]";
				}
				return "Properties.MAP_RESOURCE[SurfaceWater]";
			}
			return $"Properties.MAP_RESOURCE[{GetExtractorType(entityManager, entity):G}]";
		}

		private bool RequiresWater(EntityManager entityManager, Entity entity, out AllowedWaterTypes types)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityManager)(ref entityManager)).HasComponent<GroundWaterPoweredData>(entity))
			{
				types = AllowedWaterTypes.Groundwater;
				return true;
			}
			WaterPumpingStationData waterPumpingStationData = default(WaterPumpingStationData);
			if (EntitiesExtensions.TryGetComponent<WaterPumpingStationData>(entityManager, entity, ref waterPumpingStationData) && waterPumpingStationData.m_Types != AllowedWaterTypes.None)
			{
				types = waterPumpingStationData.m_Types;
				return true;
			}
			types = AllowedWaterTypes.None;
			return false;
		}

		private MapFeature GetExtractorType(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = entity;
			DynamicBuffer<ServiceUpgradeBuilding> val2 = default(DynamicBuffer<ServiceUpgradeBuilding>);
			if (EntitiesExtensions.TryGetBuffer<ServiceUpgradeBuilding>(entityManager, entity, true, ref val2) && val2.Length >= 1)
			{
				val = val2[0].m_Building;
			}
			PlaceholderBuildingData placeholderBuildingData = default(PlaceholderBuildingData);
			if (!EntitiesExtensions.TryGetComponent<PlaceholderBuildingData>(entityManager, val, ref placeholderBuildingData) || placeholderBuildingData.m_Type != BuildingType.ExtractorBuilding)
			{
				return MapFeature.None;
			}
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			if (!EntitiesExtensions.TryGetComponent<BuildingPropertyData>(entityManager, val, ref buildingPropertyData))
			{
				return MapFeature.None;
			}
			DynamicBuffer<Game.Prefabs.SubArea> val3 = default(DynamicBuffer<Game.Prefabs.SubArea>);
			if (!EntitiesExtensions.TryGetBuffer<Game.Prefabs.SubArea>(entityManager, entity, true, ref val3))
			{
				return MapFeature.None;
			}
			ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
			Resource allowedManufactured = buildingPropertyData.m_AllowedManufactured;
			ResourceData resourceData = default(ResourceData);
			if (!EntitiesExtensions.TryGetComponent<ResourceData>(entityManager, prefabs[allowedManufactured], ref resourceData) || !resourceData.m_RequireNaturalResource)
			{
				return MapFeature.None;
			}
			Enumerator<Game.Prefabs.SubArea> enumerator = val3.GetEnumerator();
			try
			{
				ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
				while (enumerator.MoveNext())
				{
					if (EntitiesExtensions.TryGetComponent<ExtractorAreaData>(entityManager, enumerator.Current.m_Prefab, ref extractorAreaData) && extractorAreaData.m_RequireNaturalResource)
					{
						return extractorAreaData.m_MapFeature;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return MapFeature.None;
		}
	}

	public class UpkeepModifierBinder : IntPropertyBinder
	{
		public UpkeepModifierBinder()
			: base("Properties.RESOURCE_CONSUMPTION", "percentage", signed: true)
		{
		}

		public override bool Matches(EntityManager entityManager, Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<UpkeepModifierData> val = default(DynamicBuffer<UpkeepModifierData>);
			if (EntitiesExtensions.TryGetBuffer<UpkeepModifierData>(entityManager, entity, true, ref val))
			{
				Enumerator<UpkeepModifierData> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.m_Multiplier != 1f)
						{
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return false;
		}

		public override int GetValue(EntityManager entityManager, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<UpkeepModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UpkeepModifierData>(entity, true);
			float num = 0f;
			Enumerator<UpkeepModifierData> enumerator = buffer.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					num = math.max(num, enumerator.Current.m_Multiplier);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return (int)math.round(100f * (num - 1f));
		}
	}

	private const string kGroup = "prefabs";

	private PrefabSystem m_PrefabSystem;

	private UniqueAssetTrackingSystem m_UniqueAssetTrackingSystem;

	private ImageSystem m_ImageSystem;

	private Entity m_RequirementEntity;

	private Entity m_TutorialRequirementEntity;

	private EntityQuery m_ThemeQuery;

	private EntityQuery m_ModifiedThemeQuery;

	private EntityQuery m_UnlockedPrefabQuery;

	private EntityQuery m_PollutionConfigQuery;

	private EntityQuery m_ManualUITagsConfigQuery;

	private GetterValueBinding<Dictionary<string, string>> m_UITagsBinding;

	private RawValueBinding m_ThemesBinding;

	private RawMapBinding<Entity> m_PrefabDetailsBinding;

	private int m_UnlockRequirementVersion;

	private int m_UITagVersion;

	private bool m_Initialized;

	public override GameMode gameMode => GameMode.Game;

	public List<IPrefabEffectBinder> effectBinders { get; private set; }

	public List<IPrefabPropertyBinder> constructionCostBinders { get; private set; }

	public List<IPrefabPropertyBinder> propertyBinders { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01b2: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UniqueAssetTrackingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UniqueAssetTrackingSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_RequirementEntity = ((EntityManager)(ref entityManager)).CreateEntity();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddBuffer<UnlockRequirement>(m_RequirementEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TutorialRequirementEntity = ((EntityManager)(ref entityManager)).CreateEntity();
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<ThemeData>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ThemeData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_ModifiedThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UnlockedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_PollutionConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIPollutionConfigurationData>() });
		m_ManualUITagsConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ManualUITagsConfigurationData>() });
		RawValueBinding val2 = new RawValueBinding("prefabs", "themes", (Action<IJsonWriter>)BindThemes);
		RawValueBinding binding = val2;
		m_ThemesBinding = val2;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_PrefabDetailsBinding = new RawMapBinding<Entity>("prefabs", "prefabDetails", (Action<IJsonWriter, Entity>)BindPrefabDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_UITagsBinding = new GetterValueBinding<Dictionary<string, string>>("prefabs", "manualUITags", (Func<Dictionary<string, string>>)BindManualUITags, (IWriter<Dictionary<string, string>>)(object)ValueWriters.Nullable<IDictionary<string, string>>((IWriter<IDictionary<string, string>>)(object)new DictionaryWriter<string, string>((IWriter<string>)null, (IWriter<string>)null)), (EqualityComparer<Dictionary<string, string>>)null)));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (((ComponentSystemBase)this).Enabled)
		{
			m_Initialized = true;
			constructionCostBinders = BuildDefaultConstructionCostBinders();
			propertyBinders = BuildDefaultPropertyBinders();
			effectBinders = BuildDefaultEffectBinders();
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ModifiedThemeQuery)).IsEmptyIgnoreFilter)
		{
			m_ThemesBinding.Update();
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<UnlockRequirementData>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion2 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<ManualUITagsConfigurationData>();
		if (PrefabUtils.HasUnlockedPrefab<UIObjectData>(((ComponentSystemBase)this).EntityManager, m_UnlockedPrefabQuery) || m_UnlockRequirementVersion != componentOrderVersion)
		{
			((MapBindingBase<Entity>)(object)m_PrefabDetailsBinding).UpdateAll();
		}
		if (!((EntityQuery)(ref m_ManualUITagsConfigQuery)).IsEmptyIgnoreFilter && componentOrderVersion2 != m_UITagVersion)
		{
			m_UITagsBinding.Update();
		}
		m_UnlockRequirementVersion = componentOrderVersion;
		m_UITagVersion = componentOrderVersion2;
	}

	private Dictionary<string, string> BindManualUITags()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_ManualUITagsConfigQuery)).IsEmptyIgnoreFilter)
		{
			return null;
		}
		Entity singletonEntity = ((EntityQuery)(ref m_ManualUITagsConfigQuery)).GetSingletonEntity();
		ManualUITagsConfiguration prefab = m_PrefabSystem.GetPrefab<ManualUITagsConfiguration>(singletonEntity);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		FieldInfo[] fields = typeof(ManualUITagsConfiguration).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			UITagPrefab uITagPrefab = fieldInfo.GetValue(prefab) as UITagPrefab;
			if ((Object)(object)uITagPrefab != (Object)null)
			{
				string key = fieldInfo.Name[2].ToString().ToLower() + fieldInfo.Name.Remove(0, 3);
				dictionary[key] = uITagPrefab.uiTag;
			}
		}
		return dictionary;
	}

	private void BindThemes(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(m_ThemeQuery, (Allocator)3);
		JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
		for (int i = 0; i < sortedObjects.Length; i++)
		{
			ThemePrefab prefab = m_PrefabSystem.GetPrefab<ThemePrefab>(sortedObjects[i].prefabData);
			writer.TypeBegin("prefabs.Theme");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, sortedObjects[i].entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		sortedObjects.Dispose();
	}

	private void BindPrefabDetails(IJsonWriter writer, Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		bool unique = m_UniqueAssetTrackingSystem.IsUniqueAsset(entity);
		bool placed = m_UniqueAssetTrackingSystem.IsPlacedUniqueAsset(entity);
		BindPrefabDetails(writer, entity, unique, placed);
	}

	public void BindPrefabDetails(IJsonWriter writer, Entity entity, bool unique, bool placed)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Initialized)
		{
			writer.WriteNull();
			return;
		}
		Entity val = entity;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<SubObject> val2 = default(DynamicBuffer<SubObject>);
		if (((EntityManager)(ref entityManager)).HasComponent<NetData>(val) && EntitiesExtensions.TryGetBuffer<SubObject>(((ComponentSystemBase)this).EntityManager, val, true, ref val2))
		{
			for (int i = 0; i < val2.Length; i++)
			{
				SubObject subObject = val2[i];
				if ((subObject.m_Flags & SubObjectFlags.MakeOwner) != 0)
				{
					val = subObject.m_Prefab;
					break;
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabData prefabData = default(PrefabData);
		PrefabData prefabData2 = default(PrefabData);
		if (((EntityManager)(ref entityManager)).Exists(entity) && EntitiesExtensions.TryGetEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, val, ref prefabData) && EntitiesExtensions.TryGetEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, entity, ref prefabData2))
		{
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(prefabData2);
			PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(prefabData);
			GetTitleAndDescription(val, out var titleId, out var descriptionId);
			string contentPrerequisite = PrefabUtils.GetContentPrerequisite(prefab);
			writer.TypeBegin("prefabs.PrefabDetails");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("uiTag");
			writer.Write(prefab.uiTag);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetThumbnail(prefab) ?? m_ImageSystem.placeholderIcon);
			writer.PropertyName("dlc");
			if (contentPrerequisite != null)
			{
				writer.Write("Media/DLC/" + contentPrerequisite + ".svg");
			}
			else
			{
				writer.WriteNull();
			}
			writer.PropertyName("preview");
			writer.Write(prefab2.TryGet<SignatureBuilding>(out var component) ? component.m_UnlockEventImage : null);
			writer.PropertyName("titleId");
			writer.Write(titleId);
			writer.PropertyName("descriptionId");
			writer.Write(descriptionId);
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity));
			writer.PropertyName("unique");
			writer.Write(unique);
			writer.PropertyName("placed");
			writer.Write(placed);
			writer.PropertyName("constructionCost");
			BindConstructionCost(writer, val);
			writer.PropertyName("effects");
			BindEffects(writer, val);
			writer.PropertyName("properties");
			BindProperties(writer, val);
			writer.PropertyName("requirements");
			BindPrefabRequirements(writer, entity);
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	public void GetTitleAndDescription(Entity prefabEntity, out string titleId, [CanBeNull] out string descriptionId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabEntity, out var prefab))
		{
			if (prefab is UIAssetMenuPrefab || prefab is ServicePrefab)
			{
				titleId = "Services.NAME[" + ((Object)prefab).name + "]";
				descriptionId = "Services.DESCRIPTION[" + ((Object)prefab).name + "]";
			}
			else if (prefab is UIAssetCategoryPrefab)
			{
				titleId = "SubServices.NAME[" + ((Object)prefab).name + "]";
				descriptionId = "Assets.SUB_SERVICE_DESCRIPTION[" + ((Object)prefab).name + "]";
			}
			else if (prefab.Has<Game.Prefabs.ServiceUpgrade>())
			{
				titleId = "Assets.UPGRADE_NAME[" + ((Object)prefab).name + "]";
				descriptionId = "Assets.UPGRADE_DESCRIPTION[" + ((Object)prefab).name + "]";
			}
			else
			{
				titleId = "Assets.NAME[" + ((Object)prefab).name + "]";
				descriptionId = "Assets.DESCRIPTION[" + ((Object)prefab).name + "]";
			}
		}
		else
		{
			titleId = m_PrefabSystem.GetObsoleteID(prefabEntity).GetName();
			descriptionId = "Assets.MISSING_PREFAB_DESCRIPTION";
		}
	}

	private static List<IPrefabEffectBinder> BuildDefaultEffectBinders()
	{
		return new List<IPrefabEffectBinder>
		{
			new CityModifierBinder(),
			new LocalModifierBinder(),
			new LeisureProviderBinder()
		};
	}

	public void BindEffects(IJsonWriter binder, Entity prefabEntity)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (IPrefabEffectBinder effectBinder in effectBinders)
		{
			if (effectBinder.Matches(((ComponentSystemBase)this).EntityManager, prefabEntity))
			{
				num++;
			}
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		foreach (IPrefabEffectBinder effectBinder2 in effectBinders)
		{
			if (effectBinder2.Matches(((ComponentSystemBase)this).EntityManager, prefabEntity))
			{
				effectBinder2.Bind(binder, ((ComponentSystemBase)this).EntityManager, prefabEntity);
			}
		}
		binder.ArrayEnd();
	}

	private static List<IPrefabPropertyBinder> BuildDefaultConstructionCostBinders()
	{
		return new List<IPrefabPropertyBinder>
		{
			new ConstructionCostBinder(),
			new ComponentInt2PropertyBinder<PlaceableNetData>("Common.ASSET_CONSTRUCTION_COST", "moneyPerDistance", (PlaceableNetData data) => new int2(Convert.ToInt32(data.m_DefaultConstructionCost), Convert.ToInt32(data.m_DefaultConstructionCost) * 125)),
			new ComponentIntPropertyBinder<ServiceUpgradeData>("Properties.CONSTRUCTION_COST", "money", (ServiceUpgradeData data) => Convert.ToInt32(data.m_UpgradeCost))
		};
	}

	public void BindConstructionCost(IJsonWriter binder, Entity prefabEntity)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (m_Initialized)
		{
			foreach (IPrefabPropertyBinder constructionCostBinder in constructionCostBinders)
			{
				if (constructionCostBinder.Matches(((ComponentSystemBase)this).EntityManager, prefabEntity))
				{
					constructionCostBinder.Bind(binder, ((ComponentSystemBase)this).EntityManager, prefabEntity);
					return;
				}
			}
		}
		binder.WriteNull();
	}

	private List<IPrefabPropertyBinder> BuildDefaultPropertyBinders()
	{
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		return new List<IPrefabPropertyBinder>
		{
			new RequiredResourceBinder(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>()),
			((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpkeepPropertyBinderSystem>(),
			new ComponentIntPropertyBinder<AssetStampData>("Properties.UPKEEP", "moneyPerMonth", (AssetStampData data) => (int)data.m_UpKeepCost),
			new ComponentIntPropertyBinder<PlaceableNetData>("Properties.UPKEEP", "moneyPerDistancePerMonth", (PlaceableNetData data) => Convert.ToInt32(data.m_DefaultUpkeepCost) * 125),
			new PowerProductionBinder(),
			new ComponentIntPropertyBinder<BatteryData>("Properties.BATTERY_CAPACITY", "energy", (BatteryData data) => data.m_Capacity),
			new ComponentIntPropertyBinder<BatteryData>("Properties.BATTERY_POWER_OUTPUT", "power", (BatteryData data) => data.m_PowerOutput),
			new TransformerCapacityBinder(),
			new TransformerInputBinder(),
			new TransformerOutputBinder(),
			new ElectricityConnectionBinder(),
			new WaterConnectionBinder(),
			new ComponentIntPropertyBinder<SewageOutletData>("Properties.SEWAGE_CAPACITY", "volumePerMonth", (SewageOutletData data) => data.m_Capacity),
			new ComponentIntPropertyBinder<SewageOutletData>("Properties.SEWAGE_PURIFICATION_RATE", "percentage", (SewageOutletData data) => Mathf.RoundToInt(100f * data.m_Purification)),
			new ComponentIntPropertyBinder<WaterPumpingStationData>("Properties.WATER_CAPACITY", "volumePerMonth", (WaterPumpingStationData data) => data.m_Capacity),
			new ComponentIntPropertyBinder<WaterPumpingStationData>("Properties.WATER_PURIFICATION_RATE", "percentage", (WaterPumpingStationData data) => Mathf.RoundToInt(100f * data.m_Purification)),
			new ComponentIntPropertyBinder<HospitalData>("Properties.PATIENT_CAPACITY", "integer", (HospitalData data) => data.m_PatientCapacity),
			new ComponentIntPropertyBinder<HospitalData>("Properties.AMBULANCE_COUNT", "integer", (HospitalData data) => data.m_AmbulanceCapacity),
			new ComponentIntPropertyBinder<HospitalData>("Properties.MEDICAL_HELICOPTER_COUNT", "integer", (HospitalData data) => data.m_MedicalHelicopterCapacity),
			new ComponentIntPropertyBinder<DeathcareFacilityData>("Properties.DECEASED_PROCESSING_CAPACITY", "integerPerMonth", (DeathcareFacilityData data) => Mathf.CeilToInt(data.m_ProcessingRate)),
			new ComponentIntPropertyBinder<DeathcareFacilityData>("Properties.DECEASED_STORAGE", "integer", (DeathcareFacilityData data) => data.m_StorageCapacity),
			new ComponentIntPropertyBinder<DeathcareFacilityData>("Properties.HEARSE_COUNT", "integer", (DeathcareFacilityData data) => data.m_HearseCapacity),
			new ComponentIntPropertyBinder<GarbageFacilityData>("Properties.GARBAGE_PROCESSING_CAPACITY", "weightPerMonth", (GarbageFacilityData data) => data.m_ProcessingSpeed),
			new ComponentIntPropertyBinder<GarbageFacilityData>("Properties.GARBAGE_STORAGE", "weight", (GarbageFacilityData data) => data.m_GarbageCapacity),
			new ComponentIntPropertyBinder<StorageAreaData>("Properties.GARBAGE_STORAGE", "weightPerCell", (StorageAreaData data) => data.m_Capacity * 1000),
			new ComponentIntPropertyBinder<GarbageFacilityData>("Properties.GARBAGE_TRUCK_COUNT", "integer", (GarbageFacilityData data) => data.m_VehicleCapacity),
			new ComponentIntPropertyBinder<FireStationData>("Properties.FIRE_ENGINE_COUNT", "integer", (FireStationData data) => data.m_FireEngineCapacity),
			new ComponentIntPropertyBinder<FireStationData>("Properties.FIRE_HELICOPTER_COUNT", "integer", (FireStationData data) => data.m_FireHelicopterCapacity),
			new ComponentIntPropertyBinder<EmergencyShelterData>("Properties.SHELTER_CAPACITY", "integer", (EmergencyShelterData data) => data.m_ShelterCapacity),
			new ComponentIntPropertyBinder<EmergencyShelterData>("Properties.EVACUATION_BUS_COUNT", "integer", (EmergencyShelterData data) => data.m_VehicleCapacity),
			new JailCapacityBinder(),
			new ComponentIntPropertyBinder<PoliceStationData>("Properties.PATROL_CAR_COUNT", "integer", (PoliceStationData data) => data.m_PatrolCarCapacity),
			new ComponentIntPropertyBinder<PoliceStationData>("Properties.POLICE_HELICOPTER_COUNT", "integer", (PoliceStationData data) => data.m_PoliceHelicopterCapacity),
			new ComponentIntPropertyBinder<PrisonData>("Properties.PRISON_VAN_COUNT", "integer", (PrisonData data) => data.m_PrisonVanCapacity),
			new ComponentIntPropertyBinder<SchoolData>("Properties.STUDENT_CAPACITY", "integer", (SchoolData data) => data.m_StudentCapacity),
			new ComponentIntPropertyBinder<TransportDepotData>("Properties.TRANSPORT_VEHICLE_COUNT", "integer", (TransportDepotData data) => data.m_VehicleCapacity),
			new TransportStopBinder(),
			new ComponentIntPropertyBinder<MaintenanceDepotData>("Properties.MAINTENANCE_VEHICLES", "integer", (MaintenanceDepotData data) => data.m_VehicleCapacity),
			new ComponentIntPropertyBinder<PostFacilityData>("Properties.MAIL_SORTING_RATE", "integerPerMonth", (PostFacilityData data) => data.m_SortingRate),
			new ComponentIntPropertyBinder<PostFacilityData>("Properties.MAIL_STORAGE_CAPACITY", "integer", (PostFacilityData data) => data.m_MailCapacity),
			new ComponentIntPropertyBinder<MailBoxData>("Properties.MAIL_BOX_CAPACITY", "integer", (MailBoxData data) => data.m_MailCapacity),
			new ComponentIntPropertyBinder<PostFacilityData>("Properties.POST_VAN_COUNT", "integer", (PostFacilityData data) => data.m_PostVanCapacity),
			new ComponentIntPropertyBinder<PostFacilityData>("Properties.POST_TRUCK_COUNT", "integer", (PostFacilityData data) => data.m_PostTruckCapacity),
			new ComponentIntPropertyBinder<TelecomFacilityData>("Properties.NETWORK_RANGE", "length", (TelecomFacilityData data) => Mathf.CeilToInt(data.m_Range)),
			new ComponentIntPropertyBinder<TelecomFacilityData>("Properties.NETWORK_CAPACITY", "dataRate", (TelecomFacilityData data) => Mathf.CeilToInt(data.m_NetworkCapacity)),
			new ComponentIntPropertyBinder<AttractionData>("Properties.ATTRACTIVENESS", "integer", (AttractionData data) => data.m_Attractiveness),
			new UpkeepModifierBinder(),
			new StorageLimitBinder(),
			new PollutionBinder(m_PrefabSystem.GetSingletonPrefab<UIPollutionConfigurationPrefab>(m_PollutionConfigQuery)),
			new ComponentIntPropertyBinder<PollutionModifierData>("SelectedInfoPanel.POLLUTION_LEVELS_GROUND", "percentage", (PollutionModifierData data) => Mathf.RoundToInt(data.m_GroundPollutionMultiplier * 100f), omitZero: true, signed: true, null, "Media/Game/Icons/GroundPollution.svg"),
			new ComponentIntPropertyBinder<PollutionModifierData>("SelectedInfoPanel.POLLUTION_LEVELS_AIR", "percentage", (PollutionModifierData data) => Mathf.RoundToInt(data.m_AirPollutionMultiplier * 100f), omitZero: true, signed: true, null, "Media/Game/Icons/AirPollution.svg"),
			new ComponentIntPropertyBinder<PollutionModifierData>("SelectedInfoPanel.POLLUTION_LEVELS_NOISE", "percentage", (PollutionModifierData data) => Mathf.RoundToInt(data.m_NoisePollutionMultiplier * 100f), omitZero: true, signed: true, null, "Media/Game/Icons/NoisePollution.svg"),
			new ComponentIntPropertyBinder<ParkingFacilityData>("Properties.COMFORT", "integer", (ParkingFacilityData data) => (int)math.round(100f * data.m_ComfortFactor)),
			new ComponentIntPropertyBinder<TransportStopData>("Properties.COMFORT", "integer", (TransportStopData data) => (int)math.round(100f * data.m_ComfortFactor)),
			new ComponentIntPropertyBinder<TransportStationData>("Properties.COMFORT", "integer", (TransportStationData data) => (int)math.round(100f * data.m_ComfortFactor))
		};
	}

	public void BindProperties(IJsonWriter binder, Entity prefabEntity)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (IPrefabPropertyBinder propertyBinder in propertyBinders)
		{
			if (propertyBinder.Matches(((ComponentSystemBase)this).EntityManager, prefabEntity))
			{
				num++;
			}
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		foreach (IPrefabPropertyBinder propertyBinder2 in propertyBinders)
		{
			if (propertyBinder2.Matches(((ComponentSystemBase)this).EntityManager, prefabEntity))
			{
				propertyBinder2.Bind(binder, ((ComponentSystemBase)this).EntityManager, prefabEntity);
			}
		}
		binder.ArrayEnd();
	}

	public void BindPrefabRequirements(IJsonWriter writer, Entity prefabEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<UIGroupElement>(prefabEntity))
		{
			BindUIGroupRequirements(writer, prefabEntity);
		}
		else
		{
			BindRequirements(writer, prefabEntity);
		}
	}

	private void BindUIGroupRequirements(IJsonWriter writer, Entity prefabEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		ForceUnlockRequirementData forceUnlockRequirementData = default(ForceUnlockRequirementData);
		if (EntitiesExtensions.TryGetComponent<ForceUnlockRequirementData>(((ComponentSystemBase)this).EntityManager, prefabEntity, ref forceUnlockRequirementData))
		{
			BindRequirements(writer, forceUnlockRequirementData.m_Prefab);
			return;
		}
		NativeList<Entity> requirements = default(NativeList<Entity>);
		requirements._002Ector(4, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<UnlockRequirement> val = default(NativeList<UnlockRequirement>);
		val._002Ector(4, AllocatorHandle.op_Implicit((Allocator)3));
		FindLowestRequirements(prefabEntity, requirements);
		if (requirements.Length > 1)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<UnlockRequirement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(m_RequirementEntity, false);
			DynamicBuffer<UnlockRequirement> val3 = default(DynamicBuffer<UnlockRequirement>);
			for (int i = 0; i < requirements.Length; i++)
			{
				Entity val2 = requirements[i];
				if (!EntitiesExtensions.TryGetBuffer<UnlockRequirement>(((ComponentSystemBase)this).EntityManager, val2, true, ref val3))
				{
					continue;
				}
				for (int j = 0; j < val3.Length; j++)
				{
					if (val3[j].m_Prefab != val2 && !NativeListExtensions.Contains<UnlockRequirement, UnlockRequirement>(val, val3[j]))
					{
						UnlockRequirement unlockRequirement = val3[j];
						val.Add(ref unlockRequirement);
						buffer.Add(new UnlockRequirement
						{
							m_Prefab = val3[j].m_Prefab,
							m_Flags = UnlockFlags.RequireAny
						});
					}
				}
			}
			BindRequirements(writer, m_RequirementEntity);
			buffer.Clear();
		}
		else if (requirements.Length > 0)
		{
			BindRequirements(writer, requirements[0]);
		}
		else
		{
			BindRequirements(writer, m_RequirementEntity);
		}
		requirements.Dispose();
		val.Dispose();
	}

	private int FindLowestRequirements(Entity prefabEntity, NativeList<Entity> requirements, int score = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(entityManager, ((EntityManager)(ref entityManager2)).GetBuffer<UIGroupElement>(prefabEntity, true), (Allocator)3);
		NativeParallelHashMap<Entity, UnlockFlags> devTreeNodes = default(NativeParallelHashMap<Entity, UnlockFlags>);
		devTreeNodes._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, UnlockFlags> unlockRequirements = default(NativeParallelHashMap<Entity, UnlockFlags>);
		unlockRequirements._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		Enumerator<UIObjectInfo> enumerator = sortedObjects.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				UIObjectInfo current = enumerator.Current;
				entityManager2 = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager2)).HasComponent<UIGroupElement>(current.entity))
				{
					int num = FindLowestRequirements(current.entity, requirements, score);
					if ((requirements.Length > 0 && score == -1) || num < score)
					{
						score = num;
					}
					continue;
				}
				GetRequirements(current.entity, out var milestone, devTreeNodes, unlockRequirements);
				int num2 = 0;
				if (milestone != Entity.Null)
				{
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					num2 += ((EntityManager)(ref entityManager2)).GetComponentData<MilestoneData>(milestone).m_Index * 10000;
				}
				Enumerator<Entity, UnlockFlags> enumerator2 = devTreeNodes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						KeyValue<Entity, UnlockFlags> current2 = enumerator2.Current;
						DevTreeNodePrefab prefab = m_PrefabSystem.GetPrefab<DevTreeNodePrefab>(current2.Key);
						num2 += prefab.m_HorizontalPosition * 100;
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
				}
				num2 += unlockRequirements.Count() * 10;
				enumerator2 = unlockRequirements.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						KeyValue<Entity, UnlockFlags> current3 = enumerator2.Current;
						entityManager2 = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager2)).HasComponent<UnlockRequirementData>(current3.Key))
						{
							UnlockRequirementPrefab prefab2 = m_PrefabSystem.GetPrefab<UnlockRequirementPrefab>(current3.Key);
							num2 = ((prefab2 is ZoneBuiltRequirementPrefab zoneBuiltRequirementPrefab) ? (num2 + (zoneBuiltRequirementPrefab.m_MinimumLevel * zoneBuiltRequirementPrefab.m_MinimumCount + zoneBuiltRequirementPrefab.m_MinimumSquares / 100)) : ((prefab2 is CitizenRequirementPrefab citizenRequirementPrefab) ? (num2 + (citizenRequirementPrefab.m_MinimumPopulation / 10 + citizenRequirementPrefab.m_MinimumHappiness * 100)) : ((!(prefab2 is ProcessingRequirementPrefab processingRequirementPrefab)) ? (num2 + 100) : (num2 + processingRequirementPrefab.m_MinimumProducedAmount / 10))));
						}
						else
						{
							num2 += 10;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
				}
				if ((current.entity != Entity.Null && !NativeListExtensions.Contains<Entity, Entity>(requirements, current.entity) && score == -1) || num2 <= score)
				{
					if (num2 < score)
					{
						requirements.Clear();
					}
					Entity entity = current.entity;
					requirements.Add(ref entity);
					score = num2;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		sortedObjects.Dispose();
		devTreeNodes.Dispose();
		unlockRequirements.Dispose();
		return score;
	}

	private void BindRequirements(IJsonWriter writer, Entity prefabEntity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, UnlockFlags> devTreeNodes = default(NativeParallelHashMap<Entity, UnlockFlags>);
		devTreeNodes._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, UnlockFlags> unlockRequirements = default(NativeParallelHashMap<Entity, UnlockFlags>);
		unlockRequirements._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		writer.TypeBegin("Game.UI.Game.PrefabUISystem.UnlockingRequirements");
		GetRequirements(prefabEntity, out var milestone, devTreeNodes, unlockRequirements);
		VerifyRequirements(devTreeNodes, unlockRequirements);
		BindRequirements(writer, UnlockFlags.RequireAll, milestone, devTreeNodes, unlockRequirements);
		BindRequirements(writer, UnlockFlags.RequireAny, Entity.Null, devTreeNodes, unlockRequirements);
		writer.TypeEnd();
		devTreeNodes.Dispose();
		unlockRequirements.Dispose();
	}

	private void VerifyRequirements(NativeParallelHashMap<Entity, UnlockFlags> devTreeNodes, NativeParallelHashMap<Entity, UnlockFlags> unlockRequirements)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		Entity val2 = Entity.Null;
		int num = 0;
		Enumerator<Entity, UnlockFlags> enumerator = devTreeNodes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValue<Entity, UnlockFlags> current = enumerator.Current;
				if ((current.Value & UnlockFlags.RequireAny) != 0)
				{
					val = current.Key;
					num++;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		enumerator = unlockRequirements.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValue<Entity, UnlockFlags> current2 = enumerator.Current;
				if ((current2.Value & UnlockFlags.RequireAny) != 0)
				{
					val2 = current2.Key;
					num++;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (num == 1)
		{
			if (val != Entity.Null)
			{
				devTreeNodes[val] = UnlockFlags.RequireAll;
			}
			if (val2 != Entity.Null)
			{
				unlockRequirements[val2] = UnlockFlags.RequireAll;
			}
		}
	}

	private void BindRequirements(IJsonWriter writer, UnlockFlags flag, Entity milestone, NativeParallelHashMap<Entity, UnlockFlags> devTreeNodes, NativeParallelHashMap<Entity, UnlockFlags> unlockRequirements)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		NativeList<Entity> val = default(NativeList<Entity>);
		val._002Ector(2, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> val2 = default(NativeList<Entity>);
		val2._002Ector(4, AllocatorHandle.op_Implicit((Allocator)3));
		Enumerator<Entity, UnlockFlags> enumerator = devTreeNodes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValue<Entity, UnlockFlags> current = enumerator.Current;
				if ((current.Value & flag) != 0)
				{
					Entity key = current.Key;
					val.Add(ref key);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		for (int i = 0; i < val.Length; i++)
		{
			devTreeNodes.Remove(val[i]);
		}
		enumerator = unlockRequirements.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValue<Entity, UnlockFlags> current2 = enumerator.Current;
				if ((current2.Value & flag) != 0)
				{
					Entity key = current2.Key;
					val2.Add(ref key);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		for (int j = 0; j < val2.Length; j++)
		{
			unlockRequirements.Remove(val2[j]);
		}
		writer.PropertyName((flag == UnlockFlags.RequireAll) ? "requireAll" : "requireAny");
		JsonWriterExtensions.ArrayBegin(writer, ((milestone != Entity.Null) ? 1 : 0) + val.Length + val2.Length);
		if (milestone != Entity.Null)
		{
			BindMilestoneRequirement(writer, milestone);
		}
		for (int k = 0; k < val.Length; k++)
		{
			BindDevTreeNodeRequirement(writer, val[k]);
		}
		for (int l = 0; l < val2.Length; l++)
		{
			BindUnlockRequirement(writer, val2[l]);
		}
		writer.ArrayEnd();
		val.Dispose();
		val2.Dispose();
	}

	private void GetRequirements(Entity prefabEntity, out Entity milestone, NativeParallelHashMap<Entity, UnlockFlags> devTreeNodes, NativeParallelHashMap<Entity, UnlockFlags> unlockRequirements)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, UnlockFlags> requiredPrefabs = default(NativeParallelHashMap<Entity, UnlockFlags>);
		requiredPrefabs._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		ProgressionUtils.CollectSubRequirements(((ComponentSystemBase)this).EntityManager, prefabEntity, requiredPrefabs);
		milestone = Entity.Null;
		int num = -1;
		devTreeNodes.Clear();
		unlockRequirements.Clear();
		Enumerator<Entity, UnlockFlags> enumerator = requiredPrefabs.GetEnumerator();
		try
		{
			MilestoneData milestoneData = default(MilestoneData);
			while (enumerator.MoveNext())
			{
				KeyValue<Entity, UnlockFlags> current = enumerator.Current;
				if (EntitiesExtensions.TryGetComponent<MilestoneData>(((ComponentSystemBase)this).EntityManager, current.Key, ref milestoneData) && milestoneData.m_Index > num)
				{
					milestone = current.Key;
					num = milestoneData.m_Index;
				}
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<DevTreeNodeData>(current.Key))
				{
					if (devTreeNodes.ContainsKey(current.Key))
					{
						ref NativeParallelHashMap<Entity, UnlockFlags> reference = ref devTreeNodes;
						Entity key = current.Key;
						reference[key] |= current.Value;
					}
					else
					{
						devTreeNodes.Add(current.Key, current.Value);
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<UnlockRequirementData>(current.Key))
				{
					if (unlockRequirements.ContainsKey(current.Key))
					{
						ref NativeParallelHashMap<Entity, UnlockFlags> reference = ref unlockRequirements;
						Entity key = current.Key;
						reference[key] |= current.Value;
					}
					else
					{
						unlockRequirements.Add(current.Key, current.Value);
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<TutorialData>(current.Key))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<TutorialPhaseData>(current.Key))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<TutorialTriggerData>(current.Key))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<TutorialListData>(current.Key))
							{
								continue;
							}
						}
					}
				}
				if (EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, current.Key))
				{
					if (unlockRequirements.ContainsKey(m_TutorialRequirementEntity))
					{
						ref NativeParallelHashMap<Entity, UnlockFlags> reference = ref unlockRequirements;
						Entity key = m_TutorialRequirementEntity;
						reference[key] |= current.Value;
					}
					else
					{
						unlockRequirements.Add(m_TutorialRequirementEntity, current.Value);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		requiredPrefabs.Dispose();
	}

	private void BindMilestoneRequirement(IJsonWriter binder, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		MilestoneData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MilestoneData>(entity);
		bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity);
		binder.TypeBegin("prefabs.MilestoneRequirement");
		binder.PropertyName("entity");
		UnityWriters.Write(binder, entity);
		binder.PropertyName("index");
		binder.Write(componentData.m_Index);
		binder.PropertyName("locked");
		binder.Write(flag);
		binder.TypeEnd();
	}

	private void BindDevTreeNodeRequirement(IJsonWriter binder, Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		DevTreeNodePrefab prefab = m_PrefabSystem.GetPrefab<DevTreeNodePrefab>(entity);
		bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity);
		binder.TypeBegin("prefabs.DevTreeNodeRequirement");
		binder.PropertyName("entity");
		UnityWriters.Write(binder, entity);
		binder.PropertyName("name");
		binder.Write(((Object)prefab).name);
		binder.PropertyName("locked");
		binder.Write(flag);
		binder.TypeEnd();
	}

	private void BindUnlockRequirement(IJsonWriter binder, Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (entity == m_TutorialRequirementEntity)
		{
			BindTutorialRequirement(binder, entity);
			return;
		}
		PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
		if (prefab is StrictObjectBuiltRequirementPrefab prefab2)
		{
			BindObjectBuiltRequirement(binder, entity, prefab2);
		}
		else if (prefab is ZoneBuiltRequirementPrefab prefab3)
		{
			BindZoneBuiltRequirement(binder, entity, prefab3);
		}
		else if (prefab is CitizenRequirementPrefab cr)
		{
			BindCitizenRequirement(binder, entity, cr);
		}
		else if (prefab is ProcessingRequirementPrefab prefab4)
		{
			BindProcessingRequirement(binder, entity, prefab4);
		}
		else if (prefab is ObjectBuiltRequirementPrefab prefab5)
		{
			BindOnBuildRequirement(binder, entity, prefab5);
		}
		else if (prefab is PrefabUnlockedRequirementPrefab prefab6)
		{
			BindPrefabUnlockedRequirement(binder, entity, prefab6);
		}
		else if (prefab is UnlockRequirementPrefab prefab7)
		{
			BindUnknownUnlockRequirement(binder, entity, prefab7);
		}
	}

	private void BindTutorialRequirement(IJsonWriter binder, Entity entity)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.TutorialRequirement");
		binder.PropertyName("entity");
		UnityWriters.Write(binder, entity);
		binder.PropertyName("locked");
		binder.Write(true);
		binder.TypeEnd();
	}

	private void BindObjectBuiltRequirement(IJsonWriter binder, Entity entity, StrictObjectBuiltRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.StrictObjectBuiltRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.PropertyName("icon");
		binder.Write(ImageSystem.GetThumbnail(prefab.m_Requirement) ?? m_ImageSystem.placeholderIcon);
		binder.PropertyName("requirement");
		binder.Write(((Object)prefab.m_Requirement).name);
		binder.PropertyName("minimumCount");
		binder.Write(prefab.m_MinimumCount);
		binder.PropertyName("isUpgrade");
		binder.Write(prefab.m_Requirement.Has<Game.Prefabs.ServiceUpgrade>());
		binder.TypeEnd();
	}

	private void BindZoneBuiltRequirement(IJsonWriter binder, Entity entity, ZoneBuiltRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.ZoneBuiltRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.PropertyName("icon");
		bool flag = m_PrefabSystem.HasComponent<UIObjectData>(prefab.m_RequiredZone);
		binder.Write(ImageSystem.GetIcon((PrefabBase)(flag ? (((object)prefab.m_RequiredZone) ?? ((object)prefab)) : prefab)) ?? m_ImageSystem.placeholderIcon);
		binder.PropertyName("requiredTheme");
		ThemePrefab requiredTheme = prefab.m_RequiredTheme;
		binder.Write((requiredTheme != null) ? ((Object)requiredTheme).name : null);
		binder.PropertyName("requiredZone");
		ZonePrefab requiredZone = prefab.m_RequiredZone;
		binder.Write((requiredZone != null) ? ((Object)requiredZone).name : null);
		binder.PropertyName("requiredType");
		binder.Write((int)prefab.m_RequiredType);
		binder.PropertyName("minimumSquares");
		binder.Write(prefab.m_MinimumSquares);
		binder.PropertyName("minimumCount");
		binder.Write(prefab.m_MinimumCount);
		binder.PropertyName("minimumLevel");
		binder.Write(prefab.m_MinimumLevel);
		binder.TypeEnd();
	}

	private void BindCitizenRequirement(IJsonWriter binder, Entity entity, CitizenRequirementPrefab cr)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.CitizenRequirement");
		BindUnlockRequirementProperties(binder, entity, cr);
		binder.PropertyName("minimumPopulation");
		binder.Write(cr.m_MinimumPopulation);
		binder.PropertyName("minimumHappiness");
		binder.Write(cr.m_MinimumHappiness);
		binder.TypeEnd();
	}

	private void BindProcessingRequirement(IJsonWriter binder, Entity entity, ProcessingRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.ProcessingRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.PropertyName("icon");
		binder.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
		binder.PropertyName("resourceType");
		binder.Write(Enum.GetName(typeof(ResourceInEditor), prefab.m_ResourceType));
		binder.PropertyName("minimumProducedAmount");
		binder.Write(prefab.m_MinimumProducedAmount);
		binder.TypeEnd();
	}

	private void BindOnBuildRequirement(IJsonWriter binder, Entity entity, ObjectBuiltRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.ObjectBuiltRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.PropertyName("name");
		binder.Write(((Object)prefab).name);
		binder.PropertyName("minimumCount");
		binder.Write(prefab.m_MinimumCount);
		binder.TypeEnd();
	}

	private void BindPrefabUnlockedRequirement(IJsonWriter binder, Entity entity, PrefabUnlockedRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.PrefabUnlockedRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.TypeEnd();
	}

	private void BindUnknownUnlockRequirement(IJsonWriter binder, Entity entity, UnlockRequirementPrefab prefab)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("prefabs.UnlockRequirement");
		BindUnlockRequirementProperties(binder, entity, prefab);
		binder.TypeEnd();
	}

	private void BindUnlockRequirementProperties(IJsonWriter binder, Entity entity, UnlockRequirementPrefab prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		UnlockRequirementData componentData = ((EntityManager)(ref entityManager)).GetComponentData<UnlockRequirementData>(entity);
		bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity);
		binder.PropertyName("entity");
		UnityWriters.Write(binder, entity);
		binder.PropertyName("labelId");
		binder.Write((!string.IsNullOrEmpty(prefab.m_LabelID)) ? prefab.m_LabelID : null);
		binder.PropertyName("progress");
		binder.Write(componentData.m_Progress);
		binder.PropertyName("locked");
		binder.Write(flag);
	}

	[Preserve]
	public PrefabUISystem()
	{
	}
}
