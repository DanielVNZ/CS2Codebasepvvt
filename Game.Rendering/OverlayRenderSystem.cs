using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.TextCore.LowLevel;

namespace Game.Rendering;

public class OverlayRenderSystem : GameSystemBase
{
	public struct CurveData
	{
		public Matrix4x4 m_Matrix;

		public Matrix4x4 m_InverseMatrix;

		public Matrix4x4 m_Curve;

		public Color m_OutlineColor;

		public Color m_FillColor;

		public float2 m_Size;

		public float2 m_DashLengths;

		public float2 m_Roundness;

		public float m_OutlineWidth;

		public float m_FillStyle;
	}

	public struct BoundsData
	{
		public Bounds3 m_CurveBounds;
	}

	[Flags]
	public enum StyleFlags
	{
		Grid = 1,
		Projected = 2
	}

	public struct Buffer
	{
		private NativeList<CurveData> m_ProjectedCurves;

		private NativeList<CurveData> m_AbsoluteCurves;

		private NativeValue<BoundsData> m_Bounds;

		private float m_PositionY;

		private float m_ScaleY;

		public Buffer(NativeList<CurveData> projectedCurves, NativeList<CurveData> absoluteCurves, NativeValue<BoundsData> bounds, float positionY, float scaleY)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_ProjectedCurves = projectedCurves;
			m_AbsoluteCurves = absoluteCurves;
			m_Bounds = bounds;
			m_PositionY = positionY;
			m_ScaleY = scaleY;
		}

