using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using RC_Framework;

namespace GPTGame1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texBack = null;
        Texture2D texpaddle = null;
        Texture2D texBall = null;
        Texture2D texEnemy = null;
        Texture2D texGrass1 = null;
        Texture2D texBoom = null;

        SpriteList booms = null;
        SpriteList ballList;
        SpriteList enemyList;
        SpriteFont font;
        //sprite initial position
        float xx = 30;
        float yy = 170;

        float scrollSpeed = -1;
        

        //directory
        string dir = @"C:\Users\salva\Desktop\GPTGame1\GPTGame1\obj\Windows\Content\";

        //keyboard states
        KeyboardState k;
        KeyboardState prevK;

        
        //paddle sprite
        Sprite3 paddle = null;

        //background sprite
        ScrollBackGround back1;
        ScrollBackGround grass1;
        Rectangle playArea;

        //Random
        

        //screen edges 
        int paddleSpeed = 3;
        int lhs = 10;
        int top = 10;
        int rhs = 720;
        int bot = 370;
        
        float delTime;
        float timer = 0;
        float redTimer = 0;
        int spawnTimer = 6;
        //bounding box
        bool showbb = false;
        
        //ball
        Vector2 ballOffset = new Vector2(60, 30);

        //enemy
        float enemyXX = 600;
        Random enemyYY;
        Random enemySpeed;

        //labels
        int score = 0;

        public void NewBall(float x, float y)
        {
            Sprite3 ball = new Sprite3(true, texBall, x, y);
            ball.setBBandHSFractionOfTexCentered(0.4f);
            
            ball.setPos(paddle.getPos() + ballOffset);
            ball.setDeltaSpeed(new Vector2(7, 0));
            
            ballList.addSpriteReuse(ball);

            

        }

        public void NewEnemy(float x, float y)
        {

            
            Sprite3 enemy = new Sprite3(true, texEnemy, x, (float)enemyYY.NextDouble() * y );
            enemy.setWidth(texEnemy.Width * 0.7f);
            enemy.setHeight(texEnemy.Height * 0.7f);
            enemy.setBBToTexture();
            enemy.hitPoints = 3;
            
            enemy.setDeltaSpeed(new Vector2((float)enemySpeed.NextDouble()* -10, 0));
            enemyList.addSpriteReuse(enemy);
            
        }

        void createExplosion(int x, int y)
        {
            float scale = 0.99f;
            int xoffset = -2;
            int yoffset = -20;

            Sprite3 s = new Sprite3(true, texBoom, x + xoffset, y + yoffset);
            s.setXframes(7);
            s.setYframes(3);
            s.setWidthHeight(896 / 7 * scale, 384 / 3 * scale);

            Vector2[] anim = new Vector2[21];
            anim[0].X = 0; anim[0].Y = 0;
            anim[1].X = 1; anim[1].Y = 0;
            anim[2].X = 2; anim[2].Y = 0;
            anim[3].X = 3; anim[3].Y = 0;
            anim[4].X = 4; anim[4].Y = 0;
            anim[5].X = 5; anim[5].Y = 0;
            anim[6].X = 6; anim[6].Y = 0;
            anim[7].X = 0; anim[7].Y = 1;
            anim[8].X = 1; anim[8].Y = 1;
            anim[9].X = 2; anim[9].Y = 1;
            anim[10].X = 3; anim[10].Y = 1;
            anim[11].X = 4; anim[11].Y = 1;
            anim[12].X = 5; anim[12].Y = 1;
            anim[13].X = 6; anim[13].Y = 1;
            anim[14].X = 0; anim[14].Y = 2;
            anim[15].X = 1; anim[15].Y = 2;
            anim[16].X = 2; anim[16].Y = 2;
            anim[17].X = 3; anim[17].Y = 2;
            anim[18].X = 4; anim[18].Y = 2;
            anim[19].X = 5; anim[19].Y = 2;
            anim[20].X = 6; anim[20].Y = 2;
            s.setAnimationSequence(anim, 0, 20, 4);
            s.setAnimFinished(2); // make it inactive and invisible
            s.animationStart();

            booms.addSpriteReuse(s); // add the sprite

        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 380;
            graphics.PreferredBackBufferWidth = 720;

        }

        public static Texture2D texFromFile(GraphicsDevice gd, String fName)
        {
            // note needs :using System.IO;
            Stream fs = new FileStream(fName, FileMode.Open);
            Texture2D rc = Texture2D.FromStream(gd, fs);
            fs.Close();
            return rc;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LineBatch.init(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            texBack = Util.texFromFile(GraphicsDevice, dir + "ricefield.png");
            texpaddle = Util.texFromFile(GraphicsDevice, dir + "slime.png");
            texBall = Util.texFromFile(GraphicsDevice, dir + "slime_slice.png"); //***
            texEnemy = Util.texFromFile(GraphicsDevice, dir + "ghost.png");
            texGrass1 = Util.texFromFile(GraphicsDevice, dir + "grass1.png");
            texBoom = Util.texFromFile(GraphicsDevice, dir + "Boom3.png");
            font = Content.Load<SpriteFont>("incoming");

            paddle = new Sprite3(true, texpaddle, lhs, yy);
            paddle.setBBToTexture();

            enemyList = new SpriteList();
            ballList = new SpriteList();
            booms = new SpriteList();

            back1 = new ScrollBackGround(texBack, texBack.Bounds, new Rectangle(0,0, rhs, 380), -1, 2);
            grass1 = new ScrollBackGround(texGrass1, texGrass1.Bounds, new Rectangle(0, bot - (texGrass1.Height + 30), rhs, bot/2), -1, 2);
            playArea = new Rectangle(lhs, top, rhs, bot- 10);
            enemyYY = new Random();
            enemySpeed = new Random();





        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            prevK = k;
            k = Keyboard.GetState();
            
            delTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            redTimer += delTime;
            
            timer += delTime;





            if ((timer > (float)spawnTimer))
            {
                NewEnemy(enemyXX, bot - texEnemy.Height);
                timer = 0;
                



            }

            
            







            back1.Update(gameTime);
            grass1.Update(gameTime);

            

            

            
            //player collision
            int plCol = enemyList.collisionAA(paddle);

            


            if (plCol != -1)
            {

                Sprite3 temp = enemyList.getSprite(plCol);
                createExplosion((int)temp.getPosX(), (int)temp.getPosY());
                temp.active = false;
                temp.visible = false;
                score += 1;
                
                if (spawnTimer > 1)
                {
                    spawnTimer -= 1;
                }

                else
                {
                    spawnTimer = 1;

                } 
            }

            for (int j = 0; j < enemyList.count(); j++)
            {
                Sprite3 currentEnemy = enemyList.getSprite(j);

                currentEnemy.varInt0 += delTime;
                float prevPosX = currentEnemy.getPosX();
                if (currentEnemy.varInt0 > 0.2f)
                {
                    currentEnemy.setColor(Color.White);
                    //redTimer = 0;
                    currentEnemy.setPosX(prevPosX);
                    currentEnemy.varInt0 = 0;
                    
                }
            }
            

            for(int i = 0; i < ballList.count(); i++)
            {
                Sprite3 tempBullet = ballList.getSprite(i);
                int enemyCollided = enemyList.collisionAA(tempBullet);
                


                if (enemyCollided != -1)
                {
                    Sprite3 tempEnemy = enemyList.getSprite(enemyCollided);

                    tempBullet.active = false;
                    tempBullet.visible = false;
                    
                    
                    tempEnemy.setColor(Color.Red);
                    tempEnemy.setPosX(tempEnemy.getPosX() + 10);
                    


                    tempEnemy.hitPoints -= 1;

                    
                    
                    if (tempEnemy.hitPoints < 1)
                    {
                        createExplosion((int)tempBullet.getPosX(), (int)tempBullet.getPosY());
                        tempEnemy.active = false;
                        tempEnemy.visible = false;
                        score += 1;
                        
                        if (spawnTimer > 1)
                        {
                            spawnTimer -= 1;
                        }

                        else
                        {
                            spawnTimer = 1;

                        } 
                    }
                    

                    

                    

                }

                
                


            }
            

            

            enemyList.moveDeltaXY();
            enemyList.removeIfOutside(playArea);
            ballList.moveDeltaXY();
            ballList.removeIfOutside(playArea);




            if (k.IsKeyDown(Keys.Left))
            {
                scrollSpeed = 0.5f;
                back1.setScrollSpeed(scrollSpeed);
                grass1.setScrollSpeed(1);
            } 


            if (k.IsKeyDown(Keys.Up))
            {
                if (paddle.getPosY() > top + 1) paddle.setPosY(paddle.getPosY() - paddleSpeed);

            }

            else if (k.IsKeyDown(Keys.Down))
            {
                //if (paddle.getPosY() < ((bot- top) - (texpaddle.Height + 2))) paddle.setPosY(paddle.getPosY() + paddleSpeed);
                if (paddle.getPosY() <= playArea.Height - texpaddle.Height + 7) paddle.setPosY(paddle.getPosY() + paddleSpeed);
            }

            else if (k.IsKeyDown(Keys.Right))
            {
                scrollSpeed = -1;
                back1.setScrollSpeed(scrollSpeed);
                grass1.setScrollSpeed(-30);
            }
            else if (k.IsKeyDown(Keys.Left))
            {
                scrollSpeed = 0.5f;
                back1.setScrollSpeed(scrollSpeed);
                grass1.setScrollSpeed(1);
            }



            if (k.IsKeyDown(Keys.Space) && prevK.IsKeyUp(Keys.Space))
            {

                NewBall(xx, yy);



            }
            if (k.IsKeyDown(Keys.B) && prevK.IsKeyUp(Keys.B)) // ***
            {
                showbb = !showbb;
            }

            





            booms.animationTick(gameTime);
            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();


            back1.Draw(spriteBatch); //***
            grass1.Draw(spriteBatch);
            paddle.Draw(spriteBatch); //***
            enemyList.Draw(spriteBatch);
            ballList.Draw(spriteBatch);
            booms.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Enemies Slain: " + score, new Vector2(rhs - 140, top + 10), Color.Black);

            if (showbb)
            {
                paddle.drawBB(spriteBatch, Color.Black);
                paddle.drawHS(spriteBatch, Color.Green);
                
                
                enemyList.drawInfo(spriteBatch, Color.Red, Color.Yellow);
                LineBatch.drawLineRectangle(spriteBatch, playArea, Color.Blue);
                ballList.drawInfo(spriteBatch, Color.Gray, Color.Green);
                
            }

               


            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
