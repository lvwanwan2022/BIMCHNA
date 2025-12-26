
using System;
using System.Collections.Generic;
using System.Text;

namespace Lv.BIM.Geometry
{
  public class EllipseXYZ : Base, ICurve, IHasArea
  {
    /// <summary>
    /// Gets or sets the first radius of the <see cref="Ellipse"/>. This is usually the major radius.
    /// </summary>
    public double firstRadius { get; set; }

    /// <summary>
    /// Gets or sets the second radius of the <see cref="Ellipse"/>. This is usually the minor radius.
    /// </summary>
    public double secondRadius { get; set; }

    /// <summary>
    /// Gets or sets the plane to draw this ellipse in.
    /// </summary>
    public PlaneXYZ plane { get; set; }

    /// <summary>
    /// Gets or sets the domain interval for this <see cref="Ellipse"/>.
    /// </summary>
    public Interval domain { get; set; }

    /// <summary>
    /// Gets or set the domain interval to trim this <see cref="Ellipse"/> with.
    /// </summary>
    public Interval trimDomain { get; set; }

    /// <inheritdoc />
    public Box bbox { get; set; }

    /// <inheritdoc />
    //public XYZ center { get; set; }

    /// <inheritdoc />
    public double Area { get;  }

    /// <inheritdoc />
    public double Length { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> class.
    /// This constructor is only intended for serialization/deserialization purposes.
    /// Use other constructors to manually create ellipses.
    /// </summary>
    public EllipseXYZ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> class.
    /// </summary>
    /// <param name="plane">The plane to draw the ellipse in.</param>
    /// <param name="radius1">First radius of the ellipse.</param>
    /// <param name="radius2">Second radius of the ellipse.</param>
    /// <param name="applicationId">Application ID, defaults to null.</param>
    public EllipseXYZ(PlaneXYZ plane, double radius1, double radius2)
      : this(plane, radius1, radius2, new Interval(0, 1), null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> class.
    /// </summary>
    /// <param name="plane">The plane to draw the ellipse in.</param>
    /// <param name="radius1">First radius of the ellipse.</param>
    /// <param name="radius2">Second radius of the ellipse.</param>
    /// <param name="domain">The curve's internal parametrization domain.</param>   
    /// <param name="trimDomain">The domain to trim the curve with. Will be null if the ellipse is not trimmed.</param>
    /// <param name="applicationId">Application ID, defaults to null.</param>
    public EllipseXYZ(PlaneXYZ plane, double radius1, double radius2, Interval domain, Interval trimDomain)
    {
      this.plane = plane;
      this.firstRadius = radius1;
      this.secondRadius = radius2;
      this.domain = domain;
      this.trimDomain = trimDomain;
    }

    public List<double> ToList()
    {
      var list = new List<double>();
      list.Add(firstRadius );
      list.Add(secondRadius );
      list.Add(domain.Start );
      list.Add(domain.End );

      list.AddRange(plane.ToList());

      list.Insert(0, CurveTypeEncoding.Ellipse);
      list.Insert(0, list.Count);
      return list;
    }

    public static EllipseXYZ FromList(List<double> list)
    {
      var ellipse = new EllipseXYZ();

      ellipse.firstRadius = list[2];
      ellipse.secondRadius = list[3];
      ellipse.domain = new Interval(list[4], list[5]);

      ellipse.plane = PlaneXYZ.FromList(list.GetRange(6, 13));
      return ellipse;
    }
  }
    public class EllipseUV : Base, ICurve, IHasArea
    {
        /// <summary>
        /// Gets or sets the first radius of the <see cref="Ellipse"/>. This is usually the major radius.
        /// </summary>
        public double firstRadius { get; set; }

        /// <summary>
        /// Gets or sets the second radius of the <see cref="Ellipse"/>. This is usually the minor radius.
        /// </summary>
        public double secondRadius { get; set; }

        

        /// <summary>
        /// Gets or sets the domain interval for this <see cref="Ellipse"/>.
        /// </summary>
        public Interval domain { get; set; }

        /// <summary>
        /// Gets or set the domain interval to trim this <see cref="Ellipse"/> with.
        /// </summary>
        public Interval trimDomain { get; set; }

        /// <inheritdoc />
        public Box bbox { get; set; }

        /// <inheritdoc />
        //public UV center { get; set; }

        /// <inheritdoc />
        public double Area { get; }

        /// <inheritdoc />
        public double Length { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> class.
        /// This constructor is only intended for serialization/deserialization purposes.
        /// Use other constructors to manually create ellipses.
        /// </summary>
        public EllipseUV()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> class.
        /// </summary>
        /// <param name="plane">The plane to draw the ellipse in.</param>
        /// <param name="radius1">First radius of the ellipse.</param>
        /// <param name="radius2">Second radius of the ellipse.</param>
        /// <param name="applicationId">Application ID, defaults to null.</param>
        public EllipseUV( double radius1, double radius2)
          : this(radius1, radius2, new Interval(0, 1), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> class.
        /// </summary>
        /// <param name="plane">The plane to draw the ellipse in.</param>
        /// <param name="radius1">First radius of the ellipse.</param>
        /// <param name="radius2">Second radius of the ellipse.</param>
        /// <param name="domain">The curve's internal parametrization domain.</param>   
        /// <param name="trimDomain">The domain to trim the curve with. Will be null if the ellipse is not trimmed.</param>
        /// <param name="applicationId">Application ID, defaults to null.</param>
        public EllipseUV( double radius1, double radius2, Interval domain, Interval trimDomain)
        {
    
            this.firstRadius = radius1;
            this.secondRadius = radius2;
            this.domain = domain;
            this.trimDomain = trimDomain;
        }

        public List<double> ToList()
        {
            var list = new List<double>();
            list.Add(firstRadius);
            list.Add(secondRadius);
            list.Add(domain.Start);
            list.Add(domain.End);


            list.Insert(0, CurveTypeEncoding.Ellipse);
            list.Insert(0, list.Count);
            return list;
        }

        public static EllipseUV FromList(List<double> list)
        {
            var ellipse = new EllipseUV();

            ellipse.firstRadius = list[2];
            ellipse.secondRadius = list[3];
            ellipse.domain = new Interval(list[4], list[5]);
            return ellipse;
        }
    }
}