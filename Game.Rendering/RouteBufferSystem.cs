using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class RouteBufferSystem : GameSystemBase, IPreDeserialize
{
	private class ManagedData : IDisposable
	{
		public Material m_Material;

		public ComputeBuffer m_SegmentBuffer;

		public Vector4 m_Size;

		public int m_OriginalRenderQueue;

		public bool m_Updated;

		public void Initialize(RoutePrefab routePrefab)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_Material != (Object)null)
			{
				Object.Destroy((Object)(object)m_Material);
			}
			m_Material = new Material(routePrefab.m_Material);
			((Object)m_Material).name = "Routes (" + ((Object)routePrefab).name + ")";
			m_OriginalRenderQueue = m_Material.renderQueue;
			m_Size = new Vector4(routePrefab.m_Width, routePrefab.m_Width * 0.25f, routePrefab.m_SegmentLength, 0f);
		}

		public void Dispose()
		{
			if ((Object)(object)m_Material != (Object)null)
			{
				Object.Destroy((Object)(object)m_Material);
			}
			if (m_SegmentBuffer != null)
			{
				m_SegmentBuffer.Release();
			}
		}
	}

	private struct NativeData : IDisposable
	{
		public UnsafeList<SegmentData> m_SegmentData;

		public Bounds3 m_Bounds;

		public Entity m_Entity;

		public float m_Length;

		public bool m_Updated;

		public void Initialize(Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
		}

		public void Dispose()
		{
			if (m_SegmentData.IsCreated)
			{
				m_SegmentData.Dispose();
			}
		}
	}

	private struct SegmentData
	{
		public float4x4 m_Curve;

		public float3 m_Position;

		public float3 m_SizeFactor;

		public float2 m_Opacity;

		public float2 m_DividedOpacity;

		public float m_Broken;
	}

	private struct CurveKey : IEquatable<CurveKey>
	{
		public Segment m_Line;

		public Entity m_Entity;

		public float2 m_Range;

		public bool Equals(CurveKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (m_Entity != Entity.Null)
			{
				if (((Entity)(ref m_Entity)).Equals(other.m_Entity))
				{
					return ((float2)(ref m_Range)).Equals(other.m_Range);
				}
				return false;
			}
			if (((float3)(ref m_Line.a)).Equals(other.m_Line.a))
			{
				return ((float3)(ref m_Line.b)).Equals(other.m_Line.b);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (m_Entity != Entity.Null)
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_Range)/*cast due to .constrained prefix*/).GetHashCode();
			}
			return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Line.a)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Line.b)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct CurveValue
	{
		public int m_SegmentDataIndex;

		public int m_SharedCount;
	}

	private struct SourceKey : IEquatable<SourceKey>
	{
		public Entity m_Entity;

		public bool m_Forward;

		public bool Equals(SourceKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Entity)).Equals(other.m_Entity))
			{
				return m_Forward == other.m_Forward;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode() ^ m_Forward.GetHashCode();
		}
	}

	[BurstCompile]
	private struct UpdateBufferJob : IJobParallelFor
	{
		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<PathSource> m_PathSourceData;

		[ReadOnly]
		public ComponentLookup<LivePath> m_LivePathData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegments;

		[ReadOnly]
		public BufferLookup<CurveElement> m_CurveElements;

		[ReadOnly]
		public BufferLookup<CurveSource> m_CurveSources;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[NativeDisableParallelForRestriction]
		public NativeList<NativeData> m_NativeData;

		public void Execute(int index)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06af: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0784: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_088c: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0933: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_0968: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Unknown result type (might be due to invalid IL or missing references)
			//IL_097b: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			ref NativeData reference = ref m_NativeData.ElementAt(index);
			if (!reference.m_Updated)
			{
				return;
			}
			reference.m_Updated = false;
			reference.m_SegmentData.Clear();
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[reference.m_Entity];
			DynamicBuffer<RouteSegment> val2 = m_RouteSegments[reference.m_Entity];
			NativeHashMap<CurveKey, CurveValue> curveMap = default(NativeHashMap<CurveKey, CurveValue>);
			NativeParallelMultiHashMap<SourceKey, float> val3 = default(NativeParallelMultiHashMap<SourceKey, float>);
			NativeList<int> val4 = default(NativeList<int>);
			NativeList<float> val5 = default(NativeList<float>);
			if (m_LivePathData.HasComponent(reference.m_Entity))
			{
				curveMap._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
				val3._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
				val4._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
				val5._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < val2.Length; i++)
				{
					Entity segment = val2[i].m_Segment;
					if (segment == Entity.Null)
					{
						continue;
					}
					DynamicBuffer<CurveSource> val6 = m_CurveSources[segment];
					for (int j = 0; j < val6.Length; j++)
					{
						CurveSource curveSource = val6[j];
						if (curveSource.m_Range.x != curveSource.m_Range.y)
						{
							if (curveSource.m_Range.x != 0f && curveSource.m_Range.x != 1f)
							{
								val3.Add(new SourceKey
								{
									m_Entity = curveSource.m_Entity,
									m_Forward = (curveSource.m_Range.y > curveSource.m_Range.x)
								}, curveSource.m_Range.x);
							}
							if (curveSource.m_Range.y != 0f && curveSource.m_Range.y != 1f)
							{
								val3.Add(new SourceKey
								{
									m_Entity = curveSource.m_Entity,
									m_Forward = (curveSource.m_Range.y > curveSource.m_Range.x)
								}, curveSource.m_Range.y);
							}
						}
					}
				}
			}
			float num = 0f;
			Bounds3 val7 = default(Bounds3);
			((Bounds3)(ref val7))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			DynamicBuffer<PathElement> val8 = default(DynamicBuffer<PathElement>);
			PathSource pathSource = default(PathSource);
			Transform transform = default(Transform);
			DynamicBuffer<TransformFrame> val16 = default(DynamicBuffer<TransformFrame>);
			float2 val20 = default(float2);
			float2 val24 = default(float2);
			float2 val25 = default(float2);
			float num5 = default(float);
			NativeParallelMultiHashMapIterator<SourceKey> val27 = default(NativeParallelMultiHashMapIterator<SourceKey>);
			Curve curve = default(Curve);
			float2 val29 = default(float2);
			float2 val30 = default(float2);
			for (int k = 0; k < val2.Length; k++)
			{
				Entity segment2 = val2[k].m_Segment;
				if (segment2 == Entity.Null)
				{
					continue;
				}
				float broken = 0f;
				if (m_PathElements.TryGetBuffer(segment2, ref val8))
				{
					broken = math.select(0f, 1f, val8.Length == 0);
				}
				DynamicBuffer<CurveElement> val9 = m_CurveElements[segment2];
				DynamicBuffer<CurveSource> val10 = default(DynamicBuffer<CurveSource>);
				float3 val11 = default(float3);
				float3 val12 = default(float3);
				float3 val13 = default(float3);
				float3 val14 = default(float3);
				if (val9.Length > 0)
				{
					val13 = math.normalizesafe(MathUtils.StartTangent(val9[0].m_Curve), default(float3));
					val14 = val9[0].m_Curve.a;
					if (m_PathSourceData.TryGetComponent(segment2, ref pathSource))
					{
						val10 = m_CurveSources[segment2];
						num = 0f;
						if (m_TransformData.TryGetComponent(pathSource.m_Entity, ref transform) && !m_UnspawnedData.HasComponent(pathSource.m_Entity))
						{
							EntityStorageInfo val15 = ((EntityStorageInfoLookup)(ref m_EntityLookup))[pathSource.m_Entity];
							bool flag = false;
							if (((ArchetypeChunk)(ref val15.Chunk)).Has<UpdateFrame>(m_UpdateFrameType) && m_TransformFrames.TryGetBuffer(pathSource.m_Entity, ref val16))
							{
								UpdateFrame sharedComponent = ((ArchetypeChunk)(ref val15.Chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType);
								ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_FrameTime, sharedComponent.m_Index, out var updateFrame, out var updateFrame2, out var framePosition);
								TransformFrame frame = val16[(int)updateFrame];
								TransformFrame frame2 = val16[(int)updateFrame2];
								transform = ObjectInterpolateSystem.CalculateTransform(frame, frame2, framePosition).ToTransform();
							}
							else if (m_CurrentVehicleData.HasComponent(pathSource.m_Entity))
							{
								flag = true;
							}
							if (!flag)
							{
								float3 a = val9[0].m_Curve.a;
								float3 val17 = a - transform.m_Position;
								val17 = MathUtils.Normalize(val17, ((float3)(ref val17)).xz);
								Bezier4x3 val18;
								if (((float3)(ref val13)).Equals(default(float3)))
								{
									val18 = NetUtils.StraightCurve(transform.m_Position, a);
									float num2 = (val18.a.y - val18.b.y) / math.max(1f, math.abs(val17.y));
									val18.b.y += num2;
									val18.c.y += num2;
									val13 = math.normalizesafe(MathUtils.EndTangent(val18), default(float3));
								}
								else
								{
									float3 val19 = MathUtils.Normalize(val13, ((float3)(ref val13)).xz);
									float3 startTangent = val17 * (math.dot(val19, val17) * 2f) - val19;
									val18 = NetUtils.FitCurve(transform.m_Position, startTangent, val19, a);
								}
								float num3 = MathUtils.Length(val18);
								((float2)(ref val20))._002Ector(num, num + num3);
								int num4 = AddSegment(ref reference.m_SegmentData, curveMap, val18, Entity.Null, default(float2), val20, new float2(0f, 1f), broken);
								num = val20.y;
								val7 |= MathUtils.Bounds(val18);
								val11 = val13;
								val12 = val14;
								if (val4.IsCreated)
								{
									val4.Add(ref num4);
								}
							}
						}
					}
				}
				if (!((float3)(ref val13)).Equals(default(float3)))
				{
					for (int l = 0; l < val9.Length; l++)
					{
						CurveElement curveElement = val9[l];
						float3 val21 = val13;
						float3 val22 = val14;
						float3 val23 = math.normalizesafe(MathUtils.EndTangent(curveElement.m_Curve), default(float3));
						float3 d = curveElement.m_Curve.d;
						if (l + 1 < val9.Length)
						{
							val13 = math.normalizesafe(MathUtils.StartTangent(val9[l + 1].m_Curve), default(float3));
							val14 = val9[l + 1].m_Curve.a;
						}
						else
						{
							val13 = default(float3);
							val14 = default(float3);
						}
						((float2)(ref val24))._002Ector(math.dot(val21, val11), math.dot(val23, val13));
						((float2)(ref val25))._002Ector(math.distancesq(val22, val12), math.distancesq(d, val14));
						float2 val26 = math.select(float2.op_Implicit(1f), float2.op_Implicit(0f), (val24 < 0.99f) | (val25 > 0.01f));
						val7 |= MathUtils.Bounds(curveElement.m_Curve);
						val11 = val23;
						val12 = d;
						CurveSource curveSource2 = default(CurveSource);
						if (val10.IsCreated && val3.IsCreated)
						{
							curveSource2 = val10[l];
							if (curveSource2.m_Range.x != curveSource2.m_Range.y)
							{
								bool flag2 = curveSource2.m_Range.y > curveSource2.m_Range.x;
								if (val3.TryGetFirstValue(new SourceKey
								{
									m_Entity = curveSource2.m_Entity,
									m_Forward = flag2
								}, ref num5, ref val27))
								{
									do
									{
										val5.Add(ref num5);
									}
									while (val3.TryGetNextValue(ref num5, ref val27));
									if (val5.Length >= 2)
									{
										NativeSortExtension.Sort<float>(val5);
									}
									if (!flag2)
									{
										curveElement.m_Curve = MathUtils.Invert(curveElement.m_Curve);
									}
									if (((curveSource2.m_Range.x != 0f && curveSource2.m_Range.x != 1f) || (curveSource2.m_Range.y != 0f && curveSource2.m_Range.y != 1f)) && m_CurveData.TryGetComponent(curveSource2.m_Entity, ref curve))
									{
										curveElement.m_Curve = curve.m_Bezier;
									}
									float2 range = curveSource2.m_Range;
									for (int m = 0; m <= val5.Length; m++)
									{
										if (flag2)
										{
											if (m < val5.Length)
											{
												range.y = val5[m];
												if (range.y <= range.x || range.y >= curveSource2.m_Range.y)
												{
													continue;
												}
											}
											else
											{
												range.y = curveSource2.m_Range.y;
											}
										}
										else if (m < val5.Length)
										{
											range.y = val5[val5.Length - m - 1];
											if (range.y >= range.x || range.y <= curveSource2.m_Range.y)
											{
												continue;
											}
										}
										else
										{
											range.y = curveSource2.m_Range.y;
										}
										Bezier4x3 val28 = MathUtils.Cut(curveElement.m_Curve, range);
										float num6 = MathUtils.Length(val28);
										((float2)(ref val29))._002Ector(num, num + num6);
										float2 opacity = math.select(val26, float2.op_Implicit(1f), range != curveSource2.m_Range);
										int num7 = AddSegment(ref reference.m_SegmentData, curveMap, val28, curveSource2.m_Entity, range, val29, opacity, broken);
										num = val29.y;
										range.x = range.y;
										val4.Add(ref num7);
									}
									val5.Clear();
									continue;
								}
							}
						}
						float num8 = MathUtils.Length(curveElement.m_Curve);
						((float2)(ref val30))._002Ector(num, num + num8);
						int num9 = AddSegment(ref reference.m_SegmentData, curveMap, curveElement.m_Curve, curveSource2.m_Entity, curveSource2.m_Range, val30, val26, broken);
						num = val30.y;
						if (val4.IsCreated)
						{
							val4.Add(ref num9);
						}
					}
				}
				if (val4.IsCreated)
				{
					int num10 = -1;
					val4.Add(ref num10);
				}
			}
			if (val4.IsCreated && val4.Length > 0)
			{
				bool flag3 = true;
				int num11 = 0;
				while (flag3 && num11 < 10)
				{
					int num12 = val4[0];
					flag3 = false;
					if (num11 == 0)
					{
						for (int n = 1; n < val4.Length; n++)
						{
							int num13 = val4[n];
							if (num12 != -1 && num13 != -1)
							{
								ref SegmentData reference2 = ref reference.m_SegmentData.ElementAt(num12);
								ref SegmentData reference3 = ref reference.m_SegmentData.ElementAt(num13);
								float num14 = math.max(reference2.m_SizeFactor.z, reference3.m_SizeFactor.x);
								flag3 |= num14 != reference2.m_SizeFactor.z || num14 != reference3.m_SizeFactor.x;
								reference2.m_SizeFactor.z = num14;
								reference3.m_SizeFactor.x = num14;
								float num15 = math.select(reference2.m_Opacity.y / reference3.m_Opacity.x, 1f, reference3.m_Opacity.x < 0.5f || reference2.m_Opacity.y > reference3.m_Opacity.x);
								float num16 = math.select(reference3.m_Opacity.x / reference2.m_Opacity.y, 1f, reference2.m_Opacity.y < 0.5f || reference3.m_Opacity.x > reference2.m_Opacity.y);
								reference2.m_DividedOpacity.y = math.min(reference2.m_DividedOpacity.y, num15);
								reference3.m_DividedOpacity.x = math.min(reference3.m_DividedOpacity.x, num16);
							}
							num12 = num13;
						}
						for (int num17 = 0; num17 < reference.m_SegmentData.Length; num17++)
						{
							ref SegmentData reference4 = ref reference.m_SegmentData.ElementAt(num17);
							reference4.m_Opacity = math.saturate(reference4.m_Opacity);
						}
					}
					else
					{
						for (int num18 = 1; num18 < val4.Length; num18++)
						{
							int num19 = val4[num18];
							if (num12 != -1 && num19 != -1)
							{
								ref SegmentData reference5 = ref reference.m_SegmentData.ElementAt(num12);
								ref SegmentData reference6 = ref reference.m_SegmentData.ElementAt(num19);
								float num20 = math.max(reference5.m_SizeFactor.z, reference6.m_SizeFactor.x);
								flag3 |= num20 != reference5.m_SizeFactor.z || num20 != reference6.m_SizeFactor.x;
								reference5.m_SizeFactor.z = num20;
								reference6.m_SizeFactor.x = num20;
							}
							num12 = num19;
						}
					}
					num11++;
				}
				int num21 = 0;
				for (int num22 = 0; num22 < val4.Length; num22++)
				{
					int num23 = val4[num22];
					if (num23 == -1)
					{
						float num24 = 0f;
						for (int num25 = num22 - 1; num25 >= num21; num25--)
						{
							num23 = val4[num25];
							ref SegmentData reference7 = ref reference.m_SegmentData.ElementAt(num23);
							float num26 = math.min(0f, num24 - reference7.m_Curve.c3.w);
							num24 -= reference7.m_Curve.c3.w - reference7.m_Curve.c0.w;
							reference7.m_Curve.c0.w += num26;
							reference7.m_Curve.c1.w += num26;
							reference7.m_Curve.c2.w += num26;
							reference7.m_Curve.c3.w += num26;
						}
						num21 = num22 + 1;
					}
				}
			}
			for (int num27 = 0; num27 < val.Length; num27++)
			{
				Entity waypoint = val[num27].m_Waypoint;
				float3 position = m_PositionData[waypoint].m_Position;
				AddNode(ref reference.m_SegmentData, position, 1f);
				val7 |= position;
			}
			if (curveMap.IsCreated)
			{
				curveMap.Dispose();
			}
			if (val3.IsCreated)
			{
				val3.Dispose();
			}
			if (val4.IsCreated)
			{
				val4.Dispose();
			}
			if (val5.IsCreated)
			{
				val5.Dispose();
			}
			reference.m_Bounds = val7;
			reference.m_Length = num;
		}

		private int AddSegment(ref UnsafeList<SegmentData> segments, NativeHashMap<CurveKey, CurveValue> curveMap, Bezier4x3 curve, Entity sourceEntity, float2 sourceRange, float2 offset, float2 opacity, float broken)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			float3 val = math.lerp(curve.a, curve.d, 0.5f);
			SegmentData segmentData = default(SegmentData);
			segmentData.m_Curve = new float4x4
			{
				c0 = new float4(curve.a - val, offset.x),
				c1 = new float4(curve.b - val, offset.x + math.distance(curve.a, curve.b)),
				c2 = new float4(curve.c - val, offset.y - math.distance(curve.c, curve.d)),
				c3 = new float4(curve.d - val, offset.y)
			};
			segmentData.m_Position = val;
			segmentData.m_SizeFactor = new float3(1f, 1f, 1f);
			segmentData.m_Opacity = opacity;
			segmentData.m_DividedOpacity = new float2(1f, 1f);
			segmentData.m_Broken = broken;
			int result = -1;
			if (curveMap.IsCreated)
			{
				CurveKey curveKey = new CurveKey
				{
					m_Line = new Segment(curve.a, curve.d),
					m_Entity = sourceEntity,
					m_Range = sourceRange
				};
				CurveValue curveValue = default(CurveValue);
				if (curveMap.TryGetValue(curveKey, ref curveValue))
				{
					ref SegmentData reference = ref segments.ElementAt(curveValue.m_SegmentDataIndex);
					curveValue.m_SharedCount++;
					reference.m_SizeFactor = float3.op_Implicit(GetSizeFactor(curveValue.m_SharedCount));
					ref float2 reference2 = ref reference.m_Opacity;
					reference2 += opacity;
					curveMap[curveKey] = curveValue;
					return curveValue.m_SegmentDataIndex;
				}
				curveValue.m_SharedCount = 1;
				curveValue.m_SegmentDataIndex = segments.Length;
				curveMap.Add(curveKey, curveValue);
				result = curveValue.m_SegmentDataIndex;
			}
			segments.Add(ref segmentData);
			return result;
		}

		private float GetSizeFactor(int sharedCount)
		{
			return 4f - 21f / (6f + (float)sharedCount);
		}

		private void AddNode(ref UnsafeList<SegmentData> segments, float3 position, float opacity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			SegmentData segmentData = default(SegmentData);
			segmentData.m_Curve = new float4x4
			{
				c0 = new float4(0f, 0f, -1f, -1000000f),
				c1 = new float4(0f, 0f, -1f / 3f, -1000000f),
				c2 = new float4(0f, 0f, 1f / 3f, -1000000f),
				c3 = new float4(0f, 0f, 1f, -1000000f)
			};
			segmentData.m_Position = position;
			segmentData.m_SizeFactor = new float3(1f, 1f, 1f);
			segmentData.m_Opacity = new float2(opacity, opacity);
			segmentData.m_DividedOpacity = new float2(1f, 1f);
			segmentData.m_Broken = 0f;
			segments.Add(ref segmentData);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Applied> __Game_Common_Applied_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathUpdated> __Game_Pathfind_PathUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<RouteBufferIndex> __Game_Rendering_RouteBufferIndex_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathSource> __Game_Routes_PathSource_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LivePath> __Game_Routes_LivePath_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CurveElement> __Game_Routes_CurveElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CurveSource> __Game_Routes_CurveSource_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

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
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Applied_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Applied>(true);
			__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathUpdated>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_RouteBufferIndex_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteBufferIndex>(false);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Segment>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_PathSource_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathSource>(true);
			__Game_Routes_LivePath_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LivePath>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Routes_CurveElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CurveElement>(true);
			__Game_Routes_CurveSource_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CurveSource>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_UpdatedRoutesQuery;

	private EntityQuery m_AllRoutesQuery;

	private EntityQuery m_RouteConfigQuery;

	private List<ManagedData> m_ManagedData;

	private NativeList<NativeData> m_NativeData;

	private Stack<int> m_FreeBufferIndices;

	private JobHandle m_BufferDependencies;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Route>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<PathUpdated>()
		};
		array[1] = val;
		m_UpdatedRoutesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllRoutesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Route>() });
		m_RouteConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RouteConfigurationData>() });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		Clear();
		if (m_NativeData.IsCreated)
		{
			m_NativeData.Dispose();
		}
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		Clear();
		m_Loaded = true;
	}

	private void Clear()
	{
		if (m_ManagedData != null)
		{
			for (int i = 0; i < m_ManagedData.Count; i++)
			{
				m_ManagedData[i].Dispose();
			}
			m_ManagedData.Clear();
		}
		if (m_FreeBufferIndices != null)
		{
			m_FreeBufferIndices.Clear();
		}
		if (m_NativeData.IsCreated)
		{
			((JobHandle)(ref m_BufferDependencies)).Complete();
			for (int j = 0; j < m_NativeData.Length; j++)
			{
				m_NativeData.ElementAt(j).Dispose();
			}
			m_NativeData.Clear();
		}
	}

	public unsafe void GetBuffer(int index, out Material material, out ComputeBuffer segmentBuffer, out int originalRenderQueue, out Bounds bounds, out Vector4 size)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		material = null;
		segmentBuffer = null;
		originalRenderQueue = 0;
		bounds = default(Bounds);
		size = default(Vector4);
		if (m_ManagedData == null || index < 0 || index >= m_ManagedData.Count)
		{
			return;
		}
		((JobHandle)(ref m_BufferDependencies)).Complete();
		ManagedData managedData = m_ManagedData[index];
		ref NativeData reference = ref m_NativeData.ElementAt(index);
		if (managedData.m_Updated)
		{
			managedData.m_Updated = false;
			if (managedData.m_SegmentBuffer != null && managedData.m_SegmentBuffer.count != reference.m_SegmentData.Length)
			{
				managedData.m_SegmentBuffer.Release();
				managedData.m_SegmentBuffer = null;
			}
			if (reference.m_SegmentData.Length > 0)
			{
				if (managedData.m_SegmentBuffer == null)
				{
					managedData.m_SegmentBuffer = new ComputeBuffer(reference.m_SegmentData.Length, System.Runtime.CompilerServices.Unsafe.SizeOf<SegmentData>());
					managedData.m_SegmentBuffer.name = "Route segment buffer (" + ((Object)managedData.m_Material).name + ")";
				}
				NativeArray<SegmentData> data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SegmentData>((void*)reference.m_SegmentData.Ptr, reference.m_SegmentData.Length, (Allocator)1);
				managedData.m_SegmentBuffer.SetData<SegmentData>(data);
			}
			reference.m_SegmentData.Dispose();
		}
		material = managedData.m_Material;
		segmentBuffer = managedData.m_SegmentBuffer;
		originalRenderQueue = managedData.m_OriginalRenderQueue;
		bounds = RenderingUtils.ToBounds(reference.m_Bounds);
		size = managedData.m_Size;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllRoutesQuery : m_UpdatedRoutesQuery);
		if (((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			return;
		}
		RoutePrefab routePrefab = null;
		HashSet<Entity> hashSet = null;
		((JobHandle)(ref m_BufferDependencies)).Complete();
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Created> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Applied> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Applied>(ref __TypeHandle.__Game_Common_Applied_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PathUpdated> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<PathUpdated>(ref __TypeHandle.__Game_Pathfind_PathUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<RouteBufferIndex> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<RouteBufferIndex>(ref __TypeHandle.__Game_Rendering_RouteBufferIndex_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Game.Routes.Segment> componentLookup = InternalCompilerInterface.GetComponentLookup<Game.Routes.Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Owner> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Deleted> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				NativeArray<PathUpdated> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<PathUpdated>(ref componentTypeHandle4);
				if (nativeArray.Length != 0)
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						PathUpdated pathUpdated = nativeArray[j];
						if (componentLookup.HasComponent(pathUpdated.m_Owner) && componentLookup2.HasComponent(pathUpdated.m_Owner) && !componentLookup3.HasComponent(pathUpdated.m_Owner))
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<Entity>();
							}
							hashSet.Add(componentLookup2[pathUpdated.m_Owner].m_Owner);
						}
					}
				}
				else if (((ArchetypeChunk)(ref val3)).Has<Deleted>(ref componentTypeHandle))
				{
					NativeArray<RouteBufferIndex> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<RouteBufferIndex>(ref componentTypeHandle6);
					if (m_FreeBufferIndices == null)
					{
						m_FreeBufferIndices = new Stack<int>(nativeArray2.Length);
					}
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						RouteBufferIndex routeBufferIndex = nativeArray2[k];
						ManagedData managedData = m_ManagedData[routeBufferIndex.m_Index];
						ref NativeData reference = ref m_NativeData.ElementAt(routeBufferIndex.m_Index);
						managedData.m_Updated = false;
						reference.m_Updated = false;
						m_FreeBufferIndices.Push(routeBufferIndex.m_Index);
						routeBufferIndex.m_Index = -1;
						nativeArray2[k] = routeBufferIndex;
					}
				}
				else if (loaded || (((ArchetypeChunk)(ref val3)).Has<Created>(ref componentTypeHandle2) && !((ArchetypeChunk)(ref val3)).Has<Applied>(ref componentTypeHandle3)))
				{
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					NativeArray<RouteBufferIndex> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<RouteBufferIndex>(ref componentTypeHandle6);
					NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref componentTypeHandle5);
					if (m_ManagedData == null)
					{
						m_ManagedData = new List<ManagedData>(nativeArray4.Length);
					}
					if (!m_NativeData.IsCreated)
					{
						m_NativeData = new NativeList<NativeData>(nativeArray4.Length, AllocatorHandle.op_Implicit((Allocator)4));
					}
					if (hashSet == null)
					{
						hashSet = new HashSet<Entity>();
					}
					for (int l = 0; l < nativeArray4.Length; l++)
					{
						Entity val4 = nativeArray3[l];
						RouteBufferIndex routeBufferIndex2 = nativeArray4[l];
						PrefabRef refData = nativeArray5[l];
						if (!m_PrefabSystem.TryGetPrefab<RoutePrefab>(refData, out var prefab))
						{
							RouteConfigurationData singleton = ((EntityQuery)(ref m_RouteConfigQuery)).GetSingleton<RouteConfigurationData>();
							if ((Object)(object)routePrefab != (Object)null)
							{
								prefab = routePrefab;
							}
							else if (m_PrefabSystem.TryGetPrefab<RoutePrefab>(singleton.m_MissingRoutePrefab, out prefab))
							{
								routePrefab = prefab;
							}
						}
						if (m_FreeBufferIndices != null && m_FreeBufferIndices.Count > 0)
						{
							routeBufferIndex2.m_Index = m_FreeBufferIndices.Pop();
							ManagedData managedData2 = m_ManagedData[routeBufferIndex2.m_Index];
							ref NativeData reference2 = ref m_NativeData.ElementAt(routeBufferIndex2.m_Index);
							managedData2.Initialize(prefab);
							reference2.Initialize(val4);
						}
						else
						{
							routeBufferIndex2.m_Index = m_ManagedData.Count;
							ManagedData managedData3 = new ManagedData();
							NativeData nativeData = default(NativeData);
							managedData3.Initialize(prefab);
							nativeData.Initialize(val4);
							m_ManagedData.Add(managedData3);
							m_NativeData.Add(ref nativeData);
						}
						nativeArray4[l] = routeBufferIndex2;
						hashSet.Add(val4);
					}
				}
				else
				{
					NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					if (hashSet == null)
					{
						hashSet = new HashSet<Entity>();
					}
					for (int m = 0; m < nativeArray6.Length; m++)
					{
						hashSet.Add(nativeArray6[m]);
					}
				}
			}
		}
		finally
		{
			val2.Dispose();
		}
		if (hashSet == null)
		{
			return;
		}
		foreach (Entity item in hashSet)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			RouteBufferIndex componentData = ((EntityManager)(ref entityManager)).GetComponentData<RouteBufferIndex>(item);
			if (componentData.m_Index >= 0)
			{
				ManagedData managedData4 = m_ManagedData[componentData.m_Index];
				ref NativeData reference3 = ref m_NativeData.ElementAt(componentData.m_Index);
				managedData4.m_Updated = true;
				reference3.m_Updated = true;
				if (!reference3.m_SegmentData.IsCreated)
				{
					reference3.m_SegmentData = new UnsafeList<SegmentData>(0, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)0);
				}
			}
		}
		UpdateBufferJob updateBufferJob = new UpdateBufferJob
		{
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathSourceData = InternalCompilerInterface.GetComponentLookup<PathSource>(ref __TypeHandle.__Game_Routes_PathSource_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LivePathData = InternalCompilerInterface.GetComponentLookup<LivePath>(ref __TypeHandle.__Game_Routes_LivePath_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveElements = InternalCompilerInterface.GetBufferLookup<CurveElement>(ref __TypeHandle.__Game_Routes_CurveElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveSources = InternalCompilerInterface.GetBufferLookup<CurveSource>(ref __TypeHandle.__Game_Routes_CurveSource_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_NativeData = m_NativeData
		};
		m_BufferDependencies = IJobParallelForExtensions.Schedule<UpdateBufferJob>(updateBufferJob, m_NativeData.Length, 1, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = m_BufferDependencies;
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
	public RouteBufferSystem()
	{
	}
}
