using System.Collections.Generic;
using Game.Tools;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class AggregateRenderSystem : GameSystemBase
{
	private AggregateMeshSystem m_AggregateMeshSystem;

	private RenderingSystem m_RenderingSystem;

	private ToolSystem m_ToolSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_AggregateMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AggregateMeshSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	private void Render(ScriptableRenderContext context, List<Camera> cameras)
	{
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		try
		{
			if (m_RenderingSystem.hideOverlay)
			{
				return;
			}
			if (m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireNetArrows)
			{
				int arrowMaterialCount = m_AggregateMeshSystem.GetArrowMaterialCount();
				for (int i = 0; i < arrowMaterialCount; i++)
				{
					if (!m_AggregateMeshSystem.GetArrowMesh(i, out var mesh, out var subMeshCount))
					{
						continue;
					}
					for (int j = 0; j < subMeshCount; j++)
					{
						if (!m_AggregateMeshSystem.GetArrowMaterial(i, j, out var material))
						{
							continue;
						}
						foreach (Camera camera in cameras)
						{
							if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
							{
								Graphics.DrawMesh(mesh, Matrix4x4.identity, material, 0, camera, j, (MaterialPropertyBlock)null, false, false);
							}
						}
					}
				}
				return;
			}
			int nameMaterialCount = m_AggregateMeshSystem.GetNameMaterialCount();
			for (int k = 0; k < nameMaterialCount; k++)
			{
				if (!m_AggregateMeshSystem.GetNameMesh(k, out var mesh2, out var subMeshCount2))
				{
					continue;
				}
				for (int l = 0; l < subMeshCount2; l++)
				{
					if (!m_AggregateMeshSystem.GetNameMaterial(k, l, out var material2))
					{
						continue;
					}
					foreach (Camera camera2 in cameras)
					{
						if ((int)camera2.cameraType == 1 || (int)camera2.cameraType == 2)
						{
							Graphics.DrawMesh(mesh2, Matrix4x4.identity, material2, 0, camera2, l, (MaterialPropertyBlock)null, false, false);
						}
					}
				}
			}
		}
		finally
		{
		}
	}

	[Preserve]
	public AggregateRenderSystem()
	{
	}
}
