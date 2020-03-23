using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaAttackJudge : MonoBehaviour
{
    public PiranhaController piranha;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            piranha.attackEnabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            piranha.attackEnabled = false;
        }
    }
}
