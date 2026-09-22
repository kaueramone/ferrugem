using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
namespace Ferrugem
{
    public static class FpsMotorSmoke
    {
        public static void Run()
        {
            int count=0;
            void Check(string name,bool pass) { if(!pass) throw new InvalidOperationException("MOTOR_SUITE_FAIL "+name); count++; Debug.Log($"[Ferrugem] MOTOR_CHECK name={name} pass=1"); }
            void Advance(ref FpsPlayer p, ref float3 pos, FpsInput input, int ticks) { for(int i=0;i<ticks;i++) FpsMotor.Step(ref p,ref pos,input,1f/30); }
            FpsPlayer p=default; float3 pos=float3.zero; var walk=new FpsInput { Move=new float2(0,1) };
            FpsMotor.Step(ref p,ref pos,walk,1f/30); Check("acceleration",p.Velocity.z>0 && p.Velocity.z<FpsMotor.WalkSpeed);
            Advance(ref p,ref pos,walk,30); Advance(ref p,ref pos,default,30); Check("deceleration",math.length(p.Velocity)<.001f);
            p=default;pos=new float3(8,0,-9);var sprint=walk;sprint.Sprint=1;Advance(ref p,ref pos,sprint,30);Check("sprint_speed",p.Velocity.z>5.9f && p.Velocity.z<=6.001f);
            var aim=sprint;aim.Aim=1;Advance(ref p,ref pos,aim,30);Check("aim_speed",p.Aiming==1 && p.Velocity.z<=2.001f);
            p=default;pos=float3.zero;var jump=new FpsInput();jump.Jump.Set();FpsMotor.Step(ref p,ref pos,jump,1f/30);
            Check("jump",p.Grounded==0 && p.Velocity.y>0 && pos.y>0);
            var once=p;var posOnce=pos; FpsMotor.Step(ref p,ref pos,jump,1f/30);FpsMotor.Step(ref once,ref posOnce,default,1f/30);
            Check("no_double_jump",math.abs(p.Velocity.y-once.Velocity.y)<.001f && math.distance(pos,posOnce)<.001f);
            Advance(ref p,ref pos,default,60);Check("land",p.Grounded==1 && math.abs(pos.y)<.001f && p.Velocity.y==0);
            p=new FpsPlayer{Crouched=1};pos=new float3(0,0,5);FpsMotor.Step(ref p,ref pos,default,1f/30);Check("standing_blocked",p.Crouched==1 && FpsArena.Fits(pos,FpsMotor.CrouchingHeight) && !FpsArena.Fits(pos,FpsMotor.StandingHeight));
            p=default;pos=new float3(0,0,3);Advance(ref p,ref pos,walk,30);Check("tunnel_standing_blocked",pos.z<3.66f);
            p=default;pos=new float3(0,0,3);var crouch=walk;crouch.Crouch=1;Advance(ref p,ref pos,crouch,45);Check("crouch_clearance",pos.z>4 && p.Crouched==1 && p.Velocity.z<=1.801f);
            p=new FpsPlayer{Crouched=1,Velocity=new float3(0,5,0)};pos=new float3(0,.05f,5);var held=new FpsInput{Crouch=1};FpsMotor.Step(ref p,ref pos,held,.1f);Check("ceiling_collision",pos.y+FpsMotor.CrouchingHeight<=1.251f && p.Velocity.y<=0);
            p=default;pos=new float3(-12,0,-7);Advance(ref p,ref pos,walk,60);Check("steps",pos.z> -2 && math.abs(pos.y-1)<.02f);
            p=default;pos=new float3(8,0,-1);var right=new FpsInput{Move=new float2(1,0)};Advance(ref p,ref pos,right,60);Check("step_too_high",pos.x<9.66f && pos.y<.01f);
            p=default;pos=new float3(12,0,-7);Advance(ref p,ref pos,walk,60);Check("ramp",pos.z> -2 && math.abs(pos.y-1.2f)<.02f);
            Advance(ref p,ref pos,walk,45);Check("fall",pos.z>1 && p.Grounded==1 && math.abs(pos.y)<.01f);
            Check("slope_limit",FpsMotor.WalkableSlope(FpsArena.RampSlopeDegrees) && !FpsMotor.WalkableSlope(36));
            Check("ramp_ray",!CombatRules.RayRamp(new float3(9,1,-5),new float3(1,0,0),out _) && CombatRules.RayRamp(new float3(9,.2f,-5),new float3(1,0,0),out _));
            Check("crouch_head_height",CombatRules.RayBox(new float3(0,FpsMotor.Eye(true),-5),new float3(0,0,1),new float3(-.3f,0,-.3f),new float3(.3f,FpsMotor.Height(true),.3f),out _) && FpsMotor.Eye(true)>=FpsMotor.HeadMin(true) && FpsMotor.Eye(true)<FpsMotor.HeadMin(false));
            p=default;pos=new float3(-4,0,-3);for(int i=0;i<120;i++){var push=walk;push.Jump.Set();FpsMotor.Step(ref p,ref pos,push,1f/30);}Check("no_wall_climb",pos.z< -1.34f && pos.y<1.1f);
            Debug.Log($"[Ferrugem] MOTOR_SUITE_PASS count={count}");
        }
    }
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation|WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup),OrderLast=true)]
    public partial class FpsMotorDiagnosticsSystem:SystemBase
    {
        private bool smoke, tested;
        private readonly Dictionary<Entity,FpsPlayer> before=new Dictionary<Entity,FpsPlayer>();
        private readonly HashSet<Entity> moved=new HashSet<Entity>();
        private readonly HashSet<Entity> airborneJumpObserved=new HashSet<Entity>();
        protected override void OnCreate(){smoke=Arguments.Has("--motor-smoke");}
        protected override void OnUpdate()
        {
            if(!smoke)return;
            if(World.IsServer()&&!tested){tested=true;FpsMotorSmoke.Run();}
            int localId=World.IsClient()&&SystemAPI.TryGetSingleton<NetworkId>(out var id)?id.Value:-1;
            using var query=EntityManager.CreateEntityQuery(typeof(FpsPlayer),typeof(LocalTransform),typeof(GhostOwner));using var entities=query.ToEntityArray(Allocator.Temp);
            var present=new HashSet<Entity>();
            foreach(var e in entities)
            {
                present.Add(e);var state=EntityManager.GetComponentData<FpsPlayer>(e);var pos=EntityManager.GetComponentData<LocalTransform>(e).Position;int owner=EntityManager.GetComponentData<GhostOwner>(e).NetworkId;int local=owner==localId?1:0;
                void Emit(string action)=>Debug.Log($"[Ferrugem] MOTOR_OBSERVED world={World.Name} owner={owner} local={local} event={action} grounded={state.Grounded} y={pos.y:F3} vy={state.Velocity.y:F3}");
                bool existed=before.TryGetValue(e,out var previous);
                // Prediction may first expose an airborne state before upward velocity is observable.
                // Prove real ascent above support, rather than requiring a particular snapshot transition.
                if(state.Grounded!=0) airborneJumpObserved.Remove(e);
                if(state.Grounded==0 && state.Velocity.y>1 && pos.y>FpsArena.Ground(pos,pos.y)+.05f
                    && airborneJumpObserved.Add(e)) Emit("jump");
                if(existed&&previous.Grounded==0&&state.Grounded==1)Emit("land");
                if(state.Crouched!=0&&(!existed||previous.Crouched==0))Emit("crouch");
                if(state.Aiming!=0&&(!existed||previous.Aiming==0))Emit("aim");
                if(math.distance(pos.xz,state.Spawn.xz)>1&&moved.Add(e))Emit("move");
                if(!math.all(math.isfinite(pos))||pos.y<-.05f||pos.y>4)Debug.LogError("MOTOR_INVALID_POSITION");
                before[e]=state;
            }
            foreach(var e in new List<Entity>(before.Keys))if(!present.Contains(e)){before.Remove(e);moved.Remove(e);airborneJumpObserved.Remove(e);}
        }
    }
}
