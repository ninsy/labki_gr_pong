using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace TheVillageOfLife
{
    public class Animation
    {
        public Vector2 position { get; set; }
        public Texture2D texture { get; set; }
        
        public Point currentFrame = new Point(0, 0);
        public Point sheetSize { get; set; }

        public int timeSinceLastFrame = 0;
        public int milisecondsPerFrame = 16;
        
        public Animation(Point sheetSize)
        {
            this.sheetSize = sheetSize;
        }

        public void animate(int miliseconds)
        {
            timeSinceLastFrame += miliseconds;
            if (timeSinceLastFrame >= milisecondsPerFrame)
            {
                timeSinceLastFrame -= milisecondsPerFrame;
                currentFrame.X++;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    currentFrame.Y++;
                    if (currentFrame.Y >= sheetSize.Y)
                    {
                        currentFrame.Y = 0;
                    }
                }
            }
        }
    }

    public class Score
    {
        public Paddle paddle;
        public Vector2 position;
        public SpriteFont font { get; set; }
        
        public Score(Vector2 pos, Paddle pad)
        {
            this.paddle = pad;
            this.position = pos;
        }

        public void draw(SpriteBatch sb)
        {
            sb.DrawString(font, paddle.score.ToString(), position, Color.White);
        }
    }
    
    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphicsDevice;
        public SpriteBatch spriteBatch;
        
        public Paddle left;
        public Paddle right;
        public Ball ball;

        public Score leftScore;
        public Score rightScore;

        public Texture2D background;
        
        public bool gameStarted = false;

        public Texture2D tempLeftPaddle;
        public Texture2D tempRightPaddle;
        public Texture2D tempBall;        
        
        public Game()
        {
            graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true
            };
        }

        protected override void Initialize()
        {            
            gameStarted = IsMouseVisible = Window.AllowUserResizing = false;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tempLeftPaddle = Content.Load<Texture2D>("paddle1a");
            tempRightPaddle = Content.Load<Texture2D>("paddle2a");
            tempBall = Content.Load<Texture2D>("ball-anim");
            
            left = new Paddle(
                "left",
                Keys.Q,
                Keys.A,
                new Vector2(
                    0,
                    (GraphicsDevice.Viewport.Height - tempLeftPaddle.Height) / 2
                ),
                tempLeftPaddle
            );
            
            right = new Paddle(
                "right",
                Keys.P,
                Keys.L,
                new Vector2(
                    GraphicsDevice.Viewport.Width - tempRightPaddle.Width,
                    (GraphicsDevice.Viewport.Height - tempRightPaddle.Height) / 2
                ),
                tempRightPaddle
            );
            
            ball = new Ball(
                new Vector2(
                    (GraphicsDevice.Viewport.Width - tempBall.Width / 16) / 2,
                    (GraphicsDevice.Viewport.Height- tempBall.Height / 5) / 2                    
                ),
                tempBall
            );
            ball.explosionTexture = Content.Load<Texture2D>("explosion64");
            

            
            leftScore = new Score(new Vector2(
                GraphicsDevice.Viewport.Width / 2 - 30,
                5  
            ), left);
            
            rightScore = new Score(new Vector2(
                GraphicsDevice.Viewport.Width / 2 + 20,
                5                
            ), right);
                    
            leftScore.font = Content.Load<SpriteFont>("font");
            rightScore.font = Content.Load<SpriteFont>("font");
            
            background = Content.Load<Texture2D>("pongBackground");
            
            left.rectangle = new Rectangle(0, 0, left.texture.Width, left.texture.Height);
            right.rectangle = new Rectangle(0, 0, right.texture.Width, right.texture.Height);
            ball.rectangle = new Rectangle(0, 0, ball.ballAnimationTexture.Width, ball.ballAnimationTexture.Height);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                    Keys.Escape))
            {
                Exit();
            }
            
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Space))
            {
                gameStarted = true;
            }

            bool[] boundsCollision = ball.collsionWithBounds(GraphicsDevice.Viewport.Bounds);
            bool leftCollides = ball.collisionWithPaddle(left); 
            bool rightCollides = ball.collisionWithPaddle(right);
            
            left.highlight(gameTime.ElapsedGameTime.Milliseconds);
            right.highlight(gameTime.ElapsedGameTime.Milliseconds);
            
            
            left.Move(ks, GraphicsDevice.Viewport.Height);
            right.Move(ks, GraphicsDevice.Viewport.Height);

            ball.animateBall(gameTime.ElapsedGameTime.Milliseconds);
            ball.animateExplosion(gameTime.ElapsedGameTime.Milliseconds);
            if (boundsCollision[1])
            {
                left.score++;
                ball.isScore = true;
                ball.resetPosition();

            }
            else if (boundsCollision[3])
            {
                right.score++;
                ball.isScore = true;
                ball.resetPosition();
            }
            
//            if (leftCollides)
//            {
//                left.beingHit = true;
//                left.highlight(gameTime.ElapsedGameTime.Milliseconds);
//            }
//            
//            if (rightCollides)
//            {
//                right.beingHit = true;
//                right.highlight(gameTime.ElapsedGameTime.Milliseconds);
//            }
            
            if (gameStarted)
            {
                ball.Move();
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, Color.White);
            
            leftScore.draw(spriteBatch);
            rightScore.draw(spriteBatch);

            spriteBatch.Draw(
                ball.ballAnimationTexture,
                ball.position,
                new Rectangle(
                    ball.ballCurrentFrame.X * ball.frameSize.X,
                    ball.ballCurrentFrame.Y * ball.frameSize.Y,
                    ball.frameSize.X,
                    ball.frameSize.Y
                ),
                Color.White
            );
            
            spriteBatch.Draw(left.texture, left.position, left.rectangle, left.color);
            spriteBatch.Draw(right.texture, right.position, right.rectangle, right.color);
            
            if (ball.isScore)
            {
                spriteBatch.Draw(
                    ball.explosionTexture,
                    ball.explosionPosition,
                    new Rectangle(
                        ball.explosionCurrentFrame.X * ball.frameSize.X,
                        ball.explosionCurrentFrame.Y * ball.frameSize.Y,
                        ball.frameSize.X,
                        ball.frameSize.Y),
                    Color.White
                );
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}