using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void OnClickBack()
    {
        //TO-DO : string define�� �����ؼ� ����ϱ�
        SceneManager.LoadScene("EntranceStorePopUp");
    }


}
