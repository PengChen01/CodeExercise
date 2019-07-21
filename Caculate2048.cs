using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Caculate2048
    {
        Grid currentGrid;
        Grid currentGridL;
        Grid currentGridR;
        Grid currentGridU;
        Grid currentGridD;
        public Caculate2048()
        {
            List<byte> SpaceElements = new List<byte>();
            
            //确定可能出现的元素（概率不变）
            FaceProbability faceProbability = new FaceProbability();
            MoveProbability moveProbability = new MoveProbability();
            List<Grid> origns = new List<Grid>();
            List<Grid> targets = new List<Grid>();
            origns.Add(new Grid());
            Random random = new Random();
            for (int n  = 0; n < 1000; n++)
            {
                for (int i = 0; i < origns.Count; i++)
                {
                    //确定空元素（计算每一个出现的概率）
                    //SpaceElements.Clear();
                    //currentGrid.elements.Select((Element s) =>
                    //{
                    //    if (s.face == Face.Space)
                    //    {
                    //        SpaceElements.Add(s.Index);
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //});
                    SpaceElements = origns[i].spaceElements.RandTake(3);
                    for (int j = 0; j < SpaceElements.Count; j++)
                    {
                        for (int k = 0; k < faceProbability.Count; k++)
                        {
                            currentGrid = origns[i].DeepClone() as Grid;
                            currentGrid.Replace(SpaceElements[j], faceProbability.GetFace(k));
                            currentGridL = currentGrid.DeepClone() as Grid;
                            moveProbability.Clear();
                            if (currentGridL.Move(MoveType.Left))
                            {
                                moveProbability.Add(currentGridL,MoveType.Left);
                            }
                            currentGridR = currentGrid.DeepClone() as Grid;
                            if (currentGridR.Move(MoveType.Right))
                            {
                                moveProbability.Add(currentGridR,MoveType.Right);
                            }
                            currentGridU = currentGrid.DeepClone() as Grid;
                            if (currentGridU.Move(MoveType.Up))
                            {
                                moveProbability.Add(currentGridU,MoveType.Up);
                            }
                            currentGridD = currentGrid.DeepClone() as Grid;
                            if (currentGridD.Move(MoveType.Down))
                            {
                                moveProbability.Add(currentGridD,MoveType.Down);
                            }
                            moveProbability.ResetProbility();
                            for (int r = 0; r < moveProbability.Count; r++)
                            {
                                //下一状态出现的概率=上一状态的出现的概率*在上一状态某空白位置出现数字的概率*出现某一数字的概率*方向移动概率（如果移动后状态未改变，则该概率为0，该概率也是影响决策的概率）
                                moveProbability.grids[r].probability = (1.0/SpaceElements.Count) * faceProbability.Value(faceProbability.GetFace(k)) * moveProbability.Value(r) * origns[i].probability;
                                
                                //将相同的状态合并在一起，防止状态出现指数爆炸
                                if (targets.Exists(s => s.Equals(moveProbability.grids[r])))
                                {
                                    currentGrid = targets.First(s => s.Equals(moveProbability.grids[r]));
                                    currentGrid.gameScore += moveProbability.grids[r].probability * moveProbability.grids[r].currentScore;
                                }
                                else
                                {
                                    targets.Add(moveProbability.grids[r]);//probability
                                }
                            }
                        }
                    }
                }
                origns = targets;

                targets=new List<Grid>();
                double p = 0;
                for (int i = 0; i < origns.Count; i++)
                {
                    p += origns[i].probability;
                }
                p = p / origns.Count;
                origns.RemoveAll(s => s.probability < p); //删除概率在平均值之下的状态
                //获取前50个概率最高的状态
                origns = origns.OrderByDescending(s => s.probability).Take(50).ToList();
                p = 0;
                for (int i = 0; i < origns.Count; i++)
                {
                    p += origns[i].probability;
                }
                for (int i = 0; i < origns.Count; i++)
                {
                    origns[i].probability= origns[i].probability/p;
                }
            }


        }
    }
    class MoveProbability
    {
        public List<MoveType> moveTypes= new List<MoveType>();
        public List<Grid> grids = new List<Grid>();
        public List<double> probility = new List<double>();
        public int Count
        {
            get
            {
                return moveTypes.Count;
            }
        }
        public double Value(int index)
        {
            if (index < Count && index >= 0)
            {
                return probility[index];
            }
            else
            {
                throw new Exception("数组越界");
            }
        }
        public void Add(Grid grid, MoveType type)
        {
            if (!moveTypes.Exists(s => s.Equals(type)))
            {
                moveTypes.Add(type);
                grids.Add(grid);
                probility.Add(grid.currentScore);
            }
        }

        public void ResetProbility()
        {
            double p = 0;
            for (int i = 0; i < Count; i++)
            {
                p += probility[i];
            }
            for (int i = 0; i < Count; i++)
            {
                if (p==0)
                {
                    probility[i] = 1.0 / Count;
                }
                else
                {
                    probility[i] = probility[i] / p;
                }
                
            }
        }
        internal void Clear()
        {
            grids.Clear();
            moveTypes.Clear();
            probility.Clear();
        }
    }
    class FaceProbability
    {
        public List<Face> Faces;
        public double Value(Face face)
        {
            switch (face)
            {
                case Face.F2:
                    return 2.0 / 3.0;
                case Face.F4:
                    return 1.0 / 3.0;
                default:
                    return 0;
            }
        }
        public int Count
        {
            get
            {
                return Faces.Count;
            }
        }
        public Face GetFace(int index)
        {
            if (index < Count && index >= 0)
            {
                return Faces[index];
            }
            else
            {
                throw new Exception("数组越界");
            }
        }
        public FaceProbability()
        {
            Faces = new List<Face>
            {
                Face.F2,
                Face.F4,
            };
        }
    }
    [Serializable]
    class SpaceElements
    {
        public List<byte> Elements = new List<byte>();//Index
        public double ProbabilityOfEach
        {
            get
            {
                if (Elements.Count==0)
                {
                    return 0;
                }
                else
                {
                    return 1.0 / Elements.Count;
                }
            }
        }

        public List<byte> RandTake(int number)
        {
            Random random = new Random();
            return Elements.OrderBy(s => random.Next()).Take(number).ToList();
        }
        public void Add(int index)
        {
            if (!Elements.Exists(s => s.Equals((byte)index)))
            {
                Elements.Add((byte)index);
            }
        }
        public void Remove(int index)
        {
            if (Elements.Exists(s => s.Equals((byte)index)))
            {
                Elements.RemoveAll(s => s == index);
            }
            else
            {
                throw new Exception("元素不存在");
            }
        }
        public byte GetIndex(int index)
        {
            if (index<Count&&index>=0)
            {
                return Elements[index];
            }
            else
            {
                throw new Exception("数组越界");
            }
        }
        public void Clear()
        {
            Elements.Clear();
        }
        public int Count
        {
            get
            {
                return Elements.Count;
            }
        }
    }
    enum Face
    {
        Space,
        F2,
        F4,
        F8,
        F16,
        F32,
        F64,
        F128,
        F256,
        F512,
        F1024,
        F2048,
        F4096,
        F8192,
        F16384,
        F32768,
        F65536,
        F131072,
        F262144,
        F524288,
        F1048576,
    }
    enum MoveType
    {
        Up,
        Down,
        Left,
        Right
    }
    class OperateSerial
    {
        //Grid Current

    }
    class Operate
    {
        Grid Origin;
        public Element ShowElement;
        public MoveType Move;
        Grid Result;
        public Operate(int index,MoveType move,Grid grid)
        {

        }
    }
    [Serializable]
    class Grid:ICloneable
    {
        public SpaceElements spaceElements = new SpaceElements();
        public SpaceElements NonSpaceElements = new SpaceElements();
        public double gameScore=0;
        public double currentScore = 0;
        public double probability = 1;
        public Element[] elements = new Element[16];
        public Grid()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new Element(i, Face.Space);
                spaceElements.Add(i);
            }
        }
        public void Replace(int index,Face face)
        {
            if (index>=16 ||index<0)
            {
                throw new Exception("数组越界");
            }
            if (elements[index].face == face)//替换内容相同，不应存在
            {
                return;
            }
            else
            {
                if (elements[index].face == Face.Space)//插入新元素
                {
                    spaceElements.Remove(index);
                    NonSpaceElements.Add(index);
                }
                else if(face == Face.Space)//删除元素，移动
                {
                    spaceElements.Add(index);
                    NonSpaceElements.Remove(index);
                }
                elements[index].face = face;
            }
        }
        public bool Equals(Grid grid)
        {
            for (int i = 0; i < grid.elements.Length; i++)
            {
                if (grid.elements[i].face!=elements[i].face)
                {
                    return false;
                }
            }
            return true;
        }
        public void Clear(int index)
        {
            
            if (elements[index].face!=Face.Space)
            {
                elements[index].face = Face.Space;
                NonSpaceElements.Remove(index);
                spaceElements.Add(index);
            }
        }
        /// <summary>
        /// 整个Grid的移动处理
        /// </summary>
        /// <param name="moveType"></param>
        /// <returns></returns>
        public bool Move(MoveType moveType)
        {
            currentScore = 0;
            bool isMove=false;
            //如果按下的方向是左或上，则正向遍历非空元素
            int k = moveType ==  MoveType.Left || moveType == MoveType.Up?1:0;

            byte[] bs = NonSpaceElements.Elements.ToArray();
            int N = bs.Length;
            int K = 2 * k - 1;
            int B = (1 - k) * (N - 1);
            for (int i = 0; i < N; i++)
            {
                if (itemMove(bs[K * i + B], moveType))
                {
                    isMove = true;
                }
            }
            //gameScore += currentScore;
            return isMove;
        }
        /// <summary>
        /// 移动单个元素
        /// </summary>
        /// <param name="currentItem"></param>
        /// <param name="direction"></param>
        private bool itemMove(int currentItem, MoveType direction)
        {
            int sideItem = getSideItem(currentItem, direction);

            if (sideItem <0)
            {//当前元素在最边上
             //不动
                return false;
            }
            else if (this.elements[sideItem].face == Face.Space)
            { //当前元素不在最后一个且左/右/上/下侧元素是空元素
                Replace(sideItem, elements[currentItem].face);
                Clear(currentItem);
                itemMove(sideItem, direction);
                return true;
            }
            else if (elements[sideItem].face != elements[currentItem].face)
            {//左（右、上、下）侧元素和当前元素内容不同
             //不动
                return false;
            }
            else
            {//左（右、上、下）侧元素和当前元素内容相同
             //向右合并
                elements[sideItem].DoubleFace();
                Clear(currentItem);
                currentScore += (ulong)(elements[sideItem].Score() * 10);
                // itemMove(sideItem, direction);
                //maxScore = maxScore < gameScore ? gameScore : maxScore;
                return true;
            }
        }
        //获取元素旁边的元素
        private int getSideItem(int currentItem, MoveType direction)
        {
            //当前元素的位置
            var currentItemX = elements[currentItem].X;
            var currentItemY = elements[currentItem].Y;
            var sideItemX=0;
            var sideItemY=0;
            //根据方向获取旁边元素的位置
            switch (direction)
            {
                case MoveType.Left:
                    sideItemX = currentItemX;
                    sideItemY = currentItemY - 1;
                    break;
                case MoveType.Right:
                    sideItemX = currentItemX;
                    sideItemY = currentItemY + 1;
                    break;
                case MoveType.Up:
                    sideItemX = currentItemX - 1;
                    sideItemY = currentItemY;
                    break;
                case MoveType.Down:
                    sideItemX = currentItemX + 1;
                    sideItemY = currentItemY;
                    break;
            }
            //旁边元素
            if (sideItemX>=0&& sideItemX < 4&& sideItemY >= 0 && sideItemY < 4)
            {
                return sideItemX * 4 + sideItemY;
            }
            else
            {
                return -1;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone(); //浅拷贝
        }
        public Grid DeepClone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as Grid;
            }
        }
    }
    [Serializable]
    class Element
    {
        private byte index;

        public byte Index
        {
            get => index;
            set
            {
                X = (byte)(value / 4 );
                Y = (byte)(value % 4);
                index = value;
            }
        }

        public byte X
        {
            get;
            private set;
        }
        public byte Y
        {
            get;
            private set;
        }
        public Face face;
        public Element(int index,Face face)
        {
            this.Index = (byte)index;
            this.face = face;
        }
        /// <summary>
        /// 当前盘面加倍
        /// </summary>
        internal void DoubleFace()
        {
            switch (face)
            {
                case Face.Space:
                    break;
                case Face.F2:
                    face = Face.F4;
                    break;
                case Face.F4:
                    face = Face.F8;
                    break;
                case Face.F8:
                    face = Face.F16;
                    break;
                case Face.F16:
                    face = Face.F32;
                    break;
                case Face.F32:
                    face = Face.F64;
                    break;
                case Face.F64:
                    face = Face.F128;
                    break;
                case Face.F128:
                    face = Face.F256;
                    break;
                case Face.F256:
                    face = Face.F512;
                    break;
                case Face.F512:
                    face = Face.F1024;
                    break;
                case Face.F1024:
                    face = Face.F2048;
                    break;
                case Face.F2048:
                    face = Face.F4096;
                    break;
                case Face.F4096:
                    face = Face.F8192;
                    break;
                case Face.F8192:
                    face = Face.F16384;
                    break;
                case Face.F16384:
                    face = Face.F32768;
                    break;
                case Face.F32768:
                    face = Face.F32768;
                    break;
                case Face.F65536:
                    face = Face.F131072;
                    break;
                case Face.F131072:
                    face = Face.F262144;
                    break;
                case Face.F262144:
                    face = Face.F524288;
                    break;
                case Face.F524288:
                    face = Face.F1048576;
                    break;
                case Face.F1048576:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 盘面分数
        /// </summary>
        /// <returns></returns>
        internal int Score()
        {
            if (face==Face.Space)
            {
                return 0;
            }
            else
            {
                return (int)Math.Pow(2, (int)face);
            }
            
            
        }
    }
}
