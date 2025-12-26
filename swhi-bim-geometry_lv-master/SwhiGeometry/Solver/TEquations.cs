using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using TVector = System.Collections.Generic.List<double>;
//using TMatrix = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using TMatrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using System.Diagnostics;
//using TPEquations = System.Collections.Generic.List<TExpressionTree>;
//using TJacobian= System.Collections.Generic.List<System.Collections.Generic.List<TExpressionTree>>;


namespace Lv.BIM.Solver
{
    public class LEquations
    {
        private List<TExpressionTree> _trees = new List<TExpressionTree>();
        //储存雅克比矩阵，即方程组的一阶导矩阵；
        private TExpressionTree[,] _jacobians;
        private TExpressionTree[,] _hessians;
        private TVariableTable _variableTable;
        public int CountOfEq=>_trees.Count;
        public int CountOfvars=> _variableTable.Count;
        private double abszhiadd = 3;

        //空构造函数
        public LEquations() { }
        public LEquations(List<TExpressionTree> trees, TVariableTable variableTable)
        {
            _trees = trees;
            int n = variableTable.Count;
            if (trees.Count > n)
            {
                throw new Exception("方程数大于变量数，方程组无解");
            }
            else if (trees.Count < n)
            {
                int num=n-trees.Count;
                for(int i=0; i<num; i++)
                {
                    TExpressionTree t = new TExpressionTree();
                    t.LinkVariableTable(variableTable);
                    t.SetExpression(variableTable[n-i-1]);                    
                    _trees.Add(t);
                }
            }
            _jacobians = new TExpressionTree[n, n];
            _hessians = new TExpressionTree[n, n];
            _variableTable = variableTable;
            BuildJacobian();            
        }
        private void BuildJacobian()
        {
            int i = 0;
            foreach(TExpressionTree tep in _trees)
            {
                int j = 0;
                foreach(string st in _variableTable.VariableTable)
                {
                    tep.LinkVariableTable(_variableTable);
                    TExpressionTree diff = tep.Diff(st, 1);
                    _jacobians[i,j] = diff;
                    j++;
                }
                i++;
            }
        }
        private void BuildHessian()
        {
            int i = 0;
            foreach (TExpressionTree tep in _trees)
            {
                int j = 0;
                foreach (string st in _variableTable.VariableTable)
                {
                    tep.LinkVariableTable(_variableTable);
                    TExpressionTree diff = tep.Diff(st, 1);
                    TExpressionTree diff2 = diff.Diff(_variableTable.VariableTable[i], 1);
                    _hessians[i, j] = diff2;
                    j++;
                }
                i++;
            }
        }
        private Matrix<double> CalcJacobian(TVariableTable varTable)
        {
            Matrix<double> jacobian =  Matrix<double>.Build.Dense(CountOfvars, CountOfvars);
            for(int i = 0; i < CountOfvars; i++)
            {
                for (int j = 0; j < CountOfvars; j++)
                {
                    TExpressionTree diff= _jacobians[i,j].Copythis();
                    diff.Subs(varTable.VariableTable, varTable.VariableValue);
                    jacobian[i, j] = diff.Value();

                }
            }

            return jacobian;
        }
        private Matrix<double> CalcHessian(TVariableTable varTable)
        {
            Matrix<double> hessian = Matrix<double>.Build.Dense(CountOfvars, CountOfvars);
            for (int i = 0; i < CountOfvars; i++)
            {
                for (int j = 0; j < CountOfvars; j++)
                {
                    TExpressionTree diff2 = _hessians[i, j].Copythis();
                    diff2.Subs(varTable.VariableTable, varTable.VariableValue);
                    hessian[i, j] = diff2.Value();

                }
            }

            return hessian;
        }
        private Vector<double> CalcValue(TVariableTable varTable)
        {
            List<double> vals=new List<double>();
            foreach(TExpressionTree tep in _trees)
            {
                TExpressionTree newTe=tep.Copythis();
                newTe.Subs(varTable.VariableTable, varTable.VariableValue);
                double val=double.NaN;
                try
                {
                    val = newTe.Value();
                }
                catch (Exception e)
                {
                    
                }                
                vals.Add(val);
            }
            return Vector<double>.Build.Dense(vals.ToArray());
        }
       
