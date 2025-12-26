//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Collections.Generic;


//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define String wstring
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ostream wostream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ifstream wifstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ofstream wofstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Stringstream wstringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Istringstream wistringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ostringstream wostringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define COUT wcout
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define To_string to_wstring
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define String string
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ostream ostream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ifstream ifstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ofstream ofstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Stringstream stringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Istringstream istringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define Ostringstream ostringstream
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define COUT cout
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define To_string to_string

public class TEquations : System.IDisposable
{

	//enumError eError;
	private List<bool> EquationIsTemp = new List<bool>();

	private List<List<TExpressionTree >> Jacobian = new List<List<TExpressionTree >>();
	private List<TExpressionTree > Equations = new List<TExpressionTree >();
	private List<TExpressionTree > EquationsV = new List<TExpressionTree >();
	private List<TExpressionTree > EquationsA = new List<TExpressionTree >();
	private TVariableTable VariableTableSolved = new TVariableTable(); //�ѽ��������

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void CalcPhiValue(Ostream pOS, ClassicVector<TExpressionTree > Equations, ClassicVector<double> PhiValue);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void MatrixMultiplyVector(ClassicVector<double> Result, ClassicVector<ClassicVector<double>> Matrix, ClassicVector<double> Vector);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void CalcJacobianValue(Ostream pOS, ClassicVector<ClassicVector<double>> JacobianValueResult, ClassicVector<ClassicVector<TExpressionTree >> Jacobian);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	int GetMaxAbsRowIndex(ClassicVector<ClassicVector<double>> A, int RowStart, int RowEnd, int Col);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void SwapRow(ClassicVector<ClassicVector<double>> A, ClassicVector<double> b, int i, int j);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	bool AllIs0(ClassicVector<double> V);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	bool VectorAdd(ClassicVector<double> Va, ClassicVector<double> Vb);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void Output(Ostream pOS, ClassicVector<ClassicVector<double>> m);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void Output(Ostream pOS, ClassicVector<double> v);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ReleaseTPEquations(ClassicVector<TExpressionTree > Equations);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void ReleaseJacobian(ClassicVector<ClassicVector<TExpressionTree >> Jacobian);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void SubsVar(Ostream pOS, ClassicVector<TExpressionTree > Equations, TVariableTable LinkVariableTable, string VarStr, double Value);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void BuildJacobian_inner(ClassicVector<ClassicVector<TExpressionTree >> JacobianResult, ClassicVector<TExpressionTree > Equations, TVariableTable VariableTable);
	public TEquations()
	{
		VariableTableUnsolved.bShared = true;
		VariableTableSolved.bShared = true;
	}
	public void Dispose()
	{
		//�ͷŷ�����
		ReleaseTPEquations(Equations);
		ReleaseTPEquations(EquationsV);
		ReleaseTPEquations(EquationsA);

		//�ͷ��ſɱ�
		ReleaseJacobian(Jacobian);
	}

	public double epsilon = 1e-12;
	public uint max_step = 20;
	public bool hasSolved;
	public TVariableTable VariableTable = new TVariableTable(); //�ܱ�����
	public TVariableTable VariableTableUnsolved = new TVariableTable();
	public TVariableTable VariableTableV = new TVariableTable(); //�ٶ��ܱ�����
	public TVariableTable VariableTableA = new TVariableTable(); //�ٶ��ܱ�����

	public TExpressionTree GetLastExpressionTree()
	{
		return Equations[Equations.Count - 1];
	}

	//������̶�t�󵼣��õ��ٶȷ������ұ�
	public void BuildEquationsV(Ostream pOS)
	{
		bool bOutput = pOS == null ? false : true;

		if (pOS != null)
		{
			pOS << ">>BuildEquationsV: \r\n";
			pOS << "��ǰ���̣�\r\n";
		}

		TExpressionTree pEquatemp;
		foreach (var pEqua in Equations)
		{
			pEquatemp = new TExpressionTree();
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: *pEquatemp = *pEqua;
			pEquatemp.CopyFrom(pEqua);

			pEquatemp.Diff("t", 1, bOutput);
			pEquatemp.Simplify(bOutput);

			EquationsV.Add(pEquatemp);

			if (pOS != null)
			{
				pOS << pEquatemp.OutputStr();
				pOS << "\r\n";
			}
		}
	}

