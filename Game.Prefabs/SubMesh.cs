using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[InternalBufferCapacity(1)]
public struct SubMesh : IBufferElementData, IEmptySerializable
{
	public Entity m_SubMesh;

	public float3 m_Position;

	public quaternion m_Rotation;

	public SubMeshFlags m_Flags;

	public ushort m_RandomSeed;

	public SubMesh(Entity mesh, SubMeshFlags flags, ushort randomSeed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_SubMesh = mesh;
		m_Position = default(float3);
		m_Rotation = quaternion.identity;
		m_Flags = flags;
		m_RandomSeed = randomSeed;
	}

	public SubMesh(Entity mesh, float3 position, quaternion rotation, SubMeshFlags flags, ushort randomSeed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_SubMesh = mesh;
		m_Position = position;
		m_Rotation = rotation;
		m_Flags = flags;
		m_RandomSeed = randomSeed;
	}
}
