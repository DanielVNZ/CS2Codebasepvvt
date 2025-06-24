using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterDangerSystem : GameSystemBase
{
	[BurstCompile]
	private struct WaterDangerJob : IJobChunk
	{
		private struct EndangeredStaticObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public int m_JobIndex;

			public uint m_SimulationFrame;

			public float m_DangerSpeed;

			public float m_DangerHeight;

			public Bounds1 m_PredictionDistance;

			public Entity m_Event;

			public Line2 m_StartLine;

			public WaterLevelChangeData m_WaterLevelChangeData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

			public ComponentLookup<InDanger> m_InDangerData;

			public EntityArchetype m_EndangerArchetype;

			public ParallelWriter m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				if (bounds.m_Bounds.min.y < m_DangerHeight)
				{
					return MathUtils.Intersect(m_PredictionDistance, GetDistanceBounds(((Bounds3)(ref bounds.m_Bounds)).xz, m_StartLine));
				}
				return false;
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				if (bounds.m_Bounds.min.y >= m_DangerHeight)
				{
					return;
				}
				Bounds1 distanceBounds = GetDistanceBounds(((Bounds3)(ref bounds.m_Bounds)).xz, m_StartLine);
				if (!MathUtils.Intersect(m_PredictionDistance, distanceBounds) || !m_BuildingData.HasComponent(item))
				{
					return;
				}
				DangerFlags dangerFlags = m_WaterLevelChangeData.m_DangerFlags;
				if ((dangerFlags & DangerFlags.Evacuate) != 0 && m_EmergencyShelterData.HasComponent(item))
				{
					dangerFlags = (DangerFlags)((uint)dangerFlags & 0xFFFFFFFDu);
					dangerFlags |= DangerFlags.StayIndoors;
				}
				if (m_InDangerData.HasComponent(item))
				{
					InDanger inDanger = m_InDangerData[item];
					if (inDanger.m_EndFrame >= m_SimulationFrame + 64 && (inDanger.m_Event == m_Event || !EventUtils.IsWorse(dangerFlags, inDanger.m_Flags)))
					{
						return;
					}
				}
				float num = 30f + (distanceBounds.max - m_PredictionDistance.min) / m_DangerSpeed;
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(m_JobIndex, m_EndangerArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Endanger>(m_JobIndex, val, new Endanger
				{
					m_Event = m_Event,
					m_Target = item,
					m_Flags = dangerFlags,
					m_EndFrame = m_SimulationFrame + 64 + (uint)(num * 60f)
				});
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<WaterLevelChange> m_WaterLevelChangeType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		public ComponentTypeHandle<Game.Events.DangerLevel> m_DangerLevelType;

		[ReadOnly]
		public ComponentLookup<InDanger> m_InDangerData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> m_PrefabWaterLevelChangeData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public EntityArchetype m_EndangerArchetype;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<WaterLevelChange> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterLevelChange>(ref m_WaterLevelChangeType);
			NativeArray<Duration> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
			NativeArray<Game.Events.DangerLevel> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Events.DangerLevel>(ref m_DangerLevelType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity eventEntity = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				WaterLevelChange waterLevelChange = nativeArray3[i];
				Duration duration = nativeArray4[i];
				WaterLevelChangeData waterLevelChangeData = m_PrefabWaterLevelChangeData[prefabRef.m_Prefab];
				if (m_SimulationFrame < duration.m_EndFrame && waterLevelChangeData.m_DangerFlags != 0)
				{
					FindEndangeredObjects(unfilteredChunkIndex, eventEntity, duration, waterLevelChange, waterLevelChangeData);
				}
				bool flag = m_SimulationFrame > duration.m_StartFrame && m_SimulationFrame < duration.m_EndFrame;
				nativeArray5[i] = new Game.Events.DangerLevel(flag ? waterLevelChangeData.m_DangerLevel : 0f);
			}
		}

		private void FindEndangeredObjects(int jobIndex, Entity eventEntity, Duration duration, WaterLevelChange waterLevelChange, WaterLevelChangeData waterLevelChangeData)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			float value = 10f;
			float num = 0f;
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.DisasterWarningTime);
			if (duration.m_StartFrame > m_SimulationFrame)
			{
				value -= (float)(duration.m_StartFrame - m_SimulationFrame) / 60f;
			}
			else
			{
				num = (float)(m_SimulationFrame - duration.m_StartFrame) / 60f;
			}
			value = math.max(0f, value);
			float num2 = (float)(duration.m_EndFrame - WaterLevelChangeSystem.TsunamiEndDelay - duration.m_StartFrame) / 60f;
			float num3 = WaterSystem.WaveSpeed * 60f;
			float num4 = num * num3;
			float num5 = num4 - num2 * num3;
			float2 val = (float)(WaterSystem.kMapSize / 2) * -waterLevelChange.m_Direction;
			Line2 startLine = default(Line2);
			((Line2)(ref startLine))._002Ector(val, val + MathUtils.Right(waterLevelChange.m_Direction));
			Bounds1 predictionDistance = default(Bounds1);
			((Bounds1)(ref predictionDistance))._002Ector(num5, num4);
			predictionDistance.max += value * num3;
			EndangeredStaticObjectIterator endangeredStaticObjectIterator = new EndangeredStaticObjectIterator
			{
				m_JobIndex = jobIndex,
				m_SimulationFrame = m_SimulationFrame,
				m_DangerSpeed = num3,
				m_DangerHeight = waterLevelChange.m_DangerHeight,
				m_PredictionDistance = predictionDistance,
				m_Event = eventEntity,
				m_StartLine = startLine,
				m_WaterLevelChangeData = waterLevelChangeData,
				m_BuildingData = m_BuildingData,
				m_EmergencyShelterData = m_EmergencyShelterData,
				m_InDangerData = m_InDangerData,
				m_EndangerArchetype = m_EndangerArchetype,
				m_CommandBuffer = m_CommandBuffer
			};
			m_StaticObjectSearchTree.Iterate<EndangeredStaticObjectIterator>(ref endangeredStaticObjectIterator, 0);
		}

		private static Bounds1 GetDistanceBounds(Bounds2 bounds, Line2 line)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			float num = default(float);
			((float4)(ref val))._002Ector(MathUtils.Distance(line, bounds.min, ref num), MathUtils.Distance(line, new float2(bounds.min.x, bounds.max.y), ref num), MathUtils.Distance(line, bounds.max, ref num), MathUtils.Distance(line, new float2(bounds.max.x, bounds.min.y), ref num));
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(math.cmin(val), math.cmax(val));
			float2 val3 = default(float2);
			if (MathUtils.Intersect(bounds, line, ref val3))
			{
				val2 |= 0f;
				return val2;
			}
			return val2;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<WaterLevelChange> __Game_Events_WaterLevelChange_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Events.DangerLevel> __Game_Events_DangerLevel_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<InDanger> __Game_Events_InDanger_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> __Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Events_WaterLevelChange_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterLevelChange>(false);
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Events_DangerLevel_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.DangerLevel>(false);
			__Game_Events_InDanger_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.EmergencyShelter>(true);
			__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterLevelChangeData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private SearchSystem m_ObjectSearchSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_WaterLevelChangeQuery;

	private EntityArchetype m_EndangerArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_WaterLevelChangeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaterLevelChange>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EndangerArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Endanger>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_WaterLevelChangeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		WaterDangerJob waterDangerJob = new WaterDangerJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterLevelChangeType = InternalCompilerInterface.GetComponentTypeHandle<WaterLevelChange>(ref __TypeHandle.__Game_Events_WaterLevelChange_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DangerLevelType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.DangerLevel>(ref __TypeHandle.__Game_Events_DangerLevel_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerData = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWaterLevelChangeData = InternalCompilerInterface.GetComponentLookup<WaterLevelChangeData>(ref __TypeHandle.__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_City = m_CitySystem.City,
			m_EndangerArchetype = m_EndangerArchetype,
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		waterDangerJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<WaterDangerJob>(waterDangerJob, m_WaterLevelChangeQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public WaterDangerSystem()
	{
	}
}
