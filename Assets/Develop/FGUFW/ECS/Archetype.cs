using System.Collections.Generic;
using Unity.Collections;

namespace FGUFW.ECS
{
    public struct Archetype
    {
        public List<int> ComponentTypes;

        public Archetype(params int[] compTypes)
        {
            ComponentTypes = new List<int>(compTypes);
            ComponentTypes.Sort();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool Contains(int compType)
        {
            return ComponentTypes.Contains(compType);
        }

        public void Add(int compType)
        {
            if(!ComponentTypes.Contains(compType))
            {
                ComponentTypes.Add(compType);
                ComponentTypes.Sort();
            }
        }

        public void Remove(int compType)
        {
            ComponentTypes.Remove(compType);
            ComponentTypes.Sort();
        }

        public int Length => ComponentTypes.Count;

        static public bool operator == (Archetype a1,Archetype a2)
        {
            bool b = false;
            if(a1.ComponentTypes.Count==a2.ComponentTypes.Count)
            {
                b = true;
                int length = a1.ComponentTypes.Count;
                for (int i = 0; i < length; i++)
                {
                    if(a1.ComponentTypes[i]!=a2.ComponentTypes[i])
                    {
                        return false;
                    }
                }
            }
            return b;
        }

        static public bool operator != (Archetype a1,Archetype a2)
        {
            bool b = true;
            if(a1.ComponentTypes.Count==a2.ComponentTypes.Count)
            {
                b = false;
                int length = a1.ComponentTypes.Count;
                for (int i = 0; i < length; i++)
                {
                    if(a1.ComponentTypes[i]!=a2.ComponentTypes[i])
                    {
                        return true;
                    }
                }
            }
            return b;
        }


    }

}