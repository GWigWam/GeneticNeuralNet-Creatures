using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.CreatureComponents {

    internal class Vision : Base.IDrawable {
        public const int VisionDistance = 250;
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
            for(int i = 0; i < VisionLines.Length; i++) {
                VisionLines[i] = new Tuple<Vector2, Vector2>(new Vector2(), new Vector2());
            }

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