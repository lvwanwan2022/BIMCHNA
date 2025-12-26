//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using DialogAnimation;
using System;
using System.Collections.Generic;


//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define RAD2DEG(a) (a)/M_PI*180.0
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define DEG2RAD(a) (a)/180.0*M_PI
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define IsZero(a,precision) (abs(a)<(precision))
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define ISEQUAL(a,b,precision) (abs((a)-(b))<=(precision))
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define CanMod(a,b,precision) (IsZero((a)/(b)-(long)((a)/(b)),(precision)))
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define MakeIn2Pi(a) ((abs(a)>2*M_PI)?(((a)/M_PI/2-(long)((a)/M_PI/2))*2*M_PI):(a))


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



public enum enumConditionType
{
	CONDITION_COINCIDE,
	CONDITION_FRAMEPOINT
}
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class TEquations;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class TListBoxItem;
public class TSolver: TTool
{
	private Ostream pOS;
	private IntPtr hwndOutput; //������ھ��
	private List<string> vecStrDriver = new List<string>(); //��������
	private TEquations[] Equations; //Լ������
	//String subsVar, subsValue;
	private List<string> subsVar = new List<string>(); //���ܵ��������ֵ
	private List<double> subsValue = new List<double>();
	private List<int> idOrder = new List<int>();
	private class TCondition
	{
		public enumConditionType eType;
		public int i;
		public int j;
		public DPOINT SiP = new DPOINT();
		public DPOINT SjP = new DPOINT();
	}

	private double dRelativeAngle;

	private int GetIdFromVariableStr(string varname)
	{
		int i = varname.IndexOfAny((Convert.ToString("0123456789")).ToCharArray());
		string temp = varname.Substring(i);
		return Convert.ToInt32(temp);
	}
	private void Output(ref string szFormat, params object[] LegacyParamArray)
	{
		if (szFormat == null || pOS == null)
		{
			return;
		}
		string szBuffer = new string(new char[1024]);
//		va_list pArgList;
		int ParamCount = -1;
//		va_start(pArgList, szFormat);
//C++ TO C# CONVERTER WARNING: This 'sizeof' ratio was replaced with a direct reference to the array length:
//ORIGINAL LINE: _vsntprintf(szBuffer, sizeof(szBuffer) / sizeof(sbyte), szFormat, pArgList);
		_vsntprintf(szBuffer, szBuffer.Length, szFormat, pArgList);
//		va_end(pArgList);

		pOS << szBuffer;
	}
	private void Outputln(string szFormat, params object[] LegacyParamArray)
	{
		if (pOS != null)
		{
			if (szFormat != null && _tcslen(szFormat) > 0)
			{
				string szBuffer = new string(new char[_tcslen(szFormat) + 1024 - 1]);
//				va_list pArgList;
				int ParamCount = -1;
//				va_start(pArgList, szFormat);
				_vsntprintf(szBuffer, _tcslen(szFormat) + 1024, szFormat, pArgList);
//				va_end(pArgList);

				//׷�ӻ���
				pOS << szBuffer;
				pOS << "\r\n";

				szBuffer = null;
			}
		}
	}

	//��Shape��Element����ΪVariableTable�������ֵ
	private void SetElementDisplacement(TVariableTable VariableTable)
	{
		for (size_t i = 0; i < VariableTable.VariableTable.Count; i++)
		{
			int id = 0;
			try
			{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: id = GetIdFromVariableStr(VariableTable.VariableTable[i]);
				id = GetIdFromVariableStr(new List(VariableTable.VariableTable[i]));
			}
			catch (System.Exception err)
			{
				//var is not a number
				continue;
			}
			TElement element = pShape.GetElementById(id);
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to references to value types:
//ORIGINAL LINE: const double &data = VariableTable.VariableValue[i];
			double data = VariableTable.VariableValue[i];

			switch (VariableTable.VariableTable[i][0])
			{
			case TEXT('x'):
				element.SetX(data);
				break;
			case TEXT('y'):
				element.SetY(data);
				break;
			case TEXT('p'):
				element.SetPhi(data); //MakeIn2Pi
				break;
			default:
				continue;
			}
		}
	}
	public TSolver()
	{
		hwndOutput = null;
		Equations = null;
		pOS = null;
	}
	public new void Dispose()
	{
		ClearEuqations();

		if (pOS != null)
		{
			if (pOS != null)
			{
				pOS.Dispose();
			}
		}
		base.Dispose();
	}

