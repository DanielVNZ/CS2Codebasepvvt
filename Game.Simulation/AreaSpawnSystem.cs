using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Net;
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
public class AreaSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct AreaSpawnJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> m_GeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Storage> m_StorageType;

		[ReadOnly]
		public ComponentTypeHandle<Extractor> m_ExtractorType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<CompanyData> m_CompanyData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> m_PrefabStorageAreaData;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_PrefabExtractorAreaData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public BufferLookup<Renter> m_BuildingRenters;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PlaceholderObjectElements;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirements;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public bool m_DebugFastSpawn;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_DefinitionArchetype;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeList<AreaUtils.ObjectItem> objects = default(NativeList<AreaUtils.ObjectItem>);
			NativeParallelHashSet<Entity> placeholderRequirements = default(NativeParallelHashSet<Entity>);
			NativeArray<Storage> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Storage>(ref m_StorageType);
			NativeArray<Extractor> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Extractor>(ref m_ExtractorType);
			if (nativeArray.Length != 0 || nativeArray2.Length != 0)
			{
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Area> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
				NativeArray<Geometry> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Geometry>(ref m_GeometryType);
				NativeArray<Owner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
				BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
				BufferAccessor<Game.Objects.SubObject> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectType);
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				DynamicBuffer<Game.Areas.SubArea> val4 = default(DynamicBuffer<Game.Areas.SubArea>);
				for (int i = 0; i < nativeArray3.Length; i++)
				{
					Geometry geometry = nativeArray5[i];
					PrefabRef prefabRef = nativeArray7[i];
					DynamicBuffer<Game.Objects.SubObject> val = bufferAccessor3[i];
					AreaGeometryData areaData = m_PrefabAreaGeometryData[prefabRef.m_Prefab];
					float num = 0f;
					if (nativeArray.Length != 0)
					{
						Storage storage = nativeArray[i];
						StorageAreaData prefabStorageData = m_PrefabStorageAreaData[prefabRef.m_Prefab];
						if (m_DebugFastSpawn)
						{
							storage.m_Amount = AreaUtils.CalculateStorageCapacity(geometry, prefabStorageData);
						}
						num = math.max(num, AreaUtils.CalculateStorageObjectArea(geometry, storage, prefabStorageData));
					}
					if (nativeArray2.Length != 0)
					{
						Extractor extractor = nativeArray2[i];
						ExtractorAreaData extractorAreaData = m_PrefabExtractorAreaData[prefabRef.m_Prefab];
						if (m_DebugFastSpawn)
						{
							extractor.m_TotalExtracted = extractor.m_ResourceAmount;
						}
						num = math.max(num, AreaUtils.CalculateExtractorObjectArea(geometry, extractor, extractorAreaData));
					}
					if (num < 1f)
					{
						continue;
					}
					if (!objects.IsCreated && val.Length > 0)
					{
						objects._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					float num2 = 0f;
					for (int j = 0; j < val.Length; j++)
					{
						Entity subObject = val[j].m_SubObject;
						if (m_SecondaryData.HasComponent(subObject))
						{
							continue;
						}
						PrefabRef prefabRef2 = m_PrefabRefData[subObject];
						if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData))
						{
							Transform transform = m_TransformData[subObject];
							float num3;
							if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
							{
								num3 = objectGeometryData.m_Size.x * 0.5f;
							}
							else
							{
								num3 = math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz)) * 0.5f;
								ref float3 position = ref transform.m_Position;
								float2 xz = ((float3)(ref position)).xz;
								float3 val2 = math.rotate(transform.m_Rotation, MathUtils.Center(objectGeometryData.m_Bounds));
								((float3)(ref position)).xz = xz - ((float3)(ref val2)).xz;
							}
							if (m_PrefabBuildingData.HasComponent(prefabRef2.m_Prefab))
							{
								num3 += AreaUtils.GetMinNodeDistance(areaData);
							}
							num2 += num3 * num3 * (float)Math.PI;
							AreaUtils.ObjectItem objectItem = new AreaUtils.ObjectItem(num3, ((float3)(ref transform.m_Position)).xz, subObject);
							objects.Add(ref objectItem);
						}
					}
					if (num2 >= num)
					{
						continue;
					}
					if (m_PrefabSubObjects.HasBuffer(prefabRef.m_Prefab))
					{
						DynamicBuffer<Game.Prefabs.SubObject> prefabSubObjects = m_PrefabSubObjects[prefabRef.m_Prefab];
						Owner owner = default(Owner);
						if (nativeArray6.Length != 0)
						{
							owner = nativeArray6[i];
						}
						if (TryGetObjectPrefab(ref random, ref placeholderRequirements, owner, prefabSubObjects, out var prefab))
						{
							Area area = nativeArray4[i];
							DynamicBuffer<Game.Areas.Node> nodes = bufferAccessor[i];
							DynamicBuffer<Triangle> triangles = bufferAccessor2[i];
							ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
							if (m_PrefabObjectGeometryData.HasComponent(prefab))
							{
								objectGeometryData2 = m_PrefabObjectGeometryData[prefab];
							}
							float extraRadius = 0f;
							bool flag = false;
							if (m_PrefabBuildingData.HasComponent(prefab))
							{
								extraRadius = AreaUtils.GetMinNodeDistance(areaData);
								flag = true;
							}
							if (AreaUtils.TryGetRandomObjectLocation(ref random, objectGeometryData2, area, geometry, extraRadius, nodes, triangles, objects, out var transform2))
							{
								Entity val3 = nativeArray3[i];
								Game.Objects.Elevation elevation = new Game.Objects.Elevation(0f, ElevationFlags.OnGround);
								transform2 = ObjectUtils.AdjustPosition(transform2, ref elevation, prefab, out var _, ref m_TerrainHeightData, ref m_WaterSurfaceData, ref m_PrefabPlaceableObjectData, ref m_PrefabObjectGeometryData);
								SpawnObject(unfilteredChunkIndex, val3, prefab, transform2, elevation, ref random);
								if (objects.IsCreated && objects.Length != 0)
								{
									for (int k = 0; k < objects.Length; k++)
									{
										Entity entity = objects[k].m_Entity;
										PrefabRef prefabRef3 = m_PrefabRefData[entity];
										if (m_PrefabBuildingData.HasComponent(prefabRef3.m_Prefab))
										{
											objects.RemoveAtSwapBack(k--);
											((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, entity, default(Deleted));
											flag = true;
										}
									}
									if (objects.Length != 0)
									{
										((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val3, default(Updated));
									}
								}
								if (flag && m_SubAreas.TryGetBuffer(val3, ref val4))
								{
									for (int l = 0; l < val4.Length; l++)
									{
										((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val4[l].m_Area, default(Updated));
									}
								}
							}
						}
					}
					if (objects.IsCreated)
					{
						objects.Clear();
					}
				}
			}
			if (objects.IsCreated)
			{
				objects.Dispose();
			}
			if (placeholderRequirements.IsCreated)
			{
				placeholderRequirements.Dispose();
			}
		}

		private bool TryGetObjectPrefab(ref Random random, ref NativeParallelHashSet<Entity> placeholderRequirements, Owner owner, DynamicBuffer<Game.Prefabs.SubObject> prefabSubObjects, out Entity prefab)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			prefab = Entity.Null;
			int num = 0;
			for (int i = 0; i < prefabSubObjects.Length; i++)
			{
				if ((prefabSubObjects[i].m_Flags & SubObjectFlags.EdgePlacement) == 0)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return false;
			}
			num = ((Random)(ref random)).NextInt(num);
			for (int j = 0; j < prefabSubObjects.Length; j++)
			{
				Game.Prefabs.SubObject subObject = prefabSubObjects[j];
				if ((subObject.m_Flags & SubObjectFlags.EdgePlacement) == 0 && num-- == 0)
				{
					prefab = subObject.m_Prefab;
					break;
				}
			}
			return TryGetObjectPrefab(ref random, ref prefab, ref placeholderRequirements, owner);
		}

		private bool TryGetObjectPrefab(ref Random random, ref Entity prefab, ref NativeParallelHashSet<Entity> placeholderRequirements, Owner owner)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PlaceholderObjectElements.HasBuffer(prefab))
			{
				return true;
			}
			DynamicBuffer<PlaceholderObjectElement> val = m_PlaceholderObjectElements[prefab];
			int num = 0;
			bool flag = false;
			DynamicBuffer<ObjectRequirementElement> val3 = default(DynamicBuffer<ObjectRequirementElement>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i].m_Object;
				if (m_ObjectRequirements.TryGetBuffer(val2, ref val3))
				{
					if (!flag)
					{
						flag = true;
						FillRequirements(ref placeholderRequirements, owner);
					}
					int num2 = -1;
					bool flag2 = true;
					for (int j = 0; j < val3.Length; j++)
					{
						ObjectRequirementElement objectRequirementElement = val3[j];
						if (objectRequirementElement.m_Group != num2)
						{
							if (!flag2)
							{
								break;
							}
							num2 = objectRequirementElement.m_Group;
							flag2 = false;
						}
						flag2 |= placeholderRequirements.Contains(objectRequirementElement.m_Requirement);
					}
					if (!flag2)
					{
						continue;
					}
				}
				int probability = m_PrefabSpawnableObjectData[val2].m_Probability;
				num += probability;
				if (((Random)(ref random)).NextInt(num) < probability)
				{
					prefab = val2;
				}
			}
			return ((Random)(ref random)).NextInt(100) < num;
		}

		private void FillRequirements(ref NativeParallelHashSet<Entity> placeholderRequirements, Owner owner)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			if (placeholderRequirements.IsCreated)
			{
				placeholderRequirements.Clear();
			}
			else
			{
				placeholderRequirements = new NativeParallelHashSet<Entity>(10, AllocatorHandle.op_Implicit((Allocator)2));
			}
			Owner owner2 = default(Owner);
			if (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
			{
				owner = owner2;
			}
			Entity val = owner.m_Owner;
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(owner.m_Owner, ref attachment))
			{
				val = attachment.m_Attached;
			}
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			if (!m_BuildingRenters.TryGetBuffer(val, ref val2))
			{
				return;
			}
			for (int i = 0; i < val2.Length; i++)
			{
				Entity renter = val2[i].m_Renter;
				if (m_CompanyData.HasComponent(renter))
				{
					Entity prefab = m_PrefabRefData[renter].m_Prefab;
					Entity brand = m_CompanyData[renter].m_Brand;
					if (brand != Entity.Null)
					{
						placeholderRequirements.Add(brand);
					}
					placeholderRequirements.Add(prefab);
				}
			}
		}

		private void Spawn(int jobIndex, OwnerDefinition ownerDefinition, DynamicBuffer<Game.Prefabs.SubArea> subAreas, DynamicBuffer<SubAreaNode> subAreaNodes, ref Random random)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, int> selectedSpawnables = default(NativeParallelHashMap<Entity, int>);
			for (int i = 0; i < subAreas.Length; i++)
			{
				Game.Prefabs.SubArea subArea = subAreas[i];
				int seed;
				if (m_PlaceholderObjectElements.HasBuffer(subArea.m_Prefab))
				{
					DynamicBuffer<PlaceholderObjectElement> placeholderElements = m_PlaceholderObjectElements[subArea.m_Prefab];
					if (!selectedSpawnables.IsCreated)
					{
						selectedSpawnables._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
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
					m_RandomSeed = seed
				};
				creationDefinition.m_Flags |= CreationFlags.Permanent;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
				DynamicBuffer<Game.Areas.Node> val2 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(jobIndex, val);
				val2.ResizeUninitialized(subArea.m_NodeRange.y - subArea.m_NodeRange.x + 1);
				int num = ObjectToolBaseSystem.GetFirstNodeIndex(subAreaNodes, subArea.m_NodeRange);
				int num2 = 0;
				for (int j = subArea.m_NodeRange.x; j <= subArea.m_NodeRange.y; j++)
				{
					float3 position = subAreaNodes[num].m_Position;
					float3 position2 = ObjectUtils.LocalToWorld(ownerDefinition.m_Position, ownerDefinition.m_Rotation, position);
					int parentMesh = subAreaNodes[num].m_ParentMesh;
					float elevation = math.select(float.MinValue, position.y, parentMesh >= 0);
					val2[num2] = new Game.Areas.Node(position2, elevation);
					num2++;
					if (++num == subArea.m_NodeRange.y)
					{
						num = subArea.m_NodeRange.x;
					}
				}
			}
			if (selectedSpawnables.IsCreated)
			{
				selectedSpawnables.Dispose();
			}
		}

		private void Spawn(int jobIndex, OwnerDefinition ownerDefinition, DynamicBuffer<Game.Prefabs.SubNet> subNets, ref Random random)
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
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			NativeList<float4> nodePositions = default(NativeList<float4>);
			nodePositions._002Ector(subNets.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < subNets.Length; i++)
			{
				Game.Prefabs.SubNet subNet = subNets[i];
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
				Game.Prefabs.SubNet subNet2 = NetUtils.GetSubNet(subNets, k, m_LefthandTraffic, ref m_PrefabNetGeometryData);
				CreateSubNet(jobIndex, subNet2.m_Prefab, subNet2.m_Curve, subNet2.m_NodeIndex, subNet2.m_ParentMesh, subNet2.m_Upgrades, nodePositions, ownerDefinition, ref random);
			}
			nodePositions.Dispose();
		}

		private void CreateSubNet(int jobIndex, Entity netPrefab, Bezier4x3 curve, int2 nodeIndex, int2 parentMesh, CompositionFlags upgrades, NativeList<float4> nodePositions, OwnerDefinition ownerDefinition, ref Random random)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = netPrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.Permanent;
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = ObjectUtils.LocalToWorld(ownerDefinition.m_Position, ownerDefinition.m_Rotation, curve);
			netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
			netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve), ownerDefinition.m_Rotation);
			netCourse.m_StartPosition.m_CourseDelta = 0f;
			netCourse.m_StartPosition.m_Elevation = float2.op_Implicit(curve.a.y);
			netCourse.m_StartPosition.m_ParentMesh = parentMesh.x;
			float4 val2;
			if (nodeIndex.x >= 0)
			{
				ref CoursePos startPosition = ref netCourse.m_StartPosition;
				float3 position = ownerDefinition.m_Position;
				quaternion rotation = ownerDefinition.m_Rotation;
				val2 = nodePositions[nodeIndex.x];
				startPosition.m_Position = ObjectUtils.LocalToWorld(position, rotation, ((float4)(ref val2)).xyz);
			}
			netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
			netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve), ownerDefinition.m_Rotation);
			netCourse.m_EndPosition.m_CourseDelta = 1f;
			netCourse.m_EndPosition.m_Elevation = float2.op_Implicit(curve.d.y);
			netCourse.m_EndPosition.m_ParentMesh = parentMesh.y;
			if (nodeIndex.y >= 0)
			{
				ref CoursePos endPosition = ref netCourse.m_EndPosition;
				float3 position2 = ownerDefinition.m_Position;
				quaternion rotation2 = ownerDefinition.m_Rotation;
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
		}

		private void SpawnObject(int jobIndex, Entity entity, Entity prefab, Transform transform, Game.Objects.Elevation elevation, ref Random random)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Owner = entity,
				m_Prefab = prefab
			};
			creationDefinition.m_Flags |= CreationFlags.Permanent;
			creationDefinition.m_RandomSeed = ((Random)(ref random)).NextInt();
			ObjectDefinition objectDefinition = new ObjectDefinition
			{
				m_ParentMesh = -1,
				m_Elevation = elevation.m_Elevation,
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation,
				m_LocalPosition = transform.m_Position,
				m_LocalRotation = transform.m_Rotation
			};
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_DefinitionArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CreationDefinition>(jobIndex, val, creationDefinition);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ObjectDefinition>(jobIndex, val, objectDefinition);
			OwnerDefinition ownerDefinition = new OwnerDefinition
			{
				m_Prefab = prefab,
				m_Position = objectDefinition.m_Position,
				m_Rotation = objectDefinition.m_Rotation
			};
			if (m_PrefabSubAreas.HasBuffer(prefab))
			{
				Spawn(jobIndex, ownerDefinition, m_PrefabSubAreas[prefab], m_PrefabSubAreaNodes[prefab], ref random);
			}
			if (m_PrefabSubNets.HasBuffer(prefab))
			{
				Spawn(jobIndex, ownerDefinition, m_PrefabSubNets[prefab], ref random);
			}
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
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Geometry> __Game_Areas_Geometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Storage> __Game_Areas_Storage_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Extractor> __Game_Areas_Extractor_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Secondary> __Game_Objects_Secondary_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> __Game_Prefabs_StorageAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

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
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Areas_Geometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Geometry>(true);
			__Game_Areas_Storage_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Storage>(true);
			__Game_Areas_Extractor_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Extractor>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Secondary_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Secondary>(true);
			__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_StorageAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageAreaData>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_AreaQuery;

	private EntityArchetype m_DefinitionArchetype;

	private TypeHandle __TypeHandle;

	public bool debugFastSpawn { get; set; }

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
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Geometry>(),
			ComponentType.ReadOnly<Game.Objects.SubObject>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DefinitionArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<CreationDefinition>(),
			ComponentType.ReadWrite<ObjectDefinition>(),
			ComponentType.ReadWrite<Updated>(),
			ComponentType.ReadWrite<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_AreaQuery);
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
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		AreaSpawnJob areaSpawnJob = new AreaSpawnJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GeometryType = InternalCompilerInterface.GetComponentTypeHandle<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StorageType = InternalCompilerInterface.GetComponentTypeHandle<Storage>(ref __TypeHandle.__Game_Areas_Storage_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorType = InternalCompilerInterface.GetComponentTypeHandle<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryData = InternalCompilerInterface.GetComponentLookup<Secondary>(ref __TypeHandle.__Game_Objects_Secondary_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyData = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStorageAreaData = InternalCompilerInterface.GetComponentLookup<StorageAreaData>(ref __TypeHandle.__Game_Prefabs_StorageAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabExtractorAreaData = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingRenters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderObjectElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectRequirements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DebugFastSpawn = debugFastSpawn,
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_RandomSeed = RandomSeed.Next(),
			m_DefinitionArchetype = m_DefinitionArchetype,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		areaSpawnJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<AreaSpawnJob>(areaSpawnJob, m_AreaQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
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
	public AreaSpawnSystem()
	{
	}
}
