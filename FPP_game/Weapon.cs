using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Weapon : MonoBehaviour
{
    [Header("Shooting")]
    public bool isActiveWeapon;
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletPerBurst = 3;
    public int burstBulletLeft;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpwan;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;
    public GameObject muzzleEffect;
    private Animator animator;

    [Header("Spread")]
    private float spreadIntensity;
    public float hipSpreadIntensity;
    public float ADSSpreadIntensity;

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletLeft;
    public bool isReloading;

    public TextMeshProUGUI ammoDisplay;

    bool isADS;

    [Header("TCP SERVER")]

    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;
    int lastReceivedValue = 0;
    int receivedCount = 0;

    public enum WeaponModel
    {
        M1911,
        M4
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;

    void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    private void Awake()
    {
        readyToShoot = true;
        burstBulletLeft = bulletPerBurst;
        animator = GetComponent<Animator>();

        bulletLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    void GetData()
    {
        try
        {
            // Create the server
            server = new TcpListener(IPAddress.Any, connectionPort);
            server.Start();

            // Create a client to get the data stream
            client = server.AcceptTcpClient();

            // Start listening
            running = true;
            while (running)
            {
                Connection();
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
        finally
        {
            if (server != null)
            {
                server.Stop();
            }
        }
    }

   void Connection()
{
    try
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

        // Check if data received is a valid integer
        if (int.TryParse(dataReceived, out int intValue))
        {
            // Check if the received value is the same as the last received value
            if (intValue == lastReceivedValue)
            {
                receivedCount++;
            }
            else
            {
                // Reset the count if the value is different
                lastReceivedValue = intValue;
                receivedCount = 1;
            }

            // Use the received integer if it is received 3 times consecutively
            if (receivedCount >= 1)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => PerformAction(intValue));
            }
        }
        else
        {
            Debug.LogWarning("Received data is not a valid integer: " + dataReceived);
            // Reset the count if the data is invalid
            receivedCount = 0;
        }

        // Optionally, send a response back to the client
        nwStream.Write(buffer, 0, bytesRead);
    }
    catch (Exception e)
    {
        Debug.LogError("Connection error: " + e.Message);
        running = false;
    }
}


    void PerformAction(int intValue)
    {
        switch (intValue)
        {
            case 0:
                FireWeapon();
                break;
            case 1:
                Reload();
                break;
            case 2:
                if(isADS != true)
                {
                    animator.SetTrigger("enterADS");
                    isADS = true;
                    spreadIntensity = ADSSpreadIntensity;
                }
                else{
                    animator.SetTrigger("exitADS");
                    isADS = false;
                    spreadIntensity = hipSpreadIntensity;
                }
                break;
            default:
                Debug.LogWarning("Received integer is not mapped to any action: " + intValue);
                break;
        }
    }

    void SimulateMouseButtonDown(int button)
    {
        if (button == 1)
        {
            // Simulate right mouse button down
            Debug.Log("Simulating right mouse button down");
            MouseButtonDown(MouseButton.Right);
        }
    }

    void MouseButtonDown(MouseButton button)
    {
        // Simulate mouse button down
        Debug.Log("Mouse button down: " + button);
        // Add actual mouse down simulation logic here
    }

    void OnApplicationQuit()
    {
        // Stop the thread when the application quits
        running = false;
        if (thread != null)
        {
            thread.Abort();
        }
        if (server != null)
        {
            server.Stop();
        }
        if (client != null)
        {
            client.Close();
        }
    }

    public enum MouseButton
    {
        Left,
        Right
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("enterADS");
            isADS = true;
            spreadIntensity = ADSSpreadIntensity;
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetTrigger("exitADS");
            isADS = false;
            spreadIntensity = hipSpreadIntensity;
        }

        if (bulletLeft == 0 && isShooting)
        {
            SoundManager.Instance.EmptyMagazineSoundM1911.Play();
        }
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if (Input.GetKeyDown(KeyCode.R) && bulletLeft < magazineSize && isReloading == false)
        {
            Reload();
        }

        if (readyToShoot && isShooting == false && isReloading == false && bulletLeft <= 0)
        {
            Reload();
        }

        if (readyToShoot && isShooting && bulletLeft > 0)
        {
            burstBulletLeft = bulletPerBurst;
            FireWeapon();
        }

        if (ammoDisplay != null)
        {
            ammoDisplay.text = $"{bulletLeft / bulletPerBurst}/{magazineSize / bulletPerBurst}";
        }
    }

    public void FireWeapon()
    {
        bulletLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RECOILADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);
        readyToShoot = false;
        Vector3 shootingDirection = CalculationAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpwan.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletLeft > 1)
        {
            burstBulletLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    public void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculationAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpwan.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
