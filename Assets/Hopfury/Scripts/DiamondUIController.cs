using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiamondUIController : MonoBehaviour
{
    public Image numberImage; // Deixa isto público ou [SerializeField]
    public Image diamondIconImage;
    [SerializeField] private List<Sprite> numberSprites; // Aqui vais meter os sprites dos números de 0 a 9

    private int diamondCount = 0;

    void Start()
    {
        // Procurar pelo objeto chamado "DiamondNumber" e buscar o componente Image
        if (numberImage == null)
        {
            GameObject obj = GameObject.Find("DiamondNumber");
            if (obj != null)
            {
                numberImage = obj.GetComponent<Image>();
            }
        }

        // (Opcional) procurar o ícone também se precisares
        if (diamondIconImage == null)
        {
            GameObject iconObj = GameObject.Find("DiamondIcon");
            if (iconObj != null)
            {
                diamondIconImage = iconObj.GetComponent<Image>();
            }
        }

        UpdateDiamondUI();
    }

    public void AddDiamond()
    {
        diamondCount++;
        UpdateDiamondUI();
    }

    public void ResetDiamonds()
    {
        diamondCount = 0;
        UpdateDiamondUI();
    }


    private void UpdateDiamondUI()
    {
        if (diamondCount < numberSprites.Count)
        {
            numberImage.sprite = numberSprites[diamondCount];
        }
    }
}
