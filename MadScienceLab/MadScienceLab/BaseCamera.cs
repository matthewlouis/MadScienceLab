using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MadScienceLab
{
    public class BaseCamera : GameObject3D
    {
        private const int ZOOM_SPEED = 20;
        private int maxZoom = 1000;
        public Boolean IsZooming { get; set; }

        public Matrix View       { get; protected set; }
        public Matrix Projection { get; protected set; }
        Character followTarget;
        Vector3 originalCameraDistance = new Vector3(0, 30f, 300);
        Vector3 cameraDistance;

        //hard-coded offsets from the sides of the stage that the player would have to be for the camera to move
        //float leftSideCollsion = 150f;
        float leftSideCollsion = GameConstants.SINGLE_CELL_SIZE * 4;
        float topSideCollsion = 50;
        float bottomSideCollsion = GameConstants.SINGLE_CELL_SIZE * 1;

        int xLeftWall;
        int xRightWall;
        int yFloor;

        public BaseCamera(Character followTarget){
            cameraDistance = originalCameraDistance;
            this.followTarget = followTarget;
            //Perspective allows objects to looks smaller/larger depending on distance 
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)GameConstants.X_RESOLUTION / (float)GameConstants.Y_RESOLUTION, GameConstants.MIN_Z, GameConstants.MAX_Z);
            Position = followTarget.Position + cameraDistance;
        }

        public BaseCamera()
        {
            cameraDistance = originalCameraDistance;
            //Perspective allows objects to looks smaller/larger depending on distance 
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)GameConstants.X_RESOLUTION / (float)GameConstants.Y_RESOLUTION, GameConstants.MIN_Z, GameConstants.MAX_Z);
            Position = cameraDistance;
        }

        public virtual void BuildViewMatrix()
        {
            View = Matrix.CreateLookAt(Position, (Position + Vector3.Forward), Vector3.Up);
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);

            if (followTarget != null)
            {
                Position = new Vector3(Position.X, Position.Y, cameraDistance.Z);
                CameraFollow(followTarget);
            }

            BuildViewMatrix(); //Calculate view matrix every frame
            handleInput();
        }

        public void setFollowTarget(Character target)
        {
            followTarget = target;
            //placed here in order to be after the levelbuilder variables are set in Game1 - that's the only reason
            xLeftWall = (GameConstants.SINGLE_CELL_SIZE * LevelBuilder.startWall) - (GameConstants.X_RESOLUTION / 2);
            xRightWall = (GameConstants.SINGLE_CELL_SIZE * (LevelBuilder.levelwidth - 1 + LevelBuilder.startWall)) - (GameConstants.X_RESOLUTION / 2);
            yFloor = (GameConstants.SINGLE_CELL_SIZE * (LevelBuilder.startFloor+1)) - (GameConstants.Y_RESOLUTION / 2);
        }

        public Character getFollowTarget()
        {
            return followTarget;
        }
        
        private void CameraFollow(Character character)
        {
            if (followTarget.Position.Y <= yFloor + bottomSideCollsion)
            {
                Position = new Vector3(Position.X, yFloor + bottomSideCollsion, Position.Z);
            }
            /*
            if (followTarget.Position.Y <= Position.Y)
            {
                 Position = new Vector3(Position.X, followTarget.Position.Y, Position.Z);
            }*/
            else if (followTarget.Position.Y + bottomSideCollsion >= Position.Y + topSideCollsion)
            {
                Position = new Vector3(Position.X, followTarget.Position.Y - topSideCollsion + bottomSideCollsion, Position.Z);
            }
            else
            {
                Position = new Vector3(Position.X, followTarget.Position.Y - topSideCollsion + bottomSideCollsion, Position.Z);
            }

            if (followTarget.Position.X <= xLeftWall + leftSideCollsion)
            {
                Position = new Vector3(xLeftWall + leftSideCollsion, Position.Y, Position.Z);
            }
            else if (followTarget.Position.X >= xRightWall - leftSideCollsion)
            {
                Position = new Vector3(xRightWall - leftSideCollsion, Position.Y, Position.Z);
            }
            else if (followTarget.Position.X >= xLeftWall + leftSideCollsion)
            {
                Position = new Vector3(followTarget.Position.X, Position.Y, Position.Z);
            }


        }
        private void handleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if ((currentKeyboardState.IsKeyDown(Keys.Z) ||
                currentGamePadState.IsButtonDown(Buttons.LeftTrigger) ||
                currentGamePadState.IsButtonDown(Buttons.RightTrigger)))
            {
                if (cameraDistance.Z < maxZoom)
                {
                    zoomOut();
                }
            }else if (IsZooming && cameraDistance.Z > originalCameraDistance.Z &&
                      currentKeyboardState.IsKeyUp(Keys.Z) &&
                      currentGamePadState.IsButtonUp(Buttons.LeftTrigger) &&
                      currentGamePadState.IsButtonUp(Buttons.RightTrigger))
            {
                zoomIn();
            }
        }

        private void zoomOut()
        {
            IsZooming = true;
            cameraDistance = new Vector3(cameraDistance.X, cameraDistance.Y, cameraDistance.Z + ZOOM_SPEED);
        }

        private void zoomIn()
        {
            cameraDistance = new Vector3(cameraDistance.X, cameraDistance.Y, cameraDistance.Z - ZOOM_SPEED);
            if (cameraDistance == originalCameraDistance)
            {
                IsZooming = false;
            }

        }
    }
}
