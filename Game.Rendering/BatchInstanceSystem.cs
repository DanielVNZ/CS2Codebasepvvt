using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BatchInstanceSystem : GameSystemBase
{
	[CompilerGenerated]
	public class Groups : GameSystemBase
	{
		private struct TypeHandle
		{
			public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

			public BufferLookup<FadeBatch> __Game_Rendering_FadeBatch_RW_BufferLookup;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void __AssignHandles(ref SystemState state)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
				__Game_Rendering_FadeBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FadeBatch>(false);
			}
		}

		private PreCullingSystem m_PreCullingSystem;

		private BatchManagerSystem m_BatchManagerSystem;

		public NativeParallelQueue<GroupActionData> m_GroupActionQueue;

		public NativeQueue<VelocityData> m_VelocityQueue;

		public NativeQueue<FadeData> m_FadeQueue;

		public JobHandle m_Dependency;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
			m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			JobHandle dependencies;
			NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies);
			JobHandle dependencies2;
			NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: false, out dependencies2);
			InstanceUpdater<CullingData, GroupData, BatchData, InstanceData> val = nativeBatchInstances.BeginInstanceUpdate((Allocator)3);
			JobHandle val2 = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency, dependencies);
			DequeueFadesJob dequeueFadesJob = new DequeueFadesJob
			{
				m_FadeContainer = m_PreCullingSystem.GetFadeContainer(),
				m_BatchInstances = nativeBatchInstances,
				m_VelocityQueue = m_VelocityQueue,
				m_FadeQueue = m_FadeQueue,
				m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FadeBatches = InternalCompilerInterface.GetBufferLookup<FadeBatch>(ref __TypeHandle.__Game_Rendering_FadeBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			GroupActionJob obj = new GroupActionJob
			{
				m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GroupActions = m_GroupActionQueue.AsReader(),
				m_BatchInstanceUpdater = val.AsParallel(int.MaxValue)
			};
			JobHandle val3 = IJobExtensions.Schedule<DequeueFadesJob>(dequeueFadesJob, val2);
			JobHandle val4 = IJobParallelForExtensions.Schedule<GroupActionJob>(obj, m_GroupActionQueue.HashRange, 1, val3);
			JobHandle jobHandle = nativeBatchInstances.EndInstanceUpdate(val, JobHandle.CombineDependencies(val4, dependencies2), nativeSubBatches);
			m_GroupActionQueue.Dispose(val4);
			m_VelocityQueue.Dispose(val3);
			m_FadeQueue.Dispose(val3);
			m_BatchManagerSystem.AddNativeBatchInstancesWriter(jobHandle);
			m_BatchManagerSystem.AddNativeSubBatchesWriter(jobHandle);
			((SystemBase)this).Dependency = val4;
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
		public Groups()
		{
		}
	}

	[BurstCompile]
	private struct BatchInstanceJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Error> m_ErrorData;

		[ReadOnly]
		public ComponentLookup<Warning> m_WarningData;

		[ReadOnly]
		public ComponentLookup<Override> m_OverrideData;

		[ReadOnly]
		public ComponentLookup<Highlighted> m_HighlightedData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshBatch> m_MeshBatches;

		[NativeDisableParallelForRestriction]
		public BufferLookup<FadeBatch> m_FadeBatches;

		[ReadOnly]
		public ComponentLookup<Stopped> m_StoppedData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.NetObject> m_NetObjectData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ObjectElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Marker> m_ObjectMarkerData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> m_NetOutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> m_UtilityLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Marker> m_NetMarkerData;

		[ReadOnly]
		public BufferLookup<CutRange> m_CutRanges;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public ComponentLookup<Block> m_ZoneBlockData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> m_PrefabGrowthScaleData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> m_PrefabQuantityObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> m_PrefabCompositionMeshData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> m_PrefabCompositionMeshRef;

		[ReadOnly]
		public BufferLookup<SubMesh> m_PrefabSubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_PrefabSubMeshGroups;

		[ReadOnly]
		public BufferLookup<BatchGroup> m_PrefabBatchGroups;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_MarkersVisible;

		[ReadOnly]
		public bool m_UnspawnedVisible;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public bool m_UseLodFade;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public UtilityTypes m_DilatedUtilityTypes;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[ReadOnly]
		public BoundsMask m_BecameVisible;

		[ReadOnly]
		public BoundsMask m_BecameHidden;

		[ReadOnly]
		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_BatchInstances;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public Writer<GroupActionData> m_GroupActionQueue;

		public ParallelWriter<FadeData> m_FadeQueue;

		public ParallelWriter<VelocityData> m_VelocityQueue;

		public void Execute(int index)
		{
			PreCullingData cullingData = m_CullingData[index];
			if ((cullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated | PreCullingFlags.FadeContainer)) != 0)
			{
				if ((cullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					RemoveInstances(cullingData);
				}
				else if ((cullingData.m_Flags & PreCullingFlags.Object) != 0)
				{
					UpdateObjectInstances(cullingData);
				}
				else if ((cullingData.m_Flags & PreCullingFlags.Net) != 0)
				{
					UpdateNetInstances(cullingData);
				}
				else if ((cullingData.m_Flags & PreCullingFlags.Lane) != 0)
				{
					UpdateLaneInstances(cullingData);
				}
				else if ((cullingData.m_Flags & PreCullingFlags.Zone) != 0)
				{
					UpdateZoneInstances(cullingData);
				}
				else if ((cullingData.m_Flags & PreCullingFlags.FadeContainer) != 0)
				{
					UpdateFadeInstances(cullingData);
				}
			}
		}

		private void RemoveInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> meshBatches = default(DynamicBuffer<MeshBatch>);
			if (!m_MeshBatches.TryGetBuffer(cullingData.m_Entity, ref meshBatches))
			{
				return;
			}
			bool flag = m_UseLodFade && (cullingData.m_Flags & (PreCullingFlags.Temp | PreCullingFlags.Zone)) == 0 && (((cullingData.m_Flags & PreCullingFlags.Deleted) != 0) ? ((cullingData.m_Flags & PreCullingFlags.Applied) == 0) : ((!m_UnspawnedVisible && m_UnspawnedData.HasComponent(cullingData.m_Entity)) || (m_CullingInfoData[cullingData.m_Entity].m_Mask & (m_VisibleMask | m_BecameHidden)) == 0));
			Entity val = Entity.Null;
			if (flag)
			{
				DynamicBuffer<TransformFrame> val2 = default(DynamicBuffer<TransformFrame>);
				if (m_TransformFrames.TryGetBuffer(cullingData.m_Entity, ref val2))
				{
					val = cullingData.m_Entity;
					UpdateFrame updateFrame = new UpdateFrame((uint)cullingData.m_UpdateFrame);
					ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_FrameTime, updateFrame.m_Index, out var updateFrame2, out var updateFrame3, out var framePosition);
					TransformFrame transformFrame = val2[(int)updateFrame2];
					TransformFrame transformFrame2 = val2[(int)updateFrame3];
					float3 velocity = math.lerp(transformFrame.m_Velocity, transformFrame2.m_Velocity, framePosition);
					m_VelocityQueue.Enqueue(new VelocityData
					{
						m_Source = val,
						m_Velocity = velocity
					});
				}
				else if (m_RelativeData.HasComponent(cullingData.m_Entity))
				{
					CurrentVehicle currentVehicle = default(CurrentVehicle);
					Owner owner = default(Owner);
					if (m_CurrentVehicleData.TryGetComponent(cullingData.m_Entity, ref currentVehicle))
					{
						val = currentVehicle.m_Vehicle;
					}
					else if (m_OwnerData.TryGetComponent(cullingData.m_Entity, ref owner))
					{
						val = owner.m_Owner;
						while (m_RelativeData.HasComponent(val))
						{
							val = m_OwnerData[val].m_Owner;
						}
					}
				}
			}
			RemoveInstances(meshBatches, flag, val);
			meshBatches.Clear();
		}

		private unsafe void UpdateObjectInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> meshBatches = default(DynamicBuffer<MeshBatch>);
			if (!m_MeshBatches.TryGetBuffer(cullingData.m_Entity, ref meshBatches))
			{
				return;
			}
			MeshLayer meshLayer = MeshLayer.Default;
			bool flag = (cullingData.m_Flags & PreCullingFlags.Temp) == 0 && (cullingData.m_Flags & (PreCullingFlags.Created | PreCullingFlags.Applied)) != PreCullingFlags.Applied;
			bool flag2 = m_InterpolatedTransformData.HasComponent(cullingData.m_Entity) || m_StoppedData.HasComponent(cullingData.m_Entity);
			bool flag3 = m_ObjectMarkerData.HasComponent(cullingData.m_Entity);
			bool flag4 = false;
			SubMeshFlags subMeshFlags = SubMeshFlags.DefaultMissingMesh | SubMeshFlags.HasTransform;
			subMeshFlags = (SubMeshFlags)((uint)subMeshFlags | (uint)(m_LeftHandTraffic ? 65536 : 131072));
			if (m_ErrorData.HasComponent(cullingData.m_Entity) || m_WarningData.HasComponent(cullingData.m_Entity) || m_OverrideData.HasComponent(cullingData.m_Entity) || m_HighlightedData.HasComponent(cullingData.m_Entity))
			{
				meshLayer |= MeshLayer.Outline;
				subMeshFlags |= SubMeshFlags.OutlineOnly;
				flag4 = true;
			}
			int oldBatchCount = meshBatches.Length;
			MeshBatch* ptr = stackalloc MeshBatch[oldBatchCount];
			UnsafeUtility.MemCpy((void*)ptr, meshBatches.GetUnsafeReadOnlyPtr(), (long)(oldBatchCount * UnsafeUtility.SizeOf<MeshBatch>()));
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			meshBatches.Clear();
			bool hasMeshMatches = false;
			if (flag && m_BecameVisible != 0)
			{
				flag = (m_CullingInfoData[cullingData.m_Entity].m_Mask & m_VisibleMask) != m_BecameVisible;
			}
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_PrefabSubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				MeshLayer meshLayer2 = meshLayer;
				SubMeshFlags subMeshFlags2 = subMeshFlags;
				int3 tileCounts = int3.op_Implicit(0);
				if ((cullingData.m_Flags & PreCullingFlags.Temp) != 0)
				{
					Temp temp = m_TempData[cullingData.m_Entity];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						goto IL_0627;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
					{
						meshLayer2 |= MeshLayer.Outline;
						subMeshFlags2 |= SubMeshFlags.OutlineOnly;
					}
					flag4 = (temp.m_Flags & TempFlags.Essential) != 0;
				}
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (flag3)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Marker;
				}
				else if (flag2)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Moving;
				}
				else if (m_ObjectElevationData.TryGetComponent(cullingData.m_Entity, ref elevation) && elevation.m_Elevation < 0f)
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Tunnel;
				}
				Tree tree = default(Tree);
				float3 scale;
				if (m_TreeData.TryGetComponent(cullingData.m_Entity, ref tree))
				{
					GrowthScaleData growthScaleData = default(GrowthScaleData);
					subMeshFlags2 = ((!m_PrefabGrowthScaleData.TryGetComponent(prefabRef.m_Prefab, ref growthScaleData)) ? (subMeshFlags2 | SubMeshFlags.RequireAdult) : (subMeshFlags2 | BatchDataHelpers.CalculateTreeSubMeshData(tree, growthScaleData, out scale)));
				}
				Stack stack = default(Stack);
				StackData stackData = default(StackData);
				if (m_StackData.TryGetComponent(cullingData.m_Entity, ref stack) && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
				{
					subMeshFlags2 |= BatchDataHelpers.CalculateStackSubMeshData(stack, stackData, out tileCounts, out scale, out var _);
				}
				Game.Objects.NetObject netObject = default(Game.Objects.NetObject);
				if (m_NetObjectData.TryGetComponent(cullingData.m_Entity, ref netObject))
				{
					subMeshFlags2 |= BatchDataHelpers.CalculateNetObjectSubMeshData(netObject);
				}
				Quantity quantity = default(Quantity);
				if (m_QuantityData.TryGetComponent(cullingData.m_Entity, ref quantity))
				{
					QuantityObjectData quantityObjectData = default(QuantityObjectData);
					subMeshFlags2 = ((!m_PrefabQuantityObjectData.TryGetComponent(prefabRef.m_Prefab, ref quantityObjectData)) ? (subMeshFlags2 | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, default(QuantityObjectData), m_EditorMode)) : (subMeshFlags2 | BatchDataHelpers.CalculateQuantitySubMeshData(quantity, quantityObjectData, m_EditorMode)));
				}
				UnderConstruction underConstruction = default(UnderConstruction);
				if (!m_UnderConstructionData.TryGetComponent(cullingData.m_Entity, ref underConstruction) || !(underConstruction.m_NewPrefab == Entity.Null))
				{
					Destroyed destroyed = default(Destroyed);
					ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
					if (m_DestroyedData.TryGetComponent(cullingData.m_Entity, ref destroyed) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) && (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot)) == (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot))
					{
						if (destroyed.m_Cleared >= 0f)
						{
							goto IL_0627;
						}
						meshLayer2 &= ~MeshLayer.Outline;
					}
					DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
					int num = 1;
					DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
					if (m_PrefabSubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val3) && m_MeshGroups.TryGetBuffer(cullingData.m_Entity, ref val2))
					{
						num = val2.Length;
					}
					MeshGroup meshGroup = default(MeshGroup);
					SubMeshGroup subMeshGroup = default(SubMeshGroup);
					Owner owner = default(Owner);
					for (int i = 0; i < num; i++)
					{
						if (val3.IsCreated)
						{
							CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
							subMeshGroup = val3[(int)meshGroup.m_SubMeshGroup];
						}
						else
						{
							subMeshGroup.m_SubMeshRange = new int2(0, val.Length);
						}
						for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
						{
							SubMesh subMesh = val[j];
							if ((subMesh.m_Flags & subMeshFlags2) == subMesh.m_Flags)
							{
								MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
								DynamicBuffer<BatchGroup> batchGroups = m_PrefabBatchGroups[subMesh.m_SubMesh];
								MeshLayer meshLayer3 = meshLayer2;
								if ((meshData.m_DefaultLayers != 0 && (meshLayer2 & (MeshLayer.Moving | MeshLayer.Marker)) == 0) || (meshData.m_DefaultLayers & (MeshLayer.Pipeline | MeshLayer.SubPipeline)) != 0)
								{
									meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Moving | MeshLayer.Tunnel | MeshLayer.Marker);
									m_OwnerData.TryGetComponent(cullingData.m_Entity, ref owner);
									meshLayer3 |= Game.Net.SearchSystem.GetLayers(owner, default(Game.Net.UtilityLane), meshData.m_DefaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData);
								}
								if ((meshLayer3 & MeshLayer.Outline) != 0 && (meshData.m_State & MeshFlags.Decal) != 0 && !flag4)
								{
									meshLayer3 &= ~MeshLayer.Outline;
								}
								if ((subMesh.m_Flags & SubMeshFlags.OutlineOnly) != 0)
								{
									meshLayer3 &= MeshLayer.Outline;
								}
								int num2 = 1;
								num2 = math.select(num2, tileCounts.x, (subMesh.m_Flags & SubMeshFlags.IsStackStart) != 0);
								num2 = math.select(num2, tileCounts.y, (subMesh.m_Flags & SubMeshFlags.IsStackMiddle) != 0);
								num2 = math.select(num2, tileCounts.z, (subMesh.m_Flags & SubMeshFlags.IsStackEnd) != 0);
								if (num2 >= 1)
								{
									AddInstance(ptr, ref oldBatchCount, meshBatches, batchGroups, meshLayer3, MeshType.Object, 0, flag, cullingData.m_Entity, i, j - subMeshGroup.m_SubMeshRange.x, num2, ref hasMeshMatches);
								}
							}
						}
					}
				}
			}
			goto IL_0627;
			IL_0627:
			meshBatches.TrimExcess();
			RemoveInstances(ptr, oldBatchCount, meshBatches, flag, hasMeshMatches, cullingData.m_Entity);
		}

		private unsafe void UpdateNetInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> meshBatches = default(DynamicBuffer<MeshBatch>);
			if (!m_MeshBatches.TryGetBuffer(cullingData.m_Entity, ref meshBatches))
			{
				return;
			}
			MeshLayer meshLayer = MeshLayer.Default;
			bool flag = (cullingData.m_Flags & PreCullingFlags.Temp) == 0 && (cullingData.m_Flags & (PreCullingFlags.Created | PreCullingFlags.Applied)) != PreCullingFlags.Applied;
			bool flag2 = m_NetOutsideConnectionData.HasComponent(cullingData.m_Entity);
			bool flag3 = m_NetMarkerData.HasComponent(cullingData.m_Entity);
			if (m_ErrorData.HasComponent(cullingData.m_Entity) || m_WarningData.HasComponent(cullingData.m_Entity) || m_HighlightedData.HasComponent(cullingData.m_Entity))
			{
				meshLayer |= MeshLayer.Outline;
			}
			int oldBatchCount = meshBatches.Length;
			MeshBatch* ptr = stackalloc MeshBatch[oldBatchCount];
			UnsafeUtility.MemCpy((void*)ptr, meshBatches.GetUnsafeReadOnlyPtr(), (long)(oldBatchCount * UnsafeUtility.SizeOf<MeshBatch>()));
			meshBatches.Clear();
			MeshLayer meshLayer2 = meshLayer;
			bool flag4 = false;
			bool hasMeshMatches = false;
			if (flag && m_BecameVisible != 0)
			{
				flag = (m_CullingInfoData[cullingData.m_Entity].m_Mask & m_VisibleMask) != m_BecameVisible;
			}
			if ((cullingData.m_Flags & PreCullingFlags.Temp) != 0)
			{
				Temp temp = m_TempData[cullingData.m_Entity];
				if ((temp.m_Flags & TempFlags.Hidden) != 0)
				{
					goto IL_0483;
				}
				if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
				{
					if ((temp.m_Flags & TempFlags.SubDetail) != 0)
					{
						flag4 = true;
					}
					else
					{
						meshLayer2 |= MeshLayer.Outline;
					}
				}
			}
			if (flag3)
			{
				meshLayer2 &= ~MeshLayer.Default;
				meshLayer2 |= MeshLayer.Marker;
			}
			Composition composition = default(Composition);
			Orphan orphan = default(Orphan);
			if (m_CompositionData.TryGetComponent(cullingData.m_Entity, ref composition))
			{
				Edge edge = m_EdgeData[cullingData.m_Entity];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[cullingData.m_Entity];
				StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[cullingData.m_Entity];
				EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[cullingData.m_Entity];
				PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
				NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
				if (math.any(edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length > 0.1f))
				{
					UpdateNetInstances(ptr, ref oldBatchCount, meshBatches, cullingData.m_Entity, composition.m_Edge, NetSubMesh.Edge, meshLayer2, flag, ref hasMeshMatches);
				}
				if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[edge.m_Start];
					NetGeometryData netGeometryData2 = m_PrefabNetGeometryData[prefabRef2.m_Prefab];
					NetSubMesh subMesh = (((netGeometryData.m_MergeLayers & netGeometryData2.m_MergeLayers) != Layer.None) ? NetSubMesh.StartNode : NetSubMesh.SubStartNode);
					MeshLayer meshLayer3 = meshLayer2;
					Temp temp2 = default(Temp);
					if (flag4 && m_TempData.TryGetComponent(edge.m_Start, ref temp2) && ((temp2.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent) || (temp2.m_Flags & TempFlags.Select) != 0))
					{
						meshLayer3 |= MeshLayer.Outline;
					}
					UpdateNetInstances(ptr, ref oldBatchCount, meshBatches, cullingData.m_Entity, composition.m_StartNode, subMesh, meshLayer3, flag, ref hasMeshMatches);
				}
				if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
				{
					PrefabRef prefabRef3 = m_PrefabRefData[edge.m_End];
					NetGeometryData netGeometryData3 = m_PrefabNetGeometryData[prefabRef3.m_Prefab];
					NetSubMesh subMesh2 = (((netGeometryData.m_MergeLayers & netGeometryData3.m_MergeLayers) != Layer.None) ? NetSubMesh.EndNode : NetSubMesh.SubEndNode);
					MeshLayer meshLayer4 = meshLayer2;
					Temp temp3 = default(Temp);
					if (flag4 && m_TempData.TryGetComponent(edge.m_End, ref temp3) && ((temp3.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent) || (temp3.m_Flags & TempFlags.Select) != 0))
					{
						meshLayer4 |= MeshLayer.Outline;
					}
					UpdateNetInstances(ptr, ref oldBatchCount, meshBatches, cullingData.m_Entity, composition.m_EndNode, subMesh2, meshLayer4, flag, ref hasMeshMatches);
				}
			}
			else if (!flag2 && m_OrphanData.TryGetComponent(cullingData.m_Entity, ref orphan))
			{
				UpdateNetInstances(ptr, ref oldBatchCount, meshBatches, cullingData.m_Entity, orphan.m_Composition, NetSubMesh.Orphan1, meshLayer2, flag, ref hasMeshMatches);
				UpdateNetInstances(ptr, ref oldBatchCount, meshBatches, cullingData.m_Entity, orphan.m_Composition, NetSubMesh.Orphan2, meshLayer2, flag, ref hasMeshMatches);
			}
			goto IL_0483;
			IL_0483:
			meshBatches.TrimExcess();
			RemoveInstances(ptr, oldBatchCount, meshBatches, flag, hasMeshMatches, cullingData.m_Entity);
		}

		private unsafe void UpdateNetInstances(MeshBatch* oldBatches, ref int oldBatchCount, DynamicBuffer<MeshBatch> meshBatches, Entity entity, Entity composition, NetSubMesh subMesh, MeshLayer layers, bool fadeIn, ref bool hasMeshMatches)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionMeshRef netCompositionMeshRef = m_PrefabCompositionMeshRef[composition];
			NetCompositionMeshData netCompositionMeshData = default(NetCompositionMeshData);
			if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef.m_Mesh, ref netCompositionMeshData))
			{
				DynamicBuffer<BatchGroup> batchGroups = m_PrefabBatchGroups[netCompositionMeshRef.m_Mesh];
				MeshLayer meshLayer = layers;
				if (netCompositionMeshData.m_DefaultLayers != 0 && (layers & MeshLayer.Marker) == 0)
				{
					meshLayer &= ~MeshLayer.Default;
					meshLayer |= netCompositionMeshData.m_DefaultLayers;
				}
				subMesh = (netCompositionMeshRef.m_Rotate ? NetSubMesh.RotatedEdge : subMesh);
				AddInstance(oldBatches, ref oldBatchCount, meshBatches, batchGroups, meshLayer, MeshType.Net, 0, fadeIn, entity, 0, (int)subMesh, 1, ref hasMeshMatches);
			}
		}

		private unsafe void UpdateLaneInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> meshBatches = default(DynamicBuffer<MeshBatch>);
			if (!m_MeshBatches.TryGetBuffer(cullingData.m_Entity, ref meshBatches))
			{
				return;
			}
			MeshLayer meshLayer = MeshLayer.Default;
			bool flag = (cullingData.m_Flags & PreCullingFlags.Temp) == 0 && (cullingData.m_Flags & (PreCullingFlags.Created | PreCullingFlags.Applied)) != PreCullingFlags.Applied;
			bool flag2 = false;
			if (m_ErrorData.HasComponent(cullingData.m_Entity) || m_WarningData.HasComponent(cullingData.m_Entity) || m_HighlightedData.HasComponent(cullingData.m_Entity))
			{
				meshLayer |= MeshLayer.Outline;
				flag2 = true;
			}
			SubMeshFlags subMeshFlags = ((!m_EditorMode && !m_MarkersVisible) ? SubMeshFlags.RequireEditor : ((SubMeshFlags)0u));
			subMeshFlags = (SubMeshFlags)((uint)subMeshFlags | (uint)(m_LeftHandTraffic ? 131072 : 65536));
			int oldBatchCount = meshBatches.Length;
			MeshBatch* ptr = stackalloc MeshBatch[oldBatchCount];
			UnsafeUtility.MemCpy((void*)ptr, meshBatches.GetUnsafeReadOnlyPtr(), (long)(oldBatchCount * UnsafeUtility.SizeOf<MeshBatch>()));
			Curve curve = m_CurveData[cullingData.m_Entity];
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			meshBatches.Clear();
			bool hasMeshMatches = false;
			if (flag && m_BecameVisible != 0)
			{
				flag = (m_CullingInfoData[cullingData.m_Entity].m_Mask & m_VisibleMask) != m_BecameVisible;
			}
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (curve.m_Length > 0.1f && m_PrefabSubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				MeshLayer meshLayer2 = meshLayer;
				SubMeshFlags subMeshFlags2 = subMeshFlags;
				if ((cullingData.m_Flags & PreCullingFlags.Temp) != 0)
				{
					Temp temp = m_TempData[cullingData.m_Entity];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						goto IL_052b;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent | TempFlags.SubDetail)) != 0)
					{
						meshLayer2 |= MeshLayer.Outline;
					}
					flag2 = (temp.m_Flags & TempFlags.Essential) != 0;
				}
				Owner owner = default(Owner);
				if (m_OwnerData.TryGetComponent(cullingData.m_Entity, ref owner) && IsNetOwnerTunnel(owner))
				{
					meshLayer2 &= ~MeshLayer.Default;
					meshLayer2 |= MeshLayer.Tunnel;
				}
				Game.Net.PedestrianLane pedestrianLane = default(Game.Net.PedestrianLane);
				if (m_PedestrianLaneData.TryGetComponent(cullingData.m_Entity, ref pedestrianLane) && (pedestrianLane.m_Flags & PedestrianLaneFlags.Unsafe) != 0)
				{
					subMeshFlags2 |= SubMeshFlags.RequireSafe;
				}
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				if (m_CarLaneData.TryGetComponent(cullingData.m_Entity, ref carLane))
				{
					if ((carLane.m_Flags & CarLaneFlags.Unsafe) != 0)
					{
						subMeshFlags2 |= SubMeshFlags.RequireSafe;
					}
					if ((carLane.m_Flags & CarLaneFlags.LevelCrossing) == 0)
					{
						subMeshFlags2 |= SubMeshFlags.RequireLevelCrossing;
					}
				}
				Game.Net.TrackLane trackLane = default(Game.Net.TrackLane);
				if (m_TrackLaneData.TryGetComponent(cullingData.m_Entity, ref trackLane))
				{
					if ((trackLane.m_Flags & TrackLaneFlags.LevelCrossing) == 0)
					{
						subMeshFlags2 |= SubMeshFlags.RequireLevelCrossing;
					}
					subMeshFlags2 = (SubMeshFlags)((uint)subMeshFlags2 | (uint)(((trackLane.m_Flags & (TrackLaneFlags.Switch | TrackLaneFlags.DiamondCrossing)) == 0) ? 4096 : 2048));
				}
				int num = 256;
				Game.Net.UtilityLane utilityLane = default(Game.Net.UtilityLane);
				UtilityLaneData utilityLaneData = default(UtilityLaneData);
				if (m_UtilityLaneData.TryGetComponent(cullingData.m_Entity, ref utilityLane) && m_DilatedUtilityTypes != UtilityTypes.None && m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & m_DilatedUtilityTypes) != UtilityTypes.None)
				{
					num = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(utilityLaneData.m_VisualCapacity)));
				}
				DynamicBuffer<CutRange> val2 = default(DynamicBuffer<CutRange>);
				for (int i = 0; i < val.Length; i++)
				{
					SubMesh subMesh = val[i];
					if ((subMesh.m_Flags & subMeshFlags2) != 0)
					{
						continue;
					}
					MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
					DynamicBuffer<BatchGroup> batchGroups = m_PrefabBatchGroups[subMesh.m_SubMesh];
					MeshLayer meshLayer3 = meshLayer2;
					if ((subMesh.m_Flags & SubMeshFlags.RequireEditor) != 0)
					{
						meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Tunnel);
						meshLayer3 |= MeshLayer.Marker;
					}
					if ((meshData.m_DefaultLayers != 0 && (meshLayer3 & MeshLayer.Marker) == 0) || (meshData.m_DefaultLayers & (MeshLayer.Pipeline | MeshLayer.SubPipeline)) != 0)
					{
						meshLayer3 &= ~(MeshLayer.Default | MeshLayer.Tunnel | MeshLayer.Marker);
						meshLayer3 |= Game.Net.SearchSystem.GetLayers(owner, utilityLane, meshData.m_DefaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData);
					}
					float length = MathUtils.Size(((Bounds3)(ref meshData.m_Bounds)).z);
					bool geometryTiling = (meshData.m_State & MeshFlags.Tiling) != 0;
					int num2 = 0;
					int clipCount;
					if (m_CutRanges.TryGetBuffer(cullingData.m_Entity, ref val2))
					{
						float num3 = 0f;
						for (int j = 0; j <= val2.Length; j++)
						{
							float num4;
							float num5;
							if (j < val2.Length)
							{
								CutRange cutRange = val2[j];
								num4 = cutRange.m_CurveDelta.min;
								num5 = cutRange.m_CurveDelta.max;
							}
							else
							{
								num4 = 1f;
								num5 = 1f;
							}
							if (num4 >= num3)
							{
								Curve curve2 = new Curve
								{
									m_Length = curve.m_Length * (num4 - num3)
								};
								if (curve2.m_Length > 0.1f)
								{
									num2 += BatchDataHelpers.GetTileCount(curve2, length, meshData.m_TilingCount, geometryTiling, out clipCount);
								}
							}
							num3 = num5;
						}
					}
					else
					{
						num2 = BatchDataHelpers.GetTileCount(curve, length, meshData.m_TilingCount, geometryTiling, out clipCount);
					}
					if (num2 >= 1)
					{
						if ((meshLayer3 & MeshLayer.Outline) != 0 && (meshData.m_State & MeshFlags.Decal) != 0 && !flag2)
						{
							meshLayer3 &= ~MeshLayer.Outline;
						}
						int num6 = math.min(num, (int)meshData.m_MinLod);
						AddInstance(ptr, ref oldBatchCount, meshBatches, batchGroups, meshLayer3, MeshType.Lane, (ushort)num6, flag, cullingData.m_Entity, 0, i, num2, ref hasMeshMatches);
					}
				}
			}
			goto IL_052b;
			IL_052b:
			meshBatches.TrimExcess();
			RemoveInstances(ptr, oldBatchCount, meshBatches, flag, hasMeshMatches, cullingData.m_Entity);
		}

		private unsafe void UpdateZoneInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> meshBatches = default(DynamicBuffer<MeshBatch>);
			if (m_MeshBatches.TryGetBuffer(cullingData.m_Entity, ref meshBatches))
			{
				int oldBatchCount = meshBatches.Length;
				MeshBatch* ptr = stackalloc MeshBatch[oldBatchCount];
				UnsafeUtility.MemCpy((void*)ptr, meshBatches.GetUnsafeReadOnlyPtr(), (long)(oldBatchCount * UnsafeUtility.SizeOf<MeshBatch>()));
				Block block = m_ZoneBlockData[cullingData.m_Entity];
				PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
				meshBatches.Clear();
				bool hasMeshMatches = false;
				if ((cullingData.m_Flags & PreCullingFlags.Temp) == 0 || (m_TempData[cullingData.m_Entity].m_Flags & TempFlags.Hidden) == 0)
				{
					ushort partition = (ushort)math.clamp(block.m_Size.x * block.m_Size.y - 1 >> 4, 0, 3);
					DynamicBuffer<BatchGroup> batchGroups = m_PrefabBatchGroups[prefabRef.m_Prefab];
					AddInstance(ptr, ref oldBatchCount, meshBatches, batchGroups, MeshLayer.Default, MeshType.Zone, partition, fadeIn: false, cullingData.m_Entity, 0, 0, 1, ref hasMeshMatches);
				}
				meshBatches.TrimExcess();
				RemoveInstances(ptr, oldBatchCount, meshBatches, fadeOut: false, hasMeshMatches, cullingData.m_Entity);
			}
		}

		private void UpdateFadeInstances(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> val = m_MeshBatches[cullingData.m_Entity];
			DynamicBuffer<FadeBatch> val2 = m_FadeBatches[cullingData.m_Entity];
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				if (m_BatchInstances.GetCullingData(meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex).lodFade.z == 0 || !m_UseLodFade)
				{
					m_GroupActionQueue.Enqueue(new GroupActionData
					{
						m_GroupIndex = meshBatch.m_GroupIndex,
						m_RemoveInstanceIndex = meshBatch.m_InstanceIndex
					});
					val.RemoveAtSwapBack(i);
					val2.RemoveAtSwapBack(i);
					i--;
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

		private unsafe void AddInstance(MeshBatch* oldBatches, ref int oldBatchCount, DynamicBuffer<MeshBatch> meshBatches, DynamicBuffer<BatchGroup> batchGroups, MeshLayer layers, MeshType type, ushort partition, bool fadeIn, Entity entity, int meshGroupIndex, int meshIndex, int tileCount, ref bool hasMeshMatches)
		{
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < batchGroups.Length; i++)
			{
				BatchGroup batchGroup = batchGroups[i];
				if ((batchGroup.m_Layer & layers) == 0 || batchGroup.m_Type != type || batchGroup.m_Partition != partition)
				{
					continue;
				}
				for (int j = 0; j < tileCount; j++)
				{
					bool flag = fadeIn;
					int num = 0;
					while (true)
					{
						if (num < oldBatchCount)
						{
							MeshBatch meshBatch = oldBatches[num];
							bool flag2 = meshBatch.m_MeshGroup == meshGroupIndex && meshBatch.m_MeshIndex == meshIndex && meshBatch.m_TileIndex == j;
							flag = flag && !flag2;
							if (flag2 && meshBatch.m_GroupIndex == batchGroup.m_GroupIndex)
							{
								meshBatches.Add(meshBatch);
								oldBatches[num] = oldBatches[--oldBatchCount];
								break;
							}
							hasMeshMatches |= flag2;
							num++;
							continue;
						}
						if (m_BecameVisible != 0)
						{
							flag &= (m_BecameVisible & CommonUtils.GetBoundsMask(batchGroup.m_Layer)) == 0;
						}
						InstanceData addInstanceData = new InstanceData
						{
							m_Entity = entity,
							m_MeshGroup = (byte)meshGroupIndex,
							m_MeshIndex = (byte)meshIndex,
							m_TileIndex = (byte)j
						};
						m_GroupActionQueue.Enqueue(new GroupActionData
						{
							m_GroupIndex = batchGroup.m_GroupIndex,
							m_RemoveInstanceIndex = int.MaxValue,
							m_MergeIndex = batchGroup.m_MergeIndex,
							m_AddInstanceData = addInstanceData,
							m_FadeIn = flag
						});
						meshBatches.Add(new MeshBatch
						{
							m_GroupIndex = batchGroup.m_GroupIndex,
							m_InstanceIndex = -1,
							m_MeshGroup = addInstanceData.m_MeshGroup,
							m_MeshIndex = addInstanceData.m_MeshIndex,
							m_TileIndex = addInstanceData.m_TileIndex
						});
						break;
					}
				}
			}
		}

		private unsafe void RemoveInstances(MeshBatch* oldBatches, int oldBatchCount, DynamicBuffer<MeshBatch> meshBatches, bool fadeOut, bool hasMeshMatches, Entity entity)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < oldBatchCount; i++)
			{
				MeshBatch meshBatch = oldBatches[i];
				if (meshBatch.m_GroupIndex == -1)
				{
					continue;
				}
				bool flag = fadeOut && m_UseLodFade;
				if (flag && hasMeshMatches)
				{
					for (int j = 0; j < meshBatches.Length; j++)
					{
						MeshBatch meshBatch2 = meshBatches[j];
						if (meshBatch.m_MeshGroup == meshBatch2.m_MeshGroup && meshBatch.m_MeshIndex == meshBatch2.m_MeshIndex && meshBatch.m_TileIndex == meshBatch2.m_TileIndex)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					m_FadeQueue.Enqueue(new FadeData
					{
						m_Source = Entity.Null,
						m_GroupIndex = meshBatch.m_GroupIndex,
						m_InstanceIndex = meshBatch.m_InstanceIndex
					});
				}
				else
				{
					m_GroupActionQueue.Enqueue(new GroupActionData
					{
						m_GroupIndex = meshBatch.m_GroupIndex,
						m_RemoveInstanceIndex = meshBatch.m_InstanceIndex
					});
				}
			}
		}

		private void RemoveInstances(DynamicBuffer<MeshBatch> meshBatches, bool fadeOut, Entity entity)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < meshBatches.Length; i++)
			{
				MeshBatch meshBatch = meshBatches[i];
				if (meshBatch.m_GroupIndex != -1)
				{
					if (fadeOut)
					{
						m_FadeQueue.Enqueue(new FadeData
						{
							m_Source = entity,
							m_GroupIndex = meshBatch.m_GroupIndex,
							m_InstanceIndex = meshBatch.m_InstanceIndex
						});
					}
					else
					{
						m_GroupActionQueue.Enqueue(new GroupActionData
						{
							m_GroupIndex = meshBatch.m_GroupIndex,
							m_RemoveInstanceIndex = meshBatch.m_InstanceIndex
						});
					}
				}
			}
		}
	}

	public struct GroupActionData : IComparable<GroupActionData>
	{
		public int m_GroupIndex;

		public int m_RemoveInstanceIndex;

		public int m_MergeIndex;

		public InstanceData m_AddInstanceData;

		public bool m_FadeIn;

		public int CompareTo(GroupActionData other)
		{
			return math.select(other.m_RemoveInstanceIndex - m_RemoveInstanceIndex, m_GroupIndex - other.m_GroupIndex, m_GroupIndex != other.m_GroupIndex);
		}

		public override int GetHashCode()
		{
			return m_GroupIndex;
		}
	}

	public struct VelocityData
	{
		public Entity m_Source;

		public float3 m_Velocity;
	}

	public struct FadeData
	{
		public Entity m_Source;

		public int m_GroupIndex;

		public int m_InstanceIndex;
	}

	[BurstCompile]
	private struct DequeueFadesJob : IJob
	{
		[ReadOnly]
		public Entity m_FadeContainer;

		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_BatchInstances;

		public NativeQueue<VelocityData> m_VelocityQueue;

		public NativeQueue<FadeData> m_FadeQueue;

		public BufferLookup<MeshBatch> m_MeshBatches;

		public BufferLookup<FadeBatch> m_FadeBatches;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<VelocityData> val = default(NativeArray<VelocityData>);
			if (!m_VelocityQueue.IsEmpty())
			{
				val = m_VelocityQueue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			}
			FadeData fadeData = default(FadeData);
			while (m_FadeQueue.TryDequeue(ref fadeData))
			{
				ref InstanceData reference = ref m_BatchInstances.AccessInstanceData(fadeData.m_GroupIndex, fadeData.m_InstanceIndex);
				ref CullingData reference2 = ref m_BatchInstances.AccessCullingData(fadeData.m_GroupIndex, fadeData.m_InstanceIndex);
				reference.m_Entity = m_FadeContainer;
				reference2.isFading = true;
				DynamicBuffer<MeshBatch> val2 = m_MeshBatches[m_FadeContainer];
				DynamicBuffer<FadeBatch> val3 = m_FadeBatches[m_FadeContainer];
				float3 velocity = default(float3);
				if (fadeData.m_Source != Entity.Null && val.IsCreated)
				{
					for (int i = 0; i < val.Length; i++)
					{
						VelocityData velocityData = val[i];
						if (velocityData.m_Source == fadeData.m_Source)
						{
							velocity = velocityData.m_Velocity;
							break;
						}
					}
				}
				val2.Add(new MeshBatch
				{
					m_GroupIndex = fadeData.m_GroupIndex,
					m_InstanceIndex = fadeData.m_InstanceIndex,
					m_MeshGroup = byte.MaxValue,
					m_MeshIndex = byte.MaxValue,
					m_TileIndex = byte.MaxValue
				});
				val3.Add(new FadeBatch
				{
					m_Source = fadeData.m_Source,
					m_Velocity = velocity
				});
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}
	}

	[BurstCompile]
	private struct GroupActionJob : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshBatch> m_MeshBatches;

		public Reader<GroupActionData> m_GroupActions;

		[NativeDisableParallelForRestriction]
		public ParallelInstanceUpdater<CullingData, GroupData, BatchData, InstanceData> m_BatchInstanceUpdater;

		public void Execute(int index)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<GroupActionData> val = m_GroupActions.ToArray(index, AllocatorHandle.op_Implicit((Allocator)2));
			if (val.Length >= 2)
			{
				NativeSortExtension.Sort<GroupActionData>(val);
			}
			int num = -1;
			GroupInstanceUpdater<CullingData, GroupData, BatchData, InstanceData> val2 = default(GroupInstanceUpdater<CullingData, GroupData, BatchData, InstanceData>);
			for (int i = 0; i < val.Length; i++)
			{
				GroupActionData groupActionData = val[i];
				if (groupActionData.m_GroupIndex != num)
				{
					if (num != -1)
					{
						m_BatchInstanceUpdater.EndGroup(val2);
					}
					val2 = m_BatchInstanceUpdater.BeginGroup(groupActionData.m_GroupIndex);
					num = groupActionData.m_GroupIndex;
				}
				if (groupActionData.m_RemoveInstanceIndex != int.MaxValue)
				{
					InstanceData instanceData = val2.RemoveInstance(groupActionData.m_RemoveInstanceIndex);
					if (!(instanceData.m_Entity != Entity.Null))
					{
						continue;
					}
					DynamicBuffer<MeshBatch> val3 = m_MeshBatches[instanceData.m_Entity];
					int instanceCount = val2.GetInstanceCount();
					for (int j = 0; j < val3.Length; j++)
					{
						MeshBatch meshBatch = val3[j];
						if (meshBatch.m_GroupIndex == groupActionData.m_GroupIndex && meshBatch.m_InstanceIndex == instanceCount)
						{
							meshBatch.m_InstanceIndex = groupActionData.m_RemoveInstanceIndex;
							val3[j] = meshBatch;
							break;
						}
					}
					continue;
				}
				DynamicBuffer<MeshBatch> val4 = m_MeshBatches[groupActionData.m_AddInstanceData.m_Entity];
				for (int k = 0; k < val4.Length; k++)
				{
					MeshBatch meshBatch2 = val4[k];
					if (meshBatch2.m_GroupIndex == groupActionData.m_GroupIndex && meshBatch2.m_InstanceIndex == -1 && meshBatch2.m_MeshGroup == groupActionData.m_AddInstanceData.m_MeshGroup && meshBatch2.m_MeshIndex == groupActionData.m_AddInstanceData.m_MeshIndex && meshBatch2.m_TileIndex == groupActionData.m_AddInstanceData.m_TileIndex)
					{
						int num2 = math.select(255, 0, groupActionData.m_FadeIn);
						CullingData cullingData = new CullingData
						{
							lodFade = int4.op_Implicit(num2)
						};
						meshBatch2.m_InstanceIndex = val2.AddInstance(groupActionData.m_AddInstanceData, cullingData, groupActionData.m_MergeIndex);
						val4[k] = meshBatch2;
						break;
					}
				}
			}
			if (num != -1)
			{
				m_BatchInstanceUpdater.EndGroup(val2);
			}
			val.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Error> __Game_Tools_Error_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Warning> __Game_Tools_Warning_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Override> __Game_Tools_Override_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Highlighted> __Game_Tools_Highlighted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

		public BufferLookup<FadeBatch> __Game_Rendering_FadeBatch_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Stopped> __Game_Objects_Stopped_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.NetObject> __Game_Objects_NetObject_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Marker> __Game_Objects_Marker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Marker> __Game_Net_Marker_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CutRange> __Game_Net_CutRange_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> __Game_Prefabs_GrowthScaleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<QuantityObjectData> __Game_Prefabs_QuantityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> __Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> __Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<BatchGroup> __Game_Prefabs_BatchGroup_RO_BufferLookup;

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
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Tools_Error_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Error>(true);
			__Game_Tools_Warning_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Warning>(true);
			__Game_Tools_Override_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Override>(true);
			__Game_Tools_Highlighted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Highlighted>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
			__Game_Rendering_FadeBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FadeBatch>(false);
			__Game_Objects_Stopped_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stopped>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_NetObject_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.NetObject>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Marker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Marker>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.OutsideConnection>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_UtilityLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.UtilityLane>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Net_Marker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Marker>(true);
			__Game_Net_CutRange_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CutRange>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_GrowthScaleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GrowthScaleData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_QuantityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<QuantityObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshData>(true);
			__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshRef>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_BatchGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BatchGroup>(true);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private PreCullingSystem m_PreCullingSystem;

	private UndergroundViewSystem m_UndergroundViewSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ToolSystem m_ToolSystem;

	private Groups m_Groups;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_UndergroundViewSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UndergroundViewSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_Groups = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Groups>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: true, out dependencies);
		m_Groups.m_GroupActionQueue = new NativeParallelQueue<GroupActionData>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Groups.m_VelocityQueue = new NativeQueue<VelocityData>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Groups.m_FadeQueue = new NativeQueue<FadeData>(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies2;
		BatchInstanceJob obj = new BatchInstanceJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorData = InternalCompilerInterface.GetComponentLookup<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WarningData = InternalCompilerInterface.GetComponentLookup<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverrideData = InternalCompilerInterface.GetComponentLookup<Override>(ref __TypeHandle.__Game_Tools_Override_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HighlightedData = InternalCompilerInterface.GetComponentLookup<Highlighted>(ref __TypeHandle.__Game_Tools_Highlighted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FadeBatches = InternalCompilerInterface.GetBufferLookup<FadeBatch>(ref __TypeHandle.__Game_Rendering_FadeBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedData = InternalCompilerInterface.GetComponentLookup<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetObjectData = InternalCompilerInterface.GetComponentLookup<Game.Objects.NetObject>(ref __TypeHandle.__Game_Objects_NetObject_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectMarkerData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetOutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetMarkerData = InternalCompilerInterface.GetComponentLookup<Game.Net.Marker>(ref __TypeHandle.__Game_Net_Marker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CutRanges = InternalCompilerInterface.GetBufferLookup<CutRange>(ref __TypeHandle.__Game_Net_CutRange_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGrowthScaleData = InternalCompilerInterface.GetComponentLookup<GrowthScaleData>(ref __TypeHandle.__Game_Prefabs_GrowthScaleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabQuantityObjectData = InternalCompilerInterface.GetComponentLookup<QuantityObjectData>(ref __TypeHandle.__Game_Prefabs_QuantityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionMeshData = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshData>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionMeshRef = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshRef>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBatchGroups = InternalCompilerInterface.GetBufferLookup<BatchGroup>(ref __TypeHandle.__Game_Prefabs_BatchGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_MarkersVisible = m_RenderingSystem.markersVisible,
			m_UnspawnedVisible = m_RenderingSystem.unspawnedVisible,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_UseLodFade = m_RenderingSystem.lodCrossFade,
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_DilatedUtilityTypes = m_UndergroundViewSystem.utilityTypes,
			m_VisibleMask = m_PreCullingSystem.visibleMask,
			m_BecameVisible = m_PreCullingSystem.becameVisible,
			m_BecameHidden = m_PreCullingSystem.becameHidden,
			m_BatchInstances = nativeBatchInstances,
			m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies2),
			m_GroupActionQueue = m_Groups.m_GroupActionQueue.AsWriter(),
			m_VelocityQueue = m_Groups.m_VelocityQueue.AsParallelWriter(),
			m_FadeQueue = m_Groups.m_FadeQueue.AsParallelWriter()
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<BatchInstanceJob, PreCullingData>(obj, obj.m_CullingData, 4, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		m_BatchManagerSystem.AddNativeBatchInstancesReader(val);
		m_PreCullingSystem.AddCullingDataReader(val);
		m_Groups.m_Dependency = val;
		((SystemBase)this).Dependency = val;
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
	public BatchInstanceSystem()
	{
	}
}
