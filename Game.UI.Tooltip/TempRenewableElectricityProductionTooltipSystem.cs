using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class TempRenewableElectricityProductionTooltipSystem : TooltipSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<WindPoweredData> __Game_Prefabs_WindPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroundWaterPoweredData> __Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPowered> __Game_Buildings_WaterPowered_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_WindPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WindPoweredData>(true);
			__Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroundWaterPoweredData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Buildings_WaterPowered_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPowered>(true);
		}
	}

	private WindSystem m_WindSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EntityQuery m_ErrorQuery;

	private EntityQuery m_TempQuery;

	private ProgressTooltip m_Production;

	private StringTooltip m_WindWarning;

	private StringTooltip m_GroundWaterAvailabilityWarning;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_ErrorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<RenewableElectricityProduction>(),
			ComponentType.Exclude<Deleted>()
		});
		m_Production = new ProgressTooltip
		{
			path = "renewableElectricityProduction",
			icon = "Media/Game/Icons/Electricity.svg",
			label = LocalizedString.Id("Tools.ELECTRICITY_PRODUCTION_LABEL"),
			unit = "power",
			omitMax = true
		};
		m_WindWarning = new StringTooltip
		{
			path = "windWarning",
			value = LocalizedString.Id("Tools.WARNING[NotEnoughWind]"),
			color = TooltipColor.Warning
		};
		m_GroundWaterAvailabilityWarning = new StringTooltip
		{
			path = "groundWaterAvailabilityWarning",
			value = LocalizedString.Id("Tools.WARNING[NotEnoughGroundWater]"),
			color = TooltipColor.Warning
		};
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ErrorQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		((SystemBase)this).CompleteDependency();
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		bool flag2 = false;
		JobHandle dependencies;
		NativeArray<Wind> map = m_WindSystem.GetMap(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeArray<GroundWater> map2 = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies2);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		ComponentLookup<WindPoweredData> componentLookup = InternalCompilerInterface.GetComponentLookup<WindPoweredData>(ref __TypeHandle.__Game_Prefabs_WindPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<GroundWaterPoweredData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<GroundWaterPoweredData>(ref __TypeHandle.__Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			ComponentTypeHandle<PrefabRef> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Transform> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Temp> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Game.Buildings.WaterPowered> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPowered>(ref __TypeHandle.__Game_Buildings_WaterPowered_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			Enumerator<ArchetypeChunk> enumerator = val.GetEnumerator();
			try
			{
				WaterPoweredData waterData = default(WaterPoweredData);
				GroundWaterPoweredData groundWaterData = default(GroundWaterPoweredData);
				while (enumerator.MoveNext())
				{
					ArchetypeChunk current = enumerator.Current;
					NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref current)).GetNativeArray<PrefabRef>(ref componentTypeHandle);
					NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref current)).GetNativeArray<Transform>(ref componentTypeHandle2);
					NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref current)).GetNativeArray<Temp>(ref componentTypeHandle3);
					NativeArray<Game.Buildings.WaterPowered> nativeArray4 = ((ArchetypeChunk)(ref current)).GetNativeArray<Game.Buildings.WaterPowered>(ref componentTypeHandle4);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						if ((nativeArray3[i].m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Upgrade)) == 0)
						{
							continue;
						}
						Entity prefab = nativeArray[i].m_Prefab;
						if (componentLookup.HasComponent(prefab))
						{
							Wind wind = WindSystem.GetWind(nativeArray2[i].m_Position, map);
							float2 windProduction = PowerPlantAISystem.GetWindProduction(componentLookup[prefab], wind, 1f);
							num += windProduction.x;
							num2 += windProduction.y;
							flag |= windProduction.x < windProduction.y * 0.75f;
						}
						if (nativeArray4.Length != 0 && EntitiesExtensions.TryGetComponent<WaterPoweredData>(((ComponentSystemBase)this).EntityManager, prefab, ref waterData))
						{
							Game.Buildings.WaterPowered waterPowered = nativeArray4[i];
							float waterCapacity = PowerPlantAISystem.GetWaterCapacity(waterPowered, waterData);
							float num3 = math.min(waterCapacity, waterPowered.m_Estimate * waterData.m_ProductionFactor);
							num += num3;
							num2 += waterCapacity;
						}
						if (componentLookup2.TryGetComponent(prefab, ref groundWaterData) && groundWaterData.m_MaximumGroundWater > 0)
						{
							float2 groundWaterProduction = PowerPlantAISystem.GetGroundWaterProduction(groundWaterData, nativeArray2[i].m_Position, 1f, map2);
							num += groundWaterProduction.x;
							num2 += groundWaterProduction.y;
							if (groundWaterProduction.x < groundWaterProduction.y * 0.75f)
							{
								flag2 = true;
							}
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			if (num2 > 0f)
			{
				m_Production.value = num;
				m_Production.max = num2;
				ProgressTooltip.SetCapacityColor(m_Production);
				AddMouseTooltip(m_Production);
				if (flag)
				{
					AddMouseTooltip(m_WindWarning);
				}
				if (flag2)
				{
					AddMouseTooltip(m_GroundWaterAvailabilityWarning);
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public TempRenewableElectricityProductionTooltipSystem()
	{
	}
}
