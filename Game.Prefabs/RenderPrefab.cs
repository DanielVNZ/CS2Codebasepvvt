using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { })]
public class RenderPrefab : RenderPrefabBase
{
	[SerializeField]
	private AssetReference<GeometryAsset> m_GeometryAsset;

	[SerializeField]
	private AssetReference<SurfaceAsset>[] m_SurfaceAssets;

	[SerializeField]
	private Bounds3 m_Bounds;

	[SerializeField]
	private float m_SurfaceArea;

	[SerializeField]
	private int m_IndexCount;

	[SerializeField]
	private int m_VertexCount;

	[SerializeField]
	private int m_MeshCount;

	[SerializeField]
	private bool m_IsImpostor;

	[SerializeField]
	private bool m_ManualVTRequired;

	private Material[] m_MaterialsContainer;

	public bool hasGeometryAsset => m_GeometryAsset != (AssetReference<GeometryAsset>)null;

	public GeometryAsset geometryAsset
	{
		get
		{
			return AssetReference<GeometryAsset>.op_Implicit(m_GeometryAsset);
		}
		set
		{
			m_GeometryAsset = AssetReference<GeometryAsset>.op_Implicit(value);
		}
	}

	public IEnumerable<SurfaceAsset> surfaceAssets
	{
		get
		{
			return m_SurfaceAssets.Select((AssetReference<SurfaceAsset> x) => AssetReference<SurfaceAsset>.op_Implicit(x));
		}
		set
		{
			m_SurfaceAssets = value.Select((SurfaceAsset x) => new AssetReference<SurfaceAsset>(Identifier.op_Implicit(((AssetData)x).id))).ToArray();
		}
	}

	public Bounds3 bounds
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Bounds;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = value;
		}
	}

	public float surfaceArea
	{
		get
		{
			return m_SurfaceArea;
		}
		set
		{
			m_SurfaceArea = value;
		}
	}

	public int indexCount
	{
		get
		{
			return m_IndexCount;
		}
		set
		{
			m_IndexCount = value;
		}
	}

	public int vertexCount
	{
		get
		{
			return m_VertexCount;
		}
		set
		{
			m_VertexCount = value;
		}
	}

	public int meshCount
	{
		get
		{
			return m_MeshCount;
		}
		set
		{
			m_MeshCount = value;
		}
	}

	public bool isImpostor
	{
		get
		{
			return m_IsImpostor;
		}
		set
		{
			m_IsImpostor = value;
		}
	}

	public bool manualVTRequired
	{
		get
		{
			return m_ManualVTRequired;
		}
		set
		{
			m_ManualVTRequired = value;
		}
	}

	public int materialCount => m_SurfaceAssets.Length;

	public SurfaceAsset GetSurfaceAsset(int index)
	{
		return AssetReference<SurfaceAsset>.op_Implicit(m_SurfaceAssets[index]);
	}

	public void SetSurfaceAsset(int index, SurfaceAsset value)
	{
		m_SurfaceAssets[index] = AssetReference<SurfaceAsset>.op_Implicit(value);
	}

	public Mesh[] ObtainMeshes()
	{
		ComponentBase.baseLog.TraceFormat((Object)(object)this, "ObtainMeshes {0}", (object)((Object)this).name);
		GeometryAsset obj = geometryAsset;
		if (obj == null)
		{
			return null;
		}
		return obj.ObtainMeshes(false);
	}

	public Mesh ObtainMesh(int materialIndex, out int subMeshIndex)
	{
		ComponentBase.baseLog.TraceFormat((Object)(object)this, "ObtainMesh {0}", (object)((Object)this).name);
		subMeshIndex = materialIndex;
		if (hasGeometryAsset)
		{
			GeometryAsset obj = geometryAsset;
			Mesh[] array = ((obj != null) ? obj.ObtainMeshes(false) : null);
			if (array != null)
			{
				Mesh[] array2 = array;
				foreach (Mesh val in array2)
				{
					if (materialIndex < val.subMeshCount)
					{
						subMeshIndex = materialIndex;
						return val;
					}
					materialIndex -= val.subMeshCount;
				}
			}
		}
		return null;
	}

	public void ReleaseMeshes()
	{
		ComponentBase.baseLog.TraceFormat((Object)(object)this, "ReleaseMeshes {0}", (object)((Object)this).name);
		GeometryAsset obj = geometryAsset;
		if (obj != null)
		{
			obj.ReleaseMeshes();
		}
	}

	public Material[] ObtainMaterials(bool useVT = true)
	{
		ComponentBase.baseLog.TraceFormat((Object)(object)this, "ObtainMaterials {0}", (object)((Object)this).name);
		if (m_MaterialsContainer == null || m_MaterialsContainer.Length != materialCount)
		{
			m_MaterialsContainer = (Material[])(object)new Material[materialCount];
		}
		for (int i = 0; i < materialCount; i++)
		{
			SurfaceAsset val = AssetReference<SurfaceAsset>.op_Implicit(m_SurfaceAssets[i]);
			m_MaterialsContainer[i] = val.Load(-1, true, (KeepOnCPU)0, useVT);
		}
		return m_MaterialsContainer;
	}

	public Material ObtainMaterial(int i, bool useVT = true)
	{
		Material[] array = ObtainMaterials(useVT);
		if (i < 0 || i >= array.Length)
		{
			throw new IndexOutOfRangeException($"i {i} is out of material range (length: {array.Length}) in {((Object)this).name}");
		}
		return array[i];
	}

	public void ReleaseMaterials()
	{
		ComponentBase.baseLog.TraceFormat((Object)(object)this, "ReleaseMaterials {0}", (object)((Object)this).name);
	}

	public void Release()
	{
		ReleaseMeshes();
		ReleaseMaterials();
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<MeshData>());
		components.Add(ComponentType.ReadWrite<SharedMeshData>());
		components.Add(ComponentType.ReadWrite<BatchGroup>());
		if (isImpostor)
		{
			components.Add(ComponentType.ReadWrite<ImpostorData>());
		}
	}
}
