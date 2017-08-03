using System;
using System.Collections.Generic;


using Grasshopper.Kernel;
using Rhino.Display;

using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;


namespace MyProject_0624
{
    public class ZoomIterator_Obj : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Zoom_Object class.
        /// </summary>
        public ZoomIterator_Obj()
          : base("Zoom Iterator", "Zoom Iterator",
              "Iterates through each element in the input list while zooming to them, coded as a useful tool for spot checking",
              "HS_Toolbox", "Graphics")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Input Geometry for Spot Checking", "Input", "Input Geometry for Spot Checking", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale Factor for Zooming", "Scale", "Scale Factor for Zooming", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Set True to start", "Start?", "Switch Button_Set true to Start", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Sleep Interval", "MiliSec", "Interval", GH_ParamAccess.item);
            pManager[1].Optional = true;
            //pManager[2].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddTextParameter("Object Type", "Type", "Object Type", GH_ParamAccess.item);
            //pManager.AddTextParameter("Message", "Message", "Message", GH_ParamAccess.item);
            //pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            List<GeometryBase> inputGeo = new List<GeometryBase>();
            //List<Point3d> centerPts = new List<Point3d>();
            bool switchBool = new bool();
            int miliSec = new int();
            double scaleFactor = new double();
            
         
            DA.GetDataList(0, inputGeo);

            if (inputGeo.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input Geometry Type not Supported");
            }


            DA.GetData(1, ref scaleFactor);
            DA.GetData(2, ref switchBool);
            DA.GetData(3, ref miliSec);


       
            if (switchBool == true)
            {
                foreach (GeometryBase geo in inputGeo)
                {
                    

                    BoundingBox geometryBBox = geo.GetBoundingBox(false);//get a boundingbox for zooming
                    Point3d BBoxCenterPt = geometryBBox.Center;
                    
                    //scalling the boundingbox
                    Transform scaleTransform = Rhino.Geometry.Transform.Scale(BBoxCenterPt, scaleFactor);
                    geometryBBox.Transform(scaleTransform);

                    //zooming in the viewport
                    RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ZoomBoundingBox(geometryBBox);
                    System.Threading.Thread.Sleep(miliSec);
                    RhinoDoc.ActiveDoc.Views.Redraw();
                    
                }
            }

            


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ZoomIterator_Obj;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e8f6984f-5795-464b-8edc-0aad328bacca"); }
        }
    }
}