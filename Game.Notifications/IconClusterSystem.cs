using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Notifications;

[CompilerGenerated]
public class IconClusterSystem : GameSystemBase, IPreDeserialize
{
	public struct ClusterData
	{
		private NativeList<IconCluster> m_Clusters;

		private NativeList<ClusterIcon> m_Icons;

		private NativeList<int> m_Roots;

		public bool isEmpty => m_Clusters.Length == 0;

		public ClusterData(NativeList<IconCluster> clusters, NativeList<ClusterIcon> icons, NativeList<int> roots)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Clusters = clusters;
			m_Icons = icons;
			m_Roots = roots;
		}

		public bool GetRoot(ref int index, out IconCluster cluster)
		{
			if (m_Roots.Length > index)
			{
				cluster = m_Clusters[m_Roots[index++]];
				return true;
			}
			cluster = default(IconCluster);
			return false;
		}

		public IconCluster GetCluster(int index)
		{
			return m_Clusters[index];
		}

		public NativeArray<ClusterIcon> GetIcons(int firstIcon, int iconCount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return m_Icons.AsArray().GetSubArray(firstIcon, iconCount);
		}
	}

	public struct IconCluster : IEquatable<IconCluster>
	{
		private float3 m_Center;

		private float3 m_Size;

		private int2 m_SubClusters;

		private float m_DistanceFactor;

		private float m_Radius;

		private int m_ParentCluster;

		private NativeHeapBlock m_IconAllocation;

		private int m_IconCount;

		private int m_Level;

		private int m_PrefabIndex;

		private IconClusterLayer m_Layer;

		private IconFlags m_Flags;

		private bool m_IsMoving;

		private bool m_IsTemp;

		public float3 center => m_Center;

		public float3 size => m_Size;

		public float distanceFactor => m_DistanceFactor;

		public IconClusterLayer layer => m_Layer;

		public IconFlags flags => m_Flags;

		public bool isMoving => m_IsMoving;

		public bool isTemp => m_IsTemp;

		public int parentCluster => m_ParentCluster;

		public int level => m_Level;

		public int prefabIndex => m_PrefabIndex;

		public IconCluster(float3 center, float3 size, int parentCluster, int2 subClusters, float radius, float distanceFactor, NativeHeapBlock iconAllocation, IconClusterLayer layer, IconFlags flags, int iconCount, int level, int prefabIndex, bool isMoving, bool isTemp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			m_Center = center;
			m_Size = size;
			m_ParentCluster = parentCluster;
			m_SubClusters = subClusters;
			m_DistanceFactor = distanceFactor;
			m_Radius = radius;
			m_IconAllocation = iconAllocation;
			m_Layer = layer;
			m_Flags = flags;
			m_IconCount = iconCount;
			m_Level = level;
			m_PrefabIndex = prefabIndex;
			m_IsMoving = isMoving;
			m_IsTemp = isTemp;
		}

		public IconCluster(IconCluster cluster1, IconCluster cluster2, int index1, int index2, NativeHeapBlock iconAllocation, int iconCount, int level)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			float3 val = math.min(cluster1.m_Center - cluster1.m_Size, cluster2.m_Center - cluster2.m_Size) * 0.5f;
			float3 val2 = math.max(cluster1.m_Center + cluster1.m_Size, cluster2.m_Center + cluster2.m_Size) * 0.5f;
			float num = math.distance(cluster1.m_Center, cluster2.m_Center);
			float num2 = math.select(1f, 0.5f, iconCount == cluster1.m_IconCount + cluster2.m_IconCount);
			m_Center = val + val2;
			m_Size = val2 - val;
			m_ParentCluster = 0;
			m_SubClusters = new int2(index1, index2);
			m_IconAllocation = iconAllocation;
			m_IconCount = iconCount;
			m_Radius = math.max(cluster1.m_Radius, cluster2.m_Radius);
			m_DistanceFactor = math.max(cluster1.m_DistanceFactor, cluster2.m_DistanceFactor);
			m_DistanceFactor = math.max(m_DistanceFactor, num / (m_Radius * num2));
			m_PrefabIndex = ((cluster1.m_PrefabIndex == cluster2.m_PrefabIndex) ? cluster1.m_PrefabIndex : (-1));
			m_Level = level;
			m_IsMoving = cluster1.m_IsMoving && cluster2.m_IsMoving;
			m_IsTemp = cluster1.m_IsTemp;
			m_Layer = cluster1.m_Layer;
			m_Flags = cluster1.m_Flags & cluster2.m_Flags;
		}

		public static void SetParent(ref IconCluster cluster, int parent)
		{
			cluster.m_ParentCluster = parent;
		}

		public bool GetSubClusters(out int2 subClusters)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			subClusters = m_SubClusters;
			return m_SubClusters.x != 0;
		}

		public float GetRadius(float distance)
		{
			return CalculateRadius(m_Radius, distance);
		}

		public static float CalculateRadius(float radius, float distance)
		{
			return radius * math.pow(distance, 0.6f) * 0.063f;
		}

		public Bounds3 GetBounds(float distance, float3 cameraUp)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			float radius = GetRadius(distance);
			float3 val = m_Center + cameraUp * radius;
			float3 val2 = m_Size + radius;
			return new Bounds3(val - val2, val + val2);
		}

		public bool KeepCluster(float distance)
		{
			if (m_SubClusters.x >= 0)
			{
				return math.pow(distance, 0.6f) * 0.18900001f * (1f + distance * 2E-05f) > m_DistanceFactor;
			}
			return true;
		}

		public NativeArray<ClusterIcon> GetIcons(ClusterData clusterData)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return clusterData.GetIcons((int)((NativeHeapBlock)(ref m_IconAllocation)).Begin, m_IconCount);
		}

		public NativeHeapBlock GetIcons(out int firstIcon, out int iconCount)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			firstIcon = (int)((NativeHeapBlock)(ref m_IconAllocation)).Begin;
			iconCount = m_IconCount;
			return m_IconAllocation;
		}

		public bool Equals(IconCluster other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (((float3)(ref m_Center)).Equals(other.m_Center) && ((float3)(ref m_Size)).Equals(other.m_Size) && ((int2)(ref m_SubClusters)).Equals(other.m_SubClusters) && m_DistanceFactor.Equals(other.m_DistanceFactor) && m_Radius.Equals(other.m_Radius) && m_ParentCluster.Equals(other.m_ParentCluster) && ((NativeHeapBlock)(ref m_IconAllocation)).Begin.Equals(((NativeHeapBlock)(ref other.m_IconAllocation)).Begin) && m_IconCount.Equals(other.m_IconCount) && m_Level.Equals(other.m_Level) && m_PrefabIndex.Equals(other.m_PrefabIndex) && m_Layer == other.m_Layer && m_Flags == other.m_Flags && m_IsMoving == other.m_IsMoving)
			{
				return m_IsTemp == other.m_IsTemp;
			}
			return false;
		}
	}

	public struct ClusterIcon
	{
		private Entity m_Icon;

		private Entity m_Prefab;

		private float2 m_Order;

		private IconPriority m_Priority;

		private IconFlags m_Flags;

		public Entity icon => m_Icon;

		public Entity prefab => m_Prefab;

		public float2 order => m_Order;

		public IconPriority priority => m_Priority;

		public IconFlags flags => m_Flags;

		public ClusterIcon(Entity icon, Entity prefab, float2 order, IconPriority priority, IconFlags flags)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Icon = icon;
			m_Prefab = prefab;
			m_Order = order;
			m_Priority = priority;
			m_Flags = flags;
		}
	}

	private struct TempIconCluster : IComparable<TempIconCluster>
	{
		public float3 m_Center;

		public float3 m_Size;

		public Entity m_Icon;

		public Entity m_Prefab;

		public int2 m_SubClusters;

		public float m_Radius;

		public int m_FriendIndex;

		public int m_MovingGroup;

		public IconPriority m_Priority;

		public IconClusterLayer m_ClusterLayer;

		public IconFlags m_Flags;

		public TempIconCluster(Entity entity, Entity prefab, Icon icon, NotificationIconDisplayData displayData, int movingGroup)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			m_Center = icon.m_Location;
			m_Size = default(float3);
			m_Icon = entity;
			m_Prefab = prefab;
			m_SubClusters = int2.op_Implicit(0);
			m_Radius = math.lerp(displayData.m_MinParams.x, displayData.m_MaxParams.x, (float)(int)icon.m_Priority * 0.003921569f);
			m_FriendIndex = 0;
			m_MovingGroup = movingGroup;
			m_Priority = icon.m_Priority;
			m_ClusterLayer = icon.m_ClusterLayer;
			m_Flags = icon.m_Flags;
		}

		public TempIconCluster(in TempIconCluster cluster1, in TempIconCluster cluster2, int index1, int index2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			float3 val = math.min(cluster1.m_Center - cluster1.m_Size, cluster2.m_Center - cluster2.m_Size) * 0.5f;
			float3 val2 = math.max(cluster1.m_Center + cluster1.m_Size, cluster2.m_Center + cluster2.m_Size) * 0.5f;
			m_Center = val + val2;
			m_Size = val2 - val;
			m_Icon = Entity.Null;
			m_Prefab = ((cluster1.m_Prefab == cluster2.m_Prefab) ? cluster1.m_Prefab : Entity.Null);
			m_SubClusters = new int2(index1, index2);
			m_Radius = math.max(cluster1.m_Radius, cluster2.m_Radius);
			m_FriendIndex = 0;
			m_MovingGroup = math.select(-1, cluster1.m_MovingGroup, cluster1.m_MovingGroup == cluster2.m_MovingGroup);
			m_Priority = (IconPriority)math.max((int)cluster1.m_Priority, (int)cluster2.m_Priority);
			m_ClusterLayer = cluster1.m_ClusterLayer;
			m_Flags = cluster1.m_Flags & cluster2.m_Flags;
		}

		public int CompareTo(TempIconCluster other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			return math.select(m_Icon.Index - other.m_Icon.Index, math.select(-1, 1, m_Center.x > other.m_Center.x), m_Center.x != other.m_Center.x);
		}
	}

	private struct TreeBounds : IEquatable<TreeBounds>, IBounds2<TreeBounds>
	{
		public Bounds3 m_Bounds;

		public ulong m_LevelMask;

		public ulong m_LayerMask;

		public void Reset()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			m_LevelMask = 0uL;
			m_LayerMask = 0uL;
		}

		public float2 Center()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Center(((Bounds3)(ref m_Bounds)).xz);
		}

		public float2 Size()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Size(((Bounds3)(ref m_Bounds)).xz);
		}

		public TreeBounds Merge(TreeBounds other)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return new TreeBounds
			{
				m_Bounds = (m_Bounds | other.m_Bounds),
				m_LevelMask = (m_LevelMask | other.m_LevelMask),
				m_LayerMask = (m_LayerMask | other.m_LayerMask)
			};
		}

		public bool Intersect(TreeBounds other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(m_Bounds, other.m_Bounds) && (m_LevelMask & other.m_LevelMask) != 0L)
			{
				return (m_LayerMask & other.m_LayerMask) != 0;
			}
			return false;
		}

		public bool Equals(TreeBounds other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Bounds3)(ref m_Bounds)).Equals(other.m_Bounds) && m_LevelMask == other.m_LevelMask)
			{
				return m_LayerMask == other.m_LayerMask;
			}
			return false;
		}
	}

	[BurstCompile]
	private struct IconChunkJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<DisallowCluster> m_DisallowClusterType;

		[ReadOnly]
		public ComponentTypeHandle<Animation> m_AnimationType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> m_IconDisplayData;

		public ComponentTypeHandle<Icon> m_IconType;

		public IconData m_IconData;

		public NativeList<UnsafeHashSet<int>> m_Orphans;

		public NativeList<TempIconCluster> m_TempBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			NativeList<int> tempList = default(NativeList<int>);
			tempList._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			m_IconData.HandleOldRoots(m_Orphans, tempList);
			HandleChunks(m_Orphans, m_TempBuffer);
			tempList.Dispose();
		}

		public void HandleChunks(NativeList<UnsafeHashSet<int>> orphans, NativeList<TempIconCluster> tempBuffer)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			bool flag = m_IconData.m_IconClusters.Length <= 1;
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Icon> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Icon>(ref m_IconType);
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType) || ((ArchetypeChunk)(ref val)).Has<DisallowCluster>(ref m_DisallowClusterType) || ((ArchetypeChunk)(ref val)).Has<Animation>(ref m_AnimationType))
				{
					if (!((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType))
					{
						for (int j = 0; j < nativeArray.Length; j++)
						{
							ref Icon reference = ref CollectionUtils.ElementAt<Icon>(nativeArray, j);
							m_IconData.Remove(reference.m_ClusterIndex, 0, -1, orphans);
							reference.m_ClusterIndex = 0;
						}
					}
					continue;
				}
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				if (((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType))
				{
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						Entity entity = nativeArray2[k];
						Icon icon = nativeArray[k];
						PrefabRef prefabRef = nativeArray4[k];
						if (m_IconDisplayData.IsComponentEnabled(prefabRef.m_Prefab))
						{
							NotificationIconDisplayData displayData = m_IconDisplayData[prefabRef.m_Prefab];
							int movingGroup = -1;
							if (CollectionUtils.TryGet<Owner>(nativeArray3, k, ref owner) && m_MovingData.HasComponent(owner.m_Owner))
							{
								movingGroup = owner.m_Owner.Index;
							}
							TempIconCluster tempIconCluster = new TempIconCluster(entity, prefabRef.m_Prefab, icon, displayData, movingGroup);
							tempBuffer.Add(ref tempIconCluster);
						}
					}
					continue;
				}
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Entity icon2 = nativeArray2[l];
					PrefabRef prefabRef2 = nativeArray4[l];
					ref Icon reference2 = ref CollectionUtils.ElementAt<Icon>(nativeArray, l);
					reference2.m_ClusterIndex = math.select(reference2.m_ClusterIndex, 0, flag);
					if (m_IconDisplayData.IsComponentEnabled(prefabRef2.m_Prefab))
					{
						NotificationIconDisplayData notificationIconDisplayData = m_IconDisplayData[prefabRef2.m_Prefab];
						if (reference2.m_ClusterIndex == 0)
						{
							reference2.m_ClusterIndex = m_IconData.GetNewClusterIndex();
						}
						float num = math.lerp(notificationIconDisplayData.m_MinParams.x, notificationIconDisplayData.m_MaxParams.x, (float)(int)reference2.m_Priority * 0.003921569f);
						int num2 = -1;
						if (CollectionUtils.TryGet<Owner>(nativeArray3, l, ref owner2) && m_MovingData.HasComponent(owner2.m_Owner))
						{
							num2 = owner2.m_Owner.Index;
						}
						ref IconCluster reference3 = ref m_IconData.m_IconClusters.ElementAt(reference2.m_ClusterIndex);
						int firstIcon;
						int iconCount;
						NativeHeapBlock iconAllocation = reference3.GetIcons(out firstIcon, out iconCount);
						if (iconCount == 0)
						{
							iconCount = 1;
							iconAllocation = m_IconData.AllocateIcons(iconCount);
							firstIcon = (int)((NativeHeapBlock)(ref iconAllocation)).Begin;
						}
						for (int m = 0; m < iconCount; m++)
						{
							m_IconData.m_ClusterIcons.ElementAt(firstIcon + m) = new ClusterIcon(icon2, prefabRef2.m_Prefab, ((float3)(ref reference2.m_Location)).xz, reference2.m_Priority, reference2.m_Flags);
						}
						IconCluster iconCluster = new IconCluster(reference2.m_Location, float3.op_Implicit(num), reference3.parentCluster, int2.op_Implicit(0), num, 0f, iconAllocation, reference2.m_ClusterLayer, reference2.m_Flags, iconCount, 0, prefabRef2.m_Prefab.Index, num2 != -1, isTemp: false);
						if (!iconCluster.Equals(reference3))
						{
							m_IconData.Remove(reference3.parentCluster, reference2.m_ClusterIndex, -1, orphans);
							reference3 = iconCluster;
							IconCluster.SetParent(ref reference3, 0);
							m_IconData.AddOrphan(reference2.m_ClusterIndex, 0, orphans);
							m_IconData.m_ClusterTree.AddOrUpdate(reference2.m_ClusterIndex, new TreeBounds
							{
								m_Bounds = new Bounds3(reference3.center - reference3.size, reference3.center + reference3.size),
								m_LevelMask = 1uL,
								m_LayerMask = (ulong)(1L << (int)reference3.layer)
							});
						}
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct IconClusterJob : IJob
	{
		public IconData m_IconData;

		public NativeList<UnsafeHashSet<int>> m_Orphans;

		public NativeList<TempIconCluster> m_TempBuffer;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			NativeQuadTreeSelectorBuffer<float> selectorBuffer = default(NativeQuadTreeSelectorBuffer<float>);
			selectorBuffer._002Ector((Allocator)2);
			NativeList<ClusterIcon> tempIcons = default(NativeList<ClusterIcon>);
			tempIcons._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			m_IconData.HandleOrphans(m_Orphans, selectorBuffer, tempIcons);
			m_IconData.HandleTemps(m_TempBuffer, tempIcons);
			selectorBuffer.Dispose();
			tempIcons.Dispose();
		}
	}

	private struct IconData
	{
		private struct Selector : INativeQuadTreeSelector<int, TreeBounds, float>, IUnsafeQuadTreeSelector<int, TreeBounds, float>
		{
			public float m_BestCost;

			public float m_BestDistance;

			public int m_BestClusterIndex;

			public int m_IgnoreClusterIndex;

			public ulong m_LevelMask;

			public ulong m_LayerMask;

			public IconCluster m_Cluster;

			public NativeList<IconCluster> m_IconClusters;

			public bool Check(TreeBounds bounds, out float priority)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				priority = MathUtils.DistanceSquared(bounds.m_Bounds, m_Cluster.center);
				if (priority <= m_BestDistance && (bounds.m_LevelMask & m_LevelMask) != 0L)
				{
					return (bounds.m_LayerMask & m_LayerMask) != 0;
				}
				return false;
			}

			public bool Check(float priority)
			{
				return priority <= m_BestDistance;
			}

			public bool Better(float priority1, float priority2)
			{
				return priority1 < priority2;
			}

			public void Select(TreeBounds bounds, int item)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				if (!(MathUtils.DistanceSquared(bounds.m_Bounds, m_Cluster.center) > m_BestDistance) && (bounds.m_LevelMask & m_LevelMask) != 0L && (bounds.m_LayerMask & m_LayerMask) != 0L && item != m_IgnoreClusterIndex)
				{
					IconCluster iconCluster = m_IconClusters[item];
					bool flag = iconCluster.prefabIndex == m_Cluster.prefabIndex && ((iconCluster.flags | m_Cluster.flags) & IconFlags.Unique) == 0;
					float num = math.distancesq(iconCluster.center, m_Cluster.center);
					float num2 = math.select(num, num * 0.25f, flag);
					if (num2 < m_BestCost || (num2 == m_BestCost && iconCluster.parentCluster == 0))
					{
						m_BestClusterIndex = item;
						m_BestCost = num2;
						m_BestDistance = math.select(num * 4.01f, num * 1.01f, flag);
					}
				}
			}
		}

		public NativeQuadTree<int, TreeBounds> m_ClusterTree;

		public NativeHeapAllocator m_IconAllocator;

		public NativeList<IconCluster> m_IconClusters;

		public NativeList<ClusterIcon> m_ClusterIcons;

		public NativeList<int> m_RootClusters;

		public NativeList<int> m_FreeClusterIndices;

		public void ValidateClusters()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			NativeList<int> val = default(NativeList<int>);
			val._002Ector(64, AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashSet<int> val2 = default(NativeHashSet<int>);
			val2._002Ector(64, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_RootClusters.Length; i++)
			{
				int num = m_RootClusters[i];
				val.Add(ref num);
			}
			bool flag = false;
			while (val.Length != 0)
			{
				if (!val2.Add(val[val.Length - 1]))
				{
					flag = true;
					val.Clear();
					val2.Clear();
					break;
				}
				IconCluster iconCluster = m_IconClusters[val[val.Length - 1]];
				val.RemoveAtSwapBack(val.Length - 1);
				if (iconCluster.GetSubClusters(out var subClusters))
				{
					val.Add(ref subClusters.x);
					val.Add(ref subClusters.y);
				}
			}
			if (!flag)
			{
				return;
			}
			for (int j = 0; j < m_RootClusters.Length; j++)
			{
				int num = m_RootClusters[j];
				val.Add(ref num);
			}
			while (val.Length != 0)
			{
				IconCluster iconCluster2 = m_IconClusters[val[val.Length - 1]];
				iconCluster2.GetSubClusters(out var subClusters2);
				Debug.Log((object)$"{iconCluster2.level}: {val[val.Length - 1]} -> {subClusters2.x}, {subClusters2.y}");
				if (!val2.Add(val[val.Length - 1]))
				{
					m_ClusterTree.Clear();
					((NativeHeapAllocator)(ref m_IconAllocator)).Clear();
					m_IconClusters.Clear();
					m_ClusterIcons.Clear();
					m_RootClusters.Clear();
					m_FreeClusterIndices.Clear();
					ref NativeList<IconCluster> reference = ref m_IconClusters;
					IconCluster iconCluster3 = default(IconCluster);
					reference.Add(ref iconCluster3);
					m_ClusterIcons.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_IconAllocator)).Size);
					break;
				}
				val.RemoveAtSwapBack(val.Length - 1);
				if (iconCluster2.GetSubClusters(out var subClusters3))
				{
					val.Add(ref subClusters3.x);
					val.Add(ref subClusters3.y);
				}
			}
		}

		public void HandleOldRoots(NativeList<UnsafeHashSet<int>> orphans, NativeList<int> tempList)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_RootClusters.Length; i++)
			{
				int num = m_RootClusters[i];
				ref IconCluster reference = ref m_IconClusters.ElementAt(num);
				if (reference.isTemp)
				{
					RemoveTemp(num, tempList);
				}
				else
				{
					AddOrphan(num, reference.level, orphans);
				}
			}
			m_RootClusters.Clear();
		}

		public void HandleOrphans(NativeList<UnsafeHashSet<int>> orphans, NativeQuadTreeSelectorBuffer<float> selectorBuffer, NativeList<ClusterIcon> tempIcons)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			UnsafeHashSet<int> val = default(UnsafeHashSet<int>);
			Selector selector = new Selector
			{
				m_IconClusters = m_IconClusters
			};
			for (; i < orphans.Length; i++)
			{
				UnsafeHashSet<int> val2 = orphans[i];
				selector.m_LevelMask = (ulong)(1L << i);
				while (val2.IsCreated)
				{
					UnsafeHashSet<int> val3 = val2;
					Enumerator<int> enumerator = val3.GetEnumerator();
					orphans[i] = val;
					while (enumerator.MoveNext())
					{
						ref IconCluster reference = ref m_IconClusters.ElementAt(enumerator.Current);
						if (reference.parentCluster != 0)
						{
							continue;
						}
						if (i != 63)
						{
							selector.m_LayerMask = (ulong)(1L << (int)reference.layer);
							selector.m_BestCost = float.MaxValue;
							selector.m_BestDistance = float.MaxValue;
							selector.m_BestClusterIndex = 0;
							selector.m_IgnoreClusterIndex = enumerator.Current;
							selector.m_Cluster = reference;
							m_ClusterTree.Select<Selector, float>(ref selector, selectorBuffer);
							if (selector.m_BestClusterIndex != 0)
							{
								float num = selector.m_BestCost;
								int num2 = selector.m_BestClusterIndex;
								ref IconCluster reference2 = ref m_IconClusters.ElementAt(num2);
								if (reference2.parentCluster == 0 || num < GetCurrentCost(num2, ref reference2))
								{
									selector.m_BestCost = float.MaxValue;
									selector.m_BestDistance = float.MaxValue;
									selector.m_BestClusterIndex = 0;
									selector.m_IgnoreClusterIndex = num2;
									selector.m_Cluster = reference2;
									m_ClusterTree.Select<Selector, float>(ref selector, selectorBuffer);
									if (selector.m_BestClusterIndex == enumerator.Current || selector.m_BestCost == num)
									{
										if (reference2.parentCluster != 0)
										{
											Remove(reference2.parentCluster, num2, i, orphans);
										}
										int newClusterIndex = GetNewClusterIndex();
										reference = ref m_IconClusters.ElementAt(enumerator.Current);
										reference2 = ref m_IconClusters.ElementAt(num2);
										ref IconCluster reference3 = ref m_IconClusters.ElementAt(newClusterIndex);
										AddIcons(reference, reference2, tempIcons);
										NativeHeapBlock iconAllocation = AllocateIcons(tempIcons.Length);
										for (int j = 0; j < tempIcons.Length; j++)
										{
											m_ClusterIcons[(int)((NativeHeapBlock)(ref iconAllocation)).Begin + j] = tempIcons[j];
										}
										reference3 = new IconCluster(reference, reference2, enumerator.Current, num2, iconAllocation, tempIcons.Length, i + 1);
										tempIcons.Clear();
										m_ClusterTree.AddOrUpdate(newClusterIndex, new TreeBounds
										{
											m_Bounds = new Bounds3(reference3.center - reference3.size, reference3.center + reference3.size),
											m_LevelMask = (ulong)(1L << i + 1),
											m_LayerMask = (ulong)(1L << (int)reference.layer)
										});
										UpdateLevelMask(num2, (ulong)((-1L << reference2.level) & ~(-1L << i + 1)));
										IconCluster.SetParent(ref reference, newClusterIndex);
										IconCluster.SetParent(ref reference2, newClusterIndex);
										AddOrphan(newClusterIndex, i + 1, orphans);
										continue;
									}
								}
								ulong num3 = (ulong)(-1L << reference.level);
								num3 = math.select(num3, num3 & (ulong)(~(-1L << i + 2)), i < 62);
								UpdateLevelMask(enumerator.Current, num3);
								AddOrphan(enumerator.Current, i + 1, orphans);
								continue;
							}
						}
						UpdateLevelMask(enumerator.Current, (ulong)(1L << reference.level));
						ref NativeList<int> reference4 = ref m_RootClusters;
						int current = enumerator.Current;
						reference4.Add(ref current);
					}
					enumerator.Dispose();
					val = val3;
					val.Clear();
					val2 = orphans[i];
					if (val2.IsCreated && val2.IsEmpty)
					{
						val2.Dispose();
					}
				}
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}

		public void HandleTemps(NativeList<TempIconCluster> tempBuffer, NativeList<ClusterIcon> tempIcons)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			int num = tempBuffer.Length;
			if (num > 1)
			{
				NativeSortExtension.Sort<TempIconCluster>(tempBuffer);
			}
			NativeArray<TempIconCluster> val = default(NativeArray<TempIconCluster>);
			val._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<TempIconCluster> a = tempBuffer.AsArray();
			NativeArray<TempIconCluster> b = val;
			while (num > 1)
			{
				for (int i = 0; i < num; i++)
				{
					ref TempIconCluster reference = ref CollectionUtils.ElementAt<TempIconCluster>(a, i);
					reference.m_FriendIndex = i;
					float num2 = float.MaxValue;
					float num3 = float.MaxValue;
					bool flag = true;
					int num4 = i - 1;
					int num5 = i + 1;
					while (num4 >= 0 || num5 < num)
					{
						if (num4 >= 0)
						{
							ref TempIconCluster reference2 = ref CollectionUtils.ElementAt<TempIconCluster>(a, num4);
							float num6 = reference.m_Center.x - reference2.m_Center.x;
							if (num6 * num6 > num3)
							{
								num4 = -1;
							}
							else
							{
								if (reference.m_ClusterLayer == reference2.m_ClusterLayer)
								{
									bool flag2 = reference2.m_Prefab == reference.m_Prefab && ((reference2.m_Flags | reference.m_Flags) & IconFlags.Unique) == 0;
									float num7 = math.distancesq(reference2.m_Center, reference.m_Center);
									float num8 = math.select(num7, num7 * 0.25f, flag2);
									if (num8 < num2 || (num8 == num2 && reference2.m_FriendIndex == i))
									{
										reference.m_FriendIndex = num4;
										num2 = num8;
										num3 = math.select(num7 * 4.01f, num7 * 1.01f, flag2);
										flag = reference2.m_FriendIndex != i;
									}
								}
								num4--;
							}
						}
						if (num5 >= num)
						{
							continue;
						}
						ref TempIconCluster reference3 = ref CollectionUtils.ElementAt<TempIconCluster>(a, num5);
						float num9 = reference3.m_Center.x - reference.m_Center.x;
						if (num9 * num9 > num3)
						{
							num5 = num;
							continue;
						}
						if (reference.m_ClusterLayer == reference3.m_ClusterLayer)
						{
							bool flag3 = reference.m_Prefab == reference3.m_Prefab && ((reference.m_Flags | reference3.m_Flags) & IconFlags.Unique) == 0;
							float num10 = math.distancesq(reference.m_Center, reference3.m_Center);
							float num11 = math.select(num10, num10 * 0.25f, flag3);
							if (num11 < num2 || (num11 == num2 && flag))
							{
								reference.m_FriendIndex = num5;
								num2 = num11;
								num3 = math.select(num10 * 4.01f, num10 * 1.01f, flag3);
								flag = false;
							}
						}
						num5++;
					}
				}
				int num12 = 0;
				for (int j = 0; j < num; j++)
				{
					ref TempIconCluster reference4 = ref CollectionUtils.ElementAt<TempIconCluster>(a, j);
					ref TempIconCluster reference5 = ref CollectionUtils.ElementAt<TempIconCluster>(a, reference4.m_FriendIndex);
					if (reference5.m_FriendIndex == j)
					{
						if (reference4.m_FriendIndex < j)
						{
							int newClusterIndex = GetNewClusterIndex();
							int newClusterIndex2 = GetNewClusterIndex();
							b[num12++] = new TempIconCluster(in reference5, in reference4, newClusterIndex2, newClusterIndex);
							AddCluster(in reference5, newClusterIndex2, isRoot: false, tempIcons);
							AddCluster(in reference4, newClusterIndex, isRoot: false, tempIcons);
						}
						else if (reference4.m_FriendIndex == j)
						{
							AddCluster(in reference4, GetNewClusterIndex(), isRoot: true, tempIcons);
						}
					}
					else
					{
						b[num12++] = reference4;
					}
				}
				CommonUtils.Swap(ref a, ref b);
				if (num == num12)
				{
					break;
				}
				num = num12;
				if (num > 1)
				{
					NativeSortExtension.Sort<TempIconCluster>(a.GetSubArray(0, num));
				}
			}
			for (int k = 0; k < num; k++)
			{
				AddCluster(in CollectionUtils.ElementAt<TempIconCluster>(a, k), GetNewClusterIndex(), isRoot: true, tempIcons);
			}
			val.Dispose();
		}

		private void AddCluster(in TempIconCluster tempCluster, int index, bool isRoot, NativeList<ClusterIcon> tempIcons)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			int num2;
			NativeHeapBlock iconAllocation;
			float num5;
			if (tempCluster.m_SubClusters.x != 0)
			{
				IconCluster subCluster = m_IconClusters[tempCluster.m_SubClusters.x];
				IconCluster subCluster2 = m_IconClusters[tempCluster.m_SubClusters.y];
				int num = AddIcons(subCluster, subCluster2, tempIcons);
				num2 = tempIcons.Length;
				iconAllocation = AllocateIcons(num2);
				for (int i = 0; i < tempIcons.Length; i++)
				{
					m_ClusterIcons[(int)((NativeHeapBlock)(ref iconAllocation)).Begin + i] = tempIcons[i];
				}
				tempIcons.Clear();
				float num3 = math.distance(subCluster.center, subCluster2.center);
				float num4 = math.select(1f, 0.5f, num2 == num);
				num5 = math.max(subCluster.distanceFactor, subCluster2.distanceFactor);
				num5 = math.max(num5, num3 / (tempCluster.m_Radius * num4));
			}
			else
			{
				iconAllocation = AllocateIcons(1);
				ref NativeList<ClusterIcon> reference = ref m_ClusterIcons;
				uint begin = ((NativeHeapBlock)(ref iconAllocation)).Begin;
				Entity icon = tempCluster.m_Icon;
				Entity prefab = tempCluster.m_Prefab;
				float3 val = tempCluster.m_Center;
				reference[(int)begin] = new ClusterIcon(icon, prefab, ((float3)(ref val)).xz, tempCluster.m_Priority, tempCluster.m_Flags);
				num2 = 1;
				num5 = 0f;
			}
			m_IconClusters[index] = new IconCluster(tempCluster.m_Center, tempCluster.m_Size, -1, tempCluster.m_SubClusters, tempCluster.m_Radius, num5, iconAllocation, tempCluster.m_ClusterLayer, tempCluster.m_Flags, num2, -1, -1, tempCluster.m_MovingGroup != -1, isTemp: true);
			if (isRoot)
			{
				m_RootClusters.Add(ref index);
			}
		}

		private int AddIcons(IconCluster subCluster1, IconCluster subCluster2, NativeList<ClusterIcon> tempIcons)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			int2 val = default(int2);
			int2 val2 = default(int2);
			subCluster1.GetIcons(out val.x, out val2.x);
			subCluster2.GetIcons(out val.y, out val2.y);
			int result = math.csum(val2);
			val2 += val;
			while (math.all(val < val2))
			{
				ClusterIcon icon = m_ClusterIcons[val.x];
				ClusterIcon icon2 = m_ClusterIcons[val.y];
				if ((int)icon.priority >= (int)icon2.priority)
				{
					AddIcon(icon, tempIcons);
					val.x++;
				}
				else
				{
					AddIcon(icon2, tempIcons);
					val.y++;
				}
			}
			for (int i = val.x; i < val2.x; i++)
			{
				AddIcon(m_ClusterIcons[i], tempIcons);
			}
			for (int j = val.y; j < val2.y; j++)
			{
				AddIcon(m_ClusterIcons[j], tempIcons);
			}
			return result;
		}

		private void UpdateLevelMask(int clusterIndex, ulong levelMask)
		{
			TreeBounds treeBounds = default(TreeBounds);
			if (m_ClusterTree.TryGet(clusterIndex, ref treeBounds) && treeBounds.m_LevelMask != levelMask)
			{
				treeBounds.m_LevelMask = levelMask;
				m_ClusterTree.Update(clusterIndex, treeBounds);
			}
		}

		public NativeHeapBlock AllocateIcons(int iconCount)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			NativeHeapBlock result = ((NativeHeapAllocator)(ref m_IconAllocator)).Allocate((uint)iconCount, 1u);
			while (((NativeHeapBlock)(ref result)).Empty)
			{
				((NativeHeapAllocator)(ref m_IconAllocator)).Resize(((NativeHeapAllocator)(ref m_IconAllocator)).Size + 1024);
				m_ClusterIcons.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_IconAllocator)).Size);
				result = ((NativeHeapAllocator)(ref m_IconAllocator)).Allocate((uint)iconCount, 1u);
			}
			return result;
		}

		private void AddIcon(ClusterIcon icon, NativeList<ClusterIcon> icons)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if ((icon.flags & IconFlags.Unique) == 0)
			{
				for (int i = 0; i < icons.Length; i++)
				{
					ClusterIcon clusterIcon = icons[i];
					if (clusterIcon.prefab == icon.prefab && (clusterIcon.flags & IconFlags.Unique) == 0)
					{
						return;
					}
				}
			}
			icons.Add(ref icon);
		}

		public int GetNewClusterIndex()
		{
			int num;
			if (m_FreeClusterIndices.Length != 0)
			{
				num = m_FreeClusterIndices[m_FreeClusterIndices.Length - 1];
				m_FreeClusterIndices.RemoveAt(m_FreeClusterIndices.Length - 1);
				m_IconClusters[num] = default(IconCluster);
			}
			else
			{
				num = m_IconClusters.Length;
				ref NativeList<IconCluster> reference = ref m_IconClusters;
				IconCluster iconCluster = default(IconCluster);
				reference.Add(ref iconCluster);
			}
			return num;
		}

		private float GetCurrentCost(int clusterIndex, ref IconCluster cluster)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			m_IconClusters.ElementAt(cluster.parentCluster).GetSubClusters(out var subClusters);
			ref IconCluster reference = ref m_IconClusters.ElementAt(math.select(subClusters.x, subClusters.y, clusterIndex == subClusters.x));
			bool flag = reference.prefabIndex == cluster.prefabIndex && ((reference.flags | cluster.flags) & IconFlags.Unique) == 0;
			float num = math.distancesq(reference.center, cluster.center);
			return math.select(num, num * 0.25f, flag);
		}

		public void Remove(int clusterIndex, int subCluster, int subLevel, NativeList<UnsafeHashSet<int>> orphans)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			while (clusterIndex != 0)
			{
				ref IconCluster reference = ref m_IconClusters.ElementAt(clusterIndex);
				m_ClusterTree.TryRemove(clusterIndex);
				RemoveOrphan(clusterIndex, reference.level, orphans);
				int firstIcon;
				int iconCount;
				NativeHeapBlock icons = reference.GetIcons(out firstIcon, out iconCount);
				if (!((NativeHeapBlock)(ref icons)).Empty)
				{
					((NativeHeapAllocator)(ref m_IconAllocator)).Release(icons);
				}
				if (clusterIndex == m_IconClusters.Length - 1)
				{
					m_IconClusters.RemoveAt(clusterIndex);
				}
				else
				{
					m_FreeClusterIndices.Add(ref clusterIndex);
				}
				if (reference.GetSubClusters(out var subClusters))
				{
					if (subClusters.x != subCluster)
					{
						ref IconCluster reference2 = ref m_IconClusters.ElementAt(subClusters.x);
						IconCluster.SetParent(ref reference2, 0);
						int num = math.max(reference2.level, subLevel);
						AddOrphan(subClusters.x, num, orphans);
						UpdateLevelMask(subClusters.x, (ulong)((-1L << reference2.level) & ~(-1L << num + 1)));
					}
					if (subClusters.y != subCluster)
					{
						ref IconCluster reference3 = ref m_IconClusters.ElementAt(subClusters.y);
						IconCluster.SetParent(ref reference3, 0);
						int num2 = math.max(reference3.level, subLevel);
						AddOrphan(subClusters.y, num2, orphans);
						UpdateLevelMask(subClusters.y, (ulong)((-1L << reference3.level) & ~(-1L << num2 + 1)));
					}
				}
				subCluster = clusterIndex;
				clusterIndex = reference.parentCluster;
			}
		}

		private void RemoveTemp(int clusterIndex, NativeList<int> tempList)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			tempList.Add(ref clusterIndex);
			while (!tempList.IsEmpty)
			{
				clusterIndex = tempList[tempList.Length - 1];
				tempList.RemoveAt(tempList.Length - 1);
				ref IconCluster reference = ref m_IconClusters.ElementAt(clusterIndex);
				int firstIcon;
				int iconCount;
				NativeHeapBlock icons = reference.GetIcons(out firstIcon, out iconCount);
				if (!((NativeHeapBlock)(ref icons)).Empty)
				{
					((NativeHeapAllocator)(ref m_IconAllocator)).Release(icons);
				}
				if (clusterIndex == m_IconClusters.Length - 1)
				{
					m_IconClusters.RemoveAt(clusterIndex);
				}
				else
				{
					m_FreeClusterIndices.Add(ref clusterIndex);
				}
				if (reference.GetSubClusters(out var subClusters))
				{
					tempList.Add(ref subClusters.x);
					tempList.Add(ref subClusters.y);
				}
			}
		}

		public void AddOrphan(int clusterIndex, int level, NativeList<UnsafeHashSet<int>> orphans)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (orphans.Length <= level)
			{
				orphans.Length = level + 1;
			}
			ref UnsafeHashSet<int> reference = ref orphans.ElementAt(level);
			if (!reference.IsCreated)
			{
				reference = new UnsafeHashSet<int>(10, AllocatorHandle.op_Implicit((Allocator)3));
			}
			reference.Add(clusterIndex);
		}

		private void RemoveOrphan(int clusterIndex, int level, NativeList<UnsafeHashSet<int>> orphans)
		{
			if (orphans.Length > level)
			{
				ref UnsafeHashSet<int> reference = ref orphans.ElementAt(level);
				if (reference.IsCreated)
				{
					reference.Remove(clusterIndex);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DisallowCluster> __Game_Notifications_DisallowCluster_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Animation> __Game_Notifications_Animation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> __Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		public ComponentTypeHandle<Icon> __Game_Notifications_Icon_RW_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Notifications_DisallowCluster_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DisallowCluster>(true);
			__Game_Notifications_Animation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animation>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NotificationIconDisplayData>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Notifications_Icon_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Icon>(false);
		}
	}

	private EntityQuery m_IconQuery;

	private EntityQuery m_ModifiedQuery;

	private EntityQuery m_ModifiedAndTempQuery;

	private NativeQuadTree<int, TreeBounds> m_ClusterTree;

	private NativeHeapAllocator m_IconAllocator;

	private NativeList<IconCluster> m_IconClusters;

	private NativeList<ClusterIcon> m_ClusterIcons;

	private NativeList<int> m_RootClusters;

	private NativeList<int> m_FreeClusterIndices;

	private JobHandle m_ClusterReadDeps;

	private JobHandle m_ClusterWriteDeps;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Icon>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<DisallowCluster>(),
			ComponentType.Exclude<Animation>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Icon>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_ModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Icon>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_ModifiedAndTempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_ClusterTree = new NativeQuadTree<int, TreeBounds>(1f, (Allocator)4);
		m_IconAllocator = new NativeHeapAllocator(1024u, 1u, (Allocator)4);
		m_IconClusters = new NativeList<IconCluster>(1024, AllocatorHandle.op_Implicit((Allocator)4));
		m_ClusterIcons = new NativeList<ClusterIcon>(1024, AllocatorHandle.op_Implicit((Allocator)4));
		m_RootClusters = new NativeList<int>(8, AllocatorHandle.op_Implicit((Allocator)4));
		m_FreeClusterIndices = new NativeList<int>(128, AllocatorHandle.op_Implicit((Allocator)4));
		ref NativeList<IconCluster> reference = ref m_IconClusters;
		IconCluster iconCluster = default(IconCluster);
		reference.Add(ref iconCluster);
		m_ClusterIcons.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_IconAllocator)).Size);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_IconClusters.IsCreated)
		{
			((JobHandle)(ref m_ClusterReadDeps)).Complete();
			((JobHandle)(ref m_ClusterWriteDeps)).Complete();
			m_ClusterTree.Dispose();
			((NativeHeapAllocator)(ref m_IconAllocator)).Dispose();
			m_IconClusters.Dispose();
			m_ClusterIcons.Dispose();
			m_RootClusters.Dispose();
			m_FreeClusterIndices.Dispose();
		}
		base.OnDestroy();
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_IconQuery : m_ModifiedQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			if (loaded)
			{
				ClearData();
			}
			NativeList<UnsafeHashSet<int>> orphans = default(NativeList<UnsafeHashSet<int>>);
			orphans._002Ector(64, AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<TempIconCluster> tempBuffer = default(NativeList<TempIconCluster>);
			tempBuffer._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
			IconData iconData = new IconData
			{
				m_ClusterTree = m_ClusterTree,
				m_IconAllocator = m_IconAllocator,
				m_IconClusters = m_IconClusters,
				m_ClusterIcons = m_ClusterIcons,
				m_RootClusters = m_RootClusters,
				m_FreeClusterIndices = m_FreeClusterIndices
			};
			IconChunkJob iconChunkJob = default(IconChunkJob);
			val = (loaded ? m_IconQuery : m_ModifiedAndTempQuery);
			JobHandle val2 = default(JobHandle);
			iconChunkJob.m_Chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			iconChunkJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_DisallowClusterType = InternalCompilerInterface.GetComponentTypeHandle<DisallowCluster>(ref __TypeHandle.__Game_Notifications_DisallowCluster_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_AnimationType = InternalCompilerInterface.GetComponentTypeHandle<Animation>(ref __TypeHandle.__Game_Notifications_Animation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_IconDisplayData = InternalCompilerInterface.GetComponentLookup<NotificationIconDisplayData>(ref __TypeHandle.__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_IconType = InternalCompilerInterface.GetComponentTypeHandle<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			iconChunkJob.m_IconData = iconData;
			iconChunkJob.m_Orphans = orphans;
			iconChunkJob.m_TempBuffer = tempBuffer;
			IconChunkJob iconChunkJob2 = iconChunkJob;
			IconClusterJob obj = new IconClusterJob
			{
				m_IconData = iconData,
				m_Orphans = orphans,
				m_TempBuffer = tempBuffer
			};
			JobHandle val3 = IJobExtensions.Schedule<IconChunkJob>(iconChunkJob2, JobHandle.CombineDependencies(val2, m_ClusterReadDeps, m_ClusterWriteDeps));
			JobHandle val4 = IJobExtensions.Schedule<IconClusterJob>(obj, val3);
			iconChunkJob2.m_Chunks.Dispose(val3);
			orphans.Dispose(val4);
			tempBuffer.Dispose(val4);
			m_ClusterWriteDeps = val4;
			m_ClusterReadDeps = default(JobHandle);
			((SystemBase)this).Dependency = val3;
		}
	}

	public void PreDeserialize(Context context)
	{
		ClearData();
		m_Loaded = true;
	}

	public ClusterData GetIconClusterData(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_ClusterWriteDeps : JobHandle.CombineDependencies(m_ClusterReadDeps, m_ClusterWriteDeps));
		return new ClusterData(m_IconClusters, m_ClusterIcons, m_RootClusters);
	}

	public void AddIconClusterReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ClusterReadDeps = JobHandle.CombineDependencies(m_ClusterReadDeps, jobHandle);
	}

	public void AddIconClusterWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_ClusterWriteDeps = jobHandle;
	}

	public void RecalculateClusters()
	{
		m_Loaded = true;
	}

	private void ClearData()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (m_IconClusters.IsCreated)
		{
			((JobHandle)(ref m_ClusterReadDeps)).Complete();
			((JobHandle)(ref m_ClusterWriteDeps)).Complete();
			m_ClusterReadDeps = default(JobHandle);
			m_ClusterWriteDeps = default(JobHandle);
			m_ClusterTree.Clear();
			((NativeHeapAllocator)(ref m_IconAllocator)).Clear();
			m_IconClusters.Clear();
			m_ClusterIcons.Clear();
			m_RootClusters.Clear();
			m_FreeClusterIndices.Clear();
			ref NativeList<IconCluster> reference = ref m_IconClusters;
			IconCluster iconCluster = default(IconCluster);
			reference.Add(ref iconCluster);
			m_ClusterIcons.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_IconAllocator)).Size);
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
	public IconClusterSystem()
	{
	}
}
