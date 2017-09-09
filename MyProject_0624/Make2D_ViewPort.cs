using System;
using System.Collections.Generic;
using System.Linq;

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
            pManager[1].Optional = true;
            pManager[2].Optional = true;
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
            pManager.AddCurveParameter("c", "c", "c", GH_ParamAccess.list);


        }





        private static void makeTwoD(List<Brep> inputGeos, Point3d CameraPt, Plane TargetPlane ,bool hiddenlnBool, double tol, ref List<string> message, ref List<Curve> hiddenSegment_All, ref List<Curve> solidSegment_All, ref List<Brep> breps, ref List<Point3d> curveMid, ref List<Curve> outputPoly)
        {
            
            List<Curve> solidSegment = new List<Curve>();
            List<Curve> hiddenSegment = new List<Curve>();


            int brepIndex = inputGeos.Count;



            for (int currentIndex = 0; currentIndex < brepIndex; currentIndex++)
            {
                Brep inputGeo = inputGeos[currentIndex];
                foreach (Curve brepEdge in inputGeo.Edges)
                {
                    NurbsCurve singleEdge = brepEdge.ToNurbsCurve();//getting single edgeCurve in each brep



                    List<Point3d> ptAloneCurve = new List<Point3d>();
                    List<Point3d> ptAloneCurveProjected = new List<Point3d>();
                    //List<Line> edgeProjectLnSS = new List<Line>();
                    List<bool> curveBrepIntersect = new List<bool>();

                    int controlPtCount = singleEdge.Points.Count();
                    int nurbsCurveDegree = singleEdge.Degree;

                    foreach (ControlPoint edgeControlPt in singleEdge.Points)
                    {
                        Point3d tempControlPtLocation = edgeControlPt.Location;
                        
                        Line edgeProjectLn = new Line(tempControlPtLocation, CameraPt);
                        //getting the projection line of the two ends of the line segments
                        
                        double intersectionParameter;
                        Rhino.Geometry.Intersect.Intersection.LinePlane(edgeProjectLn, TargetPlane, out intersectionParameter);



                        Point3d intersectPt = edgeProjectLn.PointAt(intersectionParameter);
                        ptAloneCurve.Add(tempControlPtLocation);//getting point location of each edgecurve//////////////////
                        ptAloneCurveProjected.Add(intersectPt);

                        

                        Vector3d projectVector = Point3d.Subtract(tempControlPtLocation, CameraPt);
                        double distance = CameraPt.DistanceTo(tempControlPtLocation);
                        Line tempProjectLn = new Line(CameraPt, projectVector, distance * 0.99);
                        Curve tempProjectCrv = new LineCurve(tempProjectLn);
                        

                        Curve[] dummyCrvsA;
                        Point3d[] dummyPtsA;
                        Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrv, inputGeo, 0.01, out dummyCrvsA, out dummyPtsA);

                        if (dummyPtsA.Length > 0)
                        {
                            curveBrepIntersect.Add(true);
                        }
                    }

                    
                    Curve projectedCurve = NurbsCurve.Create(false, nurbsCurveDegree, ptAloneCurveProjected);
                    Curve projectedCurveOrigin = NurbsCurve.Create(false, nurbsCurveDegree, ptAloneCurve);

                    //Point3d projectedMid = projectedCurve.PointAt(projectedCurve.Domain.Mid);
                    Point3d originEdgeStart = singleEdge.PointAt(singleEdge.Domain.Min);
                    Point3d originEdgeEnd = singleEdge.PointAt(singleEdge.Domain.Max);
                    double distanceToEdgeStart = CameraPt.DistanceTo(originEdgeStart);
                    double distanceToEdgeEnd = CameraPt.DistanceTo(originEdgeEnd);
                    

                    projectedCurve.UserDictionary.Set("distanceToEdge", (distanceToEdgeStart + distanceToEdgeEnd)/2);


                    Surface simpleTriangle = NurbsSurface.CreateFromCorners(CameraPt, ptAloneCurve[0], ptAloneCurve[controlPtCount-1]);


                    //ptAloneCurve.Add(CameraPt);

                    //Curve surfaceBoundryCrv = NurbsCurve.Create(true, 1, ptAloneCurve);

                    //Brep[] triangleBrep = Brep.CreatePlanarBreps(surfaceBoundryCrv);
                    //Brep selfTriangleSrf = triangleBrep[0];
                    Brep selfTriangleSrf = simpleTriangle.ToBrep();
                    

                    Curve[] dummyCrvs;
                    Point3d[] dummyPts;
                    Transform scaleTransform = Transform.Scale(CameraPt, 0.9999);
                    selfTriangleSrf.Transform(scaleTransform);
                    bool selfIntersectBool = Rhino.Geometry.Intersect.Intersection.BrepBrep(inputGeo, selfTriangleSrf, 0.01, out dummyCrvs, out dummyPts);

                    

                    bool caseAbool = false;
                    bool partialSelfBool = false;
                    bool caseCBool = false;


                    if (curveBrepIntersect.Count > 0)
                    {

                        partialSelfBool = true;
                    }

                    else
                    {
                        //PARTIAL HIDDEN
                        partialSelfBool = false;
                    }



                    if (curveBrepIntersect.Count == controlPtCount)
                    {
                        //SELF HIDDEN
                        caseAbool = true;
                    }

                    else if (curveBrepIntersect.Count > 0 && curveBrepIntersect.Count < controlPtCount)
                    {
                        //PARTIAL HIDDEN
                        partialSelfBool = true;
                    }

                    else if (curveBrepIntersect.Count == 0)
                    {
                        caseCBool = true;
                    }




                    
                    if (caseAbool == true)//Case A_Total Hidden/////////////////////////////////////////////////////
                    {
                        //any of the two end intersections got intersect
                        //therefore its total hidden
                        projectedCurve.UserDictionary.Set("lineType", "hidden");
                        hiddenSegment.Add(projectedCurve);
                    }

                    else if (caseCBool == true)//Case C_Total Solid
                    {

                        separateSolidVoid(inputGeos, currentIndex, projectedCurve, projectedCurveOrigin, selfTriangleSrf, simpleTriangle, tol,ref solidSegment, ref hiddenSegment, ref breps, ref curveMid, ref outputPoly);
                    }


                    //CASE SELF///////////////////////////
                    else if (partialSelfBool == true)
                    {
                        
                        List<NurbsCurve> intersectionCrvs_self = new List<NurbsCurve>();

                        
                        
                        if (selfIntersectBool)
                        {
                            Curve[] joinedDumCrvs = Curve.JoinCurves(dummyCrvs);
                            //caseBbool = true;


                            foreach (Curve joinedSingleCrv in joinedDumCrvs)//multiple curve per intersection
                            {
                                NurbsCurve nurbDumCrv = joinedSingleCrv.ToNurbsCurve();
                                intersectionCrvs_self.Add(nurbDumCrv);
                            }




                            if (intersectionCrvs_self.Count == 0)
                            {
                                //CASE C_ cuz in this case there is no self intersection
                                /////////////intersectionCrvs_self.Add(projectedSegment);
                                projectedCurve.UserDictionary.Set("lineType", "solid");
                                solidSegment.Add(projectedCurve);//probaly will never happen
                            }

                            else
                            {

                                //CaseB_Self
                                Line edgeA = new Line(ptAloneCurve[0], CameraPt);
                                Line edgeB = new Line(ptAloneCurve[controlPtCount-2], CameraPt);

                                foreach (NurbsCurve intCrv in intersectionCrvs_self)//looping through ALL intersection of this edge triangle
                                {
                                    List<double> distanceToEdgeA = new List<double>();
                                    List<double> distanceToEdgeB = new List<double>();
                                    List<Point3d> controlPtLocation = new List<Point3d>();
                                    List<double> distanceTwoPt = new List<double>();



                                    foreach (ControlPoint dumControlPt in intCrv.Points)//geting multiple control pts from each single 
                                    {
                                        

                                        //double distancA;
                                        //double distancB;
                                        Point3d tempDumLocation = dumControlPt.Location;



                                        Point3d clostPointOnCrvA = edgeA.ClosestPoint(tempDumLocation, true);
                                        Point3d clostPointOnCrvB = edgeB.ClosestPoint(tempDumLocation, true);

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

                                    if (newDistanceEdgeA[0] < 0.09 && newDistanceEdgeB[0] < 0.09)
                                    {
                                        //totally hidden
                                        projectedCurve.UserDictionary.Set("lineType", "hidden");
                                        hiddenSegment.Add(projectedCurve);
                                    }

                                    else
                                    {

                                        separateSolidVoid(inputGeos, currentIndex, projectedCurve, projectedCurveOrigin, selfTriangleSrf, simpleTriangle, tol ,ref solidSegment, ref hiddenSegment, ref breps, ref curveMid, ref outputPoly);
                                    }
                                }
                            }
                        }
                    }



                    else
                    {
                        //Case BC
                        //Surface triangleSrf = NurbsSurface.CreateFromCorners(ptAloneCurve[i], ptAloneCurve[i + 1], CameraPt);//created the triangle surface

                        //now there should be no self intersection
                        //Transform scaleTransform = Transform.Scale(CameraPt, 0.99999);
                        //triangleSrf.Transform(scaleTransform);

                        



                        //dispatchSolidVoid(currentIndex, triangleSrf, inputGeos, projectedSegment, CameraPt, TargetPlane, ref edgeProjectLnA, ref edgeProjectLnB, ref solidSegment, ref hiddenSegment, ref breps);
                        separateSolidVoid(inputGeos, currentIndex, projectedCurve, projectedCurveOrigin, selfTriangleSrf, simpleTriangle, tol,ref solidSegment, ref hiddenSegment, ref breps, ref curveMid, ref outputPoly);

                    }

                }
            }
            solidSegment_All = solidSegment;
            hiddenSegment_All = hiddenSegment;



        }


        private static void separateSolidVoid(List<Brep> inputGeos, int currentIndex, Curve solidLn, Curve solidLnOrigin, Brep TriangleSrf, Surface simpleTriangle, double tol ,ref List<Curve> solidSegment, ref List<Curve> hiddenSegment, ref List<Brep>breps, ref List<Point3d> curveMid, ref List<Curve> outputPoly)
        {
            List<Brep> geosForOutline = new List<Brep>();

            
            

            if (inputGeos.Count > 1)
            {
                for (int tempI = 0; tempI < inputGeos.Count; tempI++)
                {
                    if (tempI != currentIndex)
                    {
                        Point3d[] tempPt;
                        Curve[] tempCrv;

                        Rhino.Geometry.Intersect.Intersection.BrepBrep(TriangleSrf, inputGeos[tempI],0.01, out tempCrv,out tempPt );

                        if (tempPt.Length>0 || tempCrv.Length > 0)//if intersect// determind whether this brep in in the front
                        {
                            geosForOutline.Add(inputGeos[tempI]);
                        }
                        
                    }
                }
            }
            else
            {
                solidLn.UserDictionary.Set("lineType", "solid");
                solidSegment.Add(solidLn);//checking if there is only one input brep
                return;
            }


            bool behindBool = false;
            
            if (geosForOutline.Count > 0)
            {
                behindBool = true;
            }




            //breps.Add(TriangleSrf);////////////////////////////////////
            if (behindBool == false)
            {
                solidLn.UserDictionary.Set("lineType", "solid");
                solidSegment.Add(solidLn);
                return;
            }
            else
            {
                List<Curve> brepOutlines = hs_functions.getBrepOutline(geosForOutline);//////////////////



                Point3d CameraPt = new Point3d();
                Point3d TargetPt = new Point3d();

                Plane TargetPlane = new Plane();
                double FocusLength = 35;

                hs_functions.getViewportInfo(ref CameraPt, ref TargetPt, ref TargetPlane, ref FocusLength);//getting essential

                

                //Vector3d extrusionA = Vector3d.Multiply(0.1, TargetPlane.ZAxis);
                //Vector3d extrusionB = Vector3d.Multiply(-0.1, TargetPlane.ZAxis);

                List<Brep> outlineExtrusionBreps = new List<Brep>();
                //List<Brep> outlineExtrusionSrfs = new List<Brep>();
                foreach (Curve crv in brepOutlines)
                {
                    //Surface extrusionSrfA = Surface.CreateExtrusion(crv, extrusionA);
                    List<Point3d> edgePts = new List<Point3d>();
                    foreach (ControlPoint ctlPt in crv.ToNurbsCurve().Points)
                    {
                        edgePts.Add(ctlPt.Location);
                    }


                    Plane tempPlane;
                    Plane.FitPlaneToPoints(edgePts, out tempPlane);
                    Vector3d vectorZ = tempPlane.ZAxis;
                    double multiplier = 1 / vectorZ.Length;
                    Vector3d extrudeVector = Vector3d.Multiply(multiplier, vectorZ);

                    Curve polylineBoundry = new PolylineCurve(edgePts);
                    bool flatBool = crv.IsPlanar();
                    List<Point3d> projectedPt = new List<Point3d>();


                    Curve baseCrv = crv;


                    if (flatBool == false)
                    {

                        baseCrv = Curve.ProjectToPlane(polylineBoundry, tempPlane);
                        flatBool = baseCrv.IsPlanar();
                    }

                    CurveOrientation orient = baseCrv.ClosedCurveOrientation(Plane.WorldXY);
                    if (orient != CurveOrientation.Clockwise) baseCrv.Reverse();




                    Extrusion extrudedBrep = Extrusion.Create(baseCrv, 1.0, true);
                    Brep cappedBrep = extrudedBrep.ToBrep();
                    Vector3d moveVector = Vector3d.Multiply(0.5, extrudeVector);
                    cappedBrep.Translate(moveVector);


                    outlineExtrusionBreps.Add(cappedBrep);
                   

                }

                Brep[] outlineExtrusionBrepsss;
                if (outlineExtrusionBreps.Count > 1)
                {
                    outlineExtrusionBrepsss = Brep.CreateBooleanUnion(outlineExtrusionBreps, 0.1);
                }

                else
                {
                    outlineExtrusionBrepsss = outlineExtrusionBreps.ToArray();
                    

                }
                foreach(Brep b in outlineExtrusionBrepsss)
                {
                    breps.Add(b);
                }




                List<Point3d> uniquePts = new List<Point3d>();


                Point3d solidLnStartPt = solidLn.PointAtStart;
                Point3d solidLnEndPt = solidLn.PointAtEnd;


                outputPoly.Add(solidLn);///////////////////////////////////////////////

                foreach (Brep b in outlineExtrusionBrepsss)
                {
                    //breps.Add(b);
                    Curve[] tempCrv;
                    Point3d[] tempPt;
                    Rhino.Geometry.Intersect.Intersection.CurveBrep(solidLn, b, 0.01, out tempCrv, out tempPt);



                    foreach (Point3d pt in tempPt)
                    {
                        if (pt.DistanceTo(solidLnStartPt) > 0.01 && pt.DistanceTo(solidLnEndPt) > 0.01)
                        {
                            uniquePts.Add(pt);//not close to 
                        }
                    }

                    
                }



                List<double> uniqueParams = new List<double>();

                object retrievedDist;
                solidLn.UserDictionary.TryGetValue("distanceToEdge", out retrievedDist);



                //projectedCurve.UserDictionary.Set("distanceToEdge", distanceToEdge);

                foreach (Point3d uniPt in uniquePts)
                {
                    double tempParam;
                    solidLn.ClosestPoint(uniPt, out tempParam);
                    uniqueParams.Add(tempParam);
                }


                if (uniqueParams.Count > 0)
                {

                    Curve[] splittedSolidLn = solidLn.Split(uniqueParams);


                    foreach (Curve splitLn in splittedSolidLn)
                    {

                        splitLn.UserDictionary.Set("distanceToEdge", retrievedDist.ToString());

                        bool containBool = false;
                        Point3d splitMidPoint = splitLn.PointAt(splitLn.Domain.Mid);
                        curveMid.Add(splitMidPoint);
                        foreach (Brep inclusionBrep in outlineExtrusionBrepsss)
                        {
                            if (inclusionBrep.IsPointInside(splitMidPoint, tol, true))
                            {
                                containBool = true;
                            }
                        }

                        if (containBool == true)
                        {
                            splitLn.UserDictionary.Set("lineType", "hidden");
                            hiddenSegment.Add(splitLn);
                        }
                        else
                        {
                            splitLn.UserDictionary.Set("lineType", "solid");
                            solidSegment.Add(splitLn);
                        }
                    }
                }
                
                else if (uniqueParams.Count == 0)
                {
                    Point3d midPoint = solidLn.PointAt(solidLn.Domain.Mid);
                    bool containBool = false;

                    foreach (Brep inclusionBrep in outlineExtrusionBrepsss)
                    {
                        
                        if (inclusionBrep.IsPointInside(midPoint, tol, true))
                        {
                            containBool = true;
                        }
                    }

                    if (containBool == true)
                    {
                        solidLn.UserDictionary.Set("lineType", "hidden");
                        hiddenSegment.Add(solidLn);
                    }
                    else
                    {
                        solidLn.UserDictionary.Set("lineType", "solid");
                        solidSegment.Add(solidLn);
                    }



                }

            }


            
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
            List<Curve> outputPoly = new List<Curve>();
            
            
            List<string> outputMessage = new List<string>();

            DA.GetDataList(0, inputBreps);
            foreach(Brep b in inputBreps)
            {
                if (b == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Must have at least one input geometry");
                    return;
                }
                
            }

            List<Brep> booleanedInputGeos = new List<Brep>();
            //Brep[] booleanedBreps = Brep.CreateBooleanUnion(inputBreps, 0.01);
            foreach (Brep b in inputBreps)
            {
                if (b != null)
                {
                    booleanedInputGeos.Add(b);
                }
            }

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

            List<Point3d> curveMid = new List<Point3d>();
            double tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            makeTwoD(booleanedInputGeos, cameraPt, intersectionPlane, hiddenBool, tolerance, ref outputMessage, ref dashLines, ref solidLines, ref brepss, ref curveMid, ref outputPoly);

            DA.SetDataList(0, solidLines);
            DA.SetDataList(1, dashLines);
            //DA.SetDataList(3, unflattenedCrv);
            DA.SetDataList(2, outputMessage);
            DA.SetDataList(3, brepss);
            DA.SetData(4, cameraPt);
            DA.SetDataList(5, curveMid);
            DA.SetDataList(6, outputPoly);
            


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