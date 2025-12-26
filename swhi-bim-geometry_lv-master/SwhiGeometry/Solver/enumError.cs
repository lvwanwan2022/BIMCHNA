
using System;
using System.Collections.Generic;

namespace Lv.BIM.Solver
{
	public enum enumError
	{
		ERROR_NO,
		ERROR_WRONG_MATH_OPERATOR,//eOperator值错误，通常是内存跑偏引起
		ERROR_ILLEGALCHAR,//出现非法字符
		ERROR_PARENTHESIS_NOT_MATCH,//括号不匹配
		ERROR_INVALID_VARNAME,//无效变量名
		ERROR_WRONG_EXPRESSION,//表达式逻辑不正确
		ERROR_EMPTY_INPUT,//表达式为空
		ERROR_DIVIDE_ZERO,//除0
		ERROR_UNDEFINED_VARIABLE,//未定义的变量
		ERROR_ZERO_POWEROF_ZERO,//0^0
		ERROR_SUBS_NOT_EQUAL,//替换与被替换数量不对等
		ERROR_NOT_LINK_VARIABLETABLE,//未链接变量表
		ERROR_OUTOF_DOMAIN,//计算超出定义域
		ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT,//定义变量时变量数量与初始值不等
		ERROR_VAR_HAS_BEEN_DEFINED,//变量重定义
		ERROR_I,//出现虚数
		ERROR_INDETERMINATE_EQUATION,//不定方程
		ERROR_SINGULAR_MATRIX,//矩阵奇异
		ERROR_JACOBI_ROW_NOT_EQUAL_PHI_ROW,//A矩阵行数不等于b向量数
		ERROR_INFINITY_SOLUTIONS,//无穷多解
		ERROR_OVER_DETERMINED_EQUATIONS//方程组过定义

	}

	public class TError : System.Exception
	{
		public enumError id;
		public string info;
		public TError(enumError inId, string inInfo)
		{
			this.id = inId;
			this.info = inInfo;
		}
		public string GetErrorInfo(enumError err)
		{
			switch (err)
			{
				case enumError.ERROR_NO:
					return "操作成功完成。";

				case enumError.ERROR_ILLEGALCHAR:
					return "错误：出现非法字符。";

				case enumError.ERROR_PARENTHESIS_NOT_MATCH:
					return "错误：括号不匹配。";

				case enumError.ERROR_INVALID_VARNAME:
					return "错误：不正确的变量名（必须以下划线\"_\"或英文字母开头）。";

				case enumError.ERROR_WRONG_EXPRESSION:
					return "错误：错误的表达式。";

				case enumError.ERROR_EMPTY_INPUT:
					return "表达式为空。";

				case enumError.ERROR_DIVIDE_ZERO:
					return "错误：不能除以0。";

				case enumError.ERROR_UNDEFINED_VARIABLE:
					return "错误：未定义的变量。";

				case enumError.ERROR_ZERO_POWEROF_ZERO:
					return "错误：0的0次方。";

				case enumError.ERROR_SUBS_NOT_EQUAL:
					return "错误：替换与被替换数目不等。";

				case enumError.ERROR_NOT_LINK_VARIABLETABLE:
					return "程序错误：未链接变量表。";

				case enumError.ERROR_OUTOF_DOMAIN:
					return "错误：超出定义域。";

				case enumError.ERROR_VAR_COUNT_NOT_EQUAL_NUM_COUNT:
					return "错误：变量名与初始值数量不对等。";

				case enumError.ERROR_I:
					return "暂不支持虚数。";

				default:
					return "undefined error";

			}
		}
	}
}