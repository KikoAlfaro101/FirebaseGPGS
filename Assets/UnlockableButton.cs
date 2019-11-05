using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockableButton : MonoBehaviour
{
    public int unlockableID;

    public Color lockedColor;
    public Color unlockedColor;

    private Image img;

    public void Init(int id)
    {
        unlockableID = id;
        img = GetComponent<Image>();
        img.color = lockedColor;
    }

    public void OnButtonClicked()
    {
        bool isUnlocked = DatabaseManager.Instance.SwitchItemState(unlockableID);
        img.color = isUnlocked ? unlockedColor : lockedColor;
    }
}
