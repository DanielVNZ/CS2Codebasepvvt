using Colossal.Mathematics;
using Unity.Entities;

namespace Game.Net;

public struct TreeObjectAction
{
	public Entity m_Remove;

	public Entity m_Add;

	public Bounds3 m_Bounds;

	public TreeObjectAction(Entity remove)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Remove = remove;
		m_Add = Entity.Null;
		m_Bounds = default(Bounds3);
	}

	public TreeObjectAction(Entity add, Bounds3 bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_Remove = Entity.Null;
		m_Add = add;
		m_Bounds = bounds;
	}

	public TreeObjectAction(Entity remove, Entity add, Bounds3 bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Remove = remove;
		m_Add = add;
		m_Bounds = bounds;
	}
}
