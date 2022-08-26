using System.Collections.Generic;
using FGUFW.Core;
using UnityEngine;

namespace FGUFW.FlowFieldPathfinding
{
    public class SpaceV2
    {
        public int GridsWidth{get;private set;}
        public int GridsHeight{get;private set;}

        public GridV2[] _grids;
        
        private SpaceV2(){}

        public SpaceV2(int width,int height)
        {
            GridsWidth = width;
            GridsHeight = height;
            _grids = new GridV2[width*height];
        }

        public void RandomObstacle()
        {
            int length = _grids.Length;
            for (int i = 0; i < length; i++)
            {
                _grids[i].Position = Index2Position(i,GridsWidth);
                if(Random.Range(0,100)<30)
                {
                    _grids[i].Distance = -2;
                }
                else
                {
                    _grids[i].Distance = -1;
                }
            }
        }

        static public Vector2Int Index2Position(int index,int width)
        {
            var pos = Vector2Int.zero;
            pos.x = index%width;
            pos.y = index/width;
            return pos;
        }

        static public int Position2Index(Vector2Int pos,int width)
        {
            var index = 0;
            index = pos.y*width+pos.x;
            return index;
        }

        public void Flow(Vector2Int start)
        {
            bool[] complete = new bool[_grids.Length];
            Queue<GridV2> queue = new Queue<GridV2>();
            int index = Position2Index(start,GridsWidth);
            
            _grids[index].Distance=0;
            queue.Enqueue(_grids[index]);
            while (queue.Count>0)
            {
                setValue(queue.Dequeue().Position,complete,queue);
            }
        }

        private void setValue(Vector2Int pos,bool[] complete,Queue<GridV2> queue)
        {
            int index = Position2Index(pos,GridsWidth);
            complete[index]=true;

            var up = pos;
            up.y+=1;
            if(exist(up))
            {
                var upIdx = Position2Index(up,GridsWidth);
                if(_grids[upIdx].Distance!=-2)
                {
                    if(_grids[upIdx].Distance==-1 || _grids[upIdx].Distance>_grids[index].Distance+1)
                    {
                        _grids[upIdx].Distance = _grids[index].Distance+1;
                    }
                    if(!complete[upIdx] && !queue.Contains(_grids[upIdx]))
                    {
                        queue.Enqueue(_grids[upIdx]);
                    }
                }
            }

            var down = pos;
            down.y-=1;
            if(exist(down))
            {
                var downIdx = Position2Index(down,GridsWidth);
                if(_grids[downIdx].Distance!=-2)
                {
                    if(_grids[downIdx].Distance==-1 || _grids[downIdx].Distance>_grids[index].Distance+1)
                    {
                        _grids[downIdx].Distance = _grids[index].Distance+1;
                    }
                    if(!complete[downIdx] && !queue.Contains(_grids[downIdx]))
                    {
                        queue.Enqueue(_grids[downIdx]);
                    }
                }
            }

            var left = pos;
            left.x-=1;
            if(exist(left))
            {
                var leftIdx = Position2Index(left,GridsWidth);
                if(_grids[leftIdx].Distance!=-2)
                {
                    if(_grids[leftIdx].Distance==-1 || _grids[leftIdx].Distance>_grids[index].Distance+1)
                    {
                        _grids[leftIdx].Distance = _grids[index].Distance+1;
                    }
                    if(!complete[leftIdx] && !queue.Contains(_grids[leftIdx]))
                    {
                        queue.Enqueue(_grids[leftIdx]);
                    }
                }
            }

            var right = pos;
            right.x+=1;
            if(exist(right))
            {
                var rightIdx = Position2Index(right,GridsWidth);
                if(_grids[rightIdx].Distance!=-2)
                {
                    if(_grids[rightIdx].Distance==-1 || _grids[rightIdx].Distance>_grids[index].Distance+1)
                    {
                        _grids[rightIdx].Distance = _grids[index].Distance+1;
                    }
                    if(!complete[rightIdx] && !queue.Contains(_grids[rightIdx]))
                    {
                        queue.Enqueue(_grids[rightIdx]);
                    }
                }
            }
        }

        private bool exist(Vector2Int pos)
        {
            return pos.x>-1 && pos.x<GridsWidth && pos.y>-1 && pos.y<GridsHeight;
        }


    }
}