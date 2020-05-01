using LunarConsolePlugin;

[CVarContainer]
public static class LunarConsoleModel {
    public static readonly CVar TwitterScheme = new CVar( "TwitterScheme", "twitter://" );
    public static readonly CVar TwitterMsg = new CVar( "TwitterMsg", "msg" );
}
