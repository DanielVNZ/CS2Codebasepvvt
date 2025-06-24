using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Effects;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
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

namespace Game.Objects;

[CompilerGenerated]
public class SubObjectSystem : GameSystemBase
{
	private struct SubObjectOwnerData
	{
		public Entity m_Owner;

		public Entity m_Original;

		public bool m_Temp;

		public bool m_Created;

		public bool m_Deleted;

		public SubObjectOwnerData(Entity owner, Entity original, bool temp, bool created, bool deleted)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Owner = owner;
			m_Original = original;
			m_Temp = temp;
			m_Created = created;
			m_Deleted = deleted;
		}
	}

	public struct SubObjectData : IComparable<SubObjectData>
	{
		public Entity m_SubObject;

		public float m_Radius;

		public SubObjectData(Entity subObject, float radius)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_SubObject = subObject;
			m_Radius = radius;
		}

		public int CompareTo(SubObjectData other)
		{
			return math.select(0, math.select(1, -1, m_Radius > other.m_Radius), m_Radius != other.m_Radius);
		}
	}

	private struct DeepSubObjectOwnerData
	{
		public Transform m_Transform;

		public Temp m_Temp;

		public Entity m_Entity;

		public Entity m_Prefab;

		public float m_Elevation;

		public PseudoRandomSeed m_RandomSeed;

		public bool m_Deleted;

		public bool m_New;

		public bool m_HasRandomSeed;

		public bool m_IsSubRandom;

		public bool m_UnderConstruction;

		public bool m_Destroyed;

		public bool m_Overridden;

		public int m_Depth;
	}

	private struct PlaceholderKey : IEquatable<PlaceholderKey>
	{
		public Entity m_GroupPrefab;

		public int m_GroupIndex;

		public PlaceholderKey(Entity groupPrefab, int groupIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_GroupPrefab = groupPrefab;
			m_GroupIndex = groupIndex;
		}

		public bool Equals(PlaceholderKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_GroupPrefab)).Equals(other.m_GroupPrefab))
			{
				return m_GroupIndex == other.m_GroupIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_GroupPrefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_GroupIndex.GetHashCode();
		}
	}

	private struct UpdateSubObjectsData
	{
		public NativeParallelMultiHashMap<Entity, Entity> m_OldEntities;

		public NativeParallelMultiHashMap<Entity, Entity> m_OriginalEntities;

		public NativeParallelHashMap<Entity, int2> m_PlaceholderRequirements;

		public NativeParallelHashMap<PlaceholderKey, Random> m_SelectedSpawnabled;

		public NativeList<AreaUtils.ObjectItem> m_ObjectBuffer;

		public NativeList<DeepSubObjectOwnerData> m_DeepOwners;

		public NativeList<ClearAreaData> m_ClearAreas;

		public ObjectRequirementFlags m_PlaceholderRequirementFlags;

		public Resource m_StoredResources;

		public bool m_RequirementsSearched;

		public void EnsureOldEntities(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OldEntities.IsCreated)
			{
				m_OldEntities = new NativeParallelMultiHashMap<Entity, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureOriginalEntities(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OriginalEntities.IsCreated)
			{
				m_OriginalEntities = new NativeParallelMultiHashMap<Entity, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsurePlaceholderRequirements(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PlaceholderRequirements.IsCreated)
			{
				m_PlaceholderRequirements = new NativeParallelHashMap<Entity, int2>(10, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureSelectedSpawnables(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SelectedSpawnabled.IsCreated)
			{
				m_SelectedSpawnabled = new NativeParallelHashMap<PlaceholderKey, Random>(10, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureObjectBuffer(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ObjectBuffer.IsCreated)
			{
				m_ObjectBuffer = new NativeList<AreaUtils.ObjectItem>(32, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureDeepOwners(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_DeepOwners.IsCreated)
			{
				m_DeepOwners = new NativeList<DeepSubObjectOwnerData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void Clear(bool deepOwners)
		{
			if (m_OldEntities.IsCreated)
			{
				m_OldEntities.Clear();
			}
			if (m_OriginalEntities.IsCreated)
			{
				m_OriginalEntities.Clear();
			}
			if (deepOwners && m_PlaceholderRequirements.IsCreated)
			{
				m_PlaceholderRequirements.Clear();
			}
			if (deepOwners && m_SelectedSpawnabled.IsCreated)
			{
				m_SelectedSpawnabled.Clear();
			}
			if (m_ObjectBuffer.IsCreated)
			{
				m_ObjectBuffer.Clear();
			}
			if (deepOwners && m_DeepOwners.IsCreated)
			{
				m_DeepOwners.Clear();
			}
			if (m_ClearAreas.IsCreated)
			{
				m_ClearAreas.Clear();
			}
			if (deepOwners)
			{
				m_PlaceholderRequirementFlags = (ObjectRequirementFlags)0;
				m_StoredResources = Resource.NoResource;
				m_RequirementsSearched = false;
			}
		}

		public void Dispose()
		{
			if (m_OldEntities.IsCreated)
			{
				m_OldEntities.Dispose();
			}
			if (m_OriginalEntities.IsCreated)
			{
				m_OriginalEntities.Dispose();
			}
			if (m_PlaceholderRequirements.IsCreated)
			{
				m_PlaceholderRequirements.Dispose();
			}
			if (m_SelectedSpawnabled.IsCreated)
			{
				m_SelectedSpawnabled.Dispose();
			}
			if (m_ObjectBuffer.IsCreated)
			{
				m_ObjectBuffer.Dispose();
			}
			if (m_DeepOwners.IsCreated)
			{
				m_DeepOwners.Dispose();
			}
			if (m_ClearAreas.IsCreated)
			{
				m_ClearAreas.Dispose();
			}
		}
	}

	[BurstCompile]
	private struct CheckSubObjectOwnersJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<SubObject> m_SubObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Object> m_ObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<SubObjectsUpdated> m_SubObjectsUpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> m_RentersUpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Creature> m_CreatureType;

		[ReadOnly]
		public ComponentLookup<Created> m_CreatedData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<Object> m_ObjectData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SubObjectOwnerData> m_OwnerQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RentersUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RentersUpdated>(ref m_RentersUpdatedType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity property = nativeArray[i].m_Property;
					if (m_SubObjects.HasBuffer(property))
					{
						m_OwnerQueue.Enqueue(new SubObjectOwnerData(property, Entity.Null, temp: false, m_CreatedData.HasComponent(property), m_DeletedData.HasComponent(property)));
					}
				}
				return;
			}
			NativeArray<SubObjectsUpdated> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SubObjectsUpdated>(ref m_SubObjectsUpdatedType);
			if (nativeArray2.Length != 0)
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity owner = nativeArray2[j].m_Owner;
					if (m_SubObjects.HasBuffer(owner))
					{
						m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner, Entity.Null, temp: false, m_CreatedData.HasComponent(owner), m_DeletedData.HasComponent(owner)));
					}
				}
				return;
			}
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			if (((ArchetypeChunk)(ref chunk)).Has<Object>(ref m_ObjectType) && ((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.ServiceUpgrade>(ref m_ServiceUpgradeType) && !((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType) && !((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType) && !((ArchetypeChunk)(ref chunk)).Has<Creature>(ref m_CreatureType) && (nativeArray3.Length == 0 || flag))
			{
				return;
			}
			NativeArray<Entity> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			if (flag)
			{
				for (int k = 0; k < nativeArray4.Length; k++)
				{
					Entity val = nativeArray4[k];
					DynamicBuffer<SubObject> val2 = bufferAccessor[k];
					for (int l = 0; l < val2.Length; l++)
					{
						Entity subObject = val2[l].m_SubObject;
						if (m_DeletedData.HasComponent(subObject) || m_SecondaryData.HasComponent(subObject))
						{
							continue;
						}
						if (m_OwnerData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == val && (m_ServiceUpgradeData.HasComponent(subObject) || m_BuildingData.HasComponent(subObject)))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, subObject, ref m_AppliedTypes);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, subObject, default(Deleted));
							if (m_SubObjects.HasBuffer(subObject))
							{
								m_OwnerQueue.Enqueue(new SubObjectOwnerData(subObject, Entity.Null, temp: false, created: false, deleted: true));
							}
						}
						if (m_AttachedData.HasComponent(subObject) && m_AttachedData[subObject].m_Parent == val)
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Attached>(unfilteredChunkIndex, subObject, default(Attached));
						}
					}
				}
			}
			bool created = ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType);
			for (int m = 0; m < nativeArray4.Length; m++)
			{
				Entity val3 = nativeArray4[m];
				if (nativeArray3.Length != 0)
				{
					Temp temp = nativeArray3[m];
					m_OwnerQueue.Enqueue(new SubObjectOwnerData(val3, temp.m_Original, temp: true, created, flag));
					if (flag || !m_SubObjects.HasBuffer(temp.m_Original))
					{
						continue;
					}
					DynamicBuffer<SubObject> val4 = m_SubObjects[temp.m_Original];
					for (int n = 0; n < val4.Length; n++)
					{
						Entity subObject2 = val4[n].m_SubObject;
						if (!m_OwnerData.HasComponent(subObject2) || !m_AttachedData.HasComponent(subObject2) || m_SecondaryData.HasComponent(subObject2))
						{
							continue;
						}
						Owner owner2 = m_OwnerData[subObject2];
						if (owner2.m_Owner != temp.m_Original && m_AttachedData[subObject2].m_Parent == temp.m_Original && !m_HiddenData.HasComponent(owner2.m_Owner))
						{
							while (m_OwnerData.HasComponent(owner2.m_Owner) && m_ObjectData.HasComponent(owner2.m_Owner) && !m_ServiceUpgradeData.HasComponent(owner2.m_Owner) && !m_BuildingData.HasComponent(owner2.m_Owner))
							{
								owner2 = m_OwnerData[owner2.m_Owner];
							}
							m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner2.m_Owner, Entity.Null, temp: true, m_CreatedData.HasComponent(owner2.m_Owner), m_DeletedData.HasComponent(owner2.m_Owner)));
						}
					}
					continue;
				}
				m_OwnerQueue.Enqueue(new SubObjectOwnerData(val3, Entity.Null, temp: false, created, flag));
				DynamicBuffer<SubObject> val5 = bufferAccessor[m];
				for (int num = 0; num < val5.Length; num++)
				{
					Entity subObject3 = val5[num].m_SubObject;
					if (!m_OwnerData.HasComponent(subObject3) || !m_AttachedData.HasComponent(subObject3) || m_SecondaryData.HasComponent(subObject3))
					{
						continue;
					}
					Owner owner3 = m_OwnerData[subObject3];
					if (owner3.m_Owner != val3 && m_AttachedData[subObject3].m_Parent == val3 && !m_HiddenData.HasComponent(owner3.m_Owner))
					{
						while (m_OwnerData.HasComponent(owner3.m_Owner) && m_ObjectData.HasComponent(owner3.m_Owner) && !m_ServiceUpgradeData.HasComponent(owner3.m_Owner) && !m_BuildingData.HasComponent(owner3.m_Owner))
						{
							owner3 = m_OwnerData[owner3.m_Owner];
						}
						m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner3.m_Owner, Entity.Null, temp: false, m_CreatedData.HasComponent(owner3.m_Owner), m_DeletedData.HasComponent(owner3.m_Owner)));
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
	private struct CollectSubObjectOwnersJob : IJob
	{
		public NativeQueue<SubObjectOwnerData> m_OwnerQueue;

		public NativeList<SubObjectOwnerData> m_OwnerList;

		public NativeParallelHashMap<Entity, SubObjectOwnerData> m_OwnerMap;

		public void Execute()
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			SubObjectOwnerData subObjectOwnerData = default(SubObjectOwnerData);
			SubObjectOwnerData subObjectOwnerData2 = default(SubObjectOwnerData);
			while (m_OwnerQueue.TryDequeue(ref subObjectOwnerData))
			{
				if (m_OwnerMap.TryGetValue(subObjectOwnerData.m_Owner, ref subObjectOwnerData2))
				{
					if (subObjectOwnerData.m_Original != Entity.Null)
					{
						subObjectOwnerData.m_Created |= subObjectOwnerData2.m_Created;
						subObjectOwnerData.m_Deleted |= subObjectOwnerData2.m_Deleted;
						m_OwnerMap[subObjectOwnerData.m_Owner] = subObjectOwnerData;
					}
					else
					{
						subObjectOwnerData2.m_Created |= subObjectOwnerData.m_Created;
						subObjectOwnerData2.m_Deleted |= subObjectOwnerData.m_Deleted;
						m_OwnerMap[subObjectOwnerData.m_Owner] = subObjectOwnerData2;
					}
				}
				else
				{
					m_OwnerMap.Add(subObjectOwnerData.m_Owner, subObjectOwnerData);
				}
			}
			m_OwnerList.SetCapacity(m_OwnerMap.Count());
			Enumerator<Entity, SubObjectOwnerData> enumerator = m_OwnerMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				m_OwnerList.Add(ref enumerator.Current.Value);
			}
			enumerator.Dispose();
		}
	}

	[BurstCompile]
	private struct FillIgnoreSetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		public NativeParallelHashSet<Entity> m_IgnoreSet;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Temp temp = nativeArray2[i];
				m_IgnoreSet.Add(val);
				if (temp.m_Original != Entity.Null)
				{
					m_IgnoreSet.Add(temp.m_Original);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateSubObjectsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<PillarData> m_PrefabPillarData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<ThemeData> m_PrefabThemeData;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> m_PrefabMovingObjectData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> m_PrefabWorkVehicleData;

		[ReadOnly]
		public ComponentLookup<EffectData> m_PrefabEffectData;

		[ReadOnly]
		public ComponentLookup<StreetLightData> m_PrefabStreetLightData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PrefabPlaceableNetData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_PrefabCargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> m_PrefabPlaceholderObjectData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Aligned> m_AlignedData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<StreetLight> m_StreetLightData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<Surface> m_SurfaceData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducerData;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> m_GarbageFacilityData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<ResidentialProperty> m_ResidentialPropertyData;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> m_CityServiceUpkeepData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NetNodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_NetEdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_NetCurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<CompanyData> m_CompanyData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdData;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHousehold;

		[ReadOnly]
		public ComponentLookup<Area> m_AreaData;

		[ReadOnly]
		public ComponentLookup<Geometry> m_AreaGeometryData;

		[ReadOnly]
		public ComponentLookup<Clear> m_AreaClearData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<Watercraft> m_WatercraftData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		[ReadOnly]
		public BufferLookup<Renter> m_BuildingRenters;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> m_PrefabSubLanes;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PlaceholderObjects;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirements;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_PrefabActivityLocations;

		[ReadOnly]
		public BufferLookup<CompanyBrandElement> m_CompanyBrands;

		[ReadOnly]
		public BufferLookup<AffiliatedBrandElement> m_AffiliatedBrands;

		[ReadOnly]
		public BufferLookup<SubMesh> m_PrefabSubMeshes;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_PrefabProceduralBones;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_PrefabServiceUpkeepDatas;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public Entity m_DefaultTheme;

		[ReadOnly]
		public Entity m_TransformEditor;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		[ReadOnly]
		public CitizenHappinessParameterData m_HappinessParameterData;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public NativeArray<SubObjectOwnerData> m_OwnerList;

		[ReadOnly]
		public NativeParallelHashSet<Entity> m_IgnoreSet;

		[ReadOnly]
		public NativeParallelHashMap<Entity, SubObjectOwnerData> m_OwnerMap;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public ParallelWriter<Entity> m_LoopErrorPrefabs;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			SubObjectOwnerData subObjectOwnerData = m_OwnerList[index];
			PrefabRef prefabRef = m_PrefabRefData[subObjectOwnerData.m_Owner];
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool relative = false;
			bool interpolated = false;
			bool native = m_NativeData.HasComponent(subObjectOwnerData.m_Owner);
			if (m_TransformData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag = true;
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				if (!m_EditorMode && (m_PrefabMovingObjectData.HasComponent(prefabRef.m_Prefab) || (m_PrefabPlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData) && (placeableObjectData.m_Flags & PlacementFlags.Swaying) != PlacementFlags.None)))
				{
					relative = true;
					interpolated = m_InterpolatedTransformData.HasComponent(subObjectOwnerData.m_Owner);
				}
			}
			else if (m_NetNodeData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag2 = true;
			}
			else if (m_NetEdgeData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag3 = true;
			}
			else if (m_AreaData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag4 = true;
			}
			Owner owner = default(Owner);
			if (!subObjectOwnerData.m_Deleted && flag && m_ServiceUpgradeData.HasComponent(subObjectOwnerData.m_Owner) && m_OwnerData.TryGetComponent(subObjectOwnerData.m_Owner, ref owner) && m_OwnerMap.ContainsKey(owner.m_Owner))
			{
				return;
			}
			bool flag5 = false;
			Temp ownerTemp = default(Temp);
			if (m_TempData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag5 = true;
				ownerTemp = m_TempData[subObjectOwnerData.m_Owner];
			}
			bool flag6 = false;
			float ownerElevation = 0f;
			DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
			if (flag)
			{
				m_InstalledUpgrades.TryGetBuffer(subObjectOwnerData.m_Owner, ref installedUpgrades);
			}
			if (!subObjectOwnerData.m_Deleted)
			{
				if (!flag2 && !flag3)
				{
					if (subObjectOwnerData.m_Temp)
					{
						if ((ownerTemp.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0 || !flag5 || (m_EditorMode && ownerTemp.m_Original != Entity.Null && (!installedUpgrades.IsCreated || installedUpgrades.Length == 0)))
						{
							flag6 = true;
						}
					}
					else if (!subObjectOwnerData.m_Created && m_EditorMode && !m_UpdatedData.HasComponent(prefabRef.m_Prefab) && (!installedUpgrades.IsCreated || installedUpgrades.Length == 0))
					{
						return;
					}
				}
				if (m_ElevationData.HasComponent(subObjectOwnerData.m_Owner))
				{
					ownerElevation = m_ElevationData[subObjectOwnerData.m_Owner].m_Elevation;
				}
				else if (m_NetElevationData.HasComponent(subObjectOwnerData.m_Owner))
				{
					ownerElevation = math.cmin(m_NetElevationData[subObjectOwnerData.m_Owner].m_Elevation);
				}
			}
			if (subObjectOwnerData.m_Temp && !flag5)
			{
				subObjectOwnerData.m_Original = subObjectOwnerData.m_Owner;
			}
			Owner owner2 = default(Owner);
			Temp temp = default(Temp);
			if (flag6 && flag4 && (ownerTemp.m_Flags & TempFlags.Select) != 0 && m_OwnerData.TryGetComponent(subObjectOwnerData.m_Owner, ref owner2) && m_TempData.TryGetComponent(owner2.m_Owner, ref temp) && (temp.m_Flags & TempFlags.Select) != 0)
			{
				ownerTemp.m_Flags &= ~TempFlags.Select;
			}
			UpdateSubObjectsData updateData = default(UpdateSubObjectsData);
			DynamicBuffer<SubObject> subObjects = m_SubObjects[subObjectOwnerData.m_Owner];
			FillOldSubObjectsBuffer(subObjectOwnerData.m_Owner, subObjects, ref updateData, subObjectOwnerData.m_Temp, flag6);
			if (!subObjectOwnerData.m_Deleted)
			{
				if (m_SubObjects.HasBuffer(subObjectOwnerData.m_Original))
				{
					DynamicBuffer<SubObject> subObjects2 = m_SubObjects[subObjectOwnerData.m_Original];
					FillOriginalSubObjectsBuffer(subObjectOwnerData.m_Original, subObjects2, ref updateData, flag6);
				}
				if (installedUpgrades.IsCreated)
				{
					FillClearAreas(subObjectOwnerData.m_Owner, installedUpgrades, ref updateData);
				}
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				Random random = ((!m_PseudoRandomSeedData.TryGetComponent(subObjectOwnerData.m_Owner, ref pseudoRandomSeed)) ? m_RandomSeed.GetRandom(index) : pseudoRandomSeed.GetRandom(PseudoRandomSeed.kSubObject));
				Random subRandom = random;
				EnsurePlaceholderRequirements(subObjectOwnerData.m_Owner, prefabRef.m_Prefab, ref updateData, ref random, flag);
				if (flag6)
				{
					Transform ownerTransform = default(Transform);
					if (flag)
					{
						ownerTransform = m_TransformData[subObjectOwnerData.m_Owner];
					}
					DuplicateSubObjects(index, ref random, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Original, ownerTransform, ref updateData, prefabRef.m_Prefab, subObjectOwnerData.m_Temp, ownerTemp, ownerElevation, flag, native, relative, interpolated, 0);
				}
				else if (flag)
				{
					Transform transform = m_TransformData[subObjectOwnerData.m_Owner];
					bool isUnderConstruction = false;
					bool isDestroyed = m_DestroyedData.HasComponent(subObjectOwnerData.m_Owner);
					bool isOverridden = m_OverriddenData.HasComponent(subObjectOwnerData.m_Owner);
					UnderConstruction underConstruction = default(UnderConstruction);
					if (m_UnderConstructionData.TryGetComponent(subObjectOwnerData.m_Owner, ref underConstruction))
					{
						isUnderConstruction = underConstruction.m_NewPrefab == Entity.Null;
					}
					CreateSubObjects(index, ref random, ref subRandom, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Owner, transform, transform, transform, ref updateData, prefabRef.m_Prefab, isTransform: true, isEdge: false, isNode: false, subObjectOwnerData.m_Temp, ownerTemp, ownerElevation, native, relative, interpolated, isUnderConstruction, isDestroyed, isOverridden, 0);
				}
				else if (flag2)
				{
					Game.Net.Node node = m_NetNodeData[subObjectOwnerData.m_Owner];
					Transform transform2 = new Transform(node.m_Position, node.m_Rotation);
					CreateSubObjects(index, ref random, ref subRandom, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Owner, transform2, transform2, transform2, ref updateData, prefabRef.m_Prefab, isTransform: false, isEdge: false, isNode: true, subObjectOwnerData.m_Temp, ownerTemp, ownerElevation, native, relative: false, interpolated: false, isUnderConstruction: false, isDestroyed: false, isOverridden: false, 0);
				}
				else if (flag3)
				{
					Curve curve = m_NetCurveData[subObjectOwnerData.m_Owner];
					CreateSubObjects(transform1: new Transform(curve.m_Bezier.a, NetUtils.GetNodeRotation(MathUtils.StartTangent(curve.m_Bezier))), transform2: new Transform(MathUtils.Position(curve.m_Bezier, 0.5f), NetUtils.GetNodeRotation(MathUtils.Tangent(curve.m_Bezier, 0.5f))), transform3: new Transform(curve.m_Bezier.d, NetUtils.GetNodeRotation(-MathUtils.EndTangent(curve.m_Bezier))), jobIndex: index, random: ref random, subRandom: ref subRandom, topOwner: subObjectOwnerData.m_Owner, owner: subObjectOwnerData.m_Owner, updateData: ref updateData, prefab: prefabRef.m_Prefab, isTransform: false, isEdge: true, isNode: false, isTemp: subObjectOwnerData.m_Temp, ownerTemp: ownerTemp, ownerElevation: ownerElevation, native: native, relative: false, interpolated: false, isUnderConstruction: false, isDestroyed: false, isOverridden: false, depth: 0);
				}
				else if (flag4)
				{
					Area area = m_AreaData[subObjectOwnerData.m_Owner];
					Geometry geometry = m_AreaGeometryData[subObjectOwnerData.m_Owner];
					DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[subObjectOwnerData.m_Owner];
					DynamicBuffer<Triangle> triangles = m_AreaTriangles[subObjectOwnerData.m_Owner];
					RelocateSubObjects(index, ref random, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Owner, subObjectOwnerData.m_Original, area, geometry, nodes, triangles, ref updateData, prefabRef.m_Prefab, subObjectOwnerData.m_Temp, ownerTemp, ownerElevation);
				}
				if (installedUpgrades.IsCreated)
				{
					SubObjectOwnerData subObjectOwnerData2 = default(SubObjectOwnerData);
					Elevation elevation = default(Elevation);
					UnderConstruction underConstruction2 = default(UnderConstruction);
					Temp temp2 = default(Temp);
					for (int i = 0; i < installedUpgrades.Length; i++)
					{
						if (m_OwnerMap.TryGetValue(installedUpgrades[i].m_Upgrade, ref subObjectOwnerData2) && !subObjectOwnerData2.m_Deleted)
						{
							updateData.EnsureDeepOwners((Allocator)2);
							DeepSubObjectOwnerData deepSubObjectOwnerData = new DeepSubObjectOwnerData
							{
								m_Transform = m_TransformData[subObjectOwnerData2.m_Owner],
								m_Entity = subObjectOwnerData2.m_Owner,
								m_Prefab = m_PrefabRefData[subObjectOwnerData2.m_Owner],
								m_RandomSeed = m_PseudoRandomSeedData[subObjectOwnerData2.m_Owner],
								m_HasRandomSeed = true,
								m_IsSubRandom = true,
								m_Depth = 1
							};
							if (m_ElevationData.TryGetComponent(subObjectOwnerData2.m_Owner, ref elevation))
							{
								deepSubObjectOwnerData.m_Elevation = elevation.m_Elevation;
							}
							if (m_UnderConstructionData.TryGetComponent(subObjectOwnerData2.m_Owner, ref underConstruction2))
							{
								deepSubObjectOwnerData.m_UnderConstruction = underConstruction2.m_NewPrefab == Entity.Null;
							}
							if (m_TempData.TryGetComponent(subObjectOwnerData2.m_Owner, ref temp2))
							{
								deepSubObjectOwnerData.m_Temp = temp2;
							}
							deepSubObjectOwnerData.m_Destroyed = m_DestroyedData.HasComponent(subObjectOwnerData2.m_Owner);
							deepSubObjectOwnerData.m_Overridden = m_OverriddenData.HasComponent(subObjectOwnerData2.m_Owner);
							updateData.m_DeepOwners.Add(ref deepSubObjectOwnerData);
						}
					}
				}
			}
			RemoveUnusedOldSubObjects(index, subObjectOwnerData.m_Owner, subObjects, ref updateData, subObjectOwnerData.m_Temp, flag6);
			if (updateData.m_DeepOwners.IsCreated)
			{
				int num = 0;
				PseudoRandomSeed pseudoRandomSeed2 = default(PseudoRandomSeed);
				while (num < updateData.m_DeepOwners.Length)
				{
					DeepSubObjectOwnerData deepSubObjectOwnerData2 = updateData.m_DeepOwners[num++];
					updateData.Clear(deepOwners: false);
					if (!deepSubObjectOwnerData2.m_New)
					{
						subObjects = m_SubObjects[deepSubObjectOwnerData2.m_Entity];
						FillOldSubObjectsBuffer(deepSubObjectOwnerData2.m_Entity, subObjects, ref updateData, subObjectOwnerData.m_Temp, flag6);
					}
					if (!deepSubObjectOwnerData2.m_Deleted)
					{
						Random random2 = ((!deepSubObjectOwnerData2.m_HasRandomSeed) ? m_RandomSeed.GetRandom(index + num * 137209) : deepSubObjectOwnerData2.m_RandomSeed.GetRandom(PseudoRandomSeed.kSubObject));
						Random subRandom2 = random2;
						if (deepSubObjectOwnerData2.m_IsSubRandom)
						{
							random2 = ((!m_PseudoRandomSeedData.TryGetComponent(subObjectOwnerData.m_Owner, ref pseudoRandomSeed2)) ? m_RandomSeed.GetRandom(index) : pseudoRandomSeed2.GetRandom(PseudoRandomSeed.kSubObject));
						}
						bool num2 = HasSubRequirements(deepSubObjectOwnerData2.m_Prefab);
						if (num2)
						{
							updateData.m_PlaceholderRequirements.Clear();
							updateData.m_PlaceholderRequirementFlags = (ObjectRequirementFlags)0;
							updateData.m_StoredResources = Resource.NoResource;
							updateData.m_RequirementsSearched = false;
							EnsurePlaceholderRequirements(Entity.Null, deepSubObjectOwnerData2.m_Prefab, ref updateData, ref random2, isObject: true);
						}
						else
						{
							EnsurePlaceholderRequirements(subObjectOwnerData.m_Owner, prefabRef.m_Prefab, ref updateData, ref random2, flag);
						}
						if (m_SubObjects.HasBuffer(deepSubObjectOwnerData2.m_Temp.m_Original))
						{
							FillOriginalSubObjectsBuffer(subObjects: m_SubObjects[deepSubObjectOwnerData2.m_Temp.m_Original], owner: deepSubObjectOwnerData2.m_Temp.m_Original, updateData: ref updateData, useIgnoreSet: flag6);
						}
						if (flag6)
						{
							DuplicateSubObjects(index, ref random2, subObjectOwnerData.m_Owner, deepSubObjectOwnerData2.m_Entity, deepSubObjectOwnerData2.m_Temp.m_Original, deepSubObjectOwnerData2.m_Transform, ref updateData, deepSubObjectOwnerData2.m_Prefab, subObjectOwnerData.m_Temp, deepSubObjectOwnerData2.m_Temp, deepSubObjectOwnerData2.m_Elevation, hasTransform: true, native, relative, interpolated, deepSubObjectOwnerData2.m_Depth);
						}
						else
						{
							CreateSubObjects(index, ref random2, ref subRandom2, subObjectOwnerData.m_Owner, deepSubObjectOwnerData2.m_Entity, deepSubObjectOwnerData2.m_Transform, deepSubObjectOwnerData2.m_Transform, deepSubObjectOwnerData2.m_Transform, ref updateData, deepSubObjectOwnerData2.m_Prefab, isTransform: true, isEdge: false, isNode: false, subObjectOwnerData.m_Temp, deepSubObjectOwnerData2.m_Temp, deepSubObjectOwnerData2.m_Elevation, native, relative, interpolated, deepSubObjectOwnerData2.m_UnderConstruction, deepSubObjectOwnerData2.m_Destroyed, deepSubObjectOwnerData2.m_Overridden, deepSubObjectOwnerData2.m_Depth);
						}
						if (num2)
						{
							updateData.m_PlaceholderRequirements.Clear();
							updateData.m_PlaceholderRequirementFlags = (ObjectRequirementFlags)0;
							updateData.m_StoredResources = Resource.NoResource;
							updateData.m_RequirementsSearched = false;
						}
					}
					if (!deepSubObjectOwnerData2.m_New)
					{
						RemoveUnusedOldSubObjects(index, deepSubObjectOwnerData2.m_Entity, subObjects, ref updateData, subObjectOwnerData.m_Temp, flag6);
					}
				}
			}
			updateData.Dispose();
		}

		private bool HasSubRequirements(Entity ownerPrefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_ObjectRequirements.TryGetBuffer(ownerPrefab, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if ((val[i].m_Type & ObjectRequirementType.SelectOnly) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void FillOldSubObjectsBuffer(Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSubObjectsData updateData, bool isTemp, bool useIgnoreSet)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			if (subObjects.Length == 0)
			{
				return;
			}
			updateData.EnsureOldEntities((Allocator)2);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_OwnerData.HasComponent(subObject) && !m_ServiceUpgradeData.HasComponent(subObject) && !m_BuildingData.HasComponent(subObject) && !m_SecondaryData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == owner && isTemp == m_TempData.HasComponent(subObject) && (!useIgnoreSet || !m_IgnoreSet.Contains(subObject)))
				{
					if (m_EditorMode && m_EditorContainerData.HasComponent(subObject))
					{
						Game.Tools.EditorContainer editorContainer = m_EditorContainerData[subObject];
						updateData.m_OldEntities.Add(editorContainer.m_Prefab, subObject);
					}
					else
					{
						PrefabRef prefabRef = m_PrefabRefData[subObject];
						updateData.m_OldEntities.Add(prefabRef.m_Prefab, subObject);
					}
				}
			}
		}

		private void FillOriginalSubObjectsBuffer(Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSubObjectsData updateData, bool useIgnoreSet)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			if (subObjects.Length == 0)
			{
				return;
			}
			updateData.EnsureOriginalEntities((Allocator)2);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_OwnerData.HasComponent(subObject) && !m_ServiceUpgradeData.HasComponent(subObject) && !m_BuildingData.HasComponent(subObject) && !m_SecondaryData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == owner && !m_TempData.HasComponent(subObject) && (!useIgnoreSet || !m_IgnoreSet.Contains(subObject)))
				{
					if (m_EditorMode && m_EditorContainerData.HasComponent(subObject))
					{
						Game.Tools.EditorContainer editorContainer = m_EditorContainerData[subObject];
						updateData.m_OriginalEntities.Add(editorContainer.m_Prefab, subObject);
					}
					else
					{
						PrefabRef prefabRef = m_PrefabRefData[subObject];
						updateData.m_OriginalEntities.Add(prefabRef.m_Prefab, subObject);
					}
				}
			}
		}

		private void FillClearAreas(Entity owner, DynamicBuffer<InstalledUpgrade> installedUpgrades, ref UpdateSubObjectsData updateData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			ClearAreaHelpers.FillClearAreas(installedUpgrades, Entity.Null, m_TransformData, m_AreaClearData, m_PrefabRefData, m_PrefabObjectGeometryData, m_SubAreas, m_AreaNodes, m_AreaTriangles, ref updateData.m_ClearAreas);
			ClearAreaHelpers.InitClearAreas(updateData.m_ClearAreas, m_TransformData[owner]);
		}

		private void RemoveUnusedOldSubObjects(int jobIndex, Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSubObjectsData updateData, bool isTemp, bool useIgnoreSet)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
			Entity val5 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val6 = default(NativeParallelMultiHashMapIterator<Entity>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (!m_OwnerData.HasComponent(subObject) || m_ServiceUpgradeData.HasComponent(subObject) || m_BuildingData.HasComponent(subObject) || m_SecondaryData.HasComponent(subObject) || !(m_OwnerData[subObject].m_Owner == owner) || (useIgnoreSet && m_IgnoreSet.Contains(subObject)))
				{
					continue;
				}
				if (isTemp == m_TempData.HasComponent(subObject))
				{
					Entity val = ((!m_EditorMode || !m_EditorContainerData.HasComponent(subObject)) ? m_PrefabRefData[subObject].m_Prefab : m_EditorContainerData[subObject].m_Prefab);
					if (updateData.m_OldEntities.TryGetFirstValue(val, ref val2, ref val3))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val2, ref m_AppliedTypes);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, val2, default(Deleted));
						updateData.m_OldEntities.Remove(val3);
						if (m_SubObjects.HasBuffer(val2))
						{
							updateData.EnsureDeepOwners((Allocator)2);
							ref NativeList<DeepSubObjectOwnerData> reference = ref updateData.m_DeepOwners;
							DeepSubObjectOwnerData deepSubObjectOwnerData = new DeepSubObjectOwnerData
							{
								m_Entity = val2,
								m_Deleted = true
							};
							reference.Add(ref deepSubObjectOwnerData);
						}
					}
				}
				else if (isTemp && updateData.m_OriginalEntities.IsCreated)
				{
					Entity val4 = ((!m_EditorMode || !m_EditorContainerData.HasComponent(subObject)) ? m_PrefabRefData[subObject].m_Prefab : m_EditorContainerData[subObject].m_Prefab);
					if (updateData.m_OriginalEntities.TryGetFirstValue(val4, ref val5, ref val6))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(jobIndex, val5, default(Hidden));
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, val5, default(BatchesUpdated));
						updateData.m_OriginalEntities.Remove(val6);
					}
				}
			}
		}

		private void DuplicateSubObjects(int jobIndex, ref Random random, Entity topOwner, Entity owner, Entity original, Transform ownerTransform, ref UpdateSubObjectsData updateData, Entity prefab, bool isTemp, Temp ownerTemp, float ownerElevation, bool hasTransform, bool native, bool relative, bool interpolated, int depth)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = default(Transform);
			if (hasTransform && m_TransformData.TryGetComponent(original, ref transform))
			{
				transform = ObjectUtils.InverseTransform(transform);
			}
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (!m_SubObjects.TryGetBuffer(original, ref val))
			{
				return;
			}
			Relative relative2 = default(Relative);
			Elevation elevation = default(Elevation);
			Elevation elevation2 = default(Elevation);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (!m_ServiceUpgradeData.HasComponent(subObject) && !m_BuildingData.HasComponent(subObject) && !m_SecondaryData.HasComponent(subObject) && !m_IgnoreSet.Contains(subObject))
				{
					Entity prefab2 = m_PrefabRefData[subObject].m_Prefab;
					Transform transform2 = m_TransformData[subObject];
					Transform transform3 = transform2;
					int num = 0;
					int groupIndex = 0;
					int probability = 100;
					int prefabSubIndex = -1;
					if (m_LocalTransformCacheData.HasComponent(subObject))
					{
						LocalTransformCache localTransformCache = m_LocalTransformCacheData[subObject];
						transform3.m_Position = localTransformCache.m_Position;
						transform3.m_Rotation = localTransformCache.m_Rotation;
						num = localTransformCache.m_ParentMesh;
						groupIndex = localTransformCache.m_GroupIndex;
						probability = localTransformCache.m_Probability;
						prefabSubIndex = localTransformCache.m_PrefabSubIndex;
						transform2 = ObjectUtils.LocalToWorld(ownerTransform, transform3);
					}
					else if (m_RelativeData.TryGetComponent(subObject, ref relative2))
					{
						transform3 = relative2.ToTransform();
						num = ((!m_ElevationData.TryGetComponent(subObject, ref elevation)) ? (-1) : ObjectUtils.GetSubParentMesh(elevation.m_Flags));
					}
					else if (hasTransform)
					{
						transform3 = ObjectUtils.WorldToLocal(transform, transform2);
						num = ((!m_ElevationData.TryGetComponent(subObject, ref elevation2)) ? (-1) : ObjectUtils.GetSubParentMesh(elevation2.m_Flags));
					}
					SubObjectFlags subObjectFlags = (SubObjectFlags)0;
					if (num == -1)
					{
						subObjectFlags |= SubObjectFlags.OnGround;
					}
					if (m_EditorMode && m_EditorContainerData.HasComponent(subObject))
					{
						Game.Tools.EditorContainer editorContainer = m_EditorContainerData[subObject];
						CreateContainerObject(jobIndex, owner, isTemp, ownerTemp, ownerElevation, Entity.Null, transform2, transform3, ref updateData, editorContainer.m_Prefab, editorContainer.m_Scale, editorContainer.m_Intensity, num, editorContainer.m_GroupIndex, prefabSubIndex);
					}
					else
					{
						CreateSubObject(jobIndex, ref random, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transform2, transform3, subObjectFlags, ref updateData, prefab2, m_EditorMode, native, relative, interpolated, underConstruction: false, isDestroyed: false, isOverridden: false, updated: false, -1, num, groupIndex, probability, prefabSubIndex, depth);
					}
				}
			}
		}

		private void RelocateSubObjects(int jobIndex, ref Random random, Entity topOwner, Entity owner, Entity original, Area area, Geometry geometry, DynamicBuffer<Game.Areas.Node> nodes, DynamicBuffer<Triangle> triangles, ref UpdateSubObjectsData updateData, Entity prefab, bool isTemp, Temp ownerTemp, float ownerElevation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (m_SubObjects.TryGetBuffer(original, ref val))
			{
				ownerTemp.m_Flags &= ~TempFlags.Modify;
				NativeArray<SubObjectData> val2 = default(NativeArray<SubObjectData>);
				val2._002Ector(val.Length, (Allocator)2, (NativeArrayOptions)1);
				AreaGeometryData areaData = m_PrefabAreaGeometryData[prefab];
				int num = 0;
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					if (!m_SecondaryData.HasComponent(subObject))
					{
						Entity prefab2 = m_PrefabRefData[subObject].m_Prefab;
						ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
						if (m_PrefabObjectGeometryData.HasComponent(prefab2))
						{
							objectGeometryData = m_PrefabObjectGeometryData[prefab2];
						}
						float num2 = (((objectGeometryData.m_Flags & GeometryFlags.Circular) == 0) ? (math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz)) * 0.5f) : (objectGeometryData.m_Size.x * 0.5f));
						if (m_BuildingData.HasComponent(subObject))
						{
							Transform transform = m_TransformData[subObject];
							float minNodeDistance = AreaUtils.GetMinNodeDistance(areaData);
							updateData.EnsureObjectBuffer((Allocator)2);
							ref NativeList<AreaUtils.ObjectItem> reference = ref updateData.m_ObjectBuffer;
							AreaUtils.ObjectItem objectItem = new AreaUtils.ObjectItem(num2 + minNodeDistance, ((float3)(ref transform.m_Position)).xz, Entity.Null);
							reference.Add(ref objectItem);
						}
						else
						{
							val2[num++] = new SubObjectData
							{
								m_SubObject = subObject,
								m_Radius = num2
							};
						}
					}
				}
				NativeSortExtension.Sort<SubObjectData>(val2);
				for (int j = 0; j < num; j++)
				{
					Entity val3 = val2[j].m_SubObject;
					Entity prefab3 = m_PrefabRefData[val3].m_Prefab;
					Transform transform2 = m_TransformData[val3];
					ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
					if (m_PrefabObjectGeometryData.HasComponent(prefab3))
					{
						objectGeometryData2 = m_PrefabObjectGeometryData[prefab3];
					}
					float num3;
					float3 val4;
					if ((objectGeometryData2.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
					{
						num3 = objectGeometryData2.m_Size.x * 0.5f;
						val4 = default(float3);
					}
					else
					{
						num3 = math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData2.m_Bounds)).xz)) * 0.5f;
						val4 = math.rotate(transform2.m_Rotation, MathUtils.Center(objectGeometryData2.m_Bounds));
						val4.y = 0f;
					}
					float num4 = 0f;
					float3 position = transform2.m_Position + val4;
					if (AreaUtils.IntersectArea(position, num3, nodes, triangles) && !AreaUtils.IntersectEdges(position, num3, num4, nodes) && !AreaUtils.IntersectObjects(position, num3, num4, updateData.m_ObjectBuffer))
					{
						Entity val5 = FindOldSubObject(prefab3, val3, ref updateData);
						SubObjectFlags subObjectFlags = (SubObjectFlags)0;
						if (!m_ElevationData.HasComponent(val5))
						{
							subObjectFlags |= SubObjectFlags.OnGround;
						}
						if (val5 == Entity.Null)
						{
							val5.Index = -1;
						}
						updateData.EnsureObjectBuffer((Allocator)2);
						ref NativeList<AreaUtils.ObjectItem> reference2 = ref updateData.m_ObjectBuffer;
						AreaUtils.ObjectItem objectItem = new AreaUtils.ObjectItem(num3 + num4, ((float3)(ref position)).xz, Entity.Null);
						reference2.Add(ref objectItem);
						CreateSubObject(jobIndex, ref random, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, val5, transform2, transform2, default(Transform), subObjectFlags, ref updateData, prefab3, cacheTransform: false, native: false, relative: false, interpolated: false, underConstruction: false, isDestroyed: false, isOverridden: false, updated: false, -1, -1, 0, 100, -1, 0);
					}
				}
				val2.Dispose();
			}
			else
			{
				if (!m_SubObjects.HasBuffer(owner))
				{
					return;
				}
				DynamicBuffer<SubObject> val6 = m_SubObjects[owner];
				NativeArray<SubObjectData> val7 = default(NativeArray<SubObjectData>);
				val7._002Ector(val6.Length, (Allocator)2, (NativeArrayOptions)1);
				AreaGeometryData areaData2 = m_PrefabAreaGeometryData[prefab];
				int num5 = 0;
				for (int k = 0; k < val6.Length; k++)
				{
					Entity subObject2 = val6[k].m_SubObject;
					if (!m_SecondaryData.HasComponent(subObject2))
					{
						Entity prefab4 = m_PrefabRefData[subObject2].m_Prefab;
						ObjectGeometryData objectGeometryData3 = default(ObjectGeometryData);
						if (m_PrefabObjectGeometryData.HasComponent(prefab4))
						{
							objectGeometryData3 = m_PrefabObjectGeometryData[prefab4];
						}
						float num6 = (((objectGeometryData3.m_Flags & GeometryFlags.Circular) == 0) ? (math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData3.m_Bounds)).xz)) * 0.5f) : (objectGeometryData3.m_Size.x * 0.5f));
						if (m_BuildingData.HasComponent(subObject2))
						{
							Transform transform3 = m_TransformData[subObject2];
							float minNodeDistance2 = AreaUtils.GetMinNodeDistance(areaData2);
							updateData.EnsureObjectBuffer((Allocator)2);
							ref NativeList<AreaUtils.ObjectItem> reference3 = ref updateData.m_ObjectBuffer;
							AreaUtils.ObjectItem objectItem = new AreaUtils.ObjectItem(num6 + minNodeDistance2, ((float3)(ref transform3.m_Position)).xz, Entity.Null);
							reference3.Add(ref objectItem);
						}
						else
						{
							val7[num5++] = new SubObjectData
							{
								m_SubObject = subObject2,
								m_Radius = num6
							};
						}
					}
				}
				NativeSortExtension.Sort<SubObjectData>(val7);
				for (int l = 0; l < num5; l++)
				{
					Entity val8 = val7[l].m_SubObject;
					Entity prefab5 = m_PrefabRefData[val8].m_Prefab;
					Transform transform4 = m_TransformData[val8];
					ObjectGeometryData objectGeometryData4 = default(ObjectGeometryData);
					if (m_PrefabObjectGeometryData.HasComponent(prefab5))
					{
						objectGeometryData4 = m_PrefabObjectGeometryData[prefab5];
					}
					float num7;
					float3 val9;
					if ((objectGeometryData4.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
					{
						num7 = objectGeometryData4.m_Size.x * 0.5f;
						val9 = default(float3);
					}
					else
					{
						num7 = math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData4.m_Bounds)).xz)) * 0.5f;
						val9 = math.rotate(transform4.m_Rotation, MathUtils.Center(objectGeometryData4.m_Bounds));
						val9.y = 0f;
					}
					float num8 = 0f;
					float3 position2 = transform4.m_Position + val9;
					if (!AreaUtils.IntersectArea(position2, num7, nodes, triangles) || AreaUtils.IntersectEdges(position2, num7, num8, nodes) || AreaUtils.IntersectObjects(position2, num7, num8, updateData.m_ObjectBuffer))
					{
						position2 = AreaUtils.GetRandomPosition(ref random, geometry, nodes, triangles);
						if (!AreaUtils.TryFitInside(ref position2, num7, num8, area, nodes, updateData.m_ObjectBuffer))
						{
							continue;
						}
						transform4.m_Rotation = AreaUtils.GetRandomRotation(ref random, position2, nodes);
						if ((objectGeometryData4.m_Flags & GeometryFlags.Circular) == 0)
						{
							val9 = math.rotate(transform4.m_Rotation, MathUtils.Center(objectGeometryData4.m_Bounds));
							val9.y = 0f;
						}
						transform4.m_Position = position2 - val9;
					}
					SubObjectFlags subObjectFlags2 = (SubObjectFlags)0;
					if (!m_ElevationData.HasComponent(val8))
					{
						subObjectFlags2 |= SubObjectFlags.OnGround;
					}
					updateData.EnsureObjectBuffer((Allocator)2);
					ref NativeList<AreaUtils.ObjectItem> reference4 = ref updateData.m_ObjectBuffer;
					AreaUtils.ObjectItem objectItem = new AreaUtils.ObjectItem(num7 + num8, ((float3)(ref position2)).xz, Entity.Null);
					reference4.Add(ref objectItem);
					CreateSubObject(jobIndex, ref random, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, Entity.Null, transform4, transform4, default(Transform), subObjectFlags2, ref updateData, prefab5, cacheTransform: false, native: false, relative: false, interpolated: false, underConstruction: false, isDestroyed: false, isOverridden: false, updated: false, -1, -1, 0, 100, -1, 0);
				}
				val7.Dispose();
			}
		}

		private void CreateSubObjects(int jobIndex, ref Random random, ref Random subRandom, Entity topOwner, Entity owner, Transform transform1, Transform transform2, Transform transform3, ref UpdateSubObjectsData updateData, Entity prefab, bool isTransform, bool isEdge, bool isNode, bool isTemp, Temp ownerTemp, float ownerElevation, bool native, bool relative, bool interpolated, bool isUnderConstruction, bool isDestroyed, bool isOverridden, int depth)
		{
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			if (m_EditorMode && isTransform)
			{
				DynamicBuffer<Game.Prefabs.SubObject> val = default(DynamicBuffer<Game.Prefabs.SubObject>);
				if (m_PrefabSubObjects.TryGetBuffer(prefab, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						Game.Prefabs.SubObject subObject = val[i];
						if ((subObject.m_Flags & SubObjectFlags.CoursePlacement) == 0 && isEdge == ((subObject.m_Flags & SubObjectFlags.EdgePlacement) != 0))
						{
							Transform transform4 = new Transform(subObject.m_Position, subObject.m_Rotation);
							Transform transformData = ObjectUtils.LocalToWorld(transform2, transform4);
							int alignIndex = math.select(-1, i, isEdge);
							CreateSubObject(jobIndex, ref random, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, Entity.Null, transform2, transformData, transform4, subObject.m_Flags, ref updateData, subObject.m_Prefab, cacheTransform: true, native, relative, interpolated, underConstruction: false, isDestroyed: false, isOverridden, updated: false, alignIndex, subObject.m_ParentIndex, subObject.m_GroupIndex, subObject.m_Probability, i, depth);
						}
					}
				}
				if (m_PrefabEffects.HasBuffer(prefab))
				{
					DynamicBuffer<Effect> val2 = m_PrefabEffects[prefab];
					for (int j = 0; j < val2.Length; j++)
					{
						Effect effect = val2[j];
						if (!effect.m_Procedural)
						{
							Transform transform5 = new Transform(effect.m_Position, effect.m_Rotation);
							Transform transformData2 = ObjectUtils.LocalToWorld(transform2, transform5);
							CreateContainerObject(jobIndex, owner, isTemp, ownerTemp, ownerElevation, Entity.Null, transformData2, transform5, ref updateData, effect.m_Effect, effect.m_Scale, effect.m_Intensity, effect.m_ParentMesh, effect.m_AnimationIndex, j);
						}
					}
				}
				if (m_PrefabActivityLocations.HasBuffer(prefab))
				{
					DynamicBuffer<ActivityLocationElement> val3 = m_PrefabActivityLocations[prefab];
					for (int k = 0; k < val3.Length; k++)
					{
						ActivityLocationElement activityLocationElement = val3[k];
						Transform transform6 = new Transform(activityLocationElement.m_Position, activityLocationElement.m_Rotation);
						Transform transformData3 = ObjectUtils.LocalToWorld(transform2, transform6);
						CreateContainerObject(jobIndex, owner, isTemp, ownerTemp, ownerElevation, Entity.Null, transformData3, transform6, ref updateData, activityLocationElement.m_Prefab, float3.op_Implicit(1f), 1f, 0, -1, k);
					}
				}
				return;
			}
			Random random2 = random;
			Random subRandom2 = subRandom;
			DynamicBuffer<Game.Prefabs.SubObject> val4 = default(DynamicBuffer<Game.Prefabs.SubObject>);
			if (m_PrefabSubObjects.TryGetBuffer(prefab, ref val4))
			{
				NetGeometryData netGeometryData = default(NetGeometryData);
				PlaceableNetData placeableNetData = default(PlaceableNetData);
				Game.Net.Elevation elevation = default(Game.Net.Elevation);
				Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
				Game.Net.Elevation elevation3 = default(Game.Net.Elevation);
				Game.Net.Elevation elevation4 = default(Game.Net.Elevation);
				Bounds1 val6 = default(Bounds1);
				for (int l = 0; l < val4.Length; l++)
				{
					Game.Prefabs.SubObject subObject2 = val4[l];
					if ((subObject2.m_Flags & SubObjectFlags.CoursePlacement) != 0)
					{
						continue;
					}
					if ((subObject2.m_Flags & SubObjectFlags.EdgePlacement) != 0)
					{
						if (!isEdge)
						{
							if ((subObject2.m_Flags & SubObjectFlags.AllowCombine) == 0 || !IsContinuous(topOwner, Entity.Null))
							{
								continue;
							}
							subObject2.m_Position.z = 0f;
						}
					}
					else if (isEdge)
					{
						continue;
					}
					bool flag = true;
					bool flag2 = true;
					if ((subObject2.m_Flags & SubObjectFlags.RequireElevated) != 0)
					{
						if (!m_PrefabNetGeometryData.TryGetComponent(prefab, ref netGeometryData))
						{
							continue;
						}
						float2 val5 = netGeometryData.m_ElevationLimit * new float2(1f, 2f);
						if (m_PrefabPlaceableNetData.TryGetComponent(prefab, ref placeableNetData))
						{
							val5 += math.max(0f, placeableNetData.m_ElevationRange.min);
						}
						if (isEdge && (subObject2.m_Flags & (SubObjectFlags.EdgePlacement | SubObjectFlags.MiddlePlacement)) == SubObjectFlags.EdgePlacement)
						{
							Edge edge = m_NetEdgeData[topOwner];
							m_NetElevationData.TryGetComponent(topOwner, ref elevation);
							m_NetElevationData.TryGetComponent(edge.m_Start, ref elevation2);
							m_NetElevationData.TryGetComponent(edge.m_End, ref elevation3);
							bool flag3 = math.all(elevation.m_Elevation >= val5.y) || (netGeometryData.m_Flags & Game.Net.GeometryFlags.RequireElevated) != 0;
							flag = math.all(elevation2.m_Elevation >= math.select(val5.y, val5.x, flag3));
							flag2 = math.all(elevation3.m_Elevation >= math.select(val5.y, val5.x, flag3));
							if (!flag && !flag2)
							{
								continue;
							}
						}
						else
						{
							m_NetElevationData.TryGetComponent(topOwner, ref elevation4);
							if (!math.all(elevation4.m_Elevation >= val5.y) && (!isEdge || (netGeometryData.m_Flags & Game.Net.GeometryFlags.RequireElevated) == 0))
							{
								continue;
							}
						}
					}
					if ((subObject2.m_Flags & SubObjectFlags.RequireOutsideConnection) != 0 && !m_OutsideConnectionData.HasComponent(topOwner))
					{
						continue;
					}
					if ((subObject2.m_Flags & SubObjectFlags.RequireDeadEnd) != 0)
					{
						if (!IsDeadEnd(topOwner, out var isEnd))
						{
							continue;
						}
						if (isEnd)
						{
							subObject2.m_Rotation = math.mul(quaternion.RotateY((float)Math.PI), subObject2.m_Rotation);
						}
					}
					if ((subObject2.m_Flags & SubObjectFlags.RequireOrphan) != 0 && !IsOrphan(topOwner))
					{
						continue;
					}
					if ((subObject2.m_Flags & (SubObjectFlags.WaterwayCrossing | SubObjectFlags.NotWaterwayCrossing)) != 0)
					{
						if (isEdge && (subObject2.m_Flags & SubObjectFlags.MiddlePlacement) == 0)
						{
							Edge edge2 = m_NetEdgeData[topOwner];
							flag &= IsWaterwayCrossing(topOwner, edge2.m_Start) == ((subObject2.m_Flags & SubObjectFlags.WaterwayCrossing) != 0);
							flag2 &= IsWaterwayCrossing(topOwner, edge2.m_End) == ((subObject2.m_Flags & SubObjectFlags.WaterwayCrossing) != 0);
							if (!flag && !flag2)
							{
								continue;
							}
						}
						else if (isEdge)
						{
							if (IsWaterwayCrossing(topOwner) != ((subObject2.m_Flags & SubObjectFlags.WaterwayCrossing) != 0))
							{
								continue;
							}
						}
						else if (isNode && IsWaterwayCrossing(Entity.Null, topOwner) != ((subObject2.m_Flags & SubObjectFlags.WaterwayCrossing) != 0))
						{
							continue;
						}
					}
					Transform transform7 = new Transform(subObject2.m_Position, subObject2.m_Rotation);
					int parentMesh = 0;
					if (isTransform)
					{
						parentMesh = subObject2.m_ParentIndex;
					}
					else if ((subObject2.m_Flags & SubObjectFlags.FixedPlacement) != 0)
					{
						int2 fixedRange = GetFixedRange(owner);
						if ((subObject2.m_Flags & SubObjectFlags.StartPlacement) != 0)
						{
							if (fixedRange.x != subObject2.m_ParentIndex || (!isEdge && fixedRange.x == fixedRange.y))
							{
								continue;
							}
							flag2 = false;
						}
						else if ((subObject2.m_Flags & SubObjectFlags.EndPlacement) != 0)
						{
							if (fixedRange.y != subObject2.m_ParentIndex || (!isEdge && fixedRange.x == fixedRange.y))
							{
								continue;
							}
							flag = false;
						}
						else if (fixedRange.x != subObject2.m_ParentIndex || (!isEdge && fixedRange.x != fixedRange.y))
						{
							continue;
						}
					}
					if (isNode && IsAbruptEnd(topOwner))
					{
						continue;
					}
					if (isEdge && (subObject2.m_Flags & SubObjectFlags.MiddlePlacement) == 0)
					{
						Edge edge3 = m_NetEdgeData[topOwner];
						if (flag)
						{
							Transform transformData4 = ObjectUtils.LocalToWorld(transform1, transform7);
							if ((subObject2.m_Flags & SubObjectFlags.AllowCombine) == 0 || !IsContinuous(edge3.m_Start, topOwner))
							{
								CreateSubObject(jobIndex, ref random, ref subRandom, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform1, transformData4, transform7, subObject2.m_Flags, ref updateData, subObject2.m_Prefab, native, relative, interpolated, isUnderConstruction, isDestroyed, isOverridden, l, parentMesh, subObject2.m_GroupIndex, subObject2.m_Probability, l, depth);
							}
						}
						if (flag2)
						{
							Transform transformData5 = ObjectUtils.LocalToWorld(transform3, transform7);
							if ((subObject2.m_Flags & SubObjectFlags.AllowCombine) == 0 || !IsContinuous(edge3.m_End, topOwner))
							{
								CreateSubObject(jobIndex, ref random, ref subRandom, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform3, transformData5, transform7, subObject2.m_Flags, ref updateData, subObject2.m_Prefab, native, relative, interpolated, isUnderConstruction, isDestroyed, isOverridden, l, parentMesh, subObject2.m_GroupIndex, subObject2.m_Probability, l, depth);
							}
						}
					}
					else if (isEdge && (subObject2.m_Flags & SubObjectFlags.EvenSpacing) != 0)
					{
						Curve curve = m_NetCurveData[topOwner];
						float num = MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz);
						int num2 = (int)(num / math.max(1f, subObject2.m_Position.z) - 0.5f);
						transform7.m_Position.z = 0f;
						for (int m = 0; m < num2; m++)
						{
							((Bounds1)(ref val6))._002Ector(0f, 1f);
							MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val6, (float)(m + 1) * num / (float)(num2 + 1));
							Transform transform8 = new Transform(MathUtils.Position(curve.m_Bezier, val6.max), NetUtils.GetNodeRotation(MathUtils.Tangent(curve.m_Bezier, val6.max)));
							Transform transformData6 = ObjectUtils.LocalToWorld(transform8, transform7);
							int alignIndex2 = m * val4.Length + l;
							CreateSubObject(jobIndex, ref random, ref subRandom, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform8, transformData6, transform7, subObject2.m_Flags, ref updateData, subObject2.m_Prefab, native, relative, interpolated, isUnderConstruction, isDestroyed, isOverridden, alignIndex2, parentMesh, subObject2.m_GroupIndex, subObject2.m_Probability, l, depth);
						}
					}
					else
					{
						Transform transformData7 = ObjectUtils.LocalToWorld(transform2, transform7);
						int alignIndex3 = math.select(-1, l, isEdge || isNode);
						CreateSubObject(jobIndex, ref random, ref subRandom, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform2, transformData7, transform7, subObject2.m_Flags, ref updateData, subObject2.m_Prefab, native, relative, interpolated, isUnderConstruction, isDestroyed, isOverridden, alignIndex3, parentMesh, subObject2.m_GroupIndex, subObject2.m_Probability, l, depth);
					}
				}
			}
			if (isUnderConstruction && m_PrefabBuildingData.HasComponent(prefab))
			{
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefab];
				Transform transform9 = new Transform
				{
					m_Rotation = quaternion.identity
				};
				DynamicBuffer<SubMesh> val7 = default(DynamicBuffer<SubMesh>);
				if (m_PrefabSubMeshes.TryGetBuffer(prefab, ref val7) && val7.Length != 0)
				{
					((Random)(ref random2)).NextInt();
					SubMesh subMesh = val7[((Random)(ref subRandom2)).NextInt(val7.Length)];
					transform9.m_Position = subMesh.m_Position;
					transform9.m_Rotation = subMesh.m_Rotation;
				}
				transform9.m_Position.y += math.max(objectGeometryData.m_Bounds.max.y, 15f);
				transform9.m_Position.y += math.csum(math.frac(((float3)(ref transform2.m_Position)).xz / 60f)) * 5f;
				Transform transformData8 = ObjectUtils.LocalToWorld(transform2, transform9);
				CreateSubObject(jobIndex, ref random2, ref subRandom2, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform2, transformData8, transform9, (SubObjectFlags)0, ref updateData, m_BuildingConfigurationData.m_ConstructionObject, native, relative, interpolated, underConstruction: false, isDestroyed, isOverridden, -1, 0, 0, 100, -1, depth);
			}
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			DynamicBuffer<SubMesh> val8 = default(DynamicBuffer<SubMesh>);
			if (!isDestroyed || !m_PrefabObjectGeometryData.TryGetComponent(prefab, ref objectGeometryData2) || (objectGeometryData2.m_Flags & (GeometryFlags.Physical | GeometryFlags.HasLot)) != (GeometryFlags.Physical | GeometryFlags.HasLot) || !m_PrefabSubMeshes.TryGetBuffer(prefab, ref val8))
			{
				return;
			}
			int num3 = 0;
			MeshData meshData = default(MeshData);
			for (int n = 0; n < val8.Length; n++)
			{
				SubMesh subMesh2 = val8[n];
				if (!m_PrefabMeshData.TryGetComponent(subMesh2.m_SubMesh, ref meshData))
				{
					continue;
				}
				float2 val9 = MathUtils.Center(((Bounds3)(ref meshData.m_Bounds)).xz);
				float2 val10 = MathUtils.Size(((Bounds3)(ref meshData.m_Bounds)).xz);
				int2 val11 = math.max(int2.op_Implicit(1), (int2)math.sqrt(val10));
				float2 val12 = val10 / float2.op_Implicit(val11);
				float3 val13 = math.rotate(subMesh2.m_Rotation, new float3(val12.x, 0f, 0f));
				float3 val14 = math.rotate(subMesh2.m_Rotation, new float3(0f, 0f, val12.y));
				float3 val15 = subMesh2.m_Position + math.rotate(subMesh2.m_Rotation, new float3(val9.x, 0f, val9.y));
				val15 -= val13 * ((float)val11.x * 0.5f - 0.5f) + val14 * ((float)val11.y * 0.5f - 0.5f);
				for (int num4 = 0; num4 < val11.y; num4++)
				{
					for (int num5 = 0; num5 < val11.x; num5++)
					{
						float2 val16 = new float2((float)num5, (float)num4) + ((Random)(ref random)).NextFloat2(float2.op_Implicit(-0.5f), float2.op_Implicit(0.5f));
						Transform transform10 = new Transform
						{
							m_Position = val15 + val13 * val16.x + val14 * val16.y,
							m_Rotation = quaternion.RotateY(((Random)(ref subRandom2)).NextFloat(-(float)Math.PI, (float)Math.PI))
						};
						((Random)(ref random2)).NextFloat();
						Transform transformData9 = ObjectUtils.LocalToWorld(transform2, transform10);
						CreateSubObject(jobIndex, ref random2, ref subRandom2, topOwner, owner, prefab, isTemp, ownerTemp, ownerElevation, transform2, transformData9, transform10, SubObjectFlags.OnGround, ref updateData, m_BuildingConfigurationData.m_CollapsedObject, native, relative, interpolated, underConstruction: false, destroyed: false, isOverridden, -1, 0, num3++, 100, -1, depth);
					}
				}
			}
		}

		private int2 GetFixedRange(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (m_Edges.HasBuffer(owner))
			{
				PrefabRef prefabRef = m_PrefabRefData[owner];
				int2 val = default(int2);
				((int2)(ref val))._002Ector(int.MaxValue, int.MinValue);
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					if (prefabRef.m_Prefab == prefabRef2.m_Prefab && m_FixedData.HasComponent(value.m_Edge))
					{
						Fixed obj = m_FixedData[value.m_Edge];
						if (value.m_End)
						{
							val.y = math.max(val.y, obj.m_Index);
						}
						else
						{
							val.x = math.min(val.x, obj.m_Index);
						}
					}
				}
				return val;
			}
			if (m_FixedData.HasComponent(owner))
			{
				Fixed obj2 = m_FixedData[owner];
				return new int2(obj2.m_Index, obj2.m_Index);
			}
			return new int2(int.MaxValue, int.MinValue);
		}

		private bool IsDeadEnd(Entity owner, out bool isEnd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[owner];
			isEnd = false;
			if (m_Edges.HasBuffer(owner) && m_PrefabNetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
				int num = 0;
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					if ((m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_MergeLayers & netGeometryData.m_MergeLayers) != Layer.None)
					{
						isEnd = value.m_End;
						num++;
					}
				}
				return num <= 1;
			}
			return false;
		}

		private bool IsAbruptEnd(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (m_Edges.HasBuffer(owner))
			{
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				Upgraded upgraded = default(Upgraded);
				while (edgeIterator.GetNext(out value))
				{
					if (m_UpgradedData.TryGetComponent(value.m_Edge, ref upgraded) && ((value.m_End ? upgraded.m_Flags.m_Right : upgraded.m_Flags.m_Left) & CompositionFlags.Side.AbruptEnd) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsOrphan(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[owner];
			if (m_Edges.HasBuffer(owner) && m_PrefabNetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					if ((m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_MergeLayers & netGeometryData.m_MergeLayers) != Layer.None)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool IsWaterwayCrossing(Entity edge, Entity node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			if (m_Edges.HasBuffer(node) && m_PrefabRefData.TryGetComponent(node, ref prefabRef) && m_PrefabNetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				int num = 0;
				int num2 = 0;
				EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					if ((m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_MergeLayers & Layer.Waterway) != Layer.None)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				if (num >= 1)
				{
					return num2 >= 2;
				}
				return false;
			}
			return false;
		}

		private bool IsWaterwayCrossing(Entity edge)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			Edge edge2 = default(Edge);
			if (m_NetEdgeData.TryGetComponent(edge, ref edge2))
			{
				if (!IsWaterwayCrossing(edge, edge2.m_Start))
				{
					return IsWaterwayCrossing(edge, edge2.m_End);
				}
				return true;
			}
			return false;
		}

		private bool IsContinuous(Entity node, Entity edge)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[node];
			if (m_Edges.HasBuffer(node) && m_PrefabNetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
				int num = 0;
				Curve curve = default(Curve);
				EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[value.m_Edge];
					NetGeometryData netGeometryData2 = m_PrefabNetGeometryData[prefabRef2.m_Prefab];
					if ((netGeometryData2.m_MergeLayers & netGeometryData.m_MergeLayers) != Layer.None)
					{
						if (prefabRef2.m_Prefab != prefabRef.m_Prefab)
						{
							return false;
						}
						if (++num == 1)
						{
							curve = m_NetCurveData[value.m_Edge];
							if (value.m_End)
							{
								curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
							}
							continue;
						}
						Curve curve2 = m_NetCurveData[value.m_Edge];
						if (value.m_End)
						{
							curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
						}
						float3 val = MathUtils.StartTangent(curve.m_Bezier);
						float3 val2 = MathUtils.StartTangent(curve2.m_Bezier);
						if (MathUtils.TryNormalize(ref val) && MathUtils.TryNormalize(ref val2))
						{
							if (math.dot(val, val2) > -0.99f)
							{
								return false;
							}
							float3 val3 = (val - val2) * 0.5f;
							float3 val4 = curve2.m_Bezier.a - curve.m_Bezier.a;
							val4 -= val3 * math.dot(val4, val3);
							if (math.lengthsq(val4) > 0.01f)
							{
								return false;
							}
						}
					}
					else if ((netGeometryData2.m_IntersectLayers & netGeometryData.m_IntersectLayers) != Layer.None)
					{
						return false;
					}
				}
				return num == 2;
			}
			return false;
		}

		private bool CheckRequirements(Entity prefab, int groupIndex, bool isExplicit, ref UpdateSubObjectsData updateData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_ObjectRequirements.TryGetBuffer(prefab, ref val))
			{
				int num = -1;
				bool flag = true;
				int2 val2 = default(int2);
				for (int i = 0; i < val.Length; i++)
				{
					ObjectRequirementElement objectRequirementElement = val[i];
					if ((objectRequirementElement.m_Type & ObjectRequirementType.SelectOnly) != 0)
					{
						continue;
					}
					if (objectRequirementElement.m_Group != num)
					{
						if (!flag)
						{
							break;
						}
						num = objectRequirementElement.m_Group;
						flag = false;
					}
					if (objectRequirementElement.m_Requirement != Entity.Null)
					{
						if (updateData.m_PlaceholderRequirements.TryGetValue(objectRequirementElement.m_Requirement, ref val2))
						{
							if (val2.y == 0)
							{
								flag = true;
								continue;
							}
							int num2 = groupIndex % val2.y;
							num2 = math.select(num2, -val2.y, num2 == 0 && groupIndex < 0);
							flag |= num2 == val2.x;
						}
						else if (isExplicit && (objectRequirementElement.m_Type & ObjectRequirementType.IgnoreExplicit) != 0)
						{
							flag = true;
						}
					}
					else
					{
						flag |= (updateData.m_PlaceholderRequirementFlags & (objectRequirementElement.m_RequireFlags | objectRequirementElement.m_ForbidFlags)) == objectRequirementElement.m_RequireFlags;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private void CreateSubObject(int jobIndex, ref Random random, ref Random subRandom, Entity topOwner, Entity owner, Entity ownerPrefab, bool isTemp, Temp ownerTemp, float ownerElevation, Transform ownerTransform, Transform transformData, Transform localTransformData, SubObjectFlags flags, ref UpdateSubObjectsData updateData, Entity prefab, bool native, bool relative, bool interpolated, bool underConstruction, bool destroyed, bool overridden, int alignIndex, int parentMesh, int groupIndex, int probability, int prefabSubIndex, int depth)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0983: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_098b: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			PlaceholderObjectData placeholderObjectData = default(PlaceholderObjectData);
			DynamicBuffer<PlaceholderObjectElement> val = default(DynamicBuffer<PlaceholderObjectElement>);
			if (!m_PrefabPlaceholderObjectData.TryGetComponent(prefab, ref placeholderObjectData) || !m_PlaceholderObjects.TryGetBuffer(prefab, ref val))
			{
				Entity groupPrefab = prefab;
				SpawnableObjectData spawnableObjectData = default(SpawnableObjectData);
				if (m_PrefabSpawnableObjectData.TryGetComponent(prefab, ref spawnableObjectData) && spawnableObjectData.m_RandomizationGroup != Entity.Null)
				{
					groupPrefab = spawnableObjectData.m_RandomizationGroup;
				}
				if (CheckRequirements(prefab, groupIndex, isExplicit: true, ref updateData))
				{
					Random random2 = random;
					((Random)(ref random)).NextInt();
					((Random)(ref random)).NextInt();
					((Random)(ref subRandom)).NextInt();
					((Random)(ref subRandom)).NextInt();
					Random val2 = default(Random);
					if (updateData.m_SelectedSpawnabled.IsCreated && updateData.m_SelectedSpawnabled.TryGetValue(new PlaceholderKey(groupPrefab, groupIndex), ref val2))
					{
						random2 = val2;
					}
					else
					{
						updateData.EnsureSelectedSpawnables((Allocator)2);
						updateData.m_SelectedSpawnabled.TryAdd(new PlaceholderKey(groupPrefab, groupIndex), random2);
					}
					if (((Random)(ref random2)).NextInt(100) < probability)
					{
						CreateSubObject(jobIndex, ref random2, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, ref updateData, prefab, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated: false, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
					}
				}
				return;
			}
			float num = 0f;
			bool updated = false;
			bool updated2 = false;
			float num2 = -1f;
			float num3 = -1f;
			float num4 = -1f;
			Entity prefab2 = Entity.Null;
			Entity prefab3 = Entity.Null;
			Entity prefab4 = Entity.Null;
			Entity groupPrefab2 = Entity.Null;
			Entity groupPrefab3 = Entity.Null;
			Entity groupPrefab4 = Entity.Null;
			Random random3 = default(Random);
			Random random4 = default(Random);
			Random random5 = default(Random);
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (placeholderObjectData.m_RandomizeGroupIndex)
			{
				((Random)(ref random)).NextInt();
				int num8 = ((Random)(ref subRandom)).NextInt() & 0x7FFFFFFF;
				groupIndex = math.select(num8, -1 - num8, groupIndex < 0);
			}
			PillarData pillarData = default(PillarData);
			NetGeometryData netGeometryData = default(NetGeometryData);
			ObjectGeometryData objectGeometryData3 = default(ObjectGeometryData);
			NetGeometryData netGeometryData2 = default(NetGeometryData);
			ObjectGeometryData objectGeometryData4 = default(ObjectGeometryData);
			PlaceableObjectData placeableObjectData3 = default(PlaceableObjectData);
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			PlaceableObjectData placeableObjectData2 = default(PlaceableObjectData);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			QuantityObjectData quantityObjectData = default(QuantityObjectData);
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
			WorkVehicleData workVehicleData = default(WorkVehicleData);
			WorkVehicleData workVehicleData2 = default(WorkVehicleData);
			Random val7 = default(Random);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = val[i].m_Object;
				if (!CheckRequirements(val3, groupIndex, isExplicit: false, ref updateData))
				{
					continue;
				}
				float num9 = 0f;
				float num10 = 0f;
				int num11 = 0;
				bool flag = false;
				if (m_PrefabPillarData.TryGetComponent(val3, ref pillarData))
				{
					switch (pillarData.m_Type)
					{
					case PillarType.Horizontal:
						num11 = 1;
						flag = true;
						if (m_PrefabNetGeometryData.TryGetComponent(ownerPrefab, ref netGeometryData) && m_PrefabObjectGeometryData.TryGetComponent(val3, ref objectGeometryData3))
						{
							float num16 = netGeometryData.m_ElevatedWidth - 1f;
							float max = pillarData.m_OffsetRange.max;
							num10 += 1f / (1f + math.abs(objectGeometryData3.m_Size.x - num16));
							num10 += 0.01f / (1f + math.max(0f, max));
						}
						break;
					case PillarType.Vertical:
						flag = true;
						if (m_PrefabNetGeometryData.TryGetComponent(ownerPrefab, ref netGeometryData2) && m_PrefabObjectGeometryData.TryGetComponent(val3, ref objectGeometryData4))
						{
							if (m_PrefabPlaceableObjectData.TryGetComponent(val3, ref placeableObjectData3))
							{
								objectGeometryData4.m_Size.y -= placeableObjectData3.m_PlacementOffset.y;
							}
							float num17 = ownerElevation + transformData.m_Position.y - ownerTransform.m_Position.y;
							float num18 = objectGeometryData4.m_Size.y - num17;
							num10 += 1f / (1f + math.select(2f * num18, 0f - num18, num18 < 0f));
							num9 = math.max(0f, netGeometryData2.m_ElevatedWidth * 0.5f - objectGeometryData4.m_Size.x * 0.5f);
						}
						break;
					case PillarType.Standalone:
						if (m_PrefabObjectGeometryData.TryGetComponent(val3, ref objectGeometryData2))
						{
							if (m_PrefabPlaceableObjectData.TryGetComponent(val3, ref placeableObjectData2))
							{
								objectGeometryData2.m_Size.y -= placeableObjectData2.m_PlacementOffset.y;
							}
							float num14 = ownerElevation + transformData.m_Position.y - ownerTransform.m_Position.y;
							float num15 = objectGeometryData2.m_Size.y - num14;
							num10 += 1f / (1f + math.select(2f * num15, 0f - num15, num15 < 0f));
						}
						break;
					case PillarType.Base:
						num11 = 2;
						if (m_PrefabObjectGeometryData.TryGetComponent(val3, ref objectGeometryData))
						{
							if (m_PrefabPlaceableObjectData.TryGetComponent(val3, ref placeableObjectData))
							{
								objectGeometryData.m_Size.y -= placeableObjectData.m_PlacementOffset.y;
							}
							float num12 = ownerElevation + transformData.m_Position.y - ownerTransform.m_Position.y;
							float num13 = objectGeometryData.m_Size.y - num12;
							num10 += 1f / (1f + math.select(2f * num13, 0f - num13, num13 < 0f));
						}
						break;
					}
				}
				if (m_PrefabQuantityObjectData.TryGetComponent(val3, ref quantityObjectData))
				{
					if ((quantityObjectData.m_Resources & updateData.m_StoredResources) != Resource.NoResource)
					{
						num10 += 1f;
						quantityObjectData.m_Resources = Resource.NoResource;
					}
					if (quantityObjectData.m_Resources != Resource.NoResource && m_DeliveryTruckData.TryGetComponent(topOwner, ref deliveryTruck) && (quantityObjectData.m_Resources & deliveryTruck.m_Resource) != Resource.NoResource)
					{
						num10 += 1f;
						quantityObjectData.m_Resources = Resource.NoResource;
					}
					if ((quantityObjectData.m_Resources & Resource.LocalMail) != Resource.NoResource && m_MailProducerData.HasComponent(topOwner))
					{
						num10 += 0.9f;
						quantityObjectData.m_Resources = Resource.NoResource;
					}
					if ((quantityObjectData.m_Resources & Resource.Garbage) != Resource.NoResource && m_GarbageProducerData.HasComponent(topOwner))
					{
						num10 += 0.9f;
						quantityObjectData.m_Resources = Resource.NoResource;
					}
					if (quantityObjectData.m_Resources != Resource.NoResource && m_Resources.HasBuffer(topOwner))
					{
						PrefabRef prefabRef = m_PrefabRefData[topOwner];
						Resource resource = quantityObjectData.m_Resources;
						if (m_PrefabCargoTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref cargoTransportVehicleData))
						{
							resource &= cargoTransportVehicleData.m_Resources;
						}
						if (resource != Resource.NoResource)
						{
							DynamicBuffer<Resources> val4 = m_Resources[topOwner];
							for (int j = 0; j < val4.Length; j++)
							{
								if ((val4[j].m_Resource & resource) != Resource.NoResource)
								{
									num10 += 1f;
									quantityObjectData.m_Resources = Resource.NoResource;
									break;
								}
							}
						}
					}
					if (quantityObjectData.m_MapFeature != MapFeature.None)
					{
						PrefabRef prefabRef2 = m_PrefabRefData[topOwner];
						if (m_PrefabWorkVehicleData.TryGetComponent(prefabRef2.m_Prefab, ref workVehicleData) && quantityObjectData.m_MapFeature == workVehicleData.m_MapFeature)
						{
							num10 += 1f;
							quantityObjectData.m_MapFeature = MapFeature.None;
						}
					}
					if (quantityObjectData.m_Resources != Resource.NoResource)
					{
						PrefabRef prefabRef3 = m_PrefabRefData[topOwner];
						if (m_PrefabWorkVehicleData.TryGetComponent(prefabRef3.m_Prefab, ref workVehicleData2) && (quantityObjectData.m_Resources & workVehicleData2.m_Resources) != Resource.NoResource)
						{
							num10 += 1f;
							quantityObjectData.m_Resources = Resource.NoResource;
						}
					}
					if (quantityObjectData.m_Resources != Resource.NoResource || quantityObjectData.m_MapFeature != MapFeature.None)
					{
						continue;
					}
				}
				SpawnableObjectData spawnableObjectData2 = m_PrefabSpawnableObjectData[val3];
				Entity val5 = ((spawnableObjectData2.m_RandomizationGroup != Entity.Null) ? spawnableObjectData2.m_RandomizationGroup : val3);
				Random val6 = random;
				((Random)(ref random)).NextInt();
				((Random)(ref random)).NextInt();
				((Random)(ref subRandom)).NextInt();
				((Random)(ref subRandom)).NextInt();
				if (updateData.m_SelectedSpawnabled.IsCreated && updateData.m_SelectedSpawnabled.TryGetValue(new PlaceholderKey(val5, groupIndex), ref val7))
				{
					num10 += 0.5f;
					val6 = val7;
				}
				switch (num11)
				{
				case 0:
					if (num10 > num2)
					{
						num = num9;
						updated = flag;
						num2 = num10;
						prefab2 = val3;
						groupPrefab2 = val5;
						random3 = val6;
						num5 = spawnableObjectData2.m_Probability;
					}
					else if (num10 == num2)
					{
						int probability4 = spawnableObjectData2.m_Probability;
						num5 += probability4;
						((Random)(ref subRandom)).NextInt();
						if (((Random)(ref random)).NextInt(num5) < probability4)
						{
							num = num9;
							updated = flag;
							prefab2 = val3;
							groupPrefab2 = val5;
							random3 = val6;
						}
					}
					break;
				case 1:
					if (num10 > num3)
					{
						updated2 = flag;
						num3 = num10;
						prefab3 = val3;
						groupPrefab3 = val5;
						random4 = val6;
						num6 = spawnableObjectData2.m_Probability;
					}
					else if (num10 == num3)
					{
						int probability3 = spawnableObjectData2.m_Probability;
						num6 += probability3;
						((Random)(ref subRandom)).NextInt();
						if (((Random)(ref random)).NextInt(num6) < probability3)
						{
							updated2 = flag;
							prefab3 = val3;
							groupPrefab3 = val5;
							random4 = val6;
						}
					}
					break;
				case 2:
					if (num10 > num4)
					{
						num4 = num10;
						prefab4 = val3;
						groupPrefab4 = val5;
						random5 = val6;
						num7 = spawnableObjectData2.m_Probability;
					}
					else if (num10 == num4)
					{
						int probability2 = spawnableObjectData2.m_Probability;
						num7 += probability2;
						((Random)(ref subRandom)).NextInt();
						if (((Random)(ref random)).NextInt(num7) < probability2)
						{
							prefab4 = val3;
							groupPrefab4 = val5;
							random5 = val6;
						}
					}
					break;
				}
			}
			if (num5 > 0)
			{
				updateData.EnsureSelectedSpawnables((Allocator)2);
				updateData.m_SelectedSpawnabled.TryAdd(new PlaceholderKey(groupPrefab2, groupIndex), random3);
				if (((Random)(ref random3)).NextInt(100) < probability)
				{
					if (num != 0f)
					{
						Transform transform = localTransformData;
						Transform transform2 = localTransformData;
						transform.m_Position.x -= num;
						transform2.m_Position.x += num;
						Transform transformData2 = ObjectUtils.LocalToWorld(ownerTransform, transform);
						Transform transformData3 = ObjectUtils.LocalToWorld(ownerTransform, transform2);
						Random random6 = random3;
						CreateSubObject(jobIndex, ref random3, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData2, transform, flags, ref updateData, prefab2, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
						CreateSubObject(jobIndex, ref random6, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData3, transform2, flags, ref updateData, prefab2, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
					}
					else
					{
						CreateSubObject(jobIndex, ref random3, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, ref updateData, prefab2, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
					}
				}
			}
			if (num6 > 0)
			{
				updateData.EnsureSelectedSpawnables((Allocator)2);
				updateData.m_SelectedSpawnabled.TryAdd(new PlaceholderKey(groupPrefab3, groupIndex), random4);
				if (((Random)(ref random4)).NextInt(100) < probability)
				{
					CreateSubObject(jobIndex, ref random4, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, ref updateData, prefab3, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated2, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
				}
			}
			if (num7 > 0)
			{
				updateData.EnsureSelectedSpawnables((Allocator)2);
				updateData.m_SelectedSpawnabled.TryAdd(new PlaceholderKey(groupPrefab4, groupIndex), random5);
				if (((Random)(ref random5)).NextInt(100) < probability)
				{
					CreateSubObject(jobIndex, ref random5, topOwner, owner, ownerPrefab, isTemp, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, ref updateData, prefab4, cacheTransform: false, native, relative, interpolated, underConstruction, destroyed, overridden, updated: false, alignIndex, parentMesh, groupIndex, probability, prefabSubIndex, depth);
				}
			}
		}

		private void CreateSubObject(int jobIndex, ref Random random, Entity topOwner, Entity owner, Entity ownerPrefab, bool isTemp, Temp ownerTemp, float ownerElevation, Entity oldSubObject, Transform ownerTransform, Transform transformData, Transform localTransformData, SubObjectFlags flags, ref UpdateSubObjectsData updateData, Entity prefab, bool cacheTransform, bool native, bool relative, bool interpolated, bool underConstruction, bool isDestroyed, bool isOverridden, bool updated, int alignIndex, int parentMesh, int groupIndex, int probability, int prefabSubIndex, int depth)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_113c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1059: Unknown result type (might be due to invalid IL or missing references)
			//IL_100f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1170: Unknown result type (might be due to invalid IL or missing references)
			//IL_1161: Unknown result type (might be due to invalid IL or missing references)
			//IL_106f: Unknown result type (might be due to invalid IL or missing references)
			//IL_102a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_1084: Unknown result type (might be due to invalid IL or missing references)
			//IL_1044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_121a: Unknown result type (might be due to invalid IL or missing references)
			//IL_11af: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_10df: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1103: Unknown result type (might be due to invalid IL or missing references)
			//IL_1116: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_093f: Unknown result type (might be due to invalid IL or missing references)
			//IL_112d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0988: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			bool flag = m_PrefabObjectGeometryData.TryGetComponent(prefab, ref objectGeometryData);
			bool flag2 = m_PrefabData.IsComponentEnabled(prefab);
			PillarData pillarData = default(PillarData);
			if (alignIndex >= 0 && m_PrefabPillarData.TryGetComponent(prefab, ref pillarData))
			{
				switch (pillarData.m_Type)
				{
				case PillarType.Vertical:
					flags |= SubObjectFlags.AnchorTop;
					flags |= SubObjectFlags.OnGround;
					break;
				case PillarType.Standalone:
					flags |= SubObjectFlags.AnchorTop;
					flags |= SubObjectFlags.OnGround;
					break;
				case PillarType.Base:
					flags |= SubObjectFlags.OnGround;
					break;
				}
			}
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			m_PrefabPlaceableObjectData.TryGetComponent(prefab, ref placeableObjectData);
			if ((flags & SubObjectFlags.AnchorTop) != 0)
			{
				objectGeometryData.m_Bounds.max.y -= placeableObjectData.m_PlacementOffset.y;
				transformData.m_Position.y -= objectGeometryData.m_Bounds.max.y;
				localTransformData.m_Position.y -= objectGeometryData.m_Bounds.max.y;
			}
			else if ((flags & SubObjectFlags.AnchorCenter) != 0)
			{
				float num = (objectGeometryData.m_Bounds.max.y - objectGeometryData.m_Bounds.min.y) * 0.5f;
				transformData.m_Position.y -= num;
				localTransformData.m_Position.y -= num;
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if (flag)
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref random);
			}
			if (underConstruction && flag && (objectGeometryData.m_Flags & GeometryFlags.Marker) == 0 && !m_PrefabBuildingExtensionData.HasComponent(prefab) && !m_PrefabSubLanes.HasBuffer(prefab) && !m_PrefabSpawnLocationData.HasComponent(prefab))
			{
				return;
			}
			Elevation elevation = new Elevation(ownerElevation, (math.abs(parentMesh) >= 1000) ? ElevationFlags.Stacked : ((ElevationFlags)0));
			if ((flags & SubObjectFlags.OnGround) == 0)
			{
				elevation.m_Elevation += localTransformData.m_Position.y;
				if (ownerElevation >= 0f && elevation.m_Elevation >= -0.5f && elevation.m_Elevation < 0f)
				{
					elevation.m_Elevation = 0f;
				}
				if (parentMesh < 0)
				{
					elevation.m_Flags |= ElevationFlags.OnGround;
				}
			}
			else
			{
				if ((flags & (SubObjectFlags.AnchorTop | SubObjectFlags.AnchorCenter)) == 0)
				{
					transformData.m_Position.y = ownerTransform.m_Position.y - ownerElevation;
					localTransformData.m_Position.y = 0f - ownerElevation;
				}
				elevation.m_Elevation = 0f;
				elevation.m_Flags |= ElevationFlags.OnGround;
			}
			if ((elevation.m_Flags & ElevationFlags.OnGround) != 0)
			{
				bool flag3 = true;
				if (flag)
				{
					flag3 = (objectGeometryData.m_Flags & GeometryFlags.DeleteOverridden) == 0 && (objectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.Marker)) != 0;
				}
				if (flag3)
				{
					bool angledSample;
					Transform transform = ObjectUtils.AdjustPosition(transformData, ref elevation, prefab, out angledSample, ref m_TerrainHeightData, ref m_WaterSurfaceData, ref m_PrefabPlaceableObjectData, ref m_PrefabObjectGeometryData);
					if (math.abs(transform.m_Position.y - transformData.m_Position.y) >= 0.01f || (angledSample && MathUtils.RotationAngle(transform.m_Rotation, transformData.m_Rotation) >= math.radians(0.1f)))
					{
						transformData = transform;
					}
				}
			}
			if ((isDestroyed && (elevation.m_Flags & (ElevationFlags.Stacked | ElevationFlags.OnGround)) != ElevationFlags.OnGround && !m_PrefabBuildingExtensionData.HasComponent(prefab)) || ClearAreaHelpers.ShouldClear(updateData.m_ClearAreas, transformData.m_Position, (flags & SubObjectFlags.OnGround) != 0))
			{
				return;
			}
			if (oldSubObject == Entity.Null)
			{
				oldSubObject = FindOldSubObject(prefab, transformData, alignIndex, ref updateData);
			}
			else if (oldSubObject.Index < 0)
			{
				oldSubObject = Entity.Null;
			}
			if ((placeableObjectData.m_Flags & PlacementFlags.Swaying) != PlacementFlags.None)
			{
				relative = false;
			}
			int3 val = default(int3);
			((int3)(ref val))._002Ector(0, -1, -1);
			if (!m_EditorMode)
			{
				int num2 = parentMesh % 1000;
				if (num2 > 0)
				{
					((int3)(ref val)).yz = RenderingUtils.FindBoneIndex(ownerPrefab, ref localTransformData.m_Position, ref localTransformData.m_Rotation, num2, ref m_PrefabSubMeshes, ref m_PrefabProceduralBones);
					val.x = math.select(0, num2, val.y >= 0);
				}
			}
			if (oldSubObject != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldSubObject);
				Temp temp = default(Temp);
				if (isTemp)
				{
					if (m_TempData.HasComponent(oldSubObject))
					{
						temp = m_TempData[oldSubObject];
						temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Dragging | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
						if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
						{
							temp.m_Flags |= TempFlags.Modify;
						}
						temp.m_Original = FindOriginalSubObject(prefab, temp.m_Original, transformData, alignIndex, ref updateData);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, oldSubObject, temp);
						Tree tree = default(Tree);
						if (temp.m_Original != Entity.Null && flag2 && m_PrefabTreeData.HasComponent(prefab) && m_TreeData.TryGetComponent(temp.m_Original, ref tree))
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, oldSubObject, tree);
						}
					}
					if (m_PrefabObjectGeometryData.HasComponent(prefab))
					{
						interpolated = true;
					}
					if ((placeableObjectData.m_Flags & PlacementFlags.Attached) != PlacementFlags.None)
					{
						Attached attached = default(Attached);
						m_AttachedData.TryGetComponent(oldSubObject, ref attached);
						attached.m_OldParent = attached.m_Parent;
						attached.m_Parent = Entity.Null;
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Attached>(jobIndex, oldSubObject, attached);
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSubObject, transformData);
					updated = true;
				}
				else if (!transformData.Equals(m_TransformData[oldSubObject]))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSubObject, transformData);
					updated = true;
				}
				if (cacheTransform)
				{
					LocalTransformCache localTransformCache = default(LocalTransformCache);
					localTransformCache.m_Position = localTransformData.m_Position;
					localTransformCache.m_Rotation = localTransformData.m_Rotation;
					localTransformCache.m_ParentMesh = parentMesh;
					localTransformCache.m_GroupIndex = groupIndex;
					localTransformCache.m_Probability = probability;
					localTransformCache.m_PrefabSubIndex = prefabSubIndex;
					if (m_LocalTransformCacheData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<LocalTransformCache>(jobIndex, oldSubObject, localTransformCache);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, oldSubObject, localTransformCache);
					}
				}
				else if (m_LocalTransformCacheData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LocalTransformCache>(jobIndex, oldSubObject);
				}
				PseudoRandomSeed pseudoRandomSeed2 = default(PseudoRandomSeed);
				if (flag)
				{
					if (m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldSubObject, pseudoRandomSeed2);
					}
					else if (!m_PseudoRandomSeedData.TryGetComponent(oldSubObject, ref pseudoRandomSeed2))
					{
						pseudoRandomSeed2 = pseudoRandomSeed;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, oldSubObject, pseudoRandomSeed2);
					}
				}
				if ((flags & SubObjectFlags.OnGround) == 0)
				{
					if (m_ElevationData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Elevation>(jobIndex, oldSubObject, elevation);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, oldSubObject, elevation);
					}
				}
				else if (m_ElevationData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Elevation>(jobIndex, oldSubObject);
				}
				if (alignIndex >= 0)
				{
					if (m_AlignedData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Aligned>(jobIndex, oldSubObject, new Aligned((ushort)alignIndex));
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Aligned>(jobIndex, oldSubObject, new Aligned((ushort)alignIndex));
					}
				}
				else if (m_AlignedData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Aligned>(jobIndex, oldSubObject);
				}
				if (m_RelativeData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Relative>(jobIndex, oldSubObject, new Relative(localTransformData, val));
				}
				Destroyed destroyed = default(Destroyed);
				if (interpolated || val.y >= 0)
				{
					if (m_InterpolatedTransformData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<InterpolatedTransform>(jobIndex, oldSubObject, new InterpolatedTransform(transformData));
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, oldSubObject, new InterpolatedTransform(transformData));
						updated = true;
					}
				}
				else if (m_InterpolatedTransformData.HasComponent(oldSubObject) && (!m_PrefabBuildingExtensionData.HasComponent(prefab) || !m_DestroyedData.TryGetComponent(oldSubObject, ref destroyed) || destroyed.m_Cleared >= 0f))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(jobIndex, oldSubObject);
					updated = true;
				}
				UnderConstruction underConstruction2 = default(UnderConstruction);
				if (temp.m_Original != Entity.Null)
				{
					underConstruction = m_UnderConstructionData.TryGetComponent(temp.m_Original, ref underConstruction2);
				}
				if (underConstruction)
				{
					if (!m_UnderConstructionData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(jobIndex, oldSubObject, underConstruction2);
						updated = true;
					}
				}
				else if (m_UnderConstructionData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<UnderConstruction>(jobIndex, oldSubObject);
					updated = true;
				}
				Destroyed destroyed2 = default(Destroyed);
				if (temp.m_Original != Entity.Null && (ownerTemp.m_Flags & TempFlags.Upgrade) == 0)
				{
					isDestroyed = m_DestroyedData.TryGetComponent(temp.m_Original, ref destroyed2);
				}
				if (isDestroyed)
				{
					if (!m_DestroyedData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Destroyed>(jobIndex, oldSubObject, destroyed2);
						updated = true;
					}
				}
				else if (m_DestroyedData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Destroyed>(jobIndex, oldSubObject);
					updated = true;
				}
				if (m_OverriddenData.HasComponent(oldSubObject))
				{
					isOverridden = true;
				}
				else if (isOverridden)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(jobIndex, oldSubObject, default(Overridden));
					updated = true;
				}
				if (updated && !m_UpdatedData.HasComponent(oldSubObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldSubObject, default(Updated));
				}
				if (m_PrefabStreetLightData.HasComponent(prefab))
				{
					StreetLight streetLight = default(StreetLight);
					bool flag4 = false;
					StreetLight streetLight2 = default(StreetLight);
					if (m_StreetLightData.TryGetComponent(oldSubObject, ref streetLight2))
					{
						streetLight = streetLight2;
						flag4 = true;
					}
					Building building = default(Building);
					Watercraft watercraft = default(Watercraft);
					if (m_BuildingData.TryGetComponent(topOwner, ref building))
					{
						StreetLightSystem.UpdateStreetLightState(ref streetLight, building);
					}
					else if (m_WatercraftData.TryGetComponent(topOwner, ref watercraft))
					{
						StreetLightSystem.UpdateStreetLightState(ref streetLight, watercraft);
					}
					if (flag4)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<StreetLight>(jobIndex, oldSubObject, streetLight);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StreetLight>(jobIndex, oldSubObject, streetLight);
					}
				}
				StackData stackData = default(StackData);
				if (flag2 && m_PrefabStackData.TryGetComponent(prefab, ref stackData))
				{
					Stack stack = default(Stack);
					if (m_StackData.TryGetComponent(temp.m_Original, ref stack))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stack>(jobIndex, oldSubObject, stack);
					}
					else if (updated || !m_StackData.HasComponent(oldSubObject))
					{
						if (stackData.m_Direction == StackDirection.Up)
						{
							stack.m_Range.min = stackData.m_FirstBounds.min - elevation.m_Elevation;
							stack.m_Range.max = stackData.m_LastBounds.max;
						}
						else
						{
							stack.m_Range.min = stackData.m_FirstBounds.min;
							stack.m_Range.max = stackData.m_FirstBounds.max + MathUtils.Size(stackData.m_MiddleBounds) * 2f + MathUtils.Size(stackData.m_LastBounds);
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stack>(jobIndex, oldSubObject, stack);
					}
				}
				if (m_SubObjects.HasBuffer(oldSubObject))
				{
					if (depth < 7)
					{
						updateData.EnsureDeepOwners((Allocator)2);
						ref NativeList<DeepSubObjectOwnerData> reference = ref updateData.m_DeepOwners;
						DeepSubObjectOwnerData deepSubObjectOwnerData = new DeepSubObjectOwnerData
						{
							m_Transform = transformData,
							m_Temp = temp,
							m_Entity = oldSubObject,
							m_Prefab = prefab,
							m_Elevation = elevation.m_Elevation,
							m_RandomSeed = pseudoRandomSeed2,
							m_HasRandomSeed = flag,
							m_UnderConstruction = underConstruction,
							m_Destroyed = isDestroyed,
							m_Overridden = isOverridden,
							m_Depth = depth + 1
						};
						reference.Add(ref deepSubObjectOwnerData);
					}
					else
					{
						m_LoopErrorPrefabs.Enqueue(prefab);
					}
				}
				return;
			}
			ObjectData objectData = m_PrefabObjectData[prefab];
			if (!((EntityArchetype)(ref objectData.m_Archetype)).Valid)
			{
				return;
			}
			Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val2, new Owner(owner));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val2, new PrefabRef(prefab));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val2, transformData);
			if ((placeableObjectData.m_Flags & PlacementFlags.Attached) != PlacementFlags.None)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Attached>(jobIndex, val2, default(Attached));
			}
			Temp temp2 = default(Temp);
			if (isTemp)
			{
				temp2.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Dragging | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp2.m_Flags |= TempFlags.Modify;
				}
				temp2.m_Original = FindOriginalSubObject(prefab, Entity.Null, transformData, alignIndex, ref updateData);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val2, temp2);
				if (m_PrefabObjectGeometryData.HasComponent(prefab))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Animation>(jobIndex, val2, default(Animation));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, val2, default(InterpolatedTransform));
				}
				if (temp2.m_Original != Entity.Null)
				{
					Tree tree2 = default(Tree);
					if (flag2 && m_PrefabTreeData.HasComponent(prefab) && m_TreeData.TryGetComponent(temp2.m_Original, ref tree2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, val2, tree2);
					}
					if ((temp2.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0 && m_OverriddenData.HasComponent(temp2.m_Original))
					{
						isOverridden = true;
					}
					if (owner.Index >= 0 && !m_TempData.HasComponent(owner))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(jobIndex, temp2.m_Original, default(Hidden));
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, temp2.m_Original, default(BatchesUpdated));
					}
				}
			}
			if (cacheTransform)
			{
				LocalTransformCache localTransformCache2 = default(LocalTransformCache);
				localTransformCache2.m_Position = localTransformData.m_Position;
				localTransformCache2.m_Rotation = localTransformData.m_Rotation;
				localTransformCache2.m_ParentMesh = parentMesh;
				localTransformCache2.m_GroupIndex = groupIndex;
				localTransformCache2.m_Probability = probability;
				localTransformCache2.m_PrefabSubIndex = prefabSubIndex;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, val2, localTransformCache2);
			}
			PseudoRandomSeed pseudoRandomSeed3 = default(PseudoRandomSeed);
			if (flag)
			{
				if (!m_PseudoRandomSeedData.TryGetComponent(temp2.m_Original, ref pseudoRandomSeed3))
				{
					pseudoRandomSeed3 = pseudoRandomSeed;
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed3);
			}
			if ((flags & SubObjectFlags.OnGround) == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val2, elevation);
			}
			if (alignIndex >= 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Aligned>(jobIndex, val2, new Aligned((ushort)alignIndex));
			}
			if (relative || val.y >= 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Static>(jobIndex, val2);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Relative>(jobIndex, val2, new Relative(localTransformData, val));
			}
			if (interpolated || val.y >= 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, val2, new InterpolatedTransform(transformData));
			}
			UnderConstruction underConstruction3 = default(UnderConstruction);
			if (temp2.m_Original != Entity.Null)
			{
				underConstruction = m_UnderConstructionData.TryGetComponent(temp2.m_Original, ref underConstruction3);
			}
			if (underConstruction)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(jobIndex, val2, underConstruction3);
			}
			Destroyed destroyed3 = default(Destroyed);
			if (temp2.m_Original != Entity.Null && (ownerTemp.m_Flags & TempFlags.Upgrade) == 0)
			{
				isDestroyed = m_DestroyedData.TryGetComponent(temp2.m_Original, ref destroyed3);
			}
			if (isDestroyed)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Destroyed>(jobIndex, val2, destroyed3);
			}
			if (isOverridden)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(jobIndex, val2, default(Overridden));
			}
			if (native)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, val2, default(Native));
			}
			if (m_EditorMode && m_EditorContainerData.HasComponent(temp2.m_Original))
			{
				Game.Tools.EditorContainer editorContainer = m_EditorContainerData[temp2.m_Original];
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Tools.EditorContainer>(jobIndex, val2, editorContainer);
				if (m_PrefabEffectData.HasComponent(editorContainer.m_Prefab))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EnabledEffect>(jobIndex, val2);
				}
			}
			if (m_PrefabStreetLightData.HasComponent(prefab))
			{
				StreetLight streetLight3 = default(StreetLight);
				StreetLight streetLight4 = default(StreetLight);
				if (m_StreetLightData.TryGetComponent(temp2.m_Original, ref streetLight4))
				{
					streetLight3 = streetLight4;
				}
				Building building2 = default(Building);
				Watercraft watercraft2 = default(Watercraft);
				if (m_BuildingData.TryGetComponent(topOwner, ref building2))
				{
					StreetLightSystem.UpdateStreetLightState(ref streetLight3, building2);
				}
				else if (m_WatercraftData.TryGetComponent(topOwner, ref watercraft2))
				{
					StreetLightSystem.UpdateStreetLightState(ref streetLight3, watercraft2);
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<StreetLight>(jobIndex, val2, streetLight3);
			}
			StackData stackData2 = default(StackData);
			if (flag2 && m_PrefabStackData.TryGetComponent(prefab, ref stackData2))
			{
				Stack stack2 = default(Stack);
				if (m_StackData.TryGetComponent(temp2.m_Original, ref stack2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Stack>(jobIndex, val2, stack2);
				}
				else
				{
					if (stackData2.m_Direction == StackDirection.Up)
					{
						stack2.m_Range.min = stackData2.m_FirstBounds.min - elevation.m_Elevation;
						stack2.m_Range.max = stackData2.m_LastBounds.max;
					}
					else
					{
						stack2.m_Range.min = stackData2.m_FirstBounds.min;
						stack2.m_Range.max = stackData2.m_FirstBounds.max + MathUtils.Size(stackData2.m_MiddleBounds) * 2f + MathUtils.Size(stackData2.m_LastBounds);
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Stack>(jobIndex, val2, stack2);
				}
			}
			if (m_PrefabSpawnLocationData.HasComponent(prefab))
			{
				SpawnLocation spawnLocation = new SpawnLocation
				{
					m_GroupIndex = groupIndex
				};
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SpawnLocation>(jobIndex, val2, spawnLocation);
			}
			if (m_PrefabSubObjects.HasBuffer(prefab))
			{
				if (depth < 7)
				{
					updateData.EnsureDeepOwners((Allocator)2);
					ref NativeList<DeepSubObjectOwnerData> reference2 = ref updateData.m_DeepOwners;
					DeepSubObjectOwnerData deepSubObjectOwnerData = new DeepSubObjectOwnerData
					{
						m_Transform = transformData,
						m_Temp = temp2,
						m_Entity = val2,
						m_Prefab = prefab,
						m_Elevation = elevation.m_Elevation,
						m_RandomSeed = pseudoRandomSeed3,
						m_New = true,
						m_HasRandomSeed = flag,
						m_UnderConstruction = underConstruction,
						m_Destroyed = isDestroyed,
						m_Overridden = isOverridden,
						m_Depth = depth + 1
					};
					reference2.Add(ref deepSubObjectOwnerData);
				}
				else
				{
					m_LoopErrorPrefabs.Enqueue(prefab);
				}
			}
		}

		private void CreateContainerObject(int jobIndex, Entity owner, bool isTemp, Temp ownerTemp, float ownerElevation, Entity oldSubObject, Transform transformData, Transform localTransformData, ref UpdateSubObjectsData updateData, Entity prefab, float3 scale, float intensity, int parentMesh, int groupIndex, int prefabSubIndex)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			Elevation elevation = new Elevation(ownerElevation, (ElevationFlags)0);
			elevation.m_Elevation += localTransformData.m_Position.y;
			if (oldSubObject == Entity.Null)
			{
				oldSubObject = FindOldSubObject(prefab, transformData, -1, ref updateData);
			}
			if (oldSubObject != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldSubObject);
				if (isTemp)
				{
					if (m_TempData.HasComponent(oldSubObject))
					{
						Temp temp = m_TempData[oldSubObject];
						temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Dragging | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
						if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
						{
							temp.m_Flags |= TempFlags.Modify;
						}
						temp.m_Original = FindOriginalSubObject(prefab, temp.m_Original, transformData, -1, ref updateData);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, oldSubObject, temp);
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSubObject, transformData);
					if (!m_UpdatedData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldSubObject, default(Updated));
					}
				}
				else if (!transformData.Equals(m_TransformData[oldSubObject]))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSubObject, transformData);
					if (!m_UpdatedData.HasComponent(oldSubObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldSubObject, default(Updated));
					}
				}
				LocalTransformCache localTransformCache = default(LocalTransformCache);
				localTransformCache.m_Position = localTransformData.m_Position;
				localTransformCache.m_Rotation = localTransformData.m_Rotation;
				localTransformCache.m_ParentMesh = parentMesh;
				localTransformCache.m_GroupIndex = groupIndex;
				localTransformCache.m_Probability = 100;
				localTransformCache.m_PrefabSubIndex = prefabSubIndex;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<LocalTransformCache>(jobIndex, oldSubObject, localTransformCache);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Elevation>(jobIndex, oldSubObject, elevation);
				Game.Tools.EditorContainer editorContainer = new Game.Tools.EditorContainer
				{
					m_Prefab = prefab,
					m_Scale = scale,
					m_Intensity = intensity,
					m_GroupIndex = groupIndex
				};
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Tools.EditorContainer>(jobIndex, oldSubObject, editorContainer);
				return;
			}
			ObjectData objectData = m_PrefabObjectData[m_TransformEditor];
			if (!((EntityArchetype)(ref objectData.m_Archetype)).Valid)
			{
				return;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(owner));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(m_TransformEditor));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, transformData);
			Temp temp2 = default(Temp);
			if (isTemp)
			{
				temp2.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Dragging | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp2.m_Flags |= TempFlags.Modify;
				}
				temp2.m_Original = FindOriginalSubObject(prefab, Entity.Null, transformData, -1, ref updateData);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val, temp2);
			}
			LocalTransformCache localTransformCache2 = default(LocalTransformCache);
			localTransformCache2.m_Position = localTransformData.m_Position;
			localTransformCache2.m_Rotation = localTransformData.m_Rotation;
			localTransformCache2.m_ParentMesh = parentMesh;
			localTransformCache2.m_GroupIndex = groupIndex;
			localTransformCache2.m_Probability = 100;
			localTransformCache2.m_PrefabSubIndex = prefabSubIndex;
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, val, localTransformCache2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val, elevation);
			Game.Tools.EditorContainer editorContainer2 = new Game.Tools.EditorContainer
			{
				m_Prefab = prefab,
				m_Scale = scale,
				m_Intensity = intensity,
				m_GroupIndex = groupIndex
			};
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Tools.EditorContainer>(jobIndex, val, editorContainer2);
			if (m_PrefabEffectData.HasComponent(editorContainer2.m_Prefab))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EnabledEffect>(jobIndex, val);
			}
		}

		private void EnsurePlaceholderRequirements(Entity owner, Entity ownerPrefab, ref UpdateSubObjectsData updateData, ref Random random, bool isObject)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			if (updateData.m_RequirementsSearched)
			{
				return;
			}
			updateData.EnsurePlaceholderRequirements((Allocator)2);
			bool flag = false;
			bool flag2 = false;
			Owner owner2 = default(Owner);
			if (!isObject && m_OwnerData.TryGetComponent(owner, ref owner2))
			{
				owner = owner2.m_Owner;
				Attachment attachment = default(Attachment);
				if (m_AttachmentData.TryGetComponent(owner2.m_Owner, ref attachment))
				{
					owner = attachment.m_Attached;
				}
			}
			if (m_CityServiceUpkeepData.HasComponent(owner))
			{
				DynamicBuffer<ServiceUpkeepData> val = default(DynamicBuffer<ServiceUpkeepData>);
				if (m_PrefabServiceUpkeepDatas.TryGetBuffer(ownerPrefab, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						updateData.m_StoredResources |= val[i].m_Upkeep.m_Resource;
					}
				}
				StorageCompanyData storageCompanyData = default(StorageCompanyData);
				if (m_StorageCompanyData.TryGetComponent(ownerPrefab, ref storageCompanyData))
				{
					updateData.m_StoredResources |= storageCompanyData.m_StoredResources;
				}
				if (m_GarbageFacilityData.HasComponent(owner))
				{
					updateData.m_StoredResources |= Resource.Garbage;
				}
			}
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			if (m_BuildingRenters.TryGetBuffer(owner, ref val2))
			{
				CompanyData companyData = default(CompanyData);
				StorageCompanyData storageCompanyData2 = default(StorageCompanyData);
				DynamicBuffer<HouseholdCitizen> val3 = default(DynamicBuffer<HouseholdCitizen>);
				Citizen citizen = default(Citizen);
				Household household = default(Household);
				DynamicBuffer<HouseholdAnimal> val4 = default(DynamicBuffer<HouseholdAnimal>);
				for (int j = 0; j < val2.Length; j++)
				{
					Entity renter = val2[j].m_Renter;
					if (m_Deleteds.HasComponent(renter))
					{
						continue;
					}
					if (m_CompanyData.TryGetComponent(renter, ref companyData))
					{
						updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Renter;
						Entity prefab = m_PrefabRefData[renter].m_Prefab;
						if (companyData.m_Brand != Entity.Null)
						{
							updateData.m_PlaceholderRequirements.TryAdd(companyData.m_Brand, new int2(0, 1));
							AddAffiliatedBrands(prefab, ref updateData, ref random);
							flag2 = true;
						}
						if (m_StorageCompanyData.TryGetComponent(prefab, ref storageCompanyData2))
						{
							updateData.m_StoredResources |= storageCompanyData2.m_StoredResources;
						}
						updateData.m_PlaceholderRequirements.TryAdd(prefab, int2.op_Implicit(0));
					}
					else
					{
						if (!m_HouseholdCitizens.TryGetBuffer(renter, ref val3))
						{
							continue;
						}
						for (int k = 0; k < val3.Length; k++)
						{
							if (m_CitizenData.TryGetComponent(val3[k].m_Citizen, ref citizen))
							{
								switch (citizen.GetAge())
								{
								case CitizenAge.Child:
									updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Children;
									break;
								case CitizenAge.Teen:
									updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Teens;
									break;
								}
							}
						}
						if (m_HouseholdData.TryGetComponent(renter, ref household))
						{
							int2 consumptionBonuses = CitizenHappinessSystem.GetConsumptionBonuses(household.m_ConsumptionPerDay, val3.Length, in m_HappinessParameterData);
							if (consumptionBonuses.x + consumptionBonuses.y > 0)
							{
								updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.GoodWealth;
							}
						}
						if (m_HouseholdAnimals.TryGetBuffer(renter, ref val4) && val4.Length != 0)
						{
							updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Dogs;
						}
						if (m_HomelessHousehold.HasComponent(renter) && m_HomelessHousehold[renter].m_TempHome == owner)
						{
							updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Homeless;
						}
						else
						{
							updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Renter;
						}
					}
				}
			}
			if (!m_ResidentialPropertyData.HasComponent(owner))
			{
				updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Children | ObjectRequirementFlags.Teens | ObjectRequirementFlags.GoodWealth | ObjectRequirementFlags.Dogs;
			}
			Surface surface = default(Surface);
			if (m_SurfaceData.TryGetComponent(owner, ref surface) && surface.m_AccumulatedSnow >= 15)
			{
				updateData.m_PlaceholderRequirementFlags |= ObjectRequirementFlags.Snow;
			}
			DynamicBuffer<ObjectRequirementElement> val5 = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_ObjectRequirements.TryGetBuffer(ownerPrefab, ref val5))
			{
				int num = 0;
				DynamicBuffer<CompanyBrandElement> val6 = default(DynamicBuffer<CompanyBrandElement>);
				while (num < val5.Length)
				{
					ObjectRequirementElement objectRequirementElement = val5[num];
					if ((objectRequirementElement.m_Type & ObjectRequirementType.SelectOnly) != 0)
					{
						int num2 = num;
						while (++num2 < val5.Length && val5[num2].m_Group == objectRequirementElement.m_Group)
						{
						}
						Entity requirement = val5[((Random)(ref random)).NextInt(num, num2)].m_Requirement;
						updateData.m_PlaceholderRequirements.TryAdd(requirement, int2.op_Implicit(0));
						if (m_CompanyBrands.TryGetBuffer(requirement, ref val6))
						{
							if (val6.Length != 0)
							{
								Entity brand = val6[((Random)(ref random)).NextInt(val6.Length)].m_Brand;
								updateData.m_PlaceholderRequirements.TryAdd(brand, new int2(0, 1));
								AddAffiliatedBrands(requirement, ref updateData, ref random);
								flag2 = true;
							}
						}
						else if (m_PrefabThemeData.HasComponent(requirement))
						{
							flag = true;
						}
						num = num2;
					}
					else
					{
						num++;
					}
				}
			}
			if (!flag && m_DefaultTheme != Entity.Null)
			{
				updateData.m_PlaceholderRequirements.TryAdd(m_DefaultTheme, int2.op_Implicit(0));
			}
			if (!flag2 && m_BuildingConfigurationData.m_DefaultRenterBrand != Entity.Null)
			{
				updateData.m_PlaceholderRequirements.TryAdd(m_BuildingConfigurationData.m_DefaultRenterBrand, int2.op_Implicit(0));
			}
			updateData.m_RequirementsSearched = true;
		}

		private void AddAffiliatedBrands(Entity entity, ref UpdateSubObjectsData updateData, ref Random random)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<AffiliatedBrandElement> val = default(DynamicBuffer<AffiliatedBrandElement>);
			if (m_AffiliatedBrands.TryGetBuffer(entity, ref val) && val.Length != 0)
			{
				int num = val.Length + 3 >> 1;
				int num2 = 0;
				int num3 = 0;
				Random val2 = random;
				for (int i = ((Random)(ref random)).NextInt(num >> 1); i < val.Length; i += 1 + ((Random)(ref random)).NextInt(num))
				{
					num3++;
				}
				for (int j = ((Random)(ref val2)).NextInt(num >> 1); j < val.Length; j += 1 + ((Random)(ref val2)).NextInt(num))
				{
					updateData.m_PlaceholderRequirements.TryAdd(val[j].m_Brand, new int2(--num2, num3));
				}
			}
		}

		private Entity FindOldSubObject(Entity prefab, Entity original, ref UpdateSubObjectsData updateData)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (updateData.m_OldEntities.IsCreated && updateData.m_OldEntities.TryGetFirstValue(prefab, ref val, ref val2))
			{
				do
				{
					if (m_TempData.HasComponent(val) && m_TempData[val].m_Original == original)
					{
						updateData.m_OldEntities.Remove(val2);
						return val;
					}
				}
				while (updateData.m_OldEntities.TryGetNextValue(ref val, ref val2));
			}
			return Entity.Null;
		}

		private Entity FindOldSubObject(Entity prefab, Transform transform, int alignIndex, ref UpdateSubObjectsData updateData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (updateData.m_OldEntities.IsCreated && updateData.m_OldEntities.TryGetFirstValue(prefab, ref val2, ref val3))
			{
				float num = 0f;
				if (alignIndex >= 0)
				{
					Aligned aligned = default(Aligned);
					if (m_AlignedData.TryGetComponent(val2, ref aligned) && aligned.m_SubObjectIndex == alignIndex)
					{
						updateData.m_OldEntities.Remove(val3);
						return val2;
					}
				}
				else
				{
					val = val2;
					num = math.distance(m_TransformData[val2].m_Position, transform.m_Position);
				}
				NativeParallelMultiHashMapIterator<Entity> val4 = val3;
				Aligned aligned2 = default(Aligned);
				while (updateData.m_OldEntities.TryGetNextValue(ref val2, ref val3))
				{
					if (alignIndex >= 0)
					{
						if (m_AlignedData.TryGetComponent(val2, ref aligned2) && aligned2.m_SubObjectIndex == alignIndex)
						{
							updateData.m_OldEntities.Remove(val3);
							return val2;
						}
						continue;
					}
					float num2 = math.distance(m_TransformData[val2].m_Position, transform.m_Position);
					if (num2 < num)
					{
						val = val2;
						num = num2;
						val4 = val3;
					}
				}
				if (val != Entity.Null)
				{
					updateData.m_OldEntities.Remove(val4);
				}
			}
			return val;
		}

		private Entity FindOriginalSubObject(Entity prefab, Entity original, Transform transform, int alignIndex, ref UpdateSubObjectsData updateData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (updateData.m_OriginalEntities.IsCreated && updateData.m_OriginalEntities.TryGetFirstValue(prefab, ref val2, ref val3))
			{
				float num = 0f;
				if (val2 == original)
				{
					updateData.m_OriginalEntities.Remove(val3);
					return original;
				}
				if (alignIndex >= 0)
				{
					Aligned aligned = default(Aligned);
					if (m_AlignedData.TryGetComponent(val2, ref aligned) && aligned.m_SubObjectIndex == alignIndex)
					{
						updateData.m_OriginalEntities.Remove(val3);
						return val2;
					}
				}
				else
				{
					val = val2;
					num = math.distance(m_TransformData[val2].m_Position, transform.m_Position);
				}
				NativeParallelMultiHashMapIterator<Entity> val4 = val3;
				Aligned aligned2 = default(Aligned);
				while (updateData.m_OriginalEntities.TryGetNextValue(ref val2, ref val3))
				{
					if (val2 == original)
					{
						updateData.m_OriginalEntities.Remove(val3);
						return original;
					}
					if (alignIndex >= 0)
					{
						if (m_AlignedData.TryGetComponent(val2, ref aligned2) && aligned2.m_SubObjectIndex == alignIndex)
						{
							updateData.m_OriginalEntities.Remove(val3);
							return val2;
						}
						continue;
					}
					float num2 = math.distance(m_TransformData[val2].m_Position, transform.m_Position);
					if (num2 < num)
					{
						val = val2;
						num = num2;
						val4 = val3;
					}
				}
				if (val != Entity.Null)
				{
					updateData.m_OriginalEntities.Remove(val4);
				}
			}
			return val;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Object> __Game_Objects_Object_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SubObjectsUpdated> __Game_Objects_SubObjectsUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> __Game_Buildings_RentersUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Created> __Game_Common_Created_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Secondary> __Game_Objects_Secondary_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Object> __Game_Objects_Object_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PillarData> __Game_Prefabs_PillarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ThemeData> __Game_Prefabs_ThemeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> __Game_Prefabs_MovingObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> __Game_Prefabs_QuantityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> __Game_Prefabs_WorkVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StreetLightData> __Game_Prefabs_StreetLightData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> __Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aligned> __Game_Objects_Aligned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StreetLight> __Game_Objects_StreetLight_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Surface> __Game_Objects_Surface_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResidentialProperty> __Game_Buildings_ResidentialProperty_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Area> __Game_Areas_Area_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Clear> __Game_Areas_Clear_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Watercraft> __Game_Vehicles_Watercraft_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> __Game_Prefabs_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CompanyBrandElement> __Game_Prefabs_CompanyBrandElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AffiliatedBrandElement> __Game_Prefabs_AffiliatedBrandElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

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
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Objects_Object_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Object>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_SubObjectsUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SubObjectsUpdated>(true);
			__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RentersUpdated>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ServiceUpgrade>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Creatures_Creature_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(true);
			__Game_Common_Created_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Created>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Secondary_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Secondary>(true);
			__Game_Objects_Object_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Object>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_PillarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PillarData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_ThemeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ThemeData>(true);
			__Game_Prefabs_MovingObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingObjectData>(true);
			__Game_Prefabs_QuantityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<QuantityObjectData>(true);
			__Game_Prefabs_WorkVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkVehicleData>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_StreetLightData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StreetLightData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderObjectData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Objects_Aligned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aligned>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_StreetLight_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StreetLight>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Objects_Surface_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Surface>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.GarbageFacility>(true);
			__Game_Buildings_ResidentialProperty_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResidentialProperty>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CityServiceUpkeep>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.OutsideConnection>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Tools.EditorContainer>(true);
			__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Areas_Area_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Areas_Clear_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Clear>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_Watercraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Watercraft>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubLane>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Prefabs_CompanyBrandElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CompanyBrandElement>(true);
			__Game_Prefabs_AffiliatedBrandElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AffiliatedBrandElement>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
		}
	}

	private const int kMaxSubObjectDepth = 7;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ToolSystem m_ToolSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ModificationBarrier2B m_ModificationBarrier;

	private EntityQuery m_UpdateQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_ContainerQuery;

	private EntityQuery m_BuildingSettingsQuery;

	private EntityQuery m_HappinessParameterQuery;

	private ComponentTypeSet m_AppliedTypes;

	private NativeQueue<Entity> m_LoopErrorPrefabs;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2B>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SubObject>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Event>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RentersUpdated>(),
			ComponentType.ReadOnly<SubObjectsUpdated>()
		};
		array[1] = val;
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array2[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_ContainerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<EditorContainerData>(),
			ComponentType.ReadOnly<ObjectData>()
		});
		m_BuildingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_UpdateQuery);
		m_LoopErrorPrefabs = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_LoopErrorPrefabs.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_0968: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		ShowLoopErrors();
		NativeQueue<SubObjectOwnerData> ownerQueue = default(NativeQueue<SubObjectOwnerData>);
		ownerQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<SubObjectOwnerData> val = default(NativeList<SubObjectOwnerData>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashSet<Entity> ignoreSet = default(NativeParallelHashSet<Entity>);
		ignoreSet._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, SubObjectOwnerData> ownerMap = default(NativeParallelHashMap<Entity, SubObjectOwnerData>);
		ownerMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		CheckSubObjectOwnersJob checkSubObjectOwnersJob = new CheckSubObjectOwnersJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectType = InternalCompilerInterface.GetComponentTypeHandle<Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectsUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<SubObjectsUpdated>(ref __TypeHandle.__Game_Objects_SubObjectsUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RentersUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RentersUpdated>(ref __TypeHandle.__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedData = InternalCompilerInterface.GetComponentLookup<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectData = InternalCompilerInterface.GetComponentLookup<Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AppliedTypes = m_AppliedTypes
		};
		EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
		checkSubObjectOwnersJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		checkSubObjectOwnersJob.m_OwnerQueue = ownerQueue.AsParallelWriter();
		CheckSubObjectOwnersJob checkSubObjectOwnersJob2 = checkSubObjectOwnersJob;
		CollectSubObjectOwnersJob collectSubObjectOwnersJob = new CollectSubObjectOwnersJob
		{
			m_OwnerQueue = ownerQueue,
			m_OwnerList = val,
			m_OwnerMap = ownerMap
		};
		FillIgnoreSetJob fillIgnoreSetJob = new FillIgnoreSetJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IgnoreSet = ignoreSet
		};
		Entity transformEditor = Entity.Null;
		if (m_ToolSystem.actionMode.IsEditor() && !((EntityQuery)(ref m_ContainerQuery)).IsEmptyIgnoreFilter)
		{
			transformEditor = ((EntityQuery)(ref m_ContainerQuery)).GetSingletonEntity();
		}
		JobHandle deps;
		UpdateSubObjectsJob updateSubObjectsJob = new UpdateSubObjectsJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPillarData = InternalCompilerInterface.GetComponentLookup<PillarData>(ref __TypeHandle.__Game_Prefabs_PillarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabThemeData = InternalCompilerInterface.GetComponentLookup<ThemeData>(ref __TypeHandle.__Game_Prefabs_ThemeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMovingObjectData = InternalCompilerInterface.GetComponentLookup<MovingObjectData>(ref __TypeHandle.__Game_Prefabs_MovingObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabQuantityObjectData = InternalCompilerInterface.GetComponentLookup<QuantityObjectData>(ref __TypeHandle.__Game_Prefabs_QuantityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkVehicleData = InternalCompilerInterface.GetComponentLookup<WorkVehicleData>(ref __TypeHandle.__Game_Prefabs_WorkVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStreetLightData = InternalCompilerInterface.GetComponentLookup<StreetLightData>(ref __TypeHandle.__Game_Prefabs_StreetLightData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderObjectData = InternalCompilerInterface.GetComponentLookup<PlaceholderObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyData = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AlignedData = InternalCompilerInterface.GetComponentLookup<Aligned>(ref __TypeHandle.__Game_Objects_Aligned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StreetLightData = InternalCompilerInterface.GetComponentLookup<StreetLight>(ref __TypeHandle.__Game_Objects_StreetLight_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SurfaceData = InternalCompilerInterface.GetComponentLookup<Surface>(ref __TypeHandle.__Game_Objects_Surface_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducerData = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerData = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentialPropertyData = InternalCompilerInterface.GetComponentLookup<ResidentialProperty>(ref __TypeHandle.__Game_Buildings_ResidentialProperty_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityServiceUpkeepData = InternalCompilerInterface.GetComponentLookup<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyData = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdData = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHousehold = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaClearData = InternalCompilerInterface.GetComponentLookup<Clear>(ref __TypeHandle.__Game_Areas_Clear_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftData = InternalCompilerInterface.GetComponentLookup<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingRenters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderObjects = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectRequirements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyBrands = InternalCompilerInterface.GetBufferLookup<CompanyBrandElement>(ref __TypeHandle.__Game_Prefabs_CompanyBrandElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AffiliatedBrands = InternalCompilerInterface.GetBufferLookup<AffiliatedBrandElement>(ref __TypeHandle.__Game_Prefabs_AffiliatedBrandElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabServiceUpkeepDatas = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_RandomSeed = RandomSeed.Next(),
			m_DefaultTheme = m_CityConfigurationSystem.defaultTheme,
			m_TransformEditor = transformEditor,
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingSettingsQuery)).GetSingleton<BuildingConfigurationData>(),
			m_HappinessParameterData = ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_AppliedTypes = m_AppliedTypes,
			m_OwnerList = val.AsDeferredJobArray(),
			m_IgnoreSet = ignoreSet,
			m_OwnerMap = ownerMap,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_LoopErrorPrefabs = m_LoopErrorPrefabs.AsParallelWriter()
		};
		val2 = m_ModificationBarrier.CreateCommandBuffer();
		updateSubObjectsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		UpdateSubObjectsJob updateSubObjectsJob2 = updateSubObjectsJob;
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CheckSubObjectOwnersJob>(checkSubObjectOwnersJob2, m_UpdateQuery, ((SystemBase)this).Dependency);
		JobHandle val4 = IJobExtensions.Schedule<CollectSubObjectOwnersJob>(collectSubObjectOwnersJob, val3);
		JobHandle val5 = JobChunkExtensions.Schedule<FillIgnoreSetJob>(fillIgnoreSetJob, m_TempQuery, ((SystemBase)this).Dependency);
		JobHandle val6 = IJobParallelForDeferExtensions.Schedule<UpdateSubObjectsJob, SubObjectOwnerData>(updateSubObjectsJob2, val, 1, JobHandle.CombineDependencies(val4, val5, deps));
		ownerQueue.Dispose(val4);
		val.Dispose(val6);
		ignoreSet.Dispose(val6);
		ownerMap.Dispose(val6);
		m_TerrainSystem.AddCPUHeightReader(val6);
		m_WaterSystem.AddSurfaceReader(val6);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val6);
		((SystemBase)this).Dependency = val6;
	}

	private void ShowLoopErrors()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (m_LoopErrorPrefabs.IsEmpty())
		{
			return;
		}
		NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
		val._002Ector(m_LoopErrorPrefabs.Count, AllocatorHandle.op_Implicit((Allocator)2));
		Entity val2 = default(Entity);
		while (m_LoopErrorPrefabs.TryDequeue(ref val2))
		{
			val.Add(val2);
		}
		PrefabSystem existingSystemManaged = ((ComponentSystemBase)this).World.GetExistingSystemManaged<PrefabSystem>();
		NativeArray<Entity> val3 = val.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
		Enumerator<Entity> enumerator = val3.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				PrefabBase prefab = existingSystemManaged.GetPrefab<PrefabBase>(current);
				COSystemBase.baseLog.ErrorFormat("Sub objects are nested too deep in '{0}'. Are you using a parent object as a sub object?", (object)((Object)prefab).name);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		val3.Dispose();
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
	public SubObjectSystem()
	{
	}
}
