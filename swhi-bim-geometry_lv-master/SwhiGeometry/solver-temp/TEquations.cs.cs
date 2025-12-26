//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

public class TEquations
{
//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ReleaseTPEquations(TPEquations Equations)
	{
		foreach (var pEqua in Equations)
		{
			pEqua = null;
		}
		Equations.clear();
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void CalcJacobianMultiplyVector(TPEquations EquationsResult, TJacobian Jacobian, TVector Vector)
	{
		TExpressionTree expr;
		foreach (var Line in Jacobian) //ÿ��
		{
			expr = new TExpressionTree();
			for (var iter = Line.begin(); iter != Line.end(); ++iter) //ÿ��expr
			{
				//Jacobianÿ�����q'
				(**iter) * Vector[iter - Line.begin()];
				//(*iter)->Simplify(false);
    
				//������
				expr + (**iter);
			}
	#if DEBUG
			expr.OutputStr();
	#endif
			//expr->Simplify(false);
	#if DEBUG
			expr.OutputStr();
	#endif
			EquationsResult.push_back(expr);
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void MatrixMultiplyVector(TVector Result, TMatrix Matrix, TVector Vector)
	{
		Result.clear();
		double temp;
		foreach (var Row in Matrix)
		{
			temp = 0;
			for (var iter = Row.begin(); iter != Row.end(); ++iter)
			{
				temp += (*iter) * Vector[iter - Row.begin()];
			}
			Result.push_back(temp);
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void CalcEquationsARight(Ostream pOS, TVector Right)
	{
		//����Jacobian����
		TJacobian JacobianTemp = new TJacobian();
		CopyJacobian(JacobianTemp, Jacobian);
	#if DEBUG
		OutputJacobian(pOS, JacobianTemp);
	#endif
    
		//Jacobian*q' ����q'
		TPEquations EquationsTemp = new TPEquations();
		CalcJacobianMultiplyVector(EquationsTemp, JacobianTemp, VariableTableV.VariableValue);
	#if DEBUG
		OutputPhi(pOS, EquationsTemp);
	#endif
    
		//(Jacobian*q')q  ��q��
		BuildJacobian_inner(JacobianTemp, EquationsTemp, VariableTableA);
	#if DEBUG
		OutputJacobian(pOS, JacobianTemp);
	#endif
    
		//Vpa: (Jacobian*q')q
		TMatrix MatrixTemp = new TMatrix();
		CalcJacobianValue(pOS, MatrixTemp, JacobianTemp);
	#if DEBUG
		Output(pOS, MatrixTemp);
	#endif
    
		// *q'
		MatrixMultiplyVector(Right, MatrixTemp, VariableTableV.VariableValue);
	#if DEBUG
		Output(pOS, Right);
	#endif
    
		//-Phitt
		TVector Phitt = new TVector();
		CalcPhiValue(pOS, EquationsA, Phitt);
	#if DEBUG
		Output(pOS, Phitt);
	#endif
    
		//-Right-Phitt
		for (var & iter = Right.begin(); iter != Right.end(); ++iter)
		{
			(*iter) = -(*iter) + Phitt[iter - Right.begin()];
		}
    
	#if DEBUG
		Output(pOS, Right);
	#endif
    
		ReleaseJacobian(JacobianTemp);
		ReleaseTPEquations(EquationsTemp);
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void SubsVar(Ostream pOS, TPEquations Equations, TVariableTable LinkVariableTable, string VarStr, double Value)
	{
		var it = LinkVariableTable.FindVariableTable(VarStr);
	//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
		if (it != LinkVariableTable.VariableTable.end())
		{
			string Var;
	//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			Var = it;
			foreach (var pEquation in Equations)
			{
				pEquation.Subs(Var, Value, false);
			}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void CopyJacobian(TJacobian ResultJacobian, TJacobian OriginJacobian)
	{
		ReleaseJacobian(ResultJacobian);
    
		ResultJacobian.resize(OriginJacobian.size());
		TExpressionTree temp;
		for (size_t i = 0; i < OriginJacobian.size(); i++)
		{
			for (size_t j = 0; j < OriginJacobian[i].size(); j++)
			{
				temp = new TExpressionTree();
				temp = *OriginJacobian[i][j];
    
				ResultJacobian[i].push_back(temp);
			}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void ReleaseJacobian(TJacobian Jacobian)
	{
		for (size_t i = 0; i < Jacobian.size(); i++)
		{
			for (size_t j = 0; j < Jacobian[i].size(); j++)
			{
				Jacobian[i][j] = null;
			}
		}
    
		Jacobian.clear();
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void BuildJacobian_inner(TJacobian JacobianResult, TPEquations Equations, TVariableTable VariableTable)
	{
		//�ͷžɵ��ſɱ�
		ReleaseJacobian(JacobianResult);
    
		TExpressionTree temp;
    
		//�����ſɱȾ���
		JacobianResult.resize(Equations.size());
		for (size_t i = 0; i < Equations.size(); i++) //��������
		{
			//��δ������������ſɱȾ���
			Equations[i].LinkVariableTable(VariableTable);
    
			//Equations[i]->Simplify(false);
			for (size_t j = 0; j < VariableTable.VariableTable.Count; j++)
			{
				temp = new TExpressionTree();
				temp = *Equations[i];
	//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
	//ORIGINAL LINE: temp->Diff(VariableTable.VariableTable[j], 1, false);
				temp.Diff(new List(VariableTable.VariableTable[j]), 1, false);
				temp.Simplify(false);
				JacobianResult[i].push_back(temp);
			}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void OutputPhi(Ostream pOS, TPEquations Equations)
	{
		if (pOS != null)
		{
			pOS << "Phi(1x";
			pOS << Equations.size();
			pOS << ")=\r\n[";
			for (var iter = Equations.begin(); iter != Equations.end(); ++iter)
			{
				pOS << iter.OutputStr();
				if (iter != Equations.end() - 1)
				{
					pOS << ";\r\n";
				}
			}
			pOS << "]\r\n\r\n";
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void OutputJacobian(Ostream pOS, TJacobian Jacobian)
	{
		//�����
		if (pOS != null)
		{
			pOS << "Jacobian(";
			pOS << (Jacobian.size() > 0 ? Jacobian[0].size() : 1) << "x" << Jacobian.size();
			pOS << ")=\r\n[";
			for (size_t ii = 0; ii < Jacobian.size(); ii++)
			{
				for (size_t jj = 0; jj < Jacobian[ii].size(); jj++)
				{
					pOS << Jacobian[ii][jj].OutputStr();
					pOS << " ";
				}
				if (ii != Jacobian.size() - 1)
				{
					pOS << ";\r\n";
				}
			}
			pOS << "]\r\n\r\n";
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void Output(Ostream pOS, TMatrix m)
	{
		if (pOS != null)
		{
	//C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
	//		static sbyte temp[64];
			pOS << "[";
			for (size_t i = 0; i < m.size(); i++)
			{
				for (size_t j = 0; j < m[i].size(); j++)
				{
					_stprintf(Output_temp, "%f", m[i][j]);
					pOS << Output_temp;
					pOS << " ";
				}
				if (i != m.size() - 1)
				{
					pOS << ";\r\n";
				}
			}
			pOS << "]\r\n\r\n";
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void Output(Ostream pOS, TVector v)
	{
		if (pOS != null)
		{
	//C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
	//		static sbyte temp[64];
			pOS << "[";
			foreach (var value in v)
			{
				_stprintf(Output_temp, "%f", value);
				pOS << Output_temp;
				pOS << " ";
			}
			pOS << "]\r\n\r\n";
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void CalcJacobianValue(Ostream pOS, TMatrix JacobianValueResult, TJacobian Jacobian)
	{
		JacobianValueResult.clear();
		JacobianValueResult.resize(Jacobian.size());
		TExpressionTree temp;
		for (size_t i = 0; i < Jacobian.size(); i++)
		{
			foreach (var exprJacobian in Jacobian[i])
			{
				temp = new TExpressionTree();
				temp = *exprJacobian;
				try
				{
					temp.Vpa(false);
					JacobianValueResult[i].push_back(temp.Value(true)); //�õ���ʱ���ʽֵ�����ſɱ�
				}
				catch (TError err)
				{
					if (pOS != null)
					{
						pOS << "ERROR:";
						pOS << temp.OutputStr();
						pOS << "\r\nJacobian�������:";
						pOS << GetErrorInfo(err.id) + err.info;
					}
					if (temp != null)
					{
						temp.Dispose();
					}
					throw err;
				}
				if (temp != null)
				{
					temp.Dispose();
				}
			}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void CalcPhiValue(Ostream pOS, TPEquations Equations, TVector PhiValue)
	{
		PhiValue.clear();
		TExpressionTree temp;
		foreach (var PhiExpr in Equations)
		{
			temp = new TExpressionTree();
			temp = *PhiExpr;
			try
			{
				temp.Vpa(false);
				PhiValue.push_back(-temp.Value(true)); //�õ���ʱ���ʽֵ����
			}
			catch (TError err)
			{
				if (pOS != null)
				{
					pOS << "ERROR:";
					pOS << temp.OutputStr();
					pOS << "\r\nPhi�������:";
					pOS << GetErrorInfo(err.id) + err.info;
				}
				if (temp != null)
				{
					temp.Dispose();
				}
				throw err;
			}
			if (temp != null)
			{
				temp.Dispose();
			}
		}
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public int GetMaxAbsRowIndex(TMatrix A, int RowStart, int RowEnd, int Col)
	{
		double max = 0.0;
		int index = RowStart;
		for (int i = RowStart; i <= RowEnd; i++)
		{
			if (Math.Abs(A[i][Col]) > max)
			{
				max = Math.Abs(A[i][Col]);
				index = i;
			}
		}
		return index;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public void SwapRow(TMatrix A, TVector b, int i, int j)
	{
		if (i == j)
		{
			return;
		}
		TVector temp = new TVector(A[i].size());
		temp = A[i];
		A[i] = A[j];
		A[j] = temp;
    
		double n;
		n = b[i];
		b[i] = b[j];
		b[j] = n;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public enumError SolveLinear(TMatrix A, TVector x, TVector b)
	{
		var m = A.size(); //����
		var n = m; //����=δ֪������
    
		var RankA = m; //��ʼֵ
		var RankAb = m;
    
		if (x.size() != m)
		{
			x.resize(m); //���Է������
		}
    
		if (m != b.size()) //Jacobian����������Phi����
		{
			return enumError.ERROR_JACOBI_ROW_NOT_EQUAL_PHI_ROW;
		}
    
		if (m > 0)
		{
			if ((n = A[0].size()) != m) //���Ƿ���
			{
				if (m > n)
				{
					return enumError.ERROR_OVER_DETERMINED_EQUATIONS; //�����巽����
				}
				else //����������
				{
					x.resize(n);
				}
			}
		}
    
		List<decltype(m)> TrueRowNumber = new List<decltype(m)>(n);
    
		//����Ԫ��Ԫ��
		for (decltype(m) y = 0, x = 0; y < m && x < n; y++, x++)
		{
			//if (A[i].size() != m)
    
			//�ӵ�ǰ��(y)�����һ��(m-1)�У��ҳ�x������һ����y�н���
			SwapRow(A, b, y, GetMaxAbsRowIndex(A, y, m - 1, x));
    
			while (Math.Abs(A[y][x]) < epsilon) //�����ǰֵΪ0  xһֱ��������0
			{
				x++;
				if (x == n)
				{
					break;
				}
    
				//���������������
				SwapRow(A, b, y, GetMaxAbsRowIndex(A, y, m - 1, x));
			}
    
			if (x != n && x > y)
			{
				TrueRowNumber[y] = x; //���뷽��ʱ ��ǰ��Ӧ����x��
			}
    
			if (x == n) //����ȫΪ0
			{
				RankA = y;
				if (Math.Abs(b[y]) < epsilon)
				{
					RankAb = y;
				}
    
				if (RankA != RankAb) //���죬��ϵ��������������Ȳ����->�޽�
				{
					return enumError.ERROR_SINGULAR_MATRIX;
				}
				else
				{
					break; //����for���õ��ؽ�
				}
			}
    
			//���Խ��߻�Ϊ1
			double m_num = A[y][x];
			for (decltype(m) j = y; j < n; j++) //y�е�j��->��n��
			{
				A[y][j] /= m_num;
			}
			b[y] /= m_num;
    
			//ÿ�л�Ϊ0
			for (decltype(m) row = y + 1; row < m; row++) //��1��->���1��
			{
				if (Math.Abs(A[row][x]) < epsilon)
				{
					;
				}
				else
				{
					double mi = A[row][x];
					for (var col = x; col < n; col++) //row�е�x��->��n��
					{
						A[row][col] -= A[y][col] * mi;
					}
					b[row] -= b[y] * mi;
				}
			}
		}
    
		bool bIndeterminateEquation = false; //���ô˱�������Ϊ����m��=n��������ж��Ƿ�Ϊ����������
    
		//��Ϊ���������飬��ȱ��ȫ��0��������
		if (m != n)
		{
			A.resize(n); //A��Ϊn��
			for (var i = m; i < n; i++) //A��m�п�ʼÿ��n����
			{
				A[i].resize(n);
			}
			b.resize(n);
			m = n;
			bIndeterminateEquation = true;
    
			//����˳��
			for (int i = m - 1; i >= 0; i--)
			{
				if (TrueRowNumber[i] != 0)
				{
					SwapRow(A, b, i, TrueRowNumber[i]);
				}
			}
		}
    
		//���û��õ�x
		double sum_others = 0.0;
		for (int i = m - 1; i >= 0; i--) //���1��->��1��
		{
			sum_others = 0.0;
			for (decltype(m) j = i + 1; j < m; j++) //���� ���Ԫ�س�����֪x ����
			{
				sum_others += A[i][j] * x[j];
			}
			x[i] = b[i] - sum_others;
		}
    
		if (RankA < n && RankA == RankAb)
		{
			if (bIndeterminateEquation)
			{
				return enumError.ERROR_INDETERMINATE_EQUATION;
			}
			else
			{
				return enumError.ERROR_INFINITY_SOLUTIONS;
			}
		}
    
		return enumError.ERROR_NO;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public bool AllIs0(TVector V)
	{
		foreach (var value in V)
		{
			if (Math.Abs(value) >= epsilon)
			{
				return false;
			}
		}
		return true;
	}

//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
	public bool VectorAdd(TVector Va, TVector Vb)
	{
		if (Va.size() != Vb.size())
		{
			return false;
		}
		for (size_t i = 0; i < Va.size(); i++)
		{
			Va[i] += Vb[i];
		}
		return true;
	}
}