

using System;
using System.Collections.Generic;
namespace Lv.BIM.Solver
{

	public class TVariableTable 
	{
		public bool bShared; //如果是共享变量表则不delete元素
		public enumError eError;
		public List<string> VariableTable = new List<string>();
		public List<double> VariableValue = new List<double>();
		public int Count => VariableTable.Count;
		private bool isLegalName(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			if (s[0] == '_' || (s[0] >= 'A' && s[0] <= 'z'))
			{
				foreach (var c in s)
				{
					if (c == '_' || (c >= 'A' && c <= 'z') || (c >= '0' && c <= '9'))
					{
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}

		public TVariableTable()
		{
			bShared = false;
			eError = enumError.ERROR_NO;
		}
		/// <summary>
		/// 定义变量表
		/// </summary>
		/// <param name="input_str">变量字符串以空格分割"x y z"</param>
		/// <param name="input_num">变量初始值"0.52 0.56 8"</param>
		/// <param name="bIgnoreReDef">新定义的若与旧的重名可过滤掉重定义</param>
		public TVariableTable(string input_str, string input_num = "", bool bIgnoreReDef = false)
        {
			Define(input_str,input_num, bIgnoreReDef);

		}
		public string this[int idx] => VariableTable[idx];
		public int FindVariableTable(string varstr)
		{
			return VariableTable.FindIndex(c => c.Contains(varstr));
			//VariableTable.FindIndex(c=>c==varstr);
		}

		//新定义的若与旧的重名可过滤掉重定义
		public string  Define(string input_str, string input_num = "", bool bIgnoreReDef = false)
		{
			//切分str，new出每个新变量
			string pStr = "";
			List<string> tempVar = new List<string>(input_str.Split(' '));
			List<double> tempValue = new List<double>();
			
			if (string.IsNullOrEmpty(input_num))
			{
				tempValue = new List<double>(tempVar.Count);
			}
			else
			{
				tempValue = new List<double>(Array.ConvertAll<string, double>(input_num.Split(' '), s => double.Parse(s)));
			}

			if (tempVar.Count != tempValue.Count)
			{
					pStr+="变量名与初始值数量不对等。";
				eError = enumError.ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT;
				throw new TError(eError, "VAR:" + input_str + " VALUE:" + input_num);
			}

			for (int i = 0; i < tempVar.Count; ++i)
			{
				pStr += DefineOne(tempVar[i], tempValue[i], bIgnoreReDef);
			}
			return pStr;
		}

		//void TVariableTable::Remove(Ostream *pStr, const String vars, bool bIgnoreUnExisted)
		//{
		//	std::vector<String> temp = StrSliceToVector(vars);
		//	for (auto &var : temp)
		//	{
		//		RemoveOne(pStr,var,bIgnoreUnExisted);
		//	}
		//}
		public static TVariableTable operator -(TVariableTable left, TVariableTable right)
		{
			TVariableTable result = left.Copy();
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			if (null == right)
			{
				throw new Exception("空值错误");
			}
            if (left.Count != right.Count)
            {
				throw new Exception("变量数不相等");
			}
			List<double> tempVar = new List<double>(left.Count);
			for(int i=0;i< left.Count; i++)
            {
				tempVar.Add(left.VariableValue[i] - right.VariableValue[i]);

			}
			result.VariableValue=tempVar;
			return result;
		}
		public static TVariableTable operator +(TVariableTable left, TVariableTable right)
		{
			TVariableTable result = left.Copy();
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			if (null == right)
			{
				throw new Exception("空值错误");
			}
			if (left.Count != right.Count)
			{
				throw new Exception("变量数不相等");
			}
			List<double> tempVar = new List<double>(left.Count);
			for (int i = 0; i < left.Count; i++)
			{
				tempVar.Add(left.VariableValue[i] + right.VariableValue[i]);

			}
			result.VariableValue = tempVar;
			return result;
		}
		public static TVariableTable operator *(TVariableTable left, double right)
		{
			TVariableTable result = left.Copy();
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			if (null == right)
			{
				throw new Exception("空值错误");
			}
			
			List<double> tempVar = new List<double>(left.Count);
			for (int i = 0; i < left.Count; i++)
			{
				tempVar.Add(left.VariableValue[i]*right);

			}
			result.VariableValue = tempVar;
			return result;
		}
		public static TVariableTable operator /(TVariableTable left, double right)
		{
			TVariableTable result = left.Copy();
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			if (null == right)
			{
				throw new Exception("空值错误");
			}

			List<double> tempVar = new List<double>(left.Count);
			for (int i = 0; i < left.Count; i++)
			{
				tempVar.Add(left.VariableValue[i] / right);

			}
			result.VariableValue = tempVar;
			return result;
		}
		public string DefineOne(string @var, double value, bool bIgnoreReDef = false)
		{
			string pStr="";
			if (!isLegalName(@var))
			{
				eError = enumError.ERROR_INVALID_VARNAME;
				throw new TError(eError, @var);
			}

			var it = FindVariableTable(@var);
			if (it != -1) //判断是否已经存在
			{
				if (bIgnoreReDef)
				{
					VariableValue[it] = value;
					pStr = "已重新定义变量";
					return pStr;
				}
				else
				{
					eError = enumError.ERROR_VAR_HAS_BEEN_DEFINED;
					throw new TError(eError, @var);

				}
			}
			else
			{
				VariableTable.Add(@var);
				VariableValue.Add(value);
			}
			if (pStr != null)
			{
				pStr+=(">>Define: ");
				pStr+= var +=("(");
				pStr +=value +(") ");
				pStr += ("\r\n\r\n");
			}
			return pStr;
		}
		public string Output()
		{
			string result = "";
			if (result != null)
			{
				result += "已定义变量";
				result += VariableTable.Count;
				result += "个):";

				foreach (var Var in VariableTable)
				{
					result += " ";
					result += Var;
				}

				result += "\r\n";
			}
			return result;
		}
		/// <summary>
		/// 暂时尚未输出
		/// </summary>
		public string OutputValue()
		{
			string pStr = "";
			for (int i = 0; i < VariableTable.Count; ++i)
			{
				pStr += VariableTable[i];
				pStr += " = ";
				pStr += VariableValue[i];
				pStr += "\r\n";
			}
			pStr += "\r\n";
			Console.WriteLine(pStr);
			return pStr;
			
		}
		//void Remove(Ostream *pStr, const String vars, bool bIgnoreUnExisted = false);

		//连带value一起删
		public string RemoveOne(string @var, bool bIgnoreUnExisted = false)
		{
			string pStr = "";
			var it = FindVariableTable(@var);
			if (it == -1) //未定义
			{
				if (bIgnoreUnExisted)
				{
					pStr = "未找到需删除变量";
					return pStr;
				}
				else
				{
					eError = enumError.ERROR_UNDEFINED_VARIABLE;
					throw new TError(eError, @var);
				}
			}
			//int index = it - VariableTable.GetEnumerator();
			VariableTable.RemoveAt(it);
			VariableValue.RemoveAt(it);
			return "删除成功";
		}
		public double GetValueFromVarPoint(string VarStr)
		{
			var it = FindVariableTable(VarStr);
			if (it == -1)
			{
				eError = enumError.ERROR_UNDEFINED_VARIABLE;
				throw new TError(eError, VarStr);
			}
			return VariableValue[it];
		}
		public void SetValueFromVarStr(string VarStr, double value)
		{
			var it = FindVariableTable(VarStr);
			if (it == -1)
			{
				eError = enumError.ERROR_UNDEFINED_VARIABLE;
				throw new TError(eError, VarStr);
			}
			VariableValue[it] = value;
		}

		////不删掉原变量，仅仅加上VarTable的值
		public void SetValueByVarTable(TVariableTable VarTable)
		{
			for (int i = 0; i < VarTable.VariableTable.Count; ++i)
			{
				var it = FindVariableTable(VarTable.VariableTable[i]);
				if (it != -1)
				{
					VariableValue[it] = VarTable.VariableValue[i];
				}
			}
		}
		public double DifferenceBetween(TVariableTable aothervarTable)
        {
            if (Count != aothervarTable.Count)
            {
				throw new Exception("数据个数不等");
            }
			double re = 0;
			for(int i = 0; i < Count; ++i)
            {
				re += Math.Abs(VariableValue[i] - aothervarTable.VariableValue[i]);

			}
			return re;
        }
		public TVariableTable Copy()
        {
			List<string> strs= new List<string>();
			VariableTable.ForEach(a => strs.Add(a));
			List<double> vals = new List<double>();
			VariableValue.ForEach(a => vals.Add(a));
			TVariableTable newt= new TVariableTable();
			newt.VariableTable = strs;
			newt.VariableValue = vals;
			return newt;
        }
		public override string ToString()
        {
			string re = "";
			VariableValue.ForEach(a => re += "," + a.ToString("0.0000")) ;
			return re;
        }
	}

}



