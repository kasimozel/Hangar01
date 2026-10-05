using UnityEngine;

// Hedef görüş konisinin içinde mi, sağda mı solda mı?
public class Radar : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField] private float yarimAci = 35f;
    [SerializeField] private float menzil = 120f;

    public bool Goruyor { get; private set; }
    public float YanTaraf { get; private set; }

    private void Update()
    {
        if (hedef == null) return;

        Vector3 fark = hedef.position - transform.position;
        Vector3 yon = fark.normalized;

        // İç çarpım: 1 tam önümde, 0 yanımda, negatif arkamda
        float onde = Vector3.Dot(transform.forward, yon);
        float esik = Mathf.Cos(yarimAci * Mathf.Deg2Rad);
        Goruyor = onde > esik && fark.magnitude < menzil;

        // Dış çarpım: yukarı bileşen pozitifse hedef sağda
        YanTaraf = Vector3.Cross(transform.forward, yon).y;
    }

    // Sahne görünümünde radar konisini çizer (Scene view gizmo).
    private void OnDrawGizmos()
    {
        Vector3 merkez = transform.position;
        Vector3 ileri = transform.forward;

        // Menzil küresi
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(merkez, menzil);

        // İleri yön
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(merkez, merkez + ileri * menzil);

        // Sol ve sağ sınırlar (yarım açı, transform.up etrafında)
        Vector3 sol = Quaternion.AngleAxis(-yarimAci, transform.up) * ileri;
        Vector3 sag = Quaternion.AngleAxis(yarimAci, transform.up) * ileri;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(merkez, merkez + sol * menzil);
        Gizmos.DrawLine(merkez, merkez + sag * menzil);

        // Üst ve alt sınırlar + koninin uç çemberi (koni 3B olduğu için)
        Vector3 ust = Quaternion.AngleAxis(-yarimAci, transform.right) * ileri;
        Vector3 alt = Quaternion.AngleAxis(yarimAci, transform.right) * ileri;
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.6f);
        Gizmos.DrawLine(merkez, merkez + ust * menzil);
        Gizmos.DrawLine(merkez, merkez + alt * menzil);

        const int parca = 32;
        float uzaklik = menzil * Mathf.Cos(yarimAci * Mathf.Deg2Rad);
        float yaricap = menzil * Mathf.Sin(yarimAci * Mathf.Deg2Rad);
        Vector3 cemberMerkez = merkez + ileri * uzaklik;
        Vector3 onceki = cemberMerkez + transform.right * yaricap;
        for (int k = 1; k <= parca; k++)
        {
            float a = k * Mathf.PI * 2f / parca;
            Vector3 nokta = cemberMerkez + (transform.right * Mathf.Cos(a) + transform.up * Mathf.Sin(a)) * yaricap;
            Gizmos.DrawLine(onceki, nokta);
            onceki = nokta;
        }

        // Yatay yay: sol sınırdan sağ sınıra menzil mesafesinde
        Gizmos.color = Color.yellow;
        onceki = merkez + sol * menzil;
        for (int k = 1; k <= parca; k++)
        {
            float aci = Mathf.Lerp(-yarimAci, yarimAci, (float)k / parca);
            Vector3 nokta = merkez + Quaternion.AngleAxis(aci, transform.up) * ileri * menzil;
            Gizmos.DrawLine(onceki, nokta);
            onceki = nokta;
        }

        // Hedefe çizgi: görüyorsa yeşil, görmüyorsa kırmızı
        if (hedef != null)
        {
            Vector3 fark = hedef.position - merkez;
            bool goruyor = Application.isPlaying
                ? Goruyor
                : Vector3.Dot(ileri, fark.normalized) > Mathf.Cos(yarimAci * Mathf.Deg2Rad) && fark.magnitude < menzil;
            Gizmos.color = goruyor ? Color.green : Color.red;
            Gizmos.DrawLine(merkez, hedef.position);
            Gizmos.DrawWireSphere(hedef.position, 4f);
        }
    }
}
