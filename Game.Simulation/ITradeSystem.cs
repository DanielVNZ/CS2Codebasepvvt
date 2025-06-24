using Game.City;
using Game.Economy;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Simulation;

public interface ITradeSystem
{
	float GetTradePrice(Resource resource, OutsideConnectionTransferType type, bool import, DynamicBuffer<CityModifier> cityEffects);
}
