using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    [SerializeField] private Transform foxBagSlot;
    [SerializeField] private Transform foxHatSlot;

    [SerializeField] private Transform foxBag;
    [SerializeField] private Transform foxHat;

    [SerializeField] private MeshRenderer hatRenderer;
    [SerializeField] private MeshRenderer bagRenderer;

    [SerializeField] private Transform checkMarkTransform;
    
    public bool isHatOn = false;
    public bool isBagOn = false;

    public bool GetHatOnStatus()
    {
        return foxHatSlot.childCount > 1;
    }

    public bool GetBagOnStatus()
    {
        return foxBagSlot.childCount > 1;
    }

    public void ChangeCheckStatus(bool status)
    {
        checkMarkTransform.gameObject.SetActive(status);
    }

    public void UpdateAccessoryStatus(bool hatOn, bool bagOn, Vector3 hatColor, Vector3 bagColor)
    {
        isHatOn = hatOn;
        isBagOn = bagOn;

        var newHatColor = new Color(hatColor.x, hatColor.y, hatColor.z);
        hatRenderer.material.color = newHatColor;
        var newBagColor = new Color(bagColor.x, bagColor.y, bagColor.z);
        bagRenderer.material.color = newBagColor;
        
        UpdateAppearance();
    }
    
    public void UpdateAppearance()
    {
        foxBag.gameObject.SetActive(isBagOn);
        foxHat.gameObject.SetActive(isHatOn);
    }
    
    void Start()
    {
        UpdateAppearance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
