using Colossal.IO.AssetDatabase;
using Colossal.Rendering;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace Game.Rendering;

public class CustomBatch : ManagedBatch
{
	public SurfaceAsset sourceSurface { get; private set; }

	public Material sourceMaterial { get; private set; }

	public Material defaultMaterial { get; private set; }

	public Material loadedMaterial { get; private set; }

	public int sourceSubMeshIndex { get; private set; }

	public Entity sourceMeshEntity { get; private set; }

	public Entity sharedMeshEntity { get; private set; }

	public BatchFlags sourceFlags { get; private set; }

	public GeneratedType generatedType { get; private set; }

	public MeshType sourceType { get; private set; }

	public CustomBatch(int groupIndex, int batchIndex, SurfaceAsset sourceSurface, Material sourceMaterial, Material defaultMaterial, Material loadedMaterial, Mesh mesh, Entity meshEntity, Entity sharedEntity, BatchFlags flags, GeneratedType generatedType, MeshType type, int subMeshIndex, MaterialPropertyBlock customProps)
		: base(groupIndex, batchIndex, defaultMaterial, mesh, 0, customProps)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		this.sourceSurface = sourceSurface;
		this.sourceMaterial = sourceMaterial;
		this.defaultMaterial = defaultMaterial;
		this.loadedMaterial = loadedMaterial;
		sourceSubMeshIndex = subMeshIndex;
		sourceMeshEntity = meshEntity;
		sharedMeshEntity = sharedEntity;
		sourceFlags = flags;
		this.generatedType = generatedType;
		sourceType = type;
	}

	public void ReplaceMesh(Entity oldMesh, Entity newMesh)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (sourceMeshEntity == oldMesh)
		{
			sourceMeshEntity = newMesh;
		}
		if (sharedMeshEntity == oldMesh)
		{
			sharedMeshEntity = newMesh;
		}
	}

	public override void Dispose()
	{
		((ManagedBatch)this).Dispose();
	}
}
