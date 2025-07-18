using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //game manager needs to track ducklings collected and npc objects delivered


    [SerializeField] private int _ducklingsTotal = 0;
    [SerializeField] private int _nPCsTotal = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _ducklingText;
    [SerializeField] private TextMeshProUGUI _nPCText;
    [SerializeField] private CanvasGroup _ducklingsCompleteCG;
    [SerializeField] private CanvasGroup _nPCCompleteCG;

    private int ducklingsCollected = 0;
    private int nPCsHelped = 0;

    private bool allNPCsHelped = false;
    private bool allDucklingsCollected = false;

    private void Awake()
    {
        _nPCText.text = "Beavers Helped:  " + nPCsHelped.ToString() + " / " + _nPCsTotal.ToString();
        _ducklingText.text = "Ducklings Found:  " + ducklingsCollected.ToString() + " / " + _ducklingsTotal.ToString();
    }

    public void NPCHelped()
    {
        if (allNPCsHelped) return;

        nPCsHelped++;
        _nPCText.text = "Beavers Helped:  " + nPCsHelped.ToString() + " / " + _nPCsTotal.ToString();

        if (nPCsHelped == _nPCsTotal)
        {
            allNPCsHelped = true;

            _nPCCompleteCG.alpha = 1;
        }
    }

    public void DucklingCollected()
    {
        if (allDucklingsCollected) return;

        ducklingsCollected++;
        _ducklingText.text = "Ducklings Found:  " + ducklingsCollected.ToString() + " / " + _ducklingsTotal.ToString();

        if (ducklingsCollected == _ducklingsTotal)
        {
            allDucklingsCollected = true;

            _ducklingsCompleteCG.alpha = 1;
        }
    }
}
