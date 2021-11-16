﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class Button {
        public static bool cursorType; // False = Arrow, True = Hand

        public object Arg;

        private Texture2D tex;
        private Color cColour;
        private bool pressedLastFrame;
        private int fontSize;
        private int sizeX;
        private int sizeY;
        private bool drawFrame;

        public delegate void ClickEventD(string id, object arg);
        public event ClickEventD ClickEvent;
        
        public int x; public Align alh;
        public int y; public Align alv;
        public string text;
        public string id;

        public Button(Align h, Align v, string id, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            tex = Game1.Game.Textures["button"];
            
            Game1.Game.DrawEvent += Draw;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.text = text;
            this.drawFrame = drawFrame;
            this.id = id;
            alh = h;
            alv = v;
            
            Point alPos = RRender.AlPosP(h, v, x, y);
            alPos = RRender.AlPosP(h, v, alPos.X, alPos.Y, -sizeX, -sizeY);
            this.x = alPos.X;
            this.y = alPos.Y;

            cColour = Color.White;
        }

        public void Delete() {
            Game1.Game.DrawEvent -= Draw;
        }

        private void Draw(float delta) {
            MouseState ms = Mouse.GetState();

            // funny window things
            Point pos = ((ms.Position - Game1.Game.RenderRectangle.Location).ToVector2() / Game1.Game.Scaling).ToPoint();
            pos += RRender.cameraPos.ToPoint();

            bool pressed = ms.LeftButton == ButtonState.Pressed;
            
            if ( pos.X >= x && pos.X <= x + sizeX &&
                 pos.Y >= y && pos.Y <= y + sizeY ) {
                cursorType = true;
                cColour = pressed ? Color.Gray : Color.LightGray;

                if (!pressed && pressedLastFrame) {
                    ClickEvent?.Invoke(id, Arg);
                }
            }
            else {
                cColour = Color.White;
            }
            
            if (drawFrame)
                RRender.Draw(Align.None, Align.None, tex, new Rectangle(x, y, sizeX, sizeY), new Rectangle(0, 0, 600, 200), cColour);
            
            RRender.DrawString(Align.None, Align.None, Align.Center, Align.Center, text, x + sizeX / 2, y + sizeY / 2, fontSize, cColour);

            pressedLastFrame = pressed;
        }
    }
}
