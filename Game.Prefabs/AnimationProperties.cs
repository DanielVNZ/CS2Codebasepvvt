using System;
using System.Collections.Generic;
using Colossal.GPUAnimation;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class AnimationProperties : ComponentBase
{
	[Serializable]
	public class BakedAnimationClip
	{
		public string name;

		public int pixelStart;

		public int pixelEnd;

		public float animationLength;

		public bool looping;

		public Texture2DArray animationTexture;

		public AnimationType m_Type;

		public ActivityType m_Activity;

		public float m_MovementSpeed;

		public float3 m_RootOffset;

		public quaternion m_RootRotation;

		public BakedAnimationClip(Texture2DArray animTexture, AnimationClipData clipData)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			name = ((Object)clipData.clip).name;
			pixelStart = clipData.pixelStart;
			pixelEnd = clipData.pixelEnd;
			animationLength = clipData.clip.length;
			looping = (int)clipData.clip.wrapMode == 2;
			animationTexture = animTexture;
		}

		public void CalculatePlaybackData(ref AnimationClip clip)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			int width = ((Texture)animationTexture).width;
			float num = 1f / (float)width;
			float num2 = (float)pixelStart / (float)width;
			float num3 = (float)pixelEnd / (float)width;
			clip.m_TextureOffset = num2;
			clip.m_TextureRange = num3 - num2;
			clip.m_OnePixelOffset = num;
			clip.m_TextureWidth = width;
			clip.m_OneOverTextureWidth = 1f / (float)width;
			clip.m_OneOverPixelOffset = 1f / num;
			clip.m_AnimationLength = animationLength;
			clip.m_MovementSpeed = m_MovementSpeed;
			clip.m_RootOffset = m_RootOffset;
			clip.m_RootRotation = m_RootRotation;
			clip.m_Type = m_Type;
			clip.m_PropID = new AnimatedPropID(-1);
			clip.m_Activity = m_Activity;
			clip.m_Layer = AnimationLayer.Body;
			clip.m_Playback = (looping ? AnimationPlayback.RandomLoop : AnimationPlayback.Once);
		}
	}

	public BakedAnimationClip[] m_Clips;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AnimationClip>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Clips != null)
		{
			DynamicBuffer<AnimationClip> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AnimationClip>(entity, false);
			buffer.ResizeUninitialized(m_Clips.Length);
			for (int i = 0; i < m_Clips.Length; i++)
			{
				BakedAnimationClip obj = m_Clips[i];
				AnimationClip clip = default(AnimationClip);
				obj.CalculatePlaybackData(ref clip);
				buffer[i] = clip;
			}
		}
	}
}
