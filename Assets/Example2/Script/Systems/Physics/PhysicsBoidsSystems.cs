namespace Example2.UnityPhysics
{
    public class PhysicsBoidsMoveSystems : DisableableFeature
    {
        private readonly GameEntity world;
        public PhysicsBoidsMoveSystems(Contexts contexts, int layerMask)
        {
            world = contexts.game.worldEntity;
            
            Add(new PhysicsBoidsMoveSystem(contexts, layerMask));
        }

        protected override bool isEnambled()
        {
            return world.config.value.Run && world.config.value.BoidSystemType == BoidSystemType.Physics;
        }
    }
}