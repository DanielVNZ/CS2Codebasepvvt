using System.Collections.Generic;
using Game.Common;
using Game.Objects;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public abstract class ObjectGeometryPrefab : ObjectPrefab
{
	public ObjectMeshInfo[] m_Meshes;

	public bool m_Circular;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Meshes != null)
		{
			for (int i = 0; i < m_Meshes.Length; i++)
			{
				prefabs.Add(m_Meshes[i].m_Mesh);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectGeometryData>());
		components.Add(ComponentType.ReadWrite<SubMesh>());
		bool flag = false;
		bool flag2 = false;
		if (m_Meshes != null)
		{
			for (int i = 0; i < m_Meshes.Length; i++)
			{
				RenderPrefabBase mesh = m_Meshes[i].m_Mesh;
				if (!((Object)(object)mesh == (Object)null))
				{
					flag |= mesh.Has<StackProperties>();
					flag2 = flag2 || mesh is CharacterGroup;
				}
			}
		}
		if (flag)
		{
			components.Add(ComponentType.ReadWrite<StackData>());
		}
		if (flag2)
		{
			components.Add(ComponentType.ReadWrite<SubMeshGroup>());
			components.Add(ComponentType.ReadWrite<CharacterElement>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectGeometry>());
		components.Add(ComponentType.ReadWrite<CullingInfo>());
		components.Add(ComponentType.ReadWrite<MeshBatch>());
		components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		if (m_Meshes != null)
		{
			for (int i = 0; i < m_Meshes.Length; i++)
			{
				RenderPrefabBase mesh = m_Meshes[i].m_Mesh;
				if ((Object)(object)mesh == (Object)null)
				{
					continue;
				}
				flag |= mesh.Has<ColorProperties>();
				flag2 |= mesh.Has<StackProperties>();
				flag3 |= mesh.Has<AnimationProperties>();
				flag7 = flag7 || mesh is CharacterGroup;
				ProceduralAnimationProperties component = mesh.GetComponent<ProceduralAnimationProperties>();
				if ((Object)(object)component != (Object)null)
				{
					flag4 = true;
					if (component.m_Bones != null)
					{
						for (int j = 0; j < component.m_Bones.Length; j++)
						{
							switch (component.m_Bones[j].m_Type)
							{
							case BoneType.LookAtDirection:
							case BoneType.WindTurbineRotation:
							case BoneType.WindSpeedRotation:
							case BoneType.PoweredRotation:
							case BoneType.TrafficBarrierDirection:
							case BoneType.LookAtRotation:
							case BoneType.LookAtAim:
							case BoneType.LengthwiseLookAtRotation:
							case BoneType.WorkingRotation:
							case BoneType.OperatingRotation:
							case BoneType.LookAtMovementX:
							case BoneType.LookAtMovementY:
							case BoneType.LookAtMovementZ:
							case BoneType.LookAtRotationSide:
							case BoneType.LookAtAimForward:
								flag5 = true;
								break;
							}
						}
					}
				}
				if ((Object)(object)mesh.GetComponent<EmissiveProperties>() != (Object)null)
				{
					flag6 = true;
				}
			}
		}
		if (flag || flag7)
		{
			components.Add(ComponentType.ReadWrite<MeshColor>());
		}
		if (flag2)
		{
			components.Add(ComponentType.ReadWrite<Stack>());
		}
		if (flag3)
		{
			components.Add(ComponentType.ReadWrite<Animated>());
		}
		if (flag4)
		{
			components.Add(ComponentType.ReadWrite<Skeleton>());
			components.Add(ComponentType.ReadWrite<Bone>());
			components.Add(ComponentType.ReadWrite<BoneHistory>());
			if (flag5)
			{
				components.Add(ComponentType.ReadWrite<Momentum>());
			}
		}
		if (flag6)
		{
			components.Add(ComponentType.ReadWrite<Emissive>());
			components.Add(ComponentType.ReadWrite<LightState>());
		}
		if (flag7)
		{
			components.Add(ComponentType.ReadWrite<MeshGroup>());
			components.Add(ComponentType.ReadWrite<Animated>());
		}
	}
}
