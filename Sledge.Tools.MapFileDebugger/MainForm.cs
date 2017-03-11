using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Providers.Map;
using Sledge.DataStructures.MapObjects;
using Path = System.IO.Path;

namespace Sledge.Tools.MapFileDebugger
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            MapProvider.Register(new RmfProvider());
        }

        private void OpenToolStripButtonClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = @"Map Files (*.rmf, *.vmf, *.map)|*.rmf;*.vmf;*.map;*.rmx;*.vmx;*.max";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var map = MapProvider.GetMapFromFile(ofd.FileName);
                    lstLog.Items.Insert(0, Path.GetFileName(ofd.FileName) + ": Map Loaded.");
                    LoadMap(map);
                }
                catch(Exception ex)
                {
                    lstLog.Items.Insert(0, Path.GetFileName(ofd.FileName) + ": " + ex.Message);
                }
            }
        }

        private void LoadMap(Map map)
        {
            treeMap.Nodes.Clear();
            LoadMapNode(null, map.WorldSpawn);
        }

        private void LoadMapNode(TreeNode parent, MapObject obj)
        {
            var node = new TreeNode(obj.GetType().Name);
            if (obj is World)
            {
                var w = (World) obj;
                node.Nodes.Add(GetEntityNode(w.EntityData));
            }
            else if (obj is Entity)
            {
                var e = (Entity) obj;
                node.Nodes.Add(GetEntityNode(e.EntityData));
            }
            else if (obj is Solid)
            {
                var s = (Solid) obj;
                node.Nodes.Add(GetFacesNode(s.Faces));
            }
            foreach (var mo in obj.GetChildren())
            {
                LoadMapNode(node, mo);
            }
            if (parent == null) treeMap.Nodes.Add(node);
            else parent.Nodes.Add(node);
        }

        private TreeNode GetEntityNode(EntityData data)
        {
            var node = new TreeNode("Entity: " + data.Name);
            var pnode = node.Nodes.Add("Properties: " + data.Properties.Count);
            foreach (var p in data.Properties)
            {
                pnode.Nodes.Add(p.Key + " = " + p.Value);
            }
            if (data.Outputs.Count > 0)
            {
                var onode = node.Nodes.Add("Outputs: " + data.Outputs);
                foreach (var o in data.Outputs)
                {
                    onode.Nodes.Add(o.Name + " > " + o.Target);
                }
            }
            var fnode = node.Nodes.Add("Flags: " + data.Flags);
            return node;
        }

        private TreeNode GetFacesNode(IEnumerable<Face> faces)
        {
            var node = new TreeNode("Faces: " + faces.Count());
            var c = 0;
            foreach (var face in faces)
            {
                var fnode = node.Nodes.Add("Face " + c);
                c++;
                var pnode = fnode.Nodes.Add("Plane: " + face.Plane.Normal + " * " + face.Plane.DistanceFromOrigin);
                pnode.Nodes.Add("Normal: " + face.Plane.Normal);
                pnode.Nodes.Add("Distance: " + face.Plane.DistanceFromOrigin);
                pnode.Nodes.Add("A: " + face.Plane.A);
                pnode.Nodes.Add("B: " + face.Plane.B);
                pnode.Nodes.Add("C: " + face.Plane.C);
                pnode.Nodes.Add("D: " + face.Plane.D);
                var tnode = fnode.Nodes.Add("Texture: " + face.Texture.Name);
                tnode.Nodes.Add("U Axis: " + face.Texture.UAxis);
                tnode.Nodes.Add("V Axis: " + face.Texture.VAxis);
                tnode.Nodes.Add(String.Format("Scale: X = {0}, Y = {1}", face.Texture.XScale, face.Texture.YScale));
                tnode.Nodes.Add(String.Format("Offset: X = {0}, Y = {1}", face.Texture.XShift, face.Texture.YShift));
                tnode.Nodes.Add("Rotation: " + face.Texture.Rotation);
                var vnode = fnode.Nodes.Add("Vertices: " + face.Vertices.Count);
                var d = 0;
                foreach (var vertex in face.Vertices)
                {
                    var cnode = vnode.Nodes.Add("Vertex " + d + ": " + vertex.Location);
                    d++;
                }
            }
            return node;
        }
    }
}
