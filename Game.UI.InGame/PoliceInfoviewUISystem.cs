using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PoliceInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		CrimeProducerCount,
		CrimeProbability,
		ArrestedCriminals,
		JailCapacity,
		InJail,
		Prisoners,
		PrisonCapacity,
		InPrison,
		Count
	}

	[BurstCompile]
	private struct PoliceStationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<Occupant> m_OccupantHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> m_PoliceStationDataFromEntity;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgradesFromEntity;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			BufferAccessor<Occupant> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Occupant>(ref m_OccupantHandle);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			PoliceStationData data = default(PoliceStationData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = m_PrefabRefFromEntity[val].m_Prefab;
				if (BuildingUtils.GetEfficiency(bufferAccessor2, i) != 0f)
				{
					m_PoliceStationDataFromEntity.TryGetComponent(prefab, ref data);
					if (m_InstalledUpgradesFromEntity.TryGetBuffer(val, ref upgrades))
					{
						UpgradeUtils.CombineStats<PoliceStationData>(ref data, upgrades, ref m_PrefabRefFromEntity, ref m_PoliceStationDataFromEntity);
					}
					ref NativeArray<float> reference = ref m_Results;
					reference[3] = reference[3] + (float)data.m_JailCapacity;
					if (((ArchetypeChunk)(ref chunk)).Has<Occupant>(ref m_OccupantHandle))
					{
						DynamicBuffer<Occupant> val2 = bufferAccessor[i];
						reference = ref m_Results;
						reference[4] = reference[4] + (float)val2.Length;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PrisonJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<Occupant> m_OccupantHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<PrisonData> m_PrisonDataFromEntity;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgradesFromEntity;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			BufferAccessor<Occupant> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Occupant>(ref m_OccupantHandle);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			PrisonData data = default(PrisonData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = m_PrefabRefFromEntity[val].m_Prefab;
				if (BuildingUtils.GetEfficiency(bufferAccessor2, i) != 0f)
				{
					m_PrisonDataFromEntity.TryGetComponent(prefab, ref data);
					if (m_InstalledUpgradesFromEntity.TryGetBuffer(val, ref upgrades))
					{
						UpgradeUtils.CombineStats<PrisonData>(ref data, upgrades, ref m_PrefabRefFromEntity, ref m_PrisonDataFromEntity);
					}
					ref NativeArray<float> reference = ref m_Results;
					reference[6] = reference[6] + (float)data.m_PrisonerCapacity;
					if (((ArchetypeChunk)(ref chunk)).Has<Occupant>(ref m_OccupantHandle))
					{
						DynamicBuffer<Occupant> val2 = bufferAccessor[i];
						reference = ref m_Results;
						reference[7] = reference[7] + (float)val2.Length;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CrimeProducerJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> m_CrimeProducerHandle;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CrimeProducer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeProducer>(ref m_CrimeProducerHandle);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				CrimeProducer crimeProducer = nativeArray[i];
				ref NativeArray<float> reference = ref m_Results;
				reference[0] = reference[0] + 1f;
				reference = ref m_Results;
				reference[1] = reference[1] + crimeProducer.m_Crime;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CriminalJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Criminal> m_CriminalHandle;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Criminal> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Criminal>(ref m_CriminalHandle);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Criminal criminal = nativeArray[i];
				if ((criminal.m_Flags & (CriminalFlags.Prisoner | CriminalFlags.Sentenced)) != 0)
				{
					ref NativeArray<float> reference = ref m_Results;
					reference[5] = reference[5] + 1f;
				}
				else if ((criminal.m_Flags & CriminalFlags.Arrested) != 0)
				{
					ref NativeArray<float> reference = ref m_Results;
					reference[2] = reference[2] + 1f;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Occupant> __Game_Buildings_Occupant_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> __Game_Prefabs_PoliceStationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrisonData> __Game_Prefabs_PrisonData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Criminal> __Game_Citizens_Criminal_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeProducer>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Occupant_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Occupant>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PoliceStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceStationData>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_PrisonData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrisonData>(true);
			__Game_Citizens_Criminal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Criminal>(true);
		}
	}

	private const string kGroup = "policeInfo";

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ValueBinding<int> m_CrimeProducers;

	private ValueBinding<float> m_CrimeProbability;

	private ValueBinding<int> m_JailCapacity;

	private ValueBinding<int> m_ArrestedCriminals;

	private ValueBinding<int> m_InJail;

	private ValueBinding<int> m_PrisonCapacity;

	private ValueBinding<int> m_Prisoners;

	private ValueBinding<int> m_InPrison;

	private ValueBinding<int> m_Criminals;

	private ValueBinding<int> m_CrimePerMonth;

	private ValueBinding<float> m_EscapedRate;

	private GetterValueBinding<IndicatorValue> m_AverageCrimeProbability;

	private GetterValueBinding<IndicatorValue> m_JailAvailability;

	private GetterValueBinding<IndicatorValue> m_PrisonAvailability;

	private EntityQuery m_PrisonQuery;

	private EntityQuery m_PrisonModifiedQuery;

	private EntityQuery m_CriminalQuery;

	private EntityQuery m_PoliceStationQuery;

	private EntityQuery m_PoliceStationModifiedQuery;

	private EntityQuery m_CrimeProducerQuery;

	private EntityQuery m_CrimeProducerModifiedQuery;

	private NativeArray<float> m_Results;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_632591896_0;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_AverageCrimeProbability).active && !((EventBindingBase)m_JailAvailability).active && !((EventBindingBase)m_PrisonAvailability).active && !((EventBindingBase)m_CrimeProducers).active && !((EventBindingBase)m_CrimeProbability).active && !((EventBindingBase)m_PrisonCapacity).active && !((EventBindingBase)m_Prisoners).active && !((EventBindingBase)m_InPrison).active && !((EventBindingBase)m_JailCapacity).active && !((EventBindingBase)m_ArrestedCriminals).active)
			{
				return ((EventBindingBase)m_InJail).active;
			}
			return true;
		}
	}

	protected override bool Modified
	{
		get
		{
			if (((EntityQuery)(ref m_CrimeProducerModifiedQuery)).IsEmptyIgnoreFilter && ((EntityQuery)(ref m_PoliceStationModifiedQuery)).IsEmptyIgnoreFilter)
			{
				return !((EntityQuery)(ref m_PrisonModifiedQuery)).IsEmptyIgnoreFilter;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected O, but got Unknown
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_PrisonQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.Prison>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Owner>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PoliceStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.PoliceStation>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Owner>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_CrimeProducerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<CrimeProducer>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_CriminalQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<Criminal>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.Prison>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_PrisonModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.PoliceStation>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_PoliceStationModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<CrimeProducer>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[0] = val;
		m_CrimeProducerModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		AddBinding((IBinding)(object)(m_CrimeProducers = new ValueBinding<int>("policeInfo", "crimeProducers", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CrimeProbability = new ValueBinding<float>("policeInfo", "crimeProbability", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_PrisonCapacity = new ValueBinding<int>("policeInfo", "prisonCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Prisoners = new ValueBinding<int>("policeInfo", "prisoners", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_InPrison = new ValueBinding<int>("policeInfo", "inPrison", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_JailCapacity = new ValueBinding<int>("policeInfo", "jailCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ArrestedCriminals = new ValueBinding<int>("policeInfo", "arrestedCriminals", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_InJail = new ValueBinding<int>("policeInfo", "inJail", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Criminals = new ValueBinding<int>("policeInfo", "criminals", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CrimePerMonth = new ValueBinding<int>("policeInfo", "crimePerMonth", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_EscapedRate = new ValueBinding<float>("policeInfo", "escapedRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_AverageCrimeProbability = new GetterValueBinding<IndicatorValue>("policeInfo", "averageCrimeProbability", (Func<IndicatorValue>)GetCrimeProbability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_JailAvailability = new GetterValueBinding<IndicatorValue>("policeInfo", "jailAvailability", (Func<IndicatorValue>)GetJailAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_PrisonAvailability = new GetterValueBinding<IndicatorValue>("policeInfo", "prisonAvailability", (Func<IndicatorValue>)GetPrisonAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		m_Results = new NativeArray<float>(8, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		ResetResults();
		JobHandle val = JobChunkExtensions.Schedule<CrimeProducerJob>(new CrimeProducerJob
		{
			m_CrimeProducerHandle = InternalCompilerInterface.GetComponentTypeHandle<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_CrimeProducerQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<PoliceStationJob>(new PoliceStationJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OccupantHandle = InternalCompilerInterface.GetBufferTypeHandle<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationDataFromEntity = InternalCompilerInterface.GetComponentLookup<PoliceStationData>(ref __TypeHandle.__Game_Prefabs_PoliceStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradesFromEntity = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_PoliceStationQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<PrisonJob>(new PrisonJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OccupantHandle = InternalCompilerInterface.GetBufferTypeHandle<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrisonData>(ref __TypeHandle.__Game_Prefabs_PrisonData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradesFromEntity = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_PrisonQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<CriminalJob>(new CriminalJob
		{
			m_CriminalHandle = InternalCompilerInterface.GetComponentTypeHandle<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_CriminalQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_PrisonCapacity.Update((int)m_Results[6]);
		m_Prisoners.Update((int)m_Results[5]);
		m_InPrison.Update((int)m_Results[7]);
		m_JailCapacity.Update((int)m_Results[3]);
		m_ArrestedCriminals.Update((int)m_Results[2]);
		m_InJail.Update((int)m_Results[4]);
		m_CrimeProducers.Update((int)m_Results[0]);
		m_CrimeProbability.Update(m_Results[1]);
		m_Criminals.Update(((EntityQuery)(ref m_CriminalQuery)).CalculateEntityCount());
		m_AverageCrimeProbability.Update();
		m_JailAvailability.Update();
		m_PrisonAvailability.Update();
		m_CrimePerMonth.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.CrimeCount));
		m_EscapedRate.Update((m_CrimePerMonth.value == 0) ? 0f : math.min(100f, (float)m_CityStatisticsSystem.GetStatisticValue(StatisticType.EscapedArrestCount) * 100f / (float)m_CrimePerMonth.value));
	}

	private void ResetResults()
	{
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0f;
		}
	}

	private IndicatorValue GetCrimeProbability()
	{
		float value = m_CrimeProbability.value;
		int value2 = m_CrimeProducers.value;
		return new IndicatorValue(0f, ((EntityQuery)(ref __query_632591896_0)).GetSingleton<PoliceConfigurationData>().m_MaxCrimeAccumulation, (value2 > 0) ? (value / (float)value2) : 0f);
	}

	private IndicatorValue GetJailAvailability()
	{
		int value = m_JailCapacity.value;
		int value2 = m_ArrestedCriminals.value;
		return IndicatorValue.Calculate(value, value2, 0f);
	}

	private IndicatorValue GetPrisonAvailability()
	{
		int value = m_PrisonCapacity.value;
		int value2 = m_Prisoners.value;
		return IndicatorValue.Calculate(value, value2, 0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<PoliceConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_632591896_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public PoliceInfoviewUISystem()
	{
	}
}