        /// <summary>
        /// 牛顿迭代法公式x1=x0-f(x0)/f'(x0)
        /// </summary>
        /// <param name="varTable"></param>
        /// <returns></returns>
        private TVariableTable SolveOne(TVariableTable varTable, out double norm,double xiashanyinzi = 1.0)
        {
            TVariableTable newvarTable = varTable.Copy();
            Matrix<double> jacos= CalcJacobian(varTable);
            Vector<double> q=CalcValue(varTable);
            List<double> zhi = new List<double>(q.ToArray());
            abszhiadd = 0;
            zhi.ForEach(a => abszhiadd += Math.Abs(a));
            Trace.WriteLine(abszhiadd);
            Vector<double> x = jacos.Solve(q);
            List<double> xli = new List<double>(x.ToArray());
            for(int i=0;i<xli.Count;i++)
            {
                xli[i] = xli[i] * xiashanyinzi;
            }
            newvarTable.VariableValue=xli;
            norm = abszhiadd;
            return varTable-newvarTable;
        }
        /// <summary>
        /// 牛顿迭代法
        /// </summary>
        /// <param name="varTable"></param>
        /// <param name="times"></param>
        /// <param name="precision"></param>
        /// <param name="alpha">迭代步长</param>
        /// <param name="gamma"></param>
        /// <param name="sigma">步长缩减系数</param>
        /// <returns></returns>
        public List<double> NewTonSolve(TVariableTable varTable,int times, out int time,out List<double> result, double precision = 0.000001,double alpha=1.0 )
        {
            TVariableTable oldvart=varTable.Copy();
            double fanshu = 100;
            TVariableTable newvarTable = oldvart.Copy();
            time = 0;
            Vector<double> q= CalcValue(oldvart);
            while (fanshu > precision && time<times)
            {
                //初值
                newvarTable = oldvart.Copy();
                TVariableTable dvarTable = oldvart.Copy();
                Matrix<double> jacos = CalcJacobian(oldvart);//df=f'(x0)
                 q= CalcValue(oldvart);//f=f(x0)
                Vector<double> dx = -jacos.Solve(q);//x=f/df                
                List<double> xli = new List<double>(dx.ToArray());
                dvarTable.VariableValue = xli;               
                newvarTable = oldvart + (dvarTable * alpha);
                //初始步长 
                Trace.WriteLine(q.SumMagnitudes());
                //newvart = SolveOne(oldvart,out fanshu, alpha);
                oldvart= newvarTable;
                fanshu = q.SumMagnitudes();
                Trace.WriteLine(time);
                time++;
            }
            result=q.ToList();
            return newvarTable.VariableValue;
        }
        public List<double> NewTonArmijoSolve(TVariableTable varTable, int times,out int time, out List<double> result, double precision = 0.000001, double alpha = 1.0,double sigmma=0.2)
        {
            TVariableTable oldvart = varTable.Copy();
            double fanshu = 100;
            TVariableTable newvarTable = oldvart.Copy();
            time = 0;
            double kalpha = alpha;
            int j = 0;
            Vector<double> q = CalcValue(oldvart);
            //BuildHessian();
            while (fanshu > precision && time < times)
            {
                //初值
                newvarTable = oldvart.Copy();
                TVariableTable dvarTable = oldvart.Copy();
                Matrix<double> jacos = CalcJacobian(oldvart);//df=f'(x0)
                //Matrix<double> hess = CalcHessian(oldvart);//df=f''(x0)
                 q = CalcValue(oldvart);//f=f(x0)
                Vector<double> dx = -jacos.Solve(q);//x=f/df                
                List<double> xli = new List<double>(dx.ToArray());
                dvarTable.VariableValue = xli;
                newvarTable = oldvart + (dvarTable * alpha);
                Vector<double> newq = CalcValue(newvarTable);
                //初始步长
                kalpha = Math.Min(1.0, 4 * kalpha);
                while (newq.SumMagnitudes() > q.SumMagnitudes() * (1 - kalpha * sigmma))
                {
                    kalpha = kalpha / 2;
                    newvarTable = oldvart + (dvarTable * kalpha);
                    newq = CalcValue(newvarTable);
                }
                if (newq.SumMagnitudes() - q.SumMagnitudes() < 1E-6 && newq.SumMagnitudes() > 0.01)
                {
                    j++;
                }
                if (newq.SumMagnitudes()-q.SumMagnitudes()<1E-6 && newq.SumMagnitudes() > 0.01 && j>5)
                {
                    kalpha = 1;
                    List<double> x = xli;
                    for(int i=0;i<x.Count;i++)
                    {
                        Random ra=new Random(time);
                        x[i]+= ra.NextDouble()-0.5;
                    }
                    newvarTable.VariableValue = x;
                    j = 0;
                }
                //
                Trace.WriteLine(q.SumMagnitudes());
                //newvart = SolveOne(oldvart,out fanshu, alpha);
                oldvart = newvarTable;
                fanshu = q.SumMagnitudes();
                Trace.WriteLine(time);
                time++;
            }
            result=q.ToList();
            return newvarTable.VariableValue;
        }
        public List<double> GeneticSolve(TVariableTable varTable, int times, out int time,out List<double> result, double precision = 0.000001,int numOfpopulation=1000,int lenBeforeDot=7, int lenAfterDot = 7, double probability=0.3)
        {
            TVariableTable oldvart = varTable.Copy();
            double fanshu = 100;
            double oldfanshu = 100;
            TVariableTable newvarTable = oldvart.Copy();
            Vector<double> q = CalcValue(oldvart);
            time = 0;
            int numOfPopu = numOfpopulation;
            int numOfchild = (int)Math.Sqrt(numOfPopu);
            List<TVariableTable> initialpopus = initialPopulation(varTable, numOfPopu, lenBeforeDot, lenAfterDot, probability);
            initialpopus[numOfpopulation-1] =varTable;
            List<TVariableTable> childpopus = new List<TVariableTable>() ;
            List<double> fanshus=new List<double>();
            double maxDouDongjingdu = Math.Pow(10, lenBeforeDot);
            double minDouDongjingdu =Math.Pow(10, -lenAfterDot);
            double midleDouDongjingdu = Math.Pow(10, lenBeforeDot/2-lenAfterDot/2);
            double maxDouDongTime = Math.Log10( maxDouDongjingdu / minDouDongjingdu );
            double douDongjingdu = maxDouDongjingdu;
            double bianhuazhi = 0.1;
            int j = 0;//范数连续不变标志
            int m = 0;//范数连续不变标志
            //BuildHessian();
            while (fanshu > precision && time < times)
            {
                //初值
                oldfanshu = fanshu;
                fanshus.Clear();

                foreach (TVariableTable var in initialpopus)
                {
                    Vector<double> zhi = CalcValue(var);//f=f(x0)                    
                    //fanshu = zhi.SumMagnitudes();
                    if (double.IsNaN(fanshu) || double.IsInfinity(fanshu))
                    {
                        fanshus.Add(1E20);
                        continue;
                    }
                    if (fanshu<= precision)
                    {
                        newvarTable=var.Copy();
                        break;
                    }
                    fanshus.Add(zhi.SumMagnitudes());
                }
                List<int> indexes=SortedIndexes(fanshus).GetRange(0, numOfchild);
                //List<int> indexeOfNewtons=SortedIndexes(fanshus).GetRange(5,10);
                childpopus.Clear();
                foreach (int i in indexes)
                {
                    childpopus.Add(initialpopus[i]);
                }
                newvarTable = childpopus[0].Copy();
                fanshu = fanshus[indexes[0]];
                if (Math.Abs(oldfanshu - fanshu) < precision)
                {
                    j++;
                }
                //if (m > times/(lenBeforeDot + lenAfterDot))
                //{
                //    maxDouDongjingdu = maxDouDongjingdu * bianhuazhi;
                //    m = 0;
                //}
                //交叉变异产生新的种群
                initialpopus = CrosslPopulation(childpopus, probability, lenAfterDot);
                List<TVariableTable> douDongPopus=new List<TVariableTable>();
                bool liangjisame = false;
                int jingduliangji =(int)Math.Log10(Math.Abs(douDongjingdu));
                foreach(var item in newvarTable.VariableValue)
                {
                    int liangji= (int)Math.Log10(Math.Abs(item));
                    if (Math.Abs( liangji - jingduliangji)<1)
                    {
                        liangjisame=true;
                    }
                }
                if (liangjisame)
                {
                    numOfPopu = numOfPopu * 100;
                }
                for (int i=0; i< numOfPopu; i++)
                {
                    douDongPopus.Add(newvarTable);
                }
                if (liangjisame)
                {
                    numOfPopu = numOfPopu / 100;
                }
                douDongPopus = DouDongPopulation(douDongPopus, douDongjingdu);
                initialpopus.AddRange(douDongPopus);
                initialpopus[numOfpopulation - 1] = newvarTable;
                q=CalcValue(newvarTable);
                //变异
                //initialpopus = MutatePopulation(initialpopus,2,8,0.5);
                //如果范数连续5次不变，则增大抖动精度的值
                if (j >= lenBeforeDot / 2 + lenAfterDot / 2)
                {                    
                    j = 0;
                    m += 1;
                    if(m % 2 == 0)
                    {
                        douDongjingdu = maxDouDongjingdu;
                    }
                    else
                    {
                        douDongjingdu = minDouDongjingdu;
                    }
                }
                if (m%2==0)
                {
                    if (douDongjingdu > midleDouDongjingdu)
                    {
                        douDongjingdu = douDongjingdu * bianhuazhi;
                    }
                }
                else
                {
                   
                    if (douDongjingdu >= minDouDongjingdu)
                    {
                        douDongjingdu = douDongjingdu /bianhuazhi;
                    }
                }
                
               
                //}


                //TVariableTable dvarTable = oldvart.Copy();
                //Matrix<double> jacos = CalcJacobian(oldvart);//df=f'(x0)
                ////Matrix<double> hess = CalcHessian(oldvart);//df=f''(x0)
                //Vector<double> q = CalcValue(oldvart);//f=f(x0)

                //
                //Trace.WriteLine(childpopus[0]);
                ////newvart = SolveOne(oldvart,out fanshu, alpha);
                Trace.WriteLine(time);
                Trace.WriteLine(fanshu);
                Trace.WriteLine(douDongjingdu);
                
                time++;

            }
            result=q.ToList();
            return newvarTable.VariableValue;
        }
        public static double DouDongDouble(double source,double doudongJingdu)
        {
            double result=0.0;
            Random r=new Random();
            result = source + (r.NextDouble() - 0.5) * doudongJingdu;
            return result;
        }
        public static List<TVariableTable> DouDongPopulation(List<TVariableTable> sourcevartables, double doudongJingdu)
        {
            List<TVariableTable> population = new List<TVariableTable>();
            var varTable = sourcevartables[0];

                foreach (TVariableTable vtb in sourcevartables)
                {
                    TVariableTable tempvarTable = sourcevartables[0].Copy();
                    for (int j = 0; j < varTable.VariableValue.Count; j++)
                    {
                        tempvarTable.VariableValue[j] = DouDongDouble(vtb.VariableValue[j], doudongJingdu);
                    }
                    population.Add(tempvarTable);
                }
            return population;
        }
        public List<double> GeneticSolveOld(TVariableTable varTable, int times, out int time, out List<double> result, double precision = 0.000001, int numOfpopulation = 1000, int lenBeforeDot = 7, int lenAfterDot = 7, double probability = 0.3)
        {
            TVariableTable oldvart = varTable.Copy();
            double fanshu = 100;
            double oldfanshu = 100;
            TVariableTable newvarTable = oldvart.Copy();
            Vector<double> q = CalcValue(oldvart);
            time = 0;
            int numOfPopu = numOfpopulation;
            int numOfchild = (int)Math.Sqrt(numOfPopu);
            List<TVariableTable> initialpopus = initialPopulation(varTable, numOfPopu, lenBeforeDot, lenAfterDot, probability);
            initialpopus[0] = varTable;
            List<TVariableTable> childpopus = new List<TVariableTable>();
            List<double> fanshus = new List<double>();
            int j = 0;//范数连续不变标志
            //BuildHessian();
            while (fanshu > precision && time < times)
            {
                //初值
                oldfanshu = fanshu;
                fanshus.Clear();

                foreach (TVariableTable var in initialpopus)
                {
                    Vector<double> zhi = CalcValue(var);//f=f(x0)                    
                    //fanshu = zhi.SumMagnitudes();
                    if (double.IsNaN(fanshu) || double.IsInfinity(fanshu))
                    {
                        fanshus.Add(10000);
                        continue;
                    }
                    if (fanshu <= precision)
                    {
                        newvarTable = var.Copy();
                        break;
                    }
                    fanshus.Add(zhi.SumMagnitudes());
                }
                List<int> indexes = SortedIndexes(fanshus).GetRange(0, numOfchild);
                //List<int> indexeOfNewtons=SortedIndexes(fanshus).GetRange(5,10);
                childpopus.Clear();
                foreach (int i in indexes)
                {
                    childpopus.Add(initialpopus[i]);
                }
                newvarTable = childpopus[0].Copy();
                fanshu = fanshus[indexes[0]];
                if (fanshu - oldfanshu < precision)
                {
                    j++;
                }

                //交叉变异产生新的种群
                initialpopus = CrosslPopulation(childpopus, probability, lenAfterDot);
                initialpopus[0] = newvarTable;
                q = CalcValue(newvarTable);
                //变异
                //initialpopus = MutatePopulation(initialpopus,2,8,0.5);
                //如果范数连续5次不变，则将一半种群初始化，以带来新的变化
                if (j > 5)
                {
                    j = 0;
                    int start = (int)initialpopus.Count / 2;
                    int numofini = initialpopus.Count;
                    initialpopus.RemoveRange(start, numofini - start);
                    initialpopus.AddRange(initialPopulation(newvarTable, numofini - start, lenBeforeDot, lenAfterDot, probability));
                }
                //TVariableTable dvarTable = oldvart.Copy();
                //Matrix<double> jacos = CalcJacobian(oldvart);//df=f'(x0)
                ////Matrix<double> hess = CalcHessian(oldvart);//df=f''(x0)
                //Vector<double> q = CalcValue(oldvart);//f=f(x0)

                //
                //Trace.WriteLine(childpopus[0]);
                ////newvart = SolveOne(oldvart,out fanshu, alpha);

                Trace.WriteLine(fanshu);
                Trace.WriteLine(time);
                time++;

            }
            result = q.ToList();
            return newvarTable.VariableValue;
        }
        public static double MutateDouble(double source,int magnitudeBeforeDot, int magnitudeAfterDot, double probability)
        {
            double result = 0;
            Random r = new Random();
            char[] formatcharbe=new char[magnitudeBeforeDot];
            char[] formatcharaf=new char[magnitudeAfterDot];
            for(int i = 0; i < formatcharbe.Length; i++)
            {
                formatcharbe[i] = '0';
            }
            for(int i = 0; i < formatcharaf.Length; i++)
            {
                formatcharaf[i] = '0';
            }
            string formatstring = new string(formatcharbe) +"."+ new string(formatcharaf);
            string sourcestring= Math.Abs(source).ToString(formatstring);
            char[] sourcestringchars= sourcestring.ToCharArray();
            for(int i=0;i<sourcestringchars.Length;i++)
            {
                
                if (r.NextDouble() < probability && sourcestringchars[i] != '.')
                {
                    sourcestringchars[i] = r.Next(10).ToString().ToCharArray()[0];
                }
                
            }
            result=double.Parse(new string(sourcestringchars));
            //0.5概率取负值
            if (r.Next(2) == 0)
            {
                result = result *-1;
            }
            return result;
        }
        public static double CrossDouble(double left, double right, int magnitudeAfterDot, double probability = 0.2)
        {
            double result = 0;
            double leftabs = Math.Abs(left);
            double rightabs = Math.Abs(right);
            char[] formatcharaf = new char[magnitudeAfterDot];
            for (int i = 0; i < formatcharaf.Length; i++)
            {
                formatcharaf[i] = '0';
            }
            string formatstring =  "0." + new string(formatcharaf);
            double max = leftabs;
            double min = rightabs;
            if(leftabs < rightabs)
            {
                max = rightabs;
                min = leftabs;
            }
            int magnitudeBeforeDotmax = max.ToString(formatstring).Split('.')[0].Length;
            int magnitudeBeforeDotmin = min.ToString(formatstring).Split('.')[0].Length;
            double difference = magnitudeBeforeDotmax - magnitudeBeforeDotmin; 
            Random r = new Random();
            char[] formatcharbe = new char[magnitudeBeforeDotmax];
            
            for (int i = 0; i < formatcharbe.Length; i++)
            {
                formatcharbe[i] = '0';
            }
            formatstring = new string(formatcharbe) + "." + new string(formatcharaf);
            string maxstring = max.ToString(formatstring);
            string minstring = (min*Math.Pow(10,difference)).ToString(formatstring);
            char[] maxstringchars = maxstring.ToCharArray();
            char[] minstringchars = minstring.ToCharArray();
            char[] resultstringchars = maxstring.ToCharArray();
            for (int i = 0; i < maxstringchars.Length; i++)
            {               
                if (r.Next(2) == 0)
                {
                    resultstringchars[i] = maxstringchars[i];
                }
                else
                {
                    if (minstringchars.Length-1 < i) 
                    {
                        resultstringchars[i] = '0';
                    }
                    else
                    {
                        resultstringchars[i] = minstringchars[i];
                    }
                    
                }
                if (r.NextDouble() < probability)
                {
                    resultstringchars[i] = r.Next(10).ToString().ToCharArray()[0];
                }
            }
            
            result = double.Parse(new string(resultstringchars));
            //变换正负号
            if (left * right < 0)
            {
                if (r.Next(2) == 0)
                {
                    result = result * -1;
                }
            }
            else if (left < 0)
            {
                result = result * -1;
            }
            //0.5概率取小值
            if (r.Next(2) == 0)
            {
                result = result / Math.Pow(10, difference);
            }           
            
            return result;
        }
        public static double CrossDoubleBeforeDot(double left, double right, int magnitudeBeforeDot, double probability = 0.2)
        {
            double result = 0;
            Random r = new Random();
            string maxstring = left.ToString().Split('.')[0];
            string minstring = right.ToString().Split('.')[0];
            char[] maxstringchars = maxstring.ToCharArray();
            char[] minstringchars = minstring.ToCharArray();
            int len = Math.Max(maxstringchars.Length, minstringchars.Length);
            len = Math.Max(len, magnitudeBeforeDot);
            char[] foramtchars = new char[len];
            for (int i = 0; i < foramtchars.Length; i++)
            {
                foramtchars[i] = '0';
            }
            string format = new string(foramtchars);
            maxstringchars = ((int)Math.Floor(Math.Abs( left))).ToString(format).ToCharArray();
            minstringchars = ((int)Math.Floor(Math.Abs(right))).ToString(format).ToCharArray();
            char[] resultstringchars = minstringchars;
            for (int i = 0; i < minstringchars.Length; i++)
            {
                if (r.Next(2) == 0)
                {
                    resultstringchars[i] = maxstringchars[i];
                }
                else
                {
                    resultstringchars[i] = minstringchars[i];

                }
                if (r.NextDouble() < probability)
                {
                    resultstringchars[i] = r.Next(10).ToString().ToCharArray()[0];
                }
            }

            result = double.Parse(new string(resultstringchars));
            //变换正负号
            if (left * right < 0)
            {
                if (r.Next(2) == 0)
                {
                    result = result * -1;
                }
            }
            else if (left < 0)
            {
                result = result * -1;
            }

            return result;
        }
        public static double CrossDoubleAfterDot(double left, double right, int magnitudeAfterDot, double probability = 0.2)
        {
            double result = 0;
            Random r = new Random();
            char[] foramtchars = new char[magnitudeAfterDot];
            for (int i = 0; i < foramtchars.Length; i++)
            {
                foramtchars[i] = '0';
            }
            string format = "0." + new string(foramtchars);
            string beforedot = (left/2+right / 2).ToString(format).Split('.')[0];
            
            string maxstring = left.ToString(format).Split('.')[1];
            string minstring = right.ToString(format).Split('.')[1];
            char[] maxstringchars = maxstring.ToCharArray();
            char[] minstringchars = minstring.ToCharArray();
            int len = maxstringchars.Length;
            char[] resultstringchars = maxstring.ToCharArray();
            for (int i = 0; i < maxstringchars.Length; i++)
            {
                if (r.Next(2) == 0)
                {
                    resultstringchars[i] = maxstringchars[i];
                }
                else
                {
                    if (minstringchars.Length - 1 < i)
                    {
                        resultstringchars[i] = '0';
                    }
                    else
                    {
                        resultstringchars[i] = minstringchars[i];
                    }

                }
                if (r.NextDouble() < probability)
                {
                    resultstringchars[i] = r.Next(10).ToString().ToCharArray()[0];
                }
            }
            result = double.Parse(beforedot + "." + new string(resultstringchars));
            
            return result;
        }
        public static List<TVariableTable> initialPopulation(TVariableTable varTable, int numberOfPopulation,int magnitudeBeforeDot, int magnitudeAfterDot, double probability)
        {
            List<TVariableTable> population = new List<TVariableTable>();
            Random random = new Random();

            for(int i = 0; i < numberOfPopulation; i++)
            {
                TVariableTable tempvarTable = varTable.Copy();
                for (int j=0;j<varTable.VariableValue.Count;j++) 
                {                    
                    tempvarTable.VariableValue[j]= MutateDouble(random.Next()/ random.Next(), magnitudeBeforeDot ,  magnitudeAfterDot, probability);
                }
                population.Add(tempvarTable);
            }
            return population;
        }
        public static List<TVariableTable> CrosslPopulation(List<TVariableTable> sourcevartables,double probability ,int magnitudeAfterDot)
        {
            List<TVariableTable> population = new List<TVariableTable>();
            var varTable = sourcevartables[0];
            foreach (TVariableTable vta in sourcevartables)
            {
                foreach (TVariableTable vtb in sourcevartables)
                {
                    TVariableTable tempvarTable = sourcevartables[0].Copy();
                    for (int j = 0; j < varTable.VariableValue.Count; j++)
                    {
                        tempvarTable.VariableValue[j] = CrossDouble(vta.VariableValue[j], vtb.VariableValue[j], magnitudeAfterDot, probability);
                    }
                    population.Add(tempvarTable);
                }                    
            }
            return population;
        }
        public static List<TVariableTable> CrosslPopulationBeforeDot(List<TVariableTable> sourcevartables, double probability, int magnitudeBeforeDot)
        {
            List<TVariableTable> population = new List<TVariableTable>();
            var varTable = sourcevartables[0];
            foreach (TVariableTable vta in sourcevartables)
            {
                foreach (TVariableTable vtb in sourcevartables)
                {
                    TVariableTable tempvarTable = sourcevartables[0].Copy();
                    for (int j = 0; j < varTable.VariableValue.Count; j++)
                    {
                        tempvarTable.VariableValue[j] = CrossDoubleBeforeDot(vta.VariableValue[j], vtb.VariableValue[j], magnitudeBeforeDot, probability);
                    }
                    population.Add(tempvarTable);
                }
            }
            return population;
        }
        public static List<TVariableTable> CrosslPopulationAfterDot(List<TVariableTable> sourcevartables, double probability, int magnitudeAfterDot)
        {
            List<TVariableTable> population = new List<TVariableTable>();
            var varTable = sourcevartables[0];
            foreach (TVariableTable vta in sourcevartables)
            {
                foreach (TVariableTable vtb in sourcevartables)
                {
                    TVariableTable tempvarTable = sourcevartables[0].Copy();
                    for (int j = 0; j < varTable.VariableValue.Count; j++)
                    {
                        tempvarTable.VariableValue[j] = CrossDoubleAfterDot(vta.VariableValue[j], vtb.VariableValue[j], magnitudeAfterDot, probability);
                    }
                    population.Add(tempvarTable);
                }
            }
            return population;
        }
        public static List<TVariableTable> MutatePopulation(List<TVariableTable> sourcevartables, int magnitudeBeforeDot = 7, int magnitudeAfterDot = 8, double probability = 0.2)
        {
            Random r = new Random();
            List<TVariableTable> population = sourcevartables;
            var varTable = sourcevartables[0];
            for (int i=0;i< sourcevartables.Count;i++)
            {
                
                for (int j = 0; j < varTable.VariableValue.Count; j++)
                {
                    if (r.NextDouble() < probability)
                    {
                        population[i].VariableValue[j] = MutateDouble(sourcevartables[i].VariableValue[j], magnitudeBeforeDot, magnitudeAfterDot, probability);
                    }

                }
            }
            return population;
        }
        public static List<int> SortedIndexes(List<double> source)
        {
            var sorted = source
                .Select((x, i) => new KeyValuePair<int, double>(i, x))
                .OrderBy(x => x.Value)
                .ToList();
            List<int> indexes = sorted.Select(x => x.Key).ToList();
            return indexes;
        }
    }

