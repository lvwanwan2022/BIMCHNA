//建立变量表
string input_str = "x y";
string input_num = "0.1 0.1";
TVariableTable VariableTable = new TVariableTable();
VariableTable.Define(input_str, input_num);

//建立表达式树
//常规表达式cos(x)^5+3*x^2-3+x^3*4
//"tan(2*x)^2+3*x^3-3+x^3*4"
//"ln(2*x)^2+3*x^3-3+x^3*4",log10(2*x)^2,exp(2*x)
//"x^4+arctan(x^2)-3"
//arcsin(2 * x)^2,arctan(2*x),arccos(x)
// sin,cos,tan,arcsin,arccos,arctan,sqrt,ln,log10,exp
//实例：x^4+arctan(x^2)-3，求解后：tan(0+3-x^4)^(1/2)求导后:x^3*4*1+1/(1+x^2*x^2)*x*2*1,导数求val(0.5)=1.4411764
TExpressionTree temp = new TExpressionTree();
temp.LinkVariableTable(VariableTable);
temp.SetExpression("x^2+arcsin(x)-3");
temp.Simplify(true);
//求值
TExpressionTree temp = exp.Subs("x", 2);//先替换
double val = temp.Value();//再求值
MessageBox.Show(temp.OutputStr() + "=" + val.ToString());

//求导数
TExpressionTree tempdiff = new TExpressionTree();
tempdiff = temp.Diff("x", 1);
tempdiff.Simplify(true);
MessageBox.Show(tempdiff.OutputStr());//节点树生成的字符串
MessageBox.Show(tempdiff.ExpressionString);//OutputStr()字符串经过简单替换

exp.SetExpression("cos(x)*sin(x)+x^2+log10(x)-2*x+0+0");
//求导后
(+, (+, (-, (+, (+, (*, (-, (sin, x)), (sin, x)), (*, (cos, x), (cos, x))), (+, (*, (^, x, 1), (*, 2, 1)), (/, 1, (*, x, (ln, 10))))), (*, 2, 1)), 0), 0)
(-, (+, (+, (*, (-, (sin, x)), (sin, x)), (*, (cos, x), (cos, x))), (+, (*, 2, (^, x, 1)), (/, 1, (*, x, 2.302585092994046)))), 2)
cos(x) + ((sin(5) + 2 * cos(ln(x))) * 3 + ln(10.235)) * 12 + exp(y)
( +,( cos,x ),( *,(( +,( *,(( +,( sin,5 ),( *,2,( cos,( ln,x ))))),3 ),( ln,10.235 ))),12 )) +( exp,y )

//求解方程组
string input_str = "x y";
string input_num = "0.1 0.1";
TVariableTable VariableTable = new TVariableTable();
VariableTable.Define(input_str, input_num);
TExpressionTree exp1 = new TExpressionTree();
exp1.LinkVariableTable(VariableTable);
//exp1.SetExpression("sin(x)^2+x*y+y-3");
exp1.SetExpression("3*x+4*y-5");
TExpressionTree exp2 = new TExpressionTree();
exp2.LinkVariableTable(VariableTable);
//exp2.SetExpression("4*x+y^2");//结果为-1.7666,-2.6583
exp2.SetExpression("x+y*2-10");//结果为-15,12.5
List<TExpressionTree> tes = new List<TExpressionTree>();
tes.Add(exp1);
tes.Add(exp2);
LEquations le = new LEquations(tes, VariableTable);
List<double> vals = le.Solve(VariableTable, 100, 0.01);
MessageBox.Show(vals[0].ToString("0.0000") + "," + vals[1].ToString("0.0000"));



//绘图
double[] xs = DataGen.Consecutive(50);
List<double> ystemp = new List<double>();
foreach (double xa in xs)
{
    TExpressionTree temp = exp.Subs("x", xa);
    double val = temp.Value();
    ystemp.Add(val);
}
double[] ys = ystemp.ToArray();
WpfPlot1.Plot.AddScatter(xs, ys);
WpfPlot1.Refresh();