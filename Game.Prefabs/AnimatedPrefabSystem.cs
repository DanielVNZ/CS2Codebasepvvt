using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Animations;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Rendering;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class AnimatedPrefabSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CharacterStyleData> __Game_Prefabs_CharacterStyleData_RW_ComponentTypeHandle;

		public BufferTypeHandle<AnimationClip> __Game_Prefabs_AnimationClip_RW_BufferTypeHandle;

		public BufferTypeHandle<AnimationMotion> __Game_Prefabs_AnimationMotion_RW_BufferTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_CharacterStyleData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CharacterStyleData>(false);
			__Game_Prefabs_AnimationClip_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AnimationClip>(false);
			__Game_Prefabs_AnimationMotion_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AnimationMotion>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private AnimatedSystem m_AnimatedSystem;

	private EntityQuery m_PrefabQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadWrite<CharacterStyleData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected I4, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Invalid comparison between Unknown and I4
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Invalid comparison between Unknown and I4
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Invalid comparison between Unknown and I4
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<CharacterStyleData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<CharacterStyleData>(ref __TypeHandle.__Game_Prefabs_CharacterStyleData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<AnimationClip> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<AnimationMotion> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		Dictionary<(ActivityType, AnimationType, AnimatedPropID), int> dictionary = new Dictionary<(ActivityType, AnimationType, AnimatedPropID), int>();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			NativeArray<CharacterStyleData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<CharacterStyleData>(ref componentTypeHandle2);
			BufferAccessor<AnimationClip> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<AnimationClip>(ref bufferTypeHandle);
			BufferAccessor<AnimationMotion> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<AnimationMotion>(ref bufferTypeHandle2);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				CharacterStyle prefab = m_PrefabSystem.GetPrefab<CharacterStyle>(nativeArray2[j]);
				ref CharacterStyleData reference = ref CollectionUtils.ElementAt<CharacterStyleData>(nativeArray3, j);
				DynamicBuffer<AnimationClip> val3 = bufferAccessor[j];
				DynamicBuffer<AnimationMotion> val4 = bufferAccessor2[j];
				reference.m_ActivityMask = default(ActivityMask);
				reference.m_RestPoseClipIndex = -1;
				int num = prefab.m_Animations.Length;
				int num2 = 0;
				val3.ResizeUninitialized(num);
				for (int k = 0; k < num; k++)
				{
					CharacterStyle.AnimationInfo animationInfo = prefab.m_Animations[k];
					ref AnimationClip reference2 = ref val3.ElementAt(k);
					reference2 = default(AnimationClip);
					reference2.m_InfoIndex = -1;
					reference2.m_RootMotionBone = animationInfo.rootMotionBone;
					reference2.m_PropClipIndex = -1;
					AnimationLayer layer = animationInfo.layer;
					switch ((int)layer)
					{
					case 0:
						reference2.m_Layer = AnimationLayer.Body;
						break;
					case 1:
						reference2.m_Layer = AnimationLayer.Prop;
						break;
					case 2:
						reference2.m_Layer = AnimationLayer.Facial;
						break;
					case 3:
						reference2.m_Layer = AnimationLayer.Corrective;
						break;
					default:
						reference2.m_Layer = AnimationLayer.None;
						break;
					}
					if (animationInfo.rootMotion != null)
					{
						num2 += animationInfo.rootMotion.Length;
					}
					if ((int)animationInfo.type == -1 && (Object)(object)animationInfo.target == (Object)null)
					{
						reference.m_RestPoseClipIndex = k;
					}
					if ((Object)(object)animationInfo.target != (Object)null && animationInfo.target.TryGet<CharacterProperties>(out var component))
					{
						reference2.m_PropID = m_AnimatedSystem.GetPropID(component.m_AnimatedPropName);
						if ((int)animationInfo.layer == 1)
						{
							dictionary[(animationInfo.activity, animationInfo.state, reference2.m_PropID)] = k;
						}
					}
					else
					{
						reference2.m_PropID = m_AnimatedSystem.GetPropID(null);
					}
				}
				val4.ResizeUninitialized(num2);
				num2 = 0;
				float num3 = float.MaxValue;
				float num4 = 0f;
				for (int l = 0; l < num; l++)
				{
					CharacterStyle.AnimationInfo animationInfo2 = prefab.m_Animations[l];
					ref AnimationClip reference3 = ref val3.ElementAt(l);
					reference3.m_Type = animationInfo2.state;
					reference3.m_Activity = animationInfo2.activity;
					reference3.m_Conditions = animationInfo2.conditions;
					reference3.m_Playback = animationInfo2.playback;
					reference3.m_TargetValue = float.MinValue;
					if (reference3.m_Playback == AnimationPlayback.RandomLoop || reference3.m_Type == AnimationType.Move)
					{
						reference3.m_AnimationLength = (float)animationInfo2.frameCount / (float)animationInfo2.frameRate;
						reference3.m_FrameRate = animationInfo2.frameRate;
					}
					else
					{
						float num5 = (float)(animationInfo2.frameCount - 1) * (60f / (float)animationInfo2.frameRate);
						num5 = math.max(1f, math.round(num5 / 16f)) * 16f;
						reference3.m_AnimationLength = num5 * (1f / 60f);
						reference3.m_FrameRate = (float)math.max(1, animationInfo2.frameCount - 1) / reference3.m_AnimationLength;
						reference3.m_AnimationLength -= 0.001f;
					}
					if (animationInfo2.rootMotion != null)
					{
						NativeArray<AnimationMotion> subArray = val4.AsNativeArray().GetSubArray(num2, animationInfo2.rootMotion.Length);
						CleanUpRootMotion(animationInfo2.rootMotion, subArray);
						reference3.m_MotionRange = new int2(num2, num2 + animationInfo2.rootMotion.Length);
						num2 += animationInfo2.rootMotion.Length;
						if (reference3.m_Type == AnimationType.Move)
						{
							AnimationMotion animationMotion = subArray[0];
							reference3.m_MovementSpeed = math.length(animationMotion.m_EndOffset - animationMotion.m_StartOffset) * reference3.m_FrameRate / (float)math.max(1, animationInfo2.frameCount - 1);
							if (reference3.m_Conditions == (ActivityCondition)0u)
							{
								switch (reference3.m_Activity)
								{
								case ActivityType.Walking:
									num3 = reference3.m_MovementSpeed;
									break;
								case ActivityType.Running:
									num4 = reference3.m_MovementSpeed;
									break;
								}
							}
						}
					}
					else
					{
						reference3.m_RootMotionBone = -1;
					}
					if ((int)animationInfo2.layer != 1 && reference3.m_PropID.isValid && dictionary.TryGetValue((animationInfo2.activity, animationInfo2.state, reference3.m_PropID), out var value))
					{
						reference3.m_PropClipIndex = value;
					}
					reference.m_ActivityMask.m_Mask |= new ActivityMask(reference3.m_Activity).m_Mask;
					reference.m_AnimationLayerMask.m_Mask |= new AnimationLayerMask(reference3.m_Layer).m_Mask;
				}
				for (int m = 0; m < val3.Length; m++)
				{
					ref AnimationClip reference4 = ref val3.ElementAt(m);
					if (reference4.m_Layer == AnimationLayer.Body && reference4.m_Type == AnimationType.Move)
					{
						reference4.m_SpeedRange = new Bounds1(0f, float.MaxValue);
						switch (reference4.m_Activity)
						{
						case ActivityType.Walking:
							reference4.m_SpeedRange.max = math.select((num3 + num4) * 0.5f, float.MaxValue, num4 <= num3);
							break;
						case ActivityType.Running:
							reference4.m_SpeedRange.min = math.select((num3 + num4) * 0.5f, 0f, num3 >= num4);
							break;
						}
					}
				}
				reference.m_BoneCount = prefab.m_BoneCount;
				reference.m_ShapeCount = prefab.m_ShapeCount;
			}
		}
		val.Dispose();
	}

	private void CleanUpRootMotion(CharacterStyle.AnimationMotion[] source, NativeArray<AnimationMotion> target)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < source.Length; i++)
		{
			CharacterStyle.AnimationMotion animationMotion = source[i];
			ref AnimationMotion reference = ref CollectionUtils.ElementAt<AnimationMotion>(target, i);
			reference.m_StartOffset = animationMotion.startOffset;
			reference.m_EndOffset = animationMotion.endOffset;
			reference.m_StartRotation = animationMotion.startRotation;
			reference.m_EndRotation = animationMotion.endRotation;
			if (i != 0)
			{
				ref AnimationMotion reference2 = ref CollectionUtils.ElementAt<AnimationMotion>(target, 0);
				ref float3 startOffset = ref reference.m_StartOffset;
				startOffset -= reference2.m_StartOffset;
				reference.m_StartRotation = math.mul(reference.m_StartRotation, math.inverse(reference2.m_StartRotation));
				ref float3 endOffset = ref reference.m_EndOffset;
				endOffset -= reference2.m_EndOffset;
				reference.m_EndRotation = math.mul(reference.m_EndRotation, math.inverse(reference2.m_EndRotation));
			}
			reference.m_StartOffset.y = 0f;
			reference.m_EndOffset.y = 0f;
			float3 val = math.forward(reference.m_StartRotation);
			float3 val2 = math.forward(reference.m_EndRotation);
			val.y = 0f;
			val2.y = 0f;
			reference.m_StartRotation = quaternion.LookRotationSafe(val, math.up());
			reference.m_EndRotation = quaternion.LookRotationSafe(val2, math.up());
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
	public AnimatedPrefabSystem()
	{
	}
}
