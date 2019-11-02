using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AiPlayer : MonoBehaviour, PlayerInterface {

	bool skip=false;
	bool drew =false;
	string name;
	string colorToPlay;
	int locationCardPlayed=-1;
	List<Card> handList = new List<Card> ();

	public AiPlayer(string name) { //initializes
		this.name = name;
	}

	public bool skipStatus {
		get{return skip; }
		set{ skip = value; }
	}

	public void turn() { //the ai's turn
		Card currDisc = Control.discard [Control.discard.Count - 1];
		string colorDisc = currDisc.getColor ();
		int numbDisc = currDisc.getNumb ();
		if (numbDisc == 13 || numbDisc == 14)
			numbDisc = -1;

		locationCardPlayed = -1;
		colorToPlay = "";
		drew = false;

		handList = handList.OrderBy (e => e.getColor()).ThenBy(e => e.getNumb ()).ToList();
		
		int count = handList.Count (e => e.getColor () == colorDisc); //does counts of how many cards it has, either color, number, or wild
		int count2 = handList.Count (e => e.getNumb () == numbDisc);
		int count3 = handList.Count (e => e.getNumb () == 13 || e.getNumb () == 14);

		if (count > 0) 
			locationCardPlayed = handList.FindLastIndex (e => e.getColor () == colorDisc);
		
		else if (count2 > 0) 
			locationCardPlayed = handList.FindIndex (e => e.getNumb () == numbDisc);

		else if (count3 > 0) {
			locationCardPlayed = handList.FindIndex (e => e.getNumb () == 13 || e.getNumb()==14);
			colorToPlay = handList.Max (e => e.getColor ());
			if (colorToPlay.Equals ("Black")) {
				bool first = true;
				while(colorToPlay.Equals(colorDisc) || first) {
					int rand = Random.Range (1, 4);
					switch (rand) {
						case 1:
							colorToPlay = "Red";
							break;
						case 2:
							colorToPlay = "Blue";
							break;
						case 3:
							colorToPlay = "Green";
							break;
						case 4:
							colorToPlay = "Yellow";
							break;
					}
					first = false;
				}
			}
					
		}
		else {
			GameObject.Find ("Control").GetComponent<Control> ().draw (1, this);
			drew = true;
		}

		if (locationCardPlayed == -1 && !drew) 
			return;
		
		else 
			turnEnd ();
	}

	public void addCards(Card other) { //recieves cards to the hand
		handList.Add (other);
	}
	public void turnEnd() { //ends the turn
		Control cont = GameObject.Find ("Control").GetComponent<Control> ();
		if (drew) {
			cont.recieveText (string.Format ("{0} drew", name));
			cont.enabled = true;
			return;
		}
		else {
			int specNumb = handList [locationCardPlayed].getNumb ();
			if (specNumb < 10) {
				cont.recieveText (string.Format ("{0} played a {1} {2}", name, handList [locationCardPlayed].getColor (), handList [locationCardPlayed].getNumb ()));
				cont.enabled = true;
				//cont.updateCardsLeft ();
			}
			else if (specNumb == 10) {
				cont.specialCardPlay (this, 10);
				cont.recieveText (string.Format ("{0} played a {1} skip", name, handList [locationCardPlayed].getColor ()));
			}
			else if (specNumb == 11) {
				cont.specialCardPlay (this, 11);
				cont.recieveText (string.Format ("{0} played a {1} reverse", name, handList [locationCardPlayed].getColor ()));
			}
			else if (specNumb == 12) {
				cont.specialCardPlay (this, 12);
				cont.recieveText (string.Format ("{0} played a {1} draw 2", name, handList [locationCardPlayed].getColor ()));
			}
			else if (specNumb == 13) {
				cont.enabled = true;
				cont.recieveText (string.Format ("{0} played a wild card, Color: {1}", name, colorToPlay));
				handList [locationCardPlayed].changeColor (colorToPlay);
			}
			else if (specNumb == 14) {
				cont.specialCardPlay (this, 14);
				cont.recieveText (string.Format ("{0} played a wild draw 4, Color: {1}", name, colorToPlay));
				handList [locationCardPlayed].changeColor (colorToPlay);
				cont.enabled = true;
			}
		}
		cont.updateDiscPile(handList[locationCardPlayed]);
		handList.RemoveAt (locationCardPlayed);
	}
	public bool Equals(PlayerInterface other) { //equals
		return other.getName ().Equals (name);
	}
	public string getName() { //returns the name
		return name;
	}
	public int getCardsLeft() { //returns cards left
		return handList.Count;
	}
}