    /// <summary>
    /// TEquations是C++代码转译而来的，未经正确修改
    /// 建议使用新方法LEquations求解方程组
    /// </summary>
    public class TEquations
    {
        //enumError eError;
        private List<bool> EquationIsTemp = new List<bool>();

        TExpressionTree[,] Jacobian_lv = new TExpressionTree[2,2];
        private List<List<TExpressionTree>> Jacobian = new List<List<TExpressionTree>>();
        private List<TExpressionTree> Equations = new List<TExpressionTree>();
        private List<TExpressionTree> EquationsV = new List<TExpressionTree>();
        private List<TExpressionTree> EquationsA = new List<TExpressionTree>();
        private TVariableTable VariableTableSolved = new TVariableTable(); //已解出变量表
                                                                           //lvwan定义
        public double epsilon = 1e-12;
        public uint max_step = 20;
        public bool hasSolved;
        public TVariableTable VariableTable = new TVariableTable(); //总变量表
        public TVariableTable VariableTableUnsolved = new TVariableTable();
        public TVariableTable VariableTableV = new TVariableTable(); //速度总变量表
        public TVariableTable VariableTableA = new TVariableTable(); //速度总变量表
        private void MatrixMultiplyVector(ref TVector Result, ref TMatrix Matrix, ref TVector Vector)
        {
            Result.Clear();
            for (int i = 0; i < Matrix.Count; i++)
            {
                List<double> Row = Matrix[i];
                double temp = 0;
                for (var iter = 0; iter < Row.Count(); ++iter)
                {
                    temp += (iter) * Vector[iter - 0];
                }
                Result.Add(temp);
            }
        }


