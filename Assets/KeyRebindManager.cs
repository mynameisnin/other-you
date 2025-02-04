using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class KeyRebindManager : MonoBehaviour
{
    [System.Serializable]
    public class RebindableKey
    {
        public string actionName;  // 액션 이름 (예: "Move Left", "Jump")
        public InputActionReference actionReference;  // Input Action 참조
        public Button rebindButton;  // 리바인드 버튼
        public TMP_Text buttonText;  // 버튼의 텍스트
    }

    public List<RebindableKey> rebindableKeys = new List<RebindableKey>(); // 변경할 키 리스트

    private void Start()
    {
        LoadRebindData();  // 저장된 키 불러오기
        UpdateAllButtonTexts();  // 버튼 텍스트 초기화

        foreach (var key in rebindableKeys)
        {
            key.rebindButton.onClick.AddListener(() => StartCoroutine(RebindKey(key)));
        }
    }

    private void UpdateAllButtonTexts()
    {
        foreach (var key in rebindableKeys)
        {
            string currentKey = key.actionReference.action.GetBindingDisplayString();
            key.buttonText.text = $"{key.actionName}: {currentKey}";
        }
    }

    private IEnumerator RebindKey(RebindableKey key)
    {
        key.buttonText.text = "Press a key...";  // 키 입력 대기 상태 표시
        key.actionReference.action.Disable();  // 기존 입력 비활성화

        yield return new WaitForSeconds(0.1f);  // UI 업데이트 딜레이

        key.actionReference.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")  // 마우스 입력 제외
            .OnComplete(operation =>
            {
                operation.Dispose();
                key.actionReference.action.Enable();  // 새로운 키 활성화
                UpdateAllButtonTexts();  // 버튼 텍스트 업데이트
                SaveRebindData();  // 변경된 키 저장
            })
            .Start();
    }

    private void SaveRebindData()
    {
        foreach (var key in rebindableKeys)
        {
            string rebinds = key.actionReference.action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(key.actionName, rebinds);
        }
        PlayerPrefs.Save();
    }

    private void LoadRebindData()
    {
        foreach (var key in rebindableKeys)
        {
            if (PlayerPrefs.HasKey(key.actionName))
            {
                string rebinds = PlayerPrefs.GetString(key.actionName);
                key.actionReference.action.LoadBindingOverridesFromJson(rebinds);
            }
        }
    }
}