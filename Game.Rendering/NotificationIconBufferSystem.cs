using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class NotificationIconBufferSystem : GameSystemBase
{
	public struct IconData
	{
		public NativeArray<InstanceData> m_InstanceData;

		public NativeValue<Bounds3> m_IconBounds;
	}

	public struct InstanceData : IComparable<InstanceData>
	{
		public float3 m_Position;

		public float4 m_Params;

		public float m_Icon;

		public float m_Distance;

		public int CompareTo(InstanceData other)
		{
			return (int)math.sign(other.m_Distance - m_Distance);
		}
	}

	private struct HiddenPositionData
	{
		public float3 m_Position;

		public float m_Radius;

		public float m_Distance;
	}

	[BurstCompile]
	private struct NotificationIconBufferJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Icon> m_IconType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Notifications.Animation> m_AnimationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> m_IconDisplayData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<Game.Notifications.Animation> m_AnimationData;

		[ReadOnly]
		public ComponentLookup<DisallowCluster> m_DisallowClusterData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public BufferLookup<IconAnimationElement> m_IconAnimations;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraUp;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public Entity m_ConfigurationEntity;

		[ReadOnly]
		public uint m_CategoryMask;

		public IconClusterSystem.ClusterData m_ClusterData;

		public NativeList<InstanceData> m_InstanceData;

		public NativeValue<Bounds3> m_IconBounds;

		public void Execute()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0983: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_0905: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0933: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_094b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0950: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			m_InstanceData.Clear();
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			DynamicBuffer<IconAnimationElement> iconAnimations = m_IconAnimations[m_ConfigurationEntity];
			NativeParallelHashMap<Entity, HiddenPositionData> val2 = default(NativeParallelHashMap<Entity, HiddenPositionData>);
			if (!m_ClusterData.isEmpty)
			{
				NativeList<IconClusterSystem.IconCluster> val3 = default(NativeList<IconClusterSystem.IconCluster>);
				val3._002Ector(64, AllocatorHandle.op_Implicit((Allocator)2));
				int index = 0;
				IconClusterSystem.IconCluster cluster;
				while (m_ClusterData.GetRoot(ref index, out cluster))
				{
					float distance = math.distance(m_CameraPosition, cluster.center);
					val |= cluster.GetBounds(distance, m_CameraUp);
					val3.Add(ref cluster);
				}
				float4 val7 = default(float4);
				while (val3.Length != 0)
				{
					IconClusterSystem.IconCluster iconCluster = val3[val3.Length - 1];
					val3.RemoveAtSwapBack(val3.Length - 1);
					float num = math.distance(m_CameraPosition, iconCluster.center);
					int2 subClusters;
					if (iconCluster.KeepCluster(num))
					{
						float radius = iconCluster.GetRadius(num);
						NativeArray<IconClusterSystem.ClusterIcon> icons = iconCluster.GetIcons(m_ClusterData);
						bool flag;
						do
						{
							flag = false;
							IconClusterSystem.ClusterIcon clusterIcon = icons[0];
							for (int i = 1; i < icons.Length; i++)
							{
								IconClusterSystem.ClusterIcon clusterIcon2 = icons[i];
								if (clusterIcon.priority == clusterIcon2.priority)
								{
									float num2 = math.dot(((float3)(ref m_CameraRight)).xz, clusterIcon.order);
									float num3 = math.dot(((float3)(ref m_CameraRight)).xz, clusterIcon2.order);
									if (num2 > num3)
									{
										icons[i] = clusterIcon;
										icons[i - 1] = clusterIcon2;
										flag = true;
										continue;
									}
								}
								clusterIcon = clusterIcon2;
							}
						}
						while (flag);
						float3 val4 = m_CameraRight * (radius * 0.5f);
						float3 val5 = iconCluster.center;
						if (iconCluster.isMoving)
						{
							IconClusterSystem.ClusterIcon clusterIcon3 = icons[0];
							if (m_OwnerData.HasComponent(clusterIcon3.icon))
							{
								Owner owner = m_OwnerData[clusterIcon3.icon];
								if (m_InterpolatedTransformData.HasComponent(owner.m_Owner))
								{
									PrefabRef prefabRef = m_PrefabRefData[owner.m_Owner];
									Transform transform = m_InterpolatedTransformData[owner.m_Owner].ToTransform();
									Bounds3 val6 = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, m_ObjectGeometryData[prefabRef.m_Prefab]);
									((float3)(ref val5)).xz = ((float3)(ref transform.m_Position)).xz;
									val5.y = val6.max.y;
								}
							}
						}
						val5 += val4 * ((float)(icons.Length - 1) * 0.5f);
						for (int num4 = icons.Length - 1; num4 >= 0; num4--)
						{
							IconClusterSystem.ClusterIcon clusterIcon4 = icons[num4];
							float num5 = 1E-06f * (float)(num4 - (int)clusterIcon4.priority);
							if (m_HiddenData.HasComponent(clusterIcon4.icon))
							{
								if (!val2.IsCreated)
								{
									val2._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
								}
								val2.TryAdd(clusterIcon4.icon, new HiddenPositionData
								{
									m_Position = val5,
									m_Radius = radius,
									m_Distance = num * (1f + num5)
								});
							}
							else
							{
								NotificationIconDisplayData notificationIconDisplayData = m_IconDisplayData[clusterIcon4.prefab];
								float num6 = (float)(int)clusterIcon4.priority * 0.003921569f;
								((float4)(ref val7))._002Ector(math.lerp(notificationIconDisplayData.m_MinParams, notificationIconDisplayData.m_MaxParams, num6), math.select(float2.op_Implicit(1f), new float2(0.5f, 0f), (notificationIconDisplayData.m_CategoryMask & m_CategoryMask) == 0 && !iconCluster.isTemp));
								ref NativeList<InstanceData> reference = ref m_InstanceData;
								InstanceData instanceData = new InstanceData
								{
									m_Position = val5,
									m_Params = val7,
									m_Icon = notificationIconDisplayData.m_IconIndex,
									m_Distance = num * (1f + num5)
								};
								reference.Add(ref instanceData);
							}
							val5 -= val4;
						}
					}
					else if (iconCluster.GetSubClusters(out subClusters))
					{
						IconClusterSystem.IconCluster cluster2 = m_ClusterData.GetCluster(subClusters.x);
						val3.Add(ref cluster2);
						cluster2 = m_ClusterData.GetCluster(subClusters.y);
						val3.Add(ref cluster2);
					}
				}
			}
			HiddenPositionData hiddenPositionData = default(HiddenPositionData);
			float4 iconParams = default(float4);
			float4 iconParams2 = default(float4);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val8 = m_Chunks[j];
				if (((ArchetypeChunk)(ref val8)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				NativeArray<Icon> nativeArray = ((ArchetypeChunk)(ref val8)).GetNativeArray<Icon>(ref m_IconType);
				NativeArray<Game.Notifications.Animation> nativeArray2 = ((ArchetypeChunk)(ref val8)).GetNativeArray<Game.Notifications.Animation>(ref m_AnimationType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val8)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref val8)).GetNativeArray<Temp>(ref m_TempType);
				if (nativeArray4.Length != 0)
				{
					for (int k = 0; k < nativeArray.Length; k++)
					{
						Icon icon = nativeArray[k];
						PrefabRef prefabRef2 = nativeArray3[k];
						Temp temp = nativeArray4[k];
						if (!m_IconDisplayData.IsComponentEnabled(prefabRef2.m_Prefab))
						{
							continue;
						}
						NotificationIconDisplayData notificationIconDisplayData2 = m_IconDisplayData[prefabRef2.m_Prefab];
						float num7 = math.distance(icon.m_Location, m_CameraPosition);
						if (temp.m_Original != Entity.Null)
						{
							if (val2.IsCreated && val2.TryGetValue(temp.m_Original, ref hiddenPositionData))
							{
								Icon icon2 = m_IconData[temp.m_Original];
								if (math.distance(icon.m_Location, icon2.m_Location) < hiddenPositionData.m_Radius * 0.1f)
								{
									icon.m_Location = hiddenPositionData.m_Position;
									num7 = hiddenPositionData.m_Distance;
								}
							}
							else
							{
								if (!m_DisallowClusterData.HasComponent(temp.m_Original) || m_DeletedData.HasComponent(temp.m_Original))
								{
									continue;
								}
								icon.m_Location = m_IconData[temp.m_Original].m_Location;
								num7 = math.distance(icon.m_Location, m_CameraPosition);
							}
						}
						float num8 = (float)(int)icon.m_Priority * 0.003921569f;
						((float4)(ref iconParams))._002Ector(math.lerp(notificationIconDisplayData2.m_MinParams, notificationIconDisplayData2.m_MaxParams, num8), math.select(float2.op_Implicit(1f), new float2(0.5f, 0f), (notificationIconDisplayData2.m_CategoryMask & m_CategoryMask) == 0));
						if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Select)) != 0)
						{
							iconParams.x *= 1.1f;
							iconParams.y = 0f;
						}
						float num9 = IconClusterSystem.IconCluster.CalculateRadius(iconParams.x, num7);
						if (temp.m_Original != Entity.Null && m_AnimationData.HasComponent(temp.m_Original))
						{
							Animate(ref icon.m_Location, ref iconParams, num9, m_AnimationData[temp.m_Original], iconAnimations);
						}
						if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Select)) != 0)
						{
							num7 *= 0.99f;
						}
						else if ((icon.m_Flags & IconFlags.OnTop) != 0)
						{
							num7 *= 0.995f;
						}
						ref NativeList<InstanceData> reference2 = ref m_InstanceData;
						InstanceData instanceData = new InstanceData
						{
							m_Position = icon.m_Location,
							m_Params = iconParams,
							m_Icon = notificationIconDisplayData2.m_IconIndex,
							m_Distance = num7
						};
						reference2.Add(ref instanceData);
						val |= new Bounds3(icon.m_Location - num9, icon.m_Location + num9);
					}
					continue;
				}
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Icon icon3 = nativeArray[l];
					PrefabRef prefabRef3 = nativeArray3[l];
					if (!m_IconDisplayData.IsComponentEnabled(prefabRef3.m_Prefab))
					{
						continue;
					}
					NotificationIconDisplayData notificationIconDisplayData3 = m_IconDisplayData[prefabRef3.m_Prefab];
					float num10 = (float)(int)icon3.m_Priority * 0.003921569f;
					((float4)(ref iconParams2))._002Ector(math.lerp(notificationIconDisplayData3.m_MinParams, notificationIconDisplayData3.m_MaxParams, num10), math.select(float2.op_Implicit(1f), new float2(0.5f, 0f), (notificationIconDisplayData3.m_CategoryMask & m_CategoryMask) == 0));
					float num11 = math.distance(icon3.m_Location, m_CameraPosition);
					float num12 = IconClusterSystem.IconCluster.CalculateRadius(iconParams2.x, num11);
					if (nativeArray2.Length != 0)
					{
						Game.Notifications.Animation animation = nativeArray2[l];
						if (animation.m_Timer <= 0f)
						{
							continue;
						}
						Animate(ref icon3.m_Location, ref iconParams2, num12, animation, iconAnimations);
					}
					if ((icon3.m_Flags & IconFlags.OnTop) != 0)
					{
						num11 *= 0.995f;
					}
					ref NativeList<InstanceData> reference3 = ref m_InstanceData;
					InstanceData instanceData = new InstanceData
					{
						m_Position = icon3.m_Location,
						m_Params = iconParams2,
						m_Icon = notificationIconDisplayData3.m_IconIndex,
						m_Distance = num11
					};
					reference3.Add(ref instanceData);
					val |= new Bounds3(icon3.m_Location - num12, icon3.m_Location + num12);
				}
			}
			m_IconBounds.value = val;
		}

		private void Animate(ref float3 location, ref float4 iconParams, float radius, Game.Notifications.Animation animation, DynamicBuffer<IconAnimationElement> iconAnimations)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			IconAnimationElement iconAnimationElement = iconAnimations[(int)animation.m_Type];
			float3 val = ((AnimationCurve3)(ref iconAnimationElement.m_AnimationCurve)).Evaluate(animation.m_Timer / animation.m_Duration);
			((float4)(ref iconParams)).xz = ((float4)(ref iconParams)).xz * ((float3)(ref val)).xy;
			location += m_CameraUp * (radius * val.z);
		}
	}

	[BurstCompile]
	private struct NotificationIconSortJob : IJob
	{
		public NativeList<InstanceData> m_InstanceData;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (m_InstanceData.Length >= 2)
			{
				NativeSortExtension.Sort<InstanceData>(m_InstanceData);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Icon> __Game_Notifications_Icon_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Notifications.Animation> __Game_Notifications_Animation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> __Game_Tools_Hidden_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> __Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Notifications.Animation> __Game_Notifications_Animation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DisallowCluster> __Game_Notifications_DisallowCluster_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<IconAnimationElement> __Game_Prefabs_IconAnimationElement_RO_BufferLookup;

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
			__Game_Notifications_Icon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Icon>(true);
			__Game_Notifications_Animation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Notifications.Animation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NotificationIconDisplayData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Notifications_Animation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Notifications.Animation>(true);
			__Game_Notifications_DisallowCluster_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DisallowCluster>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Prefabs_IconAnimationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconAnimationElement>(true);
		}
	}

	private EntityQuery m_IconQuery;

	private EntityQuery m_ConfigurationQuery;

	private IconClusterSystem m_IconClusterSystem;

	private ToolSystem m_ToolSystem;

	private PrefabSystem m_PrefabSystem;

	private NativeList<InstanceData> m_InstanceData;

	private NativeValue<Bounds3> m_IconBounds;

	private JobHandle m_InstanceDataDeps;

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
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconClusterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconClusterSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Icon>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<DisallowCluster>(),
			ComponentType.ReadOnly<Game.Notifications.Animation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Hidden>()
		};
		array[0] = val;
		m_IconQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconConfigurationData>() });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_InstanceData.IsCreated)
		{
			((JobHandle)(ref m_InstanceDataDeps)).Complete();
			m_InstanceData.Dispose();
			m_IconBounds.Dispose();
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		if (!((Object)(object)main == (Object)null) && !((EntityQuery)(ref m_ConfigurationQuery)).IsEmptyIgnoreFilter)
		{
			if (!m_InstanceData.IsCreated)
			{
				m_InstanceData = new NativeList<InstanceData>(64, AllocatorHandle.op_Implicit((Allocator)4));
				m_IconBounds = new NativeValue<Bounds3>((Allocator)4);
			}
			Transform transform = ((Component)main).transform;
			uint categoryMask = uint.MaxValue;
			if ((Object)(object)m_ToolSystem.activeInfoview != (Object)null)
			{
				categoryMask = m_PrefabSystem.GetComponentData<InfoviewData>((PrefabBase)m_ToolSystem.activeInfoview).m_NotificationMask;
				categoryMask |= 0x80000000u;
			}
			((JobHandle)(ref m_InstanceDataDeps)).Complete();
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_IconQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			JobHandle dependencies;
			NotificationIconBufferJob notificationIconBufferJob = new NotificationIconBufferJob
			{
				m_IconType = InternalCompilerInterface.GetComponentTypeHandle<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Notifications.Animation>(ref __TypeHandle.__Game_Notifications_Animation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconDisplayData = InternalCompilerInterface.GetComponentLookup<NotificationIconDisplayData>(ref __TypeHandle.__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationData = InternalCompilerInterface.GetComponentLookup<Game.Notifications.Animation>(ref __TypeHandle.__Game_Notifications_Animation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DisallowClusterData = InternalCompilerInterface.GetComponentLookup<DisallowCluster>(ref __TypeHandle.__Game_Notifications_DisallowCluster_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconAnimations = InternalCompilerInterface.GetBufferLookup<IconAnimationElement>(ref __TypeHandle.__Game_Prefabs_IconAnimationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Chunks = chunks,
				m_CameraPosition = float3.op_Implicit(transform.position),
				m_CameraUp = float3.op_Implicit(transform.up),
				m_CameraRight = float3.op_Implicit(transform.right),
				m_ConfigurationEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity(),
				m_CategoryMask = categoryMask,
				m_ClusterData = m_IconClusterSystem.GetIconClusterData(readOnly: false, out dependencies),
				m_InstanceData = m_InstanceData,
				m_IconBounds = m_IconBounds
			};
			NotificationIconSortJob obj = new NotificationIconSortJob
			{
				m_InstanceData = m_InstanceData
			};
			JobHandle val2 = IJobExtensions.Schedule<NotificationIconBufferJob>(notificationIconBufferJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
			JobHandle instanceDataDeps = IJobExtensions.Schedule<NotificationIconSortJob>(obj, val2);
			chunks.Dispose(val2);
			m_IconClusterSystem.AddIconClusterWriter(val2);
			m_InstanceDataDeps = instanceDataDeps;
			((SystemBase)this).Dependency = val2;
		}
	}

	public IconData GetIconData()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_InstanceDataDeps)).Complete();
		m_InstanceDataDeps = default(JobHandle);
		if (m_InstanceData.IsCreated)
		{
			return new IconData
			{
				m_InstanceData = m_InstanceData.AsArray(),
				m_IconBounds = m_IconBounds
			};
		}
		return default(IconData);
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
	public NotificationIconBufferSystem()
	{
	}
}
