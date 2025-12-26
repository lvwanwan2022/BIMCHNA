//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System.Collections.Generic;


public class TVariableTable : System.IDisposable
{
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
					;
				}
				else
				{
					return false;
				}
			}
		}
		return true;
	}
	public bool bShared; //如果是共享变量表则不delete元素
	public enumError eError;
	public List<string> VariableTable = new List<string>();
	public List<double> VariableValue = new List<double>();
	public TVariableTable()
	{
		bShared = false;
		eError = enumError.ERROR_NO;
	}
	public void Dispose()
	{
	}
	public List<string>.Enumerator FindVariableTable(string varstr)
	{
		return std::find(VariableTable.GetEnumerator(), VariableTable.end(), varstr);
	}

	//�¶��������ɵ������ɹ��˵��ض���
	public void Define(Ostream pStr, string input_str, string input_num = "", bool bIgnoreReDef = false)
	{
		//�з�str��new��ÿ���±���

		List<string> tempVar = StrSliceToVector(input_str);
		List<double> tempValue = new List<double>();
		if (string.IsNullOrEmpty(input_num))
		{
			tempValue = new List<double>(tempVar.Count);
		}
		else
		{
			tempValue = StrSliceToDoubleVector(input_num);
		}

		if (tempVar.Count != tempValue.Count)
		{
			if (pStr != null)
			{
				pStr << "���������ʼֵ�������Եȡ�";
			}
			eError = enumError.ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT;
			throw new TError({eError, "VAR:" + input_str + " VALUE:" + input_num});
			return;
		}

		for (int i = 0; i < tempVar.Count; ++i)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: DefineOne(pStr, tempVar[i], tempValue[i], bIgnoreReDef);
			DefineOne(pStr, new List(tempVar[i]), new List(tempValue[i]), bIgnoreReDef);
		}

	}

	//void TVariableTable::Remove(Ostream *pStr, const String vars, bool bIgnoreUnExisted)
	//{
	//	std::vector<String> temp = StrSliceToVector(vars);
	//	for (auto &var : temp)
	//	{
	//		RemoveOne(pStr,var,bIgnoreUnExisted);
	//	}
	//}

	public void DefineOne(Ostream pStr, string @var, double value, bool bIgnoreReDef = false)
	{
		if (!isLegalName(@var))
		{
			eError = enumError.ERROR_INVALID_VARNAME;
			throw new TError({eError, @var});
			return;
		}

		var it = FindVariableTable(@var);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
		if (it != VariableTable.end()) //�Ѷ���
		{
			if (bIgnoreReDef)
			{
				VariableValue[it - VariableTable.GetEnumerator()] = value;
				return;
			}
			else
			{
				eError = enumError.ERROR_VAR_HAS_BEEN_DEFINED;
				throw new TError({eError, @var});
				return;
			}
		}
		else
		{
			VariableTable.Add(@var);
			VariableValue.Add(value);
		}

		if (pStr != null)
		{
			pStr << ">>Define: ";

			pStr << @var << "(";
			pStr << value << ") ";
			pStr << "\r\n\r\n";
		}

	}
	public void Output(Ostream pStr)
	{
		if (pStr != null)
		{
			pStr << "�Ѷ������(";
			pStr << VariableTable.Count;
			pStr << "��):";

			foreach (var Var in VariableTable)
			{
				pStr << " ";
				pStr << Var;
			}

			pStr << "\r\n";
		}
	}
	public void OutputValue(Ostream pStr)
	{
		if (pStr != null)
		{
			for (size_t i = 0; i < VariableTable.Count; ++i)
			{
				pStr << VariableTable[i];
				pStr << " = ";
				pStr << VariableValue[i];
				pStr << "\r\n";
			}
			pStr << "\r\n";
		}
	}
	//void Remove(Ostream *pStr, const String vars, bool bIgnoreUnExisted = false);

	//����valueһ��ɾ
	public void RemoveOne(Ostream pStr, string @var, bool bIgnoreUnExisted = false)
	{
		var it = FindVariableTable(@var);
		if (it == VariableTable.end()) //δ����
		{
			if (bIgnoreUnExisted)
			{
				return;
			}
			else
			{
				eError = enumError.ERROR_UNDEFINED_VARIABLE;
				throw new TError({eError, @var});
				return;
			}
		}
		int index = it - VariableTable.GetEnumerator();
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
		VariableTable.erase(it);
		VariableValue.RemoveAt(index);
	}
	public double GetValueFromVarPoint(string VarStr)
	{
		var it = FindVariableTable(VarStr);
		if (it == VariableTable.end())
		{
			eError = enumError.ERROR_UNDEFINED_VARIABLE;
			throw new TError({eError, VarStr});
			return 0.0;
		}
		return VariableValue[it - VariableTable.GetEnumerator()];
	}
	public void SetValueFromVarStr(string VarStr, double value)
	{
		var it = FindVariableTable(VarStr);
		if (it == VariableTable.end())
		{
			eError = enumError.ERROR_UNDEFINED_VARIABLE;
			throw new TError({eError, VarStr});
			return;
		}
		VariableValue[it - VariableTable.GetEnumerator()] = value;
	}

	//��ɾ��ԭ��������������VarTable��ֵ
	public void SetValueByVarTable(TVariableTable VarTable)
	{
		for (int i = 0; i < VarTable.VariableTable.Count; ++i)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: auto it = FindVariableTable(VarTable.VariableTable[i]);
			var it = FindVariableTable(new List(VarTable.VariableTable[i]));
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			if (it != VariableTable.end())
			{
				VariableValue[it - VariableTable.GetEnumerator()] = VarTable.VariableValue[i];
			}
		}
	}
}