        //利用变量表中的值计算雅可比
        private void CalcJacobianValue(string pOS, ref TMatrix JacobianValueResult, List<List<TExpressionTree>> Jacobian)
        {
            JacobianValueResult.Clear();
            //lvwan20220501：我的理解是Jacobian[0]即为一行，假设Jacobian={{1，2}，{3，4}，{5，6}}即为3行2列
            JacobianValueResult = new List<List<double>>(Jacobian.Count());// Jacobian[0].Count());
            //JacobianValueResult.resize(Jacobian.Count);
            TExpressionTree temp;
            for (int i = 0; i < Jacobian.Count(); i++)
            {
                //lvwan
                List<double> valuelisttemp = new List<double>();
                for (int j = 0; j < Jacobian[i].Count(); j++)
                {
                    //temp = new TExpressionTree();
                    TExpressionTree exprJacobian = Jacobian[i][j];
                    temp = exprJacobian;
                    try
                    {
                        temp.Vpa();
                        //lvwan
                        //JacobianValueResult[i][j] = temp.Value(true); //得到临时表达式值存入雅可比
                        valuelisttemp.Add(temp.Value());
                    }
                    catch (TError err)
                    {
                        if (pOS != null)
                        {
                            pOS = pOS + ("ERROR:");
                            pOS = pOS + temp.OutputStr();
                            pOS = pOS + ("\r\nJacobian计算出错:");
                            pOS = pOS + err.GetErrorInfo(err.id) + err.info;
                        }
                        //temp.Dispose();//20220702注释
                        throw err;
                    }
                    //temp.Dispose();//20220702注释
                }
                JacobianValueResult.Add(valuelisttemp);
            }
        }
        private int GetMaxAbsRowIndex(ref TMatrix A, int RowStart, int RowEnd, int Col)
        {
            double max = 0.0;
            int index = RowStart;
            for (int i = RowStart; i <= RowEnd; i++)
            {
                if (Math.Abs(A[i][ Col]) > max)
                {
                    max = Math.Abs(A[i][Col]);
                    index = i;
                }
            }
            return index;
        }
        private void SwapRow(ref TMatrix A, ref TVector b, int i, int j)
        {
            if (i == j)
                return;
            List<double> temp = new List<double>( A[i].Count);
            temp = A[i];
            A[i]=A[j];
            A[j]=temp;

            double n;
            n = b[i];
            b[i] = b[j];
            b[j] = n;
        }
        private bool AllIs0(ref TVector V)
        {
            foreach (var value in V)
            {
                if (Math.Abs(value) >= epsilon)
                    return false;
            }
            return true;
        }
        private bool VectorAdd(ref TVector Va, ref TVector Vb)
        {
            if (Va.Count != Vb.Count)
                return false;
            for (int i = 0; i < Va.Count(); i++)
            {
                Va[i] += Vb[i];
            }
            return true;
        }
        private string temp = new string(new char[64]);
        private void Output(string pOS, ref TMatrix m)
        {
            if (pOS != null)
            {
                //			static sbyte temp[64];
                pOS = pOS + ("[");
                for (int i = 0; i < m.Count; i++)
                {
                    for (int j = 0; j < m[i].Count(); j++)
                    {
                        //_stprintf(temp, TEXT("%f"), m[i,j]);
                        temp = ("%f") + m[i][j].ToString();
                        pOS = pOS + temp;
                        pOS = pOS + (" ");
                    }
                    if (i != m.Count - 1)
                        pOS = pOS + (";\r\n");
                }
                pOS = pOS + ("]\r\n\r\n");
            }
        }

