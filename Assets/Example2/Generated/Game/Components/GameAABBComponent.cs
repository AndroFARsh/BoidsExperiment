//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Example2.AABBComponent aABB { get { return (Example2.AABBComponent)GetComponent(GameComponentsLookup.AABB); } }
    public bool hasAABB { get { return HasComponent(GameComponentsLookup.AABB); } }

    public void AddAABB(UnityEngine.Bounds newValue) {
        var index = GameComponentsLookup.AABB;
        var component = CreateComponent<Example2.AABBComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceAABB(UnityEngine.Bounds newValue) {
        var index = GameComponentsLookup.AABB;
        var component = CreateComponent<Example2.AABBComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveAABB() {
        RemoveComponent(GameComponentsLookup.AABB);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherAABB;

    public static Entitas.IMatcher<GameEntity> AABB {
        get {
            if (_matcherAABB == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AABB);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAABB = matcher;
            }

            return _matcherAABB;
        }
    }
}