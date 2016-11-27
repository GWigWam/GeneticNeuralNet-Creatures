﻿using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public class Creature : GameObject, IWorldVisible {
        public const float MaxRotPerSecond = (float)(MathHelper.TwoPi / 2);
        public const float DistancePerSecond = 300;
        private const int DefEyeCount = 5;

        public Network Brain { get; }

        public Vector2 Color => new Vector2(1, 0);

        private Vision Eyes;

        public Creature(MainGameContent res, Vector2 position, int eyeCount = DefEyeCount) : base(res.TCreature, position) {
            Eyes = new Vision(this, eyeCount);
            Brain = Network.Create(HyperbolicTangentFunction.Instance, true, Eyes.Count * 3, (int)(Eyes.Count / 1.5), 1);
        }

        public override void Move(float secsPassed) {
            var outp = Brain.Output[0].Output;
            var rotChange = Helpers.MathHelper.ShiftRange(outp, Brain.Transfer.YMin, Brain.Transfer.YMax, -MaxRotPerSecond, MaxRotPerSecond);
            Rotation += (rotChange * secsPassed);

            CenterPosition = GeomHelper.GetRelative(CenterPosition, Rotation, DistancePerSecond * secsPassed);
        }

        public override void Interact(IEnumerable<GameObject> gameObjs, float secsPassed) {
            var notMe = gameObjs.Where(g => g != this);
            Eyes.Update(notMe);

            for(int e = 0; e < Eyes.Count; e++) {
                var cur = Eyes.Visible[e];

                var inpR = Brain.Transfer.ShiftRange(cur.X, 0, 1);
                var inpG = Brain.Transfer.ShiftRange(cur.Y, 0, 1);
                var inpA = Brain.Transfer.ShiftRange(cur.Z, 0, 1);
                Brain.Input[e * 3 + 0].Value = inpR;
                Brain.Input[e * 3 + 1].Value = inpG;
                Brain.Input[e * 3 + 2].Value = inpA;
            }
        }

        public override void Draw(SpriteBatch sb) {
            Eyes.Draw(sb);
            base.Draw(sb);
        }

        private class Vision {
            public const int VisionDistance = 150;
            private const float EyeDistanceRad = MathHelper.TwoPi / 12f;

            private Creature Owner;
            private float[] EyeAngles;

            public Vector3[] Visible { get; private set; }
            public Tuple<Vector2, Vector2>[] VisionLines { get; private set; }

            public int Count { get; }

            public Vision(Creature owner, int eyeCount) {
                Owner = owner;
                Count = eyeCount;

                EyeAngles = new float[eyeCount];
                Visible = new Vector3[eyeCount];
                VisionLines = new Tuple<Vector2, Vector2>[eyeCount];

                int index = 0;
                for(float f = -((eyeCount - 1f) / 2f); f <= ((eyeCount - 1f) / 2f); f++) {
                    EyeAngles[index++] = f * EyeDistanceRad;
                }
            }

            public void Update(IEnumerable<GameObject> gameObjs) {
                for(int ei = 0; ei < Count; ei++) {
                    var curAngle = Owner.Rotation + EyeAngles[ei];
                    var curVisLine = CalcVissionLine(curAngle);
                    VisionLines[ei] = curVisLine;

                    GameObject closestObj = null;
                    float closestDst = VisionDistance;
                    foreach(var curGameObj in gameObjs) {
                        var dist = Vector2.Distance(Owner.CenterPosition, curGameObj.CenterPosition) - Owner.Radius - curGameObj.Radius;
                        if(dist < closestDst) {
                            if(GeomHelper.LineIntersectsCircle(curVisLine.Item1, curVisLine.Item2, curGameObj.CenterPosition, curGameObj.Radius)) {
                                closestObj = curGameObj;
                                closestDst = dist;
                            }
                        }
                    }

                    var col = GetColor(closestObj);
                    var closeness = 1 - (closestDst / VisionDistance);
                    Visible[ei] = new Vector3(col.X, col.Y, closeness);
                }
            }

            public void Draw(SpriteBatch sb) {
                for(int i = 0; i < Count; i++) {
                    var vis = Visible[i];

                    var col = new Color(vis.X, vis.Y, 0, vis.Z);
                    DrawHelper.DrawLine(sb, VisionLines[i].Item1, VisionLines[i].Item2, 1, col);
                }
            }

            private Tuple<Vector2, Vector2> CalcVissionLine(float angle) {
                var lineStart = GeomHelper.GetRelative(Owner.CenterPosition, angle, Owner.Radius);
                var lineEnd = GeomHelper.GetRelative(Owner.CenterPosition, angle, VisionDistance + Owner.Radius);
                return Tuple.Create(lineStart, lineEnd);
            }

            public static Vector2 GetColor(GameObject go) {
                if(go == null) {
                    return Vector2.Zero;
                } else if(go is IWorldVisible) {
                    return ((IWorldVisible)go).Color;
                } else {
                    return Vector2.One;
                }
            }
        }
    }
}