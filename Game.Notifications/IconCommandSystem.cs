using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Notifications;

[CompilerGenerated]
public class IconCommandSystem : GameSystemBase
{
	[BurstCompile]
	private struct IconCommandPlaybackJob : IJob
	{
		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NotificationIconData> m_NotificationIconData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<IconAnimationElement> m_IconAnimations;

		public ComponentLookup<Icon> m_IconData;

		public BufferLookup<IconElement> m_IconElements;

		[ReadOnly]
		public Entity m_ConfigurationEntity;

		[ReadOnly]
		public float m_DeltaTime;

		[DeallocateOnJobCompletion]
		public NativeArray<IconCommandBuffer.Command> m_Commands;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			int length = m_Commands.Length;
			int num = 0;
			NativeSortExtension.Sort<IconCommandBuffer.Command>(m_Commands);
			while (num < length)
			{
				Entity owner = m_Commands[num].m_Owner;
				int num2 = num;
				while (++num2 < length && !(m_Commands[num2].m_Owner != owner))
				{
				}
				if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(owner))
				{
					ProcessCommands(owner, num, num2);
				}
				num = num2;
			}
		}

		private unsafe void ProcessCommands(Entity owner, int startIndex, int endIndex)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<IconElement> val = default(DynamicBuffer<IconElement>);
			DynamicBuffer<IconElement> val2 = default(DynamicBuffer<IconElement>);
			m_IconElements.TryGetBuffer(owner, ref val2);
			bool flag = m_DeletedData.HasComponent(owner);
			int2* ptr = (int2*)stackalloc int2[16];
			int num = 0;
			for (int i = startIndex; i < endIndex; i++)
			{
				IconCommandBuffer.Command command = m_Commands[i];
				if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.All) != 0)
				{
					int num2 = i + 1;
					while (num2 < endIndex)
					{
						IconCommandBuffer.Command command2 = m_Commands[num2];
						if ((command2.m_CommandFlags & IconCommandBuffer.CommandFlags.All) == 0 || command2.m_Priority != command.m_Priority)
						{
							num2++;
							continue;
						}
						goto IL_08e3;
					}
				}
				else
				{
					int num3 = i + 1;
					while (num3 < endIndex)
					{
						IconCommandBuffer.Command command3 = m_Commands[num3];
						if (!(command3.m_Prefab == command.m_Prefab) || ((command.m_Flags ^ command3.m_Flags) & IconFlags.SecondaryLocation) != 0 || (((command.m_Flags | command3.m_Flags) & IconFlags.IgnoreTarget) == 0 && !(command3.m_Target == command.m_Target)) || (flag && (command3.m_CommandFlags & IconCommandBuffer.CommandFlags.Remove) == 0))
						{
							num3++;
							continue;
						}
						goto IL_08e3;
					}
				}
				if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Add) != 0)
				{
					if (flag && command.m_ClusterLayer != IconClusterLayer.Transaction)
					{
						continue;
					}
					int num4 = 0;
					while (true)
					{
						if (num4 < num)
						{
							if ((int)command.m_Priority == ((int2)((byte*)ptr + (nint)num4 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<int2>())).x && command.m_BufferIndex != ((int2)((byte*)ptr + (nint)num4 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<int2>())).y)
							{
								break;
							}
							num4++;
							continue;
						}
						Icon iconData = GetIconData(command);
						if (command.m_ClusterLayer != IconClusterLayer.Transaction)
						{
							if (val.IsCreated)
							{
								int num5 = FindIcon(val, command);
								if (num5 >= 0)
								{
									Entity icon = val[num5].m_Icon;
									if (m_DeletedData.HasComponent(icon))
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(icon);
									}
									Icon other = m_IconData[icon];
									iconData.m_ClusterIndex = other.m_ClusterIndex;
									if (!iconData.Equals(other))
									{
										m_IconData[icon] = iconData;
										((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(icon, default(Updated));
									}
									if (command.m_Target != Entity.Null)
									{
										if (!m_TargetData.HasComponent(icon))
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Target>(icon, new Target(command.m_Target));
										}
										else if (m_TargetData[icon].m_Target != command.m_Target)
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Target>(icon, new Target(command.m_Target));
										}
									}
									else
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Target>(icon);
									}
									if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Temp) != 0)
									{
										Temp tempData = GetTempData(command);
										if (tempData.m_Flags != m_TempData[icon].m_Flags)
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(icon, tempData);
										}
									}
									if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Hidden) != 0)
									{
										if (!m_HiddenData.HasComponent(icon))
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(icon, default(Hidden));
										}
									}
									else if (m_HiddenData.HasComponent(icon))
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Hidden>(icon);
									}
									break;
								}
							}
							else if (val2.IsCreated)
							{
								int num6 = FindIcon(val2, command);
								if (num6 >= 0)
								{
									Entity icon2 = val2[num6].m_Icon;
									if (m_DeletedData.HasComponent(icon2))
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(icon2);
									}
									Icon other2 = m_IconData[icon2];
									iconData.m_ClusterIndex = other2.m_ClusterIndex;
									if (!iconData.Equals(other2))
									{
										m_IconData[icon2] = iconData;
										((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(icon2, default(Updated));
									}
									if (command.m_Target != Entity.Null)
									{
										if (!m_TargetData.HasComponent(icon2))
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Target>(icon2, new Target(command.m_Target));
										}
										else if (m_TargetData[icon2].m_Target != command.m_Target)
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Target>(icon2, new Target(command.m_Target));
										}
									}
									else
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Target>(icon2);
									}
									if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Temp) != 0)
									{
										Temp tempData2 = GetTempData(command);
										if (tempData2.m_Flags != m_TempData[icon2].m_Flags)
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(icon2, tempData2);
										}
									}
									if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Hidden) != 0)
									{
										if (!m_HiddenData.HasComponent(icon2))
										{
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(icon2, default(Hidden));
										}
									}
									else if (m_HiddenData.HasComponent(icon2))
									{
										((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Hidden>(icon2);
									}
									break;
								}
								val = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<IconElement>(owner);
								val.CopyFrom(val2);
							}
							else
							{
								val = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<IconElement>(owner);
							}
						}
						NotificationIconData notificationIconData = m_NotificationIconData[command.m_Prefab];
						Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(notificationIconData.m_Archetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val3, new PrefabRef(command.m_Prefab));
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Icon>(val3, iconData);
						if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Temp) != 0)
						{
							Temp tempData3 = GetTempData(command);
							if (tempData3.m_Original != Entity.Null || (command.m_CommandFlags & IconCommandBuffer.CommandFlags.DisallowCluster) != 0)
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<DisallowCluster>(val3, default(DisallowCluster));
							}
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val3, tempData3);
						}
						else
						{
							if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.DisallowCluster) != 0)
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<DisallowCluster>(val3, default(DisallowCluster));
							}
							DynamicBuffer<IconAnimationElement> val4 = m_IconAnimations[m_ConfigurationEntity];
							AnimationType appearAnimation = GetAppearAnimation(command.m_ClusterLayer);
							float duration = val4[(int)appearAnimation].m_Duration;
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val3, new Animation(appearAnimation, m_DeltaTime - command.m_Delay, duration));
						}
						if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Hidden) != 0)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(val3, default(Hidden));
						}
						if (command.m_Target != Entity.Null)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Target>(val3, new Target(command.m_Target));
						}
						if (command.m_ClusterLayer != IconClusterLayer.Transaction)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val3, new Owner(owner));
							val.Add(new IconElement(val3));
						}
						break;
					}
				}
				else if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Remove) != 0)
				{
					DynamicBuffer<IconElement> iconElements = (val.IsCreated ? val : val2);
					if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.All) != 0)
					{
						if (iconElements.IsCreated)
						{
							for (int j = 0; j < iconElements.Length; j++)
							{
								Entity icon3 = iconElements[j].m_Icon;
								if (icon3.Index < 0 || m_IconData[icon3].m_Priority != command.m_Priority)
								{
									DeleteIcon(icon3);
									iconElements.RemoveAt(j--);
								}
							}
						}
						if (num < 16)
						{
							System.Runtime.CompilerServices.Unsafe.Write((byte*)ptr + (nint)num++ * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<int2>(), new int2((int)command.m_Priority, command.m_BufferIndex));
						}
					}
					else if (iconElements.IsCreated)
					{
						int num7 = FindIcon(iconElements, command);
						if (num7 != -1)
						{
							DeleteIcon(iconElements[num7].m_Icon);
							iconElements.RemoveAt(num7);
						}
					}
				}
				else
				{
					if ((command.m_CommandFlags & IconCommandBuffer.CommandFlags.Update) == 0)
					{
						continue;
					}
					DynamicBuffer<IconElement> val5 = (val.IsCreated ? val : val2);
					if (!val5.IsCreated)
					{
						continue;
					}
					for (int k = 0; k < val5.Length; k++)
					{
						Entity icon4 = val5[k].m_Icon;
						if (icon4.Index < 0)
						{
							continue;
						}
						Icon icon5 = m_IconData[icon4];
						if ((icon5.m_Flags & IconFlags.CustomLocation) == 0)
						{
							float3 location = icon5.m_Location;
							if ((command.m_Flags & IconFlags.TargetLocation) != 0)
							{
								icon5.m_Location = FindLocation(command.m_Target);
							}
							else
							{
								icon5.m_Location = FindLocation(command.m_Owner);
							}
							if (!((float3)(ref location)).Equals(icon5.m_Location))
							{
								m_IconData[icon4] = icon5;
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(icon4, default(Updated));
							}
						}
					}
				}
				IL_08e3:;
			}
			if (val2.IsCreated && !val.IsCreated && val2.Length == 0)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<IconElement>(owner);
			}
		}

		private void DeleteIcon(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (entity.Index < 0 || m_TempData.HasComponent(entity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(entity, default(Deleted));
				return;
			}
			DynamicBuffer<IconAnimationElement> val = m_IconAnimations[m_ConfigurationEntity];
			AnimationType resolveAnimation = GetResolveAnimation(m_IconData[entity].m_ClusterLayer);
			float duration = val[(int)resolveAnimation].m_Duration;
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(entity, new Animation(resolveAnimation, m_DeltaTime, duration));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
		}

		private AnimationType GetAppearAnimation(IconClusterLayer layer)
		{
			return layer switch
			{
				IconClusterLayer.Default => AnimationType.WarningAppear, 
				IconClusterLayer.Marker => AnimationType.MarkerAppear, 
				IconClusterLayer.Transaction => AnimationType.Transaction, 
				_ => AnimationType.WarningAppear, 
			};
		}

		private AnimationType GetResolveAnimation(IconClusterLayer layer)
		{
			return layer switch
			{
				IconClusterLayer.Default => AnimationType.WarningResolve, 
				IconClusterLayer.Marker => AnimationType.MarkerDisappear, 
				_ => AnimationType.WarningResolve, 
			};
		}

		private Temp GetTempData(IconCommandBuffer.Command command)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			Temp result = default(Temp);
			Temp temp = default(Temp);
			if (m_TempData.TryGetComponent(command.m_Owner, ref temp))
			{
				result.m_Flags |= temp.m_Flags;
				DynamicBuffer<IconElement> iconElements = default(DynamicBuffer<IconElement>);
				if (m_IconElements.TryGetBuffer(temp.m_Original, ref iconElements))
				{
					int num = FindIcon(iconElements, command);
					if (num >= 0)
					{
						result.m_Original = iconElements[num].m_Icon;
					}
				}
			}
			return result;
		}

		private Icon GetIconData(IconCommandBuffer.Command command)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Icon result = default(Icon);
			if ((command.m_Flags & IconFlags.CustomLocation) != 0)
			{
				result.m_Location = command.m_Location;
			}
			else if ((command.m_Flags & IconFlags.TargetLocation) != 0)
			{
				result.m_Location = FindLocation(command.m_Target);
			}
			else
			{
				result.m_Location = FindLocation(command.m_Owner);
			}
			result.m_Priority = command.m_Priority;
			result.m_ClusterLayer = command.m_ClusterLayer;
			result.m_Flags = command.m_Flags;
			return result;
		}

		private float3 FindLocation(Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			float3 result = default(float3);
			if (m_ConnectedData.HasComponent(entity))
			{
				Entity connected = m_ConnectedData[entity].m_Connected;
				if (m_TransformData.HasComponent(connected))
				{
					entity = connected;
				}
			}
			else if (m_CurrentBuildingData.HasComponent(entity))
			{
				entity = m_CurrentBuildingData[entity].m_CurrentBuilding;
				if (m_OwnerData.HasComponent(entity))
				{
					entity = m_OwnerData[entity].m_Owner;
				}
			}
			else if (m_CurrentTransportData.HasComponent(entity))
			{
				entity = m_CurrentTransportData[entity].m_CurrentTransport;
			}
			if (m_CurrentVehicleData.HasComponent(entity))
			{
				entity = m_CurrentVehicleData[entity].m_Vehicle;
			}
			Transform transform = default(Transform);
			if (m_TransformData.TryGetComponent(entity, ref transform))
			{
				result = transform.m_Position;
				PrefabRef prefabRef = default(PrefabRef);
				ObjectGeometryData geometryData = default(ObjectGeometryData);
				if (m_PrefabRefData.TryGetComponent(entity, ref prefabRef) && m_ObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref geometryData))
				{
					if ((geometryData.m_Flags & Game.Objects.GeometryFlags.Marker) == 0)
					{
						Bounds3 val = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
						result.y = val.max.y;
					}
					Destroyed destroyed = default(Destroyed);
					if ((geometryData.m_Flags & (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot)) == (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot) && m_DestroyedData.TryGetComponent(entity, ref destroyed) && destroyed.m_Cleared >= 0f)
					{
						result.y = transform.m_Position.y + 5f;
					}
				}
			}
			else if (m_NodeData.HasComponent(entity))
			{
				result = m_NodeData[entity].m_Position;
				PrefabRef prefabRef2 = m_PrefabRefData[entity];
				if (m_NetGeometryData.HasComponent(prefabRef2.m_Prefab))
				{
					result.y += m_NetGeometryData[prefabRef2.m_Prefab].m_DefaultSurfaceHeight.max;
				}
			}
			else if (m_CurveData.HasComponent(entity))
			{
				result = MathUtils.Position(m_CurveData[entity].m_Bezier, 0.5f);
			}
			else if (m_PositionData.HasComponent(entity))
			{
				result = m_PositionData[entity].m_Position;
			}
			else if (m_RouteWaypoints.HasBuffer(entity))
			{
				DynamicBuffer<RouteWaypoint> val2 = m_RouteWaypoints[entity];
				if (val2.Length != 0)
				{
					result = m_PositionData[val2[0].m_Waypoint].m_Position;
				}
			}
			return result;
		}

		private int FindIcon(DynamicBuffer<IconElement> iconElements, IconCommandBuffer.Command command)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < iconElements.Length; i++)
			{
				Entity icon = iconElements[i].m_Icon;
				if (icon.Index < 0 || m_PrefabRefData[icon].m_Prefab != command.m_Prefab)
				{
					continue;
				}
				Icon icon2 = m_IconData[icon];
				if ((command.m_Flags & IconFlags.IgnoreTarget) == 0 && (icon2.m_Flags & IconFlags.IgnoreTarget) == 0)
				{
					if (m_TargetData.HasComponent(icon))
					{
						if (m_TargetData[icon].m_Target != command.m_Target)
						{
							continue;
						}
					}
					else if (command.m_Target != Entity.Null)
					{
						continue;
					}
				}
				if (((command.m_Flags ^ icon2.m_Flags) & IconFlags.SecondaryLocation) == 0)
				{
					return i;
				}
			}
			return -1;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NotificationIconData> __Game_Prefabs_NotificationIconData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<IconAnimationElement> __Game_Prefabs_IconAnimationElement_RO_BufferLookup;

		public ComponentLookup<Icon> __Game_Notifications_Icon_RW_ComponentLookup;

		public BufferLookup<IconElement> __Game_Notifications_IconElement_RW_BufferLookup;

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
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NotificationIconData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NotificationIconData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Prefabs_IconAnimationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconAnimationElement>(true);
			__Game_Notifications_Icon_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(false);
			__Game_Notifications_IconElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconElement>(false);
		}
	}

	private ModificationEndBarrier m_ModificationBarrier;

	private EntityQuery m_ConfigurationQuery;

	private List<NativeQueue<IconCommandBuffer.Command>> m_Queues;

	private JobHandle m_Dependencies;

	private int m_BufferIndex;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_Queues = new List<NativeQueue<IconCommandBuffer.Command>>();
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconConfigurationData>() });
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Dependencies)).Complete();
		for (int i = 0; i < m_Queues.Count; i++)
		{
			m_Queues[i].Dispose();
		}
		m_Queues.Clear();
		((COSystemBase)this).OnStopRunning();
	}

	public IconCommandBuffer CreateCommandBuffer()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<IconCommandBuffer.Command> item = default(NativeQueue<IconCommandBuffer.Command>);
		item._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		m_Queues.Add(item);
		return new IconCommandBuffer(item.AsParallelWriter(), m_BufferIndex++);
	}

	public void AddCommandBufferWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Dependencies = JobHandle.CombineDependencies(m_Dependencies, handle);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Dependencies)).Complete();
		m_BufferIndex = 0;
		int num = 0;
		for (int i = 0; i < m_Queues.Count; i++)
		{
			num += m_Queues[i].Count;
		}
		if (num == 0 || ((EntityQuery)(ref m_ConfigurationQuery)).IsEmptyIgnoreFilter)
		{
			for (int j = 0; j < m_Queues.Count; j++)
			{
				m_Queues[j].Dispose();
			}
			m_Queues.Clear();
			return;
		}
		NativeArray<IconCommandBuffer.Command> commands = default(NativeArray<IconCommandBuffer.Command>);
		commands._002Ector(num, (Allocator)3, (NativeArrayOptions)0);
		num = 0;
		for (int k = 0; k < m_Queues.Count; k++)
		{
			NativeQueue<IconCommandBuffer.Command> val = m_Queues[k];
			int count = val.Count;
			for (int l = 0; l < count; l++)
			{
				commands[num++] = val.Dequeue();
			}
			val.Dispose();
		}
		m_Queues.Clear();
		JobHandle val2 = IJobExtensions.Schedule<IconCommandPlaybackJob>(new IconCommandPlaybackJob
		{
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NotificationIconData = InternalCompilerInterface.GetComponentLookup<NotificationIconData>(ref __TypeHandle.__Game_Prefabs_NotificationIconData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconAnimations = InternalCompilerInterface.GetBufferLookup<IconAnimationElement>(ref __TypeHandle.__Game_Prefabs_IconAnimationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconElements = InternalCompilerInterface.GetBufferLookup<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConfigurationEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity(),
			m_DeltaTime = Time.deltaTime,
			m_Commands = commands,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public IconCommandSystem()
	{
	}
}
