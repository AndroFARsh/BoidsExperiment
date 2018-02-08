using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Example2
{
    public enum BoidSystemType
    {
        Physics,
        SpatialHash,
        ParalelSpatialHash,
    }

    public interface IConfig
    {
        int BoidNumber { get; }
        Transform BoidParent { get; }
        GameObject BoidPrefab { get; }
        float GoalRadius { get; }
        
        float MaxSpeed { get; }
        float MinSpeed { get; }
        
        float MaxFlockRadius { get; }
        float MinFlockRadius { get; }

        float DirectionFactor { get; }
        float CenterFactor { get; }
        float AvoidFactor { get; }
        
        float CellFactor { get; }
        BoidSystemType BoidSystemType { get; }

        int MaxFrameSplit { get; }
        
        bool Run { get; }
    }
    
    // World
    [Game, Unique]
    public struct WorldComponent : IComponent
    {}

    [Game]
    public struct ConfigComponent : IComponent
    {
        public IConfig value;
    }

    [Game]
    public struct GoalComponent : IComponent
    {
        public Vector3 value;
    }
    
    [Game]
    public struct FrameComponent : IComponent
    {
        public int value;
    }
    
    // Boids
    [Game]
    public struct BoidComponent : IComponent {}

    [Game]
    public struct IdComponent : IComponent
    {
        [PrimaryEntityIndex]
        public int value;
    }
    
    [Game]
    public struct SpatialHashesComponent : IComponent
    {
        public HashSet<int> value;
    }

    [Game]
    public struct PositionComponent : IComponent
    {
        public Vector3 value;
    }
    
    [Game]
    public struct RotationComponent : IComponent
    {
        public Quaternion value;
    }
    
    [Game]
    public struct SpeedComponent : IComponent
    {
        public float value;
    }
    
    [Game]
    public struct BoundsComponent : IComponent
    {
        public Bounds value;
    }
    
    [Game]
    public struct TransformComponent : IComponent
    {
        public Transform value;
    }
    
    [Game]
    public struct RendererComponent : IComponent
    {
        public Renderer value;
    }
}
