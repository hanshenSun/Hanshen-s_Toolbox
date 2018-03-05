using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class AlisonTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AlisonTest class.
        /// </summary>
        public AlisonTest()
          : base("alison", "alison",
              "Add Attribute (Userdictionary) to points",
              "HS_ToolBox", "BIM Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Srf", "Srf", "Srf", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Cir", "Cir", "Cir", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            NurbsSurface inputSrf= new NurbsSurface();

            DA.GetData(0, ref inputSrf);


            List<Point3d> pts = new List<Point3d>();
            List<Plane> plns = new List<Plane>();
            List<Line> lines = new List<Line>();
            List<Circle> cls = new List<Circle>();
            List<Curve> outputCrv = new List<Curve>();

            Point3d tempPts = new Point3d();


            Random random = new Random();
            Vector3d unitZ = new Vector3d(0, 0, 1);

            for (int i = 0; i <= 10; i = i + 2)
            {

                Vector3d[]tempVectors = new Vector3d[1];

                int randint = random.Next(0, 1);
                int randint2 = random.Next(0, 1);
                int randint3 = random.Next(0, 5);

                //Point3d pt = new Point3d(i, randint, randint2);
                //Plane pln = new Plane(pt, unitZ);
                inputSrf.Evaluate(randint, randint2, 2, out tempPts, out tempVectors);


                Circle cl = new Circle(tempPts, randint3);
                NurbsCurve newNurbs = cl.ToNurbsCurve();
                outputCrv.Add(newNurbs);

                

            }
            DA.SetData(0, outputCrv);


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
            get { return new Guid("3661a349-d4d5-4aa3-9a79-668d414c184f"); }
        }
    }
}