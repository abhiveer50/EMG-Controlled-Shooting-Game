using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;
    public AudioSource ReloadingSoundM1911;
    public AudioSource EmptyMagazineSoundM1911;
    public AudioClip M16Shot;
    public AudioClip M1911Shot;
    public AudioSource ReloadingSoundM4;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.M1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case Weapon.WeaponModel.M4:
                ShootingChannel.PlayOneShot(M16Shot);
                break;
        }
    }

    public void PlayReloadSound(Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.M1911:
                ReloadingSoundM1911.Play();
                break;
            case Weapon.WeaponModel.M4:
                ReloadingSoundM4.Play();
                break;
        }
    }
}
