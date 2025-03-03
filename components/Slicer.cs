using Godot;
using System;

public partial class Slicer : MeshInstance3D 
{
    [Export]
    public MeshInstance3D Obj { get; set; }

    public override void _Ready()
    {
        var meshInstance = Obj;
        var transform = GlobalTransform;

        transform.Origin = meshInstance.ToLocal(transform.Origin);
        transform.Basis.X = meshInstance.ToLocal(transform.Basis.X + meshInstance.GlobalPosition);
        transform.Basis.Y = meshInstance.ToLocal(transform.Basis.Y + meshInstance.GlobalPosition);
        transform.Basis.Z = meshInstance.ToLocal(transform.Basis.Z + meshInstance.GlobalPosition);

        var meshes = SliceMesh(transform, meshInstance.Mesh);
        // meshInstance.Mesh = meshes[0];
    }

    public Mesh[] SliceMesh(Transform3D sliceTransform, Mesh mesh, Material crossSectionMaterial = null)
    {
        CsgCombiner3D combiner = new();
        CsgMesh3D objCsg = new()
        {   
            Name = "MAIN MESH",
            Mesh = mesh
        };
        CsgMesh3D slicerCsg = new()
        {
            Mesh = new BoxMesh()
        };
        (slicerCsg.Mesh as BoxMesh).Material = crossSectionMaterial;

        AddChild(combiner);
        combiner.AddChild(objCsg);
        combiner.AddChild(slicerCsg);
        slicerCsg.Transform = sliceTransform;

        var maxAt = -Vector3.Inf;
        var minAt = Vector3.Inf;
        foreach (var v in mesh.GetFaces())
        {
            var lv = slicerCsg.ToLocal(v);
            maxAt = maxAt.Max(lv);
            minAt = minAt.Min(lv);
            // GD.Print(v);
        }
        minAt.Z = 0;
        slicerCsg.Position = slicerCsg.ToGlobal((maxAt + minAt) / 2f);
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