		public void DrawCircle(Color color, float3 position, float diameter)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			DrawCircleImpl(color, color, 0f, (StyleFlags)0, new float2(0f, 1f), position, diameter);
		}

		public void DrawCircle(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, float2 direction, float3 position, float diameter)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			DrawCircleImpl(outlineColor, fillColor, outlineWidth, styleFlags, direction, position, diameter);
		}

		private void DrawCircleImpl(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, float2 direction, float3 position, float diameter)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			CurveData curveData = default(CurveData);
			curveData.m_Size = new float2(diameter, diameter);
			curveData.m_DashLengths = new float2(0f, diameter);
			curveData.m_Roundness = new float2(1f, 1f);
			curveData.m_OutlineWidth = outlineWidth;
			curveData.m_FillStyle = (float)(styleFlags & StyleFlags.Grid);
			curveData.m_Curve = new Matrix4x4(float4.op_Implicit(new float4(position, 0f)), float4.op_Implicit(new float4(position, 0f)), float4.op_Implicit(new float4(position, 0f)), float4.op_Implicit(new float4(position, 0f)));
			curveData.m_OutlineColor = ((Color)(ref outlineColor)).linear;
			curveData.m_FillColor = ((Color)(ref fillColor)).linear;
			Bounds3 bounds;
			if ((styleFlags & StyleFlags.Projected) != 0)
			{
				curveData.m_Matrix = FitBox(direction, position, diameter, out bounds);
				curveData.m_InverseMatrix = ((Matrix4x4)(ref curveData.m_Matrix)).inverse;
				m_ProjectedCurves.Add(ref curveData);
			}
			else
			{
				curveData.m_Matrix = FitQuad(direction, position, diameter, out bounds);
				curveData.m_InverseMatrix = ((Matrix4x4)(ref curveData.m_Matrix)).inverse;
				m_AbsoluteCurves.Add(ref curveData);
			}
			BoundsData value = m_Bounds.value;
			ref Bounds3 curveBounds = ref value.m_CurveBounds;
			curveBounds |= bounds;
			m_Bounds.value = value;
		}

		public void DrawLine(Color color, Segment line, float width)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Segment)(ref line)).xz);
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, NetUtils.StraightCurve(line.a, line.b), width, num + width * 2f, 0f, default(float2), num);
		}

		public void DrawLine(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Segment line, float width)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Segment)(ref line)).xz);
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, NetUtils.StraightCurve(line.a, line.b), width, num + width * 2f, 0f, default(float2), num);
		}

		public void DrawLine(Color color, Segment line, float width, float2 roundness)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Segment)(ref line)).xz);
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, NetUtils.StraightCurve(line.a, line.b), width, num + width * 2f, 0f, roundness, num);
		}

		public void DrawLine(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Segment line, float width, float2 roundness)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Segment)(ref line)).xz);
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, NetUtils.StraightCurve(line.a, line.b), width, num + width * 2f, 0f, roundness, num);
		}

		public void DrawDashedLine(Color color, Segment line, float width, float dashLength, float gapLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, NetUtils.StraightCurve(line.a, line.b), width, dashLength, gapLength, default(float2), MathUtils.Length(((Segment)(ref line)).xz));
		}

		public void DrawDashedLine(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Segment line, float width, float dashLength, float gapLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, NetUtils.StraightCurve(line.a, line.b), width, dashLength, gapLength, default(float2), MathUtils.Length(((Segment)(ref line)).xz));
		}

		public void DrawDashedLine(Color color, Segment line, float width, float dashLength, float gapLength, float2 roundness)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, NetUtils.StraightCurve(line.a, line.b), width, dashLength, gapLength, roundness, MathUtils.Length(((Segment)(ref line)).xz));
		}

		public void DrawDashedLine(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Segment line, float width, float dashLength, float gapLength, float2 roundness)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, NetUtils.StraightCurve(line.a, line.b), width, dashLength, gapLength, roundness, MathUtils.Length(((Segment)(ref line)).xz));
		}

		public void DrawCurve(Color color, Bezier4x3 curve, float width)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref curve)).xz);
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, curve, width, num + width * 2f, 0f, default(float2), num);
		}

		public void DrawCurve(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Bezier4x3 curve, float width)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref curve)).xz);
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, curve, width, num + width * 2f, 0f, default(float2), num);
		}

		public void DrawCurve(Color color, Bezier4x3 curve, float width, float2 roundness)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref curve)).xz);
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, curve, width, num + width * 2f, 0f, roundness, num);
		}

		public void DrawCurve(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Bezier4x3 curve, float width, float2 roundness)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref curve)).xz);
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, curve, width, num + width * 2f, 0f, roundness, num);
		}

		public void DrawDashedCurve(Color color, Bezier4x3 curve, float width, float dashLength, float gapLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(color, color, 0f, (StyleFlags)0, curve, width, dashLength, gapLength, default(float2), MathUtils.Length(((Bezier4x3)(ref curve)).xz));
		}

		public void DrawDashedCurve(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Bezier4x3 curve, float width, float dashLength, float gapLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			DrawCurveImpl(outlineColor, fillColor, outlineWidth, styleFlags, curve, width, dashLength, gapLength, default(float2), MathUtils.Length(((Bezier4x3)(ref curve)).xz));
		}

		private void DrawCurveImpl(Color outlineColor, Color fillColor, float outlineWidth, StyleFlags styleFlags, Bezier4x3 curve, float width, float dashLength, float gapLength, float2 roundness, float length)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (!(length < 0.01f))
			{
				CurveData curveData = default(CurveData);
				curveData.m_Size = new float2(width, length);
				curveData.m_DashLengths = new float2(gapLength, dashLength);
				curveData.m_Roundness = roundness;
				curveData.m_OutlineWidth = outlineWidth;
				curveData.m_FillStyle = (float)(styleFlags & StyleFlags.Grid);
				curveData.m_Curve = float4x4.op_Implicit(BuildCurveMatrix(curve, length));
				curveData.m_OutlineColor = ((Color)(ref outlineColor)).linear;
				curveData.m_FillColor = ((Color)(ref fillColor)).linear;
				Bounds3 bounds;
				if ((styleFlags & StyleFlags.Projected) != 0)
				{
					curveData.m_Matrix = FitBox(curve, width, out bounds);
					curveData.m_InverseMatrix = ((Matrix4x4)(ref curveData.m_Matrix)).inverse;
					m_ProjectedCurves.Add(ref curveData);
				}
				else
				{
					curveData.m_Matrix = FitQuad(curve, width, out bounds);
					curveData.m_InverseMatrix = ((Matrix4x4)(ref curveData.m_Matrix)).inverse;
					m_AbsoluteCurves.Add(ref curveData);
				}
				BoundsData value = m_Bounds.value;
				ref Bounds3 curveBounds = ref value.m_CurveBounds;
				curveBounds |= bounds;
				m_Bounds.value = value;
			}
		}

		private Matrix4x4 FitBox(float2 direction, float3 position, float extend, out Bounds3 bounds)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			bounds = new Bounds3(position, position);
			ref float3 min = ref bounds.min;
			((float3)(ref min)).xz = ((float3)(ref min)).xz - extend;
			ref float3 max = ref bounds.max;
			((float3)(ref max)).xz = ((float3)(ref max)).xz + extend;
			bounds.min.y = m_PositionY;
			bounds.max.y = m_ScaleY;
			position.y = m_PositionY;
			quaternion val = quaternion.RotateY(math.atan2(direction.x, direction.y));
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(extend, m_ScaleY, extend);
			return Matrix4x4.TRS(float3.op_Implicit(position), quaternion.op_Implicit(val), float3.op_Implicit(val2));
		}

		private Matrix4x4 FitQuad(float2 direction, float3 position, float extend, out Bounds3 bounds)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			bounds = new Bounds3(position, position);
			ref float3 min = ref bounds.min;
			((float3)(ref min)).xz = ((float3)(ref min)).xz - extend;
			ref float3 max = ref bounds.max;
			((float3)(ref max)).xz = ((float3)(ref max)).xz + extend;
			quaternion val = quaternion.RotateY(math.atan2(direction.x, direction.y));
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(extend, 1f, extend);
			return Matrix4x4.TRS(float3.op_Implicit(position), quaternion.op_Implicit(val), float3.op_Implicit(val2));
		}

		private Matrix4x4 FitBox(Bezier4x3 curve, float extend, out Bounds3 bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			bounds = MathUtils.Bounds(curve);
			ref float3 min = ref bounds.min;
			((float3)(ref min)).xz = ((float3)(ref min)).xz - extend;
			ref float3 max = ref bounds.max;
			((float3)(ref max)).xz = ((float3)(ref max)).xz + extend;
			bounds.min.y = m_PositionY;
			bounds.max.y = m_ScaleY;
			float3 val = default(float3);
			((float3)(ref val))._002Ector(0f, m_PositionY, 0f);
			quaternion identity = quaternion.identity;
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(0f, m_ScaleY, 0f);
			float2 val3 = ((float3)(ref curve.d)).xz - ((float3)(ref curve.a)).xz;
			if (MathUtils.TryNormalize(ref val3))
			{
				float2 val4 = MathUtils.Right(val3);
				float2 val5 = ((float3)(ref curve.b)).xz - ((float3)(ref curve.a)).xz;
				float2 val6 = ((float3)(ref curve.c)).xz - ((float3)(ref curve.a)).xz;
				float2 val7 = ((float3)(ref curve.d)).xz - ((float3)(ref curve.a)).xz;
				float2 val8 = default(float2);
				((float2)(ref val8))._002Ector(math.dot(val5, val4), math.dot(val5, val3));
				float2 val9 = default(float2);
				((float2)(ref val9))._002Ector(math.dot(val6, val4), math.dot(val6, val3));
				float2 val10 = default(float2);
				((float2)(ref val10))._002Ector(math.dot(val7, val4), math.dot(val7, val3));
				float2 val11 = math.min(math.min(float2.op_Implicit(0f), val8), math.min(val9, val10));
				float2 val12 = math.max(math.max(float2.op_Implicit(0f), val8), math.max(val9, val10));
				float2 val13 = math.lerp(val11, val12, 0.5f);
				identity = quaternion.LookRotation(new float3(val3.x, 0f, val3.y), new float3(0f, 1f, 0f));
				float2 xz = ((float3)(ref curve.a)).xz;
				float3 val14 = math.rotate(identity, new float3(val13.x, 0f, val13.y));
				((float3)(ref val)).xz = xz + ((float3)(ref val14)).xz;
				((float3)(ref val2)).xz = (val12 - val11) * 0.5f + extend;
			}
			else
			{
				((float3)(ref val)).xz = MathUtils.Center(((Bounds3)(ref bounds)).xz);
				identity = quaternion.identity;
				((float3)(ref val2)).xz = MathUtils.Extents(((Bounds3)(ref bounds)).xz);
			}
			return Matrix4x4.TRS(float3.op_Implicit(val), quaternion.op_Implicit(identity), float3.op_Implicit(val2));
		}

		private Matrix4x4 FitQuad(Bezier4x3 curve, float extend, out Bounds3 bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			bounds = MathUtils.Bounds(curve);
			ref float3 min = ref bounds.min;
			((float3)(ref min)).xz = ((float3)(ref min)).xz - extend;
			ref float3 max = ref bounds.max;
			((float3)(ref max)).xz = ((float3)(ref max)).xz + extend;
			float3 val = MathUtils.Center(bounds);
			quaternion val2 = quaternion.identity;
			float3 val3 = float3.op_Implicit(0f);
			((float3)(ref val3)).xz = MathUtils.Extents(((Bounds3)(ref bounds)).xz);
			val3.y = 1f;
			float3 val4 = curve.d - curve.a;
			float num = math.length(val4);
			if (num > 0.1f)
			{
				val4 /= num;
				float3 val5 = math.cross(val4, curve.b - curve.a);
				float3 val6 = math.cross(val4, curve.d - curve.c);
				val5 = math.select(val5, -val5, val5.y < 0f);
				val6 = math.select(val6, -val6, val6.y < 0f);
				float3 val7 = val5 + val6;
				float num2 = math.length(val7);
				val7 = math.lerp(new float3(0f, 1f, 0f), val7, math.saturate(num2 / num * 10f));
				val7 = math.normalize(val7);
				float3 val8 = math.cross(val7, val4);
				if (MathUtils.TryNormalize(ref val8))
				{
					float3 val9 = curve.b - curve.a;
					float3 val10 = curve.c - curve.a;
					float3 val11 = curve.d - curve.a;
					float2 val12 = default(float2);
					((float2)(ref val12))._002Ector(math.dot(val9, val8), math.dot(val9, val4));
					float2 val13 = default(float2);
					((float2)(ref val13))._002Ector(math.dot(val10, val8), math.dot(val10, val4));
					float2 val14 = default(float2);
					((float2)(ref val14))._002Ector(math.dot(val11, val8), math.dot(val11, val4));
					float2 val15 = math.min(math.min(float2.op_Implicit(0f), val12), math.min(val13, val14));
					float2 val16 = math.max(math.max(float2.op_Implicit(0f), val12), math.max(val13, val14));
					float2 val17 = math.lerp(val15, val16, 0.5f);
					val2 = quaternion.LookRotation(val4, val7);
					val = curve.a + math.rotate(val2, new float3(val17.x, 0f, val17.y));
					((float3)(ref val3)).xz = (val16 - val15) * 0.5f + extend;
				}
			}
			return Matrix4x4.TRS(float3.op_Implicit(val), quaternion.op_Implicit(val2), float3.op_Implicit(val3));
		}

		private static float4x4 BuildCurveMatrix(Bezier4x3 curve, float length)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			val.x = math.distance(curve.a, curve.b);
			val.y = math.distance(curve.c, curve.d);
			val /= length;
			return new float4x4
			{
				c0 = new float4(curve.a, 0f),
				c1 = new float4(curve.b, val.x),
				c2 = new float4(curve.c, 1f - val.y),
				c3 = new float4(curve.d, 1f)
			};
		}
	}

	private RenderingSystem m_RenderingSystem;

	private TerrainSystem m_TerrainSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_SettingsQuery;

	private Mesh m_BoxMesh;

	private Mesh m_QuadMesh;

	private Material m_ProjectedMaterial;

	private Material m_AbsoluteMaterial;

	private ComputeBuffer m_ArgsBuffer;

	private ComputeBuffer m_ProjectedBuffer;

	private ComputeBuffer m_AbsoluteBuffer;

	private List<uint> m_ArgsArray;

	private int m_ProjectedInstanceCount;

	private int m_AbsoluteInstanceCount;

	private int m_CurveBufferID;

	private int m_GradientScaleID;

	private int m_ScaleRatioAID;

	private int m_FaceDilateID;

	private NativeList<CurveData> m_ProjectedData;

	private NativeList<CurveData> m_AbsoluteData;

	private NativeValue<BoundsData> m_BoundsData;

	private JobHandle m_BufferWriters;

	private TextMeshPro m_TextMesh;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OverlayConfigurationData>() });
		m_CurveBufferID = Shader.PropertyToID("colossal_OverlayCurveBuffer");
		m_GradientScaleID = Shader.PropertyToID("_GradientScale");
		m_ScaleRatioAID = Shader.PropertyToID("_ScaleRatioA");
		m_FaceDilateID = Shader.PropertyToID("_FaceDilate");
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		if ((Object)(object)m_BoxMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_BoxMesh);
		}
		if ((Object)(object)m_QuadMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_QuadMesh);
		}
		if ((Object)(object)m_ProjectedMaterial != (Object)null)
		{
			Object.Destroy((Object)(object)m_ProjectedMaterial);
		}
		if ((Object)(object)m_AbsoluteMaterial != (Object)null)
		{
			Object.Destroy((Object)(object)m_AbsoluteMaterial);
		}
		if (m_ArgsBuffer != null)
		{
			m_ArgsBuffer.Release();
		}
		if (m_ProjectedBuffer != null)
		{
			m_ProjectedBuffer.Release();
		}
		if (m_AbsoluteBuffer != null)
		{
			m_AbsoluteBuffer.Release();
		}
		if (m_ProjectedData.IsCreated)
		{
			m_ProjectedData.Dispose();
		}
		if (m_AbsoluteData.IsCreated)
		{
			m_AbsoluteData.Dispose();
		}
		if (m_BoundsData.IsCreated)
		{
			m_BoundsData.Dispose();
		}
		if ((Object)(object)m_TextMesh != (Object)null)
		{
			for (int i = 0; i < ((TMP_Text)m_TextMesh).font.fallbackFontAssetTable.Count; i++)
			{
				Object.Destroy((Object)(object)((TMP_Text)m_TextMesh).font.fallbackFontAssetTable[i]);
			}
			Object.Destroy((Object)(object)((TMP_Text)m_TextMesh).font);
			Object.Destroy((Object)(object)((Component)m_TextMesh).gameObject);
		}
		base.OnDestroy();
	}

	public Buffer GetBuffer(out JobHandle dependencies)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!m_ProjectedData.IsCreated)
		{
			m_ProjectedData = new NativeList<CurveData>(AllocatorHandle.op_Implicit((Allocator)4));
		}
		if (!m_AbsoluteData.IsCreated)
		{
			m_AbsoluteData = new NativeList<CurveData>(AllocatorHandle.op_Implicit((Allocator)4));
		}
		if (!m_BoundsData.IsCreated)
		{
			m_BoundsData = new NativeValue<BoundsData>((Allocator)4);
		}
		dependencies = m_BufferWriters;
		return new Buffer(m_ProjectedData, m_AbsoluteData, m_BoundsData, m_TerrainSystem.heightScaleOffset.y - 50f, m_TerrainSystem.heightScaleOffset.x + 100f);
	}

	public void AddBufferWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_BufferWriters = JobHandle.CombineDependencies(m_BufferWriters, handle);
	}

	public TextMeshPro GetTextMesh()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if ((Object)(object)m_TextMesh == (Object)null)
		{
			OverlayConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<OverlayConfigurationPrefab>(m_SettingsQuery);
			GameObject val = new GameObject("TextMeshPro");
			Object.DontDestroyOnLoad((Object)(object)val);
			m_TextMesh = val.AddComponent<TextMeshPro>();
			((TMP_Text)m_TextMesh).font = CreateFont(singletonPrefab.m_FontInfos[0]);
			((TMP_Text)m_TextMesh).font.fallbackFontAssetTable = new List<TMP_FontAsset>(singletonPrefab.m_FontInfos.Length - 1);
			for (int i = 1; i < singletonPrefab.m_FontInfos.Length; i++)
			{
				((TMP_Text)m_TextMesh).font.fallbackFontAssetTable.Add(CreateFont(singletonPrefab.m_FontInfos[i]));
			}
			m_TextMesh.renderer.enabled = false;
		}
		return m_TextMesh;
	}

	private TMP_FontAsset CreateFont(FontInfo info)
	{
		TMP_FontAsset obj = TMP_FontAsset.CreateFontAsset(info.m_Font, info.m_SamplingPointSize, info.m_AtlasPadding, (GlyphRenderMode)4169, info.m_AtlasWidth, info.m_AtlasHeight, (AtlasPopulationMode)1, true);
		((TMP_Asset)obj).material.SetFloat(m_FaceDilateID, 1f);
		return obj;
	}

	public void CopyFontAtlasParameters(Material source, Material target)
	{
		target.SetFloat(m_GradientScaleID, source.GetFloat(m_GradientScaleID) * 2f);
		target.SetFloat(m_ScaleRatioAID, source.GetFloat(m_ScaleRatioAID));
		target.mainTexture = source.mainTexture;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_BufferWriters)).Complete();
		m_BufferWriters = default(JobHandle);
		m_ProjectedInstanceCount = 0;
		m_AbsoluteInstanceCount = 0;
		if ((!m_ProjectedData.IsCreated || m_ProjectedData.Length == 0) && (!m_AbsoluteData.IsCreated || m_AbsoluteData.Length == 0))
		{
			return;
		}
		if (((EntityQuery)(ref m_SettingsQuery)).IsEmptyIgnoreFilter)
		{
			if (m_ProjectedData.IsCreated)
			{
				m_ProjectedData.Clear();
			}
			if (m_AbsoluteData.IsCreated)
			{
				m_AbsoluteData.Clear();
			}
			return;
		}
		if (m_ProjectedData.IsCreated && m_ProjectedData.Length != 0)
		{
			m_ProjectedInstanceCount = m_ProjectedData.Length;
			GetCurveMaterial(ref m_ProjectedMaterial, projected: true);
			GetCurveBuffer(ref m_ProjectedBuffer, m_ProjectedInstanceCount);
			m_ProjectedBuffer.SetData<CurveData>(m_ProjectedData.AsArray(), 0, 0, m_ProjectedInstanceCount);
			m_ProjectedMaterial.SetBuffer(m_CurveBufferID, m_ProjectedBuffer);
			m_ProjectedData.Clear();
		}
		if (m_AbsoluteData.IsCreated && m_AbsoluteData.Length != 0)
		{
			m_AbsoluteInstanceCount = m_AbsoluteData.Length;
			GetCurveMaterial(ref m_AbsoluteMaterial, projected: false);
			GetCurveBuffer(ref m_AbsoluteBuffer, m_AbsoluteInstanceCount);
			m_AbsoluteBuffer.SetData<CurveData>(m_AbsoluteData.AsArray(), 0, 0, m_AbsoluteInstanceCount);
			m_AbsoluteMaterial.SetBuffer(m_CurveBufferID, m_AbsoluteBuffer);
			m_AbsoluteData.Clear();
		}
	}

	private void Render(ScriptableRenderContext context, List<Camera> cameras)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Invalid comparison between Unknown and I4
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Invalid comparison between Unknown and I4
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (m_RenderingSystem.hideOverlay)
			{
				return;
			}
			int num = 0;
			if (m_ProjectedInstanceCount != 0)
			{
				num += 5;
			}
			if (m_AbsoluteInstanceCount != 0)
			{
				num += 5;
			}
			if (num == 0)
			{
				return;
			}
			if (m_ArgsBuffer != null && m_ArgsBuffer.count < num)
			{
				m_ArgsBuffer.Release();
				m_ArgsBuffer = null;
			}
			if (m_ArgsBuffer == null)
			{
				m_ArgsBuffer = new ComputeBuffer(num, 4, (ComputeBufferType)256);
				m_ArgsBuffer.name = "Overlay args buffer";
			}
			if (m_ArgsArray == null)
			{
				m_ArgsArray = new List<uint>();
			}
			m_ArgsArray.Clear();
			Bounds val = RenderingUtils.ToBounds(m_BoundsData.value.m_CurveBounds);
			int num2 = 0;
			int num3 = 0;
			if (m_ProjectedInstanceCount != 0)
			{
				GetMesh(ref m_BoxMesh, box: true);
				GetCurveMaterial(ref m_ProjectedMaterial, projected: true);
				num2 = m_ArgsArray.Count;
				m_ArgsArray.Add(m_BoxMesh.GetIndexCount(0));
				m_ArgsArray.Add((uint)m_ProjectedInstanceCount);
				m_ArgsArray.Add(m_BoxMesh.GetIndexStart(0));
				m_ArgsArray.Add(m_BoxMesh.GetBaseVertex(0));
				m_ArgsArray.Add(0u);
			}
			if (m_AbsoluteInstanceCount != 0)
			{
				GetMesh(ref m_QuadMesh, box: false);
				GetCurveMaterial(ref m_AbsoluteMaterial, projected: false);
				num3 = m_ArgsArray.Count;
				m_ArgsArray.Add(m_QuadMesh.GetIndexCount(0));
				m_ArgsArray.Add((uint)m_AbsoluteInstanceCount);
				m_ArgsArray.Add(m_QuadMesh.GetIndexStart(0));
				m_ArgsArray.Add(m_QuadMesh.GetBaseVertex(0));
				m_ArgsArray.Add(0u);
			}
			foreach (Camera camera in cameras)
			{
				if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
				{
					if (m_ProjectedInstanceCount != 0)
					{
						Graphics.DrawMeshInstancedIndirect(m_BoxMesh, 0, m_ProjectedMaterial, val, m_ArgsBuffer, num2 * 4, (MaterialPropertyBlock)null, (ShadowCastingMode)0, false, 0, camera, (LightProbeUsage)1);
					}
					if (m_AbsoluteInstanceCount != 0)
					{
						Graphics.DrawMeshInstancedIndirect(m_QuadMesh, 0, m_AbsoluteMaterial, val, m_ArgsBuffer, num3 * 4, (MaterialPropertyBlock)null, (ShadowCastingMode)0, false, 0, camera, (LightProbeUsage)1);
					}
				}
			}
			m_ArgsBuffer.SetData<uint>(m_ArgsArray, 0, 0, m_ArgsArray.Count);
		}
		finally
		{
		}
	}

	private void GetMesh(ref Mesh mesh, bool box)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mesh == (Object)null)
		{
			mesh = new Mesh();
			((Object)mesh).name = "Overlay";
			if (box)
			{
				mesh.vertices = (Vector3[])(object)new Vector3[8]
				{
					new Vector3(-1f, 0f, -1f),
					new Vector3(-1f, 0f, 1f),
					new Vector3(1f, 0f, 1f),
					new Vector3(1f, 0f, -1f),
					new Vector3(-1f, 1f, -1f),
					new Vector3(-1f, 1f, 1f),
					new Vector3(1f, 1f, 1f),
					new Vector3(1f, 1f, -1f)
				};
				mesh.triangles = new int[36]
				{
					0, 1, 5, 5, 4, 0, 3, 7, 6, 6,
					2, 3, 0, 3, 2, 2, 1, 0, 4, 5,
					6, 6, 7, 4, 0, 4, 7, 7, 3, 0,
					1, 2, 6, 6, 5, 1
				};
			}
			else
			{
				mesh.vertices = (Vector3[])(object)new Vector3[4]
				{
					new Vector3(-1f, 0f, -1f),
					new Vector3(-1f, 0f, 1f),
					new Vector3(1f, 0f, 1f),
					new Vector3(1f, 0f, -1f)
				};
				mesh.triangles = new int[12]
				{
					0, 3, 2, 2, 1, 0, 0, 1, 2, 2,
					3, 0
				};
			}
		}
	}

	private void GetCurveMaterial(ref Material material, bool projected)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		if ((Object)(object)material == (Object)null)
		{
			OverlayConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<OverlayConfigurationPrefab>(m_SettingsQuery);
			material = new Material(singletonPrefab.m_CurveMaterial);
			((Object)material).name = "Overlay curves";
			if (projected)
			{
				material.EnableKeyword("PROJECTED_MODE");
			}
		}
	}

	private void GetCurveBuffer(ref ComputeBuffer buffer, int count)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		if (buffer != null && buffer.count < count)
		{
			count = math.max(buffer.count * 2, count);
			buffer.Release();
			buffer = null;
		}
		if (buffer == null)
		{
			buffer = new ComputeBuffer(math.max(64, count), System.Runtime.CompilerServices.Unsafe.SizeOf<CurveData>());
			buffer.name = "Overlay curve buffer";
		}
	}

	[Preserve]
	public OverlayRenderSystem()
	{
	}
}
