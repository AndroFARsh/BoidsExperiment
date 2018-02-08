namespace Example2.SpatialHash
{
    public class SpatialHashBoidsSystems : DisableableFeature
    {
        private readonly GameEntity world;
        public SpatialHashBoidsSystems(Contexts contexts, SpatialHashIndexer indexer)
        {
            world = contexts.game.worldEntity;
            
            Add(new BoidsUpdateBoundsSystem(contexts));
            Add(new UpdateSpatialHashsSystem(contexts));
            Add(new SpatialHashBoidsMoveSystem(contexts, indexer));
        }

        protected override bool isEnambled()
        {
            return world.config.value.Run && world.config.value.BoidSystemType == BoidSystemType.SpatialHash;
        }
    }
}