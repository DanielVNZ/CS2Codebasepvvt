using System;
using Colossal;
using Colossal.Serialization.Entities;
using Game.Rendering;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[FormerlySerializedAs("Colossal.Terrain.TerrainDebugSystem, Game")]
public class TerrainDebugSystem : GameSystemBase
{
	public enum DebugViewMode
	{
		LODLevel,
		TreePosition
	}

	public enum DebugMode
	{
		Cascade,
		World
	}

	private DebugMode m_Mode;

	private TerrainSystem m_TerrainSystem;

	private TerrainRenderSystem m_TerrainRenderSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainRenderSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		DrawDebug();
	}

	private void DrawDebug()
	{
		if (m_Mode == DebugMode.Cascade)
		{
			DrawRoads();
			DrawCascades();
		}
	}

	public void RenderDebugUI()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Debug Mode", Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Cascades", Array.Empty<GUILayoutOption>()))
		{
			m_Mode = DebugMode.Cascade;
		}
		if (GUILayout.Button("World", Array.Empty<GUILayoutOption>()))
		{
			m_Mode = DebugMode.World;
		}
		GUILayout.EndHorizontal();
		if (m_Mode == DebugMode.Cascade)
		{
			GUIStyle val = new GUIStyle();
			GUIStyle val2 = new GUIStyle();
			val.fontSize = 16;
			val2.fontSize = 16;
			val2.normal.textColor = new Color(1f, 1f, 1f);
			val.normal.textColor = new Color(0.4f, 1f, 0.4f);
			GUILayout.Label("Last Cull Area " + ((object)m_TerrainSystem.lastCullArea/*cast due to .constrained prefix*/).ToString(), val, Array.Empty<GUILayoutOption>());
			for (int i = 0; i < 4; i++)
			{
				float4 val3 = m_TerrainSystem.heightMapSliceArea[i];
				float4 val4 = m_TerrainSystem.heightMapViewportUpdated[i];
				GUILayout.Label($"Cascade[{i}] Size min:{val3.x},{val3.y} max:{val3.z},{val3.w} Viewport[{val4.x},{val4.y},{val4.z},{val4.w}]", m_TerrainSystem.heightMapSliceUpdatedLast[i] ? val : val2, Array.Empty<GUILayoutOption>());
			}
			m_TerrainSystem.freezeCascadeUpdates = GUILayout.Toggle(m_TerrainSystem.freezeCascadeUpdates, "Freeze Cascade Updates", Array.Empty<GUILayoutOption>());
		}
		else if (m_Mode == DebugMode.World)
		{
			GUILayout.Label($"World Size {m_TerrainSystem.worldSize.x}, {m_TerrainSystem.worldSize.y}", Array.Empty<GUILayoutOption>());
			GUILayout.Label($"World Offset {m_TerrainSystem.worldOffset.x}, {m_TerrainSystem.worldOffset.y}", Array.Empty<GUILayoutOption>());
			GUILayout.Label($"Playable Size {m_TerrainSystem.playableArea.x}, {m_TerrainSystem.playableArea.y}", Array.Empty<GUILayoutOption>());
			GUILayout.Label($"Playable Offset {m_TerrainSystem.playableOffset.x}, {m_TerrainSystem.playableOffset.y}", Array.Empty<GUILayoutOption>());
		}
	}

	private void DrawRoads()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		NativeList<TerrainSystem.LaneSection> roads = m_TerrainSystem.GetRoads();
		Bounds val = default(Bounds);
		Bounds lastCullArea = m_TerrainRenderSystem.GetLastCullArea();
		Bounds cascadeRegion = m_TerrainRenderSystem.GetCascadeRegion(3);
		for (int i = 0; i < roads.Length; i++)
		{
			TerrainSystem.LaneSection laneSection = roads[i];
			float4 val2 = math.min(laneSection.m_Left.c1, laneSection.m_Right.c1);
			float num = math.min(math.min(val2.x, val2.y), math.min(val2.z, val2.w)) - 1f;
			val2 = math.max(laneSection.m_Left.c1, laneSection.m_Right.c1);
			float num2 = math.max(math.max(val2.x, val2.y), math.max(val2.z, val2.w)) + 1f;
			((Bounds)(ref val)).SetMinMax(float3.op_Implicit(new float3(laneSection.m_Bounds.min.x, num, laneSection.m_Bounds.min.y)), float3.op_Implicit(new float3(laneSection.m_Bounds.max.x, num2, laneSection.m_Bounds.max.y)));
			if ((laneSection.m_Flags & TerrainSystem.LaneFlags.ShiftTerrain) == 0)
			{
				((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(val, new Color(0.84f, 0.84f, 0.84f, 0.9f));
			}
			else if (((Bounds)(ref val)).Intersects(lastCullArea))
			{
				if (((Bounds)(ref val)).Intersects(cascadeRegion))
				{
					((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(val, new Color(0.37f, 0.84f, 0.42f, 0.9f));
				}
				else
				{
					((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(val, new Color(0.59f, 0.59f, 0.21f, 0.9f));
				}
			}
			else
			{
				((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(val, new Color(0.59f, 0.21f, 0.18f, 0.9f));
			}
		}
	}

	private void DrawCascades()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(m_TerrainRenderSystem.GetCascadeRegion(i), new Color(0.1f, 0.28f, 0.89f, 0.9f));
			if (m_TerrainSystem.heightMapSliceUpdatedLast[i])
			{
				((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(m_TerrainRenderSystem.GetCascadeViewport(i), new Color(0.1f, 0.89f, 0.28f, 0.9f));
				((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(m_TerrainRenderSystem.GetCascadeCullArea(i), new Color(1f, 0.85f, 0.2f, 0.9f));
			}
		}
		((GizmoBatcher)(ref Gizmos.batcher)).DrawWireBounds(m_TerrainRenderSystem.GetLastCullArea(), new Color(0.9f, 0.9f, 0.2f, 0.9f));
	}

	[Preserve]
	public TerrainDebugSystem()
	{
	}
}
