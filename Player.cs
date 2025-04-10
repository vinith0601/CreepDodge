using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	public int intialSpeed = 400;
	[Export]
	public int Speed = 400;
	[Export]
	public bool _IsBoostOnCooldown = false;

	public Vector2 ScreenSize;


	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}
		if (Input.IsActionJustPressed("boost"))
		{
			if (!_IsBoostOnCooldown)
			{
				GD.Print("Boost activated");
				Speed = (int)(Speed * 1.3);
				_IsBoostOnCooldown = true;
				GetNode<Timer>("BoostTimer").Start();
				GetNode<Timer>("BoostCoolDownTimer").Start();
			} else {
				GD.Print("Boost on cooldown");
			}

		}

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (velocity.Length()> 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();

		}
		else
		{
			animatedSprite.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
		if (velocity.X != 0)
		{
			animatedSprite.Animation = "walk";
			animatedSprite.FlipV = false;
			animatedSprite.FlipH = velocity.X < 0;
		} else if (velocity.Y != 0){
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
	private void OnBoostTimerTimeout()
	{
		Speed = 400;
		GD.Print("Boost deactivated");
	}
	private void OnBoostCoolDownTimerTimeout()
	{
		GD.Print("Boost cooldown finished");
		_IsBoostOnCooldown = false;
	}
}
