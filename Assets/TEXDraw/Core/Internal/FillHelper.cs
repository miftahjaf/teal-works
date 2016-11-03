using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TexDrawLib
{
    public class FillHelper
    {
        public List<Vector3> m_Positions = ListPool<Vector3>.Get();
        public List<Color32> m_Colors = ListPool<Color32>.Get();
        public List<Vector2> m_Uv0S = ListPool<Vector2>.Get();
        public List<Vector2> m_Uv1S = ListPool<Vector2>.Get();
        public List<Vector3> m_Normals = ListPool<Vector3>.Get();
        public List<Vector4> m_Tangents = ListPool<Vector4>.Get();
        public List<int> m_Indicies = ListPool<int>.Get();

        public static readonly Vector4 s_DefaultTangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
        public static readonly Vector3 s_DefaultNormal = Vector3.back;
        public static readonly Vector2 s_ZeroVector = Vector2.zero;

        public FillHelper()
        {
        }

        public FillHelper(Mesh m)
        {
            m_Positions.AddRange(m.vertices);
            m_Colors.AddRange(m.colors32);
            m_Uv0S.AddRange(m.uv);
            m_Uv1S.AddRange(m.uv2);
            m_Normals.AddRange(m.normals);
            m_Tangents.AddRange(m.tangents);
            m_Indicies.AddRange(m.GetIndices(0));
        }

        public void Clear()
        {
            m_Positions.Clear();
            m_Colors.Clear();
            m_Uv0S.Clear();
            m_Uv1S.Clear();
            m_Normals.Clear();
            m_Tangents.Clear();
            m_Indicies.Clear();
        }

        public int currentVertCount
        {
            get { return m_Positions.Count; }
        }

        public int currentIndexCount
        {
            get { return m_Indicies.Count; }
        }

        public void PopulateUIVertex(ref UIVertex vertex, int i)
        {
            vertex.position = m_Positions[i];
            vertex.color = m_Colors[i];
            vertex.uv0 = m_Uv0S[i];
            vertex.uv1 = m_Uv1S[i];
            vertex.normal = m_Normals[i];
            vertex.tangent = m_Tangents[i];
        }

        public void SetUIVertex(UIVertex vertex, int i)
        {
            m_Positions[i] = vertex.position;
            m_Colors[i] = vertex.color;
            m_Uv0S[i] = vertex.uv0;
            m_Uv1S[i] = vertex.uv1;
            m_Normals[i] = vertex.normal;
            m_Tangents[i] = vertex.tangent;
        }

        public void FillMesh(Mesh mesh)
        {
            mesh.Clear();

            if (m_Positions.Count >= 65000)
                throw new System.ArgumentException("Mesh can not have more than 65000 verticies");

            mesh.SetVertices(m_Positions);
            mesh.SetColors(m_Colors);
            mesh.SetUVs(0, m_Uv0S);
            mesh.SetUVs(1, m_Uv1S);
            mesh.SetNormals(m_Normals);
            mesh.SetTangents(m_Tangents);
            mesh.SetTriangles(m_Indicies, 0);
            mesh.RecalculateBounds();
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector3 normal, Vector4 tangent)
        {
            m_Positions.Add(position);
            m_Colors.Add(color);
            m_Uv0S.Add(uv0);
            m_Uv1S.Add(uv1);
            m_Normals.Add(normal);
            m_Tangents.Add(tangent);
            //UV3 is manual, see below
        }

        public void SetUV3(Vector2 uv, int idx)
        {
            //CanvasRenderer can't use UV3, so... we chose tangent as subtitute.
            //What choice do we have? this means that normal can never get it work if UV3 is used..
            //But who cares with normal map for UI, anyway?
	        m_Tangents[idx] = new Vector4(uv.x, uv.y);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            AddVert(position, color, uv0, uv1, uv2, s_DefaultNormal, s_DefaultTangent);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1)
        {
            AddVert(position, color, uv0, uv1, s_ZeroVector, s_DefaultNormal, s_DefaultTangent);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0)
        {
            AddVert(position, color, uv0, s_ZeroVector, s_ZeroVector, s_DefaultNormal, s_DefaultTangent);
        }

        public void AddVert(UIVertex v)
        {
            AddVert(v.position, v.color, v.uv0, v.uv1, s_ZeroVector, v.normal, v.tangent);
        }

        public void AddTriangle(int idx0, int idx1, int idx2)
        {
            m_Indicies.Add(idx0);
            m_Indicies.Add(idx1);
            m_Indicies.Add(idx2);
        }
    }
}