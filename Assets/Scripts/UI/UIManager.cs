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
    List<int> pickedNumbers = new List<int>();
    Transform _btnGo;
    int _number;
    int[] availableNumbers = { 1, 2, 3, 4, 5, 6 };
    int _index;
    int randomNumber;
    private void Awake()
    {
        GridControllerTransf = GameObject.Find("GridGameObjectController").transform;
        InitUIAction();
    }

    void InitUIAction()
    {
        //�����ȡ�ĸ��������UI

        while (pickedNumbers.Count < 4)
        {
            _index = UnityEngine.Random.Range(0,availableNumbers.Length); // ����һ���������
            _number = availableNumbers[_index]; // ��ȡ������������ֵ

            if (!pickedNumbers.Contains(_number))
            {
                pickedNumbers.Add(_number); // �����ֵ���ظ��������ӵ�����б���
            }
        }
        foreach(var _number in pickedNumbers)
        {
            CurLevelDirections.Add((Directions)_number);

        }
        for(int i = 0; i < 4; i++)
        {
            int _indexBuffer = i;
            _btnGo = transform.GetChild(i+2);
            _btnGo.GetComponentInChildren<Text>().text = CurLevelDirections[i].ToString();
            _btnGo.GetComponent<Image>().sprite = GridControllerTransf.GetComponent<TrailController>().DirectToSprite(CurLevelDirections[_indexBuffer]);

            _btnGo.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                TrailButton(_indexBuffer);
            });
        }

    }

    void TrailButton(int _index)
    {
        GridControllerTransf.GetComponent<TrailController>().SetCurTrailDirect(CurLevelDirections[_index]);
        pickedNumbers[_index] = ChangePickedNumber();
        CurLevelDirections[_index] = (Directions)pickedNumbers[_index];
        _btnGo = transform.GetChild(_index+2);
        _btnGo.GetComponentInChildren<Text>().text = CurLevelDirections[_index].ToString();
        _btnGo.GetComponent<Image>().sprite = GridControllerTransf.GetComponent<TrailController>().DirectToSprite(CurLevelDirections[_index]);

    }
    int ChangePickedNumber()
    {
        do{
            randomNumber = availableNumbers[UnityEngine.Random.Range(0,availableNumbers.Length)];
        }
        while(pickedNumbers.Contains(randomNumber));
        return randomNumber;

    }
    #endregion
}
