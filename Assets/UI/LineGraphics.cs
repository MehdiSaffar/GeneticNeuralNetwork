using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LineGraphics : Graphic{
    public List<LineSegment> segments = new List<LineSegment>();

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var segmentCount = 0;
        foreach (var lineSegment in segments) {
            UIVertex[] segment = GetLineSegmentVertices(lineSegment);
            foreach (var vertex in segment) {
                vh.AddVert(vertex);
            }
            vh.AddTriangle(segmentCount + 0, segmentCount + 1, segmentCount + 2);
            vh.AddTriangle(segmentCount + 2, segmentCount + 3, segmentCount + 1);
            segmentCount+=4;
        }
    }

    private void LateUpdate(){
        SetAllDirty();
    }

    public UIVertex[] GetLineSegmentVertices(LineSegment segment){
        var start = segment.start;
        var end = segment.end;
        var color = segment.color;
        var thickness = segment.thickness;
        var vertices = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = UIVertex.simpleVert;
            vertices[i].color = color;
        }
        var direction = (end - start).normalized;
        var perp = new Vector2(-direction.y,direction.x ) * thickness;

        vertices[0].position = new Vector2(start.x + perp.x, start.y+perp.y);
        vertices[1].position = new Vector2(start.x - perp.x, start.y - perp.y);
        vertices[2].position = new Vector2(end.x + perp.x, end.y + perp.y);
        vertices[3].position = new Vector2(end.x - perp.x, end.y - perp.y);

        return vertices;
    }
}

[Serializable]
public class LineSegment{
    public Vector2 start;
    public Vector2 end;
    public float thickness;
    public Color color;
}
