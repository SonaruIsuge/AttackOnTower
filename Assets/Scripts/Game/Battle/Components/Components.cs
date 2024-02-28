using UnityEngine;

namespace Game.Battle.Components
{
    public interface IComponent
    {
    }

    public class TransformComponent : IComponent
    {
        public int     PosX      { get; set; }
        public int     PosY      { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class BattleComponent : IComponent
    {
        public float Power        { get; set; }
        public float Cd           { get; set; }
        public float BattleRadius { get; set; }
    }

    public class HealthComponent : IComponent
    {
        public float Hp { get; private set; }
    }
}