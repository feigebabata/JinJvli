using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGBBT : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var s = test();
        while (s.MoveNext())
        {
            Debug.Log(s.Current);
        }
    }

    IEnumerator test()
    {
        yield return 0;

        yield return 1;

        yield return 2;
    }
    
}
