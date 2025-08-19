using System;
using System.Collections.Generic;

public static class LocalizationNotifier
{
    public static event Action OnLanguageChanged;
    private static List<Action> listeners = new List<Action>();

    public static void AddListener(Action listener)
    {
        listeners.Add(listener);
    }

    public static void RemoveListener(Action listener)
    {
        listeners.Remove(listener);
    }

    public static void NotifyLanguageChange()
    {
        OnLanguageChanged?.Invoke();
        foreach (var listener in listeners)
        {
            listener?.Invoke();
        }
    }
}