using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Areas;

public static class AreaUtils
{
	public struct ObjectItem
	{
		public Circle2 m_Circle;

		public Entity m_Entity;

		public ObjectItem(float radius, float2 position, Entity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			m_Circle = new Circle2(radius, position);
			m_Entity = entity;
		}
	}

	private struct FixPathItem : ILessThan<FixPathItem>
	{
		public PathNode m_Node;

		public PathElement m_PathElement;

		public float m_Cost;

		public FixPathItem(PathNode node, PathElement pathElement, float cost)
		{
			m_Node = node;
			m_PathElement = pathElement;
			m_Cost = cost;
		}

		public bool LessThan(FixPathItem other)
		{
			return m_Cost < other.m_Cost;
		}
	}

	public const float NODE_DISTANCE_TOLERANCE = 0.1f;

	public static float GetMinNodeDistance(AreaGeometryData areaData)
	{
		return areaData.m_SnapDistance * 0.5f;
	}

	public static Triangle3 GetTriangle3(DynamicBuffer<Node> nodes, Triangle triangle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return new Triangle3(nodes[triangle.m_Indices.x].m_Position, nodes[triangle.m_Indices.y].m_Position, nodes[triangle.m_Indices.z].m_Position);
	}

	public static float3 GetElevations(DynamicBuffer<Node> nodes, Triangle triangle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return new float3(nodes[triangle.m_Indices.x].m_Elevation, nodes[triangle.m_Indices.y].m_Elevation, nodes[triangle.m_Indices.z].m_Elevation);
	}

	public static Bounds3 GetBounds(Triangle triangle, Triangle3 triangle3, AreaGeometryData areaData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 result = MathUtils.Bounds(triangle3);
		result.min.y += triangle.m_HeightRange.min;
		result.max.y += triangle.m_HeightRange.max + areaData.m_MaxHeight;
		return result;
	}

	public static int CalculateStorageCapacity(Geometry geometry, StorageAreaData prefabStorageData)
	{
		return Mathf.RoundToInt(geometry.m_SurfaceArea * (1f / 64f) * (float)prefabStorageData.m_Capacity);
	}

	public static float CalculateStorageObjectArea(Geometry geometry, Storage storage, StorageAreaData prefabStorageData)
	{
		float num = geometry.m_SurfaceArea * (1f / 64f) * (float)prefabStorageData.m_Capacity;
		float num2 = (float)storage.m_Amount / math.max(1f, num);
		return math.min(0.25f, math.sqrt(num2)) * geometry.m_SurfaceArea;
	}

	public static float CalculateExtractorObjectArea(Geometry geometry, Extractor extractor, ExtractorAreaData extractorAreaData)
	{
		return math.min(extractor.m_TotalExtracted * extractorAreaData.m_ObjectSpawnFactor, geometry.m_SurfaceArea * extractorAreaData.m_MaxObjectArea);
	}

	public static Triangle2 GetTriangle2(DynamicBuffer<Node> nodes, Triangle triangle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Node node = nodes[triangle.m_Indices.x];
		float2 xz = ((float3)(ref node.m_Position)).xz;
		node = nodes[triangle.m_Indices.y];
		float2 xz2 = ((float3)(ref node.m_Position)).xz;
		node = nodes[triangle.m_Indices.z];
		return new Triangle2(xz, xz2, ((float3)(ref node.m_Position)).xz);
	}

