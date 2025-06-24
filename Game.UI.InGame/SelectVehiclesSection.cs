using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class SelectVehiclesSection : InfoSectionBase
{
	private enum Result
	{
		HasDepots,
		EnergyTypes,
		Count
	}

	[BurstCompile]
	private struct DepotsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradesType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_TransportDepotDataFromEntity;

		public TransportType m_TransportType;

		public NativeArray<int> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradesType);
			TransportDepotData data = default(TransportDepotData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = m_PrefabRefFromEntity[val].m_Prefab;
				m_TransportDepotDataFromEntity.TryGetComponent(prefab, ref data);
				if (CollectionUtils.TryGet<InstalledUpgrade>(bufferAccessor, i, ref upgrades))
				{
					UpgradeUtils.CombineStats<TransportDepotData>(ref data, upgrades, ref m_PrefabRefFromEntity, ref m_TransportDepotDataFromEntity);
				}
				if (data.m_TransportType == m_TransportType)
				{
					m_Results[0] = 1;
					ref NativeArray<int> reference = ref m_Results;
					reference[1] = reference[1] | (int)data.m_EnergyTypes;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct VehiclesListJob : IJob
	{
		public Resource m_Resources;

		public EnergyTypes m_EnergyTypes;

		public SizeClass m_SizeClass;

		public PublicTransportPurpose m_PublicTransportPurpose;

		public TransportType m_TransportType;

		public NativeList<Entity> m_PrimaryList;

		public NativeList<Entity> m_SecondaryList;

		[ReadOnly]
		public TransportVehicleSelectData m_VehicleSelectData;

		public void Execute()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			m_VehicleSelectData.ListVehicles(m_TransportType, m_EnergyTypes, m_SizeClass, m_PublicTransportPurpose, m_Resources, m_PrimaryList, m_SecondaryList);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
		}
	}

	private PrefabUISystem m_PrefabUISystem;

	private ImageSystem m_ImageSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private EntityQuery m_VehiclePrefabQuery;

	private EntityQuery m_DepotQuery;

	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	protected override string group => "SelectVehiclesSection";

	private Entity primaryVehicle { get; set; }

	private Entity secondaryVehicle { get; set; }

	private NativeList<Entity> primaryVehicles { get; set; }

	private NativeList<Entity> secondaryVehicles { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		primaryVehicles.Clear();
		secondaryVehicles.Clear();
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_DepotQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.TransportDepot>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		primaryVehicles = new NativeList<Entity>(20, AllocatorHandle.op_Implicit((Allocator)4));
		secondaryVehicles = new NativeList<Entity>(20, AllocatorHandle.op_Implicit((Allocator)4));
		m_Results = new NativeArray<int>(2, (Allocator)4, (NativeArrayOptions)1);
		AddBinding((IBinding)(object)new TriggerBinding<Entity, Entity>(group, "selectVehicles", (Action<Entity, Entity>)SetVehicleModel, (IReader<Entity>)null, (IReader<Entity>)null));
	}

	private void SetVehicleModel(Entity primary, Entity secondary)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		VehicleModel componentData = ((EntityManager)(ref entityManager)).GetComponentData<VehicleModel>(selectedEntity);
		componentData.m_PrimaryPrefab = primary;
		componentData.m_SecondaryPrefab = secondary;
		((EntityCommandBuffer)(ref val)).SetComponent<VehicleModel>(selectedEntity, componentData);
		m_InfoUISystem.RequestUpdate();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		primaryVehicles.Dispose();
		secondaryVehicles.Dispose();
		m_Results.Dispose();
		base.OnDestroy();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<VehicleModel>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<TransportLineData>(selectedPrefab);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (base.visible = Visible())
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			TransportLineData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TransportLineData>(selectedPrefab);
			JobHandle val = JobChunkExtensions.Schedule<DepotsJob>(new DepotsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradesType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportDepotDataFromEntity = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportType = componentData.m_TransportType,
				m_Results = m_Results
			}, m_DepotQuery, ((SystemBase)this).Dependency);
			((JobHandle)(ref val)).Complete();
			bool flag2 = m_InfoUISystem.tooltipTags.Contains(TooltipTags.CargoRoute);
			m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
			JobHandle jobHandle2 = IJobExtensions.Schedule<VehiclesListJob>(new VehiclesListJob
			{
				m_Resources = (Resource)(flag2 ? 8 : 0),
				m_EnergyTypes = (EnergyTypes)m_Results[1],
				m_SizeClass = componentData.m_SizeClass,
				m_PublicTransportPurpose = ((!flag2) ? PublicTransportPurpose.TransportLine : ((PublicTransportPurpose)0)),
				m_TransportType = componentData.m_TransportType,
				m_PrimaryList = primaryVehicles,
				m_SecondaryList = secondaryVehicles,
				m_VehicleSelectData = m_TransportVehicleSelectData
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
			m_TransportVehicleSelectData.PostUpdate(jobHandle2);
			((JobHandle)(ref jobHandle2)).Complete();
			base.visible = primaryVehicles.Length > 1 || secondaryVehicles.Length > 1;
		}
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		VehicleModel componentData = ((EntityManager)(ref entityManager)).GetComponentData<VehicleModel>(selectedEntity);
		primaryVehicle = componentData.m_PrimaryPrefab;
		secondaryVehicle = componentData.m_SecondaryPrefab;
		base.tooltipTags.Add("TransportLine");
		base.tooltipTags.Add("CargoRoute");
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("primaryVehicle");
		if (primaryVehicle == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			WriteVehicle(writer, primaryVehicle);
		}
		writer.PropertyName("secondaryVehicle");
		if (secondaryVehicle == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			WriteVehicle(writer, secondaryVehicle);
		}
		writer.PropertyName("primaryVehicles");
		JsonWriterExtensions.ArrayBegin(writer, primaryVehicles.Length);
		for (int i = 0; i < primaryVehicles.Length; i++)
		{
			WriteVehicle(writer, primaryVehicles[i]);
		}
		writer.ArrayEnd();
		writer.PropertyName("secondaryVehicles");
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<TrainCarriageData>(primaryVehicle))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<MultipleUnitTrainData>(primaryVehicle))
			{
				JsonWriterExtensions.ArrayBegin(writer, secondaryVehicles.Length);
				for (int j = 0; j < secondaryVehicles.Length; j++)
				{
					WriteVehicle(writer, secondaryVehicles[j]);
				}
				writer.ArrayEnd();
				return;
			}
		}
		writer.WriteNull();
	}

	private void WriteVehicle(IJsonWriter writer, Entity entity)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin(((object)this).GetType().FullName + "+VehiclePrefab");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, entity);
		writer.PropertyName("id");
		writer.Write(m_PrefabSystem.GetPrefabName(entity));
		writer.PropertyName("locked");
		writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity));
		writer.PropertyName("requirements");
		m_PrefabUISystem.BindPrefabRequirements(writer, entity);
		writer.PropertyName("thumbnail");
		writer.Write(m_ImageSystem.GetThumbnail(entity) ?? m_ImageSystem.placeholderIcon);
		writer.TypeEnd();
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
	public SelectVehiclesSection()
	{
	}
}
