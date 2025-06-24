using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Objects;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public static class ClearAreaHelpers
{
	public static void FillClearAreas(DynamicBuffer<InstalledUpgrade> installedUpgrades, Entity ignoreUpgradeOrArea, ComponentLookup<Transform> transformData, ComponentLookup<Clear> clearAreaData, ComponentLookup<PrefabRef> prefabRefData, ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, BufferLookup<Game.Areas.SubArea> subAreaBuffers, BufferLookup<Node> nodeBuffers, BufferLookup<Triangle> triangleBuffers, ref NativeList<ClearAreaData> clearAreas)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
		for (int i = 0; i < installedUpgrades.Length; i++)
		{
			Entity upgrade = installedUpgrades[i].m_Upgrade;
			if (!(upgrade == ignoreUpgradeOrArea) && subAreaBuffers.TryGetBuffer(upgrade, ref subAreas))
			{
				Transform transform = transformData[upgrade];
				ObjectGeometryData objectGeometryData = prefabObjectGeometryData[prefabRefData[upgrade].m_Prefab];
				FillClearAreas(subAreas, transform, objectGeometryData, ignoreUpgradeOrArea, ref clearAreaData, ref nodeBuffers, ref triangleBuffers, ref clearAreas);
			}
		}
	}

	public static void FillClearAreas(DynamicBuffer<Game.Areas.SubArea> subAreas, Transform transform, ObjectGeometryData objectGeometryData, Entity ignoreArea, ref ComponentLookup<Clear> clearAreaData, ref BufferLookup<Node> nodeBuffers, ref BufferLookup<Triangle> triangleBuffers, ref NativeList<ClearAreaData> clearAreas)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < subAreas.Length; i++)
		{
			Entity area = subAreas[i].m_Area;
			if (clearAreaData.HasComponent(area) && !(area == ignoreArea))
			{
				if (!clearAreas.IsCreated)
				{
					clearAreas = new NativeList<ClearAreaData>(16, AllocatorHandle.op_Implicit((Allocator)2));
				}
				DynamicBuffer<Node> nodes = nodeBuffers[area];
				DynamicBuffer<Triangle> val = triangleBuffers[area];
				float topY = transform.m_Position.y + objectGeometryData.m_Bounds.max.y + 1f;
				for (int j = 0; j < val.Length; j++)
				{
					ClearAreaData clearAreaData2 = new ClearAreaData
					{
						m_Triangle = AreaUtils.GetTriangle3(nodes, val[j]),
						m_TopY = topY
					};
					clearAreas.Add(ref clearAreaData2);
				}
			}
		}
	}

	public static void FillClearAreas(Entity ownerPrefab, Transform ownerTransform, ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, ComponentLookup<AreaGeometryData> prefabAreaGeometryData, BufferLookup<Game.Prefabs.SubArea> prefabSubAreas, BufferLookup<SubAreaNode> prefabSubAreaNodes, ref NativeList<ClearAreaData> clearAreas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Prefabs.SubArea> val = default(DynamicBuffer<Game.Prefabs.SubArea>);
		if (!prefabSubAreas.TryGetBuffer(ownerPrefab, ref val))
		{
			return;
		}
		NativeArray<float3> nodes = default(NativeArray<float3>);
		NativeList<Triangle> triangles = default(NativeList<Triangle>);
		for (int i = 0; i < val.Length; i++)
		{
			Game.Prefabs.SubArea subArea = val[i];
			if ((prefabAreaGeometryData[subArea.m_Prefab].m_Flags & Game.Areas.GeometryFlags.ClearArea) != 0)
			{
				if (!clearAreas.IsCreated)
				{
					clearAreas = new NativeList<ClearAreaData>(16, AllocatorHandle.op_Implicit((Allocator)2));
				}
				int num = subArea.m_NodeRange.y - subArea.m_NodeRange.x;
				DynamicBuffer<SubAreaNode> val2 = prefabSubAreaNodes[ownerPrefab];
				NativeArray<SubAreaNode> subArray = val2.AsNativeArray().GetSubArray(subArea.m_NodeRange.x, num);
				nodes._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
				triangles._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
				bool isCounterClockwise = GeometrySystem.Area(subArray) > 0f;
				for (int j = 0; j < num; j++)
				{
					nodes[j] = AreaUtils.GetExpandedNode(subArray, j, -0.1f, isComplete: true, isCounterClockwise);
				}
				GeometrySystem.Triangulate<NativeList<Triangle>>(nodes, triangles, default(NativeArray<Bounds2>), 0, isCounterClockwise);
				GeometrySystem.EqualizeTriangles<NativeList<Triangle>>(nodes, triangles);
				ObjectGeometryData objectGeometryData = prefabObjectGeometryData[ownerPrefab];
				float topY = ownerTransform.m_Position.y + objectGeometryData.m_Bounds.max.y + 1f;
				for (int k = 0; k < triangles.Length; k++)
				{
					int3 val3 = triangles[k].m_Indices + subArea.m_NodeRange.x;
					ClearAreaData clearAreaData = new ClearAreaData
					{
						m_Triangle = new Triangle3(ObjectUtils.LocalToWorld(ownerTransform, val2[val3.x].m_Position), ObjectUtils.LocalToWorld(ownerTransform, val2[val3.y].m_Position), ObjectUtils.LocalToWorld(ownerTransform, val2[val3.z].m_Position)),
						m_TopY = topY
					};
					clearAreas.Add(ref clearAreaData);
				}
				nodes.Dispose();
				triangles.Dispose();
			}
		}
	}

	public static void FillClearAreas(Entity ownerPrefab, Transform ownerTransform, DynamicBuffer<Node> nodes, bool isComplete, ComponentLookup<ObjectGeometryData> prefabObjectGeometryData, ref NativeList<ClearAreaData> clearAreas)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		int num = nodes.Length;
		if (num < 3)
		{
			return;
		}
		if (num >= 4)
		{
			Node node = nodes[0];
			if (((float3)(ref node.m_Position)).Equals(nodes[num - 1].m_Position))
			{
				isComplete = true;
				num--;
			}
		}
		if (!clearAreas.IsCreated)
		{
			clearAreas = new NativeList<ClearAreaData>(16, AllocatorHandle.op_Implicit((Allocator)2));
		}
		NativeArray<float3> nodes2 = default(NativeArray<float3>);
		nodes2._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
		NativeList<Triangle> triangles = default(NativeList<Triangle>);
		triangles._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		bool isCounterClockwise = GeometrySystem.Area(nodes) > 0f;
		for (int i = 0; i < num; i++)
		{
			nodes2[i] = AreaUtils.GetExpandedNode(nodes, i, -0.1f, isComplete, isCounterClockwise);
		}
		GeometrySystem.Triangulate<NativeList<Triangle>>(nodes2, triangles, default(NativeArray<Bounds2>), 0, isCounterClockwise);
		GeometrySystem.EqualizeTriangles<NativeList<Triangle>>(nodes2, triangles);
		ObjectGeometryData objectGeometryData = prefabObjectGeometryData[ownerPrefab];
		float topY = ownerTransform.m_Position.y + objectGeometryData.m_Bounds.max.y + 1f;
		for (int j = 0; j < triangles.Length; j++)
		{
			ClearAreaData clearAreaData = new ClearAreaData
			{
				m_Triangle = AreaUtils.GetTriangle3(nodes, triangles[j]),
				m_TopY = topY
			};
			clearAreas.Add(ref clearAreaData);
		}
		nodes2.Dispose();
		triangles.Dispose();
	}

	public static void TransformClearAreas(NativeList<ClearAreaData> clearAreas, Transform oldTransform, Transform newTransform)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			Transform inverseParentTransform = ObjectUtils.InverseTransform(oldTransform);
			for (int i = 0; i < clearAreas.Length; i++)
			{
				ClearAreaData clearAreaData = clearAreas[i];
				clearAreaData.m_Triangle.a = ObjectUtils.LocalToWorld(newTransform, ObjectUtils.WorldToLocal(inverseParentTransform, clearAreaData.m_Triangle.a));
				clearAreaData.m_Triangle.b = ObjectUtils.LocalToWorld(newTransform, ObjectUtils.WorldToLocal(inverseParentTransform, clearAreaData.m_Triangle.b));
				clearAreaData.m_Triangle.c = ObjectUtils.LocalToWorld(newTransform, ObjectUtils.WorldToLocal(inverseParentTransform, clearAreaData.m_Triangle.c));
				clearAreaData.m_TopY += newTransform.m_Position.y - oldTransform.m_Position.y;
				clearAreas[i] = clearAreaData;
			}
		}
	}

	public static void InitClearAreas(NativeList<ClearAreaData> clearAreas, Transform topLevelTransform)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			for (int i = 0; i < clearAreas.Length; i++)
			{
				ClearAreaData clearAreaData = clearAreas[i];
				Triangle1 y = ((Triangle3)(ref clearAreaData.m_Triangle)).y;
				clearAreaData.m_OnGround = math.any(math.abs(((Triangle1)(ref y)).abc - topLevelTransform.m_Position.y) <= 1f);
				ref Triangle3 triangle = ref clearAreaData.m_Triangle;
				((Triangle3)(ref triangle)).y = ((Triangle3)(ref triangle)).y - 1f;
				clearAreas[i] = clearAreaData;
			}
		}
	}

	public static bool ShouldClear(NativeList<ClearAreaData> clearAreas, float3 position, bool onGround)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			float2 val = default(float2);
			for (int i = 0; i < clearAreas.Length; i++)
			{
				ClearAreaData clearAreaData = clearAreas[i];
				if (MathUtils.Intersect(((Triangle3)(ref clearAreaData.m_Triangle)).xz, ((float3)(ref position)).xz, ref val))
				{
					if (clearAreaData.m_OnGround && onGround)
					{
						return true;
					}
					float num = MathUtils.Position(((Triangle3)(ref clearAreaData.m_Triangle)).y, val);
					float num2 = math.max(clearAreaData.m_TopY, num + 2f);
					if (position.y >= num && position.y <= num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool ShouldClear(NativeList<ClearAreaData> clearAreas, Bezier4x3 curve, bool onGround)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			Bounds3 val = MathUtils.Bounds(curve);
			Segment val2 = default(Segment);
			float2 val3 = default(float2);
			for (int i = 0; i < clearAreas.Length; i++)
			{
				ClearAreaData clearAreaData = clearAreas[i];
				if (!MathUtils.Intersect(MathUtils.Bounds(((Triangle3)(ref clearAreaData.m_Triangle)).xz), ((Bounds3)(ref val)).xz))
				{
					continue;
				}
				val2.a = curve.a;
				for (int j = 1; j <= 16; j++)
				{
					val2.b = MathUtils.Position(curve, (float)j * 0.0625f);
					if (MathUtils.Intersect(((Triangle3)(ref clearAreaData.m_Triangle)).xz, ((Segment)(ref val2)).xz, ref val3))
					{
						if (clearAreaData.m_OnGround && onGround)
						{
							return true;
						}
						float3 val4 = MathUtils.Position(val2, math.csum(val3) * 0.5f);
						if (MathUtils.Intersect(((Triangle3)(ref clearAreaData.m_Triangle)).xz, ((float3)(ref val4)).xz, ref val3))
						{
							float num = MathUtils.Position(((Triangle3)(ref clearAreaData.m_Triangle)).y, val3);
							float num2 = math.max(clearAreaData.m_TopY, num + 2f);
							if (val4.y >= num && val4.y <= num2)
							{
								return true;
							}
						}
					}
					val2.a = val2.b;
				}
			}
		}
		return false;
	}

	public static bool ShouldClear(NativeList<ClearAreaData> clearAreas, DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, Transform ownerTransform)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle3 triangle = AreaUtils.GetTriangle3(nodes, triangles[i]);
				if (ShouldClear(clearAreas, triangle, ownerTransform))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool ShouldClear(NativeList<ClearAreaData> clearAreas, DynamicBuffer<SubAreaNode> subAreaNodes, int2 nodeRange, Transform ownerTransform)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (clearAreas.IsCreated)
		{
			int num = nodeRange.y - nodeRange.x;
			NativeArray<SubAreaNode> subArray = subAreaNodes.AsNativeArray().GetSubArray(nodeRange.x, num);
			NativeArray<float3> nodes = default(NativeArray<float3>);
			nodes._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
			NativeList<Triangle> triangles = default(NativeList<Triangle>);
			triangles._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			bool isCounterClockwise = GeometrySystem.Area(subArray) > 0f;
			for (int i = 0; i < num; i++)
			{
				nodes[i] = AreaUtils.GetExpandedNode(subArray, i, -0.1f, isComplete: true, isCounterClockwise);
			}
			GeometrySystem.Triangulate<NativeList<Triangle>>(nodes, triangles, default(NativeArray<Bounds2>), 0, isCounterClockwise);
			GeometrySystem.EqualizeTriangles<NativeList<Triangle>>(nodes, triangles);
			Triangle3 triangle = default(Triangle3);
			for (int j = 0; j < triangles.Length; j++)
			{
				int3 val = triangles[j].m_Indices + nodeRange.x;
				((Triangle3)(ref triangle))._002Ector(ObjectUtils.LocalToWorld(ownerTransform, subAreaNodes[val.x].m_Position), ObjectUtils.LocalToWorld(ownerTransform, subAreaNodes[val.y].m_Position), ObjectUtils.LocalToWorld(ownerTransform, subAreaNodes[val.z].m_Position));
				if (ShouldClear(clearAreas, triangle, ownerTransform))
				{
					nodes.Dispose();
					triangles.Dispose();
					return true;
				}
			}
			nodes.Dispose();
			triangles.Dispose();
		}
		return false;
	}

	private static bool ShouldClear(NativeList<ClearAreaData> clearAreas, Triangle3 triangle, Transform ownerTransform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 val = MathUtils.Bounds(triangle);
		Triangle1 y = ((Triangle3)(ref triangle)).y;
		bool flag = math.any(math.abs(((Triangle1)(ref y)).abc - ownerTransform.m_Position.y) <= 1f);
		for (int i = 0; i < clearAreas.Length; i++)
		{
			ClearAreaData clearAreaData = clearAreas[i];
			Bounds3 val2 = MathUtils.Bounds(clearAreaData.m_Triangle);
			if (MathUtils.Intersect(((Bounds3)(ref val)).xz, ((Bounds3)(ref val2)).xz) && MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((Triangle3)(ref clearAreaData.m_Triangle)).xz))
			{
				if (clearAreaData.m_OnGround && flag)
				{
					return true;
				}
				float y2 = val2.min.y;
				float num = math.max(clearAreaData.m_TopY, y2 + 2f);
				if (val.max.y >= y2 && val.min.y <= num)
				{
					return true;
				}
			}
		}
		return false;
	}
}
