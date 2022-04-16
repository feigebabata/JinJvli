using Unity.Collections;

namespace FGUFW.Core
{
    static public class NativeArrayHelper
    {
        static public int IndexOf<T>(this NativeArray<T> array,T val) where T:struct
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                if(array[i].Equals(val))return i;
            }
            return -1;
        }
    }

}
