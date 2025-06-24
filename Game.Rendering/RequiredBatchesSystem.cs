using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class RequiredBatchesSystem : GameSystemBase
{
	[BurstCompile]
	private struct RequiredBatchesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> m_StoppedType;

		[ReadOnly]
		public ComponentTypeHandle<Object> m_ObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> m_ObjectMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> m_UtilityLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Marker> m_NetMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_BlockType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentTypeHandle<Warning> m_WarningType;

		[ReadOnly]
		public ComponentTypeHandle<Override> m_OverrideType;

		[ReadOnly]
		public ComponentTypeHandle<Highlighted> m_HighlightedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubMesh> m_PrefabSubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_PrefabSubMeshGroups;

		[ReadOnly]
		public BufferLookup<LodMesh> m_PrefabLodMeshes;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> m_PrefabCompositionMeshRef;

		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public BufferLookup<MeshMaterial> m_PrefabMeshMaterials;

		public ComponentLookup<NetCompositionMeshData> m_PrefabCompositionMeshData;

		public ComponentLookup<ZoneBlockData> m_PrefabZoneBlockData;

		public BufferLookup<BatchGroup> m_PrefabBatchGroups;

		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Object>(ref m_ObjectType))
			{
				UpdateObjectBatches(chunk);
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Composition>(ref m_CompositionType) || ((ArchetypeChunk)(ref chunk)).Has<Orphan>(ref m_OrphanType))
			{
				UpdateNetBatches(chunk);
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Lane>(ref m_LaneType))
			{
				UpdateLaneBatches(chunk);
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Block>(ref m_BlockType))
			{
				UpdateZoneBatches(chunk);
			}
		}

		private void UpdateObjectBatches(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Game.Objects.Elevation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.Elevation>(ref m_ElevationType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<MeshGroup> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<InterpolatedTransform>(ref m_InterpolatedTransformType) || ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref m_StoppedType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Marker>(ref m_ObjectMarkerType);
			MeshLayer meshLayer = MeshLayer.Default;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType) || ((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType) || ((ArchetypeChunk)(ref chunk)).Has<Override>(ref m_OverrideType) || ((ArchetypeChunk)(ref chunk)).Has<Highlighted>(ref m_HighlightedType))
			{
				meshLayer |= MeshLayer.Outline;
			}
			DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
			MeshGroup meshGroup = default(MeshGroup);
			SubMeshGroup subMeshGroup = default(SubMeshGroup);
			Owner owner = default(Owner);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				PrefabRef prefabRef = nativeArray4[i];
				if (!m_PrefabSubMeshes.HasBuffer(prefabRef.m_Prefab))
				{
					continue;
				}
				DynamicBuffer<SubMesh> val = m_PrefabSubMeshes[prefabRef.m_Prefab];
				MeshLayer meshLayer2 = meshLayer;
				if (nativeArray3.Length != 0)
				{
					Temp temp = nativeArray3[i];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
					{
						meshLayer2 |= MeshLayer.Outline;
					}
				}
				if (flag2)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Marker;
				}
				else if (flag)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Moving;
				}
				else if (nativeArray2.Length != 0 && nativeArray2[i].m_Elevation < 0f)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Tunnel;
				}
				DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
				int num = 1;
				if (m_PrefabSubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val3) && CollectionUtils.TryGet<MeshGroup>(bufferAccessor, i, ref val2))
				{
					num = val2.Length;
				}
				for (int j = 0; j < num; j++)
				{
					if (val3.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val2, j, ref meshGroup);
						subMeshGroup = val3[(int)meshGroup.m_SubMeshGroup];
					}
					else
					{
						subMeshGroup.m_SubMeshRange = new int2(0, val.Length);
					}
					for (int k = subMeshGroup.m_SubMeshRange.x; k < subMeshGroup.m_SubMeshRange.y; k++)
					{
						SubMesh subMesh = val[k];
						MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
						MeshLayer meshLayer3 = meshLayer2;
						if ((meshData.m_DefaultLayers != 0 && (meshLayer2 & (MeshLayer.Moving | MeshLayer.Marker)) == 0) || (meshData.m_DefaultLayers & (MeshLayer.Pipeline | MeshLayer.SubPipeline)) != 0)
						{
							meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Moving | MeshLayer.Tunnel | MeshLayer.Marker);
							CollectionUtils.TryGet<Owner>(nativeArray, i, ref owner);
							meshLayer3 |= Game.Net.SearchSystem.GetLayers(owner, default(Game.Net.UtilityLane), meshData.m_DefaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData);
						}
						MeshLayer meshLayer4 = (MeshLayer)((uint)meshLayer3 & (uint)(ushort)(~(int)meshData.m_AvailableLayers));
						MeshType meshType = (MeshType)(1 & (ushort)(~(int)meshData.m_AvailableTypes));
						if (meshLayer4 != 0 || meshType != 0)
						{
							meshData.m_AvailableLayers |= meshLayer4;
							meshData.m_AvailableTypes |= meshType;
							m_PrefabMeshData[subMesh.m_SubMesh] = meshData;
							InitializeBatchGroups(subMesh.m_SubMesh, meshLayer4, meshType, meshData.m_AvailableLayers, meshData.m_AvailableTypes, isNewPartition: false, 0);
						}
					}
				}
			}
		}

		private void UpdateNetBatches(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Composition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<Orphan> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Marker>(ref m_NetMarkerType);
			MeshLayer meshLayer = MeshLayer.Default;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType) || ((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType) || ((ArchetypeChunk)(ref chunk)).Has<Highlighted>(ref m_HighlightedType))
			{
				meshLayer |= MeshLayer.Outline;
			}
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				MeshLayer meshLayer2 = meshLayer;
				if (nativeArray3.Length != 0)
				{
					Temp temp = nativeArray3[i];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
					{
						meshLayer2 |= MeshLayer.Outline;
					}
				}
				if (flag)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Marker;
				}
				if (nativeArray.Length != 0)
				{
					Composition composition = nativeArray[i];
					UpdateNetBatches(composition.m_Edge, meshLayer2);
					UpdateNetBatches(composition.m_StartNode, meshLayer2);
					UpdateNetBatches(composition.m_EndNode, meshLayer2);
				}
				else if (nativeArray2.Length != 0)
				{
					UpdateNetBatches(nativeArray2[i].m_Composition, meshLayer2);
				}
			}
		}

		private void UpdateNetBatches(Entity composition, MeshLayer requiredLayers)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionMeshRef netCompositionMeshRef = default(NetCompositionMeshRef);
			NetCompositionMeshData netCompositionMeshData = default(NetCompositionMeshData);
			if (m_PrefabCompositionMeshRef.TryGetComponent(composition, ref netCompositionMeshRef) && m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef.m_Mesh, ref netCompositionMeshData))
			{
				MeshLayer meshLayer = requiredLayers;
				if (netCompositionMeshData.m_DefaultLayers != 0 && (requiredLayers & MeshLayer.Marker) == 0)
				{
					meshLayer &= ~MeshLayer.Default;
					meshLayer |= netCompositionMeshData.m_DefaultLayers;
				}
				MeshLayer meshLayer2 = (MeshLayer)((uint)meshLayer & (uint)(ushort)(~(int)netCompositionMeshData.m_AvailableLayers));
				if (meshLayer2 != 0)
				{
					netCompositionMeshData.m_AvailableLayers |= meshLayer2;
					m_PrefabCompositionMeshData[netCompositionMeshRef.m_Mesh] = netCompositionMeshData;
					InitializeBatchGroups(netCompositionMeshRef.m_Mesh, meshLayer2, (MeshType)0, netCompositionMeshData.m_AvailableLayers, MeshType.Net, isNewPartition: false, 0);
				}
			}
		}

		private void UpdateLaneBatches(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Game.Net.UtilityLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.UtilityLane>(ref m_UtilityLaneType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			MeshLayer meshLayer = MeshLayer.Default;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType) || ((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType) || ((ArchetypeChunk)(ref chunk)).Has<Highlighted>(ref m_HighlightedType))
			{
				meshLayer |= MeshLayer.Outline;
			}
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			Owner owner2 = default(Owner);
			Game.Net.UtilityLane utilityLane = default(Game.Net.UtilityLane);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				PrefabRef prefabRef = nativeArray4[i];
				if (!m_PrefabSubMeshes.HasBuffer(prefabRef.m_Prefab))
				{
					continue;
				}
				DynamicBuffer<SubMesh> val = m_PrefabSubMeshes[prefabRef.m_Prefab];
				MeshLayer meshLayer2 = meshLayer;
				if (nativeArray3.Length != 0)
				{
					Temp temp = nativeArray3[i];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
					{
						meshLayer2 |= MeshLayer.Outline;
					}
				}
				if (nativeArray.Length != 0)
				{
					Owner owner = nativeArray[i];
					if (IsNetOwnerTunnel(owner))
					{
						meshLayer2 &= ~MeshLayer.Default;
						meshLayer2 |= MeshLayer.Tunnel;
					}
				}
				int num = 256;
				if (nativeArray2.Length != 0 && m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & ~(UtilityTypes.StormwaterPipe | UtilityTypes.Fence | UtilityTypes.Catenary)) != UtilityTypes.None)
				{
					num = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(utilityLaneData.m_VisualCapacity)));
				}
				for (int j = 0; j < val.Length; j++)
				{
					SubMesh subMesh = val[j];
					MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
					MeshLayer meshLayer3 = meshLayer2;
					if ((subMesh.m_Flags & SubMeshFlags.RequireEditor) != 0)
					{
						meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Tunnel);
						meshLayer3 |= MeshLayer.Marker;
					}
					if ((meshData.m_DefaultLayers != 0 && (meshLayer3 & MeshLayer.Marker) == 0) || (meshData.m_DefaultLayers & (MeshLayer.Pipeline | MeshLayer.SubPipeline)) != 0)
					{
						meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Tunnel | MeshLayer.Marker);
						CollectionUtils.TryGet<Owner>(nativeArray, i, ref owner2);
						CollectionUtils.TryGet<Game.Net.UtilityLane>(nativeArray2, i, ref utilityLane);
						meshLayer3 |= Game.Net.SearchSystem.GetLayers(owner2, utilityLane, meshData.m_DefaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData);
					}
					MeshLayer meshLayer4 = (MeshLayer)((uint)meshLayer3 & (uint)(ushort)(~(int)meshData.m_AvailableLayers));
					MeshType meshType = (MeshType)(4 & (ushort)(~(int)meshData.m_AvailableTypes));
					if (meshLayer4 != 0 || meshType != 0)
					{
						meshData.m_AvailableLayers |= meshLayer4;
						meshData.m_AvailableTypes |= meshType;
						m_PrefabMeshData[subMesh.m_SubMesh] = meshData;
						InitializeBatchGroups(subMesh.m_SubMesh, meshLayer4, meshType, meshData.m_AvailableLayers, meshData.m_AvailableTypes, isNewPartition: false, meshData.m_MinLod);
						if (num < meshData.m_MinLod)
						{
							InitializeBatchGroups(subMesh.m_SubMesh, meshLayer4, meshType, meshData.m_AvailableLayers, meshData.m_AvailableTypes, isNewPartition: false, (ushort)num);
						}
					}
				}
			}
		}

		private void UpdateZoneBatches(ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Block> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Block>(ref m_BlockType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Block block = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				ZoneBlockData zoneBlockData = m_PrefabZoneBlockData[prefabRef.m_Prefab];
				ushort num = (ushort)math.clamp(block.m_Size.x * block.m_Size.y - 1 >> 4, 0, 3);
				MeshLayer meshLayer = (MeshLayer)(1 & (ushort)(~(int)zoneBlockData.m_AvailableLayers));
				ushort num2 = (ushort)((1 << (int)num) & ~zoneBlockData.m_AvailablePartitions);
				if (meshLayer != 0 || num2 != 0)
				{
					zoneBlockData.m_AvailableLayers |= meshLayer;
					zoneBlockData.m_AvailablePartitions |= num2;
					m_PrefabZoneBlockData[prefabRef.m_Prefab] = zoneBlockData;
					InitializeBatchGroups(prefabRef.m_Prefab, meshLayer, (MeshType)0, zoneBlockData.m_AvailableLayers, MeshType.Zone, num2 != 0, num);
				}
			}
		}

		private void InitializeBatchGroups(Entity mesh, MeshLayer newLayers, MeshType newTypes, MeshLayer allLayers, MeshType allTypes, bool isNewPartition, ushort partition)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			if (newLayers != 0)
			{
				MeshLayer meshLayer = MeshLayer.Default;
				while ((int)meshLayer <= 128)
				{
					if ((newLayers & meshLayer) != 0)
					{
						MeshType meshType = MeshType.Object;
						while ((int)meshType <= 8)
						{
							if ((allTypes & meshType) != 0)
							{
								InitializeBatchGroup(mesh, meshLayer, meshType, partition);
							}
							meshType = (MeshType)((uint)meshType << 1);
						}
					}
					meshLayer = (MeshLayer)((uint)meshLayer << 1);
				}
				allLayers = (MeshLayer)((uint)allLayers & (uint)(ushort)(~(int)newLayers));
			}
			if (newTypes != 0)
			{
				MeshType meshType2 = MeshType.Object;
				while ((int)meshType2 <= 8)
				{
					if ((newTypes & meshType2) != 0)
					{
						MeshLayer meshLayer2 = MeshLayer.Default;
						while ((int)meshLayer2 <= 128)
						{
							if ((allLayers & meshLayer2) != 0)
							{
								InitializeBatchGroup(mesh, meshLayer2, meshType2, partition);
							}
							meshLayer2 = (MeshLayer)((uint)meshLayer2 << 1);
						}
					}
					meshType2 = (MeshType)((uint)meshType2 << 1);
				}
				allTypes = (MeshType)((uint)allTypes & (uint)(ushort)(~(int)newTypes));
			}
			if (!isNewPartition)
			{
				return;
			}
			MeshLayer meshLayer3 = MeshLayer.Default;
			while ((int)meshLayer3 <= 128)
			{
				if ((allLayers & meshLayer3) != 0)
				{
					MeshType meshType3 = MeshType.Object;
					while ((int)meshType3 <= 8)
					{
						if ((allTypes & meshType3) != 0)
						{
							InitializeBatchGroup(mesh, meshLayer3, meshType3, partition);
						}
						meshType3 = (MeshType)((uint)meshType3 << 1);
					}
				}
				meshLayer3 = (MeshLayer)((uint)meshLayer3 << 1);
			}
		}

		private void InitializeBatchGroup(Entity mesh, MeshLayer layer, MeshType type, ushort partition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LodMesh> val;
			Bounds3 val2 = default(Bounds3);
			MeshFlags meshFlags;
			int num;
			float num2;
			int num3;
			float bias;
			float bias2;
			int num4;
			int num5;
			switch (type)
			{
			case MeshType.Zone:
				val = default(DynamicBuffer<LodMesh>);
				val2 = ZoneMeshHelpers.GetBounds(new int2(10, 6));
				meshFlags = (MeshFlags)0u;
				num = 1;
				num2 = ZoneMeshHelpers.GetIndexCount(new int2(10, 6));
				num3 = 1;
				num4 = 0;
				num5 = 0;
				bias = 0f;
				bias2 = 0f;
				break;
			case MeshType.Net:
			{
				NetCompositionMeshData netCompositionMeshData = m_PrefabCompositionMeshData[mesh];
				val = default(DynamicBuffer<LodMesh>);
				((Bounds3)(ref val2))._002Ector(new float3(netCompositionMeshData.m_Width * -0.5f, netCompositionMeshData.m_HeightRange.min, 0f), new float3(netCompositionMeshData.m_Width * 0.5f, netCompositionMeshData.m_HeightRange.max, 0f));
				meshFlags = (MeshFlags)0u;
				num = m_PrefabMeshMaterials[mesh].Length;
				num2 = netCompositionMeshData.m_IndexFactor;
				num3 = 0;
				num4 = 0;
				num5 = 0;
				bias = netCompositionMeshData.m_LodBias;
				bias2 = netCompositionMeshData.m_ShadowBias;
				if (m_PrefabLodMeshes.HasBuffer(mesh))
				{
					val = m_PrefabLodMeshes[mesh];
					num3 = val.Length;
				}
				break;
			}
			default:
			{
				MeshData meshData = m_PrefabMeshData[mesh];
				val = default(DynamicBuffer<LodMesh>);
				val2 = RenderingUtils.SafeBounds(meshData.m_Bounds);
				meshFlags = meshData.m_State;
				num = math.select(meshData.m_SubMeshCount, meshData.m_SubMeshCount + 1, (meshFlags & MeshFlags.Base) != 0);
				num2 = meshData.m_IndexCount;
				num3 = 0;
				num4 = ((type == MeshType.Lane) ? partition : meshData.m_MinLod);
				num5 = meshData.m_ShadowLod;
				bias = meshData.m_LodBias;
				bias2 = meshData.m_ShadowBias;
				if (m_PrefabLodMeshes.HasBuffer(mesh))
				{
					val = m_PrefabLodMeshes[mesh];
					num3 = val.Length;
				}
				break;
			}
			}
			DynamicBuffer<BatchGroup> val3 = m_PrefabBatchGroups[mesh];
			int num6 = num;
			if (val.IsCreated)
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (type == MeshType.Net)
					{
						num6 += m_PrefabMeshMaterials[val[i].m_LodMesh].Length;
						continue;
					}
					MeshData meshData2 = m_PrefabMeshData[val[i].m_LodMesh];
					num6 += meshData2.m_SubMeshCount;
					num6 = math.select(num6, num6 + 1, (meshData2.m_State & MeshFlags.Base) != 0);
				}
			}
			float3 secondaryCenter = MathUtils.Center(val2);
			float3 val4 = MathUtils.Size(val2);
			GroupData groupData = new GroupData
			{
				m_Mesh = mesh,
				m_SecondaryCenter = secondaryCenter,
				m_SecondarySize = val4 * 0.4f,
				m_Layer = layer,
				m_MeshType = type,
				m_Partition = partition,
				m_LodCount = (byte)num3
			};
			for (int j = 0; j < 16; j++)
			{
				groupData.SetPropertyIndex(j, -1);
			}
			int num7 = m_NativeBatchGroups.CreateGroup(groupData, num6, m_NativeBatchInstances, m_NativeSubBatches);
			val3.Add(new BatchGroup
			{
				m_GroupIndex = num7,
				m_MergeIndex = -1,
				m_Layer = layer,
				m_Type = type,
				m_Partition = partition
			});
			StackDirection stackDirection = StackDirection.None;
			if ((meshFlags & MeshFlags.StackX) != 0)
			{
				stackDirection = StackDirection.Right;
			}
			if ((meshFlags & MeshFlags.StackY) != 0)
			{
				stackDirection = StackDirection.Up;
			}
			if ((meshFlags & MeshFlags.StackZ) != 0)
			{
				stackDirection = StackDirection.Forward;
			}
			float metersPerPixel = 0f;
			switch (type)
			{
			case MeshType.Object:
				metersPerPixel = RenderingUtils.GetRenderingSize(val4, stackDirection);
				break;
			case MeshType.Net:
				metersPerPixel = RenderingUtils.GetRenderingSize(((float3)(ref val4)).xy);
				num4 = RenderingUtils.CalculateLodLimit(metersPerPixel, bias);
				num5 = RenderingUtils.CalculateLodLimit(RenderingUtils.GetShadowRenderingSize(((float3)(ref val4)).xy), bias2);
				break;
			case MeshType.Lane:
				metersPerPixel = RenderingUtils.GetRenderingSize(((float3)(ref val4)).xy);
				break;
			case MeshType.Zone:
				metersPerPixel = RenderingUtils.GetRenderingSize(val4);
				num4 = RenderingUtils.CalculateLodLimit(metersPerPixel, bias);
				num5 = RenderingUtils.CalculateLodLimit(metersPerPixel, bias2);
				break;
			}
			num4 = math.min(num4, 255 - num3);
			num5 = math.clamp(num5, num4, 255);
			for (int num8 = num3 - 1; num8 >= 0; num8--)
			{
				Entity val5;
				Bounds3 val6;
				int num9;
				float num10;
				switch (type)
				{
				case MeshType.Zone:
					val5 = Entity.Null;
					val6 = val2;
					num9 = 1;
					num10 = ZoneMeshHelpers.GetIndexCount(new int2(5, 3));
					break;
				case MeshType.Net:
				{
					val5 = val[num8].m_LodMesh;
					NetCompositionMeshData netCompositionMeshData2 = m_PrefabCompositionMeshData[val5];
					val6 = val2;
					num9 = m_PrefabMeshMaterials[val5].Length;
					num10 = netCompositionMeshData2.m_IndexFactor;
					break;
				}
				default:
				{
					val5 = val[num8].m_LodMesh;
					MeshData meshData3 = m_PrefabMeshData[val5];
					val6 = meshData3.m_Bounds;
					num9 = math.select(meshData3.m_SubMeshCount, meshData3.m_SubMeshCount + 1, (meshData3.m_State & MeshFlags.Base) != 0);
					num10 = math.select((float)meshData3.m_IndexCount, num2 * 0.25f, (meshData3.m_State & (MeshFlags.Decal | MeshFlags.Impostor)) != 0);
					break;
				}
				}
				for (int k = 0; k < num9; k++)
				{
					BatchData batchData = new BatchData
					{
						m_LodMesh = val5,
						m_VTIndex0 = -1,
						m_VTIndex1 = -1,
						m_SubMeshIndex = (byte)k,
						m_MinLod = (byte)num4,
						m_ShadowLod = (byte)num5,
						m_LodIndex = (byte)(num8 + 1)
					};
					if (m_NativeBatchGroups.CreateBatch(batchData, num7, 16, m_NativeBatchInstances, m_NativeSubBatches) < 0)
					{
						Debug.Log((object)$"Too many batches in group (max: {16})");
						return;
					}
				}
				switch (type)
				{
				case MeshType.Object:
				case MeshType.Zone:
				{
					float3 meshSize = MathUtils.Size(val6);
					metersPerPixel = RenderingUtils.GetRenderingSize(val4, meshSize, num10, stackDirection);
					break;
				}
				case MeshType.Net:
				{
					float indexFactor2 = num10;
					metersPerPixel = RenderingUtils.GetRenderingSize(((float3)(ref val4)).xy, indexFactor2);
					break;
				}
				case MeshType.Lane:
				{
					float indexFactor = num10 / math.max(1f, MathUtils.Size(((Bounds3)(ref val6)).z));
					metersPerPixel = RenderingUtils.GetRenderingSize(((float3)(ref val4)).xy, indexFactor);
					break;
				}
				}
				num4 = math.clamp(RenderingUtils.CalculateLodLimit(metersPerPixel, bias) + 1, num4 + 1, 255 - num8);
				num5 = math.max(num5, num4);
			}
			for (int l = 0; l < num; l++)
			{
				BatchData batchData2 = new BatchData
				{
					m_VTIndex0 = -1,
					m_VTIndex1 = -1,
					m_SubMeshIndex = (byte)l,
					m_MinLod = (byte)num4,
					m_ShadowLod = (byte)num5
				};
				if (m_NativeBatchGroups.CreateBatch(batchData2, num7, 16, m_NativeBatchInstances, m_NativeSubBatches) < 0)
				{
					Debug.Log((object)$"Too many batches in group (max: {16})");
					break;
				}
			}
		}

		private bool IsNetOwnerTunnel(Owner owner)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			if (m_NetElevationData.TryGetComponent(owner.m_Owner, ref elevation) && math.cmin(elevation.m_Elevation) < 0f)
			{
				return true;
			}
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(owner.m_Owner, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					ConnectedEdge connectedEdge = val[i];
					if (m_NetElevationData.TryGetComponent(connectedEdge.m_Edge, ref elevation) && math.cmin(elevation.m_Elevation) < 0f)
					{
						return true;
					}
				}
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> __Game_Objects_Stopped_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Object> __Game_Objects_Object_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> __Game_Objects_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Marker> __Game_Net_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Block> __Game_Zones_Block_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Warning> __Game_Tools_Warning_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Override> __Game_Tools_Override_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Highlighted> __Game_Tools_Highlighted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> __Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RW_BufferLookup;

		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RW_BufferLookup;

		public BufferLookup<LodMesh> __Game_Prefabs_LodMesh_RW_BufferLookup;

		public BufferLookup<MeshMaterial> __Game_Prefabs_MeshMaterial_RW_BufferLookup;

		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RW_ComponentLookup;

		public ComponentLookup<NetCompositionMeshData> __Game_Prefabs_NetCompositionMeshData_RW_ComponentLookup;

		public ComponentLookup<ZoneBlockData> __Game_Prefabs_ZoneBlockData_RW_ComponentLookup;

		public BufferLookup<BatchGroup> __Game_Prefabs_BatchGroup_RW_BufferLookup;

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
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(true);
			__Game_Objects_Stopped_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stopped>(true);
			__Game_Objects_Object_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Object>(true);
			__Game_Objects_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Elevation>(true);
			__Game_Objects_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Marker>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_Orphan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(true);
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_UtilityLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.UtilityLane>(true);
			__Game_Net_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Marker>(true);
			__Game_Zones_Block_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Block>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Tools_Warning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Warning>(true);
			__Game_Tools_Override_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Override>(true);
			__Game_Tools_Highlighted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Highlighted>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshRef>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Prefabs_SubMesh_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(false);
			__Game_Prefabs_SubMeshGroup_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(false);
			__Game_Prefabs_LodMesh_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LodMesh>(false);
			__Game_Prefabs_MeshMaterial_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshMaterial>(false);
			__Game_Prefabs_MeshData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(false);
			__Game_Prefabs_NetCompositionMeshData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshData>(false);
			__Game_Prefabs_ZoneBlockData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneBlockData>(false);
			__Game_Prefabs_BatchGroup_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BatchGroup>(false);
		}
	}

	private BatchManagerSystem m_BatchManagerSystem;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_AllQuery;

	private bool m_Loaded;

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
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MeshBatch>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MeshBatch>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllQuery : m_UpdatedQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle dependencies3;
			JobHandle val2 = JobChunkExtensions.Schedule<RequiredBatchesJob>(new RequiredBatchesJob
			{
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StoppedType = InternalCompilerInterface.GetComponentTypeHandle<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectType = InternalCompilerInterface.GetComponentTypeHandle<Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectMarkerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UtilityLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetMarkerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Marker>(ref __TypeHandle.__Game_Net_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WarningType = InternalCompilerInterface.GetComponentTypeHandle<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OverrideType = InternalCompilerInterface.GetComponentTypeHandle<Override>(ref __TypeHandle.__Game_Tools_Override_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HighlightedType = InternalCompilerInterface.GetComponentTypeHandle<Highlighted>(ref __TypeHandle.__Game_Tools_Highlighted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionMeshRef = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshRef>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLodMeshes = InternalCompilerInterface.GetBufferLookup<LodMesh>(ref __TypeHandle.__Game_Prefabs_LodMesh_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabMeshMaterials = InternalCompilerInterface.GetBufferLookup<MeshMaterial>(ref __TypeHandle.__Game_Prefabs_MeshMaterial_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabMeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionMeshData = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshData>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabZoneBlockData = InternalCompilerInterface.GetComponentLookup<ZoneBlockData>(ref __TypeHandle.__Game_Prefabs_ZoneBlockData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBatchGroups = InternalCompilerInterface.GetBufferLookup<BatchGroup>(ref __TypeHandle.__Game_Prefabs_BatchGroup_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies),
				m_NativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies2),
				m_NativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: false, out dependencies3)
			}, val, JobHandle.CombineDependencies(((SystemBase)this).Dependency, JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3)));
			m_BatchManagerSystem.AddNativeBatchInstancesWriter(val2);
			m_BatchManagerSystem.AddNativeBatchGroupsWriter(val2);
			m_BatchManagerSystem.AddNativeSubBatchesWriter(val2);
			((SystemBase)this).Dependency = val2;
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
	public RequiredBatchesSystem()
	{
	}
}
