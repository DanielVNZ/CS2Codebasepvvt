using System;
using Colossal.Serialization.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct PrefabID : IEquatable<PrefabID>, ISerializable
{
	private string m_Type;

	private string m_Name;

	public PrefabID(PrefabBase prefab)
	{
		m_Type = ((object)prefab).GetType().Name;
		m_Name = ((Object)prefab).name;
	}

	public PrefabID(string type, string name)
	{
		m_Type = type;
		m_Name = name;
	}

	public bool Equals(PrefabID other)
	{
		if (m_Type.Equals(other.m_Type))
		{
			return m_Name.Equals(other.m_Name);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return m_Name.GetHashCode();
	}

	public override string ToString()
	{
		return $"{m_Type}:{m_Name}";
	}

	public string GetName()
	{
		return m_Name;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		string type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(type);
		string name = m_Name;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(name);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.newPrefabID)
		{
			string text = default(string);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref text);
			string[] array = text.Split(':', StringSplitOptions.None);
			m_Type = array[0];
			m_Name = array[1];
		}
		else
		{
			ref string type = ref m_Type;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
			ref string name = ref m_Name;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref name);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.staticObjectPrefab && m_Type == "ObjectGeometryPrefab")
		{
			m_Type = "StaticObjectPrefab";
		}
	}
}
