using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Tools;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class AreaRenderSystem : GameSystemBase
{
	private RenderingSystem m_RenderingSystem;

	private AreaBufferSystem m_AreaBufferSystem;

	private AreaBatchSystem m_AreaBatchSystem;

	private CityBoundaryMeshSystem m_CityBoundaryMeshSystem;

	private ToolSystem m_ToolSystem;

	private int m_AreaTriangleBuffer;

	private int m_AreaBatchBuffer;

	private int m_AreaBatchColors;

	private int m_VisibleIndices;

	private Mesh m_AreaMesh;

	private GraphicsBuffer m_ArgsBuffer;

	private List<IndirectDrawIndexedArgs> m_ArgsArray;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_AreaBufferSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaBufferSystem>();
		m_AreaBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaBatchSystem>();
		m_CityBoundaryMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityBoundaryMeshSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaTriangleBuffer = Shader.PropertyToID("colossal_AreaTriangleBuffer");
		m_AreaBatchBuffer = Shader.PropertyToID("colossal_AreaBatchBuffer");
		m_AreaBatchColors = Shader.PropertyToID("colossal_AreaBatchColors");
		m_VisibleIndices = Shader.PropertyToID("colossal_VisibleIndices");
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		if ((Object)(object)m_AreaMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_AreaMesh);
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
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Invalid comparison between Unknown and I4
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Invalid comparison between Unknown and I4
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Invalid comparison between Unknown and I4
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Invalid comparison between Unknown and I4
		try
		{
			int num = 0;
			int batchCount = m_AreaBatchSystem.GetBatchCount();
			ComputeBuffer buffer;
			Material material3;
			Bounds bounds;
			if (!m_RenderingSystem.hideOverlay)
			{
				for (AreaType areaType = AreaType.Lot; areaType < AreaType.Count; areaType++)
				{
					if (!m_AreaBufferSystem.GetNameMesh(areaType, out var mesh, out var subMeshCount))
					{
						continue;
					}
					for (int i = 0; i < subMeshCount; i++)
					{
						if (!m_AreaBufferSystem.GetNameMaterial(areaType, i, out var material))
						{
							continue;
						}
						foreach (Camera camera in cameras)
						{
							if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
							{
								Graphics.DrawMesh(mesh, Matrix4x4.identity, material, 0, camera, i, (MaterialPropertyBlock)null, false, false);
							}
						}
					}
				}
				if (m_CityBoundaryMeshSystem.GetBoundaryMesh(out var mesh2, out var material2))
				{
					foreach (Camera camera2 in cameras)
					{
						if ((int)camera2.cameraType == 1 || (int)camera2.cameraType == 2)
						{
							Graphics.DrawMesh(mesh2, Matrix4x4.identity, material2, 0, camera2, 0, (MaterialPropertyBlock)null, false, false);
						}
					}
				}
				if (m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireAreas != AreaTypeMask.None)
				{
					for (int j = 0; j < 5; j++)
					{
						if (((uint)m_ToolSystem.activeTool.requireAreas & (uint)(1 << j)) != 0 && m_AreaBufferSystem.GetAreaBuffer((AreaType)j, out buffer, out material3, out bounds))
						{
							num++;
						}
					}
				}
			}
			for (int k = 0; k < batchCount; k++)
			{
				if (m_AreaBatchSystem.GetAreaBatch(k, out buffer, out var _, out var _, out material3, out bounds, out var _, out var _))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			if (m_ArgsBuffer != null && m_ArgsBuffer.count < num)
			{
				m_ArgsBuffer.Release();
				m_ArgsBuffer = null;
			}
			if (m_ArgsBuffer == null)
			{
				m_ArgsBuffer = new GraphicsBuffer((Target)256, num, System.Runtime.CompilerServices.Unsafe.SizeOf<IndirectDrawIndexedArgs>());
			}
			if (m_ArgsArray == null)
			{
				m_ArgsArray = new List<IndirectDrawIndexedArgs>();
			}
			m_ArgsArray.Clear();
			if (!m_RenderingSystem.hideOverlay && m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireAreas != AreaTypeMask.None)
			{
				RenderParams val = default(RenderParams);
				for (int l = 0; l < 5; l++)
				{
					if (((uint)m_ToolSystem.activeTool.requireAreas & (uint)(1 << l)) == 0 || !m_AreaBufferSystem.GetAreaBuffer((AreaType)l, out var buffer2, out var material4, out var bounds2))
					{
						continue;
					}
					if ((Object)(object)m_AreaMesh == (Object)null)
					{
						m_AreaMesh = CreateMesh();
					}
					IndirectDrawIndexedArgs item = default(IndirectDrawIndexedArgs);
					((IndirectDrawIndexedArgs)(ref item)).indexCountPerInstance = m_AreaMesh.GetIndexCount(0);
					((IndirectDrawIndexedArgs)(ref item)).instanceCount = (uint)buffer2.count;
					((IndirectDrawIndexedArgs)(ref item)).startIndex = m_AreaMesh.GetIndexStart(0);
					((IndirectDrawIndexedArgs)(ref item)).baseVertexIndex = m_AreaMesh.GetBaseVertex(0);
					int count2 = m_ArgsArray.Count;
					m_ArgsArray.Add(item);
					material4.SetBuffer(m_AreaTriangleBuffer, buffer2);
					foreach (Camera camera3 in cameras)
					{
						if ((int)camera3.cameraType == 1 || (int)camera3.cameraType == 2)
						{
							((RenderParams)(ref val))._002Ector(material4);
							((RenderParams)(ref val)).worldBounds = bounds2;
							((RenderParams)(ref val)).camera = camera3;
							Graphics.RenderMeshIndirect(ref val, m_AreaMesh, m_ArgsBuffer, 1, count2);
						}
					}
				}
			}
			RenderParams val2 = default(RenderParams);
			for (int m = 0; m < batchCount; m++)
			{
				if (!m_AreaBatchSystem.GetAreaBatch(m, out var buffer3, out var colors2, out var indices2, out var material5, out var bounds3, out var count3, out var rendererPriority2))
				{
					continue;
				}
				if ((Object)(object)m_AreaMesh == (Object)null)
				{
					m_AreaMesh = CreateMesh();
				}
				IndirectDrawIndexedArgs item2 = default(IndirectDrawIndexedArgs);
				((IndirectDrawIndexedArgs)(ref item2)).indexCountPerInstance = m_AreaMesh.GetIndexCount(0);
				((IndirectDrawIndexedArgs)(ref item2)).instanceCount = (uint)count3;
				((IndirectDrawIndexedArgs)(ref item2)).startIndex = m_AreaMesh.GetIndexStart(0);
				((IndirectDrawIndexedArgs)(ref item2)).baseVertexIndex = m_AreaMesh.GetBaseVertex(0);
				int count4 = m_ArgsArray.Count;
				m_ArgsArray.Add(item2);
				material5.SetBuffer(m_AreaBatchBuffer, buffer3);
				material5.SetBuffer(m_AreaBatchColors, colors2);
				material5.SetBuffer(m_VisibleIndices, indices2);
				foreach (Camera camera4 in cameras)
				{
					if ((int)camera4.cameraType != 4)
					{
						((RenderParams)(ref val2))._002Ector(material5);
						((RenderParams)(ref val2)).worldBounds = bounds3;
						((RenderParams)(ref val2)).camera = camera4;
						((RenderParams)(ref val2)).rendererPriority = rendererPriority2;
						Graphics.RenderMeshIndirect(ref val2, m_AreaMesh, m_ArgsBuffer, 1, count4);
					}
				}
			}
			m_ArgsBuffer.SetData<IndirectDrawIndexedArgs>(m_ArgsArray, 0, 0, m_ArgsArray.Count);
		}
		finally
		{
		}
	}

	private static Mesh CreateMesh()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		Vector3[] array = (Vector3[])(object)new Vector3[6];
		int[] array2 = new int[24];
		int num = 0;
		int num2 = 0;
		array[num++] = new Vector3(0f, 0f, 0f);
		array[num++] = new Vector3(0f, 0f, 1f);
		array[num++] = new Vector3(1f, 0f, 0f);
		array[num++] = new Vector3(0f, 1f, 0f);
		array[num++] = new Vector3(0f, 1f, 1f);
		array[num++] = new Vector3(1f, 1f, 0f);
		array2[num2++] = 0;
		array2[num2++] = 2;
		array2[num2++] = 1;
		array2[num2++] = 3;
		array2[num2++] = 4;
		array2[num2++] = 5;
		array2[num2++] = 3;
		array2[num2++] = 0;
		array2[num2++] = 4;
		array2[num2++] = 4;
		array2[num2++] = 0;
		array2[num2++] = 1;
		array2[num2++] = 4;
		array2[num2++] = 1;
		array2[num2++] = 5;
		array2[num2++] = 5;
		array2[num2++] = 1;
		array2[num2++] = 2;
		array2[num2++] = 5;
		array2[num2++] = 2;
		array2[num2++] = 3;
		array2[num2++] = 3;
		array2[num2++] = 2;
		array2[num2++] = 0;
		return new Mesh
		{
			name = "Area triangle volume",
			vertices = array,
			triangles = array2
		};
	}

	[Preserve]
	public AreaRenderSystem()
	{
	}
}
