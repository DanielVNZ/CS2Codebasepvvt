using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Game.Tools;
using Game.Tutorials;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ReplacePrefabSystem : GameSystemBase
{
	private struct ReplaceMesh
	{
		public Entity m_OldMesh;

		public Entity m_NewMesh;
	}

	public struct ReplacePrefabData
	{
		public Entity m_OldPrefab;

		public Entity m_SourceInstance;

		public bool m_AreasUpdated;

		public bool m_NetsUpdated;

		public bool m_LanesUpdated;
	}

	[CompilerGenerated]
	public class Finalize : GameSystemBase
	{
		public struct AreaKey : IEquatable<AreaKey>
		{
			public Entity m_Prefab;

			public float3 m_StartLocation;

			public int m_NodeCount;

			public bool Equals(AreaKey other)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (m_Prefab == other.m_Prefab && ((float3)(ref m_StartLocation)).Equals(other.m_StartLocation))
				{
					return m_NodeCount == other.m_NodeCount;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_StartLocation)/*cast due to .constrained prefix*/).GetHashCode() ^ m_NodeCount.GetHashCode();
			}
		}

		public struct NetKey : IEquatable<NetKey>
		{
			public Entity m_Prefab;

			public float3 m_StartLocation;

			public float3 m_EndLocation;

			public bool Equals(NetKey other)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				if (m_Prefab == other.m_Prefab && ((float3)(ref m_StartLocation)).Equals(other.m_StartLocation))
				{
					return ((float3)(ref m_EndLocation)).Equals(other.m_EndLocation);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_StartLocation)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_EndLocation)/*cast due to .constrained prefix*/).GetHashCode();
			}
		}

		[BurstCompile]
		private struct UpdateInstanceElementsJob : IJobParallelFor
		{
			[ReadOnly]
			public ComponentLookup<Deleted> m_DeletedData;

			[ReadOnly]
			public ComponentLookup<Owner> m_OwnerData;

			[ReadOnly]
			public ComponentLookup<Temp> m_TempData;

			[ReadOnly]
			public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

			[ReadOnly]
			public ComponentLookup<Transform> m_TransformData;

			[ReadOnly]
			public ComponentLookup<Edge> m_EdgeData;

			[ReadOnly]
			public ComponentLookup<PrefabRef> m_PrefabRefData;

			[ReadOnly]
			public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

			[ReadOnly]
			public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

			[ReadOnly]
			public BufferLookup<Game.Objects.SubObject> m_SubObjects;

			[ReadOnly]
			public BufferLookup<Game.Areas.SubArea> m_SubAreas;

			[ReadOnly]
			public BufferLookup<Game.Net.SubNet> m_SubNets;

			[ReadOnly]
			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			[ReadOnly]
			public BufferLookup<SubArea> m_PrefabSubAreas;

			[ReadOnly]
			public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

			[ReadOnly]
			public BufferLookup<SubNet> m_PrefabSubNets;

			[ReadOnly]
			public BufferLookup<SubLane> m_PrefabSubLanes;

			[ReadOnly]
			public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderElements;

			[ReadOnly]
			public bool m_EditorMode;

			[ReadOnly]
			public bool m_LefthandTraffic;

			[ReadOnly]
			public Entity m_LaneContainer;

			[ReadOnly]
			public RandomSeed m_RandomSeed;

			[ReadOnly]
			public NativeArray<Entity> m_Instances;

			[ReadOnly]
			public NativeHashMap<Entity, ReplacePrefabData> m_ReplacePrefabData;

			public ParallelWriter m_CommandBuffer;

			public void Execute(int index)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				Entity val = m_Instances[index];
				Random random = m_RandomSeed.GetRandom(index);
				NativeParallelHashMap<Entity, int> selectedSpawnables = default(NativeParallelHashMap<Entity, int>);
				PrefabRef prefabRef = default(PrefabRef);
				ReplacePrefabData replacePrefabData = default(ReplacePrefabData);
				Transform transform = default(Transform);
				if (!m_TempData.HasComponent(val) && m_PrefabRefData.TryGetComponent(val, ref prefabRef) && m_ReplacePrefabData.TryGetValue(prefabRef.m_Prefab, ref replacePrefabData) && replacePrefabData.m_SourceInstance != val && m_TransformData.TryGetComponent(val, ref transform))
				{
					bool flag = !m_OwnerData.HasComponent(val);
					bool flag2 = flag && m_EditorMode && replacePrefabData.m_LanesUpdated;
					if (replacePrefabData.m_AreasUpdated)
					{
						UpdateAreas(index, val, prefabRef.m_Prefab, transform, flag, ref random, ref selectedSpawnables);
					}
					if (replacePrefabData.m_NetsUpdated || flag2)
					{
						UpdateNets(index, val, prefabRef.m_Prefab, transform, flag, replacePrefabData.m_NetsUpdated, flag2, ref random);
					}
				}
				if (selectedSpawnables.IsCreated)
				{
					selectedSpawnables.Dispose();
				}
			}

			private void UpdateAreas(int jobIndex, Entity entity, Entity newPrefab, Transform transform, bool isTopLevel, ref Random random, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
				if (m_SubObjects.TryGetBuffer(entity, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						Entity subObject = val[i].m_SubObject;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, subObject, default(Updated));
					}
				}
				DynamicBuffer<Game.Areas.SubArea> val2 = default(DynamicBuffer<Game.Areas.SubArea>);
				if (m_SubAreas.TryGetBuffer(entity, ref val2))
				{
					for (int j = 0; j < val2.Length; j++)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, val2[j].m_Area);
					}
				}
				DynamicBuffer<SubArea> subAreas = default(DynamicBuffer<SubArea>);
				if (m_PrefabSubAreas.TryGetBuffer(newPrefab, ref subAreas))
				{
					if (!val2.IsCreated)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Areas.SubArea>(jobIndex, entity);
					}
					if (selectedSpawnables.IsCreated)
					{
						selectedSpawnables.Clear();
					}
					CreateAreas(jobIndex, entity, transform, subAreas, m_PrefabSubAreaNodes[newPrefab], ref random, ref selectedSpawnables);
				}
				else if ((!m_EditorMode || !isTopLevel) && val2.IsCreated)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Areas.SubArea>(jobIndex, entity);
				}
			}

			private void UpdateNets(int jobIndex, Entity entity, Entity newPrefab, Transform transform, bool isTopLevel, bool updateNets, bool updateLanes, ref Random random)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_029f: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_023d: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0243: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0305: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<Game.Net.SubNet> val = default(DynamicBuffer<Game.Net.SubNet>);
				if (m_SubNets.TryGetBuffer(entity, ref val))
				{
					DynamicBuffer<ConnectedEdge> val2 = default(DynamicBuffer<ConnectedEdge>);
					Owner owner = default(Owner);
					for (int i = 0; i < val.Length; i++)
					{
						Game.Net.SubNet subNet = val[i];
						if (m_EditorContainerData.HasComponent(subNet.m_SubNet))
						{
							if (!updateLanes)
							{
								continue;
							}
						}
						else if (!updateNets)
						{
							continue;
						}
						bool flag = true;
						if (m_ConnectedEdges.TryGetBuffer(subNet.m_SubNet, ref val2))
						{
							for (int j = 0; j < val2.Length; j++)
							{
								Entity edge = val2[j].m_Edge;
								if ((!m_OwnerData.TryGetComponent(edge, ref owner) || (!(owner.m_Owner == entity) && !m_DeletedData.HasComponent(owner.m_Owner))) && !m_DeletedData.HasComponent(edge))
								{
									Edge edge2 = m_EdgeData[edge];
									if (edge2.m_Start == subNet.m_SubNet || edge2.m_End == subNet.m_SubNet)
									{
										flag = false;
									}
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, edge, default(Updated));
									if (!m_DeletedData.HasComponent(edge2.m_Start))
									{
										((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, edge2.m_Start, default(Updated));
									}
									if (!m_DeletedData.HasComponent(edge2.m_End))
									{
										((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, edge2.m_End, default(Updated));
									}
								}
							}
						}
						if (flag)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, subNet.m_SubNet);
							continue;
						}
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Owner>(jobIndex, subNet.m_SubNet);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, subNet.m_SubNet, default(Updated));
					}
				}
				DynamicBuffer<SubNet> subNets = default(DynamicBuffer<SubNet>);
				m_PrefabSubNets.TryGetBuffer(newPrefab, ref subNets);
				DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
				m_PrefabSubLanes.TryGetBuffer(newPrefab, ref subLanes);
				if (subNets.IsCreated || (m_EditorMode && isTopLevel && subLanes.IsCreated))
				{
					if (val.IsCreated)
					{
						DynamicBuffer<Game.Net.SubNet> val3 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Game.Net.SubNet>(jobIndex, entity);
						if (!updateNets || !updateLanes)
						{
							for (int k = 0; k < val.Length; k++)
							{
								Game.Net.SubNet subNet2 = val[k];
								if (m_EditorContainerData.HasComponent(subNet2.m_SubNet))
								{
									if (updateLanes)
									{
										continue;
									}
								}
								else if (updateNets)
								{
									continue;
								}
								val3.Add(subNet2);
							}
						}
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Net.SubNet>(jobIndex, entity);
					}
					if (updateNets && subNets.IsCreated)
					{
						CreateNets(jobIndex, entity, transform, subNets, ref random);
					}
					if (updateLanes && subLanes.IsCreated)
					{
						CreateLanes(jobIndex, entity, transform, subLanes, ref random);
					}
				}
				else if (val.IsCreated)
				{
					if (m_EditorMode && isTopLevel)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Game.Net.SubNet>(jobIndex, entity);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Net.SubNet>(jobIndex, entity);
					}
				}
			}

			private void CreateAreas(int jobIndex, Entity owner, Transform transform, DynamicBuffer<SubArea> subAreas, DynamicBuffer<SubAreaNode> subAreaNodes, ref Random random, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0214: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<PlaceholderObjectElement> placeholderElements = default(DynamicBuffer<PlaceholderObjectElement>);
				for (int i = 0; i < subAreas.Length; i++)
				{
					SubArea subArea = subAreas[i];
					int seed;
					if (!m_EditorMode && m_PrefabPlaceholderElements.TryGetBuffer(subArea.m_Prefab, ref placeholderElements))
					{
						if (!selectedSpawnables.IsCreated)
						{
							selectedSpawnables = new NativeParallelHashMap<Entity, int>(10, AllocatorHandle.op_Implicit((Allocator)2));
						}
						if (!AreaUtils.SelectAreaPrefab(placeholderElements, m_PrefabSpawnableObjectData, selectedSpawnables, ref random, out subArea.m_Prefab, out seed))
						{
							continue;
						}
					}
					else
					{
						seed = ((Random)(ref random)).NextInt();
					}
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Prefab = subArea.m_Prefab,
						m_Owner = owner,
						m_RandomSeed = seed
					};
					creationDefinition.m_Flags |= CreationFlags.Permanent;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
					DynamicBuffer<Game.Areas.Node> val2 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(jobIndex, val);
					val2.ResizeUninitialized(subArea.m_NodeRange.y - subArea.m_NodeRange.x + 1);
					DynamicBuffer<LocalNodeCache> val3 = default(DynamicBuffer<LocalNodeCache>);
					if (m_EditorMode)
					{
						val3 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(jobIndex, val);
						val3.ResizeUninitialized(val2.Length);
					}
					int num = ObjectToolBaseSystem.GetFirstNodeIndex(subAreaNodes, subArea.m_NodeRange);
					int num2 = 0;
					for (int j = subArea.m_NodeRange.x; j <= subArea.m_NodeRange.y; j++)
					{
						float3 position = subAreaNodes[num].m_Position;
						float3 position2 = ObjectUtils.LocalToWorld(transform, position);
						int parentMesh = subAreaNodes[num].m_ParentMesh;
						float elevation = math.select(float.MinValue, position.y, parentMesh >= 0);
						val2[num2] = new Game.Areas.Node(position2, elevation);
						if (m_EditorMode)
						{
							val3[num2] = new LocalNodeCache
							{
								m_Position = position,
								m_ParentMesh = parentMesh
							};
						}
						num2++;
						if (++num == subArea.m_NodeRange.y)
						{
							num = subArea.m_NodeRange.x;
						}
					}
				}
			}

			private void CreateNets(int jobIndex, Entity owner, Transform transform, DynamicBuffer<SubNet> subNets, ref Random random)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
				NativeList<float4> nodePositions = default(NativeList<float4>);
				nodePositions._002Ector(subNets.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < subNets.Length; i++)
				{
					SubNet subNet = subNets[i];
					if (subNet.m_NodeIndex.x >= 0)
					{
						while (nodePositions.Length <= subNet.m_NodeIndex.x)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subNet.m_NodeIndex.x;
						reference[x] += new float4(subNet.m_Curve.a, 1f);
					}
					if (subNet.m_NodeIndex.y >= 0)
					{
						while (nodePositions.Length <= subNet.m_NodeIndex.y)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subNet.m_NodeIndex.y;
						reference[x] += new float4(subNet.m_Curve.d, 1f);
					}
				}
				for (int j = 0; j < nodePositions.Length; j++)
				{
					ref NativeList<float4> reference = ref nodePositions;
					int x = j;
					reference[x] /= math.max(1f, nodePositions[j].w);
				}
				for (int k = 0; k < subNets.Length; k++)
				{
					SubNet subNet2 = NetUtils.GetSubNet(subNets, k, m_LefthandTraffic, ref m_PrefabNetGeometryData);
					CreateSubNet(jobIndex, subNet2.m_Prefab, Entity.Null, subNet2.m_Curve, subNet2.m_NodeIndex, subNet2.m_ParentMesh, subNet2.m_Upgrades, nodePositions, owner, transform, ref random);
				}
				nodePositions.Dispose();
			}

			private void CreateLanes(int jobIndex, Entity owner, Transform transform, DynamicBuffer<SubLane> subLanes, ref Random random)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				NativeList<float4> nodePositions = default(NativeList<float4>);
				nodePositions._002Ector(subLanes.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < subLanes.Length; i++)
				{
					SubLane subLane = subLanes[i];
					if (subLane.m_NodeIndex.x >= 0)
					{
						while (nodePositions.Length <= subLane.m_NodeIndex.x)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subLane.m_NodeIndex.x;
						reference[x] += new float4(subLane.m_Curve.a, 1f);
					}
					if (subLane.m_NodeIndex.y >= 0)
					{
						while (nodePositions.Length <= subLane.m_NodeIndex.y)
						{
							float4 val = default(float4);
							nodePositions.Add(ref val);
						}
						ref NativeList<float4> reference = ref nodePositions;
						int x = subLane.m_NodeIndex.y;
						reference[x] += new float4(subLane.m_Curve.d, 1f);
					}
				}
				for (int j = 0; j < nodePositions.Length; j++)
				{
					ref NativeList<float4> reference = ref nodePositions;
					int x = j;
					reference[x] /= math.max(1f, nodePositions[j].w);
				}
				for (int k = 0; k < subLanes.Length; k++)
				{
					SubLane subLane2 = subLanes[k];
					CreateSubNet(jobIndex, m_LaneContainer, subLane2.m_Prefab, subLane2.m_Curve, subLane2.m_NodeIndex, subLane2.m_ParentMesh, default(CompositionFlags), nodePositions, owner, transform, ref random);
				}
				nodePositions.Dispose();
			}

			private void CreateSubNet(int jobIndex, Entity netPrefab, Entity lanePrefab, Bezier4x3 curve, int2 nodeIndex, int2 parentMesh, CompositionFlags upgrades, NativeList<float4> nodePositions, Entity owner, Transform transform, ref Random random)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_025c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0291: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = netPrefab,
					m_SubPrefab = lanePrefab,
					m_Owner = owner,
					m_RandomSeed = ((Random)(ref random)).NextInt()
				};
				creationDefinition.m_Flags |= CreationFlags.Permanent;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
				NetCourse netCourse = default(NetCourse);
				netCourse.m_Curve = ObjectUtils.LocalToWorld(transform.m_Position, transform.m_Rotation, curve);
				netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
				netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve), transform.m_Rotation);
				netCourse.m_StartPosition.m_CourseDelta = 0f;
				netCourse.m_StartPosition.m_Elevation = float2.op_Implicit(curve.a.y);
				netCourse.m_StartPosition.m_ParentMesh = parentMesh.x;
				float4 val2;
				if (nodeIndex.x >= 0)
				{
					ref CoursePos startPosition = ref netCourse.m_StartPosition;
					float3 position = transform.m_Position;
					quaternion rotation = transform.m_Rotation;
					val2 = nodePositions[nodeIndex.x];
					startPosition.m_Position = ObjectUtils.LocalToWorld(position, rotation, ((float4)(ref val2)).xyz);
				}
				netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
				netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve), transform.m_Rotation);
				netCourse.m_EndPosition.m_CourseDelta = 1f;
				netCourse.m_EndPosition.m_Elevation = float2.op_Implicit(curve.d.y);
				netCourse.m_EndPosition.m_ParentMesh = parentMesh.y;
				if (nodeIndex.y >= 0)
				{
					ref CoursePos endPosition = ref netCourse.m_EndPosition;
					float3 position2 = transform.m_Position;
					quaternion rotation2 = transform.m_Rotation;
					val2 = nodePositions[nodeIndex.y];
					endPosition.m_Position = ObjectUtils.LocalToWorld(position2, rotation2, ((float4)(ref val2)).xyz);
				}
				netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
				netCourse.m_FixedIndex = -1;
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst | CoursePosFlags.DisableMerge;
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast | CoursePosFlags.DisableMerge;
				if (((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
				{
					netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
					netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<NetCourse>(jobIndex, val, netCourse);
				if (upgrades != default(CompositionFlags))
				{
					Upgraded upgraded = new Upgraded
					{
						m_Flags = upgrades
					};
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, val, upgraded);
				}
				if (m_EditorMode)
				{
					LocalCurveCache localCurveCache = new LocalCurveCache
					{
						m_Curve = curve
					};
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(jobIndex, val, localCurveCache);
				}
			}
		}

		[BurstCompile]
		private struct CheckPrefabReplacesJob : IJob
		{
			[ReadOnly]
			public BufferLookup<SubArea> m_PrefabSubAreas;

			[ReadOnly]
			public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

			[ReadOnly]
			public BufferLookup<SubNet> m_PrefabSubNets;

			[ReadOnly]
			public BufferLookup<SubLane> m_PrefabSubLanes;

			public NativeHashMap<Entity, ReplacePrefabData> m_ReplacePrefabData;

			public void Execute()
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				NativeArray<Entity> keyArray = m_ReplacePrefabData.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < keyArray.Length; i++)
				{
					Entity val = keyArray[i];
					ReplacePrefabData replacePrefabData = m_ReplacePrefabData[val];
					replacePrefabData.m_AreasUpdated = CompareAreas(val, replacePrefabData.m_OldPrefab);
					replacePrefabData.m_NetsUpdated = CompareNets(val, replacePrefabData.m_OldPrefab);
					replacePrefabData.m_LanesUpdated = CompareLanes(val, replacePrefabData.m_OldPrefab);
					m_ReplacePrefabData[val] = replacePrefabData;
				}
				keyArray.Dispose();
			}

			private bool CompareAreas(Entity newPrefab, Entity oldPrefab)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0242: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<SubArea> val = default(DynamicBuffer<SubArea>);
				m_PrefabSubAreas.TryGetBuffer(newPrefab, ref val);
				DynamicBuffer<SubArea> val2 = default(DynamicBuffer<SubArea>);
				m_PrefabSubAreas.TryGetBuffer(oldPrefab, ref val2);
				if (!val.IsCreated || !val2.IsCreated)
				{
					if (!val.IsCreated)
					{
						return val2.IsCreated;
					}
					return true;
				}
				if (val.Length != val2.Length)
				{
					return true;
				}
				if (val.Length == 0)
				{
					return false;
				}
				DynamicBuffer<SubAreaNode> val3 = default(DynamicBuffer<SubAreaNode>);
				m_PrefabSubAreaNodes.TryGetBuffer(newPrefab, ref val3);
				DynamicBuffer<SubAreaNode> val4 = default(DynamicBuffer<SubAreaNode>);
				m_PrefabSubAreaNodes.TryGetBuffer(oldPrefab, ref val4);
				if (val3.Length != val4.Length)
				{
					return true;
				}
				NativeParallelMultiHashMap<AreaKey, int> val5 = default(NativeParallelMultiHashMap<AreaKey, int>);
				val5._002Ector(val2.Length, AllocatorHandle.op_Implicit((Allocator)2));
				bool result = false;
				for (int i = 0; i < val2.Length; i++)
				{
					SubArea subArea = val2[i];
					int num = subArea.m_NodeRange.y - subArea.m_NodeRange.x;
					AreaKey areaKey = new AreaKey
					{
						m_Prefab = subArea.m_Prefab,
						m_NodeCount = num
					};
					if (num != 0)
					{
						areaKey.m_StartLocation = val4[subArea.m_NodeRange.x].m_Position;
					}
					val5.Add(areaKey, i);
				}
				int num3 = default(int);
				NativeParallelMultiHashMapIterator<AreaKey> val6 = default(NativeParallelMultiHashMapIterator<AreaKey>);
				for (int j = 0; j < val.Length; j++)
				{
					SubArea subArea2 = val[j];
					int num2 = subArea2.m_NodeRange.y - subArea2.m_NodeRange.x;
					AreaKey areaKey2 = new AreaKey
					{
						m_Prefab = subArea2.m_Prefab,
						m_NodeCount = num2
					};
					if (num2 != 0)
					{
						areaKey2.m_StartLocation = val3[subArea2.m_NodeRange.x].m_Position;
					}
					bool flag = false;
					if (val5.TryGetFirstValue(areaKey2, ref num3, ref val6))
					{
						do
						{
							flag = true;
							SubArea subArea3 = val2[num3];
							for (int k = 0; k < num2; k++)
							{
								SubAreaNode subAreaNode = val3[subArea2.m_NodeRange.x + k];
								SubAreaNode subAreaNode2 = val4[subArea3.m_NodeRange.x + k];
								flag &= ((float3)(ref subAreaNode.m_Position)).Equals(subAreaNode2.m_Position);
								flag &= subAreaNode.m_ParentMesh == subAreaNode2.m_ParentMesh;
							}
							if (flag)
							{
								val5.Remove(val6);
								break;
							}
						}
						while (val5.TryGetNextValue(ref num3, ref val6));
					}
					if (!flag)
					{
						result = true;
						break;
					}
				}
				val5.Dispose();
				return result;
			}

			private bool CompareNets(Entity newPrefab, Entity oldPrefab)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<SubNet> val = default(DynamicBuffer<SubNet>);
				m_PrefabSubNets.TryGetBuffer(newPrefab, ref val);
				DynamicBuffer<SubNet> val2 = default(DynamicBuffer<SubNet>);
				m_PrefabSubNets.TryGetBuffer(oldPrefab, ref val2);
				if (!val.IsCreated || !val2.IsCreated)
				{
					if (!val.IsCreated)
					{
						return val2.IsCreated;
					}
					return true;
				}
				if (val.Length != val2.Length)
				{
					return true;
				}
				if (val.Length == 0)
				{
					return false;
				}
				NativeParallelMultiHashMap<NetKey, int> val3 = default(NativeParallelMultiHashMap<NetKey, int>);
				val3._002Ector(val2.Length, AllocatorHandle.op_Implicit((Allocator)2));
				bool result = false;
				for (int i = 0; i < val2.Length; i++)
				{
					SubNet subNet = val2[i];
					NetKey netKey = new NetKey
					{
						m_Prefab = subNet.m_Prefab,
						m_StartLocation = subNet.m_Curve.a,
						m_EndLocation = subNet.m_Curve.d
					};
					val3.Add(netKey, i);
				}
				int num = default(int);
				NativeParallelMultiHashMapIterator<NetKey> val4 = default(NativeParallelMultiHashMapIterator<NetKey>);
				for (int j = 0; j < val.Length; j++)
				{
					SubNet subNet2 = val[j];
					NetKey netKey2 = new NetKey
					{
						m_Prefab = subNet2.m_Prefab,
						m_StartLocation = subNet2.m_Curve.a,
						m_EndLocation = subNet2.m_Curve.d
					};
					bool flag = false;
					if (val3.TryGetFirstValue(netKey2, ref num, ref val4))
					{
						do
						{
							SubNet subNet3 = val2[num];
							flag = subNet2.m_Prefab == subNet3.m_Prefab && ((Bezier4x3)(ref subNet2.m_Curve)).Equals(subNet3.m_Curve) && ((int2)(ref subNet2.m_NodeIndex)).Equals(subNet3.m_NodeIndex) && ((int2)(ref subNet2.m_ParentMesh)).Equals(subNet3.m_ParentMesh) && subNet2.m_InvertMode == subNet3.m_InvertMode && subNet2.m_Upgrades == subNet3.m_Upgrades;
							if (flag)
							{
								val3.Remove(val4);
								break;
							}
						}
						while (val3.TryGetNextValue(ref num, ref val4));
					}
					if (!flag)
					{
						result = true;
						break;
					}
				}
				val3.Dispose();
				return result;
			}

			private bool CompareLanes(Entity newPrefab, Entity oldPrefab)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_019a: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
				m_PrefabSubLanes.TryGetBuffer(newPrefab, ref val);
				DynamicBuffer<SubLane> val2 = default(DynamicBuffer<SubLane>);
				m_PrefabSubLanes.TryGetBuffer(oldPrefab, ref val2);
				if (!val.IsCreated || !val2.IsCreated)
				{
					if (!val.IsCreated)
					{
						return val2.IsCreated;
					}
					return true;
				}
				if (val.Length != val2.Length)
				{
					return true;
				}
				if (val.Length == 0)
				{
					return false;
				}
				NativeParallelMultiHashMap<NetKey, int> val3 = default(NativeParallelMultiHashMap<NetKey, int>);
				val3._002Ector(val2.Length, AllocatorHandle.op_Implicit((Allocator)2));
				bool result = false;
				for (int i = 0; i < val2.Length; i++)
				{
					SubLane subLane = val2[i];
					NetKey netKey = new NetKey
					{
						m_Prefab = subLane.m_Prefab,
						m_StartLocation = subLane.m_Curve.a,
						m_EndLocation = subLane.m_Curve.d
					};
					val3.Add(netKey, i);
				}
				int num = default(int);
				NativeParallelMultiHashMapIterator<NetKey> val4 = default(NativeParallelMultiHashMapIterator<NetKey>);
				for (int j = 0; j < val.Length; j++)
				{
					SubLane subLane2 = val[j];
					NetKey netKey2 = new NetKey
					{
						m_Prefab = subLane2.m_Prefab,
						m_StartLocation = subLane2.m_Curve.a,
						m_EndLocation = subLane2.m_Curve.d
					};
					bool flag = false;
					if (val3.TryGetFirstValue(netKey2, ref num, ref val4))
					{
						do
						{
							SubLane subLane3 = val2[num];
							flag = subLane2.m_Prefab == subLane3.m_Prefab && ((Bezier4x3)(ref subLane2.m_Curve)).Equals(subLane3.m_Curve) && ((int2)(ref subLane2.m_NodeIndex)).Equals(subLane3.m_NodeIndex) && ((int2)(ref subLane2.m_ParentMesh)).Equals(subLane3.m_ParentMesh);
							if (flag)
							{
								val3.Remove(val4);
								break;
							}
						}
						while (val3.TryGetNextValue(ref num, ref val4));
					}
					if (!flag)
					{
						result = true;
						break;
					}
				}
				val3.Dispose();
				return result;
			}
		}

		private struct TypeHandle
		{
			[ReadOnly]
			public BufferLookup<SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<SubLane> __Game_Prefabs_SubLane_RO_BufferLookup;

			[ReadOnly]
			public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

			[ReadOnly]
			public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

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
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubArea>(true);
				__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
				__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubNet>(true);
				__Game_Prefabs_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
				__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
				__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
				__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
				__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Tools.EditorContainer>(true);
				__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
				__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
				__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
				__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
				__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
				__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
				__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
				__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
				__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
				__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			}
		}

		private ReplacePrefabSystem m_ReplacePrefabSystem;

		private EntityQuery m_LaneContainerQuery;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_ReplacePrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ReplacePrefabSystem>();
			m_LaneContainerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<EditorContainerData>(),
				ComponentType.ReadOnly<NetData>()
			});
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> instances = m_ReplacePrefabSystem.m_UpdateInstances.ToArray(AllocatorHandle.op_Implicit((Allocator)3));
			if (m_ReplacePrefabSystem.m_MeshReplaces.Length != 0)
			{
				Enumerator<ReplaceMesh> enumerator = m_ReplacePrefabSystem.m_MeshReplaces.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ReplaceMesh current = enumerator.Current;
						m_ReplacePrefabSystem.m_ManagedBatchSystem.RemoveMesh(current.m_OldMesh, current.m_NewMesh);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				m_ReplacePrefabSystem.m_MeshReplaces.Clear();
				m_ReplacePrefabSystem.m_ManagedBatchSystem.ResetSharedMeshes();
				HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
				HashSet<ComponentType> archetypeComponents = new HashSet<ComponentType>();
				hashSet.Add(ComponentType.ReadWrite<Stack>());
				hashSet.Add(ComponentType.ReadWrite<MeshColor>());
				hashSet.Add(ComponentType.ReadWrite<MeshGroup>());
				Entity instance = default(Entity);
				while (m_ReplacePrefabSystem.m_UpdateInstances.TryDequeue(ref instance))
				{
					m_ReplacePrefabSystem.CheckInstanceComponents(instance, hashSet, archetypeComponents);
				}
			}
			if (m_ReplacePrefabSystem.m_ReplacePrefabData.Count != 0)
			{
				EntityCommandBuffer val = default(EntityCommandBuffer);
				((EntityCommandBuffer)(ref val))._002Ector((Allocator)3, (PlaybackPolicy)0);
				CheckPrefabReplacesJob checkPrefabReplacesJob = new CheckPrefabReplacesJob
				{
					m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ReplacePrefabData = m_ReplacePrefabSystem.m_ReplacePrefabData
				};
				UpdateInstanceElementsJob updateInstanceElementsJob = new UpdateInstanceElementsJob
				{
					m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabPlaceholderElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EditorMode = m_ReplacePrefabSystem.m_ToolSystem.actionMode.IsEditor(),
					m_LefthandTraffic = m_ReplacePrefabSystem.m_CityConfigurationSystem.leftHandTraffic,
					m_RandomSeed = RandomSeed.Next(),
					m_Instances = instances,
					m_ReplacePrefabData = m_ReplacePrefabSystem.m_ReplacePrefabData,
					m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter()
				};
				NativeArray<Entity> val2 = ((EntityQuery)(ref m_LaneContainerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < val2.Length; i++)
				{
					Entity val3 = val2[i];
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<NetData>(val3))
					{
						updateInstanceElementsJob.m_LaneContainer = val3;
					}
				}
				val2.Dispose();
				JobHandle val4 = IJobExtensions.Schedule<CheckPrefabReplacesJob>(checkPrefabReplacesJob, ((SystemBase)this).Dependency);
				JobHandle val5 = IJobParallelForExtensions.Schedule<UpdateInstanceElementsJob>(updateInstanceElementsJob, instances.Length, 1, val4);
				((JobHandle)(ref val5)).Complete();
				((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
				((EntityCommandBuffer)(ref val)).Dispose();
			}
			instances.Dispose();
			m_ReplacePrefabSystem.m_UpdateInstances.Clear();
			m_ReplacePrefabSystem.m_ReplacePrefabData.Clear();
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
		public Finalize()
		{
		}
	}

	[BurstCompile]
	private struct RemoveBatchGroupsJob : IJob
	{
		[ReadOnly]
		public Entity m_OldMeshEntity;

		[ReadOnly]
		public Entity m_NewMeshEntity;

		public BufferLookup<MeshBatch> m_MeshBatches;

		public BufferLookup<FadeBatch> m_FadeBatches;

		public BufferLookup<BatchGroup> m_BatchGroups;

		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

		public EntityCommandBuffer m_EntityCommandBuffer;

		public NativeList<ReplaceMesh> m_ReplaceMeshes;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<BatchGroup> val = m_BatchGroups[m_OldMeshEntity];
			NativeHashSet<Entity> updateBatches = default(NativeHashSet<Entity>);
			updateBatches._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < val.Length; i++)
			{
				RemoveBatchGroup(val[i].m_GroupIndex, val[i].m_MergeIndex, updateBatches);
			}
			val.Clear();
			ref NativeList<ReplaceMesh> reference = ref m_ReplaceMeshes;
			ReplaceMesh replaceMesh = new ReplaceMesh
			{
				m_OldMesh = m_OldMeshEntity,
				m_NewMesh = m_NewMeshEntity
			};
			reference.Add(ref replaceMesh);
			Enumerator<Entity> enumerator = updateBatches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((EntityCommandBuffer)(ref m_EntityCommandBuffer)).AddComponent<BatchesUpdated>(enumerator.Current, default(BatchesUpdated));
			}
			enumerator.Dispose();
			updateBatches.Dispose();
		}

		private void RemoveBatchGroup(int groupIndex, int mergeIndex, NativeHashSet<Entity> updateBatches)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			int num = groupIndex;
			if (mergeIndex != -1)
			{
				num = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, mergeIndex);
				m_NativeBatchGroups.RemoveMergedGroup(groupIndex, mergeIndex);
			}
			else
			{
				int mergedGroupCount = m_NativeBatchGroups.GetMergedGroupCount(groupIndex);
				if (mergedGroupCount != 0)
				{
					int mergedGroupIndex = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, 0);
					GroupData groupData = m_NativeBatchGroups.GetGroupData(mergedGroupIndex);
					DynamicBuffer<BatchGroup> val = m_BatchGroups[groupData.m_Mesh];
					for (int i = 0; i < val.Length; i++)
					{
						BatchGroup batchGroup = val[i];
						if (batchGroup.m_GroupIndex == mergedGroupIndex)
						{
							batchGroup.m_MergeIndex = -1;
							val[i] = batchGroup;
							break;
						}
					}
					for (int j = 1; j < mergedGroupCount; j++)
					{
						int mergedGroupIndex2 = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, j);
						groupData = m_NativeBatchGroups.GetGroupData(mergedGroupIndex2);
						val = m_BatchGroups[groupData.m_Mesh];
						m_NativeBatchGroups.AddMergedGroup(mergedGroupIndex, mergedGroupIndex2);
						for (int k = 0; k < val.Length; k++)
						{
							BatchGroup batchGroup2 = val[k];
							if (batchGroup2.m_GroupIndex == mergedGroupIndex2)
							{
								batchGroup2.m_MergeIndex = mergedGroupIndex;
								val[j] = batchGroup2;
								break;
							}
						}
					}
				}
			}
			int instanceCount = m_NativeBatchInstances.GetInstanceCount(groupIndex);
			DynamicBuffer<MeshBatch> val2 = default(DynamicBuffer<MeshBatch>);
			DynamicBuffer<FadeBatch> val3 = default(DynamicBuffer<FadeBatch>);
			for (int l = 0; l < instanceCount; l++)
			{
				InstanceData instanceData = m_NativeBatchInstances.GetInstanceData(groupIndex, l);
				if (!m_MeshBatches.TryGetBuffer(instanceData.m_Entity, ref val2))
				{
					continue;
				}
				for (int m = 0; m < val2.Length; m++)
				{
					MeshBatch meshBatch = val2[m];
					if (meshBatch.m_GroupIndex == groupIndex && meshBatch.m_InstanceIndex == l)
					{
						if (m_FadeBatches.TryGetBuffer(instanceData.m_Entity, ref val3))
						{
							val2.RemoveAtSwapBack(m);
							val3.RemoveAtSwapBack(m);
							break;
						}
						meshBatch.m_GroupIndex = -1;
						meshBatch.m_InstanceIndex = -1;
						val2[m] = meshBatch;
						updateBatches.Add(instanceData.m_Entity);
						break;
					}
				}
			}
			m_NativeBatchInstances.RemoveInstances(groupIndex, m_NativeSubBatches);
			m_NativeBatchGroups.DestroyGroup(num, m_NativeBatchInstances, m_NativeSubBatches);
		}
	}

	[BurstCompile]
	private struct ReplacePrefabJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_OldPrefab;

		[ReadOnly]
		public Entity m_NewPrefab;

		[ReadOnly]
		public Entity m_SourceInstance;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Game.Tools.EditorContainer> m_EditorContainerType;

		public BufferTypeHandle<SubObject> m_SubObjectType;

		public BufferTypeHandle<SubNet> m_SubNetType;

		public BufferTypeHandle<SubArea> m_SubAreaType;

		public BufferTypeHandle<PlaceholderObjectElement> m_PlaceholderObjectElementType;

		public BufferTypeHandle<ServiceUpgradeBuilding> m_ServiceUpgradeBuildingType;

		public BufferTypeHandle<BuildingUpgradeElement> m_BuildingUpgradeElementType;

		public BufferTypeHandle<Effect> m_EffectType;

		public BufferTypeHandle<ActivityLocationElement> m_ActivityLocationElementType;

		public BufferTypeHandle<SubMesh> m_SubMeshType;

		public BufferTypeHandle<LodMesh> m_LodMeshType;

		public BufferTypeHandle<UIGroupElement> m_UIGroupElementType;

		public BufferTypeHandle<TutorialPhaseRef> m_TutorialPhaseType;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Entity> m_UpdateInstances;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			if (nativeArray2.Length != 0)
			{
				NativeArray<Game.Tools.EditorContainer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.EditorContainer>(ref m_EditorContainerType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					PrefabRef prefabRef = nativeArray2[i];
					if (prefabRef.m_Prefab == m_OldPrefab)
					{
						prefabRef.m_Prefab = m_NewPrefab;
						nativeArray2[i] = prefabRef;
						Entity val = nativeArray[i];
						if (m_SourceInstance != val)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val, default(Updated));
							m_UpdateInstances.Enqueue(val);
						}
					}
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					Game.Tools.EditorContainer editorContainer = nativeArray3[j];
					if (editorContainer.m_Prefab == m_OldPrefab)
					{
						editorContainer.m_Prefab = m_NewPrefab;
						nativeArray3[j] = editorContainer;
					}
				}
				return;
			}
			BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			BufferAccessor<SubNet> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubNet>(ref m_SubNetType);
			BufferAccessor<SubArea> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubArea>(ref m_SubAreaType);
			BufferAccessor<PlaceholderObjectElement> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceholderObjectElement>(ref m_PlaceholderObjectElementType);
			BufferAccessor<ServiceUpgradeBuilding> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceUpgradeBuilding>(ref m_ServiceUpgradeBuildingType);
			BufferAccessor<BuildingUpgradeElement> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BuildingUpgradeElement>(ref m_BuildingUpgradeElementType);
			BufferAccessor<Effect> bufferAccessor7 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Effect>(ref m_EffectType);
			BufferAccessor<ActivityLocationElement> bufferAccessor8 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ActivityLocationElement>(ref m_ActivityLocationElementType);
			BufferAccessor<SubMesh> bufferAccessor9 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubMesh>(ref m_SubMeshType);
			BufferAccessor<LodMesh> bufferAccessor10 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LodMesh>(ref m_LodMeshType);
			BufferAccessor<UIGroupElement> bufferAccessor11 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<UIGroupElement>(ref m_UIGroupElementType);
			BufferAccessor<TutorialPhaseRef> bufferAccessor12 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TutorialPhaseRef>(ref m_TutorialPhaseType);
			for (int k = 0; k < bufferAccessor.Length; k++)
			{
				DynamicBuffer<SubObject> val2 = bufferAccessor[k];
				for (int l = 0; l < val2.Length; l++)
				{
					SubObject subObject = val2[l];
					if (subObject.m_Prefab == m_OldPrefab)
					{
						subObject.m_Prefab = m_NewPrefab;
						val2[l] = subObject;
					}
				}
			}
			for (int m = 0; m < bufferAccessor2.Length; m++)
			{
				DynamicBuffer<SubNet> val3 = bufferAccessor2[m];
				for (int n = 0; n < val3.Length; n++)
				{
					SubNet subNet = val3[n];
					if (subNet.m_Prefab == m_OldPrefab)
					{
						subNet.m_Prefab = m_NewPrefab;
						val3[n] = subNet;
					}
				}
			}
			for (int num = 0; num < bufferAccessor3.Length; num++)
			{
				DynamicBuffer<SubArea> val4 = bufferAccessor3[num];
				for (int num2 = 0; num2 < val4.Length; num2++)
				{
					SubArea subArea = val4[num2];
					if (subArea.m_Prefab == m_OldPrefab)
					{
						subArea.m_Prefab = m_NewPrefab;
						val4[num2] = subArea;
					}
				}
			}
			for (int num3 = 0; num3 < bufferAccessor4.Length; num3++)
			{
				DynamicBuffer<PlaceholderObjectElement> val5 = bufferAccessor4[num3];
				for (int num4 = 0; num4 < val5.Length; num4++)
				{
					if (val5[num4].m_Object == m_OldPrefab)
					{
						val5.RemoveAtSwapBack(num4);
						num4--;
					}
				}
			}
			for (int num5 = 0; num5 < bufferAccessor5.Length; num5++)
			{
				DynamicBuffer<ServiceUpgradeBuilding> val6 = bufferAccessor5[num5];
				for (int num6 = 0; num6 < val6.Length; num6++)
				{
					ServiceUpgradeBuilding serviceUpgradeBuilding = val6[num6];
					if (serviceUpgradeBuilding.m_Building == m_OldPrefab)
					{
						serviceUpgradeBuilding.m_Building = m_NewPrefab;
						val6[num6] = serviceUpgradeBuilding;
					}
				}
			}
			for (int num7 = 0; num7 < bufferAccessor6.Length; num7++)
			{
				DynamicBuffer<BuildingUpgradeElement> val7 = bufferAccessor6[num7];
				for (int num8 = 0; num8 < val7.Length; num8++)
				{
					if (val7[num8].m_Upgrade == m_OldPrefab)
					{
						val7.RemoveAtSwapBack(num8);
						num8--;
					}
				}
			}
			for (int num9 = 0; num9 < bufferAccessor7.Length; num9++)
			{
				DynamicBuffer<Effect> val8 = bufferAccessor7[num9];
				for (int num10 = 0; num10 < val8.Length; num10++)
				{
					Effect effect = val8[num10];
					if (effect.m_Effect == m_OldPrefab)
					{
						effect.m_Effect = m_NewPrefab;
						val8[num10] = effect;
					}
				}
			}
			for (int num11 = 0; num11 < bufferAccessor8.Length; num11++)
			{
				DynamicBuffer<ActivityLocationElement> val9 = bufferAccessor8[num11];
				for (int num12 = 0; num12 < val9.Length; num12++)
				{
					ActivityLocationElement activityLocationElement = val9[num12];
					if (activityLocationElement.m_Prefab == m_OldPrefab)
					{
						activityLocationElement.m_Prefab = m_NewPrefab;
						val9[num12] = activityLocationElement;
					}
				}
			}
			for (int num13 = 0; num13 < bufferAccessor9.Length; num13++)
			{
				DynamicBuffer<SubMesh> val10 = bufferAccessor9[num13];
				for (int num14 = 0; num14 < val10.Length; num14++)
				{
					SubMesh subMesh = val10[num14];
					if (subMesh.m_SubMesh == m_OldPrefab)
					{
						subMesh.m_SubMesh = m_NewPrefab;
						val10[num14] = subMesh;
					}
				}
			}
			for (int num15 = 0; num15 < bufferAccessor10.Length; num15++)
			{
				DynamicBuffer<LodMesh> val11 = bufferAccessor10[num15];
				for (int num16 = 0; num16 < val11.Length; num16++)
				{
					LodMesh lodMesh = val11[num16];
					if (lodMesh.m_LodMesh == m_OldPrefab)
					{
						lodMesh.m_LodMesh = m_NewPrefab;
						val11[num16] = lodMesh;
					}
				}
			}
			for (int num17 = 0; num17 < bufferAccessor11.Length; num17++)
			{
				DynamicBuffer<UIGroupElement> val12 = bufferAccessor11[num17];
				for (int num18 = 0; num18 < val12.Length; num18++)
				{
					if (val12[num18].m_Prefab == m_OldPrefab)
					{
						val12.RemoveAtSwapBack(num18);
						num18--;
					}
				}
			}
			for (int num19 = 0; num19 < bufferAccessor12.Length; num19++)
			{
				DynamicBuffer<TutorialPhaseRef> val13 = bufferAccessor12[num19];
				for (int num20 = 0; num20 < val13.Length; num20++)
				{
					TutorialPhaseRef tutorialPhaseRef = val13[num20];
					if (tutorialPhaseRef.m_Phase == m_OldPrefab)
					{
						tutorialPhaseRef.m_Phase = m_NewPrefab;
						val13[num20] = tutorialPhaseRef;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

		public BufferLookup<FadeBatch> __Game_Rendering_FadeBatch_RW_BufferLookup;

		public BufferLookup<BatchGroup> __Game_Prefabs_BatchGroup_RW_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RW_ComponentTypeHandle;

		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RW_BufferTypeHandle;

		public BufferTypeHandle<SubNet> __Game_Prefabs_SubNet_RW_BufferTypeHandle;

		public BufferTypeHandle<SubArea> __Game_Prefabs_SubArea_RW_BufferTypeHandle;

		public BufferTypeHandle<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceUpgradeBuilding> __Game_Prefabs_ServiceUpgradeBuilding_RW_BufferTypeHandle;

		public BufferTypeHandle<BuildingUpgradeElement> __Game_Prefabs_BuildingUpgradeElement_RW_BufferTypeHandle;

		public BufferTypeHandle<Effect> __Game_Prefabs_Effect_RW_BufferTypeHandle;

		public BufferTypeHandle<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RW_BufferTypeHandle;

		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<LodMesh> __Game_Prefabs_LodMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<UIGroupElement> __Game_Prefabs_UIGroupElement_RW_BufferTypeHandle;

		public BufferTypeHandle<TutorialPhaseRef> __Game_Tutorials_TutorialPhaseRef_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
			__Game_Rendering_FadeBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FadeBatch>(false);
			__Game_Prefabs_BatchGroup_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BatchGroup>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(false);
			__Game_Tools_EditorContainer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.EditorContainer>(false);
			__Game_Prefabs_SubObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(false);
			__Game_Prefabs_SubNet_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubNet>(false);
			__Game_Prefabs_SubArea_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubArea>(false);
			__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceholderObjectElement>(false);
			__Game_Prefabs_ServiceUpgradeBuilding_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceUpgradeBuilding>(false);
			__Game_Prefabs_BuildingUpgradeElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BuildingUpgradeElement>(false);
			__Game_Prefabs_Effect_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Effect>(false);
			__Game_Prefabs_ActivityLocationElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ActivityLocationElement>(false);
			__Game_Prefabs_SubMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(false);
			__Game_Prefabs_LodMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LodMesh>(false);
			__Game_Prefabs_UIGroupElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<UIGroupElement>(false);
			__Game_Tutorials_TutorialPhaseRef_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TutorialPhaseRef>(false);
		}
	}

	private ToolSystem m_ToolSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Finalize m_FinalizeSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_PrefabRefQuery;

	private Entity m_OldPrefab;

	private Entity m_NewPrefab;

	private Entity m_SourceInstance;

	private NativeList<ReplaceMesh> m_MeshReplaces;

	private NativeQueue<Entity> m_UpdateInstances;

	private NativeHashMap<Entity, ReplacePrefabData> m_ReplacePrefabData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_FinalizeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Finalize>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadWrite<PrefabRef>(),
			ComponentType.ReadWrite<SubObject>(),
			ComponentType.ReadWrite<SubNet>(),
			ComponentType.ReadWrite<SubArea>(),
			ComponentType.ReadWrite<PlaceholderObjectElement>(),
			ComponentType.ReadWrite<ServiceUpgradeBuilding>(),
			ComponentType.ReadWrite<BuildingUpgradeElement>(),
			ComponentType.ReadWrite<Effect>(),
			ComponentType.ReadWrite<ActivityLocationElement>(),
			ComponentType.ReadWrite<SubMesh>(),
			ComponentType.ReadWrite<LodMesh>(),
			ComponentType.ReadWrite<UIGroupElement>(),
			ComponentType.ReadWrite<TutorialPhaseRef>()
		};
		array[0] = val;
		m_PrefabRefQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_MeshReplaces = new NativeList<ReplaceMesh>(1, AllocatorHandle.op_Implicit((Allocator)4));
		m_UpdateInstances = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ReplacePrefabData = new NativeHashMap<Entity, ReplacePrefabData>(1, AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabRefQuery);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_MeshReplaces.Dispose();
		m_UpdateInstances.Dispose();
		base.OnDestroy();
	}

	public void ReplacePrefab(Entity oldPrefab, Entity newPrefab, Entity sourceInstance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_OldPrefab = oldPrefab;
		m_NewPrefab = newPrefab;
		m_SourceInstance = sourceInstance;
		try
		{
			((ComponentSystemBase)this).Enabled = true;
			((ComponentSystemBase)this).Update();
		}
		finally
		{
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	public void FinalizeReplaces()
	{
		((ComponentSystemBase)m_FinalizeSystem).Update();
	}

	private void CheckInstanceComponents(Entity instance, HashSet<ComponentType> checkedComponents, HashSet<ComponentType> archetypeComponents)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ArchetypeChunk chunk = ((EntityManager)(ref entityManager)).GetChunk(instance);
		EntityArchetype archetype = ((ArchetypeChunk)(ref chunk)).Archetype;
		NativeArray<ComponentType> componentTypes = ((EntityArchetype)(ref archetype)).GetComponentTypes((Allocator)2);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(instance);
		m_PrefabSystem.GetPrefab<PrefabBase>(componentData).GetArchetypeComponents(archetypeComponents);
		Enumerator<ComponentType> enumerator = componentTypes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ComponentType current = enumerator.Current;
				if (checkedComponents.Contains(current))
				{
					if (archetypeComponents.Contains(current))
					{
						archetypeComponents.Remove(current);
						continue;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).RemoveComponent(instance, current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		foreach (ComponentType archetypeComponent in archetypeComponents)
		{
			if (checkedComponents.Contains(archetypeComponent))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent(instance, archetypeComponent);
			}
		}
		archetypeComponents.Clear();
		componentTypes.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = default(EntityCommandBuffer);
		((EntityCommandBuffer)(ref val))._002Ector((Allocator)3, (PlaybackPolicy)0);
		EntityCommandBuffer entityCommandBuffer = default(EntityCommandBuffer);
		m_ToolSystem.RequireFullUpdate();
		JobHandle val2 = default(JobHandle);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<MeshData>(m_OldPrefab))
		{
			((EntityCommandBuffer)(ref entityCommandBuffer))._002Ector((Allocator)3, (PlaybackPolicy)0);
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle dependencies3;
			RemoveBatchGroupsJob removeBatchGroupsJob = new RemoveBatchGroupsJob
			{
				m_OldMeshEntity = m_OldPrefab,
				m_NewMeshEntity = m_NewPrefab,
				m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FadeBatches = InternalCompilerInterface.GetBufferLookup<FadeBatch>(ref __TypeHandle.__Game_Rendering_FadeBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BatchGroups = InternalCompilerInterface.GetBufferLookup<BatchGroup>(ref __TypeHandle.__Game_Prefabs_BatchGroup_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies),
				m_NativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies2),
				m_NativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: false, out dependencies3),
				m_EntityCommandBuffer = entityCommandBuffer,
				m_ReplaceMeshes = m_MeshReplaces
			};
			val2 = JobHandle.CombineDependencies(val2, IJobExtensions.Schedule<RemoveBatchGroupsJob>(removeBatchGroupsJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3)));
			m_BatchManagerSystem.AddNativeBatchGroupsWriter(val2);
			m_BatchManagerSystem.AddNativeBatchInstancesWriter(val2);
			m_BatchManagerSystem.AddNativeSubBatchesWriter(val2);
		}
		else
		{
			m_ReplacePrefabData[m_NewPrefab] = new ReplacePrefabData
			{
				m_OldPrefab = m_OldPrefab,
				m_SourceInstance = m_SourceInstance
			};
		}
		if (m_SourceInstance != Entity.Null)
		{
			m_UpdateInstances.Enqueue(m_SourceInstance);
		}
		ReplacePrefabJob replacePrefabJob = new ReplacePrefabJob
		{
			m_OldPrefab = m_OldPrefab,
			m_NewPrefab = m_NewPrefab,
			m_SourceInstance = m_SourceInstance,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaType = InternalCompilerInterface.GetBufferTypeHandle<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderObjectElementType = InternalCompilerInterface.GetBufferTypeHandle<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeBuildingType = InternalCompilerInterface.GetBufferTypeHandle<ServiceUpgradeBuilding>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeBuilding_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingUpgradeElementType = InternalCompilerInterface.GetBufferTypeHandle<BuildingUpgradeElement>(ref __TypeHandle.__Game_Prefabs_BuildingUpgradeElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EffectType = InternalCompilerInterface.GetBufferTypeHandle<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocationElementType = InternalCompilerInterface.GetBufferTypeHandle<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshType = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LodMeshType = InternalCompilerInterface.GetBufferTypeHandle<LodMesh>(ref __TypeHandle.__Game_Prefabs_LodMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UIGroupElementType = InternalCompilerInterface.GetBufferTypeHandle<UIGroupElement>(ref __TypeHandle.__Game_Prefabs_UIGroupElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TutorialPhaseType = InternalCompilerInterface.GetBufferTypeHandle<TutorialPhaseRef>(ref __TypeHandle.__Game_Tutorials_TutorialPhaseRef_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter(),
			m_UpdateInstances = m_UpdateInstances.AsParallelWriter()
		};
		val2 = JobHandle.CombineDependencies(val2, JobChunkExtensions.ScheduleParallel<ReplacePrefabJob>(replacePrefabJob, m_PrefabRefQuery, ((SystemBase)this).Dependency));
		((JobHandle)(ref val2)).Complete();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<BuildingUpgradeElement>(m_OldPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<BuildingUpgradeElement> val3 = ((EntityManager)(ref entityManager)).AddBuffer<BuildingUpgradeElement>(m_NewPrefab);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<BuildingUpgradeElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<BuildingUpgradeElement>(m_OldPrefab, true);
			val3.CopyFrom(buffer);
		}
		((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
		((EntityCommandBuffer)(ref val)).Dispose();
		if (((EntityCommandBuffer)(ref entityCommandBuffer)).IsCreated)
		{
			((EntityCommandBuffer)(ref entityCommandBuffer)).Playback(((ComponentSystemBase)this).EntityManager);
			((EntityCommandBuffer)(ref entityCommandBuffer)).Dispose();
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
	public ReplacePrefabSystem()
	{
	}
}
