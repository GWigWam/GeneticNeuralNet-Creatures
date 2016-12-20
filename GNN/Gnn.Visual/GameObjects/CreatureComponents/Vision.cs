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
        internal const float VisionDistance = 250;
        internal const float EyeSeparationAngle = MathHelper.TwoPi / 16f;

        private Creature Owner;
        private float[] EyeAngles;

        public Vector3[] Visible { get; private set; }

        public int EyeCount { get; }

        public Vision(Creature owner, int eyeCount) {
            Owner = owner;
            EyeCount = eyeCount;

            EyeAngles = new float[eyeCount];
            Visible = new Vector3[eyeCount];

            int index = 0;
            for(float a = -((eyeCount - 1) / 2f); a <= ((eyeCount - 1) / 2f); a++) {
                EyeAngles[index++] = a * EyeSeparationAngle;
            }
        }

        public void Update(IEnumerable<GameObject> gameObjs) {
            var curVision = new Tuple<GameObject, float>[EyeCount];
            foreach (var curObj in gameObjs) {
                var dist = Vector2.Distance(Owner.CenterPosition, curObj.CenterPosition) - Owner.Radius - curObj.Radius;
                if (dist < VisionDistance) {
                    var curObjRelAngle = GeomHelper.Angle(Owner.CenterPosition, curObj.CenterPosition);

                    var closestEyeAngleIndex = -1;
                    var closestEyeAngleDif = float.MaxValue;
                    for (int ei = 0; ei < EyeCount; ei++) {
                        var relEyeAngle = EyeAngles[ei] + Owner.Rotation;
                        var anglDif = Math.Abs(GeomHelper.RadRange_TwoPi(curObjRelAngle) - GeomHelper.RadRange_TwoPi(relEyeAngle));
                        if (anglDif < closestEyeAngleDif && anglDif < EyeSeparationAngle / 2) {
                            closestEyeAngleIndex = ei;
                            closestEyeAngleDif = anglDif;
                        }
                    }
                    if (closestEyeAngleIndex != -1) {
                        curVision[closestEyeAngleIndex] = Tuple.Create(curObj, dist);
                    }
                }
            }

            for (int ei = 0; ei < EyeCount; ei++) {
                var curVis = curVision[ei];
                if (curVis != null) {
                    var col = GetColor(curVis.Item1);
                    var closeness = 1 - (curVis.Item2 / VisionDistance);
                    Visible[ei] = new Vector3(col.X, col.Y, closeness);
                } else {
                    Visible[ei] = Vector3.Zero;
                }
            }
        }

        public void Draw(SpriteBatch sb) {
            for (int ei = 0; ei < EyeCount; ei++) {
                var curVis = Visible[ei];
                var col = new Color(curVis.X, curVis.Y, 0, curVis.Z);
                if(col.A > 0) {
                    DrawHelper.DrawLine(sb, Owner.CenterPosition, GeomHelper.GetRelative(Owner.CenterPosition, EyeAngles[ei] + Owner.Rotation, VisionDistance), 1, col);
                }
            }
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