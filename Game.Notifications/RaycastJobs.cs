using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Notifications;

public static class RaycastJobs
{
	[BurstCompile]
	public struct RaycastIconsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public float3 m_CameraUp;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public IconClusterSystem.ClusterData m_ClusterData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_IconChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Icon> m_IconType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Static> m_StaticData;

		[ReadOnly]
		public ComponentLookup<Object> m_ObjectData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> m_IconDisplayData;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			RaycastInput input = m_Input[index];
			if ((input.m_TypeMask & (TypeMask.StaticObjects | TypeMask.MovingObjects | TypeMask.Icons)) != TypeMask.None)
			{
				if (!m_ClusterData.isEmpty)
				{
					CheckClusters(index, input);
				}
				if (m_IconChunks.Length != 0)
				{
					CheckChunks(index, input);
				}
			}
		}

		private void CheckClusters(int raycastIndex, RaycastInput input)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			IconLayerMask iconLayerMask = input.m_IconLayerMask;
			if ((input.m_TypeMask & TypeMask.StaticObjects) != TypeMask.None)
			{
				iconLayerMask |= IconLayerMask.Marker;
			}
			float3 a = input.m_Line.a;
			NativeList<IconClusterSystem.IconCluster> val = default(NativeList<IconClusterSystem.IconCluster>);
			val._002Ector(64, AllocatorHandle.op_Implicit((Allocator)2));
			int index = 0;
			IconClusterSystem.IconCluster cluster;
			while (m_ClusterData.GetRoot(ref index, out cluster))
			{
				if ((NotificationsUtils.GetIconLayerMask(cluster.layer) & iconLayerMask) != IconLayerMask.None)
				{
					val.Add(ref cluster);
				}
			}
			float num4 = default(float);
			float2 val5 = default(float2);
			while (val.Length != 0)
			{
				IconClusterSystem.IconCluster iconCluster = val[val.Length - 1];
				val.RemoveAtSwapBack(val.Length - 1);
				float distance = math.distance(a, iconCluster.center);
				int2 subClusters;
				if (iconCluster.KeepCluster(distance))
				{
					float radius = iconCluster.GetRadius(distance);
					NativeArray<IconClusterSystem.ClusterIcon> icons = iconCluster.GetIcons(m_ClusterData);
					float3 val2 = m_CameraRight * (radius * 0.5f);
					float3 val3 = iconCluster.center + val2 * ((float)(icons.Length - 1) * 0.5f);
					float3 val4 = m_CameraUp * radius;
					RaycastResult result = default(RaycastResult);
					float num = radius;
					for (int num2 = icons.Length - 1; num2 >= 0; num2--)
					{
						IconClusterSystem.ClusterIcon clusterIcon = icons[num2];
						if (!m_TempData.HasComponent(clusterIcon.icon))
						{
							float num3 = MathUtils.Distance(input.m_Line, val3 + val4, ref num4);
							if (num3 < num)
							{
								num = num3;
								result.m_Owner = clusterIcon.icon;
								result.m_Hit.m_HitEntity = clusterIcon.icon;
								result.m_Hit.m_Position = val3;
								result.m_Hit.m_HitPosition = MathUtils.Position(input.m_Line, num4);
								result.m_Hit.m_NormalizedDistance = num4 - 100f / math.max(1f, MathUtils.Length(input.m_Line));
							}
						}
						val3 -= val2;
					}
					if (result.m_Owner != Entity.Null)
					{
						ValidateResult(raycastIndex, input, result, iconCluster.layer);
					}
				}
				else if (iconCluster.GetSubClusters(out subClusters) && MathUtils.Intersect(iconCluster.GetBounds(distance, m_CameraUp), input.m_Line, ref val5))
				{
					IconClusterSystem.IconCluster cluster2 = m_ClusterData.GetCluster(subClusters.x);
					val.Add(ref cluster2);
					cluster2 = m_ClusterData.GetCluster(subClusters.y);
					val.Add(ref cluster2);
				}
			}
			val.Dispose();
		}

		private void CheckChunks(int raycastIndex, RaycastInput input)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			IconLayerMask iconLayerMask = input.m_IconLayerMask;
			if ((input.m_TypeMask & (TypeMask.StaticObjects | TypeMask.MovingObjects)) != TypeMask.None)
			{
				iconLayerMask |= IconLayerMask.Marker;
			}
			float3 a = input.m_Line.a;
			float num3 = default(float);
			for (int i = 0; i < m_IconChunks.Length; i++)
			{
				ArchetypeChunk val = m_IconChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Icon> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Icon>(ref m_IconType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Icon icon = nativeArray2[j];
					if ((NotificationsUtils.GetIconLayerMask(icon.m_ClusterLayer) & iconLayerMask) == 0)
					{
						continue;
					}
					PrefabRef prefabRef = nativeArray3[j];
					if (!m_IconDisplayData.IsComponentEnabled(prefabRef.m_Prefab))
					{
						continue;
					}
					NotificationIconDisplayData notificationIconDisplayData = m_IconDisplayData[prefabRef.m_Prefab];
					float num = (float)(int)icon.m_Priority * 0.003921569f;
					float2 val2 = math.lerp(notificationIconDisplayData.m_MinParams, notificationIconDisplayData.m_MaxParams, num);
					float num2 = IconClusterSystem.IconCluster.CalculateRadius(distance: math.distance(icon.m_Location, a), radius: val2.x);
					float3 val3 = m_CameraUp * num2;
					if (MathUtils.Distance(input.m_Line, icon.m_Location + val3, ref num3) < num2)
					{
						RaycastResult result = default(RaycastResult);
						result.m_Owner = nativeArray[j];
						result.m_Hit.m_HitEntity = result.m_Owner;
						result.m_Hit.m_Position = icon.m_Location;
						result.m_Hit.m_HitPosition = MathUtils.Position(input.m_Line, num3);
						result.m_Hit.m_NormalizedDistance = num3 - 100f / math.max(1f, MathUtils.Length(input.m_Line));
						if ((icon.m_Flags & IconFlags.OnTop) != 0)
						{
							result.m_Hit.m_NormalizedDistance *= 0.999f;
						}
						ValidateResult(raycastIndex, input, result, icon.m_ClusterLayer);
					}
				}
			}
		}

		private void ValidateResult(int raycastIndex, RaycastInput input, RaycastResult result, IconClusterLayer layer)
		{
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if ((input.m_IconLayerMask & NotificationsUtils.GetIconLayerMask(layer)) != IconLayerMask.None)
			{
				m_Results.Accumulate(raycastIndex, result);
			}
			while (m_OwnerData.HasComponent(result.m_Owner))
			{
				result.m_Owner = m_OwnerData[result.m_Owner].m_Owner;
				if (!m_ObjectData.HasComponent(result.m_Owner))
				{
					continue;
				}
				if (m_StaticData.HasComponent(result.m_Owner))
				{
					if ((input.m_TypeMask & TypeMask.StaticObjects) == 0)
					{
						break;
					}
					if (CheckPlaceholder(input, ref result.m_Owner))
					{
						result.m_Hit.m_Position = m_TransformData[result.m_Owner].m_Position;
						m_Results.Accumulate(raycastIndex, result);
						break;
					}
					continue;
				}
				if ((input.m_TypeMask & TypeMask.MovingObjects) != TypeMask.None)
				{
					result.m_Hit.m_Position = MathUtils.Center(m_CullingInfoData[result.m_Owner].m_Bounds);
					m_Results.Accumulate(raycastIndex, result);
				}
				break;
			}
		}

		private bool CheckPlaceholder(RaycastInput input, ref Entity entity)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if ((input.m_Flags & RaycastFlags.Placeholders) != 0)
			{
				return true;
			}
			if (m_PlaceholderData.HasComponent(entity))
			{
				if (m_AttachmentData.HasComponent(entity))
				{
					Attachment attachment = m_AttachmentData[entity];
					if (m_TransformData.HasComponent(attachment.m_Attached))
					{
						entity = attachment.m_Attached;
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}
}
