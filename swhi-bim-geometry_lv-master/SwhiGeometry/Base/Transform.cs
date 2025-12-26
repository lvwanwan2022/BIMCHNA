using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{

    /// <summary>
    /// The 4x4 transform matrix.
    /// </summary>
    /// <remarks>
    /// The 3x3 sub-matrix determines scaling.
    /// The 4th column defines translation, where the last value could be a divisor.
    /// </remarks>
    public class TransformXYZ : Base
    {
        //第1,2,3行用来x,y,z缩放，最后一列用来对xyz进行平移
        private double[,] _value =new double[4, 4] { 
            { 1d, 0d, 0d, 0d },
            { 0d, 1d, 0d, 0d },
            { 0d, 0d, 1d, 0d },
            { 0d, 0d, 0d, 1d } };
        public double[] value { get { return _value.Flatten(); } set { _value = value.ReSize(4, 4); } } 

        public TransformXYZ(double[,] source)
        {
            if (source.GetLength(0)==4 && source.GetLength(1) == 4)
            {
                _value = source;
            }
            else
            {
                throw new Exception("输入数组应为4行4列");
            }
           
        }
      
        //
        // 摘要:
        //     Gets or sets the matrix value at the given row and column indices.
        //
        // 参数:
        //   row:
        //     Index of row to access, must be 0, 1, 2 or 3.
        //
        //   column:
        //     Index of column to access, must be 0, 1, 2 or 3.
        //
        // 返回结果:
        //     The value at [row, column]
        //
        // 值:
        //     The new value at [row, column]
        public double this[int row, int column] { get { return _value[row,column]; } set { _value[row, column] = value; } }

        //
        // 摘要:
        //     ZeroTransformation diagonal = (0,0,0,1)
        public static TransformXYZ ZeroTransformation => new TransformXYZ( new double[4, 4] {{ 0d, 0d, 0d, 0d },
            { 0d, 0d, 0d, 0d },
            { 0d, 0d, 0d, 0d },
            { 0d, 0d, 0d, 1d } });
        //
        // 摘要:
        //     Gets an XForm filled with RhinoMath.UnsetValue.
        public static TransformXYZ Unset { get {
                TransformXYZ result = new TransformXYZ();
                result.M00 = -1.23432101234321E+308;
                result.M01 = -1.23432101234321E+308;
                result.M02 = -1.23432101234321E+308;
                result.M03 = -1.23432101234321E+308;
                result.M10 = -1.23432101234321E+308;
                result.M11 = -1.23432101234321E+308;
                result.M12 = -1.23432101234321E+308;
                result.M13 = -1.23432101234321E+308;
                result.M20 = -1.23432101234321E+308;
                result.M21 = -1.23432101234321E+308;
                result.M22 = -1.23432101234321E+308;
                result.M23 = -1.23432101234321E+308;
                result.M30 = -1.23432101234321E+308;
                result.M31 = -1.23432101234321E+308;
                result.M32 = -1.23432101234321E+308;
                result.M33 = -1.23432101234321E+308;
                return result;
            } }
        //
        // 摘要:
        //     Gets a new identity transform matrix. An identity matrix defines no transformation.
        public static TransformXYZ Identity => new TransformXYZ(new double[4, 4] {{ 1d, 0d, 0d, 0d },
            { 0d, 1d, 0d, 0d },
            { 0d, 0d, 1d, 0d },
            { 0d, 0d, 0d, 1d } });
        public double[] R0 {
            get { return new double[4] { _value[0, 0], _value[0, 1], _value[0, 2], _value[0, 3] }; }
            set {
                if (value.Length == 4) 
                { _value[0, 0] = value[0];
                    _value[0, 1] = value[1];
                    _value[0, 2] = value[2];
                    _value[0, 3] = value[3];
                }
                else
                {
                    throw new Exception("输入数组长度不对");
                }
            }
        }
        public double[] R1
        {
            get { return new double[4] { _value[1, 0], _value[1, 1], _value[1, 2], _value[1, 3] }; }
            set
            {
                if (value.Length == 4)
                {
                    _value[1, 0] = value[0];
                    _value[1, 1] = value[1];
                    _value[1, 2] = value[2];
                    _value[1, 3] = value[3];
                }
                else
                {
                    throw new Exception("输入数组长度不对");
                }
            }
        }
        public double[] R2
        {
            get { return new double[4] { _value[2, 0], _value[2, 1], _value[2, 2], _value[2, 3] }; }
            set
            {
                if (value.Length == 4)
                {
                    _value[2, 0] = value[0];
                    _value[2, 1] = value[1];
                    _value[2, 2] = value[2];
                    _value[2, 3] = value[3];
                }
                else
                {
                    throw new Exception("输入数组长度不对");
                }
            }
        }
        public double[] R3
        {
            get { return new double[4] { _value[3, 0], _value[3, 1], _value[3, 2], _value[3, 3] }; }
            set
            {
                if (value.Length == 4)
                {
                    _value[3, 0] = value[0];
                    _value[3, 1] = value[1];
                    _value[3, 2] = value[2];
                    _value[3, 3] = value[3];
                }
                else
                {
                    throw new Exception("输入数组长度不对");
                }
            }
        }
 
        public double M03 { get { return _value[0, 3]; } set { _value[0, 3] = value; } }

        public double M02 { get { return _value[0, 2]; } set { _value[0, 2] = value; } }

        public double M01 { get { return _value[0, 1]; } set { _value[0, 1] = value; } }

        public double M00 { get { return _value[0, 0]; } set { _value[0, 0] = value; } }

        public double M13 { get { return _value[1, 3]; } set { _value[1, 3] = value; } }
  
        public double M12 { get { return _value[1, 2]; } set { _value[1, 2] = value; } }

        public double M11 { get { return _value[1, 1]; } set { _value[1, 1] = value; } }

        public double M10 { get { return _value[1, 0]; } set { _value[1, 0] = value; } }

        public double M20 { get { return _value[2, 0]; } set { _value[2, 0] = value; } }

        public double M21 { get { return _value[2, 1]; } set { _value[2, 1] = value; } }

        public double M22 { get { return _value[2, 2]; } set { _value[2, 2] = value; } }

        public double M23 { get { return _value[2, 3]; } set { _value[2, 3] = value; } }

        public double M30 { get { return _value[3, 0]; } set { _value[3, 0] = value; } }

        public double M31 { get { return _value[3, 1]; } set { _value[3, 1] = value; } }
   
        public double M32 { get { return _value[3, 2]; } set { _value[3, 2] = value; } }

        public double M33 { get { return _value[3, 3]; } set { _value[3, 3] = value; } }


        [JsonIgnore] public double[] translation => value.Subset(3, 7, 11, 15);

        [JsonIgnore] public double[] scaling => value.Subset(0, 1, 2, 4, 5, 6, 8, 9, 10);
        public XYZ ScaleXVector { get { return new XYZ(M00, M01, M02); } 
            set {
                _value[0, 0] = value.X;
                _value[0, 1] = value.Y;
                _value[0, 2] = value.Z;
            } }
        public XYZ ScaleYVector
        {
            get { return new XYZ(M10, M11, M12); }
            set
            {
                _value[1, 0] = value.X;
                _value[1, 1] = value.Y;
                _value[1, 2] = value.Z;
            }
        }
        public XYZ ScaleZVector
        {
            get { return new XYZ(M20, M21, M22); }
            set
            {
                _value[2, 0] = value.X;
                _value[2, 1] = value.Y;
                _value[2, 2] = value.Z;
            }
        }
        public XYZ TranslateVector
        {
            get { return new XYZ(M03, M13, M23); }
            set
            {
                _value[0, 3] = value.X;
                _value[1, 3] = value.Y;
                _value[2, 3] = value.Z;
            }
        }
        [JsonIgnore]
        public bool isIdentity => value[0] == 1d && value[5] == 1d && value[10] == 1d && value[15] == 1d &&
                                  value[1] == 0d && value[2] == 0d && value[3] == 0d &&
                                  value[4] == 0d && value[6] == 0d && value[7] == 0d &&
                                  value[8] == 0d && value[9] == 0d && value[11] == 0d &&
                                  value[12] == 0d && value[13] == 0d && value[14] == 0d;

        [JsonIgnore] public bool isScaled => !(value[0] == 1d && value[5] == 1d && value[10] == 1d);
 

        /// <summary>
        /// True if matrix is Zero4x4, ZeroTransformation, or some other type of
        /// zero. The value xform[3][3] can be anything.
        /// </summary>
        /// <since>6.1</since>
        public bool IsZero
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4 && (3 != i || 3 != j); j++)
                    {
                        if (this[i, j] != 0.0)
                        {
                            return false;
                        }
                    }
                }
                if (!double.IsNaN(M33))
                {
                    return !double.IsInfinity(M33);
                }
                return false;
            }
        }

        /// <summary>
        /// True if all values are 0
        /// </summary>
        /// <since>1.0</since>
        public bool IsZero4x4
        {
            get
            {
                if (0.0 == M33)
                {
                    return IsZero;
                }
                return false;
            }
        }

        /// <summary>
        /// True if all values are 0, except for M33 which is 1.
        /// </summary>
        /// <seealso cref="M:Rhino.Geometry.Transform.IsZeroTransformationWithTolerance(System.Double)" />
        /// <since>1.0</since>
        public bool IsZeroTransformation
        {
            get
            {
                if (1.0 == M33)
                {
                    return IsZero;
                }
                return false;
            }
        }
        public TransformXYZ()
        {
           
        }

        public TransformXYZ(double[] value)
        {
            this._value = value.ReSize(4,4);
        }
        //1.平移图形使旋转轴AP的A点与坐标原点重合;
        //2.旋转图形使AP轴与某一坐标轴重合;
        /// <summary>
        /// Computes a change of basis transformation. A basis change is essentially a remapping of geometry from one coordinate system to another.
        /// </summary>
        /// <param name="plane0">现坐标系</param>
        /// <param name="plane1"> 目标坐标系.</param>
        /// <returns>返回转换矩阵</returns>
        public static TransformXYZ ChangeBasis(PlaneXYZ plane0, PlaneXYZ plane1)
        {
            XYZ org0 = plane0.Origin;
            XYZ org1 = plane1.Origin;        
            
            TransformXYZ ro = TransformXYZ.Rotation(plane1.Xaxis, plane1.Yaxis, plane1.Normal, plane0.Xaxis, plane0.Yaxis, plane0.Normal);
            
            TransformXYZ translate= TransformXYZ.Translation(org0 - org1);

            return  ro * translate;

        }

        public static TransformXYZ ChangeBasis(XYZ initialBasisX, XYZ initialBasisY, XYZ initialBasisZ, XYZ finalBasisX, XYZ finalBasisY, XYZ finalBasisZ)
        {
            //TransformXYZ roz = TransformXYZ.Rotation(initialBasisZ, finalBasisY,new XYZ());
            TransformXYZ rox = TransformXYZ.Rotation( finalBasisX, finalBasisY, finalBasisZ, initialBasisX, initialBasisY, initialBasisZ);
            return rox;
        }
        //
        // 摘要:
        //     Constructs a new transformation with diagonal (d0,d1,d2,1.0).
        //
        // 参数:
        //   diagonal:
        //     The diagonal values.
        //
        // 返回结果:
        //     A transformation with diagonal (d0,d1,d2,1.0).
        public static TransformXYZ Diagonal(XYZ diagonal)
        {
            return Diagonal(diagonal.X, diagonal.Y, diagonal.Z);
        }
        //
        // 摘要:
        //     Constructs a new transformation with diagonal (d0,d1,d2,1.0).
        //
        // 参数:
        //   d0:
        //     Transform.M00 value.
        //
        //   d1:
        //     Transform.M11 value.
        //
        //   d2:
        //     Transform.M22 value.
        //
        // 返回结果:
        //     A transformation with diagonal (d0,d1,d2,1.0).
        public static TransformXYZ Diagonal(double d0, double d1, double d2)
        {
            TransformXYZ identity = Identity;
            identity.M00 = d0;
            identity.M11 = d1;
            identity.M22 = d2;
            return identity;
        }
        
        public static TransformXYZ Mirror(PlaneXYZ mirrorPlane)
        {
            double xn = mirrorPlane.Normal.X;
            double yn = mirrorPlane.Normal.Y;
            double zn = mirrorPlane.Normal.Z;
            double dis = mirrorPlane.DistanceTo(new XYZ());
            TransformXYZ result = Identity;
            result.R0 = new double[4] { 1 - 2 * xn * xn, -2 * xn * yn, -2 * xn * zn, -2 * xn * dis };
            result.R1 = new double[4] {  -2 * xn * yn, 1-2 * yn * yn, -2 * yn * zn, -2 * yn * dis };
            result.R2 = new double[4] { -2 * zn * xn, -2 * zn * yn, 1-2 * zn * zn, -2 * zn * dis };
            return result;
        }
   
        public static TransformXYZ Mirror(XYZ pointOnMirrorPlane, XYZ normalToMirrorPlane)
        {
            XYZ nor = normalToMirrorPlane.Normalize();
            double xn = nor.X;
            double yn = nor.Y;
            double zn = nor.Z;
            XYZ op = new XYZ(0,0,0)- pointOnMirrorPlane ;
            double dis=op.DotProduct(nor);             
            TransformXYZ result = Identity;
            result.R0 = new double[4] { 1 - 2 * xn * xn, -2 * xn * yn, -2 * xn * zn, -2 * xn * dis };
            result.R1 = new double[4] { -2 * xn * yn, 1 - 2 * yn * yn, -2 * yn * zn, -2 * yn * dis };
            result.R2 = new double[4] { -2 * zn * xn, -2 * zn * yn, 1 - 2 * zn * zn, -2 * zn * dis };
            return result;
        }
        
    
        public static TransformXYZ PlanarProjection(PlaneXYZ plane)
        {
            TransformXYZ result= TransformXYZ.Identity;
            
            TransformXYZ project = TransformXYZ.Identity;
            XYZ origin = plane.Origin;
            XYZ xAxis = plane.Xaxis;
            XYZ yAxis = plane.Yaxis;
            XYZ nor = plane.Normal;
            project.ScaleXVector = xAxis;
            project.ScaleYVector = yAxis;
            project.ScaleZVector = new XYZ();
            TransformXYZ tranlate = TransformXYZ.Translation(origin);
            TransformXYZ tranlatena = TransformXYZ.Translation(-origin);
            result = tranlate * project * tranlatena;
            return result;

        }
        //
        // 摘要:
        //     Create a rotation transformation that orients plane0 to plane1. If you want to
        //     orient objects from one plane to another, use this form of transformation.
        //
        // 参数:
        //   plane0:
        //     The plane to orient from.
        //
        //   plane1:
        //     the plane to orient to.
        //
        // 返回结果:
        //     The translation transformation if successful, Transform.Unset on failure.
        public static TransformXYZ PlaneToPlane(PlaneXYZ plane0, PlaneXYZ plane1)
        {
            //XYZ org0 = plane0.Origin;
            //XYZ org1 = plane1.Origin;

            //TransformXYZ ro = TransformXYZ.Rotation(plane0.Xaxis, plane0.Yaxis, plane0.Normal, plane1.Xaxis, plane1.Yaxis, plane1.Normal);

            ////TransformXYZ translate = TransformXYZ.Translation(org1 - org0);

            //return ro* translate;
            throw new NotImplementedException();
        }
        //
        // 摘要:
        //     Construct a projection onto a plane along a specific direction.
        //
        // 参数:
        //   plane:
        //     Plane to project onto.
        //
        //   direction:
        //     Projection direction, must not be parallel to the plane.
        //
        // 返回结果:
        //     Projection transformation or identity transformation if projection could not
        //     be calculated.
        public static TransformXYZ ProjectAlong(PlaneXYZ plane, XYZ direction)
        {
            
            if (plane.Normal.IsPerpendicularTo(direction))
            {
                return Identity;
            }
            if (plane.Normal.IsParallelTo(direction))
            {
                return PlanarProjection(plane);
            }
            PlaneXYZ plane2 = plane;
            PlaneXYZ plane3 = new PlaneXYZ(plane.Origin, direction);
            LineXYZ intersectionLine = plane2.GetIntersectLine(plane3);
            if (!plane2.IsIntersected(plane3))
            {
                return PlanarProjection(plane);
            }
            PlaneXYZ plane4 = plane2;
            plane4.Xaxis = intersectionLine.Direction;
            plane4.Yaxis = plane4.Xaxis.CrossProduct(plane4.Normal);
            double num = plane2.Normal.AngleTo(direction);
            double num2 = Math.Sin(Math.PI / 2.0 - num);
            if (Math.Abs(num2) < 1E-64)
            {
                return Identity;
            }
            TransformXYZ transform = PlanarProjection(plane3);
            TransformXYZ transform2 = Rotation(plane3.Normal, plane2.Normal, plane2.Origin);
            return Scale(plane4, 1.0, 1.0 / num2, 1.0) * transform2 * transform;
        }
        /// <summary>
        /// Constructs a transformation that maps X0 to X1, Y0 to Y1, Z0 to Z1.
        /// The frames should be right hand orthonormal frames (unit vectors with Z = X x Y).
        /// The resulting rotation fixes the origin (0,0,0), maps initial X to final X, 
        /// initial Y to final Y, and initial Z to final Z.
        /// </summary>
        /// <param name="x0">Initial frame X.</param>
        /// <param name="y0">Initial frame Y.</param>
        /// <param name="z0">Initial frame Z.</param>
        /// <param name="x1">Final frame X.</param>
        /// <param name="y1">Final frame Y.</param>
        /// <param name="z1">Final frame Z.</param>
        /// <returns>A rotation transformation matrix.</returns>
        /// <since>1.0</since>
        public static TransformXYZ Rotation(XYZ Vector_x0, XYZ Vector_y0, XYZ Vector_z0, XYZ Vector_x1, XYZ Vector_y1, XYZ Vector_z1)
        {
            TransformXYZ transform = new TransformXYZ();
            transform[0, 0] = Vector_x0.X;
            transform[0, 1] =Vector_x0.Y;
            transform[0, 2] =Vector_x0.Z;
            transform[1, 0] =Vector_y0.X;
            transform[1, 1] =Vector_y0.Y;
            transform[1, 2] =Vector_y0.Z;
            transform[2, 0] =Vector_z0.X;
            transform[2, 1] =Vector_z0.Y;
            transform[2, 2] =Vector_z0.Z;
            transform[3, 3] = 1.0;
            TransformXYZ transform2 = new TransformXYZ();
            transform2[0, 0] = Vector_x1.X;
            transform2[0, 1] = Vector_y1.X;
            transform2[0, 2] = Vector_z1.X;
            transform2[1, 0] = Vector_x1.Y;
            transform2[1, 1] = Vector_y1.Y;
            transform2[1, 2] = Vector_z1.Y;
            transform2[2, 0] = Vector_x1.Z;
            transform2[2, 1] = Vector_y1.Z;
            transform2[2, 2] = Vector_z1.Z;
            transform2[3, 3] = 1.0;
            return transform2 * transform;
        }
        /// <summary>
        /// Constructs a new rotation transformation with start and end directions and rotation center.
        /// </summary>
        /// <param name="startDirection">A start direction.</param>
        /// <param name="endDirection">An end direction.</param>
        /// <param name="rotationCenter">3D center of rotation.</param>
        /// <returns>A rotation transformation matrix.</returns>
        /// <since>5.0</since>
        public static TransformXYZ Rotation(XYZ startDirection, XYZ endDirection, XYZ rotationCenter)
        {
            if (Math.Abs(startDirection.Length - 1.0) > 1E-08)
            {
                startDirection.Unitize();
            }
            if (Math.Abs(endDirection.Length - 1.0) > 1E-08)
            {
                endDirection.Unitize();
            }
            double num = startDirection * endDirection;
            XYZ rotationAxis = startDirection.CrossProduct(endDirection);
            double num2 = rotationAxis.Length;
            if (0.0 == num2)
            {
                throw new Exception("起始向量和终止向量重合");
            }
            if( !rotationAxis.IsUnitLength())
            {                
                rotationAxis.Unitize();
                num2 = 0.0;
                num = ((num < 0.0) ? (-1.0) : 1.0);
            }
            return Rotation(num2, num, rotationAxis, rotationCenter);
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle and rotation center. The axis of rotation is <see cref="P:Rhino.Geometry.Vector3d.ZAxis" />.
        /// </summary>
        /// <param name="angleRadians">Rotation angle in radians.</param>
        /// <param name="rotationCenter">3D center of rotation.</param>
        /// <returns>A rotation transformation matrix.</returns>
        /// <since>5.0</since>
        public static TransformXYZ Rotation(double angleRadians, XYZ rotationCenter)
        {
            return Rotation(angleRadians, new XYZ(0d,0d,1.0), rotationCenter);
        }

        /// <summary>
        /// 根据旋转角度的弧度值，旋转轴和旋转中心构造旋转转换矩阵
        /// </summary>
        /// <param name="angleRadians">旋转角度的弧度值</param>
        /// <param name="rotationAxis">旋转轴.</param>
        /// <param name="rotationCenter">旋转中心.</param>
        /// <returns>旋转转换矩阵.</returns>
        /// <since>1.0</since>
        public static TransformXYZ Rotation(double angleRadians, XYZ rotationAxis, XYZ rotationCenter)
        {
            return Rotation(Math.Sin(angleRadians), Math.Cos(angleRadians), rotationAxis, rotationCenter);
        }

        
        /// <summary>
        /// 根据sinθ，cosθ，旋转轴和旋转中心构造旋转转换矩阵
        /// </summary>
        /// <param name="sinAngle">旋转角的sin值</param>
        /// <param name="cosAngle">旋转角的cos值</param>
        /// <param name="rotationAxis">旋转轴</param>
        /// <param name="rotationCenter">旋转中心点坐标</param>
        /// <returns></returns>
        public static TransformXYZ Rotation(double sinAngle, double cosAngle, XYZ rotationAxis, XYZ rotationCenter)
        {
            double n1 = rotationAxis.X;
            double n2 = rotationAxis.Y;
            double n3 = rotationAxis.Z;
            TransformXYZ tempRo= new TransformXYZ(new double[4, 4]{
                { n1* n1 *(1 - cosAngle) + cosAngle, n1* n2*(1 - cosAngle) - n3 * sinAngle, n1* n3*(1 - cosAngle) + n2 * sinAngle ,0d},
                { n1* n2*(1 - cosAngle) + n3 * sinAngle, n2* n2*(1 - cosAngle) + cosAngle, n2* n3*(1 - cosAngle) - n1 * sinAngle,0d },
                { n1* n3*(1 - cosAngle) - n2 * sinAngle, n2* n3*(1 - cosAngle) + n1 * sinAngle, n3* n3*(1 - cosAngle) + cosAngle,0d },
                { 0d,0d,0d,1d }
            });
            TransformXYZ tempTranslate = Translation(rotationCenter);
            TransformXYZ tempTranslate_na = Translation(-rotationCenter);
            return tempTranslate * tempRo * tempTranslate_na;

        }
        //
        // 摘要:
        //     Create rotation transformation From Tait-Byran angles (also loosely known as
        //     Euler angles).
        //
        // 参数:
        //   yaw:
        //     Angle, in radians, to rotate about the Z axis.
        //
        //   pitch:
        //     Angle, in radians, to rotate about the Y axis.
        //
        //   roll:
        //     Angle, in radians, to rotate about the X axis.
        //
        // 返回结果:
        //     A transform matrix from Tait-Byran angles.
        //
        // 言论：
        //     RotationZYX(yaw, pitch, roll) = R_z(yaw) * R_y(pitch) * R_x(roll) where R_*(angle)
        //     is rotation of angle radians about the corresponding world coordinate axis.
        public static TransformXYZ RotationZYX(double yaw, double pitch, double roll)
        {
            TransformXYZ R_x = TransformXYZ.Rotation(roll, new XYZ(1, 0, 0), new XYZ(0, 0, 0));
            TransformXYZ R_y = TransformXYZ.Rotation(pitch, new XYZ(0, 1, 0), new XYZ(0, 0, 0));
            TransformXYZ R_z = TransformXYZ.Rotation(yaw, new XYZ(0, 0, 1), new XYZ(0, 0, 0));
            return R_z * R_y * R_x;

        }
        //
        // 摘要:
        //     Create rotation transformation From Euler angles.
        //
        // 参数:
        //   alpha:
        //     Angle, in radians, to rotate about the Z axis.
        //
        //   beta:
        //     Angle, in radians, to rotate about the Y axis.
        //
        //   gamma:
        //     Angle, in radians, to rotate about the X axis.
        //
        // 返回结果:
        //     A transform matrix from Euler angles.
        //
        // 言论：
        //     RotationZYZ(alpha, beta, gamma) = R_z(alpha) * R_y(beta) * R_z(gamma) where R_*(angle)
        //     is rotation of angle radians about the corresponding *-world coordinate axis.
        //     Note, alpha and gamma are in the range (-pi, pi] while beta in the range [0,
        //     pi]
        public static TransformXYZ RotationZYZ(double alpha, double beta, double gamma)
        {
            throw new NotImplementedException();
        }
        public static TransformXYZ Scale( double xScaleFactor, double yScaleFactor, double zScaleFactor) 
        {
            
            return Diagonal(xScaleFactor,yScaleFactor,zScaleFactor);
        }
        //
        // 摘要:
        //     Constructs a new non-uniform scaling transformation with a specified scaling
        //     anchor point.
        //
        // 参数:
        //   plane:
        //     Defines the center and orientation of the scaling operation.
        //
        //   xScaleFactor:
        //     Scaling factor along the anchor plane X-Axis direction.
        //
        //   yScaleFactor:
        //     Scaling factor along the anchor plane Y-Axis direction.
        //
        //   zScaleFactor:
        //     Scaling factor along the anchor plane Z-Axis direction.
        //
        // 返回结果:
        //     A transformation matrix which scales geometry non-uniformly.
        public static TransformXYZ Scale(PlaneXYZ plane, double xScaleFactor, double yScaleFactor, double zScaleFactor){throw new NotImplementedException();}
     
        /// <summary>
        /// 返回缩放转换矩阵
        /// </summary>
        /// <param name="anchor">缩放基点</param>
        /// <param name="scaleFactor">缩放比例</param>
        /// <returns></returns>
        public static TransformXYZ Scale(XYZ anchor, double scaleFactor)
        {
            //return Scale(new PlaneXYZ(anchor, new XYZ(1.0, 0.0, 0.0), new XYZ(0.0, 1.0, 0.0)), scaleFactor, scaleFactor, scaleFactor);
            //1基点平移至原点；2缩放，3平移原点到基点
            TransformXYZ py1 = Translation(-anchor);
            TransformXYZ sf = Scale(scaleFactor, scaleFactor, scaleFactor);
            TransformXYZ py2 = Translation(anchor);
            return py2 * sf * py1;

            return Scale(new PlaneXYZ(anchor, new XYZ(1.0, 0.0, 0.0), new XYZ(0.0, 1.0, 0.0)), scaleFactor, scaleFactor, scaleFactor);
        }
        //
        // 摘要:
        //     Constructs a Shear transformation.
        //
        // 参数:
        //   plane:
        //     Base plane for shear.
        //
        //   x:
        //     Shearing vector along plane x-axis.
        //
        //   y:
        //     Shearing vector along plane y-axis.
        //
        //   z:
        //     Shearing vector along plane z-axis.
        //
        // 返回结果:
        //     A transformation matrix which shear geometry.
        public static TransformXYZ Shear(PlaneXYZ plane, XYZ x, XYZ y, XYZ z){throw new NotImplementedException();}
        /// <summary>
        /// 返回平移转换矩阵
        /// </summary>
        /// <param name="dx">沿X轴平移距离</param>
        /// <param name="dy">沿Y轴平移距离</param>
        /// <param name="dz">沿Z轴平移距离</param>
        /// <returns></returns>
        public static TransformXYZ Translation(double dx, double dy, double dz)
        {
            return new TransformXYZ(new double[4, 4]
            {
                { 1d, 0d, 0d, dx },
                { 0d, 1d, 0d, dy },
                { 0d, 0d, 1d, dz },
                { 0d, 0d, 0d, 1d }
            });
           
        }
        /// <summary>
        /// 返回平移转换矩阵
        /// </summary>
        /// <param name="motion">平移向量</param>
        /// <returns></returns>
        public static TransformXYZ Translation(XYZ motion)
        {
            return new TransformXYZ(new double[4, 4]
            {
                { 1d, 0d, 0d, motion.X },
                { 0d, 1d, 0d, motion.Y },
                { 0d, 0d, 1d, motion.Z },
                { 0d, 0d, 0d, 1d }
            });
        }
        /// <summary>
        /// Get the translation, scaling
        /// </summary>
        /// <param name="scaling">The 3x3 sub-matrix</param>
        /// <param name="translation">The last column of the matrix (the last element being the divisor which is almost always 1)</param>
        public void Deconstruct(out double[] scaling, out double[] translation)
        {
            scaling = this.scaling;
            translation = this.translation;
        }

        /// <summary>
        /// Transform a flat list of doubles representing points
        /// </summary>
        public List<double> ApplyToPoints(List<double> points)
        {
            if (points.Count % 3 != 0)
                throw new Exception(
                  $"Cannot apply transform as the points list is malformed: expected length to be multiple of 3");
            var transformed = new List<double>(points.Count);
            for (var i = 0; i < points.Count; i += 3)
                transformed.AddRange(ApplyToPoint(new List<double>(3) { points[i], points[i + 1], points[i + 2] }));

            return transformed;
        }

        /// <summary>
        /// Transform a flat list of speckle Points
        /// </summary>
        public List<XYZ> ApplyToPoints(List<XYZ> points)
        {
            var transformed = new List<XYZ>(points.Count);
            for (var i = 0; i < points.Count; i++)
                transformed.Add(ApplyToPoint(points[i]));

            return transformed;
        }
        public List<UV> ApplyToPoints(List<UV> points)
        {
            var transformed = new List<UV>(points.Count);
            for (var i = 0; i < points.Count; i++)
                transformed.Add(ApplyToUV(points[i]));

            return transformed;
        }

        /// <summary>
        /// Transform a single speckle Point
        /// </summary>
        public XYZ ApplyToPoint(XYZ point)
        {
            var (x, y, z) = point;
            var newCoords = ApplyToPoint(new List<double> { x, y, z });
            return new XYZ(newCoords[0], newCoords[1], newCoords[2]);
        }
        /// <summary>
        /// Transform a single speckle Point
        /// </summary>
        public UV ApplyToUV(UV point)
        {
            var (x, y) = point;
            var newCoords = ApplyToUV(new List<double> { x, y});
            return new UV(newCoords[0], newCoords[1]);
        }
        /// <summary>
        /// Transform a list of three doubles representing a point
        /// </summary>
        public List<double> ApplyToPoint(List<double> point)
        {
            var transformed = new List<double>();
            for (var i = 0; i < 16; i += 4)
                transformed.Add(point[0] * value[i] + point[1] * value[i + 1] + point[2] * value[i + 2] + value[i + 3]);

            return new List<double>(3)
        {transformed[ 0 ] / transformed[ 3 ], transformed[ 1 ] / transformed[ 3 ], transformed[ 2 ] / transformed[ 3 ]};
        }
        /// <summary>
        /// Transform a list of three doubles representing a point
        /// </summary>
        public List<double> ApplyToUV(List<double> point)
        {
            var transformed = new List<double>();
            for (var i = 0; i < 16; i += 4)
                transformed.Add(point[0] * value[i] + point[1] * value[i + 1]  + value[i + 3]);

            return new List<double>(2)
        {transformed[ 0 ] / transformed[ 3 ], transformed[ 1 ] / transformed[ 3 ]};
        }
        /// <summary>
        /// Transform a single speckle Vector
        /// </summary>
        public XYZ ApplyToVector(XYZ vector)
        {
            var newCoords = ApplyToVector(new List<double> { vector.X, vector.Y, vector.Z });

            return new Geometry.XYZ(newCoords[0], newCoords[1], newCoords[2]);
        }

        /// <summary>
        /// Transform a list of three doubles representing a vector
        /// </summary>
        public List<double> ApplyToVector(List<double> vector)
        {
            var newPoint = new List<double>(4) { vector[0], vector[1], vector[2] };
            for (var i = 0; i < 16; i += 4)
                newPoint[i / 4] = newPoint[0] * value[i] + newPoint[1] * value[i + 1] +
                                    newPoint[2] * value[i + 2];

            return new List<double>(3)
        {newPoint[ 0 ], newPoint[ 1 ], newPoint[ 2 ]};
        }

        /// <summary>
        /// Transform a flat list of ICurves. Note that if any of the ICurves does not implement `ITransformable`,
        /// it will not be returned.
        /// </summary>
        public List<ICurve> ApplyToCurves(List<ICurve> curves, out bool success)
        {
            success = true;
            var transformed = new List<ICurve>();
            foreach (var curve in curves)
            {
                if (curve is ITransformable c)
                {
                    c.TransformTo(this, out ITransformable tc);
                    transformed.Add((ICurve)tc);
                }
                else
                    success = false;
            }
            return transformed;
        }
        public List<ICurveXYZ> ApplyToCurves(List<ICurveXYZ> curves, out bool success)
        {
            success = true;
            var transformed = new List<ICurveXYZ>();
            foreach (var curve in curves)
            {
                if (curve is ITransformable c)
                {
                    c.TransformTo(this, out ITransformable tc);
                    transformed.Add((ICurveXYZ)tc);
                }
                else
                    success = false;
            }
            return transformed;
        }
        public List<ICurveUV> ApplyToCurves(List<ICurveUV> curves, out bool success)
        {
            success = true;
            var transformed = new List<ICurveUV>();
            foreach (var curve in curves)
            {
                if (curve is ITransformable c)
                {
                    c.TransformTo(this, out ITransformable tc);
                    transformed.Add((ICurveUV)tc);
                }
                else
                    success = false;
            }
            return transformed;
        }
        public static TransformXYZ Multiply(TransformXYZ a, TransformXYZ b)
        {
            return a * b;
        }
        /// <summary>
        /// Multiplies two transform matrices together
        /// </summary>
        /// <param name="a">The first source transform</param>
        /// <param name="b">The second source transform</param>
        /// <returns></returns>
        public static TransformXYZ operator *(TransformXYZ a, TransformXYZ b)
        {
            TransformXYZ result = new TransformXYZ();
            result.M00 = a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20 + a.M03 * b.M30;
            result.M01 = a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21 + a.M03 * b.M31;
            result.M02 = a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22 + a.M03 * b.M32;
            result.M03 = a.M00 * b.M03 + a.M01 * b.M13 + a.M02 * b.M23 + a.M03 * b.M33;
            result.M10 = a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20 + a.M13 * b.M30;
            result.M11 = a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            result.M12 = a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            result.M13 = a.M10 * b.M03 + a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
            result.M20 = a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20 + a.M23 * b.M30;
            result.M21 = a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            result.M22 = a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            result.M23 = a.M20 * b.M03 + a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
            result.M30 = a.M30 * b.M00 + a.M31 * b.M10 + a.M32 * b.M20 + a.M33 * b.M30;
            result.M31 = a.M30 * b.M01 + a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            result.M32 = a.M30 * b.M02 + a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            result.M33 = a.M30 * b.M03 + a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
            return result;
        }
        //
        // 摘要:
        //     Multiplies a transformation by a point and gets a new point.
        //
        // 参数:
        //   m:
        //     A transformation.
        //
        //   p:
        //     A point.
        //
        // 返回结果:
        //     The transformed point.
        //
        // 言论：
        //     Note well: The right hand column and bottom row have an important effect when
        //     transforming a Euclidean point and have no effect when transforming a vector.
        //     Be sure you understand the differences between vectors and points when applying
        //     a 4x4 transformation.
        public static XYZ operator *(TransformXYZ transform, XYZ point) 
        {
            var newPoint = new List<double>(4) { point[0], point[1], point[2] ,1};
            for (var i = 0; i < 16; i += 4)
                newPoint[i / 4] = newPoint[0] * transform.value[i] + newPoint[1] * transform.value[i + 1] +
                                    newPoint[2] * transform.value[i + 2] + newPoint[3] * transform.value[i + 3];
           

            return new XYZ(newPoint[0], newPoint[1], newPoint[2]);
        }
        public string ToString(string format="0.000", IFormatProvider provider=null)
        {
            string result = "";
            for (var i = 0; i < 16; i += 4)
            {
                result += value[i].ToString(format,provider) +","+value[i + 1].ToString(format, provider) + "," + value[i + 2].ToString(format, provider) + "," + value[i + 3].ToString(format, provider) + "\n";
            }
            return result;
        }
        
    }

    static class ArrayUtils
    {
        // create a subset from a specific list of indices
        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            var subset = new T[indices.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }

            return subset;
        }
        public static T[] Flatten<T> (this T[,] array)
        {
            int rowcount = array.GetLength(0);
            int columncount = array.GetLength(1);
            T[] result = new T[rowcount*columncount];
            for(int i=0;i<rowcount;i++)
            {
                for (int j = 0; j < columncount; j++)
                {
                    result[i * columncount + j] = array[i, j];
                }
            }
            return result;
        }
        public static T[,] ReSize<T>(this T[] array,int rowCount,int columnCount)
        {
            if (array.Length!=rowCount*columnCount)
            {
                throw new Exception("行数*列数与数组元素个数不一致");
            }
            T[,] result = new T[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    result[i, j] = array[i * rowCount + j];
                }
            }
            return result;
        }
        public static T[] RowAt<T>(this T[,] array,int index)
        {
            if (index> array.GetLength(0)-1 || index<0)
            {
                throw new Exception("index超出行数限制");
            }
            T[] result = new T[array.GetLength(1)];
            for(int i = 0; i < array.GetLength(1); i++)
            {
                result[i] = array[index, i];
            }
            return result;
        }
        public static T[] ColunnAt<T>(this T[,] array, int index)
        {
            if (index > array.GetLength(1) - 1 || index < 0)
            {
                throw new Exception("index超出列数限制");
            }
            T[] result = new T[array.GetLength(0)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                result[i] = array[i, index];
            }
            return result;
        }
    }
}
