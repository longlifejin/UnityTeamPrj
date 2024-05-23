using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public BattleMgr battleMgr;
    public GameMgr gameMgr;

    //public Vector3 bossPos;
    //public ParticleSystem[] bossAttackParticles;
    //public AudioClip[] bossAttackAudioes;

    //public ParticleSystem bossSpecialAttackParticle;
    //public AudioSource bossAudioSource;
    //public AudioClip bossSpecialAttackAudio;

    //public Vector3 playerPos;
    //public ParticleSystem playerAttackParticle;

    //public AudioSource playerAudioSource;
    //public AudioClip playerAttackAudio;

    ParticleSystem bossParticle;


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
        
        //battleMgr.bossAttackParticles[(int)gameMgr.currentStage - 3001].transform.position = battleMgr.playerPos;
        //battleMgr.bossAttackParticles[(int)gameMgr.currentStage - 3001].Play();
        //bossAttackParticles[int.Parse(Player.Instance.currentStage)-3001].transform.position = playerPos;
        //bossAttackParticles[int.Parse(Player.Instance.currentStage) - 3001].Play();

        //bossAudioSource.PlayOneShot(bossAttackAudio);

        StartCoroutine(StopBossParticle(2f));
    }

    private IEnumerator StopBossParticle(float delay)
    {
        yield return new WaitForSeconds(delay);

        //bossAttackParticles[int.Parse(Player.Instance.currentStage) - 3001].Stop();
        bossParticle.Stop();
    }
}
