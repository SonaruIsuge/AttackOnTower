using System.Collections.Generic;
using Framework.Common;
using Game.Battle.Components;
using Game.Battle.Entities;

namespace Game.Battle.Systems
{
    public class BattleSystem
    {
        public void Update(Dictionary<uint, EntityBase> entitiesById)
        {
            foreach (var pair in entitiesById)
            {
                var id     = pair.Key;
                var entity = pair.Value;

                if (!entity.TryGetComponent<BattleComponent>(out var battleComponent))
                {
                    continue;
                }

                if (!entity.TryGetComponent<TransformComponent>(out var transformComponent))
                {
                    continue;
                }

                // 先用最笨的O(n*n)
                var targetEntity = FindTargetEntity(id, transformComponent, battleComponent.BattleRadius, entitiesById);
                if (targetEntity != null)
                {
                    Log.Info($"Entity: {entity.Name} is attacking Entity: {targetEntity.Name}");
                }
            }
        }

        private EntityBase FindTargetEntity(uint selfId, TransformComponent selfTransform, float radius,
            Dictionary<uint, EntityBase>                       entitiesById)
        {
            foreach (var pair in entitiesById)
            {
                var id     = pair.Key;
                var entity = pair.Value;

                if (id == selfId)
                {
                    continue;
                }

                if (!entity.TryGetComponent<TransformComponent>(out var transformComponent))
                {
                    continue;
                }

                if (!entity.TryGetComponent<HealthComponent>(out var healthComponent))
                {
                    continue;
                }

                if (IsInRange(selfTransform, transformComponent, radius))
                {
                    return entity;
                }
            }

            return null;
        }

        private bool IsInRange(TransformComponent trans1, TransformComponent trans2, float radius)
        {
            var deltaX = trans1.PosX - trans2.PosX;
            var deltaY = trans1.PosY - trans2.PosY;

            return (deltaX * deltaX + deltaY * deltaY) <= radius * radius;
        }
    }
}