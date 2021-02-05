using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                Vibration.Init();
            }
            catch { }
        }

        public void Vibrate()
        {
            try
            {
                Vibration.VibratePop();
            }
            catch { }
        }
    }
}