        private void Output(string pOS, ref TVector v)
        {
            if (pOS != null)
            {
                //			static sbyte temp[64];
                pOS = pOS + ("[");
                foreach (var value in v)
                {
                    //_stprintf(temp, TEXT("%f"), value);
                    temp = ("%f") + value.ToString();
                    pOS = pOS + temp;
                    pOS = pOS + (" ");
                }
                pOS = pOS + ("]\r\n\r\n");
            }
        }
        private void ReleaseTPEquations(List<TExpressionTree> Equations)
        {
            //foreach (var pEqua in Equations)
            //	pEqua = null;
            Equations.Clear();
        }
        private void ReleaseJacobian(List<List<TExpressionTree>> Jacobian)
        {
            //for (int i = 0; i < Jacobian.Count(); i++)
            //	for (int j = 0; j < Jacobian[i].Count(); j++)
            //		Jacobian[i,j] = null;

            //Jacobian.Clear();
        }
        private void SubsVar(string pOS, ref List<TExpressionTree> Equations, ref TVariableTable LinkVariableTable, string VarStr, double Value)
        {
            var it = LinkVariableTable.FindVariableTable(VarStr);
            if (it < LinkVariableTable.VariableTable.Count())
            {
                string Var;
                Var = it.ToString();
                foreach (var pEquation in Equations)
                {
                    pEquation.Subs(Var, Value);
                }
            }
        }
        public TEquations()
        {
            VariableTableUnsolved.bShared = true;
            VariableTableSolved.bShared = true;
        }
        public void Dispose()
        {
            //释放方程组
            ReleaseTPEquations(Equations);
            ReleaseTPEquations(EquationsV);
            ReleaseTPEquations(EquationsA);

            //释放雅可比
            //ReleaseJacobian(Jacobian);
        }



        public TExpressionTree GetLastExpressionTree()
        {
            return Equations[Equations.Count - 1];
        }

        //逐个方程对t求导，得到速度方程组右边
        public void BuildEquationsV(string pOS)
        {
            bool bOutput = pOS == null ? false : true;

            if (pOS != null)
            {
                pOS = pOS + (">>BuildEquationsV: \r\n");
                pOS = pOS + ("当前方程：\r\n");
            }

            TExpressionTree pEquatemp;
            foreach (var pEqua in Equations)
            {
                pEquatemp = new TExpressionTree();
                pEquatemp = pEqua;

                pEquatemp.Diff("t", 1);
                pEquatemp.Simplify();

                EquationsV.Add(pEquatemp);

                if (pOS != null)
                {
                    pOS = pOS + pEquatemp.OutputStr();
                    pOS = pOS + ("\r\n");
                }
            }
        }

        //逐个方程对t求导，得到速度方程组右边
        public void BuildEquationsA_Phitt(string pOS)
        {
            bool bOutput = pOS == null ? false : true;

            if (pOS != null)
            {
                pOS = pOS + (">>Build Equations A: \r\n");
                pOS = pOS + ("当前方程：\r\n");
            }

            TExpressionTree pEquatemp;
            foreach (var pEqua in EquationsV)
            {
                pEquatemp = new TExpressionTree();
                pEquatemp = pEqua;

                pEquatemp.Diff("t", 1);
                pEquatemp.Simplify();

                EquationsA.Add(pEquatemp);

                if (pOS != null)
                {
                    pOS = pOS + pEquatemp.OutputStr();
                    pOS = pOS + ("\r\n");
                }
            }
        }

        //应在解出位置、速度方程后调用
        public void CalcEquationsARight(string pOS, ref TVector Right)
        {
            //复制Jacobian矩阵
            List<List<TExpressionTree>> JacobianTemp = new List<List<TExpressionTree>>();
            CopyJacobian(ref JacobianTemp, ref Jacobian);
#if _DEBUG
		OutputJacobian(pOS, JacobianTemp);
#endif

            //Jacobian*q' 乘以q'
            List<TExpressionTree> EquationsTemp = new List<TExpressionTree>();
            CalcJacobianMultiplyVector(ref EquationsTemp, ref JacobianTemp, ref VariableTableV.VariableValue);
#if _DEBUG
		OutputPhi(pOS,EquationsTemp);
#endif

            //(Jacobian*q')q  对q求导
            BuildJacobian_inner(ref JacobianTemp, ref EquationsTemp, ref VariableTableA);
#if _DEBUG
		OutputJacobian(pOS, JacobianTemp);
#endif

            //Vpa: (Jacobian*q')q
            TMatrix MatrixTemp = new TMatrix(); ;
            CalcJacobianValue(pOS, ref MatrixTemp, JacobianTemp);
#if _DEBUG
		Output(pOS,MatrixTemp);
#endif

            // *q'
            MatrixMultiplyVector(ref Right, ref MatrixTemp, ref VariableTableV.VariableValue);
#if _DEBUG
		Output(pOS,Right);
#endif

            //-Phitt
            TVector Phitt = new List<double>();
            CalcPhiValue(pOS, ref EquationsA, ref Phitt);
#if _DEBUG
		Output(pOS,Phitt);
#endif

            //-Right-Phitt
            for (var iter = 0; iter < Right.Count(); ++iter)//Right.GetEnumerator()改为0
                Right[iter] = -Right[iter] + Phitt[iter];

#if _DEBUG
		Output(pOS,Right);
#endif

            ReleaseJacobian(JacobianTemp);
            ReleaseTPEquations(EquationsTemp);
        }
        public void OutputPhi(ref string pOS, List<TExpressionTree> Equations)
        {
            if (pOS != null)
            {
                pOS = pOS + ("Phi(1x");
                pOS = pOS + Equations.Count();
                pOS = pOS + (")=\r\n[");
                for (var iter = 0; iter < Equations.Count(); ++iter)
                {
                    pOS = pOS + Equations[iter].OutputStr();
                    if (iter < Equations.Count() - 1)
                        pOS = pOS + (";\r\n");
                }
                pOS = pOS + ("]\r\n\r\n");
            }
        }

        //链接VariableTableUnsolved
        public void BuildJacobian(ref string pOS)
        {
            BuildJacobian_inner(ref Jacobian, ref Equations, ref VariableTableUnsolved);

            //纯输出
            if (pOS != null)
            {
                pOS = pOS + (">>Build Jacobian:\r\n\r\n");

                OutputPhi(ref pOS, Equations);

                OutputJacobian(ref pOS, Jacobian);
            }
        }

