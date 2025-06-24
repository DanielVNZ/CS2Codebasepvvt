using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Net;

public static class RaycastJobs
{
	[BurstCompile]
	public struct RaycastEdgesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public float m_FovTan;

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeArray<RaycastSystem.EntityResult> m_Edges;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			RaycastSystem.EntityResult entityResult = m_Edges[index];
			RaycastInput input = m_Input[entityResult.m_RaycastIndex];
			if ((input.m_TypeMask & TypeMask.Net) != TypeMask.None)
			{
				if (m_EdgeData.HasComponent(entityResult.m_Entity))
				{
					CheckEdge(entityResult.m_Entity, entityResult.m_RaycastIndex, input);
				}
				else if (m_NodeData.HasComponent(entityResult.m_Entity))
				{
					CheckNode(entityResult.m_Entity, entityResult.m_RaycastIndex, input);
				}
			}
		}

		private void CheckNode(Entity entity, int raycastIndex, RaycastInput input)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			if (m_NodeGeometryData.HasComponent(entity))
			{
				float2 val = default(float2);
				if (!MathUtils.Intersect(m_NodeGeometryData[entity].m_Bounds, input.m_Line, ref val) || !m_OrphanData.HasComponent(entity))
				{
					return;
				}
				Node node = m_NodeData[entity];
				Orphan orphan = m_OrphanData[entity];
				NetCompositionData compositionData = m_PrefabCompositionData[orphan.m_Composition];
				if ((compositionData.m_State & CompositionState.Marker) != 0)
				{
					if ((input.m_Flags & RaycastFlags.Markers) == 0)
					{
						return;
					}
				}
				else if ((NetUtils.GetCollisionMask(compositionData, ignoreMarkers: true) & input.m_CollisionMask) == 0)
				{
					return;
				}
				float3 position = node.m_Position;
				if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
				{
					float maxElevation = 0f - input.m_Offset.y - compositionData.m_SurfaceHeight.max;
					SetElevationOffset(ref position, entity, maxElevation);
				}
				Segment val2 = input.m_Line + input.m_Offset;
				if ((input.m_Flags & RaycastFlags.ElevateOffset) == 0)
				{
					position.y += compositionData.m_SurfaceHeight.max;
				}
				float num = default(float);
				if (!MathUtils.Intersect(((Segment)(ref val2)).y, position.y, ref num))
				{
					return;
				}
				float3 hitPosition = MathUtils.Position(val2, num);
				if (math.distance(((float3)(ref hitPosition)).xz, ((float3)(ref position)).xz) <= compositionData.m_Width * 0.5f)
				{
					RaycastResult result = new RaycastResult
					{
						m_Owner = entity,
						m_Hit = 
						{
							m_HitEntity = entity,
							m_Position = node.m_Position,
							m_HitPosition = hitPosition,
							m_NormalizedDistance = num + 0.5f / math.max(1f, MathUtils.Length(val2))
						}
					};
					if (ValidateResult(input, ref result))
					{
						m_Results.Accumulate(raycastIndex, result);
					}
				}
			}
			else
			{
				if ((input.m_Flags & RaycastFlags.Markers) == 0)
				{
					return;
				}
				Node node2 = m_NodeData[entity];
				float3 position2 = node2.m_Position;
				if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
				{
					float maxElevation2 = 0f - input.m_Offset.y;
					SetElevationOffset(ref position2, entity, maxElevation2);
				}
				Segment val3 = input.m_Line + input.m_Offset;
				float num3 = default(float);
				float num2 = MathUtils.Distance(val3, position2, ref num3);
				if (num2 < 1f)
				{
					RaycastResult result2 = new RaycastResult
					{
						m_Owner = entity,
						m_Hit = 
						{
							m_HitEntity = entity,
							m_Position = node2.m_Position,
							m_HitPosition = MathUtils.Position(val3, num3),
							m_NormalizedDistance = num3 - (1f - num2) / math.max(1f, MathUtils.Length(val3))
						}
					};
					if (ValidateResult(input, ref result2))
					{
						m_Results.Accumulate(raycastIndex, result2);
					}
				}
			}
		}

		private void CheckEdge(Entity entity, int raycastIndex, RaycastInput input)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			if (m_EdgeGeometryData.HasComponent(entity))
			{
				EdgeGeometry geometry = m_EdgeGeometryData[entity];
				EdgeNodeGeometry geometry2 = m_StartNodeGeometryData[entity].m_Geometry;
				EdgeNodeGeometry geometry3 = m_EndNodeGeometryData[entity].m_Geometry;
				ref Bounds3 bounds = ref geometry.m_Bounds;
				bounds |= geometry.m_Bounds - input.m_Offset;
				ref Bounds3 bounds2 = ref geometry2.m_Bounds;
				bounds2 |= geometry2.m_Bounds - input.m_Offset;
				ref Bounds3 bounds3 = ref geometry3.m_Bounds;
				bounds3 |= geometry3.m_Bounds - input.m_Offset;
				bool3 val = default(bool3);
				float2 val2 = default(float2);
				val.x = MathUtils.Intersect(geometry.m_Bounds, input.m_Line, ref val2);
				val.y = MathUtils.Intersect(geometry2.m_Bounds, input.m_Line, ref val2);
				val.z = MathUtils.Intersect(geometry3.m_Bounds, input.m_Line, ref val2);
				if (!math.any(val))
				{
					return;
				}
				Composition composition = m_CompositionData[entity];
				Edge edge = m_EdgeData[entity];
				Curve curve = m_CurveData[entity];
				if (val.x)
				{
					NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
					if (((netCompositionData.m_State & CompositionState.Marker) == 0) ? ((byte)(NetUtils.GetCollisionMask(netCompositionData, ignoreMarkers: true) & input.m_CollisionMask) != 0) : ((byte)(input.m_Flags & RaycastFlags.Markers) != 0))
					{
						if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
						{
							float maxElevation = 0f - input.m_Offset.y - netCompositionData.m_SurfaceHeight.max;
							SetElevationOffset(ref geometry, entity, edge.m_Start, edge.m_End, maxElevation);
						}
						CheckSegment(input, raycastIndex, entity, entity, geometry.m_Start, curve.m_Bezier, netCompositionData);
						CheckSegment(input, raycastIndex, entity, entity, geometry.m_End, curve.m_Bezier, netCompositionData);
					}
				}
				if (val.y)
				{
					NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
					if (((netCompositionData2.m_State & CompositionState.Marker) == 0) ? ((byte)(NetUtils.GetCollisionMask(netCompositionData2, ignoreMarkers: true) & input.m_CollisionMask) != 0) : ((byte)(input.m_Flags & RaycastFlags.Markers) != 0))
					{
						if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
						{
							float maxElevation2 = 0f - input.m_Offset.y - netCompositionData2.m_SurfaceHeight.max;
							SetElevationOffset(ref geometry2, edge.m_Start, maxElevation2);
						}
						if (geometry2.m_MiddleRadius > 0f)
						{
							CheckSegment(input, raycastIndex, edge.m_Start, entity, geometry2.m_Left, curve.m_Bezier, netCompositionData2);
							Segment right = geometry2.m_Right;
							Segment right2 = geometry2.m_Right;
							right.m_Right = MathUtils.Lerp(geometry2.m_Right.m_Left, geometry2.m_Right.m_Right, 0.5f);
							right2.m_Left = MathUtils.Lerp(geometry2.m_Right.m_Left, geometry2.m_Right.m_Right, 0.5f);
							right.m_Right.d = geometry2.m_Middle.d;
							right2.m_Left.d = geometry2.m_Middle.d;
							CheckSegment(input, raycastIndex, edge.m_Start, entity, right, curve.m_Bezier, netCompositionData2);
							CheckSegment(input, raycastIndex, edge.m_Start, entity, right2, curve.m_Bezier, netCompositionData2);
						}
						else
						{
							Segment left = geometry2.m_Left;
							Segment right3 = geometry2.m_Right;
							CheckSegment(input, raycastIndex, edge.m_Start, entity, left, curve.m_Bezier, netCompositionData2);
							CheckSegment(input, raycastIndex, edge.m_Start, entity, right3, curve.m_Bezier, netCompositionData2);
							left.m_Right = geometry2.m_Middle;
							right3.m_Left = geometry2.m_Middle;
							CheckSegment(input, raycastIndex, edge.m_Start, entity, left, curve.m_Bezier, netCompositionData2);
							CheckSegment(input, raycastIndex, edge.m_Start, entity, right3, curve.m_Bezier, netCompositionData2);
						}
					}
				}
				if (!val.z)
				{
					return;
				}
				NetCompositionData netCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
				if (((netCompositionData3.m_State & CompositionState.Marker) == 0) ? ((byte)(NetUtils.GetCollisionMask(netCompositionData3, ignoreMarkers: true) & input.m_CollisionMask) != 0) : ((byte)(input.m_Flags & RaycastFlags.Markers) != 0))
				{
					if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
					{
						float maxElevation3 = 0f - input.m_Offset.y - netCompositionData3.m_SurfaceHeight.max;
						SetElevationOffset(ref geometry3, edge.m_End, maxElevation3);
					}
					if (geometry3.m_MiddleRadius > 0f)
					{
						CheckSegment(input, raycastIndex, edge.m_End, entity, geometry3.m_Left, curve.m_Bezier, netCompositionData3);
						Segment right4 = geometry3.m_Right;
						Segment right5 = geometry3.m_Right;
						right4.m_Right = MathUtils.Lerp(geometry3.m_Right.m_Left, geometry3.m_Right.m_Right, 0.5f);
						right4.m_Right.d = geometry3.m_Middle.d;
						right5.m_Left = right4.m_Right;
						CheckSegment(input, raycastIndex, edge.m_End, entity, right4, curve.m_Bezier, netCompositionData3);
						CheckSegment(input, raycastIndex, edge.m_End, entity, right5, curve.m_Bezier, netCompositionData3);
					}
					else
					{
						Segment left2 = geometry3.m_Left;
						Segment right6 = geometry3.m_Right;
						CheckSegment(input, raycastIndex, edge.m_End, entity, left2, curve.m_Bezier, netCompositionData3);
						CheckSegment(input, raycastIndex, edge.m_End, entity, right6, curve.m_Bezier, netCompositionData3);
						left2.m_Right = geometry3.m_Middle;
						right6.m_Left = geometry3.m_Middle;
						CheckSegment(input, raycastIndex, edge.m_End, entity, left2, curve.m_Bezier, netCompositionData3);
						CheckSegment(input, raycastIndex, edge.m_End, entity, right6, curve.m_Bezier, netCompositionData3);
					}
				}
			}
			else
			{
				if ((input.m_Flags & RaycastFlags.Markers) == 0)
				{
					return;
				}
				Edge edge2 = m_EdgeData[entity];
				Curve curve2 = m_CurveData[entity];
				Bezier4x3 curve3 = curve2.m_Bezier;
				if ((input.m_Flags & RaycastFlags.ElevateOffset) != 0)
				{
					float maxElevation4 = 0f - input.m_Offset.y;
					SetElevationOffset(ref curve3, entity, edge2.m_Start, edge2.m_End, maxElevation4);
				}
				Segment val3 = input.m_Line + input.m_Offset;
				float2 val4 = default(float2);
				float num = MathUtils.Distance(curve3, val3, ref val4);
				if (num < 0.5f)
				{
					RaycastResult result = new RaycastResult
					{
						m_Owner = entity,
						m_Hit = 
						{
							m_HitEntity = entity,
							m_Position = MathUtils.Position(curve2.m_Bezier, val4.x),
							m_HitPosition = MathUtils.Position(val3, val4.y),
							m_NormalizedDistance = val4.y - (0.5f - num) / math.max(1f, MathUtils.Length(val3)),
							m_CurvePosition = val4.x
						}
					};
					if (ValidateResult(input, ref result))
					{
						m_Results.Accumulate(raycastIndex, result);
					}
				}
			}
		}

		private void SetElevationOffset(ref float3 position, Entity node, float maxElevation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (m_ElevationData.HasComponent(node))
			{
				Elevation elevation = m_ElevationData[node];
				float num = math.lerp(elevation.m_Elevation.x, elevation.m_Elevation.y, 0.5f);
				position.y -= math.min(num, maxElevation);
			}
		}

		private void SetElevationOffset(ref Bezier4x3 curve, Entity edge, Entity startNode, Entity endNode, float maxElevation)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			if (m_ElevationData.HasComponent(startNode))
			{
				Elevation elevation = m_ElevationData[startNode];
				val.x = math.lerp(elevation.m_Elevation.x, elevation.m_Elevation.y, 0.5f);
			}
			if (m_ElevationData.HasComponent(edge))
			{
				Elevation elevation2 = m_ElevationData[edge];
				val.y = math.lerp(elevation2.m_Elevation.x, elevation2.m_Elevation.y, 0.5f);
			}
			if (m_ElevationData.HasComponent(endNode))
			{
				Elevation elevation3 = m_ElevationData[endNode];
				val.z = math.lerp(elevation3.m_Elevation.x, elevation3.m_Elevation.y, 0.5f);
			}
			if (math.any(val != 0f))
			{
				SetElevationOffset(ref curve, ((float3)(ref val)).xy, maxElevation);
				SetElevationOffset(ref curve, ((float3)(ref val)).yz, maxElevation);
			}
		}

		private void SetElevationOffset(ref EdgeGeometry geometry, Entity edge, Entity startNode, Entity endNode, float maxElevation)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			if (m_ElevationData.HasComponent(startNode))
			{
				Elevation elevation = m_ElevationData[startNode];
				val.x = math.lerp(elevation.m_Elevation.x, elevation.m_Elevation.y, 0.5f);
			}
			if (m_ElevationData.HasComponent(edge))
			{
				Elevation elevation2 = m_ElevationData[edge];
				val.y = math.lerp(elevation2.m_Elevation.x, elevation2.m_Elevation.y, 0.5f);
			}
			if (m_ElevationData.HasComponent(endNode))
			{
				Elevation elevation3 = m_ElevationData[endNode];
				val.z = math.lerp(elevation3.m_Elevation.x, elevation3.m_Elevation.y, 0.5f);
			}
			if (math.any(val != 0f))
			{
				SetElevationOffset(ref geometry.m_Start.m_Left, ((float3)(ref val)).xy, maxElevation);
				SetElevationOffset(ref geometry.m_Start.m_Right, ((float3)(ref val)).xy, maxElevation);
				SetElevationOffset(ref geometry.m_End.m_Left, ((float3)(ref val)).yz, maxElevation);
				SetElevationOffset(ref geometry.m_End.m_Right, ((float3)(ref val)).yz, maxElevation);
			}
		}

		private void SetElevationOffset(ref EdgeNodeGeometry geometry, Entity node, float maxElevation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (m_ElevationData.HasComponent(node))
			{
				Elevation elevation = m_ElevationData[node];
				float offset = math.lerp(elevation.m_Elevation.x, elevation.m_Elevation.y, 0.5f);
				SetElevationOffset(ref geometry.m_Left.m_Left, offset, maxElevation);
				SetElevationOffset(ref geometry.m_Left.m_Right, offset, maxElevation);
				SetElevationOffset(ref geometry.m_Right.m_Left, offset, maxElevation);
				SetElevationOffset(ref geometry.m_Right.m_Right, offset, maxElevation);
				SetElevationOffset(ref geometry.m_Middle, offset, maxElevation);
			}
		}

		private void SetElevationOffset(ref Bezier4x3 curve, float offset, float maxElevation)
		{
			curve.a.y -= math.min(offset, maxElevation);
			curve.b.y -= math.min(offset, maxElevation);
			curve.c.y -= math.min(offset, maxElevation);
			curve.d.y -= math.min(offset, maxElevation);
		}

		private void SetElevationOffset(ref Bezier4x3 curve, float2 offset, float maxElevation)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			curve.a.y -= math.min(offset.x, maxElevation);
			curve.b.y -= math.min(math.lerp(offset.x, offset.y, 1f / 3f), maxElevation);
			curve.c.y -= math.min(math.lerp(offset.x, offset.y, 2f / 3f), maxElevation);
			curve.d.y -= math.min(offset.y, maxElevation);
		}

		private void CheckSegment(RaycastInput input, int raycastIndex, Entity owner, Entity hitEntity, Segment segment, Bezier4x3 curve, NetCompositionData prefabCompositionData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			Segment val = input.m_Line + input.m_Offset;
			float3 val2 = segment.m_Left.a;
			float3 val3 = segment.m_Right.a;
			if ((input.m_Flags & RaycastFlags.ElevateOffset) == 0)
			{
				val2.y += prefabCompositionData.m_SurfaceHeight.max;
				val3.y += prefabCompositionData.m_SurfaceHeight.max;
			}
			Triangle3 val7 = default(Triangle3);
			float3 val8 = default(float3);
			float num2 = default(float);
			float num3 = default(float);
			for (int i = 1; i <= 8; i++)
			{
				float num = (float)i / 8f;
				float3 val4 = MathUtils.Position(segment.m_Left, num);
				float3 val5 = MathUtils.Position(segment.m_Right, num);
				if ((input.m_Flags & RaycastFlags.ElevateOffset) == 0)
				{
					val4.y += prefabCompositionData.m_SurfaceHeight.max;
					val5.y += prefabCompositionData.m_SurfaceHeight.max;
				}
				Triangle3 val6 = new Triangle3(val2, val3, val4);
				((Triangle3)(ref val7))._002Ector(val5, val4, val3);
				if (MathUtils.Intersect(val6, val, ref val8))
				{
					float3 hitPosition = MathUtils.Position(val, val8.z);
					MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref hitPosition)).xz, ref num2);
					RaycastResult result = new RaycastResult
					{
						m_Owner = owner,
						m_Hit = 
						{
							m_HitEntity = hitEntity,
							m_Position = MathUtils.Position(curve, num2),
							m_HitPosition = hitPosition,
							m_NormalizedDistance = val8.z + 0.5f / math.max(1f, MathUtils.Length(val)),
							m_CurvePosition = num2
						}
					};
					if (ValidateResult(input, ref result))
					{
						m_Results.Accumulate(raycastIndex, result);
					}
				}
				else if (MathUtils.Intersect(val7, val, ref val8))
				{
					float3 hitPosition2 = MathUtils.Position(val, val8.z);
					MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref hitPosition2)).xz, ref num3);
					RaycastResult result2 = new RaycastResult
					{
						m_Owner = owner,
						m_Hit = 
						{
							m_HitEntity = hitEntity,
							m_Position = MathUtils.Position(curve, num3),
							m_HitPosition = hitPosition2,
							m_NormalizedDistance = val8.z + 0.5f / math.max(1f, MathUtils.Length(val)),
							m_CurvePosition = num3
						}
					};
					if (ValidateResult(input, ref result2))
					{
						m_Results.Accumulate(raycastIndex, result2);
					}
				}
				val2 = val4;
				val3 = val5;
			}
		}

		private bool ValidateResult(RaycastInput input, ref RaycastResult result)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			TypeMask typeMask = TypeMask.Net;
			Entity owner = Entity.Null;
			TypeMask typeMask2 = TypeMask.None;
			DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
			Owner owner2 = default(Owner);
			while (true)
			{
				if ((input.m_Flags & RaycastFlags.UpgradeIsMain) != 0)
				{
					if (m_ServiceUpgradeData.HasComponent(result.m_Owner))
					{
						break;
					}
					if (m_InstalledUpgrades.TryGetBuffer(result.m_Owner, ref val) && val.Length != 0)
					{
						owner = Entity.Null;
						typeMask2 = TypeMask.None;
						typeMask = TypeMask.StaticObjects;
						result.m_Owner = val[0].m_Upgrade;
						break;
					}
				}
				else if ((input.m_Flags & RaycastFlags.SubBuildings) != 0 && m_ServiceUpgradeData.HasComponent(result.m_Owner) && (typeMask == TypeMask.Net || m_BuildingData.HasComponent(result.m_Owner)))
				{
					break;
				}
				if (!m_OwnerData.TryGetComponent(result.m_Owner, ref owner2))
				{
					break;
				}
				if ((input.m_TypeMask & typeMask) != TypeMask.None && (typeMask != TypeMask.Net || typeMask2 != TypeMask.Net || (input.m_Flags & RaycastFlags.ElevateOffset) == 0))
				{
					owner = result.m_Owner;
					typeMask2 = typeMask;
				}
				result.m_Owner = owner2.m_Owner;
				typeMask = ((!m_EdgeData.HasComponent(result.m_Owner)) ? TypeMask.StaticObjects : TypeMask.Net);
			}
			if ((input.m_Flags & RaycastFlags.SubElements) != 0 && (input.m_TypeMask & typeMask2) != TypeMask.None)
			{
				result.m_Owner = owner;
				typeMask = typeMask2;
			}
			else if ((input.m_Flags & RaycastFlags.NoMainElements) != 0)
			{
				return false;
			}
			if ((input.m_TypeMask & typeMask) == 0)
			{
				return false;
			}
			switch (typeMask)
			{
			case TypeMask.Net:
			{
				PrefabRef prefabRef = m_PrefabRefData[result.m_Owner];
				return (m_PrefabNetData[prefabRef.m_Prefab].m_ConnectLayers & input.m_NetLayerMask) != 0;
			}
			case TypeMask.StaticObjects:
				return CheckPlaceholder(input, ref result.m_Owner);
			default:
				return true;
			}
		}

		private bool CheckPlaceholder(RaycastInput input, ref Entity entity)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if ((input.m_Flags & RaycastFlags.Placeholders) != 0)
			{
				return true;
			}
			if (m_PlaceholderData.HasComponent(entity))
			{
				if (m_AttachmentData.HasComponent(entity))
				{
					Attachment attachment = m_AttachmentData[entity];
					if (m_PrefabRefData.HasComponent(attachment.m_Attached))
					{
						entity = attachment.m_Attached;
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}

	[BurstCompile]
	public struct RaycastLabelsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public NativeArray<RaycastSystem.EntityResult> m_Edges;

		[ReadOnly]
		public ComponentLookup<Aggregated> m_AggregatedData;

		[ReadOnly]
		public ComponentLookup<LabelExtents> m_LabelExtentsData;

		[ReadOnly]
		public BufferLookup<LabelPosition> m_LabelPositions;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			RaycastSystem.EntityResult entityResult = m_Edges[index];
			RaycastInput input = m_Input[entityResult.m_RaycastIndex];
			Aggregated aggregated = default(Aggregated);
			if ((input.m_TypeMask & TypeMask.Labels) != TypeMask.None && m_AggregatedData.TryGetComponent(entityResult.m_Entity, ref aggregated))
			{
				CheckAggregate(entityResult.m_RaycastIndex, input, aggregated.m_Aggregate);
			}
		}

		private void CheckAggregate(int raycastIndex, RaycastInput input, Entity aggregate)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
			LabelExtents labelExtents = default(LabelExtents);
			if (!m_LabelExtentsData.TryGetComponent(aggregate, ref labelExtents))
			{
				return;
			}
			DynamicBuffer<LabelPosition> val = m_LabelPositions[aggregate];
			Bounds1 val5 = default(Bounds1);
			Bounds1 val6 = default(Bounds1);
			float3 val8 = default(float3);
			Quad3 val9 = default(Quad3);
			float num4 = default(float);
			float3 val12 = default(float3);
			float3 val15 = default(float3);
			float num7 = default(float);
			float3 val17 = default(float3);
			float num9 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				LabelPosition labelPosition = val[i];
				if ((NetUtils.GetCollisionMask(labelPosition) & input.m_CollisionMask) == 0)
				{
					continue;
				}
				float3 val2 = MathUtils.Position(labelPosition.m_Curve, 0.5f);
				float num = math.max(math.sqrt(math.distance(input.m_Line.a, val2) * 0.0001f), 0.01f);
				if (num >= labelPosition.m_MaxScale * 0.95f)
				{
					continue;
				}
				float3 val3 = MathUtils.Tangent(labelPosition.m_Curve, 0.5f);
				Bounds2 val4 = (Bounds2)((math.dot(m_CameraRight, val3) < 0f) ? new Bounds2(-labelExtents.m_Bounds.max, -labelExtents.m_Bounds.min) : labelExtents.m_Bounds);
				val4 *= float2.op_Implicit(num);
				((Bounds1)(ref val5))._002Ector(0f, 1f);
				((Bounds1)(ref val6))._002Ector(0f, 1f);
				float num2 = 0f - val4.min.x - labelPosition.m_HalfLength;
				float num3 = val4.max.x - labelPosition.m_HalfLength;
				if (num2 < 0f)
				{
					MathUtils.ClampLength(labelPosition.m_Curve, ref val5, 0f - num2);
				}
				else
				{
					val5.max = 0f;
				}
				if (num3 < 0f)
				{
					MathUtils.ClampLengthInverse(labelPosition.m_Curve, ref val6, 0f - num3);
				}
				else
				{
					val6.min = 1f;
				}
				if (num2 > 0f)
				{
					float3 val7 = math.normalizesafe(MathUtils.StartTangent(labelPosition.m_Curve), default(float3));
					((float3)(ref val8))._002Ector(0f - val7.z, 0f, val7.x);
					val9.a = labelPosition.m_Curve.a - val7 * num2 + val8 * val4.min.y;
					val9.b = labelPosition.m_Curve.a - val7 * num2 + val8 * val4.max.y;
					val9.c = labelPosition.m_Curve.a + val8 * val4.max.y;
					val9.d = labelPosition.m_Curve.a + val8 * val4.min.y;
					if (MathUtils.Intersect(val9, input.m_Line, ref num4))
					{
						float num5 = MathUtils.Size(((Bounds2)(ref val4)).y);
						RaycastResult raycastResult = default(RaycastResult);
						raycastResult.m_Owner = aggregate;
						raycastResult.m_Hit.m_HitEntity = raycastResult.m_Owner;
						raycastResult.m_Hit.m_Position = val2;
						raycastResult.m_Hit.m_HitPosition = MathUtils.Position(input.m_Line, num4);
						raycastResult.m_Hit.m_NormalizedDistance = num4 - num5 / math.max(1f, MathUtils.Length(input.m_Line));
						raycastResult.m_Hit.m_CellIndex = new int2(labelPosition.m_ElementIndex, -1);
						m_Results.Accumulate(raycastIndex, raycastResult);
					}
				}
				else
				{
					float3 val10 = MathUtils.Position(labelPosition.m_Curve, val5.max);
					float3 val11 = math.normalizesafe(MathUtils.Tangent(labelPosition.m_Curve, val5.max), default(float3));
					((float3)(ref val12))._002Ector(0f - val11.z, 0f, val11.x);
					val9.c = val10 + val12 * val4.max.y;
					val9.d = val10 + val12 * val4.min.y;
				}
				for (int j = 1; j <= 16; j++)
				{
					float num6 = math.lerp(val5.max, val6.min, (float)j * 0.0625f);
					float3 val13 = MathUtils.Position(labelPosition.m_Curve, num6);
					float3 val14 = math.normalizesafe(MathUtils.Tangent(labelPosition.m_Curve, num6), default(float3));
					((float3)(ref val15))._002Ector(0f - val14.z, 0f, val14.x);
					val9.a = val9.d;
					val9.b = val9.c;
					val9.c = val13 + val15 * val4.max.y;
					val9.d = val13 + val15 * val4.min.y;
					if (MathUtils.Intersect(val9, input.m_Line, ref num7))
					{
						float num8 = MathUtils.Size(((Bounds2)(ref val4)).y);
						RaycastResult raycastResult2 = default(RaycastResult);
						raycastResult2.m_Owner = aggregate;
						raycastResult2.m_Hit.m_HitEntity = raycastResult2.m_Owner;
						raycastResult2.m_Hit.m_Position = val2;
						raycastResult2.m_Hit.m_HitPosition = MathUtils.Position(input.m_Line, num7);
						raycastResult2.m_Hit.m_NormalizedDistance = num7 - num8 / math.max(1f, MathUtils.Length(input.m_Line));
						raycastResult2.m_Hit.m_CellIndex = new int2(labelPosition.m_ElementIndex, -1);
						m_Results.Accumulate(raycastIndex, raycastResult2);
					}
				}
				if (num3 > 0f)
				{
					float3 val16 = math.normalizesafe(MathUtils.EndTangent(labelPosition.m_Curve), default(float3));
					((float3)(ref val17))._002Ector(0f - val16.z, 0f, val16.x);
					val9.a = val9.d;
					val9.b = val9.c;
					val9.c = labelPosition.m_Curve.d + val16 * num3 + val17 * val4.min.y;
					val9.d = labelPosition.m_Curve.d + val16 * num3 + val17 * val4.max.y;
					if (MathUtils.Intersect(val9, input.m_Line, ref num9))
					{
						float num10 = MathUtils.Size(((Bounds2)(ref val4)).y);
						RaycastResult raycastResult3 = default(RaycastResult);
						raycastResult3.m_Owner = aggregate;
						raycastResult3.m_Hit.m_HitEntity = raycastResult3.m_Owner;
						raycastResult3.m_Hit.m_Position = val2;
						raycastResult3.m_Hit.m_HitPosition = MathUtils.Position(input.m_Line, num9);
						raycastResult3.m_Hit.m_NormalizedDistance = num9 - num10 / math.max(1f, MathUtils.Length(input.m_Line));
						raycastResult3.m_Hit.m_CellIndex = new int2(labelPosition.m_ElementIndex, -1);
						m_Results.Accumulate(raycastIndex, raycastResult3);
					}
				}
			}
		}
	}

	[BurstCompile]
	public struct RaycastLanesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public float m_FovTan;

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public NativeArray<RaycastSystem.EntityResult> m_Lanes;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			RaycastSystem.EntityResult entityResult = m_Lanes[index];
			RaycastInput raycastInput = m_Input[entityResult.m_RaycastIndex];
			if ((raycastInput.m_TypeMask & TypeMask.Lanes) == 0)
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[entityResult.m_Entity];
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & raycastInput.m_UtilityTypeMask) != UtilityTypes.None)
			{
				Curve curve = m_CurveData[entityResult.m_Entity];
				float2 val = default(float2);
				float num = MathUtils.Distance(curve.m_Bezier, raycastInput.m_Line, ref val);
				float3 val2 = MathUtils.Position(raycastInput.m_Line, val.y);
				float cameraDistance = math.distance(val2, raycastInput.m_Line.a);
				float num2 = GetMinLaneRadius(m_FovTan, cameraDistance);
				NetLaneGeometryData netLaneGeometryData = default(NetLaneGeometryData);
				if (m_PrefabLaneGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netLaneGeometryData))
				{
					num2 = math.max(num2, netLaneGeometryData.m_Size.x * 0.5f);
				}
				if (num < num2)
				{
					RaycastResult raycastResult = default(RaycastResult);
					raycastResult.m_Owner = entityResult.m_Entity;
					raycastResult.m_Hit.m_HitEntity = raycastResult.m_Owner;
					raycastResult.m_Hit.m_Position = MathUtils.Position(curve.m_Bezier, val.x);
					raycastResult.m_Hit.m_HitPosition = val2;
					raycastResult.m_Hit.m_CurvePosition = val.x;
					raycastResult.m_Hit.m_NormalizedDistance = val.y - (num2 - num) / math.max(1f, MathUtils.Length(raycastInput.m_Line));
					m_Results.Accumulate(entityResult.m_RaycastIndex, raycastResult);
				}
			}
		}
	}

	public static float GetMinLaneRadius(float fovTan, float cameraDistance)
	{
		return cameraDistance * fovTan * 0.01f;
	}
}
