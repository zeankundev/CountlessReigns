using UnityEngine;

public class Utilities
{
    public static GameObject FindInHierarchy(string name)
    {
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform result = FindRecursive(root.transform, name);
            if (result != null) return result.gameObject;
        }
        return null;
    }

    public static Transform FindRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform result = FindRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }
}