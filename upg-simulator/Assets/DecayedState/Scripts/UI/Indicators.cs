using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Indicators : MonoBehaviour {
	public Scrollbar healthBar;
	public Scrollbar hydraBar;
	public Scrollbar fuelBar;
	public GameObject fuelUI;
	private GameObject Player;
	private CharacterControl playrScrpt;
	private CarControl carcontrScrpt;

	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		playrScrpt = Player.GetComponent<CharacterControl> ();
		carcontrScrpt = null;
	}
	
	// Update is called once per frame
	void Update () {
		healthBar.size = playrScrpt.health / 100f;
		hydraBar.size = playrScrpt.hydratation / 100f;
		if (playrScrpt.driving) {
			if(carcontrScrpt == null){
				carcontrScrpt = playrScrpt.currCar.GetComponent<CarControl> ();
			}
			fuelUI.SetActive (true);
			fuelBar.size = carcontrScrpt.fuel / 100f;
		} else {
			fuelUI.SetActive (false);
			carcontrScrpt = null;
		}
	}
}
