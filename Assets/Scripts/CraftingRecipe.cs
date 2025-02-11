﻿using com.ootii.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public static int craftAtual = 1;

    [Serializable]
    public struct ItemAmount
    {
        public Item item;
        [Range(1, 999)]
        public int Amount;
    }

    //public Item item;

    public int energyUsed;
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool CanCraft(ItemContainer itemContainer)
    {

        return HasMaterials(itemContainer); // && HasSpace(itemContainer);
    }


    public bool HasMaterials(ItemContainer itemContainer)
    {

        foreach (ItemAmount itemAmount in Materials)
        {


            if (itemContainer.ItemCount(itemAmount.item.name) < itemAmount.Amount)
            {

                Debug.LogWarning("You don't have the required materals.");
                return false;
            }
        }
        return true;
    }

    // private bool HasSpace(ItemContainer itemContainer)
    //{
    // foreach (ItemAmount itemAmount in Results)
    //  {
    //   if (!itemContainer.CanAddItem(itemAmount.item, itemAmount.Amount))
    //  {
    //   Debug.LogWarning("Your inventory is full.");
    //   return false;
    // }
    // }
    //  return true;
    //  }

    public void Craft(ItemContainer itemContainer)
    {
        if (Generators.currentEnergy >= energyUsed)
        {
            if (CanCraft(itemContainer))
            {
                Generators.currentEnergy -= energyUsed;
                UpdateInterface.instance.Update2();
                RemoveMaterials(itemContainer);
                AddResults(itemContainer);
            }
        }
    }

    public int CraftGenerator(ItemContainer itemContainer)
    {
        if (Generators.currentEnergy >= energyUsed)
        {
            if (CanCraft(itemContainer))
            {
                Generators.currentEnergy -= energyUsed;
                UpdateInterface.instance.Update2();
                RemoveMaterials(itemContainer);
                return 1;

            }

            else return 0;
        }

        else return 0;
    }

    public void RemoveMaterials(ItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in Materials)
        {
            for (int i = 0; i < itemAmount.Amount; i++)
            {
                // ItemAmount oldItem = itemContainer.RemoveItem(itemAmount.item.name) ;
                Inventory.instance.Remove(itemAmount.item);
            }
        }
    }
    private void AddResults(ItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in Results)
        {
            for (int i = 0; i < itemAmount.Amount; i++)
            {
                Inventory.instance.Add(itemAmount.item);
            }
        }
    }

    public string Feedback()
    {
        string stringReturn;

        stringReturn = "Energia Usada: " + energyUsed + "\n";

        foreach (ItemAmount itemAmount in Materials)
        {

            for (int i = 0; i < itemAmount.Amount; i++)
            {
                stringReturn += itemAmount.item.name + ": " + itemAmount.Amount + "\n";
            }
        }

        return stringReturn;
    }

    public Sprite RecipeIcon1()
    {
        Sprite materiais1 = null;

        foreach (ItemAmount itemAmount in Materials)
        {

            for (int i = 0; i < itemAmount.Amount; i++)
            {
                if (materiais1 == null)
                {
                    materiais1 = itemAmount.item.icon;
                }

            }
        }

        return materiais1;

    }

    public Sprite RecipeIcon2()
    {
        Sprite materiais2 = null;

        foreach (ItemAmount itemAmount in Materials)
        {

            for (int i = 0; i < itemAmount.Amount; i++)
            {
                materiais2 = itemAmount.item.icon;

            }
        }
        return materiais2;

    }

    public string RecipeEnergia()
    {
        string stringReturn = "" + energyUsed;

        return stringReturn;
    }

    public string NameItem()
    {
        string stringReturn = "";

        foreach (ItemAmount itemAmount in Results)
        {
            for (int i = 0; i < itemAmount.Amount; i++)
            {
                stringReturn = itemAmount.item.name;
            }
        }

        return stringReturn;
    }

    public bool HasEnergia()
    {
        if (Generators.currentEnergy >= energyUsed)
        {
            return true;
        }

        else return false;
    }

    public bool CraftAluminum(ItemContainer itemContainer)
    {
        if (Generators.currentEnergy >= energyUsed)
        {
            if (CanCraft(itemContainer))
            {
                Generators.currentEnergy -= energyUsed;
                UpdateInterface.instance.Update2();
                RemoveMaterials(itemContainer);
                TextTime.feedbackString = "- Bauxita";
                TextTime.textAtivado = true;


                return true;

            }

            else return false;
        }

        else return false;
    }
}






