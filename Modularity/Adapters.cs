/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Modularity
{
    public delegate void InputAdapter<T>(T item);
    public delegate T OutputAdapter<T>();
}


