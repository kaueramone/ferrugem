using Unity.Mathematics;
namespace Ferrugem
{
    public static class FpsMotor
    {
        public const float StandingHeight = 1.85f, CrouchingHeight = 1.15f, StepHeight = .3f;
        public const float Gravity = 18, JumpSpeed = 6, WalkSpeed = 3.5f, SprintSpeed = 6, CrouchSpeed = 1.8f, AimSpeed = 2;
        public static float Height(bool crouched) => crouched ? CrouchingHeight : StandingHeight;
        public static float Eye(bool crouched) => crouched ? 1.02f : 1.65f;
        public static float HeadMin(bool crouched) => crouched ? .9f : 1.45f;
        public static bool WalkableSlope(float degrees) => math.isfinite(degrees) && math.abs(degrees) <= 35;
        public static void Step(ref FpsPlayer player, ref float3 position, in FpsInput input, float dt)
        {
            if (!math.isfinite(dt) || dt <= 0 || !math.all(math.isfinite(input.Move)) || !math.isfinite(input.Yaw) || !math.isfinite(input.Pitch)) return;
            // Substeps bound collision sweeps even during server tick batching; jump edge is consumed once.
            int steps = math.max(1, (int)math.ceil(dt / .016667f)); float sub = dt / steps;
            for (int i = 0; i < steps; i++) Integrate(ref player, ref position, input, sub, i == 0 && input.Jump.IsSet);
        }
        private static void Integrate(ref FpsPlayer player, ref float3 position, in FpsInput input, float dt, bool jump)
        {
            if (input.Crouch != 0) player.Crouched = 1;
            else if (player.Crouched != 0 && FpsArena.Fits(position, StandingHeight)) player.Crouched = 0;
            player.Aiming = (byte)(input.Aim != 0 ? 1 : 0);
            float height = Height(player.Crouched != 0);
            float support = FpsArena.Ground(position, position.y + .025f);
            player.Grounded = (byte)(player.Velocity.y <= 0 && math.abs(position.y - support) <= .025f ? 1 : 0);
            if (player.Grounded != 0) { position.y = support; player.Velocity.y = 0; }
            if (jump && player.Grounded != 0 && player.Crouched == 0) { player.Velocity.y = JumpSpeed; player.Grounded = 0; }
            var move = input.Move / math.max(1, math.length(input.Move));
            float speed = player.Crouched != 0 ? CrouchSpeed : player.Aiming != 0 ? AimSpeed : input.Sprint != 0 ? SprintSpeed : WalkSpeed;
            var target = math.rotate(quaternion.RotateY(math.radians(input.Yaw % 360)), new float3(move.x, 0, move.y)) * speed;
            float acceleration = player.Grounded != 0 ? (math.lengthsq(move) > 0 ? 22 : 28) : 7;
            var delta = target.xz - player.Velocity.xz; player.Velocity.xz += math.normalizesafe(delta) * math.min(math.length(delta), acceleration * dt);
            var previous = position;
            position = FpsArena.Horizontal(position, new float3(player.Velocity.x * dt, 0, player.Velocity.z * dt), height, player.Grounded != 0);
            if (math.abs(position.x - previous.x) < .00001f) player.Velocity.x = 0;
            if (math.abs(position.z - previous.z) < .00001f) player.Velocity.z = 0;
            player.Velocity.y -= Gravity * dt;
            float nextY = position.y + player.Velocity.y * dt;
            if (player.Velocity.y > 0)
            {
                float ceiling = FpsArena.Ceiling(position, position.y + height, nextY + height);
                if (ceiling < nextY + height) { nextY = ceiling - height; player.Velocity.y = 0; }
            }
            float floor = FpsArena.Ground(position, math.max(position.y, nextY));
            if (nextY <= floor && player.Velocity.y <= 0) { position.y = floor; player.Velocity.y = 0; player.Grounded = 1; }
            else { position.y = nextY; player.Grounded = 0; }
            player.Pitch = math.clamp(input.Pitch, -85, 85);
        }
    }
}
