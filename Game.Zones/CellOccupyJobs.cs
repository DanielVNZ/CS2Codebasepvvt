using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Zones;

public static class CellOccupyJobs
{
	[BurstCompile]
	public struct ZoneAndOccupyCellsJob : IJobParallelForDefer
	{
		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_BlockEntity;

			public Block m_BlockData;

			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public int4 m_Xxzz;

			public DynamicBuffer<Cell> m_Cells;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<Elevation> m_ElevationData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

			public ComponentLookup<SignatureBuildingData> m_PrefabSignatureBuildingData;

			public ComponentLookup<PlaceholderBuildingData> m_PrefabPlaceholderBuildingData;

			public ComponentLookup<ZoneData> m_PrefabZoneData;

			public ComponentLookup<PrefabData> m_PrefabData;

			private bool m_ShouldOverride;

			private ZoneType m_OverrideZone;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				if ((bounds.m_Mask & (BoundsMask.OccupyZone | BoundsMask.NotOverridden)) != (BoundsMask.OccupyZone | BoundsMask.NotOverridden))
				{
					return false;
				}
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_04be: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04db: Unknown result type (might be due to invalid IL or missing references)
				//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0504: Unknown result type (might be due to invalid IL or missing references)
				//IL_050b: Unknown result type (might be due to invalid IL or missing references)
				//IL_050d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0512: Unknown result type (might be due to invalid IL or missing references)
				//IL_0514: Unknown result type (might be due to invalid IL or missing references)
				//IL_0516: Unknown result type (might be due to invalid IL or missing references)
				//IL_051b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0521: Unknown result type (might be due to invalid IL or missing references)
				//IL_0527: Unknown result type (might be due to invalid IL or missing references)
				//IL_052c: Unknown result type (might be due to invalid IL or missing references)
				//IL_052e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0533: Unknown result type (might be due to invalid IL or missing references)
				//IL_0535: Unknown result type (might be due to invalid IL or missing references)
				//IL_0537: Unknown result type (might be due to invalid IL or missing references)
				//IL_053c: Unknown result type (might be due to invalid IL or missing references)
				//IL_054d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0552: Unknown result type (might be due to invalid IL or missing references)
				//IL_055f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0566: Unknown result type (might be due to invalid IL or missing references)
				//IL_040f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0419: Unknown result type (might be due to invalid IL or missing references)
				//IL_0423: Unknown result type (might be due to invalid IL or missing references)
				//IL_0428: Unknown result type (might be due to invalid IL or missing references)
				//IL_042d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0431: Unknown result type (might be due to invalid IL or missing references)
				//IL_0445: Unknown result type (might be due to invalid IL or missing references)
				//IL_0450: Unknown result type (might be due to invalid IL or missing references)
				//IL_0455: Unknown result type (might be due to invalid IL or missing references)
				//IL_0459: Unknown result type (might be due to invalid IL or missing references)
				//IL_045b: Unknown result type (might be due to invalid IL or missing references)
				//IL_046d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0478: Unknown result type (might be due to invalid IL or missing references)
				//IL_047d: Unknown result type (might be due to invalid IL or missing references)
				//IL_048e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0493: Unknown result type (might be due to invalid IL or missing references)
				//IL_0574: Unknown result type (might be due to invalid IL or missing references)
				//IL_0579: Unknown result type (might be due to invalid IL or missing references)
				//IL_057c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0583: Unknown result type (might be due to invalid IL or missing references)
				//IL_0589: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03da: Unknown result type (might be due to invalid IL or missing references)
				//IL_03df: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				//IL_0304: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_030b: Unknown result type (might be due to invalid IL or missing references)
				//IL_030d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0312: Unknown result type (might be due to invalid IL or missing references)
				//IL_0319: Unknown result type (might be due to invalid IL or missing references)
				//IL_031b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0320: Unknown result type (might be due to invalid IL or missing references)
				//IL_0322: Unknown result type (might be due to invalid IL or missing references)
				//IL_0324: Unknown result type (might be due to invalid IL or missing references)
				//IL_0329: Unknown result type (might be due to invalid IL or missing references)
				//IL_032e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0331: Unknown result type (might be due to invalid IL or missing references)
				//IL_0336: Unknown result type (might be due to invalid IL or missing references)
				//IL_0338: Unknown result type (might be due to invalid IL or missing references)
				//IL_033d: Unknown result type (might be due to invalid IL or missing references)
				//IL_033f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0341: Unknown result type (might be due to invalid IL or missing references)
				//IL_0346: Unknown result type (might be due to invalid IL or missing references)
				//IL_0357: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0369: Unknown result type (might be due to invalid IL or missing references)
				//IL_0370: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_0219: Unknown result type (might be due to invalid IL or missing references)
				//IL_0228: Unknown result type (might be due to invalid IL or missing references)
				//IL_0233: Unknown result type (might be due to invalid IL or missing references)
				//IL_0238: Unknown result type (might be due to invalid IL or missing references)
				//IL_023c: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0257: Unknown result type (might be due to invalid IL or missing references)
				//IL_025c: Unknown result type (might be due to invalid IL or missing references)
				//IL_026d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0272: Unknown result type (might be due to invalid IL or missing references)
				//IL_037e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0383: Unknown result type (might be due to invalid IL or missing references)
				//IL_0386: Unknown result type (might be due to invalid IL or missing references)
				//IL_038d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0393: Unknown result type (might be due to invalid IL or missing references)
				//IL_0280: Unknown result type (might be due to invalid IL or missing references)
				//IL_0285: Unknown result type (might be due to invalid IL or missing references)
				//IL_0288: Unknown result type (might be due to invalid IL or missing references)
				//IL_028d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0290: Unknown result type (might be due to invalid IL or missing references)
				if ((bounds.m_Mask & (BoundsMask.OccupyZone | BoundsMask.NotOverridden)) != (BoundsMask.OccupyZone | BoundsMask.NotOverridden) || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					return;
				}
				bool flag = false;
				if (m_ElevationData.HasComponent(objectEntity))
				{
					Elevation elevation = m_ElevationData[objectEntity];
					if (elevation.m_Elevation < 0f)
					{
						return;
					}
					flag = elevation.m_Elevation > 0f;
				}
				PrefabRef prefabRef = m_PrefabRefData[objectEntity];
				Transform transform = m_TransformData[objectEntity];
				if (!m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				m_ShouldOverride = (objectGeometryData.m_Flags & GeometryFlags.OverrideZone) != 0;
				flag &= (objectGeometryData.m_Flags & GeometryFlags.BaseCollision) == 0;
				m_OverrideZone = ZoneType.None;
				SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
				PlaceholderBuildingData placeholderBuildingData = default(PlaceholderBuildingData);
				ZoneData zoneData2 = default(ZoneData);
				if (m_PrefabSpawnableBuildingData.TryGetComponent(prefabRef.m_Prefab, ref spawnableBuildingData))
				{
					ZoneData zoneData = default(ZoneData);
					if (m_PrefabSignatureBuildingData.HasComponent(prefabRef.m_Prefab) && m_PrefabZoneData.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zoneData) && m_PrefabData.IsComponentEnabled(spawnableBuildingData.m_ZonePrefab))
					{
						m_OverrideZone = zoneData.m_ZoneType;
					}
				}
				else if (m_PrefabPlaceholderBuildingData.TryGetComponent(prefabRef.m_Prefab, ref placeholderBuildingData) && m_PrefabZoneData.TryGetComponent(placeholderBuildingData.m_ZonePrefab, ref zoneData2) && m_PrefabData.IsComponentEnabled(placeholderBuildingData.m_ZonePrefab))
				{
					m_OverrideZone = zoneData2.m_ZoneType;
				}
				if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
				{
					objectGeometryData.m_Bounds.min.y = math.max(objectGeometryData.m_Bounds.min.y, 0f);
				}
				if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount))
				{
					Circle2 val2 = default(Circle2);
					Bounds3 val3 = default(Bounds3);
					for (int i = 0; i < legCount; i++)
					{
						float3 standingLegPosition = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, i);
						if ((objectGeometryData.m_Flags & GeometryFlags.CircularLeg) != GeometryFlags.None)
						{
							float3 val = math.max(objectGeometryData.m_LegSize - 0.16f, float3.op_Implicit(0f));
							((Circle2)(ref val2))._002Ector(val.x * 0.5f, ((float3)(ref standingLegPosition)).xz);
							Bounds3 bounds2 = bounds.m_Bounds;
							((Bounds3)(ref bounds2)).xz = MathUtils.Bounds(val2);
							bounds2.min.y = standingLegPosition.y + objectGeometryData.m_Bounds.min.y;
							if (MathUtils.Intersect(m_Quad, val2))
							{
								CheckOverlapX(m_Bounds, bounds2, m_Quad, val2, m_Xxzz, flag);
							}
							continue;
						}
						val3.min = objectGeometryData.m_LegSize * -0.5f + 0.08f;
						val3.max = objectGeometryData.m_LegSize * 0.5f - 0.08f;
						float3 val4 = MathUtils.Center(val3);
						bool3 val5 = val3.min > val3.max;
						val3.min = math.select(val3.min, val4, val5);
						val3.max = math.select(val3.max, val4, val5);
						Quad3 val6 = ObjectUtils.CalculateBaseCorners(standingLegPosition, transform.m_Rotation, val3);
						val3 = MathUtils.Bounds(val6);
						val3.min.y += objectGeometryData.m_Bounds.min.y;
						if (MathUtils.Intersect(m_Quad, ((Quad3)(ref val6)).xz))
						{
							CheckOverlapX(m_Bounds, val3, m_Quad, ((Quad3)(ref val6)).xz, m_Xxzz, flag);
						}
					}
					ref float3 position = ref transform.m_Position;
					position += math.rotate(transform.m_Rotation, new float3(0f, objectGeometryData.m_LegSize.y, 0f));
					objectGeometryData.m_Bounds.min.y = 0f;
					flag = true;
				}
				if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					float3 val7 = math.max(objectGeometryData.m_Size - 0.16f, float3.op_Implicit(0f));
					Circle2 val8 = default(Circle2);
					((Circle2)(ref val8))._002Ector(val7.x * 0.5f, ((float3)(ref transform.m_Position)).xz);
					Bounds3 bounds3 = bounds.m_Bounds;
					((Bounds3)(ref bounds3)).xz = MathUtils.Bounds(val8);
					bounds3.min.y = transform.m_Position.y + objectGeometryData.m_Bounds.min.y;
					if (MathUtils.Intersect(m_Quad, val8))
					{
						CheckOverlapX(m_Bounds, bounds3, m_Quad, val8, m_Xxzz, flag);
					}
					return;
				}
				Bounds3 val9 = MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.08f));
				float3 val10 = MathUtils.Center(val9);
				bool3 val11 = val9.min > val9.max;
				val9.min = math.select(val9.min, val10, val11);
				val9.max = math.select(val9.max, val10, val11);
				Quad3 val12 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, val9);
				val9 = MathUtils.Bounds(val12);
				val9.min.y += objectGeometryData.m_Bounds.min.y;
				if (MathUtils.Intersect(m_Quad, ((Quad3)(ref val12)).xz))
				{
					CheckOverlapX(m_Bounds, val9, m_Quad, ((Quad3)(ref val12)).xz, m_Xxzz, flag);
				}
			}

			private void CheckOverlapX(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz2.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val4, bounds2, val2, quad2, val, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val5, bounds2, val3, quad2, xxzz2, isElevated);
					}
				}
				else
				{
					CheckOverlapZ(bounds1, bounds2, quad1, quad2, xxzz1, isElevated);
				}
			}

			private void CheckOverlapZ(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz2.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val4, bounds2, val2, quad2, val, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val5, bounds2, val3, quad2, xxzz2, isElevated);
					}
					return;
				}
				if (xxzz1.y - xxzz1.x >= 2)
				{
					CheckOverlapX(bounds1, bounds2, quad1, quad2, xxzz1, isElevated);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				Cell cell = m_Cells[num2];
				if ((cell.m_State & CellFlags.Blocked) != CellFlags.None)
				{
					return;
				}
				quad1 = MathUtils.Expand(quad1, -0.01f);
				if (MathUtils.Intersect(quad1, quad2))
				{
					cell.m_State |= CellFlags.Occupied;
					if (m_ShouldOverride && (!m_OverrideZone.Equals(cell.m_Zone) || m_OverrideZone.Equals(ZoneType.None)))
					{
						cell.m_State |= CellFlags.Overridden;
						cell.m_Zone = m_OverrideZone;
					}
					m_Cells[num2] = cell;
				}
			}

			private void CheckOverlapX(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Circle2 circle2, int4 xxzz1, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz2.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val4, bounds2, val2, circle2, val, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val5, bounds2, val3, circle2, xxzz2, isElevated);
					}
				}
				else
				{
					CheckOverlapZ(bounds1, bounds2, quad1, circle2, xxzz1, isElevated);
				}
			}

			private void CheckOverlapZ(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Circle2 circle2, int4 xxzz1, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz2.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val4, bounds2, val2, circle2, val, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val5, bounds2, val3, circle2, xxzz2, isElevated);
					}
					return;
				}
				if (xxzz1.y - xxzz1.x >= 2)
				{
					CheckOverlapX(bounds1, bounds2, quad1, circle2, xxzz1, isElevated);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				Cell cell = m_Cells[num2];
				if ((cell.m_State & CellFlags.Blocked) != CellFlags.None)
				{
					return;
				}
				quad1 = MathUtils.Expand(quad1, -0.01f);
				if (!MathUtils.Intersect(quad1, circle2))
				{
					return;
				}
				if (isElevated)
				{
					cell.m_Height = (short)math.clamp(Mathf.FloorToInt(bounds2.min.y), -32768, math.min((int)cell.m_Height, 32767));
				}
				else
				{
					cell.m_State |= CellFlags.Occupied;
					if (m_ShouldOverride && (!m_OverrideZone.Equals(cell.m_Zone) || m_OverrideZone.Equals(ZoneType.None)))
					{
						cell.m_State |= CellFlags.Overridden;
						cell.m_Zone = m_OverrideZone;
					}
				}
				m_Cells[num2] = cell;
			}
		}

		private struct DeletedBlockIterator
		{
			public Quad2 m_Quad;

			public Bounds2 m_Bounds;

			public Block m_BlockData;

			public ValidArea m_ValidAreaData;

			public DynamicBuffer<Cell> m_Cells;

			public ComponentLookup<Block> m_BlockDataFromEntity;

			public ComponentLookup<ValidArea> m_ValidAreaDataFromEntity;

			public BufferLookup<Cell> m_CellsFromEntity;

			private Block m_BlockData2;

			private ValidArea m_ValidAreaData2;

			private DynamicBuffer<Cell> m_Cells2;

			private NativeArray<float> m_BestDistance;

			public void Iterate(Entity blockEntity2)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				m_ValidAreaData2 = m_ValidAreaDataFromEntity[blockEntity2];
				if (m_ValidAreaData2.m_Area.y <= m_ValidAreaData2.m_Area.x)
				{
					return;
				}
				m_BlockData2 = m_BlockDataFromEntity[blockEntity2];
				if (MathUtils.Intersect(m_Bounds, ZoneUtils.CalculateBounds(m_BlockData2)))
				{
					Quad2 val = ZoneUtils.CalculateCorners(m_BlockData2, m_ValidAreaData2);
					if (MathUtils.Intersect(MathUtils.Expand(m_Quad, -0.01f), MathUtils.Expand(val, -0.01f)))
					{
						m_Cells2 = m_CellsFromEntity[blockEntity2];
						CheckOverlapX1(m_Bounds, MathUtils.Bounds(val), m_Quad, val, m_ValidAreaData.m_Area, m_ValidAreaData2.m_Area);
					}
				}
			}

			public void Dispose()
			{
				if (m_BestDistance.IsCreated)
				{
					m_BestDistance.Dispose();
				}
			}

			private void CheckOverlapX1(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz3 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz3.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapZ1(val4, bounds2, val2, quad2, val, xxzz2);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapZ1(val5, bounds2, val3, quad2, xxzz3, xxzz2);
					}
				}
				else
				{
					CheckOverlapZ1(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapZ1(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz3 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz3.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapX2(val4, bounds2, val2, quad2, val, xxzz2);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapX2(val5, bounds2, val3, quad2, xxzz3, xxzz2);
					}
				}
				else
				{
					CheckOverlapX2(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapX2(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz2.y - xxzz2.x >= 2)
				{
					int4 val = xxzz2;
					int4 xxzz3 = xxzz2;
					val.y = xxzz2.x + xxzz2.y >> 1;
					xxzz3.x = val.y;
					Quad2 val2 = quad2;
					Quad2 val3 = quad2;
					float num = (float)(val.y - xxzz2.x) / (float)(xxzz2.y - xxzz2.x);
					val2.b = math.lerp(quad2.a, quad2.b, num);
					val2.c = math.lerp(quad2.d, quad2.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(bounds1, val4))
					{
						CheckOverlapZ2(bounds1, val4, quad1, val2, xxzz1, val);
					}
					if (MathUtils.Intersect(bounds1, val5))
					{
						CheckOverlapZ2(bounds1, val5, quad1, val3, xxzz1, xxzz3);
					}
				}
				else
				{
					CheckOverlapZ2(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapZ2(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0256: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz2.w - xxzz2.z >= 2)
				{
					int4 val = xxzz2;
					int4 xxzz3 = xxzz2;
					val.w = xxzz2.z + xxzz2.w >> 1;
					xxzz3.z = val.w;
					Quad2 val2 = quad2;
					Quad2 val3 = quad2;
					float num = (float)(val.w - xxzz2.z) / (float)(xxzz2.w - xxzz2.z);
					val2.d = math.lerp(quad2.a, quad2.d, num);
					val2.c = math.lerp(quad2.b, quad2.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(bounds1, val4))
					{
						CheckOverlapX1(bounds1, val4, quad1, val2, xxzz1, val);
					}
					if (MathUtils.Intersect(bounds1, val5))
					{
						CheckOverlapX1(bounds1, val5, quad1, val3, xxzz1, xxzz3);
					}
					return;
				}
				if (math.any(((int4)(ref xxzz1)).yw - ((int4)(ref xxzz1)).xz >= 2) | math.any(((int4)(ref xxzz2)).yw - ((int4)(ref xxzz2)).xz >= 2))
				{
					CheckOverlapX1(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				int num3 = xxzz2.z * m_BlockData2.m_Size.x + xxzz2.x;
				Cell cell = m_Cells[num2];
				Cell cell2 = m_Cells2[num3];
				if ((cell.m_State & CellFlags.Blocked) != CellFlags.None || (cell2.m_State & (CellFlags.Shared | CellFlags.Visible | CellFlags.Overridden)) != CellFlags.Visible || !cell.m_Zone.Equals(ZoneType.None) || cell2.m_Zone.Equals(ZoneType.None))
				{
					return;
				}
				float num4 = math.lengthsq(MathUtils.Center(quad1) - MathUtils.Center(quad2));
				if (num4 > 32f)
				{
					return;
				}
				if (m_BestDistance.IsCreated)
				{
					if (m_BestDistance[num2] <= num4)
					{
						return;
					}
				}
				else
				{
					m_BestDistance = new NativeArray<float>(m_BlockData.m_Size.x * m_BlockData.m_Size.y, (Allocator)2, (NativeArrayOptions)0);
					for (int i = 0; i < m_BestDistance.Length; i++)
					{
						m_BestDistance[i] = float.MaxValue;
					}
				}
				cell.m_Zone = cell2.m_Zone;
				m_Cells[num2] = cell;
				m_BestDistance[num2] = num4;
			}
		}

		[ReadOnly]
		public NativeArray<CellCheckHelpers.SortedEntity> m_Blocks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DeletedBlockChunks;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_PrefabSpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> m_PrefabSignatureBuildingData;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> m_PrefabPlaceholderBuildingData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_PrefabZoneData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Cell> m_Cells;

		public void Execute(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = m_Blocks[index].m_Entity;
			Block block = m_BlockData[entity];
			ValidArea validArea = m_ValidAreaData[entity];
			DynamicBuffer<Cell> cells = m_Cells[entity];
			ClearOverrideStatus(block, cells);
			if (validArea.m_Area.y <= validArea.m_Area.x)
			{
				return;
			}
			Quad2 quad = ZoneUtils.CalculateCorners(block, validArea);
			Bounds2 bounds = ZoneUtils.CalculateBounds(block);
			DeletedBlockIterator deletedBlockIterator = new DeletedBlockIterator
			{
				m_Quad = quad,
				m_Bounds = bounds,
				m_BlockData = block,
				m_ValidAreaData = validArea,
				m_Cells = cells,
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_CellsFromEntity = m_Cells
			};
			for (int i = 0; i < m_DeletedBlockChunks.Length; i++)
			{
				ArchetypeChunk val = m_DeletedBlockChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					deletedBlockIterator.Iterate(nativeArray[j]);
				}
			}
			deletedBlockIterator.Dispose();
			ObjectIterator objectIterator = new ObjectIterator
			{
				m_BlockEntity = entity,
				m_BlockData = block,
				m_Bounds = bounds,
				m_Quad = quad,
				m_Xxzz = validArea.m_Area,
				m_Cells = cells,
				m_TransformData = m_TransformData,
				m_ElevationData = m_ElevationData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_PrefabSpawnableBuildingData = m_PrefabSpawnableBuildingData,
				m_PrefabSignatureBuildingData = m_PrefabSignatureBuildingData,
				m_PrefabPlaceholderBuildingData = m_PrefabPlaceholderBuildingData,
				m_PrefabZoneData = m_PrefabZoneData,
				m_PrefabData = m_PrefabData
			};
			m_ObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
			SetOccupiedWithHeight(block, cells);
		}

		private static void ClearOverrideStatus(Block blockData, DynamicBuffer<Cell> cells)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < blockData.m_Size.y; i++)
			{
				for (int j = 0; j < blockData.m_Size.x; j++)
				{
					int num = i * blockData.m_Size.x + j;
					Cell cell = cells[num];
					if ((cell.m_State & CellFlags.Overridden) != CellFlags.None)
					{
						cell.m_State &= ~CellFlags.Overridden;
						cell.m_Zone = ZoneType.None;
						cells[num] = cell;
					}
				}
			}
		}

		private void SetOccupiedWithHeight(Block blockData, DynamicBuffer<Cell> cells)
		{
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < blockData.m_Size.y; i++)
			{
				for (int j = 0; j < blockData.m_Size.x; j++)
				{
					int num = i * blockData.m_Size.x + j;
					Cell cell = cells[num];
					if ((cell.m_State & CellFlags.Occupied) == 0 && cell.m_Height < short.MaxValue)
					{
						Entity val = m_ZonePrefabs[cell.m_Zone];
						ZoneData zoneData = m_PrefabZoneData[val];
						if ((float)math.max((int)zoneData.m_MinOddHeight, (int)zoneData.m_MinEvenHeight) > (float)cell.m_Height - blockData.m_Position.y)
						{
							cell.m_State |= CellFlags.Occupied;
							cells[num] = cell;
						}
					}
				}
			}
		}
	}
}
