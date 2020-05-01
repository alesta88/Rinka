using System.Linq;
using System.Collections.Generic;

public static class LinqExtensions {
    public static T Random<T>( this IList<T> list ) {
        if( list == null || !list.Any() )
            return default( T );

        return list[UnityEngine.Random.Range( 0, list.Count )];
    }
}
