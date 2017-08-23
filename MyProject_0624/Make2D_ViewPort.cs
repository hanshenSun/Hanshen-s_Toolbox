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
            pManager.AddGeometryParameter("Solid Crv", "Solid", "Solid Crv", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Hidden Crv", "Hidden", "Hidden Crv", GH_ParamAccess.list);
            pManager.AddTextParameter("Message", "Message", "Message", GH_ParamAccess.list);
            pManager.AddBrepParameter("B", "B", "B", GH_ParamAccess.list);
            pManager.AddPointParameter("Cam", "Cam", "Cam", GH_ParamAccess.item);
            pManager.AddPointParameter("corner", "corner", "corner", GH_ParamAccess.list);


        }





        private static List<Curve> makeTwoD(List<Brep> inputGeos, Point3d CameraPt, Plane TargetPlane ,bool hiddenlnBool, ref List<string> message, ref List<Curve> hiddenSegment, ref List<Brep> breps)
        {
            List<Curve> flattenedCurves = new List<Curve>();
            List<Curve> solidSegment = new List<Curve>();
            //List<Brep> breps = new List<Brep>();
            //List<Curve> hiddenSegment = new List<Curve>();

            //List<string> message = new List<string>();

            int brepIndex = inputGeos.Count;



            for (int index = 0; index <brepIndex; index++)
            {
                Brep inputGeo = inputGeos[index];
                foreach (Curve brepEdge in inputGeo.Edges)
                {
                    NurbsCurve singleEdge = brepEdge.ToNurbsCurve();//getting single edgeCurve in each brep
                    List<Point3d> ptAloneCurve = new List<Point3d>();

                    

                    foreach (ControlPoint edgeControlPt in singleEdge.Points)
                    {
                        ptAloneCurve.Add(edgeControlPt.Location);//getting point location of each edgecurve
                    }




                    for (int i = 0; i < ptAloneCurve.Count - 1; i++)
                    {
                        Line tempLn = new Line(ptAloneCurve[i], ptAloneCurve[i + 1]);//construct line segments

                        Curve tempProjectCrvA = new LineCurve(ptAloneCurve[i], CameraPt);
                        Curve tempProjectCrvB = new LineCurve(ptAloneCurve[i + 1], CameraPt);
                        

                        Line edgeProjectLnA = new Line(ptAloneCurve[i], CameraPt);
                        Line edgeProjectLnB = new Line(ptAloneCurve[i + 1], CameraPt);//getting the projection line of the two ends of the line segments


                        double intersectionParameterA;
                        double intersectionParameterB;


                        Rhino.Geometry.Intersect.Intersection.LinePlane(edgeProjectLnA, TargetPlane, out intersectionParameterA);
                        Rhino.Geometry.Intersect.Intersection.LinePlane(edgeProjectLnB, TargetPlane, out intersectionParameterB);


                        Point3d intersectPtA = edgeProjectLnA.PointAt(intersectionParameterA);
                        Point3d intersectPtB = edgeProjectLnB.PointAt(intersectionParameterB);

                        LineCurve projectedSegment = new LineCurve(intersectPtA, intersectPtB);//got projected line segments of each edge


                        //flattenedCurves.Add(projectedSegment);
                        /*
                        Curve[] dummyCrvsA;
                        Point3d[] dummyPtsA;
                        Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrvA, inputGeo, 0.01, out dummyCrvsA, out dummyPtsA);

                        Curve[] dummyCrvsB;
                        Point3d[] dummyPtsB;
                        Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrvB, inputGeo, 0.01, out dummyCrvsB, out dummyPtsB);

                        bool caseAbool = false;
                      

                        if (dummyPtsA.Length >1 || dummyPtsB.Length > 1)
                        {
                            caseAbool = true;
                        }
                        




                        //Case A_Total Hidden
                        if (caseAbool==true)
                        {
                            //any of the two end intersections got intersect
                            //therefore its total hidden
                            hiddenSegment.Add(projectedSegment);
                        }

                        else if (dummyPtsA.Length ==1 && dummyPtsB.Length == 1)
                        {
                            //case C_Total Solid
                            solidSegment.Add(projectedSegment);

                        }
                        //Case B
                        else
                        {
                            Surface triangleSrf = NurbsSurface.CreateFromCorners(ptAloneCurve[i], ptAloneCurve[i + 1], CameraPt);//created the triangle surface


                            List<NurbsCurve> intersectionCrvs = new List<NurbsCurve>();
                            //List<Point3d> intersectionPts = new List<Point3d>();
                            List<bool> intersectionResults = new List<bool>();



                            //bool caseBbool = false;//getting result fromm all breps


                            for (int ii = 0; ii<brepIndex && ii !=i ; ii++)
                            //foreach (Brep tempInputGeo in inputGeos)
                            {

                                Brep tempInputGeo = inputGeos[ii];
                                Curve[] dumIntCrv;
                                Point3d[] dumIntPt;


                                if (Rhino.Geometry.Intersect.Intersection.BrepSurface(tempInputGeo, triangleSrf, 0.01, out dumIntCrv, out dumIntPt))//has intersection
                                {
                                    //CASE 2, partial intersection
                                    intersectionResults.Add(true);
                                    Curve[] joinedDumCrvs = Curve.JoinCurves(dumIntCrv);
                                    //caseBbool = true;




                                    foreach (Curve joinedSingleCrv in joinedDumCrvs)//multiple curve per intersection
                                    {
                                        NurbsCurve nurbDumCrv = joinedSingleCrv.ToNurbsCurve();
                                        intersectionCrvs.Add(nurbDumCrv);
                                    }
                                }
                            }

                            /*
                            if (caseBbool == false)
                            {
                                //entering CASE C
                                solidSegment.Add(projectedSegment);
                            }
                            */


                            //entering CASE B

                            //DETAILING CASE 2
                            //List<Line> blockLns = new List<Line>();//this contains all blocklns from this single intersection with single brep

                            List<Brep> extrudedTri = new List<Brep>();


                            foreach (NurbsCurve intCrv in intersectionCrvs)//looping through ALL intersection of this edge triangle
                            {
                                List<double> distanceToEdgeA = new List<double>();
                                List<double> distanceToEdgeB = new List<double>();
                                List<Point3d> controlPtLocation = new List<Point3d>();



                                foreach (ControlPoint dumControlPt in intCrv.Points)//geting multiple control pts from each single 
                                {
                                    //double distancA;
                                    //double distancB;
                                    Point3d tempDumLocation = dumControlPt.Location;
                                    Point3d clostPointOnCrvA = edgeProjectLnA.ClosestPoint(tempDumLocation, false);
                                    Point3d clostPointOnCrvB = edgeProjectLnB.ClosestPoint(tempDumLocation, false);

                                    distanceToEdgeA.Add(clostPointOnCrvA.DistanceTo(tempDumLocation));
                                    distanceToEdgeB.Add(clostPointOnCrvB.DistanceTo(tempDumLocation));
                                    controlPtLocation.Add(tempDumLocation);
                                }

                                double[] newDistanceEdgeA = distanceToEdgeA.ToArray();
                                double[] newDistanceEdgeB = distanceToEdgeB.ToArray();
                                Point3d[] newControlPtLocationA = controlPtLocation.ToArray();
                                Point3d[] newControlPtLocationB = controlPtLocation.ToArray();


                                Array.Sort(newDistanceEdgeA, newControlPtLocationA);
                                Point3d closestPtA = newControlPtLocationA[0];

                                Array.Sort(newDistanceEdgeB, newControlPtLocationB);
                                Point3d closestPtB = newControlPtLocationB[0];
                                //Line blockLnA = new Line(closestPtA, closestPtB);
                                Line blockLnA = new Line(CameraPt, closestPtA);
                                Line blockLnB = new Line(CameraPt, closestPtB);//these two curves compose the TRIANGLE

                                double blockExtParamA;
                                double blockExtParamB;

                                Rhino.Geometry.Intersect.Intersection.LinePlane(blockLnA, TargetPlane, out blockExtParamA);
                                Rhino.Geometry.Intersect.Intersection.LinePlane(blockLnB, TargetPlane, out blockExtParamB);

                                Point3d blockPtA = blockLnA.PointAt(blockExtParamA);
                                Point3d blockPtB = blockLnB.PointAt(blockExtParamB);//TRIANGLE endpoints

                                //Surface blockSrfOnPlane = NurbsSurface.CreateFromCorners(blockPtA, blockPtB, CameraPt);
                                //blockSrfOnPlane.CreateExtrusion(curve, direction);


                                //Curve[] splitedCrvs = projectedSegment.Split(blockSrfOnPlane, 0.0);

                                Point3d[] blockBasePt = new Point3d[3] { blockPtA, blockPtB, CameraPt };
                                Plane tempPlane;
                                Plane.FitPlaneToPoints(blockBasePt, out tempPlane);
                                Vector3d extrudeZ = Vector3d.Multiply(2.0, tempPlane.ZAxis);


                                //PolylineCurve blockBaseCrv = new PolylineCurve(blockBasePt);
                                //bool closedResult = blockBaseCrv.MakeClosed(0.1);

                                NurbsCurve blockBaseCrv = NurbsCurve.Create(true, 1, blockBasePt);
                                Surface blockSrfOnPlane = Surface.CreateExtrusion(blockBaseCrv, extrudeZ);


                                Brep blockSrfBrep = Brep.CreateFromSurface(blockSrfOnPlane);
                                Brep blockBrepSolid = blockSrfBrep.CapPlanarHoles(0.1);



                                //Extrusion blockExtrusion = Extrusion.Create(blockBaseCrv, 10.0, true);
                                //Brep blockBrep = blockExtrusion.ToBrep();//got SINGLE triangle srf extrusion
                                extrudedTri.Add(blockBrepSolid);
                                breps.Add(blockBrepSolid);
                            }



                            Brep[] joinedBrep = Brep.CreateBooleanUnion(extrudedTri, 0.1);


                            List<Point3d> brepLnIntersectPt = new List<Point3d>();


                            flattenedCurves.Add(projectedSegment);////



                            foreach (Brep singleBrep in joinedBrep)
                            {
                                Curve[] overlapBrepCrvs;
                                Point3d[] overlapBrepPts;




                                Rhino.Geometry.Intersect.Intersection.CurveBrep(projectedSegment, singleBrep, 0.01, out overlapBrepCrvs, out overlapBrepPts);

                                int ptCount = overlapBrepPts.Length;
                                int crvCount = overlapBrepCrvs.Length;

                                foreach (Point3d pt in overlapBrepPts)
                                {
                                    brepLnIntersectPt.Add(pt);
                                }
                            }


                            //so far we should got all the intersection pt on the projected segments 
                            Point3d[] uniquePts = Point3d.CullDuplicates(brepLnIntersectPt, 0.0);
                            List<double> uniParams = new List<double>();
                            /*

                            foreach (Point3d uniPt in uniquePts)
                            {
                                double uniParam;
                                projectedSegment.ClosestPoint(uniPt, out uniParam);
                                uniParams.Add(uniParam);
                            }

                            Curve[] splitedCrvs = projectedSegment.Split(uniParams);

                            //got splited curves
                            //ready to sort the inside or outside segments




                            foreach (Curve splitedSingle in splitedCrvs)
                            {
                                double splitMidParam = splitedSingle.Domain.Mid;//SINGLE midpt on each segment
                                Point3d splitMidPt = splitedSingle.PointAt(splitMidParam);

                                List<bool> containResult = new List<bool>();


                                foreach (Brep singleBrep in joinedBrep)
                                {
                                    if (singleBrep.IsPointInside(splitMidPt, 0.0, true))
                                    {
                                        containResult.Add(true);
                                    }
                                }

                                if (containResult.Contains(true))
                                {
                                    //this segment is included in some breps
                                    hiddenSegment.Add(projectedSegment);

                                }
                                else
                                {
                                    solidSegment.Add(projectedSegment);
                                }
                            }
                            */



                        }
                           
                    }
                }
            }
        










           return solidSegment;
           //return flattenedCurves;
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
            
            List<string> outputMessage = new List<string>();

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
            


            List<Curve> solidLines = new List<Curve>();
            List<Curve> dashLines = new List<Curve>();
            List<Brep> brepss = new List<Brep>();

            solidLines = makeTwoD(inputBreps, cameraPt, intersectionPlane, hiddenBool, ref outputMessage, ref dashLines, ref brepss);

            DA.SetDataList(0, solidLines);
            DA.SetDataList(1, dashLines);
            //DA.SetDataList(3, unflattenedCrv);
            DA.SetDataList(2, outputMessage);
            DA.SetDataList(3, brepss);
            DA.SetData(4, cameraPt);
            DA.SetDataList(5, cornerPts);




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