using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PetManager
{
    // 소유 중인 펫의 공격력 퍼센트 합산
    public float GetOwnedPetAtkPercentage()
    {
        float ownedValues = 0f;

        foreach (var pet in Managers.Backend.GameData.PetInventory.PetInventoryDic)
        {
            if(pet.Value.OwningState == EOwningState.Owned)
            {
                PetData petData = Managers.Data.PetChart[Util.ParseEnum<EPetType>(pet.Key)];
                float totalOwnedValue = petData.OwnedAtkPercent + (pet.Value.Level * petData.OwnedAtkIncreasePercent);
                ownedValues += totalOwnedValue;
            }
        }

        return ownedValues;
    }

    // 장착 중인 펫의 공격력/HP 고정 보너스 합산
    public (float atkValue, float hpValue) GetEquippedPetStats()
    {
        float totalEqAtk = 0f;
        float totalEqHp = 0f;

        // 소유 중인 펫 중 해당 타입의 펫 보유 효과 합산
        foreach (var pet in Managers.Backend.GameData.PetInventory.PetInventoryDic)
        {
            if(pet.Value.OwningState == EOwningState.Owned && pet.Value.IsEquipped)
            {
                PetData petData = Managers.Data.PetChart[Util.ParseEnum<EPetType>(pet.Key)];
                // 장착 효과: (EquippedValue + 증가율 * 레벨)
                float increaseAtkRate = 1 + (petData.EquippedAtkIncreaseRate / 100f) * pet.Value.Level;
                float increaseHpRate = 1 + (petData.EquippedHpIncreaseRate / 100f) * pet.Value.Level;

                float atkVal = petData.EquippedAtkValue * increaseAtkRate;
                float hpVal = petData.EquippedHpValue * increaseHpRate;

                totalEqAtk += atkVal;
                totalEqHp += hpVal;
            }
        }

        return (totalEqAtk, totalEqHp);
    }
}
