﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;
using com.Kyle.Keebler.Items;

namespace com.Kyle.Keebler.Characters
{
    public abstract class Character : IRenderable, IMoveable
    {

        #region Fields

        //Time to before a change is avaible 
        public int TimeToWait {get;set;}

        //Position of the Character on the screen
        protected Vector2 Position = new Vector2(0,0); 

        //Which sprite is selected on the Sheet
        protected Vector2 CurrentFrame = new Vector2(0,0); 

       
        //Time between each frame
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame = 150;

        //Movement Elements
        protected Dictionary<Direction, Tuple<Point, Point>> walkFrames;
        protected Dictionary<Direction, Tuple<Point, Point>> idleFrames;
        protected Dictionary<Direction, Tuple<Point, Point>> attackFrames;

        protected int movementRate = 2;

        #endregion

        public string Name { get; set; }  //Name of the Character
        public Texture2D Texture { get; set; } //Sprite Sheet assigned to the Character
        public Point FrameSize { get; set; } //The size of one frame on the Sprite Sheet
        public Rectangle MapBoundry { get; set; }

        public int MaxHealth { get; set; } //Max health a character has
        public int CurrentHealth { get; set; } //Current amount of health a character has
        public bool CanMove { get; set; } //Allows a character to move or not to move 
        public Direction CharacterDirection { get; set; }
        public bool Attacking { get; set; }

        public int ExperiencePointValue { get; set; } //How much experience points the character gives???

        public Vector2 getPosition()
        {
            return Position; 
        }

        public Point getFrameSize()
        {
            return FrameSize;
        }

        public int getMovementRate()
        {
            return movementRate;
        }

        public Rectangle CollisionRec
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 5,
                    (int)Position.Y + 5,
                    FrameSize.X - 5,
                    FrameSize.Y - 5);
            }
        }

        public bool CanCollide { get; set; }

        public abstract void ChangeGraphic(); //allows a change to the texture
        public abstract bool ResolveAttack(); //Handles what happens when attacked?
        public abstract void Update(GameTime gameTime); //Used to Update the Character in the Game Class
        public abstract void Draw(SpriteBatch spriteBatch); //Used to Draw the Character in the Game Class
        public abstract void CollisionAction(ICollidable collisionElement);
        public abstract void CollisionActionItem(Item item);
        /// <summary>
        /// Updates the Player position on the screen when a certain direction is called.
        /// Draws the frames that correspond with direction the character is moving.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="gameTime"></param>
        public void MovePosition(Direction direction, GameTime gameTime)
        {
            //let put basic movements here
            switch (direction)
            {
                case Direction.South:
                    Position.Y += movementRate;
                    break;
                case Direction.East:
                    Position.X += movementRate;
                    break;
                case Direction.West:
                    Position.X -= movementRate;
                    break;
                case Direction.North:
                    Position.Y -= movementRate;
                    break;
                default:
                    throw new ArgumentException("Unhandled move direction.");
                    break;
            }
            WalkCharacter(gameTime, direction);
            BoundryCollision(MapBoundry);
        }

        /// <summary>
        /// This Initialize of character assigns the Character to a Sprite sheet
        /// and the position of the character on the screen.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public virtual void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;

        }

        public virtual bool Collide(Rectangle collideRec)
        {

            return CollisionRec.Intersects(collideRec);
        }

        /// <summary>
        /// Creates a Rectangle that has the exact position of the CurrentFrame.
        /// </summary>
        /// <returns></returns>
        public Rectangle renderFrame()
        {
            return new Rectangle(Convert.ToInt32(CurrentFrame.X) * Convert.ToInt32(FrameSize.X),
                    Convert.ToInt32(CurrentFrame.Y) * Convert.ToInt32(FrameSize.Y),
                    Convert.ToInt32(FrameSize.X), Convert.ToInt32(FrameSize.Y));
        }

        public void IdleCharacter(GameTime gameTime, Direction direction)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                ++CurrentFrame.X;
                if (CurrentFrame.X >= idleFrames[direction].Item2.X)
                {
                    CurrentFrame.X = idleFrames[direction].Item1.X;
                    ++CurrentFrame.Y;
                    if (CurrentFrame.Y >= idleFrames[direction].Item2.Y)
                        CurrentFrame.Y = idleFrames[direction].Item1.Y;
                }
            }
        }

        public void WalkCharacter(GameTime gameTime, Direction direction)
        {
            CharacterDirection = direction;

            CurrentFrame.Y = walkFrames[direction].Item1.Y;
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                ++CurrentFrame.X;
                if (CurrentFrame.X >= walkFrames[direction].Item2.X)
                {
                    CurrentFrame.X = walkFrames[direction].Item1.X;
                    ++CurrentFrame.Y;
                    if (CurrentFrame.Y >= walkFrames[direction].Item2.Y)
                        CurrentFrame.Y = walkFrames[direction].Item1.Y;
                }
            }
        }

        public void AttackCharacter(GameTime gameTime, Direction direction)
        {
            CharacterDirection = direction;
            CurrentFrame.X = attackFrames[direction].Item2.X;
            CurrentFrame.Y = attackFrames[direction].Item2.Y;
            CanMove = false;
            TimeToWait = 200;
        }

        public void KnockBack(Direction direction, int length)
        {
            if (direction == Direction.East)
                Position.X -= length;
            if (direction == Direction.West)
                Position.X += length;
            if (direction == Direction.South)
                Position.Y -= length;
            if (direction == Direction.North)
                Position.Y += length;
        }

        public bool BoundryCollision(Rectangle map)
        {
            if (Position.X > map.Width - FrameSize.X)
            {
                Position.X = map.Width - FrameSize.X;
                return true;
            }
            if (Position.Y > map.Height - FrameSize.Y)
            {
                Position.Y = map.Height - FrameSize.Y;
                return true;
            }
            if (Position.X < map.X)
            {
                Position.X = map.X;
                return true;
            }
            if (Position.Y < map.Y)
            {
                Position.Y = map.Y;
                return true;
            }
            return false;
        }
        public void BoundryCollisionReverse(Rectangle map)
        {
            if (CollisionRec.X + FrameSize.X > map.X)
            {
                Position.X -= movementRate * 2;
                
            }
            if (CollisionRec.Y + FrameSize.Y > map.Y)
            {
                Position.Y -= movementRate * 2;
               
            }
            if (CollisionRec.X < map.X + map.Height)
            {
                Position.X += movementRate * 2;
                
            }
            if (CollisionRec.Y < map.Y + map.Height)
            {
                Position.Y += movementRate * 2;
                
            }

        }
        #region IRenderable Members

        public void Initialize()
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}
