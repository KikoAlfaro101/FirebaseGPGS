﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public int itemID;

    public Color lockedColor;
    public Color unlockedColor;

    private Image img;

    public void Init(int id)
    {
        itemID = id;
        img = GetComponent<Image>();
        img.color = lockedColor;
    }

    public void OnButtonClicked()
    {
        DatabaseManager.Instance.SwitchItemState(itemID);
    }

    public void UpdateColor(bool unlocked)
    {
        img.color = unlocked ? unlockedColor : lockedColor; // GUI Update
    }
}
