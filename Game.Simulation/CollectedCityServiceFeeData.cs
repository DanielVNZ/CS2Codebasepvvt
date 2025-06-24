using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct CollectedCityServiceFeeData : IBufferElementData, ISerializable
{
	public int m_PlayerResource;

	public float m_Export;

	public float m_Import;

	public float m_Internal;

	public float m_ExportCount;

	public float m_ImportCount;

	public float m_InternalCount;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ref int playerResource = ref m_PlayerResource;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref playerResource);
		ref float export = ref m_Export;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref export);
		ref float import = ref m_Import;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref import);
		ref float reference = ref m_Internal;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.serviceFeeFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			int num3 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			m_ExportCount = num;
			m_ImportCount = num2;
			m_InternalCount = num3;
		}
		else
		{
			ref float exportCount = ref m_ExportCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref exportCount);
			ref float importCount = ref m_ImportCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref importCount);
			ref float internalCount = ref m_InternalCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref internalCount);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int playerResource = m_PlayerResource;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(playerResource);
		float export = m_Export;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(export);
		float import = m_Import;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(import);
		float num = m_Internal;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float exportCount = m_ExportCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(exportCount);
		float importCount = m_ImportCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(importCount);
		float internalCount = m_InternalCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(internalCount);
	}
}
