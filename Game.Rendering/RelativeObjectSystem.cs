using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
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
public class RelativeObjectSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateRelativeTransformDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<VehicleData> m_PrefabVehicleData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<BoneHistory> m_BoneHistories;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<AnimationMotion> m_AnimationMotions;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Animated> m_Animateds;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public uint m_PrevFrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public float m_FrameDelta;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public AnimatedSystem.AnimationData m_AnimationData;

		public void Execute(int index)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData cullingData = m_CullingData[index];
			if ((cullingData.m_Flags & (PreCullingFlags.NearCamera | PreCullingFlags.Temp | PreCullingFlags.InterpolatedTransform | PreCullingFlags.Relative)) != (PreCullingFlags.NearCamera | PreCullingFlags.InterpolatedTransform | PreCullingFlags.Relative))
			{
				return;
			}
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			Entity val;
			if (m_CurrentVehicleData.TryGetComponent(cullingData.m_Entity, ref currentVehicle))
			{
				val = currentVehicle.m_Vehicle;
			}
			else
			{
				Owner owner = m_OwnerData[cullingData.m_Entity];
				if (m_RelativeData.HasComponent(owner.m_Owner))
				{
					return;
				}
				val = owner.m_Owner;
			}
			Transform relativeTransform = GetRelativeTransform(m_RelativeData[cullingData.m_Entity], val, ref m_BoneHistories, ref m_PrefabRefData, ref m_SubMeshes);
			InterpolatedTransform interpolatedTransform = default(InterpolatedTransform);
			Transform parentTransform = default(Transform);
			Transform transform = (m_InterpolatedTransformData.TryGetComponent(val, ref interpolatedTransform) ? ObjectUtils.LocalToWorld(interpolatedTransform.ToTransform(), relativeTransform) : ((!m_TransformData.TryGetComponent(val, ref parentTransform)) ? relativeTransform : ObjectUtils.LocalToWorld(parentTransform, relativeTransform)));
			Random random = m_RandomSeed.GetRandom(index);
			if ((cullingData.m_Flags & PreCullingFlags.Animated) != 0)
			{
				ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_PrevFrameIndex, m_FrameTime, (uint)cullingData.m_UpdateFrame, out var _, out var _, out var framePosition, out var updateFrameChanged);
				float updateFrameToSeconds = 4f / 15f;
				float deltaTime = m_FrameDelta / 60f;
				float speedDeltaFactor = math.select(60f / m_FrameDelta, 0f, m_FrameDelta == 0f);
				UpdateInterpolatedAnimations(cullingData, transform, val, ref random, framePosition, updateFrameToSeconds, deltaTime, speedDeltaFactor, updateFrameChanged);
			}
			else
			{
				m_InterpolatedTransformData[cullingData.m_Entity] = new InterpolatedTransform(transform);
			}
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(cullingData.m_Entity, ref subObjects))
			{
				UpdateTransforms(transform, subObjects);
			}
		}

		private float3 GetVelocity(Entity parent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			DynamicBuffer<TransformFrame> val = default(DynamicBuffer<TransformFrame>);
			if (m_CullingInfoData.TryGetComponent(parent, ref cullingInfo) && cullingInfo.m_CullingIndex != 0 && m_TransformFrames.TryGetBuffer(parent, ref val))
			{
				PreCullingData preCullingData = m_CullingData[cullingInfo.m_CullingIndex];
				ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_PrevFrameIndex, m_FrameTime, (uint)preCullingData.m_UpdateFrame, out var updateFrame, out var updateFrame2, out var framePosition, out var _);
				TransformFrame transformFrame = val[(int)updateFrame];
				TransformFrame transformFrame2 = val[(int)updateFrame2];
				return math.lerp(transformFrame.m_Velocity, transformFrame2.m_Velocity, framePosition);
			}
			return float3.op_Implicit(0f);
		}

		private void UpdateInterpolatedAnimations(PreCullingData cullingData, Transform transform, Entity parent, ref Random random, float framePosition, float updateFrameToSeconds, float deltaTime, float speedDeltaFactor, int updateFrameChanged)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (cullingData.m_Flags & PreCullingFlags.NearCameraUpdated) != 0;
			ref InterpolatedTransform valueRW = ref m_InterpolatedTransformData.GetRefRW(cullingData.m_Entity).ValueRW;
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<Animated> val = m_Animateds[cullingData.m_Entity];
			DynamicBuffer<SubMesh> val2 = m_SubMeshes[prefabRef.m_Prefab];
			float stateTimer = 0f;
			TransformState state = TransformState.Idle;
			ActivityType activity = ActivityType.Driving;
			AnimatedPropID propID = new AnimatedPropID(-1);
			float steerAngle = 0f;
			float3 velocity = default(float3);
			PrefabRef prefabRef2 = default(PrefabRef);
			if (m_PrefabRefData.TryGetComponent(parent, ref prefabRef2))
			{
				DynamicBuffer<ActivityLocationElement> val3 = default(DynamicBuffer<ActivityLocationElement>);
				if (m_ActivityLocations.TryGetBuffer(prefabRef2.m_Prefab, ref val3) && val3.Length != 0)
				{
					propID = val3[0].m_PropID;
				}
				VehicleData vehicleData = default(VehicleData);
				DynamicBuffer<Bone> val4 = default(DynamicBuffer<Bone>);
				if (m_PrefabVehicleData.TryGetComponent(prefabRef2.m_Prefab, ref vehicleData) && vehicleData.m_SteeringBoneIndex != -1 && m_Bones.TryGetBuffer(parent, ref val4) && val4.Length > vehicleData.m_SteeringBoneIndex)
				{
					steerAngle = math.asin(math.mul(val4[vehicleData.m_SteeringBoneIndex].m_Rotation, math.up()).x);
				}
				velocity = GetVelocity(parent);
			}
			DynamicBuffer<MeshGroup> val5 = default(DynamicBuffer<MeshGroup>);
			DynamicBuffer<CharacterElement> val6 = default(DynamicBuffer<CharacterElement>);
			int priority = 0;
			DynamicBuffer<SubMeshGroup> val7 = default(DynamicBuffer<SubMeshGroup>);
			if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val7))
			{
				m_MeshGroups.TryGetBuffer(cullingData.m_Entity, ref val5);
				m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val6);
				valueRW = new InterpolatedTransform(transform);
				CullingInfo cullingInfo = m_CullingInfoData[cullingData.m_Entity];
				float num = RenderingUtils.CalculateMinDistance(cullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				priority = RenderingUtils.CalculateLod(num * num, m_LodParameters) - cullingInfo.m_MinLod;
			}
			else
			{
				valueRW = new InterpolatedTransform(transform);
			}
			InterpolatedTransform oldTransform = valueRW;
			MeshGroup meshGroup = default(MeshGroup);
			MeshGroup meshGroup2 = default(MeshGroup);
			for (int i = 0; i < val.Length; i++)
			{
				Animated animated = val[i];
				if (animated.m_ClipIndexBody0 != -1)
				{
					if (val6.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val5, i, ref meshGroup);
						CharacterElement characterElement = val6[(int)meshGroup.m_SubMeshGroup];
						DynamicBuffer<AnimationClip> clips = m_AnimationClips[characterElement.m_Style];
						UpdateDrivingAnimationBody(cullingData.m_Entity, in characterElement, clips, ref m_HumanData, ref m_AnimationMotions, oldTransform, valueRW, ref animated, ref random, velocity, steerAngle, propID, updateFrameToSeconds, speedDeltaFactor, deltaTime, updateFrameChanged, flag);
						ObjectInterpolateSystem.UpdateInterpolatedAnimationFace(cullingData.m_Entity, clips, ref m_HumanData, ref animated, ref random, state, activity, deltaTime, updateFrameChanged, flag);
						m_AnimationData.SetAnimationFrame(in characterElement, clips, in animated, float2.op_Implicit(ObjectInterpolateSystem.GetUpdateFrameTransition(framePosition)), priority, flag);
					}
					else
					{
						int num2 = i;
						if (val7.IsCreated)
						{
							CollectionUtils.TryGet<MeshGroup>(val5, i, ref meshGroup2);
							num2 = val7[(int)meshGroup2.m_SubMeshGroup].m_SubMeshRange.x;
						}
						SubMesh subMesh = val2[num2];
						ObjectInterpolateSystem.UpdateInterpolatedAnimation(m_AnimationClips[subMesh.m_SubMesh], oldTransform, valueRW, ref animated, stateTimer, state, activity, updateFrameToSeconds, speedDeltaFactor);
					}
				}
				val[i] = animated;
			}
		}

		private void UpdateTransforms(Transform ownerTransform, DynamicBuffer<Game.Objects.SubObject> subObjects)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_InterpolatedTransformData.HasComponent(subObject))
				{
					Transform transform = m_RelativeData[subObject].ToTransform();
					Transform transform2 = ObjectUtils.LocalToWorld(ownerTransform, transform);
					m_InterpolatedTransformData[subObject] = new InterpolatedTransform(transform2);
					if (m_SubObjects.HasBuffer(subObject))
					{
						DynamicBuffer<Game.Objects.SubObject> subObjects2 = m_SubObjects[subObject];
						UpdateTransforms(transform2, subObjects2);
					}
				}
			}
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
		public ComponentTypeHandle<Static> m_StaticType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<BoneHistory> m_BoneHistories;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Animated> m_Animateds;

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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06af: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CullingInfo> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			BufferAccessor<MeshGroup> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Static>(ref m_StaticType);
			uint updateFrame = 0u;
			uint updateFrame2 = 0u;
			float framePosition = 0f;
			if (((ArchetypeChunk)(ref chunk)).Has<UpdateFrame>(m_UpdateFrameType))
			{
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_FrameTime, index, out updateFrame, out updateFrame2, out framePosition);
			}
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			Owner owner = default(Owner);
			InterpolatedTransform interpolatedTransform = default(InterpolatedTransform);
			Transform parentTransform = default(Transform);
			DynamicBuffer<MeshGroup> val4 = default(DynamicBuffer<MeshGroup>);
			DynamicBuffer<CharacterElement> val5 = default(DynamicBuffer<CharacterElement>);
			MeshGroup meshGroup = default(MeshGroup);
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			DynamicBuffer<MeshGroup> val7 = default(DynamicBuffer<MeshGroup>);
			DynamicBuffer<CharacterElement> val8 = default(DynamicBuffer<CharacterElement>);
			DynamicBuffer<Animated> val9 = default(DynamicBuffer<Animated>);
			MeshGroup meshGroup2 = default(MeshGroup);
			MeshGroup meshGroup3 = default(MeshGroup);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Temp temp = nativeArray2[i];
				CullingInfo cullingInfo = nativeArray4[i];
				if (cullingInfo.m_CullingIndex == 0)
				{
					continue;
				}
				PreCullingData preCullingData = m_CullingData[cullingInfo.m_CullingIndex];
				if ((preCullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					continue;
				}
				if (m_InterpolatedTransformData.HasComponent(val))
				{
					if ((flag && (temp.m_Original == Entity.Null || (temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0)) || (temp.m_Flags & TempFlags.Dragging) != 0)
					{
						Entity val2;
						if (m_CurrentVehicleData.TryGetComponent(val, ref currentVehicle))
						{
							val2 = currentVehicle.m_Vehicle;
						}
						else if (m_OwnerData.TryGetComponent(val, ref owner))
						{
							if (m_RelativeData.HasComponent(owner.m_Owner))
							{
								continue;
							}
							val2 = owner.m_Owner;
						}
						else
						{
							val2 = Entity.Null;
						}
						Transform transform = m_TransformData[val];
						if (val2 != Entity.Null)
						{
							Transform relativeTransform = GetRelativeTransform(m_RelativeData[val], val2, ref m_BoneHistories, ref m_PrefabRefData, ref m_SubMeshes);
							transform = (m_InterpolatedTransformData.TryGetComponent(val2, ref interpolatedTransform) ? ObjectUtils.LocalToWorld(interpolatedTransform.ToTransform(), relativeTransform) : ((!m_TransformData.TryGetComponent(val2, ref parentTransform)) ? relativeTransform : ObjectUtils.LocalToWorld(parentTransform, relativeTransform)));
						}
						m_InterpolatedTransformData[val] = new InterpolatedTransform(transform);
						if (m_Animateds.HasBuffer(val))
						{
							PrefabRef prefabRef = nativeArray3[i];
							DynamicBuffer<Animated> val3 = m_Animateds[val];
							CollectionUtils.TryGet<MeshGroup>(bufferAccessor, i, ref val4);
							m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val5);
							for (int j = 0; j < val3.Length; j++)
							{
								Animated animated = val3[j];
								if (animated.m_ClipIndexBody0 != -1)
								{
									animated.m_ClipIndexBody0 = 0;
									animated.m_Time = float4.op_Implicit(0f);
									animated.m_MovementSpeed = float2.op_Implicit(0f);
									animated.m_Interpolation = 0f;
									animated.m_PreviousTime = 0f;
									val3[j] = animated;
								}
								if (animated.m_MetaIndex != 0 && val5.IsCreated)
								{
									CollectionUtils.TryGet<MeshGroup>(val4, j, ref meshGroup);
									CharacterElement characterElement = val5[(int)meshGroup.m_SubMeshGroup];
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
						}
						if (m_SubObjects.TryGetBuffer(val, ref subObjects))
						{
							UpdateTransforms(transform, subObjects);
						}
						continue;
					}
					if (m_TransformData.HasComponent(temp.m_Original))
					{
						Transform transform2 = m_TransformData[temp.m_Original];
						m_TransformData[val] = transform2;
						if (m_InterpolatedTransformData.HasComponent(temp.m_Original))
						{
							m_InterpolatedTransformData[val] = m_InterpolatedTransformData[temp.m_Original];
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
				if (!m_Animateds.HasBuffer(val))
				{
					continue;
				}
				PrefabRef prefabRef2 = nativeArray3[i];
				DynamicBuffer<Animated> val6 = m_Animateds[val];
				bool reset = (preCullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated)) != 0;
				float num = RenderingUtils.CalculateMinDistance(cullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int priority = RenderingUtils.CalculateLod(num * num, m_LodParameters) - cullingInfo.m_MinLod;
				CollectionUtils.TryGet<MeshGroup>(bufferAccessor, i, ref val7);
				m_CharacterElements.TryGetBuffer(prefabRef2.m_Prefab, ref val8);
				if (m_Animateds.TryGetBuffer(temp.m_Original, ref val9) && val9.Length == val6.Length)
				{
					for (int k = 0; k < val6.Length; k++)
					{
						Animated animated3 = val6[k];
						Animated animated4 = val9[k];
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
						val6[k] = animated3;
						if (animated3.m_MetaIndex != 0 && val8.IsCreated)
						{
							CollectionUtils.TryGet<MeshGroup>(val7, k, ref meshGroup2);
							CharacterElement characterElement2 = val8[(int)meshGroup2.m_SubMeshGroup];
							float num2 = framePosition * framePosition;
							num2 = 3f * num2 - 2f * num2 * framePosition;
							DynamicBuffer<AnimationClip> clips2 = m_AnimationClips[characterElement2.m_Style];
							m_AnimationData.SetAnimationFrame(in characterElement2, clips2, in animated3, float2.op_Implicit(num2), priority, reset);
						}
					}
					continue;
				}
				for (int l = 0; l < val6.Length; l++)
				{
					Animated animated5 = val6[l];
					if (animated5.m_ClipIndexBody0 != -1)
					{
						animated5.m_ClipIndexBody0 = 0;
						animated5.m_Time = float4.op_Implicit(0f);
						animated5.m_MovementSpeed = float2.op_Implicit(0f);
						animated5.m_Interpolation = 0f;
						animated5.m_PreviousTime = 0f;
						val6[l] = animated5;
					}
					if (animated5.m_MetaIndex != 0 && val8.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val7, l, ref meshGroup3);
						CharacterElement characterElement3 = val8[(int)meshGroup3.m_SubMeshGroup];
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

		private void UpdateTransforms(Transform ownerTransform, DynamicBuffer<Game.Objects.SubObject> subObjects)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_InterpolatedTransformData.HasComponent(subObject))
				{
					Transform transform = m_RelativeData[subObject].ToTransform();
					Transform transform2 = ObjectUtils.LocalToWorld(ownerTransform, transform);
					m_InterpolatedTransformData[subObject] = new InterpolatedTransform(transform2);
					if (m_SubObjects.HasBuffer(subObject))
					{
						DynamicBuffer<Game.Objects.SubObject> subObjects2 = m_SubObjects[subObject];
						UpdateTransforms(transform2, subObjects2);
					}
				}
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
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleData> __Game_Prefabs_VehicleData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Bone> __Game_Rendering_Bone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<BoneHistory> __Game_Rendering_BoneHistory_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationMotion> __Game_Prefabs_AnimationMotion_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentLookup;

		public BufferLookup<Animated> __Game_Rendering_Animated_RW_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Static> __Game_Objects_Static_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

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
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_VehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Rendering_Bone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(true);
			__Game_Rendering_BoneHistory_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BoneHistory>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(false);
			__Game_Rendering_Animated_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Animated>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
			__Game_Objects_Static_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Static>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private PreCullingSystem m_PreCullingSystem;

	private AnimatedSystem m_AnimatedSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private BatchDataSystem m_BatchDataSystem;

	private EntityQuery m_RelativeQuery;

	private EntityQuery m_InterpolateQuery;

	private uint m_PrevFrameIndex;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Relative>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<InterpolatedTransform>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_InterpolateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeList<PreCullingData> cullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies);
		JobHandle dependencies2;
		AnimatedSystem.AnimationData animationData = m_AnimatedSystem.GetAnimationData(out dependencies2);
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
		UpdateRelativeTransformDataJob updateRelativeTransformDataJob = new UpdateRelativeTransformDataJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabVehicleData = InternalCompilerInterface.GetComponentLookup<VehicleData>(ref __TypeHandle.__Game_Prefabs_VehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneHistories = InternalCompilerInterface.GetBufferLookup<BoneHistory>(ref __TypeHandle.__Game_Rendering_BoneHistory_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrevFrameIndex = m_PrevFrameIndex,
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_FrameDelta = m_RenderingSystem.frameDelta,
			m_LodParameters = lodParameters,
			m_CameraPosition = cameraPosition,
			m_CameraDirection = cameraDirection,
			m_RandomSeed = RandomSeed.Next(),
			m_CullingData = cullingData,
			m_AnimationData = animationData
		};
		UpdateQueryTransformDataJob obj = new UpdateQueryTransformDataJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StaticType = InternalCompilerInterface.GetComponentTypeHandle<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneHistories = InternalCompilerInterface.GetBufferLookup<BoneHistory>(ref __TypeHandle.__Game_Rendering_BoneHistory_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_LodParameters = lodParameters,
			m_CameraPosition = cameraPosition,
			m_CameraDirection = cameraDirection,
			m_CullingData = cullingData,
			m_AnimationData = animationData
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<UpdateRelativeTransformDataJob, PreCullingData>(updateRelativeTransformDataJob, cullingData, 16, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateQueryTransformDataJob>(obj, m_InterpolateQuery, val);
		m_PreCullingSystem.AddCullingDataReader(val2);
		m_AnimatedSystem.AddAnimationWriter(val2);
		((SystemBase)this).Dependency = val2;
		m_PrevFrameIndex = m_RenderingSystem.frameIndex;
	}

	private static Transform GetRelativeTransform(Relative relative, Entity parent, ref BufferLookup<BoneHistory> boneHistoryLookup, ref ComponentLookup<PrefabRef> prefabRefLookup, ref BufferLookup<SubMesh> subMeshLookup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (relative.m_BoneIndex.y >= 0)
		{
			DynamicBuffer<BoneHistory> val = boneHistoryLookup[parent];
			if (val.Length > relative.m_BoneIndex.y)
			{
				float4x4 matrix = val[relative.m_BoneIndex.y].m_Matrix;
				float3 val2 = math.transform(matrix, relative.m_Position);
				float3 val3 = math.rotate(matrix, math.forward(relative.m_Rotation));
				float3 val4 = math.rotate(matrix, math.mul(relative.m_Rotation, math.up()));
				quaternion val5 = quaternion.LookRotation(val3, val4);
				if (relative.m_BoneIndex.z >= 0)
				{
					SubMesh subMesh = subMeshLookup[prefabRefLookup[parent].m_Prefab][relative.m_BoneIndex.z];
					val2 = subMesh.m_Position + math.rotate(subMesh.m_Rotation, val2);
					val5 = math.mul(subMesh.m_Rotation, val5);
				}
				return new Transform(val2, val5);
			}
		}
		return new Transform(relative.m_Position, relative.m_Rotation);
	}

	public static void UpdateDrivingAnimationBody(Entity entity, in CharacterElement characterElement, DynamicBuffer<AnimationClip> clips, ref ComponentLookup<Human> humanLookup, ref BufferLookup<AnimationMotion> motionLookup, InterpolatedTransform oldTransform, InterpolatedTransform newTransform, ref Animated animated, ref Random random, float3 velocity, float steerAngle, AnimatedPropID propID, float updateFrameToSeconds, float speedDeltaFactor, float deltaTime, int updateFrameChanged, bool instantReset)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		AnimationClip clip = default(AnimationClip);
		AnimationClip clip2 = default(AnimationClip);
		AnimationClip clip3 = default(AnimationClip);
		AnimationClip clip4 = default(AnimationClip);
		if (!instantReset)
		{
			clip = clips[(int)animated.m_ClipIndexBody0];
			if (animated.m_ClipIndexBody0I != -1)
			{
				clip2 = clips[(int)animated.m_ClipIndexBody0I];
			}
			if (animated.m_ClipIndexBody1 != -1)
			{
				clip3 = clips[(int)animated.m_ClipIndexBody1];
			}
			if (animated.m_ClipIndexBody1I != -1)
			{
				clip4 = clips[(int)animated.m_ClipIndexBody1I];
			}
		}
		float3 val = math.forward(newTransform.m_Rotation);
		float num = math.dot(velocity, val);
		float num2 = math.abs(steerAngle);
		float prev = math.radians(1f);
		ActivityType activityType = ((!(num >= 1f)) ? ActivityType.Standing : ActivityType.Driving);
		if (clip2.m_Activity == ActivityType.Driving)
		{
			float targetRotation = GetTargetRotation(in clip2, math.radians(10f), prev);
			if (num2 > targetRotation)
			{
				activityType = ActivityType.Driving;
			}
		}
		else if (clip4.m_Activity == ActivityType.Driving)
		{
			float targetRotation2 = GetTargetRotation(in clip4, math.radians(10f), prev);
			if (num2 > targetRotation2)
			{
				activityType = ActivityType.Driving;
			}
		}
		if (clip.m_Activity == ActivityType.Driving)
		{
			if (clip3.m_Activity == ActivityType.Driving)
			{
				if (activityType == ActivityType.Standing)
				{
					clip3.m_Activity = ActivityType.Standing;
				}
				else
				{
					clip.m_Activity = ActivityType.Standing;
				}
			}
			else if (clip3.m_Activity == ActivityType.None && activityType == ActivityType.Standing && clip.m_Type != AnimationType.Idle)
			{
				clip.m_Activity = ActivityType.Standing;
			}
		}
		bool flag = updateFrameChanged > 0 && ((clip.m_Activity != ActivityType.None && (clip.m_Activity != activityType || clip.m_Type == AnimationType.Start || clip.m_PropID != propID)) || (clip3.m_Activity != ActivityType.None && (clip3.m_Activity != activityType || clip3.m_Type == AnimationType.Start || clip3.m_PropID != propID)));
		if (flag && clip3.m_Type != AnimationType.None)
		{
			animated.m_ClipIndexBody0 = animated.m_ClipIndexBody1;
			animated.m_ClipIndexBody0I = animated.m_ClipIndexBody1I;
			animated.m_ClipIndexBody1 = -1;
			animated.m_ClipIndexBody1I = -1;
			animated.m_Time.x = animated.m_Time.y;
			animated.m_Time.y = 0f;
			animated.m_MovementSpeed.x = animated.m_MovementSpeed.y;
			animated.m_MovementSpeed.y = 0f;
			clip = clip3;
			clip2 = clip4;
			clip3 = default(AnimationClip);
			clip4 = default(AnimationClip);
			flag &= clip.m_Activity != activityType;
		}
		if (clip.m_Activity == ActivityType.None || ((clip.m_Activity == ActivityType.Driving || clip.m_Activity == ActivityType.Standing) && clip.m_Type != AnimationType.Start && clip.m_PropID == propID))
		{
			bool num3 = clip.m_Type == AnimationType.None;
			UpdateDrivingClips(targetActivity: num3 ? activityType : clip.m_Activity, entity: entity, clip: ref clip, clipI: ref clip2, clipIndex: ref animated.m_ClipIndexBody0, clipIndexI: ref animated.m_ClipIndexBody0I, movementSpeed: ref animated.m_MovementSpeed.x, interpolation: ref animated.m_Interpolation, clips: clips, humanLookup: ref humanLookup, steerAngle: steerAngle, propID: propID);
			if (num3)
			{
				animated.m_Time.x = ((Random)(ref random)).NextFloat(clip.m_AnimationLength);
			}
		}
		if (flag || ((clip3.m_Activity == ActivityType.Driving || clip3.m_Activity == ActivityType.Standing) && clip3.m_Type != AnimationType.Start && clip3.m_PropID == propID))
		{
			bool num4 = clip3.m_Type == AnimationType.None;
			UpdateDrivingClips(targetActivity: num4 ? activityType : clip3.m_Activity, entity: entity, clip: ref clip3, clipI: ref clip4, clipIndex: ref animated.m_ClipIndexBody1, clipIndexI: ref animated.m_ClipIndexBody1I, movementSpeed: ref animated.m_MovementSpeed.y, interpolation: ref animated.m_Interpolation, clips: clips, humanLookup: ref humanLookup, steerAngle: steerAngle, propID: propID);
			if (num4)
			{
				if ((clip.m_Activity == ActivityType.Driving || clip.m_Activity == ActivityType.Standing) && clip.m_Type != AnimationType.Start && clip.m_PropID == propID)
				{
					animated.m_Time.y = animated.m_Time.x;
				}
				else
				{
					animated.m_Time.y = ((Random)(ref random)).NextFloat(clip3.m_AnimationLength);
				}
			}
		}
		if (animated.m_ClipIndexBody1 != -1 && animated.m_MovementSpeed.y == 0f)
		{
			animated.m_Time.y += deltaTime;
		}
		if (animated.m_MovementSpeed.x == 0f)
		{
			animated.m_Time.x += deltaTime;
		}
	}

	public static void UpdateDrivingClips(Entity entity, ref AnimationClip clip, ref AnimationClip clipI, ref short clipIndex, ref short clipIndexI, ref float movementSpeed, ref float interpolation, DynamicBuffer<AnimationClip> clips, ref ComponentLookup<Human> humanLookup, float steerAngle, AnimatedPropID propID, ActivityType targetActivity)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		float num = math.abs(steerAngle);
		float num2 = math.radians(1f);
		if (num > num2)
		{
			AnimationType animationType = ((steerAngle > 0f) ? AnimationType.RightMin : AnimationType.LeftMin);
			if (clipI.m_Type != animationType || clipI.m_Activity != ActivityType.Driving || clipI.m_PropID != propID)
			{
				ActivityCondition activityConditions = ObjectInterpolateSystem.GetActivityConditions(entity, ref humanLookup);
				ObjectInterpolateSystem.FindAnimationClip(clips, animationType, ActivityType.Driving, AnimationLayer.Body, propID, activityConditions, out clipI, out var index);
				clipIndexI = (short)index;
			}
			float targetRotation = GetTargetRotation(in clipI, math.radians(10f), num2);
			AnimationType animationType2 = AnimationType.Idle;
			ActivityType activity = targetActivity;
			if (num > targetRotation)
			{
				animationType2 = ((steerAngle > 0f) ? AnimationType.RightMax : AnimationType.LeftMax);
				activity = ActivityType.Driving;
			}
			if (clip.m_Type != animationType2 || clip.m_Activity != targetActivity || clip.m_PropID != propID)
			{
				ActivityCondition activityConditions2 = ObjectInterpolateSystem.GetActivityConditions(entity, ref humanLookup);
				ObjectInterpolateSystem.FindAnimationClip(clips, animationType2, activity, AnimationLayer.Body, propID, activityConditions2, out clip, out var index2);
				clipIndex = (short)index2;
				movementSpeed = 0f;
			}
			if (num > targetRotation)
			{
				float targetRotation2 = GetTargetRotation(in clip, math.radians(45f), targetRotation);
				interpolation = math.saturate(1f - (num - targetRotation) / (targetRotation2 - targetRotation));
			}
			else
			{
				interpolation = math.saturate((num - num2) / (targetRotation - num2));
			}
		}
		else
		{
			if (clip.m_Type != AnimationType.Idle || clip.m_Activity != targetActivity || clip.m_PropID != propID)
			{
				ActivityCondition activityConditions3 = ObjectInterpolateSystem.GetActivityConditions(entity, ref humanLookup);
				ObjectInterpolateSystem.FindAnimationClip(clips, AnimationType.Idle, targetActivity, AnimationLayer.Body, propID, activityConditions3, out clip, out var index3);
				clipIndex = (short)index3;
				movementSpeed = 0f;
			}
			interpolation = 0f;
			clipIndexI = -1;
			clipI = default(AnimationClip);
		}
	}

	public static float GetTargetRotation(in AnimationClip clip, float def, float prev)
	{
		return math.max(math.select(clip.m_TargetValue, def, clip.m_TargetValue == float.MinValue), prev + math.radians(1f));
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
	public RelativeObjectSystem()
	{
	}
}
