using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    private void Start()
    {
    }

    public void BossAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();

        bossAudioSource = battleMgr.bossAudioSource;
        bossAttackAudioClip = battleMgr.bossAttackAudioes[(int)gameMgr.currentStage - 3001];
        bossAudioSource.PlayOneShot(bossAttackAudioClip);

        StartCoroutine(StopBossPlay(1f));
    }
    public void BossSpecialAttackPlay()
    {
        bossParticle = Instantiate(battleMgr.bossSpecialAttackParticles[(int)gameMgr.currentStage - 3001], battleMgr.battleMap.transform);
        bossParticle.transform.localPosition = battleMgr.playerPos;
        bossParticle.Play();
        bossAudioSource = battleMgr.bossAudioSource;
        bossSpecialAttackAudioClip = battleMgr.bossSpecialAttackAudioes[(int)gameMgr.currentStage - 3001];
        bossAudioSource.PlayOneShot(bossSpecialAttackAudioClip);

        Vector3 pos = new Vector3(-2.45f, 2f, -0.5f);
        battleMgr.ShowDamage(10, pos, Color.white);

        StartCoroutine(StopBossPlay(1f));
    }

    private IEnumerator StopBossPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(bossParticle != null)
        {
            bossParticle.Stop();
            Destroy(bossParticle.gameObject);
        }
    }

    public void PlayerChargingPlay()
    {
        playerChargingAudioClip = battleMgr.playerChargingAudioes[0];
        playerAudioSource.clip = playerChargingAudioClip;
        playerAudioSource.loop = true;
        playerAudioSource.Play();

        int value = gameMgr.maxValue;
        playerParticle[0] = Instantiate(battleMgr.playerChargingParticles[0], battleMgr.battleMap.transform);
        Vector3 pos = new Vector3(-1.53f, 1.75f, -0.3f);
        playerParticle[0].transform.localPosition = pos;
        playerParticle[0].Play();
    }

    public void SetBasicState()
    {
        playerAudioSource.Stop();
        playerAudioSource.loop = false;
        foreach (var particle in playerParticle)
        {
            if (particle != null)
                particle.Stop();
        }
    }

    public void PlayerAttackPlay()
    {
        playerAudioSource.Stop();
        playerAudioSource.loop = false;

        playerAttackAudioClip = battleMgr.playerAttackAudioes[0];

        foreach (var particle in playerParticle)
        {
            if(particle != null)
                particle.Stop();
        }

        int value = gameMgr.maxValue;
        
        playerParticle[1] = Instantiate(battleMgr.playerAttackParticles[0], battleMgr.battleMap.transform);
        
        Vector3 pos = new Vector3(1.1f,0.5f,-1f);
        playerParticle[1].transform.localPosition = pos;

        playerAudioSource.PlayOneShot(playerAttackAudioClip);
        playerParticle[1].Play();
        StartCoroutine(StopPlayerPlay(1f, 1));
    }

    private IEnumerator StopPlayerPlay(float delay, int index)
    {
        yield return new WaitForSeconds(delay);

        if (playerParticle[index] != null)
        {
            playerParticle[index].Stop();
            Destroy(playerParticle[index].gameObject);
        }
    }
}
