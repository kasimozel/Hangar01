using UnityEngine;

// Uçağı hedefe doğru döndürür ve sabit hızla ilerletir.
public class HedefeGit : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField] private float hiz = 14f;
    [SerializeField] private float donusHizi = 60f;
    [SerializeField] private float varisMesafesi = 8f;

    [Header("Yatış (Bölüm 06)")]
    [Tooltip("Play başında düz uçuş süresi (saniye).")]
    [SerializeField] private float baslangicBekleme = 0.6f;
    [Tooltip("Dönüşün tam hıza çıkma süresi (saniye).")]
    [SerializeField] private float donusBaslangicSuresi = 0.8f;
    [Tooltip("Yatışın hedef açıya yaklaşma hızı (büyük = daha ani).")]
    [SerializeField] private float yatisYumusatma = 3f;

    // Yatışsız (roll'suz) yön. Yatış her karede bunun üzerine yeniden uygulanır,
    // böylece Rotate ile eklenen yatış kareden kareye birikmez.
    private Quaternion yonelim;
    private float gecenSure;
    private float yatis;   // şu anki yatış açısı (derece)

    private void Start()
    {
        yonelim = transform.rotation;
    }

    private void Update()
    {
        if (hedef == null) return;

        Vector3 fark = hedef.position - transform.position;

        float mesafe = fark.magnitude;
        if (mesafe < varisMesafesi) return;

        Vector3 yon = fark.normalized;

        gecenSure += Time.deltaTime;
        bool donuyor = gecenSure >= baslangicBekleme;

        Quaternion hedefDonusu = Quaternion.LookRotation(yon);
        if (donuyor)
        {
            // Dönüş hızı kısa bir sürede 0'dan donusHizi'na çıkar
            float rampa = donusBaslangicSuresi > 0f
                ? Mathf.Clamp01((gecenSure - baslangicBekleme) / donusBaslangicSuresi)
                : 1f;
            yonelim = Quaternion.RotateTowards(
                yonelim, hedefDonusu, donusHizi * rampa * Time.deltaTime);
        }
        transform.rotation = yonelim;

        // Dış çarpımın yukarı bileşeni: pozitifse hedef sağda
        float yan = Vector3.Cross(transform.forward, yon).y;

        // Kanadı dönüş yönüne yatır (en çok 45 derece) — hedef açıya yumuşakça yaklaş
        float hedefYatis = donuyor ? -yan * 45f : 0f;
        yatis = Mathf.Lerp(yatis, hedefYatis, 1f - Mathf.Exp(-yatisYumusatma * Time.deltaTime));
        transform.Rotate(Vector3.forward, yatis, Space.Self);

        transform.position += transform.forward * hiz * Time.deltaTime;
    }
}