        //Copy Jacobian
        public void CopyJacobian(ref List<List<TExpressionTree>> ResultJacobian, ref List<List<TExpressionTree>> OriginJacobian)
        {
            ReleaseJacobian(ResultJacobian);

            ResultJacobian = new List<List<TExpressionTree>>(OriginJacobian.Count());
            TExpressionTree temp;
            for (int i = 0; i < OriginJacobian.Count(); i++)
            {
                TEquations rowlist = new TEquations();
                for (int j = 0; j < OriginJacobian[i].Count(); j++)
                {

                    temp = new TExpressionTree();
                    temp = OriginJacobian[i][j];

                    ResultJacobian[i][j] = temp;
                }
            }
        }
        public void OutputJacobian(ref string pOS, List<List<TExpressionTree>> Jacobian)
        {
            //纯输出
            if (pOS != null)
            {
                pOS = pOS + ("Jacobian(");
                pOS = pOS + (Jacobian.Count() > 0 ? Jacobian[0].Count() : 1) + "x" + Jacobian.Count();
                pOS = pOS + (")=\r\n[");
                for (int ii = 0; ii < Jacobian.Count(); ii++)
                {
                    for (int jj = 0; jj < Jacobian[ii].Count(); jj++)
                    {
                        pOS = pOS + Jacobian[ii][jj].OutputStr();
                        pOS = pOS + (" ");
                    }
                    if (ii != Jacobian.Count() - 1)
                        pOS = pOS + (";\r\n");
                }
                pOS = pOS + ("]\r\n\r\n");
            }
        }

        //将未解出变量赋值给速度变量组
        public void BuildVariableTableV(ref string pOS)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: VariableTableV = VariableTableUnsolved;
            VariableTableV = VariableTableUnsolved;
            VariableTableV.bShared = true;
        }

