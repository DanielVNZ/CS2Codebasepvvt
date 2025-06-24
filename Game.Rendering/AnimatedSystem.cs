using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Animations;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class AnimatedSystem : GameSystemBase, IPreDeserialize
{
	public class Prepare : GameSystemBase
	{
		private AnimatedSystem m_AnimatedSystem;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			m_AnimatedSystem.m_CurrentTime = (m_AnimatedSystem.m_CurrentTime + m_AnimatedSystem.m_RenderingSystem.lodTimerDelta) & 0xFFFF;
			JobHandle val = m_AnimatedSystem.m_AllocateDeps;
			if (m_AnimatedSystem.m_IsAllocating)
			{
				JobHandle allocateDeps = IJobExtensions.Schedule<EndAllocationJob>(new EndAllocationJob
				{
					m_BoneAllocator = m_AnimatedSystem.m_BoneAllocator,
					m_MetaBufferData = m_AnimatedSystem.m_MetaBufferData,
					m_FreeMetaIndices = m_AnimatedSystem.m_FreeMetaIndices,
					m_UpdatedMetaIndices = m_AnimatedSystem.m_UpdatedMetaIndices,
					m_BoneAllocationRemoves = m_AnimatedSystem.m_BoneAllocationRemoves,
					m_MetaBufferRemoves = m_AnimatedSystem.m_MetaBufferRemoves,
					m_CurrentTime = m_AnimatedSystem.m_CurrentTime
				}, val);
				m_AnimatedSystem.m_AllocateDeps = allocateDeps;
			}
			if (m_AnimatedSystem.m_TempAnimationQueue.IsCreated)
			{
				JobHandle val2 = IJobExtensions.Schedule<AddAnimationInstancesJob>(new AddAnimationInstancesJob
				{
					m_AnimationFrameData = m_AnimatedSystem.m_TempAnimationQueue,
					m_InstanceIndices = m_AnimatedSystem.m_InstanceIndices,
					m_BodyInstances = m_AnimatedSystem.m_BodyInstances,
					m_FaceInstances = m_AnimatedSystem.m_FaceInstances,
					m_CorrectiveIndices = m_AnimatedSystem.m_CorrectiveInstances,
					m_BodyTransitions = m_AnimatedSystem.m_BodyTransitions,
					m_BodyTransitions2 = m_AnimatedSystem.m_BodyTransitions2,
					m_FaceTransitions = m_AnimatedSystem.m_FaceTransitions
				}, val);
				m_AnimatedSystem.m_AllocateDeps = JobHandle.CombineDependencies(m_AnimatedSystem.m_AllocateDeps, val2);
				m_AnimatedSystem.m_TempAnimationQueue.Dispose(val2);
			}
			if (m_AnimatedSystem.m_TempPriorityQueue.IsCreated)
			{
				JobHandle val3 = IJobExtensions.Schedule<UpdateAnimationPriorityJob>(new UpdateAnimationPriorityJob
				{
					m_ClipPriorityData = m_AnimatedSystem.m_TempPriorityQueue,
					m_ClipPriorities = m_AnimatedSystem.m_ClipPriorities
				}, val);
				m_AnimatedSystem.m_AllocateDeps = JobHandle.CombineDependencies(m_AnimatedSystem.m_AllocateDeps, val3);
				m_AnimatedSystem.m_TempPriorityQueue.Dispose(val3);
			}
		}

		[Preserve]
		public Prepare()
		{
		}
	}

	[BurstCompile]
	private struct AddAnimationInstancesJob : IJob
	{
		public NativeQueue<AnimationFrameData> m_AnimationFrameData;

		public NativeList<RestPoseInstance> m_InstanceIndices;

		public NativeList<AnimatedInstance> m_BodyInstances;

		public NativeList<AnimatedInstance> m_FaceInstances;

		public NativeList<AnimatedInstance> m_CorrectiveIndices;

		public NativeList<AnimatedTransition> m_BodyTransitions;

		public NativeList<AnimatedTransition2> m_BodyTransitions2;

		public NativeList<AnimatedTransition> m_FaceTransitions;

		public void Execute()
		{
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			m_InstanceIndices.Clear();
			m_CorrectiveIndices.Clear();
			m_BodyInstances.Clear();
			m_FaceInstances.Clear();
			m_BodyTransitions.Clear();
			m_BodyTransitions2.Clear();
			m_FaceTransitions.Clear();
			AnimationFrameData animationFrameData = default(AnimationFrameData);
			while (m_AnimationFrameData.TryDequeue(ref animationFrameData))
			{
				ref NativeList<RestPoseInstance> reference = ref m_InstanceIndices;
				RestPoseInstance restPoseInstance = new RestPoseInstance
				{
					m_MetaIndex = animationFrameData.m_MetaIndex,
					m_RestPoseIndex = animationFrameData.m_RestPoseIndex,
					m_ResetHistory = animationFrameData.m_ResetHistory
				};
				reference.Add(ref restPoseInstance);
				if (animationFrameData.m_CorrectiveIndex >= 0)
				{
					ref NativeList<AnimatedInstance> reference2 = ref m_CorrectiveIndices;
					AnimatedInstance animatedInstance = new AnimatedInstance
					{
						m_MetaIndex = animationFrameData.m_MetaIndex,
						m_CurrentIndex = animationFrameData.m_CorrectiveIndex
					};
					reference2.Add(ref animatedInstance);
				}
				if (animationFrameData.m_BodyData.m_CurrentIndex >= 0)
				{
					if (animationFrameData.m_BodyData.m_TransitionIndex.x >= 0)
					{
						if (animationFrameData.m_BodyData.m_TransitionIndex.y >= 0)
						{
							ref NativeList<AnimatedTransition2> reference3 = ref m_BodyTransitions2;
							AnimatedTransition2 animatedTransition = new AnimatedTransition2
							{
								m_MetaIndex = animationFrameData.m_MetaIndex,
								m_CurrentIndex = animationFrameData.m_BodyData.m_CurrentIndex,
								m_CurrentFrame = animationFrameData.m_BodyData.m_CurrentFrame,
								m_TransitionIndex = animationFrameData.m_BodyData.m_TransitionIndex,
								m_TransitionFrame = animationFrameData.m_BodyData.m_TransitionFrame,
								m_TransitionWeight = animationFrameData.m_BodyData.m_TransitionWeight
							};
							reference3.Add(ref animatedTransition);
						}
						else
						{
							ref NativeList<AnimatedTransition> reference4 = ref m_BodyTransitions;
							AnimatedTransition animatedTransition2 = new AnimatedTransition
							{
								m_MetaIndex = animationFrameData.m_MetaIndex,
								m_CurrentIndex = animationFrameData.m_BodyData.m_CurrentIndex,
								m_TransitionIndex = animationFrameData.m_BodyData.m_TransitionIndex.x,
								m_CurrentFrame = animationFrameData.m_BodyData.m_CurrentFrame,
								m_TransitionFrame = animationFrameData.m_BodyData.m_TransitionFrame.x,
								m_TransitionWeight = animationFrameData.m_BodyData.m_TransitionWeight.x
							};
							reference4.Add(ref animatedTransition2);
						}
					}
					else
					{
						ref NativeList<AnimatedInstance> reference5 = ref m_BodyInstances;
						AnimatedInstance animatedInstance = new AnimatedInstance
						{
							m_MetaIndex = animationFrameData.m_MetaIndex,
							m_CurrentIndex = animationFrameData.m_BodyData.m_CurrentIndex,
							m_CurrentFrame = animationFrameData.m_BodyData.m_CurrentFrame
						};
						reference5.Add(ref animatedInstance);
					}
				}
				if (animationFrameData.m_FaceData.m_CurrentIndex >= 0)
				{
					if (animationFrameData.m_FaceData.m_TransitionIndex >= 0)
					{
						ref NativeList<AnimatedTransition> reference6 = ref m_FaceTransitions;
						AnimatedTransition animatedTransition2 = new AnimatedTransition
						{
							m_MetaIndex = animationFrameData.m_MetaIndex,
							m_CurrentIndex = animationFrameData.m_FaceData.m_CurrentIndex,
							m_TransitionIndex = animationFrameData.m_FaceData.m_TransitionIndex,
							m_CurrentFrame = animationFrameData.m_FaceData.m_CurrentFrame,
							m_TransitionFrame = animationFrameData.m_FaceData.m_TransitionFrame,
							m_TransitionWeight = animationFrameData.m_FaceData.m_TransitionWeight
						};
						reference6.Add(ref animatedTransition2);
					}
					else
					{
						ref NativeList<AnimatedInstance> reference7 = ref m_FaceInstances;
						AnimatedInstance animatedInstance = new AnimatedInstance
						{
							m_MetaIndex = animationFrameData.m_MetaIndex,
							m_CurrentIndex = animationFrameData.m_FaceData.m_CurrentIndex,
							m_CurrentFrame = animationFrameData.m_FaceData.m_CurrentFrame
						};
						reference7.Add(ref animatedInstance);
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct UpdateAnimationPriorityJob : IJob
	{
		public NativeQueue<ClipPriorityData> m_ClipPriorityData;

		public NativeList<ClipPriorityData> m_ClipPriorities;

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			NativeHashMap<ClipIndex, int> val = default(NativeHashMap<ClipIndex, int>);
			val._002Ector(m_ClipPriorities.Length + 10, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_ClipPriorities.Length; i++)
			{
				ref ClipPriorityData reference = ref m_ClipPriorities.ElementAt(i);
				reference.m_Priority = math.max(reference.m_Priority - 1, -1000000);
				val.Add(reference.m_ClipIndex, i);
			}
			ClipPriorityData clipPriorityData = default(ClipPriorityData);
			int num = default(int);
			while (m_ClipPriorityData.TryDequeue(ref clipPriorityData))
			{
				if (val.TryGetValue(clipPriorityData.m_ClipIndex, ref num))
				{
					ref ClipPriorityData reference2 = ref m_ClipPriorities.ElementAt(num);
					reference2.m_Priority = math.max(reference2.m_Priority, clipPriorityData.m_Priority);
				}
				else
				{
					val.Add(clipPriorityData.m_ClipIndex, m_ClipPriorities.Length);
					m_ClipPriorities.Add(ref clipPriorityData);
				}
			}
			val.Dispose();
			for (int j = 0; j < m_ClipPriorities.Length; j++)
			{
				ClipPriorityData clipPriorityData2 = m_ClipPriorities[j];
				if (clipPriorityData2.m_Priority < 0 && !clipPriorityData2.m_IsLoading && !clipPriorityData2.m_IsLoaded)
				{
					m_ClipPriorities.RemoveAtSwapBack(j--);
				}
			}
			NativeSortExtension.Sort<ClipPriorityData>(m_ClipPriorities);
		}
	}

	public struct AnimationData
	{
		private ParallelWriter<AnimationFrameData> m_AnimationFrameData;

		private ParallelWriter<ClipPriorityData> m_ClipPriorityData;

		public AnimationData(NativeQueue<AnimationFrameData> animationFrameData, NativeQueue<ClipPriorityData> clipPriorityData)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			m_AnimationFrameData = animationFrameData.AsParallelWriter();
			m_ClipPriorityData = clipPriorityData.AsParallelWriter();
		}

		public void SetAnimationFrame(in CharacterElement characterElement, DynamicBuffer<AnimationClip> clips, in Animated animated, float2 transition, int priority, bool reset)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			AnimationClip clip = GetClip(characterElement.m_Style, clips, characterElement.m_RestPoseClipIndex, priority + 2);
			AnimationClip clipData = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexBody0, priority + 1);
			AnimationClip clipData0I = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexBody0I, priority + 1);
			AnimationClip clipData2 = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexBody1, priority + 1);
			AnimationClip clipData1I = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexBody1I, priority + 1);
			AnimationClip clipData3 = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexFace0, priority);
			AnimationClip clipData4 = GetClip(characterElement.m_Style, clips, animated.m_ClipIndexFace1, priority);
			AnimationClip clip2 = GetClip(characterElement.m_Style, clips, characterElement.m_CorrectiveClipIndex, priority);
			AnimationFrameData animationFrameData = new AnimationFrameData
			{
				m_MetaIndex = animated.m_MetaIndex,
				m_RestPoseIndex = clip.m_InfoIndex,
				m_ResetHistory = math.select(0, 1, reset),
				m_CorrectiveIndex = clip2.m_InfoIndex,
				m_BodyData = new AnimationLayerData2
				{
					m_CurrentIndex = -1,
					m_TransitionIndex = int2.op_Implicit(-1)
				},
				m_FaceData = new AnimationLayerData
				{
					m_CurrentIndex = -1,
					m_TransitionIndex = -1
				}
			};
			if (clipData.m_InfoIndex < 0)
			{
				if (!reset)
				{
					return;
				}
			}
			else
			{
				ref AnimationLayerData2 layerData = ref animationFrameData.m_BodyData;
				float4 time = animated.m_Time;
				SetLayerData(ref layerData, in clipData, in clipData0I, in clipData2, in clipData1I, ((float4)(ref time)).xy, float2.op_Implicit(animated.m_Interpolation), transition.x);
				if (clipData3.m_InfoIndex >= 0)
				{
					ref AnimationLayerData layerData2 = ref animationFrameData.m_FaceData;
					time = animated.m_Time;
					SetLayerData(ref layerData2, in clipData3, in clipData4, ((float4)(ref time)).zw, transition.y);
				}
			}
			m_AnimationFrameData.Enqueue(animationFrameData);
		}

		private AnimationClip GetClip(Entity style, DynamicBuffer<AnimationClip> clips, int index, int priority)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (index != -1)
			{
				AnimationClip result = clips[index];
				if (priority >= 0)
				{
					m_ClipPriorityData.Enqueue(new ClipPriorityData
					{
						m_ClipIndex = new ClipIndex(style, index),
						m_Priority = priority
					});
					if (result.m_PropClipIndex >= 0 && result.m_TargetValue == float.MinValue)
					{
						m_ClipPriorityData.Enqueue(new ClipPriorityData
						{
							m_ClipIndex = new ClipIndex(style, result.m_PropClipIndex),
							m_Priority = priority
						});
					}
				}
				return result;
			}
			return new AnimationClip
			{
				m_InfoIndex = -1
			};
		}

		private void SetLayerData(ref AnimationLayerData2 layerData, in AnimationClip clipData0, in AnimationClip clipData0I, in AnimationClip clipData1, in AnimationClip clipData1I, float2 time, float2 interpolation, float transition)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			SetLayerData(out layerData.m_CurrentIndex, out layerData.m_CurrentFrame, in clipData0, time.x);
			if (clipData1.m_InfoIndex >= 0)
			{
				SetLayerData(out layerData.m_TransitionIndex.x, out layerData.m_TransitionFrame.x, in clipData1, time.y);
				layerData.m_TransitionWeight.x = transition;
				if (clipData0I.m_InfoIndex >= 0)
				{
					SetLayerData(out layerData.m_TransitionIndex.y, out layerData.m_TransitionFrame.y, in clipData0I, time.x);
					if (clipData1I.m_InfoIndex == clipData0I.m_InfoIndex)
					{
						layerData.m_TransitionWeight.y = math.csum(interpolation) * 0.5f;
					}
					else
					{
						layerData.m_TransitionWeight.y = interpolation.x * (1f - transition);
					}
				}
				else if (clipData1I.m_InfoIndex >= 0)
				{
					SetLayerData(out layerData.m_TransitionIndex.y, out layerData.m_TransitionFrame.y, in clipData1I, time.y);
					layerData.m_TransitionWeight.y = interpolation.y * transition;
				}
			}
			else if (clipData0I.m_InfoIndex >= 0)
			{
				SetLayerData(out layerData.m_TransitionIndex.x, out layerData.m_TransitionFrame.x, in clipData0I, time.x);
				layerData.m_TransitionWeight.x = interpolation.x;
			}
		}

		private void SetLayerData(ref AnimationLayerData layerData, in AnimationClip clipData0, in AnimationClip clipData1, float2 time, float transition)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			SetLayerData(out layerData.m_CurrentIndex, out layerData.m_CurrentFrame, in clipData0, time.x);
			if (clipData1.m_InfoIndex >= 0)
			{
				SetLayerData(out layerData.m_TransitionIndex, out layerData.m_TransitionFrame, in clipData1, time.y);
				layerData.m_TransitionWeight = transition;
			}
		}

		private void SetLayerData(out int index, out float frame, in AnimationClip clipData, float time)
		{
			if (clipData.m_Playback != AnimationPlayback.Once && clipData.m_Playback != AnimationPlayback.OptionalOnce)
			{
				frame = math.fmod(time, clipData.m_AnimationLength);
				frame += math.select(0f, clipData.m_AnimationLength, frame < 0f);
			}
			else
			{
				frame = math.clamp(time, 0f, clipData.m_AnimationLength);
			}
			index = clipData.m_InfoIndex;
			frame *= clipData.m_FrameRate;
		}
	}

	[BurstCompile]
	private struct EndAllocationJob : IJob
	{
		public NativeHeapAllocator m_BoneAllocator;

		public NativeList<MetaBufferData> m_MetaBufferData;

		public NativeList<int> m_UpdatedMetaIndices;

		public NativeList<int> m_FreeMetaIndices;

		public NativeQueue<AllocationRemove> m_BoneAllocationRemoves;

		public NativeQueue<IndexRemove> m_MetaBufferRemoves;

		public int m_CurrentTime;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdatedMetaIndices.Length >= 2)
			{
				NativeSortExtension.Sort<int>(m_UpdatedMetaIndices);
			}
			while (!m_BoneAllocationRemoves.IsEmpty())
			{
				AllocationRemove allocationRemove = m_BoneAllocationRemoves.Peek();
				if (CheckTimeOffset(allocationRemove.m_RemoveTime))
				{
					break;
				}
				m_BoneAllocationRemoves.Dequeue();
				((NativeHeapAllocator)(ref m_BoneAllocator)).Release(allocationRemove.m_Allocation);
			}
			bool flag = false;
			while (!m_MetaBufferRemoves.IsEmpty())
			{
				IndexRemove indexRemove = m_MetaBufferRemoves.Peek();
				if (CheckTimeOffset(indexRemove.m_RemoveTime))
				{
					break;
				}
				m_MetaBufferRemoves.Dequeue();
				if (indexRemove.m_Index == m_MetaBufferData.Length - 1)
				{
					m_MetaBufferData.RemoveAt(indexRemove.m_Index);
					continue;
				}
				m_FreeMetaIndices.Add(ref indexRemove.m_Index);
				flag = true;
			}
			if (flag)
			{
				if (m_FreeMetaIndices.Length >= 2)
				{
					NativeSortExtension.Sort<int, ReverseIntComparer>(m_FreeMetaIndices, default(ReverseIntComparer));
				}
				int num = m_MetaBufferData.Length - 1;
				for (int i = 0; i < m_FreeMetaIndices.Length && m_FreeMetaIndices[i] == num; i++)
				{
					num--;
				}
				int num2 = m_MetaBufferData.Length - 1 - num;
				m_MetaBufferData.RemoveRange(num + 1, num2);
				m_FreeMetaIndices.RemoveRange(0, num2);
			}
		}

		private bool CheckTimeOffset(int removeTime)
		{
			int num = m_CurrentTime - removeTime;
			num += math.select(0, 65536, num < 0);
			return num < 255;
		}
	}

	public struct IndexRemove
	{
		public int m_Index;

		public int m_RemoveTime;
	}

	public struct AllocationRemove
	{
		public NativeHeapBlock m_Allocation;

		public int m_RemoveTime;
	}

	public struct AllocationData
	{
		private NativeHeapAllocator m_BoneAllocator;

		private NativeList<MetaBufferData> m_MetaBufferData;

		private NativeList<int> m_FreeMetaIndices;

		private NativeList<int> m_UpdatedMetaIndices;

		private NativeQueue<AllocationRemove> m_BoneAllocationRemoves;

		private NativeQueue<IndexRemove> m_MetaBufferRemoves;

		private int m_CurrentTime;

		public AllocationData(NativeHeapAllocator boneAllocator, NativeList<MetaBufferData> metaBufferData, NativeList<int> freeMetaIndices, NativeList<int> updatedMetaIndices, NativeQueue<AllocationRemove> boneAllocationRemoves, NativeQueue<IndexRemove> metaBufferRemoves, int currentTime)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			m_BoneAllocator = boneAllocator;
			m_MetaBufferData = metaBufferData;
			m_FreeMetaIndices = freeMetaIndices;
			m_UpdatedMetaIndices = updatedMetaIndices;
			m_BoneAllocationRemoves = boneAllocationRemoves;
			m_MetaBufferRemoves = metaBufferRemoves;
			m_CurrentTime = currentTime;
		}

		public NativeHeapBlock AllocateBones(int boneCount)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			NativeHeapBlock result = ((NativeHeapAllocator)(ref m_BoneAllocator)).Allocate((uint)boneCount, 1u);
			if (((NativeHeapBlock)(ref result)).Empty)
			{
				((NativeHeapAllocator)(ref m_BoneAllocator)).Resize(((NativeHeapAllocator)(ref m_BoneAllocator)).Size + 2097152u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>());
				result = ((NativeHeapAllocator)(ref m_BoneAllocator)).Allocate((uint)boneCount, 1u);
			}
			return result;
		}

		public void ReleaseBones(NativeHeapBlock allocation)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			m_BoneAllocationRemoves.Enqueue(new AllocationRemove
			{
				m_Allocation = allocation,
				m_RemoveTime = m_CurrentTime
			});
		}

		public int AddMetaBufferData(MetaBufferData metaBufferData)
		{
			int num;
			if (m_FreeMetaIndices.IsEmpty)
			{
				num = m_MetaBufferData.Length;
				m_MetaBufferData.Add(ref metaBufferData);
			}
			else
			{
				num = m_FreeMetaIndices[m_FreeMetaIndices.Length - 1];
				m_FreeMetaIndices.RemoveAt(m_FreeMetaIndices.Length - 1);
				m_MetaBufferData[num] = metaBufferData;
			}
			m_UpdatedMetaIndices.Add(ref num);
			return num;
		}

		public void RemoveMetaBufferData(int metaIndex)
		{
			m_MetaBufferRemoves.Enqueue(new IndexRemove
			{
				m_Index = metaIndex,
				m_RemoveTime = m_CurrentTime
			});
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ReverseIntComparer : IComparer<int>
	{
		public int Compare(int x, int y)
		{
			return y - x;
		}
	}

	public struct AnimationLayerData
	{
		public int m_CurrentIndex;

		public float m_CurrentFrame;

		public int m_TransitionIndex;

		public float m_TransitionFrame;

		public float m_TransitionWeight;
	}

	public struct AnimationLayerData2
	{
		public int m_CurrentIndex;

		public float m_CurrentFrame;

		public int2 m_TransitionIndex;

		public float2 m_TransitionFrame;

		public float2 m_TransitionWeight;
	}

	public struct AnimationFrameData
	{
		public int m_MetaIndex;

		public int m_RestPoseIndex;

		public int m_ResetHistory;

		public int m_CorrectiveIndex;

		public AnimationLayerData2 m_BodyData;

		public AnimationLayerData m_FaceData;
	}

	public struct ClipPriorityData : IComparable<ClipPriorityData>
	{
		public ClipIndex m_ClipIndex;

		public int m_Priority;

		public bool m_IsLoading;

		public bool m_IsLoaded;

		public int CompareTo(ClipPriorityData other)
		{
			return math.select(other.m_Priority - m_Priority, math.select(1, -1, m_IsLoading), m_IsLoading != other.m_IsLoading);
		}
	}

	public struct ClipIndex : IEquatable<ClipIndex>
	{
		public Entity m_Style;

		public int m_Index;

		public ClipIndex(Entity style, int clipIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Style = style;
			m_Index = clipIndex;
		}

		public bool Equals(ClipIndex other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Style)).Equals(other.m_Style))
			{
				return m_Index == other.m_Index;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Style)/*cast due to .constrained prefix*/).GetHashCode() * 5039 + m_Index;
		}
	}

	public struct AnimationClipData
	{
		public NativeHeapBlock m_AnimAllocation;

		public NativeHeapBlock m_HierarchyAllocation;

		public NativeHeapBlock m_ShapeAllocation;

		public NativeHeapBlock m_BoneAllocation;

		public NativeHeapBlock m_InverseBoneAllocation;
	}

	public const uint BONEBUFFER_MEMORY_DEFAULT = 8388608u;

	public const uint BONEBUFFER_MEMORY_INCREMENT = 2097152u;

	public const uint ANIMBUFFER_MEMORY_DEFAULT = 33554432u;

	public const uint ANIMBUFFER_MEMORY_INCREMENT = 8388608u;

	public const uint METABUFFER_MEMORY_DEFAULT = 1048576u;

	public const uint METABUFFER_MEMORY_INCREMENT = 262144u;

	public const uint INDEXBUFFER_MEMORY_DEFAULT = 65536u;

	public const uint INDEXBUFFER_MEMORY_INCREMENT = 16384u;

	public const uint MAX_ASYNC_LOADING_COUNT = 10u;

	private const string ANIMATION_COMPUTE_SHADER_RESOURCE = "Didimo/AnimationBlendCompute";

	private const string SHADER_BLEND_ANIMATION_LAYER0_KERNEL_NAME = "BlendAnimationLayer0";

	private const string SHADER_BLEND_ANIMATION_LAYER1_KERNEL_NAME = "BlendAnimationLayer1";

	private const string SHADER_BLEND_ANIMATION_LAYER2_KERNEL_NAME = "BlendAnimationLayer2";

	private const string SHADER_BLEND_TRANSITION_LAYER0_KERNEL_NAME = "BlendTransitionLayer0";

	private const string SHADER_BLEND_TRANSITION2_LAYER0_KERNEL_NAME = "BlendTransition2Layer0";

	private const string SHADER_BLEND_TRANSITION_LAYER1_KERNEL_NAME = "BlendTransitionLayer1";

	private const string SHADER_BLEND_REST_POSE_KERNEL_NAME = "BlendRestPose";

	private const string SHADER_CONVERT_COORDINATES_KERNEL_NAME = "ConvertLocalCoordinates";

	private const string SHADER_CONVERT_COORDINATES_WITH_HISTORY_KERNEL_NAME = "ConvertLocalCoordinatesWithHistory";

	private static ILog log;

	private RenderingSystem m_RenderingSystem;

	private PrefabSystem m_PrefabSystem;

	private NativeHeapAllocator m_BoneAllocator;

	private NativeHeapAllocator m_AnimAllocator;

	private NativeHeapAllocator m_IndexAllocator;

	private NativeList<MetaBufferData> m_MetaBufferData;

	private NativeList<int> m_FreeMetaIndices;

	private NativeList<int> m_UpdatedMetaIndices;

	private NativeList<RestPoseInstance> m_InstanceIndices;

	private NativeList<AnimatedInstance> m_BodyInstances;

	private NativeList<AnimatedInstance> m_FaceInstances;

	private NativeList<AnimatedInstance> m_CorrectiveInstances;

	private NativeList<AnimatedTransition> m_BodyTransitions;

	private NativeList<AnimatedTransition2> m_BodyTransitions2;

	private NativeList<AnimatedTransition> m_FaceTransitions;

	private NativeList<ClipPriorityData> m_ClipPriorities;

	private NativeList<AnimationClipData> m_AnimationClipData;

	private NativeList<int> m_FreeAnimIndices;

	private NativeQueue<AllocationRemove> m_BoneAllocationRemoves;

	private NativeQueue<IndexRemove> m_MetaBufferRemoves;

	private ComputeBuffer m_BoneBuffer;

	private ComputeBuffer m_BoneHistoryBuffer;

	private ComputeBuffer m_LocalTRSBlendPoseBuffer;

	private ComputeBuffer m_LocalTRSBoneBuffer;

	private ComputeBuffer m_AnimInfoBuffer;

	private ComputeBuffer m_AnimBuffer;

	private ComputeBuffer m_MetaBuffer;

	private ComputeBuffer m_IndexBuffer;

	private ComputeBuffer m_InstanceBuffer;

	private ComputeBuffer m_BodyInstanceBuffer;

	private ComputeBuffer m_FaceInstanceBuffer;

	private ComputeBuffer m_CorrectiveInstanceBuffer;

	private ComputeBuffer m_BodyTransitionBuffer;

	private ComputeBuffer m_BodyTransition2Buffer;

	private ComputeBuffer m_FaceTransitionBuffer;

	private int m_AnimationCount;

	private int m_MaxBoneCount;

	private int m_MaxActiveBoneCount;

	private int m_CurrentTime;

	private bool m_IsAllocating;

	private Dictionary<string, int> m_PropIDs;

	private ComputeShader m_AnimationComputeShader;

	private NativeQueue<AnimationFrameData> m_TempAnimationQueue;

	private NativeQueue<ClipPriorityData> m_TempPriorityQueue;

	private JobHandle m_AllocateDeps;

	private int m_BlendAnimationLayer0KernelIx;

	private int m_BlendAnimationLayer1KernelIx;

	private int m_BlendAnimationLayer2KernelIx;

	private int m_BlendTransitionLayer0KernelIx;

	private int m_BlendTransition2Layer0KernelIx;

	private int m_BlendTransitionLayer1KernelIx;

	private int m_BlendRestPoseKernelIx;

	private int m_ConvertLocalCoordinatesKernelIx;

	private int m_ConvertLocalCoordinatesWithHistoryKernelIx;

	private int m_IndexBufferID;

	private int m_MetadataBufferID;

	private int m_MetaIndexBufferID;

	private int m_AnimatedInstanceBufferID;

	private int m_AnimatedTransitionBufferID;

	private int m_AnimatedTransition2BufferID;

	private int m_AnimationInfoBufferID;

	private int m_AnimationBoneBufferID;

	private int m_InstanceCountID;

	private int m_BodyInstanceCountID;

	private int m_BodyTransitionCountID;

	private int m_BodyTransition2CountID;

	private int m_FaceInstanceCountID;

	private int m_FaceTransitionCountID;

	private int m_CorrectiveInstanceCountID;

	private int m_LocalTRSBlendPoseBufferID;

	private int m_LocalTRSBoneBufferID;

	private int m_BoneBufferID;

	private int m_BoneHistoryBufferID;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		log = LogManager.GetLogger("Rendering");
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BoneAllocator = new NativeHeapAllocator(8388608u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), 1u, (Allocator)4);
		m_AnimAllocator = new NativeHeapAllocator(33554432u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<Element>(), 1u, (Allocator)4);
		m_IndexAllocator = new NativeHeapAllocator(16384u, 1u, (Allocator)4);
		m_MetaBufferData = new NativeList<MetaBufferData>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_FreeMetaIndices = new NativeList<int>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_UpdatedMetaIndices = new NativeList<int>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_InstanceIndices = new NativeList<RestPoseInstance>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_BodyInstances = new NativeList<AnimatedInstance>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_FaceInstances = new NativeList<AnimatedInstance>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_CorrectiveInstances = new NativeList<AnimatedInstance>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_BodyTransitions = new NativeList<AnimatedTransition>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_BodyTransitions2 = new NativeList<AnimatedTransition2>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_FaceTransitions = new NativeList<AnimatedTransition>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_ClipPriorities = new NativeList<ClipPriorityData>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_AnimationClipData = new NativeList<AnimationClipData>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_FreeAnimIndices = new NativeList<int>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_BoneAllocationRemoves = new NativeQueue<AllocationRemove>(AllocatorHandle.op_Implicit((Allocator)4));
		m_MetaBufferRemoves = new NativeQueue<IndexRemove>(AllocatorHandle.op_Implicit((Allocator)4));
		m_PropIDs = new Dictionary<string, int>();
		m_AnimationComputeShader = Object.Instantiate<ComputeShader>(Resources.Load<ComputeShader>("Didimo/AnimationBlendCompute"));
		m_BlendAnimationLayer0KernelIx = m_AnimationComputeShader.FindKernel("BlendAnimationLayer0");
		m_BlendAnimationLayer1KernelIx = m_AnimationComputeShader.FindKernel("BlendAnimationLayer1");
		m_BlendAnimationLayer2KernelIx = m_AnimationComputeShader.FindKernel("BlendAnimationLayer2");
		m_BlendTransitionLayer0KernelIx = m_AnimationComputeShader.FindKernel("BlendTransitionLayer0");
		m_BlendTransition2Layer0KernelIx = m_AnimationComputeShader.FindKernel("BlendTransition2Layer0");
		m_BlendTransitionLayer1KernelIx = m_AnimationComputeShader.FindKernel("BlendTransitionLayer1");
		m_BlendRestPoseKernelIx = m_AnimationComputeShader.FindKernel("BlendRestPose");
		m_ConvertLocalCoordinatesKernelIx = m_AnimationComputeShader.FindKernel("ConvertLocalCoordinates");
		m_ConvertLocalCoordinatesWithHistoryKernelIx = m_AnimationComputeShader.FindKernel("ConvertLocalCoordinatesWithHistory");
		m_IndexBufferID = Shader.PropertyToID("IndexDataBuffer");
		m_MetadataBufferID = Shader.PropertyToID("MetaDataBuffer");
		m_MetaIndexBufferID = Shader.PropertyToID("MetaIndexBuffer");
		m_AnimatedInstanceBufferID = Shader.PropertyToID("AnimatedInstanceBuffer");
		m_AnimatedTransitionBufferID = Shader.PropertyToID("AnimatedTransitionBuffer");
		m_AnimatedTransition2BufferID = Shader.PropertyToID("AnimatedTransition2Buffer");
		m_AnimationInfoBufferID = Shader.PropertyToID("AnimationInfoBuffer");
		m_AnimationBoneBufferID = Shader.PropertyToID("AnimationBoneBuffer");
		m_InstanceCountID = Shader.PropertyToID("instanceCount");
		m_BodyInstanceCountID = Shader.PropertyToID("bodyInstanceCount");
		m_BodyTransitionCountID = Shader.PropertyToID("bodyTransitionCount");
		m_BodyTransition2CountID = Shader.PropertyToID("bodyTransition2Count");
		m_FaceInstanceCountID = Shader.PropertyToID("faceInstanceCount");
		m_FaceTransitionCountID = Shader.PropertyToID("faceTransitionCount");
		m_CorrectiveInstanceCountID = Shader.PropertyToID("correctiveInstanceCount");
		m_LocalTRSBlendPoseBufferID = Shader.PropertyToID("LocalTRSBlendPoseBuffer");
		m_LocalTRSBoneBufferID = Shader.PropertyToID("LocalTRSBoneBuffer");
		m_BoneBufferID = Shader.PropertyToID("BoneBuffer");
		m_BoneHistoryBufferID = Shader.PropertyToID("BoneHistoryBuffer");
		((NativeHeapAllocator)(ref m_BoneAllocator)).Allocate(1u, 1u);
		ref NativeList<MetaBufferData> reference = ref m_MetaBufferData;
		MetaBufferData metaBufferData = default(MetaBufferData);
		reference.Add(ref metaBufferData);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_IsAllocating)
		{
			((JobHandle)(ref m_AllocateDeps)).Complete();
			m_IsAllocating = false;
		}
		((NativeHeapAllocator)(ref m_BoneAllocator)).Dispose();
		((NativeHeapAllocator)(ref m_AnimAllocator)).Dispose();
		((NativeHeapAllocator)(ref m_IndexAllocator)).Dispose();
		m_MetaBufferData.Dispose();
		m_FreeMetaIndices.Dispose();
		m_UpdatedMetaIndices.Dispose();
		m_InstanceIndices.Dispose();
		m_BodyInstances.Dispose();
		m_FaceInstances.Dispose();
		m_CorrectiveInstances.Dispose();
		m_BodyTransitions.Dispose();
		m_BodyTransitions2.Dispose();
		m_FaceTransitions.Dispose();
		m_ClipPriorities.Dispose();
		m_AnimationClipData.Dispose();
		m_FreeAnimIndices.Dispose();
		m_BoneAllocationRemoves.Dispose();
		m_MetaBufferRemoves.Dispose();
		if (m_BoneBuffer != null)
		{
			m_BoneBuffer.Release();
		}
		if (m_BoneHistoryBuffer != null)
		{
			m_BoneHistoryBuffer.Release();
		}
		if (m_LocalTRSBlendPoseBuffer != null)
		{
			m_LocalTRSBlendPoseBuffer.Release();
		}
		if (m_LocalTRSBoneBuffer != null)
		{
			m_LocalTRSBoneBuffer.Release();
		}
		if (m_AnimInfoBuffer != null)
		{
			m_AnimInfoBuffer.Release();
		}
		if (m_AnimBuffer != null)
		{
			m_AnimBuffer.Release();
		}
		if (m_MetaBuffer != null)
		{
			m_MetaBuffer.Release();
		}
		if (m_IndexBuffer != null)
		{
			m_IndexBuffer.Release();
		}
		if (m_InstanceBuffer != null)
		{
			m_InstanceBuffer.Release();
		}
		if (m_BodyInstanceBuffer != null)
		{
			m_BodyInstanceBuffer.Release();
		}
		if (m_FaceInstanceBuffer != null)
		{
			m_FaceInstanceBuffer.Release();
		}
		if (m_CorrectiveInstanceBuffer != null)
		{
			m_CorrectiveInstanceBuffer.Release();
		}
		if (m_BodyTransitionBuffer != null)
		{
			m_BodyTransitionBuffer.Release();
		}
		if (m_BodyTransition2Buffer != null)
		{
			m_BodyTransition2Buffer.Release();
		}
		if (m_FaceTransitionBuffer != null)
		{
			m_FaceTransitionBuffer.Release();
		}
		Object.DestroyImmediate((Object)(object)m_AnimationComputeShader);
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (m_IsAllocating)
		{
			((JobHandle)(ref m_AllocateDeps)).Complete();
			m_IsAllocating = false;
			ResizeBoneBuffer();
			ResizeMetaBuffer();
			UpdateAnimations();
			UpdateMetaData();
			UpdateInstances();
		}
		PlayAnimations();
	}

	private void PlayAnimations()
	{
		if (m_InstanceIndices.Length != 0 && m_MaxBoneCount != 0)
		{
			ResizeBoneHistoryBuffer();
			m_AnimationComputeShader.SetInt(m_InstanceCountID, m_InstanceIndices.Length);
			m_AnimationComputeShader.SetInt(m_BodyInstanceCountID, m_BodyInstances.Length);
			m_AnimationComputeShader.SetInt(m_BodyTransitionCountID, m_BodyTransitions.Length);
			m_AnimationComputeShader.SetInt(m_BodyTransition2CountID, m_BodyTransitions2.Length);
			m_AnimationComputeShader.SetInt(m_FaceInstanceCountID, m_FaceInstances.Length);
			m_AnimationComputeShader.SetInt(m_FaceTransitionCountID, m_FaceTransitions.Length);
			m_AnimationComputeShader.SetInt(m_CorrectiveInstanceCountID, m_CorrectiveInstances.Length);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_MetadataBufferID, m_MetaBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_MetaIndexBufferID, m_InstanceBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
			m_AnimationComputeShader.SetBuffer(m_BlendRestPoseKernelIx, m_IndexBufferID, m_IndexBuffer);
			if (m_BodyInstances.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_AnimatedInstanceBufferID, m_BodyInstanceBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer0KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			if (m_BodyTransitions.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_AnimatedTransitionBufferID, m_BodyTransitionBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer0KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			if (m_BodyTransitions2.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_AnimatedTransition2BufferID, m_BodyTransition2Buffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransition2Layer0KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			if (m_FaceInstances.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_AnimatedInstanceBufferID, m_FaceInstanceBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer1KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			if (m_FaceTransitions.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_AnimatedTransitionBufferID, m_FaceTransitionBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendTransitionLayer1KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			if (m_CorrectiveInstances.Length != 0)
			{
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_AnimationInfoBufferID, m_AnimInfoBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_AnimationBoneBufferID, m_AnimBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_MetadataBufferID, m_MetaBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_AnimatedInstanceBufferID, m_CorrectiveInstanceBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
				m_AnimationComputeShader.SetBuffer(m_BlendAnimationLayer2KernelIx, m_IndexBufferID, m_IndexBuffer);
			}
			int num = (m_RenderingSystem.motionVectors ? m_ConvertLocalCoordinatesWithHistoryKernelIx : m_ConvertLocalCoordinatesKernelIx);
			m_AnimationComputeShader.SetBuffer(num, m_MetadataBufferID, m_MetaBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_MetaIndexBufferID, m_InstanceBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_LocalTRSBoneBufferID, m_LocalTRSBoneBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_LocalTRSBlendPoseBufferID, m_LocalTRSBlendPoseBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_BoneBufferID, m_BoneBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_IndexBufferID, m_IndexBuffer);
			m_AnimationComputeShader.SetBuffer(num, m_AnimationInfoBufferID, m_AnimInfoBuffer);
			if (m_RenderingSystem.motionVectors)
			{
				m_AnimationComputeShader.SetBuffer(num, m_BoneHistoryBufferID, m_BoneHistoryBuffer);
			}
			else
			{
				m_AnimationComputeShader.SetBuffer(num, m_BoneHistoryBufferID, m_BoneBuffer);
			}
			uint num2 = default(uint);
			uint num3 = default(uint);
			uint num4 = default(uint);
			m_AnimationComputeShader.GetKernelThreadGroupSizes(m_BlendRestPoseKernelIx, ref num2, ref num3, ref num4);
			int num5 = (m_InstanceIndices.Length + (int)num2 - 1) / (int)num2;
			int num6 = (m_MaxBoneCount + (int)num3 - 1) / (int)num3;
			int num7 = (m_MaxActiveBoneCount + (int)num3 - 1) / (int)num3;
			m_AnimationComputeShader.Dispatch(m_BlendRestPoseKernelIx, num5, num6, 1);
			if (m_BodyInstances.Length != 0)
			{
				int num8 = (m_BodyInstances.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendAnimationLayer0KernelIx, num8, num7, 1);
			}
			if (m_BodyTransitions.Length != 0)
			{
				int num9 = (m_BodyTransitions.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendTransitionLayer0KernelIx, num9, num6, 1);
			}
			if (m_BodyTransitions2.Length != 0)
			{
				int num10 = (m_BodyTransitions2.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendTransition2Layer0KernelIx, num10, num6, 1);
			}
			if (m_FaceInstances.Length != 0)
			{
				int num11 = (m_FaceInstances.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendAnimationLayer1KernelIx, num11, num7, 1);
			}
			if (m_FaceTransitions.Length != 0)
			{
				int num12 = (m_FaceTransitions.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendTransitionLayer1KernelIx, num12, num6, 1);
			}
			if (m_CorrectiveInstances.Length != 0)
			{
				int num13 = (m_CorrectiveInstances.Length + (int)num2 - 1) / (int)num2;
				m_AnimationComputeShader.Dispatch(m_BlendAnimationLayer2KernelIx, num13, num7, 1);
			}
			m_AnimationComputeShader.Dispatch(num, num5, num6, 1);
		}
	}

	private void UpdateInstances()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		UpdateInstanceBuffer<RestPoseInstance>(ref m_InstanceBuffer, m_InstanceIndices, "InstanceBuffer");
		UpdateInstanceBuffer<AnimatedInstance>(ref m_BodyInstanceBuffer, m_BodyInstances, "BodyInstanceBuffer");
		UpdateInstanceBuffer<AnimatedInstance>(ref m_FaceInstanceBuffer, m_FaceInstances, "FaceInstanceBuffer");
		UpdateInstanceBuffer<AnimatedInstance>(ref m_CorrectiveInstanceBuffer, m_CorrectiveInstances, "CorrectiveInstanceBuffer");
		UpdateInstanceBuffer<AnimatedTransition>(ref m_BodyTransitionBuffer, m_BodyTransitions, "BodyTransitionBuffer");
		UpdateInstanceBuffer<AnimatedTransition2>(ref m_BodyTransition2Buffer, m_BodyTransitions2, "BodyTransitionBuffer2");
		UpdateInstanceBuffer<AnimatedTransition>(ref m_FaceTransitionBuffer, m_FaceTransitions, "FaceTransitionBuffer");
	}

	private unsafe void UpdateInstanceBuffer<T>(ref ComputeBuffer buffer, NativeList<T> data, string name) where T : unmanaged
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (buffer == null || buffer.count != data.Capacity)
		{
			if (buffer != null)
			{
				buffer.Release();
			}
			buffer = new ComputeBuffer(data.Capacity, sizeof(T), (ComputeBufferType)16);
			buffer.name = name;
		}
		if (data.Length != 0)
		{
			buffer.SetData<T>(data.AsArray(), 0, 0, data.Length);
		}
	}

	private void UpdateAnimations()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		AnimationClip animation2 = default(AnimationClip);
		for (int i = 0; i < m_ClipPriorities.Length; i++)
		{
			ref ClipPriorityData reference = ref m_ClipPriorities.ElementAt(i);
			if (reference.m_Priority < 0 && !reference.m_IsLoading)
			{
				break;
			}
			if (reference.m_IsLoaded)
			{
				continue;
			}
			CharacterStyle prefab = m_PrefabSystem.GetPrefab<CharacterStyle>(reference.m_ClipIndex.m_Style);
			AnimationAsset animation = prefab.GetAnimation(reference.m_ClipIndex.m_Index);
			try
			{
				reference.m_IsLoading = true;
				if (animation.AsyncLoad(ref animation2))
				{
					if (LoadAnimation(reference.m_ClipIndex, animation2))
					{
						reference.m_IsLoading = false;
						reference.m_IsLoaded = true;
						((AssetData)animation).Unload(false);
					}
					else if (reference.m_Priority < 0)
					{
						reference.m_IsLoading = false;
						((AssetData)animation).Unload(false);
					}
				}
				else if ((long)(++num) == 10)
				{
					break;
				}
			}
			catch (Exception ex)
			{
				log.ErrorFormat(ex, "Error when loading animation: {0}->{1}", (object)((Object)prefab).name, (object)((AssetData)animation).name);
				reference.m_IsLoading = false;
				reference.m_IsLoaded = true;
				((AssetData)animation).Unload(false);
			}
		}
	}

	private bool LoadAnimation(ClipIndex clipIndex, AnimationClip animation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Expected I4, but got Unknown
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		CharacterStyleData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CharacterStyleData>(clipIndex.m_Style);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AnimationClip> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AnimationClip>(clipIndex.m_Style, false);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<AnimationMotion> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<AnimationMotion>(clipIndex.m_Style, true);
		ref AnimationClip reference = ref buffer.ElementAt(clipIndex.m_Index);
		DynamicBuffer<RestPoseElement> restPose = default(DynamicBuffer<RestPoseElement>);
		if (reference.m_RootMotionBone != -1 && (!EntitiesExtensions.TryGetBuffer<RestPoseElement>(((ComponentSystemBase)this).EntityManager, clipIndex.m_Style, true, ref restPose) || restPose.Length == 0))
		{
			return false;
		}
		uint num = (uint)animation.m_Animation.elements.Length;
		AnimationClipData animationClipData = new AnimationClipData
		{
			m_AnimAllocation = ((NativeHeapAllocator)(ref m_AnimAllocator)).Allocate(num, 1u)
		};
		m_AnimationCount++;
		int num2 = m_ClipPriorities.Length - 1;
		while (((NativeHeapBlock)(ref animationClipData.m_AnimAllocation)).Empty)
		{
			if (num2 >= 0)
			{
				ref ClipPriorityData reference2 = ref m_ClipPriorities.ElementAt(num2--);
				if (reference2.m_Priority >= 0)
				{
					num2 = -1;
					continue;
				}
				if (reference2.m_ClipIndex.Equals(clipIndex) || reference2.m_IsLoading || !reference2.m_IsLoaded)
				{
					continue;
				}
				reference2.m_IsLoaded = false;
				UnloadAnimation(reference2.m_ClipIndex);
			}
			else
			{
				uint num3 = 8388608u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<Element>();
				num3 = (num3 + num - 1) / num3 * num3;
				((NativeHeapAllocator)(ref m_AnimAllocator)).Resize(((NativeHeapAllocator)(ref m_AnimAllocator)).Size + num3);
			}
			animationClipData.m_AnimAllocation = ((NativeHeapAllocator)(ref m_AnimAllocator)).Allocate(num, 1u);
		}
		if (componentData.m_RestPoseClipIndex == clipIndex.m_Index)
		{
			animationClipData.m_HierarchyAllocation = AllocateIndexData((uint)animation.m_BoneHierarchy.hierarchyParentIndices.Length);
			CacheRestPose(clipIndex.m_Style, animation);
		}
		animationClipData.m_ShapeAllocation = AllocateIndexData((uint)componentData.m_ShapeCount);
		animationClipData.m_BoneAllocation = AllocateIndexData((uint)animation.m_Animation.boneIndices.Length);
		animationClipData.m_InverseBoneAllocation = AllocateIndexData((uint)componentData.m_BoneCount);
		m_MaxBoneCount = math.max(m_MaxBoneCount, animation.m_BoneHierarchy.hierarchyParentIndices.Length);
		m_MaxActiveBoneCount = math.max(m_MaxActiveBoneCount, animation.m_Animation.boneIndices.Length);
		if (m_FreeAnimIndices.IsEmpty)
		{
			reference.m_InfoIndex = m_AnimationClipData.Length;
			m_AnimationClipData.Add(ref animationClipData);
		}
		else
		{
			reference.m_InfoIndex = m_FreeAnimIndices[m_FreeAnimIndices.Length - 1];
			m_FreeAnimIndices.RemoveAt(m_FreeAnimIndices.Length - 1);
			m_AnimationClipData[reference.m_InfoIndex] = animationClipData;
		}
		ResizeAnimInfoBuffer();
		ResizeAnimBuffer();
		ResizeIndexBuffer();
		NativeArray<AnimationInfoData> val = default(NativeArray<AnimationInfoData>);
		val._002Ector(1, (Allocator)2, (NativeArrayOptions)1);
		val[0] = new AnimationInfoData
		{
			m_Offset = (int)((NativeHeapBlock)(ref animationClipData.m_AnimAllocation)).Begin,
			m_Hierarchy = (((NativeHeapBlock)(ref animationClipData.m_HierarchyAllocation)).Empty ? (-1) : ((int)((NativeHeapBlock)(ref animationClipData.m_HierarchyAllocation)).Begin)),
			m_Shapes = (int)((NativeHeapBlock)(ref animationClipData.m_ShapeAllocation)).Begin,
			m_Bones = (int)((NativeHeapBlock)(ref animationClipData.m_BoneAllocation)).Begin,
			m_InverseBones = (int)((NativeHeapBlock)(ref animationClipData.m_InverseBoneAllocation)).Begin,
			m_ShapeCount = animation.m_Animation.shapeIndices.Length,
			m_BoneCount = animation.m_Animation.boneIndices.Length,
			m_Type = (int)animation.m_Animation.type,
			m_PositionMin = animation.m_Animation.positionMin,
			m_PositionRange = animation.m_Animation.positionRange
		};
		m_AnimInfoBuffer.SetData<AnimationInfoData>(val, 0, reference.m_InfoIndex, 1);
		val.Dispose();
		NativeArray<Element> val2 = default(NativeArray<Element>);
		val2._002Ector(animation.m_Animation.elements, (Allocator)2);
		if (reference.m_RootMotionBone != -1)
		{
			NativeArray<AnimationMotion> subArray = buffer2.AsNativeArray().GetSubArray(reference.m_MotionRange.x, reference.m_MotionRange.y - reference.m_MotionRange.x);
			RemoveRootMotion(animation, reference, restPose, subArray, val2);
		}
		if (reference.m_Layer == AnimationLayer.Prop && reference.m_TargetValue == float.MinValue)
		{
			reference.m_TargetValue = FindTargetValue(animation, reference, val2);
			for (int i = 0; i < buffer.Length; i++)
			{
				if (i != clipIndex.m_Index)
				{
					ref AnimationClip reference3 = ref buffer.ElementAt(i);
					if (reference3.m_PropClipIndex == clipIndex.m_Index)
					{
						reference3.m_TargetValue = reference.m_TargetValue;
					}
				}
			}
		}
		m_AnimBuffer.SetData<Element>(val2, 0, (int)((NativeHeapBlock)(ref animationClipData.m_AnimAllocation)).Begin, (int)num);
		val2.Dispose();
		if (!((NativeHeapBlock)(ref animationClipData.m_HierarchyAllocation)).Empty)
		{
			m_IndexBuffer.SetData((Array)animation.m_BoneHierarchy.hierarchyParentIndices, 0, (int)((NativeHeapBlock)(ref animationClipData.m_HierarchyAllocation)).Begin, animation.m_BoneHierarchy.hierarchyParentIndices.Length);
		}
		if (!((NativeHeapBlock)(ref animationClipData.m_ShapeAllocation)).Empty)
		{
			NativeArray<int> val3 = default(NativeArray<int>);
			val3._002Ector(componentData.m_ShapeCount, (Allocator)2, (NativeArrayOptions)1);
			for (int j = 0; j < val3.Length; j++)
			{
				val3[j] = -1;
			}
			for (int k = 0; k < animation.m_Animation.shapeIndices.Length; k++)
			{
				val3[animation.m_Animation.shapeIndices[k]] = k;
			}
			m_IndexBuffer.SetData<int>(val3, 0, (int)((NativeHeapBlock)(ref animationClipData.m_ShapeAllocation)).Begin, val3.Length);
			val3.Dispose();
		}
		if (!((NativeHeapBlock)(ref animationClipData.m_BoneAllocation)).Empty)
		{
			m_IndexBuffer.SetData((Array)animation.m_Animation.boneIndices, 0, (int)((NativeHeapBlock)(ref animationClipData.m_BoneAllocation)).Begin, animation.m_Animation.boneIndices.Length);
		}
		if (!((NativeHeapBlock)(ref animationClipData.m_InverseBoneAllocation)).Empty && reference.m_Layer != AnimationLayer.Prop)
		{
			NativeArray<int> val4 = default(NativeArray<int>);
			val4._002Ector(componentData.m_BoneCount, (Allocator)2, (NativeArrayOptions)1);
			for (int l = 0; l < val4.Length; l++)
			{
				val4[l] = -1;
			}
			for (int m = 0; m < animation.m_Animation.boneIndices.Length; m++)
			{
				val4[animation.m_Animation.boneIndices[m]] = m;
			}
			m_IndexBuffer.SetData<int>(val4, 0, (int)((NativeHeapBlock)(ref animationClipData.m_InverseBoneAllocation)).Begin, val4.Length);
			val4.Dispose();
		}
		return true;
	}

	private NativeHeapBlock AllocateIndexData(uint size)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		NativeHeapBlock result = ((NativeHeapAllocator)(ref m_IndexAllocator)).Allocate(size, 1u);
		while (((NativeHeapBlock)(ref result)).Empty)
		{
			uint num = 4096u;
			num = (num + size - 1) / num * num;
			((NativeHeapAllocator)(ref m_IndexAllocator)).Resize(((NativeHeapAllocator)(ref m_IndexAllocator)).Size + num);
			result = ((NativeHeapAllocator)(ref m_IndexAllocator)).Allocate(size, 1u);
		}
		return result;
	}

	private void UnloadAnimation(ClipIndex clipIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		CharacterStyleData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CharacterStyleData>(clipIndex.m_Style);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ref AnimationClip reference = ref ((EntityManager)(ref entityManager)).GetBuffer<AnimationClip>(clipIndex.m_Style, false).ElementAt(clipIndex.m_Index);
		if (reference.m_InfoIndex >= 0)
		{
			AnimationClipData animationClipData = m_AnimationClipData[reference.m_InfoIndex];
			if (componentData.m_RestPoseClipIndex == clipIndex.m_Index)
			{
				UnCacheRestPose(clipIndex.m_Style);
			}
			if (!((NativeHeapBlock)(ref animationClipData.m_AnimAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_AnimAllocator)).Release(animationClipData.m_AnimAllocation);
			}
			if (!((NativeHeapBlock)(ref animationClipData.m_HierarchyAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_IndexAllocator)).Release(animationClipData.m_HierarchyAllocation);
			}
			if (!((NativeHeapBlock)(ref animationClipData.m_ShapeAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_IndexAllocator)).Release(animationClipData.m_ShapeAllocation);
			}
			if (!((NativeHeapBlock)(ref animationClipData.m_BoneAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_IndexAllocator)).Release(animationClipData.m_BoneAllocation);
			}
			if (!((NativeHeapBlock)(ref animationClipData.m_InverseBoneAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_IndexAllocator)).Release(animationClipData.m_InverseBoneAllocation);
			}
			if (reference.m_InfoIndex == m_AnimationClipData.Length - 1)
			{
				m_AnimationClipData.RemoveAt(reference.m_InfoIndex);
			}
			else
			{
				m_FreeAnimIndices.Add(ref reference.m_InfoIndex);
			}
			reference.m_InfoIndex = -1;
			m_AnimationCount--;
		}
	}

	private void CacheRestPose(Entity style, AnimationClip restPose)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<RestPoseElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RestPoseElement>(style, false);
		buffer.ResizeUninitialized(restPose.m_Animation.elements.Length);
		for (int i = 0; i < buffer.Length; i++)
		{
			ElementRaw val = AnimationEncoding.DecodeElement(restPose.m_Animation.elements[i], restPose.m_Animation.positionMin, restPose.m_Animation.positionRange);
			buffer[i] = new RestPoseElement
			{
				m_Position = val.position,
				m_Rotation = quaternion.op_Implicit(val.rotation)
			};
		}
	}

	private void UnCacheRestPose(Entity style)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<RestPoseElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RestPoseElement>(style, false);
		buffer.Clear();
		buffer.TrimExcess();
	}

	private float FindTargetValue(AnimationClip animation, AnimationClip animationClip, NativeArray<Element> elements)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (animationClip.m_Activity == ActivityType.Driving)
		{
			AnimationType type = animationClip.m_Type;
			if ((uint)(type - 6) <= 3u)
			{
				return FindTargetRotation(animation, elements);
			}
		}
		return 0f;
	}

	private float FindTargetRotation(AnimationClip animation, NativeArray<Element> elements)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		int num = animation.m_Animation.shapeIndices.Length;
		int num2 = animation.m_Animation.boneIndices.Length;
		float num3 = 0f;
		for (int i = 0; i < num2; i++)
		{
			ElementRaw val = AnimationEncoding.DecodeElement(CollectionUtils.ElementAt<Element>(elements, i * num), animation.m_Animation.positionMin, animation.m_Animation.positionRange);
			float num4 = MathUtils.RotationAngle(quaternion.identity, quaternion.op_Implicit(val.rotation));
			num3 = math.max(num3, num4);
		}
		return num3;
	}

	private void RemoveRootMotion(AnimationClip animation, AnimationClip animationClip, DynamicBuffer<RestPoseElement> restPose, NativeArray<AnimationMotion> motions, NativeArray<Element> elements)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		int[] inverseBoneIndices = animation.GetInverseBoneIndices();
		int num = animation.m_Animation.shapeIndices.Length;
		int num2 = animation.m_Animation.boneIndices.Length;
		int num3 = num * num2;
		int num4 = elements.Length / num3 - 1;
		int num5 = inverseBoneIndices.Length;
		int num6 = restPose.Length / num5;
		for (int i = 0; i <= num4; i++)
		{
			float num7 = math.select((float)i / (float)(num4 - 1), 0f, i >= num4);
			for (int j = 0; j < num; j++)
			{
				int num8 = animation.m_Animation.shapeIndices[j];
				AnimationMotion animationMotion = motions[num8];
				int num9 = inverseBoneIndices[animationClip.m_RootMotionBone];
				if (num9 < 0)
				{
					continue;
				}
				int num10 = i * num3 + num9 * num;
				ref Element reference = ref CollectionUtils.ElementAt<Element>(elements, num10 + j);
				ElementRaw val = AnimationEncoding.DecodeElement(reference, animation.m_Animation.positionMin, animation.m_Animation.positionRange);
				quaternion val2 = math.slerp(animationMotion.m_StartRotation, animationMotion.m_EndRotation, num7);
				float3 val3 = ((animationClip.m_Playback == AnimationPlayback.Once) ? MathUtils.Position(new Bezier4x3(animationMotion.m_StartOffset, animationMotion.m_StartOffset, animationMotion.m_EndOffset, animationMotion.m_EndOffset), num7) : math.lerp(animationMotion.m_StartOffset, animationMotion.m_EndOffset, num7));
				if (num8 != 0)
				{
					AnimationMotion animationMotion2 = motions[0];
					quaternion val4 = math.slerp(animationMotion2.m_StartRotation, animationMotion2.m_EndRotation, num7);
					float3 val5 = ((animationClip.m_Playback == AnimationPlayback.Once) ? MathUtils.Position(new Bezier4x3(animationMotion2.m_StartOffset, animationMotion2.m_StartOffset, animationMotion2.m_EndOffset, animationMotion2.m_EndOffset), num7) : math.lerp(animationMotion2.m_StartOffset, animationMotion2.m_EndOffset, num7));
					val3 += val5;
					val2 = math.mul(val2, val4);
				}
				for (int num11 = animation.m_BoneHierarchy.hierarchyParentIndices[animationClip.m_RootMotionBone]; num11 != -1; num11 = animation.m_BoneHierarchy.hierarchyParentIndices[num11])
				{
					num9 = inverseBoneIndices[num11];
					if (num9 >= 0)
					{
						num10 = i * num3 + num9 * num;
						ref Element reference2 = ref CollectionUtils.ElementAt<Element>(elements, num10 + j);
						ElementRaw val6 = AnimationEncoding.DecodeElement(reference2, animation.m_Animation.positionMin, animation.m_Animation.positionRange);
						val.position = val6.position + math.mul(quaternion.op_Implicit(val6.rotation), val.position);
						val.rotation = math.mul(quaternion.op_Implicit(val6.rotation), quaternion.op_Implicit(val.rotation)).value;
						reference = AnimationEncoding.EncodeElement(val, animation.m_Animation.positionMin, animation.m_Animation.positionRange);
						val6.position = float3.zero;
						val6.rotation = quaternion.identity.value;
						reference2 = AnimationEncoding.EncodeElement(val6, animation.m_Animation.positionMin, animation.m_Animation.positionRange);
					}
					else
					{
						int num12 = num11 * num6;
						RestPoseElement restPoseElement = restPose[num12 + num8];
						restPoseElement.m_Rotation = math.inverse(restPoseElement.m_Rotation);
						val3 = math.mul(restPoseElement.m_Rotation, val3 - restPoseElement.m_Position);
						val2 = math.normalize(math.mul(restPoseElement.m_Rotation, val2));
					}
				}
				val2 = math.inverse(val2);
				val.position = math.mul(val2, val.position - val3);
				val.rotation = math.normalize(math.mul(val2, quaternion.op_Implicit(val.rotation))).value;
				reference = AnimationEncoding.EncodeElement(val, animation.m_Animation.positionMin, animation.m_Animation.positionRange);
			}
		}
	}

	private void UpdateMetaData()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		while (num < m_UpdatedMetaIndices.Length)
		{
			int num2 = m_UpdatedMetaIndices[num++];
			int num3 = num2 + 1;
			while (num < m_UpdatedMetaIndices.Length)
			{
				int num4 = m_UpdatedMetaIndices[num];
				if (num4 != num3)
				{
					break;
				}
				num++;
				num3 = num4 + 1;
			}
			m_MetaBuffer.SetData<MetaBufferData>(m_MetaBufferData.AsArray(), num2, num2, num3 - num2);
		}
		m_UpdatedMetaIndices.Clear();
	}

	private void ResizeBoneBuffer()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		int num = ((m_BoneBuffer != null) ? m_BoneBuffer.count : 0);
		int size = (int)((NativeHeapAllocator)(ref m_BoneAllocator)).Size;
		if (num != size)
		{
			ComputeBuffer val = new ComputeBuffer(size, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
			val.name = "Bone buffer";
			Shader.SetGlobalBuffer("boneBuffer", val);
			if (m_BoneHistoryBuffer == null)
			{
				Shader.SetGlobalBuffer("boneHistoryBuffer", val);
			}
			if (m_BoneBuffer != null)
			{
				m_BoneBuffer.Release();
			}
			if (m_LocalTRSBlendPoseBuffer != null)
			{
				m_LocalTRSBlendPoseBuffer.Release();
			}
			if (m_LocalTRSBoneBuffer != null)
			{
				m_LocalTRSBoneBuffer.Release();
			}
			BoneElement[] array = new BoneElement[val.count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new BoneElement
				{
					m_Matrix = float4x4.identity
				};
			}
			val.SetData((Array)array);
			m_BoneBuffer = val;
			m_LocalTRSBlendPoseBuffer = new ComputeBuffer(size, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
			m_LocalTRSBoneBuffer = new ComputeBuffer(size, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
			m_LocalTRSBlendPoseBuffer.name = "LocalTRSBlendPoseBuffer";
			m_LocalTRSBoneBuffer.name = "LocalTRSBoneBuffer";
		}
	}

	private void ResizeBoneHistoryBuffer()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		int num = ((m_BoneHistoryBuffer != null) ? m_BoneHistoryBuffer.count : 0);
		int num2 = (int)(m_RenderingSystem.motionVectors ? ((NativeHeapAllocator)(ref m_BoneAllocator)).Size : 0);
		if (num == num2)
		{
			return;
		}
		if (num2 == 0)
		{
			if (m_BoneHistoryBuffer != null)
			{
				if (m_BoneHistoryBuffer != null)
				{
					m_BoneHistoryBuffer.Release();
				}
				m_BoneHistoryBuffer = null;
			}
			if (m_BoneBuffer != null)
			{
				Shader.SetGlobalBuffer("boneHistoryBuffer", m_BoneBuffer);
			}
			return;
		}
		ComputeBuffer val = new ComputeBuffer(num2, System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>(), (ComputeBufferType)16);
		val.name = "Bone history buffer";
		Shader.SetGlobalBuffer("boneHistoryBuffer", val);
		if (m_BoneHistoryBuffer != null)
		{
			m_BoneHistoryBuffer.Release();
		}
		BoneElement[] array = new BoneElement[val.count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new BoneElement
			{
				m_Matrix = float4x4.identity
			};
		}
		val.SetData((Array)array);
		m_BoneHistoryBuffer = val;
	}

	private void ResizeAnimInfoBuffer()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		int num = ((m_AnimInfoBuffer != null) ? m_AnimInfoBuffer.count : 0);
		int capacity = m_AnimationClipData.Capacity;
		if (num != capacity)
		{
			ComputeBuffer val = new ComputeBuffer(capacity, System.Runtime.CompilerServices.Unsafe.SizeOf<AnimationInfoData>(), (ComputeBufferType)16);
			val.name = "Animation info buffer";
			int num2 = math.min(num, capacity);
			if (num2 > 0)
			{
				AnimationInfoData[] array = new AnimationInfoData[num2];
				m_AnimInfoBuffer.GetData((Array)array, 0, 0, num2);
				val.SetData((Array)array);
			}
			if (m_AnimInfoBuffer != null)
			{
				m_AnimInfoBuffer.Release();
			}
			m_AnimInfoBuffer = val;
		}
	}

	private void ResizeAnimBuffer()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		int num = ((m_AnimBuffer != null) ? m_AnimBuffer.count : 0);
		int size = (int)((NativeHeapAllocator)(ref m_AnimAllocator)).Size;
		if (num != size)
		{
			ComputeBuffer val = new ComputeBuffer(size, System.Runtime.CompilerServices.Unsafe.SizeOf<Element>(), (ComputeBufferType)16);
			val.name = "Animation buffer";
			int num2 = math.min(num, size);
			if (num2 > 0)
			{
				Element[] array = (Element[])(object)new Element[num2];
				m_AnimBuffer.GetData((Array)array, 0, 0, num2);
				val.SetData((Array)array);
			}
			if (m_AnimBuffer != null)
			{
				m_AnimBuffer.Release();
			}
			m_AnimBuffer = val;
		}
	}

	private void ResizeMetaBuffer()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		int num = ((m_MetaBuffer != null) ? m_MetaBuffer.count : 0);
		int num2 = 1048576 / System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>();
		if (m_MetaBufferData.Length > num && m_MetaBufferData.Length > num2)
		{
			num2 += ((m_MetaBufferData.Length - num2) * System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>() + 262144 - 1) / 262144 * 262144 / System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>();
		}
		else if (num > num2)
		{
			num2 = num;
		}
		if (num != num2)
		{
			ComputeBuffer val = new ComputeBuffer(num2, System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>(), (ComputeBufferType)16);
			val.name = "Meta buffer";
			Shader.SetGlobalBuffer("metaBuffer", val);
			if (m_MetaBuffer != null)
			{
				val.SetData<MetaBufferData>(m_MetaBufferData.AsArray(), 0, 0, num);
				m_MetaBuffer.Release();
			}
			else
			{
				val.SetData<MetaBufferData>(m_MetaBufferData.AsArray(), 0, 0, 1);
			}
			m_MetaBuffer = val;
		}
	}

	private void ResizeIndexBuffer()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		int num = ((m_IndexBuffer != null) ? m_IndexBuffer.count : 0);
		int size = (int)((NativeHeapAllocator)(ref m_IndexAllocator)).Size;
		if (num != size)
		{
			ComputeBuffer val = new ComputeBuffer(size, 4, (ComputeBufferType)16);
			val.name = "Index buffer";
			int num2 = math.min(num, size);
			if (num2 > 0)
			{
				int[] array = new int[num2];
				m_IndexBuffer.GetData((Array)array, 0, 0, num2);
				val.SetData((Array)array);
			}
			if (m_IndexBuffer != null)
			{
				m_IndexBuffer.Release();
			}
			m_IndexBuffer = val;
		}
	}

	public void PreDeserialize(Context context)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsAllocating)
		{
			((JobHandle)(ref m_AllocateDeps)).Complete();
			m_IsAllocating = false;
		}
		((NativeHeapAllocator)(ref m_BoneAllocator)).Clear();
		m_MetaBufferData.Clear();
		m_FreeMetaIndices.Clear();
		m_UpdatedMetaIndices.Clear();
		m_InstanceIndices.Clear();
		m_BodyInstances.Clear();
		m_FaceInstances.Clear();
		m_CorrectiveInstances.Clear();
		m_BodyTransitions.Clear();
		m_FaceTransitions.Clear();
		m_BoneAllocationRemoves.Clear();
		m_MetaBufferRemoves.Clear();
		((NativeHeapAllocator)(ref m_BoneAllocator)).Allocate(1u, 1u);
		ref NativeList<MetaBufferData> reference = ref m_MetaBufferData;
		MetaBufferData metaBufferData = default(MetaBufferData);
		reference.Add(ref metaBufferData);
	}

	public AllocationData GetAllocationData(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_AllocateDeps;
		m_IsAllocating = true;
		return new AllocationData(m_BoneAllocator, m_MetaBufferData, m_FreeMetaIndices, m_UpdatedMetaIndices, m_BoneAllocationRemoves, m_MetaBufferRemoves, m_CurrentTime);
	}

	public AnimationData GetAnimationData(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_AllocateDeps;
		if (!m_TempAnimationQueue.IsCreated)
		{
			m_TempAnimationQueue = new NativeQueue<AnimationFrameData>(AllocatorHandle.op_Implicit((Allocator)3));
		}
		if (!m_TempPriorityQueue.IsCreated)
		{
			m_TempPriorityQueue = new NativeQueue<ClipPriorityData>(AllocatorHandle.op_Implicit((Allocator)3));
		}
		m_IsAllocating = true;
		return new AnimationData(m_TempAnimationQueue, m_TempPriorityQueue);
	}

	public void AddAllocationWriter(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_AllocateDeps = handle;
	}

	public void AddAnimationWriter(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_AllocateDeps = handle;
	}

	public AnimatedPropID GetPropID(string name)
	{
		int value = -1;
		if (!string.IsNullOrEmpty(name) && !m_PropIDs.TryGetValue(name, out value))
		{
			value = m_PropIDs.Count;
			m_PropIDs.Add(name, value);
		}
		return new AnimatedPropID(value);
	}

	public void GetBoneStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		((JobHandle)(ref m_AllocateDeps)).Complete();
		allocatedSize = ((NativeHeapAllocator)(ref m_BoneAllocator)).UsedSpace * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>();
		bufferSize = ((NativeHeapAllocator)(ref m_BoneAllocator)).Size * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<BoneElement>();
		if (m_RenderingSystem.motionVectors)
		{
			allocatedSize <<= 1;
			bufferSize <<= 1;
		}
		count = (uint)(m_MetaBufferData.Length - m_FreeMetaIndices.Length - 1);
	}

	public void GetAnimStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		((JobHandle)(ref m_AllocateDeps)).Complete();
		allocatedSize = ((NativeHeapAllocator)(ref m_AnimAllocator)).UsedSpace * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<Element>();
		bufferSize = ((NativeHeapAllocator)(ref m_AnimAllocator)).Size * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<Element>();
		count = (uint)m_AnimationCount;
	}

	public void GetIndexStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		((JobHandle)(ref m_AllocateDeps)).Complete();
		allocatedSize = ((NativeHeapAllocator)(ref m_IndexAllocator)).UsedSpace * 4;
		bufferSize = ((NativeHeapAllocator)(ref m_IndexAllocator)).Size * 4;
		count = (uint)m_AnimationCount;
	}

	public void GetMetaStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		((JobHandle)(ref m_AllocateDeps)).Complete();
		count = (uint)(m_MetaBufferData.Length - m_FreeMetaIndices.Length - 1);
		if (m_MetaBuffer != null)
		{
			allocatedSize = (count + 1) * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>();
			bufferSize = (uint)(m_MetaBuffer.count * System.Runtime.CompilerServices.Unsafe.SizeOf<MetaBufferData>());
		}
		else
		{
			allocatedSize = 0u;
			bufferSize = 0u;
		}
	}

	[Preserve]
	public AnimatedSystem()
	{
	}
}
