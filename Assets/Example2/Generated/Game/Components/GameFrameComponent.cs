//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Example2.FrameComponent frame { get { return (Example2.FrameComponent)GetComponent(GameComponentsLookup.Frame); } }
    public bool hasFrame { get { return HasComponent(GameComponentsLookup.Frame); } }

    public void AddFrame(int newValue) {
        var index = GameComponentsLookup.Frame;
        var component = CreateComponent<Example2.FrameComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceFrame(int newValue) {
        var index = GameComponentsLookup.Frame;
        var component = CreateComponent<Example2.FrameComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveFrame() {
        RemoveComponent(GameComponentsLookup.Frame);
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

    static Entitas.IMatcher<GameEntity> _matcherFrame;

    public static Entitas.IMatcher<GameEntity> Frame {
        get {
            if (_matcherFrame == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Frame);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherFrame = matcher;
            }

            return _matcherFrame;
        }
    }
}