	//���
	public void Solve()
	{
		clock_t start = new clock_t(); //clock_t��clock()������������
		clock_t stop = new clock_t();
		double duration;

		start = clock();

		SubsFramePoint();
		Equations.SimplifyEquations(pOS);
		Equations.BuildJacobian(pOS);
		Equations.SolveEquations(pOS); //�ⷽ��

		if (Equations.hasSolved) //���
		{
			SetElementDisplacement(Equations.VariableTable);
		}

		stop = clock();
		duration = ((double)(stop - start)) / CLK_TCK;

		Outputln("\r\n��ʱ %f s", duration);

		if (pOS != null)
		{
			RefreshWindowText();
		}
	}
	public void ClearOutput()
	{
		if (pOS != null)
		{
			pOS = null;
			pOS = new Ostringstream();
			pOS.imbue(std::locale(""));
		}
	}
	public new void RefreshEquations()
	{
		//ȫ������
		subsVar.Clear();
		subsValue.Clear();
		ClearOutput();
		ClearEuqations();

		//��ʼ
		Outputln("���ɶ�: DOF = nc - nh = nb*3 - nh = %d*3 - %d = %d", pShape.nb, pShape.nh(), pShape.DOF());
		int i;
		int j;
		DPOINT SiP = new DPOINT();
		DPOINT SjP = new DPOINT();
		DPOINT SiQ = new DPOINT();
		DPOINT SjQ = new DPOINT();
		string buffer1 = new string(new char[1024]);
		string buffer2 = new string(new char[1024]);

		Outputln("\r\nԼ������:");

		foreach (var element in pShape.Element)
		{
			switch (element.eType)
			{
			case EnumElementType.CONSTRAINT_COINCIDE:
			{
				pShape.GetSijP((TConstraintCoincide)element, SiP, SjP, ref i, ref j);

				//�õ�2���غϹ����Ĺ�������
				double xi;
				double yi;
				double phii;
				double xj;
				double yj;
				double phij;
				pShape.GetCoordinateByElement(((TConstraintCoincide)element).pElement[0], ref xi, ref yi, ref phii);
				pShape.GetCoordinateByElement(((TConstraintCoincide)element).pElement[1], ref xj, ref yj, ref phij);

				//������������ʼֵ
				//_stprintf(buffer1, TEXT("x%d y%d phi%d x%d y%d phi%d"), i, i, i, j, j, j);
				//_stprintf(buffer2, TEXT("%f %f %f %f %f %f"), xi, yi, phii, xj, yj, phij);
				//Equations->DefineVariable(pOS, buffer1, buffer2);
				Equations.DefineOneVariable(pOS, "x" + To_string(i), xi,true);
				Equations.DefineOneVariable(pOS, "y" + To_string(i), yi, true);
				Equations.DefineOneVariable(pOS, "phi" + To_string(i), phii, true);
				Equations.DefineOneVariable(pOS, "x" + To_string(j), xj, true);
				Equations.DefineOneVariable(pOS, "y" + To_string(j), yj, true);
				Equations.DefineOneVariable(pOS, "phi" + To_string(j), phij, true);

				//���뷽��
				/* �Ƶ������
				xi + xiP*cos(phii) - yiP*sin(phii) - xj  - xjP*cos(phij) + yjP*sin(phij)
				yi + xiP*sin(phii) + yiP*cos(phii) - yj  - xjP*sin(phij) - yjP*cos(phij)  */
				_stprintf(buffer1, "x%d+%f*cos(phi%d)-%f*sin(phi%d)-x%d-%f*cos(phi%d)+%f*sin(phi%d)", i, SiP.x, i, SiP.y, i, j, SjP.x, j, SjP.y, j);
				_stprintf(buffer2, "y%d+%f*sin(phi%d)+%f*cos(phi%d)-y%d-%f*sin(phi%d)-%f*cos(phi%d)", i, SiP.x, i, SiP.y, i, j, SjP.x, j, SjP.y, j);
				Equations.AddEquation(pOS, buffer1, false);
				Equations.AddEquation(pOS, buffer2, false);
				break;
			}
			case EnumElementType.CONSTRAINT_COLINEAR: //����Լ��
			{
				TConstraintColinear pColinear = (TConstraintColinear)element;
				//����i
				pShape.GetSP(pColinear.pElement[0], pColinear.PointBeginIndexOfElement[0], SiP, ref i); //xiP,yiP
				pShape.GetSQ(pColinear.pElement[0], pColinear.PointEndIndexOfElement[0], SiQ, ref i); //xiQ,yiQ

				//����j
				pShape.GetSP(pColinear.pElement[1], pColinear.PointBeginIndexOfElement[1], SjP, ref j); //xjP,yjP
				pShape.GetSQ(pColinear.pElement[1], pColinear.PointEndIndexOfElement[1], SjQ, ref j); //xjQ,yjQ

				//�õ�2���غϹ����Ĺ�������
				double xi;
				double yi;
				double phii;
				double xj;
				double yj;
				double phij;
				pShape.GetCoordinateByElement(pColinear.pElement[0], ref xi, ref yi, ref phii);
				pShape.GetCoordinateByElement(pColinear.pElement[1], ref xj, ref yj, ref phij);

				//������������ʼֵ
				//_stprintf(buffer1, TEXT("x%d y%d phi%d x%d y%d phi%d"), i, i, i, j, j, j);
				//_stprintf(buffer2, TEXT("%f %f %f %f %f %f"), xi, yi, phii, xj, yj, phij);
				//Equations->DefineVariable(pOS, buffer1, buffer2, true);
				Equations.DefineOneVariable(pOS, "x" + To_string(i), xi, true);
				Equations.DefineOneVariable(pOS, "y" + To_string(i), yi, true);
				Equations.DefineOneVariable(pOS, "phi" + To_string(i), phii, true);
				Equations.DefineOneVariable(pOS, "x" + To_string(j), xj, true);
				Equations.DefineOneVariable(pOS, "y" + To_string(j), yj, true);
				Equations.DefineOneVariable(pOS, "phi" + To_string(j), phij, true);

				//���뷽��
				//�Ƶ������
				/*
				(cos(phii)*(yiP - yiQ) + sin(phii)*(xiP - xiQ))*(xi - xj + xiP*cos(phii) - xjP*cos(phij) - yiP*sin(phii) + yjP*sin(phij))
				- (cos(phii)*(xiP - xiQ) - sin(phii)*(yiP - yiQ))*(yi - yj + yiP*cos(phii) - yjP*cos(phij) + xiP*sin(phii) - xjP*sin(phij))
				*/
				_stprintf(buffer1, "(cos(phi%d)*(%f-%f)+sin(phi%d)*(%f-%f))*(x%d-x%d+%f*cos(phi%d)-%f*cos(phi%d)-%f*sin(phi%d)+%f*sin(phi%d))																										-(cos(phi%d)*(%f-%f)-sin(phi%d)*(%f-%f))*(y%d-y%d+%f*cos(phi%d)-%f*cos(phi%d)+%f*sin(phi%d)-%f*sin(phi%d))", i, SiP.y, SiQ.y, i, SiP.x, SiQ.x, i, j, SiP.x, i, SjP.x, j, SiP.y, i, SjP.y, j, i, SiP.x, SiQ.x, i, SiP.y, SiQ.y, i, j, SiP.y, i, SjP.y, j, SiP.x, i, SjP.x, j);

				_stprintf(buffer2, "(cos(phi%d-phi%d)*(%f-%f)+sin(phi%d-phi%d)*(%f-%f))*(%f-%f)-(cos(phi%d-phi%d)*(%f-%f)-sin(phi%d-phi%d)*(%f-%f))*(%f-%f)", i, j, SiP.y, SiQ.y, i, j, SiP.x, SiQ.x, SjP.x, SjQ.x, i, j, SiP.x, SiQ.x, i, j, SiP.y, SiQ.y, SjP.y, SjQ.y);

				Equations.AddEquation(pOS, buffer1, false);
				Equations.AddEquation(pOS, buffer2, false);
				break;
			}
			case EnumElementType.ELEMENT_SLIDEWAY:
			case EnumElementType.ELEMENT_FRAMEPOINT:
			{
				int id = element.id;

				subsVar.Add("x" + To_string(id));
				subsVar.Add("y" + To_string(id));
				subsVar.Add("phi" + To_string(id));
				subsValue.Add(element.dpt.x);
				subsValue.Add(element.dpt.y);
				subsValue.Add(element.angle);
				//Solveʱ������Jacobian���滻��
				break;
			}
			case EnumElementType.DRIVER:
			{
				TDriver pDriver = (TDriver)element;
				string s;
				s += pDriver.sExprLeft + "-(" + pDriver.sExprRight + ")";
				vecStrDriver.Add(s);
				break;
			}
			}
		}

		if (Equations.GetEquationsCount() == 0)
		{
			Outputln("\r\n��Լ����\r\n");
		}

		if (pShape.DOF() == 1)
		{
			Outputln("\r\n�����϶���");
		}
		else
		{
			if (pShape.DOF() > 1)
			{
				Outputln("\r\nǷԼ����");
			}
		}

		if (pOS != null)
		{
			RefreshWindowText();
		}
	}

