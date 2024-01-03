using System.Collections;

public static class ListExtension
{
    public static void RemoveLast(this IList list)
    {
        list.RemoveAt(list.Count - 1);
    }
}