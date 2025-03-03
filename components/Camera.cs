using Godot;
using System;

public partial class Camera : Node3D {
	[Export]
	public float HSensitivity { get; set; } = .1f;
	[Export]
	public float VSensitivity { get; set; } = .1f;

	public Camera3D Cam { get; set; }
	public Node3D HGimbal { get; set; }
	public Node3D VGimbal { get; set; }

	public float HCamRotation { get; set; }
	public float VCamRotation { get; set; }
	private readonly float _vCamMin = Mathf.DegToRad(-55f);
	private readonly float _vCamMax = Mathf.DegToRad(75f);
	
	public override void _Ready() {
		// Input.MouseMode = Input.MouseModeEnum.Captured;

		Cam = GetNode<Camera3D>("H/V/Camera3D");
		HGimbal = GetNode<Node3D>("H");
		VGimbal = GetNode<Node3D>("H/V");

		HCamRotation = HGimbal.Rotation.Y;
		VCamRotation = VGimbal.Rotation.X;
	}

	public override void _Process(double delta) {
		HGimbal.Rotation = new Vector3(0, HCamRotation, 0);

		VCamRotation = Mathf.Clamp(VCamRotation, _vCamMin, _vCamMax);
		VGimbal.Rotation = new Vector3(VCamRotation, 0, 0);

		Cam.Position = new() {
			X = 0,
			Y = Cam.Position.Y,
			Z = Mathf.Clamp(Cam.Position.Z, 2, 10)
		};
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motion && Input.IsActionPressed("move_cam")) {
			Input.SetDefaultCursorShape(Input.CursorShape.Move);
			HCamRotation -= motion.Relative.X * HSensitivity * Mathf.DegToRad(1);
			VCamRotation -= motion.Relative.Y * VSensitivity * Mathf.DegToRad(1);
		}

		if (Input.IsActionJustReleased("move_cam")) Input.SetDefaultCursorShape();

		if (@event.IsActionPressed("zoom_cam_close")) {
			Cam.Position = new(Cam.Position.X, Cam.Position.Y, Cam.Position.Z - 0.75f);
		}
		
		if (@event.IsActionPressed("zoom_cam_far")) {
			Cam.Position = new(Cam.Position.X, Cam.Position.Y, Cam.Position.Z + 0.75f);
		}
	}
}
