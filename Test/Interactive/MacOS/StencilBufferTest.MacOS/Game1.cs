#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace StencilBufferTest.MacOS {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D _maskTex;
		Texture2D _bgTex;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = true;
			graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			_maskTex = Content.Load<Texture2D>( "mask" );		
			_bgTex = Content.Load<Texture2D>( "background" );	
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState ().IsKeyDown (Keys.Escape)) {
				Exit ();
			}
			#endif
			// TODO: Add your update logic here			
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear(ClearOptions.Target 
				| ClearOptions.Stencil, Color.Transparent, 0, 0);

			var m = Matrix.CreateOrthographicOffCenter(0,
				graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
				graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
				0, 0, 1
			);

			var a = new AlphaTestEffect(graphics.GraphicsDevice) {
				DiffuseColor = Color.White.ToVector3(),
				AlphaFunction = CompareFunction.Greater,
				ReferenceAlpha = 0,
				Projection = m
			};

			var s1 = new DepthStencilState {
				StencilEnable = true,
				StencilFunction = CompareFunction.Always,
				StencilPass = StencilOperation.Replace,
				ReferenceStencil = 1,
				DepthBufferEnable = false,
			};

			var s2 = new DepthStencilState {
				StencilEnable = true,
				StencilFunction = CompareFunction.LessEqual,
				StencilPass = StencilOperation.Keep,
				ReferenceStencil = 1,
				DepthBufferEnable = false,
			};

			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, s1, null, a);
			spriteBatch.Draw(_maskTex, Vector2.Zero, Color.White); //The mask                                   
			spriteBatch.End();

			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, s2, null, a);            
			spriteBatch.Draw(_bgTex, Vector2.Zero, Color.White); //The background
			spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

