using System;

// One active use of an item. Multiple entries with the same itemId are independent timers.
[Serializable]
public class ActiveEffect
{
    public string itemId;
    public float remainingTime;

    public ActiveEffect(string itemId, float duration)
    {
        this.itemId = itemId;
        remainingTime = duration;
    }
}
