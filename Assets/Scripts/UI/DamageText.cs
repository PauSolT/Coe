using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] float firstLifetime = 0.2f;
    [SerializeField] float secondLifetime = 0.4f;
    [SerializeField] float finalOpacity = 0.25f;

    [SerializeField] Vector2 velocityFirstHalf;
    [SerializeField] Vector2 velocitySecondHalf;

    [SerializeField] Vector2 initialScale;
    [SerializeField] Vector2 maxScale;
    [SerializeField] Vector2 finalScale;

    [SerializeField] Color outlineShieldColor;
    [SerializeField] Color outlineHealthColor;

    Vector3 position;
    TMP_Text text;

    Color color;
    Timer firstTimer;
    Timer secondTimer;

    public void Init(float damage, bool isShieldDamage, DamageComponent damageComponent)
    {
        color = damageComponent.ElementData.color;

        position = transform.position;
        text = GetComponentInChildren<TMP_Text>();
        text.text = damage.ToString();
        text.color = color;
        transform.localScale = initialScale;
        text.outlineColor = outlineHealthColor;
        
        if (isShieldDamage)
        {
            text.outlineColor = outlineShieldColor;
            velocityFirstHalf.x *= -1f;
            velocitySecondHalf.x *= -1f;
        }
        if (damageComponent.HitFromRight)
        {
            velocityFirstHalf.x *= -1f;
            velocitySecondHalf.x *= -1f;
        }
        if (damageComponent.GetMultiplier() > 1f)
        {
            text.text = text.text + "!";
            maxScale += maxScale / 2f;
        }

        firstTimer = TimerManager.Instance.StartTimer(firstLifetime, SecondHalf, deltaTime =>
        {
            float t = firstTimer.ProgressAscending;
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            MoveTextFirstHalf(deltaTime, easedT);
        });
    }
    void SecondHalf()
    {
        secondTimer = TimerManager.Instance.StartTimer(secondLifetime, DestroyText, deltaTime =>
        {
            float t = secondTimer.ProgressAscending;
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            MoveTextSecondHalf(deltaTime, easedT);
        });
    }

    void DestroyText()
    {
        Destroy(gameObject);
    }

    void MoveTextFirstHalf(float deltaTime, float t)
    {
        position += new Vector3(velocityFirstHalf.x, velocityFirstHalf.y, 0f) * deltaTime;
        transform.position = position;

        transform.localScale = Vector3.Lerp(
            new Vector3(initialScale.x, initialScale.y, 1f),
            new Vector3(maxScale.x, maxScale.y, 1f),
            t);
    }

    void MoveTextSecondHalf(float deltaTime, float t)
    {
        position += new Vector3(velocitySecondHalf.x, velocitySecondHalf.y, 0f) * deltaTime; 
        transform.position = position;

        transform.localScale = Vector3.Lerp(
            new Vector3(maxScale.x, maxScale.y, 1f),
            new Vector3(finalScale.x, finalScale.y, 1f),
            t);

        text.color = new Color(
            color.r,
            color.g,
            color.b,
            Mathf.Lerp(color.a, finalOpacity, t));
    }



}
