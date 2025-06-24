using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateAreasSystem : GameSystemBase
{
	private struct OldAreaData : IEquatable<OldAreaData>
	{
		public Entity m_Prefab;

		public Entity m_Original;

		public Entity m_Owner;

		public bool Equals(OldAreaData other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((Entity)(ref m_Original)).Equals(other.m_Original))
			{
				return ((Entity)(ref m_Owner)).Equals(other.m_Owner);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	[BurstCompile]
	private struct CreateAreasJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> m_OwnerDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<LocalNodeCache> m_LocalNodeCacheType;

		[ReadOnly]
		public ComponentLookup<Storage> m_StorageData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaData> m_AreaData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_AreaGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_LocalNodeCache;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DeletedChunks;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMap<OldAreaData, Entity> deletedAreas = default(NativeParallelMultiHashMap<OldAreaData, Entity>);
			deletedAreas._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_DeletedChunks.Length; i++)
			{
				FillDeletedAreas(m_DeletedChunks[i], deletedAreas);
			}
			for (int j = 0; j < m_DefinitionChunks.Length; j++)
			{
				CreateAreas(m_DefinitionChunks[j], deletedAreas);
			}
			deletedAreas.Dispose();
		}

		private void FillDeletedAreas(ArchetypeChunk chunk, NativeParallelMultiHashMap<OldAreaData, Entity> deletedAreas)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				OldAreaData oldAreaData = new OldAreaData
				{
					m_Prefab = nativeArray4[i].m_Prefab,
					m_Original = nativeArray2[i].m_Original
				};
				if (nativeArray3.Length != 0)
				{
					oldAreaData.m_Owner = nativeArray3[i].m_Owner;
				}
				deletedAreas.Add(oldAreaData, val);
			}
		}

		private void CreateAreas(ArchetypeChunk chunk, NativeParallelMultiHashMap<OldAreaData, Entity> deletedAreas)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_080e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_0934: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<OwnerDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			BufferAccessor<LocalNodeCache> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LocalNodeCache>(ref m_LocalNodeCacheType);
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<OldAreaData> val3 = default(NativeParallelMultiHashMapIterator<OldAreaData>);
			AreaGeometryData areaGeometryData = default(AreaGeometryData);
			DynamicBuffer<Game.Prefabs.SubArea> val7 = default(DynamicBuffer<Game.Prefabs.SubArea>);
			DynamicBuffer<Game.Areas.SubArea> val9 = default(DynamicBuffer<Game.Areas.SubArea>);
			NativeParallelMultiHashMapIterator<Entity> val10 = default(NativeParallelMultiHashMapIterator<Entity>);
			Entity val11 = default(Entity);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				if (m_DeletedData.HasComponent(creationDefinition.m_Owner))
				{
					continue;
				}
				OwnerDefinition ownerDefinition = default(OwnerDefinition);
				if (nativeArray2.Length != 0)
				{
					ownerDefinition = nativeArray2[i];
				}
				DynamicBuffer<Node> val = bufferAccessor[i];
				AreaFlags areaFlags = (AreaFlags)0;
				TempFlags tempFlags = (TempFlags)0u;
				if (creationDefinition.m_Original != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(creationDefinition.m_Original, default(Hidden));
					creationDefinition.m_Prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
					if ((creationDefinition.m_Flags & CreationFlags.Recreate) != 0)
					{
						tempFlags |= TempFlags.Modify;
					}
					else
					{
						areaFlags |= AreaFlags.Complete;
						if ((creationDefinition.m_Flags & CreationFlags.Delete) != 0)
						{
							tempFlags |= TempFlags.Delete;
						}
						else if ((creationDefinition.m_Flags & CreationFlags.Select) != 0)
						{
							tempFlags |= TempFlags.Select;
						}
						else if ((creationDefinition.m_Flags & CreationFlags.Relocate) != 0)
						{
							tempFlags |= TempFlags.Modify;
						}
						else if ((creationDefinition.m_Flags & CreationFlags.Duplicate) != 0)
						{
							tempFlags |= TempFlags.Duplicate;
						}
						if ((creationDefinition.m_Flags & CreationFlags.Parent) != 0)
						{
							tempFlags |= TempFlags.Parent;
						}
					}
				}
				else
				{
					tempFlags |= TempFlags.Create;
				}
				if (ownerDefinition.m_Prefab == Entity.Null)
				{
					tempFlags |= TempFlags.Essential;
				}
				if ((creationDefinition.m_Flags & CreationFlags.Hidden) != 0)
				{
					tempFlags |= TempFlags.Hidden;
				}
				bool flag = false;
				OldAreaData oldAreaData = new OldAreaData
				{
					m_Prefab = creationDefinition.m_Prefab,
					m_Original = creationDefinition.m_Original,
					m_Owner = creationDefinition.m_Owner
				};
				if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && deletedAreas.TryGetFirstValue(oldAreaData, ref val2, ref val3))
				{
					deletedAreas.Remove(val3);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(val2, new Temp(creationDefinition.m_Original, tempFlags));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
					if (ownerDefinition.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, default(Owner));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
					}
					else
					{
						if (creationDefinition.m_Owner != Entity.Null)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(creationDefinition.m_Owner));
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Owner>(val2);
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<OwnerDefinition>(val2);
					}
					if ((creationDefinition.m_Flags & CreationFlags.Native) != 0 || m_NativeData.HasComponent(creationDefinition.m_Original))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Native>(val2, default(Native));
					}
					else
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Native>(val2);
					}
				}
				else
				{
					AreaData areaData = m_AreaData[creationDefinition.m_Prefab];
					val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(areaData.m_Archetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(creationDefinition.m_Prefab));
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, new Temp(creationDefinition.m_Original, tempFlags));
					}
					if (ownerDefinition.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, default(Owner));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
					}
					else if (creationDefinition.m_Owner != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(creationDefinition.m_Owner));
					}
					if ((creationDefinition.m_Flags & CreationFlags.Native) != 0 || m_NativeData.HasComponent(creationDefinition.m_Original))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Native>(val2, default(Native));
					}
					flag = true;
				}
				DynamicBuffer<Node> val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<Node>(val2);
				bool flag2 = false;
				if ((areaFlags & AreaFlags.Complete) == 0 && val.Length >= 4)
				{
					Node node = val[0];
					if (((float3)(ref node.m_Position)).Equals(val[val.Length - 1].m_Position))
					{
						val4.ResizeUninitialized(val.Length - 1);
						for (int j = 0; j < val.Length - 1; j++)
						{
							val4[j] = val[j];
						}
						areaFlags |= AreaFlags.Complete;
						flag2 = true;
						goto IL_04ef;
					}
				}
				val4.ResizeUninitialized(val.Length);
				for (int k = 0; k < val.Length; k++)
				{
					val4[k] = val[k];
				}
				goto IL_04ef;
				IL_04ef:
				bool flag3 = false;
				bool flag4 = false;
				if (m_AreaGeometryData.TryGetComponent(creationDefinition.m_Prefab, ref areaGeometryData))
				{
					flag3 = (areaGeometryData.m_Flags & GeometryFlags.OnWaterSurface) != 0;
					flag4 = (areaGeometryData.m_Flags & GeometryFlags.PseudoRandom) != 0;
				}
				for (int l = 0; l < val4.Length; l++)
				{
					ref Node reference = ref val4.ElementAt(l);
					if (reference.m_Elevation == float.MinValue)
					{
						Node node2 = ((!flag3) ? AreaUtils.AdjustPosition(reference, ref m_TerrainHeightData) : AreaUtils.AdjustPosition(reference, ref m_TerrainHeightData, ref m_WaterSurfaceData));
						bool flag5 = math.abs(node2.m_Position.y - reference.m_Position.y) >= 0.01f;
						reference.m_Position = math.select(reference.m_Position, node2.m_Position, flag5);
					}
				}
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<LocalNodeCache> val5 = bufferAccessor2[i];
					DynamicBuffer<LocalNodeCache> val6 = ((!flag && m_LocalNodeCache.HasBuffer(val2)) ? ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<LocalNodeCache>(val2) : ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val2));
					if (flag2)
					{
						val6.ResizeUninitialized(val5.Length - 1);
						for (int m = 0; m < val5.Length - 1; m++)
						{
							val6[m] = val5[m];
						}
					}
					else
					{
						val6.ResizeUninitialized(val5.Length);
						for (int n = 0; n < val5.Length; n++)
						{
							val6[n] = val5[n];
						}
					}
				}
				else if (!flag && m_LocalNodeCache.HasBuffer(val2))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<LocalNodeCache>(val2);
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Area>(val2, new Area(areaFlags));
				if (m_StorageData.HasComponent(creationDefinition.m_Original))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Storage>(val2, m_StorageData[creationDefinition.m_Original]);
				}
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				if (flag4)
				{
					if (!m_PseudoRandomSeedData.TryGetComponent(creationDefinition.m_Original, ref pseudoRandomSeed))
					{
						pseudoRandomSeed = new PseudoRandomSeed((ushort)creationDefinition.m_RandomSeed);
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(val2, pseudoRandomSeed);
				}
				if (!m_PrefabSubAreas.TryGetBuffer(creationDefinition.m_Prefab, ref val7))
				{
					continue;
				}
				NativeParallelMultiHashMap<Entity, Entity> val8 = default(NativeParallelMultiHashMap<Entity, Entity>);
				tempFlags = (TempFlags)((uint)tempFlags & 0xFFFFFFF7u);
				areaFlags |= AreaFlags.Slave;
				if (m_SubAreas.TryGetBuffer(creationDefinition.m_Original, ref val9) && val9.Length != 0)
				{
					val8._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
					for (int num = 0; num < val9.Length; num++)
					{
						Game.Areas.SubArea subArea = val9[num];
						val8.Add(m_PrefabRefData[subArea.m_Area].m_Prefab, subArea.m_Area);
					}
				}
				for (int num2 = 0; num2 < val7.Length; num2++)
				{
					Game.Prefabs.SubArea subArea2 = val7[num2];
					oldAreaData = new OldAreaData
					{
						m_Prefab = subArea2.m_Prefab,
						m_Owner = val2
					};
					if (val8.IsCreated && val8.TryGetFirstValue(subArea2.m_Prefab, ref oldAreaData.m_Original, ref val10))
					{
						val8.Remove(val10);
					}
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0 && deletedAreas.TryGetFirstValue(oldAreaData, ref val11, ref val3))
					{
						deletedAreas.Remove(val3);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(val11, new Temp(oldAreaData.m_Original, tempFlags));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val11, default(Updated));
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val11);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val11, new Owner(val2));
						if ((creationDefinition.m_Flags & CreationFlags.Native) != 0 || m_NativeData.HasComponent(oldAreaData.m_Original))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Native>(val11, default(Native));
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Native>(val11);
						}
					}
					else
					{
						AreaData areaData2 = m_AreaData[subArea2.m_Prefab];
						val11 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(areaData2.m_Archetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val11, new PrefabRef(subArea2.m_Prefab));
						if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val11, new Temp(oldAreaData.m_Original, tempFlags));
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val11, new Owner(val2));
						if ((creationDefinition.m_Flags & CreationFlags.Native) != 0 || m_NativeData.HasComponent(oldAreaData.m_Original))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Native>(val2, default(Native));
						}
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Area>(val11, new Area(areaFlags));
					if (m_StorageData.HasComponent(oldAreaData.m_Original))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Storage>(val11, m_StorageData[oldAreaData.m_Original]);
					}
					if (flag4)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(val11, pseudoRandomSeed);
					}
				}
				if (val8.IsCreated)
				{
					val8.Dispose();
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Storage> __Game_Areas_Storage_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaData> __Game_Prefabs_AreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Tools_LocalNodeCache_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LocalNodeCache>(true);
			__Game_Areas_Storage_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Storage>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
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
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> definitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> deletedChunks = ((EntityQuery)(ref m_DeletedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle deps;
		JobHandle val3 = IJobExtensions.Schedule<CreateAreasJob>(new CreateAreasJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LocalNodeCacheType = InternalCompilerInterface.GetBufferTypeHandle<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StorageData = InternalCompilerInterface.GetComponentLookup<Storage>(ref __TypeHandle.__Game_Areas_Storage_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaData = InternalCompilerInterface.GetComponentLookup<AreaData>(ref __TypeHandle.__Game_Prefabs_AreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalNodeCache = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefinitionChunks = definitionChunks,
			m_DeletedChunks = deletedChunks,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val, val2, deps));
		definitionChunks.Dispose(val3);
		deletedChunks.Dispose(val3);
		m_TerrainSystem.AddCPUHeightReader(val3);
		m_WaterSystem.AddSurfaceReader(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
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
	public GenerateAreasSystem()
	{
	}
}
