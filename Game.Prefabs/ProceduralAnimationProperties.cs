using System;
using System.Collections.Generic;
using Colossal;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class ProceduralAnimationProperties : ComponentBase
{
	[Serializable]
	public class BoneInfo
	{
		public string name;

		public Vector3 position;

		[EulerAngles]
		public Quaternion rotation;

		public Vector3 scale;

		public Matrix4x4 bindPose;

		public int parentId;

		public BoneType m_Type;

		public float m_Speed;

		public float m_Acceleration;

		public int m_ConnectionID;

		public int m_SourceID;
	}

	public BoneInfo[] m_Bones;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ProceduralBone>());
	}
}
