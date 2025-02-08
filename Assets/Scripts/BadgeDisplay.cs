using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeDisplay : MonoBehaviour
{
    public GameObject HiddenVersion;
    public GameObject ShownVersion;
    public string ImageName;
    private bool found;
    private void Start()
    {
        if (!HiddenVersion || !ShownVersion)
        {
            Debug.LogError("Badge display does not have a reference to either its hidden or shown version, destroying!");
            Destroy(gameObject);
            return;
        }
        if (ImageName == null || ImageName == "")
        {
            Debug.LogError("Badge display does not have an associated image name, destroying!");
            Destroy(gameObject);
            return;
        }
        found = BadgeManager.IsFound(ImageName);
        HiddenVersion.SetActive(!found);
        ShownVersion.SetActive(found);

    }
    private void Update()
    {
        if (found != BadgeManager.IsFound(ImageName))
        {
            found = BadgeManager.IsFound(ImageName);
            HiddenVersion.SetActive(!found);
            ShownVersion.SetActive(found);
        }
    }
}
