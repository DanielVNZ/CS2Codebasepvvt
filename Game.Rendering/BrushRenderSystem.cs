using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BrushRenderSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_Brush_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private EntityQuery m_BrushQuery;

	private EntityQuery m_SettingsQuery;

	private ToolSystem m_ToolSystem;

	private TerrainSystem m_TerrainSystem;

	private PrefabSystem m_PrefabSystem;

	private Mesh m_Mesh;

	private MaterialPropertyBlock m_Properties;

	private int m_BrushTexture;

	private int m_BrushOpacity;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BrushQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OverlayConfigurationData>() });
		m_BrushTexture = Shader.PropertyToID("_BrushTexture");
		m_BrushOpacity = Shader.PropertyToID("_BrushOpacity");
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		CoreUtils.Destroy((Object)(object)m_Mesh);
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	private void Render(ScriptableRenderContext context, List<Camera> cameras)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Invalid comparison between Unknown and I4
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Invalid comparison between Unknown and I4
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!m_PrefabSystem.TryGetSingletonPrefab<OverlayConfigurationPrefab>(m_SettingsQuery, out var prefab))
			{
				return;
			}
			float num = m_TerrainSystem.heightScaleOffset.y - 50f;
			float num2 = m_TerrainSystem.heightScaleOffset.x + 100f;
			Mesh mesh = GetMesh();
			MaterialPropertyBlock properties = GetProperties();
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_BrushQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			((SystemBase)this).CompleteDependency();
			try
			{
				ComponentTypeHandle<Brush> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				ComponentTypeHandle<PrefabRef> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				float3 val5 = default(float3);
				TerraformingData terraformingData = default(TerraformingData);
				for (int i = 0; i < val.Length; i++)
				{
					ArchetypeChunk val2 = val[i];
					NativeArray<Brush> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Brush>(ref componentTypeHandle);
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Brush brush = nativeArray[j];
						PrefabRef refData = nativeArray2[j];
						float3 val3 = new float3(brush.m_Position.x, num, brush.m_Position.z);
						quaternion val4 = quaternion.RotateY(brush.m_Angle);
						((float3)(ref val5))._002Ector(brush.m_Size * 0.5f, num2, brush.m_Size * 0.5f);
						PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(brush.m_Tool);
						BrushPrefab prefab3 = m_PrefabSystem.GetPrefab<BrushPrefab>(refData);
						properties.Clear();
						properties.SetTexture(m_BrushTexture, (Texture)(object)prefab3.m_Texture);
						properties.SetFloat(m_BrushOpacity, brush.m_Opacity);
						Material val6 = null;
						if (prefab2 is TerraformingPrefab terraformingPrefab)
						{
							val6 = terraformingPrefab.m_BrushMaterial;
						}
						else if (prefab2 is ObjectPrefab)
						{
							val6 = prefab.m_ObjectBrushMaterial;
						}
						Matrix4x4 val7 = Matrix4x4.TRS(float3.op_Implicit(val3), quaternion.op_Implicit(val4), float3.op_Implicit(val5));
						foreach (Camera camera in cameras)
						{
							if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
							{
								Graphics.DrawMesh(mesh, val7, val6, 0, camera, 0, properties, (ShadowCastingMode)0, false);
							}
						}
						if (EntitiesExtensions.TryGetComponent<TerraformingData>(((ComponentSystemBase)this).EntityManager, brush.m_Tool, ref terraformingData))
						{
							PreviewHeight(brush, prefab3, terraformingData.m_Type);
						}
					}
				}
			}
			finally
			{
				val.Dispose();
			}
		}
		finally
		{
		}
	}

	private void PreviewHeight(Brush brush, BrushPrefab prefab, TerraformingType terraformingType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Bounds2 bounds = ToolUtils.GetBounds(brush);
		if ((terraformingType == TerraformingType.Level || terraformingType == TerraformingType.Slope) && brush.m_Strength < 0f)
		{
			if (terraformingType == TerraformingType.Level)
			{
				brush.m_Strength = math.abs(brush.m_Strength);
			}
			else
			{
				brush.m_Strength = 0f;
			}
		}
		m_TerrainSystem.PreviewBrush(terraformingType, bounds, brush, (Texture)(object)prefab.m_Texture);
	}

	private Mesh GetMesh()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Mesh == (Object)null)
		{
			m_Mesh = new Mesh();
			((Object)m_Mesh).name = "Brush";
			m_Mesh.vertices = (Vector3[])(object)new Vector3[8]
			{
				new Vector3(-1f, 0f, -1f),
				new Vector3(-1f, 0f, 1f),
				new Vector3(1f, 0f, 1f),
				new Vector3(1f, 0f, -1f),
				new Vector3(-1f, 1f, -1f),
				new Vector3(-1f, 1f, 1f),
				new Vector3(1f, 1f, 1f),
				new Vector3(1f, 1f, -1f)
			};
			m_Mesh.triangles = new int[36]
			{
				0, 1, 5, 5, 4, 0, 3, 7, 6, 6,
				2, 3, 0, 3, 2, 2, 1, 0, 4, 5,
				6, 6, 7, 4, 0, 4, 7, 7, 3, 0,
				1, 2, 6, 6, 5, 1
			};
		}
		return m_Mesh;
	}

	private MaterialPropertyBlock GetProperties()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (m_Properties == null)
		{
			m_Properties = new MaterialPropertyBlock();
		}
		return m_Properties;
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
	public BrushRenderSystem()
	{
	}
}
