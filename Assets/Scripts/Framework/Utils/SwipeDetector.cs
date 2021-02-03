using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private Vector2 activeScreenArea = Vector2.zero;

        public Vector2 Delta { get { return delta; } }

        private Vector2 fingerDown;
        private Vector2 fingerUp;
        private Vector2 delta;

        private bool activeTouch = false;

        private void LateUpdate()
        {
            if (Input.touchCount == 0)
            {
                activeTouch = false;
                return;
            }
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (IsPointerOverUIObject(touch) || !IsAllowedTouchInScreenArea(touch))
                {
                    activeTouch = false;
                    return;
                }
                activeTouch = true;

                fingerUp = touch.position;
                fingerDown = touch.position;

                UpdateDelta();
                UpdateTouchPosition();
                OnTouchDown();
            }

            if (!activeTouch)
            {
                return;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;

                    UpdateDelta();
                    UpdateTouchPosition();
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;

                UpdateDelta();
                UpdateTouchPosition();
                CheckSwipe();
                OnTouchUp();
            }
        }

        private bool IsAllowedTouchInScreenArea(Touch touch)
        {
            if (activeScreenArea == Vector2.zero ||
                (activeScreenArea.x > 0.99f && activeScreenArea.y > 0.99f))
            {
                return true;
            }

            float halfX = activeScreenArea.x / 2f;
            float halfY = activeScreenArea.y / 2f;

            Vector2 position = NormalizedPosition(touch.position);

            if (position.x >= 0.5f - halfX &&
                position.x <= 0.5f + halfX &&
                position.y >= 0.5f - halfY &&
                position.y <= 0.5f + halfY)
            {
                return true;
            }

            return false;
        }

        private void CheckSwipe()
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
                swipeDeltaNormalized.SetValue(NormalizedPosition(delta));
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
                touchPositionNormalized.SetValue(NormalizedPosition(fingerDown));
            }
        }

        #region Callback Functions
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
        #endregion

        #region Helper Methods
        private Vector2 NormalizedPosition(Vector2 position)
        {
            return new Vector2(position.x / (float)Screen.width, position.y / (float)Screen.height);
        }

        /// <summary>
        /// Checks if the pointer, or any touch is over (Raycastable) UI.
        /// WARNING: ONLY WORKS RELIABLY IF IN LateUpdate/LateTickable!
        /// </summary>
        private bool IsPointerOverUIObject()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
            {
                Touch touch = Input.GetTouch(touchIndex);
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPointerOverUIObject(Touch touch)
        {
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        #endregion
    }
}
