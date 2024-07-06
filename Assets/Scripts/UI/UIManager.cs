using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region
    Transform GridControllerTransf;
    [LabelText("���صĹ������˳��")]
    List<Directions> CurLevelDirections = new List<Directions>();


    private void Awake()
    {
        GridControllerTransf = GameObject.Find("GridGameObjectController").transform;
        InitUIAction();
    }

    void InitUIAction()
    {
        //�����ȡ�ĸ��������UI
        int[] availableNumbers = { 1, 2, 3, 4, 5, 6 };
        List<int> pickedNumbers = new List<int>();
        while (pickedNumbers.Count < 4)
        {
            int _index = UnityEngine.Random.Range(0,availableNumbers.Length); // ����һ���������
            int _number = availableNumbers[_index]; // ��ȡ������������ֵ

            if (!pickedNumbers.Contains(_number))
            {
                pickedNumbers.Add(_number); // �����ֵ���ظ�������ӵ�����б���
            }
        }
        foreach(var _number in pickedNumbers)
        {
            CurLevelDirections.Add((Directions)_number);

        }
        for(int i = 0; i < 4; i++)
        {
            int _indexBuffer = i;
            Transform _btnGo = transform.GetChild(i);
            _btnGo.GetComponentInChildren<Text>().text = CurLevelDirections[i].ToString();
            _btnGo.GetComponent<Image>().sprite = GridControllerTransf.GetComponent<TrailController>().DirectToSprite(CurLevelDirections[_indexBuffer]);

            _btnGo.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                TrailButton(_indexBuffer);
            });
        }

        TrailButton(0);
    }

    void TrailButton(int _index)
    {
        GridControllerTransf.GetComponent<TrailController>().SetCurTrailDirect(CurLevelDirections[_index]);
    }

    #endregion
}
