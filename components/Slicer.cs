using Godot;
using System;

public partial class Slicer : MeshInstance3D
{
    [Export]
    public MeshInstance3D Obj { get; set; }
    [Export]
    public Material CrossSectionMaterial { get; set; }

    public override void _Ready()
    {
        var meshes = SliceMesh();

        var mesh1 = (MeshInstance3D)Obj.Duplicate();
        mesh1.Mesh = meshes[0];
        mesh1.Name = "Fist Half";
        mesh1.Position += (Vector3.Down + Vector3.Right + Vector3.Back) / 4f;

        var mesh2 = (MeshInstance3D)Obj.Duplicate();
        mesh2.Mesh = meshes[1];
        mesh2.Name = "Second Half";
        mesh2.Position += (Vector3.Up + Vector3.Left + Vector3.Forward) / 4f;

        GetParent().CallDeferred(MethodName.RemoveChild, Obj);
        GetParent().CallDeferred(MethodName.AddChild, mesh1);
        GetParent().CallDeferred(MethodName.AddChild, mesh2);

    }

    public Mesh[] SliceMesh(Material crossSectionMaterial = null)
    {
        CsgCombiner3D combiner = new()
        {
            Name = "Combiner",
            TopLevel = true
        };
        CsgMesh3D objCsg = new()
        {
            Name = "Obj Clone",
            Mesh = Obj.Mesh
        };

        CsgMesh3D slicerCsg = new()
        {
            Name = "Slicer Box",
            Mesh = new BoxMesh()
            {
                Material = CrossSectionMaterial
            },
            Transform = this.Transform
        };

        AddChild(combiner);
        combiner.AddChild(objCsg);
        combiner.AddChild(slicerCsg);

        var maxAt = -Vector3.Inf;
        var minAt = Vector3.Inf;
        foreach (var v in objCsg.Mesh.GetFaces())
        {
            var lv = slicerCsg.ToLocal(v); // i understand it, i cannot explain it, good luck future me
            maxAt = maxAt.Max(lv);
            minAt = minAt.Min(lv);
        }
        minAt.Y = 0;
        slicerCsg.Position = slicerCsg.ToGlobal((maxAt + minAt) / 2f); // i understand it, i cannot explain it, good luck future me
        (slicerCsg.Mesh as BoxMesh).Size = maxAt - minAt;

        Mesh outMesh = null, outMesh2 = null;

        slicerCsg.Operation = CsgShape3D.OperationEnum.Subtraction;
        combiner.Call("_update_shape");
        var meshes = combiner.GetMeshes();
        if (meshes is not null) outMesh = (Mesh)meshes[1];

        slicerCsg.Operation = CsgShape3D.OperationEnum.Intersection;
        combiner.Call("_update_shape");
        meshes = combiner.GetMeshes();
        if (meshes is not null) outMesh2 = (Mesh)meshes[1];

        combiner.Free();

        return [outMesh, outMesh2];
    }
}
