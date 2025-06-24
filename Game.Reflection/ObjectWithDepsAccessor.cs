using System;
using System.Reflection;
using Colossal.Annotations;
using Unity.Jobs;

namespace Game.Reflection;

public class ObjectWithDepsAccessor<T> : ObjectAccessor<T>
{
	[CanBeNull]
	private readonly FieldInfo[] m_Deps;

	public ObjectWithDepsAccessor([NotNull] T obj, [NotNull] FieldInfo[] deps)
		: base(obj, readOnly: true)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		m_Deps = deps ?? throw new ArgumentNullException("deps");
	}

	public override object GetValue()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (m_Deps != null)
		{
			FieldInfo[] deps = m_Deps;
			for (int i = 0; i < deps.Length; i++)
			{
				JobHandle val = (JobHandle)deps[i].GetValue(m_Object);
				((JobHandle)(ref val)).Complete();
			}
		}
		return base.GetValue();
	}
}
