using Example2;

namespace Example2.ParalelSpatialHash
{
    public class ParalelSpatialHashBoidsSystems : DisableableFeature
    {
        private readonly GameEntity world;

        public ParalelSpatialHashBoidsSystems(Contexts contexts, SpatialHashIndexer indexer)
        {
            world = contexts.game.worldEntity;

            Add(new BoidsUpdateBoundsSystem(contexts));
            Add(new ParalelUpdateSpatialHashsSystem(contexts));
            Add(new ParalelSpatialHashBoidsMoveSystem(contexts, indexer));
        }

        protected override bool isEnambled()
        {
            return world.config.value.Run && world.config.value.BoidSystemType == BoidSystemType.ParalelSpatialHash;
        }
    }
}