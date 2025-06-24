using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Effects;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class PrimaryPrefabReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct FixPrefabRefJob : IJobChunk
	{
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				if (prefabRef.m_Prefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref prefabRef.m_Prefab);
					nativeArray[i] = prefabRef;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixUnderConstructionJob : IJobChunk
	{
		public ComponentTypeHandle<UnderConstruction> m_UnderConstructionType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<UnderConstruction> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnderConstruction>(ref m_UnderConstructionType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				UnderConstruction underConstruction = nativeArray[i];
				if (underConstruction.m_NewPrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref underConstruction.m_NewPrefab);
					nativeArray[i] = underConstruction;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixCompanyDataJob : IJobChunk
	{
		public ComponentTypeHandle<CompanyData> m_CompanyDataType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CompanyData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyData>(ref m_CompanyDataType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CompanyData companyData = nativeArray[i];
				if (companyData.m_Brand != Entity.Null)
				{
					m_PrefabReferences.Check(ref companyData.m_Brand);
					nativeArray[i] = companyData;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixPolicyJob : IJobChunk
	{
		public BufferTypeHandle<Policy> m_PolicyType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Policy> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Policy> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Policy policy = val[j];
					if (policy.m_Policy != Entity.Null)
					{
						m_PrefabReferences.Check(ref policy.m_Policy);
						val[j] = policy;
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
	private struct FixServiceBudgetJob : IJobChunk
	{
		public BufferTypeHandle<ServiceBudgetData> m_BudgetType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ServiceBudgetData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceBudgetData>(ref m_BudgetType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<ServiceBudgetData> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					ServiceBudgetData serviceBudgetData = val[j];
					m_PrefabReferences.Check(ref serviceBudgetData.m_Service);
					val[j] = serviceBudgetData;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixAtmosphereJob : IJobChunk
	{
		public ComponentTypeHandle<AtmosphereData> m_AtmosphereType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<AtmosphereData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AtmosphereData>(ref m_AtmosphereType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				AtmosphereData atmosphereData = nativeArray[i];
				if (atmosphereData.m_AtmospherePrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref atmosphereData.m_AtmospherePrefab);
				}
				nativeArray[i] = atmosphereData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixBiomeJob : IJobChunk
	{
		public ComponentTypeHandle<BiomeData> m_BiomeType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<BiomeData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BiomeData>(ref m_BiomeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				BiomeData biomeData = nativeArray[i];
				if (biomeData.m_BiomePrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref biomeData.m_BiomePrefab);
				}
				nativeArray[i] = biomeData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixVehicleModelJob : IJobChunk
	{
		public ComponentTypeHandle<VehicleModel> m_VehicleModelType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<VehicleModel> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<VehicleModel>(ref m_VehicleModelType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				VehicleModel vehicleModel = nativeArray[i];
				if (vehicleModel.m_PrimaryPrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref vehicleModel.m_PrimaryPrefab);
				}
				if (vehicleModel.m_SecondaryPrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref vehicleModel.m_SecondaryPrefab);
				}
				nativeArray[i] = vehicleModel;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixEditorContainerJob : IJobChunk
	{
		public ComponentTypeHandle<Game.Tools.EditorContainer> m_EditorContainerType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Tools.EditorContainer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.EditorContainer>(ref m_EditorContainerType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Game.Tools.EditorContainer editorContainer = nativeArray[i];
				if (editorContainer.m_Prefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref editorContainer.m_Prefab);
				}
				nativeArray[i] = editorContainer;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixChirpJob : IJobChunk
	{
		public ComponentTypeHandle<Game.Triggers.Chirp> m_ChirpType;

		public BufferTypeHandle<ChirpEntity> m_ChirpEntityType;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabDatas;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Triggers.Chirp> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Triggers.Chirp>(ref m_ChirpType);
			BufferAccessor<ChirpEntity> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ChirpEntity>(ref m_ChirpEntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ref Game.Triggers.Chirp reference = ref CollectionUtils.ElementAt<Game.Triggers.Chirp>(nativeArray, i);
				if (m_PrefabDatas.HasComponent(reference.m_Sender))
				{
					m_PrefabReferences.Check(ref reference.m_Sender);
				}
			}
			for (int j = 0; j < bufferAccessor.Length; j++)
			{
				DynamicBuffer<ChirpEntity> val = bufferAccessor[j];
				for (int k = 0; k < val.Length; k++)
				{
					ref ChirpEntity reference2 = ref val.ElementAt(k);
					if (m_PrefabDatas.HasComponent(reference2.m_Entity))
					{
						m_PrefabReferences.Check(ref reference2.m_Entity);
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
	private struct FixSubReplacementJob : IJobChunk
	{
		public BufferTypeHandle<SubReplacement> m_SubReplacementType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<SubReplacement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubReplacement>(ref m_SubReplacementType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<SubReplacement> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					SubReplacement subReplacement = val[j];
					if (subReplacement.m_Prefab != Entity.Null)
					{
						m_PrefabReferences.Check(ref subReplacement.m_Prefab);
					}
					val[j] = subReplacement;
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentTypeHandle;

		public ComponentTypeHandle<UnderConstruction> __Game_Objects_UnderConstruction_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RW_ComponentTypeHandle;

		public BufferTypeHandle<Policy> __Game_Policies_Policy_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceBudgetData> __Game_Simulation_ServiceBudgetData_RW_BufferTypeHandle;

		public ComponentTypeHandle<AtmosphereData> __Game_Simulation_AtmosphereData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BiomeData> __Game_Simulation_BiomeData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<VehicleModel> __Game_Routes_VehicleModel_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Triggers.Chirp> __Game_Triggers_Chirp_RW_ComponentTypeHandle;

		public BufferTypeHandle<ChirpEntity> __Game_Triggers_ChirpEntity_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		public BufferTypeHandle<SubReplacement> __Game_Net_SubReplacement_RW_BufferTypeHandle;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(false);
			__Game_Objects_UnderConstruction_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnderConstruction>(false);
			__Game_Companies_CompanyData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(false);
			__Game_Policies_Policy_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Policy>(false);
			__Game_Simulation_ServiceBudgetData_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceBudgetData>(false);
			__Game_Simulation_AtmosphereData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AtmosphereData>(false);
			__Game_Simulation_BiomeData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BiomeData>(false);
			__Game_Routes_VehicleModel_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleModel>(false);
			__Game_Tools_EditorContainer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.EditorContainer>(false);
			__Game_Triggers_Chirp_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Triggers.Chirp>(false);
			__Game_Triggers_ChirpEntity_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ChirpEntity>(false);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Net_SubReplacement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubReplacement>(false);
		}
	}

	private CheckPrefabReferencesSystem m_CheckPrefabReferencesSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ClimateSystem m_ClimateSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private EntityQuery m_PrefabRefQuery;

	private EntityQuery m_SetLevelQuery;

	private EntityQuery m_CompanyDataQuery;

	private EntityQuery m_PolicyQuery;

	private EntityQuery m_ActualBudgetQuery;

	private EntityQuery m_ServiceBudgetQuery;

	private EntityQuery m_VehicleModelQuery;

	private EntityQuery m_EditorContainerQuery;

	private EntityQuery m_AtmosphereQuery;

	private EntityQuery m_BiomeQuery;

	private EntityQuery m_ChirpQuery;

	private EntityQuery m_SubReplacementQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CheckPrefabReferencesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CheckPrefabReferencesSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		m_PrefabRefQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<NetCompositionData>(),
			ComponentType.Exclude<EffectInstance>(),
			ComponentType.Exclude<LivePath>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SetLevelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<UnderConstruction>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CompanyDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Policy>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ServiceBudgetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceBudgetData>() });
		m_AtmosphereQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AtmosphereData>() });
		m_BiomeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BiomeData>() });
		m_VehicleModelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<VehicleModel>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EditorContainerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Tools.EditorContainer>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Triggers.Chirp>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SubReplacementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<SubReplacement>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		PrefabReferences references = m_CheckPrefabReferencesSystem.GetPrefabReferences((SystemBase)(object)this, out dependencies);
		dependencies = JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies);
		FixPrefabRefJob fixPrefabRefJob = new FixPrefabRefJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixUnderConstructionJob fixUnderConstructionJob = new FixUnderConstructionJob
		{
			m_UnderConstructionType = InternalCompilerInterface.GetComponentTypeHandle<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixCompanyDataJob fixCompanyDataJob = new FixCompanyDataJob
		{
			m_CompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixPolicyJob fixPolicyJob = new FixPolicyJob
		{
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixServiceBudgetJob fixServiceBudgetJob = new FixServiceBudgetJob
		{
			m_BudgetType = InternalCompilerInterface.GetBufferTypeHandle<ServiceBudgetData>(ref __TypeHandle.__Game_Simulation_ServiceBudgetData_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixAtmosphereJob fixAtmosphereJob = new FixAtmosphereJob
		{
			m_AtmosphereType = InternalCompilerInterface.GetComponentTypeHandle<AtmosphereData>(ref __TypeHandle.__Game_Simulation_AtmosphereData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixBiomeJob fixBiomeJob = new FixBiomeJob
		{
			m_BiomeType = InternalCompilerInterface.GetComponentTypeHandle<BiomeData>(ref __TypeHandle.__Game_Simulation_BiomeData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixVehicleModelJob fixVehicleModelJob = new FixVehicleModelJob
		{
			m_VehicleModelType = InternalCompilerInterface.GetComponentTypeHandle<VehicleModel>(ref __TypeHandle.__Game_Routes_VehicleModel_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixEditorContainerJob obj = new FixEditorContainerJob
		{
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixChirpJob fixChirpJob = new FixChirpJob
		{
			m_ChirpType = InternalCompilerInterface.GetComponentTypeHandle<Game.Triggers.Chirp>(ref __TypeHandle.__Game_Triggers_Chirp_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ChirpEntityType = InternalCompilerInterface.GetBufferTypeHandle<ChirpEntity>(ref __TypeHandle.__Game_Triggers_ChirpEntity_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDatas = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		FixSubReplacementJob fixSubReplacementJob = new FixSubReplacementJob
		{
			m_SubReplacementType = InternalCompilerInterface.GetBufferTypeHandle<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = references
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FixPrefabRefJob>(fixPrefabRefJob, m_PrefabRefQuery, dependencies);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<FixUnderConstructionJob>(fixUnderConstructionJob, m_SetLevelQuery, dependencies);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FixCompanyDataJob>(fixCompanyDataJob, m_CompanyDataQuery, dependencies);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<FixPolicyJob>(fixPolicyJob, m_PolicyQuery, dependencies);
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<FixServiceBudgetJob>(fixServiceBudgetJob, m_ServiceBudgetQuery, dependencies);
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<FixAtmosphereJob>(fixAtmosphereJob, m_AtmosphereQuery, dependencies);
		JobHandle val7 = JobChunkExtensions.ScheduleParallel<FixBiomeJob>(fixBiomeJob, m_BiomeQuery, dependencies);
		JobHandle val8 = JobChunkExtensions.ScheduleParallel<FixVehicleModelJob>(fixVehicleModelJob, m_VehicleModelQuery, dependencies);
		JobHandle val9 = JobChunkExtensions.ScheduleParallel<FixEditorContainerJob>(obj, m_EditorContainerQuery, dependencies);
		JobHandle val10 = JobChunkExtensions.ScheduleParallel<FixChirpJob>(fixChirpJob, m_ChirpQuery, dependencies);
		JobHandle val11 = JobChunkExtensions.ScheduleParallel<FixSubReplacementJob>(fixSubReplacementJob, m_SubReplacementQuery, dependencies);
		((JobHandle)(ref dependencies)).Complete();
		m_CityConfigurationSystem.PatchReferences(ref references);
		m_ClimateSystem.PatchReferences(ref references);
		m_TerrainMaterialSystem.PatchReferences(ref references);
		dependencies = JobUtils.CombineDependencies(val9, val8, val, val3, val4, val5, val2, val6, val7, val10, val11);
		m_CheckPrefabReferencesSystem.AddPrefabReferencesUser(dependencies);
		((SystemBase)this).Dependency = dependencies;
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
	public PrimaryPrefabReferencesSystem()
	{
	}
}
