using UnityEngine;
using UnityEngine.UI;

public class MainMusicControl : MonoBehaviour
{
    public Slider bgmSlider; // ���� ������ ���� �����̴�
    private const string BGMVolumeKey = "BGMVolume"; // PlayerPrefs�� ������ Ű

    private void Start()
    {
        if (Bgmcontrol.Instance != null)
        {
            // ����� ���� �ҷ����� (������ �⺻�� 0.5)
            float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);

            // �����̴� �� ���� ���� ������ �߰� (UI ������ ����)
            bgmSlider.onValueChanged.AddListener(ChangebgmVolume);

            // �����̴��� �ʱ� ���� ����
            bgmSlider.value = savedVolume;

            // ����� ������ ����
            Bgmcontrol.Instance.bgmAudioSource.volume = savedVolume;
        }
    }

    // �����̴� �� ���� �� ȣ��Ǵ� �޼���
    public void ChangebgmVolume(float value)
    {
        if (Bgmcontrol.Instance != null)
        {
            // ����� ���� ����
            Bgmcontrol.Instance.bgmAudioSource.volume = value;

            // ���� �� ����
            PlayerPrefs.SetFloat(BGMVolumeKey, value);
            PlayerPrefs.Save();
        }
    }
}
