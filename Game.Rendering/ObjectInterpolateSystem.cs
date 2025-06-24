using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Effects;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class ObjectInterpolateSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateTransformDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<PointOfInterest> m_PointOfInterestData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<TrafficLight> m_TrafficLightData;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencyData;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_BuildingElectricityConsumer;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ExtractorFacility> m_BuildingExtractorFacility;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<CarTrailer> m_CarTrailerData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SwayingData> m_PrefabSwayingData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<EnabledEffect> m_EffectInstances;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<AnimationMotion> m_AnimationMotions;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public BufferLookup<ProceduralLight> m_ProceduralLights;

		[ReadOnly]
		public BufferLookup<LightAnimation> m_LightAnimations;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Swaying> m_SwayingData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Skeleton> m_Skeletons;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Emissive> m_Emissives;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Animated> m_Animateds;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Bone> m_Bones;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Momentum> m_Momentums;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LightState> m_Lights;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public uint m_PrevFrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public float m_FrameDelta;

		[ReadOnly]
		public float m_TimeOfDay;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public CellMapData<Wind> m_WindData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public WaterSurfaceData m_WaterVelocityData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public AnimatedSystem.AnimationData m_AnimationData;

		public void Execute(int index)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData cullingData = m_CullingData[index];
			if ((cullingData.m_Flags & (PreCullingFlags.InterpolatedTransform | PreCullingFlags.Animated | PreCullingFlags.Skeleton | PreCullingFlags.Emissive)) != 0 && (cullingData.m_Flags & (PreCullingFlags.NearCamera | PreCullingFlags.Temp | PreCullingFlags.Relative)) == PreCullingFlags.NearCamera)
			{
				Random random = m_RandomSeed.GetRandom(index);
				DynamicBuffer<TransformFrame> transformFrames = default(DynamicBuffer<TransformFrame>);
				if (m_TransformFrames.TryGetBuffer(cullingData.m_Entity, ref transformFrames))
				{
					UpdateInterpolatedTransforms(cullingData, transformFrames, ref random);
				}
				else
				{
					UpdateStaticAnimations(cullingData, ref random);
				}
			}
		}

		private void UpdateStaticAnimations(PreCullingData cullingData, ref Random random)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			float num = m_FrameDelta / 60f;
			float speedDeltaFactor = math.select(60f / m_FrameDelta, 0f, m_FrameDelta == 0f);
			bool flag = (cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0;
			Transform transform = m_TransformData[cullingData.m_Entity];
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			if ((cullingData.m_Flags & PreCullingFlags.InterpolatedTransform) != 0)
			{
				ref InterpolatedTransform valueRW = ref m_InterpolatedTransformData.GetRefRW(cullingData.m_Entity).ValueRW;
				Destroyed destroyed = default(Destroyed);
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				if (m_DestroyedData.TryGetComponent(cullingData.m_Entity, ref destroyed) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					Random random2 = m_PseudoRandomSeedData[cullingData.m_Entity].GetRandom(PseudoRandomSeed.kCollapse);
					quaternion val = ((Random)(ref random2)).NextQuaternionRotation();
					float collapseTime = BuildingUtils.GetCollapseTime(objectGeometryData.m_Size.y);
					float num2 = ((!flag) ? BuildingUtils.GetCollapseTime(transform.m_Position.y - valueRW.m_Position.y) : (collapseTime + destroyed.m_Cleared));
					num2 = math.max(0f, num2 + num);
					valueRW.m_Position = transform.m_Position;
					valueRW.m_Position.y -= BuildingUtils.GetCollapseHeight(num2);
					valueRW.m_Rotation = math.slerp(transform.m_Rotation, val, num2 / (10f + collapseTime * 10f));
				}
				else if (m_SwayingData.HasComponent(cullingData.m_Entity) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					ref Swaying valueRW2 = ref m_SwayingData.GetRefRW(cullingData.m_Entity).ValueRW;
					InterpolatedTransform oldTransform = valueRW;
					valueRW.m_Position = transform.m_Position;
					valueRW.m_Rotation = transform.m_Rotation;
					float num3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, transform.m_Position);
					float2 val2 = WaterUtils.SampleVelocity(ref m_WaterVelocityData, transform.m_Position);
					ref float3 lastVelocity = ref valueRW2.m_LastVelocity;
					((float3)(ref lastVelocity)).xz = ((float3)(ref lastVelocity)).xz - val2 * num;
					float num4 = ((float)(m_FrameIndex & 0xFFFF) + m_FrameTime) * (1f / 60f);
					num3 += noise.pnoise(new float3(num4, transform.m_Position.z, transform.m_Position.x), float3.op_Implicit(1092.2667f)) * 0.5f;
					val2.x += noise.pnoise(new float3(transform.m_Position.x, num4, transform.m_Position.z), float3.op_Implicit(1092.2667f));
					val2.y += noise.pnoise(new float3(transform.m_Position.z, transform.m_Position.x, num4), float3.op_Implicit(1092.2667f));
					if (flag)
					{
						valueRW.m_Position.y = num3;
						valueRW2.m_LastVelocity = new float3(val2.x, 0f, val2.y);
						valueRW2.m_SwayVelocity = float3.op_Implicit(0f);
						valueRW2.m_SwayPosition = float3.op_Implicit(0f);
					}
					else
					{
						valueRW.m_Position.y = num3;
						ref float3 position = ref oldTransform.m_Position;
						((float3)(ref position)).xz = ((float3)(ref position)).xz - val2 * num;
						valueRW2.m_SwayPosition.y = oldTransform.m_Position.y - num3;
						float3 val3 = MathUtils.Size(objectGeometryData.m_Bounds);
						float3 val4 = math.max(float3.op_Implicit(0.01f), val3 * val3);
						((float3)(ref val4)).xz = 12f / (((float3)(ref val4)).yy + ((float3)(ref val4)).xz);
						val3 *= 0.5f;
						UpdateSwaying(new SwayingData
						{
							m_VelocityFactors = new float3((0f - val3.y) * val4.x, 1f, (0f - val3.y) * val4.z),
							m_DampingFactors = float3.op_Implicit(0.02f),
							m_MaxPosition = new float3((float)Math.PI / 2f, 1000f, (float)Math.PI / 2f),
							m_SpringFactors = new float3(val3.x * val4.x * 50f, 50f, val3.z * val4.z * 50f)
						}, oldTransform, ref valueRW, ref valueRW2, num, speedDeltaFactor, localSway: false, out var _, out var _);
					}
				}
				else
				{
					valueRW = new InterpolatedTransform(transform);
				}
			}
			if ((cullingData.m_Flags & (PreCullingFlags.Skeleton | PreCullingFlags.Emissive)) == 0)
			{
				return;
			}
			Entity owner = Entity.Null;
			Owner owner2 = default(Owner);
			if (m_OwnerData.TryGetComponent(cullingData.m_Entity, ref owner2))
			{
				owner = owner2.m_Owner;
			}
			float2 wind = float2.op_Implicit(float.NaN);
			float2 efficiency = float2.op_Implicit(-1f);
			float num5 = -1f;
			if (m_VehicleData.HasComponent(cullingData.m_Entity))
			{
				efficiency = float2.op_Implicit(math.select(1f, 0f, m_ParkedCarData.HasComponent(cullingData.m_Entity) || m_ParkedTrainData.HasComponent(cullingData.m_Entity)));
				num5 = math.select(1f, 0f, m_DestroyedData.HasComponent(cullingData.m_Entity));
			}
			else
			{
				DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
				if (m_BuildingEfficiencyData.TryGetBuffer(cullingData.m_Entity, ref buffer))
				{
					efficiency = GetEfficiency(buffer);
				}
				ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
				num5 = ((!m_BuildingElectricityConsumer.TryGetComponent(cullingData.m_Entity, ref electricityConsumer)) ? efficiency.x : math.select(0f, 1f, electricityConsumer.electricityConnected));
			}
			float working = -1f;
			Game.Objects.TrafficLightState trafficLightState = Game.Objects.TrafficLightState.None;
			TrafficLight trafficLight = default(TrafficLight);
			if (m_TrafficLightData.TryGetComponent(cullingData.m_Entity, ref trafficLight))
			{
				trafficLightState = trafficLight.m_State;
			}
			DynamicBuffer<SubMesh> val5 = m_SubMeshes[prefabRef.m_Prefab];
			if ((cullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				DynamicBuffer<Skeleton> val6 = m_Skeletons[cullingData.m_Entity];
				DynamicBuffer<Bone> bones = m_Bones[cullingData.m_Entity];
				DynamicBuffer<Momentum> momentums = default(DynamicBuffer<Momentum>);
				m_Momentums.TryGetBuffer(cullingData.m_Entity, ref momentums);
				for (int i = 0; i < val6.Length; i++)
				{
					ref Skeleton reference = ref val6.ElementAt(i);
					if (!((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
					{
						SubMesh subMesh = val5[i];
						DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
						Transform transform2 = transform;
						if ((subMesh.m_Flags & SubMeshFlags.HasTransform) != 0)
						{
							transform2 = ObjectUtils.LocalToWorld(transform, subMesh.m_Position, subMesh.m_Rotation);
						}
						for (int j = 0; j < proceduralBones.Length; j++)
						{
							AnimateStaticBone(proceduralBones, bones, momentums, transform2, prefabRef, ref reference, j, num, cullingData.m_Entity, owner, flag, trafficLightState, ref random, ref wind, ref efficiency, ref num5, ref working);
						}
					}
				}
			}
			if ((cullingData.m_Flags & PreCullingFlags.Emissive) == 0)
			{
				return;
			}
			PseudoRandomSeed pseudoRandomSeed = m_PseudoRandomSeedData[cullingData.m_Entity];
			DynamicBuffer<Emissive> val7 = m_Emissives[cullingData.m_Entity];
			DynamicBuffer<LightState> lights = m_Lights[cullingData.m_Entity];
			DynamicBuffer<EnabledEffect> effects = default(DynamicBuffer<EnabledEffect>);
			m_EffectInstances.TryGetBuffer(cullingData.m_Entity, ref effects);
			CarFlags carFlags = (CarFlags)0u;
			Car car = default(Car);
			if (m_CarData.TryGetComponent(cullingData.m_Entity, ref car))
			{
				carFlags = car.m_Flags;
			}
			Random random3 = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kLightState);
			bool isBuildingActive = false;
			Building building = default(Building);
			if (m_BuildingData.TryGetComponent(cullingData.m_Entity, ref building))
			{
				isBuildingActive = !BuildingUtils.CheckOption(building, BuildingOption.Inactive);
			}
			DynamicBuffer<LightAnimation> lightAnimations = default(DynamicBuffer<LightAnimation>);
			for (int k = 0; k < val7.Length; k++)
			{
				ref Emissive reference2 = ref val7.ElementAt(k);
				if (!((NativeHeapBlock)(ref reference2.m_BufferAllocation)).Empty)
				{
					SubMesh subMesh2 = val5[k];
					DynamicBuffer<ProceduralLight> proceduralLights = m_ProceduralLights[subMesh2.m_SubMesh];
					m_LightAnimations.TryGetBuffer(subMesh2.m_SubMesh, ref lightAnimations);
					for (int l = 0; l < proceduralLights.Length; l++)
					{
						AnimateStaticLight(proceduralLights, lightAnimations, lights, isBuildingActive, ref reference2, l, num, owner, flag, random3, trafficLightState, carFlags, effects, ref num5);
					}
				}
			}
		}

		private void AnimateStaticBone(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, DynamicBuffer<Momentum> momentums, Transform transform, PrefabRef prefabRef, ref Skeleton skeleton, int index, float deltaTime, Entity entity, Entity owner, bool instantReset, Game.Objects.TrafficLightState trafficLightState, ref Random random, ref float2 wind, ref float2 efficiency, ref float electricity, ref float working)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_0976: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			ProceduralBone proceduralBone = proceduralBones[index];
			Momentum momentum = default(Momentum);
			int num = skeleton.m_BoneOffset + index;
			ref Bone reference = ref bones.ElementAt(num);
			ref Momentum reference2 = ref momentum;
			if (momentums.IsCreated)
			{
				reference2 = ref momentums.ElementAt(num);
			}
			float3 val9;
			switch (proceduralBone.m_Type)
			{
			case BoneType.LookAtDirection:
			{
				RequireEfficiency(ref efficiency, owner);
				PointOfInterest pointOfInterest = default(PointOfInterest);
				if (m_PointOfInterestData.TryGetComponent(entity, ref pointOfInterest) && pointOfInterest.m_IsValid)
				{
					quaternion val4 = LocalToWorld(proceduralBones, bones, transform, skeleton, proceduralBone.m_ParentIndex, proceduralBone.m_Rotation);
					float3 val5 = pointOfInterest.m_Position - transform.m_Position;
					val5 = math.mul(math.inverse(val4), val5);
					val5 = math.select(val5, -val5, proceduralBone.m_Speed < 0f);
					float targetSpeed2 = math.abs(proceduralBone.m_Speed) * efficiency.x;
					AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, ((float3)(ref val5)).xz, targetSpeed2, deltaTime, instantReset);
				}
				else
				{
					AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, new float2(0f, 1f), 0f, deltaTime, instantReset);
				}
				break;
			}
			case BoneType.WindTurbineRotation:
			{
				RequireWind(ref wind, transform);
				RequireEfficiency(ref efficiency, owner);
				float targetSpeed8 = proceduralBone.m_Speed * math.length(wind) * efficiency.y;
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetSpeed8, deltaTime, instantReset);
				break;
			}
			case BoneType.WindSpeedRotation:
			{
				RequireWind(ref wind, transform);
				float targetSpeed11 = proceduralBone.m_Speed * math.length(wind);
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetSpeed11, deltaTime, instantReset);
				break;
			}
			case BoneType.PoweredRotation:
			{
				RequireElectricity(ref electricity, owner);
				float targetSpeed5 = proceduralBone.m_Speed * electricity;
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetSpeed5, deltaTime, instantReset);
				break;
			}
			case BoneType.OperatingRotation:
			{
				RequireEfficiency(ref efficiency, owner);
				float targetSpeed4 = proceduralBone.m_Speed * efficiency.x;
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetSpeed4, deltaTime, instantReset);
				break;
			}
			case BoneType.WorkingRotation:
			{
				RequireEfficiency(ref efficiency, owner);
				RequireWorking(ref working, entity);
				float targetSpeed9 = proceduralBone.m_Speed * efficiency.x * working;
				AnimateRotatingBoneX(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetSpeed9, deltaTime, instantReset);
				break;
			}
			case BoneType.TrafficBarrierDirection:
			{
				float2 targetDir = math.select(new float2(math.select(-1f, 1f, proceduralBone.m_Speed < 0f), 0f), new float2(0f, 1f), (trafficLightState & (Game.Objects.TrafficLightState.Red | Game.Objects.TrafficLightState.Yellow)) == Game.Objects.TrafficLightState.Red);
				float targetSpeed = math.abs(proceduralBone.m_Speed);
				AnimateRotatingBoneZ(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, targetDir, targetSpeed, deltaTime, instantReset);
				break;
			}
			case BoneType.LookAtRotation:
			case BoneType.LookAtRotationSide:
			{
				RequireEfficiency(ref efficiency, owner);
				PointOfInterest pointOfInterest4 = default(PointOfInterest);
				if (m_PointOfInterestData.TryGetComponent(entity, ref pointOfInterest4) && pointOfInterest4.m_IsValid)
				{
					float3 position3 = proceduralBone.m_Position;
					quaternion rotation3 = proceduralBone.m_Rotation;
					LocalToWorld(proceduralBones, bones, transform, skeleton, proceduralBone.m_ParentIndex, ref position3, ref rotation3);
					float3 val12 = pointOfInterest4.m_Position - position3;
					val12 = math.mul(math.inverse(rotation3), val12);
					((float3)(ref val12)).xz = math.select(((float3)(ref val12)).xz, MathUtils.Right(((float3)(ref val12)).xz), proceduralBone.m_Type == BoneType.LookAtRotationSide);
					val12 = math.select(val12, -val12, proceduralBone.m_Speed < 0f);
					float targetSpeed7 = math.abs(proceduralBone.m_Speed) * efficiency.x;
					AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, ((float3)(ref val12)).xz, targetSpeed7, deltaTime, instantReset);
				}
				else
				{
					ProceduralBone proceduralBone5 = proceduralBone;
					ref Momentum momentum3 = ref reference2;
					val9 = math.forward();
					AnimateRotatingBoneY(proceduralBone5, ref skeleton, ref reference, ref momentum3, ref random, ((float3)(ref val9)).xz, 0f, deltaTime, instantReset);
				}
				break;
			}
			case BoneType.LengthwiseLookAtRotation:
			{
				RequireEfficiency(ref efficiency, owner);
				PointOfInterest pointOfInterest3 = default(PointOfInterest);
				if (m_PointOfInterestData.TryGetComponent(entity, ref pointOfInterest3) && pointOfInterest3.m_IsValid)
				{
					float3 position2 = proceduralBone.m_Position;
					quaternion rotation2 = proceduralBone.m_Rotation;
					LocalToWorld(proceduralBones, bones, transform, skeleton, proceduralBone.m_ParentIndex, ref position2, ref rotation2);
					float3 val8 = pointOfInterest3.m_Position - position2;
					val8 = math.mul(math.inverse(rotation2), val8);
					val8 = math.select(val8, -val8, proceduralBone.m_Speed < 0f);
					float targetSpeed6 = math.abs(proceduralBone.m_Speed) * efficiency.x;
					AnimateRotatingBoneZ(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, ((float3)(ref val8)).xy, targetSpeed6, deltaTime, instantReset);
				}
				else
				{
					ProceduralBone proceduralBone4 = proceduralBone;
					ref Momentum momentum2 = ref reference2;
					val9 = math.up();
					AnimateRotatingBoneZ(proceduralBone4, ref skeleton, ref reference, ref momentum2, ref random, ((float3)(ref val9)).xy, 0f, deltaTime, instantReset);
				}
				break;
			}
			case BoneType.LookAtAim:
			case BoneType.LookAtAimForward:
			{
				RequireEfficiency(ref efficiency, entity);
				PointOfInterest pointOfInterest5 = default(PointOfInterest);
				if (m_PointOfInterestData.TryGetComponent(entity, ref pointOfInterest5) && pointOfInterest5.m_IsValid)
				{
					float3 position4 = proceduralBone.m_Position;
					quaternion rotation4 = proceduralBone.m_Rotation;
					LookAtLocalToWorld(proceduralBones, bones, transform, skeleton, pointOfInterest5, proceduralBone.m_ParentIndex, ref position4, ref rotation4);
					float3 val14 = pointOfInterest5.m_Position - position4;
					val14 = math.mul(math.inverse(rotation4), val14);
					((float3)(ref val14)).yz = math.select(((float3)(ref val14)).yz, MathUtils.Left(((float3)(ref val14)).yz), proceduralBone.m_Type == BoneType.LookAtAimForward);
					val14 = math.select(val14, -val14, proceduralBone.m_Speed < 0f);
					float targetSpeed10 = math.abs(proceduralBone.m_Speed) * efficiency.x;
					AnimateRotatingBoneX(proceduralBone, ref skeleton, ref reference, ref reference2, ref random, ((float3)(ref val14)).yz, targetSpeed10, deltaTime, instantReset);
				}
				else
				{
					ProceduralBone proceduralBone7 = proceduralBone;
					ref Momentum momentum4 = ref reference2;
					val9 = math.up();
					AnimateRotatingBoneX(proceduralBone7, ref skeleton, ref reference, ref momentum4, ref random, ((float3)(ref val9)).yz, 0f, deltaTime, instantReset);
				}
				break;
			}
			case BoneType.FixedRotation:
			{
				ProceduralBone proceduralBone6 = proceduralBones[proceduralBone.m_ParentIndex];
				Bone bone = bones.ElementAt(skeleton.m_BoneOffset + proceduralBone.m_ParentIndex);
				quaternion val13 = math.mul(math.inverse(LocalToObject(proceduralBones, bones, skeleton, proceduralBone6.m_ParentIndex, bone.m_Rotation)), proceduralBone.m_ObjectRotation);
				skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val13);
				reference.m_Rotation = val13;
				break;
			}
			case BoneType.TimeRotation:
			{
				RequireElectricity(ref electricity, owner);
				float num4 = m_TimeOfDay * proceduralBone.m_Speed;
				float num5;
				if (instantReset)
				{
					num5 = math.select(((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI), num4, electricity != 0f);
				}
				else
				{
					val9 = math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.right());
					float2 val10 = math.normalizesafe(((float3)(ref val9)).xz, default(float2));
					num5 = math.atan2(0f - val10.y, val10.x);
					float num6 = math.abs(deltaTime) * electricity;
					num5 += math.clamp(MathUtils.RotationAngle(num5, num4), 0f - num6, num6);
				}
				quaternion val11 = math.mul(proceduralBone.m_Rotation, quaternion.RotateY(num5));
				skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val11);
				reference.m_Rotation = val11;
				break;
			}
			case BoneType.LookAtMovementX:
			case BoneType.LookAtMovementY:
			case BoneType.LookAtMovementZ:
			{
				RequireEfficiency(ref efficiency, owner);
				float3 val6 = math.select(float3.op_Implicit(0f), float3.op_Implicit(1f), new bool3(proceduralBone.m_Type == BoneType.LookAtMovementX, proceduralBone.m_Type == BoneType.LookAtMovementY, proceduralBone.m_Type == BoneType.LookAtMovementZ));
				float3 moveDirection = math.rotate(proceduralBone.m_Rotation, val6);
				PointOfInterest pointOfInterest2 = default(PointOfInterest);
				if (m_PointOfInterestData.TryGetComponent(entity, ref pointOfInterest2) && pointOfInterest2.m_IsValid)
				{
					float3 position = proceduralBone.m_Position;
					quaternion rotation = proceduralBone.m_Rotation;
					LookAtLocalToWorld(proceduralBones, bones, transform, skeleton, pointOfInterest2, proceduralBone.m_ParentIndex, ref position, ref rotation);
					float3 val7 = math.rotate(rotation, val6);
					float num3 = math.dot(pointOfInterest2.m_Position - position, val7);
					num3 = math.select(num3, 0f - num3, proceduralBone.m_Speed < 0f);
					float targetSpeed3 = math.abs(proceduralBone.m_Speed) * efficiency.x;
					AnimateMovingBone(proceduralBone, ref skeleton, ref reference, ref reference2, moveDirection, num3, targetSpeed3, deltaTime, instantReset);
				}
				else
				{
					AnimateMovingBone(proceduralBone, ref skeleton, ref reference, ref reference2, moveDirection, 0f, 0f, deltaTime, instantReset);
				}
				break;
			}
			case BoneType.PantographRotation:
				AnimatePantographBone(proceduralBones, bones, transform, prefabRef, proceduralBone, ref skeleton, ref reference, index, deltaTime, active: false, instantReset, ref m_CurveData, ref m_PrefabRefData, ref m_PrefabUtilityLaneData, ref m_PrefabObjectGeometryData, ref m_LaneSearchTree);
				break;
			case BoneType.RotationXFromMovementY:
				if (proceduralBone.m_SourceIndex >= 0)
				{
					ProceduralBone proceduralBone3 = proceduralBones[proceduralBone.m_SourceIndex];
					float num2 = (bones.ElementAt(skeleton.m_BoneOffset + proceduralBone.m_SourceIndex).m_Position.y - proceduralBone3.m_Position.y) * proceduralBone.m_Speed;
					quaternion val3 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num2));
					skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val3);
					reference.m_Rotation = val3;
				}
				break;
			case BoneType.ScaledMovement:
				if (proceduralBone.m_SourceIndex >= 0)
				{
					ProceduralBone proceduralBone2 = proceduralBones[proceduralBone.m_SourceIndex];
					float3 val = bones.ElementAt(skeleton.m_BoneOffset + proceduralBone.m_SourceIndex).m_Position - proceduralBone2.m_Position;
					float3 val2 = proceduralBone.m_Position + val * proceduralBone.m_Speed;
					skeleton.m_CurrentUpdated |= !((float3)(ref reference.m_Position)).Equals(val2);
					reference.m_Position = val2;
				}
				break;
			case BoneType.RollingTire:
			case BoneType.SteeringTire:
			case BoneType.SuspensionMovement:
			case BoneType.SteeringRotation:
			case BoneType.SuspensionRotation:
			case BoneType.FixedTire:
			case BoneType.DebugMovement:
			case BoneType.VehicleConnection:
			case BoneType.TrainBogie:
			case BoneType.RollingRotation:
			case BoneType.PropellerRotation:
			case BoneType.PropellerAngle:
			case BoneType.SteeringSuspension:
				break;
			}
		}

		private void AnimateStaticLight(DynamicBuffer<ProceduralLight> proceduralLights, DynamicBuffer<LightAnimation> lightAnimations, DynamicBuffer<LightState> lights, bool isBuildingActive, ref Emissive emissive, int index, float deltaTime, Entity owner, bool instantReset, Random pseudoRandom, Game.Objects.TrafficLightState trafficLightState, CarFlags carFlags, DynamicBuffer<EnabledEffect> effects, ref float electricity)
		{
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			ProceduralLight proceduralLight = proceduralLights[index];
			int num = emissive.m_LightOffset + index;
			ref LightState light = ref lights.ElementAt(num);
			switch (proceduralLight.m_Purpose)
			{
			case EmissiveProperties.Purpose.TrafficLight_Red:
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState & Game.Objects.TrafficLightState.Red) != 0, (trafficLightState & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			case EmissiveProperties.Purpose.TrafficLight_Yellow:
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState & Game.Objects.TrafficLightState.Yellow) != 0, (trafficLightState & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			case EmissiveProperties.Purpose.TrafficLight_Green:
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState & Game.Objects.TrafficLightState.Green) != 0, (trafficLightState & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			case EmissiveProperties.Purpose.PedestrianLight_Stop:
			{
				Game.Objects.TrafficLightState trafficLightState3 = (Game.Objects.TrafficLightState)((int)trafficLightState >> 4);
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState3 & Game.Objects.TrafficLightState.Red) != 0, (trafficLightState3 & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			}
			case EmissiveProperties.Purpose.PedestrianLight_Walk:
			{
				Game.Objects.TrafficLightState trafficLightState2 = (Game.Objects.TrafficLightState)((int)trafficLightState >> 4);
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState2 & Game.Objects.TrafficLightState.Green) != 0, (trafficLightState2 & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			}
			case EmissiveProperties.Purpose.RailCrossing_Stop:
				AnimateTrafficLight(proceduralLight, lightAnimations, pseudoRandom, ref emissive, ref light, deltaTime, instantReset, (trafficLightState & (Game.Objects.TrafficLightState.Red | Game.Objects.TrafficLightState.Yellow)) != 0, (trafficLightState & Game.Objects.TrafficLightState.Flashing) != 0);
				break;
			case EmissiveProperties.Purpose.NeonSign:
			case EmissiveProperties.Purpose.DecorativeLight:
			{
				RequireElectricity(ref electricity, owner);
				float targetIntensity3 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, m_FrameIndex, m_FrameTime, electricity);
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity3, instantReset);
				break;
			}
			case EmissiveProperties.Purpose.Emergency1:
			case EmissiveProperties.Purpose.Emergency2:
			case EmissiveProperties.Purpose.Emergency3:
			case EmissiveProperties.Purpose.Emergency4:
			case EmissiveProperties.Purpose.Emergency5:
			case EmissiveProperties.Purpose.Emergency6:
			case EmissiveProperties.Purpose.RearAlarmLights:
			case EmissiveProperties.Purpose.FrontAlarmLightsLeft:
			case EmissiveProperties.Purpose.FrontAlarmLightsRight:
			case EmissiveProperties.Purpose.Warning1:
			case EmissiveProperties.Purpose.Warning2:
			case EmissiveProperties.Purpose.Emergency7:
			case EmissiveProperties.Purpose.Emergency8:
			case EmissiveProperties.Purpose.Emergency9:
			case EmissiveProperties.Purpose.Emergency10:
			case EmissiveProperties.Purpose.AntiCollisionLightsRed:
			case EmissiveProperties.Purpose.AntiCollisionLightsWhite:
			{
				float targetIntensity6 = 0f;
				if ((carFlags & (CarFlags.Emergency | CarFlags.Warning)) != 0)
				{
					targetIntensity6 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, m_FrameIndex, m_FrameTime, 1f);
				}
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity6, instantReset);
				break;
			}
			case EmissiveProperties.Purpose.TaxiSign:
			{
				float targetIntensity5 = 0f;
				if ((carFlags & CarFlags.Sign) != 0)
				{
					targetIntensity5 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, m_FrameIndex, m_FrameTime, 1f);
				}
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity5, instantReset);
				break;
			}
			case EmissiveProperties.Purpose.CollectionLights:
			case EmissiveProperties.Purpose.WorkLights:
			{
				float targetIntensity4 = math.select(0f, 1f, (carFlags & CarFlags.Sign) != 0);
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity4, instantReset);
				break;
			}
			case EmissiveProperties.Purpose.DaytimeRunningLight:
			case EmissiveProperties.Purpose.Headlight_HighBeam:
			case EmissiveProperties.Purpose.Headlight_LowBeam:
			case EmissiveProperties.Purpose.TurnSignalLeft:
			case EmissiveProperties.Purpose.TurnSignalRight:
			case EmissiveProperties.Purpose.RearLight:
			case EmissiveProperties.Purpose.BrakeLight:
			case EmissiveProperties.Purpose.ReverseLight:
			case EmissiveProperties.Purpose.Clearance:
			case EmissiveProperties.Purpose.DaytimeRunningLightLeft:
			case EmissiveProperties.Purpose.DaytimeRunningLightRight:
			case EmissiveProperties.Purpose.SignalGroup1:
			case EmissiveProperties.Purpose.SignalGroup2:
			case EmissiveProperties.Purpose.SignalGroup3:
			case EmissiveProperties.Purpose.SignalGroup4:
			case EmissiveProperties.Purpose.SignalGroup5:
			case EmissiveProperties.Purpose.SignalGroup6:
			case EmissiveProperties.Purpose.SignalGroup7:
			case EmissiveProperties.Purpose.SignalGroup8:
			case EmissiveProperties.Purpose.SignalGroup9:
			case EmissiveProperties.Purpose.SignalGroup10:
			case EmissiveProperties.Purpose.SignalGroup11:
			case EmissiveProperties.Purpose.Interior1:
			case EmissiveProperties.Purpose.DaytimeRunningLightAlt:
			case EmissiveProperties.Purpose.Dashboard:
			case EmissiveProperties.Purpose.Clearance2:
			case EmissiveProperties.Purpose.MarkerLights:
			case EmissiveProperties.Purpose.BrakeAndTurnSignalLeft:
			case EmissiveProperties.Purpose.BrakeAndTurnSignalRight:
			case EmissiveProperties.Purpose.TaxiLights:
			case EmissiveProperties.Purpose.LandingLights:
			case EmissiveProperties.Purpose.WingInspectionLights:
			case EmissiveProperties.Purpose.LogoLights:
			case EmissiveProperties.Purpose.PositionLightLeft:
			case EmissiveProperties.Purpose.PositionLightRight:
			case EmissiveProperties.Purpose.PositionLights:
			case EmissiveProperties.Purpose.SearchLightsFront:
			case EmissiveProperties.Purpose.SearchLights360:
			case EmissiveProperties.Purpose.NumberLight:
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, 0f, instantReset);
				break;
			case EmissiveProperties.Purpose.EffectSource:
				if (effects.IsCreated)
				{
					float targetIntensity2 = 0f;
					int num2 = 0;
					if (effects.Length > num2)
					{
						EnabledEffect enabledEffect = effects[num2];
						targetIntensity2 = math.select(0f, 1f, (m_EnabledData[enabledEffect.m_EnabledIndex].m_Flags & EnabledEffectFlags.IsEnabled) != 0);
					}
					AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity2, instantReset);
				}
				break;
			case EmissiveProperties.Purpose.BuildingActive:
			{
				if (!isBuildingActive)
				{
					AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, 0f, instantReset);
					break;
				}
				RequireElectricity(ref electricity, owner);
				float targetIntensity = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, m_FrameIndex, m_FrameTime, electricity);
				AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity, instantReset);
				break;
			}
			case EmissiveProperties.Purpose.Interior2:
			case EmissiveProperties.Purpose.BoardingLightLeft:
			case EmissiveProperties.Purpose.BoardingLightRight:
				break;
			}
		}

		private void AnimateTrafficLight(ProceduralLight proceduralLight, DynamicBuffer<LightAnimation> lightAnimations, Random pseudoRandom, ref Emissive emissive, ref LightState light, float deltaTime, bool instantReset, bool on, bool flashing)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			float num = math.select(0f, 1f, on);
			if (flashing)
			{
				num = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, m_FrameIndex, m_FrameTime, num);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, num, instantReset);
		}

		private void RequireWind(ref float2 wind, Transform transform)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (math.isnan(wind.x))
			{
				wind = Wind.SampleWind(m_WindData, transform.m_Position);
			}
		}

		private void RequireEfficiency(ref float2 efficiency, Entity owner)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (efficiency.x >= 0f)
			{
				return;
			}
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			if (m_BuildingEfficiencyData.TryGetBuffer(owner, ref buffer))
			{
				efficiency = GetEfficiency(buffer);
				return;
			}
			Owner owner2 = default(Owner);
			while (m_OwnerData.TryGetComponent(owner, ref owner2))
			{
				owner = owner2.m_Owner;
				if (m_BuildingEfficiencyData.TryGetBuffer(owner, ref buffer))
				{
					efficiency = GetEfficiency(buffer);
					return;
				}
			}
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(owner, ref attachment) && m_BuildingEfficiencyData.TryGetBuffer(attachment.m_Attached, ref buffer))
			{
				efficiency = GetEfficiency(buffer);
			}
			else
			{
				efficiency = float2.op_Implicit(1f);
			}
		}

		private float2 GetEfficiency(DynamicBuffer<Efficiency> buffer)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			float2 val = float2.op_Implicit(1f);
			Enumerator<Efficiency> enumerator = buffer.GetEnumerator();
			try
			{
				float2 val2 = default(float2);
				while (enumerator.MoveNext())
				{
					Efficiency current = enumerator.Current;
					val2.x = current.m_Efficiency;
					val2.y = math.select(1f, current.m_Efficiency, current.m_Factor != EfficiencyFactor.Fire);
					val *= math.max(float2.op_Implicit(0f), val2);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return math.select(float2.op_Implicit(0f), math.max(float2.op_Implicit(0.01f), math.round(100f * val) * 0.01f), val > 0f);
		}

		private void RequireElectricity(ref float electricity, Entity owner)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			if (electricity >= 0f)
			{
				return;
			}
			electricity = 1f;
			ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
			if (m_BuildingElectricityConsumer.TryGetComponent(owner, ref electricityConsumer))
			{
				electricity = math.select(0f, 1f, electricityConsumer.electricityConnected);
				return;
			}
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			if (m_BuildingEfficiencyData.TryGetBuffer(owner, ref buffer))
			{
				electricity = BuildingUtils.GetEfficiency(buffer);
			}
			Owner owner2 = default(Owner);
			while (m_OwnerData.TryGetComponent(owner, ref owner2))
			{
				owner = owner2.m_Owner;
				if (m_BuildingElectricityConsumer.TryGetComponent(owner, ref electricityConsumer))
				{
					electricity = math.select(0f, 1f, electricityConsumer.electricityConnected);
					return;
				}
				if (m_BuildingEfficiencyData.TryGetBuffer(owner, ref buffer))
				{
					electricity = BuildingUtils.GetEfficiency(buffer);
				}
			}
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(owner, ref attachment))
			{
				if (m_BuildingElectricityConsumer.TryGetComponent(attachment.m_Attached, ref electricityConsumer))
				{
					electricity = math.select(0f, 1f, electricityConsumer.electricityConnected);
				}
				else if (m_BuildingEfficiencyData.TryGetBuffer(attachment.m_Attached, ref buffer))
				{
					electricity = BuildingUtils.GetEfficiency(buffer);
				}
			}
		}

		private void UpdateInterpolatedTransforms(PreCullingData cullingData, DynamicBuffer<TransformFrame> transformFrames, ref Random random)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			CalculateUpdateFrames(m_FrameIndex, m_PrevFrameIndex, m_FrameTime, (uint)cullingData.m_UpdateFrame, out var updateFrame, out var updateFrame2, out var framePosition, out var updateFrameChanged);
			float updateFrameToSeconds = 4f / 15f;
			float deltaTime = m_FrameDelta / 60f;
			float speedDeltaFactor = math.select(60f / m_FrameDelta, 0f, m_FrameDelta == 0f);
			if ((cullingData.m_Flags & PreCullingFlags.Animated) != 0)
			{
				UpdateInterpolatedAnimations(cullingData, transformFrames, ref random, updateFrame, updateFrame2, framePosition, updateFrameToSeconds, deltaTime, speedDeltaFactor, updateFrameChanged);
			}
			else if ((cullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				UpdateInterpolatedAnimations(cullingData, transformFrames, updateFrame, updateFrame2, framePosition, updateFrameToSeconds, deltaTime, speedDeltaFactor, ref random);
			}
			else
			{
				UpdateInterpolatedTransforms(cullingData, transformFrames, updateFrame, updateFrame2, framePosition, updateFrameToSeconds, deltaTime, speedDeltaFactor);
			}
		}

		private void RequireWorking(ref float working, Entity owner)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (!(working >= 0f))
			{
				Game.Buildings.ExtractorFacility extractorFacility = default(Game.Buildings.ExtractorFacility);
				if (m_BuildingExtractorFacility.TryGetComponent(owner, ref extractorFacility))
				{
					working = math.select(0f, 1f, (extractorFacility.m_Flags & ExtractorFlags.Working) != 0);
				}
				else
				{
					working = 1f;
				}
			}
		}

		private void UpdateInterpolatedTransforms(PreCullingData cullingData, DynamicBuffer<TransformFrame> frames, uint updateFrame1, uint updateFrame2, float framePosition, float updateFrameToSeconds, float deltaTime, float speedDeltaFactor)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			ref InterpolatedTransform valueRW = ref m_InterpolatedTransformData.GetRefRW(cullingData.m_Entity).ValueRW;
			InterpolatedTransform oldTransform = valueRW;
			TransformFrame frame = frames[(int)updateFrame1];
			TransformFrame frame2 = frames[(int)updateFrame2];
			valueRW = CalculateTransform(frame, frame2, framePosition);
			if (m_SwayingData.HasComponent(cullingData.m_Entity))
			{
				ref Swaying valueRW2 = ref m_SwayingData.GetRefRW(cullingData.m_Entity).ValueRW;
				PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
				SwayingData swayingData = default(SwayingData);
				if ((cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0)
				{
					valueRW2.m_LastVelocity = math.lerp(frame.m_Velocity, frame2.m_Velocity, framePosition);
					valueRW2.m_SwayVelocity = float3.op_Implicit(0f);
					valueRW2.m_SwayPosition = float3.op_Implicit(0f);
				}
				else if (m_PrefabSwayingData.TryGetComponent(prefabRef.m_Prefab, ref swayingData))
				{
					UpdateSwaying(swayingData, oldTransform, ref valueRW, ref valueRW2, deltaTime, speedDeltaFactor, localSway: true, out var _, out var _);
				}
			}
		}

		private void UpdateInterpolatedAnimations(PreCullingData cullingData, DynamicBuffer<TransformFrame> frames, uint updateFrame1, uint updateFrame2, float framePosition, float updateFrameToSeconds, float deltaTime, float speedDeltaFactor, ref Random random)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = false;
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(cullingData.m_Entity, ref controller))
			{
				flag = controller.m_Controller != Entity.Null;
				if (m_CarTrailerData.HasComponent(cullingData.m_Entity))
				{
					CullingInfo cullingInfo = default(CullingInfo);
					if (m_CullingInfoData.TryGetComponent(controller.m_Controller, ref cullingInfo) && cullingInfo.m_CullingIndex != 0 && (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0)
					{
						return;
					}
					flag2 = true;
				}
			}
			flag2 |= (cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0;
			ref InterpolatedTransform valueRW = ref m_InterpolatedTransformData.GetRefRW(cullingData.m_Entity).ValueRW;
			InterpolatedTransform interpolatedTransform = valueRW;
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			TransformFrame frame = frames[(int)updateFrame1];
			TransformFrame frame2 = frames[(int)updateFrame2];
			valueRW = CalculateTransform(frame, frame2, framePosition);
			quaternion swayRotation = quaternion.identity;
			float swayOffset = 0f;
			if (m_SwayingData.HasComponent(cullingData.m_Entity))
			{
				ref Swaying valueRW2 = ref m_SwayingData.GetRefRW(cullingData.m_Entity).ValueRW;
				SwayingData swayingData = default(SwayingData);
				if (flag2)
				{
					valueRW2.m_LastVelocity = math.lerp(frame.m_Velocity, frame2.m_Velocity, framePosition);
					valueRW2.m_SwayVelocity = float3.op_Implicit(0f);
					valueRW2.m_SwayPosition = float3.op_Implicit(0f);
				}
				else if (m_PrefabSwayingData.TryGetComponent(prefabRef.m_Prefab, ref swayingData))
				{
					UpdateSwaying(swayingData, interpolatedTransform, ref valueRW, ref valueRW2, deltaTime, speedDeltaFactor, localSway: true, out swayRotation, out swayOffset);
				}
			}
			DynamicBuffer<SubMesh> val = m_SubMeshes[prefabRef.m_Prefab];
			if ((cullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				DynamicBuffer<Skeleton> val2 = m_Skeletons[cullingData.m_Entity];
				DynamicBuffer<Bone> bones = m_Bones[cullingData.m_Entity];
				DynamicBuffer<Momentum> momentums = default(DynamicBuffer<Momentum>);
				m_Momentums.TryGetBuffer(cullingData.m_Entity, ref momentums);
				CarData carData = default(CarData);
				for (int i = 0; i < val2.Length; i++)
				{
					ref Skeleton reference = ref val2.ElementAt(i);
					if (!((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
					{
						SubMesh subMesh = val[i];
						DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
						InterpolatedTransform oldTransform = interpolatedTransform;
						InterpolatedTransform newTransform = valueRW;
						if ((subMesh.m_Flags & SubMeshFlags.HasTransform) != 0)
						{
							oldTransform = ObjectUtils.LocalToWorld(interpolatedTransform, subMesh.m_Position, subMesh.m_Rotation);
							newTransform = ObjectUtils.LocalToWorld(valueRW, subMesh.m_Position, subMesh.m_Rotation);
						}
						float steeringRadius = 0f;
						if (m_PrefabCarData.TryGetComponent(prefabRef.m_Prefab, ref carData))
						{
							steeringRadius = CalculateSteeringRadius(proceduralBones, bones, oldTransform, newTransform, ref reference, carData);
						}
						for (int j = 0; j < proceduralBones.Length; j++)
						{
							AnimateInterpolatedBone(proceduralBones, bones, momentums, oldTransform, newTransform, prefabRef, ref reference, swayRotation, swayOffset, steeringRadius, carData.m_PivotOffset, j, deltaTime, cullingData.m_Entity, flag2, m_FrameIndex, m_FrameTime, ref random, ref m_PointOfInterestData, ref m_CurveData, ref m_PrefabRefData, ref m_PrefabUtilityLaneData, ref m_PrefabObjectGeometryData, ref m_LaneSearchTree);
						}
					}
				}
			}
			if ((cullingData.m_Flags & PreCullingFlags.Emissive) == 0)
			{
				return;
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if (!flag || !m_PseudoRandomSeedData.TryGetComponent(controller.m_Controller, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = m_PseudoRandomSeedData[cullingData.m_Entity];
			}
			DynamicBuffer<Emissive> val3 = m_Emissives[cullingData.m_Entity];
			DynamicBuffer<LightState> lights = m_Lights[cullingData.m_Entity];
			Random random2 = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kLightState);
			DynamicBuffer<LightAnimation> lightAnimations = default(DynamicBuffer<LightAnimation>);
			for (int k = 0; k < val3.Length; k++)
			{
				ref Emissive reference2 = ref val3.ElementAt(k);
				if (!((NativeHeapBlock)(ref reference2.m_BufferAllocation)).Empty)
				{
					SubMesh subMesh2 = val[k];
					DynamicBuffer<ProceduralLight> proceduralLights = m_ProceduralLights[subMesh2.m_SubMesh];
					m_LightAnimations.TryGetBuffer(subMesh2.m_SubMesh, ref lightAnimations);
					for (int l = 0; l < proceduralLights.Length; l++)
					{
						AnimateInterpolatedLight(proceduralLights, lightAnimations, lights, valueRW.m_Flags, random2, ref reference2, l, m_FrameIndex, m_FrameTime, deltaTime, flag2);
					}
				}
			}
		}

		private void UpdateInterpolatedAnimations(PreCullingData cullingData, DynamicBuffer<TransformFrame> frames, ref Random random, uint updateFrame1, uint updateFrame2, float framePosition, float updateFrameToSeconds, float deltaTime, float speedDeltaFactor, int updateFrameChanged)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0;
			ref InterpolatedTransform valueRW = ref m_InterpolatedTransformData.GetRefRW(cullingData.m_Entity).ValueRW;
			InterpolatedTransform oldTransform = valueRW;
			TransformFrame transformFrame = frames[(int)updateFrame1];
			TransformFrame transformFrame2 = frames[(int)updateFrame2];
			if ((cullingData.m_Flags & PreCullingFlags.Animated) != 0)
			{
				PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
				DynamicBuffer<Animated> val = m_Animateds[cullingData.m_Entity];
				DynamicBuffer<SubMesh> val2 = m_SubMeshes[prefabRef.m_Prefab];
				float stateTimer = 0f;
				TransformState state = TransformState.Default;
				ActivityType activity = ActivityType.None;
				DynamicBuffer<MeshGroup> val3 = default(DynamicBuffer<MeshGroup>);
				DynamicBuffer<CharacterElement> val4 = default(DynamicBuffer<CharacterElement>);
				int priority = 0;
				DynamicBuffer<SubMeshGroup> val5 = default(DynamicBuffer<SubMeshGroup>);
				if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val5))
				{
					m_MeshGroups.TryGetBuffer(cullingData.m_Entity, ref val3);
					m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val4);
					valueRW = CalculateTransform(transformFrame, transformFrame2, framePosition);
					CullingInfo cullingInfo = m_CullingInfoData[cullingData.m_Entity];
					float num = RenderingUtils.CalculateMinDistance(cullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					priority = RenderingUtils.CalculateLod(num * num, m_LodParameters) - cullingInfo.m_MinLod;
				}
				else
				{
					if (framePosition >= 0.5f)
					{
						stateTimer = (float)(int)transformFrame2.m_StateTimer + (framePosition - 0.5f);
						state = transformFrame2.m_State;
						activity = (ActivityType)transformFrame2.m_Activity;
					}
					else
					{
						stateTimer = (float)(int)transformFrame.m_StateTimer + (framePosition + 0.5f);
						state = transformFrame.m_State;
						activity = (ActivityType)transformFrame.m_Activity;
					}
					TransformState state2 = transformFrame.m_State;
					if (state2 - 3 <= TransformState.Move)
					{
						if (framePosition >= 0.5f)
						{
							valueRW.m_Position = transformFrame2.m_Position;
							valueRW.m_Rotation = transformFrame2.m_Rotation;
							valueRW.m_Flags = transformFrame2.m_Flags;
						}
						else
						{
							valueRW.m_Position = transformFrame.m_Position;
							valueRW.m_Rotation = transformFrame.m_Rotation;
							valueRW.m_Flags = transformFrame.m_Flags;
						}
					}
					else
					{
						valueRW = CalculateTransform(transformFrame, transformFrame2, framePosition);
					}
				}
				MeshGroup meshGroup = default(MeshGroup);
				MeshGroup meshGroup2 = default(MeshGroup);
				for (int i = 0; i < val.Length; i++)
				{
					Animated animated = val[i];
					if (animated.m_ClipIndexBody0 != -1)
					{
						if (val4.IsCreated)
						{
							CollectionUtils.TryGet<MeshGroup>(val3, i, ref meshGroup);
							CharacterElement characterElement = val4[(int)meshGroup.m_SubMeshGroup];
							DynamicBuffer<AnimationClip> clips = m_AnimationClips[characterElement.m_Style];
							UpdateInterpolatedAnimationBody(cullingData.m_Entity, in characterElement, clips, ref m_HumanData, ref m_CurrentVehicleData, ref m_PrefabRefData, ref m_ActivityLocations, ref m_AnimationMotions, oldTransform, valueRW, ref animated, ref random, transformFrame, transformFrame2, framePosition, updateFrameToSeconds, speedDeltaFactor, deltaTime, updateFrameChanged, flag);
							UpdateInterpolatedAnimationFace(cullingData.m_Entity, clips, ref m_HumanData, ref animated, ref random, state, activity, deltaTime, updateFrameChanged, flag);
							m_AnimationData.SetAnimationFrame(in characterElement, clips, in animated, float2.op_Implicit(GetUpdateFrameTransition(framePosition)), priority, flag);
						}
						else
						{
							int num2 = i;
							if (val5.IsCreated)
							{
								CollectionUtils.TryGet<MeshGroup>(val3, i, ref meshGroup2);
								num2 = val5[(int)meshGroup2.m_SubMeshGroup].m_SubMeshRange.x;
							}
							SubMesh subMesh = val2[num2];
							UpdateInterpolatedAnimation(m_AnimationClips[subMesh.m_SubMesh], oldTransform, valueRW, ref animated, stateTimer, state, activity, updateFrameToSeconds, speedDeltaFactor);
						}
					}
					val[i] = animated;
				}
			}
			else
			{
				valueRW = CalculateTransform(transformFrame, transformFrame2, framePosition);
			}
		}
	}

	[BurstCompile]
	private struct UpdateTrailerTransformDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<PointOfInterest> m_PointOfInterestData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SwayingData> m_PrefabSwayingData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<CarTractorData> m_PrefabCarTractorData;

		[ReadOnly]
		public ComponentLookup<CarTrailerData> m_PrefabCarTrailerData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<TrainBogieFrame> m_BogieFrames;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public BufferLookup<ProceduralLight> m_ProceduralLights;

		[ReadOnly]
		public BufferLookup<LightAnimation> m_LightAnimations;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Swaying> m_SwayingData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Skeleton> m_Skeletons;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Emissive> m_Emissive;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Bone> m_Bones;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Momentum> m_Momentums;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LightState> m_LightStates;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public float m_FrameDelta;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public void Execute(int index)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData cullingData = m_CullingData[index];
			if ((cullingData.m_Flags & (PreCullingFlags.NearCamera | PreCullingFlags.Temp | PreCullingFlags.VehicleLayout | PreCullingFlags.Relative)) != (PreCullingFlags.NearCamera | PreCullingFlags.VehicleLayout))
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(index);
			DynamicBuffer<TransformFrame> val = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(cullingData.m_Entity, ref val))
			{
				if (m_CarData.HasComponent(cullingData.m_Entity))
				{
					UpdateInterpolatedCarTrailers(cullingData, ref random);
				}
				else
				{
					UpdateInterpolatedLayoutAnimations(cullingData);
				}
			}
			else if (m_ParkedTrainData.HasComponent(cullingData.m_Entity))
			{
				UpdateStaticLayoutAnimations(cullingData);
			}
		}

		private void UpdateInterpolatedCarTrailers(PreCullingData cullingData, ref Random random)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			CalculateUpdateFrames(m_FrameIndex, m_FrameTime, (uint)cullingData.m_UpdateFrame, out var updateFrame, out var updateFrame2, out var framePosition);
			float deltaTime = m_FrameDelta / 60f;
			float speedDeltaFactor = math.select(60f / m_FrameDelta, 0f, m_FrameDelta == 0f);
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<LayoutElement> val = m_LayoutElements[cullingData.m_Entity];
			if (val.Length <= 1)
			{
				return;
			}
			InterpolatedTransform interpolatedTransform = m_InterpolatedTransformData[cullingData.m_Entity];
			PseudoRandomSeed pseudoRandomSeed = m_PseudoRandomSeedData[cullingData.m_Entity];
			CarTractorData carTractorData = m_PrefabCarTractorData[prefabRef.m_Prefab];
			bool flag = (cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0;
			SwayingData swayingData = default(SwayingData);
			CarData carData2 = default(CarData);
			DynamicBuffer<LightAnimation> lightAnimations = default(DynamicBuffer<LightAnimation>);
			for (int i = 1; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				InterpolatedTransform interpolatedTransform2 = m_InterpolatedTransformData[vehicle];
				PrefabRef prefabRef2 = m_PrefabRefData[vehicle];
				DynamicBuffer<TransformFrame> val2 = m_TransformFrames[vehicle];
				CarData carData = m_PrefabCarData[prefabRef2.m_Prefab];
				CarTrailerData carTrailerData = m_PrefabCarTrailerData[prefabRef2.m_Prefab];
				TransformFrame frame = val2[(int)updateFrame];
				TransformFrame frame2 = val2[(int)updateFrame2];
				InterpolatedTransform newTransform = CalculateTransform(frame, frame2, framePosition);
				switch (carTrailerData.m_MovementType)
				{
				case TrailerMovementType.Free:
				{
					float3 val3 = interpolatedTransform.m_Position + math.rotate(interpolatedTransform.m_Rotation, carTractorData.m_AttachPosition);
					float3 val4 = newTransform.m_Position + math.rotate(newTransform.m_Rotation, new float3(((float3)(ref carTrailerData.m_AttachPosition)).xy, carData.m_PivotOffset));
					newTransform.m_Rotation = interpolatedTransform.m_Rotation;
					float3 val5 = val3 - val4;
					if (MathUtils.TryNormalize(ref val5))
					{
						newTransform.m_Rotation = quaternion.LookRotationSafe(val5, math.up());
					}
					newTransform.m_Position = val3 - math.rotate(newTransform.m_Rotation, carTrailerData.m_AttachPosition);
					break;
				}
				case TrailerMovementType.Locked:
				{
					newTransform.m_Position = interpolatedTransform.m_Position;
					ref float3 position = ref newTransform.m_Position;
					position -= math.rotate(newTransform.m_Rotation, carTrailerData.m_AttachPosition);
					ref float3 position2 = ref newTransform.m_Position;
					position2 += math.rotate(interpolatedTransform.m_Rotation, carTractorData.m_AttachPosition);
					newTransform.m_Rotation = interpolatedTransform.m_Rotation;
					break;
				}
				}
				quaternion swayRotation = quaternion.identity;
				float swayOffset = 0f;
				if (m_SwayingData.HasComponent(vehicle))
				{
					Swaying swaying = m_SwayingData[vehicle];
					if (flag)
					{
						swaying.m_LastVelocity = math.lerp(frame.m_Velocity, frame2.m_Velocity, framePosition);
						swaying.m_SwayVelocity = float3.op_Implicit(0f);
						swaying.m_SwayPosition = float3.op_Implicit(0f);
					}
					else if (m_PrefabSwayingData.TryGetComponent(prefabRef2.m_Prefab, ref swayingData))
					{
						swayingData.m_MaxPosition.z = 0f;
						UpdateSwaying(swayingData, interpolatedTransform2, ref newTransform, ref swaying, deltaTime, speedDeltaFactor, localSway: true, out swayRotation, out swayOffset);
					}
					m_SwayingData[vehicle] = swaying;
				}
				if (m_Skeletons.HasBuffer(vehicle))
				{
					DynamicBuffer<Skeleton> val6 = m_Skeletons[vehicle];
					DynamicBuffer<Bone> bones = m_Bones[vehicle];
					DynamicBuffer<SubMesh> val7 = m_SubMeshes[prefabRef2.m_Prefab];
					DynamicBuffer<Momentum> momentums = default(DynamicBuffer<Momentum>);
					if (m_Momentums.HasBuffer(vehicle))
					{
						momentums = m_Momentums[vehicle];
					}
					for (int j = 0; j < val6.Length; j++)
					{
						ref Skeleton reference = ref val6.ElementAt(j);
						if (!((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
						{
							SubMesh subMesh = val7[j];
							DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
							InterpolatedTransform oldTransform = interpolatedTransform2;
							InterpolatedTransform newTransform2 = newTransform;
							if ((subMesh.m_Flags & SubMeshFlags.HasTransform) != 0)
							{
								oldTransform = ObjectUtils.LocalToWorld(interpolatedTransform2, subMesh.m_Position, subMesh.m_Rotation);
								newTransform2 = ObjectUtils.LocalToWorld(newTransform, subMesh.m_Position, subMesh.m_Rotation);
							}
							float steeringRadius = 0f;
							if (m_PrefabCarData.TryGetComponent(prefabRef.m_Prefab, ref carData2))
							{
								steeringRadius = CalculateSteeringRadius(proceduralBones, bones, oldTransform, newTransform2, ref reference, carData2);
							}
							for (int k = 0; k < proceduralBones.Length; k++)
							{
								AnimateInterpolatedBone(proceduralBones, bones, momentums, oldTransform, newTransform2, prefabRef, ref reference, swayRotation, swayOffset, steeringRadius, carData2.m_PivotOffset, k, deltaTime, vehicle, flag, m_FrameIndex, m_FrameTime, ref random, ref m_PointOfInterestData, ref m_CurveData, ref m_PrefabRefData, ref m_PrefabUtilityLaneData, ref m_PrefabObjectGeometryData, ref m_LaneSearchTree);
							}
						}
					}
				}
				if (m_Emissive.HasBuffer(vehicle))
				{
					DynamicBuffer<Emissive> val8 = m_Emissive[vehicle];
					DynamicBuffer<LightState> lights = m_LightStates[vehicle];
					DynamicBuffer<SubMesh> val9 = m_SubMeshes[prefabRef2.m_Prefab];
					Random random2 = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kLightState);
					for (int l = 0; l < val8.Length; l++)
					{
						ref Emissive reference2 = ref val8.ElementAt(l);
						if (!((NativeHeapBlock)(ref reference2.m_BufferAllocation)).Empty)
						{
							SubMesh subMesh2 = val9[l];
							DynamicBuffer<ProceduralLight> proceduralLights = m_ProceduralLights[subMesh2.m_SubMesh];
							m_LightAnimations.TryGetBuffer(subMesh2.m_SubMesh, ref lightAnimations);
							for (int m = 0; m < proceduralLights.Length; m++)
							{
								AnimateInterpolatedLight(proceduralLights, lightAnimations, lights, newTransform.m_Flags, random2, ref reference2, m, m_FrameIndex, m_FrameTime, deltaTime, flag);
							}
						}
					}
				}
				m_InterpolatedTransformData[vehicle] = newTransform;
				if (i != val.Length - 1)
				{
					interpolatedTransform = newTransform;
					carTractorData = m_PrefabCarTractorData[prefabRef2.m_Prefab];
				}
			}
		}

		private void UpdateInterpolatedLayoutAnimations(PreCullingData cullingData)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			CalculateUpdateFrames(m_FrameIndex, m_FrameTime, (uint)cullingData.m_UpdateFrame, out var updateFrame, out var updateFrame2, out var _);
			DynamicBuffer<LayoutElement> val = m_LayoutElements[cullingData.m_Entity];
			if (val.Length == 0)
			{
				return;
			}
			Entity val2 = val[0].m_Vehicle;
			PrefabRef prefabRef = m_PrefabRefData[val2];
			InterpolatedTransform prevTransform = default(InterpolatedTransform);
			InterpolatedTransform interpolatedTransform = m_InterpolatedTransformData[val2];
			ObjectGeometryData prevGeometryData = default(ObjectGeometryData);
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			bool prevReversed = false;
			bool flag = false;
			Train train = default(Train);
			if (m_TrainData.TryGetComponent(val2, ref train))
			{
				flag = (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0;
			}
			DynamicBuffer<TrainBogieFrame> val6 = default(DynamicBuffer<TrainBogieFrame>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = default(Entity);
				PrefabRef prefabRef2 = default(PrefabRef);
				InterpolatedTransform interpolatedTransform2 = default(InterpolatedTransform);
				ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
				bool flag2 = false;
				if (i < val.Length - 1)
				{
					val3 = val[i + 1].m_Vehicle;
					prefabRef2 = m_PrefabRefData[val3];
					interpolatedTransform2 = m_InterpolatedTransformData[val3];
					objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
					if (m_TrainData.TryGetComponent(val3, ref train))
					{
						flag2 = (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0;
					}
				}
				if (m_Skeletons.HasBuffer(val2))
				{
					DynamicBuffer<Skeleton> val4 = m_Skeletons[val2];
					DynamicBuffer<Bone> bones = m_Bones[val2];
					DynamicBuffer<SubMesh> val5 = m_SubMeshes[prefabRef.m_Prefab];
					TrainBogieFrame bogieFrame = default(TrainBogieFrame);
					TrainBogieFrame bogieFrame2 = default(TrainBogieFrame);
					if (m_BogieFrames.TryGetBuffer(val2, ref val6))
					{
						bogieFrame = val6[(int)updateFrame];
						bogieFrame2 = val6[(int)updateFrame2];
					}
					for (int j = 0; j < val4.Length; j++)
					{
						ref Skeleton reference = ref val4.ElementAt(j);
						if (!((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
						{
							SubMesh subMesh = val5[j];
							DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
							for (int k = 0; k < proceduralBones.Length; k++)
							{
								AnimateInterpolatedLayoutBone(proceduralBones, bones, prevTransform, interpolatedTransform, interpolatedTransform2, prevGeometryData, objectGeometryData, objectGeometryData2, bogieFrame, bogieFrame2, prevReversed, flag, flag2, ref reference, k);
							}
						}
					}
				}
				prevTransform = interpolatedTransform;
				prevGeometryData = objectGeometryData;
				prevReversed = flag;
				val2 = val3;
				prefabRef = prefabRef2;
				interpolatedTransform = interpolatedTransform2;
				objectGeometryData = objectGeometryData2;
				flag = flag2;
			}
		}

		private void UpdateStaticLayoutAnimations(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LayoutElement> val = m_LayoutElements[cullingData.m_Entity];
			if (val.Length == 0)
			{
				return;
			}
			Entity val2 = val[0].m_Vehicle;
			PrefabRef prefabRef = m_PrefabRefData[val2];
			Transform prevTransform = default(Transform);
			Transform transform = m_TransformData[val2];
			ObjectGeometryData prevGeometryData = default(ObjectGeometryData);
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			bool prevReversed = false;
			bool flag = false;
			Train train = default(Train);
			if (m_TrainData.TryGetComponent(val2, ref train))
			{
				flag = (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = default(Entity);
				PrefabRef prefabRef2 = default(PrefabRef);
				Transform transform2 = default(Transform);
				ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
				bool flag2 = false;
				if (i < val.Length - 1)
				{
					val3 = val[i + 1].m_Vehicle;
					prefabRef2 = m_PrefabRefData[val3];
					transform2 = m_TransformData[val3];
					objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
					if (m_TrainData.TryGetComponent(val3, ref train))
					{
						flag2 = (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0;
					}
				}
				if (m_Skeletons.HasBuffer(val2))
				{
					ParkedTrain parkedTrain = m_ParkedTrainData[val2];
					DynamicBuffer<Skeleton> val4 = m_Skeletons[val2];
					DynamicBuffer<Bone> bones = m_Bones[val2];
					DynamicBuffer<SubMesh> val5 = m_SubMeshes[prefabRef.m_Prefab];
					for (int j = 0; j < val4.Length; j++)
					{
						ref Skeleton reference = ref val4.ElementAt(j);
						if (!((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
						{
							SubMesh subMesh = val5[j];
							DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
							for (int k = 0; k < proceduralBones.Length; k++)
							{
								AnimateStaticLayoutBone(proceduralBones, bones, prevTransform, transform, transform2, parkedTrain, prevGeometryData, objectGeometryData, objectGeometryData2, prevReversed, flag, flag2, ref reference, k);
							}
						}
					}
				}
				prevTransform = transform;
				prevGeometryData = objectGeometryData;
				prevReversed = flag;
				val2 = val3;
				prefabRef = prefabRef2;
				transform = transform2;
				objectGeometryData = objectGeometryData2;
				flag = flag2;
			}
		}

		private void AnimateInterpolatedLayoutBone(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, InterpolatedTransform prevTransform, InterpolatedTransform curTransform, InterpolatedTransform nextTransform, ObjectGeometryData prevGeometryData, ObjectGeometryData curGeometryData, ObjectGeometryData nextGeometryData, TrainBogieFrame bogieFrame1, TrainBogieFrame bogieFrame2, bool prevReversed, bool curReversed, bool nextReversed, ref Skeleton skeleton, int index)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			ProceduralBone proceduralBone = proceduralBones[index];
			int num = skeleton.m_BoneOffset + index;
			ref Bone bone = ref bones.ElementAt(num);
			switch (proceduralBone.m_Type)
			{
			case BoneType.VehicleConnection:
				AnimateVehicleConnectionBone(proceduralBone, ref skeleton, ref bone, prevGeometryData, curGeometryData, nextGeometryData, prevTransform.ToTransform(), curTransform.ToTransform(), nextTransform.ToTransform(), prevReversed, curReversed, nextReversed);
				break;
			case BoneType.TrainBogie:
			{
				float num2 = 2f;
				Entity val;
				Entity val2;
				if (proceduralBone.m_ObjectPosition.z >= 0f == curReversed)
				{
					val = bogieFrame1.m_RearLane;
					val2 = bogieFrame2.m_RearLane;
				}
				else
				{
					val = bogieFrame1.m_FrontLane;
					val2 = bogieFrame2.m_FrontLane;
				}
				float3 val3 = ObjectUtils.LocalToWorld(curTransform.ToTransform(), new float3(0f, 0f, proceduralBone.m_ObjectPosition.z));
				float3 position = default(float3);
				float3 tangent = default(float3);
				Curve curve = default(Curve);
				if (m_CurveData.TryGetComponent(val, ref curve))
				{
					float num4 = default(float);
					float num3 = MathUtils.Distance(curve.m_Bezier, val3, ref num4);
					if (num3 < num2)
					{
						position = MathUtils.Position(curve.m_Bezier, num4);
						tangent = MathUtils.Tangent(curve.m_Bezier, num4);
						num2 = num3;
					}
				}
				Curve curve2 = default(Curve);
				if (val != val2 && m_CurveData.TryGetComponent(val2, ref curve2))
				{
					float num6 = default(float);
					float num5 = MathUtils.Distance(curve2.m_Bezier, val3, ref num6);
					if (num5 < num2)
					{
						position = MathUtils.Position(curve2.m_Bezier, num6);
						tangent = MathUtils.Tangent(curve2.m_Bezier, num6);
						num2 = num5;
					}
				}
				AnimateTrainBogieBone(curTransform.ToTransform(), proceduralBone, ref skeleton, ref bone, position, tangent, num2 != 2f);
				break;
			}
			}
		}

		private void AnimateStaticLayoutBone(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform prevTransform, Transform curTransform, Transform nextTransform, ParkedTrain parkedTrain, ObjectGeometryData prevGeometryData, ObjectGeometryData curGeometryData, ObjectGeometryData nextGeometryData, bool prevReversed, bool curReversed, bool nextReversed, ref Skeleton skeleton, int index)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			ProceduralBone proceduralBone = proceduralBones[index];
			int num = skeleton.m_BoneOffset + index;
			ref Bone bone = ref bones.ElementAt(num);
			switch (proceduralBone.m_Type)
			{
			case BoneType.VehicleConnection:
				AnimateVehicleConnectionBone(proceduralBone, ref skeleton, ref bone, prevGeometryData, curGeometryData, nextGeometryData, prevTransform, curTransform, nextTransform, prevReversed, curReversed, nextReversed);
				break;
			case BoneType.TrainBogie:
			{
				Entity val;
				float num2;
				if (proceduralBone.m_ObjectPosition.z >= 0f == curReversed)
				{
					val = parkedTrain.m_RearLane;
					num2 = parkedTrain.m_CurvePosition.y;
				}
				else
				{
					val = parkedTrain.m_FrontLane;
					num2 = parkedTrain.m_CurvePosition.x;
				}
				float3 position = default(float3);
				float3 tangent = default(float3);
				Curve curve = default(Curve);
				bool flag = m_CurveData.TryGetComponent(val, ref curve);
				if (flag)
				{
					position = MathUtils.Position(curve.m_Bezier, num2);
					tangent = MathUtils.Tangent(curve.m_Bezier, num2);
				}
				AnimateTrainBogieBone(curTransform, proceduralBone, ref skeleton, ref bone, position, tangent, flag);
				break;
			}
			}
		}

		private void AnimateVehicleConnectionBone(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ObjectGeometryData prevGeometryData, ObjectGeometryData curGeometryData, ObjectGeometryData nextGeometryData, Transform prevTransform, Transform curTransform, Transform nextTransform, bool prevReversed, bool curReversed, bool nextReversed)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			quaternion val = math.inverse(curTransform.m_Rotation);
			float3 val2 = default(float3);
			float3 val3 = default(float3);
			quaternion val4;
			if (proceduralBone.m_ObjectPosition.z >= 0f == curReversed)
			{
				if (nextGeometryData.m_Bounds.max.z == nextGeometryData.m_Bounds.min.z)
				{
					skeleton.m_CurrentUpdated |= !((float3)(ref bone.m_Position)).Equals(proceduralBone.m_Position) | !((quaternion)(ref bone.m_Rotation)).Equals(proceduralBone.m_Rotation);
					bone.m_Position = proceduralBone.m_Position;
					bone.m_Rotation = proceduralBone.m_Rotation;
					return;
				}
				((float3)(ref val2))._002Ector(((float3)(ref proceduralBone.m_Position)).xy, math.select(curGeometryData.m_Bounds.min.z, curGeometryData.m_Bounds.max.z, curReversed));
				((float3)(ref val3))._002Ector(((float3)(ref proceduralBone.m_Position)).xy, math.select(nextGeometryData.m_Bounds.max.z, nextGeometryData.m_Bounds.min.z, nextReversed));
				val3 = math.rotate(nextTransform.m_Rotation, val3);
				val3 = math.rotate(val, val3 + (nextTransform.m_Position - curTransform.m_Position));
				val4 = math.mul(val, nextTransform.m_Rotation);
				if (nextReversed != curReversed)
				{
					val4 = math.mul(val4, quaternion.RotateY((float)Math.PI));
				}
			}
			else
			{
				if (prevGeometryData.m_Bounds.max.z == prevGeometryData.m_Bounds.min.z)
				{
					skeleton.m_CurrentUpdated |= !((float3)(ref bone.m_Position)).Equals(proceduralBone.m_Position) | !((quaternion)(ref bone.m_Rotation)).Equals(proceduralBone.m_Rotation);
					bone.m_Position = proceduralBone.m_Position;
					bone.m_Rotation = proceduralBone.m_Rotation;
					return;
				}
				((float3)(ref val2))._002Ector(((float3)(ref proceduralBone.m_Position)).xy, math.select(curGeometryData.m_Bounds.max.z, curGeometryData.m_Bounds.min.z, curReversed));
				((float3)(ref val3))._002Ector(((float3)(ref proceduralBone.m_Position)).xy, math.select(prevGeometryData.m_Bounds.min.z, prevGeometryData.m_Bounds.max.z, prevReversed));
				val3 = math.rotate(prevTransform.m_Rotation, val3);
				val3 = math.rotate(val, val3 + (prevTransform.m_Position - curTransform.m_Position));
				val4 = math.mul(val, prevTransform.m_Rotation);
				if (prevReversed != curReversed)
				{
					val4 = math.mul(val4, quaternion.RotateY((float)Math.PI));
				}
			}
			float num = math.sign(val2.z) * math.distance(val2, val3) * 0.25f;
			val2.z += num;
			val3 += math.rotate(val4, new float3(0f, 0f, 0f - num));
			float3 val5 = (val2 + val3) * 0.5f;
			quaternion val6 = math.slerp(quaternion.identity, val4, 0.5f);
			skeleton.m_CurrentUpdated |= !((float3)(ref bone.m_Position)).Equals(val5) | !((quaternion)(ref bone.m_Rotation)).Equals(val6);
			bone.m_Position = val5;
			bone.m_Rotation = val6;
		}

		private void AnimateTrainBogieBone(Transform transform, ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, float3 position, float3 tangent, bool positionValid)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			float3 val = proceduralBone.m_Position;
			quaternion val2 = proceduralBone.m_Rotation;
			if (positionValid)
			{
				quaternion val3 = math.inverse(transform.m_Rotation);
				val = math.rotate(val3, position - transform.m_Position);
				val.y += proceduralBone.m_Position.y;
				float3 val4 = math.forward(math.mul(transform.m_Rotation, proceduralBone.m_ObjectRotation));
				tangent = math.select(tangent, -tangent, math.dot(tangent, val4) < 0f);
				val2 = math.mul(val3, quaternion.LookRotationSafe(tangent, math.up()));
			}
			skeleton.m_CurrentUpdated |= !((float3)(ref bone.m_Position)).Equals(val) | !((quaternion)(ref bone.m_Rotation)).Equals(val2);
			bone.m_Position = val;
			bone.m_Rotation = val2;
		}
	}

	[BurstCompile]
	private struct UpdateQueryTransformDataJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public ComponentTypeHandle<Swaying> m_SwayingType;

		[ReadOnly]
		public ComponentTypeHandle<Static> m_StaticType;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> m_StoppedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.Animation> m_AnimationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		[ReadOnly]
		public BufferTypeHandle<EnabledEffect> m_EffectInstancesType;

		[ReadOnly]
		public BufferTypeHandle<IconElement> m_IconElementType;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<ProceduralLight> m_ProceduralLights;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Animated> m_Animateds;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Skeleton> m_Skeletons;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Emissive> m_Emissives;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Bone> m_Bones;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LightState> m_Lights;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public AnimatedSystem.AnimationData m_AnimationData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_082f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0842: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<CullingInfo> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			uint updateFrame = 0u;
			uint updateFrame2 = 0u;
			float framePosition = 0f;
			if (((ArchetypeChunk)(ref chunk)).Has<UpdateFrame>(m_UpdateFrameType))
			{
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				CalculateUpdateFrames(m_FrameIndex, m_FrameTime, index, out updateFrame, out updateFrame2, out framePosition);
			}
			if (nativeArray2.Length != 0)
			{
				NativeArray<Game.Tools.Animation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.Animation>(ref m_AnimationType);
				NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<MeshGroup> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
				BufferAccessor<EnabledEffect> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<EnabledEffect>(ref m_EffectInstancesType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Swaying>(ref m_SwayingType);
				bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Static>(ref m_StaticType);
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref m_StoppedType);
				bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<IconElement>(ref m_IconElementType);
				Game.Tools.Animation animation = default(Game.Tools.Animation);
				DynamicBuffer<MeshGroup> val3 = default(DynamicBuffer<MeshGroup>);
				DynamicBuffer<CharacterElement> val4 = default(DynamicBuffer<CharacterElement>);
				MeshGroup meshGroup = default(MeshGroup);
				Transform transform2 = default(Transform);
				InterpolatedTransform interpolatedTransform = default(InterpolatedTransform);
				DynamicBuffer<MeshGroup> val6 = default(DynamicBuffer<MeshGroup>);
				DynamicBuffer<CharacterElement> val7 = default(DynamicBuffer<CharacterElement>);
				DynamicBuffer<Animated> val8 = default(DynamicBuffer<Animated>);
				MeshGroup meshGroup2 = default(MeshGroup);
				MeshGroup meshGroup3 = default(MeshGroup);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val = nativeArray[i];
					Temp temp = nativeArray2[i];
					if (!flag4)
					{
						CullingInfo cullingInfo = nativeArray3[i];
						if (cullingInfo.m_CullingIndex == 0 || (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) == 0)
						{
							continue;
						}
					}
					if (m_InterpolatedTransformData.HasComponent(val))
					{
						if (((flag2 || flag3) && (temp.m_Original == Entity.Null || (temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0)) || (temp.m_Flags & TempFlags.Dragging) != 0)
						{
							Transform transform = ((!CollectionUtils.TryGet<Game.Tools.Animation>(nativeArray4, i, ref animation)) ? m_TransformData[val] : animation.ToTransform());
							if (flag)
							{
								transform.m_Position.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, transform.m_Position);
							}
							m_InterpolatedTransformData[val] = new InterpolatedTransform(transform);
							if (!m_Animateds.HasBuffer(val))
							{
								continue;
							}
							PrefabRef prefabRef = nativeArray5[i];
							DynamicBuffer<Animated> val2 = m_Animateds[val];
							CollectionUtils.TryGet<MeshGroup>(bufferAccessor, i, ref val3);
							m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val4);
							for (int j = 0; j < val2.Length; j++)
							{
								Animated animated = val2[j];
								if (animated.m_ClipIndexBody0 != -1)
								{
									animated.m_ClipIndexBody0 = 0;
									animated.m_Time = float4.op_Implicit(0f);
									animated.m_MovementSpeed = float2.op_Implicit(0f);
									animated.m_Interpolation = 0f;
									animated.m_PreviousTime = 0f;
									val2[j] = animated;
								}
								if (animated.m_MetaIndex != 0 && val4.IsCreated)
								{
									CollectionUtils.TryGet<MeshGroup>(val3, j, ref meshGroup);
									CharacterElement characterElement = val4[(int)meshGroup.m_SubMeshGroup];
									Animated animated2 = new Animated
									{
										m_MetaIndex = animated.m_MetaIndex,
										m_ClipIndexBody0 = -1,
										m_ClipIndexBody0I = -1,
										m_ClipIndexBody1 = -1,
										m_ClipIndexBody1I = -1,
										m_ClipIndexFace0 = -1,
										m_ClipIndexFace1 = -1
									};
									DynamicBuffer<AnimationClip> clips = m_AnimationClips[characterElement.m_Style];
									m_AnimationData.SetAnimationFrame(in characterElement, clips, in animated2, float2.op_Implicit(0f), -1, reset: true);
								}
							}
							continue;
						}
						if (m_TransformData.TryGetComponent(temp.m_Original, ref transform2))
						{
							m_TransformData[val] = transform2;
							if (m_InterpolatedTransformData.TryGetComponent(temp.m_Original, ref interpolatedTransform))
							{
								m_InterpolatedTransformData[val] = interpolatedTransform;
							}
							else
							{
								m_InterpolatedTransformData[val] = new InterpolatedTransform(transform2);
							}
						}
						else
						{
							m_InterpolatedTransformData[val] = new InterpolatedTransform(m_TransformData[val]);
						}
					}
					if (m_Animateds.HasBuffer(val))
					{
						PrefabRef prefabRef2 = nativeArray5[i];
						DynamicBuffer<Animated> val5 = m_Animateds[val];
						bool reset = false;
						CullingInfo cullingInfo2 = nativeArray3[i];
						if (cullingInfo2.m_CullingIndex != 0)
						{
							reset = (m_CullingData[cullingInfo2.m_CullingIndex].m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated)) != 0;
						}
						float num = RenderingUtils.CalculateMinDistance(cullingInfo2.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
						int priority = RenderingUtils.CalculateLod(num * num, m_LodParameters) - cullingInfo2.m_MinLod;
						CollectionUtils.TryGet<MeshGroup>(bufferAccessor, i, ref val6);
						m_CharacterElements.TryGetBuffer(prefabRef2.m_Prefab, ref val7);
						if (m_Animateds.TryGetBuffer(temp.m_Original, ref val8) && val8.Length == val5.Length)
						{
							for (int k = 0; k < val5.Length; k++)
							{
								Animated animated3 = val5[k];
								Animated animated4 = val8[k];
								animated3.m_ClipIndexBody0 = animated4.m_ClipIndexBody0;
								animated3.m_ClipIndexBody0I = animated4.m_ClipIndexBody0I;
								animated3.m_ClipIndexBody1 = animated4.m_ClipIndexBody1;
								animated3.m_ClipIndexBody1I = animated4.m_ClipIndexBody1I;
								animated3.m_ClipIndexFace0 = animated4.m_ClipIndexFace0;
								animated3.m_ClipIndexFace1 = animated4.m_ClipIndexFace1;
								animated3.m_Time = animated4.m_Time;
								animated3.m_MovementSpeed = animated4.m_MovementSpeed;
								animated3.m_Interpolation = animated4.m_Interpolation;
								animated3.m_PreviousTime = animated4.m_PreviousTime;
								val5[k] = animated3;
								if (animated3.m_MetaIndex != 0 && val7.IsCreated)
								{
									CollectionUtils.TryGet<MeshGroup>(val6, k, ref meshGroup2);
									CharacterElement characterElement2 = val7[(int)meshGroup2.m_SubMeshGroup];
									float num2 = framePosition * framePosition;
									num2 = 3f * num2 - 2f * num2 * framePosition;
									DynamicBuffer<AnimationClip> clips2 = m_AnimationClips[characterElement2.m_Style];
									m_AnimationData.SetAnimationFrame(in characterElement2, clips2, in animated3, float2.op_Implicit(num2), priority, reset);
								}
							}
						}
						else
						{
							for (int l = 0; l < val5.Length; l++)
							{
								Animated animated5 = val5[l];
								if (animated5.m_ClipIndexBody0 != -1)
								{
									animated5.m_ClipIndexBody0 = 0;
									animated5.m_Time = float4.op_Implicit(0f);
									animated5.m_MovementSpeed = float2.op_Implicit(0f);
									animated5.m_Interpolation = 0f;
									animated5.m_PreviousTime = 0f;
									val5[l] = animated5;
								}
								if (animated5.m_MetaIndex != 0 && val7.IsCreated)
								{
									CollectionUtils.TryGet<MeshGroup>(val6, l, ref meshGroup3);
									CharacterElement characterElement3 = val7[(int)meshGroup3.m_SubMeshGroup];
									Animated animated6 = new Animated
									{
										m_MetaIndex = animated5.m_MetaIndex,
										m_ClipIndexBody0 = -1,
										m_ClipIndexBody0I = -1,
										m_ClipIndexBody1 = -1,
										m_ClipIndexBody1I = -1,
										m_ClipIndexFace0 = -1,
										m_ClipIndexFace1 = -1
									};
									DynamicBuffer<AnimationClip> clips3 = m_AnimationClips[characterElement3.m_Style];
									m_AnimationData.SetAnimationFrame(in characterElement3, clips3, in animated6, float2.op_Implicit(0f), -1, reset);
								}
							}
						}
					}
					if (m_Bones.HasBuffer(val))
					{
						DynamicBuffer<Skeleton> val9 = m_Skeletons[val];
						DynamicBuffer<Bone> val10 = m_Bones[val];
						if (m_Bones.HasBuffer(temp.m_Original))
						{
							DynamicBuffer<Bone> val11 = m_Bones[temp.m_Original];
							if (val10.Length == val11.Length)
							{
								for (int m = 0; m < val9.Length; m++)
								{
									val9.ElementAt(m).m_CurrentUpdated = true;
								}
								for (int n = 0; n < val10.Length; n++)
								{
									val10[n] = val11[n];
								}
							}
						}
					}
					if (!m_Lights.HasBuffer(val))
					{
						continue;
					}
					DynamicBuffer<Emissive> val12 = m_Emissives[val];
					DynamicBuffer<LightState> val13 = m_Lights[val];
					DynamicBuffer<EnabledEffect> val14 = default(DynamicBuffer<EnabledEffect>);
					if (bufferAccessor2.Length != 0)
					{
						val14 = bufferAccessor2[i];
					}
					PrefabRef prefabRef3 = nativeArray5[i];
					DynamicBuffer<SubMesh> val15 = m_SubMeshes[prefabRef3.m_Prefab];
					DynamicBuffer<LightState> val16 = default(DynamicBuffer<LightState>);
					bool flag5 = false;
					if (m_Lights.HasBuffer(temp.m_Original))
					{
						val16 = m_Lights[temp.m_Original];
						flag5 = val16.Length == val13.Length;
					}
					for (int num3 = 0; num3 < val12.Length; num3++)
					{
						ref Emissive reference = ref val12.ElementAt(num3);
						if (((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
						{
							continue;
						}
						reference.m_Updated = true;
						SubMesh subMesh = val15[num3];
						DynamicBuffer<ProceduralLight> val17 = m_ProceduralLights[subMesh.m_SubMesh];
						for (int num4 = 0; num4 < val17.Length; num4++)
						{
							int num5 = reference.m_LightOffset + num4;
							ProceduralLight proceduralLight = val17[num4];
							ref LightState reference2 = ref val13.ElementAt(num5);
							if (proceduralLight.m_Purpose == EmissiveProperties.Purpose.EffectSource)
							{
								if (val14.IsCreated)
								{
									reference2.m_Intensity = 0f;
									int num6 = 0;
									if (val14.Length > num6)
									{
										EnabledEffect enabledEffect = val14[num6];
										reference2.m_Intensity = math.select(0f, 1f, (m_EnabledData[enabledEffect.m_EnabledIndex].m_Flags & EnabledEffectFlags.IsEnabled) != 0);
									}
								}
							}
							else if (flag5)
							{
								reference2 = val16[num5];
							}
						}
					}
				}
				return;
			}
			BufferAccessor<TransformFrame> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			for (int num7 = 0; num7 < nativeArray.Length; num7++)
			{
				CullingInfo cullingInfo3 = nativeArray3[num7];
				if (cullingInfo3.m_CullingIndex == 0 || (m_CullingData[cullingInfo3.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					DynamicBuffer<TransformFrame> val18 = bufferAccessor3[num7];
					TransformFrame frame = val18[(int)updateFrame];
					TransformFrame frame2 = val18[(int)updateFrame2];
					m_InterpolatedTransformData[nativeArray[num7]] = CalculateTransform(frame, frame2, framePosition);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct CatenaryIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds3 m_Bounds;

		public Segment m_Line;

		public float3 m_Result;

		public float m_Default;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(m_Bounds, bounds.m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(m_Bounds, bounds.m_Bounds))
			{
				PrefabRef prefabRef = m_PrefabRefData[item];
				UtilityLaneData utilityLaneData = default(UtilityLaneData);
				if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & (UtilityTypes.LowVoltageLine | UtilityTypes.Catenary)) != UtilityTypes.None)
				{
					Curve curve = m_CurveData[item];
					float num = default(float);
					MathUtils.Distance(curve.m_Bezier, MathUtils.Position(m_Line, 0.5f), ref num);
					float3 val = MathUtils.Position(curve.m_Bezier, num);
					float num2 = math.max(0f, MathUtils.Distance(m_Line, val, ref num) - m_Default * 0.5f);
					float num3 = num * m_Default * 2f;
					float3 val2 = default(float3);
					((float3)(ref val2))._002Ector(num3, num2, num3 + num2);
					m_Result = math.select(m_Result, val2, val2.z < m_Result.z);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PointOfInterest> __Game_Common_PointOfInterest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLight> __Game_Objects_TrafficLight_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ExtractorFacility> __Game_Buildings_ExtractorFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailer> __Game_Vehicles_CarTrailer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SwayingData> __Game_Prefabs_SwayingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationMotion> __Game_Prefabs_AnimationMotion_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralLight> __Game_Prefabs_ProceduralLight_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LightAnimation> __Game_Prefabs_LightAnimation_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentLookup;

		public ComponentLookup<Swaying> __Game_Rendering_Swaying_RW_ComponentLookup;

		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RW_BufferLookup;

		public BufferLookup<Emissive> __Game_Rendering_Emissive_RW_BufferLookup;

		public BufferLookup<Animated> __Game_Rendering_Animated_RW_BufferLookup;

		public BufferLookup<Bone> __Game_Rendering_Bone_RW_BufferLookup;

		public BufferLookup<Momentum> __Game_Rendering_Momentum_RW_BufferLookup;

		public BufferLookup<LightState> __Game_Rendering_LightState_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTractorData> __Game_Prefabs_CarTractorData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailerData> __Game_Prefabs_CarTrailerData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TrainBogieFrame> __Game_Vehicles_TrainBogieFrame_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Swaying> __Game_Rendering_Swaying_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Static> __Game_Objects_Static_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> __Game_Objects_Stopped_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.Animation> __Game_Tools_Animation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<IconElement> __Game_Notifications_IconElement_RO_BufferTypeHandle;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

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
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Common_PointOfInterest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PointOfInterest>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_TrafficLight_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLight>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_ExtractorFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ExtractorFacility>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Vehicles_CarTrailer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailer>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Prefabs_SwayingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SwayingData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Effects_EnabledEffect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
			__Game_Prefabs_ProceduralLight_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralLight>(true);
			__Game_Prefabs_LightAnimation_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LightAnimation>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(false);
			__Game_Rendering_Swaying_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Swaying>(false);
			__Game_Rendering_Skeleton_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(false);
			__Game_Rendering_Emissive_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(false);
			__Game_Rendering_Animated_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Animated>(false);
			__Game_Rendering_Bone_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(false);
			__Game_Rendering_Momentum_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Momentum>(false);
			__Game_Rendering_LightState_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LightState>(false);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Prefabs_CarTractorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTractorData>(true);
			__Game_Prefabs_CarTrailerData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerData>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_TrainBogieFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TrainBogieFrame>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
			__Game_Rendering_Swaying_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Swaying>(true);
			__Game_Objects_Static_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Static>(true);
			__Game_Objects_Stopped_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stopped>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Animation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.Animation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_TransformFrame_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Effects_EnabledEffect_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<EnabledEffect>(true);
			__Game_Notifications_IconElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<IconElement>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private PreCullingSystem m_PreCullingSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EffectControlSystem m_EffectControlSystem;

	private WindSystem m_WindSystem;

	private AnimatedSystem m_AnimatedSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private BatchDataSystem m_BatchDataSystem;

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_InterpolateQuery;

	private uint m_PrevFrameIndex;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<Animated>(),
			ComponentType.ReadWrite<Bone>(),
			ComponentType.ReadWrite<LightState>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Relative>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<IconElement>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<TransformFrame>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Relative>()
		};
		array[1] = val;
		m_InterpolateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_094d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeList<PreCullingData> cullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies2);
		JobHandle dependencies3;
		NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies3);
		JobHandle dependencies4;
		AnimatedSystem.AnimationData animationData = m_AnimatedSystem.GetAnimationData(out dependencies4);
		JobHandle deps;
		WaterSurfaceData surfaceData = m_WaterSystem.GetSurfaceData(out deps);
		JobHandle deps2;
		WaterSurfaceData velocitiesSurfaceData = m_WaterSystem.GetVelocitiesSurfaceData(out deps2);
		TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
		float3 cameraPosition = default(float3);
		float3 cameraDirection = default(float3);
		float4 lodParameters = default(float4);
		if (m_CameraUpdateSystem.TryGetLODParameters(out var lodParameters2))
		{
			cameraPosition = float3.op_Implicit(((LODParameters)(ref lodParameters2)).cameraPosition);
			IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
			lodParameters = RenderingUtils.CalculateLodParameters(m_BatchDataSystem.GetLevelOfDetail(m_RenderingSystem.frameLod, activeCameraController), lodParameters2);
			cameraDirection = m_CameraUpdateSystem.activeViewer.forward;
		}
		JobHandle dependencies5;
		UpdateTransformDataJob updateTransformDataJob = new UpdateTransformDataJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PointOfInterestData = InternalCompilerInterface.GetComponentLookup<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightData = InternalCompilerInterface.GetComponentLookup<TrafficLight>(ref __TypeHandle.__Game_Objects_TrafficLight_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencyData = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingElectricityConsumer = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingExtractorFacility = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ExtractorFacility>(ref __TypeHandle.__Game_Buildings_ExtractorFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerData = InternalCompilerInterface.GetComponentLookup<CarTrailer>(ref __TypeHandle.__Game_Vehicles_CarTrailer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSwayingData = InternalCompilerInterface.GetComponentLookup<SwayingData>(ref __TypeHandle.__Game_Prefabs_SwayingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectInstances = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralLights = InternalCompilerInterface.GetBufferLookup<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LightAnimations = InternalCompilerInterface.GetBufferLookup<LightAnimation>(ref __TypeHandle.__Game_Prefabs_LightAnimation_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SwayingData = InternalCompilerInterface.GetComponentLookup<Swaying>(ref __TypeHandle.__Game_Rendering_Swaying_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Momentums = InternalCompilerInterface.GetBufferLookup<Momentum>(ref __TypeHandle.__Game_Rendering_Momentum_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lights = InternalCompilerInterface.GetBufferLookup<LightState>(ref __TypeHandle.__Game_Rendering_LightState_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrevFrameIndex = m_PrevFrameIndex,
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_FrameDelta = m_RenderingSystem.frameDelta,
			m_TimeOfDay = m_RenderingSystem.timeOfDay,
			m_LodParameters = lodParameters,
			m_CameraPosition = cameraPosition,
			m_CameraDirection = cameraDirection,
			m_RandomSeed = RandomSeed.Next(),
			m_WindData = m_WindSystem.GetData(readOnly: true, out dependencies5),
			m_LaneSearchTree = laneSearchTree,
			m_CullingData = cullingData,
			m_EnabledData = enabledData,
			m_AnimationData = animationData,
			m_WaterSurfaceData = surfaceData,
			m_WaterVelocityData = velocitiesSurfaceData,
			m_TerrainHeightData = heightData
		};
		UpdateTrailerTransformDataJob updateTrailerTransformDataJob = new UpdateTrailerTransformDataJob
		{
			m_PointOfInterestData = InternalCompilerInterface.GetComponentLookup<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSwayingData = InternalCompilerInterface.GetComponentLookup<SwayingData>(ref __TypeHandle.__Game_Prefabs_SwayingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarTractorData = InternalCompilerInterface.GetComponentLookup<CarTractorData>(ref __TypeHandle.__Game_Prefabs_CarTractorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarTrailerData = InternalCompilerInterface.GetComponentLookup<CarTrailerData>(ref __TypeHandle.__Game_Prefabs_CarTrailerData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BogieFrames = InternalCompilerInterface.GetBufferLookup<TrainBogieFrame>(ref __TypeHandle.__Game_Vehicles_TrainBogieFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralLights = InternalCompilerInterface.GetBufferLookup<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LightAnimations = InternalCompilerInterface.GetBufferLookup<LightAnimation>(ref __TypeHandle.__Game_Prefabs_LightAnimation_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SwayingData = InternalCompilerInterface.GetComponentLookup<Swaying>(ref __TypeHandle.__Game_Rendering_Swaying_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissive = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Momentums = InternalCompilerInterface.GetBufferLookup<Momentum>(ref __TypeHandle.__Game_Rendering_Momentum_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LightStates = InternalCompilerInterface.GetBufferLookup<LightState>(ref __TypeHandle.__Game_Rendering_LightState_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_FrameDelta = m_RenderingSystem.frameDelta,
			m_RandomSeed = RandomSeed.Next(),
			m_LaneSearchTree = laneSearchTree,
			m_CullingData = cullingData
		};
		UpdateQueryTransformDataJob obj = new UpdateQueryTransformDataJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SwayingType = InternalCompilerInterface.GetComponentTypeHandle<Swaying>(ref __TypeHandle.__Game_Rendering_Swaying_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StaticType = InternalCompilerInterface.GetComponentTypeHandle<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedType = InternalCompilerInterface.GetComponentTypeHandle<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.Animation>(ref __TypeHandle.__Game_Tools_Animation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EffectInstancesType = InternalCompilerInterface.GetBufferTypeHandle<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementType = InternalCompilerInterface.GetBufferTypeHandle<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralLights = InternalCompilerInterface.GetBufferLookup<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lights = InternalCompilerInterface.GetBufferLookup<LightState>(ref __TypeHandle.__Game_Rendering_LightState_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_LodParameters = lodParameters,
			m_CameraPosition = cameraPosition,
			m_CameraDirection = cameraDirection,
			m_CullingData = cullingData,
			m_EnabledData = enabledData,
			m_AnimationData = animationData,
			m_WaterSurfaceData = surfaceData,
			m_TerrainHeightData = heightData
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<UpdateTransformDataJob, PreCullingData>(updateTransformDataJob, cullingData, 16, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3, dependencies5, dependencies4, deps, deps2));
		JobHandle val2 = IJobParallelForDeferExtensions.Schedule<UpdateTrailerTransformDataJob, PreCullingData>(updateTrailerTransformDataJob, cullingData, 16, val);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateQueryTransformDataJob>(obj, m_InterpolateQuery, val2);
		m_WindSystem.AddReader(val);
		m_NetSearchSystem.AddLaneSearchTreeReader(val2);
		m_PreCullingSystem.AddCullingDataReader(val3);
		m_EffectControlSystem.AddEnabledDataReader(val3);
		m_AnimatedSystem.AddAnimationWriter(val3);
		m_WaterSystem.AddSurfaceReader(val3);
		m_WaterSystem.AddVelocitySurfaceReader(val);
		m_TerrainSystem.AddCPUHeightReader(val3);
		((SystemBase)this).Dependency = val3;
		m_PrevFrameIndex = m_RenderingSystem.frameIndex;
	}

	private static void UpdateSwaying(SwayingData swayingData, InterpolatedTransform oldTransform, ref InterpolatedTransform newTransform, ref Swaying swaying, float deltaTime, float speedDeltaFactor, bool localSway, out quaternion swayRotation, out float swayOffset)
	{
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (deltaTime != 0f)
		{
			float3 val = oldTransform.m_Position;
			if (localSway)
			{
				val -= math.mul(oldTransform.m_Rotation, new float3(0f, swaying.m_SwayPosition.y, 0f));
			}
			else
			{
				val.y -= swaying.m_SwayPosition.y;
			}
			float3 val2 = (newTransform.m_Position - val) * speedDeltaFactor;
			float3 val3 = val2 - swaying.m_LastVelocity;
			if (localSway)
			{
				val3 = math.mul(math.inverse(newTransform.m_Rotation), val3);
			}
			ref float3 swayVelocity = ref swaying.m_SwayVelocity;
			swayVelocity += val3 * swayingData.m_VelocityFactors - swaying.m_SwayPosition * swayingData.m_SpringFactors * deltaTime;
			ref float3 swayVelocity2 = ref swaying.m_SwayVelocity;
			swayVelocity2 *= math.pow(swayingData.m_DampingFactors, float3.op_Implicit(deltaTime));
			ref float3 swayPosition = ref swaying.m_SwayPosition;
			swayPosition += swaying.m_SwayVelocity * deltaTime;
			swaying.m_SwayVelocity = math.select(swaying.m_SwayVelocity, float3.op_Implicit(0f), ((swaying.m_SwayPosition >= swayingData.m_MaxPosition) & (swaying.m_SwayVelocity >= 0f)) | ((swaying.m_SwayPosition <= -swayingData.m_MaxPosition) & (swaying.m_SwayVelocity <= 0f)));
			swaying.m_SwayPosition = math.clamp(swaying.m_SwayPosition, -swayingData.m_MaxPosition, swayingData.m_MaxPosition);
			swaying.m_LastVelocity = val2;
		}
		float2 xz = ((float3)(ref swaying.m_SwayPosition)).xz;
		if (MathUtils.TryNormalize(ref xz))
		{
			swayRotation = quaternion.AxisAngle(new float3(0f - xz.y, 0f, xz.x), math.length(((float3)(ref swaying.m_SwayPosition)).xz));
			if (localSway)
			{
				newTransform.m_Rotation = math.mul(newTransform.m_Rotation, swayRotation);
			}
			else
			{
				newTransform.m_Rotation = math.mul(swayRotation, newTransform.m_Rotation);
			}
			swayRotation = math.inverse(swayRotation);
		}
		else
		{
			swayRotation = quaternion.identity;
		}
		if (localSway)
		{
			ref float3 position = ref newTransform.m_Position;
			position += math.mul(newTransform.m_Rotation, new float3(0f, swaying.m_SwayPosition.y, 0f));
		}
		else
		{
			newTransform.m_Position.y += swaying.m_SwayPosition.y;
		}
		swayOffset = 0f - swaying.m_SwayPosition.y;
	}

	private static float CalculateSteeringRadius(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, InterpolatedTransform oldTransform, InterpolatedTransform newTransform, ref Skeleton skeleton, CarData carData)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		float num2 = -1f;
		float num3 = 0f;
		float2 val3 = default(float2);
		for (int i = 0; i < proceduralBones.Length; i++)
		{
			ProceduralBone proceduralBone = proceduralBones[i];
			int num4 = skeleton.m_BoneOffset + i;
			ref Bone reference = ref bones.ElementAt(num4);
			BoneType type = proceduralBone.m_Type;
			ProceduralBone proceduralBone2;
			float3 val;
			float3 val2;
			float num5;
			if (type != BoneType.SteeringTire)
			{
				if (type != BoneType.SteeringRotation)
				{
					if (type != BoneType.SteeringSuspension || !FindChildBone(proceduralBones, i, out var childIndex))
					{
						continue;
					}
					proceduralBone2 = proceduralBones[childIndex];
					if (FindChildBone(proceduralBones, childIndex, out var childIndex2))
					{
						proceduralBone2 = proceduralBones[childIndex2];
					}
					val = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone2.m_ObjectPosition);
					val2 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone2.m_ObjectPosition);
					num5 = math.asin(math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.left()).z);
				}
				else
				{
					if (!FindChildBone(proceduralBones, i, out var childIndex3))
					{
						continue;
					}
					proceduralBone2 = proceduralBones[childIndex3];
					if (FindChildBone(proceduralBones, childIndex3, out var childIndex4))
					{
						proceduralBone2 = proceduralBones[childIndex4];
					}
					val = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone2.m_ObjectPosition);
					val2 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone2.m_ObjectPosition);
					num5 = math.asin(math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.right()).y);
				}
			}
			else
			{
				val = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
				val2 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition);
				proceduralBone2 = proceduralBone;
				num5 = math.asin(math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.left()).z);
			}
			((float2)(ref val3))._002Ector(proceduralBone2.m_ObjectPosition.x, proceduralBone2.m_ObjectPosition.z - carData.m_PivotOffset);
			val3.y *= 0.5f;
			num3 = math.max(num3, math.csum(math.abs(val3)));
			float3 val4 = val2 - val;
			float3 val5 = math.mul(newTransform.m_Rotation, math.right());
			float3 val6 = math.forward(newTransform.m_Rotation);
			float num6 = math.dot(val4, val6);
			float num7 = math.dot(val4, val5);
			num6 += math.select(0.001f, -0.001f, num6 < 0f);
			float3 val7 = math.normalizesafe(val6 * num6 + val5 * num7, default(float3));
			val7 = math.select(val7, -val7, num6 < 0f);
			float num8 = math.abs(math.dot(val4, val7));
			if (!(num8 <= num2))
			{
				num2 = num8;
				float num9 = math.asin(math.dot(val5, val7));
				float num10 = num8 / math.max(0.01f, proceduralBone2.m_ObjectPosition.y * 2f);
				num5 += math.clamp(num9 - num5, 0f - num10, num10);
				num = (proceduralBone2.m_ObjectPosition.z - carData.m_PivotOffset) / math.tan(num5) + proceduralBone2.m_ObjectPosition.x;
			}
		}
		num = math.select(num, num3, num < num3 && num >= 0f);
		return math.select(num, 0f - num3, num > 0f - num3 && num < 0f);
	}

	private static void AnimateInterpolatedBone(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, DynamicBuffer<Momentum> momentums, InterpolatedTransform oldTransform, InterpolatedTransform newTransform, PrefabRef prefabRef, ref Skeleton skeleton, quaternion swayRotation, float swayOffset, float steeringRadius, float pivotOffset, int index, float deltaTime, Entity entity, bool instantReset, uint frameIndex, float frameTime, ref Random random, ref ComponentLookup<PointOfInterest> pointOfInterests, ref ComponentLookup<Curve> curveDatas, ref ComponentLookup<PrefabRef> prefabRefDatas, ref ComponentLookup<UtilityLaneData> prefabUtilityLaneDatas, ref ComponentLookup<ObjectGeometryData> prefabObjectGeometryDatas, ref NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_099c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_105d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1001: Unknown result type (might be due to invalid IL or missing references)
		//IL_1003: Unknown result type (might be due to invalid IL or missing references)
		//IL_1015: Unknown result type (might be due to invalid IL or missing references)
		//IL_101a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1033: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1080: Unknown result type (might be due to invalid IL or missing references)
		//IL_1085: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_1088: Unknown result type (might be due to invalid IL or missing references)
		//IL_10aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_110c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1125: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		ProceduralBone proceduralBone = proceduralBones[index];
		Momentum momentum = default(Momentum);
		int num = skeleton.m_BoneOffset + index;
		ref Bone reference = ref bones.ElementAt(num);
		ref Momentum momentum2 = ref momentum;
		if (momentums.IsCreated)
		{
			momentum2 = ref momentums.ElementAt(num);
		}
		float3 val5;
		switch (proceduralBone.m_Type)
		{
		case BoneType.RollingTire:
		{
			float3 val16 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val17 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val16;
			float3 val18 = math.forward(newTransform.m_Rotation);
			float num11 = math.dot(val17, val18) / math.max(0.01f, proceduralBone.m_ObjectPosition.y);
			val5 = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation));
			float2 yz = ((float3)(ref val5)).yz;
			float num12 = num11 - math.atan2(yz.x, yz.y);
			float3 val19 = math.mul(swayRotation, proceduralBone.m_Position);
			val19.y += swayOffset;
			quaternion val20 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num12));
			skeleton.m_CurrentUpdated |= !((float3)(ref reference.m_Position)).Equals(val19) | !((quaternion)(ref reference.m_Rotation)).Equals(val20);
			reference.m_Position = val19;
			reference.m_Rotation = val20;
			break;
		}
		case BoneType.SteeringTire:
		{
			float3 val50 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val51 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val50;
			float3 val52 = math.mul(newTransform.m_Rotation, math.right());
			float3 val53 = math.forward(newTransform.m_Rotation);
			float num28 = math.dot(val51, val53);
			float num29 = math.dot(val51, val52);
			num28 += math.select(0.001f, -0.001f, num28 < 0f);
			float3 val54 = val53 * num28 + val52 * num29;
			val5 = default(float3);
			float3 val55 = math.normalizesafe(val54, val5);
			val55 = math.select(val55, -val55, num28 < 0f);
			float num30 = math.dot(val51, val55) / math.max(0.01f, proceduralBone.m_ObjectPosition.y);
			quaternion val56 = math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation);
			float3 val57 = math.forward(val56);
			float num31 = math.length(((float3)(ref val57)).xz);
			num31 = math.select(num31, 0f - num31, val57.z < 0f);
			float num32 = num30 - math.atan2(val57.y, num31);
			float num34;
			if (steeringRadius == 0f)
			{
				float num33 = math.asin(math.dot(val52, val55));
				num34 = math.asin(math.mul(val56, math.left()).z);
				float num35 = math.length(val51) / math.max(0.01f, proceduralBone.m_ObjectPosition.y);
				num34 += math.clamp(num33 - num34, 0f - num35, num35);
			}
			else
			{
				num34 = math.atan((proceduralBone.m_ObjectPosition.z - pivotOffset) / (steeringRadius - proceduralBone.m_ObjectPosition.x));
			}
			float3 val58 = math.mul(swayRotation, proceduralBone.m_Position);
			val58.y += swayOffset;
			quaternion val59 = math.mul(proceduralBone.m_Rotation, math.mul(quaternion.RotateY(num34), quaternion.RotateX(num32)));
			skeleton.m_CurrentUpdated |= !((float3)(ref reference.m_Position)).Equals(val58) | !((quaternion)(ref reference.m_Rotation)).Equals(val59);
			reference.m_Position = val58;
			reference.m_Rotation = val59;
			break;
		}
		case BoneType.SuspensionMovement:
		{
			if (FindChildBone(proceduralBones, index, out var childIndex3))
			{
				ProceduralBone proceduralBone3 = proceduralBones[childIndex3];
				float3 position5 = proceduralBone.m_Position;
				position5.z += math.mul(swayRotation, proceduralBone3.m_ObjectPosition).y - proceduralBone3.m_ObjectPosition.y;
				position5.z += swayOffset;
				skeleton.m_CurrentUpdated |= !((float3)(ref reference.m_Position)).Equals(position5);
				reference.m_Position = position5;
			}
			break;
		}
		case BoneType.SteeringRotation:
		{
			if (FindChildBone(proceduralBones, index, out var childIndex4))
			{
				ProceduralBone proceduralBone4 = proceduralBones[childIndex4];
				if (FindChildBone(proceduralBones, childIndex4, out var childIndex5))
				{
					proceduralBone4 = proceduralBones[childIndex5];
				}
				float num25;
				if (steeringRadius == 0f)
				{
					float3 val42 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone4.m_ObjectPosition);
					float3 val43 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone4.m_ObjectPosition) - val42;
					float3 val44 = math.mul(newTransform.m_Rotation, math.right());
					float3 val45 = math.forward(newTransform.m_Rotation);
					float num22 = math.dot(val43, val45);
					float num23 = math.dot(val43, val44);
					num22 += math.select(0.001f, -0.001f, num22 < 0f);
					float3 val46 = val45 * num22 + val44 * num23;
					val5 = default(float3);
					float3 val47 = math.normalizesafe(val46, val5);
					val47 = math.select(val47, -val47, num22 < 0f);
					float num24 = math.asin(math.dot(val44, val47));
					num25 = math.asin(math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.right()).y);
					float num26 = math.length(val43) / math.max(0.01f, proceduralBone4.m_ObjectPosition.y);
					num25 += math.clamp(num24 - num25, 0f - num26, num26);
				}
				else
				{
					num25 = math.atan((proceduralBone4.m_ObjectPosition.z - pivotOffset) / (steeringRadius - proceduralBone4.m_ObjectPosition.x));
				}
				quaternion val48 = math.mul(proceduralBone.m_Rotation, quaternion.RotateZ(num25));
				skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val48);
				reference.m_Rotation = val48;
			}
			break;
		}
		case BoneType.SuspensionRotation:
		{
			if (FindChildBone(proceduralBones, index, out var childIndex6))
			{
				ProceduralBone proceduralBone5 = proceduralBones[childIndex6];
				float num27 = 0f - math.atan((math.mul(swayRotation, proceduralBone5.m_ObjectPosition).y - proceduralBone5.m_ObjectPosition.y + swayOffset) / proceduralBone5.m_Position.z);
				quaternion val49 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num27));
				skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val49);
				reference.m_Rotation = val49;
			}
			break;
		}
		case BoneType.FixedRotation:
		{
			ProceduralBone proceduralBone6 = proceduralBones[proceduralBone.m_ParentIndex];
			Bone bone = bones.ElementAt(skeleton.m_BoneOffset + proceduralBone.m_ParentIndex);
			quaternion val60 = math.mul(math.inverse(LocalToObject(proceduralBones, bones, skeleton, proceduralBone6.m_ParentIndex, bone.m_Rotation)), proceduralBone.m_ObjectRotation);
			skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val60);
			reference.m_Rotation = val60;
			break;
		}
		case BoneType.FixedTire:
		{
			float3 val35 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val36 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val35;
			float3 val37 = math.rotate(LocalToWorld(proceduralBones, bones, newTransform.ToTransform(), skeleton, proceduralBone.m_ParentIndex, proceduralBone.m_Rotation), math.right());
			float3 val38 = math.rotate(newTransform.m_Rotation, math.up());
			float3 val39 = math.cross(val37, val38);
			val5 = default(float3);
			float3 val40 = math.normalizesafe(val39, val5);
			float num20 = math.dot(val36, val40) / math.max(0.01f, proceduralBone.m_ObjectPosition.y);
			val5 = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation));
			float2 yz3 = ((float3)(ref val5)).yz;
			float num21 = num20 - math.atan2(yz3.x, yz3.y);
			quaternion val41 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num21));
			skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val41);
			reference.m_Rotation = val41;
			break;
		}
		case BoneType.DebugMovement:
		{
			float3 position3 = proceduralBone.m_Position;
			float num10 = ((float)(frameIndex & 0xFF) + frameTime) * (3f / 128f);
			if (num10 < 1f)
			{
				position3.x += math.smoothstep(0f, 1f, num10);
			}
			else if (num10 < 2f)
			{
				position3.x += math.smoothstep(2f, 1f, num10);
			}
			else if (num10 < 3f)
			{
				position3.y += math.smoothstep(2f, 3f, num10);
			}
			else if (num10 < 4f)
			{
				position3.y += math.smoothstep(4f, 3f, num10);
			}
			else if (num10 < 5f)
			{
				position3.z += math.smoothstep(4f, 5f, num10);
			}
			else
			{
				position3.z += math.smoothstep(6f, 5f, num10);
			}
			skeleton.m_CurrentUpdated |= !((float3)(ref reference.m_Position)).Equals(position3);
			reference.m_Position = position3;
			break;
		}
		case BoneType.RollingRotation:
		{
			float3 val21 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val22 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val21;
			float3 val23 = math.rotate(LocalToWorld(proceduralBones, bones, newTransform.ToTransform(), skeleton, proceduralBone.m_ParentIndex, proceduralBone.m_Rotation), math.right());
			float3 val24 = math.rotate(newTransform.m_Rotation, math.up());
			float3 val25 = math.cross(val23, val24);
			val5 = default(float3);
			float3 val26 = math.normalizesafe(val25, val5);
			float num13 = math.dot(val22, val26) * proceduralBone.m_Speed;
			val5 = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation));
			float2 yz2 = ((float3)(ref val5)).yz;
			float num14 = num13 - math.atan2(yz2.x, yz2.y);
			quaternion val27 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num14));
			skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val27);
			reference.m_Rotation = val27;
			break;
		}
		case BoneType.PropellerRotation:
		{
			float3 val2 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val3 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val2;
			float3 val4 = math.rotate(LocalToWorld(proceduralBones, bones, newTransform.ToTransform(), skeleton, proceduralBone.m_ParentIndex, proceduralBone.m_Rotation), math.up());
			float num2 = math.dot(val3, val4) * proceduralBone.m_Speed;
			val5 = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation));
			float2 xz = ((float3)(ref val5)).xz;
			float num3 = num2 + math.atan2(xz.x, xz.y);
			quaternion val6 = math.mul(proceduralBone.m_Rotation, quaternion.RotateY(num3));
			skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val6);
			reference.m_Rotation = val6;
			break;
		}
		case BoneType.PoweredRotation:
		case BoneType.OperatingRotation:
		{
			float speed = proceduralBone.m_Speed;
			AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref momentum2, ref random, speed, deltaTime, instantReset);
			break;
		}
		case BoneType.PropellerAngle:
		{
			float3 val7 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone.m_ObjectPosition);
			float3 val8 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone.m_ObjectPosition) - val7;
			float3 val9 = math.mul(newTransform.m_Rotation, math.right());
			float3 val10 = math.forward(newTransform.m_Rotation);
			float num4 = math.dot(val8, val10);
			float num5 = math.dot(val8, val9);
			num4 += math.select(0.001f, -0.001f, num4 < 0f);
			float3 val11 = val10 * num4 + val9 * num5;
			val5 = default(float3);
			float3 val12 = math.normalizesafe(val11, val5);
			float num6 = math.atan2(math.dot(val9, val12), math.dot(val10, val12));
			float3 val13 = math.mul(reference.m_Rotation, math.forward());
			float num7 = math.atan2(val13.x, val13.z);
			float num8 = math.length(val8) * proceduralBone.m_Speed;
			float num9 = num6 - num7;
			num9 = math.select(num9, num9 - (float)Math.PI, num9 > (float)Math.PI);
			num9 = math.select(num9, num9 + (float)Math.PI, num9 < -(float)Math.PI);
			num7 += math.clamp(num9, 0f - num8, num8);
			quaternion val14 = math.mul(quaternion.RotateY(num7), proceduralBone.m_Rotation);
			skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val14);
			reference.m_Rotation = val14;
			break;
		}
		case BoneType.PantographRotation:
		{
			bool active = (newTransform.m_Flags & TransformFlags.Pantograph) != 0;
			AnimatePantographBone(proceduralBones, bones, newTransform.ToTransform(), prefabRef, proceduralBone, ref skeleton, ref reference, index, deltaTime, active, instantReset, ref curveDatas, ref prefabRefDatas, ref prefabUtilityLaneDatas, ref prefabObjectGeometryDatas, ref laneSearchTree);
			break;
		}
		case BoneType.SteeringSuspension:
		{
			if (FindChildBone(proceduralBones, index, out var childIndex))
			{
				ProceduralBone proceduralBone2 = proceduralBones[childIndex];
				if (FindChildBone(proceduralBones, childIndex, out var childIndex2))
				{
					proceduralBone2 = proceduralBones[childIndex2];
				}
				float num18;
				if (steeringRadius == 0f)
				{
					float3 val28 = ObjectUtils.LocalToWorld(oldTransform.ToTransform(), proceduralBone2.m_ObjectPosition);
					float3 val29 = ObjectUtils.LocalToWorld(newTransform.ToTransform(), proceduralBone2.m_ObjectPosition) - val28;
					float3 val30 = math.mul(newTransform.m_Rotation, math.right());
					float3 val31 = math.forward(newTransform.m_Rotation);
					float num15 = math.dot(val29, val31);
					float num16 = math.dot(val29, val30);
					num15 += math.select(0.001f, -0.001f, num15 < 0f);
					float3 val32 = val31 * num15 + val30 * num16;
					val5 = default(float3);
					float3 val33 = math.normalizesafe(val32, val5);
					val33 = math.select(val33, -val33, num15 < 0f);
					float num17 = math.asin(math.dot(val30, val33));
					num18 = math.asin(math.mul(math.mul(math.inverse(proceduralBone.m_Rotation), reference.m_Rotation), math.left()).z);
					float num19 = math.length(val29) / math.max(0.01f, proceduralBone2.m_ObjectPosition.y);
					num18 += math.clamp(num17 - num18, 0f - num19, num19);
				}
				else
				{
					num18 = math.atan((proceduralBone2.m_ObjectPosition.z - pivotOffset) / (steeringRadius - proceduralBone2.m_ObjectPosition.x));
				}
				quaternion val34 = math.mul(proceduralBone.m_Rotation, quaternion.RotateY(num18));
				float3 position4 = proceduralBone.m_Position;
				position4.z += math.mul(swayRotation, proceduralBone2.m_ObjectPosition).y - proceduralBone2.m_ObjectPosition.y;
				position4.z += swayOffset;
				skeleton.m_CurrentUpdated |= !((quaternion)(ref reference.m_Rotation)).Equals(val34) | !((float3)(ref reference.m_Position)).Equals(position4);
				reference.m_Rotation = val34;
				reference.m_Position = position4;
			}
			break;
		}
		case BoneType.LookAtRotation:
		case BoneType.LookAtRotationSide:
		{
			PointOfInterest pointOfInterest2 = default(PointOfInterest);
			if (pointOfInterests.TryGetComponent(entity, ref pointOfInterest2) && pointOfInterest2.m_IsValid)
			{
				float3 position2 = proceduralBone.m_Position;
				quaternion rotation2 = proceduralBone.m_Rotation;
				LocalToWorld(proceduralBones, bones, newTransform.ToTransform(), skeleton, proceduralBone.m_ParentIndex, ref position2, ref rotation2);
				float3 val15 = pointOfInterest2.m_Position - position2;
				val15 = math.mul(math.inverse(rotation2), val15);
				((float3)(ref val15)).xz = math.select(((float3)(ref val15)).xz, MathUtils.Right(((float3)(ref val15)).xz), proceduralBone.m_Type == BoneType.LookAtRotationSide);
				val15 = math.select(val15, -val15, proceduralBone.m_Speed < 0f);
				float targetSpeed2 = math.abs(proceduralBone.m_Speed);
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref momentum2, ref random, ((float3)(ref val15)).xz, targetSpeed2, deltaTime, instantReset);
			}
			else
			{
				AnimateRotatingBoneY(proceduralBone, ref skeleton, ref reference, ref momentum2, ref random, 0f, deltaTime, instantReset);
			}
			break;
		}
		case BoneType.LookAtAim:
		case BoneType.LookAtAimForward:
		{
			PointOfInterest pointOfInterest = default(PointOfInterest);
			if (pointOfInterests.TryGetComponent(entity, ref pointOfInterest) && pointOfInterest.m_IsValid)
			{
				float3 position = proceduralBone.m_Position;
				quaternion rotation = proceduralBone.m_Rotation;
				LookAtLocalToWorld(proceduralBones, bones, newTransform.ToTransform(), skeleton, pointOfInterest, proceduralBone.m_ParentIndex, ref position, ref rotation);
				float3 val = pointOfInterest.m_Position - position;
				val = math.mul(math.inverse(rotation), val);
				((float3)(ref val)).yz = math.select(((float3)(ref val)).yz, MathUtils.Left(((float3)(ref val)).yz), proceduralBone.m_Type == BoneType.LookAtAimForward);
				val = math.select(val, -val, proceduralBone.m_Speed < 0f);
				float targetSpeed = math.abs(proceduralBone.m_Speed);
				AnimateRotatingBoneX(proceduralBone, ref skeleton, ref reference, ref momentum2, ref random, ((float3)(ref val)).yz, targetSpeed, deltaTime, instantReset);
			}
			else
			{
				AnimateRotatingBoneX(proceduralBone, ref skeleton, ref reference, ref momentum2, ref random, 0f, deltaTime, instantReset);
			}
			break;
		}
		case BoneType.TrafficBarrierDirection:
		case BoneType.VehicleConnection:
		case BoneType.TrainBogie:
		case BoneType.LengthwiseLookAtRotation:
		case BoneType.WorkingRotation:
		case BoneType.TimeRotation:
		case BoneType.LookAtMovementX:
		case BoneType.LookAtMovementY:
		case BoneType.LookAtMovementZ:
		case BoneType.RotationXFromMovementY:
		case BoneType.ScaledMovement:
			break;
		}
	}

	private static void AnimatePantographBone(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform transform, PrefabRef prefabRef, ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, int index, float deltaTime, bool active, bool instantReset, ref ComponentLookup<Curve> curveDatas, ref ComponentLookup<PrefabRef> prefabRefDatas, ref ComponentLookup<UtilityLaneData> prefabUtilityLaneDatas, ref ComponentLookup<ObjectGeometryData> prefabObjectGeometryDatas, ref NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		ProceduralBone proceduralBone2 = proceduralBones[proceduralBone.m_ParentIndex];
		quaternion val2;
		int childIndex;
		if (proceduralBone2.m_Type == BoneType.PantographRotation)
		{
			Bone bone2 = bones.ElementAt(skeleton.m_BoneOffset + proceduralBone.m_ParentIndex);
			quaternion val = math.mul(math.inverse(proceduralBone2.m_Rotation), bone2.m_Rotation);
			val.value.x = 0f - val.value.x;
			val2 = math.mul(math.mul(val, val), proceduralBone.m_Rotation);
		}
		else if (FindChildBone(proceduralBones, index, out childIndex))
		{
			float num = 0f;
			if (active)
			{
				ProceduralBone proceduralBone3 = proceduralBones[childIndex];
				ObjectGeometryData objectGeometryData = prefabObjectGeometryDatas[prefabRef.m_Prefab];
				float3 objectPosition = proceduralBone.m_ObjectPosition;
				objectPosition.y = objectGeometryData.m_Bounds.max.y;
				objectPosition = ObjectUtils.LocalToWorld(transform, objectPosition);
				float num2 = math.length(((float3)(ref proceduralBone3.m_Position)).yz);
				if (proceduralBone3.m_Type == BoneType.PantographRotation && FindChildBone(proceduralBones, childIndex, out var childIndex2))
				{
					ProceduralBone proceduralBone4 = proceduralBones[childIndex2];
					num2 += math.length(((float3)(ref proceduralBone4.m_Position)).yz);
				}
				float defaultHeight = num2 * 0.38268343f;
				float num3 = FindCatenaryHeight(objectPosition, transform.m_Rotation, defaultHeight, ref curveDatas, ref prefabRefDatas, ref prefabUtilityLaneDatas, ref laneSearchTree);
				num = math.asin(math.min(0.9f, num3 / math.max(num2, 0.001f)));
				num = math.select(num, 0f - num, proceduralBone3.m_Position.z > 0f);
			}
			float3 val3 = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation));
			float2 yz = ((float3)(ref val3)).yz;
			float num4 = 0f - math.atan2(yz.x, yz.y);
			float num5 = proceduralBone.m_Speed * deltaTime;
			num = math.select(math.clamp(num, num4 - num5, num4 + num5), num, instantReset);
			val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num));
		}
		else
		{
			val2 = bone.m_Rotation;
		}
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static float FindCatenaryHeight(float3 position, quaternion rotation, float defaultHeight, ref ComponentLookup<Curve> curveDatas, ref ComponentLookup<PrefabRef> prefabRefDatas, ref ComponentLookup<UtilityLaneData> prefabUtilityLaneDatas, ref NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(position, position + math.mul(rotation, new float3(0f, defaultHeight * 2f, 0f)));
		float3 val2 = MathUtils.Position(val, 0.5f);
		CatenaryIterator catenaryIterator = new CatenaryIterator
		{
			m_Bounds = new Bounds3(val2 - defaultHeight, val2 + defaultHeight),
			m_Line = val,
			m_Result = float3.op_Implicit(1000f),
			m_Default = defaultHeight,
			m_CurveData = curveDatas,
			m_PrefabRefData = prefabRefDatas,
			m_PrefabUtilityLaneData = prefabUtilityLaneDatas
		};
		laneSearchTree.Iterate<CatenaryIterator>(ref catenaryIterator, 0);
		curveDatas = catenaryIterator.m_CurveData;
		prefabRefDatas = catenaryIterator.m_PrefabRefData;
		prefabUtilityLaneDatas = catenaryIterator.m_PrefabUtilityLaneData;
		return math.lerp(catenaryIterator.m_Result.x, defaultHeight, math.min(1f, catenaryIterator.m_Result.y / (defaultHeight * 0.5f)));
	}

	private static bool FindChildBone(DynamicBuffer<ProceduralBone> proceduralBones, int index, out int childIndex)
	{
		for (int i = 0; i < proceduralBones.Length; i++)
		{
			if (proceduralBones[i].m_ParentIndex == index)
			{
				childIndex = i;
				return true;
			}
		}
		childIndex = -1;
		return false;
	}

	private static void AnimateMovingBone(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, float3 moveDirection, float targetOffset, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		float3 position = proceduralBone.m_Position;
		if (instantReset)
		{
			position += moveDirection * targetOffset;
			momentum.m_Momentum = 0f;
		}
		else
		{
			float num = math.dot(bone.m_Position - position, moveDirection);
			float num2 = targetOffset - num;
			targetSpeed = math.select(targetSpeed, 0f - targetSpeed, num2 < 0f);
			float num3 = math.sqrt(math.abs(num2 * proceduralBone.m_Acceleration));
			targetSpeed = math.clamp(targetSpeed, 0f - num3, num3);
			float num4 = targetSpeed - momentum.m_Momentum;
			float num5 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num4, 0f - num5, num5);
			position += moveDirection * (num + momentum.m_Momentum * deltaTime);
		}
		skeleton.m_CurrentUpdated |= !((float3)(ref bone.m_Position)).Equals(position);
		bone.m_Position = position;
	}

	private static void AnimateRotatingBoneY(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float2 targetDir, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		float3 val;
		float num;
		if (instantReset)
		{
			if (MathUtils.TryNormalize(ref targetDir))
			{
				val = math.forward();
				num = MathUtils.RotationAngleSignedRight(((float3)(ref val)).xz, targetDir);
			}
			else
			{
				num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
			}
			momentum.m_Momentum = 0f;
		}
		else
		{
			val = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation));
			float2 xz = ((float3)(ref val)).xz;
			if (MathUtils.TryNormalize(ref xz) && MathUtils.TryNormalize(ref targetDir))
			{
				float num2 = MathUtils.RotationAngleSignedRight(xz, targetDir);
				targetSpeed = math.select(targetSpeed, 0f - targetSpeed, num2 < 0f);
				float num3 = math.sqrt(math.abs(num2 * proceduralBone.m_Acceleration));
				targetSpeed = math.clamp(targetSpeed, 0f - num3, num3);
			}
			else
			{
				targetSpeed = 0f;
			}
			float num4 = targetSpeed - momentum.m_Momentum;
			float num5 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num4, 0f - num5, num5);
			num = math.atan2(xz.x, xz.y) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateY(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateRotatingBoneZ(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float2 targetDir, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		float3 val;
		float num;
		if (instantReset)
		{
			if (MathUtils.TryNormalize(ref targetDir))
			{
				val = math.up();
				num = MathUtils.RotationAngleSignedLeft(((float3)(ref val)).xy, targetDir);
			}
			else
			{
				num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
			}
			momentum.m_Momentum = 0f;
		}
		else
		{
			val = math.rotate(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation), math.up());
			float2 xy = ((float3)(ref val)).xy;
			if (MathUtils.TryNormalize(ref xy) && MathUtils.TryNormalize(ref targetDir))
			{
				float num2 = MathUtils.RotationAngleSignedLeft(xy, targetDir);
				targetSpeed = math.select(targetSpeed, 0f - targetSpeed, num2 < 0f);
				float num3 = math.sqrt(math.abs(num2 * proceduralBone.m_Acceleration));
				targetSpeed = math.clamp(targetSpeed, 0f - num3, num3);
			}
			else
			{
				targetSpeed = 0f;
			}
			float num4 = targetSpeed - momentum.m_Momentum;
			float num5 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num4, 0f - num5, num5);
			num = math.atan2(0f - xy.x, xy.y) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateZ(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateRotatingBoneX(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float2 targetDir, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		float3 val;
		float num;
		if (instantReset)
		{
			if (MathUtils.TryNormalize(ref targetDir))
			{
				val = math.up();
				num = MathUtils.RotationAngleSignedLeft(((float3)(ref val)).yz, targetDir);
			}
			else
			{
				num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
			}
			momentum.m_Momentum = 0f;
		}
		else
		{
			val = math.rotate(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation), math.up());
			float2 yz = ((float3)(ref val)).yz;
			if (MathUtils.TryNormalize(ref yz) && MathUtils.TryNormalize(ref targetDir))
			{
				float num2 = MathUtils.RotationAngleSignedLeft(yz, targetDir);
				targetSpeed = math.select(targetSpeed, 0f - targetSpeed, num2 < 0f);
				float num3 = math.sqrt(math.abs(num2 * proceduralBone.m_Acceleration));
				targetSpeed = math.clamp(targetSpeed, 0f - num3, num3);
			}
			else
			{
				targetSpeed = 0f;
			}
			float num4 = targetSpeed - momentum.m_Momentum;
			float num5 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num4, 0f - num5, num5);
			num = math.atan2(yz.y, yz.x) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateRotatingBoneY(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		float num;
		if (instantReset)
		{
			momentum.m_Momentum = targetSpeed;
			num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
		}
		else
		{
			float num2 = targetSpeed - momentum.m_Momentum;
			float num3 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num2, 0f - num3, num3);
			float3 val = math.forward(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation));
			float2 xz = ((float3)(ref val)).xz;
			num = math.atan2(xz.x, xz.y) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateY(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateRotatingBoneZ(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		float num;
		if (instantReset)
		{
			momentum.m_Momentum = targetSpeed;
			num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
		}
		else
		{
			float num2 = targetSpeed - momentum.m_Momentum;
			float num3 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num2, 0f - num3, num3);
			float3 val = math.rotate(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation), math.up());
			float2 xy = ((float3)(ref val)).xy;
			num = math.atan2(0f - xy.x, xy.y) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateZ(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateRotatingBoneX(ProceduralBone proceduralBone, ref Skeleton skeleton, ref Bone bone, ref Momentum momentum, ref Random random, float targetSpeed, float deltaTime, bool instantReset)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		float num;
		if (instantReset)
		{
			momentum.m_Momentum = targetSpeed;
			num = ((Random)(ref random)).NextFloat(-(float)Math.PI, (float)Math.PI);
		}
		else
		{
			float num2 = targetSpeed - momentum.m_Momentum;
			float num3 = math.abs(deltaTime * proceduralBone.m_Acceleration);
			momentum.m_Momentum += math.clamp(num2, 0f - num3, num3);
			float3 val = math.rotate(math.mul(math.inverse(proceduralBone.m_Rotation), bone.m_Rotation), math.up());
			float2 yz = ((float3)(ref val)).yz;
			num = math.atan2(yz.y, yz.x) + momentum.m_Momentum * deltaTime;
		}
		quaternion val2 = math.mul(proceduralBone.m_Rotation, quaternion.RotateX(num));
		skeleton.m_CurrentUpdated |= !((quaternion)(ref bone.m_Rotation)).Equals(val2);
		bone.m_Rotation = val2;
	}

	private static void AnimateInterpolatedLight(DynamicBuffer<ProceduralLight> proceduralLights, DynamicBuffer<LightAnimation> lightAnimations, DynamicBuffer<LightState> lights, TransformFlags transformFlags, Random pseudoRandom, ref Emissive emissive, int index, uint frame, float frameTime, float deltaTime, bool instantReset)
	{
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		ProceduralLight proceduralLight = proceduralLights[index];
		int num = emissive.m_LightOffset + index;
		ref LightState light = ref lights.ElementAt(num);
		switch (proceduralLight.m_Purpose)
		{
		case EmissiveProperties.Purpose.DaytimeRunningLight:
		case EmissiveProperties.Purpose.DaytimeRunningLightAlt:
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, 1f, instantReset);
			break;
		case EmissiveProperties.Purpose.RearLight:
		{
			float targetIntensity5 = math.select(0f, 1f, (transformFlags & TransformFlags.RearLights) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity5, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.Headlight_LowBeam:
		case EmissiveProperties.Purpose.TaxiLights:
		case EmissiveProperties.Purpose.SearchLightsFront:
		{
			float targetIntensity13 = math.select(0f, 1f, (transformFlags & TransformFlags.MainLights) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity13, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.Headlight_HighBeam:
		case EmissiveProperties.Purpose.LandingLights:
		{
			float targetIntensity4 = math.select(0f, 1f, (transformFlags & TransformFlags.ExtraLights) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity4, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.TurnSignalLeft:
		{
			float targetIntensity10 = 0f;
			if ((transformFlags & TransformFlags.TurningLeft) != 0)
			{
				targetIntensity10 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity10, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.TurnSignalRight:
		{
			float targetIntensity14 = 0f;
			if ((transformFlags & TransformFlags.TurningRight) != 0)
			{
				targetIntensity14 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity14, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.BrakeLight:
		{
			float targetIntensity8 = math.select(0f, 1f, (transformFlags & TransformFlags.Braking) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity8, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.DaytimeRunningLightLeft:
		{
			float targetIntensity7 = math.select(1f, 0f, (transformFlags & TransformFlags.TurningLeft) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity7, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.DaytimeRunningLightRight:
		{
			float targetIntensity9 = math.select(1f, 0f, (transformFlags & TransformFlags.TurningRight) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity9, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.BrakeAndTurnSignalLeft:
		{
			float targetIntensity6 = math.select(0f, 1f, (transformFlags & TransformFlags.Braking) != 0);
			if ((transformFlags & TransformFlags.TurningLeft) != 0)
			{
				targetIntensity6 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity6, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.BrakeAndTurnSignalRight:
		{
			float targetIntensity16 = math.select(0f, 1f, (transformFlags & TransformFlags.Braking) != 0);
			if ((transformFlags & TransformFlags.TurningRight) != 0)
			{
				targetIntensity16 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity16, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.ReverseLight:
		{
			float targetIntensity15 = math.select(0f, 1f, (transformFlags & TransformFlags.Reversing) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity15, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.Emergency1:
		case EmissiveProperties.Purpose.Emergency2:
		case EmissiveProperties.Purpose.Emergency3:
		case EmissiveProperties.Purpose.Emergency4:
		case EmissiveProperties.Purpose.Emergency5:
		case EmissiveProperties.Purpose.Emergency6:
		case EmissiveProperties.Purpose.RearAlarmLights:
		case EmissiveProperties.Purpose.FrontAlarmLightsLeft:
		case EmissiveProperties.Purpose.FrontAlarmLightsRight:
		case EmissiveProperties.Purpose.Warning1:
		case EmissiveProperties.Purpose.Warning2:
		case EmissiveProperties.Purpose.Emergency7:
		case EmissiveProperties.Purpose.Emergency8:
		case EmissiveProperties.Purpose.Emergency9:
		case EmissiveProperties.Purpose.Emergency10:
		case EmissiveProperties.Purpose.AntiCollisionLightsRed:
		case EmissiveProperties.Purpose.AntiCollisionLightsWhite:
		{
			float targetIntensity12 = 0f;
			if ((transformFlags & TransformFlags.WarningLights) != 0)
			{
				targetIntensity12 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity12, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.CollectionLights:
		case EmissiveProperties.Purpose.TaxiSign:
		case EmissiveProperties.Purpose.WorkLights:
		case EmissiveProperties.Purpose.SearchLights360:
		{
			float targetIntensity11 = math.select(0f, 1f, (transformFlags & TransformFlags.WorkLights) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity11, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.SignalGroup1:
		case EmissiveProperties.Purpose.SignalGroup2:
		case EmissiveProperties.Purpose.SignalGroup3:
		case EmissiveProperties.Purpose.SignalGroup4:
		case EmissiveProperties.Purpose.SignalGroup5:
		case EmissiveProperties.Purpose.SignalGroup6:
		case EmissiveProperties.Purpose.SignalGroup7:
		case EmissiveProperties.Purpose.SignalGroup8:
		case EmissiveProperties.Purpose.SignalGroup9:
		case EmissiveProperties.Purpose.SignalGroup10:
		case EmissiveProperties.Purpose.SignalGroup11:
		{
			int num4 = (int)(proceduralLight.m_Purpose - 12);
			SignalGroupMask signalGroupMask = (SignalGroupMask)(1 << num4);
			float targetIntensity3 = 0f;
			if ((transformFlags & (TransformFlags.SignalAnimation1 | TransformFlags.SignalAnimation2)) != 0)
			{
				int num5 = 0;
				num5 |= (((transformFlags & TransformFlags.SignalAnimation1) != 0) ? 1 : 0);
				num5 |= (((transformFlags & TransformFlags.SignalAnimation2) != 0) ? 2 : 0);
				num5--;
				targetIntensity3 = AnimateIntensity(signalGroupMask, num5, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			}
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity3, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.NeonSign:
		case EmissiveProperties.Purpose.DecorativeLight:
		{
			float targetIntensity2 = AnimateIntensity(proceduralLight, lightAnimations, pseudoRandom, frame, frameTime, 1f);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity2, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.BoardingLightLeft:
		{
			float num3 = math.select(1f, 0f, (transformFlags & TransformFlags.BoardingLeft) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, new float2(1f, num3), instantReset);
			break;
		}
		case EmissiveProperties.Purpose.BoardingLightRight:
		{
			float num2 = math.select(1f, 0f, (transformFlags & TransformFlags.BoardingRight) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, new float2(1f, num2), instantReset);
			break;
		}
		case EmissiveProperties.Purpose.Interior1:
		case EmissiveProperties.Purpose.Interior2:
		{
			float targetIntensity = math.select(0f, 0.003f, (transformFlags & TransformFlags.InteriorLights) != 0);
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, targetIntensity, instantReset);
			break;
		}
		case EmissiveProperties.Purpose.Clearance:
		case EmissiveProperties.Purpose.Dashboard:
		case EmissiveProperties.Purpose.Clearance2:
		case EmissiveProperties.Purpose.MarkerLights:
		case EmissiveProperties.Purpose.WingInspectionLights:
		case EmissiveProperties.Purpose.LogoLights:
		case EmissiveProperties.Purpose.PositionLightLeft:
		case EmissiveProperties.Purpose.PositionLightRight:
		case EmissiveProperties.Purpose.PositionLights:
		case EmissiveProperties.Purpose.NumberLight:
			AnimateLight(proceduralLight, ref emissive, ref light, deltaTime, 1f, instantReset);
			break;
		case EmissiveProperties.Purpose.TrafficLight_Red:
		case EmissiveProperties.Purpose.TrafficLight_Yellow:
		case EmissiveProperties.Purpose.TrafficLight_Green:
		case EmissiveProperties.Purpose.PedestrianLight_Stop:
		case EmissiveProperties.Purpose.PedestrianLight_Walk:
		case EmissiveProperties.Purpose.RailCrossing_Stop:
			break;
		}
	}

	private static float AnimateIntensity(ProceduralLight proceduralLight, DynamicBuffer<LightAnimation> lightAnimations, Random pseudoRandom, uint frame, float frameTime, float intensity)
	{
		if (proceduralLight.m_AnimationIndex >= 0 && lightAnimations.IsCreated)
		{
			LightAnimation lightAnimation = lightAnimations[proceduralLight.m_AnimationIndex];
			float num = (float)((frame + ((Random)(ref pseudoRandom)).NextUInt(lightAnimation.m_DurationFrames)) % lightAnimation.m_DurationFrames) + frameTime;
			intensity *= ((AnimationCurve1)(ref lightAnimation.m_AnimationCurve)).Evaluate(num / (float)lightAnimation.m_DurationFrames);
		}
		return intensity;
	}

	private static float AnimateIntensity(SignalGroupMask signalGroupMask, int signalAnimationIndex, DynamicBuffer<LightAnimation> lightAnimations, Random pseudoRandom, uint frame, float frameTime, float intensity)
	{
		if (signalAnimationIndex >= 0 && lightAnimations.IsCreated)
		{
			LightAnimation lightAnimation = lightAnimations[signalAnimationIndex];
			float num = (float)((frame + ((Random)(ref pseudoRandom)).NextUInt(lightAnimation.m_DurationFrames)) % lightAnimation.m_DurationFrames) + frameTime;
			intensity *= lightAnimation.m_SignalAnimation.Evaluate(signalGroupMask, num / (float)lightAnimation.m_DurationFrames);
		}
		return intensity;
	}

	public static void AnimateLight(ProceduralLight proceduralLight, ref Emissive emissive, ref LightState light, float deltaTime, float targetIntensity, bool instantReset)
	{
		float num = math.abs(deltaTime) * proceduralLight.m_ResponseSpeed;
		float num2 = math.select(math.clamp(targetIntensity, light.m_Intensity - num, light.m_Intensity + num), targetIntensity, instantReset);
		emissive.m_Updated |= light.m_Intensity != num2;
		light.m_Intensity = num2;
	}

	public static void AnimateLight(ProceduralLight proceduralLight, ref Emissive emissive, ref LightState light, float deltaTime, float2 target, bool instantReset)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		((float2)(ref val))._002Ector(light.m_Intensity, light.m_Color);
		float num = math.abs(deltaTime) * proceduralLight.m_ResponseSpeed;
		float2 val2 = math.select(math.clamp(target, val - num, val + num), target, instantReset);
		emissive.m_Updated |= math.any(val2 != val);
		light.m_Intensity = val2.x;
		light.m_Color = val2.y;
	}

	public static void UpdateInterpolatedAnimation(DynamicBuffer<AnimationClip> clips, InterpolatedTransform oldTransform, InterpolatedTransform newTransform, ref Animated animated, float stateTimer, TransformState state, ActivityType activity, float updateFrameToSeconds, float speedDeltaFactor)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		AnimationClip clip = clips[(int)animated.m_ClipIndexBody0];
		float3 val = math.forward(newTransform.m_Rotation);
		float num = math.dot(newTransform.m_Position - oldTransform.m_Position, val);
		GetClipType(clip, state, num, speedDeltaFactor, out var type, ref activity);
		if (clip.m_Type != type || clip.m_Activity != activity || clip.m_Layer != AnimationLayer.Body)
		{
			FindAnimationClip(clips, type, activity, AnimationLayer.Body, new AnimatedPropID(-1), (ActivityCondition)0u, out clip, out var index);
			animated.m_ClipIndexBody0 = (short)index;
			animated.m_Time.x = 0f;
		}
		animated.m_PreviousTime = animated.m_Time.x;
		if (clip.m_MovementSpeed != 0f)
		{
			animated.m_Time.x += num / clip.m_MovementSpeed;
		}
		else
		{
			animated.m_Time.x = stateTimer * updateFrameToSeconds;
		}
	}

	public static void UpdateInterpolatedAnimationBody(Entity entity, in CharacterElement characterElement, DynamicBuffer<AnimationClip> clips, ref ComponentLookup<Human> humanLookup, ref ComponentLookup<CurrentVehicle> currentVehicleLookup, ref ComponentLookup<PrefabRef> prefabRefLookup, ref BufferLookup<ActivityLocationElement> activityLocationLookup, ref BufferLookup<AnimationMotion> motionLookup, InterpolatedTransform oldTransform, InterpolatedTransform newTransform, ref Animated animated, ref Random random, TransformFrame frame0, TransformFrame frame1, float framePosition, float updateFrameToSeconds, float speedDeltaFactor, float deltaTime, int updateFrameChanged, bool instantReset)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.forward(newTransform.m_Rotation);
		float num = math.dot(newTransform.m_Position - oldTransform.m_Position, val);
		if (instantReset)
		{
			AnimationClip clip = clips[(int)animated.m_ClipIndexBody0];
			ActivityType activity = (ActivityType)frame1.m_Activity;
			GetClipType(clip, frame1.m_State, num, speedDeltaFactor, out var type, ref activity);
			AnimatedPropID propID = GetPropID(entity, activity, ref currentVehicleLookup, ref prefabRefLookup, ref activityLocationLookup);
			ActivityCondition activityConditions = GetActivityConditions(entity, ref humanLookup);
			FindAnimationClip(clips, type, activity, AnimationLayer.Body, propID, activityConditions, out clip, out var index);
			animated.m_ClipIndexBody0 = (short)index;
			animated.m_ClipIndexBody0I = -1;
			animated.m_ClipIndexBody1 = -1;
			animated.m_ClipIndexBody1I = -1;
			animated.m_MovementSpeed = new float2(GetMovementSpeed(in characterElement, in clip, ref motionLookup), 0f);
			((float4)(ref animated.m_Time)).xy = float2.op_Implicit(0f);
			if (animated.m_MovementSpeed.x != 0f || frame1.m_State == TransformState.Idle)
			{
				animated.m_Time.x = ((Random)(ref random)).NextFloat(clip.m_AnimationLength);
			}
		}
		else if (updateFrameChanged > 0)
		{
			AnimationClip clip2;
			if (animated.m_ClipIndexBody1 != -1)
			{
				clip2 = clips[(int)animated.m_ClipIndexBody1];
				animated.m_ClipIndexBody0 = animated.m_ClipIndexBody1;
				animated.m_ClipIndexBody0I = animated.m_ClipIndexBody1I;
				animated.m_Time.x = animated.m_Time.y;
				animated.m_MovementSpeed.x = animated.m_MovementSpeed.y;
			}
			else
			{
				clip2 = clips[(int)animated.m_ClipIndexBody0];
			}
			ActivityType activity2 = (ActivityType)frame1.m_Activity;
			GetClipType(clip2, frame1.m_State, num, speedDeltaFactor, out var type2, ref activity2);
			AnimatedPropID propID2 = GetPropID(entity, activity2, ref currentVehicleLookup, ref prefabRefLookup, ref activityLocationLookup);
			animated.m_ClipIndexBody1 = -1;
			animated.m_ClipIndexBody1I = -1;
			animated.m_MovementSpeed.y = 0f;
			animated.m_Time.y = 0f;
			if (clip2.m_Type != type2 || clip2.m_Activity != activity2 || clip2.m_PropID != propID2)
			{
				ActivityCondition activityConditions2 = GetActivityConditions(entity, ref humanLookup);
				float animationLength = clip2.m_AnimationLength;
				if (FindAnimationClip(clips, type2, activity2, AnimationLayer.Body, propID2, activityConditions2, out clip2, out var index2))
				{
					animated.m_ClipIndexBody1 = (short)index2;
					animated.m_ClipIndexBody1I = -1;
					animated.m_MovementSpeed.y = GetMovementSpeed(in characterElement, in clip2, ref motionLookup);
					animated.m_Time.y = GetInitialTime(ref random, in clip2, animated.m_MovementSpeed.y, animationLength, animated.m_MovementSpeed.x, animated.m_Time.x);
				}
			}
		}
		else if (updateFrameChanged < 0)
		{
			AnimationClip clip3 = clips[(int)animated.m_ClipIndexBody0];
			ActivityType activity3 = (ActivityType)frame0.m_Activity;
			GetClipType(clip3, frame0.m_State, num, speedDeltaFactor, out var type3, ref activity3);
			AnimatedPropID propID3 = GetPropID(entity, activity3, ref currentVehicleLookup, ref prefabRefLookup, ref activityLocationLookup);
			animated.m_ClipIndexBody1 = -1;
			animated.m_ClipIndexBody1I = -1;
			animated.m_Time.y = 0f;
			animated.m_MovementSpeed.y = 0f;
			if (clip3.m_Type != type3 || clip3.m_Activity != activity3 || clip3.m_PropID != propID3)
			{
				ActivityCondition activityConditions3 = GetActivityConditions(entity, ref humanLookup);
				float animationLength2 = clip3.m_AnimationLength;
				if (FindAnimationClip(clips, type3, activity3, AnimationLayer.Body, propID3, activityConditions3, out clip3, out var index3))
				{
					animated.m_ClipIndexBody1 = animated.m_ClipIndexBody0;
					animated.m_ClipIndexBody1I = animated.m_ClipIndexBody0I;
					animated.m_MovementSpeed.y = animated.m_MovementSpeed.x;
					animated.m_Time.y = animated.m_Time.x;
					animated.m_ClipIndexBody0 = (short)index3;
					animated.m_ClipIndexBody0I = -1;
					animated.m_MovementSpeed.x = GetMovementSpeed(in characterElement, in clip3, ref motionLookup);
					animated.m_Time.x = GetInitialTime(ref random, in clip3, animated.m_MovementSpeed.x, animationLength2, animated.m_MovementSpeed.y, animated.m_Time.y);
				}
			}
		}
		if (animated.m_ClipIndexBody1 != -1)
		{
			if (math.all(animated.m_MovementSpeed != 0f))
			{
				SynchronizeMovementTime(clips, ref animated, num, framePosition);
				return;
			}
			if (animated.m_MovementSpeed.y != 0f)
			{
				animated.m_Time.y += num / animated.m_MovementSpeed.y;
			}
			else if (clips[(int)animated.m_ClipIndexBody1].m_Type == Game.Prefabs.AnimationType.Idle)
			{
				animated.m_Time.y += deltaTime;
			}
			else
			{
				animated.m_Time.y = ((float)(int)frame1.m_StateTimer + framePosition - 1f) * updateFrameToSeconds;
			}
		}
		if (animated.m_MovementSpeed.x != 0f)
		{
			animated.m_Time.x += num / animated.m_MovementSpeed.x;
		}
		else if (clips[(int)animated.m_ClipIndexBody0].m_Type == Game.Prefabs.AnimationType.Idle)
		{
			animated.m_Time.x += deltaTime;
		}
		else
		{
			animated.m_Time.x = ((float)(int)frame0.m_StateTimer + framePosition) * updateFrameToSeconds;
		}
	}

	public static float GetUpdateFrameTransition(float framePosition)
	{
		float num = framePosition * framePosition;
		return 3f * num - 2f * num * framePosition;
	}

	public static void SynchronizeMovementTime(DynamicBuffer<AnimationClip> clips, ref Animated animated, float movementDelta, float framePosition)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		AnimationClip animationClip = clips[(int)animated.m_ClipIndexBody0];
		AnimationClip animationClip2 = clips[(int)animated.m_ClipIndexBody1];
		float2 val = default(float2);
		((float2)(ref val))._002Ector(animationClip.m_AnimationLength, animationClip2.m_AnimationLength);
		float2 val2 = movementDelta / (animated.m_MovementSpeed * val);
		ref float4 time = ref animated.m_Time;
		((float4)(ref time)).xy = ((float4)(ref time)).xy + math.lerp(val2.x, val2.y, framePosition) * val;
	}

	public static float GetInitialTime(ref Random random, in AnimationClip clip, float movementSpeed, float prevClipLength, float prevMovementSpeed, float prevTime)
	{
		if (movementSpeed != 0f && prevMovementSpeed != 0f && prevClipLength != 0f)
		{
			return prevTime / prevClipLength * clip.m_AnimationLength;
		}
		return clip.m_Playback switch
		{
			AnimationPlayback.RandomLoop => ((Random)(ref random)).NextFloat(clip.m_AnimationLength), 
			AnimationPlayback.HalfLoop => math.select(0f, clip.m_AnimationLength * 0.5f, ((Random)(ref random)).NextBool()), 
			_ => 0f, 
		};
	}

	public static void UpdateInterpolatedAnimationFace(Entity entity, DynamicBuffer<AnimationClip> clips, ref ComponentLookup<Human> humanLookup, ref Animated animated, ref Random random, TransformState state, ActivityType activity, float deltaTime, int updateFrameChanged, bool instantReset)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (instantReset)
		{
			ActivityCondition activityConditions = GetActivityConditions(entity, ref humanLookup);
			FindAnimationClip(clips, Game.Prefabs.AnimationType.Idle, ActivityType.None, AnimationLayer.Facial, new AnimatedPropID(-1), activityConditions, out var clip, out var index);
			animated.m_ClipIndexFace0 = (short)index;
			animated.m_Time.z = ((Random)(ref random)).NextFloat(clip.m_AnimationLength);
		}
		else if (updateFrameChanged > 0)
		{
			AnimationClip clip2;
			if (animated.m_ClipIndexFace1 != -1)
			{
				clip2 = clips[(int)animated.m_ClipIndexFace1];
				animated.m_ClipIndexFace0 = animated.m_ClipIndexFace1;
				animated.m_Time.z = animated.m_Time.w;
			}
			else
			{
				clip2 = clips[(int)animated.m_ClipIndexFace0];
			}
			ActivityCondition activityConditions2 = GetActivityConditions(entity, ref humanLookup);
			animated.m_ClipIndexFace1 = -1;
			animated.m_Time.w = 0f;
			if (((clip2.m_Conditions ^ activityConditions2) & (ActivityCondition.Angry | ActivityCondition.Sad | ActivityCondition.Happy | ActivityCondition.Waiting)) != 0 && FindAnimationClip(clips, Game.Prefabs.AnimationType.Idle, ActivityType.None, AnimationLayer.Facial, new AnimatedPropID(-1), activityConditions2, out clip2, out var index2))
			{
				animated.m_ClipIndexFace1 = (short)index2;
				animated.m_Time.w = ((Random)(ref random)).NextFloat(clip2.m_AnimationLength);
			}
		}
		else if (updateFrameChanged < 0)
		{
			AnimationClip clip3 = clips[(int)animated.m_ClipIndexFace0];
			ActivityCondition activityConditions3 = GetActivityConditions(entity, ref humanLookup);
			animated.m_ClipIndexFace1 = -1;
			animated.m_Time.w = 0f;
			if (((clip3.m_Conditions ^ activityConditions3) & (ActivityCondition.Angry | ActivityCondition.Sad | ActivityCondition.Happy | ActivityCondition.Waiting)) != 0 && FindAnimationClip(clips, Game.Prefabs.AnimationType.Idle, ActivityType.None, AnimationLayer.Facial, new AnimatedPropID(-1), activityConditions3, out clip3, out var index3))
			{
				animated.m_ClipIndexFace1 = animated.m_ClipIndexFace0;
				animated.m_Time.w = animated.m_Time.z;
				animated.m_ClipIndexFace0 = (short)index3;
				animated.m_Time.z = ((Random)(ref random)).NextFloat(clip3.m_AnimationLength);
			}
		}
		ref float4 time = ref animated.m_Time;
		((float4)(ref time)).zw = ((float4)(ref time)).zw + deltaTime;
		animated.m_Time.w = 0f;
	}

	public static void CalculateUpdateFrames(uint simulationFrameIndex, float simulationFrameTime, uint updateFrameIndex, out uint updateFrame1, out uint updateFrame2, out float framePosition)
	{
		uint num = simulationFrameIndex - updateFrameIndex - 32;
		updateFrame1 = (num >> 4) & 3;
		updateFrame2 = (updateFrame1 + 1) & 3;
		framePosition = ((float)(num & 0xF) + simulationFrameTime) * 0.0625f;
	}

	public static void CalculateUpdateFrames(uint simulationFrameIndex, uint prevSimulationFrameIndex, float simulationFrameTime, uint updateFrameIndex, out uint updateFrame1, out uint updateFrame2, out float framePosition, out int updateFrameChanged)
	{
		uint num = simulationFrameIndex - updateFrameIndex - 32;
		uint num2 = prevSimulationFrameIndex - updateFrameIndex - 32;
		updateFrame1 = num >> 4;
		uint num3 = num2 >> 4;
		updateFrameChanged = math.select(0, math.select(-1, 1, updateFrame1 > num3), updateFrame1 != num3);
		updateFrame1 &= 3u;
		updateFrame2 = (updateFrame1 + 1) & 3;
		framePosition = ((float)(num & 0xF) + simulationFrameTime) * 0.0625f;
	}

	public static InterpolatedTransform CalculateTransform(TransformFrame frame1, TransformFrame frame2, float framePosition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 val = default(Bezier4x3);
		((Bezier4x3)(ref val))._002Ector(frame1.m_Position, frame1.m_Position + frame1.m_Velocity * (4f / 45f), frame2.m_Position - frame2.m_Velocity * (4f / 45f), frame2.m_Position);
		InterpolatedTransform result = default(InterpolatedTransform);
		result.m_Position = MathUtils.Position(val, framePosition);
		result.m_Rotation = math.slerp(frame1.m_Rotation, frame2.m_Rotation, framePosition);
		result.m_Flags = ((framePosition >= 0.5f) ? frame2.m_Flags : frame1.m_Flags);
		return result;
	}

	private static quaternion LocalToWorld(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform transform, Skeleton skeleton, int index, quaternion rotation)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		while (index >= 0)
		{
			rotation = math.mul(bones[skeleton.m_BoneOffset + index].m_Rotation, rotation);
			index = proceduralBones[index].m_ParentIndex;
		}
		return math.mul(transform.m_Rotation, rotation);
	}

	private static quaternion LocalToObject(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Skeleton skeleton, int index, quaternion rotation)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		while (index >= 0)
		{
			rotation = math.mul(bones[skeleton.m_BoneOffset + index].m_Rotation, rotation);
			index = proceduralBones[index].m_ParentIndex;
		}
		return rotation;
	}

	private static float3 LocalToWorld(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform transform, Skeleton skeleton, int index, float3 position)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		while (index >= 0)
		{
			Bone bone = bones[skeleton.m_BoneOffset + index];
			position = bone.m_Position + math.mul(bone.m_Rotation, position);
			index = proceduralBones[index].m_ParentIndex;
		}
		return transform.m_Position + math.mul(transform.m_Rotation, position);
	}

	private static void LocalToWorld(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform transform, Skeleton skeleton, int index, ref float3 position, ref quaternion rotation)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		while (index >= 0)
		{
			Bone bone = bones[skeleton.m_BoneOffset + index];
			position = bone.m_Position + math.mul(bone.m_Rotation, position);
			rotation = math.mul(bone.m_Rotation, rotation);
			index = proceduralBones[index].m_ParentIndex;
		}
		position = transform.m_Position + math.mul(transform.m_Rotation, position);
		rotation = math.mul(transform.m_Rotation, rotation);
	}

	private static void LookAtLocalToWorld(DynamicBuffer<ProceduralBone> proceduralBones, DynamicBuffer<Bone> bones, Transform transform, Skeleton skeleton, PointOfInterest pointOfInterest, int parentIndex, ref float3 position, ref quaternion rotation)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		ProceduralBone proceduralBone = proceduralBones[parentIndex];
		float3 val2;
		if (proceduralBone.m_Type == BoneType.LookAtRotation || proceduralBone.m_Type == BoneType.LookAtRotationSide)
		{
			float3 position2 = proceduralBone.m_Position;
			quaternion rotation2 = proceduralBone.m_Rotation;
			LocalToWorld(proceduralBones, bones, transform, skeleton, proceduralBone.m_ParentIndex, ref position2, ref rotation2);
			float3 val = pointOfInterest.m_Position - position2;
			val = math.mul(math.inverse(rotation2), val);
			((float3)(ref val)).xz = math.select(((float3)(ref val)).xz, MathUtils.Right(((float3)(ref val)).xz), proceduralBone.m_Type == BoneType.LookAtRotationSide);
			val = math.select(val, -val, proceduralBone.m_Speed < 0f);
			float2 xz = ((float3)(ref val)).xz;
			if (MathUtils.TryNormalize(ref xz))
			{
				val2 = math.forward();
				float num = MathUtils.RotationAngleSignedRight(((float3)(ref val2)).xz, xz);
				rotation2 = math.mul(rotation2, quaternion.RotateY(num));
			}
			position = position2 + math.mul(rotation2, position);
			rotation = math.mul(rotation2, rotation);
		}
		else if (proceduralBone.m_Type == BoneType.LengthwiseLookAtRotation)
		{
			float3 position3 = proceduralBone.m_Position;
			quaternion rotation3 = proceduralBone.m_Rotation;
			LocalToWorld(proceduralBones, bones, transform, skeleton, proceduralBone.m_ParentIndex, ref position3, ref rotation3);
			float3 val3 = pointOfInterest.m_Position - position3;
			val3 = math.mul(math.inverse(rotation3), val3);
			val3 = math.select(val3, -val3, proceduralBone.m_Speed < 0f);
			float2 xy = ((float3)(ref val3)).xy;
			if (MathUtils.TryNormalize(ref xy))
			{
				val2 = math.up();
				float num2 = MathUtils.RotationAngleSignedLeft(((float3)(ref val2)).xy, xy);
				rotation3 = math.mul(rotation3, quaternion.RotateZ(num2));
			}
			position = position3 + math.mul(rotation3, position);
			rotation = math.mul(rotation3, rotation);
		}
		else
		{
			LocalToWorld(proceduralBones, bones, transform, skeleton, parentIndex, ref position, ref rotation);
		}
	}

	public static bool FindAnimationClip(DynamicBuffer<AnimationClip> clips, Game.Prefabs.AnimationType type, ActivityType activity, AnimationLayer animationLayer, AnimatedPropID propID, ActivityCondition conditions, out AnimationClip clip, out int index)
	{
		int num = int.MaxValue;
		clip = clips[0];
		index = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			AnimationClip animationClip = clips[i];
			if (animationClip.m_Type == type && animationClip.m_Activity == activity && animationClip.m_Layer == animationLayer && animationClip.m_PropID == propID)
			{
				ActivityCondition activityCondition = animationClip.m_Conditions ^ conditions;
				if (activityCondition == (ActivityCondition)0u)
				{
					clip = animationClip;
					index = i;
					return true;
				}
				int num2 = math.countbits((uint)activityCondition);
				if (num2 < num)
				{
					num = num2;
					clip = animationClip;
					index = i;
				}
			}
		}
		return num != int.MaxValue;
	}

	public static float GetMovementSpeed(in CharacterElement characterElement, in AnimationClip clip, ref BufferLookup<AnimationMotion> motionLookup)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (clip.m_Type == Game.Prefabs.AnimationType.Move && clip.m_MotionRange.y > clip.m_MotionRange.x + 1)
		{
			DynamicBuffer<AnimationMotion> motions = motionLookup[characterElement.m_Style];
			AnimationMotion motion = motions[clip.m_MotionRange.x];
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight0);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight1);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight2);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight3);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight4);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight5);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight6);
			AddMotionOffset(ref motion, motions, clip.m_MotionRange, characterElement.m_ShapeWeights.m_Weight7);
			float num = clip.m_AnimationLength * clip.m_FrameRate;
			float num2 = clip.m_FrameRate / math.max(1f, num - 1f);
			return math.length(motion.m_EndOffset - motion.m_StartOffset) * num2;
		}
		return clip.m_MovementSpeed;
	}

	private static void AddMotionOffset(ref AnimationMotion motion, DynamicBuffer<AnimationMotion> motions, int2 range, BlendWeight weight)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		AnimationMotion animationMotion = motions[range.x + weight.m_Index + 1];
		ref float3 startOffset = ref motion.m_StartOffset;
		startOffset += animationMotion.m_StartOffset * weight.m_Weight;
		ref float3 endOffset = ref motion.m_EndOffset;
		endOffset += animationMotion.m_EndOffset * weight.m_Weight;
	}

	public static AnimatedPropID GetPropID(Entity entity, ActivityType activity, ref ComponentLookup<CurrentVehicle> currentVehicleLookup, ref ComponentLookup<PrefabRef> prefabRefLookup, ref BufferLookup<ActivityLocationElement> activityLocationLookup)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		AnimatedPropID result = new AnimatedPropID(-1);
		CurrentVehicle currentVehicle = default(CurrentVehicle);
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
		if ((activity == ActivityType.Enter || activity == ActivityType.Exit) && currentVehicleLookup.TryGetComponent(entity, ref currentVehicle) && prefabRefLookup.TryGetComponent(currentVehicle.m_Vehicle, ref prefabRef) && activityLocationLookup.TryGetBuffer(prefabRef.m_Prefab, ref val) && val.Length != 0)
		{
			return val[0].m_PropID;
		}
		return result;
	}

	public static ActivityCondition GetActivityConditions(Entity entity, ref ComponentLookup<Human> humanLookup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Human human = default(Human);
		if (humanLookup.TryGetComponent(entity, ref human))
		{
			return CreatureUtils.GetConditions(human);
		}
		return (ActivityCondition)0u;
	}

	public static void GetClipType(AnimationClip clip, TransformState state, float movementDelta, float speedDeltaFactor, out Game.Prefabs.AnimationType type, ref ActivityType activity)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		switch (state)
		{
		case TransformState.Move:
			type = Game.Prefabs.AnimationType.Move;
			if (activity == ActivityType.None)
			{
				switch (clip.m_Activity)
				{
				case ActivityType.Walking:
				{
					float num2 = math.abs(movementDelta * speedDeltaFactor);
					activity = ((speedDeltaFactor != 0f && num2 > clip.m_SpeedRange.max) ? ActivityType.Running : ActivityType.Walking);
					break;
				}
				case ActivityType.Running:
				{
					float num = math.abs(movementDelta * speedDeltaFactor);
					activity = ((speedDeltaFactor != 0f && num < clip.m_SpeedRange.min) ? ActivityType.Walking : ActivityType.Running);
					break;
				}
				default:
					activity = ActivityType.Walking;
					break;
				}
			}
			break;
		case TransformState.Start:
			type = Game.Prefabs.AnimationType.Start;
			break;
		case TransformState.End:
			type = Game.Prefabs.AnimationType.End;
			break;
		case TransformState.Action:
		case TransformState.Done:
			type = Game.Prefabs.AnimationType.Action;
			break;
		default:
			type = Game.Prefabs.AnimationType.Idle;
			if (activity == ActivityType.None)
			{
				activity = ActivityType.Standing;
			}
			break;
		}
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
	public ObjectInterpolateSystem()
	{
	}
}
