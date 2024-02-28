using System;
using System.Collections.Generic;
using Framework.Common;
using Game.Battle.Components;

namespace Game.Battle.Entities
{
    public abstract class EntityBase
    {
        public uint   Id   { get; set; }
        public string Name { get; set; }

        private Dictionary<Type, IComponent> _componentsByType = new();

        public bool TryAddComponent<T>(T component) where T : IComponent
        {
            if (_componentsByType.ContainsKey(typeof(T)))
            {
                return false;
            }

            _componentsByType.Add(typeof(T), component);
            return true;
        }

        public bool TryGetComponent<T>(out T outComponent) where T : IComponent
        {
            outComponent = default(T);
            if (_componentsByType.TryGetValue(typeof(T), out var component))
            {
                outComponent = (T)component;
                return true;
            }

            return false;
        }

        public bool TryRemoveComponent<T>() where T : IComponent
        {
            if (!_componentsByType.ContainsKey(typeof(T)))
            {
                return false;
            }

            _componentsByType.Remove(typeof(T));
            return true;
        }
    }

    public class BattleEntity : EntityBase
    {
        public TransformComponent TransformComponent => _transformComponent;

        public BattleComponent BattleComponent => _battleComponent;

        public HealthComponent HealthComponent => _healthComponent;
        
        private TransformComponent _transformComponent;
        private BattleComponent    _battleComponent;
        private HealthComponent    _healthComponent;
        
        public BattleEntity()
        {
            _transformComponent = new TransformComponent();
            _battleComponent    = new BattleComponent();
            _healthComponent    = new HealthComponent();
            
            if (!TryAddComponent(_transformComponent))
            {
                Log.Error($"新增TransformComponent失敗");
            }
            
            if (!TryAddComponent(_battleComponent))
            {
                Log.Error($"新增BattleComponent失敗");
            }

            if (!TryAddComponent(_healthComponent))
            {
                Log.Error($"新增HealthComponent失敗");
            }
        }
    }
}