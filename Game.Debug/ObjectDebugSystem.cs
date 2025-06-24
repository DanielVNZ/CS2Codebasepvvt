using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class ObjectDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct ObjectGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_GeometryOption;

		[ReadOnly]
		public bool m_MarkerOption;

		[ReadOnly]
		public bool m_PivotOption;

		[ReadOnly]
		public bool m_OutlineOption;

		[ReadOnly]
		public bool m_InterpolatedOption;

		[ReadOnly]
		public bool m_NetConnectionOption;

		[ReadOnly]
		public bool m_GroupConnectionOption;

		[ReadOnly]
		public bool m_DistrictOption;

		[ReadOnly]
		public bool m_LotHeightOption;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Stack> m_StackType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometry> m_ObjectGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> m_MarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Lot> m_BuildingLotType;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_NetCurveData;

		[ReadOnly]
		public ComponentLookup<Geometry> m_AreaGeometryData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public Entity m_Selected;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_094b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0950: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
			int num;
			int num2;
			if (m_Selected != Entity.Null)
			{
				num = (num2 = -1);
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (nativeArray[i] == m_Selected)
					{
						num = i;
						num2 = i + 1;
						break;
					}
				}
				if (num == -1)
				{
					return;
				}
			}
			else
			{
				num = 0;
				num2 = ((ArchetypeChunk)(ref chunk)).Count;
			}
			Color pivotColor;
			Color outlineColor;
			if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				pivotColor = Color.blue;
				outlineColor = Color.blue;
			}
			else
			{
				pivotColor = Color.cyan;
				outlineColor = Color.white;
			}
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Stack> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CurrentVehicle> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			if (nativeArray5.Length != 0)
			{
				NativeArray<GroupMember> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
				if (!m_GeometryOption && (!m_GroupConnectionOption || nativeArray6.Length == 0))
				{
					return;
				}
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				StackData stackData = default(StackData);
				for (int j = num; j < num2; j++)
				{
					Entity entity = nativeArray7[j];
					Transform transform = nativeArray2[j];
					PrefabRef prefabRef = nativeArray4[j];
					CurrentVehicle currentVehicle = nativeArray5[j];
					ObjectGeometryData prefabObjectData = m_PrefabGeometryData[prefabRef.m_Prefab];
					GetVehicleTransform(entity, currentVehicle, prefabObjectData, ref transform);
					if (m_GeometryOption)
					{
						if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
						{
							DrawObject(transform, nativeArray3[j], prefabObjectData, stackData, pivotColor, outlineColor);
						}
						else
						{
							DrawObject(transform, prefabObjectData, pivotColor, outlineColor);
						}
					}
					if (!m_GroupConnectionOption || nativeArray6.Length == 0)
					{
						continue;
					}
					GroupMember groupMember = nativeArray6[j];
					if (m_TransformData.HasComponent(groupMember.m_Leader))
					{
						Transform transform2 = m_TransformData[groupMember.m_Leader];
						if (m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
						{
							CurrentVehicle currentVehicle2 = m_CurrentVehicleData[groupMember.m_Leader];
							PrefabRef prefabRef2 = m_PrefabRefData[groupMember.m_Leader];
							ObjectGeometryData prefabObjectData2 = m_PrefabGeometryData[prefabRef2.m_Prefab];
							GetVehicleTransform(groupMember.m_Leader, currentVehicle2, prefabObjectData2, ref transform2);
						}
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform.m_Position, transform2.m_Position, Color.green);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform.m_Position, 2f, Color.red);
					}
				}
				return;
			}
			NativeArray<Attached> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Attached>(ref m_AttachedType);
			if (((ArchetypeChunk)(ref chunk)).Has<ObjectGeometry>(ref m_ObjectGeometryType))
			{
				if (m_GeometryOption)
				{
					NativeArray<InterpolatedTransform> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
					if (nativeArray9.Length != 0 && m_InterpolatedOption)
					{
						StackData stackData2 = default(StackData);
						for (int k = num; k < num2; k++)
						{
							Transform transform3 = nativeArray2[k];
							InterpolatedTransform interpolatedTransform = nativeArray9[k];
							PrefabRef prefabRef3 = nativeArray4[k];
							ObjectGeometryData prefabObjectData3 = m_PrefabGeometryData[prefabRef3.m_Prefab];
							if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef3.m_Prefab, ref stackData2))
							{
								DrawObject(transform3, nativeArray3[k], prefabObjectData3, stackData2, pivotColor, outlineColor);
								DrawObject(interpolatedTransform.ToTransform(), nativeArray3[k], prefabObjectData3, stackData2, Color.green, Color.green);
							}
							else
							{
								DrawObject(transform3, prefabObjectData3, pivotColor, outlineColor);
								DrawObject(interpolatedTransform.ToTransform(), prefabObjectData3, Color.green, Color.green);
							}
						}
					}
					else if (nativeArray8.Length != 0)
					{
						StackData stackData3 = default(StackData);
						for (int l = num; l < num2; l++)
						{
							Transform transform4 = nativeArray2[l];
							Attached attached = nativeArray8[l];
							PrefabRef prefabRef4 = nativeArray4[l];
							ObjectGeometryData prefabObjectData4 = m_PrefabGeometryData[prefabRef4.m_Prefab];
							Color val = ((attached.m_Parent == Entity.Null) ? Color.red : ((!m_PrefabRefData.HasComponent(attached.m_Parent)) ? Color.yellow : Color.green));
							if (m_NetConnectionOption && GetAttachPosition(attached, out var attachPosition))
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform4.m_Position, attachPosition, val);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(attachPosition, 1f, val);
							}
							if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef4.m_Prefab, ref stackData3))
							{
								DrawObject(transform4, nativeArray3[l], prefabObjectData4, stackData3, val, outlineColor);
							}
							else
							{
								DrawObject(transform4, prefabObjectData4, val, outlineColor);
							}
						}
					}
					else
					{
						StackData stackData4 = default(StackData);
						for (int m = num; m < num2; m++)
						{
							Transform transform5 = nativeArray2[m];
							PrefabRef prefabRef5 = nativeArray4[m];
							ObjectGeometryData prefabObjectData5 = m_PrefabGeometryData[prefabRef5.m_Prefab];
							if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef5.m_Prefab, ref stackData4))
							{
								DrawObject(transform5, nativeArray3[m], prefabObjectData5, stackData4, pivotColor, outlineColor);
							}
							else
							{
								DrawObject(transform5, prefabObjectData5, pivotColor, outlineColor);
							}
						}
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Marker>(ref m_MarkerType) && m_MarkerOption)
			{
				if (nativeArray8.Length != 0)
				{
					StackData stackData5 = default(StackData);
					for (int n = num; n < num2; n++)
					{
						Transform transform6 = nativeArray2[n];
						Attached attached2 = nativeArray8[n];
						PrefabRef prefabRef6 = nativeArray4[n];
						ObjectGeometryData prefabObjectData6 = m_PrefabGeometryData[prefabRef6.m_Prefab];
						Color val2 = ((attached2.m_Parent == Entity.Null) ? Color.red : ((!m_PrefabRefData.HasComponent(attached2.m_Parent)) ? Color.yellow : Color.green));
						if (m_NetConnectionOption && GetAttachPosition(attached2, out var attachPosition2))
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform6.m_Position, attachPosition2, val2);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(attachPosition2, 1f, val2);
						}
						if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef6.m_Prefab, ref stackData5))
						{
							DrawObject(transform6, nativeArray3[n], prefabObjectData6, stackData5, val2, outlineColor);
						}
						else
						{
							DrawObject(transform6, prefabObjectData6, val2, outlineColor);
						}
					}
				}
				else
				{
					StackData stackData6 = default(StackData);
					for (int num3 = num; num3 < num2; num3++)
					{
						Transform transform7 = nativeArray2[num3];
						PrefabRef prefabRef7 = nativeArray4[num3];
						ObjectGeometryData prefabObjectData7 = m_PrefabGeometryData[prefabRef7.m_Prefab];
						if (nativeArray3.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef7.m_Prefab, ref stackData6))
						{
							DrawObject(transform7, nativeArray3[num3], prefabObjectData7, stackData6, pivotColor, outlineColor);
						}
						else
						{
							DrawObject(transform7, prefabObjectData7, pivotColor, outlineColor);
						}
					}
				}
			}
			if (m_NetConnectionOption)
			{
				NativeArray<Building> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
				if (nativeArray10.Length != 0)
				{
					for (int num4 = num; num4 < num2; num4++)
					{
						Building building = nativeArray10[num4];
						Transform transform8 = nativeArray2[num4];
						PrefabRef prefabRef8 = nativeArray4[num4];
						BuildingData buildingData = m_PrefabBuildingData[prefabRef8.m_Prefab];
						if ((buildingData.m_Flags & Game.Prefabs.BuildingFlags.NoRoadConnection) == 0)
						{
							float3 val3 = BuildingUtils.CalculateFrontPosition(transform8, buildingData.m_LotSize.y);
							if (building.m_RoadEdge != Entity.Null)
							{
								float3 val4 = MathUtils.Position(m_NetCurveData[building.m_RoadEdge].m_Bezier, building.m_CurvePosition);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val3, val4, Color.green);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val3, 2f, Color.green);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val4, 1f, Color.green);
							}
							else
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val3, 2f, Color.red);
							}
						}
					}
				}
				NativeArray<Game.Objects.SpawnLocation> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
				if (nativeArray11.Length != 0)
				{
					for (int num5 = num; num5 < num2; num5++)
					{
						Game.Objects.SpawnLocation spawnLocation = nativeArray11[num5];
						Transform transform9 = nativeArray2[num5];
						if (spawnLocation.m_ConnectedLane1 != Entity.Null)
						{
							float3 val5 = MathUtils.Position(m_NetCurveData[spawnLocation.m_ConnectedLane1].m_Bezier, spawnLocation.m_CurvePosition1);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform9.m_Position, 2f, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform9.m_Position, val5, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val5, 1f, Color.green);
							if (spawnLocation.m_ConnectedLane2 != Entity.Null)
							{
								float3 val6 = MathUtils.Position(m_NetCurveData[spawnLocation.m_ConnectedLane2].m_Bezier, spawnLocation.m_CurvePosition2);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform9.m_Position, val6, Color.green);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val6, 1f, Color.green);
							}
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform9.m_Position, 2f, Color.red);
						}
					}
				}
			}
			if (m_GroupConnectionOption)
			{
				NativeArray<GroupMember> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
				if (nativeArray12.Length != 0)
				{
					for (int num6 = num; num6 < num2; num6++)
					{
						Transform transform10 = nativeArray2[num6];
						GroupMember groupMember2 = nativeArray12[num6];
						if (m_TransformData.HasComponent(groupMember2.m_Leader))
						{
							Transform transform11 = m_TransformData[groupMember2.m_Leader];
							if (m_CurrentVehicleData.HasComponent(groupMember2.m_Leader))
							{
								CurrentVehicle currentVehicle3 = m_CurrentVehicleData[groupMember2.m_Leader];
								PrefabRef prefabRef9 = m_PrefabRefData[groupMember2.m_Leader];
								ObjectGeometryData prefabObjectData8 = m_PrefabGeometryData[prefabRef9.m_Prefab];
								GetVehicleTransform(groupMember2.m_Leader, currentVehicle3, prefabObjectData8, ref transform11);
							}
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform10.m_Position, transform11.m_Position, Color.green);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform10.m_Position, 2f, Color.red);
						}
					}
				}
			}
			if (m_DistrictOption)
			{
				NativeArray<CurrentDistrict> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
				if (nativeArray13.Length != 0)
				{
					for (int num7 = num; num7 < num2; num7++)
					{
						CurrentDistrict currentDistrict = nativeArray13[num7];
						Transform transform12 = nativeArray2[num7];
						if (currentDistrict.m_District != Entity.Null)
						{
							Geometry geometry = m_AreaGeometryData[currentDistrict.m_District];
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform12.m_Position, geometry.m_CenterPosition, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform12.m_Position, 8f, Color.green);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform12.m_Position, 8f, Color.red);
						}
					}
				}
			}
			if (!m_LotHeightOption)
			{
				return;
			}
			NativeArray<Game.Buildings.Lot> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.Lot>(ref m_BuildingLotType);
			if (nativeArray14.Length == 0)
			{
				return;
			}
			BuildingData buildingData2 = default(BuildingData);
			BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
			for (int num8 = num; num8 < num2; num8++)
			{
				Game.Buildings.Lot lot = nativeArray14[num8];
				Transform transform13 = nativeArray2[num8];
				PrefabRef prefabRef10 = nativeArray4[num8];
				int2 val7 = int2.op_Implicit(1);
				if (m_PrefabBuildingData.TryGetComponent(prefabRef10.m_Prefab, ref buildingData2))
				{
					val7 = buildingData2.m_LotSize;
				}
				else if (m_PrefabBuildingExtensionData.TryGetComponent(prefabRef10.m_Prefab, ref buildingExtensionData))
				{
					if (!buildingExtensionData.m_External)
					{
						continue;
					}
					val7 = buildingExtensionData.m_LotSize;
				}
				Quad3 val8 = BuildingUtils.CalculateCorners(transform13, val7);
				Bezier4x3 val9 = NetUtils.StraightCurve(val8.a, val8.b);
				Bezier4x3 val10 = NetUtils.StraightCurve(val8.b, val8.c);
				Bezier4x3 val11 = NetUtils.StraightCurve(val8.c, val8.d);
				Bezier4x3 val12 = NetUtils.StraightCurve(val8.d, val8.a);
				Bezier4x1 y = ((Bezier4x3)(ref val9)).y;
				((Bezier4x3)(ref val9)).y = new Bezier4x1(((Bezier4x1)(ref y)).abcd + new float4(lot.m_FrontHeights, lot.m_RightHeights.x));
				y = ((Bezier4x3)(ref val10)).y;
				((Bezier4x3)(ref val10)).y = new Bezier4x1(((Bezier4x1)(ref y)).abcd + new float4(lot.m_RightHeights, lot.m_BackHeights.x));
				y = ((Bezier4x3)(ref val11)).y;
				((Bezier4x3)(ref val11)).y = new Bezier4x1(((Bezier4x1)(ref y)).abcd + new float4(lot.m_BackHeights, lot.m_LeftHeights.x));
				y = ((Bezier4x3)(ref val12)).y;
				((Bezier4x3)(ref val12)).y = new Bezier4x1(((Bezier4x1)(ref y)).abcd + new float4(lot.m_LeftHeights, lot.m_FrontHeights.x));
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val9, (float)val7.x * 8f, Color.magenta, -1);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val10, (float)val7.y * 8f, Color.magenta, -1);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val11, (float)val7.x * 8f, Color.magenta, -1);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val12, (float)val7.y * 8f, Color.magenta, -1);
			}
		}

		private void GetVehicleTransform(Entity entity, CurrentVehicle currentVehicle, ObjectGeometryData prefabObjectData, ref Transform transform)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransformData.HasComponent(currentVehicle.m_Vehicle))
			{
				Random val = default(Random);
				((Random)(ref val))._002Ector((uint)(1851936439 + entity.Index));
				((Random)(ref val)).NextInt();
				PrefabRef prefabRef = m_PrefabRefData[currentVehicle.m_Vehicle];
				transform = m_TransformData[currentVehicle.m_Vehicle];
				ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				float3 val2 = math.max(float3.op_Implicit(0f), objectGeometryData.m_Size - prefabObjectData.m_Size);
				float3 val3 = ((Random)(ref val)).NextFloat3(val2);
				((float3)(ref val3)).xz = ((float3)(ref val3)).xz - ((float3)(ref val2)).xz * 0.5f;
				ref float3 position = ref transform.m_Position;
				position += math.rotate(transform.m_Rotation, val3);
			}
		}

		private bool GetAttachPosition(Attached attached, out float3 attachPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (m_NetCurveData.HasComponent(attached.m_Parent))
			{
				attachPosition = MathUtils.Position(m_NetCurveData[attached.m_Parent].m_Bezier, attached.m_CurvePosition);
				return true;
			}
			attachPosition = default(float3);
			return false;
		}

		private void DrawObject(Transform transform, ObjectGeometryData prefabObjectData, Color pivotColor, Color outlineColor)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			if (m_PivotOption)
			{
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform.m_Position, math.sqrt(prefabObjectData.m_Size.x + prefabObjectData.m_Size.z) * 0.25f, pivotColor);
			}
			if (!m_OutlineOption)
			{
				return;
			}
			if (ObjectUtils.GetStandingLegCount(prefabObjectData, out var legCount))
			{
				float4x4 val = default(float4x4);
				for (int i = 0; i < legCount; i++)
				{
					float3 standingLegPosition = ObjectUtils.GetStandingLegPosition(prefabObjectData, transform, i);
					((float4x4)(ref val))._002Ector(transform.m_Rotation, standingLegPosition);
					if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.CircularLeg) != Game.Objects.GeometryFlags.None)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val, new float3(0f, prefabObjectData.m_LegSize.y * 0.5f, 0f), prefabObjectData.m_LegSize.x * 0.5f, prefabObjectData.m_LegSize.y, outlineColor, 36);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val, new float3(0f, prefabObjectData.m_LegSize.y * 0.5f, 0f), prefabObjectData.m_LegSize, outlineColor);
					}
				}
				float4x4 val2 = default(float4x4);
				((float4x4)(ref val2))._002Ector(transform.m_Rotation, transform.m_Position);
				if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					float num = prefabObjectData.m_Size.y - prefabObjectData.m_LegSize.y;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val2, new float3(0f, prefabObjectData.m_LegSize.y + num * 0.5f, 0f), prefabObjectData.m_Size.x * 0.5f, num, outlineColor, 36);
					return;
				}
				prefabObjectData.m_Bounds.min.y = prefabObjectData.m_LegSize.y;
				float3 val3 = MathUtils.Center(prefabObjectData.m_Bounds);
				float3 val4 = MathUtils.Size(prefabObjectData.m_Bounds);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val2, val3, val4, outlineColor);
			}
			else
			{
				float4x4 val5 = default(float4x4);
				((float4x4)(ref val5))._002Ector(transform.m_Rotation, transform.m_Position);
				if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val5, new float3(0f, prefabObjectData.m_Size.y * 0.5f, 0f), prefabObjectData.m_Size.x * 0.5f, prefabObjectData.m_Size.y, outlineColor, 36);
					return;
				}
				float3 val6 = MathUtils.Center(prefabObjectData.m_Bounds);
				float3 val7 = MathUtils.Size(prefabObjectData.m_Bounds);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val5, val6, val7, outlineColor);
			}
		}

		private void DrawObject(Transform transform, Stack stack, ObjectGeometryData prefabObjectData, StackData stackData, Color pivotColor, Color outlineColor)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			if (m_PivotOption)
			{
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(transform.m_Position, math.sqrt(prefabObjectData.m_Size.x + prefabObjectData.m_Size.z) * 0.25f, pivotColor);
			}
			if (!m_OutlineOption)
			{
				return;
			}
			switch (stackData.m_Direction)
			{
			case StackDirection.Right:
			{
				float3 val3 = math.rotate(transform.m_Rotation, math.right());
				ref float3 position3 = ref transform.m_Position;
				position3 += val3 * (stack.m_Range.min - prefabObjectData.m_Bounds.min.x);
				prefabObjectData.m_Size.x = stack.m_Range.max - stack.m_Range.min;
				((Bounds3)(ref prefabObjectData.m_Bounds)).x = stack.m_Range - (stack.m_Range.min - prefabObjectData.m_Bounds.min.x);
				break;
			}
			case StackDirection.Up:
			{
				float3 val2 = math.rotate(transform.m_Rotation, math.up());
				ref float3 position2 = ref transform.m_Position;
				position2 += val2 * (stack.m_Range.min - prefabObjectData.m_Bounds.min.y);
				prefabObjectData.m_LegSize.y -= stack.m_Range.min - prefabObjectData.m_Bounds.min.y;
				prefabObjectData.m_Size.y = stack.m_Range.max - stack.m_Range.min;
				((Bounds3)(ref prefabObjectData.m_Bounds)).y = stack.m_Range - (stack.m_Range.min - prefabObjectData.m_Bounds.min.y);
				break;
			}
			case StackDirection.Forward:
			{
				float3 val = math.rotate(transform.m_Rotation, math.forward());
				ref float3 position = ref transform.m_Position;
				position += val * (stack.m_Range.min - prefabObjectData.m_Bounds.min.z);
				prefabObjectData.m_Size.z = stack.m_Range.max - stack.m_Range.min;
				((Bounds3)(ref prefabObjectData.m_Bounds)).z = stack.m_Range - (stack.m_Range.min - prefabObjectData.m_Bounds.min.z);
				break;
			}
			}
			if (ObjectUtils.GetStandingLegCount(prefabObjectData, out var legCount))
			{
				float4x4 val4 = default(float4x4);
				for (int i = 0; i < legCount; i++)
				{
					float3 standingLegPosition = ObjectUtils.GetStandingLegPosition(prefabObjectData, transform, i);
					((float4x4)(ref val4))._002Ector(transform.m_Rotation, standingLegPosition);
					if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.CircularLeg) != Game.Objects.GeometryFlags.None)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val4, new float3(0f, prefabObjectData.m_LegSize.y * 0.5f, 0f), prefabObjectData.m_LegSize.x * 0.5f, prefabObjectData.m_LegSize.y, outlineColor, 36);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val4, new float3(0f, prefabObjectData.m_LegSize.y * 0.5f, 0f), prefabObjectData.m_LegSize, outlineColor);
					}
				}
				float4x4 val5 = default(float4x4);
				((float4x4)(ref val5))._002Ector(transform.m_Rotation, transform.m_Position);
				if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					float num = prefabObjectData.m_Size.y - prefabObjectData.m_LegSize.y;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val5, new float3(0f, prefabObjectData.m_LegSize.y + num * 0.5f, 0f), prefabObjectData.m_Size.x * 0.5f, num, outlineColor, 36);
					return;
				}
				prefabObjectData.m_Bounds.min.y = prefabObjectData.m_LegSize.y;
				float3 val6 = MathUtils.Center(prefabObjectData.m_Bounds);
				float3 val7 = MathUtils.Size(prefabObjectData.m_Bounds);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val5, val6, val7, outlineColor);
			}
			else
			{
				float4x4 val8 = default(float4x4);
				((float4x4)(ref val8))._002Ector(transform.m_Rotation, transform.m_Position);
				if ((prefabObjectData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val8, new float3(0f, prefabObjectData.m_Size.y * 0.5f, 0f), prefabObjectData.m_Size.x * 0.5f, prefabObjectData.m_Size.y, outlineColor, 36);
					return;
				}
				float3 val9 = MathUtils.Center(prefabObjectData.m_Bounds);
				float3 val10 = MathUtils.Size(prefabObjectData.m_Bounds);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val8, val9, val10, outlineColor);
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stack> __Game_Objects_Stack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometry> __Game_Objects_ObjectGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> __Game_Objects_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Lot> __Game_Buildings_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

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
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Stack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stack>(true);
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Objects_ObjectGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometry>(true);
			__Game_Objects_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Marker>(true);
			__Game_Objects_SpawnLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Lot>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
		}
	}

	private EntityQuery m_ObjectGroup;

	private GizmosSystem m_GizmosSystem;

	private ToolSystem m_ToolSystem;

	private Option m_GeometryOption;

	private Option m_MarkerOption;

	private Option m_PivotOption;

	private Option m_OutlineOption;

	private Option m_InterpolatedOption;

	private Option m_NetConnectionOption;

	private Option m_GroupConnectionOption;

	private Option m_DistrictOption;

	private Option m_LotHeightOption;

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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ObjectGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_GeometryOption = AddOption("Physical Objects", defaultEnabled: true);
		m_MarkerOption = AddOption("Marker Objects", defaultEnabled: true);
		m_PivotOption = AddOption("Draw Pivots", defaultEnabled: true);
		m_OutlineOption = AddOption("Draw Outlines", defaultEnabled: true);
		m_InterpolatedOption = AddOption("Interpolated Positions", defaultEnabled: true);
		m_NetConnectionOption = AddOption("Net Connections", defaultEnabled: true);
		m_GroupConnectionOption = AddOption("Group Connections", defaultEnabled: true);
		m_DistrictOption = AddOption("District Connections", defaultEnabled: false);
		m_LotHeightOption = AddOption("Lot Heights", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ObjectGroup)).IsEmptyIgnoreFilter)
		{
			((SystemBase)this).Dependency = DrawObjectGizmos(m_ObjectGroup, ((SystemBase)this).Dependency);
		}
	}

	private JobHandle DrawObjectGizmos(EntityQuery group, JobHandle inputDeps)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<ObjectGizmoJob>(new ObjectGizmoJob
		{
			m_GeometryOption = m_GeometryOption.enabled,
			m_MarkerOption = m_MarkerOption.enabled,
			m_PivotOption = m_PivotOption.enabled,
			m_OutlineOption = m_OutlineOption.enabled,
			m_InterpolatedOption = m_InterpolatedOption.enabled,
			m_NetConnectionOption = m_NetConnectionOption.enabled,
			m_GroupConnectionOption = m_GroupConnectionOption.enabled,
			m_DistrictOption = m_DistrictOption.enabled,
			m_LotHeightOption = m_LotHeightOption.enabled,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StackType = InternalCompilerInterface.GetComponentTypeHandle<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryType = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometry>(ref __TypeHandle.__Game_Objects_ObjectGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MarkerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingLotType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Selected = m_ToolSystem.selected,
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, group, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
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
	public ObjectDebugSystem()
	{
	}
}