	//������̶�t�󵼣��õ��ٶȷ������ұ�
	public void BuildEquationsA_Phitt(Ostream pOS)
	{
		bool bOutput = pOS == null ? false : true;

		if (pOS != null)
		{
			pOS << ">>Build Equations A: \r\n";
			pOS << "��ǰ���̣�\r\n";
		}

		TExpressionTree pEquatemp;
		foreach (var pEqua in EquationsV)
		{
			pEquatemp = new TExpressionTree();
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: *pEquatemp = *pEqua;
			pEquatemp.CopyFrom(pEqua);

			pEquatemp.Diff("t", 1, bOutput);
			pEquatemp.Simplify(bOutput);

			EquationsA.Add(pEquatemp);

			if (pOS != null)
			{
				pOS << pEquatemp.OutputStr();
				pOS << "\r\n";
			}
		}
	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void CalcEquationsARight(Ostream pOS, ClassicVector<double> Right);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void OutputPhi(Ostream pOS, ClassicVector<TExpressionTree > Equations);

	//����VariableTableUnsolved
	public void BuildJacobian(Ostream pOS)
	{
		BuildJacobian_inner(Jacobian, Equations, VariableTableUnsolved);

		//�����
		if (pOS != null)
		{
			pOS << ">>Build Jacobian:\r\n\r\n";

			OutputPhi(pOS, Equations);

			OutputJacobian(pOS, Jacobian);
		}
	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void CopyJacobian(ClassicVector<ClassicVector<TExpressionTree >> Result, ClassicVector<ClassicVector<TExpressionTree >> Origin);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void OutputJacobian(Ostream pOS, ClassicVector<ClassicVector<TExpressionTree >> Jacobian);

	//��δ���������ֵ���ٶȱ�����
	public void BuildVariableTableV(Ostream pOS)
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: VariableTableV = VariableTableUnsolved;
		VariableTableV.CopyFrom(VariableTableUnsolved);
		VariableTableV.bShared = true;
	}

	//��δ���������ֵ���ٶȱ�����
	public void BuildVariableTableA(Ostream pOS)
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: VariableTableA = VariableTableUnsolved;
		VariableTableA.CopyFrom(VariableTableUnsolved);
		VariableTableA.bShared = true;
	}
	public size_t GetEquationsCount()
	{
		return Equations.Count;
	}
	public void AddEquation(Ostream pOS, string szInput, bool istemp)
	{
		TExpressionTree temp = new TExpressionTree();
		temp.LinkVariableTable(VariableTable);
		temp.Read(szInput, false);

		temp.Simplify(false);

		//���뷽����
		Equations.Add(temp);
		EquationIsTemp.Add(istemp);

		hasSolved = false;

		if (pOS != null)
		{
			pOS << ">>Add:\r\n";
			pOS << temp.OutputStr();
			pOS << "\r\n\r\n";
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
				List<bool>.Enumerator iter1 = EquationIsTemp.GetEnumerator() + i;
				List<TExpressionTree >.Enumerator iter2 = Equations.GetEnumerator() + i;
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
				EquationIsTemp.erase(iter1);

				if (Equations[i] != null)
				{
					Equations[i].Dispose();
				}
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
				Equations.erase(iter2);
			}
		}

		ReleaseTPEquations(EquationsV);
		ReleaseTPEquations(EquationsA);

