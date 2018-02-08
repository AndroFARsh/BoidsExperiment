//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Example2.GoalComponent goal { get { return (Example2.GoalComponent)GetComponent(GameComponentsLookup.Goal); } }
    public bool hasGoal { get { return HasComponent(GameComponentsLookup.Goal); } }

    public void AddGoal(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.Goal;
        var component = CreateComponent<Example2.GoalComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceGoal(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.Goal;
        var component = CreateComponent<Example2.GoalComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveGoal() {
        RemoveComponent(GameComponentsLookup.Goal);
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

    static Entitas.IMatcher<GameEntity> _matcherGoal;

    public static Entitas.IMatcher<GameEntity> Goal {
        get {
            if (_matcherGoal == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Goal);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGoal = matcher;
            }

            return _matcherGoal;
        }
    }
}
