using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEnum
{
    /// <summary>
    /// ÿ������µĿ�������ʱ������ط�ҲҪ����ö���������
    /// </summary>
    //��������
    public enum CardTypeEnum
    {
        ChiZi,  //1������
        DingShuJi,//2�������
        FeiJi, //3���ɻ�
        GangBi,//4���ֱ�
        GuTiJiao,//5�����彺
        JianDao,//6������
        JiSuanQi,//7��������
        KeBen,//8���α�
        MaoBi,//9��ë��
        MoShuiPing,//10��īˮƿ
        QianBi,//11��Ǧ��
        ShiGuan,//12���Թ�
        ShuBao,//13�����
        XiangPi,//14����Ƥ
        XianWeiJing//15����΢��

    }

    public enum LevelMode
    {
        Normal,
        Gold,
        HanBao,
        Bamboo,
        BlindBox,
        Lap
    }

    public enum CardDirection
    {
       Left,//����
       Right,//�ҷ���
       Up,//�Ϸ���
       Down//�·���
    }
}