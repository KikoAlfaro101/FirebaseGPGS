public class User
{
    public string username = ""; // Considered as "primary key"
    public string items = "";

    public User(string username)
    {
        this.username = username;
    }

    public User(string username, int itemsAmount)
    {
        this.username = username;
        CreateItems(itemsAmount);
    }

    private void CreateItems(int amount)
    {
        string temp = "";

        for (int i = 0; i < amount; i++)
        {
            temp += "0";
        }

        items = temp;
    }

    public override string ToString()
    {
        return string.Format("Username: {0}", username);
    }

    public void UnlockItem(int id)
    {
        string newItems = "";

        for (int i = 0; i < items.Length; i++)
        {
            if (i == id - 1) newItems += "1"; // unlocked
            else newItems += items[i]; // not changes
        }

        items = newItems; // update data
    }

    public void LockItem(int id)
    {
        string newItems = "";

        for (int i = 0; i < items.Length; i++)
        {
            if (i == id - 1) newItems += "0"; // unlocked
            else newItems += items[i]; // not changes
        }

        items = newItems; // update data
    }

    public bool IsItemUnlocked(int id)
    {
        return items[id - 1] == '1'; // If equals 1, the item is unlocked
    }
}