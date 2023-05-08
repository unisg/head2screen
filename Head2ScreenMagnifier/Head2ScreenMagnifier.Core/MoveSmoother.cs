using System;
using System.Numerics;

namespace Head2ScreenMagnifier.Core
{
    public class MoveSmoother
    {
        #region Private Variables

        private Vector2 currentPosition;
        private Vector2 currentTarget;
        private Vector2 thresholdStart;
        private Vector2 thresholdEnd;
        private Vector2 moveStart;

        private bool isMoving;
        private bool lerpIsActive = false;

        private float deceleratorRadius;
        private float lerpCurrentAmount;
        private float lerpAccelerator;
        private float lerpDecelerator;
        private float lerpAmount;

        #endregion

        #region Constructor

        public MoveSmoother()
        {
            this.thresholdStart = new Vector2(1.5f, 1.5f);
            this.thresholdEnd = new Vector2(1f, 1f);
            this.lerpAccelerator = 1.3f;
            this.lerpDecelerator = 0.3f;
            this.lerpAmount = 0.05f;
            this.lerpIsActive = true;
            this.deceleratorRadius = 20f;
        }

        public MoveSmoother(Vector2 thresholdMoveStart, Vector2 thresholdMoveStop, float lerpAmount, float lerpAccelerator, float lerpDecelerator, bool lerpIsActive,float deceleratorRadius)
        {
            this.thresholdStart = thresholdMoveStart;
            this.thresholdEnd = thresholdMoveStop;
            this.lerpAccelerator = lerpAccelerator;
            this.lerpDecelerator = lerpDecelerator;
            this.lerpAmount = lerpAmount;
            this.lerpIsActive = lerpIsActive;
            this.deceleratorRadius = deceleratorRadius;
        }

        #endregion

        #region Public Methods

        public Vector2 Smooth()
        {
            return this.Smooth(this.currentTarget);
        }

        public Vector2 Smooth(Vector2 postion)
        {
            currentTarget = postion;
            var distance = Vector2.Abs(currentPosition - postion);
            var newPos = currentPosition;

            bool moveX = false;
            bool moveY = false;

            if (isMoving)
            {
                if (distance.X > thresholdEnd.X)
                {
                    //   newPos.X = postion.X;
                    moveX = true;
                }
                if (distance.Y > thresholdEnd.Y)
                {
                    //    newPos.Y = postion.Y;
                    moveY = true;
                }
            }
            else
            {
                if ((distance.X > thresholdStart.X))
                {
                    //   newPos.X = postion.X;
                    moveX = true;
                }
                if ((distance.Y > thresholdStart.Y))
                {
                    //    newPos.Y = postion.Y;
                    moveY = true;
                }
            }

            if (moveX || moveY)
            {
                newPos = postion;
                //var doneDistance = Vector2.Distance(moveStart, currentPosition);
                //var totalDistance = Vector2.Distance(moveStart, pos);

                if (isMoving)
                {
                    // var halfDistance = totalDistance / 2;                   
                    if (Vector2.Distance(currentPosition, newPos) > deceleratorRadius) // firstPart of the Move -> Accelerate
                    {
                        lerpCurrentAmount *= lerpAccelerator;
                        if (lerpCurrentAmount > 0.8) lerpCurrentAmount = 0.8f;
                    }
                    else
                    {
                        if (lerpCurrentAmount > lerpAmount)
                        {
                            lerpCurrentAmount *= lerpDecelerator;
                        }
                        else
                        {
                            lerpCurrentAmount = lerpAmount;
                        }
                    }
                }
                else
                {
                    moveStart = currentPosition;
                    isMoving = true;
                    lerpCurrentAmount = lerpAmount;
                }

                if (lerpIsActive)
                {
                    //Vector2 direction = newPos - currentPosition;

                    float directionX = newPos.X - currentPosition.X;
                    float directionY = newPos.Y - currentPosition.Y;

                    float lerpedX = (moveX) ? currentPosition.X + (directionX * lerpCurrentAmount) : currentPosition.X;
                    float lerpedY = (moveY) ? currentPosition.Y + (directionY * lerpCurrentAmount) : currentPosition.Y;

                    Vector2 lerpedPos = new Vector2(lerpedX, lerpedY);
                    currentPosition = lerpedPos;
                }
                else
                {
                    currentPosition = newPos;
                }
            }
            else
            {
                isMoving = false;
            }

            return currentPosition;
        }

        public Vector2 FollowPosition()
        {
            return this.FollowPosition(this.currentTarget);
        }

        public Vector2 FollowPosition(Vector2 postion)
        {
            this.currentTarget = postion;
            var distance = Vector2.Abs(currentPosition - postion);

            if (distance.X > this.thresholdStart.X || distance.Y > this.thresholdStart.Y)
            {
                isMoving = true;
            }

            if (isMoving)
            {
                this.currentPosition += (postion - this.currentPosition) * this.lerpAmount;
            }

            if (isMoving && (distance.X < thresholdEnd.X && distance.Y < thresholdEnd.Y))
            {
                isMoving = false;
            }

            return this.currentPosition;
        }

        #endregion
    }
}