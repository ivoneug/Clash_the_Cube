using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            Vibration.Init();
        }

        public void Vibrate()
        {
            Vibration.VibratePop();
        }
    }
}
