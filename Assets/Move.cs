using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update

    // �����܂łɂ����鎞��
    private float timeTaken = 0.2f;
    // �o�ߎ���
    private float timeElapsed;
    // �ړI�n
    private Vector3 destination;
    // �o���n
    private Vector3 original;

    public void MoveTo(Vector3 newDestination)
    {
        // �ړI�n���ς�����ꍇ�݈̂ړ����J�n����
        if (newDestination != destination)
        {
            // �o�ߎ��Ԃ�������
            timeElapsed = 0;
            // �o���n�����݂̈ʒu�ɐݒ�
            original = transform.position;
            // �V�����ړI�n��ݒ�
            destination = newDestination;
        }
    }

    void Start()
    {
        //�ړI�n�E�o���n�����ݒn�ŏ�����
        destination = transform.position;
        original = destination;
    }

    // Update is called once per frame
    void Update()
    {
        // �ړI�n�ɓ������Ă����珈�����Ȃ�
        if (original == destination) { return; }

        // ���Ԍo�߂����Z
        timeElapsed += Time.deltaTime;
        // �o�ߎ��Ԃ��������Ԃ̉��������Z�o
        float timeRate = timeElapsed / timeTaken;
        // �������Ԃ𒴂���悤�ł���Ύ��s�������ԑ����Ɋۂ߂�
        if (timeRate > 1) { timeRate = 1; }
        // �C�[�W���O�p�v�Z�i���j�A�j
        float easing = timeRate;
        // ���W���Z�o
        Vector3 currentPosition = Vector3.Lerp(original, destination, easing);
        // �Z�o�������W��position�ɑ��
        transform.position = currentPosition;

        // �ړ�������������original��destination�ɐݒ�
        if (timeRate == 1)
        {
            original = destination;
        }
    }
}
