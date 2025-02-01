using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter {
    public void TakeDamage(float damage);

    public bool IsDefending();
    T GetComponent<T>();
}