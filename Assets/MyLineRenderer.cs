

//namespace DefaultNamespace{
//    [RequireComponent(typeof(RectTransform))]
//    public class MyLineRenderer : MonoBehaviour {
////        public List<LineSegment> segments;
////
////		private UIVertex[] CreateLineSegment(LineSegment segment){
////			var start = segment.start;
////			var end = segment.end;
////
////			float lineThickness = segment.thickness;
////			var offset = new Vector2(start.y - end.y, end.x - start.x).normalized * lineThickness / 2;
////
////			var v1 = start - offset;
////			var v2 = start + offset;
////			var v3 = end + offset;
////			var v4 = end - offset;
////            //Return the VDO with the correct uvs
////            return SetVbo(new[] { v1, v2, v3, v4 }, segment.color);
////		}
////
////
////        private UIVertex[] SetVbo(Vector2[] vertices, Color color)
////        {
////            var vbo = new UIVertex[4];
////            for (var i = 0; i < vertices.Length; i++)
////            {
////                var vert = UIVertex.simpleVert;
////                vert.color = color;
////                vert.position = vertices[i];
////                vbo[i] = vert;
////            }
////            return vbo;
////        }
////    }
////
////	public class LineSegment{
////		public Vector2 start;
////		public Vector2 end;
////		public Color color;
////		public float thickness;
////
////	}
//
//}