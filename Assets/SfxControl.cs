using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxControl : MonoBehaviour
{
    public Slider volumeSlider; // ���� ������ ���� �����̴�
    public BTNSFX btnSound; // BTNSFX �ν��Ͻ�
    private const string VolumeKey = "SfxVolume"; // PlayerPrefs�� ������ Ű

    private void Start()
    {
        // PlayerPrefs���� ���� ���� �ҷ�����
        // �ҷ��� �� ���� ��� �⺻�� 0.5f�� ����
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);

        // �����̴��� �ʱ� ���� ����� ���� ������ ����
        volumeSlider.value = savedVolume;

        // Ŭ�� ������ �ʱ� ������ ����
        btnSound.SetClickSoundVolume(savedVolume);

        // �����̴��� ���� ����� �� SetVolume �޼��带 ȣ���ϵ��� ����
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // �����̴��� ���� ����� �� ȣ��Ǵ� �޼���
    private void SetVolume(float value)
    {
        Debug.Log("SFX�����̴� ��: " + value); // �����̴��� ���� ���� �α׿� ���

        // Ŭ�� ������ ������ �����̴��� ������ ����
        btnSound.SetClickSoundVolume(value);

        // ���� ���� ���� PlayerPrefs�� ����
        PlayerPrefs.SetFloat(VolumeKey, value);

        // ����� PlayerPrefs ���� ����
        PlayerPrefs.Save();
    }
}
