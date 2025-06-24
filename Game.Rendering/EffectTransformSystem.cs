using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Effects;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class EffectTransformSystem : GameSystemBase
{
	[BurstCompile]
	private struct EffectTransformJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<EffectData> m_EffectDatas;

		[ReadOnly]
		public ComponentLookup<RandomTransformData> m_RandomTransformDatas;

		[ReadOnly]
		public ComponentLookup<EffectColorData> m_EffectColorDatas;

		[ReadOnly]
		public ComponentLookup<Game.Events.Event> m_EventData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public BufferLookup<BoneHistory> m_BoneHistories;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColors;

		[ReadOnly]
		public BufferLookup<Effect> m_Effects;

		[ReadOnly]
		public BufferLookup<EffectAnimation> m_EffectAnimations;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[NativeDisableParallelForRestriction]
		public NativeList<EnabledEffectData> m_EnabledData;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		public void Execute(int index)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			ref EnabledEffectData reference = ref m_EnabledData.ElementAt(index);
			if ((reference.m_Flags & EnabledEffectFlags.IsEnabled) == 0 || (reference.m_Flags & (EnabledEffectFlags.EnabledUpdated | EnabledEffectFlags.DynamicTransform | EnabledEffectFlags.OwnerUpdated)) == 0)
			{
				return;
			}
			PrefabRef prefabRef = m_Prefabs[reference.m_Owner];
			EffectData effectData = m_EffectDatas[reference.m_Prefab];
			Effect prefabEffect;
			if ((reference.m_Flags & EnabledEffectFlags.EditorContainer) != 0)
			{
				Game.Tools.EditorContainer editorContainer = m_EditorContainerData[reference.m_Owner];
				prefabEffect = new Effect
				{
					m_Effect = editorContainer.m_Prefab,
					m_Scale = editorContainer.m_Scale,
					m_Intensity = editorContainer.m_Intensity,
					m_AnimationIndex = editorContainer.m_GroupIndex,
					m_Rotation = quaternion.identity,
					m_BoneIndex = int2.op_Implicit(-1)
				};
			}
			else
			{
				prefabEffect = m_Effects[prefabRef.m_Prefab][reference.m_EffectIndex];
			}
			Entity val = reference.m_Owner;
			Temp temp = default(Temp);
			if ((reference.m_Flags & EnabledEffectFlags.TempOwner) != 0 && m_TempData.TryGetComponent(val, ref temp) && temp.m_Original != Entity.Null)
			{
				val = temp.m_Original;
			}
			Random random = Random.CreateFromIndex((uint)(val.Index ^ reference.m_EffectIndex));
			if ((reference.m_Flags & EnabledEffectFlags.RandomTransform) != 0)
			{
				RandomTransformData randomTransformData = m_RandomTransformDatas[prefabEffect.m_Effect];
				float3 val2 = ((Random)(ref random)).NextFloat3(randomTransformData.m_AngleRange.min, randomTransformData.m_AngleRange.max);
				prefabEffect.m_Rotation = math.mul(prefabEffect.m_Rotation, quaternion.Euler(val2, (RotationOrder)4));
				ref float3 position = ref prefabEffect.m_Position;
				position += ((Random)(ref random)).NextFloat3(randomTransformData.m_PositionRange.min, randomTransformData.m_PositionRange.max);
			}
			bool num = effectData.m_OwnerCulling || IsNearCamera(reference.m_Owner) || m_EventData.HasComponent(reference.m_Owner);
			bool flag = (reference.m_Flags & EnabledEffectFlags.IsLight) != 0;
			reference.m_Scale = prefabEffect.m_Scale;
			reference.m_Intensity = prefabEffect.m_Intensity;
			if (flag)
			{
				EffectColorData effectColorData = m_EffectColorDatas[prefabEffect.m_Effect];
				Color color = effectColorData.m_Color;
				DynamicBuffer<MeshColor> val3 = default(DynamicBuffer<MeshColor>);
				if (effectColorData.m_Source != EffectColorSource.Effect && prefabEffect.m_ParentMesh >= 0 && m_MeshColors.TryGetBuffer(reference.m_Owner, ref val3) && val3.Length > prefabEffect.m_ParentMesh)
				{
					color = val3[prefabEffect.m_ParentMesh].m_ColorSet[(int)(effectColorData.m_Source - 1)];
					color *= effectColorData.m_Color.a;
				}
				if (math.any(effectColorData.m_VaritationRanges != 0f))
				{
					Randomize(ref color, ref random, 1f - effectColorData.m_VaritationRanges, 1f + effectColorData.m_VaritationRanges);
				}
				reference.m_Scale = new float3(color.r, color.g, color.b);
			}
			InterpolatedTransform interpolatedTransform = default(InterpolatedTransform);
			Relative relative = default(Relative);
			Curve curve = default(Curve);
			Transform parentTransform3 = default(Transform);
			Node node = default(Node);
			if (num && m_InterpolatedTransformData.TryGetComponent(reference.m_Owner, ref interpolatedTransform))
			{
				Transform effectTransform = GetEffectTransform(prefabEffect, reference.m_Owner);
				effectTransform = ObjectUtils.LocalToWorld(interpolatedTransform.ToTransform(), effectTransform);
				if ((reference.m_Flags & EnabledEffectFlags.OwnerCollapsed) != 0)
				{
					effectTransform.m_Position.y = math.max(effectTransform.m_Position.y, m_TransformData[reference.m_Owner].m_Position.y);
				}
				reference.m_Position = effectTransform.m_Position;
				reference.m_Rotation = effectTransform.m_Rotation;
				if (flag && ((effectData.m_Flags.m_RequiredFlags | effectData.m_Flags.m_ForbiddenFlags) & (EffectConditionFlags.MainLights | EffectConditionFlags.ExtraLights | EffectConditionFlags.WarningLights)) != EffectConditionFlags.None)
				{
					bool flag2 = TestFlags(effectData.m_Flags.m_RequiredFlags, effectData.m_Flags.m_ForbiddenFlags, interpolatedTransform.m_Flags);
					reference.m_Intensity = math.select(0f, reference.m_Intensity, flag2);
				}
			}
			else if (m_RelativeData.TryGetComponent(reference.m_Owner, ref relative))
			{
				Owner owner = m_OwnerData[reference.m_Owner];
				Transform relativeTransform = GetRelativeTransform(relative, owner.m_Owner);
				Transform transform = new Transform(prefabEffect.m_Position, prefabEffect.m_Rotation);
				Transform parentTransform2 = default(Transform);
				Transform parentTransform = (m_InterpolatedTransformData.TryGetComponent(owner.m_Owner, ref interpolatedTransform) ? ObjectUtils.LocalToWorld(interpolatedTransform.ToTransform(), relativeTransform) : ((!m_TransformData.TryGetComponent(owner.m_Owner, ref parentTransform2)) ? relativeTransform : ObjectUtils.LocalToWorld(parentTransform2, relativeTransform)));
				transform = ObjectUtils.LocalToWorld(parentTransform, transform);
				reference.m_Position = transform.m_Position;
				reference.m_Rotation = transform.m_Rotation;
			}
			else if (m_CurveData.TryGetComponent(reference.m_Owner, ref curve))
			{
				reference.m_Position = MathUtils.Position(curve.m_Bezier, 0.5f);
				reference.m_Rotation = quaternion.identity;
			}
			else if (m_TransformData.TryGetComponent(reference.m_Owner, ref parentTransform3))
			{
				Transform effectTransform2 = GetEffectTransform(prefabEffect, reference.m_Owner);
				effectTransform2 = ObjectUtils.LocalToWorld(parentTransform3, effectTransform2);
				if ((reference.m_Flags & EnabledEffectFlags.OwnerCollapsed) != 0)
				{
					effectTransform2.m_Position.y = parentTransform3.m_Position.y;
				}
				reference.m_Position = effectTransform2.m_Position;
				reference.m_Rotation = effectTransform2.m_Rotation;
			}
			else if (m_NodeData.TryGetComponent(reference.m_Owner, ref node))
			{
				reference.m_Position = node.m_Position;
				reference.m_Rotation = node.m_Rotation;
			}
			DynamicBuffer<EffectAnimation> val4 = default(DynamicBuffer<EffectAnimation>);
			if (prefabEffect.m_AnimationIndex >= 0 && reference.m_Intensity != 0f && m_EffectAnimations.TryGetBuffer(prefabRef.m_Prefab, ref val4) && val4.Length > prefabEffect.m_AnimationIndex)
			{
				Random random2 = m_PseudoRandomSeedData[reference.m_Owner].GetRandom(PseudoRandomSeed.kLightState);
				EffectAnimation effectAnimation = val4[prefabEffect.m_AnimationIndex];
				float num2 = (float)((m_FrameIndex + ((Random)(ref random2)).NextUInt(effectAnimation.m_DurationFrames)) % effectAnimation.m_DurationFrames) + m_FrameTime;
				reference.m_Intensity *= ((AnimationCurve1)(ref effectAnimation.m_AnimationCurve)).Evaluate(num2 / (float)effectAnimation.m_DurationFrames);
			}
		}

		private void Randomize(ref Color color, ref Random random, float3 min, float3 max)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			Color.RGBToHSV(color, ref val.x, ref val.y, ref val.z);
			float3 val2 = ((Random)(ref random)).NextFloat3(min, max);
			val.x = math.frac(val.x + val2.x);
			((float3)(ref val)).yz = ((float3)(ref val)).yz * ((float3)(ref val2)).yz;
			val.y = math.saturate(val.y);
			val.z = math.max(0f, val.z);
			color = Color.HSVToRGB(val.x, val.y, val.z);
		}

		private bool IsNearCamera(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			if (m_CullingInfoData.TryGetComponent(entity, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
			{
				return (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
			}
			return false;
		}

		private Transform GetEffectTransform(Effect prefabEffect, Entity owner)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (prefabEffect.m_BoneIndex.x >= 0)
			{
				DynamicBuffer<BoneHistory> val = m_BoneHistories[owner];
				if (val.Length >= prefabEffect.m_BoneIndex.x)
				{
					float4x4 matrix = val[prefabEffect.m_BoneIndex.x].m_Matrix;
					float3 val2 = math.transform(matrix, prefabEffect.m_Position);
					float3 val3 = math.rotate(matrix, math.forward(prefabEffect.m_Rotation));
					float3 val4 = math.rotate(matrix, math.mul(prefabEffect.m_Rotation, math.up()));
					quaternion val5 = quaternion.LookRotation(val3, val4);
					if (prefabEffect.m_BoneIndex.y >= 0)
					{
						PrefabRef prefabRef = m_Prefabs[owner];
						SubMesh subMesh = m_SubMeshes[prefabRef.m_Prefab][prefabEffect.m_BoneIndex.y];
						val2 = subMesh.m_Position + math.rotate(subMesh.m_Rotation, val2);
						val5 = math.mul(subMesh.m_Rotation, val5);
					}
					return new Transform(val2, val5);
				}
			}
			return new Transform(prefabEffect.m_Position, prefabEffect.m_Rotation);
		}

		private Transform GetRelativeTransform(Relative relative, Entity owner)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (relative.m_BoneIndex.y >= 0)
			{
				DynamicBuffer<BoneHistory> val = m_BoneHistories[owner];
				if (val.Length > relative.m_BoneIndex.y)
				{
					float4x4 matrix = val[relative.m_BoneIndex.y].m_Matrix;
					float3 val2 = math.transform(matrix, relative.m_Position);
					float3 val3 = math.rotate(matrix, math.forward(relative.m_Rotation));
					float3 val4 = math.rotate(matrix, math.mul(relative.m_Rotation, math.up()));
					quaternion val5 = quaternion.LookRotation(val3, val4);
					if (relative.m_BoneIndex.z >= 0)
					{
						PrefabRef prefabRef = m_Prefabs[owner];
						SubMesh subMesh = m_SubMeshes[prefabRef.m_Prefab][relative.m_BoneIndex.z];
						val2 = subMesh.m_Position + math.rotate(subMesh.m_Rotation, val2);
						val5 = math.mul(subMesh.m_Rotation, val5);
					}
					return new Transform(val2, val5);
				}
			}
			return new Transform(relative.m_Position, relative.m_Rotation);
		}

		private bool TestFlags(EffectConditionFlags requiredFlags, EffectConditionFlags forbiddenFlags, TransformFlags transformFlags)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			int4 val = new int4(65536, 131072, 262144, 524288);
			int4 val2 = default(int4);
			((int4)(ref val2))._002Ector(1, 2, 1024, 4096);
			bool4 val3 = (val & (int)requiredFlags) != 0;
			bool4 val4 = (val & (int)forbiddenFlags) != 0;
			bool4 val5 = (val2 & (int)transformFlags) != 0;
			return math.any(val3 & val5) & !math.any(val4 & val5);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RandomTransformData> __Game_Prefabs_RandomTransformData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectColorData> __Game_Prefabs_EffectColorData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Events.Event> __Game_Events_Event_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<BoneHistory> __Game_Rendering_BoneHistory_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<EffectAnimation> __Game_Prefabs_EffectAnimation_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

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
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Tools.EditorContainer>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_RandomTransformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RandomTransformData>(true);
			__Game_Prefabs_EffectColorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectColorData>(true);
			__Game_Events_Event_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Events.Event>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Rendering_BoneHistory_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BoneHistory>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
			__Game_Prefabs_EffectAnimation_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EffectAnimation>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private EffectControlSystem m_EffectControlSystem;

	private PreCullingSystem m_PreCullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		EffectTransformJob obj = new EffectTransformJob
		{
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectDatas = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomTransformDatas = InternalCompilerInterface.GetComponentLookup<RandomTransformData>(ref __TypeHandle.__Game_Prefabs_RandomTransformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectColorDatas = InternalCompilerInterface.GetComponentLookup<EffectColorData>(ref __TypeHandle.__Game_Prefabs_EffectColorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EventData = InternalCompilerInterface.GetComponentLookup<Game.Events.Event>(ref __TypeHandle.__Game_Events_Event_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneHistories = InternalCompilerInterface.GetBufferLookup<BoneHistory>(ref __TypeHandle.__Game_Rendering_BoneHistory_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Effects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectAnimations = InternalCompilerInterface.GetBufferLookup<EffectAnimation>(ref __TypeHandle.__Game_Prefabs_EffectAnimation_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EnabledData = m_EffectControlSystem.GetEnabledData(readOnly: false, out dependencies),
			m_CullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies2),
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<EffectTransformJob, EnabledEffectData>(obj, obj.m_EnabledData, 16, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		m_EffectControlSystem.AddEnabledDataWriter(val);
		m_PreCullingSystem.AddCullingDataReader(val);
		((SystemBase)this).Dependency = val;
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
	public EffectTransformSystem()
	{
	}
}
