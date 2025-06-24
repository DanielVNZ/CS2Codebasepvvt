using Unity.Entities;

namespace Game.Companies;

public struct CompanyTransportInstance : IComponentData, IQueryTypeParameter
{
	public Entity m_Company;
}
