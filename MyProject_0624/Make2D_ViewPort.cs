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
            pManager.AddNumberParameter("tolerance", "tolerance", "tolerance", GH_ParamAccess.item);
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


        }





        private static void makeTwoD(List<Brep> inputGeos, Point3d CameraPt, Plane TargetPlane ,bool hiddenlnBool, double tol, ref List<string> message, ref List<Curve> hiddenSegment_All, ref List<Curve> solidSegment_All, ref List<Brep> breps)
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
                    List<bool> curveBrepIntersect = new List<bool>();



                    foreach (ControlPoint edgeControlPt in singleEdge.Points)
                    {
                        Point3d tempControlPtLocation = edgeControlPt.Location;




                        Vector3d projectVector = Point3d.Subtract(tempControlPtLocation, CameraPt);
                        double distance = CameraPt.DistanceTo(tempControlPtLocation);

                        Line tempProjectLn = new Line(CameraPt, projectVector, distance * 0.95);
                        Curve tempProjectCrv = new LineCurve(tempProjectLn);

                        Line edgeProjectLn = new Line(tempControlPtLocation, CameraPt);
                        //getting the projection line of the two ends of the line segments


                        double intersectionParameter;
                        Rhino.Geometry.Intersect.Intersection.LinePlane(edgeProjectLn, TargetPlane, out intersectionParameter);


                        Point3d intersectPt = edgeProjectLn.PointAt(intersectionParameter);
                        ptAloneCurve.Add(intersectPt);//getting point location of each edgecurve//////////////////



                        Curve[] dummyCrvsA;
                        Point3d[] dummyPtsA;
                        Rhino.Geometry.Intersect.Intersection.CurveBrep(tempProjectCrv, inputGeo, tol, out dummyCrvsA, out dummyPtsA);

                        if (dummyPts.Length > 0)
                        {
                            curveBrepIntersect.Add(true);
                        }


                    }


                    Curve projectedCurve = NurbsCurve.Create(false, ptAloneCurve.Count, ptAloneCurve);

                    ptAloneCurve.Add(CameraPt);

                    Curve surfaceBoundryCrv = NurbsCurve.Create(true, 1, ptAloneCurve);
                    Brep[] triangleBrep = Brep.CreatePlanarBreps(surfaceBoundryCrv);
                    Surface selfTriangleSrf = triangleBrep[0].Surfaces[0];




                    //LineCurve projectedCurve = new LineCurve(intersectPtA, intersectPtB);//got projected line segments of each edge




                    Curve[] dummyCrvs;
                    Point3d[] dummyPts;
                    Transform scaleTransform = Transform.Scale(CameraPt, 0.9999);
                    selfTriangleSrf.Transform(scaleTransform);
                    bool selfIntersectBool = Rhino.Geometry.Intersect.Intersection.BrepSurface(inputGeo, selfTriangleSrf, tol, out dummyCrvs, out dummyPts);




                    bool caseAbool = false;
                        bool partialSelfBool = false;
                        bool caseCBool = false;
                        if (dummyCrvs.Length > 0)
                        {
                            
                            partialSelfBool = true;
                        }
                        
                        else
                        {
                            //PARTIAL HIDDEN
                            partialSelfBool = false;
                        }


                        
                        if (dummyPtsA.Length > 0 && dummyPtsB.Length > 0)
                        {
                            //SELF HIDDEN
                            caseAbool = true;
                        }

                        else if (dummyPtsA.Length == 0 && dummyPtsB.Length > 0)
                        {
                            //PARTIAL HIDDEN
                            partialSelfBool = true;
                        }
                        else if (dummyPtsA.Length > 0 && dummyPtsB.Length == 0)
                        {
                            //PARTIAL HIDDEN
                            partialSelfBool = true;
                        }
                        else if (dummyCrvs.Length == 0 && dummyPtsA.Length == 0 && dummyPtsB.Length == 0)
                        {
                            caseCBool = true;
                        }




                        //Case A_Total Hidden/////////////////////////////////////////////////////
                        if (caseAbool == true)
                        {
                            //any of the two end intersections got intersect
                            //therefore its total hidden
                            hiddenSegment.Add(projectedCurve);
                        }
                        
                        else if (caseCBool == true)
                        {
                            separateSolidVoid(inputGeos, currentIndex, projectedCurve, selfTriangleSrf, ref solidSegment, ref hiddenSegment, ref breps);
                        }


                        //CASE SELF///////////////////////////
                        else if (partialSelfBool == true)
                        {
                            //self intersecting partial hidden
                            //Surface triangleSrf = NurbsSurface.CreateFromCorners(ptAloneCurve[i], ptAloneCurve[i + 1], CameraPt);//created the triangle surface
                            //Transform scaleTransform = Transform.Scale(CameraPt, 0.99999);
                            //triangleSrf.Transform(scaleTransform);
                            

                            edgeProjectLnA = new Line(ptAloneCurve[0], CameraPt);
                            edgeProjectLnB = new Line(ptAloneCurve[1], CameraPt);


                            //now there should be no self intersection


                            List<NurbsCurve> intersectionCrvs_self = new List<NurbsCurve>();

                            //Curve[] dumIntCrv;
                            //Point3d[] dumIntPt;


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
                                    solidSegment.Add(projectedCurve);//probaly will never happen

                                }

                                else
                                {

                                    //CaseB_Self
                                    
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

                                            Point3d clostPointOnCrvA = edgeProjectLnA.ClosestPoint(tempDumLocation, true);
                                            Point3d clostPointOnCrvB = edgeProjectLnB.ClosestPoint(tempDumLocation, true);

                                            //edgeProjectLnA.PointAt(distancA);
                                            //edgeProjectLnB.PointAt(distancB);

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
                                            hiddenSegment.Add(projectedCurve);
                                        }

                                        else
                                        {
                                            //loop through alllllll


                                            //dispatchSolidVoid(currentIndex, triangleSrf, inputGeos, projectedSegment, CameraPt, TargetPlane, ref edgeProjectLnA, ref edgeProjectLnB, ref solidSegment, ref hiddenSegment, ref breps);
                                            separateSolidVoid(inputGeos, currentIndex, projectedCurve, selfTriangleSrf, ref solidSegment, ref hiddenSegment, ref breps);
                                        }
                                    }
                                }
                            }
                        }



                        else
                        {
                            //Case BC
                            Surface triangleSrf = NurbsSurface.CreateFromCorners(ptAloneCurve[i], ptAloneCurve[i + 1], CameraPt);//created the triangle surface
                            
                            //now there should be no self intersection
                            //Transform scaleTransform = Transform.Scale(CameraPt, 0.99999);
                            triangleSrf.Transform(scaleTransform);
                            
                            
                            edgeProjectLnA = new Line(ptAloneCurve[0], CameraPt);
                            edgeProjectLnB = new Line(ptAloneCurve[1], CameraPt);



                            //dispatchSolidVoid(currentIndex, triangleSrf, inputGeos, projectedSegment, CameraPt, TargetPlane, ref edgeProjectLnA, ref edgeProjectLnB, ref solidSegment, ref hiddenSegment, ref breps);
                            separateSolidVoid(inputGeos, currentIndex, projectedCurve, selfTriangleSrf, ref solidSegment, ref hiddenSegment, ref breps);

                        }

                    }
                }
            }
            solidSegment_All = solidSegment;
            hiddenSegment_All = hiddenSegment;
        }

        /*
        private static void dispatchSolidVoid(int brepIndex, Surface triangleSrf, List<Brep> inputGeos, Curve solidLns, Point3d cameraPt,Plane TargetPlane, ref Line edgeProjectLnA, ref Line edgeProjectLnB,  ref List<Curve> solidSegment_ref, ref List<Curve> hiddenSegment_ref, ref List<Brep> breps)
        {

            List<NurbsCurve> intersectionCrvs = new List<NurbsCurve>();


            for (int ii = 0; ii < brepIndex; ii++)
            //foreach (Brep tempInputGeo in inputGeos)
            {
                if (ii != brepIndex)
                {
                    Brep tempInputGeo = inputGeos[ii];
                    Curve[] dumIntCrv;
                    Point3d[] dumIntPt;


                    if (Rhino.Geometry.Intersect.Intersection.BrepSurface(tempInputGeo, triangleSrf, 0.01, out dumIntCrv, out dumIntPt))//has intersection
                    {
                        //CASE BC

                        Curve[] joinedDumCrvs = Curve.JoinCurves(dumIntCrv);
                        //caseBbool = true;


                        foreach (Curve joinedSingleCrv in joinedDumCrvs)//multiple curve per intersection
                        {
                            NurbsCurve nurbDumCrv = joinedSingleCrv.ToNurbsCurve();
                            intersectionCrvs.Add(nurbDumCrv);
                        }
                    }

                }

            }



            if (intersectionCrvs.Count == 0)
            {
                //CASE C_ cuz in this case there is no self intersection
                solidSegment_ref.Add(solidLns);
            }

            else
            {

                //CaseB
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


                    Line blockLnA = new Line(cameraPt, closestPtA);
                    Line blockLnB = new Line(cameraPt, closestPtB);//these two curves compose the TRIANGLE

                    double blockExtParamA;
                    double blockExtParamB;

                    Rhino.Geometry.Intersect.Intersection.LinePlane(blockLnA, TargetPlane, out blockExtParamA);
                    Rhino.Geometry.Intersect.Intersection.LinePlane(blockLnB, TargetPlane, out blockExtParamB);

                    Point3d blockPtA = blockLnA.PointAt(blockExtParamA);
                    Point3d blockPtB = blockLnB.PointAt(blockExtParamB);//TRIANGLE endpoints



                    Point3d[] blockBasePt = new Point3d[3] { blockPtA, blockPtB, cameraPt };
                    Plane tempPlane;
                    Plane.FitPlaneToPoints(blockBasePt, out tempPlane);
                    Vector3d extrudeZA = Vector3d.Multiply(0.1, tempPlane.ZAxis);
                    Vector3d extrudeZB = Vector3d.Multiply(-0.1, tempPlane.ZAxis);



                    NurbsCurve blockBaseCrv = NurbsCurve.Create(true, 1, blockBasePt);
                    Surface blockSrfOnPlaneA = Surface.CreateExtrusion(blockBaseCrv, extrudeZA);
                    
                    Surface blockSrfOnPlaneB = Surface.CreateExtrusion(blockBaseCrv, extrudeZB);


                    Brep blockSrfBrepA = Brep.CreateFromSurface(blockSrfOnPlaneA);
                    Brep blockBrepSolidA = blockSrfBrepA.CapPlanarHoles(0.1);

                    Brep blockSrfBrepB = Brep.CreateFromSurface(blockSrfOnPlaneB);
                    Brep blockBrepSolidB = blockSrfBrepB.CapPlanarHoles(0.1);

                    
                    //Extrusion blockExtrusion = Extrusion.Create(blockBaseCrv, 10.0, true);
                    //Brep blockBrep = blockExtrusion.ToBrep();//got SINGLE triangle srf extrusion

                    
                    
                    Brep[] tempB = new Brep[2] { blockBrepSolidA, blockBrepSolidB };
                    Brep[] tempJoinedB = Brep.CreateBooleanUnion(tempB, 0.5);

                    extrudedTri.Add(tempJoinedB[0]);
                }



                Brep[] joinedB = Brep.CreateBooleanUnion(extrudedTri, 0.1);
                List<Brep> joinedBrep = new List<Brep>();
                //bool nullresult = false;

                if (joinedB == null)
                {
                    joinedBrep = extrudedTri;
                }
                
                else
                {
                    foreach (Brep b in joinedB)
                    {
                        joinedBrep.Add(b);
                        breps.Add(b);
                    }
                }


                List<Point3d> brepLnIntersectPt = new List<Point3d>();





                foreach (Brep singleBrep in joinedBrep)
                {
                    Curve[] overlapBrepCrvs;
                    Point3d[] overlapBrepPts;



                    Rhino.Geometry.Intersect.Intersection.CurveBrep(solidLns, singleBrep, 0.01, out overlapBrepCrvs, out overlapBrepPts);

                    int ptCount = overlapBrepPts.Length;
                    int crvCount = overlapBrepCrvs.Length;

                    foreach (Curve crv in overlapBrepCrvs)
                    {
                        if (crv.GetLength() > solidLns.GetLength()*0.99)////////////////////////////////
                        {
                            //Case A_Total Hidden
                            solidSegment_ref.Add(solidLns);
                        }
                        else if (crv.GetLength() < solidLns.GetLength())
                        {
                            //Case B
                            //Get endPt Parameter
                            Point3d startPt = crv.PointAtStart;
                            Point3d endPt = crv.PointAtEnd;
                            Point3d[] ptsOnSegment = new Point3d[] { crv.PointAtStart, crv.PointAtEnd, solidLns.PointAtStart, solidLns.PointAtEnd };
                            Point3d[] endPtsOnSegment = new Point3d[] { solidLns.PointAtStart, solidLns.PointAtEnd };

                            Point3d[] culledPts = Point3d.CullDuplicates(ptsOnSegment, 0.1);
                            if (culledPts.Length == 2)
                            {
                                //Case C Total Hidden
                                hiddenSegment_ref.Add(solidLns);
                            }
                            else
                            {
                                //CASE B Partial Hidden
                                List<double> splitParam = new List<double>();
                                List<Point3d> uniqueSplitPt = new List<Point3d>();

                                foreach (Point3d cleanPt in culledPts)
                                {
                                    if (cleanPt.DistanceTo(solidLns.PointAtStart) > 0.05 && cleanPt.DistanceTo(solidLns.PointAtEnd) > 0.05)
                                    {
                                        uniqueSplitPt.Add(cleanPt);
                                    }
                                }

                                Point3d[] newUniqueSplitPt = Point3d.CullDuplicates(uniqueSplitPt, 0.1);
                                foreach (Point3d pt in newUniqueSplitPt)
                                {
                                    double uniqueParam;
                                    solidLns.ClosestPoint(pt, out uniqueParam);
                                    splitParam.Add(uniqueParam);
                                }


                                Curve[] splitedCrvs = solidLns.Split(splitParam);

                                //got splited curves
                                //ready to sort the inside or outside segments



                                foreach (Curve splitedSingle in splitedCrvs)
                                {
                                    double splitMidParam = splitedSingle.Domain.Mid;//SINGLE midpt on each segment
                                    Point3d splitMidPt = splitedSingle.PointAt(splitMidParam);

                                    List<bool> containResult = new List<bool>();


                                    foreach (Brep singBrep in joinedBrep)
                                    {
                                        if (singBrep.IsPointInside(splitMidPt, 0.01, true))
                                        {
                                            containResult.Add(true);
                                        }
                                    }

                                    if (containResult.Contains(true))
                                    {
                                        //this segment is included in some breps
                                        solidSegment_ref.Add(splitedSingle);

                                    }
                                    else
                                    {
                                        
                                        hiddenSegment_ref.Add(splitedSingle);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        */
        private static void separateSolidVoid(List<Brep> inputGeos, int currentIndex, Curve solidLn, Surface TriangleSrf, ref List<Curve> solidSegment, ref List<Curve> hiddenSegment, ref List<Brep>breps)
        {
            List<Brep> geosForOutline = new List<Brep>();

            
            

            if (inputGeos.Count > 1)
            {
                for (int tempI = 0; tempI < inputGeos.Count; tempI++)
                {
                    if (tempI != currentIndex)
                    {
                        geosForOutline.Add(inputGeos[tempI]);
                    }
                }
            }
            else
            {
                solidSegment.Add(solidLn);//checking if there is only one input brep
                return;
            }


            //if multiple brep, which if this edge is shaded  by any other
            bool behindBool = false;
            for (int ii = 0; ii < inputGeos.Count; ii++)
            {
                if (ii != currentIndex)
                {
                    Brep tempBrep = inputGeos[ii];

                    Point3d[] triangleIntersectPts;
                    Curve[] triangleIntersectCrvs;

                    Rhino.Geometry.Intersect.Intersection.BrepSurface(tempBrep, TriangleSrf, 0.01, out triangleIntersectCrvs, out triangleIntersectPts);

                    if (triangleIntersectPts.Length > 0 || triangleIntersectCrvs.Length > 0)
                    {
                        behindBool = true;
                    }
                }
                
            }

            if (behindBool == false)
            {
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




                if (brepOutlines.Count != 1)
                {
                    GH_RuntimeMessage message = new GH_RuntimeMessage("more than one outline", GH_RuntimeMessageLevel.Warning);

                }

                Vector3d extrusionA = Vector3d.Multiply(0.1, TargetPlane.ZAxis);
                Vector3d extrusionB = Vector3d.Multiply(-0.1, TargetPlane.ZAxis);

                List<Brep> outlineExtrusionBreps = new List<Brep>();
                List<Brep> outlineExtrusionSrfs = new List<Brep>();
                foreach (Curve crv in brepOutlines)
                {
                    Surface extrusionSrfA = Surface.CreateExtrusion(crv, extrusionA);
                    Surface extrusionSrfB = Surface.CreateExtrusion(crv, extrusionB);

                    Brep[] planarsrf = Brep.CreatePlanarBreps(crv);
                    outlineExtrusionSrfs.Add(planarsrf[0]);

                    Extrusion extrudeA = Rhino.Geometry.Extrusion.Create(crv, 1.0, true);
                    Extrusion extrudeB = Rhino.Geometry.Extrusion.Create(crv, -1.0, true);

                    Brep cappedBrepA = extrudeA.ToBrep();
                    Brep cappedBrepB = extrudeB.ToBrep();

                    //Brep extrusionBrepA = Brep.CreateFromSurface(extrusionSrfA);
                    //Brep extrusionBrepB = Brep.CreateFromSurface(extrusionSrfB);

                    //Brep cappedBrepA = extrusionBrepA.CapPlanarHoles(0.5);
                    //Brep cappedBrepB = extrusionBrepB.CapPlanarHoles(0.5;

                    Brep[] cappedBreps = new Brep[] { cappedBrepA, cappedBrepB };
                    Brep[] unionedSides = Brep.CreateBooleanUnion(cappedBreps, 0.1);
                    foreach (Brep b in unionedSides)
                    {
                        outlineExtrusionBreps.Add(b);
                    }

                }
                



                Brep[] outlineExtrusionBrepsss = Brep.CreateBooleanUnion(outlineExtrusionBreps, 0.1);




                List<Point3d> uniquePts = new List<Point3d>();


                Point3d solidLnStartPt = solidLn.PointAtStart;
                Point3d solidLnEndPt = solidLn.PointAtEnd;

                foreach (Brep b in outlineExtrusionBrepsss)
                {
                    breps.Add(b);
                    Curve[] tempCrv;
                    Point3d[] tempPt;
                    Rhino.Geometry.Intersect.Intersection.CurveBrep(solidLn, b, 0.1, out tempCrv, out tempPt);



                    foreach (Point3d pt in tempPt)
                    {
                        if (pt.DistanceTo(solidLnStartPt) > 0.01 && pt.DistanceTo(solidLnEndPt) > 0.01)
                        {
                            uniquePts.Add(pt);//not close to 
                        }
                    }
                    uniquePts.Add(solidLn.PointAt(solidLn.Domain.Mid));
                }



                List<double> uniqueParams = new List<double>();


                if (uniquePts.Count == 0)
                {
                    solidSegment.Add(solidLn);
                }
                else if (uniquePts.Count > 0)
                {
                    foreach (Point3d uniPt in uniquePts)
                    {
                        double tempParam;
                        solidLn.ClosestPoint(uniPt, out tempParam);
                        uniqueParams.Add(tempParam);
                    }
                }

                Curve[] splittedSolidLn = solidLn.Split(uniqueParams);
                foreach (Curve splitLn in splittedSolidLn)
                {
                    bool containBool = false;

                    Point3d midPoint = splitLn.PointAt(splitLn.Domain.Mid);
                    foreach (Brep inclusionBrep in outlineExtrusionBrepsss)
                    {
                        if (inclusionBrep.IsPointInside(midPoint, 0.01, true))
                        {
                            containBool = true;
                        }
                    }

                    if (containBool == true)
                    {
                        hiddenSegment.Add(splitLn);
                    }
                    else
                    {
                        solidSegment.Add(splitLn);
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
            double tolerance = new double();
            
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
            Brep[] booleanedBreps = Brep.CreateBooleanUnion(inputBreps, 0.01);
            foreach (Brep b in booleanedBreps)
            {
                if (b != null)
                {
                    booleanedInputGeos.Add(b);
                }
            }

            DA.GetData(1, ref startBool);
            DA.GetData(2, ref hiddenBool);
            DA.GetData(3, ref tolerance);

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

            makeTwoD(booleanedInputGeos, cameraPt, intersectionPlane, hiddenBool, tolerance, ref outputMessage, ref dashLines, ref solidLines, ref brepss);

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