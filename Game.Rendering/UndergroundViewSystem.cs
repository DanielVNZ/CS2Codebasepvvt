using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class UndergroundViewSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetGeometryData> __Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> __Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> __Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetGeometryData>(true);
			__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetStatusData>(true);
			__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewCoverageData>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private UtilityLodUpdateSystem m_UtilityLodUpdateSystem;

	private RenderingSystem m_RenderingSystem;

	private EntityQuery m_InfomodeQuery;

	private bool m_LastWasWaterways;

	private bool m_LastWasMarkers;

	private bool m_Loaded;

	private UtilityTypes m_LastUtilityTypes;

	private TypeHandle __TypeHandle;

	public bool undergroundOn { get; private set; }

	public bool tunnelsOn { get; private set; }

	public bool pipelinesOn { get; private set; }

	public bool subPipelinesOn { get; private set; }

	public bool waterwaysOn { get; private set; }

	public bool contourLinesOn { get; private set; }

	public bool markersOn { get; private set; }

	public UtilityTypes utilityTypes { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_UtilityLodUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UtilityLodUpdateSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeActive>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<InfoviewNetGeometryData>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>(),
			ComponentType.ReadOnly<InfoviewCoverageData>()
		};
		array[0] = val;
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.GetAvailableSnapMask(out var onMask, out var offMask);
			undergroundOn = m_ToolSystem.activeTool.requireUnderground;
			tunnelsOn = m_ToolSystem.activeTool.requireUnderground || (m_ToolSystem.activeTool.requireNet & (Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.SubwayTrack | Layer.PublicTransportRoad)) != 0;
			subPipelinesOn = (m_ToolSystem.activeTool.requireNet & (Layer.PowerlineLow | Layer.PowerlineHigh | Layer.WaterPipe | Layer.SewagePipe)) != Layer.None || (undergroundOn && (m_ToolSystem.activeTool.requireNet & Layer.ResourceLine) != 0);
			pipelinesOn = m_ToolSystem.activeTool.requirePipelines || subPipelinesOn || (undergroundOn && tunnelsOn);
			waterwaysOn = (m_ToolSystem.activeTool.requireNet & Layer.Waterway) != 0;
			contourLinesOn = (ToolBaseSystem.GetActualSnap(m_ToolSystem.activeTool.selectedSnap, onMask, offMask) & Snap.ContourLines) != 0;
		}
		else
		{
			undergroundOn = false;
			tunnelsOn = false;
			pipelinesOn = false;
			subPipelinesOn = false;
			waterwaysOn = false;
			contourLinesOn = false;
		}
		markersOn = !m_RenderingSystem.hideOverlay;
		utilityTypes = UtilityTypes.None;
		if (!((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			ComponentTypeHandle<InfoviewNetGeometryData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetGeometryData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewNetStatusData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfoviewCoverageData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<InfoviewCoverageData>(ref __TypeHandle.__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<InfoviewNetGeometryData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewNetGeometryData>(ref componentTypeHandle);
				NativeArray<InfoviewNetStatusData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewNetStatusData>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					switch (nativeArray[j].m_Type)
					{
					case NetType.Road:
						tunnelsOn = true;
						break;
					case NetType.TrainTrack:
						tunnelsOn = true;
						break;
					case NetType.TramTrack:
						tunnelsOn = true;
						break;
					case NetType.Waterway:
						waterwaysOn = true;
						break;
					case NetType.SubwayTrack:
						tunnelsOn = true;
						break;
					}
				}
				for (int k = 0; k < nativeArray2.Length; k++)
				{
					switch (nativeArray2[k].m_Type)
					{
					case NetStatusType.Wear:
						tunnelsOn = true;
						break;
					case NetStatusType.TrafficFlow:
						tunnelsOn = true;
						break;
					case NetStatusType.TrafficVolume:
						tunnelsOn = true;
						break;
					case NetStatusType.LowVoltageFlow:
						pipelinesOn = true;
						subPipelinesOn = true;
						utilityTypes |= UtilityTypes.LowVoltageLine;
						break;
					case NetStatusType.HighVoltageFlow:
						pipelinesOn = true;
						subPipelinesOn = true;
						utilityTypes |= UtilityTypes.HighVoltageLine;
						break;
					case NetStatusType.PipeWaterFlow:
						pipelinesOn = true;
						subPipelinesOn = true;
						utilityTypes |= UtilityTypes.WaterPipe;
						break;
					case NetStatusType.PipeSewageFlow:
						pipelinesOn = true;
						subPipelinesOn = true;
						utilityTypes |= UtilityTypes.SewagePipe;
						break;
					case NetStatusType.OilFlow:
						pipelinesOn = true;
						subPipelinesOn = true;
						utilityTypes |= UtilityTypes.Resource;
						break;
					}
				}
				if (((ArchetypeChunk)(ref val2)).Has<InfoviewCoverageData>(ref componentTypeHandle3))
				{
					tunnelsOn = true;
				}
			}
			val.Dispose();
		}
		if (utilityTypes != m_LastUtilityTypes)
		{
			m_LastUtilityTypes = utilityTypes;
			if (!loaded)
			{
				((ComponentSystemBase)m_UtilityLodUpdateSystem).Update();
			}
		}
		if (waterwaysOn != m_LastWasWaterways)
		{
			m_LastWasWaterways = waterwaysOn;
			Camera main = Camera.main;
			if ((Object)(object)main != (Object)null)
			{
				if (waterwaysOn)
				{
					main.cullingMask |= 1 << LayerMask.NameToLayer("Waterway");
				}
				else
				{
					main.cullingMask &= ~(1 << LayerMask.NameToLayer("Waterway"));
				}
			}
		}
		if (markersOn == m_LastWasMarkers)
		{
			return;
		}
		m_LastWasMarkers = markersOn;
		Camera main2 = Camera.main;
		if ((Object)(object)main2 != (Object)null)
		{
			if (markersOn)
			{
				main2.cullingMask |= 1 << LayerMask.NameToLayer("Marker");
			}
			else
			{
				main2.cullingMask &= ~(1 << LayerMask.NameToLayer("Marker"));
			}
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
	public UndergroundViewSystem()
	{
	}
}
