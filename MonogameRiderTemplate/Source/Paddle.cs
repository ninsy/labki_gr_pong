using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheVillageOfLife
{
    public class Paddle
    {
        public Texture2D texture;
        public int width { get; set; }
        public int height { get; set; }
        public Vector2 position { get; set; }

        public Rectangle rectangle;
        public List<Rectangle> sections;
        public int sectionCount = 5;
        
        public enum Actions { moveUp, moveDown }
        public Dictionary<Actions, Keys> keyboardActions { get; set; }
        
        public int score;

        public Color color;
        public int highlightTime = 300;
        public int highlightTimeElapsed = 0;
        public bool beingHit = false;
        public string side;
        
        public Paddle(string s, Keys moveUpKey, Keys moveDownKey, Vector2 pos, Texture2D textr)
        {
            texture = textr;
            
            score = 0;
            side = s;
            keyboardActions = new Dictionary<Actions, Keys>();
            keyboardActions.Add(Actions.moveUp, moveUpKey);
            keyboardActions.Add(Actions.moveDown, moveDownKey);

            
            sections = new List<Rectangle>();
            color = Color.White;
            
            position = pos;
            
            initSections();
        }

        public void Move(KeyboardState ks, int viewportHeight)
        {
            if (ks.IsKeyDown(keyboardActions[Actions.moveUp]) && (position.Y + 10 + texture.Height) < viewportHeight)
            {
                position = new Vector2(position.X, position.Y + 10);
            }
            if (ks.IsKeyDown(keyboardActions[Actions.moveDown]) && position.Y - 10 > 0)
            {
                position = new Vector2(position.X, position.Y - 10);
            }

            setSections();

        }

        public void initSections()
        {
            for (int i = 0; i < sectionCount; i++)
            {
                sections.Add(new Rectangle(
                    (int)position.X,
                    (int)position.Y + i * texture.Height / sectionCount,
                    texture.Width,
                    texture.Height / sectionCount
                ));
            }
        }
        
        public void setSections()
        {
            for (int i = 0; i < sectionCount; i++)
            {
                sections[i] = new Rectangle(
                    (int)position.X,
                    (int)position.Y + i * texture.Height / sectionCount,
                    texture.Width,
                    texture.Height / sectionCount
                );
            }
        }

        public void highlight(int miliseconds)
        {
            if (beingHit)
            {
                highlightTimeElapsed += miliseconds;
                if (highlightTimeElapsed >= highlightTime)
                {
                    beingHit = false;
                    highlightTimeElapsed = 0;
                    color = Color.White;
                }
            }
        }
    }
}