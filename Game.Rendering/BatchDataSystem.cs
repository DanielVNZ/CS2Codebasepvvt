using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BatchDataSystem : GameSystemBase, IPostDeserialize
{
	private struct UpdateMask
	{
		private uint m_Mask;

		public void UpdateAll()
		{
			m_Mask = uint.MaxValue;
		}

		public void UpdateTransform()
		{
			m_Mask |= 1u;
		}

		public bool ShouldUpdateAll()
		{
			return m_Mask == uint.MaxValue;
		}

		public bool ShouldUpdateNothing()
		{
			return m_Mask == 0;
		}

		public bool ShouldUpdateTransform()
		{
			return (m_Mask & 1) != 0;
		}

		public void UpdateProperty(ObjectProperty property)
		{
			m_Mask |= (uint)(2 << (int)property);
		}

		public bool ShouldUpdateProperty(ObjectProperty property)
		{
			return (m_Mask & (uint)(2 << (int)property)) != 0;
		}

		public bool ShouldUpdateProperty(ObjectProperty property, in GroupData groupData, out int index)
		{
			index = -1;
			if ((m_Mask & (uint)(2 << (int)property)) != 0)
			{
				return groupData.GetPropertyIndex((int)property, out index);
			}
			return false;
		}

		public void UpdateProperty(NetProperty property)
		{
			m_Mask |= (uint)(2 << (int)property);
		}

		public bool ShouldUpdateProperty(NetProperty property)
		{
			return (m_Mask & (uint)(2 << (int)property)) != 0;
		}

		public bool ShouldUpdateProperty(NetProperty property, in GroupData groupData, out int index)
		{
			index = -1;
			if ((m_Mask & (uint)(2 << (int)property)) != 0)
			{
				return groupData.GetPropertyIndex((int)property, out index);
			}
			return false;
		}

		public void UpdateProperty(LaneProperty property)
		{
			m_Mask |= (uint)(2 << (int)property);
		}

		public bool ShouldUpdateProperty(LaneProperty property, in GroupData groupData, out int index)
		{
			index = -1;
			if ((m_Mask & (uint)(2 << (int)property)) != 0)
			{
				return groupData.GetPropertyIndex((int)property, out index);
			}
			return false;
		}

		public bool ShouldUpdateProperty(LaneProperty property)
		{
			return (m_Mask & (uint)(2 << (int)property)) != 0;
		}

		public void UpdateProperty(ZoneProperty property)
		{
			m_Mask |= (uint)(2 << (int)property);
		}

		public bool ShouldUpdateProperty(ZoneProperty property, in GroupData groupData, out int index)
		{
			index = -1;
			if ((m_Mask & (uint)(2 << (int)property)) != 0)
			{
				return groupData.GetPropertyIndex((int)property, out index);
			}
			return false;
		}

		public bool ShouldUpdateProperty(ZoneProperty property)
		{
			return (m_Mask & (uint)(2 << (int)property)) != 0;
		}
	}

	private struct UpdateMasks
	{
		public UpdateMask m_ObjectMask;

		public UpdateMask m_NetMask;

		public UpdateMask m_LaneMask;

		public UpdateMask m_ZoneMask;

		public void UpdateAll()
		{
			m_ObjectMask.UpdateAll();
			m_NetMask.UpdateAll();
			m_LaneMask.UpdateAll();
			m_ZoneMask.UpdateAll();
		}

		public bool ShouldUpdateAll()
		{
			return m_ObjectMask.ShouldUpdateAll();
		}
	}

	private enum SmoothingType
	{
		SurfaceWetness,
		SurfaceDamage,
		SurfaceDirtyness,
		ColorMask
	}

	private struct SmoothingNeeded : IAccumulable<SmoothingNeeded>
	{
		private uint m_Value;

		public SmoothingNeeded(SmoothingType type)
		{
			m_Value = (uint)(1 << (int)type);
		}

		public bool IsNeeded(SmoothingType type)
		{
			return (m_Value & (uint)(1 << (int)type)) != 0;
		}

		public void Accumulate(SmoothingNeeded other)
		{
			m_Value |= other.m_Value;
		}
	}

	private struct CellTypes
	{
		public float4x4 m_CellTypes0;

		public float4x4 m_CellTypes1;

		public float4x4 m_CellTypes2;

		public float4x4 m_CellTypes3;
	}

	[BurstCompile]
	private struct BatchDataJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Error> m_ErrorData;

		[ReadOnly]
		public ComponentLookup<Warning> m_WarningData;

		[ReadOnly]
		public ComponentLookup<Override> m_OverrideData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public BufferLookup<MeshBatch> m_MeshBatches;

		[ReadOnly]
		public BufferLookup<FadeBatch> m_FadeBatches;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColors;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<Animated> m_Animateds;

		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Emissive> m_Emissives;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<Transform> m_ObjectTransformData;

		[ReadOnly]
		public ComponentLookup<Color> m_ObjectColorData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ObjectElevationData;

		[ReadOnly]
		public ComponentLookup<Surface> m_ObjectSurfaceData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_ObjectDamagedData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<CitizenPresence> m_CitizenPresenceData;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_BuildingAbandonedData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public BufferLookup<Passenger> m_Passengers;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<NodeLane> m_NodeLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<EdgeColor> m_NetEdgeColorData;

		[ReadOnly]
		public ComponentLookup<NodeColor> m_NetNodeColorData;

		[ReadOnly]
		public ComponentLookup<LaneColor> m_LaneColorData;

		[ReadOnly]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public ComponentLookup<HangingLane> m_HangingLaneData;

		[ReadOnly]
		public BufferLookup<SubFlow> m_SubFlows;

		[ReadOnly]
		public BufferLookup<CutRange> m_CutRanges;

		[ReadOnly]
		public ComponentLookup<Block> m_ZoneBlockData;

		[ReadOnly]
		public BufferLookup<Cell> m_ZoneCells;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> m_PrefabGrowthScaleData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PrefabPublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_PrefabMeshData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public BufferLookup<SubMesh> m_PrefabSubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_PrefabSubMeshGroups;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_PrefabAnimationClips;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public float m_LightFactor;

		[ReadOnly]
		public float m_FrameDelta;

		[ReadOnly]
		public float4 m_BuildingStateOverride;

		[ReadOnly]
		public PreCullingFlags m_CullingFlags;

		[ReadOnly]
		public float m_SmoothnessDelta;

		[ReadOnly]
		public UpdateMasks m_UpdateMasks;

		[ReadOnly]
		public RenderingSettingsData m_RenderingSettingsData;

		[ReadOnly]
		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public CellMapData<Wind> m_WindData;

		public ParallelInstanceWriter<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public ParallelWriter<SmoothingNeeded> m_SmoothingNeeded;

		public void Execute(int index)
		{
			PreCullingData preCullingData = m_CullingData[index];
			if ((preCullingData.m_Flags & m_CullingFlags) != 0 && (preCullingData.m_Flags & PreCullingFlags.NearCamera) != 0)
			{
				UpdateMasks updateMasks = m_UpdateMasks;
				if (!updateMasks.ShouldUpdateAll() && (preCullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated)) != 0)
				{
					updateMasks.UpdateAll();
				}
				if ((preCullingData.m_Flags & PreCullingFlags.Object) != 0)
				{
					UpdateObjectData(preCullingData, updateMasks.m_ObjectMask);
				}
				else if ((preCullingData.m_Flags & PreCullingFlags.Net) != 0)
				{
					UpdateNetData(preCullingData, updateMasks.m_NetMask);
				}
				else if ((preCullingData.m_Flags & PreCullingFlags.Lane) != 0)
				{
					UpdateLaneData(preCullingData, updateMasks.m_LaneMask);
				}
				else if ((preCullingData.m_Flags & PreCullingFlags.Zone) != 0)
				{
					UpdateZoneData(preCullingData, updateMasks.m_ZoneMask);
				}
				else if ((preCullingData.m_Flags & PreCullingFlags.FadeContainer) != 0)
				{
					UpdateFadeData(preCullingData);
				}
			}
		}

		private void UpdateObjectData(PreCullingData preCullingData, UpdateMask updateMask)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_098a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_09da: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ffd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1001: Unknown result type (might be due to invalid IL or missing references)
			//IL_1006: Unknown result type (might be due to invalid IL or missing references)
			//IL_1008: Unknown result type (might be due to invalid IL or missing references)
			//IL_1010: Unknown result type (might be due to invalid IL or missing references)
			//IL_101c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1021: Unknown result type (might be due to invalid IL or missing references)
			//IL_1023: Unknown result type (might be due to invalid IL or missing references)
			//IL_1028: Unknown result type (might be due to invalid IL or missing references)
			//IL_102d: Unknown result type (might be due to invalid IL or missing references)
			//IL_102f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1036: Unknown result type (might be due to invalid IL or missing references)
			//IL_103b: Unknown result type (might be due to invalid IL or missing references)
			//IL_103d: Unknown result type (might be due to invalid IL or missing references)
			//IL_103f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1044: Unknown result type (might be due to invalid IL or missing references)
			//IL_104b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1050: Unknown result type (might be due to invalid IL or missing references)
			//IL_1052: Unknown result type (might be due to invalid IL or missing references)
			//IL_1057: Unknown result type (might be due to invalid IL or missing references)
			//IL_105c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1064: Unknown result type (might be due to invalid IL or missing references)
			//IL_1079: Unknown result type (might be due to invalid IL or missing references)
			//IL_107b: Unknown result type (might be due to invalid IL or missing references)
			//IL_107d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbc: Unknown result type (might be due to invalid IL or missing references)
			if (!updateMask.ShouldUpdateTransform() && (m_CullingFlags & preCullingData.m_Flags & (PreCullingFlags.TreeGrowth | PreCullingFlags.InterpolatedTransform)) != 0)
			{
				updateMask.UpdateTransform();
			}
			if (!updateMask.ShouldUpdateProperty(ObjectProperty.AnimationCoordinate) && (preCullingData.m_Flags & PreCullingFlags.Animated) != 0)
			{
				updateMask.UpdateProperty(ObjectProperty.AnimationCoordinate);
			}
			if (!updateMask.ShouldUpdateProperty(ObjectProperty.ColorMask1) && (preCullingData.m_Flags & PreCullingFlags.ColorsUpdated) != 0)
			{
				updateMask.UpdateProperty(ObjectProperty.ColorMask1);
				updateMask.UpdateProperty(ObjectProperty.ColorMask2);
				updateMask.UpdateProperty(ObjectProperty.ColorMask3);
			}
			DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
			if (updateMask.ShouldUpdateNothing() || !m_MeshBatches.TryGetBuffer(preCullingData.m_Entity, ref val))
			{
				return;
			}
			DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
			m_MeshGroups.TryGetBuffer(preCullingData.m_Entity, ref val2);
			DynamicBuffer<MeshBatch> val3 = default(DynamicBuffer<MeshBatch>);
			MeshGroup meshGroup = default(MeshGroup);
			DynamicBuffer<SubMeshGroup> val8 = default(DynamicBuffer<SubMeshGroup>);
			GrowthScaleData growthScaleData = default(GrowthScaleData);
			Stack stack = default(Stack);
			MeshGroup meshGroup2 = default(MeshGroup);
			DynamicBuffer<SubMeshGroup> val19 = default(DynamicBuffer<SubMeshGroup>);
			Color color = default(Color);
			Owner owner = default(Owner);
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			Color color2 = default(Color);
			Owner owner2 = default(Owner);
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
			DynamicBuffer<Passenger> val23 = default(DynamicBuffer<Passenger>);
			Owner owner3 = default(Owner);
			CitizenPresence citizenPresence = default(CitizenPresence);
			Building building = default(Building);
			DynamicBuffer<MeshColor> val24 = default(DynamicBuffer<MeshColor>);
			MeshGroup meshGroup3 = default(MeshGroup);
			DynamicBuffer<Skeleton> val25 = default(DynamicBuffer<Skeleton>);
			MeshGroup meshGroup4 = default(MeshGroup);
			MeshGroup meshGroup5 = default(MeshGroup);
			DynamicBuffer<Emissive> val29 = default(DynamicBuffer<Emissive>);
			Surface surface = default(Surface);
			Owner owner4 = default(Owner);
			Surface surface2 = default(Surface);
			float4 val33 = default(float4);
			Game.Objects.Elevation elevation2 = default(Game.Objects.Elevation);
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				GroupData groupData = m_NativeBatchGroups.GetGroupData(meshBatch.m_GroupIndex);
				if (updateMask.ShouldUpdateAll() && (preCullingData.m_Flags & PreCullingFlags.Temp) != 0)
				{
					Temp temp = m_TempData[preCullingData.m_Entity];
					if (m_MeshBatches.TryGetBuffer(temp.m_Original, ref val3))
					{
						for (int j = 0; j < val3.Length; j++)
						{
							MeshBatch meshBatch2 = val3[j];
							if (meshBatch2.m_MeshGroup == meshBatch.m_MeshGroup && meshBatch2.m_MeshIndex == meshBatch.m_MeshIndex && meshBatch2.m_TileIndex == meshBatch.m_TileIndex)
							{
								ref CullingData reference = ref m_NativeBatchInstances.AccessCullingData(meshBatch2.m_GroupIndex, meshBatch2.m_InstanceIndex);
								ref CullingData reference2 = ref m_NativeBatchInstances.AccessCullingData(meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
								reference2.lodFade = reference.lodFade;
								int4 lodFade = reference2.lodFade;
								int2 zw = ((int4)(ref lodFade)).zw;
								int4 val4 = new int4(zw, -zw);
								int4 val5 = new int4(-zw, zw);
								byte lodCount = groupData.m_LodCount;
								lodFade = reference2.lodFade;
								bool2 val6 = (((int)lodCount - ((int4)(ref lodFade)).xy) & 1) != 0;
								int4 val7 = math.select(val4, val5, ((bool2)(ref val6)).xyxy);
								((int4)(ref val7)).xz = (1065353471 - ((int4)(ref val7)).xz) | (255 - ((int4)(ref val7)).yw << 11);
								if (updateMask.ShouldUpdateProperty(ObjectProperty.LodFade0, in groupData, out var index))
								{
									m_NativeBatchInstances.SetPropertyValue<int>(val7.x, meshBatch.m_GroupIndex, index, meshBatch.m_InstanceIndex);
								}
								if (updateMask.ShouldUpdateProperty(ObjectProperty.LodFade1, in groupData, out var index2))
								{
									m_NativeBatchInstances.SetPropertyValue<int>(val7.z, meshBatch.m_GroupIndex, index2, meshBatch.m_InstanceIndex);
								}
								if (m_NativeBatchInstances.InitializeTransform(meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex, meshBatch2.m_GroupIndex, meshBatch2.m_InstanceIndex))
								{
									break;
								}
							}
						}
					}
				}
				if (updateMask.ShouldUpdateTransform())
				{
					CullingInfo cullingInfo = m_CullingInfoData[preCullingData.m_Entity];
					PrefabRef prefabRef = m_PrefabRefData[preCullingData.m_Entity];
					int num = meshBatch.m_MeshIndex;
					if (CollectionUtils.TryGet<MeshGroup>(val2, (int)meshBatch.m_MeshGroup, ref meshGroup) && m_PrefabSubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val8))
					{
						num += val8[(int)meshGroup.m_SubMeshGroup].m_SubMeshRange.x;
					}
					SubMesh subMesh = m_PrefabSubMeshes[prefabRef.m_Prefab][num];
					float3 subMeshScale = float3.op_Implicit(1f);
					if ((preCullingData.m_Flags & PreCullingFlags.TreeGrowth) != 0 && m_PrefabGrowthScaleData.TryGetComponent(prefabRef.m_Prefab, ref growthScaleData))
					{
						BatchDataHelpers.CalculateTreeSubMeshData(m_TreeData[preCullingData.m_Entity], growthScaleData, out var scale);
						subMeshScale *= scale;
					}
					if ((subMesh.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd)) != 0 && m_StackData.TryGetComponent(preCullingData.m_Entity, ref stack) && m_PrefabStackData.HasComponent(prefabRef.m_Prefab))
					{
						StackData stackData = m_PrefabStackData[prefabRef.m_Prefab];
						BatchDataHelpers.CalculateStackSubMeshData(stack, stackData, out var _, out var offsets, out var scale2);
						BatchDataHelpers.CalculateStackSubMeshData(stackData, offsets, scale2, meshBatch.m_TileIndex, subMesh.m_Flags, ref subMesh.m_Position, ref subMeshScale);
					}
					Transform transform = (((preCullingData.m_Flags & PreCullingFlags.InterpolatedTransform) == 0) ? m_ObjectTransformData[preCullingData.m_Entity] : m_InterpolatedTransformData[preCullingData.m_Entity].ToTransform());
					float3 val9;
					quaternion val10;
					if ((subMesh.m_Flags & (SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd | SubMeshFlags.HasTransform)) != 0)
					{
						val9 = transform.m_Position + math.rotate(transform.m_Rotation, subMesh.m_Position);
						val10 = math.mul(transform.m_Rotation, subMesh.m_Rotation);
					}
					else
					{
						val9 = transform.m_Position;
						val10 = transform.m_Rotation;
					}
					int lodOffset = 0;
					float3x4 val13;
					float3x4 val14;
					if ((subMesh.m_Flags & SubMeshFlags.DefaultMissingMesh) != 0)
					{
						MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
						ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
						if ((subMesh.m_Flags & SubMeshFlags.IsStackMiddle) != 0)
						{
							switch (m_PrefabStackData[prefabRef.m_Prefab].m_Direction)
							{
							case StackDirection.Right:
								((Bounds3)(ref objectGeometryData.m_Bounds)).x = new Bounds1(-1f, 1f);
								break;
							case StackDirection.Up:
								((Bounds3)(ref objectGeometryData.m_Bounds)).y = new Bounds1(-1f, 1f);
								break;
							case StackDirection.Forward:
								((Bounds3)(ref objectGeometryData.m_Bounds)).z = new Bounds1(-1f, 1f);
								break;
							}
						}
						float3 val11 = val9 + math.rotate(val10, subMeshScale * MathUtils.Center(objectGeometryData.m_Bounds));
						float3 val12 = subMeshScale * MathUtils.Size(objectGeometryData.m_Bounds) * 0.5f;
						val13 = TransformHelper.TRS(val11, val10, val12);
						val14 = val13;
						lodOffset = objectGeometryData.m_MinLod - meshData.m_MinLod;
					}
					else
					{
						float3 val15 = val9 + math.rotate(val10, subMeshScale * groupData.m_SecondaryCenter);
						float3 val16 = subMeshScale * groupData.m_SecondarySize;
						val13 = TransformHelper.TRS(val9, val10, subMeshScale);
						val14 = TransformHelper.TRS(val15, val10, val16);
					}
					ref CullingData reference3 = ref m_NativeBatchInstances.SetTransformValue(val13, val14, meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
					reference3.m_Bounds = cullingInfo.m_Bounds;
					reference3.isHidden = m_HiddenData.HasComponent(preCullingData.m_Entity);
					reference3.lodOffset = lodOffset;
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.AnimationCoordinate, in groupData, out var index3))
				{
					float3 val17 = float3.zero;
					if ((preCullingData.m_Flags & PreCullingFlags.Animated) != 0)
					{
						DynamicBuffer<Animated> val18 = m_Animateds[preCullingData.m_Entity];
						PrefabRef prefabRef2 = m_PrefabRefData[preCullingData.m_Entity];
						int num2 = meshBatch.m_MeshIndex;
						int num3 = meshBatch.m_MeshIndex;
						if (CollectionUtils.TryGet<MeshGroup>(val2, (int)meshBatch.m_MeshGroup, ref meshGroup2) && m_PrefabSubMeshGroups.TryGetBuffer(prefabRef2.m_Prefab, ref val19))
						{
							num3 += val19[(int)meshGroup2.m_SubMeshGroup].m_SubMeshRange.x;
							num2 = meshBatch.m_MeshGroup;
						}
						Animated animated = val18[num2];
						if (animated.m_ClipIndexBody0 != -1)
						{
							SubMesh subMesh2 = m_PrefabSubMeshes[prefabRef2.m_Prefab][num3];
							val17 = BatchDataHelpers.GetAnimationCoordinate(m_PrefabAnimationClips[subMesh2.m_SubMesh][(int)animated.m_ClipIndexBody0], animated.m_Time.x, animated.m_PreviousTime);
						}
					}
					m_NativeBatchInstances.SetPropertyValue<float3>(val17, meshBatch.m_GroupIndex, index3, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.InfoviewColor, in groupData, out var index4))
				{
					float2 val20 = default(float2);
					if ((preCullingData.m_Flags & PreCullingFlags.InfoviewColor) != 0)
					{
						if (m_ObjectColorData.TryGetComponent(preCullingData.m_Entity, ref color))
						{
							((float2)(ref val20))._002Ector((float)(int)color.m_Index + 0.5f, (float)(int)color.m_Value * 0.003921569f);
						}
						else if (m_OwnerData.TryGetComponent(preCullingData.m_Entity, ref owner))
						{
							bool flag = m_ObjectElevationData.TryGetComponent(preCullingData.m_Entity, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0;
							while (true)
							{
								if (m_ObjectColorData.TryGetComponent(owner.m_Owner, ref color2))
								{
									if (flag || color2.m_SubColor)
									{
										((float2)(ref val20))._002Ector((float)(int)color2.m_Index + 0.5f, (float)(int)color2.m_Value * 0.003921569f);
									}
									break;
								}
								if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
								{
									break;
								}
								flag &= m_ObjectElevationData.TryGetComponent(owner.m_Owner, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0;
								owner = owner2;
							}
						}
					}
					m_NativeBatchInstances.SetPropertyValue<float2>(val20, meshBatch.m_GroupIndex, index4, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.BuildingState, in groupData, out var index5))
				{
					float4 val21;
					if (m_EditorMode)
					{
						val21 = m_BuildingStateOverride;
					}
					else
					{
						Entity val22 = preCullingData.m_Entity;
						if ((preCullingData.m_Flags & PreCullingFlags.Temp) != 0)
						{
							Temp temp2 = m_TempData[preCullingData.m_Entity];
							if (temp2.m_Original != Entity.Null)
							{
								val22 = temp2.m_Original;
							}
						}
						m_PseudoRandomSeedData.TryGetComponent(val22, ref pseudoRandomSeed);
						bool flag2 = m_DestroyedData.HasComponent(val22);
						if (m_VehicleData.HasComponent(val22))
						{
							int num4 = 0;
							int passengersCount = 0;
							if (m_PublicTransportData.TryGetComponent(val22, ref publicTransport))
							{
								PrefabRef prefabRef3 = m_PrefabRefData[val22];
								if (m_PrefabPublicTransportVehicleData.TryGetComponent(prefabRef3.m_Prefab, ref publicTransportVehicleData))
								{
									num4 = publicTransportVehicleData.m_PassengerCapacity;
								}
								if ((publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0)
								{
									Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kDummyPassengers);
									passengersCount = ((Random)(ref random)).NextInt(0, num4 + 1);
								}
								else if (m_Passengers.TryGetBuffer(val22, ref val23))
								{
									passengersCount = val23.Length;
								}
							}
							else
							{
								Random random2 = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kDummyPassengers);
								num4 = 1000;
								passengersCount = ((Random)(ref random2)).NextInt(0, num4 + 1);
							}
							val21 = BatchDataHelpers.GetBuildingState(pseudoRandomSeed, passengersCount, num4, m_LightFactor, flag2);
						}
						else
						{
							if (m_OwnerData.TryGetComponent(val22, ref owner3))
							{
								val22 = owner3.m_Owner;
							}
							m_CitizenPresenceData.TryGetComponent(val22, ref citizenPresence);
							bool flag3 = m_BuildingAbandonedData.HasComponent(val22);
							bool electricity = true;
							if (m_BuildingData.TryGetComponent(val22, ref building))
							{
								electricity = (building.m_Flags & Game.Buildings.BuildingFlags.Illuminated) != 0;
							}
							val21 = BatchDataHelpers.GetBuildingState(pseudoRandomSeed, citizenPresence, m_LightFactor, flag3 || flag2, electricity);
						}
					}
					m_NativeBatchInstances.SetPropertyValue<float4>(val21, meshBatch.m_GroupIndex, index5, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.ColorMask1))
				{
					Color color3;
					Color color4;
					Color color5;
					if (m_MeshColors.TryGetBuffer(preCullingData.m_Entity, ref val24))
					{
						int num5 = meshBatch.m_MeshIndex;
						if (CollectionUtils.TryGet<MeshGroup>(val2, (int)meshBatch.m_MeshGroup, ref meshGroup3))
						{
							num5 += meshGroup3.m_ColorOffset;
						}
						MeshColor meshColor = val24[num5];
						color3 = ((Color)(ref meshColor.m_ColorSet.m_Channel0)).linear;
						color4 = ((Color)(ref meshColor.m_ColorSet.m_Channel1)).linear;
						color5 = ((Color)(ref meshColor.m_ColorSet.m_Channel2)).linear;
					}
					else
					{
						color3 = Color.white;
						color4 = Color.white;
						color5 = Color.white;
					}
					bool smooth = (preCullingData.m_Flags & PreCullingFlags.SmoothColor) != 0;
					UpdateColorMask(in groupData, in meshBatch, updateMask, ObjectProperty.ColorMask1, color3, smooth);
					UpdateColorMask(in groupData, in meshBatch, updateMask, ObjectProperty.ColorMask2, color4, smooth);
					UpdateColorMask(in groupData, in meshBatch, updateMask, ObjectProperty.ColorMask3, color5, smooth);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.BoneParameters, in groupData, out var index6))
				{
					float2 val26;
					if (m_Skeletons.TryGetBuffer(preCullingData.m_Entity, ref val25))
					{
						int num6 = meshBatch.m_MeshIndex;
						if (CollectionUtils.TryGet<MeshGroup>(val2, (int)meshBatch.m_MeshGroup, ref meshGroup4))
						{
							num6 += meshGroup4.m_MeshOffset;
						}
						val26 = BatchDataHelpers.GetBoneParameters(val25[num6]);
					}
					else if ((preCullingData.m_Flags & PreCullingFlags.Animated) != 0)
					{
						DynamicBuffer<Animated> val27 = m_Animateds[preCullingData.m_Entity];
						PrefabRef prefabRef4 = m_PrefabRefData[preCullingData.m_Entity];
						int num7 = meshBatch.m_MeshIndex;
						if (m_PrefabSubMeshGroups.HasBuffer(prefabRef4.m_Prefab))
						{
							num7 = ((val2.IsCreated && val2.Length != 0) ? meshBatch.m_MeshGroup : 0);
						}
						val26 = BatchDataHelpers.GetBoneParameters(val27[num7]);
					}
					else
					{
						val26 = float2.zero;
					}
					m_NativeBatchInstances.SetPropertyValue<float2>(val26, meshBatch.m_GroupIndex, index6, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.LightParameters, in groupData, out var index7))
				{
					int num8 = meshBatch.m_MeshIndex;
					if (CollectionUtils.TryGet<MeshGroup>(val2, (int)meshBatch.m_MeshGroup, ref meshGroup5))
					{
						num8 += meshGroup5.m_MeshOffset;
					}
					float2 val28 = ((!m_Emissives.TryGetBuffer(preCullingData.m_Entity, ref val29)) ? float2.zero : BatchDataHelpers.GetLightParameters(val29[num8]));
					m_NativeBatchInstances.SetPropertyValue<float2>(val28, meshBatch.m_GroupIndex, index7, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.OutlineColors, in groupData, out var index8))
				{
					Color val30 = (m_ErrorData.HasComponent(preCullingData.m_Entity) ? m_RenderingSettingsData.m_ErrorColor : (m_WarningData.HasComponent(preCullingData.m_Entity) ? m_RenderingSettingsData.m_WarningColor : (m_OverrideData.HasComponent(preCullingData.m_Entity) ? m_RenderingSettingsData.m_OverrideColor : (((preCullingData.m_Flags & PreCullingFlags.Temp) == 0) ? m_RenderingSettingsData.m_HoveredColor : (((m_TempData[preCullingData.m_Entity].m_Flags & TempFlags.Parent) == 0) ? m_RenderingSettingsData.m_HoveredColor : m_RenderingSettingsData.m_OwnerColor)))));
					val30 = ((Color)(ref val30)).linear;
					m_NativeBatchInstances.SetPropertyValue<Color>(val30, meshBatch.m_GroupIndex, index8, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.MetaParameters, in groupData, out var index9))
				{
					float num9 = 0f;
					if ((preCullingData.m_Flags & PreCullingFlags.Animated) != 0)
					{
						DynamicBuffer<Animated> val31 = m_Animateds[preCullingData.m_Entity];
						PrefabRef prefabRef5 = m_PrefabRefData[preCullingData.m_Entity];
						int num10 = meshBatch.m_MeshIndex;
						if (m_PrefabSubMeshGroups.HasBuffer(prefabRef5.m_Prefab))
						{
							num10 = ((val2.IsCreated && val2.Length != 0) ? meshBatch.m_MeshGroup : 0);
						}
						num9 = math.asfloat(val31[num10].m_MetaIndex);
					}
					m_NativeBatchInstances.SetPropertyValue<float>(num9, meshBatch.m_GroupIndex, index9, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.SurfaceWetness, in groupData, out var index10))
				{
					float4 val32 = default(float4);
					if ((preCullingData.m_Flags & PreCullingFlags.SurfaceState) != 0)
					{
						if (m_ObjectSurfaceData.TryGetComponent(preCullingData.m_Entity, ref surface))
						{
							val32 = BatchDataHelpers.GetWetness(surface);
						}
						else if (m_OwnerData.TryGetComponent(preCullingData.m_Entity, ref owner4))
						{
							while (true)
							{
								if (m_ObjectSurfaceData.TryGetComponent(owner4.m_Owner, ref surface2))
								{
									val32 = BatchDataHelpers.GetWetness(surface2);
									break;
								}
								if (!m_OwnerData.HasComponent(owner4.m_Owner))
								{
									break;
								}
								owner4 = m_OwnerData[owner4.m_Owner];
							}
						}
					}
					if (m_NativeBatchInstances.GetPropertyValue<float4>(ref val33, meshBatch.m_GroupIndex, index10, meshBatch.m_InstanceIndex))
					{
						if (math.any(val33 != val32))
						{
							bool4 val34 = val32 < val33;
							val33 += math.select(float4.op_Implicit(m_SmoothnessDelta), float4.op_Implicit(0f - m_SmoothnessDelta), val34);
							val33 = math.clamp(val33, math.select(float4.op_Implicit(0f), val32, val34), math.select(val32, float4.op_Implicit(1f), val34));
							m_NativeBatchInstances.SetPropertyValue<float4>(val33, meshBatch.m_GroupIndex, index10, meshBatch.m_InstanceIndex);
							if (math.any(val33 != val32))
							{
								m_SmoothingNeeded.Accumulate(new SmoothingNeeded(SmoothingType.SurfaceWetness));
							}
						}
					}
					else
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(val32, meshBatch.m_GroupIndex, index10, meshBatch.m_InstanceIndex);
					}
				}
				if (updateMask.ShouldUpdateProperty(ObjectProperty.BaseState, in groupData, out var index11))
				{
					float num11 = 0f;
					if (m_ObjectElevationData.TryGetComponent(preCullingData.m_Entity, ref elevation2) && (elevation2.m_Flags & (ElevationFlags.Stacked | ElevationFlags.OnGround | ElevationFlags.Lowered)) != ElevationFlags.OnGround)
					{
						num11 = 1f;
					}
					m_NativeBatchInstances.SetPropertyValue<float>(num11, meshBatch.m_GroupIndex, index11, meshBatch.m_InstanceIndex);
				}
			}
		}

		private void UpdateColorMask(in GroupData groupData, in MeshBatch meshBatch, UpdateMask updateMask, ObjectProperty property, Color color, bool smooth)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (updateMask.ShouldUpdateProperty(property, in groupData, out var index))
			{
				UpdateColorMask(in meshBatch, color, index, smooth);
			}
		}

		private void UpdateColorMask(in GroupData groupData, in MeshBatch meshBatch, UpdateMask updateMask, LaneProperty property, Color color, bool smooth)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (updateMask.ShouldUpdateProperty(property, in groupData, out var index))
			{
				UpdateColorMask(in meshBatch, color, index, smooth);
			}
		}

		private void UpdateColorMask(in MeshBatch meshBatch, Color color, int colorMask, bool smooth)
		{
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			Color val = default(Color);
			if (smooth && m_NativeBatchInstances.GetPropertyValue<Color>(ref val, meshBatch.m_GroupIndex, colorMask, meshBatch.m_InstanceIndex))
			{
				float4 val2 = float4.op_Implicit(Color.op_Implicit(color));
				float4 val3 = float4.op_Implicit(Color.op_Implicit(val));
				float4 val4 = val2 - val3;
				if (math.any(val4 != 0f))
				{
					bool4 val5 = val4 < 0f;
					val3 += val4 * (m_SmoothnessDelta / math.cmax(math.abs(val4)));
					val3 = math.clamp(val3, math.select(float4.op_Implicit(0f), val2, val5), math.select(val2, float4.op_Implicit(1f), val5));
					val = Color.op_Implicit(float4.op_Implicit(val3));
					m_NativeBatchInstances.SetPropertyValue<Color>(val, meshBatch.m_GroupIndex, colorMask, meshBatch.m_InstanceIndex);
					if (math.any(val3 != val2))
					{
						m_SmoothingNeeded.Accumulate(new SmoothingNeeded(SmoothingType.ColorMask));
					}
				}
			}
			else
			{
				m_NativeBatchInstances.SetPropertyValue<Color>(color, meshBatch.m_GroupIndex, colorMask, meshBatch.m_InstanceIndex);
			}
		}

		private void UpdateNetData(PreCullingData preCullingData, UpdateMask updateMask)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
			if (updateMask.ShouldUpdateNothing() || !m_MeshBatches.TryGetBuffer(preCullingData.m_Entity, ref val))
			{
				return;
			}
			EdgeColor edgeColor = default(EdgeColor);
			float2 val3 = default(float2);
			float2 val4 = default(float2);
			NodeColor nodeColor = default(NodeColor);
			NodeColor nodeColor2 = default(NodeColor);
			NodeColor nodeColor3 = default(NodeColor);
			Temp temp3 = default(Temp);
			Temp temp2 = default(Temp);
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				NetSubMesh meshIndex = (NetSubMesh)meshBatch.m_MeshIndex;
				GroupData groupData = m_NativeBatchGroups.GetGroupData(meshBatch.m_GroupIndex);
				if (updateMask.ShouldUpdateTransform())
				{
					CullingInfo cullingInfo = m_CullingInfoData[preCullingData.m_Entity];
					BatchDataHelpers.CompositionParameters compositionParameters = default(BatchDataHelpers.CompositionParameters);
					switch (meshIndex)
					{
					case NetSubMesh.Edge:
					case NetSubMesh.RotatedEdge:
						BatchDataHelpers.CalculateEdgeParameters(m_EdgeGeometryData[preCullingData.m_Entity], meshIndex == NetSubMesh.RotatedEdge, out compositionParameters);
						break;
					case NetSubMesh.StartNode:
					case NetSubMesh.SubStartNode:
					{
						Composition composition2 = m_CompositionData[preCullingData.m_Entity];
						StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[preCullingData.m_Entity];
						BatchDataHelpers.CalculateNodeParameters(prefabCompositionData: m_PrefabCompositionData[composition2.m_StartNode], nodeGeometry: startNodeGeometry.m_Geometry, compositionParameters: out compositionParameters);
						break;
					}
					case NetSubMesh.EndNode:
					case NetSubMesh.SubEndNode:
					{
						Composition composition = m_CompositionData[preCullingData.m_Entity];
						EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[preCullingData.m_Entity];
						BatchDataHelpers.CalculateNodeParameters(prefabCompositionData: m_PrefabCompositionData[composition.m_EndNode], nodeGeometry: endNodeGeometry.m_Geometry, compositionParameters: out compositionParameters);
						break;
					}
					case NetSubMesh.Orphan1:
					case NetSubMesh.Orphan2:
					{
						Node node = m_NodeData[preCullingData.m_Entity];
						Orphan orphan = m_OrphanData[preCullingData.m_Entity];
						NodeGeometry nodeGeometry = m_NodeGeometryData[preCullingData.m_Entity];
						NetCompositionData prefabCompositionData = m_PrefabCompositionData[orphan.m_Composition];
						BatchDataHelpers.CalculateOrphanParameters(node, nodeGeometry, prefabCompositionData, meshIndex == NetSubMesh.Orphan1, out compositionParameters);
						break;
					}
					}
					ref CullingData reference = ref m_NativeBatchInstances.SetTransformValue(compositionParameters.m_TransformMatrix, compositionParameters.m_TransformMatrix, meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
					reference.m_Bounds = cullingInfo.m_Bounds;
					reference.isHidden = m_HiddenData.HasComponent(preCullingData.m_Entity);
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix0, in groupData, out var index))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix0, meshBatch.m_GroupIndex, index, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix1, in groupData, out var index2))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix1, meshBatch.m_GroupIndex, index2, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix2, in groupData, out var index3))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix2, meshBatch.m_GroupIndex, index3, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix3, in groupData, out var index4))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix3, meshBatch.m_GroupIndex, index4, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix4, in groupData, out var index5))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix4, meshBatch.m_GroupIndex, index5, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix5, in groupData, out var index6))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix5, meshBatch.m_GroupIndex, index6, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix6, in groupData, out var index7))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix6, meshBatch.m_GroupIndex, index7, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionMatrix7, in groupData, out var index8))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(compositionParameters.m_CompositionMatrix7, meshBatch.m_GroupIndex, index8, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionSync0, in groupData, out var index9))
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(compositionParameters.m_CompositionSync0, meshBatch.m_GroupIndex, index9, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionSync1, in groupData, out var index10))
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(compositionParameters.m_CompositionSync1, meshBatch.m_GroupIndex, index10, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionSync2, in groupData, out var index11))
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(compositionParameters.m_CompositionSync2, meshBatch.m_GroupIndex, index11, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(NetProperty.CompositionSync3, in groupData, out var index12))
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(compositionParameters.m_CompositionSync3, meshBatch.m_GroupIndex, index12, meshBatch.m_InstanceIndex);
					}
				}
				if (updateMask.ShouldUpdateProperty(NetProperty.InfoviewColor, in groupData, out var index13))
				{
					float4 val2 = default(float4);
					if ((preCullingData.m_Flags & PreCullingFlags.InfoviewColor) != 0)
					{
						if (m_NetEdgeColorData.TryGetComponent(preCullingData.m_Entity, ref edgeColor))
						{
							Edge edge = m_EdgeData[preCullingData.m_Entity];
							((float2)(ref val3))._002Ector((float)(int)edgeColor.m_Index + 0.5f, (float)(int)edgeColor.m_Value0 * 0.003921569f);
							((float2)(ref val4))._002Ector((float)(int)edgeColor.m_Index + 0.5f, (float)(int)edgeColor.m_Value1 * 0.003921569f);
							float2 val5 = val3;
							float2 val6 = val4;
							if (m_NetNodeColorData.TryGetComponent(edge.m_Start, ref nodeColor))
							{
								((float2)(ref val5))._002Ector((float)(int)nodeColor.m_Index + 0.5f, (float)(int)nodeColor.m_Value * 0.003921569f);
							}
							if (m_NetNodeColorData.TryGetComponent(edge.m_End, ref nodeColor2))
							{
								((float2)(ref val6))._002Ector((float)(int)nodeColor2.m_Index + 0.5f, (float)(int)nodeColor2.m_Value * 0.003921569f);
							}
							switch (meshIndex)
							{
							case NetSubMesh.Edge:
							case NetSubMesh.SubStartNode:
							case NetSubMesh.SubEndNode:
								((float4)(ref val2))._002Ector(val3, val4);
								break;
							case NetSubMesh.RotatedEdge:
								((float4)(ref val2))._002Ector(val4, val3);
								break;
							case NetSubMesh.StartNode:
								((float4)(ref val2))._002Ector(val3, val5);
								break;
							case NetSubMesh.EndNode:
								((float4)(ref val2))._002Ector(val4, val6);
								break;
							}
						}
						else if (m_NetNodeColorData.TryGetComponent(preCullingData.m_Entity, ref nodeColor3))
						{
							((float4)(ref val2))._002Ector((float)(int)nodeColor3.m_Index + 0.5f, (float)(int)nodeColor3.m_Value * 0.003921569f, (float)(int)nodeColor3.m_Index + 0.5f, (float)(int)nodeColor3.m_Value * 0.003921569f);
						}
					}
					m_NativeBatchInstances.SetPropertyValue<float4>(val2, meshBatch.m_GroupIndex, index13, meshBatch.m_InstanceIndex);
				}
				if (!updateMask.ShouldUpdateProperty(NetProperty.OutlineColors, in groupData, out var index14))
				{
					continue;
				}
				Color val7;
				if (m_ErrorData.HasComponent(preCullingData.m_Entity))
				{
					val7 = m_RenderingSettingsData.m_ErrorColor;
				}
				else if (m_WarningData.HasComponent(preCullingData.m_Entity))
				{
					val7 = m_RenderingSettingsData.m_WarningColor;
				}
				else if ((preCullingData.m_Flags & PreCullingFlags.Temp) != 0)
				{
					Temp temp = m_TempData[preCullingData.m_Entity];
					val7 = m_RenderingSettingsData.m_HoveredColor;
					if ((temp.m_Flags & TempFlags.Parent) != 0)
					{
						val7 = m_RenderingSettingsData.m_OwnerColor;
					}
					if ((temp.m_Flags & TempFlags.SubDetail) != 0)
					{
						switch (meshIndex)
						{
						case NetSubMesh.StartNode:
						case NetSubMesh.SubStartNode:
						{
							Edge edge3 = m_EdgeData[preCullingData.m_Entity];
							if (m_TempData.TryGetComponent(edge3.m_Start, ref temp3) && (temp3.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent))
							{
								val7 = m_RenderingSettingsData.m_OwnerColor;
							}
							break;
						}
						case NetSubMesh.EndNode:
						case NetSubMesh.SubEndNode:
						{
							Edge edge2 = m_EdgeData[preCullingData.m_Entity];
							if (m_TempData.TryGetComponent(edge2.m_End, ref temp2) && (temp2.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent))
							{
								val7 = m_RenderingSettingsData.m_OwnerColor;
							}
							break;
						}
						}
					}
				}
				else
				{
					val7 = m_RenderingSettingsData.m_HoveredColor;
				}
				val7 = ((Color)(ref val7)).linear;
				m_NativeBatchInstances.SetPropertyValue<Color>(val7, meshBatch.m_GroupIndex, index14, meshBatch.m_InstanceIndex);
			}
		}

		private void UpdateLaneData(PreCullingData preCullingData, UpdateMask updateMask)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_095c: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b86: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			if (!updateMask.ShouldUpdateProperty(LaneProperty.ColorMask1) && (preCullingData.m_Flags & PreCullingFlags.ColorsUpdated) != 0)
			{
				updateMask.UpdateProperty(LaneProperty.ColorMask1);
				updateMask.UpdateProperty(LaneProperty.ColorMask2);
				updateMask.UpdateProperty(LaneProperty.ColorMask3);
			}
			DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
			if (updateMask.ShouldUpdateNothing() || !m_MeshBatches.TryGetBuffer(preCullingData.m_Entity, ref val))
			{
				return;
			}
			float4 val4 = default(float4);
			DynamicBuffer<CutRange> val5 = default(DynamicBuffer<CutRange>);
			float2 val8 = default(float2);
			Bounds1 val9 = default(Bounds1);
			Bounds1 val10 = default(Bounds1);
			NodeLane nodeLane = default(NodeLane);
			EdgeLane edgeLane = default(EdgeLane);
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			LaneColor laneColor = default(LaneColor);
			Owner owner = default(Owner);
			EdgeLane edgeLane2 = default(EdgeLane);
			EdgeColor edgeColor = default(EdgeColor);
			float2 val17 = default(float2);
			float2 val18 = default(float2);
			NodeColor nodeColor = default(NodeColor);
			Color color = default(Color);
			Owner owner2 = default(Owner);
			DynamicBuffer<MeshColor> val19 = default(DynamicBuffer<MeshColor>);
			DynamicBuffer<SubFlow> val21 = default(DynamicBuffer<SubFlow>);
			Owner owner3 = default(Owner);
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			LaneCondition laneCondition = default(LaneCondition);
			PrefabRef prefabRef2 = default(PrefabRef);
			Curve curve3 = default(Curve);
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			HangingLane hangingLane = default(HangingLane);
			float4 val25 = default(float4);
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				GroupData groupData = m_NativeBatchGroups.GetGroupData(meshBatch.m_GroupIndex);
				if (updateMask.ShouldUpdateTransform())
				{
					CullingInfo cullingInfo = m_CullingInfoData[preCullingData.m_Entity];
					Curve curve = m_CurveData[preCullingData.m_Entity];
					PrefabRef prefabRef = m_PrefabRefData[preCullingData.m_Entity];
					SubMesh subMesh = m_PrefabSubMeshes[prefabRef.m_Prefab][(int)meshBatch.m_MeshIndex];
					MeshData meshData = m_PrefabMeshData[subMesh.m_SubMesh];
					float3 val2 = MathUtils.Size(meshData.m_Bounds);
					float3 val3 = MathUtils.Center(meshData.m_Bounds);
					((float4)(ref val4))._002Ector(val2, val3.y);
					bool flag = (meshData.m_State & MeshFlags.Tiling) != 0;
					int num = 1;
					int clipCount = 0;
					int num2 = meshBatch.m_TileIndex;
					if (m_CutRanges.TryGetBuffer(preCullingData.m_Entity, ref val5))
					{
						float num3 = 0f;
						for (int j = 0; j <= val5.Length; j++)
						{
							float num4;
							float num5;
							if (j < val5.Length)
							{
								CutRange cutRange = val5[j];
								num4 = cutRange.m_CurveDelta.min;
								num5 = cutRange.m_CurveDelta.max;
							}
							else
							{
								num4 = 1f;
								num5 = 1f;
							}
							if (num4 >= num3)
							{
								Curve curve2 = new Curve
								{
									m_Length = curve.m_Length * (num4 - num3)
								};
								if (curve2.m_Length > 0.1f)
								{
									num = BatchDataHelpers.GetTileCount(curve2, val4.z, meshData.m_TilingCount, flag, out clipCount);
									if (num > num2)
									{
										curve2.m_Bezier = MathUtils.Cut(curve.m_Bezier, new Bounds1(num3, num4));
										curve = curve2;
										break;
									}
									num2 -= num;
								}
							}
							num3 = num5;
						}
					}
					else if (flag)
					{
						num = BatchDataHelpers.GetTileCount(curve, val4.z, meshData.m_TilingCount, geometryTiling: true, out clipCount);
					}
					if ((meshData.m_State & MeshFlags.Invert) != 0)
					{
						curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					}
					if (num > 1)
					{
						int2 val6 = new int2(num2, num2 + 1) * clipCount / num;
						float2 val7 = curve.m_Length * float2.op_Implicit(val6) / (float)clipCount;
						((float2)(ref val8))._002Ector(0f, 1f);
						if (val6.x != 0)
						{
							((Bounds1)(ref val9))._002Ector(0f, 1f);
							MathUtils.ClampLength(curve.m_Bezier, ref val9, val7.x);
							val8.x = val9.max;
						}
						if (val6.y != clipCount)
						{
							((Bounds1)(ref val10))._002Ector(0f, 1f);
							MathUtils.ClampLength(curve.m_Bezier, ref val10, val7.y);
							val8.y = val10.max;
						}
						curve.m_Bezier = MathUtils.Cut(curve.m_Bezier, val8);
						curve.m_Length = val7.y - val7.x;
					}
					bool flag2 = m_NodeLaneData.TryGetComponent(preCullingData.m_Entity, ref nodeLane);
					float4 val11;
					if (flag2)
					{
						NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
						val11 = BatchDataHelpers.BuildCurveScale(nodeLane, netLaneData);
					}
					else
					{
						val11 = BatchDataHelpers.BuildCurveScale();
					}
					bool isDecal = (meshData.m_State & MeshFlags.Decal) != 0;
					float3x4 val12 = TransformHelper.Convert(BatchDataHelpers.BuildTransformMatrix(curve, val4, val11, meshData.m_SmoothingDistance, isDecal, isLoaded: true));
					float3x4 val13 = TransformHelper.Convert(BatchDataHelpers.BuildTransformMatrix(curve, val4, val11, meshData.m_SmoothingDistance, isDecal, isLoaded: false));
					float4x4 val14 = BatchDataHelpers.BuildCurveMatrix(curve, val12, val4, meshData.m_TilingCount);
					ref CullingData reference = ref m_NativeBatchInstances.SetTransformValue(val12, val13, meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
					reference.m_Bounds = cullingInfo.m_Bounds;
					reference.isHidden = m_HiddenData.HasComponent(preCullingData.m_Entity);
					if (updateMask.ShouldUpdateProperty(LaneProperty.CurveMatrix, in groupData, out var index))
					{
						m_NativeBatchInstances.SetPropertyValue<float4x4>(val14, meshBatch.m_GroupIndex, index, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(LaneProperty.CurveParams, in groupData, out var index2))
					{
						float4 val15 = (flag2 ? BatchDataHelpers.BuildCurveParams(val4, nodeLane) : (m_EdgeLaneData.TryGetComponent(preCullingData.m_Entity, ref edgeLane) ? BatchDataHelpers.BuildCurveParams(val4, edgeLane) : ((!m_NetElevationData.TryGetComponent(preCullingData.m_Entity, ref elevation)) ? BatchDataHelpers.BuildCurveParams(val4) : BatchDataHelpers.BuildCurveParams(val4, elevation))));
						m_NativeBatchInstances.SetPropertyValue<float4>(val15, meshBatch.m_GroupIndex, index2, meshBatch.m_InstanceIndex);
					}
					if (updateMask.ShouldUpdateProperty(LaneProperty.CurveScale, in groupData, out var index3))
					{
						m_NativeBatchInstances.SetPropertyValue<float4>(val11, meshBatch.m_GroupIndex, index3, meshBatch.m_InstanceIndex);
					}
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.InfoviewColor, in groupData, out var index4))
				{
					float4 val16 = default(float4);
					if ((preCullingData.m_Flags & PreCullingFlags.InfoviewColor) != 0)
					{
						if (m_LaneColorData.TryGetComponent(preCullingData.m_Entity, ref laneColor))
						{
							((float4)(ref val16))._002Ector((float)(int)laneColor.m_Index + 0.5f, (float)(int)laneColor.m_Value0 * 0.003921569f, (float)(int)laneColor.m_Index + 0.5f, (float)(int)laneColor.m_Value1 * 0.003921569f);
						}
						else if (m_OwnerData.TryGetComponent(preCullingData.m_Entity, ref owner))
						{
							if (m_EdgeLaneData.TryGetComponent(preCullingData.m_Entity, ref edgeLane2))
							{
								if (m_NetEdgeColorData.TryGetComponent(owner.m_Owner, ref edgeColor))
								{
									((float2)(ref val17))._002Ector((float)(int)edgeColor.m_Index + 0.5f, (float)(int)edgeColor.m_Value0 * 0.003921569f);
									((float2)(ref val18))._002Ector((float)(int)edgeColor.m_Index + 0.5f, (float)(int)edgeColor.m_Value1 * 0.003921569f);
									val16 = math.lerp(((float2)(ref val17)).xyxy, ((float2)(ref val18)).xyxy, ((float2)(ref edgeLane2.m_EdgeDelta)).xxyy);
								}
							}
							else if (m_NodeLaneData.HasComponent(preCullingData.m_Entity))
							{
								if (m_NetNodeColorData.TryGetComponent(owner.m_Owner, ref nodeColor))
								{
									((float4)(ref val16))._002Ector((float)(int)nodeColor.m_Index + 0.5f, (float)(int)nodeColor.m_Value * 0.003921569f, (float)(int)nodeColor.m_Index + 0.5f, (float)(int)nodeColor.m_Value * 0.003921569f);
								}
							}
							else
							{
								while (true)
								{
									if (m_ObjectColorData.TryGetComponent(owner.m_Owner, ref color))
									{
										if (color.m_SubColor)
										{
											((float4)(ref val16))._002Ector((float)(int)color.m_Index + 0.5f, (float)(int)color.m_Value * 0.003921569f, (float)(int)color.m_Index + 0.5f, (float)(int)color.m_Value * 0.003921569f);
										}
										break;
									}
									if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
									{
										break;
									}
									owner = owner2;
								}
							}
						}
					}
					m_NativeBatchInstances.SetPropertyValue<float4>(val16, meshBatch.m_GroupIndex, index4, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.ColorMask1))
				{
					Color color2;
					Color color3;
					Color color4;
					if (m_MeshColors.TryGetBuffer(preCullingData.m_Entity, ref val19))
					{
						MeshColor meshColor = val19[(int)meshBatch.m_MeshIndex];
						color2 = ((Color)(ref meshColor.m_ColorSet.m_Channel0)).linear;
						color3 = ((Color)(ref meshColor.m_ColorSet.m_Channel1)).linear;
						color4 = ((Color)(ref meshColor.m_ColorSet.m_Channel2)).linear;
					}
					else
					{
						color2 = Color.white;
						color3 = Color.white;
						color4 = Color.white;
					}
					bool smooth = (preCullingData.m_Flags & PreCullingFlags.SmoothColor) != 0;
					UpdateColorMask(in groupData, in meshBatch, updateMask, LaneProperty.ColorMask1, color2, smooth);
					UpdateColorMask(in groupData, in meshBatch, updateMask, LaneProperty.ColorMask2, color3, smooth);
					UpdateColorMask(in groupData, in meshBatch, updateMask, LaneProperty.ColorMask3, color4, smooth);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.FlowMatrix, in groupData, out var index5))
				{
					float4x4 val20 = default(float4x4);
					if ((preCullingData.m_Flags & PreCullingFlags.InfoviewColor) != 0 && m_SubFlows.TryGetBuffer(preCullingData.m_Entity, ref val21) && val21.Length == 16)
					{
						val20.c0 = new float4((float)val21[0].m_Value, (float)val21[4].m_Value, (float)val21[8].m_Value, (float)val21[12].m_Value) * (1f / 127f);
						val20.c1 = new float4((float)val21[1].m_Value, (float)val21[5].m_Value, (float)val21[9].m_Value, (float)val21[13].m_Value) * (1f / 127f);
						val20.c2 = new float4((float)val21[2].m_Value, (float)val21[6].m_Value, (float)val21[10].m_Value, (float)val21[14].m_Value) * (1f / 127f);
						val20.c3 = new float4((float)val21[3].m_Value, (float)val21[7].m_Value, (float)val21[11].m_Value, (float)val21[15].m_Value) * (1f / 127f);
					}
					m_NativeBatchInstances.SetPropertyValue<float4x4>(val20, meshBatch.m_GroupIndex, index5, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.FlowOffset, in groupData, out var index6))
				{
					float num6 = 0f;
					if ((preCullingData.m_Flags & PreCullingFlags.InfoviewColor) != 0 && m_OwnerData.TryGetComponent(preCullingData.m_Entity, ref owner3) && m_PseudoRandomSeedData.TryGetComponent(owner3.m_Owner, ref pseudoRandomSeed))
					{
						Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kFlowOffset);
						num6 = ((Random)(ref random)).NextFloat(1f);
					}
					m_NativeBatchInstances.SetPropertyValue<float>(num6, meshBatch.m_GroupIndex, index6, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.CurveDeterioration, in groupData, out var index7))
				{
					float4 val22 = float4.op_Implicit(0f);
					if ((preCullingData.m_Flags & PreCullingFlags.LaneCondition) != 0 && m_LaneConditionData.TryGetComponent(preCullingData.m_Entity, ref laneCondition))
					{
						float num7 = laneCondition.m_Wear / 10f;
						((float4)(ref val22))._002Ector(num7, num7, num7, 0f);
					}
					m_NativeBatchInstances.SetPropertyValue<float4>(val22, meshBatch.m_GroupIndex, index7, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.OutlineColors, in groupData, out var index8))
				{
					Color val23 = (m_ErrorData.HasComponent(preCullingData.m_Entity) ? m_RenderingSettingsData.m_ErrorColor : (m_WarningData.HasComponent(preCullingData.m_Entity) ? m_RenderingSettingsData.m_WarningColor : (((preCullingData.m_Flags & PreCullingFlags.Temp) == 0) ? m_RenderingSettingsData.m_HoveredColor : (((m_TempData[preCullingData.m_Entity].m_Flags & TempFlags.Parent) == 0) ? m_RenderingSettingsData.m_HoveredColor : m_RenderingSettingsData.m_OwnerColor))));
					val23 = ((Color)(ref val23)).linear;
					m_NativeBatchInstances.SetPropertyValue<Color>(val23, meshBatch.m_GroupIndex, index8, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(LaneProperty.HangingDistances, in groupData, out var index9))
				{
					float4 val24 = float4.op_Implicit(0f);
					if (m_PrefabRefData.TryGetComponent(preCullingData.m_Entity, ref prefabRef2) && m_CurveData.TryGetComponent(preCullingData.m_Entity, ref curve3) && m_PrefabUtilityLaneData.TryGetComponent(prefabRef2.m_Prefab, ref utilityLaneData))
					{
						m_HangingLaneData.TryGetComponent(preCullingData.m_Entity, ref hangingLane);
						((float4)(ref val24)).xw = ((float2)(ref hangingLane.m_Distances)).xy;
						((float4)(ref val24)).yz = (((float2)(ref hangingLane.m_Distances)).xy + utilityLaneData.m_Hanging * curve3.m_Length) * (2f / 3f);
						((float4)(ref val25))._002Ector(math.lengthsq(Wind.SampleWind(m_WindData, curve3.m_Bezier.a)), math.lengthsq(Wind.SampleWind(m_WindData, curve3.m_Bezier.b)), math.lengthsq(Wind.SampleWind(m_WindData, curve3.m_Bezier.c)), math.lengthsq(Wind.SampleWind(m_WindData, curve3.m_Bezier.d)));
						val24 *= math.sqrt(val25);
					}
					m_NativeBatchInstances.SetPropertyValue<float4>(val24, meshBatch.m_GroupIndex, index9, meshBatch.m_InstanceIndex);
				}
			}
		}

		private unsafe void UpdateZoneData(PreCullingData preCullingData, UpdateMask updateMask)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
			if (updateMask.ShouldUpdateNothing() || !m_MeshBatches.TryGetBuffer(preCullingData.m_Entity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				GroupData groupData = m_NativeBatchGroups.GetGroupData(meshBatch.m_GroupIndex);
				if (updateMask.ShouldUpdateTransform())
				{
					CullingInfo cullingInfo = m_CullingInfoData[preCullingData.m_Entity];
					Block block = m_ZoneBlockData[preCullingData.m_Entity];
					float3 position = block.m_Position;
					float2 val2 = float2.op_Implicit(block.m_Size - new int2(10, 6)) * 4f;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + block.m_Direction * val2.y;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + MathUtils.Right(block.m_Direction) * val2.x;
					float3x4 val3 = TransformHelper.TRS(position, ZoneUtils.GetRotation(block), new float3(1f, 1f, 1f));
					ref CullingData reference = ref m_NativeBatchInstances.SetTransformValue(val3, val3, meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
					reference.m_Bounds = cullingInfo.m_Bounds;
					reference.isHidden = m_HiddenData.HasComponent(preCullingData.m_Entity);
				}
				if (!updateMask.ShouldUpdateProperty(ZoneProperty.CellType0))
				{
					continue;
				}
				Block block2 = m_ZoneBlockData[preCullingData.m_Entity];
				DynamicBuffer<Cell> val4 = m_ZoneCells[preCullingData.m_Entity];
				CellTypes cellTypes = default(CellTypes);
				void* ptr = &cellTypes;
				for (int j = 0; j < block2.m_Size.y; j++)
				{
					for (int k = 0; k < block2.m_Size.x; k++)
					{
						Cell cell = val4[j * block2.m_Size.x + k];
						int colorIndex = ZoneUtils.GetColorIndex(cell.m_State, cell.m_Zone);
						int num = j + k * 6;
						UnsafeUtility.WriteArrayElement<float>(ptr, num, (float)colorIndex);
					}
				}
				if (updateMask.ShouldUpdateProperty(ZoneProperty.CellType0, in groupData, out var index))
				{
					m_NativeBatchInstances.SetPropertyValue<float4x4>(cellTypes.m_CellTypes0, meshBatch.m_GroupIndex, index, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ZoneProperty.CellType1, in groupData, out var index2))
				{
					m_NativeBatchInstances.SetPropertyValue<float4x4>(cellTypes.m_CellTypes1, meshBatch.m_GroupIndex, index2, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ZoneProperty.CellType2, in groupData, out var index3))
				{
					m_NativeBatchInstances.SetPropertyValue<float4x4>(cellTypes.m_CellTypes2, meshBatch.m_GroupIndex, index3, meshBatch.m_InstanceIndex);
				}
				if (updateMask.ShouldUpdateProperty(ZoneProperty.CellType3, in groupData, out var index4))
				{
					m_NativeBatchInstances.SetPropertyValue<float4x4>(cellTypes.m_CellTypes3, meshBatch.m_GroupIndex, index4, meshBatch.m_InstanceIndex);
				}
			}
		}

		private void UpdateFadeData(PreCullingData preCullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<MeshBatch> val = m_MeshBatches[preCullingData.m_Entity];
			DynamicBuffer<FadeBatch> val2 = m_FadeBatches[preCullingData.m_Entity];
			float3x4 val3 = default(float3x4);
			float3x4 val4 = default(float3x4);
			for (int i = 0; i < val.Length; i++)
			{
				MeshBatch meshBatch = val[i];
				FadeBatch fadeBatch = val2[i];
				if (!((float3)(ref fadeBatch.m_Velocity)).Equals(default(float3)) && m_NativeBatchInstances.GetTransformValue(meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex, ref val3, ref val4))
				{
					float3 val5 = fadeBatch.m_Velocity * (m_FrameDelta * (1f / 60f));
					ref float3 c = ref val3.c3;
					c += val5;
					ref float3 c2 = ref val4.c3;
					c2 += val5;
					ref Bounds3 bounds = ref m_NativeBatchInstances.SetTransformValue(val3, val4, meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex).m_Bounds;
					bounds += val5;
				}
			}
		}
	}

	[BurstCompile]
	private struct BatchLodJob : IJobParallelFor
	{
		private struct LodData
		{
			public int2 m_MinLod;

			public int m_MaxPriority;

			public int m_SelectLod;
		}

		[ReadOnly]
		public bool m_DisableLods;

		[ReadOnly]
		public bool m_UseLodFade;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float m_PixelSizeFactor;

		[ReadOnly]
		public int m_LodFadeDelta;

		[ReadOnly]
		public NativeList<MeshLoadingState> m_LoadingState;

		[ReadOnly]
		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public ParallelCullingWriter<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		[NativeDisableParallelForRestriction]
		public NativeList<int> m_BatchPriority;

		[NativeDisableParallelForRestriction]
		public NativeList<float> m_VTRequestsMaxPixels0;

		[NativeDisableParallelForRestriction]
		public NativeList<float> m_VTRequestsMaxPixels1;

		public void Execute(int index)
		{
			int groupIndex = m_NativeBatchInstances.GetGroupIndex(index);
			GroupData groupData = m_NativeBatchGroups.GetGroupData(groupIndex);
			if ((groupData.m_LodCount == 0) | m_DisableLods)
			{
				SingleLod(index, groupIndex, in groupData);
			}
			else
			{
				MultiLod(index, groupIndex, in groupData);
			}
		}

		private bool GetLodPropertyIndex(in GroupData groupData, int dataIndex, out int index)
		{
			switch (groupData.m_MeshType)
			{
			case MeshType.Object:
				return groupData.GetPropertyIndex(9 + dataIndex, out index);
			case MeshType.Lane:
				return groupData.GetPropertyIndex(6 + dataIndex, out index);
			case MeshType.Net:
				return groupData.GetPropertyIndex(14 + dataIndex, out index);
			default:
				index = -1;
				return false;
			}
		}

		private unsafe void SingleLod(int activeGroup, int groupIndex, in GroupData groupData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			NativeBatchAccessor<BatchData> batchAccessor = m_NativeBatchGroups.GetBatchAccessor(groupIndex);
			WriteableCullingAccessor<CullingData> cullingAccessor = m_NativeBatchInstances.GetCullingAccessor(activeGroup);
			MergeIndexAccessor mergeIndexAccessor = m_NativeBatchInstances.GetMergeIndexAccessor(groupIndex);
			WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData> val = default(WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData>);
			WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData> val2 = default(WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData>);
			bool flag = false;
			if (m_UseLodFade)
			{
				if (GetLodPropertyIndex(in groupData, 0, out var index))
				{
					val = m_NativeBatchInstances.GetPropertyAccessor(activeGroup, index);
					flag = true;
				}
				if (GetLodPropertyIndex(in groupData, 1, out var index2))
				{
					val2 = m_NativeBatchInstances.GetPropertyAccessor(activeGroup, index2);
					flag = true;
				}
			}
			int length = batchAccessor.Length;
			Bounds3 val3 = default(Bounds3);
			((Bounds3)(ref val3))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			Bounds3 val4 = default(Bounds3);
			((Bounds3)(ref val4))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			bool flag2 = false;
			BatchData batchData = batchAccessor.GetBatchData(0);
			int2 val5 = default(int2);
			((int2)(ref val5))._002Ector((int)batchData.m_MinLod, (int)batchData.m_ShadowLod);
			int num = -1000000;
			int num2 = 0;
			int num3 = 0;
			int num4 = val5.x;
			bool flag3 = (batchData.m_VTIndex0 >= 0) | (batchData.m_VTIndex1 >= 0);
			for (int i = 1; i < length; i++)
			{
				batchData = batchAccessor.GetBatchData(i);
				flag3 |= (batchData.m_VTIndex0 >= 0) | (batchData.m_VTIndex1 >= 0);
				if (batchData.m_MinLod != num4)
				{
					num4 = batchData.m_MinLod;
					num3 = i;
					num2++;
				}
			}
			int num5 = 1;
			if (flag3)
			{
				num5 += m_NativeBatchGroups.GetMergedGroupCount(groupIndex);
			}
			float* ptr = stackalloc float[num5];
			for (int j = 0; j < num5; j++)
			{
				ptr[j] = float.MaxValue;
			}
			int managedBatchIndex = batchAccessor.GetManagedBatchIndex(num3);
			if (managedBatchIndex >= 0)
			{
				flag2 = m_LoadingState[managedBatchIndex] < MeshLoadingState.Complete;
			}
			int4 val8 = default(int4);
			for (int k = 0; k < cullingAccessor.Length; k++)
			{
				ref CullingData reference = ref cullingAccessor.Get(k);
				float num6 = RenderingUtils.CalculateMinDistance(reference.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num7 = RenderingUtils.CalculateLod(num6 * num6, m_LodParameters);
				int2 val6 = val5 + reference.lodOffset;
				bool flag4 = !reference.isHidden;
				bool2 val7 = (num7 >= val6) & !reference.isFading;
				((int4)(ref val8))._002Ector(num2, num2, math.select(new int2(0), new int2(255), val7));
				if (flag)
				{
					int4 lodFade = reference.lodFade;
					if (math.any(lodFade != val8))
					{
						((int4)(ref lodFade)).xy = int2.op_Implicit(num2);
						((int4)(ref lodFade)).zw = math.clamp(((int4)(ref lodFade)).zw + math.select(int2.op_Implicit(-m_LodFadeDelta), int2.op_Implicit(m_LodFadeDelta), val7), int2.op_Implicit(0), int2.op_Implicit(255));
						val7 = ((int4)(ref lodFade)).zw != 0;
						if (val7.x)
						{
							int2 val9 = math.select(((int4)(ref lodFade)).zw, ((int4)(ref lodFade)).zw - 255, reference.isFading);
							int4 val10 = new int4(val9, -val9);
							int4 val11 = new int4(-val9, val9);
							bool2 val12 = (((int)groupData.m_LodCount - ((int4)(ref lodFade)).xy) & 1) != 0;
							int4 val13 = math.select(val10, val11, ((bool2)(ref val12)).xyxy);
							num = math.max(num, 0);
							((int4)(ref val13)).xz = (1065353471 - ((int4)(ref val13)).xz) | (255 - ((int4)(ref val13)).yw << 11);
							if (val.Length != 0)
							{
								val.SetPropertyValue<int>(val13.x, k);
							}
							if (val2.Length != 0)
							{
								val2.SetPropertyValue<int>(val13.z, k);
							}
						}
						reference.lodFade = lodFade;
					}
				}
				else
				{
					reference.lodFade = val8;
				}
				int4 lodRange = int4.op_Implicit(0);
				val7 &= flag4;
				if (val7.x)
				{
					((int4)(ref lodRange)).xy = new int2(num2, num2 + 1);
					val3 |= reference.m_Bounds;
					if (val7.y)
					{
						((int4)(ref lodRange)).zw = new int2(num2, num2 + 1);
						val4 |= reference.m_Bounds;
					}
					if (flag3)
					{
						int num8 = ((num5 != 1) ? (1 + ((MergeIndexAccessor)(ref mergeIndexAccessor)).Get(k)) : 0);
						ptr[num8] = math.min(ptr[num8], num6);
					}
				}
				reference.lodRange = lodRange;
				num = math.max(num, num7 - val6.x);
			}
			for (int l = num3; l < length; l++)
			{
				if (flag3)
				{
					batchData = batchAccessor.GetBatchData(l);
					if ((batchData.m_VTIndex0 >= 0) | (batchData.m_VTIndex1 >= 0))
					{
						if (*ptr != float.MaxValue)
						{
							AddVTRequests(in batchData, *ptr);
						}
						for (int m = 1; m < num5; m++)
						{
							if (ptr[m] != float.MaxValue)
							{
								int mergedGroupIndex = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, m - 1);
								AddVTRequests(m_NativeBatchGroups.GetBatchData(mergedGroupIndex, l), ptr[m]);
							}
						}
					}
				}
				managedBatchIndex = batchAccessor.GetManagedBatchIndex(l);
				if (managedBatchIndex >= 0)
				{
					ref int reference2 = ref m_BatchPriority.ElementAt(managedBatchIndex);
					reference2 = math.max(reference2, num);
				}
			}
			float3 val14;
			float3 val15;
			if (val3.min.x != float.MaxValue)
			{
				val14 = MathUtils.Center(val3);
				val15 = MathUtils.Extents(val3);
			}
			else
			{
				val14 = default(float3);
				val15 = float3.op_Implicit(float.MinValue);
			}
			float3 val16;
			float3 val17;
			if (val4.min.x != float.MaxValue)
			{
				val16 = MathUtils.Center(val4);
				val17 = MathUtils.Extents(val4);
			}
			else
			{
				val16 = default(float3);
				val17 = float3.op_Implicit(float.MinValue);
			}
			m_NativeBatchInstances.UpdateCulling(activeGroup, val14, val15, val16, val17, flag2);
		}

		private unsafe void MultiLod(int activeGroup, int groupIndex, in GroupData groupData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0961: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_0988: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0917: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
			NativeBatchAccessor<BatchData> batchAccessor = m_NativeBatchGroups.GetBatchAccessor(groupIndex);
			WriteableCullingAccessor<CullingData> cullingAccessor = m_NativeBatchInstances.GetCullingAccessor(activeGroup);
			MergeIndexAccessor mergeIndexAccessor = m_NativeBatchInstances.GetMergeIndexAccessor(groupIndex);
			WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData> val = default(WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData>);
			WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData> val2 = default(WriteablePropertyAccessor<CullingData, GroupData, BatchData, InstanceData>);
			bool flag = false;
			if (m_UseLodFade)
			{
				if (GetLodPropertyIndex(in groupData, 0, out var index))
				{
					val = m_NativeBatchInstances.GetPropertyAccessor(activeGroup, index);
					flag = true;
				}
				if (GetLodPropertyIndex(in groupData, 1, out var index2))
				{
					val2 = m_NativeBatchInstances.GetPropertyAccessor(activeGroup, index2);
					flag = true;
				}
			}
			int length = batchAccessor.Length;
			int num = 0;
			Bounds3 val3 = default(Bounds3);
			((Bounds3)(ref val3))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			Bounds3 val4 = default(Bounds3);
			((Bounds3)(ref val4))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			bool flag2 = false;
			bool flag3 = false;
			LodData* ptr = stackalloc LodData[groupData.m_LodCount + 1];
			ref LodData reference = ref *ptr;
			reference.m_MinLod = int2.op_Implicit(-1);
			int managedBatchIndex;
			for (int i = 0; i < length; i++)
			{
				BatchData batchData = batchAccessor.GetBatchData(i);
				flag3 |= (batchData.m_VTIndex0 >= 0) | (batchData.m_VTIndex1 >= 0);
				if (batchData.m_MinLod != reference.m_MinLod.x)
				{
					reference = ref ptr[num++];
					reference.m_MinLod = new int2((int)batchData.m_MinLod, (int)batchData.m_ShadowLod);
					reference.m_MaxPriority = -1000000;
					reference.m_SelectLod = num - 1;
				}
				if (num != 1)
				{
					managedBatchIndex = batchAccessor.GetManagedBatchIndex(i);
					int num2 = ptr[num - 2].m_SelectLod;
					reference.m_SelectLod = math.select(num - 1, num2, reference.m_SelectLod == num2 || (managedBatchIndex >= 0 && m_LoadingState[managedBatchIndex] < MeshLoadingState.Complete));
				}
			}
			int num3 = 1;
			int num4 = 1;
			if (flag3)
			{
				num3 += m_NativeBatchGroups.GetMergedGroupCount(groupIndex);
				num4 = num3 * num;
			}
			float* ptr2 = stackalloc float[num4];
			for (int j = 0; j < num4; j++)
			{
				ptr2[j] = float.MaxValue;
			}
			managedBatchIndex = batchAccessor.GetManagedBatchIndex(0);
			if (managedBatchIndex >= 0)
			{
				flag2 = m_LoadingState[managedBatchIndex] < MeshLoadingState.Complete;
			}
			int4 val8 = default(int4);
			for (int k = 0; k < cullingAccessor.Length; k++)
			{
				ref CullingData reference2 = ref cullingAccessor.Get(k);
				reference = ref *ptr;
				float num5 = RenderingUtils.CalculateMinDistance(reference2.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num6 = RenderingUtils.CalculateLod(num5 * num5, m_LodParameters);
				bool flag4 = !reference2.isHidden;
				bool2 val5 = num6 >= reference.m_MinLod;
				int2 val6 = math.select(int2.op_Implicit(-1), int2.op_Implicit(reference.m_SelectLod), val5);
				int2 val7 = int2.op_Implicit(1);
				reference.m_MaxPriority = math.max(reference.m_MaxPriority, num6 - reference.m_MinLod.x);
				val5 &= !reference2.isFading;
				for (int l = 1; l < num; l++)
				{
					reference = ref ptr[l];
					val6 = math.select(val6, int2.op_Implicit(reference.m_SelectLod), num6 >= reference.m_MinLod);
					reference.m_MaxPriority = math.max(reference.m_MaxPriority, num6 - reference.m_MinLod.x);
				}
				if (flag)
				{
					((int4)(ref val8))._002Ector(val6, math.select(new int2(0), new int2(255), val5));
					int4 val9 = reference2.lodFade;
					if (math.any(val9 != val8))
					{
						bool2 val12;
						if (reference2.isFading)
						{
							((int4)(ref val9)).zw = ((int4)(ref val9)).zw - m_LodFadeDelta;
							((int4)(ref val8)).xy = math.select(int2.op_Implicit(0), val6, val6 >= 0);
							int4 val10 = val9;
							int4 val11 = val8;
							val12 = (((int4)(ref val9)).xy == 255) | (((int4)(ref val9)).zw <= 0);
							val9 = math.select(val10, val11, ((bool2)(ref val12)).xyxy);
							val5 = ((int4)(ref val9)).zw != 0;
							if (val5.x)
							{
								reference = ref ptr[val9.x];
								reference.m_MaxPriority = math.max(reference.m_MaxPriority, 0);
								val6.x = reference.m_SelectLod;
								val7.x = 1;
								if (val5.y)
								{
									val6.y = ptr[val9.y].m_SelectLod;
									val7.y = 1;
								}
								int2 val13 = ((int4)(ref val9)).zw - 255;
								int4 val14 = new int4(val13, -val13);
								int4 val15 = new int4(-val13, val13);
								val12 = (((int)groupData.m_LodCount - ((int4)(ref val9)).xy) & 1) != 0;
								int4 val16 = math.select(val14, val15, ((bool2)(ref val12)).xyxy);
								((int4)(ref val16)).xz = (1065353471 - ((int4)(ref val16)).xz) | (255 - ((int4)(ref val16)).yw << 11);
								if (val.Length != 0)
								{
									val.SetPropertyValue<int>(val16.x, k);
								}
								if (val2.Length != 0)
								{
									val2.SetPropertyValue<int>(val16.z, k);
								}
							}
						}
						else
						{
							if (val6.x >= val9.x)
							{
								val9.z += m_LodFadeDelta;
								((int4)(ref val9)).xz = ((int4)(ref val9)).xz + math.select(int2.op_Implicit(0), new int2(1, -255), val9.z >= 256);
								((int4)(ref val9)).xz = math.select(((int4)(ref val9)).xz, new int2(val6.x, 255), val9.x > val6.x);
							}
							else
							{
								val9.z -= m_LodFadeDelta;
								((int4)(ref val9)).xz = ((int4)(ref val9)).xz + math.select(int2.op_Implicit(0), new int2(-1, 255), val9.z <= 0);
								((int4)(ref val8)).xz = math.select(int2.op_Implicit(0), new int2(val6.x, 255), val5.x);
								((int4)(ref val9)).xz = math.select(((int4)(ref val9)).xz, ((int4)(ref val8)).xz, (val9.x == val6.x) | (val9.x >= 254));
							}
							if (val6.y >= val9.y)
							{
								val9.w += m_LodFadeDelta;
								((int4)(ref val9)).yw = ((int4)(ref val9)).yw + math.select(int2.op_Implicit(0), new int2(1, -255), val9.w >= 256);
								((int4)(ref val9)).yw = math.select(((int4)(ref val9)).yw, new int2(val6.y, 255), val9.y > val6.y);
							}
							else
							{
								val9.w -= m_LodFadeDelta;
								((int4)(ref val9)).yw = ((int4)(ref val9)).yw + math.select(int2.op_Implicit(0), new int2(-1, 255), val9.w <= 0);
								((int4)(ref val8)).yw = math.select(int2.op_Implicit(0), new int2(val6.y, 255), val5.y);
								((int4)(ref val9)).yw = math.select(((int4)(ref val9)).yw, ((int4)(ref val8)).yw, (val9.y == val6.y) | (val9.y >= 254));
							}
							val5 = ((int4)(ref val9)).zw != 0;
							if (val5.x)
							{
								int num7 = val9.x - math.select(0, 1, (val9.z != 255) & (val9.x != 0));
								reference = ref ptr[num7];
								reference.m_MaxPriority = math.max(reference.m_MaxPriority, 0);
								val6.x = reference.m_SelectLod;
								reference = ref ptr[val9.x];
								reference.m_MaxPriority = math.max(reference.m_MaxPriority, 0);
								val7.x = reference.m_SelectLod - val6.x + 1;
								if (val5.y)
								{
									num7 = val9.y - math.select(0, 1, (val9.w != 255) & (val9.y != 0));
									val6.y = ptr[num7].m_SelectLod;
									val7.y = ptr[val9.y].m_SelectLod - val6.y + 1;
								}
								int2 zw = ((int4)(ref val9)).zw;
								int4 val17 = new int4(zw, -zw);
								int4 val18 = new int4(-zw, zw);
								val12 = (((int)groupData.m_LodCount - ((int4)(ref val9)).xy) & 1) != 0;
								int4 val19 = math.select(val17, val18, ((bool2)(ref val12)).xyxy);
								((int4)(ref val19)).xz = (1065353471 - ((int4)(ref val19)).xz) | (255 - ((int4)(ref val19)).yw << 11);
								if (val.Length != 0)
								{
									val.SetPropertyValue<int>(val19.x, k);
								}
								if (val2.Length != 0)
								{
									val2.SetPropertyValue<int>(val19.z, k);
								}
							}
						}
						reference2.lodFade = val9;
					}
				}
				else
				{
					reference2.lodFade = math.select(int4.op_Implicit(0), new int4(val6, 255, 255), ((bool2)(ref val5)).xyxy);
				}
				int4 lodRange = int4.op_Implicit(0);
				val5 &= flag4;
				if (val5.x)
				{
					((int4)(ref lodRange)).xy = new int2(val6.x, val6.x + val7.x);
					val3 |= reference2.m_Bounds;
					if (val5.y)
					{
						((int4)(ref lodRange)).zw = new int2(val6.y, val6.y + val7.y);
						val4 |= reference2.m_Bounds;
					}
					if (flag3)
					{
						int num8 = ((num3 != 1) ? (1 + ((MergeIndexAccessor)(ref mergeIndexAccessor)).Get(k)) : 0);
						int num9 = val6.x * num3 + num8;
						ptr2[num9] = math.min(ptr2[num9], num5);
					}
				}
				reference2.lodRange = lodRange;
			}
			num = 1;
			reference = ref *ptr;
			int num10 = 0;
			for (int m = 0; m < length; m++)
			{
				BatchData batchData2 = batchAccessor.GetBatchData(m);
				if (batchData2.m_MinLod != reference.m_MinLod.x)
				{
					reference = ref ptr[num++];
					num10 += num3;
				}
				if ((batchData2.m_VTIndex0 >= 0) | (batchData2.m_VTIndex1 >= 0))
				{
					if (ptr2[num10] != float.MaxValue)
					{
						AddVTRequests(in batchData2, ptr2[num10]);
					}
					for (int n = 1; n < num3; n++)
					{
						if (ptr2[num10 + n] != float.MaxValue)
						{
							int mergedGroupIndex = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, n - 1);
							AddVTRequests(m_NativeBatchGroups.GetBatchData(mergedGroupIndex, m), ptr2[num10 + n]);
						}
					}
				}
				managedBatchIndex = batchAccessor.GetManagedBatchIndex(m);
				if (managedBatchIndex >= 0)
				{
					ref int reference3 = ref m_BatchPriority.ElementAt(managedBatchIndex);
					reference3 = math.max(reference3, reference.m_MaxPriority);
				}
			}
			float3 val20;
			float3 val21;
			if (val3.min.x != float.MaxValue)
			{
				val20 = MathUtils.Center(val3);
				val21 = MathUtils.Extents(val3);
			}
			else
			{
				val20 = default(float3);
				val21 = float3.op_Implicit(float.MinValue);
			}
			float3 val22;
			float3 val23;
			if (val4.min.x != float.MaxValue)
			{
				val22 = MathUtils.Center(val4);
				val23 = MathUtils.Extents(val4);
			}
			else
			{
				val22 = default(float3);
				val23 = float3.op_Implicit(float.MinValue);
			}
			m_NativeBatchInstances.UpdateCulling(activeGroup, val20, val21, val22, val23, flag2);
		}

		private void AddVTRequests(in BatchData batchData, float minDistance)
		{
			float num = math.atan(batchData.m_VTSizeFactor / minDistance) * m_PixelSizeFactor;
			if (batchData.m_VTIndex0 >= 0)
			{
				ref float reference = ref m_VTRequestsMaxPixels0.ElementAt(batchData.m_VTIndex0);
				if (num > reference)
				{
					float num2 = num;
					float num3 = num;
					do
					{
						num3 = num2;
						num2 = Interlocked.Exchange(ref reference, num3);
					}
					while (num2 > num3);
				}
			}
			if (batchData.m_VTIndex1 < 0)
			{
				return;
			}
			ref float reference2 = ref m_VTRequestsMaxPixels1.ElementAt(batchData.m_VTIndex1);
			if (num > reference2)
			{
				float num4 = num;
				float num5 = num;
				do
				{
					num5 = num4;
					num4 = Interlocked.Exchange(ref reference2, num5);
				}
				while (num4 > num5);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Error> __Game_Tools_Error_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Warning> __Game_Tools_Warning_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Override> __Game_Tools_Override_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<FadeBatch> __Game_Rendering_FadeBatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Animated> __Game_Rendering_Animated_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Emissive> __Game_Rendering_Emissive_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Color> __Game_Objects_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Surface> __Game_Objects_Surface_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CitizenPresence> __Game_Buildings_CitizenPresence_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeLane> __Game_Net_NodeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneColor> __Game_Net_LaneColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HangingLane> __Game_Net_HangingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeColor> __Game_Net_EdgeColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeColor> __Game_Net_NodeColor_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubFlow> __Game_Net_SubFlow_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CutRange> __Game_Net_CutRange_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GrowthScaleData> __Game_Prefabs_GrowthScaleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

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
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_Error_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Error>(true);
			__Game_Tools_Warning_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Warning>(true);
			__Game_Tools_Override_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Override>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Rendering_MeshBatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(true);
			__Game_Rendering_FadeBatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FadeBatch>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Rendering_Animated_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Animated>(true);
			__Game_Rendering_Skeleton_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(true);
			__Game_Rendering_Emissive_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Color>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Surface_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Surface>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_CitizenPresence_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CitizenPresence>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Vehicles_Passenger_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_NodeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeLane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Net_LaneColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneColor>(true);
			__Game_Net_LaneCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(true);
			__Game_Net_HangingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangingLane>(true);
			__Game_Net_EdgeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeColor>(true);
			__Game_Net_NodeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeColor>(true);
			__Game_Net_SubFlow_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubFlow>(true);
			__Game_Net_CutRange_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CutRange>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_GrowthScaleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GrowthScaleData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
		}
	}

	public const float LOD_FADE_DURATION = 0.25f;

	public const float DEBUG_FADE_DURATION = 2.5f;

	private RenderingSystem m_RenderingSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private PreCullingSystem m_PreCullingSystem;

	private LightingSystem m_LightingSystem;

	private MeshColorSystem m_MeshColorSystem;

	private SimulationSystem m_SimulationSystem;

	private CitizenPresenceSystem m_CitizenPresenceSystem;

	private TreeGrowthSystem m_TreeGrowthSystem;

	private WetnessSystem m_WetnessSystem;

	private WindSystem m_WindSystem;

	private DirtynessSystem m_DirtynessSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_RenderingSettingsQuery;

	private NativeAccumulator<SmoothingNeeded> m_SmoothingNeeded;

	private int m_SHCoefficients;

	private int m_LodParameters;

	private bool m_UpdateAll;

	private float m_LastLightFactor;

	private float m_LodFadeTimer;

	private float4 m_LastBuildingStateOverride;

	private uint m_LastCitizenPresenceVersion;

	private uint m_LastTreeGrowthVersion;

	private uint m_LastWetnessVersion;

	private uint m_LastDirtynessVersion;

	private uint m_LastFireDamageVersion;

	private uint m_LastWaterDamageVersion;

	private uint m_LastWeatherDamageVersion;

	private uint m_LastLaneConditionFrame;

	private uint m_LastDamagedFrame;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_MeshColorSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MeshColorSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitizenPresenceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitizenPresenceSystem>();
		m_TreeGrowthSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TreeGrowthSystem>();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_WetnessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WetnessSystem>();
		m_DirtynessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DirtynessSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_RenderingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RenderingSettingsData>() });
		m_SmoothingNeeded = new NativeAccumulator<SmoothingNeeded>(AllocatorHandle.op_Implicit((Allocator)4));
		m_SHCoefficients = Shader.PropertyToID("unity_SHCoefficients");
		m_LodParameters = Shader.PropertyToID("colossal_LodParameters");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_SmoothingNeeded.Dispose();
		base.OnDestroy();
	}

	public void InstancePropertiesUpdated()
	{
		m_UpdateAll = true;
	}

	public void PostDeserialize(Context context)
	{
		m_LastLaneConditionFrame = m_SimulationSystem.frameIndex;
		m_LastDamagedFrame = m_SimulationSystem.frameIndex;
	}

	public float GetLevelOfDetail(float levelOfDetail, IGameCameraController cameraController)
	{
		if (cameraController != null)
		{
			levelOfDetail *= 1f - 1f / (2f + 0.01f * cameraController.zoom);
		}
		return levelOfDetail;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
		float4 val = float4.op_Implicit(1f);
		float3 cameraPosition = float3.op_Implicit(0f);
		float3 cameraDirection = float3.op_Implicit(0f);
		float pixelSizeFactor = 1f;
		if (m_CameraUpdateSystem.TryGetLODParameters(out var lodParameters))
		{
			IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
			val = RenderingUtils.CalculateLodParameters(GetLevelOfDetail(m_RenderingSystem.frameLod, activeCameraController), lodParameters);
			cameraPosition = float3.op_Implicit(((LODParameters)(ref lodParameters)).cameraPosition);
			cameraDirection = m_CameraUpdateSystem.activeViewer.forward;
			pixelSizeFactor = (float)((LODParameters)(ref lodParameters)).cameraPixelHeight / math.radians(((LODParameters)(ref lodParameters)).fieldOfView);
		}
		Shader.SetGlobalVector(m_LodParameters, new Vector4(val.x, val.y, 0f, 0f));
		bool flag = m_BatchManagerSystem.IsLodFadeEnabled();
		int lodFadeDelta = 0;
		if (flag)
		{
			m_LodFadeTimer += Time.deltaTime * (m_RenderingSystem.debugCrossFade ? 102f : 1020f);
			lodFadeDelta = Mathf.FloorToInt(m_LodFadeTimer);
			m_LodFadeTimer -= lodFadeDelta;
			lodFadeDelta = math.clamp(lodFadeDelta, 0, 255);
		}
		m_BatchMeshSystem.UpdateMeshes();
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies2);
		GetDataQuery(out var cullingFlags, out var updateMasks);
		((JobHandle)(ref dependencies2)).Complete();
		UpdateGlobalValues(nativeBatchInstances);
		int activeGroupCount = nativeBatchInstances.GetActiveGroupCount();
		CullingWriter<CullingData, GroupData, BatchData, InstanceData> val2 = nativeBatchInstances.BeginCulling((Allocator)3);
		JobHandle dependencies3;
		JobHandle dependencies4;
		BatchDataJob batchDataJob = new BatchDataJob
		{
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorData = InternalCompilerInterface.GetComponentLookup<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WarningData = InternalCompilerInterface.GetComponentLookup<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverrideData = InternalCompilerInterface.GetComponentLookup<Override>(ref __TypeHandle.__Game_Tools_Override_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FadeBatches = InternalCompilerInterface.GetBufferLookup<FadeBatch>(ref __TypeHandle.__Game_Rendering_FadeBatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectTransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectColorData = InternalCompilerInterface.GetComponentLookup<Color>(ref __TypeHandle.__Game_Objects_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectSurfaceData = InternalCompilerInterface.GetComponentLookup<Surface>(ref __TypeHandle.__Game_Objects_Surface_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectDamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenPresenceData = InternalCompilerInterface.GetComponentLookup<CitizenPresence>(ref __TypeHandle.__Game_Buildings_CitizenPresence_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingAbandonedData = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeLaneData = InternalCompilerInterface.GetComponentLookup<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneColorData = InternalCompilerInterface.GetComponentLookup<LaneColor>(ref __TypeHandle.__Game_Net_LaneColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HangingLaneData = InternalCompilerInterface.GetComponentLookup<HangingLane>(ref __TypeHandle.__Game_Net_HangingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetEdgeColorData = InternalCompilerInterface.GetComponentLookup<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetNodeColorData = InternalCompilerInterface.GetComponentLookup<NodeColor>(ref __TypeHandle.__Game_Net_NodeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubFlows = InternalCompilerInterface.GetBufferLookup<SubFlow>(ref __TypeHandle.__Game_Net_SubFlow_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CutRanges = InternalCompilerInterface.GetBufferLookup<CutRange>(ref __TypeHandle.__Game_Net_CutRange_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneCells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGrowthScaleData = InternalCompilerInterface.GetComponentLookup<GrowthScaleData>(ref __TypeHandle.__Game_Prefabs_GrowthScaleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_LightFactor = m_LastLightFactor,
			m_FrameDelta = m_RenderingSystem.frameDelta,
			m_SmoothnessDelta = m_RenderingSystem.frameDelta * 0.0016666667f,
			m_BuildingStateOverride = m_LastBuildingStateOverride,
			m_CullingFlags = cullingFlags,
			m_UpdateMasks = updateMasks,
			m_NativeBatchGroups = nativeBatchGroups,
			m_CullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies3),
			m_WindData = m_WindSystem.GetData(readOnly: true, out dependencies4),
			m_NativeBatchInstances = nativeBatchInstances.AsParallelInstanceWriter(),
			m_SmoothingNeeded = m_SmoothingNeeded.AsParallelWriter()
		};
		JobHandle dependencies5;
		JobHandle dependencies6;
		BatchLodJob batchLodJob = new BatchLodJob
		{
			m_DisableLods = m_RenderingSystem.disableLodModels,
			m_UseLodFade = flag,
			m_LodParameters = val,
			m_CameraPosition = cameraPosition,
			m_CameraDirection = cameraDirection,
			m_PixelSizeFactor = pixelSizeFactor,
			m_LodFadeDelta = lodFadeDelta,
			m_LoadingState = m_BatchMeshSystem.GetLoadingState(out dependencies5),
			m_NativeBatchGroups = nativeBatchGroups,
			m_NativeBatchInstances = val2.AsParallel(),
			m_BatchPriority = m_BatchMeshSystem.GetBatchPriority(out dependencies6)
		};
		if (!((EntityQuery)(ref m_RenderingSettingsQuery)).IsEmptyIgnoreFilter)
		{
			batchDataJob.m_RenderingSettingsData = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<RenderingSettingsData>();
		}
		JobHandle vTRequestMaxPixels = m_ManagedBatchSystem.GetVTRequestMaxPixels(out batchLodJob.m_VTRequestsMaxPixels0, out batchLodJob.m_VTRequestsMaxPixels1);
		JobHandle val3 = IJobParallelForDeferExtensions.Schedule<BatchDataJob, PreCullingData>(batchDataJob, batchDataJob.m_CullingData, 16, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies3, dependencies4, dependencies));
		JobHandle val4 = IJobParallelForExtensions.Schedule<BatchLodJob>(batchLodJob, activeGroupCount, 1, JobUtils.CombineDependencies(val3, vTRequestMaxPixels, dependencies6, dependencies5));
		JobHandle jobHandle = nativeBatchInstances.EndCulling(val2, val4);
		m_BatchManagerSystem.AddNativeBatchGroupsReader(val4);
		m_BatchManagerSystem.AddNativeBatchInstancesWriter(jobHandle);
		m_BatchMeshSystem.AddBatchPriorityWriter(val4);
		m_BatchMeshSystem.AddLoadingStateReader(val4);
		m_ManagedBatchSystem.AddVTRequestWriter(val4);
		m_PreCullingSystem.AddCullingDataReader(val3);
		m_WindSystem.AddReader(val3);
		m_BatchMeshSystem.UpdateBatchPriorities();
		((SystemBase)this).Dependency = val3;
	}

	private void GetDataQuery(out PreCullingFlags cullingFlags, out UpdateMasks updateMasks)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		cullingFlags = PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated | PreCullingFlags.FadeContainer | PreCullingFlags.InterpolatedTransform | PreCullingFlags.Animated | PreCullingFlags.ColorsUpdated;
		updateMasks = default(UpdateMasks);
		float4 editorBuildingStateOverride = m_RenderingSystem.editorBuildingStateOverride;
		if (!((float4)(ref editorBuildingStateOverride)).Equals(m_LastBuildingStateOverride))
		{
			m_LastBuildingStateOverride = m_RenderingSystem.editorBuildingStateOverride;
			m_LastCitizenPresenceVersion--;
		}
		uint lastSystemVersion = ((ComponentSystemBase)m_CitizenPresenceSystem).LastSystemVersion;
		uint lastSystemVersion2 = ((ComponentSystemBase)m_TreeGrowthSystem).LastSystemVersion;
		uint lastSystemVersion3 = ((ComponentSystemBase)m_WetnessSystem).LastSystemVersion;
		uint lastSystemVersion4 = ((ComponentSystemBase)m_DirtynessSystem).LastSystemVersion;
		uint num = ((m_RenderingSystem.frameIndex >= m_LastLaneConditionFrame + 128) ? m_RenderingSystem.frameIndex : m_LastLaneConditionFrame);
		uint num2 = ((m_RenderingSystem.frameIndex >= m_LastDamagedFrame + 128) ? m_RenderingSystem.frameIndex : m_LastDamagedFrame);
		float num3 = CalculateLightFactor();
		if (m_UpdateAll)
		{
			cullingFlags |= PreCullingFlags.NearCamera | PreCullingFlags.InfoviewColor | PreCullingFlags.BuildingState | PreCullingFlags.TreeGrowth | PreCullingFlags.LaneCondition | PreCullingFlags.SurfaceState | PreCullingFlags.SurfaceDamage | PreCullingFlags.SmoothColor;
			updateMasks.UpdateAll();
			m_UpdateAll = false;
		}
		else
		{
			SmoothingNeeded result = m_SmoothingNeeded.GetResult(0);
			if ((Object)(object)m_ToolSystem.activeInfoview != (Object)null)
			{
				cullingFlags |= PreCullingFlags.InfoviewColor;
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.InfoviewColor);
				updateMasks.m_NetMask.UpdateProperty(NetProperty.InfoviewColor);
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.InfoviewColor);
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.FlowMatrix);
			}
			if (lastSystemVersion != m_LastCitizenPresenceVersion || num3 != m_LastLightFactor)
			{
				cullingFlags |= PreCullingFlags.BuildingState;
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.BuildingState);
			}
			if (lastSystemVersion2 != m_LastTreeGrowthVersion)
			{
				cullingFlags |= PreCullingFlags.TreeGrowth;
			}
			if (lastSystemVersion3 != m_LastWetnessVersion || result.IsNeeded(SmoothingType.SurfaceWetness))
			{
				cullingFlags |= PreCullingFlags.SurfaceState;
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.SurfaceWetness);
			}
			if (lastSystemVersion4 != m_LastDirtynessVersion || result.IsNeeded(SmoothingType.SurfaceDirtyness))
			{
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.SurfaceDamage);
			}
			if (num != m_LastLaneConditionFrame)
			{
				cullingFlags |= PreCullingFlags.LaneCondition;
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.CurveDeterioration);
			}
			if (num2 != m_LastDamagedFrame || result.IsNeeded(SmoothingType.SurfaceDamage))
			{
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.SurfaceDamage);
			}
			if (m_MeshColorSystem.smoothColorsUpdated || result.IsNeeded(SmoothingType.ColorMask))
			{
				cullingFlags |= PreCullingFlags.SmoothColor;
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.ColorMask1);
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.ColorMask2);
				updateMasks.m_ObjectMask.UpdateProperty(ObjectProperty.ColorMask3);
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.ColorMask1);
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.ColorMask2);
				updateMasks.m_LaneMask.UpdateProperty(LaneProperty.ColorMask3);
			}
		}
		m_SmoothingNeeded.Clear();
		m_LastCitizenPresenceVersion = lastSystemVersion;
		m_LastTreeGrowthVersion = lastSystemVersion2;
		m_LastWetnessVersion = lastSystemVersion3;
		m_LastDirtynessVersion = lastSystemVersion4;
		m_LastLightFactor = num3;
		m_LastLaneConditionFrame = num;
		m_LastDamagedFrame = num2;
	}

	private float CalculateLightFactor()
	{
		float dayLightBrightness = m_LightingSystem.dayLightBrightness;
		return math.saturate(1f - math.round(dayLightBrightness * 100f) * 0.01f);
	}

	private void UpdateGlobalValues(NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SHCoefficients val = default(SHCoefficients);
		((SHCoefficients)(ref val))._002Ector(RenderSettings.ambientProbe);
		nativeBatchInstances.SetGlobalValue<SHCoefficients>(val, m_SHCoefficients);
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
	public BatchDataSystem()
	{
	}
}
