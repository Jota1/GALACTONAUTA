﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{

	public Item item;   // Item para colocar no inventário se pegado

	// Quando o jogador interage com o Item
	public override void Interact()
	{
		base.Interact();

		PickUp();
	}

	// Pick up the item
	public void PickUp()
	{
		if(item.name == "Alumínio")
		{
			AluminumProduction.AluminioProduzido = false;
		}
		TextTime.feedbackString = "+ " + item.name;
		TextTime.textAtivado = true;

		Inventory.instance.Add(item);   // Adiciona ao inventário

		Destroy(gameObject);    // Destrói o item da cena
	}

}
