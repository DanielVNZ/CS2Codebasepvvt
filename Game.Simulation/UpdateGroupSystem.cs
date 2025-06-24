using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class UpdateGroupSystem : GameSystemBase
{
	public struct UpdateGroupTypes
	{
		public ComponentTypeHandle<Moving> m_MovingType;

		public ComponentTypeHandle<Stopped> m_StoppedType;

		public ComponentTypeHandle<Plant> m_PlantType;

		public ComponentTypeHandle<Building> m_BuildingType;

		public ComponentTypeHandle<Extension> m_ExtensionType;

		public ComponentTypeHandle<Node> m_NodeType;

		public ComponentTypeHandle<Edge> m_EdgeType;

		public ComponentTypeHandle<Lane> m_LaneType;

		public ComponentTypeHandle<CompanyData> m_CompanyType;

		public ComponentTypeHandle<Household> m_HouseholdType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		public ComponentTypeHandle<HouseholdPet> m_HouseholdPetType;

		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		public UpdateGroupTypes(SystemBase system)
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
			m_MovingType = ((ComponentSystemBase)system).GetComponentTypeHandle<Moving>(true);
			m_StoppedType = ((ComponentSystemBase)system).GetComponentTypeHandle<Stopped>(true);
			m_PlantType = ((ComponentSystemBase)system).GetComponentTypeHandle<Plant>(true);
			m_BuildingType = ((ComponentSystemBase)system).GetComponentTypeHandle<Building>(true);
			m_ExtensionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Extension>(true);
			m_NodeType = ((ComponentSystemBase)system).GetComponentTypeHandle<Node>(true);
			m_EdgeType = ((ComponentSystemBase)system).GetComponentTypeHandle<Edge>(true);
			m_LaneType = ((ComponentSystemBase)system).GetComponentTypeHandle<Lane>(true);
			m_CompanyType = ((ComponentSystemBase)system).GetComponentTypeHandle<CompanyData>(true);
			m_HouseholdType = ((ComponentSystemBase)system).GetComponentTypeHandle<Household>(true);
			m_CitizenType = ((ComponentSystemBase)system).GetComponentTypeHandle<Citizen>(true);
			m_HouseholdPetType = ((ComponentSystemBase)system).GetComponentTypeHandle<HouseholdPet>(true);
			m_CurrentVehicleType = ((ComponentSystemBase)system).GetComponentTypeHandle<CurrentVehicle>(true);
		}

		public void Update(SystemBase system)
		{
			m_MovingType.Update(system);
			m_StoppedType.Update(system);
			m_PlantType.Update(system);
			m_BuildingType.Update(system);
			m_ExtensionType.Update(system);
			m_NodeType.Update(system);
			m_EdgeType.Update(system);
			m_LaneType.Update(system);
			m_CompanyType.Update(system);
			m_HouseholdType.Update(system);
			m_CitizenType.Update(system);
			m_HouseholdPetType.Update(system);
			m_CurrentVehicleType.Update(system);
		}
	}

	public struct UpdateGroupSizes
	{
		private NativeArray<int> m_MovingObjectUpdateGroupSizes;

		private NativeArray<int> m_TreeUpdateGroupSizes;

		private NativeArray<int> m_BuildingUpdateGroupSizes;

		private NativeArray<int> m_NetUpdateGroupSizes;

		private NativeArray<int> m_LaneUpdateGroupSizes;

		private NativeArray<int> m_CompanyUpdateGroupSizes;

		private NativeArray<int> m_HouseholdUpdateGroupSizes;

		private NativeArray<int> m_CitizenUpdateGroupSizes;

		private NativeArray<int> m_HouseholdPetUpdateGroupSizes;

		public UpdateGroupSizes(Allocator allocator)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			m_MovingObjectUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_TreeUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_BuildingUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_NetUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_LaneUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_CompanyUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_HouseholdUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_CitizenUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
			m_HouseholdPetUpdateGroupSizes = new NativeArray<int>(16, allocator, (NativeArrayOptions)1);
		}

		public void Clear()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			CollectionUtils.Fill<int>(m_MovingObjectUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_TreeUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_BuildingUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_NetUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_LaneUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_CompanyUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_HouseholdUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_CitizenUpdateGroupSizes, 0);
			CollectionUtils.Fill<int>(m_HouseholdPetUpdateGroupSizes, 0);
		}

		public void Dispose()
		{
			m_MovingObjectUpdateGroupSizes.Dispose();
			m_TreeUpdateGroupSizes.Dispose();
			m_BuildingUpdateGroupSizes.Dispose();
			m_NetUpdateGroupSizes.Dispose();
			m_LaneUpdateGroupSizes.Dispose();
			m_CompanyUpdateGroupSizes.Dispose();
			m_HouseholdUpdateGroupSizes.Dispose();
			m_CitizenUpdateGroupSizes.Dispose();
			m_HouseholdPetUpdateGroupSizes.Dispose();
		}

		public NativeArray<int> Get(ArchetypeChunk chunk, UpdateGroupTypes types)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Moving>(ref types.m_MovingType) || ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref types.m_StoppedType) || ((ArchetypeChunk)(ref chunk)).Has<CurrentVehicle>(ref types.m_CurrentVehicleType))
			{
				return m_MovingObjectUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Plant>(ref types.m_PlantType))
			{
				return m_TreeUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Building>(ref types.m_BuildingType) || ((ArchetypeChunk)(ref chunk)).Has<Extension>(ref types.m_ExtensionType))
			{
				return m_BuildingUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Node>(ref types.m_NodeType) || ((ArchetypeChunk)(ref chunk)).Has<Edge>(ref types.m_EdgeType))
			{
				return m_NetUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Lane>(ref types.m_LaneType))
			{
				return m_LaneUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<CompanyData>(ref types.m_CompanyType))
			{
				return m_CompanyUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Household>(ref types.m_HouseholdType))
			{
				return m_HouseholdUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Citizen>(ref types.m_CitizenType))
			{
				return m_CitizenUpdateGroupSizes;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<HouseholdPet>(ref types.m_HouseholdPetType))
			{
				return m_HouseholdPetUpdateGroupSizes;
			}
			return default(NativeArray<int>);
		}
	}

	[BurstCompile]
	private struct UpdateGroupJob : IJob
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Applied> m_AppliedType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Created> m_CreatedData;

		[ReadOnly]
		public ComponentLookup<UpdateFrameData> m_PrefabUpdateFrameData;

		public BufferLookup<TransformFrame> m_TransformFrameData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public UpdateGroupTypes m_UpdateGroupTypes;

		public EntityCommandBuffer m_CommandBuffer;

		public UpdateGroupSizes m_UpdateGroupSizes;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk chunk = m_Chunks[i];
				if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType) && !((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
				{
					CheckDeleted(chunk);
				}
			}
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk chunk2 = m_Chunks[j];
				if (((ArchetypeChunk)(ref chunk2)).Has<Created>(ref m_CreatedType) && !((ArchetypeChunk)(ref chunk2)).Has<Deleted>(ref m_DeletedType))
				{
					CheckCreated(chunk2);
				}
			}
		}

		private NativeArray<int> GetGroupSizeArray(ArchetypeChunk chunk)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<int> result = m_UpdateGroupSizes.Get(chunk, m_UpdateGroupTypes);
			if (!result.IsCreated)
			{
				EntityArchetype archetype = ((ArchetypeChunk)(ref chunk)).Archetype;
				NativeArray<ComponentType> componentTypes = ((EntityArchetype)(ref archetype)).GetComponentTypes((Allocator)2);
				Debug.Log((object)"UpdateFrame added to unsupported type");
				for (int i = 0; i < componentTypes.Length; i++)
				{
					Debug.Log((object)$"Component: {componentTypes[i]}");
				}
				componentTypes.Dispose();
			}
			return result;
		}

		private void CheckDeleted(ArchetypeChunk chunk)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				NativeArray<int> groupSizeArray = GetGroupSizeArray(chunk);
				if (index < groupSizeArray.Length)
				{
					groupSizeArray[(int)index] = groupSizeArray[(int)index] - ((ArchetypeChunk)(ref chunk)).Count;
				}
			}
		}

		private int GetUpdateFrame(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (m_CreatedData.HasComponent(entity))
			{
				return -1;
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(entity))
			{
				return -1;
			}
			ArchetypeChunk chunk = ((EntityStorageInfoLookup)(ref m_EntityLookup))[entity].Chunk;
			if (!((ArchetypeChunk)(ref chunk)).Has<UpdateFrame>(m_UpdateFrameType))
			{
				return -1;
			}
			return (int)((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
		}

		private void CheckCreated(ArchetypeChunk chunk)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<int> groupSizeArray = GetGroupSizeArray(chunk);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<InterpolatedTransform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
			NativeArray<Controller> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Controller>(ref m_ControllerType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					int num = -1;
					if (nativeArray4.Length != 0)
					{
						Controller controller = nativeArray4[i];
						if (controller.m_Controller != Entity.Null && controller.m_Controller != val)
						{
							num = GetUpdateFrame(controller.m_Controller);
							if (num == -1)
							{
								continue;
							}
						}
					}
					Temp temp = nativeArray2[i];
					if (temp.m_Original != Entity.Null)
					{
						num = GetUpdateFrame(temp.m_Original);
					}
					uint index;
					if (nativeArray5.Length != 0)
					{
						PrefabRef prefabRef = nativeArray5[i];
						index = FindUpdateIndex(num, groupSizeArray, prefabRef);
					}
					else
					{
						index = FindUpdateIndex(num, groupSizeArray);
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetSharedComponent<UpdateFrame>(val, new UpdateFrame(index));
					if (bufferAccessor.Length == 0)
					{
						continue;
					}
					DynamicBuffer<LayoutElement> val2 = bufferAccessor[i];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity vehicle = val2[j].m_Vehicle;
						if (vehicle != val)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).SetSharedComponent<UpdateFrame>(vehicle, new UpdateFrame(index));
						}
					}
				}
				if (nativeArray3.Length == 0)
				{
					return;
				}
				NativeArray<Transform> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					Temp temp2 = nativeArray2[k];
					Transform transform = nativeArray6[k];
					if (m_TransformFrameData.HasBuffer(val3))
					{
						DynamicBuffer<TransformFrame> val4 = m_TransformFrameData[val3];
						if (m_TransformFrameData.HasBuffer(temp2.m_Original))
						{
							DynamicBuffer<TransformFrame> val5 = m_TransformFrameData[temp2.m_Original];
							val4.ResizeUninitialized(val5.Length);
							for (int l = 0; l < val4.Length; l++)
							{
								val4[l] = val5[l];
							}
						}
						else
						{
							val4.ResizeUninitialized(4);
							for (int m = 0; m < val4.Length; m++)
							{
								val4[m] = new TransformFrame(transform);
							}
						}
					}
					nativeArray3[k] = new InterpolatedTransform(transform);
				}
				return;
			}
			uint index2 = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Applied>(ref m_AppliedType);
			for (int n = 0; n < nativeArray.Length; n++)
			{
				Entity val6 = nativeArray[n];
				uint num2;
				if (flag)
				{
					num2 = index2;
				}
				else
				{
					int num3 = -1;
					if (nativeArray4.Length != 0 && !flag)
					{
						Controller controller2 = nativeArray4[n];
						if (controller2.m_Controller != Entity.Null && controller2.m_Controller != val6)
						{
							num3 = GetUpdateFrame(controller2.m_Controller);
							if (num3 == -1)
							{
								continue;
							}
						}
					}
					if (nativeArray5.Length != 0)
					{
						PrefabRef prefabRef2 = nativeArray5[n];
						num2 = FindUpdateIndex(num3, groupSizeArray, prefabRef2);
					}
					else
					{
						num2 = FindUpdateIndex(num3, groupSizeArray);
					}
				}
				int num4 = 1;
				if (!flag)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetSharedComponent<UpdateFrame>(val6, new UpdateFrame(num2));
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<LayoutElement> val7 = bufferAccessor[n];
						for (int num5 = 0; num5 < val7.Length; num5++)
						{
							Entity vehicle2 = val7[num5].m_Vehicle;
							if (vehicle2 != val6)
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).SetSharedComponent<UpdateFrame>(vehicle2, new UpdateFrame(num2));
								num4++;
							}
						}
					}
				}
				if (num2 < groupSizeArray.Length)
				{
					groupSizeArray[(int)num2] = groupSizeArray[(int)num2] + num4;
				}
			}
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<Transform> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			for (int num6 = 0; num6 < nativeArray.Length; num6++)
			{
				Entity val8 = nativeArray[num6];
				Transform transform2 = nativeArray7[num6];
				if (m_TransformFrameData.HasBuffer(val8))
				{
					DynamicBuffer<TransformFrame> val9 = m_TransformFrameData[val8];
					val9.ResizeUninitialized(4);
					for (int num7 = 0; num7 < val9.Length; num7++)
					{
						val9[num7] = new TransformFrame(transform2);
					}
				}
				nativeArray3[num6] = new InterpolatedTransform(transform2);
			}
		}

		private uint FindUpdateIndex(NativeArray<int> groupSizes, PrefabRef prefabRef)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabUpdateFrameData.HasComponent(prefabRef.m_Prefab))
			{
				return (uint)m_PrefabUpdateFrameData[prefabRef.m_Prefab].m_UpdateGroupIndex;
			}
			return FindUpdateIndex(groupSizes);
		}

		private uint FindUpdateIndex(int originalIndex, NativeArray<int> groupSizes, PrefabRef prefabRef)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (originalIndex != -1)
			{
				return (uint)originalIndex;
			}
			return FindUpdateIndex(groupSizes, prefabRef);
		}

		private uint FindUpdateIndex(NativeArray<int> groupSizes)
		{
			uint result = uint.MaxValue;
			int num = int.MaxValue;
			for (int i = 0; i < groupSizes.Length; i++)
			{
				int num2 = groupSizes[i];
				if (num2 < num)
				{
					num = num2;
					result = (uint)i;
				}
			}
			return result;
		}

		private uint FindUpdateIndex(int originalIndex, NativeArray<int> groupSizes)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (originalIndex != -1)
			{
				return (uint)originalIndex;
			}
			return FindUpdateIndex(groupSizes);
		}
	}

	[BurstCompile]
	private struct MovingObjectsUpdatedJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> m_HumanNavigationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[NativeDisableParallelForRestriction]
		public BufferLookup<TransformFrame> m_TransformFrameData;

		[ReadOnly]
		public uint m_SimulationFrame;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<HumanNavigation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_HumanNavigationType);
			NativeArray<InterpolatedTransform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
			NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			uint num = m_SimulationFrame % 16;
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			uint num2 = (m_SimulationFrame + num - index) / 16 % 4;
			DynamicBuffer<TransformFrame> val2 = default(DynamicBuffer<TransformFrame>);
			DynamicBuffer<TransformFrame> val3 = default(DynamicBuffer<TransformFrame>);
			HumanNavigation humanNavigation = default(HumanNavigation);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				Entity val = nativeArray[i];
				Transform transform = nativeArray2[i];
				Temp temp = default(Temp);
				if (nativeArray5.Length != 0)
				{
					temp = nativeArray5[i];
				}
				if (m_TransformFrameData.TryGetBuffer(val, ref val2))
				{
					if (m_TransformFrameData.TryGetBuffer(temp.m_Original, ref val3))
					{
						val2.ResizeUninitialized(val3.Length);
						for (int j = 0; j < val2.Length; j++)
						{
							val2[j] = val3[j];
						}
					}
					else
					{
						val2.ResizeUninitialized(4);
						TransformFrame transformFrame = new TransformFrame(transform);
						if (CollectionUtils.TryGet<HumanNavigation>(nativeArray3, i, ref humanNavigation))
						{
							transformFrame.m_Activity = humanNavigation.m_LastActivity;
							transformFrame.m_State = humanNavigation.m_TransformState;
						}
						for (int k = 0; k < val2.Length; k++)
						{
							TransformFrame transformFrame2 = transformFrame;
							transformFrame2.m_StateTimer = (ushort)k;
							int num3 = k - (int)num2 - 1;
							num3 = math.select(num3, num3 + val2.Length, num3 < 0);
							val2[num3] = transformFrame2;
						}
					}
				}
				nativeArray4[i] = new InterpolatedTransform(transform);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Applied> __Game_Common_Applied_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Created> __Game_Common_Created_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UpdateFrameData> __Game_Prefabs_UpdateFrameData_RO_ComponentLookup;

		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Applied_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Applied>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Created_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Created>(true);
			__Game_Prefabs_UpdateFrameData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UpdateFrameData>(true);
			__Game_Objects_TransformFrame_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(false);
			__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_CreatedQuery;

	private EntityQuery m_UpdatedQuery;

	private UpdateGroupTypes m_UpdateGroupTypes;

	private UpdateGroupSizes m_UpdateGroupSizes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UpdateFrame>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<Moving>(),
			ComponentType.Exclude<Created>()
		});
		m_UpdateGroupTypes = new UpdateGroupTypes((SystemBase)(object)this);
		m_UpdateGroupSizes = new UpdateGroupSizes((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_UpdateGroupSizes.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_CreatedQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_CreatedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			m_UpdateGroupTypes.Update((SystemBase)(object)this);
			UpdateGroupJob updateGroupJob = new UpdateGroupJob
			{
				m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AppliedType = InternalCompilerInterface.GetComponentTypeHandle<Applied>(ref __TypeHandle.__Game_Common_Applied_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedData = InternalCompilerInterface.GetComponentLookup<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabUpdateFrameData = InternalCompilerInterface.GetComponentLookup<UpdateFrameData>(ref __TypeHandle.__Game_Prefabs_UpdateFrameData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformFrameData = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Chunks = chunks,
				m_UpdateGroupTypes = m_UpdateGroupTypes,
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
				m_UpdateGroupSizes = m_UpdateGroupSizes
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateGroupJob>(updateGroupJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			chunks.Dispose(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		if (!((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter)
		{
			MovingObjectsUpdatedJob movingObjectsUpdatedJob = new MovingObjectsUpdatedJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HumanNavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformFrameData = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SimulationFrame = m_SimulationSystem.frameIndex
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<MovingObjectsUpdatedJob>(movingObjectsUpdatedJob, m_UpdatedQuery, ((SystemBase)this).Dependency);
		}
	}

	public UpdateGroupSizes GetUpdateGroupSizes()
	{
		return m_UpdateGroupSizes;
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
	public UpdateGroupSystem()
	{
	}
}
