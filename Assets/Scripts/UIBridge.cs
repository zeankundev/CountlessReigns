using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIBridge : MonoBehaviour
{
    private TMP_Text subtitleText;
    private TMP_Text timedStatusText;
    private GameObject timedStatusPieGameObject;
    private RectTransform playerHealthBar;
    public TMP_FontAsset font;
    private Image timedStatusPie;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        subtitleText = transform.Find("OutlineText").GetComponent<TMP_Text>();
        timedStatusPieGameObject = transform.Find("TimedParent").gameObject;
        timedStatusPie = transform.Find("TimedParent/RotationalTimedStatus").GetComponent<Image>();
        timedStatusText = transform.Find("TimedParent/RotationalTimedStatus/TimedStatusText").GetComponent<TMP_Text>();
        playerHealthBar = transform.Find("PlayerHealthBar").GetComponent<RectTransform>();
        Debug.LogWarning(subtitleText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisplayText(string text, float duration)
    {
        CancelInvoke("ClearText");
        subtitleText.text = text;
        subtitleText.alpha = 1.0f;
        subtitleText.CrossFadeAlpha(1.0f, 0.5f, false);
        Invoke("ClearText", duration + 0.5f);
    }
    private void ClearText()
    {
        subtitleText.CrossFadeAlpha(0.0f, 0.5f, false);
    }
    public void ToggleTimedStatusState(bool state)
    {
        timedStatusPieGameObject.SetActive(state);
    }
    public void ShowTimedStatus(string text)
    {
        timedStatusText.text = text;
        timedStatusPie.fillAmount = 1.0f;
    }
    public void UpdateTimedStatus(float normalizedTime)
    {
        timedStatusPie.fillAmount = normalizedTime;
    }
    public void UpdateHealthBar(float healthPercent)
    {
        playerHealthBar.sizeDelta = new Vector2(healthPercent * 100, playerHealthBar.sizeDelta.y);
    }

    private int _activeItemCount = 0;
    private const float ItemWidth     = 280f;
    private const float ItemHeight    = 56f;
    private const float ItemSpacing   = 8f;
    private const float SlideDistance = 350f;
    private const float SlideDuration = 0.35f;
    private const float DisplayDuration = 5f;
    private const float ImageSize     = 40f;
    private const float TextPadding   = 12f;

    public void LerpFromLeftToOriginalVectorItemShowing(Sprite itemSprite, string text)
    {
        // ── 1. Root container (RectTransform + CanvasGroup for group fade) ──────────
        GameObject itemObject = new GameObject("ItemNotif_" + _activeItemCount);
        RectTransform rootRect = itemObject.AddComponent<RectTransform>();
        CanvasGroup canvasGroup = itemObject.AddComponent<CanvasGroup>();
        Image bgImage = itemObject.AddComponent<Image>();

        rootRect.SetParent(transform, false);
        rootRect.sizeDelta = new Vector2(ItemWidth, ItemHeight);
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0f, 1f); // top-left anchor
        rootRect.pivot = new Vector2(0f, 1f);

        // Stack downward: each new item offsets further down
        float yOffset = -_activeItemCount * (ItemHeight + ItemSpacing);
        Vector2 targetPos  = new Vector2(0f, yOffset);
        Vector2 offscreenPos = new Vector2(-SlideDistance - ItemWidth, yOffset);

        rootRect.anchoredPosition = offscreenPos;

        // Optional: subtle background pill
        bgImage.color = new Color(0f, 0f, 0f, 0f);

        // ── 2. Icon child (Image) ────────────────────────────────────────────────────
        GameObject iconObject = new GameObject("Icon");
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        Image iconImage = iconObject.AddComponent<Image>();

        iconRect.SetParent(rootRect, false);
        iconRect.sizeDelta = new Vector2(ImageSize, ImageSize);
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(TextPadding, 0f);

        iconImage.sprite = itemSprite;
        iconImage.preserveAspect = true;

        // ── 3. Label child (TMP_Text) ─────────────────────────────────────────────
        GameObject textObject = new GameObject("Label");
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();

        textRect.SetParent(rootRect, false);

        float textX = TextPadding + ImageSize + TextPadding;
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(textX, 0f);
        textRect.offsetMax = new Vector2(-TextPadding, 0f);

        label.text = text;
        label.font = font;
        label.fontSize = 16f;
        label.fontStyle = FontStyles.Normal;
        label.verticalAlignment = VerticalAlignmentOptions.Middle;
        label.overflowMode = TextOverflowModes.Ellipsis;

        // ── 4. Start hidden, drive animation ─────────────────────────────────────
        canvasGroup.alpha = 0f;
        _activeItemCount++;

        StartCoroutine(AnimateNotification(itemObject, rootRect, canvasGroup, offscreenPos, targetPos));
    }

    private IEnumerator AnimateNotification(
        GameObject   itemObject,
        RectTransform rect,
        CanvasGroup   cg,
        Vector2       from,
        Vector2       to)
    {
        // ── Slide in from left + fade in ─────────────────────────────────────────
        float elapsed = 0f;
        while (elapsed < SlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / SlideDuration);
            rect.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            cg.alpha = t;
            yield return null;
        }
        rect.anchoredPosition = to;
        cg.alpha = 1f;

        // ── Hold ─────────────────────────────────────────────────────────────────
        yield return new WaitForSeconds(DisplayDuration);

        // ── Slide out to original left + fade out ────────────────────────────────
        elapsed = 0f;
        while (elapsed < SlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / SlideDuration);
            rect.anchoredPosition = Vector2.LerpUnclamped(to, from, t);
            cg.alpha = 1f - t;
            yield return null;
        }

        _activeItemCount--;
        Destroy(itemObject);
    }
}
