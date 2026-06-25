using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SlotMachineController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] symbols; // 0=Seven, 1=Cherry, 2=Bell, 3=Bar

    [Header("Lever")]
    public GameObject leverObject;
    public Sprite leverUp;
    public Sprite leverDown;

    [Header("Win Popup")]
    public GameObject winPopup;        // assign the popup panel
    public TextMeshProUGUI popupText;  // text inside popup

    [Header("UI")]
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI resultText;
    public Button spinButton;

    [Header("Settings")]
    public int balance = 1000;
    public int bet = 10;
    public float spinDuration = 1.5f;

    private float symbolWidth  = 120f;
    private float symbolHeight = 120f;
    private int   visibleRows  = 3;
    private float[] reelX = { -160f, 0f, 160f };

    // Weighted symbol pool: 7 is rare, cherry is common
    private int[] symbolPool = { 0, 1, 1, 1, 2, 2, 3, 3, 1, 2 };

    private List<RectTransform> strips  = new List<RectTransform>();
    private List<int>           results = new List<int>();
    private bool isSpinning = false;
    private int reelsDone = 0;

    void Start()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        strips.Clear();
        for (int i = 0; i < 3; i++)
            BuildReel(i);

        if (winPopup != null) winPopup.SetActive(false);

        UpdateUI();
        spinButton.onClick.RemoveAllListeners();
        spinButton.onClick.AddListener(OnSpin);
    }

    void BuildReel(int idx)
    {
        float windowH = symbolHeight * visibleRows;

        var reel = new GameObject("Reel" + (idx + 1));
        reel.transform.SetParent(transform, false);
        var reelRT = reel.AddComponent<RectTransform>();
        reelRT.anchorMin = reelRT.anchorMax = reelRT.pivot = new Vector2(0.5f, 0.5f);
        reelRT.sizeDelta        = new Vector2(symbolWidth, windowH);
        reelRT.anchoredPosition = new Vector2(reelX[idx], 0f);
        var reelImg = reel.AddComponent<Image>();
        reelImg.color = new Color(0.08f, 0.04f, 0.18f, 1f);
        var mask = reel.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        // Use all 4 symbols shuffled across the strip
        int total = symbols.Length * 4;
        var strip = new GameObject("Strip");
        strip.transform.SetParent(reel.transform, false);
        var stripRT = strip.AddComponent<RectTransform>();
        stripRT.anchorMin = stripRT.anchorMax = new Vector2(0.5f, 1f);
        stripRT.pivot = new Vector2(0.5f, 1f);
        stripRT.sizeDelta = new Vector2(symbolWidth, symbolHeight * total);
        stripRT.anchoredPosition = new Vector2(0, 0);
        strips.Add(stripRT);

        for (int s = 0; s < total; s++)
        {
            var sym = new GameObject("S" + s);
            sym.transform.SetParent(strip.transform, false);
            var symRT = sym.AddComponent<RectTransform>();
            symRT.anchorMin = symRT.anchorMax = new Vector2(0.5f, 1f);
            symRT.pivot = new Vector2(0.5f, 1f);
            symRT.sizeDelta        = new Vector2(symbolWidth - 6f, symbolHeight - 6f);
            symRT.anchoredPosition = new Vector2(0, -s * symbolHeight);
            var img = sym.AddComponent<Image>();
            img.sprite         = symbols[s % symbols.Length]; // cycles all 4
            img.color          = Color.white;
            img.preserveAspect = true;
        }
    }

    public void OnSpin()
    {
        if (isSpinning) return;
        if (balance < bet) { resultText.text = "Not enough credits!"; return; }

        if (winPopup != null) winPopup.SetActive(false);

        balance -= bet;
        UpdateUI();
        resultText.text = "Spinning...";
        spinButton.interactable = false;
        isSpinning = true;

        // Pick results using weighted pool
        results.Clear();
        for (int i = 0; i < 3; i++)
            results.Add(symbolPool[Random.Range(0, symbolPool.Length)]);

        reelsDone = 0;
        StartCoroutine(AnimateLever());
        for (int i = 0; i < 3; i++)
            StartCoroutine(SpinReel(strips[i], results[i], i * 0.25f));
    }

    IEnumerator AnimateLever()
    {
        if (leverObject == null) yield break;
        Image leverImg = leverObject.GetComponent<Image>();
        if (leverImg == null) yield break;
        leverImg.sprite = leverDown;
        yield return new WaitForSeconds(0.4f);
        leverImg.sprite = leverUp;
    }

    IEnumerator SpinReel(RectTransform strip, int target, float extraTime)
    {
        float speed    = symbolHeight * 15f;
        float elapsed  = 0f;
        float duration = spinDuration + extraTime;
        float totalH   = strip.sizeDelta.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var pos = strip.anchoredPosition;
            pos.y -= speed * Time.deltaTime;
            if (pos.y < -totalH) pos.y += totalH;
            strip.anchoredPosition = pos;
            yield return null;
        }

        float snapY = -(target * symbolHeight) + symbolHeight;
        strip.anchoredPosition = new Vector2(0, snapY);

        reelsDone++;
        if (reelsDone == 3)
        {
            CheckWin();
            spinButton.interactable = true;
            isSpinning = false;
        }
    }

    void CheckWin()
    {
        int a = results[0], b = results[1], c = results[2];
        int win = 0;
        string message = "";

        if (a == b && b == c)
        {
            if      (a == 0) { win = bet * 10; message = "🎰 JACKPOT!\n777!"; }
            else if (a == 3) { win = bet * 5;  message = "BIG WIN!\nBAR BAR BAR!"; }
            else if (a == 2) { win = bet * 4;  message = "GREAT WIN!\nBell Bell Bell!"; }
            else             { win = bet * 3;  message = "WINNER!\nCherry Cherry Cherry!"; }
        }
        else if (a == b || b == c || a == c)
        {
            win = bet * 2;
            message = "Nice!\nTwo of a kind!";
        }
        else if (a == 1 || b == 1 || c == 1)
        {
            win = bet;
            message = "Cherry bonus!\nBet returned!";
        }

        if (win > 0)
        {
            balance += win;
            resultText.text = message.Split('\n')[0] + " +" + win;
            ShowPopup(message + "\n+" + win + " credits!");
        }
        else
        {
            resultText.text = "No win — try again!";
        }

        UpdateUI();
    }

    void ShowPopup(string msg)
    {
        if (winPopup == null || popupText == null) return;
        popupText.text = msg;
        winPopup.SetActive(true);
        StartCoroutine(HidePopupAfter(2.5f));
    }

    IEnumerator HidePopupAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (winPopup != null) winPopup.SetActive(false);
    }

    void UpdateUI()
    {
        if (balanceText) balanceText.text = "Credits: " + balance;
        if (betText)     betText.text     = "Bet: "     + bet;
    }
}