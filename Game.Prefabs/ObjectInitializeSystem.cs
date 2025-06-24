using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ObjectInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct FixPlaceholdersJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferTypeHandle<PlaceholderObjectElement> m_PlaceholderObjectElementType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<PlaceholderObjectElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceholderObjectElement>(ref m_PlaceholderObjectElementType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<PlaceholderObjectElement> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					if (m_DeletedData.HasComponent(val[j].m_Object))
					{
						val.RemoveAtSwapBack(j--);
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
	private struct InitializeSubNetsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		public ComponentTypeHandle<PlaceableObjectData> m_PlaceableObjectDataType;

		public BufferTypeHandle<SubNet> m_SubNetType;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			NativeArray<PlaceableObjectData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PlaceableObjectData>(ref m_PlaceableObjectDataType);
			BufferAccessor<SubNet> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubNet>(ref m_SubNetType);
			NativeList<int> val2 = default(NativeList<int>);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<SubNet> val3 = bufferAccessor[i];
				Game.Objects.PlacementFlags placementFlags = Game.Objects.PlacementFlags.None;
				if (val3.Length != 0)
				{
					if (!val2.IsCreated)
					{
						val2._002Ector(val3.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
					}
					for (int j = 0; j < val3.Length; j++)
					{
						ref SubNet reference = ref val3.ElementAt(j);
						if (reference.m_NodeIndex.x >= 0)
						{
							int num;
							while (val2.Length <= reference.m_NodeIndex.x)
							{
								num = 0;
								val2.Add(ref num);
							}
							num = reference.m_NodeIndex.x;
							int num2 = val2[num];
							val2[num] = num2 + 1;
						}
						if (reference.m_NodeIndex.y >= 0 && reference.m_NodeIndex.y != reference.m_NodeIndex.x)
						{
							int num2;
							while (val2.Length <= reference.m_NodeIndex.y)
							{
								num2 = 0;
								val2.Add(ref num2);
							}
							num2 = reference.m_NodeIndex.y;
							int num = val2[num2];
							val2[num2] = num + 1;
						}
					}
					for (int k = 0; k < val3.Length; k++)
					{
						ref SubNet reference2 = ref val3.ElementAt(k);
						if (reference2.m_NodeIndex.x >= 0 && val2[reference2.m_NodeIndex.x] == 1)
						{
							reference2.m_Snapping.x = GetEnableSnapping(reference2.m_Prefab);
						}
						else
						{
							reference2.m_Snapping.x = false;
						}
						if (reference2.m_NodeIndex.y >= 0 && reference2.m_NodeIndex.y != reference2.m_NodeIndex.x && val2[reference2.m_NodeIndex.y] == 1)
						{
							reference2.m_Snapping.y = GetEnableSnapping(reference2.m_Prefab);
						}
						else
						{
							reference2.m_Snapping.y = false;
						}
						if (math.any(reference2.m_Snapping))
						{
							placementFlags |= Game.Objects.PlacementFlags.SubNetSnap;
						}
					}
					val2.Clear();
				}
				if (nativeArray.Length != 0)
				{
					CollectionUtils.ElementAt<PlaceableObjectData>(nativeArray, i).m_Flags |= placementFlags;
				}
			}
			if (val2.IsCreated)
			{
				val2.Dispose();
			}
		}

		private bool GetEnableSnapping(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			NetData netData = default(NetData);
			if (m_NetData.TryGetComponent(prefab, ref netData))
			{
				return (netData.m_RequiredLayers & (Layer.MarkerPathway | Layer.MarkerTaxiway)) == 0;
			}
			return false;
		}
	}

	[BurstCompile]
	private struct FindPlaceholderRequirementsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public BufferTypeHandle<PlaceholderObjectElement> m_PlaceholderObjectElementType;

		public ComponentTypeHandle<PlaceholderObjectData> m_PlaceholderObjectDataType;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirementElements;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			NativeArray<PlaceholderObjectData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PlaceholderObjectData>(ref m_PlaceholderObjectDataType);
			BufferAccessor<PlaceholderObjectElement> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<PlaceholderObjectElement>(ref m_PlaceholderObjectElementType);
			DynamicBuffer<ObjectRequirementElement> val3 = default(DynamicBuffer<ObjectRequirementElement>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ref PlaceholderObjectData reference = ref CollectionUtils.ElementAt<PlaceholderObjectData>(nativeArray, i);
				DynamicBuffer<PlaceholderObjectElement> val2 = bufferAccessor[i];
				ObjectRequirementFlags objectRequirementFlags = (ObjectRequirementFlags)0;
				for (int j = 0; j < val2.Length; j++)
				{
					PlaceholderObjectElement placeholderObjectElement = val2[j];
					if (m_ObjectRequirementElements.TryGetBuffer(placeholderObjectElement.m_Object, ref val3))
					{
						for (int k = 0; k < val3.Length; k++)
						{
							ObjectRequirementElement objectRequirementElement = val3[k];
							objectRequirementFlags |= objectRequirementElement.m_RequireFlags;
							objectRequirementFlags |= objectRequirementElement.m_ForbidFlags;
						}
					}
				}
				reference.m_RequirementMask = objectRequirementFlags;
			}
		}
	}

	[BurstCompile]
	private struct FindSubObjectRequirementsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public BufferTypeHandle<SubObject> m_SubObjectType;

		public ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryDataType;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> m_PlaceholderObjectData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			if (!((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				NativeArray<ObjectGeometryData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryDataType);
				BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					ref ObjectGeometryData reference = ref CollectionUtils.ElementAt<ObjectGeometryData>(nativeArray, i);
					DynamicBuffer<SubObject> subObjects = bufferAccessor[i];
					reference.m_SubObjectMask = GetRequirementMask(subObjects);
				}
			}
		}

		private ObjectRequirementFlags GetRequirementMask(DynamicBuffer<SubObject> subObjects)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			ObjectRequirementFlags objectRequirementFlags = (ObjectRequirementFlags)0;
			PlaceholderObjectData placeholderObjectData = default(PlaceholderObjectData);
			DynamicBuffer<SubObject> subObjects2 = default(DynamicBuffer<SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				SubObject subObject = subObjects[i];
				if (m_PlaceholderObjectData.TryGetComponent(subObject.m_Prefab, ref placeholderObjectData))
				{
					objectRequirementFlags |= placeholderObjectData.m_RequirementMask;
				}
				else if (m_SubObjects.TryGetBuffer(subObject.m_Prefab, ref subObjects2))
				{
					objectRequirementFlags |= GetRequirementMask(subObjects2);
				}
			}
			return objectRequirementFlags;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UtilityObjectData> __Game_Prefabs_UtilityObjectData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PillarData> __Game_Prefabs_PillarData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MoveableBridgeData> __Game_Prefabs_MoveableBridgeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetObjectData> __Game_Prefabs_NetObjectData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PlantData> __Game_Prefabs_PlantData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanData> __Game_Prefabs_HumanData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<VehicleData> __Game_Prefabs_VehicleData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AssetStampData> __Game_Prefabs_AssetStampData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<GrowthScaleData> __Game_Prefabs_GrowthScaleData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<StackData> __Game_Prefabs_StackData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<QuantityObjectData> __Game_Prefabs_QuantityObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CreatureData> __Game_Prefabs_CreatureData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle;

		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RW_BufferTypeHandle;

		public BufferTypeHandle<CharacterElement> __Game_Prefabs_CharacterElement_RW_BufferTypeHandle;

		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RW_BufferTypeHandle;

		public BufferTypeHandle<SubNet> __Game_Prefabs_SubNet_RW_BufferTypeHandle;

		public BufferTypeHandle<SubLane> __Game_Prefabs_SubLane_RW_BufferTypeHandle;

		public BufferTypeHandle<SubArea> __Game_Prefabs_SubArea_RW_BufferTypeHandle;

		public BufferTypeHandle<SubAreaNode> __Game_Prefabs_SubAreaNode_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public BufferTypeHandle<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<PlaceholderObjectData> __Game_Prefabs_PlaceholderObjectData_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> __Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_UtilityObjectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UtilityObjectData>(true);
			__Game_Prefabs_PillarData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PillarData>(true);
			__Game_Prefabs_MoveableBridgeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MoveableBridgeData>(true);
			__Game_Prefabs_NetObjectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetObjectData>(true);
			__Game_Prefabs_PlantData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlantData>(true);
			__Game_Prefabs_HumanData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(true);
			__Game_Prefabs_VehicleData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleData>(true);
			__Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingExtensionData>(false);
			__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(false);
			__Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceableObjectData>(false);
			__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableObjectData>(false);
			__Game_Prefabs_AssetStampData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AssetStampData>(false);
			__Game_Prefabs_GrowthScaleData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GrowthScaleData>(false);
			__Game_Prefabs_StackData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StackData>(false);
			__Game_Prefabs_QuantityObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<QuantityObjectData>(false);
			__Game_Prefabs_CreatureData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreatureData>(false);
			__Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingTerraformData>(false);
			__Game_Prefabs_SubMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(false);
			__Game_Prefabs_SubMeshGroup_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMeshGroup>(false);
			__Game_Prefabs_CharacterElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CharacterElement>(false);
			__Game_Prefabs_SubObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(false);
			__Game_Prefabs_SubNet_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubNet>(false);
			__Game_Prefabs_SubLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(false);
			__Game_Prefabs_SubArea_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubArea>(false);
			__Game_Prefabs_SubAreaNode_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubAreaNode>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceholderObjectElement>(false);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceholderObjectElement>(true);
			__Game_Prefabs_PlaceholderObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceholderObjectData>(false);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
			__Game_Prefabs_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderObjectData>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
		}
	}

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_PlaceholderQuery;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ObjectData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_PlaceholderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PlaceholderObjectElement>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff7: Unknown result type (might be due to invalid IL or missing references)
		//IL_100f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1014: Unknown result type (might be due to invalid IL or missing references)
		//IL_102c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1031: Unknown result type (might be due to invalid IL or missing references)
		//IL_1049: Unknown result type (might be due to invalid IL or missing references)
		//IL_104e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1061: Unknown result type (might be due to invalid IL or missing references)
		//IL_1062: Unknown result type (might be due to invalid IL or missing references)
		//IL_107a: Unknown result type (might be due to invalid IL or missing references)
		//IL_107f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1097: Unknown result type (might be due to invalid IL or missing references)
		//IL_109c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_111f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1124: Unknown result type (might be due to invalid IL or missing references)
		//IL_113c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1141: Unknown result type (might be due to invalid IL or missing references)
		//IL_1159: Unknown result type (might be due to invalid IL or missing references)
		//IL_115e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1176: Unknown result type (might be due to invalid IL or missing references)
		//IL_117b: Unknown result type (might be due to invalid IL or missing references)
		//IL_118c: Unknown result type (might be due to invalid IL or missing references)
		//IL_118e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1193: Unknown result type (might be due to invalid IL or missing references)
		//IL_119f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		bool flag = false;
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<UtilityObjectData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<UtilityObjectData>(ref __TypeHandle.__Game_Prefabs_UtilityObjectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PillarData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<PillarData>(ref __TypeHandle.__Game_Prefabs_PillarData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<MoveableBridgeData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<MoveableBridgeData>(ref __TypeHandle.__Game_Prefabs_MoveableBridgeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetObjectData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<NetObjectData>(ref __TypeHandle.__Game_Prefabs_NetObjectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PlantData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<PlantData>(ref __TypeHandle.__Game_Prefabs_PlantData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<HumanData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<HumanData>(ref __TypeHandle.__Game_Prefabs_HumanData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BuildingData> componentTypeHandle9 = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<VehicleData> componentTypeHandle10 = InternalCompilerInterface.GetComponentTypeHandle<VehicleData>(ref __TypeHandle.__Game_Prefabs_VehicleData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BuildingExtensionData> componentTypeHandle11 = InternalCompilerInterface.GetComponentTypeHandle<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ObjectGeometryData> componentTypeHandle12 = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PlaceableObjectData> componentTypeHandle13 = InternalCompilerInterface.GetComponentTypeHandle<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SpawnableObjectData> componentTypeHandle14 = InternalCompilerInterface.GetComponentTypeHandle<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<AssetStampData> componentTypeHandle15 = InternalCompilerInterface.GetComponentTypeHandle<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<GrowthScaleData> componentTypeHandle16 = InternalCompilerInterface.GetComponentTypeHandle<GrowthScaleData>(ref __TypeHandle.__Game_Prefabs_GrowthScaleData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<StackData> componentTypeHandle17 = InternalCompilerInterface.GetComponentTypeHandle<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<QuantityObjectData> componentTypeHandle18 = InternalCompilerInterface.GetComponentTypeHandle<QuantityObjectData>(ref __TypeHandle.__Game_Prefabs_QuantityObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<CreatureData> componentTypeHandle19 = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BuildingTerraformData> componentTypeHandle20 = InternalCompilerInterface.GetComponentTypeHandle<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubMesh> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubMeshGroup> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<CharacterElement> bufferTypeHandle3 = InternalCompilerInterface.GetBufferTypeHandle<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubObject> bufferTypeHandle4 = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubNet> bufferTypeHandle5 = InternalCompilerInterface.GetBufferTypeHandle<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubLane> bufferTypeHandle6 = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubArea> bufferTypeHandle7 = InternalCompilerInterface.GetBufferTypeHandle<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubAreaNode> bufferTypeHandle8 = InternalCompilerInterface.GetBufferTypeHandle<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			DynamicBuffer<SubMeshGroup> meshGroups = default(DynamicBuffer<SubMeshGroup>);
			DynamicBuffer<CharacterElement> characterElements = default(DynamicBuffer<CharacterElement>);
			Bounds2 xz2 = default(Bounds2);
			SubArea subArea = default(SubArea);
			for (int i = 0; i < chunks.Length; i++)
			{
				ArchetypeChunk val = chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref componentTypeHandle))
				{
					flag |= ((ArchetypeChunk)(ref val)).Has<SpawnableObjectData>(ref componentTypeHandle14);
					continue;
				}
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
				NativeArray<ObjectGeometryData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<ObjectGeometryData>(ref componentTypeHandle12);
				NativeArray<PlaceableObjectData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<PlaceableObjectData>(ref componentTypeHandle13);
				NativeArray<SpawnableObjectData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<SpawnableObjectData>(ref componentTypeHandle14);
				NativeArray<AssetStampData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<AssetStampData>(ref componentTypeHandle15);
				NativeArray<GrowthScaleData> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<GrowthScaleData>(ref componentTypeHandle16);
				NativeArray<StackData> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<StackData>(ref componentTypeHandle17);
				NativeArray<QuantityObjectData> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<QuantityObjectData>(ref componentTypeHandle18);
				NativeArray<CreatureData> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<CreatureData>(ref componentTypeHandle19);
				NativeArray<PillarData> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<PillarData>(ref componentTypeHandle4);
				NativeArray<BuildingTerraformData> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingTerraformData>(ref componentTypeHandle20);
				NativeArray<BuildingExtensionData> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingExtensionData>(ref componentTypeHandle11);
				BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubObject>(ref bufferTypeHandle4);
				BufferAccessor<SubArea> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubArea>(ref bufferTypeHandle7);
				BufferAccessor<SubMesh> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubMesh>(ref bufferTypeHandle);
				BufferAccessor<SubMeshGroup> bufferAccessor4 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubMeshGroup>(ref bufferTypeHandle2);
				BufferAccessor<CharacterElement> bufferAccessor5 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<CharacterElement>(ref bufferTypeHandle3);
				BufferAccessor<SubNet> bufferAccessor6 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubNet>(ref bufferTypeHandle5);
				BufferAccessor<SubLane> bufferAccessor7 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubLane>(ref bufferTypeHandle6);
				bool flag2 = ((ArchetypeChunk)(ref val)).Has<UtilityObjectData>(ref componentTypeHandle3);
				bool flag3 = ((ArchetypeChunk)(ref val)).Has<MoveableBridgeData>(ref componentTypeHandle5);
				bool flag4 = ((ArchetypeChunk)(ref val)).Has<NetObjectData>(ref componentTypeHandle6);
				bool isPlantObject = ((ArchetypeChunk)(ref val)).Has<PlantData>(ref componentTypeHandle7);
				bool flag5 = ((ArchetypeChunk)(ref val)).Has<HumanData>(ref componentTypeHandle8);
				bool flag6 = ((ArchetypeChunk)(ref val)).Has<BuildingData>(ref componentTypeHandle9) || nativeArray12.Length != 0;
				bool isVehicleObject = ((ArchetypeChunk)(ref val)).Has<VehicleData>(ref componentTypeHandle10);
				bool isCreatureObject = nativeArray9.Length != 0;
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					ObjectPrefab prefab = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[j]);
					ObjectGeometryData objectGeometryData = nativeArray2[j];
					objectGeometryData.m_MinLod = 255;
					objectGeometryData.m_Layers = (MeshLayer)0;
					PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
					if (nativeArray3.Length != 0)
					{
						placeableObjectData = nativeArray3[j];
					}
					GrowthScaleData growthScaleData = default(GrowthScaleData);
					if (nativeArray6.Length != 0)
					{
						growthScaleData = nativeArray6[j];
					}
					StackData stackData = default(StackData);
					if (nativeArray7.Length != 0)
					{
						stackData = nativeArray7[j];
						stackData.m_FirstBounds = new Bounds1(float.MaxValue, float.MinValue);
						stackData.m_MiddleBounds = new Bounds1(float.MaxValue, float.MinValue);
						stackData.m_LastBounds = new Bounds1(float.MaxValue, float.MinValue);
					}
					QuantityObjectData quantityObjectData = default(QuantityObjectData);
					if (nativeArray8.Length != 0)
					{
						quantityObjectData = nativeArray8[j];
					}
					CreatureData creatureData = default(CreatureData);
					if (nativeArray9.Length != 0)
					{
						creatureData = nativeArray9[j];
						CreaturePrefab creaturePrefab = prefab as CreaturePrefab;
						creatureData.m_Gender = creaturePrefab.m_Gender;
						if (!flag5)
						{
							objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.LowCollisionPriority;
						}
					}
					if (prefab is AssetStampPrefab)
					{
						AssetStampData assetStampData = nativeArray5[j];
						InitializePrefab(prefab as AssetStampPrefab, ref assetStampData, ref placeableObjectData, ref objectGeometryData);
						nativeArray5[j] = assetStampData;
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.ExclusiveGround | Game.Objects.GeometryFlags.WalkThrough | Game.Objects.GeometryFlags.OccupyZone | Game.Objects.GeometryFlags.Stampable | Game.Objects.GeometryFlags.HasLot;
					}
					else if (prefab is ObjectGeometryPrefab)
					{
						CollectionUtils.TryGet<SubMeshGroup>(bufferAccessor4, j, ref meshGroups);
						CollectionUtils.TryGet<CharacterElement>(bufferAccessor5, j, ref characterElements);
						InitializePrefab(prefab as ObjectGeometryPrefab, placeableObjectData, ref objectGeometryData, ref growthScaleData, ref stackData, ref quantityObjectData, ref creatureData, bufferAccessor3[j], meshGroups, characterElements, isPlantObject, flag5, flag6, isVehicleObject, isCreatureObject);
						if (nativeArray10.Length != 0)
						{
							if (nativeArray10[j].m_Type == PillarType.Horizontal)
							{
								objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.IgnoreBottomCollision;
							}
							else
							{
								objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.BaseCollision;
							}
							if (flag3)
							{
								objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.WalkThrough;
							}
							objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.OccupyZone | Game.Objects.GeometryFlags.CanSubmerge | Game.Objects.GeometryFlags.OptionalAttach;
						}
						else if (flag2)
						{
							objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.IgnoreSecondaryCollision;
						}
						else if (flag4)
						{
							objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.OccupyZone;
						}
						else
						{
							objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Brushable;
						}
					}
					else if (prefab is MarkerObjectPrefab)
					{
						InitializePrefab(prefab as MarkerObjectPrefab, placeableObjectData, ref objectGeometryData, bufferAccessor3[j]);
						placeableObjectData.m_Flags |= Game.Objects.PlacementFlags.CanOverlap;
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.WalkThrough;
					}
					if (!flag6 && nativeArray11.Length != 0)
					{
						BuildingTerraformOverride component = prefab.GetComponent<BuildingTerraformOverride>();
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							float2 xz = ((float3)(ref objectGeometryData.m_Pivot)).xz;
							float2 val2 = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f + objectGeometryData.m_LegOffset;
							((Bounds2)(ref xz2))._002Ector(xz - val2, xz + val2);
						}
						else
						{
							xz2 = ((Bounds3)(ref objectGeometryData.m_Bounds)).xz;
						}
						BuildingTerraformData buildingTerraformData = nativeArray11[j];
						BuildingInitializeSystem.InitializeTerraformData(component, ref buildingTerraformData, xz2, xz2);
						nativeArray11[j] = buildingTerraformData;
					}
					nativeArray2[j] = objectGeometryData;
					if (nativeArray3.Length != 0)
					{
						nativeArray3[j] = placeableObjectData;
					}
					if (nativeArray6.Length != 0)
					{
						nativeArray6[j] = growthScaleData;
					}
					if (nativeArray7.Length != 0)
					{
						if (stackData.m_FirstBounds.min > stackData.m_FirstBounds.max)
						{
							stackData.m_FirstBounds = default(Bounds1);
						}
						if (stackData.m_MiddleBounds.min > stackData.m_MiddleBounds.max)
						{
							stackData.m_MiddleBounds = default(Bounds1);
						}
						if (stackData.m_LastBounds.min > stackData.m_LastBounds.max)
						{
							stackData.m_LastBounds = default(Bounds1);
						}
						nativeArray7[j] = stackData;
					}
					if (nativeArray8.Length != 0)
					{
						nativeArray8[j] = quantityObjectData;
					}
					if (nativeArray9.Length != 0)
					{
						nativeArray9[j] = creatureData;
					}
				}
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					ObjectSubObjects component2 = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[k]).GetComponent<ObjectSubObjects>();
					if (component2.m_SubObjects == null)
					{
						continue;
					}
					DynamicBuffer<SubObject> val3 = bufferAccessor[k];
					for (int l = 0; l < component2.m_SubObjects.Length; l++)
					{
						ObjectSubObjectInfo objectSubObjectInfo = component2.m_SubObjects[l];
						ObjectPrefab objectPrefab = objectSubObjectInfo.m_Object;
						if (!((Object)(object)objectPrefab == (Object)null) && m_PrefabSystem.TryGetEntity(objectPrefab, out var entity))
						{
							SubObject subObject = new SubObject
							{
								m_Prefab = entity,
								m_Position = objectSubObjectInfo.m_Position,
								m_Rotation = objectSubObjectInfo.m_Rotation,
								m_ParentIndex = objectSubObjectInfo.m_ParentMesh,
								m_GroupIndex = objectSubObjectInfo.m_GroupIndex,
								m_Probability = math.select(objectSubObjectInfo.m_Probability, 100, objectSubObjectInfo.m_Probability == 0)
							};
							if (objectSubObjectInfo.m_ParentMesh == -1)
							{
								subObject.m_Flags |= SubObjectFlags.OnGround;
							}
							val3.Add(subObject);
						}
					}
				}
				for (int m = 0; m < bufferAccessor6.Length; m++)
				{
					ObjectPrefab prefab2 = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[m]);
					ObjectSubNets component3 = prefab2.GetComponent<ObjectSubNets>();
					if (component3.m_SubNets == null)
					{
						continue;
					}
					bool flag7 = false;
					DynamicBuffer<SubNet> val4 = bufferAccessor6[m];
					for (int n = 0; n < component3.m_SubNets.Length; n++)
					{
						ObjectSubNetInfo objectSubNetInfo = component3.m_SubNets[n];
						NetPrefab netPrefab = objectSubNetInfo.m_NetPrefab;
						if (!((Object)(object)netPrefab == (Object)null) && m_PrefabSystem.TryGetEntity(netPrefab, out var entity2))
						{
							SubNet subNet = new SubNet
							{
								m_Prefab = entity2,
								m_Curve = objectSubNetInfo.m_BezierCurve,
								m_NodeIndex = objectSubNetInfo.m_NodeIndex,
								m_InvertMode = component3.m_InvertWhen,
								m_ParentMesh = objectSubNetInfo.m_ParentMesh
							};
							if (MathUtils.Min(objectSubNetInfo.m_BezierCurve).y <= -2f)
							{
								flag7 = true;
							}
							NetCompositionHelpers.GetRequirementFlags(objectSubNetInfo.m_Upgrades, out subNet.m_Upgrades, out var sectionFlags);
							if (sectionFlags != 0)
							{
								COSystemBase.baseLog.ErrorFormat((Object)(object)prefab2, "ObjectSubNets ({0}[{1}]) cannot upgrade section flags: {2}", (object)((Object)prefab2).name, (object)n, (object)sectionFlags);
							}
							val4.Add(subNet);
						}
					}
					if (flag7)
					{
						if (nativeArray3.Length != 0)
						{
							PlaceableObjectData placeableObjectData2 = nativeArray3[m];
							placeableObjectData2.m_Flags |= Game.Objects.PlacementFlags.HasUndergroundElements;
							nativeArray3[m] = placeableObjectData2;
						}
						if (nativeArray12.Length != 0)
						{
							BuildingExtensionData buildingExtensionData = nativeArray12[m];
							buildingExtensionData.m_HasUndergroundElements = true;
							nativeArray12[m] = buildingExtensionData;
						}
					}
				}
				for (int num = 0; num < bufferAccessor7.Length; num++)
				{
					ObjectSubLanes component4 = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[num]).GetComponent<ObjectSubLanes>();
					if (component4.m_SubLanes == null)
					{
						continue;
					}
					DynamicBuffer<SubLane> val5 = bufferAccessor7[num];
					for (int num2 = 0; num2 < component4.m_SubLanes.Length; num2++)
					{
						ObjectSubLaneInfo objectSubLaneInfo = component4.m_SubLanes[num2];
						NetLanePrefab lanePrefab = objectSubLaneInfo.m_LanePrefab;
						if (!((Object)(object)lanePrefab == (Object)null) && m_PrefabSystem.TryGetEntity(lanePrefab, out var entity3))
						{
							val5.Add(new SubLane
							{
								m_Prefab = entity3,
								m_Curve = objectSubLaneInfo.m_BezierCurve,
								m_NodeIndex = objectSubLaneInfo.m_NodeIndex,
								m_ParentMesh = objectSubLaneInfo.m_ParentMesh
							});
						}
					}
				}
				if (bufferAccessor2.Length != 0)
				{
					BufferAccessor<SubAreaNode> bufferAccessor8 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubAreaNode>(ref bufferTypeHandle8);
					for (int num3 = 0; num3 < bufferAccessor2.Length; num3++)
					{
						ObjectSubAreas component5 = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[num3]).GetComponent<ObjectSubAreas>();
						if (component5.m_SubAreas == null)
						{
							continue;
						}
						int num4 = 0;
						for (int num5 = 0; num5 < component5.m_SubAreas.Length; num5++)
						{
							ObjectSubAreaInfo objectSubAreaInfo = component5.m_SubAreas[num5];
							if (!((Object)(object)objectSubAreaInfo.m_AreaPrefab == (Object)null) && m_PrefabSystem.TryGetEntity(objectSubAreaInfo.m_AreaPrefab, out var _))
							{
								num4 += objectSubAreaInfo.m_NodePositions.Length;
							}
						}
						DynamicBuffer<SubArea> val6 = bufferAccessor2[num3];
						DynamicBuffer<SubAreaNode> val7 = bufferAccessor8[num3];
						val6.EnsureCapacity(component5.m_SubAreas.Length);
						val7.ResizeUninitialized(num4);
						num4 = 0;
						for (int num6 = 0; num6 < component5.m_SubAreas.Length; num6++)
						{
							ObjectSubAreaInfo objectSubAreaInfo2 = component5.m_SubAreas[num6];
							if ((Object)(object)objectSubAreaInfo2.m_AreaPrefab == (Object)null || !m_PrefabSystem.TryGetEntity(objectSubAreaInfo2.m_AreaPrefab, out var entity5))
							{
								continue;
							}
							subArea.m_Prefab = entity5;
							subArea.m_NodeRange.x = num4;
							if (objectSubAreaInfo2.m_ParentMeshes != null && objectSubAreaInfo2.m_ParentMeshes.Length != 0)
							{
								for (int num7 = 0; num7 < objectSubAreaInfo2.m_NodePositions.Length; num7++)
								{
									float3 position = objectSubAreaInfo2.m_NodePositions[num7];
									int parentMesh = objectSubAreaInfo2.m_ParentMeshes[num7];
									val7[num4++] = new SubAreaNode(position, parentMesh);
								}
							}
							else
							{
								for (int num8 = 0; num8 < objectSubAreaInfo2.m_NodePositions.Length; num8++)
								{
									float3 position2 = objectSubAreaInfo2.m_NodePositions[num8];
									int parentMesh2 = -1;
									val7[num4++] = new SubAreaNode(position2, parentMesh2);
								}
							}
							subArea.m_NodeRange.y = num4;
							val6.Add(subArea);
						}
					}
				}
				if (nativeArray4.Length == 0)
				{
					continue;
				}
				NativeArray<Entity> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray(entityTypeHandle);
				for (int num9 = 0; num9 < nativeArray4.Length; num9++)
				{
					Entity obj = nativeArray13[num9];
					SpawnableObjectData spawnableObjectData = nativeArray4[num9];
					SpawnableObject component6 = m_PrefabSystem.GetPrefab<ObjectPrefab>(nativeArray[num9]).GetComponent<SpawnableObject>();
					if (component6.m_Placeholders != null)
					{
						for (int num10 = 0; num10 < component6.m_Placeholders.Length; num10++)
						{
							ObjectPrefab objectPrefab2 = component6.m_Placeholders[num10];
							if (!((Object)(object)objectPrefab2 == (Object)null) && m_PrefabSystem.TryGetEntity(objectPrefab2, out var entity6))
							{
								EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
								((EntityManager)(ref entityManager)).GetBuffer<PlaceholderObjectElement>(entity6, false).Add(new PlaceholderObjectElement(obj));
							}
						}
					}
					if ((Object)(object)component6.m_RandomizationGroup != (Object)null)
					{
						spawnableObjectData.m_RandomizationGroup = m_PrefabSystem.GetEntity(component6.m_RandomizationGroup);
					}
					spawnableObjectData.m_Probability = component6.m_Probability;
					nativeArray4[num9] = spawnableObjectData;
				}
			}
			JobHandle val8 = default(JobHandle);
			if (flag)
			{
				val8 = JobChunkExtensions.ScheduleParallel<FixPlaceholdersJob>(new FixPlaceholdersJob
				{
					m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PlaceholderObjectElementType = InternalCompilerInterface.GetBufferTypeHandle<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
				}, m_PlaceholderQuery, ((SystemBase)this).Dependency);
			}
			FindPlaceholderRequirementsJob findPlaceholderRequirementsJob = new FindPlaceholderRequirementsJob
			{
				m_Chunks = chunks,
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderObjectElementType = InternalCompilerInterface.GetBufferTypeHandle<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderObjectDataType = InternalCompilerInterface.GetComponentTypeHandle<PlaceholderObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectRequirementElements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			FindSubObjectRequirementsJob findSubObjectRequirementsJob = new FindSubObjectRequirementsJob
			{
				m_Chunks = chunks,
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryDataType = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderObjectData = InternalCompilerInterface.GetComponentLookup<PlaceholderObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			InitializeSubNetsJob obj2 = new InitializeSubNetsJob
			{
				m_Chunks = chunks,
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceableObjectDataType = InternalCompilerInterface.GetComponentTypeHandle<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val9 = IJobParallelForExtensions.Schedule<FindPlaceholderRequirementsJob>(findPlaceholderRequirementsJob, chunks.Length, 1, val8);
			JobHandle val10 = IJobParallelForExtensions.Schedule<FindSubObjectRequirementsJob>(findSubObjectRequirementsJob, chunks.Length, 1, val9);
			JobHandle val11 = IJobParallelForExtensions.Schedule<InitializeSubNetsJob>(obj2, chunks.Length, 1, default(JobHandle));
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(val10, val11);
		}
		finally
		{
			chunks.Dispose(((SystemBase)this).Dependency);
		}
	}

	private void InitializePrefab(AssetStampPrefab stampPrefab, ref AssetStampData assetStampData, ref PlaceableObjectData placeableObjectData, ref ObjectGeometryData objectGeometryData)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		assetStampData.m_Size = new int2(stampPrefab.m_Width, stampPrefab.m_Depth);
		placeableObjectData.m_ConstructionCost = stampPrefab.m_ConstructionCost;
		assetStampData.m_UpKeepCost = stampPrefab.m_UpKeepCost;
		float2 val = float2.op_Implicit(assetStampData.m_Size);
		val *= 8f;
		objectGeometryData.m_MinLod = math.min(objectGeometryData.m_MinLod, RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(val.x, 0f, val.y))));
		objectGeometryData.m_Layers = MeshLayer.Default;
		val -= 0.4f;
		((float3)(ref objectGeometryData.m_Size)).xz = val;
		objectGeometryData.m_Size.y = math.max(objectGeometryData.m_Size.y, 5f);
		((float3)(ref objectGeometryData.m_Bounds.min)).xz = val * -0.5f;
		objectGeometryData.m_Bounds.min.y = math.min(objectGeometryData.m_Bounds.min.y, 0f);
		((float3)(ref objectGeometryData.m_Bounds.max)).xz = val * 0.5f;
		objectGeometryData.m_Bounds.max.y = math.max(objectGeometryData.m_Bounds.max.y, 5f);
	}

	private static MeshGroupFlags GetMeshGroupFlag(ObjectState state, bool inverse)
	{
		if (inverse)
		{
			switch (state)
			{
			case ObjectState.Cold:
				return MeshGroupFlags.RequireWarm;
			case ObjectState.Warm:
				return MeshGroupFlags.RequireCold;
			case ObjectState.Home:
				return MeshGroupFlags.RequireHomeless;
			case ObjectState.Homeless:
				return MeshGroupFlags.RequireHome;
			case ObjectState.Motorcycle:
				return MeshGroupFlags.ForbidMotorcycle;
			}
		}
		else
		{
			switch (state)
			{
			case ObjectState.Cold:
				return MeshGroupFlags.RequireCold;
			case ObjectState.Warm:
				return MeshGroupFlags.RequireWarm;
			case ObjectState.Home:
				return MeshGroupFlags.RequireHome;
			case ObjectState.Homeless:
				return MeshGroupFlags.RequireHomeless;
			case ObjectState.Motorcycle:
				return MeshGroupFlags.RequireMotorcycle;
			}
		}
		return (MeshGroupFlags)0u;
	}

	private void InitializePrefab(ObjectGeometryPrefab objectPrefab, PlaceableObjectData placeableObjectData, ref ObjectGeometryData objectGeometryData, ref GrowthScaleData growthScaleData, ref StackData stackData, ref QuantityObjectData quantityObjectData, ref CreatureData creatureData, DynamicBuffer<SubMesh> meshes, DynamicBuffer<SubMeshGroup> meshGroups, DynamicBuffer<CharacterElement> characterElements, bool isPlantObject, bool isHumanObject, bool isBuildingObject, bool isVehicleObject, bool isCreatureObject)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
		//IL_103f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1040: Unknown result type (might be due to invalid IL or missing references)
		//IL_104a: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1064: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Unknown result type (might be due to invalid IL or missing references)
		//IL_1008: Unknown result type (might be due to invalid IL or missing references)
		//IL_1009: Unknown result type (might be due to invalid IL or missing references)
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1014: Unknown result type (might be due to invalid IL or missing references)
		//IL_102d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10de: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1101: Unknown result type (might be due to invalid IL or missing references)
		//IL_1103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = default(Bounds3);
		((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
		bool flag = false;
		Bounds3 val2 = default(Bounds3);
		Bounds3 val3 = default(Bounds3);
		Bounds3 val4 = default(Bounds3);
		Bounds3 val5 = default(Bounds3);
		Bounds3 val6 = default(Bounds3);
		if (objectPrefab.m_Meshes != null)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = objectPrefab.m_Meshes.Length;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			ObjectMeshInfo objectMeshInfo = null;
			CharacterGroup.Character[] array = null;
			CharacterGroup.OverrideInfo[] array2 = null;
			CharacterGroup.OverrideInfo overrideInfo = null;
			RenderPrefab[] array3 = null;
			List<RenderPrefab> list = null;
			MeshGroupFlags meshGroupFlags = (MeshGroupFlags)0u;
			MeshGroupFlags meshGroupFlags2 = (MeshGroupFlags)0u;
			Entity val7 = Entity.Null;
			DynamicBuffer<AnimationClip> val9 = default(DynamicBuffer<AnimationClip>);
			CharacterStyleData characterStyleData = default(CharacterStyleData);
			while (true)
			{
				RenderPrefab renderPrefab = null;
				EntityManager entityManager;
				while (true)
				{
					if (num4 < num9)
					{
						renderPrefab = ((array3 == null) ? list[num4++] : array3[num4++]);
						break;
					}
					if (num2 < num6)
					{
						CharacterGroup.Character character = array[num2++];
						if ((character.m_Style.m_Gender & creatureData.m_Gender) != creatureData.m_Gender)
						{
							continue;
						}
						val7 = m_PrefabSystem.GetEntity(character.m_Style);
						num4 = 0;
						CharacterElement characterElement = new CharacterElement
						{
							m_Style = val7,
							m_ShapeWeights = RenderingUtils.GetBlendWeights(character.m_Meta.shapeWeights),
							m_TextureWeights = RenderingUtils.GetBlendWeights(character.m_Meta.textureWeights),
							m_OverlayWeights = RenderingUtils.GetBlendWeights(character.m_Meta.overlayWeights),
							m_MaskWeights = RenderingUtils.GetBlendWeights(character.m_Meta.maskWeights)
						};
						entityManager = ((ComponentSystemBase)this).EntityManager;
						characterElement.m_RestPoseClipIndex = ((EntityManager)(ref entityManager)).GetComponentData<CharacterStyleData>(val7).m_RestPoseClipIndex;
						characterElement.m_CorrectiveClipIndex = -1;
						CharacterElement characterElement2 = characterElement;
						if (overrideInfo != null)
						{
							CharacterGroup.Character character2 = overrideInfo.m_Group.m_Characters[num2 - 1];
							if (overrideInfo.m_OverrideShapeWeights)
							{
								characterElement2.m_ShapeWeights = RenderingUtils.GetBlendWeights(character2.m_Meta.shapeWeights);
							}
							if (list == null)
							{
								list = new List<RenderPrefab>();
							}
							else
							{
								list.Clear();
							}
							for (int i = 0; i < character.m_MeshPrefabs.Length; i++)
							{
								RenderPrefab renderPrefab2 = character.m_MeshPrefabs[i];
								if (!renderPrefab2.TryGet<CharacterProperties>(out var component) || (component.m_BodyParts & overrideInfo.m_OverrideBodyParts) == 0)
								{
									list.Add(renderPrefab2);
								}
							}
							for (int j = 0; j < character2.m_MeshPrefabs.Length; j++)
							{
								RenderPrefab renderPrefab3 = character2.m_MeshPrefabs[j];
								if (!renderPrefab3.TryGet<CharacterProperties>(out var component2) || (component2.m_BodyParts & overrideInfo.m_OverrideBodyParts) != 0)
								{
									list.Add(renderPrefab3);
								}
							}
							array3 = null;
							num9 = list.Count;
						}
						else
						{
							array3 = character.m_MeshPrefabs;
							num9 = array3.Length;
						}
						meshGroups.Add(new SubMeshGroup
						{
							m_SubGroupCount = num8,
							m_SubMeshRange = new int2(meshes.Length, meshes.Length + num9),
							m_Flags = (meshGroupFlags2 | GetMeshGroupFlag(objectMeshInfo.m_RequireState, inverse: false))
						});
						characterElements.Add(characterElement2);
						continue;
					}
					if (num3 < num7)
					{
						overrideInfo = array2[num3++];
						num2 = 0;
						meshGroupFlags2 = (meshGroupFlags & ~GetMeshGroupFlag(overrideInfo.m_RequireState, inverse: true)) | GetMeshGroupFlag(overrideInfo.m_RequireState, inverse: false);
						continue;
					}
					if (num >= num5)
					{
						break;
					}
					objectMeshInfo = objectPrefab.m_Meshes[num++];
					array = null;
					if (objectMeshInfo.m_Mesh is RenderPrefab renderPrefab4)
					{
						renderPrefab = renderPrefab4;
						val7 = Entity.Null;
						break;
					}
					if (!(objectMeshInfo.m_Mesh is CharacterGroup characterGroup))
					{
						continue;
					}
					array = characterGroup.m_Characters;
					array2 = characterGroup.m_Overrides;
					overrideInfo = null;
					num2 = 0;
					num6 = array.Length;
					num3 = 0;
					num7 = ((array2 != null) ? array2.Length : 0);
					num8 = 0;
					CharacterGroup.Character[] array4 = array;
					for (int k = 0; k < array4.Length; k++)
					{
						if ((array4[k].m_Style.m_Gender & creatureData.m_Gender) == creatureData.m_Gender)
						{
							num8++;
						}
					}
					meshGroupFlags = (MeshGroupFlags)0u;
					CharacterGroup.OverrideInfo[] array5 = array2;
					foreach (CharacterGroup.OverrideInfo overrideInfo2 in array5)
					{
						meshGroupFlags |= GetMeshGroupFlag(overrideInfo2.m_RequireState, inverse: true);
					}
					meshGroupFlags2 = meshGroupFlags;
				}
				if ((Object)(object)renderPrefab == (Object)null)
				{
					break;
				}
				Entity entity = m_PrefabSystem.GetEntity(renderPrefab);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(entity);
				Bounds3 bounds = renderPrefab.bounds;
				float3 val8 = MathUtils.Size(bounds);
				bounds = ((!objectPrefab.m_Circular && !((quaternion)(ref objectMeshInfo.m_Rotation)).Equals(quaternion.identity)) ? MathUtils.Bounds(MathUtils.Box(bounds, objectMeshInfo.m_Rotation, objectMeshInfo.m_Position)) : (bounds + objectMeshInfo.m_Position));
				SubMeshFlags subMeshFlags = (SubMeshFlags)0u;
				switch (objectMeshInfo.m_RequireState)
				{
				case ObjectState.Child:
					subMeshFlags |= SubMeshFlags.RequireChild;
					val2 |= bounds;
					break;
				case ObjectState.Teen:
					subMeshFlags |= SubMeshFlags.RequireTeen;
					val3 |= bounds;
					break;
				case ObjectState.Adult:
					subMeshFlags |= SubMeshFlags.RequireAdult;
					val4 |= bounds;
					break;
				case ObjectState.Elderly:
					subMeshFlags |= SubMeshFlags.RequireElderly;
					val5 |= bounds;
					break;
				case ObjectState.Dead:
					subMeshFlags |= SubMeshFlags.RequireDead;
					val6 |= bounds;
					break;
				case ObjectState.Stump:
					subMeshFlags |= SubMeshFlags.RequireStump;
					break;
				case ObjectState.Empty:
					subMeshFlags |= SubMeshFlags.RequireEmpty;
					quantityObjectData.m_StepMask |= 1u;
					break;
				case ObjectState.Full:
					subMeshFlags |= SubMeshFlags.RequireFull;
					quantityObjectData.m_StepMask |= 8u;
					break;
				case ObjectState.Clear:
					subMeshFlags |= SubMeshFlags.RequireClear;
					break;
				case ObjectState.Track:
					subMeshFlags |= SubMeshFlags.RequireTrack;
					break;
				case ObjectState.Partial1:
					subMeshFlags |= SubMeshFlags.RequirePartial1;
					quantityObjectData.m_StepMask |= 2u;
					break;
				case ObjectState.Partial2:
					subMeshFlags |= SubMeshFlags.RequirePartial2;
					quantityObjectData.m_StepMask |= 4u;
					break;
				case ObjectState.LefthandTraffic:
					subMeshFlags |= SubMeshFlags.RequireLeftHandTraffic;
					break;
				case ObjectState.RighthandTraffic:
					subMeshFlags |= SubMeshFlags.RequireRightHandTraffic;
					break;
				case ObjectState.Forward:
					subMeshFlags |= SubMeshFlags.RequireForward;
					break;
				case ObjectState.Backward:
					subMeshFlags |= SubMeshFlags.RequireBackward;
					break;
				case ObjectState.Outline:
					subMeshFlags |= SubMeshFlags.OutlineOnly;
					break;
				}
				float renderingSize;
				float metersPerPixel;
				if ((componentData.m_State & (MeshFlags.StackX | MeshFlags.StackY | MeshFlags.StackZ)) != 0)
				{
					StackProperties component3 = renderPrefab.GetComponent<StackProperties>();
					renderingSize = RenderingUtils.GetRenderingSize(val8, component3.m_Direction);
					metersPerPixel = RenderingUtils.GetShadowRenderingSize(val8, component3.m_Direction);
					if ((stackData.m_Direction != StackDirection.None && stackData.m_Direction != component3.m_Direction) || component3.m_Direction == StackDirection.None)
					{
						COSystemBase.baseLog.WarnFormat((Object)(object)objectPrefab, "{0}: Stack direction mismatch ({1})", (object)((Object)objectPrefab).name, (object)((Object)renderPrefab).name);
					}
					else
					{
						stackData.m_Direction = component3.m_Direction;
					}
					switch (component3.m_Order)
					{
					case StackOrder.First:
						subMeshFlags |= SubMeshFlags.IsStackStart;
						stackData.m_DontScale.x |= component3.m_ForbidScaling;
						UpdateStackBounds(ref stackData.m_FirstBounds, ref bounds, component3);
						break;
					case StackOrder.Middle:
						subMeshFlags |= SubMeshFlags.IsStackMiddle;
						stackData.m_DontScale.y |= component3.m_ForbidScaling;
						UpdateStackBounds(ref stackData.m_MiddleBounds, ref bounds, component3);
						break;
					case StackOrder.Last:
						subMeshFlags |= SubMeshFlags.IsStackEnd;
						stackData.m_DontScale.z |= component3.m_ForbidScaling;
						if (stackData.m_Direction == StackDirection.Up && (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							bounds.max.y = math.max(bounds.max.y, objectGeometryData.m_LegSize.y + 0.1f);
						}
						UpdateStackBounds(ref stackData.m_LastBounds, ref bounds, component3);
						break;
					}
				}
				else
				{
					if (renderPrefab.surfaceArea <= 0f)
					{
						if (isHumanObject)
						{
							val8.x /= 3f;
						}
						else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							((float3)(ref val8)).xz = math.lerp(((float3)(ref val8)).xz, math.min(((float3)(ref val8)).xz, ((float3)(ref objectGeometryData.m_LegSize)).xz + objectGeometryData.m_LegOffset * 2f), math.saturate(objectGeometryData.m_LegSize.y / math.max(0.001f, val8.y)));
						}
					}
					val8 = math.min(val8, math.max(math.min(((float3)(ref val8)).yzx, ((float3)(ref val8)).zxy) * 8f, math.max(((float3)(ref val8)).yzx, ((float3)(ref val8)).zxy) * 4f));
					renderingSize = RenderingUtils.GetRenderingSize(val8);
					metersPerPixel = renderingSize;
				}
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
				{
					bounds.max.y = math.max(bounds.max.y, objectGeometryData.m_LegSize.y + 0.1f);
				}
				componentData.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(renderingSize, componentData.m_LodBias);
				componentData.m_ShadowLod = (byte)RenderingUtils.CalculateLodLimit(metersPerPixel, componentData.m_ShadowBias);
				MeshLayer meshLayer = ((componentData.m_DefaultLayers == (MeshLayer)0) ? MeshLayer.Default : componentData.m_DefaultLayers);
				objectGeometryData.m_Layers |= meshLayer;
				if (!((float3)(ref objectMeshInfo.m_Position)).Equals(default(float3)) || !((quaternion)(ref objectMeshInfo.m_Rotation)).Equals(quaternion.identity))
				{
					subMeshFlags |= SubMeshFlags.HasTransform;
				}
				ushort randomSeed = (ushort)num4;
				if (array != null)
				{
					randomSeed = GetRandomSeed(((Object)renderPrefab).name);
				}
				meshes.Add(new SubMesh(entity, objectMeshInfo.m_Position, objectMeshInfo.m_Rotation, subMeshFlags, randomSeed));
				flag |= (componentData.m_State & MeshFlags.Decal) == 0;
				if (array == null || num4 == 1)
				{
					val |= bounds;
					objectGeometryData.m_MinLod = math.min(objectGeometryData.m_MinLod, (int)componentData.m_MinLod);
				}
				else
				{
					componentData.m_MinLod = (byte)math.max((int)componentData.m_MinLod, objectGeometryData.m_MinLod);
				}
				if (EntitiesExtensions.TryGetBuffer<AnimationClip>(((ComponentSystemBase)this).EntityManager, entity, false, ref val9))
				{
					float num10 = float.MaxValue;
					float num11 = 0f;
					for (int l = 0; l < val9.Length; l++)
					{
						AnimationClip animationClip = val9[l];
						if (animationClip.m_Type == AnimationType.Move)
						{
							switch (animationClip.m_Activity)
							{
							case ActivityType.Walking:
								num10 = animationClip.m_MovementSpeed;
								break;
							case ActivityType.Running:
								num11 = animationClip.m_MovementSpeed;
								break;
							}
						}
						creatureData.m_SupportedActivities.m_Mask |= new ActivityMask(animationClip.m_Activity).m_Mask;
					}
					for (int m = 0; m < val9.Length; m++)
					{
						AnimationClip animationClip2 = val9[m];
						if (animationClip2.m_Type == AnimationType.Move)
						{
							animationClip2.m_SpeedRange = new Bounds1(0f, float.MaxValue);
							switch (animationClip2.m_Activity)
							{
							case ActivityType.Walking:
								animationClip2.m_SpeedRange.max = math.select((num10 + num11) * 0.5f, float.MaxValue, num11 <= num10);
								break;
							case ActivityType.Running:
								animationClip2.m_SpeedRange.min = math.select((num10 + num11) * 0.5f, 0f, num10 >= num11);
								break;
							}
							val9[m] = animationClip2;
						}
					}
				}
				else if (EntitiesExtensions.TryGetComponent<CharacterStyleData>(((ComponentSystemBase)this).EntityManager, val7, ref characterStyleData))
				{
					creatureData.m_SupportedActivities.m_Mask |= characterStyleData.m_ActivityMask.m_Mask;
					if (renderPrefab.TryGet<CharacterProperties>(out var component4) && !string.IsNullOrEmpty(component4.m_CorrectiveAnimationName))
					{
						CharacterStyle prefab = m_PrefabSystem.GetPrefab<CharacterStyle>(val7);
						ref CharacterElement reference = ref characterElements.ElementAt(characterElements.Length - 1);
						for (int n = 0; n < prefab.m_Animations.Length; n++)
						{
							if (prefab.m_Animations[n].name == component4.m_CorrectiveAnimationName)
							{
								reference.m_CorrectiveClipIndex = n;
								break;
							}
						}
					}
				}
				if (isBuildingObject)
				{
					componentData.m_DecalLayer |= DecalLayers.Buildings;
				}
				if (isVehicleObject)
				{
					componentData.m_DecalLayer |= DecalLayers.Vehicles;
				}
				if (isCreatureObject)
				{
					componentData.m_DecalLayer |= DecalLayers.Creatures;
				}
				bool flag2 = renderPrefab.TryGet<BaseProperties>(out var component5) && (Object)(object)component5.m_BaseType != (Object)null;
				flag2 |= isBuildingObject && (Object)(object)component5 == (Object)null;
				if (flag2 && (componentData.m_State & (MeshFlags.Decal | MeshFlags.Impostor)) == 0)
				{
					componentData.m_State |= MeshFlags.Base;
				}
				if ((componentData.m_State & MeshFlags.Base) != 0)
				{
					objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.HasBase;
				}
				if (array != null)
				{
					componentData.m_State |= MeshFlags.Character;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<MeshData>(entity, componentData);
				if (!(isBuildingObject || isVehicleObject || isCreatureObject || flag2) || !renderPrefab.TryGet<LodProperties>(out var component6) || component6.m_LodMeshes == null)
				{
					continue;
				}
				for (int num12 = 0; num12 < component6.m_LodMeshes.Length; num12++)
				{
					if (!((Object)(object)component6.m_LodMeshes[num12] == (Object)null))
					{
						Entity entity2 = m_PrefabSystem.GetEntity(component6.m_LodMeshes[num12]);
						entityManager = ((ComponentSystemBase)this).EntityManager;
						MeshData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(entity2);
						if (isBuildingObject)
						{
							componentData2.m_DecalLayer |= DecalLayers.Buildings;
						}
						if (isVehicleObject)
						{
							componentData2.m_DecalLayer |= DecalLayers.Vehicles;
						}
						if (isCreatureObject)
						{
							componentData2.m_DecalLayer |= DecalLayers.Creatures;
						}
						if (flag2 && (componentData2.m_State & (MeshFlags.Decal | MeshFlags.Impostor)) == 0)
						{
							componentData2.m_State |= componentData.m_State & (MeshFlags.Base | MeshFlags.MinBounds);
						}
						if (array != null)
						{
							componentData2.m_State |= MeshFlags.Character;
						}
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<MeshData>(entity2, componentData2);
					}
				}
			}
		}
		if (val.min.x > val.max.x)
		{
			val = default(Bounds3);
		}
		objectGeometryData.m_Bounds = val;
		objectGeometryData.m_Size = ObjectUtils.GetSize(val);
		if (isPlantObject)
		{
			float num13 = 0.5f;
			if (objectPrefab.TryGet<PlantObject>(out var component7))
			{
				num13 = math.min(num13, 1f - component7.m_PotCoverage);
			}
			float3 val10 = default(float3);
			((float3)(ref val10)).xz = ((float3)(ref objectGeometryData.m_Size)).xz * num13;
			val10.y = math.min(objectGeometryData.m_Size.y * num13, math.cmin(((float3)(ref val10)).xz) * 0.25f);
			ref float3 size = ref objectGeometryData.m_Size;
			size -= val10;
			((float3)(ref objectGeometryData.m_Bounds.min)).xz = math.max(((float3)(ref objectGeometryData.m_Bounds.min)).xz, ((float3)(ref objectGeometryData.m_Size)).xz * -0.5f);
			((float3)(ref objectGeometryData.m_Bounds.max)).xz = math.min(((float3)(ref objectGeometryData.m_Bounds.max)).xz, ((float3)(ref objectGeometryData.m_Size)).xz * 0.5f);
			objectGeometryData.m_Bounds.max.y = objectGeometryData.m_Size.y;
		}
		else if (isHumanObject)
		{
			objectGeometryData.m_Size.x = math.max(objectGeometryData.m_Size.z, objectGeometryData.m_Size.x / 3f);
			objectGeometryData.m_Bounds.min.x = objectGeometryData.m_Size.x * -0.5f;
			objectGeometryData.m_Bounds.max.x = objectGeometryData.m_Size.x * 0.5f;
		}
		if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Wall) != Game.Objects.PlacementFlags.None)
		{
			objectGeometryData.m_Pivot = default(float3);
		}
		else if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hanging) != Game.Objects.PlacementFlags.None)
		{
			objectGeometryData.m_Pivot = new float3(0f, math.lerp(val.min.y, val.max.y, 0.9f), 0f);
		}
		else
		{
			objectGeometryData.m_Pivot = new float3(0f, math.lerp(val.min.y, val.max.y, 0.25f), 0f);
		}
		if (objectPrefab.m_Circular)
		{
			objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.Circular;
			((float3)(ref objectGeometryData.m_Size)).xz = float2.op_Implicit(math.max(objectGeometryData.m_Size.x, objectGeometryData.m_Size.z));
		}
		if (flag)
		{
			objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.Physical;
		}
		else
		{
			objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.WalkThrough;
		}
		growthScaleData.m_ChildSize = ObjectUtils.GetSize(val2);
		growthScaleData.m_TeenSize = ObjectUtils.GetSize(val3);
		growthScaleData.m_AdultSize = ObjectUtils.GetSize(val4);
		growthScaleData.m_ElderlySize = ObjectUtils.GetSize(val5);
		growthScaleData.m_DeadSize = ObjectUtils.GetSize(val6);
		if (!meshGroups.IsCreated)
		{
			return;
		}
		MeshGroupFlags meshGroupFlags3 = (MeshGroupFlags)0u;
		for (int num14 = 0; num14 < meshGroups.Length; num14++)
		{
			meshGroupFlags3 |= meshGroups[num14].m_Flags;
		}
		if ((meshGroupFlags3 & (MeshGroupFlags.RequireHome | MeshGroupFlags.RequireHomeless)) != MeshGroupFlags.RequireHomeless)
		{
			return;
		}
		for (int num15 = 0; num15 < meshGroups.Length; num15++)
		{
			ref SubMeshGroup reference2 = ref meshGroups.ElementAt(num15);
			if ((reference2.m_Flags & (MeshGroupFlags.RequireCold | MeshGroupFlags.RequireWarm)) != 0)
			{
				reference2.m_Flags |= MeshGroupFlags.RequireHome;
			}
		}
	}

	private ushort GetRandomSeed(string name)
	{
		uint num = 0u;
		for (int i = 0; i < name.Length; i++)
		{
			num = (num << 1) ^ name[i];
		}
		return (ushort)num;
	}

	private void UpdateStackBounds(ref Bounds1 stackBounds, ref Bounds3 meshBounds, StackProperties properties)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		switch (properties.m_Direction)
		{
		case StackDirection.Right:
			stackBounds |= new Bounds1(meshBounds.min.x + properties.m_StartOverlap, meshBounds.max.x - properties.m_EndOverlap);
			meshBounds.min.x = ((properties.m_Order == StackOrder.First) ? math.min(meshBounds.min.x, 0f) : 0f);
			meshBounds.max.x = ((properties.m_Order == StackOrder.Last) ? math.max(meshBounds.max.x, 0f) : 0f);
			break;
		case StackDirection.Up:
			stackBounds |= new Bounds1(meshBounds.min.y + properties.m_StartOverlap, meshBounds.max.y - properties.m_EndOverlap);
			meshBounds.min.y = ((properties.m_Order == StackOrder.First) ? math.min(meshBounds.min.y, 0f) : 0f);
			meshBounds.max.y = ((properties.m_Order == StackOrder.Last) ? math.max(meshBounds.max.y, 0f) : 0f);
			break;
		case StackDirection.Forward:
			stackBounds |= new Bounds1(meshBounds.min.z + properties.m_StartOverlap, meshBounds.max.z - properties.m_EndOverlap);
			meshBounds.min.z = ((properties.m_Order == StackOrder.First) ? math.min(meshBounds.min.z, 0f) : 0f);
			meshBounds.max.z = ((properties.m_Order == StackOrder.Last) ? math.max(meshBounds.max.z, 0f) : 0f);
			break;
		}
	}

	private void InitializePrefab(MarkerObjectPrefab objectPrefab, PlaceableObjectData placeableObjectData, ref ObjectGeometryData objectGeometryData, DynamicBuffer<SubMesh> meshes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = default(Bounds3);
		if ((Object)(object)objectPrefab.m_Mesh != (Object)null)
		{
			Entity entity = m_PrefabSystem.GetEntity(objectPrefab.m_Mesh);
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(entity);
			Bounds3 bounds = objectPrefab.m_Mesh.bounds;
			float renderingSize = RenderingUtils.GetRenderingSize(MathUtils.Size(bounds));
			componentData.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(renderingSize, componentData.m_LodBias);
			componentData.m_ShadowLod = (byte)RenderingUtils.CalculateLodLimit(renderingSize, componentData.m_ShadowBias);
			objectGeometryData.m_MinLod = math.min(objectGeometryData.m_MinLod, (int)componentData.m_MinLod);
			objectGeometryData.m_Layers = ((componentData.m_DefaultLayers == (MeshLayer)0) ? MeshLayer.Default : componentData.m_DefaultLayers);
			val |= bounds;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<MeshData>(entity, componentData);
			meshes.Add(new SubMesh(entity, (SubMeshFlags)0u, 0));
		}
		objectGeometryData.m_Bounds = val;
		objectGeometryData.m_Size = ObjectUtils.GetSize(val);
		if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Wall) != Game.Objects.PlacementFlags.None)
		{
			objectGeometryData.m_Pivot = default(float3);
		}
		else if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hanging) != Game.Objects.PlacementFlags.None)
		{
			objectGeometryData.m_Pivot = new float3(0f, math.lerp(val.min.y, val.max.y, 0.9f), 0f);
		}
		else
		{
			objectGeometryData.m_Pivot = new float3(0f, math.lerp(val.min.y, val.max.y, 0.25f), 0f);
		}
		if (objectPrefab.m_Circular)
		{
			objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.Circular;
			((float3)(ref objectGeometryData.m_Size)).xz = float2.op_Implicit(math.max(objectGeometryData.m_Size.x, objectGeometryData.m_Size.z));
		}
		objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.Marker;
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
	public ObjectInitializeSystem()
	{
	}
}
