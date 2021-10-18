using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace FGUFW.Core
{
    public class BehaviorTreeWindow : EditorWindow
    {
        [MenuItem("Window/行为树窗口")]
        public static void OpenWindow()
        {
            var window = GetWindow<BehaviorTreeWindow>();
            window.titleContent = new GUIContent("行为树编辑器");
        }

        private List<Node> _nodes = new List<Node>();
        private GUIStyle NodeStyle;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            NodeStyle = new GUIStyle();
            NodeStyle.normal.background = Resources.Load<Texture2D>("nodeBG");
            NodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        void OnGUI()
        {
            DrawGrid(20,0.2f,Color.gray);
            DrawGrid(100,0.4f,Color.gray);

            drawNodes();
            processEvents(Event.current);
            if (GUI.changed) Repaint();
        }

        private void processEvents(Event current)
        {
            switch (current.type)
            {
                case EventType.MouseDown:
                {
                    if(current.button==1)
                    {
                        Debug.Log(current.mousePosition);
                        _nodes.Add(new Node(current.mousePosition,400,100,NodeStyle));
                    }
                }
                break;
            }
        }

        private void drawNodes()
        {
            foreach (var node in _nodes)
            {
                node.Draw();
            }
        }
        
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
            Vector2 offset = Vector2.zero,drag = Vector2.zero;
    
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
    
            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
    
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }
    
            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }
    
            Handles.color = Color.white;
            Handles.EndGUI();
        }

    }

    public class Node
    {
        public Rect Rect;
        public string Title="Node";
        public GUIStyle Style;

        public Node(Vector2 position,float width,float height,GUIStyle nodeStyle)
        {
            Rect = new Rect(position.x,position.y,width,height);
            Style = nodeStyle;
        }

        public void Draw()
        {
            GUI.Box(Rect,Title,Style);
            // GUI.Button(Rect,Title,Style);
        }
    }

}
