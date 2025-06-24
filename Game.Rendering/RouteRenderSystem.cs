using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class RouteRenderSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteBufferIndex> __Game_Rendering_RouteBufferIndex_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Color> __Game_Routes_Color_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Highlighted> __Game_Tools_Highlighted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

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
			__Game_Rendering_RouteBufferIndex_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteBufferIndex>(true);
			__Game_Routes_Color_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Color>(true);
			__Game_Tools_Highlighted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Highlighted>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private RouteBufferSystem m_RouteBufferSystem;

	private EntityQuery m_RouteQuery;

	private EntityQuery m_LivePathQuery;

	private EntityQuery m_InfomodeQuery;

	private Mesh m_Mesh;

	private ComputeBuffer m_ArgsBuffer;

	private List<uint> m_ArgsArray;

	private int m_RouteSegmentBuffer;

	private int m_RouteColor;

	private int m_RouteSize;

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
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_RouteBufferSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteBufferSystem>();
		m_RouteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<RouteSegment>(),
			ComponentType.Exclude<HiddenRoute>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_LivePathQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<RouteSegment>(),
			ComponentType.Exclude<HiddenRoute>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewRouteData>()
		});
		m_RouteSegmentBuffer = Shader.PropertyToID("colossal_RouteSegmentBuffer");
		m_RouteColor = Shader.PropertyToID("colossal_RouteColor");
		m_RouteSize = Shader.PropertyToID("colossal_RouteSize");
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		if ((Object)(object)m_Mesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_Mesh);
		}
		if (m_ArgsBuffer != null)
		{
			m_ArgsBuffer.Release();
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	private void Render(ScriptableRenderContext context, List<Camera> cameras)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Invalid comparison between Unknown and I4
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Invalid comparison between Unknown and I4
		try
		{
			EntityQuery val = (ShouldRenderRoutes() ? m_RouteQuery : m_LivePathQuery);
			if (((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
			{
				return;
			}
			NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				if (val2.Length == 0)
				{
					return;
				}
				EnsureMesh();
				if (m_ArgsArray == null)
				{
					m_ArgsArray = new List<uint>();
				}
				m_ArgsArray.Clear();
				uint indexCount = m_Mesh.GetIndexCount(0);
				uint indexStart = m_Mesh.GetIndexStart(0);
				uint baseVertex = m_Mesh.GetBaseVertex(0);
				((SystemBase)this).CompleteDependency();
				EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
				ComponentTypeHandle<RouteBufferIndex> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<RouteBufferIndex>(ref __TypeHandle.__Game_Rendering_RouteBufferIndex_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				ComponentTypeHandle<Color> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				ComponentTypeHandle<Highlighted> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Highlighted>(ref __TypeHandle.__Game_Tools_Highlighted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				ComponentTypeHandle<Temp> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				int num = 0;
				for (int i = 0; i < val2.Length; i++)
				{
					ArchetypeChunk val3 = val2[i];
					num += ((ArchetypeChunk)(ref val3)).Count * 5;
				}
				if (m_ArgsBuffer != null && m_ArgsBuffer.count < num)
				{
					m_ArgsBuffer.Release();
					m_ArgsBuffer = null;
				}
				if (m_ArgsBuffer == null)
				{
					m_ArgsBuffer = new ComputeBuffer(num, 4, (ComputeBufferType)256);
					m_ArgsBuffer.name = "Route args buffer";
				}
				Entity selected = m_ToolSystem.selected;
				for (int j = 0; j < val2.Length; j++)
				{
					ArchetypeChunk val4 = val2[j];
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(entityTypeHandle);
					NativeArray<RouteBufferIndex> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<RouteBufferIndex>(ref componentTypeHandle);
					NativeArray<Color> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Color>(ref componentTypeHandle2);
					NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Temp>(ref componentTypeHandle4);
					bool flag = ((ArchetypeChunk)(ref val4)).Has<Highlighted>(ref componentTypeHandle3);
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						RouteBufferIndex routeBufferIndex = nativeArray2[k];
						m_RouteBufferSystem.GetBuffer(routeBufferIndex.m_Index, out var material, out var segmentBuffer, out var originalRenderQueue, out var bounds, out var size);
						if ((Object)(object)material == (Object)null || segmentBuffer == null)
						{
							continue;
						}
						int count = m_ArgsArray.Count;
						m_ArgsArray.Add(indexCount);
						m_ArgsArray.Add((uint)segmentBuffer.count);
						m_ArgsArray.Add(indexStart);
						m_ArgsArray.Add(baseVertex);
						m_ArgsArray.Add(0u);
						Color color = nativeArray3[k];
						if (nativeArray[k] == selected || flag || (nativeArray4.Length != 0 && (nativeArray4[k].m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify)) != 0))
						{
							color.m_Color.a = byte.MaxValue;
							size.x *= 1.3333334f;
							material.renderQueue = originalRenderQueue + 1;
						}
						else
						{
							color.m_Color.a = 128;
							material.renderQueue = originalRenderQueue;
						}
						material.SetBuffer(m_RouteSegmentBuffer, segmentBuffer);
						material.SetColor(m_RouteColor, Color32.op_Implicit(color.m_Color));
						material.SetVector(m_RouteSize, size);
						((Bounds)(ref bounds)).Expand(size.x);
						foreach (Camera camera in cameras)
						{
							if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
							{
								Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, material, bounds, m_ArgsBuffer, count * 4, (MaterialPropertyBlock)null, (ShadowCastingMode)0, false, 0, camera, (LightProbeUsage)1);
							}
						}
					}
				}
			}
			finally
			{
				val2.Dispose();
			}
			if (m_ArgsArray.Count > 0)
			{
				m_ArgsBuffer.SetData<uint>(m_ArgsArray, 0, 0, m_ArgsArray.Count);
			}
		}
		finally
		{
		}
	}

	private bool ShouldRenderRoutes()
	{
		if ((m_ToolSystem.activeTool == null || m_ToolSystem.activeTool.requireRoutes == RouteType.None) && ((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		return true;
	}

	private void EnsureMesh()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		if (!((Object)(object)m_Mesh == (Object)null))
		{
			return;
		}
		Vector3[] array = (Vector3[])(object)new Vector3[68];
		Vector2[] array2 = (Vector2[])(object)new Vector2[array.Length];
		int[] array3 = new int[192];
		int num = 0;
		int num2 = 0;
		for (int i = 0; i <= 16; i++)
		{
			float num3 = (float)i / 16f;
			array[num] = new Vector3(-1f, 0f, num3);
			array2[num] = new Vector2(0f, num3);
			num++;
			array[num] = new Vector3(1f, 0f, num3);
			array2[num] = new Vector2(0f, num3);
			num++;
			if (i != 0)
			{
				array3[num2++] = num - 4;
				array3[num2++] = num - 3;
				array3[num2++] = num - 2;
				array3[num2++] = num - 2;
				array3[num2++] = num - 3;
				array3[num2++] = num - 1;
			}
		}
		for (int j = 0; j <= 16; j++)
		{
			float num4 = (float)j / 16f;
			array[num] = new Vector3(0f, -1f, num4);
			array2[num] = new Vector2(1f, num4);
			num++;
			array[num] = new Vector3(0f, 1f, num4);
			array2[num] = new Vector2(1f, num4);
			num++;
			if (j != 0)
			{
				array3[num2++] = num - 4;
				array3[num2++] = num - 3;
				array3[num2++] = num - 2;
				array3[num2++] = num - 2;
				array3[num2++] = num - 3;
				array3[num2++] = num - 1;
			}
		}
		m_Mesh = new Mesh();
		((Object)m_Mesh).name = "Route segment";
		m_Mesh.vertices = array;
		m_Mesh.uv = array2;
		m_Mesh.triangles = array3;
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
	public RouteRenderSystem()
	{
	}
}