	//������Լ��
	public void AddMouseConstraint(int index, DPOINT dptm)
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: AddMouseConstraint(pShape->Element[index],dptm);
		AddMouseConstraint(new List(pShape.Element[index]), new DPOINT(dptm));
	}

	//������Լ��
	public void AddMouseConstraint(TElement pElement, DPOINT dptm)
	{
		if (pElement.CanBeDragged())
		{
			string temp = new string(new char[200]);
			double xm = dptm.x;
			double ym = dptm.y;
			int id = pElement.id;

			_stprintf(temp, "(%f-x%d)*sin(phi%d+%f)-(%f-y%d)*cos(phi%d+%f)",xm, id, id, dRelativeAngle, ym, id,id, dRelativeAngle);

			string szVar = new string(new char[100]);
			string szValue = new string(new char[100]);
			_stprintf(szVar, "x%d y%d phi%d", id, id, id);
			_stprintf(szValue, "%f %f %f", pElement.dpt.x, pElement.dpt.y, pElement.angle);

			Equations.DefineVariable(pOS, szVar, szValue,true);
			Equations.AddEquation(pOS, temp, true);

			return;
		}
	}
	public void RecordStartDragPos(int index, DPOINT dpt)
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: RecordStartDragPos(pShape->Element[index], dpt);
		RecordStartDragPos(new List(pShape.Element[index]), new DPOINT(dpt));
	}
	public void RecordStartDragPos(TElement pElement, DPOINT dpt)
	{
		DPOINT dptSPP = TDraw.GetRelative(dpt, pElement.dpt, pElement.angle);

		if (!(Math.Abs(dptSPP.x) < (precision)))
		{
			dRelativeAngle = Math.Atan(dptSPP.y / dptSPP.x);
		}
		else
		{
			dRelativeAngle = 0;
		}
	}
	public void SetHwnd(IntPtr hwnd)
	{
		hwndOutput = hwnd;
		if (hwnd != null)
		{
			pOS = new Ostringstream();
			pOS.imbue(std::locale(""));
		}
		else
		{
			if (pOS != null)
			{
				pOS = null;
			}
			pOS = null;
		}
	}

	//����ʹ���˱�������ˢ��Edit����
	public void RefreshWindowText()
	{
		if (hwndOutput != null)
		{
			SetWindowText(hwndOutput, ((Ostringstream)pOS).str().c_str());

			//SetFocus(Edit.m_hWnd);

			int len = global::GetWindowTextLength(hwndOutput);
			global::PostMessage(hwndOutput, EM_SETSEL, len, len);
			PostMessage(hwndOutput, EM_SCROLLCARET, 0, 0);
		}
	}
	public void ClearConstraint()
	{
		Equations.RemoveTempEquations();
	}
	public void Demo()
	{
		//Outputln(Equations->VariableTable.Define(true, TEXT("x1 y1 phi1 x2 y2 phi2 l t"),TEXT("0 0 0 2.3 0 0 1.3 0")));
		//Outputln(Equations->AddEquation(true, TEXT("x1"), false));
		//Outputln(Equations->AddEquation(true, TEXT("y1"), false));
		//Outputln(Equations->AddEquation(true, TEXT("y2"), false));
		//Outputln(Equations->AddEquation(true, TEXT("x1-x2+cos(phi1)+l*sin(phi2)"), false));
		//Outputln(Equations->AddEquation(true, TEXT("y1-y2+sin(phi1)-l*cos(phi2)"), false));
		//Outputln(Equations->AddEquation(true, TEXT("phi1-t"), false));
		//Outputln(Equations->BuildJacobian(true, TEXT("l t"), TEXT("1.3 0")));

		/*
		Outputln(Equations->DefineVariable(true, TEXT("x2 phi2 z t l"), TEXT("0 0 0 0 0")));
		Outputln(Equations->AddEquation(true, TEXT("-x2+1+1.3*sin(phi2)*l"), false));
		Outputln(Equations->AddEquation(true, TEXT("-1.3*(-phi2/(5+4))-20"), false));
		Outputln(Equations->AddEquation(true, TEXT("2*sin(z)+0.5*cos(z)+t"), false));
		Outputln(Equations->AddEquation(true, TEXT("t-0.3"), false));
		Outputln(Equations->Subs(true, TEXT("l"), TEXT("1.3")));
		Outputln(Equations->SimplifyEquations(true));
		Outputln(Equations->BuildJacobian(true));
		Outputln(Equations->SolveEquations(true));*/

		Equations.RemoveTempEquations();

		Equations.DefineOneVariable(pOS, "t", 0.1);
		Equations.AddEquation(pOS, "phi2-ln(t)", true);
		Equations.Subs(pOS, subsVar, subsValue);

		Equations.BuildEquationsV(pOS); //��ʱt���ڱ�������
		Equations.BuildEquationsA_Phitt(pOS);

		Equations.Subs(pOS, "t", 0.1);
		//Equations->SimplifyEquations(pOS);

		Equations.BuildVariableTableV(pOS);
		Equations.BuildVariableTableA(pOS);

		Equations.BuildJacobian(pOS);

		//���λ�Ʒ���
		Equations.SolveEquations(pOS);

		if (Equations.hasSolved)
		{
			//����ٶȷ���
			Equations.SubsV(pOS, "t", 0.1);
			Equations.SolveEquationsV(pOS);

			//������ٶȷ���
			Equations.SubsA(pOS, "t", 0.1);
			Equations.SolveEquationsA(pOS);

			//����λ��
			SetElementDisplacement(Equations.VariableTable);
		}
		RefreshWindowText();

		//EquationsV->RemoveTempEquations();
		//EquationsV->DefineVariable(pOS, TEXT("t"), TEXT("0"));
		//EquationsV->AddEquation(pOS, TEXT("phi2-sin(t)"), true);
		//EquationsV->Subs(pOS, subsVar, subsValue);
		//EquationsV->BuildJacobian(pOS);
		//for (auto pEqua : Equations->Equations)
		//{
		//	Outputln(pEqua->Diff(TEXT("t"), 1, true));
		//	Outputln(pEqua->Simplify(true));
		//}
		//Equations->SolveEquations(pOS);

		//����ExpressionTree
		//TExpressionTree ex;
		//TVariableTable VarTable;
		//Outputln(VarTable.Define(true,TEXT("  x   y z   ")));
		//Outputln(ex.LinkVariableTable(&VarTable));
		//Outputln(ex.Read(TEXT("sqrt(x/y)"), true));
		//Outputln(ex.Simplify(true));
		//Outputln(ex.Diff(TEXT("x"), 1, true));
		//Outputln(ex.Simplify(true));
		//Outputln(ex.Subs(TEXT("x y"), TEXT(" y 0.1 "), true));
		//Outputln(ex.Simplify(true));
		//Outputln(TEXT("%f"), ex.Value(true));
	}
	public void ClearEuqations()
	{
		if (Equations != null)
		{
			if (Equations != null)
			{
				Equations.Dispose();
			}
		}
		Equations = new TEquations();

		//if (EquationsV != NULL)
		//	delete EquationsV;
		//EquationsV = new TEquations;

		vecStrDriver.Clear();
	}

	//���֮ǰ��Ҫ����SubsFramePoint
	//�����Ƿ����ɹ�
	public bool Solve(double t)
	{
		if (pOS != null)
		{
			string s = new string(30, TEXT('='));
			pOS << s << " Solve(t=" << t << ") " << s << "\r\n";
		}

		Equations.RemoveTempEquations();

		Equations.DefineOneVariable(pOS, "t", t,true);

		SubsFramePoint();

		//������������
		foreach (var StrDriver in vecStrDriver)
		{
			Equations.AddEquation(pOS, StrDriver, true);
		}

		Equations.BuildEquationsV(pOS); //��ʱt���ڱ�������
		Equations.BuildEquationsA_Phitt(pOS);

		Equations.Subs(pOS, "t", t); //���������t
		//Equations->SimplifyEquations(pOS);

		Equations.BuildVariableTableV(pOS);
		Equations.BuildVariableTableA(pOS);

		Equations.BuildJacobian(pOS);

		//���λ�Ʒ���
		Equations.SolveEquations(pOS);

		if (Equations.hasSolved)
		{
			//����ٶȷ���
			Equations.SubsV(pOS, "t", t);
			Equations.SolveEquationsV(pOS);

			//������ٶȷ���
			Equations.SubsA(pOS, "t", t);
			Equations.SolveEquationsA(pOS);

			//����λ��
			SetElementDisplacement(Equations.VariableTable);
		}
		RefreshWindowText();

		return Equations.hasSolved;
	}

	//����x y phi�����ձ������˳�򣬽�vecpValue���ӵ�Ԫ�ص�ֵ
	public void LinkpValue(List<double > vecpValue)
	{
		vecpValue.Clear();
		TVariableTable VariableTable = Equations.VariableTable;
		for (size_t i = 0; i < VariableTable.VariableTable.Count; i++)
		{
//C++ TO C# CONVERTER WARNING: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: int id = GetIdFromVariableStr(VariableTable.VariableTable[i]);
			int id = GetIdFromVariableStr(new List(VariableTable.VariableTable[i]));
			TElement element = pShape.GetElementById(id);

			switch (VariableTable.VariableTable[i][0])
			{
			case TEXT('x'):
				vecpValue.Add((element.dpt.x));
				break;
			case TEXT('y'):
				vecpValue.Add((element.dpt.y));
				break;
			case TEXT('p'):
				vecpValue.Add((element.angle));
				break;
			default:
				continue;
			}

		}
	}
	public void GetResult(ref List<double> vecResult)
	{
		vecResult = Equations.VariableTable.VariableValue;
	}
	public void SubsFramePoint()
	{
		if (subsVar.Count > 0 && subsValue.Count > 0)
		{
			Equations.Subs(pOS, subsVar, subsValue);
		}
	}

	public void GetMesureResult(List<DialogAnimation.TListBoxItem> vecItems, List<int> vecIndex)
	{
		for (size_t i = 0; i < vecItems.Count; ++i)
		{
			TVariableTable pVarTable;
			switch (vecItems[i].type)
			{
//C++ TO C# CONVERTER TODO TASK: C# does not allow fall-through from a non-empty 'case':
			case D:
			{
				pVarTable = (Equations.VariableTableUnsolved); //ָ��VariableTable

				DPOINT dptAb = vecItems[i].pElement.GetAbsolutePointByIndex(vecItems[i].index_of_point);
				switch (vecItems[i].value_type)
				{
				case X:
					vecItems[i].data.Add(dptAb.x);
					break;
				case Y:
					vecItems[i].data.Add(dptAb.y);
					break;
				case PHI:
					vecItems[i].data.Add(pVarTable.VariableValue[vecIndex[i]]);
					break;
				}
				break;
			}
			case V:
				pVarTable = (Equations.VariableTableV); //ָ��VariableTableV
				vecItems[i].data.Add(pVarTable.VariableValue[vecIndex[i]]);
				break;
			case A:
				pVarTable = (Equations.VariableTableA); //ָ��VariableTableA
				vecItems[i].data.Add(pVarTable.VariableValue[vecIndex[i]]);
				break;
			}
		}
	}

	//��δ���������Ϊ˳�����ݣ�����ǰ����SubsFramePoint
	public void LinkMesureResult(List<DialogAnimation.TListBoxItem> vecItems, List<int> vecIndex)
	{
		string temp;
		foreach (var Item in vecItems)
		{
			switch (Item.value_type)
			{
//C++ TO C# CONVERTER TODO TASK: C# does not allow fall-through from a non-empty 'case':
			case X:
				temp = "x";
				break;
			case Y:
				temp = "y";
				break;
			case PHI:
				temp = "phi";
				break;
			}
			temp += To_string(Item.id);
			//���˵õ�������

			//λ�Ʊ���ٶȱ���Ŀ˳��һ�£��õ���������
			TVariableTable pVarTable = (Equations.VariableTableUnsolved); //��δ���������˳��Ϊ׼

			var it = find(pVarTable.VariableTable.GetEnumerator(), pVarTable.VariableTable.end(), temp);
			if (it != pVarTable.VariableTable.end())
			{
				vecIndex.Add(it - pVarTable.VariableTable.GetEnumerator());
			}
		}

	}
}




