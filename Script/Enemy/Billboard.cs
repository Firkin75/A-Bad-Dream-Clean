using UnityEngine;

/// <summary>
/// ���˥����η�����򤯥ӥ�ܩ`�ɄI��
/// </summary>
public class Billboard : MonoBehaviour
{
    private Transform cameraTransform; // �ᥤ�󥫥���Transform�ؤβ���

    void Start()
    {
        // �ᥤ�󥫥���Transform��ȡ��
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            // ����餬Ҋ�Ĥ���ʤ����Ϥϥ���`��å��`�������
            Debug.LogError("Billboard: �ᥤ�󥫥�餬Ҋ�Ĥ���ޤ���");
        }
    }

    void LateUpdate()
    {
        // ����餬δ�O���Έ��ϤτI����Ф�ʤ�
        if (cameraTransform == null) return;

        // ������λ�äȤ��Υ��֥������Ȥ�λ�äβ�֥٥��ȥ��Ӌ�㣨�������η���
        Vector3 direction = cameraTransform.position - transform.position;

        // Y�S�λ�ܞ��̶����k�����׷����������᷽��Τ߻�ܞ��
        direction.y = 0;

        // �����η�����򤯤褦�˥��֥������Ȥ��ܞ
        transform.rotation = Quaternion.LookRotation(direction);

        // ���֥������Ȥ������˱����򤱤ʤ��褦��180�Ȼ�ܞ�����ץ饤�Ȥ���ˤʤ�Τ������
        transform.Rotate(0, 180f, 0);
    }
}
