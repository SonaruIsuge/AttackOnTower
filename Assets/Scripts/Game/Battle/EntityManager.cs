using System.Collections.Generic;
using Framework.Common;
using Game.Battle.Entities;
using Game.Battle.Systems;

namespace Game.Battle
{
    public class EntityManager
    {
        #region - Entity Operation -

        private uint _entitySerialId = 0;

        private Dictionary<uint, EntityBase> _entitiesById = new();

        public bool TryAddEntity(EntityBase entity)
        {
            if (_entitiesById.ContainsKey(entity.Id))
            {
                Log.Warn($"新增Entity失敗: Entity({entity.Id.ToString()}已存在)");
                return false;
            }

            ++_entitySerialId;
            entity.Id = _entitySerialId;
            return _entitiesById.TryAdd(_entitySerialId, entity);
        }

        public bool TryGetEntity(uint id, out EntityBase outEntity)
        {
            return _entitiesById.TryGetValue(id, out outEntity);
        }

        public bool TryRemoveEntity(uint id)
        {
            return _entitiesById.ContainsKey(id) && _entitiesById.Remove(id);
        }

        #endregion
        
        private BattleSystem _battleSystem;

        public EntityManager()
        {
            _battleSystem = new BattleSystem();
        }
        
        public void Update()
        {
            _battleSystem.Update(_entitiesById);
        }
    }
}