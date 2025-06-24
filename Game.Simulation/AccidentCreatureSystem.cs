using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Net;
using Game.Notifications;
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
public class AccidentCreatureSystem : GameSystemBase
{
	[BurstCompile]
	private struct AccidentCreatureJob : IJobChunk
	{
		private struct EdgeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float3 m_Position;

			public float m_MaxDistance;

			public Entity m_Result;

			public ComponentLookup<AccidentSite> m_AccidentSiteData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<Road> m_RoadData;

			public ComponentLookup<Curve> m_CurveData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(m_Bounds, bounds.m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity edge)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(m_Bounds, bounds.m_Bounds) && m_RoadData.HasComponent(edge) && !m_AccidentSiteData.HasComponent(edge) && m_CurveData.HasComponent(edge))
				{
					Curve curve = m_CurveData[edge];
					EdgeGeometry edgeGeometry = m_EdgeGeometryData[edge];
					float num = math.distance(edgeGeometry.m_Start.m_Left.d, edgeGeometry.m_Start.m_Right.d);
					float num3 = default(float);
					float num2 = MathUtils.Distance(curve.m_Bezier, m_Position, ref num3) - num * 0.5f;
					if (num2 < m_MaxDistance)
					{
						m_MaxDistance = num2;
						m_Result = edge;
					}
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Creatures.Resident> m_ResidentType;

		[ReadOnly]
		public ComponentTypeHandle<InvolvedInAccident> m_InvolvedInAccidentType;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> m_StumblingType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		public ComponentTypeHandle<Creature> m_CreatureType;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> m_HearseData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> m_AmbulanceData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public PoliceConfigurationData m_PoliceConfigurationData;

		[ReadOnly]
		public EntityArchetype m_AddAccidentSiteArchetype;

		[ReadOnly]
		public EntityArchetype m_AddProblemArchetype;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<InvolvedInAccident> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InvolvedInAccident>(ref m_InvolvedInAccidentType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (((ArchetypeChunk)(ref chunk)).Has<Stumbling>(ref m_StumblingType))
			{
				NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<Moving> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
				NativeArray<Game.Creatures.Resident> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Resident>(ref m_ResidentType);
				NativeArray<Creature> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Creature>(ref m_CreatureType);
				for (int i = 0; i < nativeArray6.Length; i++)
				{
					ref Creature reference = ref CollectionUtils.ElementAt<Creature>(nativeArray6, i);
					reference.m_QueueEntity = Entity.Null;
					reference.m_QueueArea = default(Sphere3);
				}
				if (((ArchetypeChunk)(ref chunk)).Has<CurrentVehicle>(ref m_CurrentVehicleType))
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val = nativeArray[j];
						Transform transform = nativeArray3[j];
						InvolvedInAccident involvedInAccident = nativeArray2[j];
						if (nativeArray5.Length != 0)
						{
							Game.Creatures.Resident resident = nativeArray5[j];
							HealthProblemFlags num = AddInjury(unfilteredChunkIndex, involvedInAccident, resident, ref random);
							StopStumbling(unfilteredChunkIndex, val);
							if ((num & HealthProblemFlags.RequireTransport) == 0)
							{
								m_IconCommandBuffer.Add(val, m_PoliceConfigurationData.m_TrafficAccidentNotificationPrefab, IconPriority.MajorProblem, IconClusterLayer.Default, IconFlags.IgnoreTarget, involvedInAccident.m_Event);
							}
							if (!m_TargetElements.HasBuffer(involvedInAccident.m_Event))
							{
								continue;
							}
							Entity val2 = FindAccidentSite(involvedInAccident.m_Event);
							if (val2 == Entity.Null)
							{
								val2 = FindSuitableAccidentSite(transform.m_Position);
								if (val2 != Entity.Null)
								{
									AddAccidentSite(unfilteredChunkIndex, ref involvedInAccident, val2);
								}
							}
						}
						else
						{
							ClearAccident(unfilteredChunkIndex, val);
						}
					}
					return;
				}
				if (nativeArray4.Length != 0)
				{
					for (int k = 0; k < nativeArray4.Length; k++)
					{
						Entity val3 = nativeArray[k];
						Transform transform2 = nativeArray3[k];
						Moving moving = nativeArray4[k];
						InvolvedInAccident involvedInAccident2 = nativeArray2[k];
						if (transform2.m_Position.y < -1000f)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val3, default(Deleted));
						}
						else
						{
							if (!(math.lengthsq(moving.m_Velocity) < 0.0001f) || !(math.lengthsq(moving.m_AngularVelocity) < 0.0001f))
							{
								continue;
							}
							if (nativeArray5.Length != 0)
							{
								Game.Creatures.Resident resident2 = nativeArray5[k];
								if ((AddInjury(unfilteredChunkIndex, involvedInAccident2, resident2, ref random) & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
								{
									StopMoving(unfilteredChunkIndex, val3);
								}
								else
								{
									StopStumbling(unfilteredChunkIndex, val3);
									m_IconCommandBuffer.Add(val3, m_PoliceConfigurationData.m_TrafficAccidentNotificationPrefab, IconPriority.MajorProblem, IconClusterLayer.Default, IconFlags.IgnoreTarget, involvedInAccident2.m_Event);
								}
								if (!m_TargetElements.HasBuffer(involvedInAccident2.m_Event))
								{
									continue;
								}
								Entity val4 = FindAccidentSite(involvedInAccident2.m_Event);
								if (val4 == Entity.Null)
								{
									val4 = FindSuitableAccidentSite(transform2.m_Position);
									if (val4 != Entity.Null)
									{
										AddAccidentSite(unfilteredChunkIndex, ref involvedInAccident2, val4);
									}
								}
							}
							else
							{
								ClearAccident(unfilteredChunkIndex, val3);
							}
						}
					}
					return;
				}
				NativeArray<Target> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Entity entity = nativeArray[l];
					InvolvedInAccident involvedInAccident3 = nativeArray2[l];
					Target target = nativeArray7[l];
					if (IsSecured(involvedInAccident3) || m_HearseData.HasComponent(target.m_Target) || m_AmbulanceData.HasComponent(target.m_Target))
					{
						StartMoving(unfilteredChunkIndex, entity);
						ClearAccident(unfilteredChunkIndex, entity);
					}
				}
				return;
			}
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			for (int m = 0; m < nativeArray.Length; m++)
			{
				Entity entity2 = nativeArray[m];
				InvolvedInAccident involvedInAccident4 = nativeArray2[m];
				Target target2 = nativeArray8[m];
				if (IsSecured(involvedInAccident4) || m_HearseData.HasComponent(target2.m_Target) || m_AmbulanceData.HasComponent(target2.m_Target))
				{
					ClearAccident(unfilteredChunkIndex, entity2);
				}
			}
		}

		private HealthProblemFlags AddInjury(int jobIndex, InvolvedInAccident involvedInAccident, Game.Creatures.Resident resident, ref Random random)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			int num = 50;
			if (m_PrefabRefData.HasComponent(resident.m_Citizen) && ((Random)(ref random)).NextInt(100) < num)
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_AddProblemArchetype);
				AddHealthProblem addHealthProblem = new AddHealthProblem
				{
					m_Event = involvedInAccident.m_Event,
					m_Target = resident.m_Citizen,
					m_Flags = HealthProblemFlags.RequireTransport
				};
				addHealthProblem.m_Flags |= (HealthProblemFlags)((((Random)(ref random)).NextInt(100) < 20) ? 2 : 4);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AddHealthProblem>(jobIndex, val, addHealthProblem);
				return addHealthProblem.m_Flags;
			}
			return HealthProblemFlags.None;
		}

		private bool IsSecured(InvolvedInAccident involvedInAccident)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Entity val = FindAccidentSite(involvedInAccident.m_Event);
			if (val != Entity.Null)
			{
				AccidentSite accidentSite = m_AccidentSiteData[val];
				if ((accidentSite.m_Flags & AccidentSiteFlags.Secured) == 0)
				{
					return m_SimulationFrame >= accidentSite.m_CreationFrame + 14400;
				}
				return true;
			}
			return true;
		}

		private void StopMoving(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Moving>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stopped>(jobIndex, entity, default(Stopped));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
		}

		private void StartMoving(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Stopped>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, entity, default(InterpolatedTransform));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Moving>(jobIndex, entity, default(Moving));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
		}

		private void StopStumbling(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Stumbling>(jobIndex, entity);
		}

		private void ClearAccident(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InvolvedInAccident>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Stumbling>(jobIndex, entity);
			m_IconCommandBuffer.Remove(entity, m_PoliceConfigurationData.m_TrafficAccidentNotificationPrefab);
		}

		private Entity FindAccidentSite(Entity _event)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (m_TargetElements.HasBuffer(_event))
			{
				DynamicBuffer<TargetElement> val = m_TargetElements[_event];
				for (int i = 0; i < val.Length; i++)
				{
					Entity entity = val[i].m_Entity;
					if (m_AccidentSiteData.HasComponent(entity))
					{
						return entity;
					}
				}
			}
			return Entity.Null;
		}

		private Entity FindSuitableAccidentSite(float3 position)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			float num = 30f;
			EdgeIterator edgeIterator = new EdgeIterator
			{
				m_Bounds = new Bounds3(position - num, position + num),
				m_Position = position,
				m_MaxDistance = num,
				m_AccidentSiteData = m_AccidentSiteData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_RoadData = m_RoadData,
				m_CurveData = m_CurveData
			};
			m_NetSearchTree.Iterate<EdgeIterator>(ref edgeIterator, 0);
			return edgeIterator.m_Result;
		}

		private void AddAccidentSite(int jobIndex, ref InvolvedInAccident involvedInAccident, Entity target)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			AddAccidentSite addAccidentSite = new AddAccidentSite
			{
				m_Event = involvedInAccident.m_Event,
				m_Target = target,
				m_Flags = AccidentSiteFlags.TrafficAccident
			};
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_AddAccidentSiteArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AddAccidentSite>(jobIndex, val, addAccidentSite);
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
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> __Game_Creatures_Stumbling_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> __Game_Vehicles_Hearse_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TargetElement> __Game_Events_TargetElement_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Resident>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InvolvedInAccident>(true);
			__Game_Creatures_Stumbling_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stumbling>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_Creature_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(false);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Vehicles_Hearse_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Hearse>(true);
			__Game_Vehicles_Ambulance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Ambulance>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Events_TargetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_CreatureQuery;

	private EntityQuery m_ConfigQuery;

	private EntityArchetype m_AddAccidentSiteArchetype;

	private EntityArchetype m_AddProblemArchetype;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<InvolvedInAccident>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_AddAccidentSiteArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddAccidentSite>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_AddProblemArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddHealthProblem>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatureQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		PoliceConfigurationData singleton = ((EntityQuery)(ref m_ConfigQuery)).GetSingleton<PoliceConfigurationData>();
		JobHandle dependencies;
		AccidentCreatureJob accidentCreatureJob = new AccidentCreatureJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentType = InternalCompilerInterface.GetComponentTypeHandle<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StumblingType = InternalCompilerInterface.GetComponentTypeHandle<Stumbling>(ref __TypeHandle.__Game_Creatures_Stumbling_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HearseData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Hearse>(ref __TypeHandle.__Game_Vehicles_Hearse_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AmbulanceData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_PoliceConfigurationData = singleton,
			m_AddAccidentSiteArchetype = m_AddAccidentSiteArchetype,
			m_AddProblemArchetype = m_AddProblemArchetype,
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		accidentCreatureJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		accidentCreatureJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<AccidentCreatureJob>(accidentCreatureJob, m_CreatureQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_NetSearchSystem.AddNetSearchTreeReader(val2);
		m_IconCommandSystem.AddCommandBufferWriter(val2);
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
	public AccidentCreatureSystem()
	{
	}
}
