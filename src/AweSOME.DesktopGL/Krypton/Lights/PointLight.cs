﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Lights
{
    public class PointLight : ILight2D
    {
        private bool mIsOn = true;
        private Vector2 mPosition = Vector2.Zero;
        private float mAngle = 0;
        private Texture2D mTexture = null;
        private Color mColor = Color.White;
        private float mRange = 1;
        private float mFov = MathHelper.TwoPi;
        private float mIntensity = 1;

        //private RenderTarget2D mMap = null;

        #region Parameters

        /// <summary>
        /// The light's position
        /// </summary>
        public Vector2 Position
        {
            get { return this.mPosition; }
            set { this.mPosition = value; }
        }

        /// <summary>
        /// The X coordinate of the light's position
        /// </summary>
        public float X
        {
            get { return this.mPosition.X; }
            set { this.mPosition.X = value; }
        }

        /// <summary>
        /// The Y coordinate of the light's position
        /// </summary>
        public float Y
        {
            get { return this.mPosition.Y; }
            set { this.mPosition.Y = value; }
        }

        /// <summary>
        /// The light's angle
        /// </summary>
        public float Angle
        {
            get { return this.mAngle; }
            set { this.mAngle = value; }
        }

        /// <summary>
        /// The texture used as the base light map, from which shadows will be subtracted
        /// </summary>
        public Texture2D Texture
        {
            get { return this.mTexture; }
            set { this.mTexture = value; }
        }

        /// <summary>
        /// The color used to tint the light's texture
        /// </summary>
        public Color Color
        {
            get { return this.mColor; }
            set { this.mColor = value; }
        }

        /// <summary>
        /// The light's maximum radius, or width
        /// </summary>
        public float Range
        {
            get { return this.mRange; }
            set { this.mRange = value; }
        }

        public float Fov
        {
            get { return this.mFov; }
            set
            {
                this.mFov = MathHelper.Clamp(value, 0, MathHelper.TwoPi);
            }
        }

        public float Intensity
        {
            get { return this.mIntensity; }
            set { this.mIntensity = MathHelper.Clamp(value, 0.01f, 3f); }
        }

        #endregion Parameters

        #region ILight Implementation

        /// <summary>
        /// Gets or sets a value indicating weither or not to draw the light
        /// </summary>
        public bool IsOn
        {
            get { return this.mIsOn; }
            set { this.mIsOn = value; }
        }

        /// <summary>
        /// Draws the light with texture and color
        /// </summary>
        /// <param name="helper">A render helper for drawing the light</param>
        public void Draw(KryptonRenderHelper helper)
        {
            // Set effect parameters and technique
            helper.Effect.Parameters["Texture0"].SetValue(this.mTexture);
            helper.Effect.Parameters["LightIntensityFactor"].SetValue(1 / (this.mIntensity * this.mIntensity));
            helper.Effect.CurrentTechnique = helper.Effect.Techniques["LightTexture"];

            // Draw the light
            foreach (var effectPass in helper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                helper.DrawClippedFov(this.mPosition, this.mAngle, this.mRange, this.mColor, this.mFov);
            }
        }

        /// <summary>
        /// Draws shadows from the light's position outward
        /// </summary>
        /// <param name="helper">A render helper for drawing shadows</param>
        /// <param name="hulls">The shadow hulls used to draw shadows</param>
        public void DrawShadows(KryptonRenderHelper helper, List<ShadowHull> hulls)
        {
            // Make sure we only render the following hulls
            helper.ShadowHullVertices.Clear();
            helper.ShadowHullIndicies.Clear();

            // Loop through each hull
            foreach(ShadowHull hull in hulls)
            {
                // Add the hulls to the buffer only if they are within the light's range
                if (hull.Enabled && PointLight.IsInRange(hull.Position - this.Position, hull.MaxRadius + this.Range))
                {
                    helper.BufferAddShadowHull(hull);
                }
            }

            // Set the effect parameters
            helper.Effect.Parameters["LightPosition"].SetValue(this.mPosition);
            helper.Effect.CurrentTechnique = helper.Effect.Techniques["PointLight_Shadow"];
            foreach (var effectPass in helper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                helper.BufferDraw();
            }
        }

        /// <summary>
        /// Determines if a vector's length is less than a specified value
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="dist">Distance</param>
        /// <returns></returns>
        private static bool IsInRange(Vector2 offset, float dist)
        {
            if (offset.X * offset.X + offset.Y * offset.Y < dist * dist)
                return true;

            return false;
        }

        #endregion ILight Implementation
    }
}
