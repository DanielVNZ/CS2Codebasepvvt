using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class AttachSystem : GameSystemBase
{
	public struct RemovedAttached
	{
		public Entity m_Entity;

		public Entity m_Parent;
	}

	[BurstCompile]
	private struct FindAttachedParentsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_AttachedChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_ParentChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> m_PlaceholderBuildingData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Attached> m_AttachedData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_AttachedChunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Temp>(ref m_TempType);
			bool flag = nativeArray5.Length != 0;
			bool flag2 = nativeArray3.Length != 0;
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val2 = nativeArray[i];
				Attached attached = m_AttachedData[val2];
				if (attached.m_Parent != Entity.Null)
				{
					if (m_PlaceholderBuildingData.HasComponent(attached.m_Parent))
					{
						attached.m_Parent = FindParent(attached.m_Parent, nativeArray2[i], flag);
						m_AttachedData[val2] = attached;
					}
					continue;
				}
				Attached originalAttached = default(Attached);
				bool relocating = false;
				if (flag)
				{
					Temp temp = nativeArray5[i];
					relocating = (temp.m_Flags & TempFlags.Modify) != 0;
					if (m_AttachedData.TryGetComponent(temp.m_Original, ref originalAttached) && !m_HiddenData.HasComponent(originalAttached.m_Parent))
					{
						attached.m_Parent = originalAttached.m_Parent;
						attached.m_CurvePosition = originalAttached.m_CurvePosition;
						m_AttachedData[val2] = attached;
						continue;
					}
				}
				PrefabRef prefabRef = nativeArray4[i];
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				if (m_PlaceableObjectData.HasComponent(prefabRef.m_Prefab))
				{
					placeableObjectData = m_PlaceableObjectData[prefabRef.m_Prefab];
					if ((placeableObjectData.m_Flags & (PlacementFlags.RoadSide | PlacementFlags.OwnerSide | PlacementFlags.Shoreline | PlacementFlags.Floating)) != PlacementFlags.None)
					{
						continue;
					}
				}
				if (!m_ObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					continue;
				}
				if ((objectGeometryData.m_Flags & GeometryFlags.OptionalAttach) != GeometryFlags.None)
				{
					originalAttached = default(Attached);
				}
				Entity val3 = val2;
				Entity ignoreParent = Entity.Null;
				if (flag2)
				{
					ignoreParent = nativeArray3[i].m_Owner;
					if (!m_BuildingData.HasComponent(val2) && (objectGeometryData.m_Flags & GeometryFlags.OptionalAttach) == 0)
					{
						val3 = nativeArray3[i].m_Owner;
						while (m_OwnerData.HasComponent(val3) && !m_BuildingData.HasComponent(val3))
						{
							val3 = m_OwnerData[val3].m_Owner;
						}
					}
				}
				Transform transform = nativeArray2[i];
				if (FindParent(ref attached, originalAttached, transform, flag, relocating, val3, ignoreParent, objectGeometryData, placeableObjectData.m_Flags))
				{
					m_AttachedData[val2] = attached;
				}
			}
		}

		private Entity FindParent(Entity prefab, Transform transform, bool isTemp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			Entity result = Entity.Null;
			for (int i = 0; i < m_ParentChunks.Length; i++)
			{
				ArchetypeChunk val = m_ParentChunks[i];
				if (isTemp != ((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (!(nativeArray3[j].m_Prefab != prefab))
					{
						Transform transform2 = nativeArray2[j];
						float num2 = math.distance(transform.m_Position, transform2.m_Position);
						if (num2 < num)
						{
							num = num2;
							result = nativeArray[j];
						}
					}
				}
			}
			return result;
		}

		private bool FindParent(ref Attached attached, Attached originalAttached, Transform transform, bool isTemp, bool relocating, Entity topLevelOwner, Entity ignoreParent, ObjectGeometryData objectGeometryData, PlacementFlags placementFlags)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0880: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, objectGeometryData);
			float2 val2 = default(float2);
			((float2)(ref val2))._002Ector(0f, 10f);
			bool result = false;
			Temp temp = default(Temp);
			Bounds3 val6 = default(Bounds3);
			Temp temp2 = default(Temp);
			float curvePosition = default(float);
			float curvePosition2 = default(float);
			float curvePosition3 = default(float);
			float curvePosition4 = default(float);
			for (int i = 0; i < m_ParentChunks.Length; i++)
			{
				ArchetypeChunk val3 = m_ParentChunks[i];
				NativeArray<Temp> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref m_TempType);
				bool flag = nativeArray.Length != 0;
				if (isTemp != flag)
				{
					continue;
				}
				if ((placementFlags & PlacementFlags.RoadNode) != PlacementFlags.None)
				{
					NativeArray<Node> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Node>(ref m_NodeType);
					if (nativeArray2.Length != 0)
					{
						NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_EntityType);
						NativeArray<Game.Net.Elevation> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Game.Net.Elevation>(ref m_ElevationType);
						NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
						for (int j = 0; j < nativeArray2.Length; j++)
						{
							Entity val4 = nativeArray3[j];
							if (val4 == ignoreParent || (CollectionUtils.TryGet<Temp>(nativeArray, j, ref temp) && (temp.m_Flags & TempFlags.Delete) != 0))
							{
								continue;
							}
							PrefabRef prefabRef = nativeArray5[j];
							if (!m_NetGeometryData.HasComponent(prefabRef.m_Prefab))
							{
								continue;
							}
							NetGeometryData netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
							int num = 0;
							EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, val4, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
							while (true)
							{
								if (edgeIterator.GetNext(out var value))
								{
									Entity edge = value.m_Edge;
									if (edge == ignoreParent)
									{
										Edge edge2 = m_EdgeData[edge];
										if (edge2.m_Start == val4 || edge2.m_End == val4)
										{
											break;
										}
									}
									PrefabRef prefabRef2 = m_PrefabRefData[edge];
									if ((m_NetGeometryData[prefabRef2.m_Prefab].m_MergeLayers & netGeometryData.m_MergeLayers) != Layer.None)
									{
										num++;
									}
									continue;
								}
								if ((placementFlags & (PlacementFlags.RoadNode | PlacementFlags.RoadEdge)) != PlacementFlags.RoadNode && num < 3)
								{
									break;
								}
								Node node = nativeArray2[j];
								Entity val5 = val4;
								if ((objectGeometryData.m_Flags & GeometryFlags.OptionalAttach) == 0)
								{
									while (m_OwnerData.HasComponent(val5) && !m_BuildingData.HasComponent(val5))
									{
										val5 = m_OwnerData[val5].m_Owner;
									}
								}
								float num2 = math.select(val2.x, val2.y, topLevelOwner == val5);
								float num3 = netGeometryData.m_DefaultWidth * 0.5f;
								val6.min = node.m_Position - new float3(num3, 0f - netGeometryData.m_DefaultHeightRange.min, num3);
								val6.max = node.m_Position + new float3(num3, netGeometryData.m_DefaultHeightRange.max, num3);
								if (num2 > 0f)
								{
									ref float3 min = ref val6.min;
									min -= num2;
									ref float3 max = ref val6.max;
									max += num2;
								}
								if ((objectGeometryData.m_Flags & GeometryFlags.BaseCollision) != GeometryFlags.None)
								{
									((Bounds3)(ref val)).y = ((Bounds3)(ref val6)).y;
									transform.m_Position.y = node.m_Position.y;
									if (nativeArray4.Length != 0)
									{
										float num4 = math.csum(nativeArray4[j].m_Elevation) * 0.5f;
										((Bounds3)(ref val)).y = ((Bounds3)(ref val)).y - num4;
										transform.m_Position.y -= num4;
									}
								}
								if (MathUtils.Intersect(val6, val))
								{
									float num5 = math.distance(node.m_Position, transform.m_Position) - num3;
									num5 += math.clamp(num5, 0f - num3, 0f);
									if (num5 < num2)
									{
										val2 = math.min(val2, float2.op_Implicit(num5));
										attached.m_Parent = val4;
										attached.m_CurvePosition = 0f;
										result = true;
									}
								}
								if (flag && !relocating && originalAttached.m_Parent != Entity.Null && attached.m_Parent == Entity.Null && temp.m_Original == originalAttached.m_Parent)
								{
									attached.m_Parent = val4;
									attached.m_CurvePosition = 0f;
									result = true;
								}
								break;
							}
						}
					}
				}
				if ((placementFlags & (PlacementFlags.RoadNode | PlacementFlags.RoadEdge)) == PlacementFlags.RoadNode)
				{
					continue;
				}
				NativeArray<Edge> nativeArray6 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Edge>(ref m_EdgeType);
				if (nativeArray6.Length == 0)
				{
					continue;
				}
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref val3)).GetNativeArray(m_EntityType);
				NativeArray<Curve> nativeArray8 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Game.Net.Elevation> nativeArray9 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Game.Net.Elevation>(ref m_ElevationType);
				NativeArray<PrefabRef> nativeArray10 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int k = 0; k < nativeArray6.Length; k++)
				{
					Entity val7 = nativeArray7[k];
					if (val7 == ignoreParent)
					{
						continue;
					}
					Edge edge3 = nativeArray6[k];
					if (edge3.m_Start == ignoreParent || edge3.m_End == ignoreParent || (CollectionUtils.TryGet<Temp>(nativeArray, k, ref temp2) && (temp2.m_Flags & TempFlags.Delete) != 0))
					{
						continue;
					}
					PrefabRef prefabRef3 = nativeArray10[k];
					if (!m_NetGeometryData.HasComponent(prefabRef3.m_Prefab))
					{
						continue;
					}
					Curve curve = nativeArray8[k];
					NetGeometryData netGeometryData2 = m_NetGeometryData[prefabRef3.m_Prefab];
					Entity val8 = val7;
					if ((objectGeometryData.m_Flags & GeometryFlags.OptionalAttach) == 0)
					{
						while (m_OwnerData.HasComponent(val8) && !m_BuildingData.HasComponent(val8))
						{
							val8 = m_OwnerData[val8].m_Owner;
						}
					}
					float num6 = math.select(val2.x, val2.y, topLevelOwner == val8);
					float num7 = netGeometryData2.m_DefaultWidth * 0.5f;
					Bounds3 val9 = MathUtils.Bounds(curve.m_Bezier);
					val9.min -= new float3(num7, 0f - netGeometryData2.m_DefaultHeightRange.min, num7);
					val9.max += new float3(num7, netGeometryData2.m_DefaultHeightRange.max, num7);
					if (num6 > 0f)
					{
						ref float3 min2 = ref val9.min;
						min2 -= num6;
						ref float3 max2 = ref val9.max;
						max2 += num6;
					}
					if ((placementFlags & PlacementFlags.Waterway) != PlacementFlags.None)
					{
						if ((netGeometryData2.m_Flags & Game.Net.GeometryFlags.OnWater) == 0)
						{
							continue;
						}
						if (MathUtils.Intersect(val9, val))
						{
							float num8 = MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref transform.m_Position)).xz, ref curvePosition) - num7;
							if (num8 < num6)
							{
								val2 = math.min(val2, float2.op_Implicit(num8));
								attached.m_Parent = val7;
								attached.m_CurvePosition = curvePosition;
								result = true;
							}
						}
					}
					else
					{
						if ((netGeometryData2.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
						{
							continue;
						}
						if ((objectGeometryData.m_Flags & GeometryFlags.BaseCollision) != GeometryFlags.None)
						{
							((Bounds3)(ref val)).y = ((Bounds3)(ref val9)).y;
							float num9 = 0f;
							if (nativeArray9.Length != 0)
							{
								num9 = math.csum(nativeArray9[k].m_Elevation) * 0.5f;
								if (num9 > 0f && (netGeometryData2.m_Flags & Game.Net.GeometryFlags.RaisedIsElevated) == 0 && (!math.all(nativeArray9[k].m_Elevation >= netGeometryData2.m_ElevationLimit * 2f) || (netGeometryData2.m_Flags & Game.Net.GeometryFlags.ElevatedIsRaised) != 0) && (netGeometryData2.m_Flags & Game.Net.GeometryFlags.RequireElevated) == 0)
								{
									num9 = 0f;
								}
								((Bounds3)(ref val)).y = ((Bounds3)(ref val)).y - num9;
							}
							if (MathUtils.Intersect(val9, val))
							{
								float num10 = MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref transform.m_Position)).xz, ref curvePosition2);
								num10 = math.length(new float2(num9, num10)) - num7;
								if (num10 < num6)
								{
									val2 = math.min(val2, float2.op_Implicit(num10));
									attached.m_Parent = val7;
									attached.m_CurvePosition = curvePosition2;
									result = true;
								}
							}
						}
						else if (MathUtils.Intersect(val9, val))
						{
							float num11 = MathUtils.Distance(curve.m_Bezier, transform.m_Position, ref curvePosition3) - num7;
							if (num11 < num6)
							{
								val2 = math.min(val2, float2.op_Implicit(num11));
								attached.m_Parent = val7;
								attached.m_CurvePosition = curvePosition3;
								result = true;
							}
						}
					}
					if (flag && !relocating && originalAttached.m_Parent != Entity.Null && attached.m_Parent == Entity.Null && temp2.m_Original == originalAttached.m_Parent)
					{
						MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref transform.m_Position)).xz, ref curvePosition4);
						attached.m_Parent = val7;
						attached.m_CurvePosition = curvePosition4;
						result = true;
					}
				}
			}
			return result;
		}
	}

	[BurstCompile]
	private struct UpdateAttachedReferencesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_AttachedChunks;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Recent> m_RecentData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameterData;

		public BufferLookup<SubObject> m_SubObjects;

		public ComponentLookup<Attachment> m_AttachmentData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			NativeHashMap<Entity, Entity> newAttachments = default(NativeHashMap<Entity, Entity>);
			newAttachments._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_AttachedChunks.Length; i++)
			{
				Execute(m_AttachedChunks[i], newAttachments);
			}
			if (newAttachments.Count != 0)
			{
				NativeArray<Entity> keyArray = newAttachments.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int j = 0; j < keyArray.Length; j++)
				{
					Entity val = keyArray[j];
					Entity val2 = newAttachments[val];
					if (val2 != Entity.Null)
					{
						if (m_AttachmentData.HasComponent(val))
						{
							m_AttachmentData[val] = new Attachment(val2);
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Attachment>(val, new Attachment(val2));
						}
					}
					else if (m_AttachmentData.HasComponent(val))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Attachment>(val);
					}
				}
				keyArray.Dispose();
			}
			newAttachments.Dispose();
		}

		private void Execute(ArchetypeChunk chunk, NativeHashMap<Entity, Entity> newAttachments)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Attached> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Attached>(ref m_AttachedType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				DynamicBuffer<SubObject> val2 = default(DynamicBuffer<SubObject>);
				DynamicBuffer<SubObject> val3 = default(DynamicBuffer<SubObject>);
				Entity attached = default(Entity);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					ref Attached reference = ref CollectionUtils.ElementAt<Attached>(nativeArray2, i);
					if (reference.m_OldParent != Entity.Null && reference.m_OldParent != reference.m_Parent && m_SubObjects.TryGetBuffer(reference.m_OldParent, ref val2))
					{
						CollectionUtils.RemoveValue<SubObject>(val2, new SubObject(val));
					}
					reference.m_OldParent = Entity.Null;
					if (reference.m_Parent == val)
					{
						reference.m_Parent = Entity.Null;
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Attached>(val);
						continue;
					}
					if (m_SubObjects.TryGetBuffer(reference.m_Parent, ref val3))
					{
						CollectionUtils.RemoveValue<SubObject>(val3, new SubObject(val));
					}
					if (!m_PlaceholderData.HasComponent(reference.m_Parent))
					{
						continue;
					}
					if (newAttachments.TryGetValue(reference.m_Parent, ref attached))
					{
						if (attached == val)
						{
							newAttachments.Remove(reference.m_Parent);
							newAttachments.TryAdd(reference.m_Parent, Entity.Null);
						}
					}
					else if (m_AttachmentData.HasComponent(reference.m_Parent))
					{
						attached = m_AttachmentData[reference.m_Parent].m_Attached;
						if (attached == val)
						{
							newAttachments.TryAdd(reference.m_Parent, Entity.Null);
						}
					}
				}
				return;
			}
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			if (nativeArray3.Length != 0)
			{
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType);
				DynamicBuffer<SubObject> val5 = default(DynamicBuffer<SubObject>);
				Temp temp2 = default(Temp);
				DynamicBuffer<SubObject> val6 = default(DynamicBuffer<SubObject>);
				Recent recent = default(Recent);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity val4 = nativeArray[j];
					ref Attached reference2 = ref CollectionUtils.ElementAt<Attached>(nativeArray2, j);
					Temp temp = nativeArray3[j];
					PrefabRef prefabRef = nativeArray4[j];
					if (reference2.m_OldParent != Entity.Null && reference2.m_OldParent != reference2.m_Parent && m_SubObjects.TryGetBuffer(reference2.m_OldParent, ref val5))
					{
						CollectionUtils.RemoveValue<SubObject>(val5, new SubObject(val4));
					}
					reference2.m_OldParent = Entity.Null;
					if (reference2.m_Parent == val4)
					{
						reference2.m_Parent = Entity.Null;
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Attached>(val4);
						continue;
					}
					bool flag2 = reference2.m_Parent != Entity.Null;
					if (m_TempData.TryGetComponent(reference2.m_Parent, ref temp2))
					{
						flag2 = (temp2.m_Flags & TempFlags.Delete) == 0;
						if (m_SubObjects.TryGetBuffer(reference2.m_Parent, ref val6))
						{
							CollectionUtils.TryAddUniqueValue<SubObject>(val6, new SubObject(val4));
						}
						if (m_PlaceholderData.HasComponent(reference2.m_Parent))
						{
							newAttachments.Remove(reference2.m_Parent);
							newAttachments.TryAdd(reference2.m_Parent, val4);
						}
					}
					if (flag || flag2 || (temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Upgrade | TempFlags.Parent | TempFlags.Duplicate)) != 0)
					{
						continue;
					}
					ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
					if (m_ObjectGeometryData.HasComponent(prefabRef.m_Prefab))
					{
						objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
					}
					if ((objectGeometryData.m_Flags & GeometryFlags.OptionalAttach) == 0)
					{
						temp.m_Flags |= TempFlags.Delete;
						if (m_RecentData.TryGetComponent(temp.m_Original, ref recent))
						{
							temp.m_Cost = -ObjectUtils.GetRefundAmount(recent, m_SimulationFrame, m_EconomyParameterData);
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(val4, temp);
					}
				}
				return;
			}
			DynamicBuffer<SubObject> val8 = default(DynamicBuffer<SubObject>);
			DynamicBuffer<SubObject> val9 = default(DynamicBuffer<SubObject>);
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				Entity val7 = nativeArray[k];
				ref Attached reference3 = ref CollectionUtils.ElementAt<Attached>(nativeArray2, k);
				if (reference3.m_OldParent != Entity.Null && reference3.m_OldParent != reference3.m_Parent && m_SubObjects.TryGetBuffer(reference3.m_OldParent, ref val8))
				{
					CollectionUtils.RemoveValue<SubObject>(val8, new SubObject(val7));
				}
				reference3.m_OldParent = Entity.Null;
				if (reference3.m_Parent == val7)
				{
					reference3.m_Parent = Entity.Null;
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Attached>(val7);
					continue;
				}
				if (m_SubObjects.TryGetBuffer(reference3.m_Parent, ref val9))
				{
					CollectionUtils.TryAddUniqueValue<SubObject>(val9, new SubObject(val7));
				}
				if (m_PlaceholderData.HasComponent(reference3.m_Parent))
				{
					newAttachments.Remove(reference3.m_Parent);
					newAttachments.TryAdd(reference3.m_Parent, val7);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		public ComponentLookup<Attached> __Game_Objects_Attached_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Recent> __Game_Tools_Recent_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Placeholder> __Game_Objects_Placeholder_RO_ComponentLookup;

		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RW_ComponentTypeHandle;

		public BufferLookup<SubObject> __Game_Objects_SubObject_RW_BufferLookup;

		public ComponentLookup<Attachment> __Game_Objects_Attachment_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Elevation>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderBuildingData>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_Attached_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(false);
			__Game_Tools_Recent_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Recent>(true);
			__Game_Objects_Placeholder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Placeholder>(true);
			__Game_Objects_Attached_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(false);
			__Game_Objects_SubObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(false);
			__Game_Objects_Attachment_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(false);
		}
	}

	private ModificationBarrier3 m_ModificationBarrier;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_EconomyParameterQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier3>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Attached>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SubObject>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Placeholder>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array2[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ObjectQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> val = ((EntityQuery)(ref m_ObjectQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle val3 = JobHandle.CombineDependencies(val2, ((SystemBase)this).Dependency);
		if (!((EntityQuery)(ref m_TempQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val4 = default(JobHandle);
			NativeList<ArchetypeChunk> parentChunks = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindAttachedParentsJob, ArchetypeChunk>(new FindAttachedParentsJob
			{
				m_AttachedChunks = val.AsDeferredJobArray(),
				m_ParentChunks = parentChunks,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderBuildingData = InternalCompilerInterface.GetComponentLookup<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			}, val, 1, JobHandle.CombineDependencies(val3, val4));
			parentChunks.Dispose(val5);
			val3 = val5;
		}
		JobHandle val6 = IJobExtensions.Schedule<UpdateAttachedReferencesJob>(new UpdateAttachedReferencesJob
		{
			m_AttachedChunks = val,
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RecentData = InternalCompilerInterface.GetComponentLookup<Recent>(ref __TypeHandle.__Game_Tools_Recent_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderData = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, val3);
		val.Dispose(val6);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val6);
		((SystemBase)this).Dependency = val6;
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
	public AttachSystem()
	{
	}
}
