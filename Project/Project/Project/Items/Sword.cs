﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.Kyle.Keebler.Characters;

namespace com.Kyle.Keebler.Items
{
    public class Sword : Item
    {
        public Sword(String Name, Texture2D Texture, Vector2 Position, ItemType TypeOfItem)
        {
            this.Name = Name;
            this.Texture = Texture;
            this.Position = Position;
            this.TypeOfItem = TypeOfItem;
            IsPickedUp = false;
            CanCollide = true;

        }
        public void Update(GameTime gameTime)
        {
           
        }

        public override void PickMeUp(Character pickedUpBy)
        {
            base.PickMeUp(pickedUpBy);
            this.CanCollide = true;
        }
    }
}