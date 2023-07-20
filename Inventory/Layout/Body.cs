using UnityEngine;
using UnityEngine.UI;

namespace Models.Inventory.Layout
{
    /**
     * Class to change the image and text associated with this item entry
     */    
    public class Body : MonoBehaviour
    {
        // the text label for the item name
        [SerializeField] private TMPro.TextMeshProUGUI _text;
        // the image object for the item sprite
        [SerializeField] private Image _image;
        // on initializing this object, change the sprite and name
        public void Initialize(Sprite sprite, string name)
        {
            print("body: " + name);
            _image.sprite = sprite;
            _text.text = name;
        }            
    }
}