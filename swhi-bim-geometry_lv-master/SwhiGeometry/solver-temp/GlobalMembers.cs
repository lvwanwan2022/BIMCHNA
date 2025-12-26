//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

public static class GlobalMembers
{
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

	static int Main()
	{
		setlocale(LC_ALL, "");

		//TEquations Eq;

		//Eq.max_step = 100;
		//Eq.epsilon = 1e-6;
		//Eq.DefineVariable(&COUT, TEXT("x y"), TEXT("0.1 0.1"));
		//Eq.AddEquation(&COUT, TEXT("sin(x)^2+x*y+y-3"), false);
		//Eq.AddEquation(&COUT, TEXT("4*x+y^2"), false);
		//Eq.BuildJacobian(&COUT);
		//Eq.SolveEquations(&COUT);

		//TEquations Eq;

		//Eq.max_step = 100;
		//Eq.epsilon = 1e-16;
		//Eq.DefineVariable(&COUT, TEXT("x"),TEXT("0"));
		//Eq.AddEquation(&COUT, TEXT("sin(x)*(cos(x)/cos(0.6981)-1)^1.5-0.0027"), false);
		//Eq.BuildJacobian(&COUT);
		//Eq.SolveEquations(&COUT);

		TExpressionTree Expr = new TExpressionTree();
		TVariableTable VarTable = new TVariableTable();
		VarTable.Define(COUT, "x y");
		Expr.LinkVariableTable(VarTable);

		COUT << ">>Read:" << "\n";
		Expr.Read("-sin(x)+0.75*y^3-(y/ln(x))*4", true);

		//COUT << TEXT(">>Subs(TEXT(\"x y\"), TEXT(\"x+1 2*y\"), true):") << endl;
		//COUT << Expr.Subs(TEXT("x y"), TEXT("x+1 2*y"), true) << endl << endl;

		//COUT << TEXT(">>Diff(TEXT(\"x\"), 2, true)") << endl;
		//COUT << Expr.Diff(TEXT("x"), 2, true) << endl << endl;

		//COUT << TEXT(">>Simplify(true)") << endl;
		//COUT << Expr.Simplify(true) << endl << endl;

		//COUT << TEXT(">>Subs(TEXT(\"x y\"), TEXT(\"1 2\"), true)") << endl;
		//COUT << Expr.Subs(TEXT("x y"), TEXT("1 2"), true) << endl << endl;

		//COUT << TEXT(">>Calc()") << endl;
		//COUT << Expr.Calc() << endl << endl;

		//TExpressionTree Expr2;
		//Expr2.LinkVariableTable(&VarTable);

		//COUT << TEXT(">>Solve(\"7*(tan(x+1)-2)+5=0\"):") << endl;
		//COUT << Expr2.Read(TEXT("7*(tan(x+1)-2)+5"), false);
		//double v;
		//TCHAR *var_x;
		//COUT << TEXT("x = ");
		//COUT << Expr2.Solve(var_x, v);
		//COUT << TEXT(" = ") << v << endl;

		system("pause");

		return 0;
	}
}