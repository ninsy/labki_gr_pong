using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheVillageOfLife
{
    public class Ball
    {
        public Vector2 position { get; set; }
        public Vector2 defaultPosition { get; set; }
        public Vector2 speed { get; set; }
        
        public int collisionOffset = 10;
        public Point frameSize = new Point(64, 64);

        public Random rand;
        public bool isScore { get; set;}

        public Vector2 explosionPosition { get; set; }
        public Texture2D explosionTexture { get; set; }
        public Point explosionCurrentFrame = new Point(0, 0);
        public Point explosionSheetSize = new Point(4,4);
        public int timeSinceLastFrameExplosion = 0;

        public Rectangle rectangle { get; set; }
        public Texture2D ballAnimationTexture { get; set; }
        public Point ballCurrentFrame = new Point(0, 0);
        public Point ballSheetSize = new Point(15,4);
        public int timeSinceLastFrameBall = 0;
        
        public int milisecondsPerFrame = 16;
        
        public Ball(Vector2 pos, Texture2D textr)
        {
            ballAnimationTexture = textr;
            rand = new Random();
            position = pos;
            defaultPosition = position;
            speed = new Vector2(randomExceptZero(), 0);
            isScore = false;
        }

        public int randomExceptZero()
        {
            int val = 0;
            while (val == 0)
            {
                val = rand.Next(-5, 5);
            }

            return val;
        }
        
        public void Move()
        {
            position += speed;
        }

        public void animateBall(int miliseconds)
        {
            timeSinceLastFrameBall += miliseconds;
            if (timeSinceLastFrameBall >= milisecondsPerFrame)
            {
                timeSinceLastFrameBall -= milisecondsPerFrame;
                ballCurrentFrame.X++;
                if (ballCurrentFrame.X >= ballSheetSize.X)
                {
                    ballCurrentFrame.X = 0;
                    ballCurrentFrame.Y++;
                    if (ballCurrentFrame.Y >= ballSheetSize.Y)
                    {
                        ballCurrentFrame.Y = 0;
                    }
                }
            }
        }

        public void animateExplosion(int miliseconds)
        {
            timeSinceLastFrameExplosion += miliseconds;
            if (timeSinceLastFrameExplosion >= milisecondsPerFrame)
            {
                timeSinceLastFrameExplosion -= milisecondsPerFrame;
                explosionCurrentFrame.X++;
                if (explosionCurrentFrame.X >= explosionSheetSize.X)
                {
                    explosionCurrentFrame.X = 0;
                    explosionCurrentFrame.Y++;
                    if (explosionCurrentFrame.Y >= explosionSheetSize.Y)
                    {
                        explosionCurrentFrame.Y = 0;
                        isScore = false;
                    }
                }
            }
        }
        
        public void resetPosition()
        {
            position = defaultPosition;
            speed = new Vector2(randomExceptZero(), 0);
        }
  

        public bool[] collsionWithBounds(Rectangle bounds)
        {
            // [ top, right, bottom, left ]
            bool[] collisions = new bool[4];
            rectangle = new Rectangle(
                (int)position.X + collisionOffset,
                (int)position.Y + collisionOffset,
                frameSize.X - collisionOffset * 2,
                frameSize.Y - collisionOffset * 2
            );
            
            if (rectangle.Top <= bounds.Top)
            {
                speed *= new Vector2(1, -1);
                collisions[0] = true;
            }
            if (rectangle.Right >= bounds.Right)
            {
                speed *= new Vector2(-1, 1);
                explosionPosition = position;
                collisions[1] = true;
            }
            if (rectangle.Bottom >= bounds.Bottom)
            {
                speed *= new Vector2(1, -1);
                collisions[2] = true;
            }
            if (rectangle.Left <= bounds.Left)
            {
                speed *= new Vector2(-1, 1);
                explosionPosition = position;
                collisions[3] = true;
            }

            return collisions;
        }
        
        public bool collisionWithPaddle(Paddle paddle)
        {
            rectangle = new Rectangle((int)position.X + collisionOffset, (int)position.Y + collisionOffset, frameSize.X - (collisionOffset* 2), frameSize.Y - (collisionOffset * 2));

            for (int i = 1; i <= paddle.sectionCount; i++)
            {
                Point sideCenter = new Point(rectangle.Center.X + ((paddle.side== "left") ? -rectangle.Width / 2 : rectangle.Width / 2), rectangle.Center.Y);
                if (paddle.sections[i - 1].Contains(sideCenter))
                {
                    speed *= new Vector2(-1, 1);
                    if (speed.X < 0)
                        speed -= Vector2.UnitX;
                    else
                        speed += Vector2.UnitX;

                    if (i < 3)
                    {
                        if (speed.Y > 0)
                            speed *= new Vector2(1, -i);
                        else if (speed.Y == 0)
                            speed -= (Vector2.UnitY * i);
                    }
                    else if (i == 3)
                        speed = new Vector2(speed .X, 0);
                    else if (i == 4)
                    {
                        if (speed .Y < 0)
                            speed *= new Vector2(1, -1);
                        else if (speed.Y == 0)
                            speed += (Vector2.UnitY);
                    }
                    else
                    {
                        if (speed.Y < 0)
                            speed *= new Vector2(1, -2);
                        else if (speed.Y == 0)
                            speed += (Vector2.UnitY * 2);
                    }
                    paddle.beingHit = true;
                    paddle.color = Color.Red;

                    return true;
                }
                else if (i == 1 || i == 5)
                {
                    if (paddle.sections[i - 1].Intersects(rectangle))
                    {
                        speed  *= new Vector2(-1, 1);
                        if (speed.Y == 0)
                            speed -= (Vector2.UnitY * 1);
                        if (i == 1)
                        {
                            if (speed .Y > 0)
                                speed  *= new Vector2(1, -i);
                        }
                        else
                        {
                            if (speed.Y < 0)
                                speed *= new Vector2(1, -1);
                        }
                        paddle.beingHit = true;
                        paddle.color = Color.Red;
                        
                        return true;
                    }
                }
            }

            return false;
        }
        
//        public bool collisionWithPaddle(Paddle paddle)
//        {
//            rectangle = new Rectangle(
//                (int)position.X + collisionOffset,
//                (int)position.Y + collisionOffset,
//                frameSize.X - collisionOffset * 2,
//                frameSize.Y - collisionOffset * 2
//            );
//
//            for (int i = 0; i < paddle.sectionCount; i++)
//            {
//                if (paddle.sections[i].Intersects(rectangle))
//                {
//                    paddle.beingHit = true;
//                    /*
//                     * [0]
//                     * [1]
//                     * [2]
//                     * [3]
//                     * [4]
//                     */
//                    switch (i)
//                    {
//                        case 0:
//                            speed *= new Vector2(-(speed.X+1), 3);
//                            break;
//                        case 1:
//                            speed *= new Vector2(-(speed.X+1), 2);
//                            break;
//                        case 2:
//                            speed *= new Vector2(-(speed.X+1), 1);
//                            break;                        
//                        case 3:
//                            speed *= new Vector2(-(speed.X+1), -2);
//                            break;
//                        case 4:
//                            speed *= new Vector2(-(speed.X+1), -3);
//                            break;
//                    }
//                    return true;
//                }
//            }
//
//            return false;
//        }
    }
}