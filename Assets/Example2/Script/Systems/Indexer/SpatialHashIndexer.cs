using System.Collections.Generic;
using System.Linq;
using Entitas;
using Smooth.Pools;

namespace Example2
{
    public class SpatialHashIndexer
    {
        private readonly Dictionary<GameEntity, int> retain = new Dictionary<GameEntity, int>();
        private readonly Dictionary<int, HashSet<GameEntity>> lookup = new Dictionary<int, HashSet<GameEntity>>(1024);
        private readonly IGroup<GameEntity> observedCollection;
        
        public SpatialHashIndexer(Contexts contexts)
        {
            observedCollection = contexts.game.GetGroup(GameMatcher.SpatialHashes);
            observedCollection.OnEntityAdded += AddEntity;
            observedCollection.OnEntityRemoved += RemoveEntity;
        }

        public IEnumerable<GameEntity> FindGameEntitiesForHash(int hash)
        {
            return lookup.ContainsKey(hash) ? lookup[hash] : Enumerable.Empty<GameEntity>();
        }

        private void AddEntity(IGroup<GameEntity> collection, GameEntity entity, int index, IComponent component)
        {
            if (!(component is SpatialHashesComponent)) return;

            var en = ((SpatialHashesComponent) component).value.GetEnumerator() ;
            while (en.MoveNext())
            {
                var hash = en.Current;
                var set = lookup.ContainsKey(hash)
                    ? lookup[hash]
                    : lookup[hash] = HashSetPool<GameEntity>.Instance.Borrow();

                if (set.Add(entity))
                {
                    if (!retain.ContainsKey(entity))
                    {
                        retain[entity] = 1;
                        entity.Retain(this);
                    }
                    else
                    {
                        retain[entity] = retain[entity] + 1;
                    }
                }

            }
            en.Dispose();
        }
        
        private void RemoveEntity(IGroup<GameEntity> collection, GameEntity entity, int index, IComponent component)
        {
            if (!(component is SpatialHashesComponent)) return;
            
            var en = ((SpatialHashesComponent) component).value.GetEnumerator() ;
            while (en.MoveNext())
            {
                var hash = en.Current;
                if (lookup.ContainsKey(hash))
                {
                    var set = lookup[hash];
                    if (set.Remove(entity))
                    {
                        var retained = retain[entity] - 1;
                        if (retained > 0)
                        {
                            retain[entity] = retained;
                        }
                        else
                        {
                            retain.Remove(entity);
                            entity.Release(this);
                        }
                    }

                    if (set.Count == 0)
                    {
                        lookup.Remove(hash);
                        HashSetPool<GameEntity>.Instance.Release(set);
                    }
                }
                    
            }
            en.Dispose();
        }
    }
}