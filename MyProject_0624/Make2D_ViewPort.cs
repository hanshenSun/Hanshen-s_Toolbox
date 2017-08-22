using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.Display;

namespace MyProject_0624
{
    public class Make2D_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Make2D_ViewPort class.
        /// </summary>
        public Make2D_ViewPort()
          : base("Make2D_ViewPort", "Make2D_ViewPort",
              "Make2d of current viewport",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry for Making2D", "Geometry", "List of geometries for making 2d", GH_ParamAccess.list);
            //pManager.AddPointParameter("Camera Location", "Camera", "Camera Pt",GH_ParamAccess.item);
            pManager.AddBooleanParameter("start?", "start?", "start?", GH_ParamAccess.item);
            pManager.AddBooleanParameter("hiddenLine?", "hidden?", "show hidden line?", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddLineParameter("Output 2D Curves", "Curves", "Flattened 2D Curves", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Crv", "Crv", "Crv", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Focus Plane", "Plane", "Plane", GH_ParamAccess.item);
            pManager.AddPointParameter("pt", "pt", "pt", GH_ParamAccess.list);
            
        }





        private static List<PolylineCurve> makeTwoD(List<Brep> inputGeos, Point3d CameraPt, Plane TargetPlane ,bool hiddenlnBool)
        {
            List<PolylineCurve> flattenedCurves = new List<PolylineCurve>();
            foreach(Brep inputGeo in inputGeos)
            {
                foreach (Curve brepEdge in inputGeo.Edges)
                {
                    NurbsCurve singleEdge = brepEdge.ToNurbsCurve();//getting single edgeCurve in each brep
                    List<Point3d> ptAloneCurve = new List<Point3d>();

                    List<Line> solidSegment = new List<Line>();
                    List<Line> hiddenSegment = new List<Line>();


                    foreach (ControlPoint edgeControlPt in singleEdge.Points)
                    {
                        ptAloneCurve.Add(edgeControlPt.Location);//gettomg point location of each edgecurve
                    }



                    
                    for (int i = 0; i < ptAloneCurve.Count-1; i++)
                    {
                        Line tempLn = new Line(ptAloneCurve[i], ptAloneCurve[i + 1]);//construct line segments

                        Line tempProjectLnA = new Line(ptAloneCurve[i], CameraPt);
                        Line tempProjectLnB = new Line(ptAloneCurve[i], CameraPt);//getting the projection line of the two ends of the line segments

                        
                        double intersectionParameterA;
                        double intersectionParameterB;


                        Rhino.Geometry.Intersect.Intersection.LinePlane(tempProjectLnA, TargetPlane, out intersectionParameterA);
                        Rhino.Geometry.Intersect.Intersection.LinePlane(tempProjectLnB, TargetPlane, out intersectionParameterB);


                        Point3d intersectPtA = tempProjectLnA.PointAt(intersectionParameterA);
                        Point3d intersectPtB = tempProjectLnB.PointAt(intersectionParameterB);

                        Line projectedSegment = new Line(intersectPtA, intersectPtB);//got projected line segments of each edge

                        Curve tempProjectCrvA = new LineCurve(tempProjectLnA);
                        Curve tempProjectCrvB = new LineCurve(tempProjectLnB);
                        Curve[] dummyCrvs;
                        Point3d[] dummyPts;

                        //Case 1_Total Hidden
                        if (Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrvA, inputGeo, 0.0, out dummyCrvs, out dummyPts)|| Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrvB, inputGeo, 0.0, out dummyCrvs, out dummyPts))
                        {
                            //any of the two end intersections got intersect
                            //therefore its total hidden
                            hiddenSegment.Add(projectedSegment);
                        }

                        //Case 2/3
                        else
                        {
                            Surface triangleSrf = NurbsSurface.CreateFromCorners(ptAloneCurve[i], ptAloneCurve[i + 1], CameraPt);

                            foreach (Brep tempInputGeo in inputGeos)
                            {
                                /////if ////
                            }


                        }
                        




                        Curve[] intersectionCrv;
                        Point3d[] intersectionPt;
                        if (Rhino.Geometry.Intersect.Intersection.CurveBrep(tempLnCrv, inputGeo, 0.0, out intersectionCrv, out intersectionPt))
                        {
                            //that means this is hidden curve
                            hiddenSegment.Add(projectedSegment);
                        }
                        else
                        {
                            solidSegment.Add(projectedSegment);
                        }
                    }




                    //NurbsCurve projectedCrv = NurbsCurve.Create(false, 3, ptAloneCurve);





                    PolylineCurve projectedCrv = new PolylineCurve(ptAloneCurve);

                    flattenedCurves.Add(projectedCrv);
                }
            
                

            }
            return flattenedCurves;
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> inputBreps = new List<Brep>();
            bool startBool = false;
            bool hiddenBool = true;
            List<Curve> outputCrv = new List<Curve>();
            List<PolylineCurve> unflattenedCrv = new List<PolylineCurve>();

            DA.GetDataList(0, inputBreps);
            DA.GetData(1, ref startBool);
            DA.GetData(2, ref hiddenBool);


            RhinoViewport vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            Point3d cameraPt = vp.CameraLocation;
            Point3d targetPt = vp.CameraTarget;
            double focusLength = vp.Camera35mmLensLength;
            
            
            Point3d[] cornerPts = vp.GetFarRect();
            Plane intersectionPlane = new Plane(cornerPts[0], cornerPts[1], cornerPts[2]);


            double averageX = (cornerPts[0].X + cornerPts[1].X + cornerPts[2].X + cornerPts[3].X) / 4;
            double averageY = (cornerPts[0].Y + cornerPts[1].Y + cornerPts[2].Y + cornerPts[3].Y) / 4;
            double averageZ = (cornerPts[0].Z + cornerPts[1].Z + cornerPts[2].Z + cornerPts[3].Z) / 4;


            Point3d planeCenterPt = new Point3d(averageX, averageY, averageZ);
            Vector3d planeMoveVector = Point3d.Subtract(planeCenterPt, cornerPts[0]);

            intersectionPlane.Translate(planeMoveVector);





            List<Point3d> corners = new List<Point3d>();
             

            

            unflattenedCrv = makeTwoD(inputBreps, cameraPt, intersectionPlane, hiddenBool);

            DA.SetDataList(0, unflattenedCrv);
            DA.SetData(1, intersectionPlane);
            DA.SetDataList(2, cornerPts);
            //DA.SetDataList(3, unflattenedCrv);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4e39cbdf-85dd-4d21-80f7-26594dcbcaf4"); }
        }
    }
}