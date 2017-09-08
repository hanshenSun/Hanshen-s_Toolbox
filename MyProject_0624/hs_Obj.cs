﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;


namespace MyProject_0624
{
    public class hs_Point
    {
        public Point3d basePt;
        public List<string> Name;
        public List<string> Value;

        public hs_Point(Point3d pt, List<string> dictName, List<string> dictValue)
        {
            basePt = pt;
            Name = dictName;
            Value = dictValue;
        }
        
        public hs_Point()
        {

        }

    }

    public class hs_Curve
    {
        public Curve baseCrv;
        public List<string> Name;
        public List<string> Value;

        public hs_Curve(Curve crv, List<string> dictName, List<string> dictValue)
        {
            baseCrv = crv;
            Name = dictName;
            Value = dictValue;

        }
        public hs_Curve()
        {

        }



    }

    public class hs_functions
    {
        
        public static List<Curve> projectEdge (List<Curve> inputCrvs, Plane targetPlane, Point3d cameraPt)
        {
            List<Curve> projectedEdges = new List<Curve>();

            foreach (Curve crv in inputCrvs)
            {
                NurbsCurve nurbsEdge = crv.ToNurbsCurve();
                List<Point3d> controlPoints = new List<Point3d>();

                foreach (ControlPoint ctlPt in nurbsEdge.Points)
                {
                    Line startCamLn = new Line(cameraPt, ctlPt.Location);
                    double projectParam;
                    Rhino.Geometry.Intersect.Intersection.LinePlane(startCamLn, targetPlane, out projectParam);

                    Point3d projectPt = startCamLn.PointAt(projectParam);
                    controlPoints.Add(projectPt);
                }

                Curve newNurbCrv = NurbsCurve.Create(false, 3, controlPoints);
                projectedEdges.Add(newNurbCrv);
            }

            return projectedEdges;
        }

        public static void getViewportInfo(ref Point3d cameraPt, ref Point3d targetPt, ref Plane targetPlane, ref double focusLength)
        {

            RhinoViewport vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            cameraPt = vp.CameraLocation;
            targetPt = vp.CameraTarget;
            focusLength = vp.Camera35mmLensLength;


            Point3d[] cornerPts = vp.GetFarRect();
            targetPlane = new Plane(cornerPts[0], cornerPts[1], cornerPts[2]);
        }

        public static List<Curve> getBrepEdgeProjection(Brep inputGeo)
        {
            Point3d CameraPt = new Point3d();
            Point3d TargetPt = new Point3d();

            Plane TargetPlane = new Plane();
            double FocusLength = 35;

            getViewportInfo(ref CameraPt, ref TargetPt, ref TargetPlane, ref FocusLength);//getting essential

            List<Curve> edgestoProject = new List<Curve>();
            
            foreach (BrepEdge edge in inputGeo.Edges)
            {
                edgestoProject.Add(edge.ToNurbsCurve());
            }

            List<Curve> projectedEdges = projectEdge(edgestoProject, TargetPlane, CameraPt);

            return projectedEdges;

        }

        public static List<Curve> getBrepOutline(List<Brep> inputBreps)
        {
            
            Point3d CameraPt = new Point3d();
            Point3d TargetPt = new Point3d();

            Plane TargetPlane = new Plane();
            double FocusLength = 35;

            getViewportInfo(ref CameraPt, ref TargetPt, ref TargetPlane, ref FocusLength);//getting essential
            

            List<Curve> brepOutlines = new List<Curve>();

            RhinoViewport vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            foreach (Brep b in inputBreps)
            {
                Mesh[] inputMesh = Mesh.CreateFromBrep(b, MeshingParameters.Coarse);


                Mesh joinedMesh = new Mesh();
                foreach (Mesh face in inputMesh)
                {
                    joinedMesh.Append(face);//gathering meshes
                }
                

                List<Curve> meshOutCrv = new List<Curve>();



                foreach(Polyline poly in joinedMesh.GetOutlines(vp))
                {
                    meshOutCrv.Add(poly.ToNurbsCurve());
                }


                if (meshOutCrv.Count > 1)
                {
                    List<Curve> tempCrv = new List<Curve>();
                    foreach (Curve poly in meshOutCrv)
                    {
                        tempCrv.Add(poly.ToNurbsCurve());
                    }

                    Curve[] joinedMeshOutline = Curve.CreateBooleanUnion(tempCrv);
                    meshOutCrv = joinedMeshOutline.ToList();
                }
                



                foreach (Curve poly in meshOutCrv)
                {
                    NurbsCurve tempNurb = poly.ToNurbsCurve();
                    //brepOutlines.Add(tempNurb);
                    
                    List<Point3d> polyLnPt = new List<Point3d>();
                    foreach (ControlPoint ctlPt in tempNurb.Points)
                    {
                        Line controlPtLn = new Line(CameraPt, ctlPt.Location);
                        double tempDouble;
                        Rhino.Geometry.Intersect.Intersection.LinePlane(controlPtLn, TargetPlane, out tempDouble);

                        Point3d projectedPt = controlPtLn.PointAt(tempDouble);
                        polyLnPt.Add(projectedPt);
                    }
                    Curve projectedOutline = NurbsCurve.Create(true, 1, polyLnPt);
                    brepOutlines.Add(projectedOutline);
                }
            }
            


            List<Curve> outlines = new List<Curve>();

            if (brepOutlines.Count == 1)//only contains one curve
            {
                outlines.Add(brepOutlines[0]);
                return outlines;
            }

            else
            {
                Curve[] booledOutline = Curve.CreateBooleanUnion(brepOutlines);
                if (booledOutline.Length > 1)
                {
                    GH_RuntimeMessage message = new GH_RuntimeMessage("more than one outline", GH_RuntimeMessageLevel.Warning, "more than one outline");

                }
                foreach (Curve crv in booledOutline)
                {

                    outlines.Add(crv);
                }
                return outlines;
            }
                



            


        }

        public static bool pointInBreps(Point3d inputPt, Brep closedBrep, double tolerance)
        {
            int brepFaceCount = closedBrep.Faces.Count;
            bool inBrep = false;


            List<double> ptAngle = new List<double>();
            List<Point3d> ptsOnSrf = new List<Point3d>();
            List<Surface> surfaces = new List<Surface>();
            for (int i = 0; i < brepFaceCount; i++)
            {
                BrepFace bf = closedBrep.Faces[i];
                Surface bSrf = bf.ToNurbsSurface();
                surfaces.Add(bSrf);

                //double tempU;
                //double tempV;
                //bf.ClosestPoint(inputPt, out tempU, out tempV);
                //Point3d ptOnSrf = bf.PointAt(tempU, tempV);
                
                
                Plane tempPlane;
                bf.FrameAt(bSrf.Domain(0).Mid, bSrf.Domain(1).Mid, out tempPlane);

                Point3d ptOnSrf = tempPlane.ClosestPoint(inputPt);

                Vector3d tempVector = Point3d.Subtract(ptOnSrf, inputPt);

                Vector3d brepVector = tempPlane.ZAxis;

                double vectorAngle = Vector3d.VectorAngle(tempVector, brepVector)* 57.2958;

                if (vectorAngle > -10 && vectorAngle < 10)
                {
                    ptAngle.Add(vectorAngle);
                }
            }
            
            if (ptAngle.Count == brepFaceCount)
            {
                inBrep = true;
            }

            return inBrep;


        }

    }
}
