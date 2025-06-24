using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Audio.Radio;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.PSI;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.Tutorials;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Triggers;

[CompilerGenerated]
public class TriggerSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct TriggerActionJob : IJob
	{
		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public TriggerPrefabData m_TriggerPrefabData;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<TriggerAction> m_Actions;

		public NativeQueue<ChirpCreationData> m_ChirpQueue;

		public NativeQueue<LifePathEventCreationData> m_LifePathEventQueue;

		public NativeQueue<RadioTag> m_RadioTagQueue;

		public NativeQueue<RadioTag> m_EmergencyRadioTagQueue;

		public NativeQueue<Entity> m_TutorialTriggerQueue;

		public NativeParallelHashMap<Entity, uint> m_TriggerFrames;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjectData;

		[ReadOnly]
		public BufferLookup<TriggerChirpData> m_ChirpData;

		[ReadOnly]
		public ComponentLookup<LifePathEventData> m_LifePathEventData;

		[ReadOnly]
		public ComponentLookup<RadioEventData> m_RadioEventData;

		[ReadOnly]
		public BufferLookup<TutorialActivationEventData> m_TutorialEventData;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> m_TrafficAccidentData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PolicyData> m_PolicyData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfo;

		[ReadOnly]
		public BufferLookup<TriggerConditionData> m_TriggerConditions;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public ComponentLookup<TriggerLimitData> m_TriggerLimitData;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(0);
			PrefabRef prefabRef = default(PrefabRef);
			for (int i = 0; i < m_Actions.Length; i++)
			{
				TriggerAction triggerAction = m_Actions[i];
				if (!m_TriggerPrefabData.HasAnyPrefabs(triggerAction.m_TriggerType, triggerAction.m_TriggerPrefab))
				{
					continue;
				}
				TargetType targetType = TargetType.Nothing;
				if (m_BuildingData.HasComponent(triggerAction.m_PrimaryTarget))
				{
					targetType = TargetType.Building;
					if (m_PrefabRefData.TryGetComponent(triggerAction.m_PrimaryTarget, ref prefabRef) && m_ServiceObjectData.HasComponent(prefabRef.m_Prefab))
					{
						targetType |= TargetType.ServiceBuilding;
					}
				}
				if (m_CitizenData.HasComponent(triggerAction.m_PrimaryTarget))
				{
					targetType = TargetType.Citizen;
				}
				if (m_PolicyData.HasComponent(triggerAction.m_TriggerPrefab))
				{
					targetType = TargetType.Policy;
				}
				if (m_RoadData.HasComponent(triggerAction.m_PrimaryTarget))
				{
					targetType = TargetType.Road;
					if (m_TrafficAccidentData.HasComponent(triggerAction.m_TriggerPrefab) && triggerAction.m_PrimaryTarget != Entity.Null && m_CullingInfo.HasComponent(triggerAction.m_PrimaryTarget))
					{
						triggerAction.m_Value = math.distance(MathUtils.Center(m_CullingInfo[triggerAction.m_PrimaryTarget].m_Bounds), m_CameraPosition);
					}
				}
				if (!m_TriggerPrefabData.TryGetFirstPrefab(triggerAction.m_TriggerType, targetType, triggerAction.m_TriggerPrefab, out var prefab, out var iterator))
				{
					continue;
				}
				do
				{
					if (CheckInterval(prefab) && CheckConditions(prefab, triggerAction))
					{
						CreateEntity(ref random, prefab, triggerAction);
					}
				}
				while (m_TriggerPrefabData.TryGetNextPrefab(triggerAction.m_TriggerType, targetType, triggerAction.m_TriggerPrefab, out prefab, ref iterator));
			}
		}

		private bool CheckInterval(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			TriggerLimitData triggerLimitData = default(TriggerLimitData);
			uint num = default(uint);
			if (m_TriggerLimitData.TryGetComponent(prefab, ref triggerLimitData) && m_TriggerFrames.TryGetValue(prefab, ref num))
			{
				return m_SimulationFrame >= num + triggerLimitData.m_FrameInterval;
			}
			return true;
		}

		private bool CheckConditions(Entity prefab, TriggerAction action)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_TriggerConditions.HasBuffer(prefab))
			{
				DynamicBuffer<TriggerConditionData> val = m_TriggerConditions[prefab];
				for (int i = 0; i < val.Length; i++)
				{
					TriggerConditionData triggerConditionData = val[i];
					switch (triggerConditionData.m_Type)
					{
					case TriggerConditionType.Equals:
						if (Math.Abs(triggerConditionData.m_Value - action.m_Value) > float.Epsilon)
						{
							return false;
						}
						break;
					case TriggerConditionType.GreaterThan:
						if (action.m_Value <= triggerConditionData.m_Value)
						{
							return false;
						}
						break;
					case TriggerConditionType.LessThan:
						if (action.m_Value >= triggerConditionData.m_Value)
						{
							return false;
						}
						break;
					}
				}
			}
			return true;
		}

		private void CreateEntity(ref Random random, Entity prefab, TriggerAction triggerAction)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			if (m_TriggerLimitData.HasComponent(prefab))
			{
				m_TriggerFrames[prefab] = m_SimulationFrame;
			}
			if (m_ChirpData.HasBuffer(prefab))
			{
				m_ChirpQueue.Enqueue(new ChirpCreationData
				{
					m_TriggerPrefab = prefab,
					m_Sender = triggerAction.m_PrimaryTarget,
					m_Target = triggerAction.m_SecondaryTarget
				});
			}
			else if (m_LifePathEventData.HasComponent(prefab))
			{
				m_LifePathEventQueue.Enqueue(new LifePathEventCreationData
				{
					m_EventPrefab = prefab,
					m_Sender = triggerAction.m_PrimaryTarget,
					m_Target = triggerAction.m_SecondaryTarget,
					m_OriginalSender = Entity.Null
				});
			}
			else if (m_RadioEventData.HasComponent(prefab))
			{
				RadioEventData radioEventData = m_RadioEventData[prefab];
				if (radioEventData.m_SegmentType == Radio.SegmentType.Emergency)
				{
					m_EmergencyRadioTagQueue.Enqueue(new RadioTag
					{
						m_Event = prefab,
						m_Target = triggerAction.m_PrimaryTarget,
						m_SegmentType = radioEventData.m_SegmentType,
						m_EmergencyFrameDelay = radioEventData.m_EmergencyFrameDelay
					});
				}
				else
				{
					m_RadioTagQueue.Enqueue(new RadioTag
					{
						m_Event = prefab,
						m_Target = triggerAction.m_PrimaryTarget,
						m_SegmentType = radioEventData.m_SegmentType
					});
				}
			}
			else if (m_TutorialEventData.HasBuffer(prefab))
			{
				DynamicBuffer<TutorialActivationEventData> val = m_TutorialEventData[prefab];
				for (int i = 0; i < val.Length; i++)
				{
					m_TutorialTriggerQueue.Enqueue(val[i].m_Tutorial);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TriggerChirpData> __Game_Prefabs_TriggerChirpData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LifePathEventData> __Game_Prefabs_LifePathEventData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RadioEventData> __Game_Prefabs_RadioEventData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TutorialActivationEventData> __Game_Prefabs_TutorialActivationEventData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PolicyData> __Game_Prefabs_PolicyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TriggerConditionData> __Game_Prefabs_TriggerConditionData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> __Game_Prefabs_TrafficAccidentData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TriggerLimitData> __Game_Prefabs_TriggerLimitData_RO_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Prefabs_TriggerChirpData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TriggerChirpData>(true);
			__Game_Prefabs_LifePathEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LifePathEventData>(true);
			__Game_Prefabs_RadioEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RadioEventData>(true);
			__Game_Prefabs_TutorialActivationEventData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TutorialActivationEventData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PolicyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PolicyData>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Prefabs_TriggerConditionData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TriggerConditionData>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficAccidentData>(true);
			__Game_Prefabs_TriggerLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TriggerLimitData>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private TriggerPrefabSystem m_TriggerPrefabSystem;

	private ModificationEndBarrier m_ModificationBarrier;

	private List<NativeQueue<TriggerAction>> m_Queues;

	private JobHandle m_Dependencies;

	private CreateChirpSystem m_CreateChirpSystem;

	private LifePathEventSystem m_LifePathEventSystem;

	private RadioTagSystem m_RadioTagSystem;

	private TutorialEventActivationSystem m_TutorialEventActivationSystem;

	private DateTime m_LastTimedEventTime;

	private TimeSpan m_TimedEventInterval;

	private EntityQuery m_EDWSBuildingQuery;

	private NativeParallelHashMap<Entity, uint> m_TriggerFrames;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerPrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerPrefabSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_CreateChirpSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CreateChirpSystem>();
		m_LifePathEventSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LifePathEventSystem>();
		m_RadioTagSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RadioTagSystem>();
		m_TutorialEventActivationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialEventActivationSystem>();
		m_Queues = new List<NativeQueue<TriggerAction>>();
		m_LastTimedEventTime = DateTime.MinValue;
		m_TimedEventInterval = new TimeSpan(0, 15, 0);
		m_TriggerFrames = new NativeParallelHashMap<Entity, uint>(32, AllocatorHandle.op_Implicit((Allocator)4));
		m_EDWSBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.EarlyDisasterWarningSystem>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
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

	public NativeQueue<TriggerAction> CreateActionBuffer()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		NativeQueue<TriggerAction> val = default(NativeQueue<TriggerAction>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		m_Queues.Add(val);
		return val;
	}

	public void AddActionBufferWriter(JobHandle handle)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = ((SystemBase)this).Dependency;
		((JobHandle)(ref dependency)).Complete();
		((JobHandle)(ref m_Dependencies)).Complete();
		int num = 0;
		for (int i = 0; i < m_Queues.Count; i++)
		{
			num += m_Queues[i].Count;
		}
		if (num == 0)
		{
			for (int j = 0; j < m_Queues.Count; j++)
			{
				m_Queues[j].Dispose();
			}
			m_Queues.Clear();
		}
		NativeArray<TriggerAction> actions = default(NativeArray<TriggerAction>);
		actions._002Ector(num, (Allocator)3, (NativeArrayOptions)0);
		num = 0;
		for (int k = 0; k < m_Queues.Count; k++)
		{
			NativeQueue<TriggerAction> val = m_Queues[k];
			int count = val.Count;
			for (int l = 0; l < count; l++)
			{
				actions[num++] = val.Dequeue();
			}
			val.Dispose();
		}
		m_Queues.Clear();
		JobHandle dependencies;
		JobHandle deps;
		JobHandle deps2;
		JobHandle deps3;
		JobHandle deps4;
		JobHandle dependency2;
		TriggerActionJob triggerActionJob = new TriggerActionJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_TriggerPrefabData = m_TriggerPrefabSystem.ReadTriggerPrefabData(out dependencies),
			m_Actions = actions,
			m_ChirpQueue = m_CreateChirpSystem.GetQueue(out deps),
			m_TriggerFrames = m_TriggerFrames,
			m_LifePathEventQueue = m_LifePathEventSystem.GetQueue(out deps2),
			m_RadioTagQueue = m_RadioTagSystem.GetInputQueue(out deps3),
			m_EmergencyRadioTagQueue = m_RadioTagSystem.GetEmergencyInputQueue(out deps4),
			m_TutorialTriggerQueue = m_TutorialEventActivationSystem.GetQueue(out dependency2),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjectData = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ChirpData = InternalCompilerInterface.GetBufferLookup<TriggerChirpData>(ref __TypeHandle.__Game_Prefabs_TriggerChirpData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LifePathEventData = InternalCompilerInterface.GetComponentLookup<LifePathEventData>(ref __TypeHandle.__Game_Prefabs_LifePathEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RadioEventData = InternalCompilerInterface.GetComponentLookup<RadioEventData>(ref __TypeHandle.__Game_Prefabs_RadioEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TutorialEventData = InternalCompilerInterface.GetBufferLookup<TutorialActivationEventData>(ref __TypeHandle.__Game_Prefabs_TutorialActivationEventData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PolicyData = InternalCompilerInterface.GetComponentLookup<PolicyData>(ref __TypeHandle.__Game_Prefabs_PolicyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerConditions = InternalCompilerInterface.GetBufferLookup<TriggerConditionData>(ref __TypeHandle.__Game_Prefabs_TriggerConditionData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfo = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficAccidentData = InternalCompilerInterface.GetComponentLookup<TrafficAccidentData>(ref __TypeHandle.__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerLimitData = InternalCompilerInterface.GetComponentLookup<TriggerLimitData>(ref __TypeHandle.__Game_Prefabs_TriggerLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		Camera main = Camera.main;
		if ((Object)(object)main != (Object)null)
		{
			triggerActionJob.m_CameraPosition = float3.op_Implicit(((Component)main).transform.position);
		}
		JobHandle val2 = IJobExtensions.Schedule<TriggerActionJob>(triggerActionJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, deps2, deps, deps3, deps4, dependency2));
		m_CreateChirpSystem.AddQueueWriter(val2);
		m_LifePathEventSystem.AddQueueWriter(val2);
		m_RadioTagSystem.AddInputQueueWriter(val2);
		m_RadioTagSystem.AddEmergencyInputQueueWriter(val2);
		m_TriggerPrefabSystem.AddReader(val2);
		m_TutorialEventActivationSystem.AddQueueWriter(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
		if (DateTime.Now - m_LastTimedEventTime >= m_TimedEventInterval)
		{
			Telemetry.CityStats();
			m_LastTimedEventTime = DateTime.Now;
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_TriggerFrames.Dispose();
	}

	public void SetDefaults(Context context)
	{
		m_TriggerFrames.Clear();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		NativeKeyValueArrays<Entity, uint> keyValueArrays = m_TriggerFrames.GetKeyValueArrays(AllocatorHandle.op_Implicit((Allocator)2));
		int length = keyValueArrays.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		for (int i = 0; i < keyValueArrays.Length; i++)
		{
			Entity val = keyValueArrays.Keys[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
			uint num = keyValueArrays.Values[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		}
		keyValueArrays.Dispose();
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		m_TriggerFrames.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		Entity val = default(Entity);
		uint num2 = default(uint);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			if (val != Entity.Null)
			{
				m_TriggerFrames.Add(val, num2);
			}
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
	public TriggerSystem()
	{
	}
}