		//VariableTableUnsolved = VariableTable;
		//VariableTableUnsolved.bShared = true;

	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	enumError SolveLinear(ClassicVector<ClassicVector<double>> A, ClassicVector<double> x, ClassicVector<double> b); //�����Է����� ϵ��A��δ֪��x

	//ţ��-����ɭ�������
	public void SolveEquations(Ostream pOS)
	{
		if (hasSolved == false)
		{
			if (pOS != null)
			{
				pOS << ">>SolveEquations:\r\n";
				pOS << "��ǰδ֪����\r\n";
			}
			VariableTableUnsolved.Output(pOS); //�����ǰ����

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
					pOS << "q(";
					pOS << (int)n;
					pOS << ")=\r\n";
					Output(pOS, Q);
					pOS << "\r\n";
				}

				try
				{
					CalcJacobianValue(pOS, JacobianValue, Jacobian);
				}
				catch (TError err)
				{
					if (pOS != null)
					{
						pOS << "�޷����㡣\r\n";
						pOS << GetErrorInfo(err.id) << err.info << "\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				}

				if (pOS != null)
				{
					pOS << "Jacobian(";
					pOS << (int)n;
					pOS << ")=\r\n";
					Output(pOS, JacobianValue);
					pOS << "\r\n";
				}

				try
				{
					CalcPhiValue(pOS, Equations, PhiValue);
				}
				catch (TError err)
				{
					if (pOS != null)
					{
						pOS << "�޷����㡣\r\n";
						pOS << GetErrorInfo(err.id) << err.info << "\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				}
				if (pOS != null)
				{
					pOS << "Phi(";
					pOS << (int)n;
					pOS << ")=\r\n";
					Output(pOS, PhiValue);
					pOS << "\r\n";
				}

				switch (SolveLinear(JacobianValue, DeltaQ, PhiValue))
				{
				case enumError.ERROR_SINGULAR_MATRIX:
					//��������
					if (pOS != null)
					{
						pOS << "Jacobian�����������޽⣨��ֵ�����ʻ��ߴ���ì�ܷ��̣���\r\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				case enumError.ERROR_INDETERMINATE_EQUATION:
					if (pOS != null)
					{
						pOS << "���������顣����һ���ؽ⡣\r\n";
					}
					break;
				case enumError.ERROR_JACOBI_ROW_NOT_EQUAL_PHI_ROW:
					if (pOS != null)
					{
						pOS << "Jacobian������Phi�����������ȣ���������\r\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				case enumError.ERROR_INFINITY_SOLUTIONS:
					if (pOS != null)
					{
						pOS << "Jacobian�������죬���������⣨���ڵȼ۷��̣�������һ���ؽ⡣\r\n";
					}
					break;
				case enumError.ERROR_OVER_DETERMINED_EQUATIONS:
					if (pOS != null)
					{
						pOS << "ì�ܷ����飬�޷���⡣\r\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				}

				if (pOS != null) //���DeltaQ
				{
					pOS << "��q(";
					pOS << (int)n;
					pOS << ")=\r\n";
					Output(pOS, DeltaQ);
					pOS << "\r\n\r\n";
				}

				VectorAdd(Q, DeltaQ);

				if (AllIs0(DeltaQ))
				{
					break;
				}

				if (n > max_step - 1)
				{
					if (pOS != null)
					{
						pOS << "����" << (int)max_step << "����δ������\r\n";
					}
					VariableTableUnsolved.VariableValue = VariableValueBackup;
					return;
				}
				n++;
			}
			//�˴��ѽ��

			VariableTable.SetValueByVarTable(VariableTableUnsolved);

			hasSolved = true;
		}

		if (pOS != null)
		{
			pOS << "\r\n�õ������\r\n";
		}
		VariableTableUnsolved.OutputValue(pOS);
	}
	public void SolveEquationsV(Ostream pOS)
	{
		TMatrix JacobianV = new TMatrix();
		TVector Phi = new TVector();
		TVector dQ = VariableTableV.VariableValue;
		CalcPhiValue(pOS, EquationsV, Phi);
		CalcJacobianValue(pOS, JacobianV, Jacobian);
		SolveLinear(JacobianV, dQ, Phi);

		if (pOS != null)
		{
			pOS << ">>SolveEquationsV:\r\n";
			if (pOS != null)
			{
				pOS << "\r\n�õ������\r\n";
			}
			VariableTableV.OutputValue(pOS);
		}
	}
	public void SolveEquationsA(Ostream pOS)
	{
		TMatrix JacobianA = new TMatrix();
		TVector Phi = new TVector();
		TVector ddQ = VariableTableA.VariableValue;
		CalcJacobianValue(pOS, JacobianA, Jacobian); //JacobianA��Jacobian���
		CalcEquationsARight(pOS, Phi);
		SolveLinear(JacobianA, ddQ, Phi);

		if (pOS != null)
		{
			pOS << ">>SolveEquationsA:\r\n";
			if (pOS != null)
			{
				pOS << "\r\n�õ������\r\n";
			}
			VariableTableA.OutputValue(pOS);
		}
	}
	public void SimplifyEquations(Ostream pOS)
	{
		List<bool> vecHasSolved = new List<bool>(Equations.Count, false);
		//for (auto pExpr : Equations)
		for (size_t i = 0; i < Equations.Count; ++i)
		{
			if (vecHasSolved[i] == false)
			{
				TExpressionTree pExpr = Equations[i];

				//����
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: pExpr->Subs(VariableTableSolved.VariableTable, VariableTableSolved.VariableValue, pOS != null);
				pExpr.Subs(new List(VariableTableSolved.VariableTable), new List(VariableTableSolved.VariableValue), pOS != null);

				if (pExpr.CheckOnlyOneVar())
				{
					string @var;
					double value;
					pExpr.Solve(@var, ref value);
					VariableTableSolved.VariableTable.Add(@var); //Ϊ�����λ����������������
					VariableTableSolved.VariableValue.Add(value);

					//VariableTableUnsolved.DeleteByAddress(var);//����ѽ������

					vecHasSolved[i] = true;
					i = -1; //�ػ����
				}
			}
		}

		//����ѽ������
		foreach (var pVar in VariableTableSolved.VariableTable)
		{
			VariableTableUnsolved.RemoveOne(pOS, pVar, true);
		}

		if (pOS != null)
		{
			pOS << ">>Simplify:\r\n\r\n";
		}

		//������ѽ������
		for (int i = vecHasSolved.Count - 1; i >= 0; --i)
		{
			if (vecHasSolved[i] == true)
			{
				if (pOS != null)
				{
					pOS << Equations[i].OutputStr();
					pOS << "\r\n";
				}
				if (Equations[i] != null)
				{
					Equations[i].Dispose();
				}
				var iter = Equations.GetEnumerator() + i;
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
				Equations.erase(iter);

				var iter2 = EquationIsTemp.GetEnumerator() + i;
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
				EquationIsTemp.erase(iter2);
			}
		}

		VariableTable.SetValueByVarTable(VariableTableSolved);

		//
		if (VariableTableUnsolved.VariableTable.Count == 0)
		{
			hasSolved = true; //false��AddEquation����
		}

		if (pOS != null)
		{
			pOS << "\r\n��ã�\r\n";
			VariableTableSolved.OutputValue(pOS);
			pOS << "\r\n";
		}
	}
	public void DefineVariable(Ostream pOS, string input_str, string input_num = "", bool bIgnoreReDef = false)
	{
		VariableTable.Define(pOS, input_str, input_num, bIgnoreReDef);

		VariableTableUnsolved.VariableTable = new List<string>(VariableTable.VariableTable);
		VariableTableUnsolved.VariableValue = new List<double>(VariableTable.VariableValue);
	}
	public void DefineOneVariable(Ostream pOS, string @var, double value, bool bIgnoreReDef = false)
	{
		VariableTable.DefineOne(pOS, @var, value, bIgnoreReDef);

		VariableTableUnsolved.VariableTable = new List<string>(VariableTable.VariableTable);
		VariableTableUnsolved.VariableValue = new List<double>(VariableTable.VariableValue);

	}
	public void Subs(Ostream pOS, string @var, double value)
	{
		if (string.IsNullOrEmpty(@var))
		{
			throw new TError({enumError.ERROR_EMPTY_INPUT, ""});
		}

		//Table�д���
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
		var find1 = VariableTable.FindVariableTable(@var) != VariableTable.VariableTable.end();

		//�ѽ���д���
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
		var find2 = VariableTableSolved.FindVariableTable(@var) != VariableTableSolved.VariableTable.end();
		if (find1 && find2 == false)
		{
			VariableTableSolved.VariableTable.Add(@var);
			VariableTableSolved.VariableValue.Add(value);
		}
		//else
		//	throw TError{ ERROR_UNDEFINED_VARIABLE, var };


		if (pOS != null)
		{
			pOS << ">>Subs: [" << @var;
			pOS << "] -> [";
			pOS << value;
			pOS << "]\r\n\r\n��ǰ���̣�\r\n";
		}
		foreach (var pExpr in Equations) //��������
		{
			pExpr.LinkVariableTable(VariableTableUnsolved);

			//�滻
			pExpr.Subs(@var, value, false);

			if (pOS != null)
			{
				pOS << pExpr.OutputStr();
				pOS << "\r\n";
			}
		}

		if (pOS != null)
		{
			pOS << "\r\n";
		}

		//�޳�����滻��ı���
		VariableTableUnsolved.RemoveOne(pOS, @var, true);
	}

	//�ѽ����������� δ����������޳�
	public void Subs(Ostream pOS, string subsVars, string subsValues)
	{
		List<string> tempVars = StrSliceToVector(subsVars);
		List<double> tempValues = StrSliceToDoubleVector(subsValues);

		Subs(pOS, tempVars, tempValues);
	}
	public void Subs(Ostream pOS, List<string> subsVars, List<double> subsValue)
	{
		if (subsVars.Count != subsValue.Count)
		{
			throw new TError({enumError.ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT,""});
		}

		for (int i = 0; i < subsVars.Count; ++i)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: Subs(pOS, subsVars[i], subsValue[i]);
			Subs(pOS, new List(subsVars[i]), new List(subsValue[i]));
		}
	}

	//�滻��һ����
	public void SubsV(Ostream pOS, string VarStr, double Value)
	{
		SubsVar(pOS, EquationsV, VariableTable, VarStr, Value);
	}

	//�滻��һ����
	public void SubsA(Ostream pOS, string VarStr, double Value)
	{
		SubsVar(pOS, EquationsA, VariableTable, VarStr, Value);
	}
	public double GetValue(string @var)
	{
		var it = VariableTable.FindVariableTable(@var);
		if (it == VariableTable.VariableTable.end())
		{
			throw new TError({enumError.ERROR_UNDEFINED_VARIABLE, @var});
		}

		return VariableTable.VariableValue[it - VariableTable.VariableTable.GetEnumerator()];
	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void CalcJacobianMultiplyVector(ClassicVector<TExpressionTree > EquationsResult, ClassicVector<ClassicVector<TExpressionTree >> Jacobian, ClassicVector<double> Vector);

}


//JacobianΪ���ž��󣬳���Vector��ֵ�������õ����ŷ����������������EquationsResult
//Ax=b



//Ӧ�ڽ��λ�á��ٶȷ��̺����


//Copy Jacobian





//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private string Output_temp = new string(new char[64]);

//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private string Output_temp = new string(new char[64]);


//���ñ������е�ֵ�����ſɱ�

//���ñ������е�ֵ���㣬�����в�ǰ׺���ţ������ֵ�Ӹ���




