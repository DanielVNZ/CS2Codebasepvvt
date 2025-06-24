using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
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
public class WeatherPhenomenonSystem : GameSystemBase
{
	[BurstCompile]
	private struct WeatherPhenomenonJob : IJobChunk
	{
		private struct LightningTargetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Game.Events.WeatherPhenomenon m_WeatherPhenomenon;

			public Entity m_SelectedEntity;

			public float3 m_SelectedPosition;

			public float m_BestDistance;

			public ComponentLookup<Building> m_BuildingData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				float num = MathUtils.Distance(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_WeatherPhenomenon.m_HotspotPosition)).xz);
				if (num < m_WeatherPhenomenon.m_HotspotRadius)
				{
					return num * 0.5f - bounds.m_Bounds.max.y < m_BestDistance;
				}
				return false;
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				float2 val = MathUtils.Center(((Bounds3)(ref bounds.m_Bounds)).xz);
				float num = math.distance(val, ((float3)(ref m_WeatherPhenomenon.m_HotspotPosition)).xz);
				if (!(num >= m_WeatherPhenomenon.m_HotspotRadius))
				{
					num = num * 0.5f - bounds.m_Bounds.max.y;
					if (!(num >= m_BestDistance) && ((bounds.m_Mask & BoundsMask.IsTree) != 0 || m_BuildingData.HasComponent(item)))
					{
						m_SelectedEntity = item;
						m_SelectedPosition = new float3(val.x, bounds.m_Bounds.max.y, val.y);
						m_BestDistance = num;
					}
				}
			}
		}

		private struct EndangeredStaticObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public int m_JobIndex;

			public uint m_SimulationFrame;

			public float m_DangerSpeed;

			public Entity m_Event;

			public Segment m_Line;

			public float m_Radius;

			public WeatherPhenomenonData m_WeatherPhenomenonData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

			public ComponentLookup<Placeholder> m_PlaceholderData;

			public ComponentLookup<InDanger> m_InDangerData;

			public EntityArchetype m_EndangerArchetype;

			public ParallelWriter m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(MathUtils.Expand(((Bounds3)(ref bounds.m_Bounds)).xz, float2.op_Implicit(m_Radius)), m_Line, ref val);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0194: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (!MathUtils.Intersect(MathUtils.Expand(((Bounds3)(ref bounds.m_Bounds)).xz, float2.op_Implicit(m_Radius)), m_Line, ref val))
				{
					return;
				}
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, new Circle2(m_Radius, m_Line.a)))
				{
					float2 val2 = m_Line.b - m_Line.a;
					if (!MathUtils.TryNormalize(ref val2))
					{
						return;
					}
					if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, new Circle2(m_Radius, m_Line.b)))
					{
						float2 val3 = MathUtils.Right(val2);
						Quad2 val4 = default(Quad2);
						((Quad2)(ref val4))._002Ector(m_Line.a - val3, m_Line.a + val3, m_Line.b + val3, m_Line.b - val3);
						if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, val4))
						{
							return;
						}
					}
				}
				if (!m_BuildingData.HasComponent(item) || m_PlaceholderData.HasComponent(item))
				{
					return;
				}
				DangerFlags dangerFlags = m_WeatherPhenomenonData.m_DangerFlags;
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
				float num = 30f + math.max(m_Radius, MathUtils.Distance(((Bounds3)(ref bounds.m_Bounds)).xz, m_Line.a)) / m_DangerSpeed;
				Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(m_JobIndex, m_EndangerArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Endanger>(m_JobIndex, val5, new Endanger
				{
					m_Event = m_Event,
					m_Target = item,
					m_Flags = dangerFlags,
					m_EndFrame = m_SimulationFrame + 64 + (uint)(num * 60f)
				});
			}
		}

		private struct AffectedStaticObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public int m_JobIndex;

			public Entity m_Event;

			public Circle2 m_Circle;

			public Game.Events.WeatherPhenomenon m_WeatherPhenomenon;

			public WeatherPhenomenonData m_WeatherPhenomenonData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<Placeholder> m_PlaceholderData;

			public ComponentLookup<Destroyed> m_DestroyedData;

			public ComponentLookup<FacingWeather> m_FacingWeatherData;

			public EntityArchetype m_FaceWeatherArchetype;

			public ParallelWriter m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle) || !m_BuildingData.HasComponent(item) || m_PlaceholderData.HasComponent(item))
				{
					return;
				}
				float num = 0f;
				if (m_FacingWeatherData.HasComponent(item))
				{
					FacingWeather facingWeather = m_FacingWeatherData[item];
					if (facingWeather.m_Event == m_Event)
					{
						return;
					}
					num = facingWeather.m_Severity;
				}
				if (!m_DestroyedData.HasComponent(item))
				{
					float severity = EventUtils.GetSeverity(m_TransformData[item].m_Position, m_WeatherPhenomenon, m_WeatherPhenomenonData);
					if (severity > num)
					{
						Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(m_JobIndex, m_FaceWeatherArchetype);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<FaceWeather>(m_JobIndex, val, new FaceWeather
						{
							m_Event = m_Event,
							m_Target = item,
							m_Severity = severity
						});
					}
				}
			}
		}

		private struct AffectedNetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public int m_JobIndex;

			public Entity m_Event;

			public Circle2 m_Circle;

			public Game.Events.WeatherPhenomenon m_WeatherPhenomenon;

			public TrafficAccidentData m_TrafficAccidentData;

			public Random m_Random;

			public float m_DividedProbability;

			public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

			public ComponentLookup<Car> m_CarData;

			public ComponentLookup<Moving> m_MovingData;

			public ComponentLookup<Transform> m_TransformData;

			public BufferLookup<Game.Net.SubLane> m_SubLanes;

			public BufferLookup<LaneObject> m_LaneObjects;

			public EntityArchetype m_EventImpactArchetype;

			public ParallelWriter m_CommandBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Circle) || ((Random)(ref m_Random)).NextFloat(1f) >= m_DividedProbability)
				{
					return;
				}
				Entity val = TryFindSubject(item, ref m_Random, m_TrafficAccidentData);
				if (val != Entity.Null)
				{
					Transform transform = m_TransformData[val];
					float num = math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref m_WeatherPhenomenon.m_HotspotPosition)).xz);
					float num2 = 4f / 15f;
					float num3 = m_DividedProbability * (m_WeatherPhenomenon.m_HotspotRadius - num) * num2;
					if (!(((Random)(ref m_Random)).NextFloat(m_WeatherPhenomenon.m_HotspotRadius) >= num3))
					{
						AddImpact(m_JobIndex, m_Event, ref m_Random, val, m_TrafficAccidentData);
					}
				}
			}

			private Entity TryFindSubject(Entity entity, ref Random random, TrafficAccidentData trafficAccidentData)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				Entity result = Entity.Null;
				int num = 0;
				if (m_SubLanes.HasBuffer(entity))
				{
					DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[entity];
					for (int i = 0; i < val.Length; i++)
					{
						Entity subLane = val[i].m_SubLane;
						if (!m_LaneObjects.HasBuffer(subLane))
						{
							continue;
						}
						DynamicBuffer<LaneObject> val2 = m_LaneObjects[subLane];
						for (int j = 0; j < val2.Length; j++)
						{
							Entity laneObject = val2[j].m_LaneObject;
							if (trafficAccidentData.m_SubjectType == EventTargetType.MovingCar && m_CarData.HasComponent(laneObject) && m_MovingData.HasComponent(laneObject) && !m_InvolvedInAccidentData.HasComponent(laneObject))
							{
								num++;
								if (((Random)(ref random)).NextInt(num) == num - 1)
								{
									result = laneObject;
								}
							}
						}
					}
				}
				return result;
			}

			private void AddImpact(int jobIndex, Entity eventEntity, ref Random random, Entity target, TrafficAccidentData trafficAccidentData)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				Impact impact = new Impact
				{
					m_Event = eventEntity,
					m_Target = target
				};
				if (trafficAccidentData.m_AccidentType == TrafficAccidentType.LoseControl && m_MovingData.HasComponent(target))
				{
					Moving moving = m_MovingData[target];
					impact.m_Severity = 5f;
					if (((Random)(ref random)).NextBool())
					{
						impact.m_AngularVelocityDelta.y = -2f;
						((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Left(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
					}
					else
					{
						impact.m_AngularVelocityDelta.y = 2f;
						((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Right(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
					}
				}
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EventImpactArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Impact>(jobIndex, val, impact);
			}
		}

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_FaceWeatherArchetype;

		[ReadOnly]
		public EntityArchetype m_ImpactArchetype;

		[ReadOnly]
		public EntityArchetype m_EndangerArchetype;

		[ReadOnly]
		public EntityArchetype m_EventIgniteArchetype;

		[ReadOnly]
		public CellMapData<Wind> m_WindData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<LightningStrike> m_LightningStrikes;

		[ReadOnly]
		public NativeArray<Entity> m_EarlyDisasterWarningSystems;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Game.Events.WeatherPhenomenon> m_WeatherPhenomenonType;

		public BufferTypeHandle<HotspotFrame> m_HotspotFrameType;

		public ComponentTypeHandle<Game.Events.DangerLevel> m_DangerLevelType;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> m_EmergencyShelterData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<FacingWeather> m_FacingWeatherData;

		[ReadOnly]
		public ComponentLookup<InDanger> m_InDangerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<WeatherPhenomenonData> m_WeatherPhenomenonData;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> m_TrafficAccidentData;

		[ReadOnly]
		public ComponentLookup<FireData> m_PrefabFireData;

		[ReadOnly]
		public ComponentLookup<DestructibleObjectData> m_PrefabDestructibleObjectData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.pow(0.9f, num);
			int num3 = (int)((m_SimulationFrame / 16) & 3);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Duration> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
			NativeArray<Game.Events.WeatherPhenomenon> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Events.WeatherPhenomenon>(ref m_WeatherPhenomenonType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<HotspotFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HotspotFrame>(ref m_HotspotFrameType);
			NativeArray<Game.Events.DangerLevel> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Events.DangerLevel>(ref m_DangerLevelType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity val = nativeArray[i];
				Duration duration = nativeArray2[i];
				Game.Events.WeatherPhenomenon weatherPhenomenon = nativeArray3[i];
				PrefabRef eventPrefabRef = nativeArray4[i];
				WeatherPhenomenonData weatherPhenomenonData = m_WeatherPhenomenonData[eventPrefabRef.m_Prefab];
				float intensity = weatherPhenomenon.m_Intensity;
				if (duration.m_EndFrame <= m_SimulationFrame)
				{
					weatherPhenomenon.m_Intensity = math.max(0f, weatherPhenomenon.m_Intensity - num * 0.2f);
				}
				else if (duration.m_StartFrame <= m_SimulationFrame)
				{
					weatherPhenomenon.m_Intensity = math.min(1f, weatherPhenomenon.m_Intensity + num * 0.2f);
				}
				float2 val2 = Wind.SampleWind(m_WindData, weatherPhenomenon.m_PhenomenonPosition) * 20f;
				if (weatherPhenomenon.m_Intensity != 0f)
				{
					ref float3 phenomenonPosition = ref weatherPhenomenon.m_PhenomenonPosition;
					((float3)(ref phenomenonPosition)).xz = ((float3)(ref phenomenonPosition)).xz + val2 * num;
					weatherPhenomenon.m_PhenomenonPosition.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, weatherPhenomenon.m_PhenomenonPosition);
					float num4 = weatherPhenomenon.m_PhenomenonRadius - weatherPhenomenon.m_HotspotRadius;
					float2 val3 = ((float3)(ref weatherPhenomenon.m_PhenomenonPosition)).xz - ((float3)(ref weatherPhenomenon.m_HotspotPosition)).xz;
					float2 val4 = val2 + (val3 + ((Random)(ref random)).NextFloat2(float2.op_Implicit(0f - num4), float2.op_Implicit(num4))) * weatherPhenomenonData.m_HotspotInstability;
					((float3)(ref weatherPhenomenon.m_HotspotVelocity)).xz = math.lerp(val4, ((float3)(ref weatherPhenomenon.m_HotspotVelocity)).xz, num2);
					float num5 = math.length(val3);
					if (num5 >= 0.001f)
					{
						float num6 = (num4 - num5) * weatherPhenomenonData.m_HotspotInstability;
						float num7 = math.dot(val3, val2 - ((float3)(ref weatherPhenomenon.m_HotspotVelocity)).xz) / num5;
						ref float3 hotspotVelocity = ref weatherPhenomenon.m_HotspotVelocity;
						((float3)(ref hotspotVelocity)).xz = ((float3)(ref hotspotVelocity)).xz + val3 * (math.max(0f, num7 - num6) / num5);
					}
					ref float3 hotspotPosition = ref weatherPhenomenon.m_HotspotPosition;
					hotspotPosition += weatherPhenomenon.m_HotspotVelocity * num;
					weatherPhenomenon.m_HotspotPosition.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, weatherPhenomenon.m_HotspotPosition);
					if (weatherPhenomenonData.m_DamageSeverity != 0f)
					{
						FindAffectedObjects(unfilteredChunkIndex, val, weatherPhenomenon, weatherPhenomenonData);
					}
					if (weatherPhenomenon.m_LightningTimer != 0f)
					{
						weatherPhenomenon.m_LightningTimer -= num;
						while (weatherPhenomenon.m_LightningTimer <= 0f)
						{
							LightningStrike(ref random, unfilteredChunkIndex, val, weatherPhenomenon, eventPrefabRef);
							float num8 = ((Random)(ref random)).NextFloat(weatherPhenomenonData.m_LightningInterval.min, weatherPhenomenonData.m_LightningInterval.max);
							if (num8 <= 0f)
							{
								weatherPhenomenon.m_LightningTimer = 0f;
								break;
							}
							weatherPhenomenon.m_LightningTimer += num8;
						}
					}
					if (m_TrafficAccidentData.HasComponent(eventPrefabRef.m_Prefab))
					{
						TrafficAccidentData trafficAccidentData = m_TrafficAccidentData[eventPrefabRef.m_Prefab];
						FindAffectedEdges(unfilteredChunkIndex, ref random, val, weatherPhenomenon, trafficAccidentData);
					}
				}
				else
				{
					weatherPhenomenon.m_HotspotVelocity = float3.op_Implicit(0f);
				}
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<HotspotFrame> val5 = bufferAccessor[i];
					val5[num3] = new HotspotFrame
					{
						m_Position = weatherPhenomenon.m_HotspotPosition - weatherPhenomenon.m_HotspotVelocity * num * 0.5f,
						m_Velocity = weatherPhenomenon.m_HotspotVelocity
					};
				}
				if (m_SimulationFrame < duration.m_EndFrame && weatherPhenomenonData.m_DangerFlags != 0)
				{
					FindEndangeredObjects(unfilteredChunkIndex, val, duration, weatherPhenomenon, val2, weatherPhenomenonData);
				}
				if (intensity != 0f != (weatherPhenomenon.m_Intensity != 0f))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, val, default(EffectsUpdated));
				}
				bool flag = m_SimulationFrame > duration.m_StartFrame && m_SimulationFrame < duration.m_EndFrame;
				nativeArray5[i] = new Game.Events.DangerLevel(flag ? weatherPhenomenonData.m_DangerLevel : 0f);
				nativeArray3[i] = weatherPhenomenon;
				if (m_SimulationFrame <= duration.m_EndFrame)
				{
					continue;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
				Enumerator<Entity> enumerator = m_EarlyDisasterWarningSystems.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Entity current = enumerator.Current;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, current, default(EffectsUpdated));
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
		}

		private void LightningStrike(ref Random random, int jobIndex, Entity eventEntity, Game.Events.WeatherPhenomenon weatherPhenomenon, PrefabRef eventPrefabRef)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			LightningTargetIterator lightningTargetIterator = new LightningTargetIterator
			{
				m_WeatherPhenomenon = weatherPhenomenon,
				m_BestDistance = float.MaxValue,
				m_BuildingData = m_BuildingData
			};
			m_StaticObjectSearchTree.Iterate<LightningTargetIterator>(ref lightningTargetIterator, 0);
			if (lightningTargetIterator.m_SelectedEntity != Entity.Null)
			{
				m_LightningStrikes.Enqueue(new LightningStrike
				{
					m_HitEntity = lightningTargetIterator.m_SelectedEntity,
					m_Position = lightningTargetIterator.m_SelectedPosition
				});
			}
			PrefabRef prefabRef = default(PrefabRef);
			if (!m_PrefabRefData.TryGetComponent(lightningTargetIterator.m_SelectedEntity, ref prefabRef))
			{
				return;
			}
			bool flag = false;
			FireData fireData = default(FireData);
			if (m_PrefabFireData.TryGetComponent(eventPrefabRef.m_Prefab, ref fireData))
			{
				float startProbability = fireData.m_StartProbability;
				if (startProbability > 0.01f)
				{
					flag = ((Random)(ref random)).NextFloat(100f) < startProbability;
				}
			}
			DestructibleObjectData destructibleObjectData = default(DestructibleObjectData);
			if (flag && m_PrefabDestructibleObjectData.TryGetComponent((Entity)prefabRef, ref destructibleObjectData) && destructibleObjectData.m_FireHazard == 0f)
			{
				flag = false;
			}
			if (flag)
			{
				Ignite ignite = new Ignite
				{
					m_Target = lightningTargetIterator.m_SelectedEntity,
					m_Event = eventEntity,
					m_Intensity = fireData.m_StartIntensity,
					m_RequestFrame = m_SimulationFrame
				};
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EventIgniteArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Ignite>(jobIndex, val, ignite);
			}
		}

		private void FindEndangeredObjects(int jobIndex, Entity eventEntity, Duration duration, Game.Events.WeatherPhenomenon weatherPhenomenon, float2 wind, WeatherPhenomenonData weatherPhenomenonData)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			float value = 0f;
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.DisasterWarningTime);
			if (duration.m_StartFrame > m_SimulationFrame)
			{
				value -= (float)(duration.m_StartFrame - m_SimulationFrame) / 60f;
			}
			value = math.max(0f, value);
			EndangeredStaticObjectIterator endangeredStaticObjectIterator = new EndangeredStaticObjectIterator
			{
				m_JobIndex = jobIndex,
				m_DangerSpeed = math.length(wind),
				m_SimulationFrame = m_SimulationFrame,
				m_Event = eventEntity,
				m_Line = new Segment(((float3)(ref weatherPhenomenon.m_PhenomenonPosition)).xz, ((float3)(ref weatherPhenomenon.m_PhenomenonPosition)).xz + wind * value),
				m_Radius = weatherPhenomenon.m_PhenomenonRadius,
				m_WeatherPhenomenonData = weatherPhenomenonData,
				m_BuildingData = m_BuildingData,
				m_EmergencyShelterData = m_EmergencyShelterData,
				m_PlaceholderData = m_PlaceholderData,
				m_InDangerData = m_InDangerData,
				m_EndangerArchetype = m_EndangerArchetype,
				m_CommandBuffer = m_CommandBuffer
			};
			m_StaticObjectSearchTree.Iterate<EndangeredStaticObjectIterator>(ref endangeredStaticObjectIterator, 0);
		}

		private void FindAffectedObjects(int jobIndex, Entity eventEntity, Game.Events.WeatherPhenomenon weatherPhenomenon, WeatherPhenomenonData weatherPhenomenonData)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			AffectedStaticObjectIterator affectedStaticObjectIterator = new AffectedStaticObjectIterator
			{
				m_JobIndex = jobIndex,
				m_Event = eventEntity,
				m_Circle = new Circle2(weatherPhenomenon.m_HotspotRadius, ((float3)(ref weatherPhenomenon.m_HotspotPosition)).xz),
				m_WeatherPhenomenon = weatherPhenomenon,
				m_WeatherPhenomenonData = weatherPhenomenonData,
				m_BuildingData = m_BuildingData,
				m_TransformData = m_TransformData,
				m_PlaceholderData = m_PlaceholderData,
				m_DestroyedData = m_DestroyedData,
				m_FacingWeatherData = m_FacingWeatherData,
				m_FaceWeatherArchetype = m_FaceWeatherArchetype,
				m_CommandBuffer = m_CommandBuffer
			};
			m_StaticObjectSearchTree.Iterate<AffectedStaticObjectIterator>(ref affectedStaticObjectIterator, 0);
		}

		private void FindAffectedEdges(int jobIndex, ref Random random, Entity eventEntity, Game.Events.WeatherPhenomenon weatherPhenomenon, TrafficAccidentData trafficAccidentData)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			float dividedProbability = math.sqrt(trafficAccidentData.m_OccurenceProbability * 0.01f);
			AffectedNetIterator affectedNetIterator = new AffectedNetIterator
			{
				m_JobIndex = jobIndex,
				m_Event = eventEntity,
				m_Circle = new Circle2(weatherPhenomenon.m_HotspotRadius, ((float3)(ref weatherPhenomenon.m_HotspotPosition)).xz),
				m_WeatherPhenomenon = weatherPhenomenon,
				m_TrafficAccidentData = trafficAccidentData,
				m_Random = random,
				m_DividedProbability = dividedProbability,
				m_InvolvedInAccidentData = m_InvolvedInAccidentData,
				m_CarData = m_CarData,
				m_MovingData = m_MovingData,
				m_TransformData = m_TransformData,
				m_SubLanes = m_SubLanes,
				m_LaneObjects = m_LaneObjects,
				m_EventImpactArchetype = m_ImpactArchetype,
				m_CommandBuffer = m_CommandBuffer
			};
			m_NetSearchTree.Iterate<AffectedNetIterator>(ref affectedNetIterator, 0);
			random = affectedNetIterator.m_Random;
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
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Events.WeatherPhenomenon> __Game_Events_WeatherPhenomenon_RW_ComponentTypeHandle;

		public BufferTypeHandle<HotspotFrame> __Game_Events_HotspotFrame_RW_BufferTypeHandle;

		public ComponentTypeHandle<Game.Events.DangerLevel> __Game_Events_DangerLevel_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.EmergencyShelter> __Game_Buildings_EmergencyShelter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Placeholder> __Game_Objects_Placeholder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FacingWeather> __Game_Events_FacingWeather_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InDanger> __Game_Events_InDanger_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WeatherPhenomenonData> __Game_Prefabs_WeatherPhenomenonData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> __Game_Prefabs_TrafficAccidentData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireData> __Game_Prefabs_FireData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DestructibleObjectData> __Game_Prefabs_DestructibleObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

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
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Events_WeatherPhenomenon_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.WeatherPhenomenon>(false);
			__Game_Events_HotspotFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HotspotFrame>(false);
			__Game_Events_DangerLevel_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.DangerLevel>(false);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_EmergencyShelter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.EmergencyShelter>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Objects_Placeholder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Placeholder>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Events_FacingWeather_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FacingWeather>(true);
			__Game_Events_InDanger_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WeatherPhenomenonData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WeatherPhenomenonData>(true);
			__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficAccidentData>(true);
			__Game_Prefabs_FireData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireData>(true);
			__Game_Prefabs_DestructibleObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DestructibleObjectData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private WindSystem m_WindSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private CitySystem m_CitySystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private ClimateRenderSystem m_ClimateRenderSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_PhenomenonQuery;

	private EntityArchetype m_FaceWeatherArchetype;

	private EntityArchetype m_ImpactArchetype;

	private EntityArchetype m_EndangerArchetype;

	private EntityArchetype m_EventIgniteArchetype;

	private EntityQuery m_EDWSBuildingQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 0;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ClimateRenderSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ClimateRenderSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PhenomenonQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Game.Events.WeatherPhenomenon>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_FaceWeatherArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<FaceWeather>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ImpactArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Impact>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EndangerArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Endanger>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EventIgniteArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Ignite>()
		});
		m_EDWSBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.EarlyDisasterWarningSystem>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PhenomenonQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle deps;
		JobHandle dependencies2;
		JobHandle dependencies3;
		WeatherPhenomenonJob weatherPhenomenonJob = new WeatherPhenomenonJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_FaceWeatherArchetype = m_FaceWeatherArchetype,
			m_ImpactArchetype = m_ImpactArchetype,
			m_EndangerArchetype = m_EndangerArchetype,
			m_EventIgniteArchetype = m_EventIgniteArchetype,
			m_WindData = m_WindSystem.GetData(readOnly: true, out dependencies),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_City = m_CitySystem.City,
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies3)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		weatherPhenomenonJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		weatherPhenomenonJob.m_LightningStrikes = m_ClimateRenderSystem.GetLightningStrikeQueue(out var dependencies4).AsParallelWriter();
		weatherPhenomenonJob.m_EarlyDisasterWarningSystems = ((EntityQuery)(ref m_EDWSBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		weatherPhenomenonJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_WeatherPhenomenonType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.WeatherPhenomenon>(ref __TypeHandle.__Game_Events_WeatherPhenomenon_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_HotspotFrameType = InternalCompilerInterface.GetBufferTypeHandle<HotspotFrame>(ref __TypeHandle.__Game_Events_HotspotFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_DangerLevelType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.DangerLevel>(ref __TypeHandle.__Game_Events_DangerLevel_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_EmergencyShelterData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.EmergencyShelter>(ref __TypeHandle.__Game_Buildings_EmergencyShelter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_PlaceholderData = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_FacingWeatherData = InternalCompilerInterface.GetComponentLookup<FacingWeather>(ref __TypeHandle.__Game_Events_FacingWeather_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_InDangerData = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_WeatherPhenomenonData = InternalCompilerInterface.GetComponentLookup<WeatherPhenomenonData>(ref __TypeHandle.__Game_Prefabs_WeatherPhenomenonData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_TrafficAccidentData = InternalCompilerInterface.GetComponentLookup<TrafficAccidentData>(ref __TypeHandle.__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_PrefabFireData = InternalCompilerInterface.GetComponentLookup<FireData>(ref __TypeHandle.__Game_Prefabs_FireData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_PrefabDestructibleObjectData = InternalCompilerInterface.GetComponentLookup<DestructibleObjectData>(ref __TypeHandle.__Game_Prefabs_DestructibleObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		weatherPhenomenonJob.m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		WeatherPhenomenonJob weatherPhenomenonJob2 = weatherPhenomenonJob;
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<WeatherPhenomenonJob>(weatherPhenomenonJob2, m_PhenomenonQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, deps, dependencies2, dependencies3, dependencies4));
		weatherPhenomenonJob2.m_EarlyDisasterWarningSystems.Dispose(val2);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val2);
		m_NetSearchSystem.AddNetSearchTreeReader(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_WindSystem.AddReader(val2);
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
		m_ClimateRenderSystem.AddLightningStrikeWriter(val2);
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
	public WeatherPhenomenonSystem()
	{
	}
}
