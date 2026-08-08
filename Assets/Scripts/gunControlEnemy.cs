using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class gunControlEnemy : MonoBehaviour
{
    private Coroutine fireCoroutine;
    private Vector3 debugEndPos;
    private Vector3 debugStartPos;
    private bool isAlive;
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private GameObject spark;
    [SerializeField] private Transform barrelTip;
    [SerializeField] private enemyManager enemyManager;
    public void IsAliveSet(bool isAlive)
    {
        this.isAlive = isAlive;
    }
    public void Fire(Transform target, float delay, float spread)
    {
        if (isAlive)
        {
            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireCoroutine(target, delay, spread));
        }
        else
        {
            StopAllCoroutines();
        }
        
    }
    IEnumerator FireCoroutine (Transform target, float delay, float spread)
    {
        RaycastHit hit;
        do
        {
            fireSound.pitch = Random.Range(0.9f, 1.5f);
            fireSound.Play();
            bool didHit;
            Ray ray = new Ray(barrelTip.position, (target.position - barrelTip.position));
            didHit = Physics.Raycast(ray, out hit, Mathf.Infinity);
            debugStartPos = barrelTip.position;
            debugEndPos = hit.point;

            Vector3 direction;
            if (!didHit) direction = ray.GetPoint(75) - barrelTip.position;
            else direction = hit.point - barrelTip.position;
            muzzleFlash.Play();
            GameObject bullet;
            bullet = Instantiate(bulletObject, barrelTip.position, Quaternion.LookRotation(direction + BullSpread(enemyManager.GetSpread())));
            bullet.GetComponent<ParticleSystem>().Play();
            bullet.GetComponent<bulletScript>().AddGunRef(this.gameObject);
            yield return new WaitForSeconds(delay);
        } while (isAlive);
    }

    public void BulletHit(Vector3 hitLocation, Vector3 hitNormal, GameObject hitObject)
    {
        Instantiate(spark, hitLocation, Quaternion.LookRotation(hitNormal)).GetComponent<ParticleSystem>().Play();
        if (hitObject.GetComponent<Rigidbody>() != null && !hitObject.CompareTag("Player"))
        { hitObject.GetComponent<Rigidbody>().AddForceAtPosition((debugEndPos - debugStartPos) * 10f, hitLocation, ForceMode.Impulse); }
    }
    private Vector3 BullSpread(float spread)
    {
        Vector3 bullSpread;
        bullSpread.x = Random.Range(-spread, spread);
        bullSpread.y = Random.Range(-spread, spread);
        bullSpread.z = Random.Range(-spread, spread);
        return bullSpread;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (debugEndPos - debugStartPos));
    }
}
