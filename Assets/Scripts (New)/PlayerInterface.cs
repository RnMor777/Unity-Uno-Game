using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerInterface { //the interface the is inherited by the player objs
	void turn();
	bool skipStatus {
		get;
		set;
	}
	void addCards(Card other);
	string getName();
	bool Equals(PlayerInterface other);
	int getCardsLeft();
}
