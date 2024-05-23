using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public BattleMgr battleMgr;
    public GameMgr gameMgr;

    ParticleSystem bossParticle;
    ParticleSystem playerParticle;


    private void Awake()
    {
        GameObject gameManager = GameObject.FindWithTag("GameMgr");
        gameMgr = gameManager.GetComponent<GameMgr>();
        
        GameObject battleManager = GameObject.FindWithTag("BattleMgr");
        battleMgr = battleManager.GetComponent<BattleMgr>();
    }
    public void BossAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();

        StartCoroutine(StopBossPlay(2f));
    }
    public void BossSpecialAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossSpecialAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();

        StartCoroutine(StopBossPlay(2f));
    }

    private IEnumerator StopBossPlay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bossParticle.Stop();
        Destroy(bossParticle);
    }

    public void PlayerAttackPlay()
    {
        int value = gameMgr.maxValue;
        int index = 0;
        int count = 1;

        if(value <= 16)
        {
            index = 0;
            count = 1;
        }
        else if(value == 32)
        {
            index = 1;
            count = 1;
        }
        else if(value == 64)
        {
            index = 2;
            count = 1;
        }
        else if(value == 128)
        {
            index = 3;
            count = 2;
        }
        else if(value == 256)
        {
            index = 5;
            count = 2;
        }

        for(int i = 0; i < count; ++i)
        {
            playerParticle = Instantiate(battleMgr.playerAttackParticles[index+i], battleMgr.battleMap.transform);
            playerParticle.transform.localPosition = battleMgr.bossPos;
            playerParticle.Play();
            StartCoroutine(StopPlayer(2f));
        }
    }

    private IEnumerator StopPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerParticle.Stop();
        Destroy(playerParticle);
    }
}
