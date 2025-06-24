using Game.City;
using Unity.Entities;

namespace Game.Prefabs;

public struct VehicleSelectRequirementData
{
	public struct Chunk
	{
		internal EnabledMask m_LockedMask;

		internal BufferAccessor<ObjectRequirementElement> m_ObjectRequirements;
	}

	private ComponentTypeHandle<Locked> m_LockedType;

	private BufferTypeHandle<ObjectRequirementElement> m_ObjectRequirementType;

	private ComponentLookup<ThemeData> m_ThemeData;

	private Entity m_DefaultTheme;

	public VehicleSelectRequirementData(SystemBase system)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		m_LockedType = ((ComponentSystemBase)system).GetComponentTypeHandle<Locked>(true);
		m_ObjectRequirementType = ((ComponentSystemBase)system).GetBufferTypeHandle<ObjectRequirementElement>(true);
		m_ThemeData = system.GetComponentLookup<ThemeData>(true);
		m_DefaultTheme = default(Entity);
	}

	public void Update(SystemBase system, CityConfigurationSystem cityConfigurationSystem)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		m_LockedType.Update(system);
		m_ObjectRequirementType.Update(system);
		m_ThemeData.Update(system);
		m_DefaultTheme = cityConfigurationSystem.defaultTheme;
	}

	public Chunk GetChunk(ArchetypeChunk chunk)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return new Chunk
		{
			m_LockedMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<Locked>(ref m_LockedType),
			m_ObjectRequirements = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ObjectRequirementElement>(ref m_ObjectRequirementType)
		};
	}

	public bool CheckRequirements(ref Chunk chunk, int index, bool ignoreTheme = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		SafeBitRef enableBit = ((EnabledMask)(ref chunk.m_LockedMask)).EnableBit;
		if (((SafeBitRef)(ref enableBit)).IsValid && ((EnabledMask)(ref chunk.m_LockedMask))[index])
		{
			return false;
		}
		if (chunk.m_ObjectRequirements.Length != 0)
		{
			DynamicBuffer<ObjectRequirementElement> val = chunk.m_ObjectRequirements[index];
			int num = -1;
			bool flag = true;
			for (int i = 0; i < val.Length; i++)
			{
				ObjectRequirementElement objectRequirementElement = val[i];
				if (objectRequirementElement.m_Group != num)
				{
					if (!flag)
					{
						break;
					}
					num = objectRequirementElement.m_Group;
					flag = false;
				}
				flag |= m_DefaultTheme == objectRequirementElement.m_Requirement || (ignoreTheme && m_ThemeData.HasComponent(objectRequirementElement.m_Requirement));
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