	public static Triangle2 GetTriangle2(DynamicBuffer<Node> nodes, Triangle triangle, float expandAmount, bool isCounterClockwise)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		float3 expandedNode = GetExpandedNode(nodes, triangle.m_Indices.x, expandAmount, isComplete: true, isCounterClockwise);
		Triangle2 result = default(Triangle2);
		result.a = ((float3)(ref expandedNode)).xz;
		expandedNode = GetExpandedNode(nodes, triangle.m_Indices.y, expandAmount, isComplete: true, isCounterClockwise);
		result.b = ((float3)(ref expandedNode)).xz;
		expandedNode = GetExpandedNode(nodes, triangle.m_Indices.z, expandAmount, isComplete: true, isCounterClockwise);
		result.c = ((float3)(ref expandedNode)).xz;
		return result;
	}

	public static bool3 IsEdge(DynamicBuffer<Node> nodes, Triangle triangle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		int3 val = math.abs(triangle.m_Indices - ((int3)(ref triangle.m_Indices)).yzx);
		return (val == 1) | (val == nodes.Length - 1);
	}

	public static quaternion CalculateLabelRotation(float3 cameraRight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.cross(cameraRight, math.up());
		return quaternion.LookRotation(new float3(0f, -1f, 0f), val);
	}

	public static float3 CalculateLabelPosition(Geometry geometry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return geometry.m_CenterPosition;
	}

	public static float CalculateLabelScale(float3 cameraPosition, float3 labelPosition)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return math.max(0.01f, math.sqrt(math.distance(cameraPosition, labelPosition) * 0.001f));
	}

	public static float4x4 CalculateLabelMatrix(float3 cameraPosition, float3 labelPosition, quaternion labelRotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float num = CalculateLabelScale(cameraPosition, labelPosition);
		return float4x4.TRS(labelPosition, labelRotation, float3.op_Implicit(num));
	}

	public static bool CheckOption(District district, DistrictOption option)
	{
		return (district.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static void ApplyModifier(ref float value, DynamicBuffer<DistrictModifier> modifiers, DistrictModifierType type)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (modifiers.Length > (int)type)
		{
			float2 delta = modifiers[(int)type].m_Delta;
			value += delta.x;
			value += value * delta.y;
		}
	}

	public static bool HasOption(DistrictOptionData optionData, DistrictOption option)
	{
		return (optionData.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static bool CheckServiceDistrict(Entity district, Entity service, BufferLookup<ServiceDistrict> serviceDistricts)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ServiceDistrict> val = default(DynamicBuffer<ServiceDistrict>);
		if (!serviceDistricts.TryGetBuffer(service, ref val))
		{
			return true;
		}
		if (val.Length == 0)
		{
			return true;
		}
		if (district == Entity.Null)
		{
			return false;
		}
		return CollectionUtils.ContainsValue<ServiceDistrict>(val, new ServiceDistrict(district));
	}

	public static bool CheckServiceDistrict(Entity district1, Entity district2, Entity service, BufferLookup<ServiceDistrict> serviceDistricts)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!serviceDistricts.HasBuffer(service))
		{
			return true;
		}
		DynamicBuffer<ServiceDistrict> val = serviceDistricts[service];
		if (val.Length == 0)
		{
			return true;
		}
		if (district1 == Entity.Null && district2 == Entity.Null)
		{
			return false;
		}
		if (!CollectionUtils.ContainsValue<ServiceDistrict>(val, new ServiceDistrict(district1)))
		{
			return CollectionUtils.ContainsValue<ServiceDistrict>(val, new ServiceDistrict(district2));
		}
		return true;
	}

	public static bool CheckServiceDistrict(Entity building, DynamicBuffer<ServiceDistrict> serviceDistricts, ref ComponentLookup<CurrentDistrict> currentDistricts)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		CurrentDistrict currentDistrict = default(CurrentDistrict);
		if (serviceDistricts.IsCreated && serviceDistricts.Length != 0 && currentDistricts.TryGetComponent(building, ref currentDistrict))
		{
			return CollectionUtils.ContainsValue<ServiceDistrict>(serviceDistricts, new ServiceDistrict(currentDistrict.m_District));
		}
		return true;
	}

	public static CollisionMask GetCollisionMask(AreaGeometryData areaGeometryData)
	{
		CollisionMask collisionMask = CollisionMask.OnGround | CollisionMask.Overground | CollisionMask.ExclusiveGround;
		if (areaGeometryData.m_Type != AreaType.Lot)
		{
			collisionMask |= CollisionMask.Underground;
		}
		return collisionMask;
	}

	public static bool TryGetRandomObjectLocation(ref Random random, ObjectGeometryData objectGeometryData, Area area, Geometry geometry, float extraRadius, DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, NativeList<ObjectItem> objects, out Transform transform)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		transform.m_Position = GetRandomPosition(ref random, geometry, nodes, triangles);
		float num = (((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) == 0) ? (math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz)) * 0.5f) : (objectGeometryData.m_Size.x * 0.5f));
		bool result = TryFitInside(ref transform.m_Position, num, extraRadius, area, nodes, objects, canOverride: true);
		if (objects.IsCreated)
		{
			float num2 = (num + extraRadius) * 0.5f;
			int num3 = 0;
			for (int i = 0; i < objects.Length; i++)
			{
				ObjectItem objectItem = objects[i];
				if (objectItem.m_Circle.radius < num2)
				{
					float num4 = num + objectItem.m_Circle.radius;
					if (math.distancesq(((float3)(ref transform.m_Position)).xz, objectItem.m_Circle.position) < num4 * num4)
					{
						objects[num3++] = objectItem;
					}
				}
			}
			if (num3 < objects.Length)
			{
				objects.RemoveRange(num3, objects.Length - num3);
			}
		}
		transform.m_Rotation = GetRandomRotation(ref random, transform.m_Position, nodes);
		if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) == 0)
		{
			ref float3 position = ref transform.m_Position;
			float2 xz = ((float3)(ref position)).xz;
			float3 val = math.rotate(transform.m_Rotation, MathUtils.Center(objectGeometryData.m_Bounds));
			((float3)(ref position)).xz = xz - ((float3)(ref val)).xz;
		}
		return result;
	}

	public static float3 GetRandomPosition(ref Random random, Geometry geometry, DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Random)(ref random)).NextFloat(geometry.m_SurfaceArea);
		for (int i = 0; i < triangles.Length; i++)
		{
			Triangle3 triangle = GetTriangle3(nodes, triangles[i]);
			num -= MathUtils.Area(((Triangle3)(ref triangle)).xz);
			if (num <= 0f)
			{
				float2 val = ((Random)(ref random)).NextFloat2(float2.op_Implicit(1f));
				val = math.select(val, 1f - val, math.csum(val) > 1f);
				return MathUtils.Position(triangle, val);
			}
		}
		if (nodes.Length >= 2)
		{
			return math.lerp(nodes[0].m_Position, nodes[1].m_Position, ((Random)(ref random)).NextFloat(1f));
		}
		if (nodes.Length == 1)
		{
			return nodes[0].m_Position;
		}
		return default(float3);
	}

	public static quaternion GetRandomRotation(ref Random random, float3 position, DynamicBuffer<Node> nodes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		float num = float.MaxValue;
		Node node = nodes[nodes.Length - 1];
		Segment val2 = default(Segment);
		val2.a = ((float3)(ref node.m_Position)).xz;
		float num3 = default(float);
		for (int i = 0; i < nodes.Length; i++)
		{
			node = nodes[i];
			val2.b = ((float3)(ref node.m_Position)).xz;
			float num2 = MathUtils.DistanceSquared(val2, ((float3)(ref position)).xz, ref num3);
			if (num2 < num)
			{
				val = val2.b - val2.a;
				num = num2;
			}
			val2.a = val2.b;
		}
		float num4;
		if (MathUtils.TryNormalize(ref val))
		{
			num4 = math.atan2(val.x, val.y);
			num4 += (float)((Random)(ref random)).NextInt(4) * ((float)Math.PI / 2f);
		}
		else
		{
			num4 = ((Random)(ref random)).NextFloat((float)Math.PI * 2f);
		}
		return quaternion.RotateY(num4);
	}

	public static bool TryFitInside(ref float3 position, float radius, float extraRadius, Area area, DynamicBuffer<Node> nodes, NativeList<ObjectItem> objects, bool canOverride = false)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		float num = radius + extraRadius;
		num *= num;
		bool flag = false;
		Node node = nodes[nodes.Length - 1];
		Segment val = default(Segment);
		val.a = ((float3)(ref node.m_Position)).xz;
		float num2 = default(float);
		for (int i = 0; i < nodes.Length; i++)
		{
			node = nodes[i];
			val.b = ((float3)(ref node.m_Position)).xz;
			if (MathUtils.DistanceSquared(val, ((float3)(ref position)).xz, ref num2) < num)
			{
				float2 val2 = math.normalizesafe(val.b - val.a, default(float2));
				float2 val3 = ((float3)(ref position)).xz - MathUtils.Position(val, num2);
				float num3 = math.dot(val2, val3);
				val2 = (((area.m_Flags & AreaFlags.CounterClockwise) == 0) ? MathUtils.Right(val2) : MathUtils.Left(val2));
				val2 *= math.sqrt(num - num3 * num3) - math.dot(val2, val3) + 0.01f;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + val2;
				flag = true;
			}
			val.a = val.b;
		}
		if (objects.IsCreated)
		{
			float num4 = (radius + extraRadius) * 0.5f;
			for (int j = 0; j < objects.Length; j++)
			{
				ObjectItem objectItem = objects[j];
				if (!canOverride || !(objectItem.m_Circle.radius < num4))
				{
					float num5 = radius + objectItem.m_Circle.radius;
					float num6 = math.distancesq(((float3)(ref position)).xz, objectItem.m_Circle.position);
					if (num6 < num5 * num5)
					{
						float num7 = math.sqrt(num6);
						float2 val4 = (objectItem.m_Circle.position - ((float3)(ref position)).xz) * (num5 / num7 - 1f);
						((float3)(ref position)).xz = ((float3)(ref position)).xz + val4;
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			if (!IntersectEdges(position, radius, extraRadius, nodes))
			{
				return !IntersectObjects(position, radius, extraRadius, objects, canOverride);
			}
			return false;
		}
		return true;
	}

	public static bool IntersectArea(float3 position, float radius, DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Circle2 val = default(Circle2);
		((Circle2)(ref val))._002Ector(radius, ((float3)(ref position)).xz);
		for (int i = 0; i < triangles.Length; i++)
		{
			if (MathUtils.Intersect(GetTriangle2(nodes, triangles[i]), val))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IntersectEdges(float3 position, float radius, float extraRadius, DynamicBuffer<Node> nodes)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		float num = radius + extraRadius;
		num *= num;
		Node node = nodes[nodes.Length - 1];
		Segment val = default(Segment);
		val.a = ((float3)(ref node.m_Position)).xz;
		float num2 = default(float);
		for (int i = 0; i < nodes.Length; i++)
		{
			node = nodes[i];
			val.b = ((float3)(ref node.m_Position)).xz;
			if (MathUtils.DistanceSquared(val, ((float3)(ref position)).xz, ref num2) < num)
			{
				return true;
			}
			val.a = val.b;
		}
		return false;
	}

	public static bool IntersectObjects(float3 position, float radius, float extraRadius, NativeList<ObjectItem> objects, bool canOverride = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (objects.IsCreated)
		{
			float num = (radius + extraRadius) * 0.5f;
			for (int i = 0; i < objects.Length; i++)
			{
				ObjectItem objectItem = objects[i];
				if (!canOverride || !(objectItem.m_Circle.radius < num))
				{
					float num2 = radius + objectItem.m_Circle.radius;
					if (math.distancesq(((float3)(ref position)).xz, objectItem.m_Circle.position) < num2 * num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static float GetMinNodeDistance(AreaType areaType)
	{
		return areaType switch
		{
			AreaType.Lot => 8f, 
			AreaType.District => 32f, 
			AreaType.MapTile => 64f, 
			AreaType.Space => 1f, 
			AreaType.Surface => 0.75f, 
			_ => 1f, 
		};
	}

	public static AreaTypeMask GetTypeMask(AreaType type)
	{
		if (type != AreaType.None)
		{
			return (AreaTypeMask)(1 << (int)type);
		}
		return AreaTypeMask.None;
	}

	public static float3 GetExpandedNode(DynamicBuffer<Node> nodes, int index, float expandAmount, bool isComplete, bool isCounterClockwise)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		Node node;
		if (!isComplete)
		{
			if (nodes.Length == 1)
			{
				return nodes[index].m_Position;
			}
			if (index == 0)
			{
				node = nodes[math.select(index + 1, 0, index == nodes.Length - 1)];
				float2 xz = ((float3)(ref node.m_Position)).xz;
				float3 position = nodes[index].m_Position;
				float2 val = math.normalizesafe(xz - ((float3)(ref position)).xz, default(float2));
				float2 val2 = math.select(MathUtils.Left(val), MathUtils.Right(val), isCounterClockwise) - val;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + val2 * expandAmount;
				return position;
			}
			if (index == nodes.Length - 1)
			{
				node = nodes[math.select(index - 1, nodes.Length - 1, index == 0)];
				float2 xz2 = ((float3)(ref node.m_Position)).xz;
				float3 position2 = nodes[index].m_Position;
				float2 val3 = math.normalizesafe(xz2 - ((float3)(ref position2)).xz, default(float2));
				float2 val4 = math.select(MathUtils.Right(val3), MathUtils.Left(val3), isCounterClockwise) - val3;
				((float3)(ref position2)).xz = ((float3)(ref position2)).xz + val4 * expandAmount;
				return position2;
			}
		}
		node = nodes[math.select(index - 1, nodes.Length - 1, index == 0)];
		float2 xz3 = ((float3)(ref node.m_Position)).xz;
		node = nodes[math.select(index + 1, 0, index == nodes.Length - 1)];
		float2 xz4 = ((float3)(ref node.m_Position)).xz;
		float3 position3 = nodes[index].m_Position;
		float2 val5 = math.normalizesafe(xz3 - ((float3)(ref position3)).xz, default(float2));
		float2 val6 = math.normalizesafe(xz4 - ((float3)(ref position3)).xz, default(float2));
		float2 val7 = math.select(MathUtils.Right(val5), MathUtils.Left(val5), isCounterClockwise);
		float num = math.acos(math.clamp(math.dot(val5, val6), -1f, 1f));
		float num2 = math.sign(math.dot(val7, val6));
		float num3 = math.tan(num * 0.5f);
		val7 += val5 * math.select(num2 / num3, 0f, num3 < 0.001f);
		((float3)(ref position3)).xz = ((float3)(ref position3)).xz + val7 * expandAmount;
		return position3;
	}

	public static float3 GetExpandedNode(NativeArray<SubAreaNode> nodes, int index, float expandAmount, bool isComplete, bool isCounterClockwise)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		SubAreaNode subAreaNode;
		if (!isComplete)
		{
			if (nodes.Length == 1)
			{
				return nodes[index].m_Position;
			}
			if (index == 0)
			{
				subAreaNode = nodes[math.select(index + 1, 0, index == nodes.Length - 1)];
				float2 xz = ((float3)(ref subAreaNode.m_Position)).xz;
				float3 position = nodes[index].m_Position;
				float2 val = math.normalizesafe(xz - ((float3)(ref position)).xz, default(float2));
				float2 val2 = math.select(MathUtils.Left(val), MathUtils.Right(val), isCounterClockwise) - val;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + val2 * expandAmount;
				return position;
			}
			if (index == nodes.Length - 1)
			{
				subAreaNode = nodes[math.select(index - 1, nodes.Length - 1, index == 0)];
				float2 xz2 = ((float3)(ref subAreaNode.m_Position)).xz;
				float3 position2 = nodes[index].m_Position;
				float2 val3 = math.normalizesafe(xz2 - ((float3)(ref position2)).xz, default(float2));
				float2 val4 = math.select(MathUtils.Right(val3), MathUtils.Left(val3), isCounterClockwise) - val3;
				((float3)(ref position2)).xz = ((float3)(ref position2)).xz + val4 * expandAmount;
				return position2;
			}
		}
		subAreaNode = nodes[math.select(index - 1, nodes.Length - 1, index == 0)];
		float2 xz3 = ((float3)(ref subAreaNode.m_Position)).xz;
		subAreaNode = nodes[math.select(index + 1, 0, index == nodes.Length - 1)];
		float2 xz4 = ((float3)(ref subAreaNode.m_Position)).xz;
		float3 position3 = nodes[index].m_Position;
		float2 val5 = math.normalizesafe(xz3 - ((float3)(ref position3)).xz, default(float2));
		float2 val6 = math.normalizesafe(xz4 - ((float3)(ref position3)).xz, default(float2));
		float2 val7 = math.select(MathUtils.Right(val5), MathUtils.Left(val5), isCounterClockwise);
		float num = math.acos(math.clamp(math.dot(val5, val6), -1f, 1f));
		float num2 = math.sign(math.dot(val7, val6));
		float num3 = math.tan(num * 0.5f);
		val7 += val5 * math.select(num2 / num3, 0f, num3 < 0.001f);
		((float3)(ref position3)).xz = ((float3)(ref position3)).xz + val7 * expandAmount;
		return position3;
	}

	public static float3 GetExpandedNode<TNodeList>(TNodeList nodes, int index, int prevIndex, int nextIndex, float expandAmount, bool isCounterClockwise) where TNodeList : INativeList<Node>
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		Node node = ((INativeList<Node>)nodes)[prevIndex];
		float2 xz = ((float3)(ref node.m_Position)).xz;
		node = ((INativeList<Node>)nodes)[nextIndex];
		float2 xz2 = ((float3)(ref node.m_Position)).xz;
		float3 position = ((INativeList<Node>)nodes)[index].m_Position;
		float2 val = math.normalizesafe(xz - ((float3)(ref position)).xz, default(float2));
		float2 val2 = math.normalizesafe(xz2 - ((float3)(ref position)).xz, default(float2));
		float2 val3 = math.select(MathUtils.Right(val), MathUtils.Left(val), isCounterClockwise);
		float num = math.acos(math.clamp(math.dot(val, val2), -1f, 1f));
		float num2 = math.sign(math.dot(val3, val2));
		float num3 = math.tan(num * 0.5f);
		val3 += val * math.select(num2 / num3, 0f, num3 < 0.001f);
		((float3)(ref position)).xz = ((float3)(ref position)).xz + val3 * expandAmount;
		return position;
	}

	public static bool SelectAreaPrefab(DynamicBuffer<PlaceholderObjectElement> placeholderElements, ComponentLookup<SpawnableObjectData> spawnableDatas, NativeParallelHashMap<Entity, int> selectedSpawnables, ref Random random, out Entity result, out int seed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		bool flag = false;
		result = Entity.Null;
		seed = 0;
		for (int i = 0; i < placeholderElements.Length; i++)
		{
			PlaceholderObjectElement placeholderObjectElement = placeholderElements[i];
			SpawnableObjectData spawnableObjectData = spawnableDatas[placeholderObjectElement.m_Object];
			int num2 = 0;
			if (selectedSpawnables.IsCreated && selectedSpawnables.TryGetValue(placeholderObjectElement.m_Object, ref num2))
			{
				if (!flag)
				{
					num = 0;
					flag = true;
				}
			}
			else if (flag)
			{
				continue;
			}
			num += spawnableObjectData.m_Probability;
			if (((Random)(ref random)).NextInt(num) < spawnableObjectData.m_Probability)
			{
				result = placeholderObjectElement.m_Object;
				seed = (flag ? num2 : ((Random)(ref random)).NextInt());
			}
		}
		if (result != Entity.Null)
		{
			if (!flag && selectedSpawnables.IsCreated)
			{
				selectedSpawnables.Add(result, seed);
			}
			return true;
		}
		return false;
	}

	public static void FindAreaPath(ref Random random, NativeList<PathElement> path, DynamicBuffer<Game.Net.SubLane> lanes, Entity startEntity, float startCurvePos, Entity endEntity, float endCurvePos, ComponentLookup<Lane> laneData, ComponentLookup<Curve> curveData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		if (startEntity == endEntity)
		{
			PathElement pathElement = new PathElement(startEntity, new float2(startCurvePos, endCurvePos));
			path.Add(ref pathElement);
			return;
		}
		NativeParallelMultiHashMap<PathNode, Entity> val = default(NativeParallelMultiHashMap<PathNode, Entity>);
		val._002Ector(lanes.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
		NativeParallelHashMap<PathNode, PathElement> val2 = default(NativeParallelHashMap<PathNode, PathElement>);
		val2._002Ector(lanes.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
		NativeMinHeap<FixPathItem> val3 = default(NativeMinHeap<FixPathItem>);
		val3._002Ector(lanes.Length, (Allocator)2);
		for (int i = 0; i < lanes.Length; i++)
		{
			Entity subLane = lanes[i].m_SubLane;
			Lane lane = laneData[subLane];
			val.Add(lane.m_StartNode, subLane);
			val.Add(lane.m_EndNode, subLane);
		}
		Lane lane2 = laneData[endEntity];
		Curve curve = curveData[endEntity];
		float cost = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve.m_Length * endCurvePos;
		float cost2 = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve.m_Length * (1f - endCurvePos);
		val3.Insert(new FixPathItem(lane2.m_StartNode, new PathElement(endEntity, new float2(0f, endCurvePos)), cost));
		val3.Insert(new FixPathItem(lane2.m_EndNode, new PathElement(endEntity, new float2(1f, endCurvePos)), cost2));
		PathElement pathElement2 = default(PathElement);
		Entity val4 = default(Entity);
		NativeParallelMultiHashMapIterator<PathNode> val5 = default(NativeParallelMultiHashMapIterator<PathNode>);
		while (val3.Length != 0)
		{
			FixPathItem fixPathItem = val3.Extract();
			if (!val2.TryAdd(fixPathItem.m_Node, fixPathItem.m_PathElement))
			{
				continue;
			}
			if (fixPathItem.m_PathElement.m_Target == startEntity)
			{
				path.Add(ref fixPathItem.m_PathElement);
				Lane lane3 = laneData[startEntity];
				PathNode pathNode = ((fixPathItem.m_PathElement.m_TargetDelta.y == 0f) ? lane3.m_StartNode : lane3.m_EndNode);
				while (val2.TryGetValue(pathNode, ref pathElement2))
				{
					path.Add(ref pathElement2);
					if (pathElement2.m_Target == endEntity)
					{
						break;
					}
					lane3 = laneData[pathElement2.m_Target];
					pathNode = ((pathElement2.m_TargetDelta.y == 0f) ? lane3.m_StartNode : lane3.m_EndNode);
				}
				break;
			}
			if (!val.TryGetFirstValue(fixPathItem.m_Node, ref val4, ref val5))
			{
				continue;
			}
			do
			{
				if (val4 == fixPathItem.m_PathElement.m_Target)
				{
					continue;
				}
				Lane lane4 = laneData[val4];
				Curve curve2 = curveData[val4];
				if (lane4.m_EndNode.Equals(fixPathItem.m_Node))
				{
					if (val4 == startEntity)
					{
						float num = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve2.m_Length * (1f - startCurvePos);
						val3.Insert(new FixPathItem(lane4.m_MiddleNode, new PathElement(startEntity, new float2(startCurvePos, 1f)), fixPathItem.m_Cost + num));
					}
					else if (!val2.ContainsKey(lane4.m_StartNode))
					{
						float num2 = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve2.m_Length;
						val3.Insert(new FixPathItem(lane4.m_StartNode, new PathElement(val4, new float2(0f, 1f)), fixPathItem.m_Cost + num2));
					}
				}
				else if (lane4.m_StartNode.Equals(fixPathItem.m_Node))
				{
					if (val4 == startEntity)
					{
						float num3 = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve2.m_Length * startCurvePos;
						val3.Insert(new FixPathItem(lane4.m_MiddleNode, new PathElement(startEntity, new float2(startCurvePos, 0f)), fixPathItem.m_Cost + num3));
					}
					else if (!val2.ContainsKey(lane4.m_EndNode))
					{
						float num4 = ((Random)(ref random)).NextFloat(0.5f, 1f) * curve2.m_Length;
						val3.Insert(new FixPathItem(lane4.m_EndNode, new PathElement(val4, new float2(1f, 0f)), fixPathItem.m_Cost + num4));
					}
				}
			}
			while (val.TryGetNextValue(ref val4, ref val5));
		}
		val.Dispose();
		val2.Dispose();
		val3.Dispose();
	}

	public static Node AdjustPosition(Node node, ref TerrainHeightData terrainHeightData, ref WaterSurfaceData waterSurfaceData)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Node result = node;
		result.m_Position.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, node.m_Position);
		return result;
	}

	public static Node AdjustPosition(Node node, ref TerrainHeightData terrainHeightData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Node result = node;
		result.m_Position.y = TerrainUtils.SampleHeight(ref terrainHeightData, node.m_Position);
		return result;
	}

	public static void SetCollisionFlags(ref AreaGeometryData areaGeometryData, bool ignoreMarkers)
	{
		if (!ignoreMarkers)
		{
			AreaType type = areaGeometryData.m_Type;
			if ((uint)(type - 3) <= 1u)
			{
				areaGeometryData.m_Flags |= GeometryFlags.PhysicalGeometry;
			}
		}
	}
}
