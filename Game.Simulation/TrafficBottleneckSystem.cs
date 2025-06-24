using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TrafficBottleneckSystem : GameSystemBase
{
	private struct GroupData
	{
		public int m_Count;

		public int m_Merged;
	}

	private struct BottleneckData
	{
		public BottleneckState m_State;

		public int2 m_Range;
	}

	private enum BottleneckState
	{
		Remove,
		Keep,
		Add
	}

	[BurstCompile]
	private struct TrafficBottleneckJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<Bottleneck> m_BottleneckType;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_BlockerChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_BottleneckChunks;

		[ReadOnly]
		public TrafficConfigurationData m_TrafficConfigurationData;

		public IconCommandBuffer m_IconCommandBuffer;

		public EntityCommandBuffer m_EntityCommandBuffer;

		public NativeQueue<TriggerAction> m_TriggerActionQueue;

		public void Execute()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, int> groupMap = default(NativeParallelHashMap<Entity, int>);
			groupMap._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<Entity, BottleneckData> bottleneckMap = default(NativeParallelHashMap<Entity, BottleneckData>);
			bottleneckMap._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<GroupData> groups = default(NativeList<GroupData>);
			groups._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_BottleneckChunks.Length; i++)
			{
				FillBottlenecks(m_BottleneckChunks[i], bottleneckMap);
			}
			for (int j = 0; j < m_BlockerChunks.Length; j++)
			{
				FormGroups(m_BlockerChunks[j], groupMap, groups);
			}
			for (int k = 0; k < m_BlockerChunks.Length; k++)
			{
				AddBottlenecks(m_BlockerChunks[k], groupMap, groups, bottleneckMap);
			}
			int num = 0;
			for (int l = 0; l < m_BottleneckChunks.Length; l++)
			{
				num += CheckBottlenecks(m_BottleneckChunks[l], bottleneckMap);
			}
			m_TriggerActionQueue.Enqueue(new TriggerAction(TriggerType.TrafficBottleneck, Entity.Null, num));
			groupMap.Dispose();
			bottleneckMap.Dispose();
			groups.Dispose();
		}

		private void FillBottlenecks(ArchetypeChunk chunk, NativeParallelHashMap<Entity, BottleneckData> bottleneckMap)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				bottleneckMap.Add(val, new BottleneckData
				{
					m_State = BottleneckState.Remove
				});
			}
		}

		private void FormGroups(ArchetypeChunk chunk, NativeParallelHashMap<Entity, int> groupMap, NativeList<GroupData> groups)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Blocker> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			Controller controller = default(Controller);
			int num2 = default(int);
			int num3 = default(int);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Blocker blocker = nativeArray2[i];
				if (blocker.m_Type != BlockerType.Continuing || !(blocker.m_Blocker != Entity.Null))
				{
					continue;
				}
				Entity val2 = blocker.m_Blocker;
				if (m_ControllerData.TryGetComponent(val2, ref controller))
				{
					val2 = controller.m_Controller;
				}
				if (val2 == val)
				{
					Debug.Log((object)$"TrafficBottleneckSystem: Self blocking entity {val.Index}");
					continue;
				}
				bool num = groupMap.TryGetValue(val, ref num2);
				bool flag = groupMap.TryGetValue(val2, ref num3);
				if (num)
				{
					GroupData groupData = groups[num2];
					if (groupData.m_Merged != -1)
					{
						do
						{
							num2 = groupData.m_Merged;
							groupData = groups[num2];
						}
						while (groupData.m_Merged != -1);
						groupMap[val] = num2;
					}
					if (flag)
					{
						GroupData groupData2 = groups[num3];
						while (groupData2.m_Merged != -1)
						{
							num3 = groupData2.m_Merged;
							groupData2 = groups[num3];
						}
						if (num2 != num3)
						{
							groupData.m_Count += groupData2.m_Count;
							groupData2.m_Count = 0;
							groupData2.m_Merged = num2;
							groups[num2] = groupData;
							groups[num3] = groupData2;
						}
						groupMap[val2] = num2;
					}
					else
					{
						groupData.m_Count++;
						groups[num2] = groupData;
						groupMap.Add(val2, num2);
					}
				}
				else if (flag)
				{
					GroupData groupData3 = groups[num3];
					if (groupData3.m_Merged != -1)
					{
						do
						{
							num3 = groupData3.m_Merged;
							groupData3 = groups[num3];
						}
						while (groupData3.m_Merged != -1);
						groupMap[val2] = num3;
					}
					groupData3.m_Count++;
					groups[num3] = groupData3;
					groupMap.Add(val, num3);
				}
				else
				{
					groupMap.Add(val, groups.Length);
					groupMap.Add(val2, groups.Length);
					GroupData groupData4 = new GroupData
					{
						m_Count = 2,
						m_Merged = -1
					};
					groups.Add(ref groupData4);
				}
			}
		}

		private void AddBottlenecks(ArchetypeChunk chunk, NativeParallelHashMap<Entity, int> groupMap, NativeList<GroupData> groups, NativeParallelHashMap<Entity, BottleneckData> bottleneckMap)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Blocker> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			int num = default(int);
			CarCurrentLane carCurrentLane = default(CarCurrentLane);
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Blocker blocker = nativeArray2[i];
				if (!groupMap.TryGetValue(val, ref num))
				{
					continue;
				}
				GroupData groupData = groups[num];
				while (groupData.m_Merged != -1)
				{
					num = groupData.m_Merged;
					groupData = groups[num];
				}
				if (groupData.m_Count >= 10)
				{
					Entity lane = Entity.Null;
					float2 curvePosition = float2.op_Implicit(0f);
					if (m_CarCurrentLaneData.TryGetComponent(val, ref carCurrentLane))
					{
						lane = ((!(carCurrentLane.m_ChangeLane != Entity.Null)) ? carCurrentLane.m_Lane : carCurrentLane.m_ChangeLane);
						curvePosition = ((float3)(ref carCurrentLane.m_CurvePosition)).xy;
					}
					else if (m_TrainCurrentLaneData.TryGetComponent(val, ref trainCurrentLane))
					{
						lane = trainCurrentLane.m_Front.m_Lane;
						curvePosition = ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yz;
					}
					if ((blocker.m_Type == BlockerType.Continuing && blocker.m_Blocker != Entity.Null) || (long)groupData.m_Count < 50L)
					{
						KeepBottleneck(bottleneckMap, lane, curvePosition);
					}
					else
					{
						AddBottleneck(bottleneckMap, lane, curvePosition);
					}
				}
			}
		}

		private void KeepBottleneck(NativeParallelHashMap<Entity, BottleneckData> bottleneckMap, Entity lane, float2 curvePosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			BottleneckData bottleneckData = default(BottleneckData);
			if (bottleneckMap.TryGetValue(lane, ref bottleneckData) && bottleneckData.m_State == BottleneckState.Remove)
			{
				bottleneckMap[lane] = new BottleneckData
				{
					m_State = BottleneckState.Keep
				};
			}
		}

		private void AddBottleneck(NativeParallelHashMap<Entity, BottleneckData> bottleneckMap, Entity lane, float2 curvePosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			BottleneckData bottleneckData = default(BottleneckData);
			if (bottleneckMap.TryGetValue(lane, ref bottleneckData))
			{
				curvePosition.y += curvePosition.y - curvePosition.x;
				int2 val = math.clamp(new int2(Mathf.RoundToInt(math.cmin(curvePosition) * 255f), Mathf.RoundToInt(math.cmax(curvePosition) * 255f)), int2.op_Implicit(0), int2.op_Implicit(255));
				if (bottleneckData.m_State == BottleneckState.Add)
				{
					bottleneckData.m_Range.x = math.min(bottleneckData.m_Range.x, val.x);
					bottleneckData.m_Range.y = math.max(bottleneckData.m_Range.y, val.y);
					bottleneckMap[lane] = bottleneckData;
				}
				else
				{
					bottleneckMap[lane] = new BottleneckData
					{
						m_State = BottleneckState.Add,
						m_Range = val
					};
				}
			}
			else if (m_CurveData.HasComponent(lane))
			{
				curvePosition.y += curvePosition.y - curvePosition.x;
				int2 val2 = math.clamp(new int2(Mathf.RoundToInt(math.cmin(curvePosition) * 255f), Mathf.RoundToInt(math.cmax(curvePosition) * 255f)), int2.op_Implicit(0), int2.op_Implicit(255));
				((EntityCommandBuffer)(ref m_EntityCommandBuffer)).AddComponent<Bottleneck>(lane, new Bottleneck((byte)val2.x, (byte)val2.y, 5));
				bottleneckMap.Add(lane, new BottleneckData
				{
					m_State = BottleneckState.Add,
					m_Range = val2
				});
			}
		}

		private int CheckBottlenecks(ArchetypeChunk chunk, NativeParallelHashMap<Entity, BottleneckData> bottleneckMap)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Bottleneck> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Bottleneck>(ref m_BottleneckType);
			int num = 0;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Bottleneck bottleneck = nativeArray2[i];
				if (bottleneck.m_Timer >= 20)
				{
					num++;
				}
				BottleneckData bottleneckData = bottleneckMap[val];
				switch (bottleneckData.m_State)
				{
				case BottleneckState.Remove:
					if (bottleneck.m_Timer >= 23)
					{
						bottleneck.m_Timer -= 3;
					}
					else if (bottleneck.m_Timer >= 20)
					{
						bottleneck.m_Timer = 0;
						m_IconCommandBuffer.Remove(val, m_TrafficConfigurationData.m_BottleneckNotification);
						((EntityCommandBuffer)(ref m_EntityCommandBuffer)).RemoveComponent<Bottleneck>(val);
					}
					else if (bottleneck.m_Timer > 3)
					{
						bottleneck.m_Timer -= 3;
					}
					else
					{
						((EntityCommandBuffer)(ref m_EntityCommandBuffer)).RemoveComponent<Bottleneck>(val);
					}
					break;
				case BottleneckState.Keep:
					if (bottleneck.m_Timer >= 21)
					{
						bottleneck.m_Timer--;
					}
					else if (bottleneck.m_Timer >= 20)
					{
						bottleneck.m_Timer = 0;
						m_IconCommandBuffer.Remove(val, m_TrafficConfigurationData.m_BottleneckNotification);
						((EntityCommandBuffer)(ref m_EntityCommandBuffer)).RemoveComponent<Bottleneck>(val);
					}
					else if (bottleneck.m_Timer > 1)
					{
						bottleneck.m_Timer--;
					}
					else
					{
						((EntityCommandBuffer)(ref m_EntityCommandBuffer)).RemoveComponent<Bottleneck>(val);
					}
					break;
				case BottleneckState.Add:
				{
					int position = bottleneck.m_Position;
					bottleneck.m_MinPos = (byte)math.min(bottleneck.m_MinPos + 2, bottleneckData.m_Range.x);
					bottleneck.m_MaxPos = (byte)math.max(bottleneck.m_MaxPos - 2, bottleneckData.m_Range.y);
					if (bottleneck.m_Position < bottleneck.m_MinPos || bottleneck.m_Position > bottleneck.m_MaxPos)
					{
						bottleneck.m_Position = (byte)(bottleneck.m_MinPos + bottleneck.m_MaxPos + 1 >> 1);
					}
					if (bottleneck.m_Timer >= 20)
					{
						bottleneck.m_Timer = (byte)math.min(40, bottleneck.m_Timer + 5);
						if (position != bottleneck.m_Position)
						{
							float3 location = MathUtils.Position(m_CurveData[val].m_Bezier, (float)(int)bottleneck.m_Position * 0.003921569f);
							m_IconCommandBuffer.Add(val, m_TrafficConfigurationData.m_BottleneckNotification, location, IconPriority.Problem);
						}
					}
					else if (bottleneck.m_Timer >= 15)
					{
						bottleneck.m_Timer = 40;
						float3 location2 = MathUtils.Position(m_CurveData[val].m_Bezier, (float)(int)bottleneck.m_Position * 0.003921569f);
						m_IconCommandBuffer.Add(val, m_TrafficConfigurationData.m_BottleneckNotification, location2, IconPriority.Problem);
					}
					else
					{
						bottleneck.m_Timer += 5;
					}
					break;
				}
				}
				nativeArray2[i] = bottleneck;
			}
			return num;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Bottleneck> __Game_Net_Bottleneck_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_Blocker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(true);
			__Game_Net_Bottleneck_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Bottleneck>(false);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
		}
	}

	private IconCommandSystem m_IconCommandSystem;

	private TriggerSystem m_TriggerSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_BlockerQuery;

	private EntityQuery m_BottleneckQuery;

	private EntityQuery m_ConfigurationQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_BlockerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Blocker>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_BottleneckQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Bottleneck>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficConfigurationData>() });
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_BlockerQuery, m_BottleneckQuery });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> blockerChunks = ((EntityQuery)(ref m_BlockerQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> bottleneckChunks = ((EntityQuery)(ref m_BottleneckQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle val3 = IJobExtensions.Schedule<TrafficBottleneckJob>(new TrafficBottleneckJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BottleneckType = InternalCompilerInterface.GetComponentTypeHandle<Bottleneck>(ref __TypeHandle.__Game_Net_Bottleneck_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerChunks = blockerChunks,
			m_BottleneckChunks = bottleneckChunks,
			m_TrafficConfigurationData = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingleton<TrafficConfigurationData>(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_EntityCommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_TriggerActionQueue = m_TriggerSystem.CreateActionBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
		blockerChunks.Dispose(val3);
		bottleneckChunks.Dispose(val3);
		m_IconCommandSystem.AddCommandBufferWriter(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public TrafficBottleneckSystem()
	{
	}
}
