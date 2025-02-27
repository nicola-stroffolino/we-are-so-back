using Godot;
using System;

public partial class Camera : Node3D {
	[Export]
	public float HSensitivity { get; set; } = .1f;
	[Export]
	public float VSensitivity { get; set; } = .1f;

	public Node3D HGimbal { get; set; }
	public Node3D VGimbal { get; set; }

	public float HCamRotation { get; set; }
	public float VCamRotation { get; set; }
	private readonly float _vCamMin = Mathf.DegToRad(-55f);
	private readonly float _vCamMax = Mathf.DegToRad(75f);
	
	public override void _Ready() {
		// if (Actor is not null && Actor is Player p) p.CameraController = this;

		HGimbal = GetNode<Node3D>("H");
		VGimbal = GetNode<Node3D>("H/V");

		// Input.MouseMode = Input.MouseModeEnum.Captured;
		HCamRotation = HGimbal.Rotation.Y;
		VCamRotation = VGimbal.Rotation.X;
	}

	public override void _Process(double delta) {
		HGimbal.Rotation = new Vector3(0, HCamRotation, 0);

		VCamRotation = Mathf.Clamp(VCamRotation, _vCamMin, _vCamMax);
		VGimbal.Rotation = new Vector3(VCamRotation, 0, 0);
		
		// if (Attached) GlobalPosition = Actor.GlobalPosition; 
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motion) {
			HCamRotation -= motion.Relative.X * HSensitivity * Mathf.DegToRad(1);
			VCamRotation -= motion.Relative.Y * VSensitivity * Mathf.DegToRad(1);
		}
	
		// if (@event.IsActionPressed("lock_to_target") && LockController.Target is not null) return GetState<Locked>();
	}
}
