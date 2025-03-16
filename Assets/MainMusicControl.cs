using UnityEngine;
using UnityEngine.UI;

public class MainMusicControl : MonoBehaviour
{
    public Slider bgmSlider; // ���� ������ ���� �����̴�
    private const string BGMVolumeKey = "BGMVolume"; // PlayerPrefs�� ������ Ű

    private void Start()
    {
        // Bgmcontrol �ν��Ͻ��� �����ϴ��� Ȯ��
        if (Bgmcontrol.Instance != null)
        {
            // PlayerPrefs���� BGM ���� ���� �ҷ�����, �⺻���� 0.5f
            float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
            bgmSlider.value = savedVolume; // �����̴��� �ʱ� ���� ����

            // BgmSource�� ������ �����̴��� ������ ����
            Bgmcontrol.Instance.bgmAudioSource.volume = bgmSlider.value;

            // �����̴��� ���� ����� �� ChangeVolume �޼��带 ȣ��
            bgmSlider.onValueChanged.AddListener(ChangebgmVolume);
        }
    }

    // �����̴��� ���� ����� �� ȣ��Ǵ� �޼���
    public void ChangebgmVolume(float value)
    {
        Debug.Log("New Volume: " + value); // ���ο� ���� ���� �α׿� ���
        if (Bgmcontrol.Instance != null)
        {
            // bgmcontrol�� AudioSource ������ �����̴��� ������ ����
            Bgmcontrol.Instance.bgmAudioSource.volume = value;

            // ���� ���� ���� PlayerPrefs�� ����
            PlayerPrefs.SetFloat(BGMVolumeKey, value);
            PlayerPrefs.Save(); // ���� ���� ����
        }
    }
}