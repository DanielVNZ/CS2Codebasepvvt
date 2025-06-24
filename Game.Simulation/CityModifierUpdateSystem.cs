using System;
using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Policies;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CityModifierUpdateSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateCityModifiersJob : IJobChunk
	{
		[ReadOnly]
		public CityModifierRefreshData m_CityModifierRefreshData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EffectProviderChunks;

		[ReadOnly]
		public BufferTypeHandle<Policy> m_PolicyType;

		public ComponentTypeHandle<Game.City.City> m_CityType;

		public BufferTypeHandle<CityModifier> m_CityModifierType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			NativeList<CityModifierData> tempModifierList = default(NativeList<CityModifierData>);
			tempModifierList._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<Game.City.City> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.City.City>(ref m_CityType);
			BufferAccessor<CityModifier> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CityModifier>(ref m_CityModifierType);
			BufferAccessor<Policy> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Game.City.City city = nativeArray[i];
				DynamicBuffer<CityModifier> modifiers = bufferAccessor[i];
				DynamicBuffer<Policy> policies = bufferAccessor2[i];
				m_CityModifierRefreshData.RefreshCityOptions(ref city, policies);
				m_CityModifierRefreshData.RefreshCityModifiers(modifiers, policies, m_EffectProviderChunks, tempModifierList);
				nativeArray[i] = city;
			}
			tempModifierList.Dispose();
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct CityModifierRefreshData
	{
		public BufferTypeHandle<Efficiency> m_BuildingEfficiencyType;

		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public ComponentTypeHandle<Signature> m_SignatureType;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<PolicySliderData> m_PolicySliderData;

		public ComponentLookup<CityOptionData> m_CityOptionData;

		public BufferLookup<CityModifierData> m_CityModifierData;

		public CityModifierRefreshData(SystemBase system)
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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			m_BuildingEfficiencyType = ((ComponentSystemBase)system).GetBufferTypeHandle<Efficiency>(true);
			m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
			m_InstalledUpgradeType = ((ComponentSystemBase)system).GetBufferTypeHandle<InstalledUpgrade>(true);
			m_SignatureType = ((ComponentSystemBase)system).GetComponentTypeHandle<Signature>(true);
			m_PrefabRefData = system.GetComponentLookup<PrefabRef>(true);
			m_PolicySliderData = system.GetComponentLookup<PolicySliderData>(true);
			m_CityOptionData = system.GetComponentLookup<CityOptionData>(true);
			m_CityModifierData = system.GetBufferLookup<CityModifierData>(true);
		}

		public void Update(SystemBase system)
		{
			m_BuildingEfficiencyType.Update(system);
			m_PrefabRefType.Update(system);
			m_InstalledUpgradeType.Update(system);
			m_SignatureType.Update(system);
			m_PrefabRefData.Update(system);
			m_PolicySliderData.Update(system);
			m_CityOptionData.Update(system);
			m_CityModifierData.Update(system);
		}

		public void RefreshCityOptions(ref Game.City.City city, DynamicBuffer<Policy> policies)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			city.m_OptionMask = 0u;
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) != 0 && m_CityOptionData.HasComponent(policy.m_Policy))
				{
					CityOptionData cityOptionData = m_CityOptionData[policy.m_Policy];
					city.m_OptionMask |= cityOptionData.m_OptionMask;
				}
			}
		}

		public void RefreshCityModifiers(DynamicBuffer<CityModifier> modifiers, DynamicBuffer<Policy> policies, NativeList<ArchetypeChunk> effectProviderChunks, NativeList<CityModifierData> tempModifierList)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			modifiers.Clear();
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) == 0 || !m_CityModifierData.HasBuffer(policy.m_Policy))
				{
					continue;
				}
				DynamicBuffer<CityModifierData> val = m_CityModifierData[policy.m_Policy];
				for (int j = 0; j < val.Length; j++)
				{
					CityModifierData modifierData = val[j];
					float delta;
					if (m_PolicySliderData.HasComponent(policy.m_Policy))
					{
						PolicySliderData policySliderData = m_PolicySliderData[policy.m_Policy];
						float num = (policy.m_Adjustment - policySliderData.m_Range.min) / (policySliderData.m_Range.max - policySliderData.m_Range.min);
						num = math.select(num, 0f, policySliderData.m_Range.min == policySliderData.m_Range.max);
						num = math.saturate(num);
						delta = math.lerp(modifierData.m_Range.min, modifierData.m_Range.max, num);
					}
					else
					{
						delta = modifierData.m_Range.min;
					}
					AddModifier(modifiers, modifierData, delta);
				}
			}
			for (int k = 0; k < effectProviderChunks.Length; k++)
			{
				ArchetypeChunk val2 = effectProviderChunks[k];
				bool num2 = ((ArchetypeChunk)(ref val2)).Has<Signature>(ref m_SignatureType);
				BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
				if (!num2 && bufferAccessor.Length != 0)
				{
					for (int l = 0; l < nativeArray.Length; l++)
					{
						PrefabRef prefabRef = nativeArray[l];
						float efficiency = BuildingUtils.GetEfficiency(bufferAccessor[l]);
						if (m_CityModifierData.HasBuffer(prefabRef.m_Prefab))
						{
							InitializeTempList(tempModifierList, m_CityModifierData[prefabRef.m_Prefab]);
						}
						else
						{
							InitializeTempList(tempModifierList);
						}
						if (bufferAccessor2.Length != 0)
						{
							AddToTempList(tempModifierList, bufferAccessor2[l]);
						}
						for (int m = 0; m < tempModifierList.Length; m++)
						{
							CityModifierData modifierData2 = tempModifierList[m];
							float delta2 = math.lerp(modifierData2.m_Range.min, modifierData2.m_Range.max, efficiency);
							AddModifier(modifiers, modifierData2, delta2);
						}
					}
					continue;
				}
				for (int n = 0; n < nativeArray.Length; n++)
				{
					PrefabRef prefabRef2 = nativeArray[n];
					if (m_CityModifierData.HasBuffer(prefabRef2.m_Prefab))
					{
						InitializeTempList(tempModifierList, m_CityModifierData[prefabRef2.m_Prefab]);
					}
					else
					{
						InitializeTempList(tempModifierList);
					}
					if (bufferAccessor2.Length != 0)
					{
						AddToTempList(tempModifierList, bufferAccessor2[n]);
					}
					for (int num3 = 0; num3 < tempModifierList.Length; num3++)
					{
						CityModifierData modifierData3 = tempModifierList[num3];
						AddModifier(modifiers, modifierData3, modifierData3.m_Range.max);
					}
				}
			}
		}

		private void AddToTempList(NativeList<CityModifierData> tempModifierList, DynamicBuffer<InstalledUpgrade> upgrades)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<CityModifierData> cityModifiers = default(DynamicBuffer<CityModifierData>);
			for (int i = 0; i < upgrades.Length; i++)
			{
				InstalledUpgrade installedUpgrade = upgrades[i];
				if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive) && m_CityModifierData.TryGetBuffer(m_PrefabRefData[installedUpgrade.m_Upgrade].m_Prefab, ref cityModifiers))
				{
					CityModifierUpdateSystem.AddToTempList(tempModifierList, cityModifiers);
				}
			}
		}

		private static void AddModifier(DynamicBuffer<CityModifier> modifiers, CityModifierData modifierData, float delta)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			while (modifiers.Length <= (int)modifierData.m_Type)
			{
				modifiers.Add(default(CityModifier));
			}
			CityModifier cityModifier = modifiers[(int)modifierData.m_Type];
			switch (modifierData.m_Mode)
			{
			case ModifierValueMode.Relative:
				cityModifier.m_Delta.y = cityModifier.m_Delta.y * (1f + delta) + delta;
				break;
			case ModifierValueMode.Absolute:
				cityModifier.m_Delta.x += delta;
				break;
			case ModifierValueMode.InverseRelative:
				delta = 1f / math.max(0.001f, 1f + delta) - 1f;
				cityModifier.m_Delta.y = cityModifier.m_Delta.y * (1f + delta) + delta;
				break;
			}
			modifiers[(int)modifierData.m_Type] = cityModifier;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Policy> __Game_Policies_Policy_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.City.City> __Game_City_City_RW_ComponentTypeHandle;

		public BufferTypeHandle<CityModifier> __Game_City_CityModifier_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Policies_Policy_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Policy>(true);
			__Game_City_City_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.City.City>(false);
			__Game_City_CityModifier_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CityModifier>(false);
		}
	}

	private EntityQuery m_CityQuery;

	private EntityQuery m_EffectProviderQuery;

	private CityModifierRefreshData m_CityModifierRefreshData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityModifierRefreshData = new CityModifierRefreshData((SystemBase)(object)this);
		m_CityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<Game.City.City>() });
		m_EffectProviderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<CityEffectProvider>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CityQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> effectProviderChunks = ((EntityQuery)(ref m_EffectProviderQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		m_CityModifierRefreshData.Update((SystemBase)(object)this);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateCityModifiersJob>(new UpdateCityModifiersJob
		{
			m_CityModifierRefreshData = m_CityModifierRefreshData,
			m_EffectProviderChunks = effectProviderChunks,
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CityType = InternalCompilerInterface.GetComponentTypeHandle<Game.City.City>(ref __TypeHandle.__Game_City_City_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifierType = InternalCompilerInterface.GetBufferTypeHandle<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_CityQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		effectProviderChunks.Dispose(val2);
		((SystemBase)this).Dependency = val2;
	}

	public static void InitializeTempList(NativeList<CityModifierData> tempModifierList)
	{
		tempModifierList.Clear();
	}

	public static void InitializeTempList(NativeList<CityModifierData> tempModifierList, DynamicBuffer<CityModifierData> cityModifiers)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		tempModifierList.Clear();
		tempModifierList.AddRange(cityModifiers.AsNativeArray());
	}

	public static void AddToTempList(NativeList<CityModifierData> tempModifierList, DynamicBuffer<CityModifierData> cityModifiers)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < cityModifiers.Length; i++)
		{
			CityModifierData cityModifierData = cityModifiers[i];
			int num = 0;
			while (true)
			{
				if (num < tempModifierList.Length)
				{
					CityModifierData cityModifierData2 = tempModifierList[num];
					if (cityModifierData2.m_Type == cityModifierData.m_Type)
					{
						if (cityModifierData2.m_Mode != cityModifierData.m_Mode)
						{
							throw new Exception($"Modifier mode mismatch (type: {cityModifierData.m_Type})");
						}
						cityModifierData2.m_Range.min += cityModifierData.m_Range.min;
						cityModifierData2.m_Range.max += cityModifierData.m_Range.max;
						tempModifierList[num] = cityModifierData2;
						break;
					}
					num++;
					continue;
				}
				tempModifierList.Add(ref cityModifierData);
				break;
			}
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
	public CityModifierUpdateSystem()
	{
	}
}
