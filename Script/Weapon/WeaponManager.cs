using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>(); // 固定スロット数分の武器リスト
    public Transform weaponHolder;     // 武器を保持するTransform（子オブジェクト）
    public AudioSource pickupSound;    // 武器取得時の効果音

    private int currentWeaponIndex = 0; // 現在装備中のスロット
    private const int maxSlots = 2;     // スロット数の上限（例：2つまで装備可能）

    void Start()
    {
        // スロット数分の空スロットを初期化
        weapons = new List<GameObject>(new GameObject[maxSlots]);

        // 初期武器がある場合のセットアップ（コメントアウトされている）
        // SelectWeapon(0);
    }

    void Update()
    {
        if (GameManager.isReading || GameManager.gameIsPaused || !GameManager.isAlive)
            return;

        // リロードや発砲中、持ち替え中は切り替え不可
        if (IsWeaponAnimating("Reload") || IsWeaponAnimating("Fire") || IsWeaponAnimating("Equip"))
            return;

        // マウスホイールによる武器切り替え
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            NextWeapon();
        }
        else if (scroll < 0f)
        {
            PreviousWeapon();
        }

        // 数字キーで武器切り替え（1〜2）
        for (int i = 0; i < maxSlots; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && weapons[i] != null)
            {
                if (currentWeaponIndex == i) return; // 同じ武器なら無視
                SelectWeapon(i);
            }
        }
    }

    // 次の武器へ切り替え
    void NextWeapon()
    {
        if (weapons.Count(w => w != null) <= 1) return;

        int startIndex = currentWeaponIndex;
        int nextIndex = (currentWeaponIndex + 1) % maxSlots;

        while (weapons[nextIndex] == null)
        {
            nextIndex = (nextIndex + 1) % maxSlots;
            if (nextIndex == startIndex) return;
        }

        SelectWeapon(nextIndex);
    }

    // 前の武器へ切り替え
    void PreviousWeapon()
    {
        if (weapons.Count(w => w != null) <= 1) return;

        int startIndex = currentWeaponIndex;
        int prevIndex = (currentWeaponIndex - 1 + maxSlots) % maxSlots;

        while (weapons[prevIndex] == null)
        {
            prevIndex = (prevIndex - 1 + maxSlots) % maxSlots;
            if (prevIndex == startIndex) return;
        }

        SelectWeapon(prevIndex);
    }

    // 武器切り替え処理
    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count || weapons[index] == null) return;

        // 現在の武器を非表示に
        if (currentWeaponIndex >= 0 && weapons[currentWeaponIndex] != null)
        {
            weapons[currentWeaponIndex].SetActive(false);
        }

        currentWeaponIndex = index;
        weapons[currentWeaponIndex].SetActive(true);

        // アニメーション状態を初期化（Equip）
        ResetWeaponAnimation(weapons[currentWeaponIndex]);
    }

    // 現在の武器が特定アニメーション中かどうかをチェック
    bool IsWeaponAnimating(string animationName)
    {
        if (weapons[currentWeaponIndex] == null) return false;

        Animator animator = weapons[currentWeaponIndex].GetComponent<Animator>();
        if (animator == null) return false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    // 武器を取得する
    public void PickupWeapon(string weaponName, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots)
        {
            Debug.LogWarning("Invalid slotIndex: " + slotIndex);
            return;
        }

        // 既にその名前の武器を持っているなら無視
        if (weapons.Any(w => w != null && w.name == weaponName))
            return;

        // 武器ホルダー内で該当する名前の武器を探す
        Transform newWeaponTransform = weaponHolder.Find(weaponName);
        if (newWeaponTransform != null)
        {
            GameObject newWeapon = newWeaponTransform.gameObject;
            weapons[slotIndex] = newWeapon;
            newWeapon.SetActive(false);

            pickupSound.Play();
            SelectWeapon(slotIndex);
        }
        else
        {
            Debug.LogWarning("Weapon " + weaponName + " not found under weaponHolder!");
        }
    }

    // アニメーションを"Equip"にリセット
    void ResetWeaponAnimation(GameObject weapon)
    {
        if (weapon == null) return;

        Animator animator = weapon.GetComponent<Animator>();
        if (animator == null) return;

        bool wasInactive = !weapon.activeSelf;

        if (wasInactive) weapon.SetActive(true);

        if (animator.HasState(0, Animator.StringToHash("Equip")))
        {
            animator.Play("Equip", 0);
        }
        else
        {
            Debug.LogWarning("Animator does not have 'Equip' state, skipping reset.");
        }

        animator.Update(0);

        if (wasInactive) weapon.SetActive(false);
    }
}
