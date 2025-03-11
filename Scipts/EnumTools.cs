using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeEnum
{
    /// <summary>
    /// 每次添加新的卡牌种类时候，这个地方也要进行枚举类型添加
    /// </summary>
    //卡牌种类
    public enum CardTypeEnum
    {
        ChiZi,  //1、尺子
        DingShuJi,//2、订书机
        FeiJi, //3、飞机
        GangBi,//4、钢笔
        GuTiJiao,//5、固体胶
        JianDao,//6、剪刀
        JiSuanQi,//7、计算器
        KeBen,//8、课本
        MaoBi,//9、毛笔
        MoShuiPing,//10、墨水瓶
        QianBi,//11、铅笔
        ShiGuan,//12、试管
        ShuBao,//13、书包
        XiangPi,//14、橡皮
        XianWeiJing//15、显微镜

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
       Left,//左方向
       Right,//右方向
       Up,//上方向
       Down//下方向
    }
}