using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Events;
using Framework.Variables;

namespace Framework.Utils
{
    public class SwipeDetector : MonoBehaviour
    {
        [SerializeField] private GameEvent swipeLeftEvent;
        [SerializeField] private GameEvent swipeRightEvent;
        [SerializeField] private GameEvent swipeUpEvent;
        [SerializeField] private GameEvent swipeDownEvent;
        [SerializeField] private GameEvent touchDownEvent;
        [SerializeField] private GameEvent touchUpEvent;

        [SerializeField] private Vector2Variable touchPosition;
        [SerializeField] private Vector2Variable touchPositionNormalized;
        [SerializeField] private Vector2Variable swipeDelta;
        [SerializeField] private Vector2Variable swipeDeltaNormalized;

        [SerializeField] private bool detectSwipeOnlyAfterRelease = false;
        [SerializeField] private float SWIPE_THRESHOLD = 20f;

        public Vector2 Delta { get { return delta; } }

        private Vector2 fingerDown;
        private Vector2 fingerUp;
        private Vector2 delta;

        private void Update()
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUp = touch.position;
                    fingerDown = touch.position;

                    UpdateDelta();
                    UpdateTouchPosition();
                    OnTouchDown();
                }

                //Detects Swipe while finger is still moving
                if (touch.phase == TouchPhase.Moved)
                {
                    if (!detectSwipeOnlyAfterRelease)
                    {
                        fingerDown = touch.position;

                        UpdateDelta();
                        UpdateTouchPosition();
                        checkSwipe();
                    }
                }

                //Detects swipe after finger is released
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;

                    UpdateDelta();
                    UpdateTouchPosition();
                    checkSwipe();
                    OnTouchUp();
                }
            }
        }

        private void checkSwipe()
        {
            //Check if Vertical swipe
            if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
            {
                float delta = yDelta();

                if (delta > 0) // up swipe
                {
                    OnSwipeUp();
                }
                else if (delta < 0) // down swipe
                {
                    OnSwipeDown();
                }

                fingerUp = fingerDown;
            }

            //Check if Horizontal swipe
            else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
            {
                float delta = xDelta();

                if (delta > 0) // right swipe
                {
                    OnSwipeRight();
                }
                else if (delta < 0) // left swipe
                {
                    OnSwipeLeft();
                }

                fingerUp = fingerDown;
            }
            //No Movement at-all
            else
            {
                //Debug.Log("No Swipe!");
            }
        }

        private float verticalMove()
        {
            return Mathf.Abs(fingerDown.y - fingerUp.y);
        }

        private float horizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.x);
        }

        private float xDelta()
        {
            return fingerDown.x - fingerUp.x;
        }

        private float yDelta()
        {
            return fingerDown.y - fingerUp.y;
        }

        private void UpdateDelta()
        {
            delta = new Vector2(xDelta(), yDelta());
            if (swipeDelta)
            {
                swipeDelta.SetValue(delta);
            }
            if (swipeDeltaNormalized)
            {
                swipeDeltaNormalized.SetValue(new Vector2(delta.x / (float)Screen.width, delta.y / (float)Screen.height));
            }
        }

        private void UpdateTouchPosition()
        {
            if (touchPosition)
            {
                touchPosition.SetValue(fingerDown);
            }
            if (touchPositionNormalized)
            {
                touchPositionNormalized.SetValue(new Vector2(fingerDown.x / (float)Screen.width, fingerDown.y / (float)Screen.height));
            }
        }

        //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
        private void OnTouchDown()
        {
            // Debug.Log("Touch down");
            if (touchDownEvent)
            {
                touchDownEvent.Raise();
            }
        }

        private void OnTouchUp()
        {
            // Debug.Log("Touch up");
            if (touchUpEvent)
            {
                touchUpEvent.Raise();
            }
        }

        private void OnSwipeUp()
        {
            // Debug.Log("Swipe up: " + yDelta());
            if (swipeUpEvent)
            {
                swipeUpEvent.Raise();
            }
        }

        private void OnSwipeDown()
        {
            // Debug.Log("Swipe down: " + yDelta());
            if (swipeDownEvent)
            {
                swipeDownEvent.Raise();
            }
        }

        private void OnSwipeLeft()
        {
            // Debug.Log("Swipe left: " + xDelta());
            if (swipeLeftEvent)
            {
                swipeLeftEvent.Raise();
            }
        }

        private void OnSwipeRight()
        {
            // Debug.Log("Swipe right: " + xDelta());
            if (swipeRightEvent)
            {
                swipeRightEvent.Raise();
            }
        }
    }
}
