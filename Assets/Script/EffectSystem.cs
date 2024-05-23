using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public BattleMgr battleMgr;
    public GameMgr gameMgr;

    ParticleSystem bossParticle;
    ParticleSystem[] playerParticle = new ParticleSystem[2];

    private AudioSource playerAudioSource;
    private AudioSource bossAudioSource;
    private AudioClip playerChargingAudioClip;
    private AudioClip playerAttackAudioClip;
    private AudioClip bossAttackAudioClip;
    private AudioClip bossSpecialAttackAudioClip;


    private void Awake()
    {
        GameObject gameManager = GameObject.FindWithTag("GameMgr");
        gameMgr = gameManager.GetComponent<GameMgr>();
        
        GameObject battleManager = GameObject.FindWithTag("BattleMgr");
        battleMgr = battleManager.GetComponent<BattleMgr>();

        int stage = (int)gameMgr.currentStage - 3001;

        playerAudioSource = battleMgr.playerAudioSource;

    }
    public void BossAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();

        bossAudioSource = battleMgr.bossAudioSource;
        bossAttackAudioClip = battleMgr.bossAttackAudioes[(int)gameMgr.currentStage - 3001];
        bossAudioSource.PlayOneShot(bossAttackAudioClip);

        StartCoroutine(StopBossPlay(2f));
    }
    public void BossSpecialAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossSpecialAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();
        bossSpecialAttackAudioClip = battleMgr.bossSpecialAttackAudioes[(int)gameMgr.currentStage - 3001];
        bossAudioSource.PlayOneShot(bossSpecialAttackAudioClip);

        StartCoroutine(StopBossPlay(2f));
    }

    private IEnumerator StopBossPlay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bossParticle.Stop();
        Destroy(bossParticle.gameObject);
    }

    public void PlayerChargingPlay()
    {
        playerChargingAudioClip = battleMgr.playerChargingAudioes[(int)gameMgr.currentStage - 3001];
        playerAudioSource.clip = playerChargingAudioClip;
        playerAudioSource.loop = true;
        playerAudioSource.Play();

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
            playerParticle[i] = Instantiate(battleMgr.playerChargingParticles[index+i], battleMgr.battleMap.transform);
            Vector3 pos = new Vector3(-1.53f, 1.75f, -0.3f);
            playerParticle[i].transform.localPosition = pos;
            playerParticle[i].Play();
        }
    }

    public void PlayerAttackPlay()
    {
        playerAudioSource.Stop();
        playerAudioSource.loop = false;

        playerAttackAudioClip = battleMgr.playerAttackAudioes[(int)gameMgr.currentStage - 3001];

        foreach (var particle in playerParticle)
        {
            if(particle != null)
                particle.Stop();
        }

        int value = gameMgr.maxValue;
        int index = 0;

        if (value <= 16)
        {
            index = 0;
        }
        else if (value == 32)
        {
            index = 1;
        }
        else if (value == 64)
        {
            index = 2;
        }
        else if (value == 128)
        {
            index = 3;
        }
        else if (value == 256)
        {
            index = 5;
        }

        playerParticle[1] = Instantiate(battleMgr.playerAttackParticles[index], battleMgr.battleMap.transform);
        
        Vector3 pos = new Vector3(1.1f,0.5f,-0.5f);
        playerParticle[1].transform.localPosition = pos;

        playerAudioSource.PlayOneShot(playerAttackAudioClip);
        playerParticle[1].Play();
        StartCoroutine(StopPlayerPlay(1.5f, 1));

    }

    private IEnumerator StopPlayerPlay(float delay, int index)
    {
        yield return new WaitForSeconds(delay);

        playerParticle[index].Stop();
        Destroy(playerParticle[index].gameObject);
    }
}
