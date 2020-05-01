using System;

public static class CommonExtensions {
    public static void NullSafe( this Action action ) => action?.Invoke();
    public static void NullSafe<T>( this Action<T> action, T arg ) => action?.Invoke( arg );

    public static TResult NullSafe<TResult>( this Func<TResult> func ) {
        return func != null ? func() : default( TResult );
    }

    public static TResult NullSafe<T, TResult>( this Func<T, TResult> func, T arg ) {
        return func != null ? func( arg ) : default( TResult );
    }
}
