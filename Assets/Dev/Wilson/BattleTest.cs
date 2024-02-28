using Game.Battle;
using Game.Battle.Entities;
using UnityEngine;

namespace Dev.Wilson
{
    public class BattleTest : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _entity1;

        [SerializeField]
        private float _radius1;

        [SerializeField]
        private Vector2 _entity2;
        [SerializeField]
        private float _radius2;

        private BattleEntity _battleEntity1;
        private BattleEntity _battleEntity2;

        private EntityManager _entityManager;

        private void Awake()
        {
            _entityManager = new EntityManager();

            _battleEntity1 = new BattleEntity()
            {
                Name = "BattleEntity 1"
            };
            _battleEntity1.BattleComponent.BattleRadius = _radius1;
            _battleEntity1.TransformComponent.PosX = (int)_entity1.x;
            _battleEntity1.TransformComponent.PosY = (int)_entity1.y;
            
            _battleEntity2 = new BattleEntity()
            {
                Name = "BattleEntity 2"
            };
            _battleEntity2.BattleComponent.BattleRadius = _radius2;
            _battleEntity2.TransformComponent.PosX      = (int)_entity2.x;
            _battleEntity2.TransformComponent.PosY      = (int)_entity2.y;
            
            _entityManager.TryAddEntity(_battleEntity1);
            _entityManager.TryAddEntity(_battleEntity2);
        }

        private void Update()
        {
            _battleEntity1.BattleComponent.BattleRadius = _radius1;
            _battleEntity1.TransformComponent.PosX      = (int)_entity1.x;
            _battleEntity1.TransformComponent.PosY      = (int)_entity1.y;
            
            _battleEntity2.BattleComponent.BattleRadius = _radius2;
            _battleEntity2.TransformComponent.PosX      = (int)_entity2.x;
            _battleEntity2.TransformComponent.PosY      = (int)_entity2.y;
            
            _entityManager.Update();
        }
    }
}