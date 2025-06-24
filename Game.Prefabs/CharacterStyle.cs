using System;
using System.Collections.Generic;
using Colossal;
using Colossal.Animations;
using Colossal.IO.AssetDatabase;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

public class CharacterStyle : PrefabBase
{
	[Serializable]
	public class AnimationMotion
	{
		public float3 startOffset;

		public float3 endOffset;

		public quaternion startRotation;

		public quaternion endRotation;
	}

	[Serializable]
	public class AnimationInfo
	{
		public string name;

		public AssetReference<AnimationAsset> animationAsset;

		public RenderPrefab target;

		public AnimationType type;

		public AnimationLayer layer;

		public int frameCount;

		public int frameRate;

		public int rootMotionBone;

		public AnimationMotion[] rootMotion;

		public ActivityType activity;

		public AnimationType state;

		[BitMask]
		public ActivityCondition conditions;

		public AnimationPlayback playback;
	}

	public int m_ShapeCount;

	public int m_BoneCount;

	public GenderMask m_Gender = GenderMask.Any;

	public AnimationInfo[] m_Animations;

	public override bool ignoreUnlockDependencies => true;

	public AnimationAsset GetAnimation(int index)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return AssetDatabase.global.GetAsset<AnimationAsset>(AssetReference<AnimationAsset>.op_Implicit(m_Animations[index].animationAsset));
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Animations.Length; i++)
		{
			AnimationInfo animationInfo = m_Animations[i];
			if ((Object)(object)animationInfo.target != (Object)null)
			{
				prefabs.Add(animationInfo.target);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<CharacterStyleData>());
		components.Add(ComponentType.ReadWrite<AnimationClip>());
		components.Add(ComponentType.ReadWrite<Game.Prefabs.AnimationMotion>());
		components.Add(ComponentType.ReadWrite<RestPoseElement>());
	}

	public void CalculateRootMotion(BoneHierarchy hierarchy, Animation animation, Animation restPose, int infoIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		int num = ((animation.shapeIndices.Length <= 1) ? 1 : m_ShapeCount);
		int num2 = 0;
		if ((int)animation.layer == 0)
		{
			for (int i = 0; i < animation.boneIndices.Length; i++)
			{
				if (animation.boneIndices[i] == 1)
				{
					num2 = 1;
					break;
				}
			}
		}
		int[] array = new int[hierarchy.hierarchyParentIndices.Length];
		int[] array2 = new int[num];
		AnimationMotion[] array3 = new AnimationMotion[num];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = -1;
		}
		for (int k = 0; k < animation.boneIndices.Length; k++)
		{
			if (animation.boneIndices[k] < array.Length)
			{
				array[animation.boneIndices[k]] = k;
			}
		}
		if (num > 1)
		{
			for (int l = 0; l < animation.shapeIndices.Length; l++)
			{
				array2[animation.shapeIndices[l]] = l;
			}
		}
		int num3 = animation.shapeIndices.Length;
		int num4 = animation.boneIndices.Length;
		for (int m = 0; m < num; m++)
		{
			AnimationMotion animationMotion = (array3[m] = new AnimationMotion());
			int num5 = array[num2];
			int num6 = array2[m];
			ElementRaw val;
			ElementRaw val2;
			if (num5 >= 0)
			{
				int num7 = num5 * num3;
				int num8 = num7 + (animation.frameCount - 1) * num4 * num3;
				val = ((Animation)(ref animation)).DecodeElement(num7 + num6);
				val2 = ((Animation)(ref animation)).DecodeElement(num8 + num6);
			}
			else
			{
				int num9 = num2 * restPose.shapeIndices.Length;
				val = ((Animation)(ref restPose)).DecodeElement(num9 + m);
				val2 = val;
			}
			for (int num10 = hierarchy.hierarchyParentIndices[num2]; num10 != -1; num10 = hierarchy.hierarchyParentIndices[num10])
			{
				num5 = array[num10];
				ElementRaw val3;
				ElementRaw val4;
				if (num5 >= 0)
				{
					int num11 = num5 * num3;
					int num12 = num11 + (animation.frameCount - 1) * num4 * num3;
					val3 = ((Animation)(ref animation)).DecodeElement(num11 + num6);
					val4 = ((Animation)(ref animation)).DecodeElement(num12 + num6);
				}
				else
				{
					int num13 = num10 * restPose.shapeIndices.Length;
					val3 = ((Animation)(ref restPose)).DecodeElement(num13 + m);
					val4 = val3;
				}
				val.position = val3.position + math.mul(quaternion.op_Implicit(val3.rotation), val.position);
				val.rotation = math.mul(quaternion.op_Implicit(val3.rotation), quaternion.op_Implicit(val.rotation)).value;
				val2.position = val4.position + math.mul(quaternion.op_Implicit(val4.rotation), val2.position);
				val2.rotation = math.mul(quaternion.op_Implicit(val4.rotation), quaternion.op_Implicit(val2.rotation)).value;
			}
			animationMotion.startOffset = val.position;
			animationMotion.endOffset = val2.position;
			animationMotion.startRotation = math.normalize(quaternion.op_Implicit(val.rotation));
			animationMotion.endRotation = math.normalize(quaternion.op_Implicit(val2.rotation));
		}
		m_Animations[infoIndex].rootMotionBone = num2;
		m_Animations[infoIndex].rootMotion = array3;
	}
}
