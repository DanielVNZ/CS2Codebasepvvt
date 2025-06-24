using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
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
public class SecondaryObjectSystem : GameSystemBase
{
	private struct UpdateData
	{
		public Entity m_Owner;

		public Entity m_Prefab;

		public Transform m_Transform;
	}

	private struct SubObjectOwnerData : IComparable<SubObjectOwnerData>
	{
		public Entity m_Owner;

		public Entity m_Original;

		public bool m_Temp;

		public bool m_Deleted;

		public SubObjectOwnerData(Entity owner, Entity original, bool temp, bool deleted)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Owner = owner;
			m_Original = original;
			m_Temp = temp;
			m_Deleted = deleted;
		}

		public int CompareTo(SubObjectOwnerData other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_Owner.Index - other.m_Owner.Index;
		}
	}

	private struct TrafficSignNeeds
	{
		public uint m_SignTypeMask;

		public uint m_RemoveSignTypeMask;

		public uint m_SignTypeMask2;

		public uint m_RemoveSignTypeMask2;

		public ushort m_SpeedLimit;

		public ushort m_SpeedLimit2;

		public float m_VehicleLanesLeft;

		public float m_VehicleLanesRight;

		public ushort m_VehicleMask;

		public ushort m_CrossingLeftMask;

		public ushort m_CrossingRightMask;

		public LaneDirectionType m_Left;

		public LaneDirectionType m_Forward;

		public LaneDirectionType m_Right;
	}

	private struct TrafficSignData
	{
		public Transform m_ParentTransform;

		public Transform m_ObjectTransform;

		public Transform m_LocalTransform;

		public float2 m_ForwardDirection;

		public Entity m_Prefab;

		public int m_Probability;

		public SubObjectFlags m_Flags;

		public TrafficSignNeeds m_TrafficSignNeeds;

		public bool m_IsLowered;
	}

	private struct StreetLightData
	{
		public Transform m_ParentTransform;

		public Transform m_ObjectTransform;

		public Transform m_LocalTransform;

		public Entity m_Prefab;

		public int m_Probability;

		public SubObjectFlags m_Flags;

		public StreetLightLayer m_Layer;

		public float m_Spacing;

		public float m_Priority;

		public bool m_IsLowered;
	}

	private struct UtilityObjectData
	{
		public float3 m_UtilityPosition;
	}

	private struct UtilityNodeData
	{
		public Transform m_Transform;

		public Entity m_Prefab;

		public PathNode m_PathNode;

		public int m_Count;

		public float m_LanePriority;

		public float m_NodePriority;

		public float m_Elevation;

		public UtilityTypes m_UtilityTypes;

		public bool m_Unsure;

		public bool m_Vertical;

		public bool m_IsNew;
	}

	private struct TargetLaneData
	{
		public CarLaneFlags m_CarLaneFlags;

		public CarLaneFlags m_AndCarLaneFlags;

		public float2 m_SpeedLimit;
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

	private struct UpdateSecondaryObjectsData
	{
		public NativeParallelMultiHashMap<Entity, Entity> m_OldEntities;

		public NativeParallelMultiHashMap<Entity, Entity> m_OriginalEntities;

		public NativeParallelHashSet<Entity> m_PlaceholderRequirements;

		public NativeParallelHashMap<PlaceholderKey, Random> m_SelectedSpawnabled;

		public NativeParallelHashMap<PathNode, TargetLaneData> m_SourceLanes;

		public NativeParallelHashMap<PathNode, TargetLaneData> m_TargetLanes;

		public NativeList<TrafficSignData> m_TrafficSigns;

		public NativeList<StreetLightData> m_StreetLights;

		public NativeList<UtilityObjectData> m_UtilityObjects;

		public NativeList<UtilityNodeData> m_UtilityNodes;

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
				m_PlaceholderRequirements = new NativeParallelHashSet<Entity>(10, AllocatorHandle.op_Implicit(allocator));
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

		public void EnsureSourceLanes(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SourceLanes.IsCreated)
			{
				m_SourceLanes = new NativeParallelHashMap<PathNode, TargetLaneData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureTargetLanes(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TargetLanes.IsCreated)
			{
				m_TargetLanes = new NativeParallelHashMap<PathNode, TargetLaneData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureTrafficSigns(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TrafficSigns.IsCreated)
			{
				m_TrafficSigns = new NativeList<TrafficSignData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureStreetLights(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_StreetLights.IsCreated)
			{
				m_StreetLights = new NativeList<StreetLightData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureUtilityObjects(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_UtilityObjects.IsCreated)
			{
				m_UtilityObjects = new NativeList<UtilityObjectData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void EnsureUtilityNodes(Allocator allocator)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_UtilityNodes.IsCreated)
			{
				m_UtilityNodes = new NativeList<UtilityNodeData>(16, AllocatorHandle.op_Implicit(allocator));
			}
		}

		public void Clear()
		{
			if (m_OldEntities.IsCreated)
			{
				m_OldEntities.Clear();
			}
			if (m_OriginalEntities.IsCreated)
			{
				m_OriginalEntities.Clear();
			}
			if (m_PlaceholderRequirements.IsCreated)
			{
				m_PlaceholderRequirements.Clear();
			}
			if (m_SelectedSpawnabled.IsCreated)
			{
				m_SelectedSpawnabled.Clear();
			}
			if (m_SourceLanes.IsCreated)
			{
				m_SourceLanes.Clear();
			}
			if (m_TargetLanes.IsCreated)
			{
				m_TargetLanes.Clear();
			}
			if (m_TrafficSigns.IsCreated)
			{
				m_TrafficSigns.Clear();
			}
			if (m_StreetLights.IsCreated)
			{
				m_StreetLights.Clear();
			}
			if (m_UtilityObjects.IsCreated)
			{
				m_UtilityObjects.Clear();
			}
			if (m_UtilityNodes.IsCreated)
			{
				m_UtilityNodes.Clear();
			}
			m_RequirementsSearched = false;
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
			if (m_SourceLanes.IsCreated)
			{
				m_SourceLanes.Dispose();
			}
			if (m_TargetLanes.IsCreated)
			{
				m_TargetLanes.Dispose();
			}
			if (m_TrafficSigns.IsCreated)
			{
				m_TrafficSigns.Dispose();
			}
			if (m_StreetLights.IsCreated)
			{
				m_StreetLights.Dispose();
			}
			if (m_UtilityObjects.IsCreated)
			{
				m_UtilityObjects.Dispose();
			}
			if (m_UtilityNodes.IsCreated)
			{
				m_UtilityNodes.Dispose();
			}
		}
	}

	[BurstCompile]
	private struct FillUpdateMapJob : IJob
	{
		public NativeQueue<UpdateData> m_UpdateQueue;

		public NativeParallelMultiHashMap<Entity, UpdateData> m_UpdateMap;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			UpdateData updateData = default(UpdateData);
			while (m_UpdateQueue.TryDequeue(ref updateData))
			{
				m_UpdateMap.Add(updateData.m_Owner, updateData);
			}
		}
	}

	[BurstCompile]
	private struct SecondaryLaneAnchorJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> m_UtilityLaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> m_PrefabSubLanes;

		[ReadOnly]
		public NativeParallelMultiHashMap<Entity, UpdateData> m_UpdateMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Game.Net.UtilityLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.UtilityLane>(ref m_UtilityLaneType);
			NativeArray<EdgeLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeLane>(ref m_EdgeLaneType);
			NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Game.Net.UtilityLane utilityLane = nativeArray2[i];
				if ((utilityLane.m_Flags & (UtilityLaneFlags.SecondaryStartAnchor | UtilityLaneFlags.SecondaryEndAnchor)) == 0)
				{
					continue;
				}
				Owner owner = nativeArray[i];
				Curve curve = nativeArray4[i];
				PrefabRef prefabRef = nativeArray5[i];
				if ((utilityLane.m_Flags & UtilityLaneFlags.SecondaryStartAnchor) != 0)
				{
					Entity owner2 = owner.m_Owner;
					float3 val = -MathUtils.StartTangent(curve.m_Bezier);
					val.y = 0f;
					val = math.normalizesafe(val, default(float3));
					float requireDirection = float.MinValue;
					if (nativeArray3.Length != 0)
					{
						EdgeLane edgeLane = nativeArray3[i];
						if (edgeLane.m_EdgeDelta.x == 0f)
						{
							owner2 = m_EdgeData[owner.m_Owner].m_Start;
						}
						else if (edgeLane.m_EdgeDelta.x == 1f)
						{
							owner2 = m_EdgeData[owner.m_Owner].m_End;
						}
						if (edgeLane.m_EdgeDelta.x == edgeLane.m_EdgeDelta.y)
						{
							requireDirection = math.dot(val, curve.m_Bezier.d - curve.m_Bezier.a);
						}
					}
					curve.m_Bezier.a = FindAnchorPosition(owner2, prefabRef.m_Prefab, curve.m_Bezier.a, val, requireDirection);
				}
				if ((utilityLane.m_Flags & UtilityLaneFlags.SecondaryEndAnchor) != 0)
				{
					Entity owner3 = owner.m_Owner;
					float3 val2 = MathUtils.EndTangent(curve.m_Bezier);
					val2.y = 0f;
					val2 = math.normalizesafe(val2, default(float3));
					float requireDirection2 = float.MinValue;
					if (nativeArray3.Length != 0)
					{
						EdgeLane edgeLane2 = nativeArray3[i];
						if (edgeLane2.m_EdgeDelta.y == 0f)
						{
							owner3 = m_EdgeData[owner.m_Owner].m_Start;
						}
						else if (edgeLane2.m_EdgeDelta.y == 1f)
						{
							owner3 = m_EdgeData[owner.m_Owner].m_End;
						}
						if (edgeLane2.m_EdgeDelta.x == edgeLane2.m_EdgeDelta.y)
						{
							requireDirection2 = math.dot(val2, curve.m_Bezier.a - curve.m_Bezier.d);
						}
					}
					curve.m_Bezier.d = FindAnchorPosition(owner3, prefabRef.m_Prefab, curve.m_Bezier.d, val2, requireDirection2);
				}
				UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef.m_Prefab];
				if (utilityLaneData.m_Hanging != 0f)
				{
					curve.m_Bezier.b = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 1f / 3f);
					curve.m_Bezier.c = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 2f / 3f);
					float num = math.distance(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.d)).xz) * utilityLaneData.m_Hanging * 1.3333334f;
					curve.m_Bezier.b.y -= num;
					curve.m_Bezier.c.y -= num;
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
				nativeArray4[i] = curve;
			}
		}

		private float3 FindAnchorPosition(Entity owner, Entity prefab, float3 position, float3 direction, float requireDirection)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			float3 bestPosition = position;
			float bestDistance = float.MaxValue;
			DynamicBuffer<SubObject> val2 = default(DynamicBuffer<SubObject>);
			if (m_UpdatedData.HasComponent(owner))
			{
				UpdateData updateData = default(UpdateData);
				NativeParallelMultiHashMapIterator<Entity> val = default(NativeParallelMultiHashMapIterator<Entity>);
				if (m_UpdateMap.TryGetFirstValue(owner, ref updateData, ref val))
				{
					do
					{
						FindAnchorPosition(updateData.m_Prefab, prefab, updateData.m_Transform, position, direction, requireDirection, ref bestPosition, ref bestDistance);
					}
					while (m_UpdateMap.TryGetNextValue(ref updateData, ref val));
				}
			}
			else if (m_SubObjects.TryGetBuffer(owner, ref val2))
			{
				for (int i = 0; i < val2.Length; i++)
				{
					SubObject subObject = val2[i];
					PrefabRef prefabRef = m_PrefabRefData[subObject.m_SubObject];
					Transform ownerTransform = m_TransformData[subObject.m_SubObject];
					FindAnchorPosition(prefabRef.m_Prefab, prefab, ownerTransform, position, direction, requireDirection, ref bestPosition, ref bestDistance);
				}
			}
			return bestPosition;
		}

		private void FindAnchorPosition(Entity ownerPrefab, Entity lanePrefab, Transform ownerTransform, float3 lanePosition, float3 laneDirection, float requireDirection, ref float3 bestPosition, ref float bestDistance)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Prefabs.SubLane> val = default(DynamicBuffer<Game.Prefabs.SubLane>);
			if (!m_PrefabSubLanes.TryGetBuffer(ownerPrefab, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Game.Prefabs.SubLane subLane = val[i];
				if (subLane.m_Prefab != lanePrefab || subLane.m_NodeIndex.x != subLane.m_NodeIndex.y)
				{
					continue;
				}
				float3 val2 = ObjectUtils.LocalToWorld(ownerTransform, subLane.m_Curve.a);
				float3 val3 = val2 - lanePosition;
				if (requireDirection != float.MinValue)
				{
					if (math.dot(laneDirection, val3) < requireDirection)
					{
						continue;
					}
				}
				else
				{
					val3 -= laneDirection * (math.dot(laneDirection, val3) * 0.75f);
				}
				float num = math.lengthsq(val3);
				if (num < bestDistance)
				{
					bestPosition = val2;
					bestDistance = num;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
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
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SubObjectOwnerData> m_OwnerQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					DynamicBuffer<SubObject> val2 = bufferAccessor[i];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity subObject = val2[j].m_SubObject;
						if (!m_DeletedData.HasComponent(subObject) && m_SecondaryData.HasComponent(subObject) && m_OwnerData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == val)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, subObject, ref m_AppliedTypes);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, subObject, default(Deleted));
							if (m_SubObjects.HasBuffer(subObject))
							{
								m_OwnerQueue.Enqueue(new SubObjectOwnerData(subObject, Entity.Null, temp: false, deleted: true));
							}
						}
					}
				}
				return;
			}
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity owner = nativeArray[k];
				if (nativeArray2.Length != 0)
				{
					Temp temp = nativeArray2[k];
					if ((temp.m_Flags & TempFlags.Replace) != 0)
					{
						m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner, Entity.Null, temp: true, deleted: false));
					}
					else
					{
						m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner, temp.m_Original, temp: true, deleted: false));
					}
				}
				else
				{
					m_OwnerQueue.Enqueue(new SubObjectOwnerData(owner, Entity.Null, temp: false, deleted: false));
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

		public void Execute()
		{
			int count = m_OwnerQueue.Count;
			if (count != 0)
			{
				m_OwnerList.ResizeUninitialized(count);
				for (int i = 0; i < count; i++)
				{
					m_OwnerList[i] = m_OwnerQueue.Dequeue();
				}
				MergeOwners();
			}
		}

		private void MergeOwners()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			NativeSortExtension.Sort<SubObjectOwnerData>(m_OwnerList);
			SubObjectOwnerData subObjectOwnerData = default(SubObjectOwnerData);
			int num = 0;
			int num2 = 0;
			while (num < m_OwnerList.Length)
			{
				SubObjectOwnerData subObjectOwnerData2 = m_OwnerList[num++];
				if (subObjectOwnerData2.m_Owner != subObjectOwnerData.m_Owner)
				{
					if (subObjectOwnerData.m_Owner != Entity.Null && !subObjectOwnerData.m_Deleted)
					{
						m_OwnerList[num2++] = subObjectOwnerData;
					}
					subObjectOwnerData = subObjectOwnerData2;
				}
				else if (subObjectOwnerData2.m_Original != Entity.Null)
				{
					subObjectOwnerData2.m_Deleted |= subObjectOwnerData.m_Deleted;
					subObjectOwnerData = subObjectOwnerData2;
				}
				else
				{
					subObjectOwnerData.m_Deleted |= subObjectOwnerData2.m_Deleted;
				}
			}
			if (subObjectOwnerData.m_Owner != Entity.Null && !subObjectOwnerData.m_Deleted)
			{
				m_OwnerList[num2++] = subObjectOwnerData;
			}
			if (num2 < m_OwnerList.Length)
			{
				m_OwnerList.RemoveRange(num2, m_OwnerList.Length - num2);
			}
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
		public ComponentLookup<NetCompositionData> m_PrefabNetCompositionData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<TrafficLightData> m_PrefabTrafficLightData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.TrafficSignData> m_PrefabTrafficSignData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.StreetLightData> m_PrefabStreetLightData;

		[ReadOnly]
		public ComponentLookup<LaneDirectionData> m_PrefabLaneDirectionData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<ThemeData> m_PrefabThemeData;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> m_PrefabPlaceholderObjectData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.UtilityObjectData> m_PrefabUtilityObjectData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<TrafficLight> m_TrafficLightData;

		[ReadOnly]
		public ComponentLookup<StreetLight> m_StreetLightData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NetNodeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_NetEdgeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_NetCompositionData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> m_UtilityLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<TrafficLights> m_TrafficLightsData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<NetCompositionObject> m_NetCompositionObjects;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PlaceholderObjects;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirements;

		[ReadOnly]
		public BufferLookup<DefaultNetLane> m_DefaultNetLanes;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public Entity m_DefaultTheme;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public ComponentTypeSet m_SecondaryOwnerTypes;

		[ReadOnly]
		public ComponentTypeSet m_TempAnimationTypes;

		[ReadOnly]
		public NativeArray<SubObjectOwnerData> m_OwnerList;

		public ParallelWriter<UpdateData> m_UpdateQueue;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			SubObjectOwnerData subObjectOwnerData = m_OwnerList[index];
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool isNative = m_NativeData.HasComponent(subObjectOwnerData.m_Owner);
			if (m_NetNodeData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag = true;
			}
			else if (m_NetEdgeData.HasComponent(subObjectOwnerData.m_Owner))
			{
				flag2 = true;
			}
			else if (m_AreaNodes.HasBuffer(subObjectOwnerData.m_Owner))
			{
				flag3 = true;
			}
			UpdateSecondaryObjectsData updateData = default(UpdateSecondaryObjectsData);
			DynamicBuffer<SubObject> subObjects = m_SubObjects[subObjectOwnerData.m_Owner];
			FillOldSubObjectsBuffer(subObjectOwnerData.m_Owner, subObjects, ref updateData);
			if (subObjectOwnerData.m_Original != Entity.Null && m_SubObjects.HasBuffer(subObjectOwnerData.m_Original))
			{
				DynamicBuffer<SubObject> subObjects2 = m_SubObjects[subObjectOwnerData.m_Original];
				FillOriginalSubObjectsBuffer(subObjectOwnerData.m_Original, subObjects2, ref updateData);
			}
			Temp ownerTemp = default(Temp);
			if (subObjectOwnerData.m_Temp)
			{
				ownerTemp = m_TempData[subObjectOwnerData.m_Owner];
			}
			Random random = ((!m_PseudoRandomSeedData.HasComponent(subObjectOwnerData.m_Owner)) ? m_RandomSeed.GetRandom(index) : m_PseudoRandomSeedData[subObjectOwnerData.m_Owner].GetRandom(PseudoRandomSeed.kSecondaryObject));
			bool hasStreetLights = false;
			bool alwaysLit = false;
			if (flag)
			{
				CreateSecondaryNodeObjects(index, ref random, subObjectOwnerData.m_Owner, ref updateData, subObjectOwnerData.m_Temp, isNative, ownerTemp, out hasStreetLights, out alwaysLit);
			}
			else if (flag2)
			{
				CreateSecondaryEdgeObjects(index, ref random, subObjectOwnerData.m_Owner, ref updateData, subObjectOwnerData.m_Temp, isNative, ownerTemp, out hasStreetLights, out alwaysLit);
			}
			else if (flag3)
			{
				CreateSecondaryAreaObjects(index, ref random, subObjectOwnerData.m_Owner, ref updateData, subObjectOwnerData.m_Temp, isNative, ownerTemp);
			}
			Road road = default(Road);
			if (m_RoadData.TryGetComponent(subObjectOwnerData.m_Owner, ref road))
			{
				if (hasStreetLights)
				{
					road.m_Flags |= Game.Net.RoadFlags.IsLit;
					if (alwaysLit)
					{
						road.m_Flags |= Game.Net.RoadFlags.AlwaysLit;
					}
					else
					{
						road.m_Flags &= ~Game.Net.RoadFlags.AlwaysLit;
					}
				}
				else
				{
					road.m_Flags &= ~(Game.Net.RoadFlags.IsLit | Game.Net.RoadFlags.AlwaysLit);
				}
				m_RoadData[subObjectOwnerData.m_Owner] = road;
			}
			RemoveUnusedOldSubObjects(index, subObjectOwnerData.m_Owner, subObjects, ref updateData);
			updateData.Dispose();
		}

		private void FillOldSubObjectsBuffer(Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (subObjects.Length == 0)
			{
				return;
			}
			updateData.EnsureOldEntities((Allocator)2);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_OwnerData.HasComponent(subObject) && m_SecondaryData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == owner)
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					updateData.m_OldEntities.Add(prefabRef.m_Prefab, subObject);
				}
			}
		}

		private void FillOriginalSubObjectsBuffer(Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (subObjects.Length == 0)
			{
				return;
			}
			updateData.EnsureOriginalEntities((Allocator)2);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_OwnerData.HasComponent(subObject) && m_SecondaryData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == owner)
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					updateData.m_OriginalEntities.Add(prefabRef.m_Prefab, subObject);
				}
			}
		}

		private void RemoveUnusedOldSubObjects(int jobIndex, Entity owner, DynamicBuffer<SubObject> subObjects, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_OwnerData.HasComponent(subObject) && m_SecondaryData.HasComponent(subObject) && m_OwnerData[subObject].m_Owner == owner)
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					if (updateData.m_OldEntities.TryGetFirstValue(prefabRef.m_Prefab, ref val, ref val2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val, ref m_AppliedTypes);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, val, default(Deleted));
						updateData.m_OldEntities.Remove(val2);
					}
				}
			}
		}

		private float Remap(float t, float4 syncOffsets, float4 syncTargets)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			if (t < syncOffsets.x)
			{
				return syncTargets.x * math.saturate(t / syncOffsets.x);
			}
			if (t < syncOffsets.y)
			{
				return math.lerp(syncTargets.x, syncTargets.y, math.saturate((t - syncOffsets.x) / (syncOffsets.y - syncOffsets.x)));
			}
			if (t < syncOffsets.z)
			{
				return math.lerp(syncTargets.y, syncTargets.z, math.saturate((t - syncOffsets.y) / (syncOffsets.z - syncOffsets.y)));
			}
			if (t < syncOffsets.w)
			{
				return math.lerp(syncTargets.z, syncTargets.w, math.saturate((t - syncOffsets.z) / (syncOffsets.w - syncOffsets.z)));
			}
			return math.lerp(syncTargets.w, 1f, math.saturate((t - syncOffsets.w) / (1f - syncOffsets.w)));
		}

		private Bezier4x3 LerpRemap(Bezier4x3 left, Bezier4x3 right, float t, float4 syncOffsets, float4 syncTargets)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			float num = Remap(t, syncOffsets, syncTargets);
			float num2 = math.distance(((float3)(ref left.a)).xz, ((float3)(ref right.a)).xz);
			float num3 = math.distance(((float3)(ref left.d)).xz, ((float3)(ref right.d)).xz);
			float num4 = 0.5f;
			syncTargets = math.lerp(syncOffsets * num2, syncTargets * num3, num4) / math.max(1E-06f, math.lerp(num2, num3, num4));
			num4 = Remap(t, syncOffsets, syncTargets);
			Bezier4x3 result = default(Bezier4x3);
			result.a = math.lerp(left.a, right.a, t);
			result.b = math.lerp(left.b, right.b, t);
			result.c = math.lerp(left.c, right.c, num4);
			result.d = math.lerp(left.d, right.d, num);
			return result;
		}

		private Bezier4x3 LerpRemap2(Bezier4x3 left, Bezier4x3 right, float t, float4 syncOffsets, float4 syncTargets)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			float num = Remap(t, syncOffsets, syncTargets);
			Bezier4x3 result = default(Bezier4x3);
			result.a = math.lerp(left.a, right.a, num);
			result.b = math.lerp(left.b, right.b, num);
			result.c = math.lerp(left.c, right.c, num);
			result.d = math.lerp(left.d, right.d, num);
			return result;
		}

		private void CreateSecondaryNodeObjects(int jobIndex, ref Random random, Entity owner, ref UpdateSecondaryObjectsData updateData, bool isTemp, bool isNative, Temp ownerTemp, out bool hasStreetLights, out bool alwaysLit)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_1008: Unknown result type (might be due to invalid IL or missing references)
			//IL_100d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1012: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_1025: Unknown result type (might be due to invalid IL or missing references)
			//IL_102a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1032: Unknown result type (might be due to invalid IL or missing references)
			//IL_1044: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d44: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ad2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1499: Unknown result type (might be due to invalid IL or missing references)
			//IL_1056: Unknown result type (might be due to invalid IL or missing references)
			//IL_1067: Unknown result type (might be due to invalid IL or missing references)
			//IL_1075: Unknown result type (might be due to invalid IL or missing references)
			//IL_107a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_108f: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_10aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_10af: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d57: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_14cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_14de: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1500: Unknown result type (might be due to invalid IL or missing references)
			//IL_1505: Unknown result type (might be due to invalid IL or missing references)
			//IL_1507: Unknown result type (might be due to invalid IL or missing references)
			//IL_1148: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1103: Unknown result type (might be due to invalid IL or missing references)
			//IL_1108: Unknown result type (might be due to invalid IL or missing references)
			//IL_1116: Unknown result type (might be due to invalid IL or missing references)
			//IL_1120: Unknown result type (might be due to invalid IL or missing references)
			//IL_1125: Unknown result type (might be due to invalid IL or missing references)
			//IL_112a: Unknown result type (might be due to invalid IL or missing references)
			//IL_112f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d88: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d99: Unknown result type (might be due to invalid IL or missing references)
			//IL_1da7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dac: Unknown result type (might be due to invalid IL or missing references)
			//IL_1837: Unknown result type (might be due to invalid IL or missing references)
			//IL_183f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1519: Unknown result type (might be due to invalid IL or missing references)
			//IL_1522: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_21bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_21ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_1de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1df0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e11: Unknown result type (might be due to invalid IL or missing references)
			//IL_153d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1348: Unknown result type (might be due to invalid IL or missing references)
			//IL_2cbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_21e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_21ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_21fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_169f: Unknown result type (might be due to invalid IL or missing references)
			//IL_154b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1188: Unknown result type (might be due to invalid IL or missing references)
			//IL_13eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1360: Unknown result type (might be due to invalid IL or missing references)
			//IL_1365: Unknown result type (might be due to invalid IL or missing references)
			//IL_136c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1371: Unknown result type (might be due to invalid IL or missing references)
			//IL_1376: Unknown result type (might be due to invalid IL or missing references)
			//IL_1389: Unknown result type (might be due to invalid IL or missing references)
			//IL_138e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_2cd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_223a: Unknown result type (might be due to invalid IL or missing references)
			//IL_226d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2272: Unknown result type (might be due to invalid IL or missing references)
			//IL_2277: Unknown result type (might be due to invalid IL or missing references)
			//IL_227b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2282: Unknown result type (might be due to invalid IL or missing references)
			//IL_2288: Unknown result type (might be due to invalid IL or missing references)
			//IL_228a: Unknown result type (might be due to invalid IL or missing references)
			//IL_228f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2293: Unknown result type (might be due to invalid IL or missing references)
			//IL_2298: Unknown result type (might be due to invalid IL or missing references)
			//IL_229d: Unknown result type (might be due to invalid IL or missing references)
			//IL_22a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_22a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_22ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_22b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_22b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_22b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_22be: Unknown result type (might be due to invalid IL or missing references)
			//IL_22c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_22c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_22cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_22d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_22d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_22da: Unknown result type (might be due to invalid IL or missing references)
			//IL_22df: Unknown result type (might be due to invalid IL or missing references)
			//IL_22e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_22e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_22ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_22f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_22f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_22f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_2313: Unknown result type (might be due to invalid IL or missing references)
			//IL_2318: Unknown result type (might be due to invalid IL or missing references)
			//IL_2325: Unknown result type (might be due to invalid IL or missing references)
			//IL_232a: Unknown result type (might be due to invalid IL or missing references)
			//IL_232c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2346: Unknown result type (might be due to invalid IL or missing references)
			//IL_234b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2358: Unknown result type (might be due to invalid IL or missing references)
			//IL_235d: Unknown result type (might be due to invalid IL or missing references)
			//IL_235f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2379: Unknown result type (might be due to invalid IL or missing references)
			//IL_237e: Unknown result type (might be due to invalid IL or missing references)
			//IL_238b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2390: Unknown result type (might be due to invalid IL or missing references)
			//IL_2392: Unknown result type (might be due to invalid IL or missing references)
			//IL_23ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_23b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_200a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2fee: Unknown result type (might be due to invalid IL or missing references)
			//IL_3026: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b84: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b92: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b97: Unknown result type (might be due to invalid IL or missing references)
			//IL_16cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_125d: Unknown result type (might be due to invalid IL or missing references)
			//IL_119b: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1403: Unknown result type (might be due to invalid IL or missing references)
			//IL_1408: Unknown result type (might be due to invalid IL or missing references)
			//IL_140f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1414: Unknown result type (might be due to invalid IL or missing references)
			//IL_1419: Unknown result type (might be due to invalid IL or missing references)
			//IL_142c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1431: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_2cf8: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d17: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d25: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d32: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d34: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d39: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d42: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d47: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e54: Unknown result type (might be due to invalid IL or missing references)
			//IL_20da: Unknown result type (might be due to invalid IL or missing references)
			//IL_2023: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c11: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c37: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c47: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ca8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cce: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_1749: Unknown result type (might be due to invalid IL or missing references)
			//IL_174e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1270: Unknown result type (might be due to invalid IL or missing references)
			//IL_1277: Unknown result type (might be due to invalid IL or missing references)
			//IL_127c: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_23d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_23d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_23e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_23e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_24a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_24ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_24b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_24c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f24: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e67: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e73: Unknown result type (might be due to invalid IL or missing references)
			//IL_20f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_205b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2060: Unknown result type (might be due to invalid IL or missing references)
			//IL_2067: Unknown result type (might be due to invalid IL or missing references)
			//IL_206c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2071: Unknown result type (might be due to invalid IL or missing references)
			//IL_2084: Unknown result type (might be due to invalid IL or missing references)
			//IL_2089: Unknown result type (might be due to invalid IL or missing references)
			//IL_2030: Unknown result type (might be due to invalid IL or missing references)
			//IL_30ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_30d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_176e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1586: Unknown result type (might be due to invalid IL or missing references)
			//IL_1598: Unknown result type (might be due to invalid IL or missing references)
			//IL_159d: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_15bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_15cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d80: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d85: Unknown result type (might be due to invalid IL or missing references)
			//IL_2d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_2da5: Unknown result type (might be due to invalid IL or missing references)
			//IL_2daa: Unknown result type (might be due to invalid IL or missing references)
			//IL_2db6: Unknown result type (might be due to invalid IL or missing references)
			//IL_2dca: Unknown result type (might be due to invalid IL or missing references)
			//IL_2dce: Unknown result type (might be due to invalid IL or missing references)
			//IL_2de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_2dec: Unknown result type (might be due to invalid IL or missing references)
			//IL_23f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_2401: Unknown result type (might be due to invalid IL or missing references)
			//IL_240d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2416: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f37: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f43: Unknown result type (might be due to invalid IL or missing references)
			//IL_212b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2130: Unknown result type (might be due to invalid IL or missing references)
			//IL_2137: Unknown result type (might be due to invalid IL or missing references)
			//IL_213c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2141: Unknown result type (might be due to invalid IL or missing references)
			//IL_2154: Unknown result type (might be due to invalid IL or missing references)
			//IL_2159: Unknown result type (might be due to invalid IL or missing references)
			//IL_2100: Unknown result type (might be due to invalid IL or missing references)
			//IL_2043: Unknown result type (might be due to invalid IL or missing references)
			//IL_11db: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_099c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_093f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_2e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_2e4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_2447: Unknown result type (might be due to invalid IL or missing references)
			//IL_2450: Unknown result type (might be due to invalid IL or missing references)
			//IL_245c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2465: Unknown result type (might be due to invalid IL or missing references)
			//IL_2113: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1611: Unknown result type (might be due to invalid IL or missing references)
			//IL_1615: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2eba: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ebc: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ec3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ea2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1949: Unknown result type (might be due to invalid IL or missing references)
			//IL_1961: Unknown result type (might be due to invalid IL or missing references)
			//IL_1626: Unknown result type (might be due to invalid IL or missing references)
			//IL_162f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2790: Unknown result type (might be due to invalid IL or missing references)
			//IL_2799: Unknown result type (might be due to invalid IL or missing references)
			//IL_279b: Unknown result type (might be due to invalid IL or missing references)
			//IL_27ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_27bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_27c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f72: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f77: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0876: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0829: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_1fe8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_2af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa4: Unknown result type (might be due to invalid IL or missing references)
			alwaysLit = false;
			float num = 0f;
			float3 val = default(float3);
			int num2 = 0;
			float ownerElevation = 0f;
			if (m_NetElevationData.HasComponent(owner))
			{
				ownerElevation = math.cmin(m_NetElevationData[owner].m_Elevation);
			}
			uint num3 = 0u;
			uint num4 = 0u;
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_NetEdgeData, m_TempData, m_HiddenData, includeMiddleConnections: true);
			EdgeIteratorValue value;
			float3 val4;
			float2 val6 = default(float2);
			Bounds1 val11 = default(Bounds1);
			Bezier4x3 val12 = default(Bezier4x3);
			Bezier4x3 val13 = default(Bezier4x3);
			Bounds1 val16 = default(Bounds1);
			Bounds1 val17 = default(Bounds1);
			Bounds1 val19 = default(Bounds1);
			SlaveLane slaveLane = default(SlaveLane);
			Game.Net.CarLane carLane2 = default(Game.Net.CarLane);
			while (edgeIterator.GetNext(out value))
			{
				int num5 = 0;
				if (updateData.m_TrafficSigns.IsCreated)
				{
					num5 = updateData.m_TrafficSigns.Length;
				}
				if (!value.m_Middle)
				{
					Composition composition = m_NetCompositionData[value.m_Edge];
					EdgeGeometry edgeGeometry = m_EdgeGeometryData[value.m_Edge];
					NetCompositionData netCompositionData = m_PrefabNetCompositionData[composition.m_Edge];
					EdgeNodeGeometry geometry;
					NetCompositionData netCompositionData2;
					DynamicBuffer<NetCompositionObject> val2;
					if (value.m_End)
					{
						geometry = m_EndNodeGeometryData[value.m_Edge].m_Geometry;
						netCompositionData2 = m_PrefabNetCompositionData[composition.m_EndNode];
						val2 = m_NetCompositionObjects[composition.m_EndNode];
					}
					else
					{
						geometry = m_StartNodeGeometryData[value.m_Edge].m_Geometry;
						netCompositionData2 = m_PrefabNetCompositionData[composition.m_StartNode];
						val2 = m_NetCompositionObjects[composition.m_StartNode];
					}
					alwaysLit |= (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0 && ((netCompositionData2.m_Flags.m_Left | netCompositionData2.m_Flags.m_Right) & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) == 0;
					bool isLowered = ((netCompositionData2.m_Flags.m_Left | netCompositionData2.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0;
					float3 val3 = ((!value.m_End) ? (-(MathUtils.StartTangent(edgeGeometry.m_Start.m_Left) + MathUtils.StartTangent(edgeGeometry.m_Start.m_Right))) : (MathUtils.EndTangent(edgeGeometry.m_End.m_Left) + MathUtils.StartTangent(edgeGeometry.m_End.m_Right)));
					float2 val5;
					if (geometry.m_MiddleRadius > 0f)
					{
						num += math.max(math.distance(geometry.m_Middle.a, geometry.m_Middle.d) * 2f, geometry.m_Left.m_Length.x + geometry.m_Left.m_Length.y + geometry.m_Right.m_Length.x + geometry.m_Right.m_Length.y);
						val4 = MathUtils.StartTangent(geometry.m_Left.m_Left);
						float2 xz = ((float3)(ref val4)).xz;
						val4 = MathUtils.StartTangent(geometry.m_Left.m_Right);
						val5 = xz + ((float3)(ref val4)).xz;
					}
					else
					{
						num += math.max(math.distance(geometry.m_Middle.a, geometry.m_Middle.d) * 2f, geometry.m_Left.m_Length.x + geometry.m_Right.m_Length.y);
						if (math.any(geometry.m_Left.m_Length > 0.05f) | math.any(geometry.m_Right.m_Length > 0.05f))
						{
							val4 = MathUtils.StartTangent(geometry.m_Left.m_Left);
							float2 xz2 = ((float3)(ref val4)).xz;
							val4 = MathUtils.StartTangent(geometry.m_Right.m_Right);
							val5 = xz2 + ((float3)(ref val4)).xz;
						}
						else
						{
							val5 = ((float3)(ref val3)).xz;
						}
					}
					PrefabRef prefabRef = m_PrefabRefData[value.m_Edge];
					NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
					TrafficSignNeeds trafficSignNeeds = new TrafficSignNeeds
					{
						m_Left = LaneDirectionType.None,
						m_Forward = LaneDirectionType.None,
						m_Right = LaneDirectionType.None
					};
					if ((netGeometryData.m_MergeLayers & (Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad)) != Layer.None)
					{
						val5 = math.normalizesafe(val5, default(float2));
						float num6 = math.atan2(val5.x, val5.y);
						uint num7 = (uint)(1 << (Mathf.FloorToInt(num6 * (16f / (float)Math.PI)) & 0x1F));
						if (((uint)netCompositionData.m_State & (uint)(value.m_End ? 8 : 64)) != 0)
						{
							num3 |= num7;
						}
						if (((uint)netCompositionData.m_State & (uint)(value.m_End ? 64 : 8)) != 0)
						{
							num4 |= num7;
						}
						if ((netCompositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes)) == (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes))
						{
							trafficSignNeeds.m_RemoveSignTypeMask2 = Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.DoNotEnter) | Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Oneway);
						}
						if ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Intersection) == 0)
						{
							trafficSignNeeds.m_RemoveSignTypeMask = Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Street) | Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnLeft) | Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnRight) | Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnLeft) | Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnRight);
						}
						if ((netGeometryData.m_IntersectLayers & Layer.Waterway) == 0 || (netCompositionData.m_Flags.m_General & CompositionFlags.General.FixedNodeSize) == 0 || (netCompositionData2.m_Flags.m_General & CompositionFlags.General.FixedNodeSize) != 0)
						{
							trafficSignNeeds.m_RemoveSignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.MoveableBridge);
						}
					}
					val = geometry.m_Middle.d;
					num2++;
					for (int i = 0; i < val2.Length; i++)
					{
						NetCompositionObject netCompositionObject = val2[i];
						float num8 = netCompositionObject.m_Position.x / math.max(1f, netCompositionData2.m_Width) + 0.5f;
						float num9 = netCompositionData2.m_MiddleOffset / math.max(1f, netCompositionData2.m_Width) + 0.5f;
						((float2)(ref val6))._002Ector(math.cmin(netCompositionObject.m_CurveOffsetRange), math.cmax(netCompositionObject.m_CurveOffsetRange));
						if (val6.y > 0.5f)
						{
							if (val6.x > 0.5f)
							{
								val6 = 1f - ((float2)(ref val6)).yx;
							}
							else
							{
								((float2)(ref val6))._002Ector((val6.x + 1f - val6.y) * 0.5f, 0.5f);
							}
						}
						float num10 = math.lerp(val6.x, val6.y, ((Random)(ref random)).NextFloat(1f));
						float num11;
						float3 position;
						float3 val9;
						float3 val10;
						if ((netCompositionObject.m_Flags & (SubObjectFlags.OnMedian | SubObjectFlags.PreserveShape)) != 0)
						{
							Bezier4x3 middle = geometry.m_Middle;
							float3 val7;
							float3 val8;
							if (geometry.m_MiddleRadius > 0f)
							{
								val7 = geometry.m_Left.m_Right.a - geometry.m_Left.m_Left.a;
								val8 = geometry.m_Right.m_Right.d - geometry.m_Right.m_Left.d;
							}
							else
							{
								val7 = geometry.m_Right.m_Right.a - geometry.m_Left.m_Left.a;
								val8 = geometry.m_Right.m_Right.d - geometry.m_Left.m_Left.d;
							}
							if ((netCompositionObject.m_Flags & SubObjectFlags.PreserveShape) != 0)
							{
								val8 = val7;
							}
							num11 = MathUtils.Length(((Bezier4x3)(ref middle)).xz);
							if (num11 < 0.01f)
							{
								position = middle.a + val7 * (num8 - num9);
								val9 = ((!(geometry.m_MiddleRadius > 0f)) ? (MathUtils.StartTangent(geometry.m_Left.m_Left) + MathUtils.StartTangent(geometry.m_Right.m_Right)) : (MathUtils.StartTangent(geometry.m_Left.m_Left) + MathUtils.StartTangent(geometry.m_Left.m_Right)));
								val10 = val9;
							}
							else
							{
								val9 = MathUtils.StartTangent(middle);
								float num12 = num10 * 2f * num11 + netCompositionObject.m_Position.y;
								if (num12 > 0.001f)
								{
									((Bounds1)(ref val11))._002Ector(0f, 1f);
									MathUtils.ClampLength(((Bezier4x3)(ref middle)).xz, ref val11, num12);
									position = MathUtils.Position(middle, val11.max) + math.lerp(val7, val8, val11.max) * (num8 - num9);
									val10 = MathUtils.Tangent(middle, val11.max);
								}
								else
								{
									position = middle.a + val7 * (num8 - num9);
									val10 = val9;
								}
							}
							if (geometry.m_MiddleRadius == 0f && math.all(geometry.m_Left.m_Length <= 0.05f) && math.all(geometry.m_Right.m_Length < 0.05f))
							{
								val9 = val3;
								val10 = val3;
							}
						}
						else if (geometry.m_MiddleRadius > 0f)
						{
							MathUtils.Divide(geometry.m_Middle, ref val12, ref val13, 0.99f);
							Bezier4x3 val14;
							Bezier4x3 val15;
							if (num8 >= num9)
							{
								val14 = LerpRemap(val12, geometry.m_Left.m_Right, (num8 - num9) / math.max(1E-05f, 1f - num9), netCompositionData2.m_SyncVertexOffsetsRight, geometry.m_SyncVertexTargetsRight);
								val15 = LerpRemap2(val13, geometry.m_Right.m_Right, (num8 - num9) / math.max(1E-05f, 1f - num9), netCompositionData2.m_SyncVertexOffsetsRight, geometry.m_SyncVertexTargetsRight);
							}
							else
							{
								val14 = LerpRemap(geometry.m_Left.m_Left, val12, num8 / math.max(1E-05f, num9), netCompositionData2.m_SyncVertexOffsetsLeft, geometry.m_SyncVertexTargetsLeft);
								val15 = LerpRemap2(geometry.m_Right.m_Left, val13, num8 / math.max(1E-05f, num9), netCompositionData2.m_SyncVertexOffsetsLeft, geometry.m_SyncVertexTargetsLeft);
							}
							float num13 = MathUtils.Length(((Bezier4x3)(ref val14)).xz);
							float num14 = MathUtils.Length(((Bezier4x3)(ref val15)).xz);
							num11 = num13 + num14;
							val9 = MathUtils.StartTangent(val14);
							float num15 = num10 * 2f * num11 + netCompositionObject.m_Position.y;
							if (num15 > num13)
							{
								((Bounds1)(ref val16))._002Ector(0f, 1f);
								MathUtils.ClampLength(((Bezier4x3)(ref val15)).xz, ref val16, num15 - num13);
								position = MathUtils.Position(val15, val16.max);
								val10 = MathUtils.Tangent(val15, val16.max);
							}
							else if (num15 > 0.001f)
							{
								((Bounds1)(ref val17))._002Ector(0f, 1f);
								MathUtils.ClampLength(((Bezier4x3)(ref val14)).xz, ref val17, num15);
								position = MathUtils.Position(val14, val17.max);
								val10 = MathUtils.Tangent(val14, val17.max);
							}
							else
							{
								position = val14.a;
								val10 = val9;
							}
						}
						else
						{
							Bezier4x3 val18 = ((!(num8 >= num9)) ? LerpRemap(geometry.m_Left.m_Left, geometry.m_Left.m_Right, num8 / math.max(1E-05f, num9), netCompositionData2.m_SyncVertexOffsetsLeft, geometry.m_SyncVertexTargetsLeft) : LerpRemap(geometry.m_Right.m_Left, geometry.m_Right.m_Right, (num8 - num9) / math.max(1E-05f, 1f - num9), netCompositionData2.m_SyncVertexOffsetsRight, geometry.m_SyncVertexTargetsRight));
							num11 = MathUtils.Length(((Bezier4x3)(ref val18)).xz);
							val9 = MathUtils.StartTangent(val18);
							float num16 = num10 * 2f * num11 + netCompositionObject.m_Position.y;
							if (num16 > 0.001f)
							{
								((Bounds1)(ref val19))._002Ector(0f, 1f);
								MathUtils.ClampLength(((Bezier4x3)(ref val18)).xz, ref val19, num16);
								position = MathUtils.Position(val18, val19.max);
								val10 = MathUtils.Tangent(val18, val19.max);
							}
							else
							{
								position = val18.a;
								val10 = val9;
							}
							if (math.all(geometry.m_Left.m_Length <= 0.05f) && math.all(geometry.m_Right.m_Length < 0.05f))
							{
								val9 = val3;
								val10 = val3;
							}
						}
						val9.y = math.lerp(0f, val9.y, netCompositionObject.m_UseCurveRotation.x);
						val10.y = math.lerp(0f, val10.y, netCompositionObject.m_UseCurveRotation.x);
						val9 = math.normalizesafe(val9, new float3(0f, 0f, 1f));
						val10 = math.normalizesafe(val10, val9);
						quaternion val20 = quaternion.LookRotationSafe(val9, math.up());
						quaternion val21 = quaternion.LookRotationSafe(val10, math.up());
						quaternion rotation = math.slerp(val20, val21, netCompositionObject.m_UseCurveRotation.y);
						Transform transform = new Transform(position, rotation);
						Transform transform2 = new Transform(netCompositionObject.m_Offset, netCompositionObject.m_Rotation);
						Transform transform3 = ObjectUtils.LocalToWorld(transform, transform2);
						if (netCompositionObject.m_Probability < 100)
						{
							netCompositionObject.m_Probability = math.clamp(Mathf.RoundToInt((float)netCompositionObject.m_Probability * (num11 / netGeometryData.m_EdgeLengthRange.max)), 1, netCompositionObject.m_Probability);
						}
						if (m_PrefabStreetLightData.HasComponent(netCompositionObject.m_Prefab))
						{
							Game.Prefabs.StreetLightData streetLightData = m_PrefabStreetLightData[netCompositionObject.m_Prefab];
							updateData.EnsureStreetLights((Allocator)2);
							int num17 = 0;
							while (true)
							{
								if (num17 < updateData.m_StreetLights.Length)
								{
									if (math.distancesq(updateData.m_StreetLights[num17].m_ObjectTransform.m_Position, transform3.m_Position) < 1f)
									{
										break;
									}
									num17++;
									continue;
								}
								ref NativeList<StreetLightData> reference = ref updateData.m_StreetLights;
								StreetLightData streetLightData2 = new StreetLightData
								{
									m_ParentTransform = transform,
									m_ObjectTransform = transform3,
									m_LocalTransform = transform2,
									m_Prefab = netCompositionObject.m_Prefab,
									m_Probability = netCompositionObject.m_Probability,
									m_Flags = netCompositionObject.m_Flags,
									m_Layer = streetLightData.m_Layer,
									m_Spacing = netCompositionObject.m_Spacing,
									m_Priority = num11,
									m_IsLowered = isLowered
								};
								reference.Add(ref streetLightData2);
								break;
							}
							continue;
						}
						if (m_PrefabTrafficSignData.HasComponent(netCompositionObject.m_Prefab) || m_PrefabTrafficLightData.HasComponent(netCompositionObject.m_Prefab))
						{
							updateData.EnsureTrafficSigns((Allocator)2);
							ref NativeList<TrafficSignData> reference2 = ref updateData.m_TrafficSigns;
							TrafficSignData trafficSignData = new TrafficSignData
							{
								m_ParentTransform = transform,
								m_ObjectTransform = transform3,
								m_LocalTransform = transform2
							};
							val4 = math.forward(transform3.m_Rotation);
							trafficSignData.m_ForwardDirection = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
							trafficSignData.m_Prefab = netCompositionObject.m_Prefab;
							trafficSignData.m_Probability = netCompositionObject.m_Probability;
							trafficSignData.m_Flags = netCompositionObject.m_Flags;
							trafficSignData.m_TrafficSignNeeds = trafficSignNeeds;
							trafficSignData.m_IsLowered = isLowered;
							reference2.Add(ref trafficSignData);
							continue;
						}
						if (m_PrefabUtilityObjectData.HasComponent(netCompositionObject.m_Prefab))
						{
							Game.Prefabs.UtilityObjectData utilityObjectData = m_PrefabUtilityObjectData[netCompositionObject.m_Prefab];
							float3 val22 = ObjectUtils.LocalToWorld(transform3, utilityObjectData.m_UtilityPosition);
							updateData.EnsureUtilityObjects((Allocator)2);
							int num18 = 0;
							while (num18 < updateData.m_UtilityObjects.Length)
							{
								if (!(math.distancesq(updateData.m_UtilityObjects[num18].m_UtilityPosition, val22) < 1f))
								{
									num18++;
									continue;
								}
								goto IL_0fec;
							}
							ref NativeList<UtilityObjectData> reference3 = ref updateData.m_UtilityObjects;
							UtilityObjectData utilityObjectData2 = new UtilityObjectData
							{
								m_UtilityPosition = val22
							};
							reference3.Add(ref utilityObjectData2);
						}
						CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, transform, transform3, transform2, netCompositionObject.m_Flags, default(TrafficSignNeeds), ref updateData, netCompositionObject.m_Prefab, 0, netCompositionObject.m_Probability);
						IL_0fec:;
					}
				}
				DynamicBuffer<Game.Net.SubLane> val23 = m_SubLanes[value.m_Edge];
				for (int j = 0; j < val23.Length; j++)
				{
					Entity subLane = val23[j].m_SubLane;
					if (m_SecondaryLaneData.HasComponent(subLane))
					{
						continue;
					}
					if (m_UtilityLaneData.HasComponent(subLane))
					{
						PrefabRef prefabRef2 = m_PrefabRefData[subLane];
						UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef2.m_Prefab];
						if (!(utilityLaneData.m_NodeObjectPrefab != Entity.Null))
						{
							continue;
						}
						Curve curve = m_CurveData[subLane];
						float num19 = math.length(MathUtils.Size(m_PrefabObjectGeometryData[utilityLaneData.m_NodeObjectPrefab].m_Bounds));
						bool2 val24 = bool2.op_Implicit(false);
						bool flag = false;
						if (m_EdgeLaneData.HasComponent(subLane))
						{
							if (!value.m_Middle)
							{
								EdgeLane edgeLane = m_EdgeLaneData[subLane];
								val24 = (!value.m_End & (edgeLane.m_EdgeDelta == 0f)) | (value.m_End & (edgeLane.m_EdgeDelta == 1f));
							}
						}
						else if (value.m_Middle)
						{
							((bool2)(ref val24))._002Ector(false, true);
							flag = true;
						}
						if (!math.any(val24))
						{
							continue;
						}
						updateData.EnsureUtilityNodes((Allocator)2);
						for (int k = 0; k < updateData.m_UtilityNodes.Length; k++)
						{
							UtilityNodeData utilityNodeData = updateData.m_UtilityNodes[k];
							if ((utilityNodeData.m_UtilityTypes & utilityLaneData.m_UtilityTypes) == 0)
							{
								continue;
							}
							if (val24.x && math.distancesq(utilityNodeData.m_Transform.m_Position, curve.m_Bezier.a) < 0.01f)
							{
								utilityNodeData.m_Unsure &= flag;
								if (!flag && num19 > utilityNodeData.m_NodePriority)
								{
									utilityNodeData.m_Prefab = utilityLaneData.m_NodeObjectPrefab;
									utilityNodeData.m_NodePriority = num19;
								}
								if (num19 > utilityNodeData.m_LanePriority)
								{
									utilityNodeData.m_LanePriority = num19;
									utilityNodeData.m_Count = 1;
									utilityNodeData.m_Vertical = flag;
								}
								else if (num19 == utilityNodeData.m_LanePriority)
								{
									utilityNodeData.m_Count++;
									utilityNodeData.m_Vertical |= flag;
								}
								updateData.m_UtilityNodes[k] = utilityNodeData;
								val24.x = false;
								if (!val24.y)
								{
									break;
								}
							}
							if (val24.y && math.distancesq(utilityNodeData.m_Transform.m_Position, curve.m_Bezier.d) < 0.01f)
							{
								utilityNodeData.m_Unsure &= flag;
								if (!flag && num19 > utilityNodeData.m_NodePriority)
								{
									utilityNodeData.m_Prefab = utilityLaneData.m_NodeObjectPrefab;
									utilityNodeData.m_NodePriority = num19;
								}
								if (num19 > utilityNodeData.m_LanePriority)
								{
									utilityNodeData.m_LanePriority = num19;
									utilityNodeData.m_Count = 1;
									utilityNodeData.m_Vertical = flag;
								}
								else if (num19 == utilityNodeData.m_LanePriority)
								{
									utilityNodeData.m_Count++;
									utilityNodeData.m_Vertical |= flag;
								}
								updateData.m_UtilityNodes[k] = utilityNodeData;
								val24.y = false;
								if (!val24.x)
								{
									break;
								}
							}
						}
						if (val24.x)
						{
							UtilityNodeData utilityNodeData2 = new UtilityNodeData
							{
								m_Transform = new Transform(curve.m_Bezier.a, NetUtils.GetNodeRotation(MathUtils.StartTangent(curve.m_Bezier))),
								m_Prefab = utilityLaneData.m_NodeObjectPrefab,
								m_Count = 1,
								m_LanePriority = num19,
								m_NodePriority = math.select(num19, 0f, flag),
								m_UtilityTypes = utilityLaneData.m_UtilityTypes,
								m_Unsure = flag,
								m_Vertical = flag
							};
							updateData.m_UtilityNodes.Add(ref utilityNodeData2);
						}
						if (val24.y)
						{
							UtilityNodeData utilityNodeData3 = new UtilityNodeData
							{
								m_Transform = new Transform(curve.m_Bezier.d, NetUtils.GetNodeRotation(MathUtils.EndTangent(curve.m_Bezier))),
								m_Prefab = utilityLaneData.m_NodeObjectPrefab,
								m_Count = 1,
								m_LanePriority = num19,
								m_NodePriority = math.select(num19, 0f, flag),
								m_UtilityTypes = utilityLaneData.m_UtilityTypes,
								m_Unsure = flag,
								m_Vertical = flag
							};
							updateData.m_UtilityNodes.Add(ref utilityNodeData3);
						}
					}
					else
					{
						if (!m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane) || !m_EdgeLaneData.HasComponent(subLane))
						{
							continue;
						}
						Game.Net.CarLane carLane = m_CarLaneData[subLane];
						bool2 val25 = m_EdgeLaneData[subLane].m_EdgeDelta == math.select(0f, 1f, value.m_End);
						if (!math.any(val25))
						{
							continue;
						}
						Lane lane = m_LaneData[subLane];
						PathNode pathNode = (val25.x ? lane.m_StartNode : lane.m_EndNode);
						if (val25.x && pathNode.OwnerEquals(new PathNode(owner, 0)) && updateData.m_TrafficSigns.IsCreated && updateData.m_TrafficSigns.Length > num5)
						{
							Curve curve2 = m_CurveData[subLane];
							int num20 = -1;
							float num21 = float.MaxValue;
							val4 = MathUtils.StartTangent(curve2.m_Bezier);
							float2 val26 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
							float3 a = curve2.m_Bezier.a;
							((float3)(ref a)).xz = ((float3)(ref a)).xz + MathUtils.Right(val26) * math.select(1.25f, -1.25f, m_LeftHandTraffic);
							for (int l = num5; l < updateData.m_TrafficSigns.Length; l++)
							{
								TrafficSignData trafficSignData2 = updateData.m_TrafficSigns[l];
								if (math.dot(val26, trafficSignData2.m_ForwardDirection) > 0.70710677f)
								{
									float num22 = math.distance(a, trafficSignData2.m_ObjectTransform.m_Position);
									if (num22 < num21)
									{
										num20 = l;
										num21 = num22;
									}
								}
							}
							if (num20 != -1)
							{
								TrafficSignData trafficSignData3 = updateData.m_TrafficSigns[num20];
								trafficSignData3.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.MoveableBridge);
								updateData.m_TrafficSigns[num20] = trafficSignData3;
							}
						}
						if (!pathNode.OwnerEquals(new PathNode(value.m_Edge, 0)))
						{
							continue;
						}
						updateData.EnsureTargetLanes((Allocator)2);
						CarLaneFlags carLaneFlags = carLane.m_Flags;
						if (m_SlaveLaneData.TryGetComponent(subLane, ref slaveLane))
						{
							for (int m = slaveLane.m_MinIndex; m <= slaveLane.m_MaxIndex; m++)
							{
								if (m != j && m_CarLaneData.TryGetComponent(val23[m].m_SubLane, ref carLane2))
								{
									carLaneFlags |= carLane2.m_Flags;
								}
							}
						}
						TargetLaneData targetLaneData = new TargetLaneData
						{
							m_CarLaneFlags = carLane.m_Flags,
							m_AndCarLaneFlags = carLaneFlags,
							m_SpeedLimit = float2.op_Implicit(carLane.m_DefaultSpeedLimit)
						};
						if (!updateData.m_TargetLanes.TryAdd(pathNode, targetLaneData))
						{
							Debug.Log((object)$"SecondaryObjectSystem: Duplicate node for lane {subLane.Index}");
						}
					}
				}
			}
			hasStreetLights = updateData.m_StreetLights.IsCreated && updateData.m_StreetLights.Length != 0;
			bool flag2 = updateData.m_TrafficSigns.IsCreated && updateData.m_TrafficSigns.Length != 0;
			if (hasStreetLights)
			{
				while (updateData.m_StreetLights.Length != 0)
				{
					StreetLightLayer streetLightLayer = updateData.m_StreetLights[0].m_Layer;
					float num23 = -1f;
					int num24 = 0;
					float num25 = 0f;
					int num26 = 0;
					for (int n = 0; n < updateData.m_StreetLights.Length; n++)
					{
						StreetLightData streetLightData3 = updateData.m_StreetLights[n];
						if (streetLightData3.m_Layer == streetLightLayer)
						{
							float num27 = math.distance(val, streetLightData3.m_ObjectTransform.m_Position) + streetLightData3.m_Priority;
							if (num27 > num23)
							{
								num23 = num27;
								num24 = n;
							}
							num25 += streetLightData3.m_Spacing;
							num26++;
						}
					}
					if (num24 != 0)
					{
						StreetLightData streetLightData4 = updateData.m_StreetLights[num24];
						updateData.m_StreetLights[num24] = updateData.m_StreetLights[0];
						updateData.m_StreetLights[0] = streetLightData4;
					}
					int num28 = math.min(num26, Mathf.RoundToInt(num * (float)num26 / math.max(1f, num25)));
					num28 = math.select(num28, 2, num28 == 3 && num2 == 4);
					for (int num29 = 1; num29 < num28; num29++)
					{
						num23 = -1f;
						num24 = num29;
						for (int num30 = num29; num30 < updateData.m_StreetLights.Length; num30++)
						{
							StreetLightData streetLightData5 = updateData.m_StreetLights[num30];
							if (streetLightData5.m_Layer == streetLightLayer)
							{
								float num31 = float.MaxValue;
								for (int num32 = 0; num32 < num29; num32++)
								{
									num31 = math.min(num31, math.distance(streetLightData5.m_ObjectTransform.m_Position, updateData.m_StreetLights[num32].m_ObjectTransform.m_Position));
								}
								num31 += streetLightData5.m_Priority;
								if (num31 > num23)
								{
									num23 = num31;
									num24 = num30;
								}
							}
						}
						if (num24 != num29)
						{
							StreetLightData streetLightData6 = updateData.m_StreetLights[num24];
							updateData.m_StreetLights[num24] = updateData.m_StreetLights[num29];
							updateData.m_StreetLights[num29] = streetLightData6;
						}
					}
					for (int num33 = 0; num33 < num28; num33++)
					{
						StreetLightData streetLightData7 = updateData.m_StreetLights[num33];
						CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, streetLightData7.m_IsLowered, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, streetLightData7.m_ParentTransform, streetLightData7.m_ObjectTransform, streetLightData7.m_LocalTransform, streetLightData7.m_Flags, default(TrafficSignNeeds), ref updateData, streetLightData7.m_Prefab, 0, streetLightData7.m_Probability);
					}
					int num34 = 0;
					while (num34 < updateData.m_StreetLights.Length)
					{
						if (updateData.m_StreetLights[num34].m_Layer != streetLightLayer)
						{
							num34++;
						}
						else
						{
							updateData.m_StreetLights.RemoveAtSwapBack(num34);
						}
					}
				}
			}
			if (num2 == 0)
			{
				PrefabRef prefabRef3 = m_PrefabRefData[owner];
				DynamicBuffer<DefaultNetLane> val27 = default(DynamicBuffer<DefaultNetLane>);
				if (m_DefaultNetLanes.TryGetBuffer(prefabRef3.m_Prefab, ref val27))
				{
					Game.Net.Node node = m_NetNodeData[owner];
					NetGeometryData netGeometryData2 = m_PrefabNetGeometryData[prefabRef3.m_Prefab];
					for (int num35 = 0; num35 < val27.Length; num35++)
					{
						NetCompositionLane netCompositionLane = new NetCompositionLane(val27[num35]);
						if ((netCompositionLane.m_Flags & LaneFlags.Utility) == 0 || (netCompositionLane.m_Flags & LaneFlags.FindAnchor) != 0)
						{
							continue;
						}
						bool flag3 = (netCompositionLane.m_Flags & LaneFlags.Invert) != 0;
						if (((uint)netCompositionLane.m_Flags & (uint)(flag3 ? 512 : 256)) == 0)
						{
							UtilityLaneData utilityLaneData2 = m_PrefabUtilityLaneData[netCompositionLane.m_Lane];
							if (utilityLaneData2.m_NodeObjectPrefab != Entity.Null)
							{
								float num36 = math.length(MathUtils.Size(m_PrefabObjectGeometryData[utilityLaneData2.m_NodeObjectPrefab].m_Bounds));
								netCompositionLane.m_Position.x = 0f - netCompositionLane.m_Position.x;
								float num37 = netCompositionLane.m_Position.x / math.max(1f, netGeometryData2.m_DefaultWidth) + 0.5f;
								float3 val28 = node.m_Position + math.rotate(node.m_Rotation, new float3(netGeometryData2.m_DefaultWidth * -0.5f, 0f, 0f));
								float3 position2 = math.lerp(node.m_Position + math.rotate(node.m_Rotation, new float3(netGeometryData2.m_DefaultWidth * 0.5f, 0f, 0f)), val28, num37);
								UtilityNodeData utilityNodeData4 = new UtilityNodeData
								{
									m_Transform = 
									{
										m_Position = position2
									}
								};
								utilityNodeData4.m_Transform.m_Position.y += netCompositionLane.m_Position.y;
								utilityNodeData4.m_Transform.m_Rotation = quaternion.identity;
								utilityNodeData4.m_Prefab = utilityLaneData2.m_NodeObjectPrefab;
								utilityNodeData4.m_Count = 1;
								utilityNodeData4.m_Elevation = netCompositionLane.m_Position.y;
								utilityNodeData4.m_LanePriority = num36;
								utilityNodeData4.m_NodePriority = num36;
								utilityNodeData4.m_UtilityTypes = utilityLaneData2.m_UtilityTypes;
								updateData.EnsureUtilityNodes((Allocator)2);
								updateData.m_UtilityNodes.Add(ref utilityNodeData4);
							}
						}
					}
				}
			}
			DynamicBuffer<Game.Net.SubLane> val29 = m_SubLanes[owner];
			Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
			Game.Net.Elevation elevation4 = default(Game.Net.Elevation);
			TargetLaneData targetLaneData2 = default(TargetLaneData);
			TargetLaneData targetLaneData3 = default(TargetLaneData);
			TargetLaneData targetLaneData4 = default(TargetLaneData);
			TargetLaneData targetLaneData5 = default(TargetLaneData);
			TargetLaneData targetLaneData6 = default(TargetLaneData);
			TargetLaneData targetLaneData7 = default(TargetLaneData);
			for (int num38 = 0; num38 < val29.Length; num38++)
			{
				Entity subLane2 = val29[num38].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane2))
				{
					continue;
				}
				if (m_UtilityLaneData.HasComponent(subLane2))
				{
					PrefabRef prefabRef4 = m_PrefabRefData[subLane2];
					UtilityLaneData utilityLaneData3 = m_PrefabUtilityLaneData[prefabRef4.m_Prefab];
					if (!(utilityLaneData3.m_NodeObjectPrefab != Entity.Null))
					{
						continue;
					}
					Curve curve3 = m_CurveData[subLane2];
					if (curve3.m_Length <= 0.1f)
					{
						continue;
					}
					Game.Net.UtilityLane utilityLane = m_UtilityLaneData[subLane2];
					float num39 = math.length(MathUtils.Size(m_PrefabObjectGeometryData[utilityLaneData3.m_NodeObjectPrefab].m_Bounds));
					bool2 val30 = bool2.op_Implicit(false);
					bool flag4 = (utilityLane.m_Flags & UtilityLaneFlags.VerticalConnection) != 0;
					updateData.EnsureUtilityNodes((Allocator)2);
					for (int num40 = 0; num40 < updateData.m_UtilityNodes.Length; num40++)
					{
						UtilityNodeData utilityNodeData5 = updateData.m_UtilityNodes[num40];
						if ((utilityNodeData5.m_UtilityTypes & utilityLaneData3.m_UtilityTypes) == 0)
						{
							continue;
						}
						if (!val30.x && math.distancesq(utilityNodeData5.m_Transform.m_Position, curve3.m_Bezier.a) < 0.01f)
						{
							utilityNodeData5.m_Unsure = false;
							if (!flag4 && num39 > utilityNodeData5.m_NodePriority)
							{
								utilityNodeData5.m_Prefab = utilityLaneData3.m_NodeObjectPrefab;
								utilityNodeData5.m_NodePriority = num39;
							}
							if (num39 > utilityNodeData5.m_LanePriority)
							{
								utilityNodeData5.m_LanePriority = num39;
								utilityNodeData5.m_Count = 1;
								utilityNodeData5.m_Vertical = flag4;
							}
							else if (num39 == utilityNodeData5.m_LanePriority)
							{
								utilityNodeData5.m_Count++;
								utilityNodeData5.m_Vertical |= flag4;
							}
							updateData.m_UtilityNodes[num40] = utilityNodeData5;
							val30.x = true;
							if (val30.y)
							{
								break;
							}
						}
						if (!val30.y && math.distancesq(utilityNodeData5.m_Transform.m_Position, curve3.m_Bezier.d) < 0.01f)
						{
							utilityNodeData5.m_Unsure = false;
							if (!flag4 && num39 > utilityNodeData5.m_NodePriority)
							{
								utilityNodeData5.m_Prefab = utilityLaneData3.m_NodeObjectPrefab;
								utilityNodeData5.m_NodePriority = num39;
							}
							if (num39 > utilityNodeData5.m_LanePriority)
							{
								utilityNodeData5.m_LanePriority = num39;
								utilityNodeData5.m_Count = 1;
								utilityNodeData5.m_Vertical = flag4;
							}
							else if (num39 == utilityNodeData5.m_LanePriority)
							{
								utilityNodeData5.m_Count++;
								utilityNodeData5.m_Vertical |= flag4;
							}
							updateData.m_UtilityNodes[num40] = utilityNodeData5;
							val30.y = true;
							if (val30.x)
							{
								break;
							}
						}
					}
					if (!val30.x)
					{
						float elevation = 0f;
						if (m_NetElevationData.TryGetComponent(subLane2, ref elevation2) && elevation2.m_Elevation.x != float.MinValue)
						{
							elevation = elevation2.m_Elevation.x;
						}
						UtilityNodeData utilityNodeData6 = new UtilityNodeData
						{
							m_Transform = new Transform(curve3.m_Bezier.a, NetUtils.GetNodeRotation(MathUtils.StartTangent(curve3.m_Bezier))),
							m_Prefab = utilityLaneData3.m_NodeObjectPrefab,
							m_Count = 1,
							m_Elevation = elevation,
							m_LanePriority = num39,
							m_NodePriority = num39,
							m_UtilityTypes = utilityLaneData3.m_UtilityTypes,
							m_Vertical = flag4
						};
						updateData.m_UtilityNodes.Add(ref utilityNodeData6);
					}
					if (!val30.y)
					{
						float elevation3 = 0f;
						if (m_NetElevationData.TryGetComponent(subLane2, ref elevation4) && elevation4.m_Elevation.y != float.MinValue)
						{
							elevation3 = elevation4.m_Elevation.y;
						}
						UtilityNodeData utilityNodeData7 = new UtilityNodeData
						{
							m_Transform = new Transform(curve3.m_Bezier.d, NetUtils.GetNodeRotation(MathUtils.EndTangent(curve3.m_Bezier))),
							m_Prefab = utilityLaneData3.m_NodeObjectPrefab,
							m_Count = 1,
							m_Elevation = elevation3,
							m_LanePriority = num39,
							m_NodePriority = num39,
							m_UtilityTypes = utilityLaneData3.m_UtilityTypes,
							m_Vertical = flag4
						};
						updateData.m_UtilityNodes.Add(ref utilityNodeData7);
					}
				}
				else
				{
					if (!flag2)
					{
						continue;
					}
					if (m_CarLaneData.HasComponent(subLane2))
					{
						if (m_MasterLaneData.HasComponent(subLane2))
						{
							continue;
						}
						Game.Net.CarLane carLane3 = m_CarLaneData[subLane2];
						Lane lane2 = m_LaneData[subLane2];
						Curve curve4 = m_CurveData[subLane2];
						if ((carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)
						{
							carLane3.m_Flags &= CarLaneFlags.Unsafe | CarLaneFlags.Highway | CarLaneFlags.RightLimit | CarLaneFlags.LeftLimit;
						}
						bool flag5 = (carLane3.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.RightLimit | CarLaneFlags.LeftLimit)) != 0;
						bool flag6 = m_LaneSignalData.HasComponent(subLane2);
						int num41 = -1;
						int num42 = -1;
						int num43 = -1;
						int num44 = -1;
						float num45 = float.MaxValue;
						float num46 = float.MaxValue;
						float num47 = float.MaxValue;
						float num48 = float.MaxValue;
						val4 = MathUtils.StartTangent(curve4.m_Bezier);
						float2 val31 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
						val4 = MathUtils.EndTangent(curve4.m_Bezier);
						float2 val32 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
						float3 a2 = curve4.m_Bezier.a;
						float3 a3 = curve4.m_Bezier.a;
						float3 d = curve4.m_Bezier.d;
						float3 d2 = curve4.m_Bezier.d;
						((float3)(ref a2)).xz = ((float3)(ref a2)).xz + MathUtils.Right(val31) * math.select(1.25f, -1.25f, m_LeftHandTraffic);
						((float3)(ref a3)).xz = ((float3)(ref a3)).xz + MathUtils.Left(val31) * math.select(1.25f, -1.25f, m_LeftHandTraffic);
						((float3)(ref d)).xz = ((float3)(ref d)).xz + MathUtils.Right(val32) * math.select(1.25f, -1.25f, m_LeftHandTraffic);
						((float3)(ref d2)).xz = ((float3)(ref d2)).xz + MathUtils.Left(val32) * math.select(1.25f, -1.25f, m_LeftHandTraffic);
						for (int num49 = 0; num49 < updateData.m_TrafficSigns.Length; num49++)
						{
							TrafficSignData trafficSignData4 = updateData.m_TrafficSigns[num49];
							float num50 = math.dot(val31, trafficSignData4.m_ForwardDirection);
							float num51 = math.dot(val32, trafficSignData4.m_ForwardDirection);
							if (num50 < -0.70710677f)
							{
								float num52 = math.distance(a2, trafficSignData4.m_ObjectTransform.m_Position);
								float num53 = math.distance(a3, trafficSignData4.m_ObjectTransform.m_Position);
								if (num52 < num45)
								{
									num41 = num49;
									num45 = num52;
								}
								if (num53 < num46)
								{
									num42 = num49;
									num46 = num53;
								}
							}
							if (num51 > 0.70710677f)
							{
								float num54 = math.distance(d, trafficSignData4.m_ObjectTransform.m_Position);
								float num55 = math.distance(d2, trafficSignData4.m_ObjectTransform.m_Position);
								if (num54 < num47)
								{
									num43 = num49;
									num47 = num54;
								}
								if (num55 < num48)
								{
									num44 = num49;
									num48 = num55;
								}
							}
						}
						float num56 = math.atan2(val31.x, val31.y);
						float num57 = math.atan2(val32.x, val32.y);
						int num58 = Mathf.FloorToInt(num56 * (16f / (float)Math.PI)) & 0x1F;
						int num59 = (Mathf.FloorToInt(num57 * (16f / (float)Math.PI)) + 16) & 0x1F;
						if ((flag5 || flag6) && num41 != -1)
						{
							TrafficSignData trafficSignData5 = updateData.m_TrafficSigns[num41];
							if (flag5 && updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_StartNode, ref targetLaneData2))
							{
								if ((carLane3.m_Flags & CarLaneFlags.Stop) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Stop);
								}
								if ((carLane3.m_Flags & targetLaneData2.m_AndCarLaneFlags & CarLaneFlags.Yield) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Yield);
								}
								if ((carLane3.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.GentleTurnLeft)) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_RemoveSignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnLeft);
								}
								else if ((carLane3.m_Flags & CarLaneFlags.LeftLimit) != 0)
								{
									uint num60 = (uint)(2032 << num58) | math.select(2032u >> 32 - num58, 0u, num58 == 0);
									if (((num3 | num4) & num60) != 0)
									{
										trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnLeft);
									}
								}
								if ((carLane3.m_Flags & CarLaneFlags.UTurnLeft) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_RemoveSignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnLeft);
								}
								else if ((carLane3.m_Flags & CarLaneFlags.LeftLimit) != 0)
								{
									uint num61 = (uint)(14 << num58) | math.select(14u >> 32 - num58, 0u, num58 == 0);
									if (((num3 | num4) & num61) != 0)
									{
										trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnLeft);
									}
								}
								if ((carLane3.m_Flags & (CarLaneFlags.TurnRight | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnRight)) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_RemoveSignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnRight);
								}
								else if ((carLane3.m_Flags & CarLaneFlags.RightLimit) != 0)
								{
									uint num62 = (uint)(532676608 << num58) | math.select(532676608u >> 32 - num58, 0u, num58 == 0);
									if (((num3 | num4) & num62) != 0)
									{
										trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoTurnRight);
									}
								}
								if ((carLane3.m_Flags & CarLaneFlags.UTurnRight) != 0)
								{
									trafficSignData5.m_TrafficSignNeeds.m_RemoveSignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnRight);
								}
								else if ((carLane3.m_Flags & CarLaneFlags.RightLimit) != 0)
								{
									uint num63 = (uint)(-536870912 << num58) | math.select(3758096384u >> 32 - num58, 0u, num58 == 0);
									if (((num3 | num4) & num63) != 0)
									{
										trafficSignData5.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.NoUTurnRight);
									}
								}
							}
							if (flag6)
							{
								LaneSignal laneSignal = m_LaneSignalData[subLane2];
								float num64 = math.dot(MathUtils.Right(val31), ((float3)(ref trafficSignData5.m_ObjectTransform.m_Position)).xz - ((float3)(ref curve4.m_Bezier.a)).xz);
								if (num64 > 0f)
								{
									trafficSignData5.m_TrafficSignNeeds.m_VehicleLanesRight = math.max(trafficSignData5.m_TrafficSignNeeds.m_VehicleLanesRight, num64);
								}
								else
								{
									trafficSignData5.m_TrafficSignNeeds.m_VehicleLanesLeft = math.max(trafficSignData5.m_TrafficSignNeeds.m_VehicleLanesLeft, 0f - num64);
								}
								trafficSignData5.m_TrafficSignNeeds.m_VehicleMask |= laneSignal.m_GroupMask;
							}
							updateData.m_TrafficSigns[num41] = trafficSignData5;
						}
						if (flag5 && num42 != -1)
						{
							TrafficSignData trafficSignData6 = updateData.m_TrafficSigns[num42];
							if (flag5 && updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_StartNode, ref targetLaneData3))
							{
								if (m_LeftHandTraffic)
								{
									if ((carLane3.m_Flags & CarLaneFlags.RightLimit) != 0)
									{
										uint num65 = (uint)(536870896 << num58) | math.select(536870896u >> 32 - num58, 0u, num58 == 0);
										if ((num3 & num65) != 0)
										{
											trafficSignData6.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.DoNotEnter);
										}
									}
								}
								else if ((carLane3.m_Flags & CarLaneFlags.LeftLimit) != 0)
								{
									uint num66 = (uint)(536870896 << num58) | math.select(536870896u >> 32 - num58, 0u, num58 == 0);
									if ((num3 & num66) != 0)
									{
										trafficSignData6.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.DoNotEnter);
									}
								}
							}
							updateData.m_TrafficSigns[num42] = trafficSignData6;
						}
						if (num43 != -1)
						{
							TrafficSignData trafficSignData7 = updateData.m_TrafficSigns[num43];
							if (updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_EndNode, ref targetLaneData4))
							{
								if (m_LeftHandTraffic)
								{
									if ((targetLaneData4.m_CarLaneFlags & (CarLaneFlags.Highway | CarLaneFlags.LeftLimit)) == CarLaneFlags.LeftLimit && (carLane3.m_Flags & CarLaneFlags.TurnLeft) != 0)
									{
										trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Street);
									}
								}
								else if ((targetLaneData4.m_CarLaneFlags & (CarLaneFlags.Highway | CarLaneFlags.RightLimit)) == CarLaneFlags.RightLimit && (carLane3.m_Flags & CarLaneFlags.TurnRight) != 0)
								{
									trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Street);
								}
								uint num67 = (uint)(536870896 << num58) | math.select(536870896u >> 32 - num58, 0u, num58 == 0);
								uint num68 = (uint)(536870896 << num59) | math.select(536870896u >> 32 - num59, 0u, num59 == 0);
								if ((num3 & num67) != 0 || (num4 & num68) != 0)
								{
									trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Oneway);
								}
								if ((targetLaneData4.m_CarLaneFlags & ~carLane3.m_Flags & CarLaneFlags.Highway) != 0)
								{
									trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Motorway);
								}
								if (math.abs(targetLaneData4.m_SpeedLimit.x - carLane3.m_DefaultSpeedLimit) > 1f)
								{
									trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.SpeedLimit);
									trafficSignData7.m_TrafficSignNeeds.m_SpeedLimit2 = (ushort)Mathf.RoundToInt(targetLaneData4.m_SpeedLimit.x * 3.6f);
								}
								trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.MoveableBridge);
								CarLaneFlags carLaneFlags2 = (m_LeftHandTraffic ? CarLaneFlags.ParkingLeft : CarLaneFlags.ParkingRight);
								if ((targetLaneData4.m_CarLaneFlags & carLaneFlags2) != 0)
								{
									if (updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_StartNode, ref targetLaneData5))
									{
										if ((targetLaneData5.m_CarLaneFlags & carLaneFlags2) == 0 && (carLane3.m_Flags & CarLaneFlags.Unsafe) == 0)
										{
											trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Parking);
										}
									}
									else
									{
										trafficSignData7.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Parking);
									}
								}
							}
							updateData.m_TrafficSigns[num43] = trafficSignData7;
						}
						if (num44 == -1)
						{
							continue;
						}
						TrafficSignData trafficSignData8 = updateData.m_TrafficSigns[num44];
						if (updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_EndNode, ref targetLaneData6))
						{
							CarLaneFlags carLaneFlags3 = (m_LeftHandTraffic ? CarLaneFlags.ParkingRight : CarLaneFlags.ParkingLeft);
							if ((targetLaneData6.m_CarLaneFlags & carLaneFlags3) != 0)
							{
								if (updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane2.m_StartNode, ref targetLaneData7))
								{
									if ((targetLaneData7.m_CarLaneFlags & carLaneFlags3) == 0 && (carLane3.m_Flags & CarLaneFlags.Unsafe) == 0)
									{
										trafficSignData8.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Parking);
									}
								}
								else
								{
									trafficSignData8.m_TrafficSignNeeds.m_SignTypeMask2 |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.Parking);
								}
							}
						}
						updateData.m_TrafficSigns[num44] = trafficSignData8;
					}
					else
					{
						if (!m_PedestrianLaneData.HasComponent(subLane2) || (m_PedestrianLaneData[subLane2].m_Flags & PedestrianLaneFlags.Crosswalk) == 0 || !m_LaneSignalData.HasComponent(subLane2))
						{
							continue;
						}
						LaneSignal laneSignal2 = m_LaneSignalData[subLane2];
						Curve curve5 = m_CurveData[subLane2];
						int num69 = -1;
						int num70 = -1;
						float num71 = float.MaxValue;
						float num72 = float.MaxValue;
						val4 = MathUtils.StartTangent(curve5.m_Bezier);
						float2 val33 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
						val4 = MathUtils.EndTangent(curve5.m_Bezier);
						float2 val34 = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
						for (int num73 = 0; num73 < updateData.m_TrafficSigns.Length; num73++)
						{
							TrafficSignData trafficSignData9 = updateData.m_TrafficSigns[num73];
							float num74 = 1f + math.distance(curve5.m_Bezier.a, trafficSignData9.m_ObjectTransform.m_Position);
							float num75 = 1f + math.distance(curve5.m_Bezier.d, trafficSignData9.m_ObjectTransform.m_Position);
							num74 *= 1f + math.abs(math.dot(val33, trafficSignData9.m_ForwardDirection));
							num75 *= 1f + math.abs(math.dot(val34, trafficSignData9.m_ForwardDirection));
							if (num74 < num71)
							{
								num69 = num73;
								num71 = num74;
							}
							if (num75 < num72)
							{
								num70 = num73;
								num72 = num75;
							}
						}
						if (num69 != -1)
						{
							TrafficSignData trafficSignData10 = updateData.m_TrafficSigns[num69];
							if (math.dot(MathUtils.Right(val33), trafficSignData10.m_ForwardDirection) > 0f)
							{
								trafficSignData10.m_TrafficSignNeeds.m_CrossingLeftMask |= laneSignal2.m_GroupMask;
							}
							else
							{
								trafficSignData10.m_TrafficSignNeeds.m_CrossingRightMask |= laneSignal2.m_GroupMask;
							}
							updateData.m_TrafficSigns[num69] = trafficSignData10;
						}
						if (num70 != -1)
						{
							TrafficSignData trafficSignData11 = updateData.m_TrafficSigns[num70];
							if (math.dot(MathUtils.Right(val34), trafficSignData11.m_ForwardDirection) > 0f)
							{
								trafficSignData11.m_TrafficSignNeeds.m_CrossingRightMask |= laneSignal2.m_GroupMask;
							}
							else
							{
								trafficSignData11.m_TrafficSignNeeds.m_CrossingLeftMask |= laneSignal2.m_GroupMask;
							}
							updateData.m_TrafficSigns[num70] = trafficSignData11;
						}
					}
				}
			}
			bool flag7 = updateData.m_UtilityNodes.IsCreated && updateData.m_UtilityNodes.Length != 0;
			if (flag2)
			{
				for (int num76 = 0; num76 < updateData.m_TrafficSigns.Length; num76++)
				{
					TrafficSignData trafficSignData12 = updateData.m_TrafficSigns[num76];
					trafficSignData12.m_TrafficSignNeeds.m_SignTypeMask &= ~trafficSignData12.m_TrafficSignNeeds.m_RemoveSignTypeMask;
					trafficSignData12.m_TrafficSignNeeds.m_SignTypeMask2 &= ~trafficSignData12.m_TrafficSignNeeds.m_RemoveSignTypeMask2;
					if (trafficSignData12.m_TrafficSignNeeds.m_SignTypeMask != 0 || trafficSignData12.m_TrafficSignNeeds.m_SignTypeMask2 != 0 || trafficSignData12.m_TrafficSignNeeds.m_VehicleMask != 0 || trafficSignData12.m_TrafficSignNeeds.m_CrossingLeftMask != 0 || trafficSignData12.m_TrafficSignNeeds.m_CrossingRightMask != 0)
					{
						CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, trafficSignData12.m_IsLowered, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, trafficSignData12.m_ParentTransform, trafficSignData12.m_ObjectTransform, trafficSignData12.m_LocalTransform, trafficSignData12.m_Flags, trafficSignData12.m_TrafficSignNeeds, ref updateData, trafficSignData12.m_Prefab, 0, trafficSignData12.m_Probability);
					}
				}
			}
			if (!flag7)
			{
				return;
			}
			for (int num77 = 0; num77 < updateData.m_UtilityNodes.Length; num77++)
			{
				UtilityNodeData utilityNodeData8 = updateData.m_UtilityNodes[num77];
				if (!utilityNodeData8.m_Unsure && (utilityNodeData8.m_Count != 2 || utilityNodeData8.m_Vertical))
				{
					Transform localTransformData = default(Transform);
					localTransformData.m_Position.y += utilityNodeData8.m_Elevation;
					CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered: false, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, utilityNodeData8.m_Transform, utilityNodeData8.m_Transform, localTransformData, SubObjectFlags.AnchorCenter, default(TrafficSignNeeds), ref updateData, utilityNodeData8.m_Prefab, 0, 100);
				}
			}
		}

		private void AddNodeLanes(Entity node, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubLanes.HasBuffer(node))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[node];
			TargetLaneData targetLaneData = default(TargetLaneData);
			TargetLaneData targetLaneData2 = default(TargetLaneData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane) || !m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane))
				{
					continue;
				}
				Game.Net.CarLane carLane = m_CarLaneData[subLane];
				if ((carLane.m_Flags & CarLaneFlags.Unsafe) == 0)
				{
					Lane lane = m_LaneData[subLane];
					if (m_SlaveLaneData.HasComponent(subLane))
					{
						SlaveLane slaveLane = m_SlaveLaneData[subLane];
						if (slaveLane.m_MasterIndex < val.Length)
						{
							lane = m_LaneData[val[(int)slaveLane.m_MasterIndex].m_SubLane];
						}
					}
					updateData.EnsureSourceLanes((Allocator)2);
					if (updateData.m_SourceLanes.TryGetValue(lane.m_EndNode, ref targetLaneData))
					{
						targetLaneData.m_CarLaneFlags |= carLane.m_Flags;
						targetLaneData.m_AndCarLaneFlags &= carLane.m_Flags;
						targetLaneData.m_SpeedLimit.x = math.min(targetLaneData.m_SpeedLimit.x, carLane.m_DefaultSpeedLimit);
						targetLaneData.m_SpeedLimit.y = math.max(targetLaneData.m_SpeedLimit.y, carLane.m_DefaultSpeedLimit);
						updateData.m_SourceLanes[lane.m_EndNode] = targetLaneData;
					}
					else
					{
						targetLaneData = new TargetLaneData
						{
							m_CarLaneFlags = carLane.m_Flags,
							m_AndCarLaneFlags = carLane.m_Flags,
							m_SpeedLimit = float2.op_Implicit(carLane.m_DefaultSpeedLimit)
						};
						updateData.m_SourceLanes.Add(lane.m_EndNode, targetLaneData);
					}
				}
				else
				{
					if ((carLane.m_Flags & CarLaneFlags.Forbidden) == 0)
					{
						continue;
					}
					Lane lane2 = m_LaneData[subLane];
					if (m_SlaveLaneData.HasComponent(subLane))
					{
						SlaveLane slaveLane2 = m_SlaveLaneData[subLane];
						if (slaveLane2.m_MasterIndex < val.Length)
						{
							lane2 = m_LaneData[val[(int)slaveLane2.m_MasterIndex].m_SubLane];
						}
					}
					updateData.EnsureTargetLanes((Allocator)2);
					if (updateData.m_TargetLanes.TryGetValue(lane2.m_StartNode, ref targetLaneData2))
					{
						targetLaneData2.m_CarLaneFlags |= carLane.m_Flags;
						targetLaneData2.m_AndCarLaneFlags &= carLane.m_Flags;
						targetLaneData2.m_SpeedLimit.x = math.min(targetLaneData2.m_SpeedLimit.x, carLane.m_DefaultSpeedLimit);
						targetLaneData2.m_SpeedLimit.y = math.max(targetLaneData2.m_SpeedLimit.y, carLane.m_DefaultSpeedLimit);
						updateData.m_TargetLanes[lane2.m_StartNode] = targetLaneData2;
					}
					else
					{
						targetLaneData2 = new TargetLaneData
						{
							m_CarLaneFlags = carLane.m_Flags,
							m_AndCarLaneFlags = carLane.m_Flags,
							m_SpeedLimit = float2.op_Implicit(carLane.m_DefaultSpeedLimit)
						};
						updateData.m_TargetLanes.Add(lane2.m_StartNode, targetLaneData2);
					}
				}
			}
		}

		private void AddEdgeLanes(Entity edge, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubLanes.HasBuffer(edge))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[edge];
			TargetLaneData targetLaneData = default(TargetLaneData);
			TargetLaneData targetLaneData2 = default(TargetLaneData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane) || !m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane) || (m_CarLaneData[subLane].m_Flags & CarLaneFlags.Unsafe) != 0)
				{
					continue;
				}
				Lane lane = m_LaneData[subLane];
				if (m_SlaveLaneData.HasComponent(subLane))
				{
					SlaveLane slaveLane = m_SlaveLaneData[subLane];
					if (slaveLane.m_MasterIndex < val.Length)
					{
						lane = m_LaneData[val[(int)slaveLane.m_MasterIndex].m_SubLane];
					}
				}
				if (updateData.m_SourceLanes.IsCreated && updateData.m_SourceLanes.TryGetValue(lane.m_StartNode, ref targetLaneData) && (targetLaneData.m_CarLaneFlags & CarLaneFlags.Approach) == 0)
				{
					targetLaneData.m_CarLaneFlags |= CarLaneFlags.Approach;
					updateData.m_SourceLanes.TryAdd(lane.m_EndNode, targetLaneData);
				}
				if (updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane.m_EndNode, ref targetLaneData2) && (targetLaneData2.m_CarLaneFlags & CarLaneFlags.Approach) == 0)
				{
					targetLaneData2.m_CarLaneFlags |= CarLaneFlags.Approach;
					updateData.m_TargetLanes.TryAdd(lane.m_StartNode, targetLaneData2);
				}
			}
		}

		private void CreateSecondaryEdgeObjects(int jobIndex, ref Random random, Entity owner, ref UpdateSecondaryObjectsData updateData, bool isTemp, bool isNative, Temp ownerTemp, out bool hasStreetLights, out bool alwaysLit)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0faa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1007: Unknown result type (might be due to invalid IL or missing references)
			//IL_100c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1160: Unknown result type (might be due to invalid IL or missing references)
			//IL_1191: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1021: Unknown result type (might be due to invalid IL or missing references)
			//IL_1032: Unknown result type (might be due to invalid IL or missing references)
			//IL_103c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1041: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			Composition composition = m_NetCompositionData[owner];
			EdgeGeometry edgeGeometry = m_EdgeGeometryData[owner];
			PrefabRef prefabRef = m_PrefabRefData[owner];
			NetGeometryData netGeometryData = m_PrefabNetGeometryData[prefabRef.m_Prefab];
			float ownerElevation = 0f;
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			if (m_NetElevationData.TryGetComponent(owner, ref elevation))
			{
				ownerElevation = math.cmin(elevation.m_Elevation);
			}
			DynamicBuffer<SubReplacement> val = default(DynamicBuffer<SubReplacement>);
			m_SubReplacements.TryGetBuffer(owner, ref val);
			NetCompositionData netCompositionData = m_PrefabNetCompositionData[composition.m_Edge];
			DynamicBuffer<NetCompositionObject> val2 = m_NetCompositionObjects[composition.m_Edge];
			bool flag = false;
			bool isLowered = ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0;
			hasStreetLights = false;
			alwaysLit = (netCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			Bounds1 val7 = default(Bounds1);
			Bounds1 val11 = default(Bounds1);
			TargetLaneData targetLaneData = default(TargetLaneData);
			TargetLaneData targetLaneData2 = default(TargetLaneData);
			for (int i = 0; i < val2.Length; i++)
			{
				NetCompositionObject netCompositionObject = val2[i];
				float num = edgeGeometry.m_Start.middleLength + edgeGeometry.m_End.middleLength;
				float num2 = netCompositionObject.m_Position.y;
				int num3 = 1;
				if ((netCompositionObject.m_Flags & SubObjectFlags.EvenSpacing) != 0)
				{
					NetCompositionData netCompositionData2 = m_PrefabNetCompositionData[composition.m_StartNode];
					NetCompositionData netCompositionData3 = m_PrefabNetCompositionData[composition.m_EndNode];
					if ((netCompositionData2.m_Flags.m_General & netCompositionObject.m_SpacingIgnore) == 0)
					{
						EdgeNodeGeometry geometry = m_StartNodeGeometryData[owner].m_Geometry;
						float num4 = (geometry.m_Left.middleLength + geometry.m_Right.middleLength) * 0.5f;
						num4 = math.min(num4, netCompositionObject.m_Spacing * (1f / 3f));
						num += num4;
						num2 -= num4;
					}
					if ((netCompositionData3.m_Flags.m_General & netCompositionObject.m_SpacingIgnore) == 0)
					{
						EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[owner].m_Geometry;
						float num5 = (geometry2.m_Left.middleLength + geometry2.m_Right.middleLength) * 0.5f;
						num5 = math.min(num5, netCompositionObject.m_Spacing * (1f / 3f));
						num += num5;
					}
				}
				if (num < netCompositionObject.m_MinLength)
				{
					continue;
				}
				if (netCompositionObject.m_Spacing > 0.1f)
				{
					num3 = Mathf.FloorToInt(num / netCompositionObject.m_Spacing + 0.5f);
					num3 = (((netCompositionObject.m_Flags & SubObjectFlags.EvenSpacing) == 0) ? math.select(num3, 1, (num3 == 0) & (num > netCompositionObject.m_Spacing * 0.1f)) : (num3 - 1));
					if (netCompositionObject.m_AvoidSpacing > 0.1f)
					{
						int num6 = Mathf.FloorToInt(num / netCompositionObject.m_AvoidSpacing + 0.5f);
						num6 = (((netCompositionObject.m_Flags & SubObjectFlags.EvenSpacing) == 0) ? math.select(num6, 1, (num6 == 0) & (num > netCompositionObject.m_AvoidSpacing * 0.1f)) : (num6 - 1));
						if ((num3 & 1) == (num6 & 1))
						{
							int2 val3 = num3 + new int2(-1, 1);
							float2 val4 = math.abs(float2.op_Implicit(val3) * netCompositionObject.m_Spacing - num);
							num3 = math.select(val3.x, val3.y, val4.y < val4.x || val3.x == 0);
						}
					}
				}
				if (num3 <= 0)
				{
					continue;
				}
				float num7 = netCompositionObject.m_Position.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
				Bezier4x3 val5 = MathUtils.Lerp(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, num7);
				Bezier4x3 val6 = MathUtils.Lerp(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, num7);
				float num8 = math.lerp(netCompositionObject.m_CurveOffsetRange.x, netCompositionObject.m_CurveOffsetRange.y, ((Random)(ref random)).NextFloat(1f));
				float num9;
				if ((netCompositionObject.m_Flags & SubObjectFlags.EvenSpacing) != 0)
				{
					num8 += 0.5f;
					num9 = num / (float)(num3 + 1);
				}
				else
				{
					num9 = num / (float)num3;
				}
				SubReplacementType subReplacementType = SubReplacementType.None;
				Game.Tools.AgeMask ageMask = (Game.Tools.AgeMask)0;
				bool flag2 = m_PrefabTrafficSignData.HasComponent(netCompositionObject.m_Prefab);
				bool flag3 = m_PrefabLaneDirectionData.HasComponent(netCompositionObject.m_Prefab);
				if (!hasStreetLights)
				{
					hasStreetLights = m_PrefabStreetLightData.HasComponent(netCompositionObject.m_Prefab);
				}
				if (m_PrefabPlaceableObjectData.TryGetComponent(netCompositionObject.m_Prefab, ref placeableObjectData))
				{
					subReplacementType = placeableObjectData.m_SubReplacementType;
				}
				if (subReplacementType != SubReplacementType.None && val.IsCreated)
				{
					SubReplacementSide subReplacementSide = (((netCompositionObject.m_Flags & SubObjectFlags.OnMedian) == 0) ? ((netCompositionObject.m_Position.x >= 0f) ? SubReplacementSide.Right : SubReplacementSide.Left) : SubReplacementSide.Middle);
					for (int j = 0; j < val.Length; j++)
					{
						SubReplacement subReplacement = val[j];
						if (subReplacement.m_Type == subReplacementType && subReplacement.m_Side == subReplacementSide)
						{
							netCompositionObject.m_Prefab = subReplacement.m_Prefab;
							ageMask = subReplacement.m_AgeMask;
						}
					}
				}
				for (int k = 0; k < num3; k++)
				{
					float num10 = ((float)k + num8) * num9 + num2;
					float3 position;
					float3 val9;
					float3 val10;
					if (num10 > edgeGeometry.m_Start.middleLength)
					{
						((Bounds1)(ref val7))._002Ector(0f, 1f);
						Bezier4x3 val8 = MathUtils.Lerp(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, 0.5f);
						MathUtils.ClampLength(((Bezier4x3)(ref val8)).xz, ref val7, num10 - edgeGeometry.m_Start.middleLength);
						position = MathUtils.Position(val6, val7.max);
						val9 = MathUtils.EndTangent(val6);
						val10 = MathUtils.Tangent(val6, val7.max);
					}
					else
					{
						((Bounds1)(ref val11))._002Ector(0f, 1f);
						Bezier4x3 val12 = MathUtils.Lerp(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, 0.5f);
						MathUtils.ClampLength(((Bezier4x3)(ref val12)).xz, ref val11, num10);
						position = MathUtils.Position(val5, val11.max);
						val9 = MathUtils.StartTangent(val5);
						val10 = MathUtils.Tangent(val5, val11.max);
					}
					val9.y = math.lerp(0f, val9.y, netCompositionObject.m_UseCurveRotation.x);
					val10.y = math.lerp(0f, val10.y, netCompositionObject.m_UseCurveRotation.x);
					val9 = math.normalizesafe(val9, new float3(0f, 0f, 1f));
					val10 = math.normalizesafe(val10, val9);
					quaternion val13 = quaternion.LookRotationSafe(val9, math.up());
					quaternion val14 = quaternion.LookRotationSafe(val10, math.up());
					quaternion rotation = math.slerp(val13, val14, netCompositionObject.m_UseCurveRotation.y);
					Transform transform = new Transform(position, rotation);
					Transform transform2 = new Transform(netCompositionObject.m_Offset, netCompositionObject.m_Rotation);
					Transform transformData = ObjectUtils.LocalToWorld(transform, transform2);
					if (netCompositionObject.m_Probability < 100)
					{
						netCompositionObject.m_Probability = math.clamp(Mathf.RoundToInt((float)netCompositionObject.m_Probability * (num / netGeometryData.m_EdgeLengthRange.max)), 1, netCompositionObject.m_Probability);
					}
					TrafficSignNeeds trafficSignNeeds = new TrafficSignNeeds
					{
						m_Left = LaneDirectionType.None,
						m_Forward = LaneDirectionType.None,
						m_Right = LaneDirectionType.None
					};
					if ((flag2 || flag3) && GetClosestCarLane(owner, transformData.m_Position, 1f, out var result, out var hasTurningLanes))
					{
						Game.Net.CarLane carLane = m_CarLaneData[result];
						Lane lane = m_LaneData[result];
						if (m_SlaveLaneData.HasComponent(result))
						{
							SlaveLane slaveLane = m_SlaveLaneData[result];
							DynamicBuffer<Game.Net.SubLane> val15 = m_SubLanes[owner];
							if (slaveLane.m_MasterIndex < val15.Length)
							{
								lane = m_LaneData[val15[(int)slaveLane.m_MasterIndex].m_SubLane];
							}
							if (flag3)
							{
								if ((slaveLane.m_Flags & SlaveLaneFlags.MergeLeft) != 0)
								{
									trafficSignNeeds.m_Left = LaneDirectionType.Merge;
								}
								if ((slaveLane.m_Flags & SlaveLaneFlags.MergeRight) != 0)
								{
									trafficSignNeeds.m_Right = LaneDirectionType.Merge;
								}
							}
						}
						if (!flag)
						{
							flag = true;
							Game.Net.Edge edge = m_NetEdgeData[owner];
							AddNodeLanes(edge.m_Start, ref updateData);
							AddNodeLanes(edge.m_End, ref updateData);
							AddEdgeLanes(owner, ref updateData);
						}
						if (flag3)
						{
							bool flag4 = false;
							if (!hasTurningLanes && updateData.m_TargetLanes.IsCreated && updateData.m_TargetLanes.TryGetValue(lane.m_EndNode, ref targetLaneData))
							{
								flag4 = (targetLaneData.m_CarLaneFlags & CarLaneFlags.Forbidden) != 0 && (targetLaneData.m_CarLaneFlags & (CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) != 0;
							}
							if (hasTurningLanes || flag4)
							{
								if ((carLane.m_Flags & CarLaneFlags.UTurnLeft) != 0)
								{
									trafficSignNeeds.m_Left = LaneDirectionType.UTurn;
								}
								if ((carLane.m_Flags & CarLaneFlags.UTurnRight) != 0)
								{
									trafficSignNeeds.m_Right = LaneDirectionType.UTurn;
								}
								if ((carLane.m_Flags & CarLaneFlags.GentleTurnLeft) != 0)
								{
									trafficSignNeeds.m_Left = LaneDirectionType.Gentle;
								}
								if ((carLane.m_Flags & CarLaneFlags.GentleTurnRight) != 0)
								{
									trafficSignNeeds.m_Right = LaneDirectionType.Gentle;
								}
								if ((carLane.m_Flags & CarLaneFlags.TurnLeft) != 0)
								{
									trafficSignNeeds.m_Left = LaneDirectionType.Square;
								}
								if ((carLane.m_Flags & CarLaneFlags.TurnRight) != 0)
								{
									trafficSignNeeds.m_Right = LaneDirectionType.Square;
								}
								if ((carLane.m_Flags & CarLaneFlags.Forward) != 0)
								{
									trafficSignNeeds.m_Forward = LaneDirectionType.Straight;
								}
							}
						}
						if (flag2)
						{
							if (updateData.m_SourceLanes.IsCreated && updateData.m_SourceLanes.TryGetValue(lane.m_StartNode, ref targetLaneData2))
							{
								if ((carLane.m_Flags & ~targetLaneData2.m_AndCarLaneFlags & CarLaneFlags.PublicOnly) != 0 && (targetLaneData2.m_CarLaneFlags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.PublicOnly | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) != CarLaneFlags.PublicOnly)
								{
									trafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.BusOnly);
									trafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.TaxiOnly);
								}
								else if (math.any(math.abs(targetLaneData2.m_SpeedLimit - carLane.m_DefaultSpeedLimit) > 1f))
								{
									trafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.SpeedLimit);
									trafficSignNeeds.m_SpeedLimit = (ushort)Mathf.RoundToInt(carLane.m_DefaultSpeedLimit * 3.6f);
								}
							}
							if ((carLane.m_Flags & CarLaneFlags.Roundabout) != 0)
							{
								if (m_LeftHandTraffic)
								{
									trafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.RoundaboutClockwise);
								}
								else
								{
									trafficSignNeeds.m_SignTypeMask |= Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.RoundaboutCounterclockwise);
								}
							}
						}
					}
					CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered, isNative, ageMask, ownerTemp, ownerElevation, transform, transformData, transform2, netCompositionObject.m_Flags, trafficSignNeeds, ref updateData, netCompositionObject.m_Prefab, 0, netCompositionObject.m_Probability);
				}
			}
			DynamicBuffer<Game.Net.SubLane> val16 = m_SubLanes[owner];
			Temp temp = default(Temp);
			Game.Net.Elevation elevation3 = default(Game.Net.Elevation);
			Transform transform3 = default(Transform);
			Transform transform4 = default(Transform);
			for (int l = 0; l < val16.Length; l++)
			{
				Entity subLane = val16[l].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				if (m_UtilityLaneData.HasComponent(subLane) && !m_EdgeLaneData.HasComponent(subLane))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[subLane];
					UtilityLaneData utilityLaneData = m_PrefabUtilityLaneData[prefabRef2.m_Prefab];
					if (!(utilityLaneData.m_NodeObjectPrefab != Entity.Null))
					{
						continue;
					}
					Curve curve = m_CurveData[subLane];
					Lane lane2 = m_LaneData[subLane];
					bool flag5 = true;
					if (isTemp && m_TempData.TryGetComponent(subLane, ref temp))
					{
						flag5 = temp.m_Original == Entity.Null;
					}
					float num11 = math.length(MathUtils.Size(m_PrefabObjectGeometryData[utilityLaneData.m_NodeObjectPrefab].m_Bounds));
					updateData.EnsureUtilityNodes((Allocator)2);
					bool flag6 = false;
					for (int m = 0; m < updateData.m_UtilityNodes.Length; m++)
					{
						UtilityNodeData utilityNodeData = updateData.m_UtilityNodes[m];
						if ((utilityNodeData.m_UtilityTypes & utilityLaneData.m_UtilityTypes) != UtilityTypes.None && utilityNodeData.m_PathNode.Equals(lane2.m_StartNode))
						{
							if (num11 > utilityNodeData.m_LanePriority)
							{
								utilityNodeData.m_Prefab = utilityLaneData.m_NodeObjectPrefab;
								utilityNodeData.m_LanePriority = num11;
								utilityNodeData.m_Count = 1;
							}
							else if (num11 == utilityNodeData.m_LanePriority)
							{
								utilityNodeData.m_Count++;
							}
							utilityNodeData.m_IsNew &= flag5;
							updateData.m_UtilityNodes[m] = utilityNodeData;
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						float elevation2 = 0f;
						if (m_NetElevationData.TryGetComponent(subLane, ref elevation3) && elevation3.m_Elevation.x != float.MinValue)
						{
							elevation2 = elevation3.m_Elevation.x;
						}
						UtilityNodeData utilityNodeData2 = new UtilityNodeData
						{
							m_Transform = new Transform(curve.m_Bezier.a, NetUtils.GetNodeRotation(MathUtils.StartTangent(curve.m_Bezier))),
							m_Prefab = utilityLaneData.m_NodeObjectPrefab,
							m_PathNode = lane2.m_StartNode,
							m_Count = 1,
							m_Elevation = elevation2,
							m_LanePriority = num11,
							m_UtilityTypes = utilityLaneData.m_UtilityTypes,
							m_Vertical = true,
							m_IsNew = flag5
						};
						updateData.m_UtilityNodes.Add(ref utilityNodeData2);
					}
				}
				else
				{
					if (!m_TrackLaneData.HasComponent(subLane))
					{
						continue;
					}
					Game.Net.TrackLane trackLane = m_TrackLaneData[subLane];
					if ((trackLane.m_Flags & (TrackLaneFlags.StartingLane | TrackLaneFlags.EndingLane)) == 0)
					{
						continue;
					}
					PrefabRef prefabRef3 = m_PrefabRefData[subLane];
					TrackLaneData trackLaneData = m_PrefabTrackLaneData[prefabRef3.m_Prefab];
					if (!(trackLaneData.m_EndObjectPrefab != Entity.Null))
					{
						continue;
					}
					Curve curve2 = m_CurveData[subLane];
					if ((trackLane.m_Flags & TrackLaneFlags.StartingLane) != 0)
					{
						transform3.m_Position = curve2.m_Bezier.a;
						float3 val17 = MathUtils.StartTangent(curve2.m_Bezier);
						if (MathUtils.TryNormalize(ref val17))
						{
							transform3.m_Rotation = quaternion.LookRotation(val17, math.up());
						}
						else
						{
							transform3.m_Rotation = quaternion.identity;
						}
						transform3.m_Position.y += netCompositionData.m_SurfaceHeight.max;
						CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, transform3, transform3, default(Transform), (SubObjectFlags)0, default(TrafficSignNeeds), ref updateData, trackLaneData.m_EndObjectPrefab, 0, 100);
					}
					if ((trackLane.m_Flags & TrackLaneFlags.EndingLane) != 0)
					{
						transform4.m_Position = curve2.m_Bezier.d;
						float3 val18 = -MathUtils.EndTangent(curve2.m_Bezier);
						if (MathUtils.TryNormalize(ref val18))
						{
							transform4.m_Rotation = quaternion.LookRotation(val18, math.up());
						}
						else
						{
							transform4.m_Rotation = quaternion.identity;
						}
						transform4.m_Position.y += netCompositionData.m_SurfaceHeight.max;
						CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, transform4, transform4, default(Transform), (SubObjectFlags)0, default(TrafficSignNeeds), ref updateData, trackLaneData.m_EndObjectPrefab, 0, 100);
					}
				}
			}
			if (!updateData.m_UtilityNodes.IsCreated || updateData.m_UtilityNodes.Length == 0)
			{
				return;
			}
			for (int n = 0; n < val16.Length; n++)
			{
				Entity subLane2 = val16[n].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane2) || !m_UtilityLaneData.HasComponent(subLane2) || !m_EdgeLaneData.HasComponent(subLane2))
				{
					continue;
				}
				PrefabRef prefabRef4 = m_PrefabRefData[subLane2];
				UtilityLaneData utilityLaneData2 = m_PrefabUtilityLaneData[prefabRef4.m_Prefab];
				if (!(utilityLaneData2.m_NodeObjectPrefab != Entity.Null))
				{
					continue;
				}
				Lane lane3 = m_LaneData[subLane2];
				float num12 = math.length(MathUtils.Size(m_PrefabObjectGeometryData[utilityLaneData2.m_NodeObjectPrefab].m_Bounds));
				for (int num13 = 0; num13 < updateData.m_UtilityNodes.Length; num13++)
				{
					UtilityNodeData utilityNodeData3 = updateData.m_UtilityNodes[num13];
					if ((utilityNodeData3.m_UtilityTypes & utilityLaneData2.m_UtilityTypes) != UtilityTypes.None && utilityNodeData3.m_PathNode.EqualsIgnoreCurvePos(lane3.m_MiddleNode))
					{
						if (num12 > utilityNodeData3.m_LanePriority)
						{
							utilityNodeData3.m_LanePriority = num12;
							utilityNodeData3.m_Count = 2;
							utilityNodeData3.m_Vertical = false;
						}
						else if (num12 == utilityNodeData3.m_LanePriority)
						{
							utilityNodeData3.m_Count += 2;
						}
						utilityNodeData3.m_Prefab = utilityLaneData2.m_NodeObjectPrefab;
						updateData.m_UtilityNodes[num13] = utilityNodeData3;
					}
				}
			}
			for (int num14 = 0; num14 < updateData.m_UtilityNodes.Length; num14++)
			{
				UtilityNodeData utilityNodeData4 = updateData.m_UtilityNodes[num14];
				if (utilityNodeData4.m_Count != 2 || utilityNodeData4.m_Vertical)
				{
					Transform localTransformData = default(Transform);
					localTransformData.m_Position.y += utilityNodeData4.m_Elevation;
					CreateSecondaryObject(jobIndex, ref random, owner, isTemp, utilityNodeData4.m_IsNew, isLowered: false, isNative, (Game.Tools.AgeMask)0, ownerTemp, ownerElevation, utilityNodeData4.m_Transform, utilityNodeData4.m_Transform, localTransformData, SubObjectFlags.AnchorCenter, default(TrafficSignNeeds), ref updateData, utilityNodeData4.m_Prefab, 0, 100);
				}
			}
		}

		private void CreateSecondaryAreaObjects(int jobIndex, ref Random random, Entity owner, ref UpdateSecondaryObjectsData updateData, bool isTemp, bool isNative, Temp ownerTemp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[owner];
			DynamicBuffer<Game.Areas.Node> val = m_AreaNodes[owner];
			DynamicBuffer<Game.Prefabs.SubObject> val2 = default(DynamicBuffer<Game.Prefabs.SubObject>);
			if (!m_PrefabSubObjects.TryGetBuffer(prefabRef.m_Prefab, ref val2))
			{
				return;
			}
			for (int i = 0; i < val2.Length; i++)
			{
				Game.Prefabs.SubObject subObject = val2[i];
				if ((subObject.m_Flags & SubObjectFlags.EdgePlacement) == 0)
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					Game.Areas.Node node = val[j];
					Game.Areas.Node node2 = val[math.select(j + 1, 0, j + 1 >= val.Length)];
					float2 val3 = math.normalizesafe(((float3)(ref node2.m_Position)).xz - ((float3)(ref node.m_Position)).xz, default(float2));
					quaternion rotation = quaternion.LookRotationSafe(new float3(val3.x, 0f, val3.y), math.up());
					Transform transform = new Transform(node.m_Position, rotation);
					Transform transform2 = new Transform(subObject.m_Position, subObject.m_Rotation);
					Transform transformData = ObjectUtils.LocalToWorld(transform, transform2);
					if (node.m_Elevation == float.MinValue)
					{
						subObject.m_Flags |= SubObjectFlags.OnGround;
						node.m_Elevation = 0f;
					}
					else
					{
						subObject.m_Flags &= ~SubObjectFlags.OnGround;
					}
					CreateSecondaryObject(jobIndex, ref random, owner, isTemp, isNew: false, isLowered: false, isNative, (Game.Tools.AgeMask)0, ownerTemp, node.m_Elevation, transform, transformData, transform2, subObject.m_Flags, default(TrafficSignNeeds), ref updateData, subObject.m_Prefab, 0, subObject.m_Probability);
				}
			}
		}

		private bool CheckRequirements(Entity owner, Entity prefab, bool isExplicit, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_ObjectRequirements.TryGetBuffer(prefab, ref val))
			{
				EnsurePlaceholderRequirements(owner, ref updateData);
				int num = -1;
				bool flag = true;
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
						flag |= updateData.m_PlaceholderRequirements.Contains(objectRequirementElement.m_Requirement) || (isExplicit && (objectRequirementElement.m_Type & ObjectRequirementType.IgnoreExplicit) != 0);
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private void CreateSecondaryObject(int jobIndex, ref Random random, Entity owner, bool isTemp, bool isNew, bool isLowered, bool isNative, Game.Tools.AgeMask ageMask, Temp ownerTemp, float ownerElevation, Transform ownerTransform, Transform transformData, Transform localTransformData, SubObjectFlags flags, TrafficSignNeeds trafficSignNeeds, ref UpdateSecondaryObjectsData updateData, Entity prefab, int groupIndex, int probability)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
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
				if (CheckRequirements(owner, prefab, isExplicit: true, ref updateData))
				{
					Random random2 = random;
					((Random)(ref random)).NextInt();
					((Random)(ref random)).NextInt();
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
						CreateSecondaryObject(jobIndex, ref random2, owner, isTemp, isNew, isLowered, isNative, ageMask, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, trafficSignNeeds, ref updateData, prefab, cacheTransform: false, 0, groupIndex, probability);
					}
				}
				return;
			}
			if (placeholderObjectData.m_RandomizeGroupIndex)
			{
				groupIndex = ((Random)(ref random)).NextInt();
			}
			float num = -1f;
			Entity prefab2 = Entity.Null;
			Entity groupPrefab2 = Entity.Null;
			Random random3 = default(Random);
			bool flag = false;
			int num2 = 0;
			Random val8 = default(Random);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = val[i].m_Object;
				if (!CheckRequirements(owner, val3, isExplicit: false, ref updateData))
				{
					continue;
				}
				float num3 = 0f;
				bool flag2 = false;
				if (m_PrefabTrafficSignData.HasComponent(val3))
				{
					Game.Prefabs.TrafficSignData trafficSignData = m_PrefabTrafficSignData[val3];
					uint num4 = trafficSignData.m_TypeMask & trafficSignNeeds.m_SignTypeMask;
					int num5 = trafficSignNeeds.m_SpeedLimit;
					if (num4 == 0)
					{
						num4 = trafficSignData.m_TypeMask & trafficSignNeeds.m_SignTypeMask2;
						num5 = trafficSignNeeds.m_SpeedLimit2;
						if (num4 == 0)
						{
							continue;
						}
						flag2 = true;
					}
					float num6 = 10f + math.log2((float)num4);
					if ((num4 & Game.Prefabs.TrafficSignData.GetTypeMask(TrafficSignType.SpeedLimit)) != 0)
					{
						num6 /= 1f + (float)math.abs(trafficSignData.m_SpeedLimit - num5);
					}
					num3 += num6;
				}
				if (m_PrefabTrafficLightData.HasComponent(val3))
				{
					TrafficLightData trafficLightData = m_PrefabTrafficLightData[val3];
					int num7 = 0;
					if (trafficSignNeeds.m_VehicleLanesLeft > 0f)
					{
						if ((trafficLightData.m_Type & TrafficLightType.VehicleLeft) == 0)
						{
							continue;
						}
						num7 += 10;
					}
					else if ((trafficLightData.m_Type & TrafficLightType.VehicleLeft) != 0)
					{
						if (trafficSignNeeds.m_VehicleMask == 0)
						{
							continue;
						}
						num7--;
					}
					if (trafficSignNeeds.m_VehicleLanesRight > 0f)
					{
						if ((trafficLightData.m_Type & TrafficLightType.VehicleRight) == 0)
						{
							continue;
						}
						num7 += 10;
					}
					else if ((trafficLightData.m_Type & TrafficLightType.VehicleRight) != 0)
					{
						if (trafficSignNeeds.m_VehicleMask == 0)
						{
							continue;
						}
						num7--;
					}
					if (trafficSignNeeds.m_CrossingLeftMask != 0 && trafficSignNeeds.m_CrossingRightMask != 0)
					{
						num7 = (((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight)) != (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight)) ? (num7 - 1) : (num7 + 10));
					}
					else if (trafficSignNeeds.m_CrossingLeftMask != 0)
					{
						if ((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight)) == TrafficLightType.CrossingLeft)
						{
							num7 += 10;
						}
						else if ((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight | TrafficLightType.AllowFlipped)) == (TrafficLightType.CrossingRight | TrafficLightType.AllowFlipped))
						{
							flag2 = true;
							num7 += 9;
						}
						else
						{
							if ((trafficLightData.m_Type & TrafficLightType.CrossingRight) != 0)
							{
								continue;
							}
							num7--;
						}
					}
					else if (trafficSignNeeds.m_CrossingRightMask != 0)
					{
						if ((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight)) == TrafficLightType.CrossingRight)
						{
							num7 += 10;
						}
						else if ((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight | TrafficLightType.AllowFlipped)) == (TrafficLightType.CrossingLeft | TrafficLightType.AllowFlipped))
						{
							flag2 = true;
							num7 += 9;
						}
						else
						{
							if ((trafficLightData.m_Type & TrafficLightType.CrossingLeft) != 0)
							{
								continue;
							}
							num7--;
						}
					}
					else if ((trafficLightData.m_Type & (TrafficLightType.CrossingLeft | TrafficLightType.CrossingRight)) != 0)
					{
						continue;
					}
					if (num7 <= 0)
					{
						continue;
					}
					num3 += (float)(50 * num7);
					if ((trafficLightData.m_Type & TrafficLightType.VehicleLeft) != 0)
					{
						ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[val3];
						Bounds1 val4 = trafficLightData.m_ReachOffset - objectGeometryData.m_Bounds.min.x;
						if (val4.min > math.max(trafficSignNeeds.m_VehicleLanesLeft, 1f))
						{
							continue;
						}
						val4 = trafficSignNeeds.m_VehicleLanesLeft - val4;
						num3 -= 50f * math.max(0f, math.max(0f - val4.min, val4.max));
					}
					if ((trafficLightData.m_Type & TrafficLightType.VehicleRight) != 0)
					{
						ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[val3];
						Bounds1 val5 = trafficLightData.m_ReachOffset + objectGeometryData2.m_Bounds.max.x;
						if (val5.min > math.max(trafficSignNeeds.m_VehicleLanesRight, 1f))
						{
							continue;
						}
						val5 = trafficSignNeeds.m_VehicleLanesRight - val5;
						num3 -= 50f * math.max(0f, math.max(0f - val5.min, val5.max));
					}
				}
				if (m_PrefabLaneDirectionData.HasComponent(val3))
				{
					LaneDirectionData laneDirectionData = m_PrefabLaneDirectionData[val3];
					if (trafficSignNeeds.m_Left == LaneDirectionType.None && trafficSignNeeds.m_Forward == LaneDirectionType.None && trafficSignNeeds.m_Right == LaneDirectionType.None)
					{
						continue;
					}
					int num8 = 0;
					num8 += 180 - math.abs((int)(trafficSignNeeds.m_Left - laneDirectionData.m_Left));
					num8 += 180 - math.abs((int)(trafficSignNeeds.m_Forward - laneDirectionData.m_Forward));
					num8 += 180 - math.abs((int)(trafficSignNeeds.m_Right - laneDirectionData.m_Right));
					num3 += (float)num8 * 0.1f;
				}
				SpawnableObjectData spawnableObjectData2 = m_PrefabSpawnableObjectData[val3];
				Entity val6 = ((spawnableObjectData2.m_RandomizationGroup != Entity.Null) ? spawnableObjectData2.m_RandomizationGroup : val3);
				Random val7 = random;
				((Random)(ref random)).NextInt();
				((Random)(ref random)).NextInt();
				if (updateData.m_SelectedSpawnabled.IsCreated && updateData.m_SelectedSpawnabled.TryGetValue(new PlaceholderKey(val6, groupIndex), ref val8))
				{
					num3 += 0.5f;
					val7 = val8;
				}
				if (num3 > num)
				{
					num = num3;
					prefab2 = val3;
					groupPrefab2 = val6;
					random3 = val7;
					flag = flag2;
					num2 = m_PrefabSpawnableObjectData[val3].m_Probability;
				}
				else if (num3 == num)
				{
					int probability2 = m_PrefabSpawnableObjectData[val3].m_Probability;
					num2 += probability2;
					if (((Random)(ref random)).NextInt(num2) < probability2)
					{
						prefab2 = val3;
						groupPrefab2 = val6;
						random3 = val7;
						flag = flag2;
					}
				}
			}
			if (num2 <= 0)
			{
				return;
			}
			updateData.EnsureSelectedSpawnables((Allocator)2);
			updateData.m_SelectedSpawnabled.TryAdd(new PlaceholderKey(groupPrefab2, groupIndex), random3);
			if (((Random)(ref random3)).NextInt(100) < probability)
			{
				if (flag)
				{
					transformData.m_Rotation = math.mul(quaternion.RotateY((float)Math.PI), transformData.m_Rotation);
				}
				CreateSecondaryObject(jobIndex, ref random3, owner, isTemp, isNew, isLowered, isNative, ageMask, ownerTemp, ownerElevation, Entity.Null, ownerTransform, transformData, localTransformData, flags, trafficSignNeeds, ref updateData, prefab2, cacheTransform: false, 0, groupIndex, probability);
			}
		}

		private bool GetClosestCarLane(Entity owner, float3 position, float maxDistance, out Entity result, out bool hasTurningLanes)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			result = Entity.Null;
			hasTurningLanes = false;
			if (!m_SubLanes.HasBuffer(owner))
			{
				return false;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
			bool2 val2 = bool2.op_Implicit(false);
			bool flag = false;
			float num2 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane) || m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				Game.Net.CarLane carLane = m_CarLaneData[subLane];
				if ((carLane.m_Flags & CarLaneFlags.Unsafe) == 0)
				{
					if ((carLane.m_Flags & CarLaneFlags.Invert) != 0)
					{
						val2.y |= (carLane.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) != 0;
					}
					else
					{
						val2.x |= (carLane.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) != 0;
					}
					float num = MathUtils.Distance(m_CurveData[subLane].m_Bezier, position, ref num2);
					if (num < maxDistance)
					{
						maxDistance = num;
						result = subLane;
						flag = (carLane.m_Flags & CarLaneFlags.Invert) != 0;
					}
				}
			}
			hasTurningLanes = (flag ? val2.y : val2.x);
			return result != Entity.Null;
		}

		private void CreateSecondaryObject(int jobIndex, ref Random random, Entity owner, bool isTemp, bool isNew, bool isLowered, bool isNative, Game.Tools.AgeMask ageMask, Temp ownerTemp, float ownerElevation, Entity oldSecondaryObject, Transform ownerTransform, Transform transformData, Transform localTransformData, SubObjectFlags flags, TrafficSignNeeds trafficSignNeeds, ref UpdateSecondaryObjectsData updateData, Entity prefab, bool cacheTransform, int parentMesh, int groupIndex, int probability)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_095e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			bool flag = m_PrefabObjectGeometryData.HasComponent(prefab);
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			bool num = m_PrefabPlaceableObjectData.TryGetComponent(prefab, ref placeableObjectData);
			bool flag2 = m_PrefabData.IsComponentEnabled(prefab);
			if ((flags & SubObjectFlags.AnchorTop) != 0)
			{
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefab];
				objectGeometryData.m_Bounds.max.y -= placeableObjectData.m_PlacementOffset.y;
				transformData.m_Position.y -= objectGeometryData.m_Bounds.max.y;
				localTransformData.m_Position.y -= objectGeometryData.m_Bounds.max.y;
			}
			else if ((flags & SubObjectFlags.AnchorCenter) != 0)
			{
				ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefab];
				float num2 = (objectGeometryData2.m_Bounds.max.y - objectGeometryData2.m_Bounds.min.y) * 0.5f;
				transformData.m_Position.y -= num2;
				localTransformData.m_Position.y -= num2;
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
				else if (elevation.m_Elevation < 0f && isLowered)
				{
					elevation.m_Flags |= ElevationFlags.Lowered;
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
			if (oldSecondaryObject == Entity.Null)
			{
				oldSecondaryObject = FindOldSecondaryObject(prefab, transformData, ref updateData);
			}
			uint num3 = ((Random)(ref random)).NextUInt(268435456u);
			PseudoRandomSeed pseudoRandomSeed = new PseudoRandomSeed((ushort)(num3 >> 12));
			if (num && placeableObjectData.m_RotationSymmetry != RotationSymmetry.None)
			{
				uint num4 = num3 & 0xFFF;
				if (placeableObjectData.m_RotationSymmetry != RotationSymmetry.Any)
				{
					num4 = (uint)(((int)num4 * (int)placeableObjectData.m_RotationSymmetry) & -4096) / (uint)placeableObjectData.m_RotationSymmetry;
				}
				float num5 = (float)num4 * 0.0015339808f;
				transformData.m_Rotation = math.mul(quaternion.RotateY(num5), transformData.m_Rotation);
			}
			m_UpdateQueue.Enqueue(new UpdateData
			{
				m_Owner = owner,
				m_Prefab = prefab,
				m_Transform = transformData
			});
			if (oldSecondaryObject != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldSecondaryObject);
				Temp temp = default(Temp);
				if (isTemp)
				{
					if (m_TempData.HasComponent(oldSecondaryObject))
					{
						temp = m_TempData[oldSecondaryObject];
						temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
						if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
						{
							temp.m_Flags |= TempFlags.Modify;
						}
						if (isNew)
						{
							temp.m_Original = Entity.Null;
						}
						else
						{
							temp.m_Original = FindOriginalSecondaryObject(prefab, temp.m_Original, transformData, ref updateData);
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, oldSecondaryObject, temp);
						Tree tree = default(Tree);
						if (temp.m_Original != Entity.Null && flag2 && m_PrefabTreeData.HasComponent(prefab) && m_TreeData.TryGetComponent(temp.m_Original, ref tree))
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, oldSecondaryObject, tree);
						}
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSecondaryObject, transformData);
					if (!m_UpdatedData.HasComponent(oldSecondaryObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldSecondaryObject, default(Updated));
					}
				}
				else if (!transformData.Equals(m_TransformData[oldSecondaryObject]))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldSecondaryObject, transformData);
					if (!m_UpdatedData.HasComponent(oldSecondaryObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldSecondaryObject, default(Updated));
					}
				}
				if (m_PrefabStreetLightData.HasComponent(prefab))
				{
					StreetLight streetLight = default(StreetLight);
					bool flag3 = false;
					StreetLight streetLight2 = default(StreetLight);
					if (m_StreetLightData.TryGetComponent(oldSecondaryObject, ref streetLight2))
					{
						streetLight = streetLight2;
						flag3 = true;
					}
					Road road = default(Road);
					if (m_RoadData.TryGetComponent(owner, ref road))
					{
						StreetLightSystem.UpdateStreetLightState(ref streetLight, road);
					}
					if (flag3)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<StreetLight>(jobIndex, oldSecondaryObject, streetLight);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StreetLight>(jobIndex, oldSecondaryObject, streetLight);
					}
				}
				if (m_PrefabTrafficLightData.HasComponent(prefab))
				{
					TrafficLight trafficLight = default(TrafficLight);
					bool flag4 = false;
					TrafficLight trafficLight2 = default(TrafficLight);
					if (m_TrafficLightData.TryGetComponent(oldSecondaryObject, ref trafficLight2))
					{
						trafficLight = trafficLight2;
						flag4 = true;
					}
					trafficLight.m_GroupMask0 = trafficSignNeeds.m_VehicleMask;
					trafficLight.m_GroupMask1 = (ushort)(trafficSignNeeds.m_CrossingLeftMask | trafficSignNeeds.m_CrossingRightMask);
					TrafficLights trafficLights = default(TrafficLights);
					if (m_TrafficLightsData.TryGetComponent(owner, ref trafficLights))
					{
						TrafficLightSystem.UpdateTrafficLightState(trafficLights, ref trafficLight);
					}
					if (flag4)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrafficLight>(jobIndex, oldSecondaryObject, trafficLight);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TrafficLight>(jobIndex, oldSecondaryObject, trafficLight);
					}
				}
				if (temp.m_Original == Entity.Null && m_PrefabTreeData.HasComponent(prefab))
				{
					Tree tree2 = ObjectUtils.InitializeTreeState(ToolUtils.GetRandomAge(ref random, ageMask));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, oldSecondaryObject, tree2);
				}
				if (cacheTransform)
				{
					LocalTransformCache localTransformCache = default(LocalTransformCache);
					localTransformCache.m_Position = localTransformData.m_Position;
					localTransformCache.m_Rotation = localTransformData.m_Rotation;
					localTransformCache.m_ParentMesh = parentMesh;
					localTransformCache.m_GroupIndex = groupIndex;
					localTransformCache.m_Probability = probability;
					localTransformCache.m_PrefabSubIndex = -1;
					if (m_LocalTransformCacheData.HasComponent(oldSecondaryObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<LocalTransformCache>(jobIndex, oldSecondaryObject, localTransformCache);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, oldSecondaryObject, localTransformCache);
					}
				}
				else if (m_LocalTransformCacheData.HasComponent(oldSecondaryObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LocalTransformCache>(jobIndex, oldSecondaryObject);
				}
				if (flag)
				{
					if (m_PseudoRandomSeedData.HasComponent(temp.m_Original))
					{
						pseudoRandomSeed = m_PseudoRandomSeedData[temp.m_Original];
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldSecondaryObject, pseudoRandomSeed);
				}
				if ((flags & SubObjectFlags.OnGround) == 0)
				{
					if (m_ElevationData.HasComponent(oldSecondaryObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Elevation>(jobIndex, oldSecondaryObject, elevation);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, oldSecondaryObject, elevation);
					}
				}
				else if (m_ElevationData.HasComponent(oldSecondaryObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Elevation>(jobIndex, oldSecondaryObject);
				}
				return;
			}
			ObjectData objectData = m_PrefabObjectData[prefab];
			if (!((EntityArchetype)(ref objectData.m_Archetype)).Valid)
			{
				return;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val, ref m_SecondaryOwnerTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val, new Owner(owner));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(prefab));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, transformData);
			Temp temp2 = default(Temp);
			if (isTemp)
			{
				temp2.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp2.m_Flags |= TempFlags.Modify;
				}
				if (!isNew)
				{
					temp2.m_Original = FindOriginalSecondaryObject(prefab, Entity.Null, transformData, ref updateData);
				}
				if (m_PrefabObjectGeometryData.HasComponent(prefab))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val, ref m_TempAnimationTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val, temp2);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val, temp2);
				}
				Tree tree3 = default(Tree);
				if (temp2.m_Original != Entity.Null && flag2 && m_PrefabTreeData.HasComponent(prefab) && m_TreeData.TryGetComponent(temp2.m_Original, ref tree3))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, val, tree3);
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
				Road road2 = default(Road);
				if (m_RoadData.TryGetComponent(owner, ref road2))
				{
					StreetLightSystem.UpdateStreetLightState(ref streetLight3, road2);
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<StreetLight>(jobIndex, val, streetLight3);
			}
			if (m_PrefabTrafficLightData.HasComponent(prefab))
			{
				TrafficLight trafficLight3 = default(TrafficLight);
				TrafficLight trafficLight4 = default(TrafficLight);
				if (m_TrafficLightData.TryGetComponent(temp2.m_Original, ref trafficLight4))
				{
					trafficLight3 = trafficLight4;
				}
				trafficLight3.m_GroupMask0 = trafficSignNeeds.m_VehicleMask;
				trafficLight3.m_GroupMask1 = (ushort)(trafficSignNeeds.m_CrossingLeftMask | trafficSignNeeds.m_CrossingRightMask);
				TrafficLights trafficLights2 = default(TrafficLights);
				if (m_TrafficLightsData.TryGetComponent(owner, ref trafficLights2))
				{
					TrafficLightSystem.UpdateTrafficLightState(trafficLights2, ref trafficLight3);
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrafficLight>(jobIndex, val, trafficLight3);
			}
			if (temp2.m_Original == Entity.Null && m_PrefabTreeData.HasComponent(prefab))
			{
				Tree tree4 = ObjectUtils.InitializeTreeState(ToolUtils.GetRandomAge(ref random, ageMask));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, val, tree4);
			}
			if (isNative)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, val, default(Native));
			}
			if (cacheTransform)
			{
				LocalTransformCache localTransformCache2 = default(LocalTransformCache);
				localTransformCache2.m_Position = localTransformData.m_Position;
				localTransformCache2.m_Rotation = localTransformData.m_Rotation;
				localTransformCache2.m_ParentMesh = parentMesh;
				localTransformCache2.m_GroupIndex = groupIndex;
				localTransformCache2.m_Probability = probability;
				localTransformCache2.m_PrefabSubIndex = -1;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, val, localTransformCache2);
			}
			if (flag)
			{
				if (m_PseudoRandomSeedData.HasComponent(temp2.m_Original))
				{
					pseudoRandomSeed = m_PseudoRandomSeedData[temp2.m_Original];
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, pseudoRandomSeed);
			}
			if ((flags & SubObjectFlags.OnGround) == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val, elevation);
			}
		}

		private void EnsurePlaceholderRequirements(Entity owner, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (!updateData.m_RequirementsSearched)
			{
				updateData.EnsurePlaceholderRequirements((Allocator)2);
				if (0 == 0 && m_DefaultTheme != Entity.Null)
				{
					updateData.m_PlaceholderRequirements.Add(m_DefaultTheme);
				}
				updateData.m_RequirementsSearched = true;
			}
		}

		private Entity FindOldSecondaryObject(Entity prefab, Transform transform, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (updateData.m_OldEntities.IsCreated && updateData.m_OldEntities.TryGetFirstValue(prefab, ref val, ref val2))
			{
				result = val;
				float num = math.distance(m_TransformData[val].m_Position, transform.m_Position);
				NativeParallelMultiHashMapIterator<Entity> val3 = val2;
				while (updateData.m_OldEntities.TryGetNextValue(ref val, ref val2))
				{
					float num2 = math.distance(m_TransformData[val].m_Position, transform.m_Position);
					if (num2 < num)
					{
						result = val;
						num = num2;
						val3 = val2;
					}
				}
				updateData.m_OldEntities.Remove(val3);
			}
			return result;
		}

		private Entity FindOriginalSecondaryObject(Entity prefab, Entity original, Transform transform, ref UpdateSecondaryObjectsData updateData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (updateData.m_OriginalEntities.IsCreated && updateData.m_OriginalEntities.TryGetFirstValue(prefab, ref val, ref val2))
			{
				if (val == original)
				{
					updateData.m_OriginalEntities.Remove(val2);
					return original;
				}
				result = val;
				float num = math.distance(m_TransformData[val].m_Position, transform.m_Position);
				NativeParallelMultiHashMapIterator<Entity> val3 = val2;
				while (updateData.m_OriginalEntities.TryGetNextValue(ref val, ref val2))
				{
					if (val == original)
					{
						updateData.m_OriginalEntities.Remove(val2);
						return original;
					}
					float num2 = math.distance(m_TransformData[val].m_Position, transform.m_Position);
					if (num2 < num)
					{
						result = val;
						num = num2;
						val3 = val2;
					}
				}
				updateData.m_OriginalEntities.Remove(val3);
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Curve> __Game_Net_Curve_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> __Game_Prefabs_SubLane_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Secondary> __Game_Objects_Secondary_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLightData> __Game_Prefabs_TrafficLightData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.TrafficSignData> __Game_Prefabs_TrafficSignData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.StreetLightData> __Game_Prefabs_StreetLightData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneDirectionData> __Game_Prefabs_LaneDirectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ThemeData> __Game_Prefabs_ThemeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderObjectData> __Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.UtilityObjectData> __Game_Prefabs_UtilityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLight> __Game_Objects_TrafficLight_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StreetLight> __Game_Objects_StreetLight_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLights> __Game_Net_TrafficLights_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		public ComponentLookup<Road> __Game_Net_Road_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionObject> __Game_Prefabs_NetCompositionObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DefaultNetLane> __Game_Prefabs_DefaultNetLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

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
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_UtilityLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.UtilityLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Curve_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(false);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Prefabs_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubLane>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Secondary_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Secondary>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_TrafficLightData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLightData>(true);
			__Game_Prefabs_TrafficSignData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.TrafficSignData>(true);
			__Game_Prefabs_StreetLightData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.StreetLightData>(true);
			__Game_Prefabs_LaneDirectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneDirectionData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_ThemeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ThemeData>(true);
			__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderObjectData>(true);
			__Game_Prefabs_UtilityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.UtilityObjectData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Objects_TrafficLight_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLight>(true);
			__Game_Objects_StreetLight_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StreetLight>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.SecondaryLane>(true);
			__Game_Net_UtilityLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.UtilityLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_TrafficLights_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLights>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Net_Road_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(false);
			__Game_Prefabs_NetCompositionObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionObject>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
			__Game_Prefabs_DefaultNetLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DefaultNetLane>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_LaneQuery;

	private ComponentTypeSet m_AppliedTypes;

	private ComponentTypeSet m_SecondaryOwnerTypes;

	private ComponentTypeSet m_TempAnimationTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SubObject>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		array[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.UtilityLane>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Owner>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array2[0] = val;
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		m_SecondaryOwnerTypes = new ComponentTypeSet(ComponentType.ReadWrite<Secondary>(), ComponentType.ReadWrite<Owner>());
		m_TempAnimationTypes = new ComponentTypeSet(ComponentType.ReadWrite<Temp>(), ComponentType.ReadWrite<Animation>(), ComponentType.ReadWrite<InterpolatedTransform>());
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_ObjectQuery)).IsEmptyIgnoreFilter;
		bool flag2 = !((EntityQuery)(ref m_LaneQuery)).IsEmptyIgnoreFilter;
		if (flag || flag2)
		{
			NativeQueue<UpdateData> updateQueue = default(NativeQueue<UpdateData>);
			updateQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			if (flag)
			{
				UpdateObjects(updateQueue);
			}
			if (flag2)
			{
				UpdateLanes(updateQueue);
			}
			updateQueue.Dispose(((SystemBase)this).Dependency);
		}
	}

	private void UpdateLanes(NativeQueue<UpdateData> updateQueue)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelMultiHashMap<Entity, UpdateData> updateMap = default(NativeParallelMultiHashMap<Entity, UpdateData>);
		updateMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		FillUpdateMapJob fillUpdateMapJob = new FillUpdateMapJob
		{
			m_UpdateQueue = updateQueue,
			m_UpdateMap = updateMap
		};
		SecondaryLaneAnchorJob obj = new SecondaryLaneAnchorJob
		{
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateMap = updateMap
		};
		JobHandle val = IJobExtensions.Schedule<FillUpdateMapJob>(fillUpdateMapJob, ((SystemBase)this).Dependency);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<SecondaryLaneAnchorJob>(obj, m_LaneQuery, val);
		updateMap.Dispose(val2);
		((SystemBase)this).Dependency = val2;
	}

	private void UpdateObjects(NativeQueue<UpdateData> updateQueue)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<SubObjectOwnerData> ownerQueue = default(NativeQueue<SubObjectOwnerData>);
		ownerQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<SubObjectOwnerData> val = default(NativeList<SubObjectOwnerData>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		CheckSubObjectOwnersJob checkSubObjectOwnersJob = new CheckSubObjectOwnersJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
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
			m_OwnerList = val
		};
		UpdateSubObjectsJob updateSubObjectsJob = new UpdateSubObjectsJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrafficLightData = InternalCompilerInterface.GetComponentLookup<TrafficLightData>(ref __TypeHandle.__Game_Prefabs_TrafficLightData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrafficSignData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.TrafficSignData>(ref __TypeHandle.__Game_Prefabs_TrafficSignData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStreetLightData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.StreetLightData>(ref __TypeHandle.__Game_Prefabs_StreetLightData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneDirectionData = InternalCompilerInterface.GetComponentLookup<LaneDirectionData>(ref __TypeHandle.__Game_Prefabs_LaneDirectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabThemeData = InternalCompilerInterface.GetComponentLookup<ThemeData>(ref __TypeHandle.__Game_Prefabs_ThemeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderObjectData = InternalCompilerInterface.GetComponentLookup<PlaceholderObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityObjectData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.UtilityObjectData>(ref __TypeHandle.__Game_Prefabs_UtilityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightData = InternalCompilerInterface.GetComponentLookup<TrafficLight>(ref __TypeHandle.__Game_Objects_TrafficLight_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StreetLightData = InternalCompilerInterface.GetComponentLookup<StreetLight>(ref __TypeHandle.__Game_Objects_StreetLight_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightsData = InternalCompilerInterface.GetComponentLookup<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionObjects = InternalCompilerInterface.GetBufferLookup<NetCompositionObject>(ref __TypeHandle.__Game_Prefabs_NetCompositionObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderObjects = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectRequirements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultNetLanes = InternalCompilerInterface.GetBufferLookup<DefaultNetLane>(ref __TypeHandle.__Game_Prefabs_DefaultNetLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_RandomSeed = RandomSeed.Next(),
			m_DefaultTheme = m_CityConfigurationSystem.defaultTheme,
			m_AppliedTypes = m_AppliedTypes,
			m_SecondaryOwnerTypes = m_SecondaryOwnerTypes,
			m_TempAnimationTypes = m_TempAnimationTypes,
			m_OwnerList = val.AsDeferredJobArray(),
			m_UpdateQueue = updateQueue.AsParallelWriter()
		};
		val2 = m_ModificationBarrier.CreateCommandBuffer();
		updateSubObjectsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		UpdateSubObjectsJob updateSubObjectsJob2 = updateSubObjectsJob;
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CheckSubObjectOwnersJob>(checkSubObjectOwnersJob2, m_ObjectQuery, ((SystemBase)this).Dependency);
		JobHandle val4 = IJobExtensions.Schedule<CollectSubObjectOwnersJob>(collectSubObjectOwnersJob, val3);
		JobHandle val5 = IJobParallelForDeferExtensions.Schedule<UpdateSubObjectsJob, SubObjectOwnerData>(updateSubObjectsJob2, val, 1, val4);
		ownerQueue.Dispose(val4);
		val.Dispose(val5);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val5);
		((SystemBase)this).Dependency = val5;
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
	public SecondaryObjectSystem()
	{
	}
}
