using System;
using UnityEngine;

namespace FGUFW.Core
{
    public class AssetLoadHandel
    {
        public string Location{get;private set;}
        public Action<UnityEngine.Object> Completed;

        public AssetLoadHandel(string location)
        {
            Location = location;
        }
    }

    public class SceneLoadHandel
    {
        public string Location{get;private set;}
        public Action Completed;

        public SceneLoadHandel(string location)
        {
            Location = location;
        }
    }
    public class PrefabLoadHandel
    {
        public string Location{get;private set;}
        public Action<GameObject> Completed;
        public Transform Parent{get;private set;}

        public PrefabLoadHandel(string location,Transform parent=null)
        {
            Location = location;
            Parent = parent;
        }
    }

}