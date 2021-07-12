using System;
using UnityEngine;

namespace GamePlay.StepGrid
{
    [CreateAssetMenu]
    public class StepGridConfig:ScriptableObject
    {
        [Header("席位配置")]
        public Seta[] Setas;
        
        [Header("格子大小")]
        public Vector2 GridSize = new Vector2(1,3);
        
        [Header("格子间隔")]
        public Vector2 Spacing = new Vector2(0.1f,0.2f);
        
        [Header("横向格子数")]
        public int GridGroupWidth = 4;
        
        [Header("移动加速度")]
        public float Acceleration=0.1f;
        
        [Header("初始速度")]
        public float StartSpeed=2;
        
        [Header("格子正常颜色")]
        public Color DefCol = Color.white;
        
        [Header("格子正确触发颜色")]
        public Color SelectCol = Color.green;
        
        [Header("格子异常触发颜色")]
        public Color ErrCol = Color.red;

    }

    [Serializable]
    public class Seta
    {
        public Color Color;
    }
}