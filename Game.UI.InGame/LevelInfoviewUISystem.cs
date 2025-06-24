using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LevelInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		Residential,
		Commercial,
		Industrial,
		Office,
		ResultCount
	}

	[BurstCompile]
	private struct UpdateLevelsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentialProperty> m_ResidentialPropertyHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> m_CommercialPropertyHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> m_IndustrialPropertyHandle;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> m_OfficeBuildingFromEntity;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingFromEntity;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> m_SignatureBuildingFromEntity;

		public NativeArray<Levels> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefHandle);
			Levels levels = default(Levels);
			Levels levels2 = default(Levels);
			Levels levels3 = default(Levels);
			Levels levels4 = default(Levels);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				if (!m_SpawnableBuildingFromEntity.TryGetComponent(prefabRef.m_Prefab, ref spawnableBuildingData) || m_SignatureBuildingFromEntity.HasComponent(prefabRef.m_Prefab))
				{
					continue;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<ResidentialProperty>(ref m_ResidentialPropertyHandle))
				{
					AddLevel(spawnableBuildingData, ref levels);
				}
				if (((ArchetypeChunk)(ref chunk)).Has<CommercialProperty>(ref m_CommercialPropertyHandle))
				{
					AddLevel(spawnableBuildingData, ref levels2);
				}
				if (((ArchetypeChunk)(ref chunk)).Has<IndustrialProperty>(ref m_IndustrialPropertyHandle))
				{
					if (m_OfficeBuildingFromEntity.HasComponent(prefabRef.m_Prefab))
					{
						AddLevel(spawnableBuildingData, ref levels4);
					}
					else
					{
						AddLevel(spawnableBuildingData, ref levels3);
					}
				}
			}
			ref NativeArray<Levels> reference = ref m_Results;
			reference[0] = reference[0] + levels;
			reference = ref m_Results;
			reference[1] = reference[1] + levels2;
			reference = ref m_Results;
			reference[2] = reference[2] + levels3;
			reference = ref m_Results;
			reference[3] = reference[3] + levels4;
		}

		private void AddLevel(SpawnableBuildingData spawnableBuildingData, ref Levels levels)
		{
			switch (spawnableBuildingData.m_Level)
			{
			case 1:
				levels.Level1++;
				break;
			case 2:
				levels.Level2++;
				break;
			case 3:
				levels.Level3++;
				break;
			case 4:
				levels.Level4++;
				break;
			case 5:
				levels.Level5++;
				break;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct Levels
	{
		public int Level1;

		public int Level2;

		public int Level3;

		public int Level4;

		public int Level5;

		public Levels(int level1, int level2, int level3, int level4, int level5)
		{
			Level1 = level1;
			Level2 = level2;
			Level3 = level3;
			Level4 = level4;
			Level5 = level5;
		}

		public static Levels operator +(Levels left, Levels right)
		{
			return new Levels(left.Level1 + right.Level1, left.Level2 + right.Level2, left.Level3 + right.Level3, left.Level4 + right.Level4, left.Level5 + right.Level5);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentialProperty> __Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommercialProperty> __Game_Buildings_CommercialProperty_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProperty> __Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle;

		public ComponentLookup<OfficeBuilding> __Game_Prefabs_OfficeBuilding_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentialProperty>(true);
			__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommercialProperty>(true);
			__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialProperty>(true);
			__Game_Prefabs_OfficeBuilding_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OfficeBuilding>(false);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SignatureBuildingData>(true);
		}
	}

	private const string kGroup = "levelInfo";

	private RawValueBinding m_ResidentialLevels;

	private RawValueBinding m_CommercialLevels;

	private RawValueBinding m_IndustrialLevels;

	private RawValueBinding m_OfficeLevels;

	private EntityQuery m_SpawnableQuery;

	private NativeArray<Levels> m_Results;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_ResidentialLevels).active && !((EventBindingBase)m_CommercialLevels).active && !((EventBindingBase)m_IndustrialLevels).active)
			{
				return ((EventBindingBase)m_OfficeLevels).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00be: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e8: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0112: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_013c: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.ReadOnly<CommercialProperty>(),
			ComponentType.ReadOnly<IndustrialProperty>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_SpawnableQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		RawValueBinding val2 = new RawValueBinding("levelInfo", "residential", (Action<IJsonWriter>)UpdateResidentialLevels);
		RawValueBinding binding = val2;
		m_ResidentialLevels = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("levelInfo", "commercial", (Action<IJsonWriter>)UpdateCommercialLevels);
		binding = val3;
		m_CommercialLevels = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("levelInfo", "industrial", (Action<IJsonWriter>)UpdateIndustrialLevels);
		binding = val4;
		m_IndustrialLevels = val4;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val5 = new RawValueBinding("levelInfo", "office", (Action<IJsonWriter>)UpdateOfficeLevels);
		binding = val5;
		m_OfficeLevels = val5;
		AddBinding((IBinding)(object)binding);
		m_Results = new NativeArray<Levels>(4, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		UpdateBindings();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		UpdateBindings();
	}

	private void UpdateBindings()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		ResetResults<Levels>(m_Results);
		JobHandle val = JobChunkExtensions.Schedule<UpdateLevelsJob>(new UpdateLevelsJob
		{
			m_PrefabRefHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentialPropertyHandle = InternalCompilerInterface.GetComponentTypeHandle<ResidentialProperty>(ref __TypeHandle.__Game_Buildings_ResidentialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommercialPropertyHandle = InternalCompilerInterface.GetComponentTypeHandle<CommercialProperty>(ref __TypeHandle.__Game_Buildings_CommercialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialPropertyHandle = InternalCompilerInterface.GetComponentTypeHandle<IndustrialProperty>(ref __TypeHandle.__Game_Buildings_IndustrialProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OfficeBuildingFromEntity = InternalCompilerInterface.GetComponentLookup<OfficeBuilding>(ref __TypeHandle.__Game_Prefabs_OfficeBuilding_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingFromEntity = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SignatureBuildingFromEntity = InternalCompilerInterface.GetComponentLookup<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_SpawnableQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_ResidentialLevels.Update();
		m_CommercialLevels.Update();
		m_IndustrialLevels.Update();
		m_OfficeLevels.Update();
	}

	private void UpdateResidentialLevels(IJsonWriter writer)
	{
		Levels levels = m_Results[0];
		WriteLevels(writer, levels);
	}

	private void UpdateCommercialLevels(IJsonWriter writer)
	{
		Levels levels = m_Results[1];
		WriteLevels(writer, levels);
	}

	private void UpdateIndustrialLevels(IJsonWriter writer)
	{
		Levels levels = m_Results[2];
		WriteLevels(writer, levels);
	}

	private void UpdateOfficeLevels(IJsonWriter writer)
	{
		Levels levels = m_Results[3];
		WriteLevels(writer, levels);
	}

	private void WriteLevels(IJsonWriter writer, Levels levels)
	{
		InfoviewsUIUtils.UpdateFiveSlicePieChartData(writer, levels.Level1, levels.Level2, levels.Level3, levels.Level4, levels.Level5);
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
	public LevelInfoviewUISystem()
	{
	}
}