        //将未解出变量赋值给速度变量组
        public void BuildVariableTableA(ref string pOS)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: VariableTableA = VariableTableUnsolved;
            VariableTableA = VariableTableUnsolved;
            VariableTableA.bShared = true;
        }
        public int GetEquationsCount()
        {
            return Equations.Count();
        }
        public void AddEquation(ref string pOS, string szInput, bool istemp)
        {
            TExpressionTree temp = new TExpressionTree();
            temp.LinkVariableTable(VariableTable);
            temp.SetExpression(szInput);
            temp.Simplify();

            //加入方程组
            Equations.Add(temp);
            EquationIsTemp.Add(istemp);

            hasSolved = false;

            if (pOS != null)
            {
                pOS = pOS + (">>Add:\r\n");
                pOS = pOS + temp.OutputStr();
                pOS = pOS + ("\r\n\r\n");
            }
        }
        public void RemoveTempEquations()
        {
            int i;
            i = EquationIsTemp.Count - 1;
            for (; i > -1; i--)
            {
                if (EquationIsTemp[i] == true)
                {
                    List<bool>.Enumerator iter1 = EquationIsTemp.GetEnumerator() ;//去掉+i
                    List<TExpressionTree>.Enumerator iter2 = Equations.GetEnumerator() ;//去掉+i
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
                    //EquationIsTemp.erase(iter1);

                    Equations[i] = null;
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
                    //Equations.erase(iter2);
                }
            }

            ReleaseTPEquations(EquationsV);
            ReleaseTPEquations(EquationsA);

            //VariableTableUnsolved = VariableTable;
            //VariableTableUnsolved.bShared = true;

        }
        public enumError SolveLinear(ref TMatrix A, ref TVector x, ref TVector b)
        {
            var m = A.Count; //行数
            var n = m; //列数=未知数个数

            var RankA = m;
            var RankAb = m; //初始值

            if (x.Count != m) //仅对方阵成立
            {
                x = new List<double>(m);
                //x.resize(m);
            }


            if (m != b.Count) //Jacobian行数不等于Phi行数
                return enumError.ERROR_JACOBI_ROW_NOT_EQUAL_PHI_ROW;

            if (m > 0)
                if ((n = A[0].Count()) != m) //不是方阵
                {
                    if (m > n)
                        return enumError.ERROR_OVER_DETERMINED_EQUATIONS; //过定义方程组
                    else //不定方程组
                    {
                        x = new List<double>(n);
                        //x=new 
                    }


                }

            List<int> TrueRowNumber = new List<int>(n);
            //lvwan以下方法为自己改写方法，可能存在错误
            int xi = 0;
            int yi = 0;
            for (int j = 0; j < n; j++)
            {
                //if (A[i].Count() != m)

                //从当前行(y)到最后一行(m-1)中，找出x列最大的一行与y行交换
                SwapRow(ref A, ref b, yi, GetMaxAbsRowIndex(ref A, yi, m - 1, xi));

                while (Math.Abs(A[yi][xi]) < epsilon) //如果当前值为0 x一直递增到非0
                {
                    xi++;
                    if (xi == n)
                        break;

                    //交换本行与最大行
                    SwapRow(ref A, ref b, yi, GetMaxAbsRowIndex(ref A, yi, m - 1, xi));
                }

                if (xi != n && xi > yi)
                {
                    TrueRowNumber[yi] = xi; //补齐方程时 当前行应换到xi行
                }

                if (xi == n) //本行全为0
                {
                    RankA = yi;
                    if (Math.Abs(b[yi]) < epsilon)
                        RankAb = yi;

                    if (RankA != RankAb) //奇异，且系数矩阵及增广矩阵秩不相等->无解
                        return enumError.ERROR_SINGULAR_MATRIX;
                    else
                        break; //跳出for，得到特解
                }

                //主对角线化为1
                double m_num = A[yi][ xi];
                for (int jj = yi; jj < n; jj++) //yi行第j个->第n个
                    A[yi][jj] /= m_num;
                b[yi] /= m_num;

                //每行化为0
                for (int row = yi + 1; row < m; row++) //下1行->最后1行
                {
                    if (Math.Abs(A[row][ xi]) < epsilon)
                    { }
                    else
                    {
                        double mi = A[row][xi];
                        for (int col = xi; col < n; col++) //row行第x个->第n个
                        {
                            A[row][ col] -= A[yi][col] * mi;
                        }
                        b[row] -= b[yi] * mi;
                    }
                }
                xi++;
                yi++;
                if (xi == n && yi == m)
                {
                    break;
                }
            }
            //列主元消元法
            //for (decltype(m) y = 0, x = 0; y < m && x < n; y++, x++)
            //{
            //	//if (A[i].Count() != m)

            //	//从当前行(y)到最后一行(m-1)中，找出x列最大的一行与y行交换
            //	SwapRow(A,b, y, GetMaxAbsRowIndex(A, y, m - 1, x));

            //	while (Math.Abs(A[y][x]) < epsilon) //如果当前值为0 x一直递增到非0
            //	{
            //		x++;
            //		if (x == n)
            //			break;

            //		//交换本行与最大行
            //		SwapRow(A,b, y, GetMaxAbsRowIndex(A, y, m - 1, x));
            //	}

            //	if (x != n && x > y)
            //	{
            //		TrueRowNumber[y] = x; //补齐方程时 当前行应换到x行
            //	}

            //	if (x == n) //本行全为0
            //	{
            //		RankA = y;
            //		if (Math.Abs(b[y]) < epsilon)
            //			RankAb = y;

            //		if (RankA != RankAb) //奇异，且系数矩阵及增广矩阵秩不相等->无解
            //			return enumError.ERROR_SINGULAR_MATRIX;
            //		else
            //			break; //跳出for，得到特解
            //	}

            //	//主对角线化为1
            //	double m_num = A[y][x];
            //	for (decltype(m) j = y; j < n; j++) //y行第j个->第n个
            //		A[y][j] /= m_num;
            //	b[y] /= m_num;

            //	//每行化为0
            //	for (decltype(m) row = y + 1; row < m; row++) //下1行->最后1行
            //	{
            //		if (Math.Abs(A[row][x]) < epsilon)
            //			;
            //		else
            //		{
            //			double mi = A[row][x];
            //			for (var col = x; col < n; col++) //row行第x个->第n个
            //			{
            //				A[row][col] -= A[y][col]  mi;
            //			}
            //			b[row] -= b[y]  mi;
            //		}
            //	}
            //}

            bool bIndeterminateEquation = false; //设置此变量是因为后面m将=n，标记以判断是否为不定方程组

            //若为不定方程组，空缺行全填0继续运算
            if (m != n)
            {
                A=new TMatrix(n); //A改为n行
                for (var i = m; i < n; i++) //A从m行开始每行n个数
                    A[i]=new TVector(n);
                b = new List<double>(n);
                m = n;
                bIndeterminateEquation = true;

                //调整顺序
                for (int i = m - 1; i >= 0; i--)
                {
                    if (TrueRowNumber[i] != 0)
                    {
                        SwapRow(ref A, ref b, i, TrueRowNumber[i]);
                    }
                }
            }

            //后置换得到x
            double sum_others = 0.0;
            for (int i = m - 1; i >= 0; i--) //最后1行->第1行
            {
                sum_others = 0.0;
                for (int j = i + 1; j < m; j++) //本列 后的元素乘以已知x 加总
                {
                    sum_others += A[i][ j] * x[j];
                }
                x[i] = b[i] - sum_others;
            }

            if (RankA < n && RankA == RankAb)
            {
                if (bIndeterminateEquation)
                    return enumError.ERROR_INDETERMINATE_EQUATION;
                else
                    return enumError.ERROR_INFINITY_SOLUTIONS;
            }

            return enumError.ERROR_NO;
        }

        //牛顿-拉夫森方法求解
        public void SolveEquations(ref string pOS)
        {
            if (hasSolved == false)
            {
                if (pOS != null)
                {
                    pOS = pOS + (">>SolveEquations:\r\n");
                    pOS = pOS + ("当前未知量：\r\n");
                }
                pOS=VariableTableUnsolved.Output(); //输出当前变量

                TMatrix JacobianValue = new TMatrix();
                TVector PhiValue = new TVector();
                TVector DeltaQ = new TVector();
                TVector Q = VariableTableUnsolved.VariableValue;
                TVector VariableValueBackup = VariableTableUnsolved.VariableValue;
                uint n = 0;

                while (true)
                {
                    if (pOS != null)
                    {
                        pOS = pOS + ("q(");
                        pOS = pOS + n;
                        pOS = pOS + (")=\r\n");
                        Output(pOS, ref Q);
                        pOS = pOS + ("\r\n");
                    }

                    try
                    {
                        CalcJacobianValue(pOS, ref JacobianValue, Jacobian);
                    }
                    catch (TError err)
                    {
                        if (pOS != null)
                        {
                            pOS = pOS + ("无法计算。\r\n");
                            pOS = pOS + err.GetErrorInfo(err.id) + err.info + "\n";
                        }
                        VariableTableUnsolved.VariableValue = VariableValueBackup;
                        return;
                    }

                    if (pOS != null)
                    {
                        pOS = pOS + ("Jacobian(");
                        pOS = pOS + n;
                        pOS = pOS + (")=\r\n");
                        Output(pOS, ref JacobianValue);
                        pOS = pOS + ("\r\n");
                    }

                    try
                    {
                        CalcPhiValue(pOS, ref Equations, ref PhiValue);
                    }
                    catch (TError err)
                    {
                        if (pOS != null)
                        {
                            pOS = pOS + ("无法计算。\r\n");
                            pOS = pOS + err.GetErrorInfo(err.id) + err.info + "\n";
                        }
                        VariableTableUnsolved.VariableValue = VariableValueBackup;
                        return;
                    }
                    if (pOS != null)
                    {
                        pOS = pOS + ("Phi(");
                        pOS = pOS + n;
                        pOS = pOS + (")=\r\n");
                        Output(pOS, ref PhiValue);
                        pOS = pOS + ("\r\n");
                    }

                    switch (SolveLinear(ref JacobianValue, ref DeltaQ, ref PhiValue))
                    {
                        case enumError.ERROR_SINGULAR_MATRIX:
                            //矩阵奇异
                            if (pOS != null)
                                pOS = pOS + ("Jacobian矩阵奇异且无解（初值不合适或者存在矛盾方程）。\r\n");
                            VariableTableUnsolved.VariableValue = VariableValueBackup;
                            return;
                        case enumError.ERROR_INDETERMINATE_EQUATION:
                            if (pOS != null)
                                pOS = pOS + ("不定方程组。返回一组特解。\r\n");
                            break;
                        case enumError.ERROR_JACOBI_ROW_NOT_EQUAL_PHI_ROW:
                            if (pOS != null)
                                pOS = pOS + ("Jacobian矩阵与Phi向量行数不等，程序出错。\r\n");
                            VariableTableUnsolved.VariableValue = VariableValueBackup;
                            return;
                        case enumError.ERROR_INFINITY_SOLUTIONS:
                            if (pOS != null)
                                pOS = pOS + ("Jacobian矩阵奇异，但有无穷多解（存在等价方程）。返回一组特解。\r\n");
                            break;
                        case enumError.ERROR_OVER_DETERMINED_EQUATIONS:
                            if (pOS != null)
                                pOS = pOS + ("矛盾方程组，无法求解。\r\n");
                            VariableTableUnsolved.VariableValue = VariableValueBackup;
                            return;
                    }

                    if (pOS != null) //输出DeltaQ
                    {
                        pOS = pOS + ("Δq(");
                        pOS = pOS + n;
                        pOS = pOS + (")=\r\n");
                        Output(pOS, ref DeltaQ);
                        pOS = pOS + ("\r\n\r\n");
                    }

                    VectorAdd(ref Q, ref DeltaQ);

                    if (AllIs0(ref DeltaQ))
                        break;

                    if (n > max_step - 1)
                    {
                        if (pOS != null)
                            pOS = pOS + ("超过") + max_step + "步仍未收敛。\r\n";
                        VariableTableUnsolved.VariableValue = VariableValueBackup;
                        return;
                    }
                    n++;
                }
                //此处已解出

                VariableTable.SetValueByVarTable(VariableTableUnsolved);

                hasSolved = true;
            }

            if (pOS != null)
                pOS = pOS + ("\r\n得到结果：\r\n");
            pOS+=VariableTableUnsolved.OutputValue();
        }
        public void SolveEquationsV(string pOS)
        {
            TMatrix JacobianV = new TMatrix();
            TVector Phi = new TVector();
            TVector dQ = VariableTableV.VariableValue;
            CalcPhiValue(pOS, ref EquationsV, ref Phi);
            CalcJacobianValue(pOS, ref JacobianV, Jacobian);
            SolveLinear(ref JacobianV, ref dQ, ref Phi);

            if (pOS != null)
            {
                pOS = pOS + (">>SolveEquationsV:\r\n");
                if (pOS != null)
                    pOS = pOS + ("\r\n得到结果：\r\n");
                pOS += VariableTableV.OutputValue();
            }
        }
        public void SolveEquationsA(string pOS)
        {
            TMatrix JacobianA = new TMatrix();
            TVector Phi = new TVector();
            TVector ddQ = VariableTableA.VariableValue;
            CalcJacobianValue(pOS, ref JacobianA, Jacobian); //JacobianA与Jacobian相等
            CalcEquationsARight(pOS, ref Phi);
            SolveLinear(ref JacobianA, ref ddQ, ref Phi);

            if (pOS != null)
            {
                pOS = pOS + (">>SolveEquationsA:\r\n");
                if (pOS != null)
                    pOS = pOS + ("\r\n得到结果：\r\n");
                pOS += VariableTableA.OutputValue();
            }
        }
        public void SimplifyEquations(string pOS)
        {
            List<bool> vecHasSolved = new List<bool>(Equations.Count);
            //foreach (var pExpr in Equations)
            for (int i = 0; i < Equations.Count(); ++i)
            {
                if (vecHasSolved[i] == false)
                {
                    TExpressionTree pExpr = Equations[i];

                    //代入
                    pExpr.Subs(VariableTableSolved.VariableTable, VariableTableSolved.VariableValue);

                    if (pExpr.CheckOnlyOneVar() != null)
                    {
                        string var="";
                        double @value=0.0;
                        pExpr.Solve();//lvwan20220703
                        VariableTableSolved.VariableTable.Add(var); //为共享单位，不负责析构变量
                        VariableTableSolved.VariableValue.Add(@value);

                        //VariableTableUnsolved.DeleteByAddress(var);//清除已解出变量

                        vecHasSolved[i] = true;
                        i = -1; //重回起点
                    }
                }
            }

            //清除已解出变量
            foreach (var pVar in VariableTableSolved.VariableTable)
            {
                pOS+= VariableTableUnsolved.RemoveOne( pVar, true);
            }

            if (pOS != null)
                pOS = pOS + (">>Simplify:\r\n\r\n");

            //清理掉已解出方程
            for (int i = vecHasSolved.Count - 1; i >= 0; --i)
            {
                if (vecHasSolved[i] == true)
                {
                    if (pOS != null)
                    {
                        pOS = pOS + Equations[i].OutputStr();
                        pOS = pOS + ("\r\n");
                    }
                    Equations[i] = null;
                    var iter = Equations.GetEnumerator();//删除+i
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
                    //Equations.erase(iter);

                    var iter2 = EquationIsTemp.GetEnumerator() ;//删除+i
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
                    //EquationIsTemp.erase(iter2);
                }
            }

            VariableTable.SetValueByVarTable(VariableTableSolved);

            //
            if (VariableTableUnsolved.VariableTable.Count == 0)
                hasSolved = true; //false由AddEquation触发

            if (pOS != null)
            {
                pOS = pOS + ("\r\n解得：\r\n");
                pOS += VariableTableSolved.OutputValue();
                pOS = pOS + ("\r\n");
            }
        }
        public void DefineVariable(ref string pOS, string input_str, string input_num)
        {
            DefineVariable(ref pOS, input_str, input_num, false);
        }
        public void DefineVariable(ref string pOS, string input_str)
        {
            DefineVariable(ref pOS, input_str, "", false);
        }

        public void DefineVariable(ref string pOS, string input_str, string input_num, bool bIgnoreReDef)
        {
            pOS=VariableTable.Define( input_str, input_num, bIgnoreReDef);

            VariableTableUnsolved.VariableTable = VariableTable.VariableTable;
            VariableTableUnsolved.VariableValue = VariableTable.VariableValue;
        }
        public void DefineOneVariable(ref string pOS, string var, double value)
        {
            DefineOneVariable(ref pOS, var, value, false);
        }
        public void DefineOneVariable(ref string pOS, string var, double @value, bool bIgnoreReDef)
        {
            pOS= VariableTable.DefineOne(var, @value, bIgnoreReDef);

            VariableTableUnsolved.VariableTable = VariableTable.VariableTable;
            VariableTableUnsolved.VariableValue = VariableTable.VariableValue;

        }
        public void Subs(string pOS, string var, double @value)
        {
            if (string.IsNullOrEmpty(var))
                throw new TError(enumError.ERROR_EMPTY_INPUT, (""));

            //Table中存在
            var find1 = VariableTable.FindVariableTable(var) < VariableTable.VariableTable.Count();

            //已解出中存在
            var find2 = VariableTableSolved.FindVariableTable(var) < VariableTableSolved.VariableTable.Count();
            if (find1 && find2 == false)
            {
                VariableTableSolved.VariableTable.Add(var);
                VariableTableSolved.VariableValue.Add(@value);
            }
            //else
            //	throw new TError{ ERROR_UNDEFINED_VARIABLE, var };


            if (pOS != null)
            {
                pOS = pOS + (">>Subs: [") + var;
                pOS = pOS + ("] -> [");
                pOS = pOS + @value;
                pOS = pOS + ("]\r\n\r\n当前方程：\r\n");
            }
            foreach (var pExpr in Equations) //遍历方程
            {
                pExpr.LinkVariableTable(VariableTableUnsolved);

                //替换
                pExpr.Subs(var, @value);

                if (pOS != null)
                {
                    pOS = pOS + pExpr.OutputStr();
                    pOS = pOS + ("\r\n");
                }
            }

            if (pOS != null)
            {
                pOS = pOS + ("\r\n");
            }

            //剔除掉被替换掉的变量
            //VariableTableUnsolved.RemoveOne(pOS, var, true);
            pOS+=VariableTableUnsolved.RemoveOne(var, true);//可能有错
        }
        public void Subs(string pOS, ref List<string> subsVars, ref TVector subsValue)
        {
            if (subsVars.Count != subsValue.Count)
                throw new TError(enumError.ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT, "");

            for (int i = 0; i < subsVars.Count(); ++i)
            {
                Subs(pOS, subsVars[i], subsValue[i]);
            }
        }
        //代入

        //替换单一变量
        public void SubsV(string pOS, string VarStr, double Value)
        {
            SubsVar(pOS, ref EquationsV, ref VariableTable, VarStr, Value);
        }

        //替换单一变量
        public void SubsA(string pOS, string VarStr, double Value)
        {
            SubsVar(pOS, ref EquationsA, ref VariableTable, VarStr, Value);
        }
        public double GetValue(ref string var)
        {
            var it = VariableTable.FindVariableTable(var);
            if (it == VariableTable.VariableTable.Count())
                throw new TError(
                enumError.ERROR_UNDEFINED_VARIABLE, var);

            return VariableTable.VariableValue[it];
        }

        //Jacobian为符号矩阵，乘以Vector数值向量，得到符号方程向量。结果存入EquationsResult
        //Ax=b
        public void CalcJacobianMultiplyVector(ref List<TExpressionTree> EquationsResult, ref List<List<TExpressionTree>> Jacobian, ref TVector Vector)
        {
            TExpressionTree expr;
            List<List<TExpressionTree>> jacotemp = new List<List<TExpressionTree>>();
            foreach (var Line in Jacobian) //每行
            {
                expr = new TExpressionTree();
                List<TExpressionTree> treelisttemp = new List<TExpressionTree>();
                for (var iter = 0; iter < Line.Count(); ++iter) //每个expr
                {
                    //Jacobian每项乘以q'
                    var temp = Line[iter] * Vector[iter];
                    //(iter)->Simplify(false);
                    treelisttemp.Add(temp);
                    //加起来
                    expr = expr+temp;
                }

#if _DEBUG
			expr.OutputStr();
#endif
                //expr->Simplify(false);
#if _DEBUG
			expr.OutputStr();
#endif
                jacotemp.Add(treelisttemp);
                EquationsResult.Add(expr);
            }
            Jacobian = jacotemp;
        }

        public void BuildJacobian_inner(ref List<List<TExpressionTree>> JacobianResult, ref List<TExpressionTree> Equations, ref TVariableTable VariableTable)
        {
            //释放旧的雅可比
            ReleaseJacobian(JacobianResult);

            TExpressionTree temp;

            //构建雅可比矩阵
            JacobianResult = new List<List<TExpressionTree>>();//Equations.Count()
            for (int i = 0; i < Equations.Count(); i++)//遍历方程
            {
                //以未解出变量建立雅可比矩阵
                Equations[i].LinkVariableTable(VariableTable);
                List<TExpressionTree> treelisttemp = new List<TExpressionTree>();
                //Equations[i]->Simplify(false);
                for (int j = 0; j < VariableTable.VariableTable.Count(); j++)
                {
                    temp = new TExpressionTree();
                    temp = Equations[i];
                    TExpressionTree diffExpressionTree = new TExpressionTree();
                    diffExpressionTree=temp.Diff(VariableTable.VariableTable[j], 1);
                    diffExpressionTree.Simplify();
                    //JacobianResult[i].Add(temp);
                    treelisttemp.Add(temp);
                }
                JacobianResult.Add(treelisttemp);
            }
        }

        //利用变量表中的值计算，方程中不前缀负号，计算出值加负号
        void CalcPhiValue(string pOS, ref List<TExpressionTree> Equations, ref TVector PhiValue)
        {
            //PhiValue.clear();
            TExpressionTree temp;
            foreach (var PhiExpr in Equations)
            {
                temp = new TExpressionTree();
                temp = PhiExpr;
                try
                {
                    temp.Vpa();
                    PhiValue.Add(-temp.Value());//得到临时表达式值存入
                }
                catch (TError err)
		{
                if (pOS != null)
                {

                    pOS +=("ERROR:");

                    pOS+=temp.OutputStr();

                    pOS += ("\r\nPhi计算出错:");

                    pOS+=err. GetErrorInfo(err.id) + err.info;
                }
                    temp = null ;
                throw err;
            }
                // delete temp;
                temp = null;
            }
    }

}

}
