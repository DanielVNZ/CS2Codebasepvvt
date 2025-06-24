using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialObjectPlacementTriggerSystem : TutorialTriggerSystemBase
{
	private struct ClearCountJob : IJobChunk
	{
		public ComponentTypeHandle<ObjectPlacementTriggerCountData> m_CountType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ObjectPlacementTriggerCountData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectPlacementTriggerCountData>(ref m_CountType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ObjectPlacementTriggerCountData objectPlacementTriggerCountData = nativeArray[i];
				objectPlacementTriggerCountData.m_Count = 0;
				nativeArray[i] = objectPlacementTriggerCountData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CheckObjectsJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CreatedObjectChunks;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> m_ForcedUnlockDataFromEntity;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> m_UnlockRequirementFromEntity;

		[ReadOnly]
		public BufferTypeHandle<ObjectPlacementTriggerData> m_TriggerType;

		[ReadOnly]
		public ComponentLookup<Native> m_Natives;

		[ReadOnly]
		public ComponentLookup<ElectricityProducer> m_ElectricityProducers;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.SewageOutlet> m_SewageOutlets;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Transformer> m_Transformers;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_Placeholder;

		[ReadOnly]
		public ComponentLookup<Edge> m_Edges;

		[ReadOnly]
		public ComponentLookup<Game.Net.ElectricityConnection> m_ElectricityConnections;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> m_ResourceConnection;

		[ReadOnly]
		public ComponentLookup<Road> m_Roads;

		[ReadOnly]
		public ComponentLookup<Game.Net.WaterPipeConnection> m_WaterPipeConnections;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ElectricityConnection> m_ElectricityConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.WaterPipeConnection> m_WaterPipeConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ResourceConnection> m_ResourceConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public bool m_HasElevation;

		public ComponentTypeHandle<ObjectPlacementTriggerCountData> m_CountType;

		public ParallelWriter m_CommandBuffer;

		public bool m_FirstTimeCheck;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ObjectPlacementTriggerData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ObjectPlacementTriggerData>(ref m_TriggerType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ObjectPlacementTriggerCountData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectPlacementTriggerCountData>(ref m_CountType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (!Check(bufferAccessor[i]))
				{
					continue;
				}
				ObjectPlacementTriggerCountData objectPlacementTriggerCountData = nativeArray2[i];
				objectPlacementTriggerCountData.m_Count++;
				if (objectPlacementTriggerCountData.m_Count >= objectPlacementTriggerCountData.m_RequiredCount)
				{
					if (m_FirstTimeCheck)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerPreCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					TutorialSystem.ManualUnlock(nativeArray[i], m_UnlockEventArchetype, ref m_ForcedUnlockDataFromEntity, ref m_UnlockRequirementFromEntity, m_CommandBuffer, unfilteredChunkIndex);
				}
				nativeArray2[i] = objectPlacementTriggerCountData;
			}
		}

		private bool Check(DynamicBuffer<ObjectPlacementTriggerData> triggerDatas)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < triggerDatas.Length; i++)
			{
				ObjectPlacementTriggerData triggerData = triggerDatas[i];
				for (int j = 0; j < m_CreatedObjectChunks.Length; j++)
				{
					ArchetypeChunk val = m_CreatedObjectChunks[j];
					bool num = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.AllowSubObject);
					bool flag = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireElevation);
					if ((!num && ((ArchetypeChunk)(ref val)).Has<Owner>(ref m_OwnerType)) || (flag && !m_HasElevation))
					{
						continue;
					}
					NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					if (((ArchetypeChunk)(ref val)).Has<Edge>(ref m_EdgeType))
					{
						NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Edge>(ref m_EdgeType);
						bool flag2 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireOutsideConnection);
						bool flag3 = false;
						if (((ArchetypeChunk)(ref val)).Has<Game.Net.WaterPipeConnection>(ref m_WaterPipeConnectionType))
						{
							if (flag2)
							{
								flag3 = CheckNodes<Game.Net.WaterPipeConnection, Native>(triggerData, m_WaterPipeConnections, m_Natives, 1, nativeArray2, nativeArray);
							}
							if (!(!flag2 || flag3))
							{
								continue;
							}
							bool flag4 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireRoadConnection);
							bool flag5 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireSewageOutletConnection);
							if (flag4 || flag5)
							{
								bool flag6 = false;
								bool flag7 = false;
								if (flag4)
								{
									flag6 = CheckEdges<Game.Net.WaterPipeConnection, Road>(triggerData, m_WaterPipeConnections, m_Roads, 1, nativeArray2, nativeArray);
								}
								if (flag5)
								{
									flag7 = CheckNodes<Game.Net.WaterPipeConnection, Game.Buildings.SewageOutlet>(triggerData, m_WaterPipeConnections, m_SewageOutlets, 1, nativeArray2, nativeArray);
								}
								if ((!flag4 || flag6) && (!flag5 || flag7))
								{
									return true;
								}
								continue;
							}
							if (Check(triggerData, nativeArray))
							{
								return true;
							}
						}
						if (((ArchetypeChunk)(ref val)).Has<Game.Net.ElectricityConnection>(ref m_ElectricityConnectionType))
						{
							if (flag2)
							{
								flag3 = CheckEdges<Game.Net.ElectricityConnection, Native>(triggerData, m_ElectricityConnections, m_Natives, 1, nativeArray2, nativeArray);
							}
							if (!(!flag2 || flag3))
							{
								continue;
							}
							bool flag8 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireRoadConnection);
							bool flag9 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireTransformerConnection);
							bool flag10 = FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireElectricityProducerConnection);
							if (flag8 || flag9 || flag10)
							{
								bool flag11 = false;
								bool flag12 = false;
								bool flag13 = false;
								if (flag8)
								{
									flag11 = CheckEdges<Game.Net.ElectricityConnection, Road>(triggerData, m_ElectricityConnections, m_Roads, 1, nativeArray2, nativeArray);
								}
								if (flag9)
								{
									flag12 = CheckNodes<Game.Net.ElectricityConnection, Game.Buildings.Transformer>(triggerData, m_ElectricityConnections, m_Transformers, 1, nativeArray2, nativeArray);
								}
								if (flag10)
								{
									flag13 = CheckNodes<Game.Net.ElectricityConnection, ElectricityProducer>(triggerData, m_ElectricityConnections, m_ElectricityProducers, 1, nativeArray2, nativeArray);
								}
								if ((!flag8 || flag11) && (!flag9 || flag12) && (!flag10 || flag13))
								{
									return true;
								}
								continue;
							}
							if (Check(triggerData, nativeArray))
							{
								return true;
							}
						}
						if (((ArchetypeChunk)(ref val)).Has<Game.Net.ResourceConnection>(ref m_ResourceConnectionType))
						{
							if (FlagsMatch(triggerData, ObjectPlacementTriggerFlags.RequireResourceConnection) && CheckNodes<Game.Net.ResourceConnection, Placeholder>(triggerData, m_ResourceConnection, m_Placeholder, 1, nativeArray2, nativeArray))
							{
								return true;
							}
						}
						else if (Check(triggerData, nativeArray))
						{
							return true;
						}
					}
					else if (Check(triggerData, nativeArray))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool FlagsMatch(ObjectPlacementTriggerData triggerData, ObjectPlacementTriggerFlags flags)
		{
			return (triggerData.m_Flags & flags) == flags;
		}

		private bool CheckEdges<T1, T2>(ObjectPlacementTriggerData triggerData, ComponentLookup<T1> matchData, ComponentLookup<T2> searchData, int requiredCount, NativeArray<Edge> edges, NativeArray<PrefabRef> prefabRefs) where T1 : unmanaged, IComponentData where T2 : unmanaged, IComponentData
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			NativeList<Entity> stack = default(NativeList<Entity>);
			stack._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<Entity, int> onStack = default(NativeParallelHashMap<Entity, int>);
			onStack._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < prefabRefs.Length; i++)
			{
				if (prefabRefs[i].m_Prefab == triggerData.m_Object && CheckEdgesImpl<T1, T2>(edges[i].m_Start, matchData, searchData, m_Edges, m_ConnectedEdges, requiredCount, stack, onStack) >= requiredCount)
				{
					return true;
				}
			}
			onStack.Dispose();
			stack.Dispose();
			return false;
		}

		private int CheckEdgesImpl<T1, T2>(Entity node, ComponentLookup<T1> matchData, ComponentLookup<T2> searchData, ComponentLookup<Edge> edgesData, BufferLookup<ConnectedEdge> connectedEdgesData, int requiredCount, NativeList<Entity> stack, NativeParallelHashMap<Entity, int> onStack) where T1 : unmanaged, IComponentData where T2 : unmanaged, IComponentData
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Push(node, stack, onStack);
			while (stack.Length > 0)
			{
				Entity val = Pop(stack);
				if (!connectedEdgesData.HasBuffer(val))
				{
					continue;
				}
				DynamicBuffer<ConnectedEdge> val2 = connectedEdgesData[val];
				for (int i = 0; i < val2.Length; i++)
				{
					Entity edge = val2[i].m_Edge;
					if (searchData.HasComponent(edge) && onStack[val] == 1)
					{
						num++;
					}
					if (num >= requiredCount)
					{
						return num;
					}
					if (edgesData.HasComponent(edge) && matchData.HasComponent(edge))
					{
						Edge edge2 = edgesData[edge];
						if (!onStack.ContainsKey(edge2.m_Start) || onStack[edge2.m_Start] < 2)
						{
							Push(edge2.m_Start, stack, onStack);
						}
						if (!onStack.ContainsKey(edge2.m_End) || onStack[edge2.m_End] < 2)
						{
							Push(edge2.m_End, stack, onStack);
						}
					}
				}
			}
			return num;
		}

		private bool CheckNodes<T1, T2>(ObjectPlacementTriggerData triggerData, ComponentLookup<T1> matchData, ComponentLookup<T2> searchData, int requiredCount, NativeArray<Edge> edges, NativeArray<PrefabRef> prefabRefs) where T1 : unmanaged, IComponentData where T2 : unmanaged, IComponentData
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			NativeList<Entity> stack = default(NativeList<Entity>);
			stack._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<Entity, int> onStack = default(NativeParallelHashMap<Entity, int>);
			onStack._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < prefabRefs.Length; i++)
			{
				if (prefabRefs[i].m_Prefab == triggerData.m_Object && CheckNodesImpl<T1, T2>(edges[i].m_Start, matchData, searchData, m_Owners, m_Edges, m_ConnectedEdges, m_ConnectedNodes, requiredCount, stack, onStack) >= requiredCount)
				{
					return true;
				}
			}
			onStack.Dispose();
			stack.Dispose();
			return false;
		}

		private int CheckNodesImpl<T1, T2>(Entity node, ComponentLookup<T1> matchData, ComponentLookup<T2> searchData, ComponentLookup<Owner> ownerData, ComponentLookup<Edge> edgesData, BufferLookup<ConnectedEdge> connectedEdgesData, BufferLookup<ConnectedNode> connectedNodesData, int requiredCount, NativeList<Entity> stack, NativeParallelHashMap<Entity, int> onStack) where T1 : unmanaged, IComponentData where T2 : unmanaged, IComponentData
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Push(node, stack, onStack);
			while (stack.Length > 0)
			{
				Entity val = Pop(stack);
				if (searchData.HasComponent(val) && onStack[val] == 1)
				{
					num++;
				}
				else if (ownerData.HasComponent(val) && searchData.HasComponent(ownerData[val].m_Owner) && onStack[val] == 1)
				{
					num++;
				}
				if (num >= requiredCount)
				{
					return num;
				}
				if (!connectedEdgesData.HasBuffer(val))
				{
					continue;
				}
				DynamicBuffer<ConnectedEdge> val2 = connectedEdgesData[val];
				for (int i = 0; i < val2.Length; i++)
				{
					Entity edge = val2[i].m_Edge;
					if (!edgesData.HasComponent(edge) || !matchData.HasComponent(edge))
					{
						continue;
					}
					Edge edge2 = edgesData[edge];
					if (connectedNodesData.HasBuffer(edge))
					{
						DynamicBuffer<ConnectedNode> val3 = connectedNodesData[edge];
						for (int j = 0; j < val3.Length; j++)
						{
							Entity node2 = val3[j].m_Node;
							if (!onStack.ContainsKey(node2) || onStack[node2] < 1)
							{
								Push(node2, stack, onStack);
							}
						}
					}
					if (!onStack.ContainsKey(edge2.m_Start) || onStack[edge2.m_Start] < 2)
					{
						Push(edge2.m_Start, stack, onStack);
					}
					if (!onStack.ContainsKey(edge2.m_End) || onStack[edge2.m_End] < 2)
					{
						Push(edge2.m_End, stack, onStack);
					}
				}
			}
			return num;
		}

		private void Push(Entity entity, NativeList<Entity> stack, NativeParallelHashMap<Entity, int> onStack)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (!onStack.ContainsKey(entity))
			{
				onStack[entity] = 1;
			}
			else
			{
				Entity val = entity;
				onStack[val] += 1;
			}
			stack.Add(ref entity);
		}

		private Entity Pop(NativeList<Entity> stack)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			if (stack.Length > 0)
			{
				result = stack[stack.Length - 1];
				stack.RemoveAtSwapBack(stack.Length - 1);
			}
			return result;
		}

		private bool Check(ObjectPlacementTriggerData triggerData, NativeArray<PrefabRef> prefabRefs)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < prefabRefs.Length; i++)
			{
				if (prefabRefs[i].m_Prefab == triggerData.m_Object)
				{
					return true;
				}
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<ObjectPlacementTriggerCountData> __Game_Tutorials_ObjectPlacementTriggerCountData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ElectricityConnection> __Game_Net_ElectricityConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ElectricityConnection> __Game_Net_ElectricityConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Net.WaterPipeConnection> __Game_Net_WaterPipeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.WaterPipeConnection> __Game_Net_WaterPipeConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> __Game_Net_ResourceConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ResourceConnection> __Game_Net_ResourceConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.SewageOutlet> __Game_Buildings_SewageOutlet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Transformer> __Game_Buildings_Transformer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Placeholder> __Game_Objects_Placeholder_RO_ComponentLookup;

		[ReadOnly]
		public BufferTypeHandle<ObjectPlacementTriggerData> __Game_Tutorials_ObjectPlacementTriggerData_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> __Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> __Game_Prefabs_UnlockRequirement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

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
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tutorials_ObjectPlacementTriggerCountData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectPlacementTriggerCountData>(false);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Buildings_ElectricityProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityProducer>(true);
			__Game_Net_ElectricityConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ElectricityConnection>(true);
			__Game_Net_ElectricityConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ElectricityConnection>(true);
			__Game_Net_WaterPipeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.WaterPipeConnection>(true);
			__Game_Net_WaterPipeConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.WaterPipeConnection>(true);
			__Game_Net_ResourceConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ResourceConnection>(true);
			__Game_Net_ResourceConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ResourceConnection>(true);
			__Game_Buildings_SewageOutlet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.SewageOutlet>(true);
			__Game_Buildings_Transformer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Transformer>(true);
			__Game_Objects_Placeholder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Placeholder>(true);
			__Game_Tutorials_ObjectPlacementTriggerData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ObjectPlacementTriggerData>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ForceUIGroupUnlockData>(true);
			__Game_Prefabs_UnlockRequirement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UnlockRequirement>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private NetToolSystem m_NetToolSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_CreatedObjectQuery;

	private EntityQuery m_ObjectQuery;

	private EntityArchetype m_UnlockEventArchetype;

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
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ObjectPlacementTriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.ReadWrite<ObjectPlacementTriggerCountData>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Game.Net.WaterPipeConnection>(),
			ComponentType.ReadOnly<Game.Net.ElectricityConnection>(),
			ComponentType.ReadOnly<Game.Prefabs.ResourceConnection>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<Waterway>(),
			ComponentType.ReadOnly<ServiceUpgradeBuilding>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Native>()
		};
		array[0] = val;
		m_CreatedObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() };
		val.Any = (ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Game.Net.WaterPipeConnection>(),
			ComponentType.ReadOnly<Game.Net.ElectricityConnection>(),
			ComponentType.ReadOnly<Game.Net.ResourceConnection>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<Waterway>(),
			ComponentType.ReadOnly<ServiceUpgradeBuilding>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Native>()
		};
		array2[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		((ComponentSystemBase)this).RequireForUpdate(m_ActiveTriggerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		EntityCommandBuffer val2;
		if (base.triggersChanged)
		{
			ClearCountJob clearCountJob = new ClearCountJob
			{
				m_CountType = InternalCompilerInterface.GetComponentTypeHandle<ObjectPlacementTriggerCountData>(ref __TypeHandle.__Game_Tutorials_ObjectPlacementTriggerCountData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ClearCountJob>(clearCountJob, m_ActiveTriggerQuery, ((SystemBase)this).Dependency);
			JobHandle val = default(JobHandle);
			CheckObjectsJob checkObjectsJob = new CheckObjectsJob
			{
				m_CreatedObjectChunks = ((EntityQuery)(ref m_ObjectQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_Natives = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityProducers = InternalCompilerInterface.GetComponentLookup<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnections = InternalCompilerInterface.GetComponentLookup<Game.Net.ElectricityConnection>(ref __TypeHandle.__Game_Net_ElectricityConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ElectricityConnection>(ref __TypeHandle.__Game_Net_ElectricityConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeConnections = InternalCompilerInterface.GetComponentLookup<Game.Net.WaterPipeConnection>(ref __TypeHandle.__Game_Net_WaterPipeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.WaterPipeConnection>(ref __TypeHandle.__Game_Net_WaterPipeConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnection = InternalCompilerInterface.GetComponentLookup<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SewageOutlets = InternalCompilerInterface.GetComponentLookup<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transformers = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Placeholder = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TriggerType = InternalCompilerInterface.GetBufferTypeHandle<ObjectPlacementTriggerData>(ref __TypeHandle.__Game_Tutorials_ObjectPlacementTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CountType = InternalCompilerInterface.GetComponentTypeHandle<ObjectPlacementTriggerCountData>(ref __TypeHandle.__Game_Tutorials_ObjectPlacementTriggerCountData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkObjectsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkObjectsJob.m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Roads = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Edges = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_UnlockEventArchetype = m_UnlockEventArchetype;
			checkObjectsJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_HasElevation = HasElevation();
			checkObjectsJob.m_FirstTimeCheck = true;
			CheckObjectsJob checkObjectsJob2 = checkObjectsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckObjectsJob>(checkObjectsJob2, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			checkObjectsJob2.m_CreatedObjectChunks.Dispose(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		else if (!((EntityQuery)(ref m_CreatedObjectQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val3 = default(JobHandle);
			CheckObjectsJob checkObjectsJob = new CheckObjectsJob
			{
				m_CreatedObjectChunks = ((EntityQuery)(ref m_CreatedObjectQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3),
				m_Natives = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityProducers = InternalCompilerInterface.GetComponentLookup<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnections = InternalCompilerInterface.GetComponentLookup<Game.Net.ElectricityConnection>(ref __TypeHandle.__Game_Net_ElectricityConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ElectricityConnection>(ref __TypeHandle.__Game_Net_ElectricityConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeConnections = InternalCompilerInterface.GetComponentLookup<Game.Net.WaterPipeConnection>(ref __TypeHandle.__Game_Net_WaterPipeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaterPipeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.WaterPipeConnection>(ref __TypeHandle.__Game_Net_WaterPipeConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnection = InternalCompilerInterface.GetComponentLookup<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SewageOutlets = InternalCompilerInterface.GetComponentLookup<Game.Buildings.SewageOutlet>(ref __TypeHandle.__Game_Buildings_SewageOutlet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transformers = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Placeholder = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TriggerType = InternalCompilerInterface.GetBufferTypeHandle<ObjectPlacementTriggerData>(ref __TypeHandle.__Game_Tutorials_ObjectPlacementTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CountType = InternalCompilerInterface.GetComponentTypeHandle<ObjectPlacementTriggerCountData>(ref __TypeHandle.__Game_Tutorials_ObjectPlacementTriggerCountData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkObjectsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkObjectsJob.m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Roads = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Edges = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_UnlockEventArchetype = m_UnlockEventArchetype;
			checkObjectsJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			checkObjectsJob.m_HasElevation = HasElevation();
			checkObjectsJob.m_FirstTimeCheck = false;
			CheckObjectsJob checkObjectsJob3 = checkObjectsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckObjectsJob>(checkObjectsJob3, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
			checkObjectsJob3.m_CreatedObjectChunks.Dispose(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
	}

	private bool HasElevation()
	{
		if (m_ToolSystem.activeTool == m_NetToolSystem)
		{
			return math.abs(m_NetToolSystem.elevation) > 0.0001f;
		}
		return false;
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
	public TutorialObjectPlacementTriggerSystem()
	{
	}
}
