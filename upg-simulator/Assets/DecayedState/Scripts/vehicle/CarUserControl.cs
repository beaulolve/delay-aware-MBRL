using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
		private CarController m_Car; // the car controller we want to use
		public CharacterControl CharCtrlScript;
		private GameObject Player;
		private CarControl carcntrlScrpt;

        private void Awake()
        {
            // get the car controller
			Player = GameObject.FindGameObjectWithTag("Player");
			CharCtrlScript = Player.GetComponent<CharacterControl> ();
            m_Car = GetComponent<CarController>();
			carcntrlScrpt = this.GetComponent<CarControl> ();
        }

        private void FixedUpdate(){
			if((CharCtrlScript.driving)&&(CharCtrlScript.currCar == this.gameObject)&&(carcntrlScrpt.fuel>0.5f)) {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
			}
			else{
				m_Car.Move(0, 0, 0, 1);
			}
		}
    }
}