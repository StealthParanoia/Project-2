using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Project_2._0
{
	public partial class GameControl : UserControl
	{

		// Jordan TODO - Background images, change models, visual effect upon shell contact
		// Adjust the ramming damage and increase the speed at which it slows down
		// Make the ship look better - need to tell which end of the ship you're controlling (add a bowsprit, et cetera)

		#region Constructors

		public GameControl()
		{
			InitializeComponent();
		}

		#endregion

		#region Fields

		private GameData _data;

		private bool _activated;

		private bool _deactivated;

		private bool _deactivationRequested;

		private Stopwatch _stopwatch;

		private double _lastUpdateTime;

		private volatile bool _framerateLimiter = true;

		private long _lastFpsCalcFrame = 0;

		private double _lastFpsCalcTime;

		private long _frameNumber;

		private volatile float _frameRate;

		private volatile bool _haveFrameRate;

		private volatile bool _updateRequired;

		private readonly object _updateRequiredLock = new object();

		private SynchronizationContext _context;

		private Region _updateRegion;

		private readonly object _updateLock = new object();

		private int _regionUnionCount;

		private bool _upPressed;

		private bool _downPressed;

		private bool _leftPressed;

		private bool _rightPressed;

		private bool _spacePressed;

		#endregion

		#region Properties

		public GameData Data
		{
			get { return _data; }
			set { _data = value; }
		}

		#endregion

		#region Events

		public event EventHandler Deactivated;

		#endregion

		#region Methods - Activation & Deactivated

		public void Activate()
		{
			if (_activated)
				return;

			if (_deactivationRequested)
				return;

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);

			_context = SynchronizationContext.Current;

			var threadStart = new ThreadStart(BackgroundUpdateWorker);
			var updateThread = new Thread(threadStart);
			updateThread.Start();

			_activated = true;

			this.Invalidate();
		}

		public void Deactivate()
		{
			_deactivationRequested = true;
		}

		public bool IsActive()
		{
			return _activated && !_deactivated;
		}

		#endregion

		#region Methods - Region Processing & Update

		/// <summary>
		/// Processes the update region determined by the last update cycle.
		/// Must be invoked on the UI thread.
		/// </summary>
		private void ProcessUpdateRegion()
		{
			lock (_updateLock)
			{
				if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
				{
					// Check the control hasn't been disposed while this was queued.
					return;
				}

				Region region;
				bool updateRequired;

				var unionCount = _regionUnionCount;

				lock (_updateRequiredLock)
				{
					if (_regionUnionCount > 10)
					{
						region = null;
					}
					else
					{
						region = _updateRegion;
					}

					updateRequired = _updateRequired;
					_updateRequired = false;
					_updateRegion = null;
					_regionUnionCount = 0;
				}

				if (!updateRequired)
				{
					return;
				}

				if (region != null)
				{
					Invalidate(region);
				}
				else
				{
					Invalidate();
				}
			}
		}

		public void UpdateRequired()
		{
			lock (_updateRequiredLock)
			{
				_updateRequired = true;
				_updateRegion = null;
				_regionUnionCount = 0;
			}
		}

		public void UpdateRequired(Region region)
		{
			lock (_updateRequiredLock)
			{
				if (_updateRequired && _updateRegion == null)
				{
					return; // Entire frame is being updated, nothing to do.
				}

				_updateRequired = true;

				if (_updateRegion != null)
				{
					_regionUnionCount++;
					_updateRegion.Union(region);
				}
				else
				{
					_updateRegion = region;
				}
			}
		}

		public void UpdateRequired(RectangleF rect)
		{
			//			UpdateRequired();
			//			return;
			lock (_updateRequiredLock)
			{
				if (_updateRequired && _updateRegion == null)
				{
					return; // Entire frame is being updated, nothing to do.
				}

				_updateRequired = true;
				if (_updateRegion != null)
				{
					_regionUnionCount++;
					_updateRegion.Union(rect);
				}
				else
				{
					_updateRegion = new Region(rect);
				}
			}
		}

		/// <summary>
		/// Calls the process update region on the UI thread.
		/// </summary>
		private void ProcessUpdateRegionCaller()
		{
			_context.Post(delegate { ProcessUpdateRegion(); }, null);
		}

		/// <summary>
		/// Runs on its worn thread and acts as the main graphics loop.
		/// </summary>
		public void BackgroundUpdateWorker()
		{
			// Currently using a stopwatch, seems to work ok for this purpose but we might want a better solution at some point.
			_stopwatch = new Stopwatch();
			_stopwatch.Start();

			_lastUpdateTime = _stopwatch.Elapsed.TotalMilliseconds;

			var sleepAdjuster = 1.0;

			try
			{
				while (!_deactivationRequested)
				{
					try
					{
						if (this.Disposing || this.IsDisposed)
						{
							// If we get here it probably means the caller forgot to call Deactivate
							return;
						}

						while (!this.IsHandleCreated)
						{
							if (this.Disposing || this.IsDisposed)
							{
								return;
							}
							Thread.Sleep(5);
						}

						// Wait for 16 ms to elapse (limits to a max of about 60fps)
						var newTime = _stopwatch.Elapsed.TotalMilliseconds;

						var millisecondsElapsed = newTime - _lastUpdateTime;

						if (millisecondsElapsed < 0)
							millisecondsElapsed = 0;

						// Make sure all pending ui events are processed, no point doing these updates if
						// Windows doesn't give the UI thread enough time to draw them.
						Application.DoEvents();

						if (_framerateLimiter)
						{
							// We always sleep for slightly shorter than actually required because there is a delay in sleeping and resuming threads.
							if (millisecondsElapsed < 14)
							{
								Thread.Sleep(Math.Max(1, (int)((14 - millisecondsElapsed) * sleepAdjuster)));
								newTime = _stopwatch.ElapsedMilliseconds;
								millisecondsElapsed = newTime - _lastUpdateTime;
							}
							else
							{
								Thread.Sleep(1);	// Always put in a sleep, otherwise we can make the entire system unusable.
							}
						}
						else
						{
							Thread.Sleep(1);	// Always put in a sleep, otherwise we can make the entire system unusable.
							newTime = _stopwatch.ElapsedMilliseconds;
							millisecondsElapsed = newTime - _lastUpdateTime;
						}

						if (millisecondsElapsed < 0)
							millisecondsElapsed = 0;


						// The length of time it takes to sleep and resume a thread appears to differ significantly between systems.
						// The sleep adjuster is an attempt to dynamically take this into account. If it is taking too long it will
						// sleep less.
						if (_framerateLimiter)
						{
							if (millisecondsElapsed > 16.66666667)
							{
								if (sleepAdjuster > 0.1)
								{
									sleepAdjuster -= 0.1;

									if (sleepAdjuster < 0.1)
										sleepAdjuster = 0.1;
								}
							}

							if (millisecondsElapsed < 14)
							{
								if (sleepAdjuster < 1.0)
								{
									sleepAdjuster += 0.1;
									if (sleepAdjuster > 1.0)
										sleepAdjuster = 1.0;
								}
							}


							while (millisecondsElapsed < 16.66666667)
							{
								newTime = _stopwatch.Elapsed.TotalMilliseconds;
								millisecondsElapsed = newTime - _lastUpdateTime;
							}
						}

						if (newTime - _lastFpsCalcTime > 250 && (_frameNumber - _lastFpsCalcFrame) > 0)
						{
							// Recalc fps
							var framesSinceLastCalc = _frameNumber - _lastFpsCalcFrame;

							_frameRate = (float)(framesSinceLastCalc / (newTime - _lastFpsCalcTime) * 1000);

							_lastFpsCalcTime = newTime;
							_lastFpsCalcFrame = _frameNumber;
							_haveFrameRate = true;
						}


						// Move on to the next animation timestep.
						PerformAnimationStep(millisecondsElapsed);

						//                        PerformSmoothZooming(millisecondsElapsed);

						_lastUpdateTime = newTime;

						//if (_benchmarkingMode)
						//{
						//    UpdateRequired();
						//}
						//else
						//{
						//    DetermineAnimationRegion();
						//}

						// Find out if anything we did up above means we need to do an update.
						bool updateRequired;

						lock (_updateRequiredLock)
						{
							//							if (!_updateRequired)
							{
								//								if (_drawableItems.Count> 0)
								{
									//									UpdateRequired(_drawableItems[2].GetScreenRect());
								}
							}

							//							UpdateRequired();

							updateRequired = _updateRequired;
						}

						if (updateRequired)
						{
							ProcessUpdateRegionCaller();
						}



						if (!updateRequired)
						{
							// Frame rate information is useless if we aren't doing any drawing this cycle
							_lastFpsCalcTime = newTime;
							_lastFpsCalcFrame = _frameNumber;
							_haveFrameRate = false;
						}

						// Post animation step.
						PostAnimationStep();

					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}
			}
			finally
			{
				_deactivated = true;

				InvokeHelper.SendEvent(this, new EventArgs(), Deactivated);
			}
		}

		#endregion

		#region Methods - Painting

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_data == null) { return; }

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			foreach (var cannonball in _data.Cannonballs)
			{
				DrawCannonball(e.Graphics, cannonball);
			}

			foreach (var ship in _data.Ships)
			{
				if (ship.Health <= 0)
				{
					DrawWreckage(e.Graphics, ship);
					continue;
				}

				DrawShip(e.Graphics, ship, ship.DColour, ship.FColour);

			}



		}

		private void DrawShip(Graphics g, Ship t, Pen dc, Brush fc)
		{

			// Make it look like a ship

			Pen drawingPen = new Pen(fc);
			Point[] bowPoints = new Point[4] { new Point(20, 20), new Point(0, 50), new Point(0, 50), new Point(-20, 20) };
			Point[] sternPoints = new Point[4] { new Point(-20, -20), new Point(0, -50), new Point(0, -50), new Point(20, -20) };

			g.TranslateTransform(t.X, t.Y);
			g.RotateTransform(t.Angle);

			// Ship's Hull

			var shipHull = new Rectangle(-20, -20, 40, 40);
			g.DrawRectangle(dc, shipHull);
			g.FillRectangle(fc, shipHull);

			// Ship's Bow

			g.DrawLine(drawingPen, bowPoints[0], bowPoints[1]);
			g.DrawLine(drawingPen, bowPoints[2], bowPoints[3]);
			g.FillPolygon(fc, bowPoints);

			// Ship's Stern

			g.DrawLine(drawingPen, sternPoints[0], sternPoints[1]);
			g.DrawLine(drawingPen, sternPoints[2], sternPoints[3]);
			g.FillPolygon(fc, sternPoints);
			

			// TODO - Re-draw this

			/* var shipCannon = new Rectangle(0, 20, 2, 30);
			g.DrawRectangle(Pens.Blue, shipCannon);
			g.FillRectangle(new SolidBrush(Color.Blue), shipCannon); */

			g.ResetTransform();
		}

		private void DrawCannonball(Graphics g, Cannonball s)
		{
			g.TranslateTransform(s.X, s.Y);
			g.RotateTransform(s.Angle);

			var shipCannonball = new Rectangle(0, 5, 2, 10);
			g.DrawRectangle(Pens.Yellow, shipCannonball);
			g.FillRectangle(new SolidBrush(Color.Yellow), shipCannonball);

			g.ResetTransform();
		}

		private void DrawWreckage(Graphics g, Ship t)
		{
			g.TranslateTransform(t.X, t.Y);
			g.RotateTransform(t.Angle);

			var shipWreckage = new Rectangle(-20, -20, 40, 40);
			g.DrawRectangle(Pens.Brown, shipWreckage);
			g.FillRectangle(new SolidBrush(Color.Brown), shipWreckage);

			g.ResetTransform();
		}

		#endregion

		#region Methods - Movement & Animation

		/// <summary>
		/// Performed before drawing to move all animations onto the next timestep.
		/// </summary>
		/// <param name="elapsedMilliseconds">Time elapsed.</param>
		private void PerformAnimationStep(double elapsedMilliseconds)
		{
			if (_leftPressed)
			{
				_data.Ships[2].Angle -= (float)(elapsedMilliseconds / 1000) * _data.Settings.TurnSpeed;

				if (_data.Ships[2].Angle < 0)
				{
					_data.Ships[2].Angle += 360;
				}
			}

			if (_rightPressed)
			{
				_data.Ships[2].Angle += (float)(elapsedMilliseconds / 1000) * _data.Settings.TurnSpeed;

				if (_data.Ships[2].Angle > 360)
				{
					_data.Ships[2].Angle -= 360;
				}
			}

			if (_upPressed)
			{
				_data.Ships[2].Speed += (float)(elapsedMilliseconds / 1000) * _data.Settings.Acceleration;

				if (_data.Ships[2].Speed > _data.Settings.MaxSpeed)
					_data.Ships[2].Speed = _data.Settings.MaxSpeed;
			}

			if (_downPressed)
			{
				_data.Ships[2].Speed -= (float)(elapsedMilliseconds / 1000) * _data.Settings.Deceleration;

				if (_data.Ships[2].Speed < -_data.Settings.MaxReverseSpeed)
					_data.Ships[2].Speed = -_data.Settings.MaxReverseSpeed;
			}
			if (_spacePressed)
			{
				if (_data.Ships[2].Reload <= 0)
				{
					_data.Cannonballs.Add(_data.Ships[2].FireCannonball(_data.Settings));				
				}				
			}

			if (_data.Ships[2].Reload > 0)
			{
				_data.Ships[2].Reload -= (float)elapsedMilliseconds / 1000F;
			}

			foreach (var ship in _data.Ships)
			{
				var originalLocation = new PointF(ship.X, ship.Y);

				ship.MoveShip((float)elapsedMilliseconds / 1000F);

				var newLocation = new PointF(ship.X, ship.Y);

				var velocity = new Vector(newLocation.X - originalLocation.X, newLocation.Y - originalLocation.Y);

				var tankPolygon = new Polygon();
				tankPolygon.Points.Add(new Vector(-20, -20));
				tankPolygon.Points.Add(new Vector(20, -20));
				tankPolygon.Points.Add(new Vector(20, 20));
				tankPolygon.Points.Add(new Vector(-20, 20));
				tankPolygon.Rotate(ship.Angle);
				tankPolygon.Offset(ship.X, ship.Y);
				tankPolygon.BuildEdges();

				// Check for collisions with other tanks.

				foreach(var otherShip in _data.Ships)
				{
					if (otherShip == ship)
						continue;

					var otherShipPolygon_hull = new Polygon();
					otherShipPolygon_hull.Points.Add(new Vector(-20, -20));
					otherShipPolygon_hull.Points.Add(new Vector(20, -20));
					otherShipPolygon_hull.Points.Add(new Vector(20, 20));
					otherShipPolygon_hull.Points.Add(new Vector(-20, 20));
					otherShipPolygon_hull.Rotate(otherShip.Angle);
					otherShipPolygon_hull.Offset(otherShip.X, otherShip.Y);
					otherShipPolygon_hull.BuildEdges();

					// Point(20, 20), new Point(0, 50), new Point(0, 50), new Point(-20, 20)

					var otherShipPolygon_bow = new Polygon();
					otherShipPolygon_bow.Points.Add(new Vector(20, 20));
					otherShipPolygon_bow.Points.Add(new Vector(0, 50));
					otherShipPolygon_bow.Points.Add(new Vector(0, 50));
					otherShipPolygon_bow.Points.Add(new Vector(-20, 20));
					otherShipPolygon_bow.Rotate(otherShip.Angle);
					otherShipPolygon_bow.Offset(otherShip.X, otherShip.Y);
					otherShipPolygon_bow.BuildEdges();

					var otherShipPolygon_stern = new Polygon();
					otherShipPolygon_stern.Points.Add(new Vector(-20, -20));
					otherShipPolygon_stern.Points.Add(new Vector(0, -50));
					otherShipPolygon_stern.Points.Add(new Vector(0, -50));
					otherShipPolygon_stern.Points.Add(new Vector(20, -20));
					otherShipPolygon_stern.Rotate(otherShip.Angle);
					otherShipPolygon_stern.Offset(otherShip.X, otherShip.Y);
					otherShipPolygon_stern.BuildEdges();

					// TODO - Add otherShipPolygon_bow, otherShipPolygon_stern, to a Polygon collision detection

					PolygonCollisionResult r = Collisions.PolygonCollision(tankPolygon, otherShipPolygon_hull, velocity);

					if (r.WillIntersect)
					{
						//playerTranslation = velocity + r.MinimumTranslationVector;

							ship.X += velocity.X + r.MinimumTranslationVector.X / 2.0F;
							ship.Y += velocity.Y + r.MinimumTranslationVector.Y / 2.0F;

							otherShip.X -= r.MinimumTranslationVector.X / 2.0F;
							otherShip.Y -= r.MinimumTranslationVector.Y / 2.0F;

							// Slow the tank down since it's hit it.

							if (ship.Speed > 0)
							{

								if (ship.Speed > _data.Settings.MaxSpeed / 2)
								{
									otherShip.Health -= (ship.Speed - (_data.Settings.MaxSpeed / 2)) * (float) (1000.0 * (elapsedMilliseconds / 100000));
								}
								
								ship.Speed -= (float)(1000.0 * (elapsedMilliseconds / 1000));								

								if (ship.Speed < 0)
								{
									ship.Speed = 0;
								}
							}

							break;

					}
				}
			}

			for (var i = 0; i < _data.Cannonballs.Count; i++)
			{
				// TODO: Update required for old location

				var shell = _data.Cannonballs[i];
				
				var velocity = shell.GetMovementVector((float)elapsedMilliseconds/1000F);

				// Find out if this overlaps any tank except its owner

				
				foreach(var ship in _data.Ships)
				{
					if (ship == shell.Ship)
						continue;

					var shipPolygon_hull = new Polygon();
					shipPolygon_hull.Points.Add(new Vector(-20, -20));
					shipPolygon_hull.Points.Add(new Vector(20, -20));
					shipPolygon_hull.Points.Add(new Vector(20, 20));
					shipPolygon_hull.Points.Add(new Vector(-20, 20));
					shipPolygon_hull.Rotate(ship.Angle);
					shipPolygon_hull.Offset(ship.X, ship.Y);
					shipPolygon_hull.BuildEdges();

					var shipPolygon_bow = new Polygon();
					shipPolygon_bow.Points.Add(new Vector(20, 20));
					shipPolygon_bow.Points.Add(new Vector(0, 50));
					shipPolygon_bow.Points.Add(new Vector(0, 50));
					shipPolygon_bow.Points.Add(new Vector(-20, 20));
					shipPolygon_bow.Rotate(ship.Angle);
					shipPolygon_bow.Offset(ship.X, ship.Y);
					shipPolygon_bow.BuildEdges();

					var shipPolygon_stern = new Polygon();
					shipPolygon_stern.Points.Add(new Vector(-20, -20));
					shipPolygon_stern.Points.Add(new Vector(0, -50));
					shipPolygon_stern.Points.Add(new Vector(0, -50));
					shipPolygon_stern.Points.Add(new Vector(20, -20));
					shipPolygon_stern.Rotate(ship.Angle);
					shipPolygon_stern.Offset(ship.X, ship.Y);
					shipPolygon_stern.BuildEdges();

					// TODO - Add shipPolygon_bow, shipPolygon_stern, to a Polygon collision detection 

					PolygonCollisionResult r = Collisions.PolygonCollision(shell.cannonballPolygon, shipPolygon_hull, velocity);

					if (r.WillIntersect)
					{
						//playerTranslation = velocity + r.MinimumTranslationVector;
						// No need to do a translation, this is a hit!
						ship.Health -= _data.Settings.HitDamage;
						shell.Life = 0;
						break;
					}
				}

				shell.MoveCannonball(velocity);

				shell.Life -= (float)elapsedMilliseconds / 1000F;

				if (shell.Life <= 0)
				{
					_data.Cannonballs.RemoveAt(i);
					i--;
				}

				// TODO: Update required for new location
			}

			UpdateRequired();

		}

		/// <summary>
		/// Performed after the drawing region has been established for the current frame.
		/// </summary>
		private void PostAnimationStep()
		{
			// TODO: Post animation.
		}

		#endregion

		#region Methods - Key Processing

		private void GameControl_KeyUp(object sender, KeyEventArgs e)
		{
			
			if (e.KeyCode == Keys.Up)
			{
				_upPressed = false;
			}
			if (e.KeyCode == Keys.Down)
			{
				_downPressed = false;
			}
			if (e.KeyCode == Keys.Left)
			{
				_leftPressed = false;
			}
			if (e.KeyCode == Keys.Right)
			{
				_rightPressed = false;
			}
			if (e.KeyCode == Keys.Space)
			{
				_spacePressed = false;
			}

		}


		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (msg.Msg == 256)
			{
				if (keyData == Keys.Up)
				{
					_upPressed = true;
					return true;
				}
				if (keyData == Keys.Down)
				{
					_downPressed = true;
					return true;
				}
				if (keyData == Keys.Left)
				{
					_leftPressed = true;
					return true;
				}
				if (keyData == Keys.Right)
				{
					_rightPressed = true;
					return true;
				}
				if (keyData == Keys.Space)
				{
					_spacePressed = true;
					return true;
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion
	}
}